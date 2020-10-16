using System.Collections.Generic;

public interface IFormService
{
  public string SaveXml(string xml);
  
  public string ValidXsd(string xml_file, string xsd_file);

  public string SaveHtml(string xml_file, string xsl_file);
  
  public string SignDocument(string xml_file, string xsl_file, string xsd_file);

    public string saveXades(string data);
}