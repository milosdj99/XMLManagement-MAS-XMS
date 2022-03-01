using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XMS
{
    public class IntegrityCheck
    {
        internal static Dictionary<string, string> Checksums = new Dictionary<string, string>();

        public static string GenerateCheckSum(string xmlString)
        {
            var xmlBytes = new UnicodeEncoding().GetBytes(xmlString);
            var hashedXmlBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(xmlBytes);
            var hashedString = BitConverter.ToString(hashedXmlBytes);

            return hashedString;
        }

        public static bool Check(string filename)
        {
            XDocument doc = XDocument.Load(filename);
            
            string novi = doc.ToString();

            string noviCheckSum = GenerateCheckSum(novi);

            if(noviCheckSum.Equals(Checksums[filename]))
            {
                return true;

            } else
            {
                return false;
            }

            
        }

        public static void LoadCheckSums()
        {
            string[] lines = File.ReadAllLines("checksums.txt");

            Checksums = new Dictionary<string, string>();

            foreach (string line in lines)
            {
                string[] par = line.Split(':');

                Checksums.Add(par[0], par[1]);

            }

        }

        public static void StoreCheckSums()
        {

            File.WriteAllText("checksums.txt", String.Empty);

            StreamWriter sw = new StreamWriter("checksums.txt");


            foreach(string filename in Checksums.Keys)
            {
                sw.WriteLine($"{filename}:{Checksums[filename]}");

            }

            sw.Close();

        }
    }
}
