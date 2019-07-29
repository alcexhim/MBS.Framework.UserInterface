﻿using System;
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

		public static DefaultSettingsProvider DefaultSettingsProvider { get; } = new DefaultSettingsProvider();
		public static SettingsProvider.SettingsProviderCollection SettingsProviders { get; } = new SettingsProvider.SettingsProviderCollection();

		private static int mvarExitCode = 0;
		public static int ExitCode { get { return mvarExitCode; } }

		public static Guid ID { get; set; } = Guid.Empty;
		public static string UniqueName { get; set; } = null;
		public static string ShortName { get; set; }

		public static event EventHandler Startup;
		public static event EventHandler Shutdown;

		public static Command.CommandCollection Commands { get; } = new Command.CommandCollection();
		public static bool AttachCommandEventHandler(string commandID, EventHandler handler)
		{
			Command cmd = Commands[commandID];
			if (cmd != null)
			{
				cmd.Executed += handler;
				return true;
			}
			Console.WriteLine("attempted to attach handler for unknown command '" + commandID + "'");
			return false;
		}
		public static void ExecuteCommand(string id)
		{
			Command cmd = Commands [id];
			if (cmd == null)
				return;
			
			cmd.Execute ();
		}
		
		private static string[] EnumerateDataPaths()
		{
			string basePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			return new string[]
			{
				// first look in the application root directory since this will be overridden by everything else
				basePath,
				// then look in /usr/share/universal-editor or C:\ProgramData\Mike Becker's Software\Universal Editor
				String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData),
					Application.ShortName
				}),
				// then look in ~/.local/share/universal-editor or C:\Users\USERNAME\AppData\Local\Mike Becker's Software\Universal Editor
				String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
					Application.ShortName
				}),
				// then look in ~/.universal-editor or C:\Users\USERNAME\AppData\Roaming\Mike Becker's Software\Universal Editor
				String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
					Application.ShortName
				})
			};
		}

		public static string ExpandRelativePath(string relativePath)
		{
			if (relativePath.StartsWith("~/"))
			{
				string[] potentialFileNames = EnumerateDataPaths();
				for (int i = potentialFileNames.Length - 1; i >= 0; i--)
				{
					potentialFileNames[i] = potentialFileNames[i] + '/' + relativePath.Substring(2);
					Console.WriteLine("Looking for " + potentialFileNames[i]);

					if (System.IO.File.Exists(potentialFileNames[i]))
					{
						Console.WriteLine("Using " + potentialFileNames[i]);
						return potentialFileNames[i];
					}
				}
			}
			if (System.IO.File.Exists(relativePath))
			{
				return relativePath;
			}
			return null;
		}

		public static event EventHandler ApplicationExited;

		private static void OnApplicationExited(EventArgs e)
		{
			if (ApplicationExited != null) ApplicationExited(null, e);
		}

        [DebuggerNonUserCode()]
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

			// after initialization, load option providers

			List<SettingsProvider> listOptionProviders = new List<SettingsProvider>();
			System.Collections.Specialized.StringCollection listOptionProviderTypeNames = new System.Collections.Specialized.StringCollection ();

			// load the already-known list
			foreach (SettingsProvider provider in Application.SettingsProviders) {
				listOptionProviders.Add (provider);
				listOptionProviderTypeNames.Add (provider.GetType ().FullName);
			}


			System.Reflection.Assembly[] asms = UniversalEditor.Common.Reflection.GetAvailableAssemblies();
			foreach (System.Reflection.Assembly asm in asms) {
				Type[] types = null;
				try {
					types = asm.GetTypes ();
				} catch (System.Reflection.ReflectionTypeLoadException ex) {
					Console.Error.WriteLine ("ReflectionTypeLoadException(" + ex.LoaderExceptions.Length.ToString () + "): " + asm.FullName);
					Console.Error.WriteLine (ex.Message);

					types = ex.Types;
				}

				foreach (Type type in types) {
					if (type.IsSubclassOf (typeof(SettingsProvider))) {
						if (!listOptionProviderTypeNames.Contains (type.FullName)) {
							try {
								SettingsProvider provider = (type.Assembly.CreateInstance (type.FullName) as SettingsProvider);
								if (provider == null) {
									Console.Error.WriteLine ("ue: reflection: couldn't load OptionProvider '{0}'", type.FullName);
									continue;
								}
								listOptionProviderTypeNames.Add (type.FullName);
								listOptionProviders.Add (provider);
								Console.WriteLine ("loaded option provider \"{0}\"", type.FullName);
							} catch (System.Reflection.TargetInvocationException ex) {
								Console.WriteLine ("binding error: " + ex.InnerException.Message);
							} catch (Exception ex) {
								Console.WriteLine ("error while loading editor '" + type.FullName + "': " + ex.Message);
							}
						} else {
							Console.WriteLine ("skipping already loaded OptionProvider '{0}'", type.FullName);
						}
					}
				}
			}

			foreach (SettingsProvider provider in listOptionProviders) {
				Application.SettingsProviders.Add (provider);
			}
		}

		static Application()
		{
			Engine[] engines = Engine.Get();
			if (engines.Length > 0) mvarEngine = engines[0];
			
			string sv = System.Reflection.Assembly.GetEntryAssembly().Location;
			if (sv.StartsWith("/")) sv = sv.Substring(1);
			
			sv = sv.Replace(".", "_");
			sv = sv.Replace("\\", ".");
			sv = sv.Replace("/", ".");
			
			// ID = Guid.NewGuid();
			// sv = sv + ID.ToString().Replace("-", String.Empty);
			UniqueName = sv;

			// configure UWT-provided settings
			Application.SettingsProviders.Add(Application.DefaultSettingsProvider);
		}

		// [DebuggerNonUserCode()]
		public static int Start(Window waitForClose = null)
		{
			Console.WriteLine("Application_Start");
			if (waitForClose != null)
			{
				if (mvarEngine.IsControlDisposed(waitForClose))
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

		public static void DoEvents()
		{
			mvarEngine?.DoEvents();
		}
    }
}
