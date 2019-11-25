using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MBS.Framework.UserInterface
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

		/// <summary>
		/// The event that is called the first time an applicati
		/// </summary>
		public static event EventHandler Startup;
		private static void OnStartup(EventArgs e)
		{
			Startup?.Invoke(typeof(Application), e);
		}

		public static event ApplicationActivatedEventHandler Activated;
		private static void OnActivated(ApplicationActivatedEventArgs e)
		{
			Activated?.Invoke(typeof(Application), e);
		}

		public static event EventHandler Shutdown;

		/// <summary>
		/// Gets a collection of <see cref="Context" /> objects representing system, application, user, and custom contexts for settings and other items.
		/// </summary>
		/// <value>A collection of <see cref="Context" /> objects representing contexts for settings and other items.</value>
		public static Context.ContextCollection Contexts { get; } = new Context.ContextCollection();

		private static void Application_MenuBar_Item_Click(object sender, EventArgs e)
		{
			CommandMenuItem mi = (sender as CommandMenuItem);
			if (mi == null)
				return;

			Command cmd = Application.Commands[mi.Name];
			if (cmd == null)
			{
				Console.WriteLine("unknown cmd '" + mi.Name + "'");
				return;
			}

			cmd.Execute();
		}

		private static List<Window> _windows = new List<Window>();
		private static System.Collections.ObjectModel.ReadOnlyCollection<Window> _windowsRO = null;
		public static System.Collections.ObjectModel.ReadOnlyCollection<Window> Windows
		{
			get
			{
				if (_windowsRO == null)
				{
					_windowsRO = new System.Collections.ObjectModel.ReadOnlyCollection<Window>(_windows);
				}
				return _windowsRO;
			}
		}
		internal static void AddWindow(Window window)
		{
			_windows.Add(window);
		}

		public static ContextChangedEventHandler ContextAdded;
		private static void OnContextAdded(ContextChangedEventArgs e)
		{
			ContextAdded?.Invoke(typeof(Application), e);
		}

		public static ContextChangedEventHandler ContextRemoved;
		private static void OnContextRemoved(ContextChangedEventArgs e)
		{
			ContextRemoved?.Invoke(typeof(Application), e);
		}

		private static Dictionary<Context, List<MenuItem>> _listContextMenuItems = new Dictionary<Context, List<MenuItem>>();

		/// <summary>
		/// Handles updating the menus, toolbars, keyboard shortcuts, and other UI elements associated with the application <see cref="Context" />.
		/// </summary>
		internal static void AddContext(Context ctx)
		{
			foreach (Command cmd in ctx.Commands)
			{
				Application.Commands.Add(cmd);
			}

			if (!_listContextMenuItems.ContainsKey(ctx))
			{
				_listContextMenuItems[ctx] = new List<MenuItem>();
			}

			foreach (CommandItem ci in ctx.MenuItems)
			{
				MenuItem mi = MenuItem.LoadMenuItem(ci, Application_MenuBar_Item_Click);
				_listContextMenuItems[ctx].Add(mi);
				foreach (Window w in Application.Windows)
				{
					w.MenuBar.Items.Add(mi);
				}
			}

			OnContextAdded(new ContextChangedEventArgs(ctx));
		}
		/// <summary>
		/// Handles updating the menus, toolbars, keyboard shortcuts, and other UI elements associated with the application <see cref="Context" />.
		/// </summary>
		internal static void RemoveContext(Context ctx)
		{
			if (_listContextMenuItems.ContainsKey(ctx))
			{
				foreach (Window w in Application.Windows)
				{
					foreach (MenuItem mi in _listContextMenuItems[ctx])
					{
						w.MenuBar.Items.Remove(mi);
					}
				}
			}
			_listContextMenuItems[ctx].Clear();

			foreach (Command cmd in ctx.Commands)
			{
				Application.Commands.Remove(cmd);
			}
		}


		private static Dictionary<string, List<EventHandler>> _CommandEventHandlers = new Dictionary<string, List<EventHandler>>();

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

			// handle command event handlers attached without a Command instance
			if (!_CommandEventHandlers.ContainsKey(commandID))
			{
				_CommandEventHandlers.Add(commandID, new List<EventHandler>());
			}
			if (!_CommandEventHandlers[commandID].Contains(handler))
			{
				_CommandEventHandlers[commandID].Add(handler);
			}
			return false;
		}
		public static void ExecuteCommand(string id)
		{
			Command cmd = Commands[id];

			// handle command event handlers attached without a Command instance
			if (_CommandEventHandlers.ContainsKey(id))
			{
				List<EventHandler> c = _CommandEventHandlers[id];
				for (int i = 0;  i < c.Count; i++)
				{
					c[i](cmd, EventArgs.Empty);
				}
				return;
			}

			// handle command event handlers attached in a context, most recently added first
			for (int i = Contexts.Count - 1; i >= 0; i--)
			{
				if (Contexts[i].ExecuteCommand(id))
					return;
			}

			if (cmd == null)
				return;

			cmd.Execute ();
		}
		
		public static string[] EnumerateDataPaths()
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
			foreach (SettingsProvider provider in Application.SettingsProviders) {
				provider.SaveSettings ();
			}

			if (ApplicationExited != null) ApplicationExited(null, e);
		}

        // [DebuggerNonUserCode()]
		public static void Initialize()
		{
			if (mvarEngine == null)
			{
				Engine[] engines = Engine.Get();
				if (engines.Length > 0) mvarEngine = engines[0];

				if (mvarEngine == null) throw new ArgumentNullException("Application.Engine", "No engines were found or could be loaded");
			}

			if (mvarEngine != null)
			{
				Console.WriteLine("Using engine {0}", mvarEngine.GetType().FullName);
				mvarEngine.Initialize();
			}

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
					if (type == null)
						continue;

					if (type.IsSubclassOf (typeof(SettingsProvider)) && !type.IsAbstract) {
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
				if (provider is ApplicationSettingsProvider) {
					Application.SettingsProviders.Add (provider);
					provider.LoadSettings ();
				}
			}

			Plugin[] plugins = Plugin.Get();
			for (int i = 0; i < plugins.Length; i++)
			{
				Console.WriteLine("initializing plugin '{0}'", plugins[i].GetType().FullName);
				plugins[i].Initialize();

				if (plugins[i].Context != null)
					Application.Contexts.Add(plugins[i].Context);
			}
		}

		static Application()
		{
			Type tKnownContexts = typeof(KnownContexts);
			System.Reflection.PropertyInfo[] pis = tKnownContexts.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			for (int i = 0; i < pis.Length; i++)
			{
				Context ctx = (Context)pis[i].GetValue(null, null);
				Application.Contexts.Add(ctx);
			}

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
				return; // why bother getting an engine? we're stopping...

			mvarEngine.Stop(exitCode);
		}

		public static void DoEvents()
		{
			mvarEngine?.DoEvents();
		}

		public static T GetSetting<T>(string name, T defaultValue = default(T))
		{
			try 
			{
				object value = GetSetting(name);
				if (value == null) {
					return defaultValue;
				}
				return (T)value;
			}
			catch {
				return defaultValue;
			}
		}
		public static void SetSetting<T>(string name, T value)
		{
			SetSetting (name, (object)value);
		}

		public static SettingsGroup FindSettingGroup(string name, out string realName, out string groupPath)
		{
			string[] namePath = name.Split (new char[] { ':' });
			realName = namePath [namePath.Length - 1];
			groupPath = String.Join (":", namePath, 0, namePath.Length - 1);

			foreach (SettingsProvider provider in SettingsProviders) {
				foreach (SettingsGroup group in provider.SettingsGroups) {
					string path = String.Join (":", group.Path);
					path = path.Replace (' ', '_');
					if (path.Equals (groupPath)) {
						return group;
					}
				}
			}

			realName = null;
			groupPath = null;
			return null;
		}

		public static object GetSetting(string name)
		{
			string realName = null;
			string groupPath = null;

			SettingsGroup group = FindSettingGroup (name, out realName, out groupPath);
			if (group == null)
				return null;
			if (group.Settings [realName] != null) {
				return group.Settings [realName].GetValue ();
			}
			return null;
		}
		public static void SetSetting(string name, object value)
		{
			string realName = null;
			string groupPath = null;

			SettingsGroup group = FindSettingGroup (name, out realName, out groupPath);
			if (group == null)
				return;
			if (group.Settings [realName] != null) {
				group.Settings [realName].SetValue (value);
			}
		}
    }
}
