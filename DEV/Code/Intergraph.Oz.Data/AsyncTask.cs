using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intergraph.Oz.Data
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	/// <typeparam name="TDelegate"></typeparam>
	public struct AsyncTask<TObject, TDelegate>
	{
		public TObject Object;
		public TDelegate Caller;
		public IAsyncResult AsyncResult;
	}
}
