using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

namespace SIPVS
{
    public class FormService: IFormService
    {
        public string SaveXml(FormModel formModel)
        {
            System.Console.WriteLine("SaveXml processed");
            return "SaveXml processed";
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