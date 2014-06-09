<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:i="http://www.cobos.co.uk/build/ignore"
                exclude-result-prefixes="msxsl i"
>
	<!--
	============================================================================
	Filename: heat-ingore-template.xslt
	Description: Custom ignore defintions.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis             Date: 2012-09-04
	Updated by:                     Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: 

	Exclude directories and files from the heat harvest.
	Add the following elements:

	<Dir>...</Dir>    Directory names to exclude.
	<File>...</File>  File names to exclude.
	<Ext>...</Ext>    File extensions to exclude.

	Names and extensions must be supplied in *lower case*.  String comparisons 
	are *case sensitive*.  All folder and file names are converted lower case 
	before comparison.

	The definitions here are merged with the definitions in heat-ignore.xslt.
	============================================================================
	-->

	<!-- Include the base stylesheet containing all of the processing rules -->
	<xsl:include href="\QualitySystem\BUILD\INSTALL\Stylesheets\heat-ignore-custom.xslt"/>

	<xsl:variable name="ignore-node-set">
		<Ignore xmlns="http://www.cobos.co.uk/build/ignore">
			<!-- 
			~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			Directories 
			~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			-->
			<!-- <Dir>any_directory_name</Dir> -->
			<!-- 
			~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			Files 
			~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			-->
			<!-- <File>any_file_name</Dir> -->
			<!-- 
			~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			File extensions
			~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			-->
			<!-- <Ext>xyz</Ext> -->
		</Ignore>
	</xsl:variable>

</xsl:stylesheet>
