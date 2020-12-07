using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
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
    public class VerificationService: IVerificationService
    {
        public string SignaturesVerification()
        {
            string report = "# Cvičenie 5 – Overenie „XADES-T“ podpisu\n\n";
            string[] files = Directory.GetFiles("./Priklady");
            Array.Sort(files);
            foreach(string filename in files) 
            {
                report += $" * {filename.Split('/')[2]}: \n\t* {verify(filename)}\n\n";
                System.Console.WriteLine(filename);
            }
            return report;
        }

        private string verify(string filename)
        {
            string status;

            if((status=step_1(filename)) != "OK") //Overenie dátovej obálky
            {
                return status;
            }
            if((status=step_2(filename)) != "OK") //Overenie XML Signature
            {
                return status;
            }
            if((status=step_3(filename)) != "OK") //TODO: Core validation
            {
                return status;
            }
            if((status=step_4(filename)) != "OK") //TODO: Overenie časovej pečiatky 
            {
                return status;
            }
            if((status=step_5(filename)) != "OK") //TODO: Overenie platnosti podpisového certifikátu 
            {
                return status;
            }
            return "OK";
        }

        private string step_1(string filename) //Overenie dátovej obálky
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement root = doc.DocumentElement;

            if(root.Attributes["xmlns:xzep"].Value == "http://www.ditec.sk/ep/signature_formats/xades_zep/v1.0" 
                && root.Attributes["xmlns:ds"].Value == "http://www.w3.org/2000/09/xmldsig#")
            {
                return "OK";
            } 


            return "koreňový element musí obsahovať atribúty xmlns:xzep a xmlns:ds podľa profilu XADES_ZEP";
        }
        
        private string step_2(string filename) //Overenie XML Signature
        {
            string[] SUPPORTED_SIGNATURE_ALGOS = {
                "http://www.w3.org/2000/09/xmldsig#dsa-sha1",
                "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
                "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
                "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384",
                "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512"
            };
            string[] SUPPORTED_DIGEST_ALGOS = {
                "http://www.w3.org/2000/09/xmldsig#sha1",
                "http://www.w3.org/2001/04/xmldsig-more#sha224",
                "http://www.w3.org/2001/04/xmlenc#sha256",
                "https://www.w3.org/2001/04/xmldsig-more#sha384",
                "http://www.w3.org/2001/04/xmlenc#sha512"
            };

            XmlDocument xades = new XmlDocument();
            xades.Load(filename);
            var namespaceId = new XmlNamespaceManager(xades.NameTable);
            namespaceId.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            var signedInfo = xades.SelectSingleNode("//ds:SignedInfo ", namespaceId);
            
            string signatureMethodAlgorithm = signedInfo.SelectSingleNode("//ds:SignatureMethod", namespaceId).Attributes["Algorithm"].Value;
            string canonicalizationMethodMethodAlgorithm = signedInfo.SelectSingleNode("//ds:CanonicalizationMethod", namespaceId).Attributes["Algorithm"].Value;
            if(!SUPPORTED_SIGNATURE_ALGOS.Contains(signatureMethodAlgorithm) ||
                canonicalizationMethodMethodAlgorithm != "http://www.w3.org/TR/2001/REC-xml-c14n-20010315")
            {
                return "kontrola obsahu ds:SignatureMethod a ds:CanonicalizationMethod – musia obsahovať URI niektorého z podporovaných algoritmov pre dané elementy podľa profilu XAdES_ZEP";
            }

            foreach(XmlElement reference in signedInfo.SelectNodes("//ds:Reference", namespaceId)) 
            {
                foreach(XmlElement transform in reference.SelectSingleNode("//ds:Transforms", namespaceId))
                {
                    string transformsAlgorithm = transform.SelectSingleNode("//ds:Transform", namespaceId).Attributes["Algorithm"].Value;
                    if(transformsAlgorithm != "http://www.w3.org/TR/2001/REC-xml-c14n-20010315")
                    {
                        return "kontrola obsahu ds:Transforms a ds:DigestMethod vo všetkých referenciách v ds:SignedInfo – musia obsahovať URI niektorého z podporovaných algoritmov podľa profilu XAdES_ZEP";
                    }
                }
                string digestMethodAlgorithm = reference.SelectSingleNode("//ds:DigestMethod", namespaceId).Attributes["Algorithm"].Value;
                if(!SUPPORTED_DIGEST_ALGOS.Contains(digestMethodAlgorithm))
                {
                    return "kontrola obsahu ds:Transforms a ds:DigestMethod vo všetkých referenciách v ds:SignedInfo – musia obsahovať URI niektorého z podporovaných algoritmov podľa profilu XAdES_ZEP";
                }
            }
            
            return "OK";
        }
        
        private string step_3(string filename) //TODO: Core validation
        {
            return "TODO: dalsie body overenia";
        }
        
        private string step_4(string filename) //TODO: Overenie časovej pečiatky
        {
            return "TODO: dalsie body overenia";
        }
        
        private string step_5(string filename) //TODO: Overenie platnosti podpisového certifikátu
        {
            return "TODO: dalsie body overenia";
        }
    }
}
