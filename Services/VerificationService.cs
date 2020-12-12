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
//using LanguageExt;
using System.Numerics;
using System.Globalization;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Policy;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;

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
            if((status=step_3(filename)) != "OK") //Core validation
            {
                return status;
            }
            if((status=step_4(filename)) != "OK") //TODO: Overenie časovej pečiatky  a  Overenie platnosti podpisového certifikátu
            {
                return status;
            }
            /*
            if((status=step_5(filename)) != "OK") //TODO:  
            {
                return status;
            }*/
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

        private bool verifySign(byte[] certificateData, byte[] signature, byte[] data, string digestAlg, out string errorMessage)
        {
            try
            {
                Org.BouncyCastle.Asn1.X509.SubjectPublicKeyInfo ski = Org.BouncyCastle.Asn1.X509.X509CertificateStructure.GetInstance(Org.BouncyCastle.Asn1.Asn1Object.FromByteArray(certificateData)).SubjectPublicKeyInfo;
                Org.BouncyCastle.Crypto.AsymmetricKeyParameter pk = Org.BouncyCastle.Security.PublicKeyFactory.CreateKey(ski);

                string algStr = ""; //signature alg

                //find digest
                switch (digestAlg)
                {
                    case "http://www.w3.org/2000/09/xmldsig#rsa-sha1":
                        algStr = "sha1";
                        break;
                    case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
                        algStr = "sha256";
                        break;
                    case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384":
                        algStr = "sha384";
                        break;
                    case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512":
                        algStr = "sha512";
                        break;
                }

                //find encryption
                switch (ski.AlgorithmID.ObjectID.Id)
                {
                    case "1.2.840.10040.4.1": //dsa
                        algStr += "withdsa";
                        break;
                    case "1.2.840.113549.1.1.1": //rsa
                        algStr += "withrsa";
                        break;
                    default:
                        errorMessage = "verifySign 5: Unknown key algId = " + ski.AlgorithmID.ObjectID.Id;
                        return false;
                }

                Console.WriteLine("Hash digest pred decryptom: " + Convert.ToBase64String(data));
               

                errorMessage = "verifySign 8: Creating signer: " + algStr;
                Org.BouncyCastle.Crypto.ISigner verif = Org.BouncyCastle.Security.SignerUtilities.GetSigner(algStr);
                verif.Init(false, pk);
                verif.BlockUpdate(data, 0, data.Length);
                bool res = verif.VerifySignature(signature);

                Console.WriteLine("Hodnota pk je: " + pk.GetHashCode());

                Console.WriteLine("Hash digest po decrypte: " + Convert.ToBase64String(data));

                Console.WriteLine("- ");
                Console.WriteLine("Hodnota je " + res);
                Console.WriteLine("- ");
                if (!res)
                {
                    errorMessage = "verifySign 9: VerifySignature=false: dataB64=" + Convert.ToBase64String(data) + Environment.NewLine + "signatureB64=" + Convert.ToBase64String(signature) + Environment.NewLine + "certificateDataB64=" + Convert.ToBase64String(certificateData);
                }

                return res;

            }
            catch (Exception ex)
            {
                errorMessage = "verifySign 10: " + ex.ToString();
                return false;
            }
        }


        private string step_3(string filename) //Core validation
        {


            string[] SUPPORTED_TRANSFORMS = {
                "http://www.w3.org/TR/2001/REC-xml-c14n-20010315",
                "http://www.w3.org/2000/09/xmldsig#base64",
            };
            string[] SUPPORTED_DIGEST_METHOD = {
                "http://www.w3.org/2001/04/xmlenc#sha512 ",
                "http://www.w3.org/2001/04/xmldsig-more#sha384",
                "http://www.w3.org/2001/04/xmlenc#sha256",
                "http://www.w3.org/2001/04/xmldsig-more#sha224",
                "http://www.w3.org/2000/09/xmldsig#sha1",
            };

            XmlDocument xades = new XmlDocument();
            xades.Load(filename);
            var namespaceId = new XmlNamespaceManager(xades.NameTable);
            namespaceId.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            namespaceId.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
            
            




            //check ds:SignedInfo and ds:Manifest
            XmlNode signedInfoN = xades.SelectSingleNode(@"//ds:SignedInfo", namespaceId);
            XmlNodeList referenceElements = signedInfoN.SelectNodes(@"//ds:Reference", namespaceId);
            //Reference in SignedInfo
            foreach (XmlNode reference in referenceElements)
            {
                String ReferenceURI = reference.Attributes.GetNamedItem("URI").Value;
                ReferenceURI = ReferenceURI.Substring(1);
                XmlNode digestMethod = reference.SelectSingleNode("ds:DigestMethod", namespaceId);
                String digestMethodAlgorithm = digestMethod.Attributes.GetNamedItem("Algorithm").Value;
                string dsDigestValue = reference.SelectSingleNode("ds:DigestValue", namespaceId).InnerText;

                if (ReferenceURI.StartsWith("ManifestObject"))
                {
                    //get Manifest XML and check DigestValue
                    string manifestXML = xades.SelectSingleNode("//ds:Manifest[@Id='" + ReferenceURI + "']", namespaceId).OuterXml;
                    MemoryStream sManifest = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(manifestXML));
                    XmlDsigC14NTransform t = new XmlDsigC14NTransform();
                    t.LoadInput(sManifest);
                    HashAlgorithm hash = null;

                    switch (digestMethodAlgorithm)
                    {
                        case "http://www.w3.org/2000/09/xmldsig#sha1":
                            hash = new System.Security.Cryptography.SHA1Managed();
                            break;
                        case "http://www.w3.org/2001/04/xmlenc#sha256":
                            hash = new System.Security.Cryptography.SHA256Managed();
                            break;
                        case "http://www.w3.org/2001/04/xmldsig-more#sha384":
                            hash = new System.Security.Cryptography.SHA384Managed();
                            break;
                        case "http://www.w3.org/2001/04/xmlenc#sha512":
                            hash = new System.Security.Cryptography.SHA512Managed();
                            break;
                    }

                    if (hash == null)
                        return "nesprávny hashovací algoritmus " + digestMethodAlgorithm;

                    byte[] digest = t.GetDigestedOutput(hash);
                    string result = Convert.ToBase64String(digest);

                    Console.WriteLine("-");
                    Console.WriteLine("Overenie hodnoty podpisu");
                    Console.WriteLine(result);
                    Console.WriteLine(dsDigestValue);
                    Console.WriteLine("-");

                    if (!result.Equals(dsDigestValue))
                        return "hodnoty DigestValue s výpočtom Manifest sa nezhodujú";
                }
            }

            //signed info kanonikalizovat

            XmlNode checkData = xades.SelectSingleNode(@"//ds:KeyInfo/ds:X509Data/ds:X509Certificate", namespaceId);
            if (checkData == null)
                return "neobsahuje element ds:X509Data";

            byte[] signatureCertificate = Convert.FromBase64String(xades.SelectSingleNode(@"//ds:KeyInfo/ds:X509Data/ds:X509Certificate", namespaceId).InnerText);
            byte[] signature = Convert.FromBase64String(xades.SelectSingleNode(@"//ds:SignatureValue", namespaceId).InnerText);
            XmlNode signedInfoNnn = xades.SelectSingleNode(@"//ds:SignedInfo", namespaceId);
            string signedInfoTransformAlg = xades.SelectSingleNode(@"//ds:SignedInfo/ds:CanonicalizationMethod", namespaceId).Attributes.GetNamedItem("Algorithm").Value;
            string signedInfoSignatureAlg = xades.SelectSingleNode(@"//ds:SignedInfo/ds:SignatureMethod", namespaceId).Attributes.GetNamedItem("Algorithm").Value;

            XmlDsigC14NTransform t1 = new XmlDsigC14NTransform(false);
            XmlDocument pom = new XmlDocument();
            pom.LoadXml(signedInfoNnn.OuterXml);
            t1.LoadInput(pom);
            byte[] data = ((MemoryStream)t1.GetOutput()).ToArray();

            string errMsg = "";
            bool res = this.verifySign(signatureCertificate, signature, data, signedInfoSignatureAlg, out errMsg);
            if (!res)
            {
                Console.WriteLine("Error " + errMsg);
                return errMsg;
            }


            //check Id in ds:Signature
            XmlNode dsSignatureId = xades.SelectSingleNode("//ds:Signature", namespaceId).Attributes["Id"];
            if (dsSignatureId == null)
                return "ds:Signature nemá atribút Id";
            //check namespace in ds:Signature
            XmlNode dsSignatureXmlns = xades.SelectSingleNode("//ds:Signature", namespaceId).Attributes["xmlns:ds"];
            if (dsSignatureXmlns == null)
                return "ds:Signature nemá špecifikovaný namespace xmlns:ds";



            //check SignatureValue Id
            XmlNode dsSignatureValueId = xades.SelectSingleNode("//ds:SignatureValue", namespaceId).Attributes["Id"];
            if (dsSignatureValueId == null)
                return "ds:SignatureValue nemá atribút Id";



            //check reference in ds:SignedInfo
            XmlNode signedInfo = xades.SelectSingleNode("//ds:SignedInfo", namespaceId);
            XmlNodeList dsKeyInfoId = signedInfo.SelectNodes("//ds:Reference", namespaceId);
            if (dsKeyInfoId.Count < 1)
                return "ds:SignedInfo neobsahuje ds:Reference";
            String KeyInfo = "";
            String SignatureProperties = "";
            String SignedProperties = "";
            List<string> Manifest = new List<string>();
            //get URI 
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
            //check if exist ds:KeyInfo, ds:SignatureProperties, xades:SignedProperties
            XmlNode ElementKeyInfo = xades.SelectSingleNode("//ds:KeyInfo", namespaceId);
            XmlNode ElementSignatureProperties = xades.SelectSingleNode("//ds:SignatureProperties", namespaceId);
            XmlNode ElementSignedProperties = xades.SelectSingleNode("//xades:SignedProperties", namespaceId);
            
            if (ElementKeyInfo.Attributes["Id"] == null)
                return "ds:Keyinfo nemá atribút Id";
            if (!ElementKeyInfo.Attributes["Id"].Value.Equals(KeyInfo))
                return "ds:Keyinfo, nezhoduje sa Id s URI";

            if (ElementSignatureProperties.Attributes["Id"] == null)
                return "ds:SignatureProperties nemá atribút Id";
            if (!ElementSignatureProperties.Attributes["Id"].Value.Equals(SignatureProperties))
                return "ds:SignatureProperties, nezhoduje sa Id s URI";

            if (ElementSignedProperties.Attributes["Id"] == null)
                return "xades:SignedProperties nemá atribút Id";
            if (!ElementSignedProperties.Attributes["Id"].Value.Equals(SignedProperties))
                return "xades:SignedProperties, nezhoduje sa Id s URI";

            //check if exist ds:Manifest
            XmlNodeList ElementManifest = xades.SelectNodes("//ds:Manifest", namespaceId);
            bool flag = false;
            foreach (XmlNode OneManifest in ElementManifest)
            {
                foreach(String ManifestURI in Manifest)
                {
                    if (OneManifest.Attributes["Id"] == null || !OneManifest.Attributes["Id"].Value.Equals(ManifestURI))
                        flag = true;
                }
            }
            if(!flag)
                return "ds:Manifest nemá atribút Id alebo sa nezhoduje Id s URI";



            //check ds:KeyInfo Id
            if (ElementKeyInfo.Attributes["Id"] == null)
                return "element ds:KeyInfo nemá atribút Id";
            //check ds:KeyInfo elements
            XmlNode X509Data = ElementKeyInfo.SelectSingleNode("//ds:X509Data", namespaceId);
            if (X509Data == null)
                return "ds:KeyInfo neobsahuje element ds:X509Data";
            if (X509Data.ChildNodes.Count < 3)
                return " chýbajú podelementy pre ds:X509Data";
            //check ds:KeyInfo values
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
            BigInteger hex = BigInteger.Parse(cert.SerialNumber, NumberStyles.AllowHexSpecifier);
            if (!cert.Subject.Equals(SubjectName))
                return "hodnota ds:X509SubjectName sa nezhoduje s príslušnou hodnotou v certifikáte";
            if (!cert.Issuer.Equals(IssuerSerialFirst))
                return "hodnota ds:X509IssuerName sa nezhoduje s príslušnou hodnotou v certifikáte";
            if (!hex.ToString().Equals(IssuerSerialSecond))
                return "hodnota ds:X509SerialNumber sa nezhoduje s príslušnou hodnotou v certifikáte";



            //check ds:SignatureProperties Id
            if (ElementSignatureProperties.Attributes["Id"] == null)
                return "element ds:SignatureProperties nema atribut Id";
            //check ds:SignatureProperties numbers of elements
            XmlNodeList elemListSignatureProperties = ElementSignatureProperties.ChildNodes;
            if (elemListSignatureProperties.Count < 2)
                return "ds:SignatureProperties neobsahuje dva elementy";
            //check ds:SignatureProperties elements 
            for (int i = 0; i < elemListSignatureProperties.Count; i++)
            {
                if (elemListSignatureProperties[i].FirstChild.Name.Equals("xzep:SignatureVersion") ||
                   elemListSignatureProperties[i].FirstChild.Name.Equals("xzep:ProductInfos"))
                {
                    String tmpTargetValue = elemListSignatureProperties[i].Attributes["Target"].Value;
                    tmpTargetValue = tmpTargetValue.Substring(1);
                    if (!tmpTargetValue.Equals(dsSignatureId.Value))
                        return "atribút Target v elemente ds:SignatureProperty nie je nastavený na element ds:Signature";
                }
            }



            //check Manifest and Manifest references
            String ManifestReferenceUri = "";
            String algorithmDigestMethod = "";
            String algorithmTransforms = "";
            String digestValue = "";
            bool flag1 = false;
            


            for (int i = 0; i < ElementManifest.Count; i++)
            {
                //check ds:Manifest Id
                if (ElementManifest[i].Attributes["Id"] == null)
                    return "jeden z elementov ds:Manifest nemá atribút Id";
                //check number of reference, ds:Object
                XmlNodeList ManifestChildNodes = ElementManifest[i].ChildNodes;
                if (ManifestChildNodes.Count > 1 || ManifestChildNodes.Count < 1)
                    return "ds:Manifest neobsahuje práve jedenu referenciu";
                if (!ManifestChildNodes[0].Attributes["Type"].Value.Contains("Object"))
                    return "nezhoduje sa Type v ds:Manifest";
                //check value attribute Type
                if (ManifestChildNodes[0].Attributes["Type"].Value.Equals("http://www.w3.org/2000/09/xmldsig#Object"))
                {
                    //check supported ds:Transforms and ds:DigestMethod 
                    XmlNodeList ReferenceElementsChild = ManifestChildNodes[0].ChildNodes;
                    for (int l = 0; l < ReferenceElementsChild.Count; l++)
                    {
                        if(ReferenceElementsChild[l].Name.Equals("ds:Transforms"))
                        {
                            algorithmTransforms = ReferenceElementsChild[l].FirstChild.Attributes["Algorithm"].Value;
                            if (!SUPPORTED_TRANSFORMS.Contains(algorithmTransforms))
                                return "ds:Transforms neobsahuje podporovaný algoritmus pre daný element podľa profilu XAdES_ZEP";
                        }
                        if (ReferenceElementsChild[l].Name.Equals("ds:DigestMethod"))
                        {
                            algorithmDigestMethod = ReferenceElementsChild[l].Attributes["Algorithm"].Value;
                            if (!SUPPORTED_DIGEST_METHOD.Contains(algorithmDigestMethod))
                                return "ds:DigestMethod neobsahuje podporovaný algoritmus pre daný element podľa profilu XAdES_ZEP";
                        }
                        if (ReferenceElementsChild[l].Name.Equals("ds:DigestValue"))
                        {
                            digestValue = ReferenceElementsChild[l].InnerText;
                        }
                    }
                    ManifestReferenceUri = ManifestChildNodes[0].Attributes["URI"].Value;
                    ManifestReferenceUri = ManifestReferenceUri.Substring(1);

                    //check values ds:Manifest and ds:Object
                    XmlNodeList ObjectElement = xades.SelectNodes("//ds:Object", namespaceId);
                    for (int j = 0; j < ObjectElement.Count; j++)
                    {
                        if (ObjectElement[j].Attributes["Id"] == null)
                            continue;
                        if (ObjectElement[j].Attributes["Id"].Value.Equals(ManifestReferenceUri))
                        { 
                            flag1 = true;

                            XmlDsigC14NTransform t = new XmlDsigC14NTransform();
                            XmlDocument myDoc = new XmlDocument();
                            myDoc.LoadXml(ObjectElement[i].OuterXml);
                            t.LoadInput(myDoc);
                            Stream s = (Stream)t.GetOutput(typeof(Stream));
                            byte[] hash;
                            string base64String = "";
                            switch (algorithmDigestMethod)
                            {
                                case "http://www.w3.org/2001/04/xmldsig#sha1":
                                    SHA1 sha1 = SHA1.Create();
                                    hash = sha1.ComputeHash(s);
                                    base64String = Convert.ToBase64String(hash);
                                    break;
                                case "http://www.w3.org/2001/04/xmlenc#sha256":
                                    SHA256 sha256 = SHA256.Create();
                                    hash = sha256.ComputeHash(s);
                                    base64String = Convert.ToBase64String(hash);
                                    break;
                                case "http://www.w3.org/2001/04/xmldsig-more#sha384":
                                    SHA384 sha384 = SHA384.Create();
                                    hash = sha384.ComputeHash(s);
                                    base64String = Convert.ToBase64String(hash);
                                    break;
                                case "http://www.w3.org/2001/04/xmlenc#sha512":
                                    SHA512 sha512 = SHA512.Create();
                                    hash = sha512.ComputeHash(s);
                                    base64String = Convert.ToBase64String(hash);
                                    break;
                            }
                            Console.WriteLine("-");
                            Console.WriteLine("Overenie hodnoty ds:DigestValue");
                            Console.WriteLine("First " + base64String);
                            Console.WriteLine("Second " + digestValue);
                            Console.WriteLine("-");
                            
                        }
                    }
                    if (!flag1)
                        return "odkaz z ds:Manifest na ds:Object sa nezhoduje";
                    flag1 = false;
                }
                else
                    return "ds:Reference, hodnota atribútu Type sa neoverila voči profilu XADES_ZEP";
            }


            return "OK";
        }

        private string step_4(string filename) //TODO: Overenie časovej pečiatky
        {
            XmlDocument xades = new XmlDocument();
            xades.Load(filename);
            var namespaceId = new XmlNamespaceManager(xades.NameTable);
            namespaceId.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            namespaceId.AddNamespace("xades", "http://uri.etsi.org/01903/v1.3.2#");
            string timestamp = xades.SelectSingleNode("//xades:EncapsulatedTimeStamp", namespaceId).InnerText;
            byte[] newBytes = Convert.FromBase64String(timestamp);

            byte[] signatureCertificate = Convert.FromBase64String(xades.SelectSingleNode(@"//ds:KeyInfo/ds:X509Data/ds:X509Certificate", namespaceId).InnerText);
            X509CertificateParser x509parser = new X509CertificateParser();
            Org.BouncyCastle.X509.X509Certificate x509cert = x509parser.ReadCertificate(signatureCertificate);

            string signedInfoSignatureAlg = xades.SelectSingleNode(@"//ds:SignedInfo/ds:SignatureMethod", namespaceId).Attributes.GetNamedItem("Algorithm").Value;

            byte[] signature = Convert.FromBase64String(xades.SelectSingleNode(@"//ds:SignatureValue", namespaceId).InnerText);


            TimeStampToken token = new TimeStampToken(new Org.BouncyCastle.Cms.CmsSignedData(newBytes));

            try
            {
                
                Org.BouncyCastle.X509.X509Certificate signerCert = null;
                Org.BouncyCastle.X509.Store.IX509Store x509Certs = token.GetCertificates("Collection");
                ArrayList certs = new ArrayList(x509Certs.GetMatches(null));

                // nájdenie podpisového certifikátu tokenu v kolekcii
                foreach (Org.BouncyCastle.X509.X509Certificate cert in certs)
                {
                    string cerIssuerName = cert.IssuerDN.ToString(true, new Hashtable());
                    string signerIssuerName = token.SignerID.Issuer.ToString(true, new Hashtable());

                    // kontrola issuer name a seriového čísla
                    if (cerIssuerName == signerIssuerName && cert.SerialNumber.Equals(token.SignerID.SerialNumber))
                    {
                        signerCert = cert;
                        break;
                    }
                }

                //check certificate, UtcNow
                int result1 = DateTime.Compare(signerCert.NotAfter, DateTime.UtcNow);
                int result2 = DateTime.Compare(DateTime.UtcNow, signerCert.NotBefore);
                //check x509 certtificate, timestamtoken.GenTime
                int result3 = DateTime.Compare(x509cert.NotAfter, token.TimeStampInfo.TstInfo.GenTime.ToDateTime());
                int result4 = DateTime.Compare(token.TimeStampInfo.TstInfo.GenTime.ToDateTime(), x509cert.NotBefore);


                if (result1 < 0)
                    return "platnosť certifikátu vypršala";
                if (result2 < 0)
                    return "certifikát nenadobúdol platnosť";
                if (result3 < 0)
                    return "platnosť podpisového certifikátu v čase T vypršala";
                if (result4 < 0)
                    return "platnosť podpisového certifikátu v čase T nenadobudla plastnosť";

                /*
                Console.WriteLine("step4");
                //check messageImprint against SignatureValue
                string errMsg = "";
                bool res = this.verifySign(signatureCertificate, signature, token.TimeStampInfo.GetMessageImprintDigest(), signedInfoSignatureAlg, out errMsg);
                if (!res)
                {
                    Console.WriteLine("Error " + errMsg);
                    return errMsg;
                }
                */

                //check certificate, CRL
                byte[] buf1 = File.ReadAllBytes("./Crl/certCasvovejPeciatky.crl");
                X509CrlParser parserCrl1 = new X509CrlParser();
                X509Crl readCrl1 = parserCrl1.ReadCrl(buf1);
                if (readCrl1.IsRevoked(signerCert))
                    return "certifikát je neplatný";

                //check certificate, CRL
                byte[] buf2 = File.ReadAllBytes("./Crl/dtctsa.crl");
                X509CrlParser parserCrl2 = new X509CrlParser();
                X509Crl readCrl2 = parserCrl2.ReadCrl(buf2);
                if (readCrl2.IsRevoked(x509cert))
                    return "certifikát je neplatný";



            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }

            return "OK";
        }
        
        private string step_5(string filename) //TODO: Overenie platnosti podpisového certifikátu
        {
            

            return "OK";
        }
    }
}
