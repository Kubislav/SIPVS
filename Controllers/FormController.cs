using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace SIPVS {
    [ApiController]
    [Route("api")]

    public class FormController: ControllerBase
    {
        private IFormService _formService;
        public FormController(IFormService formService) {
            this._formService = formService;
        }

        [HttpGet("save_xml")]
        public IActionResult saveXml(string xml)
        { 
            return Ok(this._formService.SaveXml(xml));
        }

        [HttpGet("valid_xsd")]
        public IActionResult validXsd(string xml_file, string xsd_file)
        { 
            return Ok(this._formService.ValidXsd(xml_file, xsd_file));
        }

        [HttpGet("save_html")]
        public IActionResult saveHtml(string xml_file, string xsl_file)
        { 
            return Ok(this._formService.SaveHtml(xml_file, xsl_file));
        }
		
		[HttpGet("sign")]
        public IActionResult signDocument(string xml_file, string xsl_file, string xsd_file)
        {
            return Ok(this._formService.SignDocument(xml_file, xsl_file, xsd_file));
        }

        [HttpPost("xades")]
        public IActionResult saveXades([FromBody]FormModel formModel)
        { 
            return Ok(this._formService.saveXades(formModel.data));
        }

        [HttpGet("make_stamp")]
        public IActionResult makeStamp(string xades_file)
        { 
            return Ok(this._formService.MakeStamp(xades_file));
        }
    }
}