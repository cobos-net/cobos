using System;
using System.Collections.Generic;
using Cobos.Script;

[ScriptClass]
public class DemoScript : IDisposable
{
	#region Setup/Teardown

	public DemoScript()
	{
		Logger.Instance.Information( "Initialising DemoScript" );
	}

	public void Dispose()
	{
        Logger.Instance.Information( "Disposing DemoScript" );
	}

	#endregion

	[ScriptMethod]
	public void DemoMethod()
	{
		Logger.Instance.Information( "Calling DemoMethod" );
	}
}