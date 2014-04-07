<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
					 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 exclude-result-prefixes="msxsl">
  <xsl:include href="./Values.xslt"/>
  <xsl:include href="./Select.xslt"/>
  <xsl:include href="./Insert.xslt"/>
  <xsl:include href="./Update.xslt"/>
  <xsl:include href="./Delete.xslt"/>
  <xsl:include href="./Join.xslt"/>
  <xsl:include href="./Where.xslt"/>
  <xsl:include href="./GroupBy.xslt"/>
  <xsl:include href="./Having.xslt"/>
  <xsl:include href="./OrderBy.xslt"/>
</xsl:stylesheet>