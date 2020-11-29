using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;
using System.Xml.XPath;
using Org.BouncyCastle;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509.Store;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Asn1.Tsp;

namespace SIPVS
{
    public class FormService: IFormService
    {
        public string SaveXml(string xml)
        {
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + "_file.xml";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("./Data/", fileName)))
            {
                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                XDocument doc = XDocument.Parse(xml);
                outputFile.WriteLine(doc.ToString());
            }
            return fileName;
        }

        public string ValidXsd(string xml_file, string xsd_file)
        {
            XmlReader forms = null;
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add("http://www.w3.org", "./Data/" + xsd_file);
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationEventHandler += new ValidationEventHandler(settingsValidationEventHandler);


                forms = XmlReader.Create("./Data/" + xml_file, settings);

                while (forms.Read()) { }

                return "ok";
            }
            catch (Exception e)
            {

                return e.Message;
            }
            finally
            {
                if (forms != null)
                {
                    forms.Close();
                }
            }

        }

        private static void settingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Console.Write("WARNING!");
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Console.Write("ERROR!");
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }
        }

        public string SaveHtml(string xml_file, string xsl_file)
        {
            // Load the style sheet.
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load("./Data/" + xsl_file);

            string[] stringToSplit = xml_file.Split('.');
            string fileName = stringToSplit[0] + ".html";
 
            // Execute the transform and output the results to a file.
            xslt.Transform("./Data/" + xml_file, "./Data/" + fileName);

            return fileName;
            
        }
		
		public string SignDocument(string xml_file, string xsl_file, string xsd_file)
        {
            string jsonString = JsonSerializer.Serialize(new {
                xml_file = File.ReadAllText(Path.Combine("./Data/", xml_file)),
                xsl_file = File.ReadAllText(Path.Combine("./Data/", xsl_file)),
                xsd_file = File.ReadAllText(Path.Combine("./Data/", xsd_file)),
            });
            return jsonString;
        }

        public string saveXades(string data)
        {
            string fileName = "xades.xml";
            byte[] bytes = System.Convert.FromBase64String(data);
            string decoded = System.Text.ASCIIEncoding.ASCII.GetString(bytes);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("./Data/", fileName)))
            {
                XDocument doc = XDocument.Parse(decoded);
                outputFile.WriteLine(doc.ToString());
            }
            return fileName;
            
        }
    
        public string MakeStamp(string xades_file)
        {
            XmlDocument xades = new XmlDocument();
            xades.Load("./Data/" + xades_file);

            var namespaceId = new XmlNamespaceManager(xades.NameTable);
            namespaceId.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            string data = xades.SelectSingleNode("//ds:SignatureValue", namespaceId).InnerXml;
            
            String signatureTimeStamp = getSignatureTimeStamp(data);
            TimeStampResponse tokenResponse = new TimeStampResponse(Convert.FromBase64CharArray(signatureTimeStamp.ToCharArray(), 0, signatureTimeStamp.Length));
            Console.WriteLine(Convert.ToBase64String(tokenResponse.TimeStampToken.GetEncoded()));
            
            XmlNodeList elemList = xades.GetElementsByTagName("xades:QualifyingProperties");
            XmlElement UnsignedElem = xades.CreateElement("xades" , "UnsignedProperties", "http://uri.etsi.org/01903/v1.3.2#");
            XmlElement UnsignedSignElem = xades.CreateElement("xades" , "UnsignedSignatureProperties", "http://uri.etsi.org/01903/v1.3.2#");
            XmlElement SigTimeElem = xades.CreateElement("xades" , "SignatureTimeStamp", "http://uri.etsi.org/01903/v1.3.2#");
            XmlElement EncapsulatedTimestamp = xades.CreateElement("xades", "EncapsulatedTimeStamp", "http://uri.etsi.org/01903/v1.3.2#");
            SigTimeElem.SetAttribute("Id", "IdSignatureTimeStamp");
            EncapsulatedTimestamp.InnerText = Convert.ToBase64String(tokenResponse.TimeStampToken.GetEncoded());

            UnsignedElem.AppendChild(UnsignedSignElem);
            UnsignedSignElem.AppendChild(SigTimeElem);
            SigTimeElem.AppendChild(EncapsulatedTimestamp);

            elemList[0].InsertAfter(UnsignedElem, elemList[0].LastChild);
            xades.Save("./Data/xades.xml");
            Console.Write("text" + elemList[0]);
            return "xades.xml";
        }

        /**
            Funkcia vracia casovu peciatku 
        */
        private static string getSignatureTimeStamp(String data) {
            using (var client = new WebClient())
            {
                var response = client.UploadString(
                    new Uri("http://test.ditec.sk/timestampws/TS.aspx"), 
                    "POST", 
                    data
                );
                return response;
            }
        }
    }
}