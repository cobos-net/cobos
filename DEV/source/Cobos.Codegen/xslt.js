var adTypeBinary = 1;
var adSaveCreateOverWrite = 2;
var adSaveCreateNotExist = 1;

try
{
   var args = WScript.Arguments;

   if (args.length < 3)
   {
      WScript.Echo("Usage: xsl.js file.xml file.xsl output.txt mode:name param=value ...");
      WScript.Quit(1);
   }
   else
   {
      var xml = args(0);
      var xsl = args(1);
      var out = args(2);

      // parse the rest of the command line for optional statup mode and parameters
      var regMode = /mode\:(.*)/i;
      var regParam = /(.*)=(.*)/

      var mode = null;
      var params = [];
      var result;
      
      for ( var a = 3; a < args.length; ++a )
      {
         result = regMode.exec( args( a ) );
         if ( result != null )
         {
            mode = result[ 1 ];
            WScript.Echo( "Xslt mode: " +  mode );
            continue;
         }         
         
         result = regParam.exec( args( a ) );
         if ( result != null )
         {
            WScript.Echo( "Xslt Parameter: " + result[ 1 ] + " = " + result[ 2 ] );
            params.push( { name: result[ 1 ], value: result[ 2 ] } );
            continue;
         }         
      }
      
      // NOTE: there appears to be a bug in MSXML 6.0 pre SP3:
      //
      // xmlTemplate.stylesheet = xslDoc; // fails in 6.0
      //
      // MSXML 6.0 SP3 is only widely available in Windows 7. 
      // Windows XP SP3 and Vista include SP2 only.  
      // The fix is to use MSXML 3.0 objects.

      //var xmlDoc = new ActiveXObject("Msxml2.DOMDocument.6.0");
      var xmlDoc = new ActiveXObject("Msxml2.DOMDocument.3.0");
      xmlDoc.async = false;

      //var xslDoc = new ActiveXObject("Msxml2.DOMDocument.6.0");
      var xslDoc = new ActiveXObject("Msxml2.FreeThreadedDOMDocument.3.0");
      xslDoc.async = false;
      
      /* Create a binary IStream */
      var outDoc = new ActiveXObject("ADODB.Stream");
      outDoc.type = adTypeBinary;
      outDoc.open();

      if (xmlDoc.load(xml) == false)
      {
         throw new Error("Could not load XML document (" + xml + "): " + xmlDoc.parseError.reason);
      }

      if (xslDoc.load(xsl) == false)
      {
         throw new Error("Could not load XSL document (" + xsl + "): "  + xslDoc.parseError.reason);
      }
      
      //var xmlTemplate = new ActiveXObject( "Msxml2.XSLTemplate.6.0" );
      var xmlTemplate = new ActiveXObject( "Msxml2.XSLTemplate.3.0" );
      xmlTemplate.stylesheet = xslDoc; // fails in 6.0

      var xsltProcessor = xmlTemplate.createProcessor();
      xsltProcessor.input = xmlDoc; 
      xsltProcessor.output = outDoc;

      if ( mode != null )
      {
         xsltProcessor.setStartMode( mode );
      }
      
      for ( var p = 0; p < params.length; ++p )
      {
         xsltProcessor.addParameter( params[ p ].name, params[ p ].value );
      }

      xsltProcessor.transform();

      //xmlDoc.transformNodeToObject(xslDoc, outDoc);
      outDoc.SaveToFile(out, adSaveCreateOverWrite);
      
      WScript.Echo("Saved output file: " + out);
   }
}
catch (e)
{
   WScript.Echo(e.message);
   WScript.Quit(1);
}