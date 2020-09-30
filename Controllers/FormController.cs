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

        // [HttpPost]
        // public void Create([FromBody]Transaction transaction)
        // { 
        //     this._transactionService.SaveTransaction(transaction);
        // }

        [HttpGet("save_xml")]
        public IActionResult saveXml()
        { 
            FormModel formModel = new FormModel();
            formModel.FullName = "Florian";
            return Ok(this._formService.SaveXml(formModel));
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