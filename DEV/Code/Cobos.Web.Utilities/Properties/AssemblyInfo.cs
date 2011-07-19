using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if INTERGRAPH_BRANDING
[assembly: AssemblyTitle( "Intergraph.AsiaPac.Web.Utilities" )]
#else
[assembly: AssemblyTitle( "Cobos.Web.Utilities" )]
#endif
[assembly: AssemblyDescription( "Utility objects to support web applications and services." )]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration( "Release" )]
#endif
#if INTERGRAPH_BRANDING
[assembly: AssemblyCompany( "Intergraph Corporation, Asia Pacific Region" )]
[assembly: AssemblyProduct( "Intergraph.AsiaPac.Web.Utilities" )]
[assembly: AssemblyCopyright( "Copyright © Intergraph Corporation Pty Ltd 2011" )]
#else
[assembly: AssemblyCompany( "Cobos Development Kit" )]
[assembly: AssemblyProduct( "Cobos.Web.Utilities" )]
[assembly: AssemblyCopyright( "Copyright © Nicholas Davis 2009-2011" )]
#endif
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "1e5fe5d5-38d9-4bd0-b4de-f6b9327540be" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion( "0.1.1.0" )]
[assembly: AssemblyFileVersion( "0.1.1.0" )]
