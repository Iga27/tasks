using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace LinqToXml
{
    public static class LinqToXml
    {
        /// <summary>
        /// Creates hierarchical data grouped by category
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation (refer to CreateHierarchySourceFile.xml in Resources)</param>
        /// <returns>Xml representation (refer to CreateHierarchyResultFile.xml in Resources)</returns>
        public static string CreateHierarchy(string xmlRepresentation)
        {
            var root = XElement.Parse(xmlRepresentation);

            return new XElement("Root", from data in root.Elements("Data")
                                               group data by (string)data.Element("Category")
                                                   into groups
                                                   select new XElement("Group", new XAttribute("ID", groups.Key),
                                                      from g in groups
                                                      select new XElement("Data", g.Element("Quantity"),
                                                          g.Element("Price")))).ToString();
        }

        /// <summary>
        /// Get list of orders numbers (where shipping state is NY) from xml representation
        /// </summary>
        /// <param name="xmlRepresentation">Orders xml representation (refer to PurchaseOrdersSourceFile.xml in Resources)</param>
        /// <returns>Concatenated orders numbers</returns>
        /// <example>
        /// 99301,99189,99110
        /// </example>
        public static string GetPurchaseOrders(string xmlRepresentation)
        {
            XNamespace aw = "http://www.adventure-works.com";
            var root = XElement.Parse(xmlRepresentation);

            var numbers = root.Elements(aw + "PurchaseOrder")
                .Where(x => (string)x.Element(aw + "Address").Attribute(aw + "Type") == "Shipping" &&
                                          (string)x.Element(aw + "Address").Element(aw + "State") == "NY")
                     .Select(x => (string)x.Attribute(aw + "PurchaseOrderNumber"));

            return String.Join(",", numbers);
        }

        /// <summary>
        /// Reads csv representation and creates appropriate xml representation
        /// </summary>
        /// <param name="customers">Csv customers representation (refer to XmlFromCsvSourceFile.csv in Resources)</param>
        /// <returns>Xml customers representation (refer to XmlFromCsvResultFile.xml in Resources)</returns>
        public static string ReadCustomersFromCsv(string customers)
        {
            var lines = customers.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var root = new XElement("Root");

            foreach(var line in lines)
            {
                var fields = line.Split(',');
                root.Add(new XElement("Customer",  
        new XAttribute("CustomerID", fields[0]),  
        new XElement("CompanyName", fields[1]),  
        new XElement("ContactName", fields[2]),  
        new XElement("ContactTitle", fields[3]),  
        new XElement("Phone", fields[4]),  
        new XElement("FullAddress",  
            new XElement("Address", fields[5]),  
            new XElement("City", fields[6]),  
            new XElement("Region", fields[7]),  
            new XElement("PostalCode", fields[8]),  
            new XElement("Country", fields[9]) )));
            }
            return root.ToString();
        }

        /// <summary>
        /// Gets recursive concatenation of elements
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation of document with Sentence, Word and Punctuation elements. (refer to ConcatenationStringSource.xml in Resources)</param>
        /// <returns>Concatenation of all this element values.</returns>
        public static string GetConcatenationString(string xmlRepresentation)
        {
            var root = XElement.Parse(xmlRepresentation);
            return String.Join("",root.Elements("Sentence").Elements().Select(x => (string)x));           
        }

        /// <summary>
        /// Replaces all "customer" elements with "contact" elements with the same childs
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with customers (refer to ReplaceCustomersWithContactsSource.xml in Resources)</param>
        /// <returns>Xml representation with contacts (refer to ReplaceCustomersWithContactsResult.xml in Resources)</returns>
        public static string ReplaceAllCustomersWithContacts(string xmlRepresentation)
        {
            var root = XElement.Parse(xmlRepresentation);
            var content = root.Elements("customer").Select(x => new XElement("contact", x.Elements()));
            return new XElement("Document", content).ToString();
        }

        /// <summary>
        /// Finds all ids for channels with 2 or more subscribers and mark the "DELETE" comment
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with channels (refer to FindAllChannelsIdsSource.xml in Resources)</param>
        /// <returns>Sequence of channels ids</returns>
        public static IEnumerable<int> FindChannelsIds(string xmlRepresentation)
        {
            var root = XElement.Parse(xmlRepresentation);
            const int quantity = 2;
            return root.Elements("channel").Where(x => x.Elements("subscriber").Count() >= quantity &&
           x.DescendantNodes().OfType<XComment>().FirstOrDefault(c => c.Value == "DELETE") != null)
           .Select(x => (int)x.Attribute("id"));
        }

        /// <summary>
        /// Sort customers in docement by Country and City
        /// </summary>
        /// <param name="xmlRepresentation">Customers xml representation (refer to GeneralCustomersSourceFile.xml in Resources)</param>
        /// <returns>Sorted customers representation (refer to GeneralCustomersResultFile.xml in Resources)</returns>
        public static string SortCustomers(string xmlRepresentation)
        {
            var root = XElement.Parse(xmlRepresentation);
            var content = root.Elements().OrderBy(x => (string)x.Element("FullAddress").Element("Country")).ThenBy(x => (string)x.Element("FullAddress").Element("City"));

            return new XElement("Root", content).ToString();
        }

        /// <summary>
        /// Gets XElement flatten string representation to save memory
        /// </summary>
        /// <param name="xmlRepresentation">XElement object</param>
        /// <returns>Flatten string representation</returns>
        /// <example>
        ///     <root><element>something</element></root>
        /// </example>
        public static string GetFlattenString(XElement xmlRepresentation)
        {
            return String.Join("", xmlRepresentation.ToString().Replace(Environment.NewLine, ""));
        }

        /// <summary>
        /// Gets total value of orders by calculating products value
        /// </summary>
        /// <param name="xmlRepresentation">Orders and products xml representation (refer to GeneralOrdersFileSource.xml in Resources)</param>
        /// <returns>Total purchase value</returns>
        public static int GetOrdersValue(string xmlRepresentation)
        {
            var root = XElement.Parse(xmlRepresentation);
            int sum = 0;
            var products = root.Elements("Orders").Elements("Order").Elements("product");
            var values = root.Elements("products").Descendants();

            foreach (var product in products)
            {
                foreach (var value in values)
                {
                    if ((int)value.Attribute("Id") == (int)product)
                        sum += (int)value.Attribute("Value");
                }
            }
            return sum;
        }

    }
}
