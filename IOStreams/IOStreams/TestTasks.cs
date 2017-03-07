using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
 

namespace IOStreams
{

    public static class TestTasks
    {
        /// <summary>
        /// Parses Resourses\Planets.xlsx file and returns the planet data: 
        ///   Jupiter     69911.00
        ///   Saturn      58232.00
        ///   Uranus      25362.00
        ///    ...
        /// See Resourses\Planets.xlsx for details
        /// </summary>
        /// <param name="xlsxFileName">source file name</param>
        /// <returns>sequence of PlanetInfo</returns>
        public static IEnumerable<PlanetInfo> ReadPlanetInfoFromXlsx(string xlsxFileName)
        {
            var list = new List<PlanetInfo>();

            using (var package = Package.Open(xlsxFileName, FileMode.Open, FileAccess.Read))
            {
                var part1 = package.GetPart(new Uri("/xl/sharedStrings.xml", UriKind.Relative));
                var part2 = package.GetPart(new Uri("/xl/worksheets/sheet1.xml", UriKind.Relative));

                using (var planetSource = part1.GetStream(FileMode.Open, FileAccess.Read))
                {
                    XDocument xDoc1 = XDocument.Load(planetSource);

                    var descendants = xDoc1.Root.Descendants(); 

                    foreach (var lvl1 in descendants.Take(descendants.Count() - 4))
                    {
                        foreach (var lvl2 in lvl1.Elements())
                        {
                            list.Add(new PlanetInfo() { Name = lvl2.Value });
                        }
                    }
                }
                using (var radiusSource = part2.GetStream(FileMode.Open, FileAccess.Read))
                {
                    XDocument xDoc2 = XDocument.Load(radiusSource);

                    var radii = xDoc2.Root.Descendants(xDoc2.Root.Name.Namespace + "v").Skip(3).Where((m, n) => n % 2 == 0);
                    for (int i = 0; i < radii.Count(); i++)
                    {
                        list[i].MeanRadius = double.Parse(String.Format("{0}", radii.ElementAt(i).Value.Replace('.', ',')));
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// Calculates hash of stream using specifued algorithm
        /// </summary>
        /// <param name="stream">source stream</param>
        /// <param name="hashAlgorithmName">hash algorithm ("MD5","SHA1","SHA256" and other supported by .NET)</param>
        /// <returns></returns>
        public static string CalculateHash(this Stream stream, string hashAlgorithmName)
        {
            HashAlgorithm algorithm = HashAlgorithm.Create(hashAlgorithmName);

            if(algorithm==null)
            throw new ArgumentException();

            return BitConverter.ToString(algorithm.ComputeHash(stream)).Replace("-","");   
        }


        /// <summary>
        /// Returns decompressed strem from file. 
        /// </summary>
        /// <param name="fileName">source file</param>
        /// <param name="method">method used for compression (none, deflate, gzip)</param>
        /// <returns>output stream</returns>
        public static Stream DecompressStream(string fileName, DecompressionMethods method)
        {
            var outStream = new MemoryStream();

            var source = File.Open(fileName, FileMode.Open, FileAccess.Read);
             
                if (method == DecompressionMethods.Deflate)
                {
                    using (var decompressStream = new DeflateStream(source, CompressionMode.Decompress))
                    {
                        decompressStream.CopyTo(outStream);
                    }   
                }
                if (method == DecompressionMethods.GZip)
                {
                    using (var decompressStream = new GZipStream(source, CompressionMode.Decompress))
                    {
                        decompressStream.CopyTo(outStream);
                    }
                }
                if (method == DecompressionMethods.None)
                    return source;
             
            outStream.Position = 0;
            return outStream;
        }


        /// <summary>
        /// Reads file content econded with non Unicode encoding
        /// </summary>
        /// <param name="fileName">source file name</param>
        /// <param name="encoding">encoding name</param>
        /// <returns>Unicoded file content</returns>
        public static string ReadEncodedText(string fileName, string encoding)
        {
             string text;
             using (var source = new StreamReader(fileName, System.Text.Encoding.GetEncoding(encoding)))
             {
                  text = source.ReadToEnd();
             }
             return text;
        }
    }


    public class PlanetInfo : IEquatable<PlanetInfo>
    {
        public string Name { get; set; }
        public double MeanRadius { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, MeanRadius);
        }

        public bool Equals(PlanetInfo other)
        {
            return Name.Equals(other.Name)
                && MeanRadius.Equals(other.MeanRadius);
        }
    }

}