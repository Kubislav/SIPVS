using System.Collections.Generic;

public interface IFormService
{
  public string SaveXml(FormModel formModel);
  
  public string ValidXsd();

  public string SaveHtml();
}