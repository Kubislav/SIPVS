<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:tim2="http://www.w3.org">
  <xsl:template match="/tim2:registration">
    <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
    <html xsl:version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
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