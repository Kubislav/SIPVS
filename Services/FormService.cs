using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using System.IO;
using System.Xml.Linq;

namespace SIPVS
{
    public class FormService: IFormService
    {
        public string SaveXml(string xml)
        {
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + "_file.xml";
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), fileName)))
            {
                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                XDocument doc = XDocument.Parse(xml);
                outputFile.WriteLine(doc.ToString());
            }
            return fileName;
        }

        public string ValidXsd(string xml_file, string xsd_file)
        {
            System.Console.WriteLine(xml_file + " and " + xsd_file);
            return "xml not yet validated!";
        }

        public string SaveHtml(string xml_file, string xsl_file)
        {
            System.Console.WriteLine(xml_file + " and " + xsl_file);
            return "html not yet created!";
        }
    }
}