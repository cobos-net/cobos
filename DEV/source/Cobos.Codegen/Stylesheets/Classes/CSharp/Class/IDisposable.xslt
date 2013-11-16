<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
					 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
					 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
					 xmlns:cobos="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns="http://schemas.cobos.co.uk/datamodel/1.0.0"
					 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
					 exclude-result-prefixes="msxsl">
  <!-- 
	=============================================================================
	Filename: .xslt
	Description: Implement the IDisposable pattern.
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	Notes: 
	
	
	============================================================================
	-->
  <!--
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	IDisposable pattern 
	~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	-->
  <xsl:template match="cobos:Object" mode="classIDisposable">
    <![CDATA[/// <summary>
    /// Implements IDisposable.
    /// </summary>]]>
    public partial class <xsl:value-of select="@name"/> : IDisposable
    {
        <![CDATA[/// <summary>
        /// Indicates whether the instance has been disposed.
        /// </summary>]]>
        private bool disposed = false;

        <![CDATA[/// <summary>
        /// Finalizes an instance of the <see cref="]]><xsl:value-of select="@name"/><![CDATA["/> class.
        /// </summary>]]>
        ~<xsl:value-of select="@name"/>()
        {
            this.Dispose(false);
        }

        <![CDATA[/// <summary>
        /// Gets a value indicating whether the instance has been disposed.
        /// </summary>]]>
        public bool IsDisposed
        {
            get { return this.disposed; }
        }

        <![CDATA[/// <summary>
        /// Dispose the current instance.
        /// </summary>]]>
        public void Dispose()
        {
            this.Dispose(true);
        }

        <![CDATA[/// <summary>
        /// Dispose the current instance.
        /// </summary>
        /// <param name="disposing">true if the instance is disposing; otherwise false if the instance is finalizing.</param>]]>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            this.disposed = true;
        }
    }
  </xsl:template>
</xsl:stylesheet>