using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit
{
    public static class Application
    {
		private static Engine mvarEngine = null;
		public static Engine Engine { get { return mvarEngine; } }

		private static int mvarExitCode = 0;
		public static int ExitCode { get { return mvarExitCode; } }

		public static event EventHandler ApplicationExited;

		private static void OnApplicationExited(EventArgs e)
		{
			if (ApplicationExited != null) ApplicationExited(null, e);
		}

		static Application()
		{
			Engine[] engines = Engine.Get();
			if (engines.Length > 0) mvarEngine = engines[0];
		}

		// [DebuggerNonUserCode()]
		public static void Start()
		{
			if (mvarEngine == null) throw new ArgumentNullException("No engines were found or could be loaded");

			int exitCode = mvarEngine.Start();
			
			mvarExitCode = exitCode;
			OnApplicationExited(EventArgs.Empty);
		}

		public static void Stop()
		{
			Engine[] engines = Engine.Get();
			if (engines.Length > 0) mvarEngine = engines[0];

			if (mvarEngine == null) throw new ArgumentNullException("No engines were found or could be loaded");

			mvarEngine.Stop();
		}
    }
}
