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
            using (var package = Package.Open(xlsxFileName, FileMode.Open, FileAccess.Read))
            {
                var descendants = GetDocument(package, "/xl/sharedStrings.xml").Root.Descendants();
                var planets = descendants.Reverse().Skip(4).Reverse().SelectMany(lvl1 => lvl1.Elements().Select(lvl2 =>lvl2.Value));
               
                var xDoc = GetDocument(package, "/xl/worksheets/sheet1.xml");
                var radii = xDoc.Root.Descendants(xDoc.Root.Name.Namespace + "v").Skip(3)
                    .Where((m, n) => n % 2 == 0).Select(x => double.Parse(String.Format("{0}", x.Value.Replace('.', ','))));

               return planets.Zip(radii, (p, r) => new PlanetInfo() { Name = p, MeanRadius = r });
            }            
        }

        private static XDocument GetDocument(Package package,string path)
        {
            var part = package.GetPart(new Uri(path, UriKind.Relative));
            using(var source = part.GetStream(FileMode.Open, FileAccess.Read))
            return XDocument.Load(source);
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
            var source = File.Open(fileName, FileMode.Open, FileAccess.Read);

            if (method == DecompressionMethods.Deflate)
            {
                return new DeflateStream(source, CompressionMode.Decompress);
            }
            if (method == DecompressionMethods.GZip)
            {
                return new GZipStream(source, CompressionMode.Decompress);
            }

            return source;
        }


        /// <summary>
        /// Reads file content econded with non Unicode encoding
        /// </summary>
        /// <param name="fileName">source file name</param>
        /// <param name="encoding">encoding name</param>
        /// <returns>Unicoded file content</returns>
        public static string ReadEncodedText(string fileName, string encoding)
        {
            return File.ReadAllText(fileName, Encoding.GetEncoding(encoding));
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