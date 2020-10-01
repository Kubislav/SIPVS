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

        // [HttpPost("save_xml")]
        // public IActionResult saveXml(DataModel data)
        // { 
        //     System.Console.WriteLine("data from post request: " + data.data);
        //     FormModel formModel = new FormModel();
        //     formModel.FullName = "Florian";
        //     return Ok(this._formService.SaveXml(formModel));
        // }

        [HttpGet("save_xml")]
        public IActionResult saveXml(string xml)
        { 
            return Ok(this._formService.SaveXml(xml));
        }

        [HttpGet("valid_xsd")]
        public IActionResult validXsd()
        { 
            return Ok(this._formService.ValidXsd());
        }

        [HttpGet("save_html")]
        public IActionResult saveHtml()
        { 
            return Ok(this._formService.SaveHtml());
        }
    }
}