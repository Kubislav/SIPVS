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
              margin-left: 20%;
              margin-top: 3%;
              margin-bottom: 1%;
              font-size: 2rem
          }

          .header2 {
              margin-left: 20%;
              margin-top: 6%;
              margin-bottom: 1%;
              font-size: 2rem
          }

          .title {
              text-align: center;
              margin-top: 2%;
              margin-bottom: 2%;
          }

          hr {
              width: 80%;
              margin: auto;
          }

          .row {
              width: 700px;
              margin-right: auto;
              margin-left: auto;
              margin-bottom: 10px;
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
              width: 220px;
          }

          .buttons {
              margin-top: 5%;
          }

          .first_button {
              margin-left: 20%;
              width: 70%;
              margin-bottom: 2%;
          }

          .files{
            margin-left: 25%;
              float: left;
              width: 70%;
            margin-bottom: 2%;
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
					<h1>Vypožičanie kníh</h1>
				</div>
				<hr></hr>
				<div class="personal_form">

					<h2>Základné údaje</h2>
					<div class="row">
						<div class="form1">
							<span><b>Meno: </b></span>
							<span><xsl:apply-templates select="tim2:personal_info/tim2:first_name"/></span>
						</div>
						<div class="form2">
							<span><b>Priezvisko: </b></span>
							<span><xsl:apply-templates select="tim2:personal_info/tim2:last_name"/></span>
						</div>
					</div>
					<div class="row">
						<div class="form1">
							<span><b>E-mail: </b></span>
							<span><xsl:apply-templates select="tim2:personal_info/tim2:email_customer"/></span>
						</div>
						<div class="form2">
							<span><b>Dátum narodenia: </b></span>
							<span><xsl:apply-templates select="tim2:personal_info/tim2:date"/></span>
						</div>
					</div>

					<h2>Adresa</h2>
					<div class="row">
						<div class="form1">
							<span><b>Ulica: </b></span>
							<span><xsl:apply-templates select="tim2:address/tim2:street"/></span>
						</div>
						<div class="form2">
							<span><b>Číslo ulice: </b></span>
							<span><xsl:apply-templates select="tim2:address/tim2:adress_number"/></span>
						</div>
					</div>
					<div class="row">
						<div class="form1">
							<span><b>PSČ: </b></span>
							<span><xsl:apply-templates select="tim2:address/tim2:postal_code"/></span>
						</div>
						<div class="form2">
							<span><b>Mesto: </b></span>
							<span><xsl:apply-templates select="tim2:address/tim2:town_customer"/></span>
						</div>
					</div>
					<div class="row">
						<div class="form1">
							<span><b>Krajina: </b></span>
							<span><xsl:apply-templates select="tim2:address/tim2:country"/></span>
						</div>
					</div>

					<h2>Knihy</h2>
					<xsl:for-each select="tim2:books/tim2:book">

						<div class="book-area">
							<div class="row">
								<div class="form1">
									<span><b>Názov: </b></span>
									<span><xsl:apply-templates select="tim2:book_name"/></span>
								</div>
								<div class="form2">
									<span><b>Autor: </b></span>
									<span><xsl:apply-templates select="tim2:book_author"/></span>
								</div>
							</div>
							<div class="row">
								<div class="form1">
									<span><b>Žáner: </b></span>
									<span><xsl:apply-templates select="tim2:book_genre"/></span>
								</div>
								<div class="form2">
									<span><b>Dátum vypožičania: </b></span>
									<span><xsl:apply-templates select="tim2:book_reserve"/></span>
								</div>
							</div>
							<div class="row">
								<div class="form1">
									<span><b>Dátum vrátenia: </b></span>
									<span><xsl:apply-templates select="tim2:book_return"/></span>
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