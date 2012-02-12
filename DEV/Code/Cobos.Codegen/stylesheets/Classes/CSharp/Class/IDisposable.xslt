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
		#region IDisposable

		~<xsl:value-of select="@name"/>()
		{
			Dispose( false );
		}
		
		public void Dispose()
		{
			Dispose( true );
		}
		
		public void Dispose( bool disposing )
		{
			if ( _disposed )
			{
				return;
			}
			
			if ( disposing )
			{
				ObjectDataRow.Delete();
				
				GC.SuppressFinalize( this );
			}
			
			_disposed = true;
		}
		
		bool _disposed = false;
		
		public bool IsDisposed
		{
			get { return _disposed; }
		}
		
		#endregion
	</xsl:template>
					 
</xsl:stylesheet>