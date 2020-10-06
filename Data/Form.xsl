<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:tim2="http://www.w3.org">
  <xsl:template match="/tim2:registration">
    <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
    <html xsl:version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
      <head>
        <style type="text/css">
          .footer {
            position: absolute;
            bottom: 0;
            width: 100%;
            white-space: nowrap;
            line-height: 60px; /* Vertically center the text there */
          }


          * {
              margin: 0;
              padding: 0;
              background-color: #ADD8E6;
          }

          h1 {
              font-size: 3rem
          }

          h2 {
              margin-top: 40px;
              margin-bottom: 20px;
              font-size: 2rem
          }

          .header2 {
              margin-top: 100px;
              margin-bottom: 20px;
              font-size: 2rem
          }

          .title {
              width: 700px;
              margin: 45px auto;
          }

          hr {
              width: 80%;
              margin: auto;
          }

          .row {
              width: 700px;
              margin-right: auto;
              margin-left: auto;
              margin-bottom: 15px;
          }
		  
		 .h2-row {
              width: 800px;
              margin-right: auto;
              margin-left: auto;
          }

          .form1 {
              float: left;
          }

          .form2 {
              margin-left: 50%;
          }

          label {
              font-size: 1em;
              font-weight: 900;
			  display: block;
          }

          input {
              width: 250px;
          }

          .buttons {
              margin-top: 100px;
          }

          .first_button {
              width: 700px;
              margin-right: auto;
              margin-left: auto;
          }

          .files{
				margin-left: auto;
				margin-right: auto;
				width: 700px;
				margin-bottom: 70px;
				margin-top: 30px;
          }

          .xml_file, .xsd_file, .xsl_file{
            width: 30%;
          }

		  .book-area{
              margin-bottom: 40px;
          }

          button {
              border-radius: 4px;
              padding: 14px 40px;
              font-size: 14px;
              background-color: #C0C0C0;
          }

          button:hover {
              background-color: #4CAF50;
              color: white;
          }
        </style>
      </head>
<body>
		<div class="page">
			<section class="personal_info">
				<div class="title">
					<h1>Vypožičanie kníh - záznam</h1>
				</div>
				<hr></hr>
				<div class="personal_form">

					<div class="h2-row"><h2>Základné údaje</h2></div>
					<div class="row">
						<div class="form1">
              <label>Meno</label>
              <input type="text" value="{tim2:personal_info/tim2:first_name}" readonly=""/> 
						</div>
						<div class="form2">
              <label>Priezvisko</label>
              <input type="text" value="{tim2:personal_info/tim2:last_name}" readonly=""/> 
						</div>
					</div>
					<div class="row">
						<div class="form1">
              <label>E-mail</label>
              <input type="text" value="{tim2:personal_info/tim2:email_customer}" readonly=""/> 
						</div>
						<div class="form2">
              <label>Dátum narodenia</label>
              <input type="date" value="{tim2:personal_info/tim2:date}" readonly=""/> 
						</div>
					</div>

					<div class="h2-row"><h2>Adresa</h2></div>
					<div class="row">
						<div class="form1">
              <label>Ulica</label>
              <input type="text" value="{tim2:address/tim2:street}" readonly=""/> 
						</div>
						<div class="form2">
							<label>Číslo ulice</label>
              <input type="text" value="{tim2:address/tim2:adress_number}" readonly=""/> 
						</div>
					</div>
					<div class="row">
						<div class="form1">
							<label>PSČ</label>
              <input type="text" value="{tim2:address/tim2:postal_code}" readonly=""/> 
						</div>
						<div class="form2">
							<label>Mesto</label>
              <input type="text" value="{tim2:address/tim2:town_customer}" readonly=""/> 
						</div>
					</div>
					<div class="row">
						<div class="form1">
							<label>Krajina</label>
              <input type="text" value="{tim2:address/tim2:country}" readonly=""/> 
						</div>
					</div>

					<xsl:for-each select="tim2:books/tim2:book">
          <div class="h2-row"><h2 class="header2">Kniha</h2></div>
						<div class="book-area">
							<div class="row">
								<div class="form1">
                  <label>Názov</label>
                  <input type="text" value="{tim2:book_name}" readonly=""/> 
								</div>
								<div class="form2">
									<label>Autor</label>
                  <input type="text" value="{tim2:book_author}" readonly=""/> 
								</div>
							</div>
							<div class="row">
								<div class="form1">
									<label>Žáner</label>
                  <input type="text" value="{tim2:book_genre}" readonly=""/> 
								</div>
								<div class="form2">
									<label>Dátum vypožičania</label>
                  <input type="date" value="{tim2:book_reserve}" readonly=""/> 
								</div>
							</div>
							<div class="row">
								<div class="form1">
									<label>Dátum vrátenia</label>
                  <input type="date" value="{tim2:book_return}" readonly=""/> 
								</div>
							</div>
							<br/>
						</div>
					</xsl:for-each>
				</div>
			</section>
		</div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>