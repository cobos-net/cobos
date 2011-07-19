using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if INTERGRAPH_BRANDING
[assembly: AssemblyTitle( "Intergraph.AsiaPac.Utilities" )]
#else
[assembly: AssemblyTitle( "Cobos.Utilities" )]
#endif
[assembly: AssemblyDescription( "Utility objects to support all applications." )]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration( "Release" )]
#endif
#if INTERGRAPH_BRANDING
[assembly: AssemblyCompany( "Intergraph Corporation, Asia Pacific Region" )]
[assembly: AssemblyProduct( "Intergraph.AsiaPac.Utilities" )]
[assembly: AssemblyCopyright( "Copyright © Intergraph Corporation Pty Ltd 2011" )]
#else
[assembly: AssemblyCompany( "Cobos Softare Development Kit" )]
[assembly: AssemblyProduct( "Cobos.Utilities" )]
[assembly: AssemblyCopyright( "Copyright © Nicholas Davis 2009-2011" )]
#endif
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "5d949c7d-cc9b-42be-bb63-686059efb165" )]

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
[assembly: AssemblyVersion( "0.1.0.0" )]
[assembly: AssemblyFileVersion( "0.1.2.0" )]
