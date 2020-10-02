using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Schema;

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

                System.Console.WriteLine(xml_file + " and " + xsd_file);
                return "xml successfully validated!";
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

        static void settingsValidationEventHandler(object sender, ValidationEventArgs e)
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
            System.Console.WriteLine(xml_file + " and " + xsl_file);
            return "html not yet created!";
        }
    }
}