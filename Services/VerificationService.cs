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
using System.Security.Cryptography.X509Certificates;
using LanguageExt;
using System.Numerics;

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
                report += $" * {filename.Split('\\')[1]}: \n\t* {verify(filename)}\n\n";
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
            XmlDocument xades = new XmlDocument();
            xades.Load(filename);
            var namespaceId = new XmlNamespaceManager(xades.NameTable);
            namespaceId.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            namespaceId.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");

            //check Signature Id
            XmlNode dsSignatureId = xades.SelectSingleNode("//ds:Signature", namespaceId).Attributes["Id"];
            if (dsSignatureId == null)
                return "ds:Signature neobsahuje Id atribut";

            //check Signature namespace xmlns:ds
            XmlNode dsSignatureXmlns = xades.SelectSingleNode("//ds:Signature", namespaceId).Attributes["xmlns:ds"];
            if (dsSignatureXmlns == null)
                return "ds:Signature nema specifikovany namespace xmlns:ds";

            //check SignatureValue Id
            XmlNode dsSignatureValueId = xades.SelectSingleNode("//ds:SignatureValue", namespaceId).Attributes["Id"];
            if (dsSignatureValueId == null)
                return "ds:SignatureValue neobsahuje Id atribut";

            //check SignedInfo
            XmlNode signedInfo = xades.SelectSingleNode("//ds:SignedInfo", namespaceId);
            XmlNodeList dsKeyInfoId = signedInfo.SelectNodes("//ds:Reference", namespaceId);

            if (dsKeyInfoId.Count < 1)
                return "ds:SignedInfo neobsahuje ds:Reference";

            String KeyInfo = "";
            String SignatureProperties = "";
            String SignedProperties = "";
            List<string> Manifest = new List<string>();
            foreach (XmlNode ReferenceList in dsKeyInfoId)
            {
                if (ReferenceList.Attributes["Id"] == null)
                    continue;
                else if(ReferenceList.Attributes["Id"] != null)
                {
                    if(ReferenceList.Attributes["Type"].Value.Contains("Object"))
                    {
                        KeyInfo = ReferenceList.Attributes["URI"].Value;
                        KeyInfo = KeyInfo.Substring(1);
                    }
                    else if(ReferenceList.Attributes["Type"].Value.Contains("SignatureProperties"))
                    {
                        SignatureProperties = ReferenceList.Attributes["URI"].Value;
                        SignatureProperties = SignatureProperties.Substring(1);
                    }
                    else if (ReferenceList.Attributes["Type"].Value.Contains("SignedProperties"))
                    {
                        SignedProperties = ReferenceList.Attributes["URI"].Value;
                        SignedProperties = SignedProperties.Substring(1);
                    }
                    else if (ReferenceList.Attributes["Type"].Value.Contains("Manifest"))
                    {
                        String tmp = ReferenceList.Attributes["URI"].Value;
                        tmp = tmp.Substring(1);
                        Manifest.Add(tmp);
                    }
                }  
            }

            //use in element keyinfo
            XmlNode ElementKeyInfo = xades.SelectSingleNode("//ds:KeyInfo", namespaceId);
            XmlNode ElementSignatureProperties = xades.SelectSingleNode("//ds:SignatureProperties", namespaceId);
            XmlNode ElementSignedProperties = xades.SelectSingleNode("//xades:SignedProperties", namespaceId);
            if (ElementKeyInfo.Attributes["Id"] == null || !ElementKeyInfo.Attributes["Id"].Value.Equals(KeyInfo))
                return "ds:Keyinfo nema Id alebo sa nezhoduje Id s URI";

            if (ElementSignatureProperties.Attributes["Id"] == null || !ElementSignatureProperties.Attributes["Id"].Value.Equals(SignatureProperties))
                return "ds:SignatureProperties nema Id alebo sa nezhoduje Id s URI";
            
            if (ElementSignedProperties.Attributes["Id"] == null || !ElementSignedProperties.Attributes["Id"].Value.Equals(SignedProperties))
                return "xades:SignedProperties nema Id alebo sa nezhoduje Id s URI";

            XmlNodeList ElementManifest = xades.SelectNodes("//ds:Manifest", namespaceId);
            bool flag = false;
            foreach(XmlNode OneManifest in ElementManifest)
            {
                foreach(String ManifestURI in Manifest)
                {
                    if (OneManifest.Attributes["Id"] == null || !OneManifest.Attributes["Id"].Value.Equals(ManifestURI))
                        flag = true;
                }
            }

            if(!flag)
                return "ds:Manifest nema Id alebo sa nezhoduje Id s URI";

            //check KeyInfo
            if (ElementKeyInfo.Attributes["Id"] == null)
                return "element ds:KeyInfo nema atribut Id";

            XmlNode X509Data = ElementKeyInfo.SelectSingleNode("//ds:X509Data", namespaceId);
            if (X509Data == null)
                return "neobsahuje ds:X509Data";

            if (X509Data.ChildNodes.Count < 3)
                return "chybaju elementy z X509Data";

            XmlNodeList elemList = X509Data.ChildNodes;
            byte[] bytes;
            var cert = new X509Certificate2();
            String IssuerSerialFirst = "";
            String IssuerSerialSecond = "";
            String SubjectName = "";
            for (int i = 0; i < elemList.Count; i++)
            {
                switch (elemList[i].Name)
                {
                    case "ds:X509Certificate":
                        bytes = Convert.FromBase64String(elemList[i].InnerText);
                        cert = new X509Certificate2(bytes);
                        break;
                    case "ds:X509IssuerSerial":
                        if(elemList[i].HasChildNodes)
                        {
                            IssuerSerialFirst = elemList[i].FirstChild.InnerText;
                            IssuerSerialSecond = elemList[i].LastChild.InnerText;
                        }
                        break;
                    case "ds:X509SubjectName":
                        SubjectName = elemList[i].InnerText;
                        break;
                }
            }

            if (!cert.Subject.Equals(SubjectName))
                return "Subject sa nezhoduje";
            if (!cert.Issuer.Equals(IssuerSerialFirst))
                return "Issuername sa nezhoduje";
            if (!cert.SerialNumber.Equals(IssuerSerialSecond))
                return "Serialnumber sa nezhoduje";

            return "OK";
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
