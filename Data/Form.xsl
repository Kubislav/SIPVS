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
              width: 50%;
              height: 20%;
              margin-right: auto;
              margin-left: auto;
              margin-bottom: 1%;
          }

          .form1 {
              width: 20%;
              float: left;
          }

          .form2 {
              width: 20%;
              margin-left: 50%;
          }

          label {
              font-size: 1em;
              font-weight: 900;
          }

          input {
              width: 180%;
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
        <h3 style="text-align: center">Osobné údaje</h3>
        <table style="margin-left:43%">
          <tr>
            <td>Krstné meno:</td>
            <td>
              <xsl:apply-templates select="tim2:personal_info/tim2:first_name"/>
            </td>
          </tr>
          <tr>
            <td>Priezvisko:</td>
            <td>
              <xsl:apply-templates select="tim2:personal_info/tim2:last_name"/>
            </td>
          </tr>
          <tr>
            <td>E-mail:</td>
            <td>
              <xsl:apply-templates select="tim2:personal_info/tim2:email_customer"/>
            </td>
          </tr>
          <tr>
            <td>Dátum narodenia:</td>
            <td>
              <xsl:apply-templates select="tim2:personal_info/tim2:date"/>
            </td>
          </tr>
        </table>
        <h3 style="text-align: center">Adresa</h3>
        <table style="margin-left:43%">
          <tr>
            <td>Ulica:</td>
            <td>
              <xsl:apply-templates select="tim2:address/tim2:street"/>
            </td>
          </tr>
          <tr>
            <td>Číslo ulice:</td>
            <td>
              <xsl:apply-templates select="tim2:address/tim2:adress_number"/>
            </td>
          </tr>
          <tr>
            <td>PSČ:</td>
            <td>
              <xsl:apply-templates select="tim2:address/tim2:postal_code"/>
            </td>
          </tr>
          <tr>
            <td>Mesto:</td>
            <td>
              <xsl:apply-templates select="tim2:address/tim2:town_customer"/>
            </td>
          </tr>
          <tr>
            <td>Krajina:</td>
            <td>
              <xsl:apply-templates select="tim2:address/tim2:country"/>
            </td>
          </tr>
        </table>
        <h3 style="text-align: center">Knihy</h3>
        <xsl:for-each select="tim2:books/tim2:book">
          <table style="margin-left:43%">
            <tr>
              <td>Meno knihy:</td>
              <td>
                <xsl:apply-templates select="tim2:book_name"/>
              </td>
            </tr>
            <tr>
              <td>Autor knihy:</td>
              <td>
                <xsl:apply-templates select="tim2:book_author"/>
              </td>
            </tr>
            <tr>
              <td>Žáner knihy:</td>
              <td>
                <xsl:apply-templates select="tim2:book_genre"/>
              </td>
            </tr>
            <tr>
              <td>Rezervované od:</td>
              <td>
                <xsl:apply-templates select="tim2:book_reserve"/>
              </td>
            </tr>
            <tr>
              <td>Rezervované do:</td>
              <td>
                <xsl:apply-templates select="tim2:book_return"/>
              </td>
            </tr>
          </table>
          <br/>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>