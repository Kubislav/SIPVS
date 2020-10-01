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
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("./Data/", fileName)))
            {
                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                XDocument doc = XDocument.Parse(xml);
                outputFile.WriteLine(doc.ToString());
            }
            return fileName + " created!";
        }

        public string ValidXsd()
        {
            System.Console.WriteLine("ValidXsd processed");
            return "ValidXsd processed";
        }

        public string SaveHtml()
        {
            System.Console.WriteLine("SaveHtml processed");
            return "SaveHtml processed";
        }
    }
}