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
	Description: 
	============================================================================
	Created by: N.Davis                        Date: 2010-04-09
	Modified by:                               Date:
	============================================================================
	Notes: 
	
	
	============================================================================
	-->
  <!--
	============================================================================
	AcceptChanges method body: Commit all changes to the database
	============================================================================
	
	

	
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="acceptChangesMethodBody">
        /// &lt;summary&gt;
        /// Commit all changes to the database.
        /// &lt;/summary&gt;
        public void AcceptChanges()
        {
            if (this.HasChanges() == false)
            {
                return;
            }
            
            var factory = this.ProviderFactory;
        
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = this.ConnectionString;

                var command = factory.CreateCommand();
                command.CommandText = SelectTemplate.ToString();
                command.Connection = connection;

                var adapter = factory.CreateDataAdapter();
                adapter.SelectCommand = command;

                var builder = factory.CreateCommandBuilder();
                builder.DataAdapter = adapter;

                adapter.InsertCommand = builder.GetInsertCommand();
                adapter.UpdateCommand = builder.GetUpdateCommand();
                adapter.DeleteCommand = builder.GetDeleteCommand();

                adapter.Update(this.Table);
            }
        }
  </xsl:template>
  <!--
	============================================================================
	HasChanges method body: Check whether this has been modified.
	============================================================================
	
	

	
	============================================================================
	-->
  <xsl:template match="cobos:Object" mode="hasChangesMethodBody">
        /// &lt;summary&gt;
        /// Check whether this has changes.
        /// &lt;/summary&gt;
        /// &lt;returns&gt;true if the table has changes; otherwise false.&lt;/returns&gt;
        public bool HasChanges()
        {
            return this.Table.GetChanges() != null;
        }
  </xsl:template>
  <!--
  ============================================================================
  RejectChanges method body: Undo all in-memory changes.
  ============================================================================
	
	

	
  ============================================================================
  -->
  <xsl:template match="cobos:Object" mode="rejectChangesMethodBody">
        /// &lt;summary&gt;
        /// Reject all in-memory changes.
        /// &lt;/summary&gt;
        public void RejectChanges()
        {
            this.Table.RejectChanges();
        }
  </xsl:template>
</xsl:stylesheet>