#pragma checksum "C:\Users\Owner\Downloads\SIPVS-master (1)\SIPVS-master\Pages\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "dba606aa6de7639ce359bb73f92f41c7f278c611"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(SIPVS.Pages.Pages_Index), @"mvc.1.0.razor-page", @"/Pages/Index.cshtml")]
namespace SIPVS.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\Owner\Downloads\SIPVS-master (1)\SIPVS-master\Pages\_ViewImports.cshtml"
using SIPVS;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"dba606aa6de7639ce359bb73f92f41c7f278c611", @"/Pages/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ff1b0c1fe3262982062b255fac16d7c6d302815c", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Index : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "C:\Users\Owner\Downloads\SIPVS-master (1)\SIPVS-master\Pages\Index.cshtml"
  
    ViewData["Title"] = "Home page";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<section class=""personal_info"">
    <div class=""title"">
        <h1>Registračný formulár</h1>
    </div>
    <hr>
    <div class=""personal_form"">
        <h2>Základné údaje</h2>
        <div class=""row"">
            <div class=""form1"">
                <label>Meno</label>
                <input class=""firstName"" />
            </div>
            <div class=""form2"">
                <label>Priezvisko</label>
                <input class=""lastName"" />
            </div>
        </div>
        <div class=""row"">
            <div class=""form1"">
                <label>E-mail</label>
                <input class=""email_customer"" />
            </div>
            <div class=""form2"">
                <label>Dátum narodenia</label>
                <input class=""date"" type=""date"" />
            </div>
        </div>
        <h2>Adresa</h2>
        <div class=""row"">
            <div class=""form1"">
                <label>Ulica</label>
                <input class=""street"" />
            </di");
            WriteLiteral(@"v>
            <div class=""form2"">
                <label>Číslo ulice</label>
                <input class=""adress_number"" />
            </div>
        </div>
        <div class=""row"">
            <div class=""form1"">
                <label>PSČ</label>
                <input class=""postal_Code"" />
            </div>
            <div class=""form2"">
                <label>Mesto</label>
                <input class=""town_customer"" />
            </div>
        </div>
        <div class=""row"">
            <div class=""form1"">
                <label>Krajina</label>
                <input class=""Country"" />
            </div>
        </div>
        <h2 class=""header2"">Kniha</h2>
        <div class=""row"">
            <div class=""form1"">
                <label>Názov</label>
                <input class=""book_name"" />
            </div>
            <div class=""form2"">
                <label>Autor</label>
                <input class=""book_author"" />
            </div>
        </div>
   ");
            WriteLiteral(@"     <div class=""row"">
            <div class=""form1"">
                <label>Žáner</label>
                <input class=""book_genre"" />
            </div>
            <div class=""form2"">
                <label>Dátum vypožičania</label>
                <input class=""book_reserve"" />
            </div>
        </div>
        <div class=""row"">
            <div class=""form1"">
                <label>Dátum vrátenia</label>
                <input class=""book_return"" />
            </div>
        </div>
    </div>
</section>

<div id=""next_book""></div>
<section class=""buttons"">
    <div class=""first_button"">
        <button class=""add_book"" onclick=""myFunction()"">Pridat knihu</button>
        <button class=""save_xml"">Uložiť XML</button>
        <button class=""valid_xsd"">Validácia XSD</button>
        <button class=""save_html"">Uložiť HTML</button>
    </div>
    
</section>



<script>
    function myFunction() {
        
        var btn = document.createElement(""next_book"");
     ");
            WriteLiteral(@"       btn.innerHTML = '<h2 class=""header2"">Kniha</h2><div class=""row""><div class=""form1""><label>Názov</label><input class=""book_name"" /></div><div class=""form2""><label>Autor</label><input class=""book_author"" /></div></div ><div class=""row""><div class=""form1""><label>Žáner</label><input class=""book_genre"" /></div><div class=""form2""><label>Dátum vypožičania</label><input class=""book_reserve"" /></div></div><div class=""row""><div class=""form1""><label>Dátum vrátenia</label><input class=""book_return"" /></div></div>';
        document.getElementById(""next_book"").appendChild(btn);
    }

</script>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IndexModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<IndexModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<IndexModel>)PageContext?.ViewData;
        public IndexModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591
