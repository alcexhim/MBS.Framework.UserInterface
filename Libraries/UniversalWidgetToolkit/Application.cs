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

		public static void Initialize()
		{
			if (mvarEngine == null)
			{
				Engine[] engines = Engine.Get();
				if (engines.Length > 0) mvarEngine = engines[0];

				if (mvarEngine == null) throw new ArgumentNullException("Application.Engine", "No engines were found or could be loaded");
			}

			if (mvarEngine != null)
				mvarEngine.Initialize ();
		}

		static Application()
		{
			Engine[] engines = Engine.Get();
			if (engines.Length > 0) mvarEngine = engines[0];
		}

		// [DebuggerNonUserCode()]
		public static int Start(Window waitForClose = null)
		{
			if (waitForClose != null)
			{
				mvarEngine.CreateControl (waitForClose);
				waitForClose.Show();
			}

			int exitCode = mvarEngine.Start(waitForClose);
			
			mvarExitCode = exitCode;
			OnApplicationExited(EventArgs.Empty);

			return exitCode;
		}

		public static void Stop(int exitCode = 0)
		{
			if (mvarEngine == null)
			{
				Engine[] engines = Engine.Get();
				if (engines.Length > 0) mvarEngine = engines[0];

				if (mvarEngine == null) throw new ArgumentNullException("No engines were found or could be loaded");
			}
			mvarEngine.Stop(exitCode);
		}
    }
}
