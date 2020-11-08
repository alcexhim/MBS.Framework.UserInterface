﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MBS.Framework.UserInterface;
using MBS.Framework.UserInterface.Dialogs;
using UniversalEditor;
using UniversalEditor.Accessors;
using UniversalEditor.DataFormats.Markup.XML;
using UniversalEditor.ObjectModels.Markup;
using UniversalEditor.ObjectModels.PropertyList;

namespace MBS.Framework.UserInterface
{
    public class UIApplication : Application
    {
		private Engine mvarEngine = null;
		public Engine Engine { get { return mvarEngine; } }

		public Feature.FeatureCollection Features { get; } = new Feature.FeatureCollection();

		public SettingsProvider.SettingsProviderCollection SettingsProviders { get; } = new SettingsProvider.SettingsProviderCollection();

		public bool Exited { get; internal set; } = false;

		public SettingsProfile.SettingsProfileCollection SettingsProfiles { get; } = new SettingsProfile.SettingsProfileCollection();

		public DpiAwareness DpiAwareness { get; set; } = DpiAwareness.Default;
		internal bool ShouldDpiScale
		{
			// TODO: implement other forms of DpiAwareness
			get { return false; } // DpiAwareness == DpiAwareness.Default && Application.DpiAwareness == DpiAwareness.Default && System.Environment.OSVersion.Platform == PlatformID.Unix; }
		}

		private string mvarBasePath = null;
		public string BasePath
		{
			get
			{
				if (mvarBasePath == null)
				{
					// Set up the base path for the current application. Should this be able to be
					// overridden with a switch (/basepath:...) ?
					mvarBasePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				}
				return mvarBasePath;
			}
		}

		private string mvarDataPath = null;
		public string DataPath
		{
			get
			{
				if (mvarDataPath == null)
				{
					mvarDataPath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
					{
						// The directory that serves as a common repository for application-specific data for the current roaming user.
						Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
						ShortName
					});
				}
				return mvarDataPath;
			}
		}

		public string[] EnumerateDataPaths()
		{
			return new string[]
			{
				// first look in the application root directory since this will be overridden by everything else
				BasePath,
				// then look in /usr/share/universal-editor or C:\ProgramData\Mike Becker's Software\Universal Editor
				String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData),
					ShortName
				}),
				// then look in ~/.local/share/universal-editor or C:\Users\USERNAME\AppData\Local\Mike Becker's Software\Universal Editor
				String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
					ShortName
				}),
				// then look in ~/.universal-editor or C:\Users\USERNAME\AppData\Roaming\Mike Becker's Software\Universal Editor
				String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
					ShortName
				})
			};
		}
		public Accessor[] EnumerateDataFiles(string filter)
		{
			List<Accessor> xmlFilesList = new List<Accessor>();

			// TODO: change "universal-editor" string to platform-dependent "universal-editor" on *nix or "Mike Becker's Software/Universal Editor" on Windowds
			string[] paths = EnumerateDataPaths();

			foreach (string path in paths)
			{
				// skip this one if the path doesn't exist
				if (!System.IO.Directory.Exists(path)) continue;

				string[] xmlfilesPath = null;
				try
				{
					xmlfilesPath = System.IO.Directory.GetFiles(path, filter, System.IO.SearchOption.AllDirectories);
				}
				catch (UnauthorizedAccessException ex)
				{
					Console.WriteLine("UE: warning: access to data path {0} denied", path);
					continue;
				}

				foreach (string s in xmlfilesPath)
				{
					xmlFilesList.Add(new FileAccessor(s));
				}
			}

			MBS.Framework.Reflection.ManifestResourceStream[] streams = MBS.Framework.Reflection.GetAvailableManifestResourceStreams();
			for (int j = 0; j < streams.Length; j++)
			{
				if (streams[j].Name.Match(((UIApplication)Application.Instance).ConfigurationFileNameFilter) || streams[j].Name.EndsWith(".xml"))
				{
					StreamAccessor sa = new StreamAccessor(streams[j].Stream);
					sa.FileName = streams[j].Name;
					xmlFilesList.Add(sa);
				}
			}
			return xmlFilesList.ToArray();
		}

		/// <summary>
		/// The aggregated raw markup of all the various XML files loaded in the current search path.
		/// </summary>
		private MarkupObjectModel mvarRawMarkup = new MarkupObjectModel();
		public MarkupObjectModel RawMarkup { get { return mvarRawMarkup; } }

		private Language mvarDefaultLanguage = null;
		/// <summary>
		/// The default <see cref="Language"/> used to display translatable text in this application.
		/// </summary>
		public Language DefaultLanguage { get { return mvarDefaultLanguage; } set { mvarDefaultLanguage = value; } }

		private Language.LanguageCollection mvarLanguages = new Language.LanguageCollection();
		/// <summary>
		/// The languages defined for this application. Translations can be added through XML files in the ~/Languages folder.
		/// </summary>
		public Language.LanguageCollection Languages { get { return mvarLanguages; } }

		private CommandBar.CommandBarCollection mvarCommandBars = new CommandBar.CommandBarCollection();
		/// <summary>
		/// The command bars loaded in this application, which can each hold multiple <see cref="CommandItem"/>s.
		/// </summary>
		public CommandBar.CommandBarCollection CommandBars { get { return mvarCommandBars; } }


		private void InitializeCommandBar(MarkupTagElement tag)
		{
			MarkupAttribute attID = tag.Attributes["ID"];
			if (attID == null) return;

			CommandBar cb = new CommandBar();
			cb.ID = attID.Value;

			MarkupAttribute attTitle = tag.Attributes["Title"];
			if (attTitle != null)
			{
				cb.Title = attTitle.Value;
			}
			else
			{
				cb.Title = cb.ID;
			}

			MarkupTagElement tagItems = tag.Elements["Items"] as MarkupTagElement;
			if (tagItems != null)
			{
				foreach (MarkupElement elItem in tagItems.Elements)
				{
					MarkupTagElement tagItem = (elItem as MarkupTagElement);
					if (tagItem == null) continue;

					InitializeCommandBarItem(tagItem, null, cb);
				}
			}

			mvarCommandBars.Add(cb);
		}

		internal static void InitializeCommandBarItem(MarkupTagElement tag, Command parent, CommandBar parentCommandBar)
		{
			CommandItem item = CommandItemLoader.FromMarkup(tag);
			item.AddToCommandBar(parent, parentCommandBar);
		}

		private ApplicationMainMenu mvarMainMenu = new ApplicationMainMenu();
		/// <summary>
		/// The main menu of this application, which can hold multiple <see cref="CommandItem"/>s.
		/// </summary>
		public ApplicationMainMenu MainMenu { get { return mvarMainMenu; } }

		public void UpdateSplashScreenStatus(string value)
		{
			// TODO: implement this
			splasher.SetStatus(value);
		}
		public void UpdateSplashScreenStatus(string value, int progressValue, int progressMinimum = 0, int progressMaximum = 100)
		{
			// TODO: implement this
			splasher.SetStatus(value, progressValue, progressMinimum, progressMaximum);
		}

		public string ConfigurationFileNameFilter { get; set; } = null;

		/// <summary>
		/// Enumerates and loads the XML configuration files for the application. Blatantly stolen^W^WAdapted from Universal Editor.
		/// </summary>
		private void InitializeXMLConfiguration()
		{
			OnBeforeConfigurationLoaded(EventArgs.Empty);

			#region Load the XML files
			string configurationFileNameFilter = ConfigurationFileNameFilter; 
			if (configurationFileNameFilter == null) configurationFileNameFilter = System.Configuration.ConfigurationManager.AppSettings["ApplicationFramework.Configuration.ConfigurationFileNameFilter"];
			if (configurationFileNameFilter == null) configurationFileNameFilter = "*.xml";

			Accessor[] xmlfiles = EnumerateDataFiles(configurationFileNameFilter);

			UpdateSplashScreenStatus("Loading XML configuration files", 0, 0, xmlfiles.Length);

			XMLDataFormat xdf = new XMLDataFormat();
			foreach (Accessor xmlfile in xmlfiles)
			{
				MarkupObjectModel markup = new MarkupObjectModel();
				Document doc = new Document(markup, xdf, xmlfile);
				doc.Accessor.DefaultEncoding = UniversalEditor.IO.Encoding.UTF8;

				doc.Accessor.Open();
				doc.Load();
				doc.Close();

				markup.CopyTo(mvarRawMarkup);

				UpdateSplashScreenStatus("Loading XML configuration files", Array.IndexOf(xmlfiles, xmlfile) + 1, 0, xmlfiles.Length);
			}

			#endregion

			#region Initialize the configuration with the loaded data
			#region Commands
			UpdateSplashScreenStatus("Loading available commands");
			MarkupTagElement tagCommands = (mvarRawMarkup.FindElement("ApplicationFramework", "Commands") as MarkupTagElement);
			if (tagCommands != null)
			{
				foreach (MarkupElement elCommand in tagCommands.Elements)
				{
					MarkupTagElement tagCommand = (elCommand as MarkupTagElement);
					if (tagCommand == null) continue;
					if (tagCommand.FullName != "Command") continue;

					MarkupAttribute attID = tagCommand.Attributes["ID"];
					if (attID == null) continue;

					Command cmd = CommandLoader.FromMarkup(tagCommand);
					((UIApplication)Application.Instance).Commands.Add(cmd);
				}
			}
			#endregion
			#region Settings providers
			UpdateSplashScreenStatus("Loading settings providers");
			MarkupTagElement tagSettingsProviders = (mvarRawMarkup.FindElement("ApplicationFramework", "SettingsProviders") as MarkupTagElement);
			if (tagSettingsProviders != null)
			{
				foreach (MarkupElement elSettingsProvider in tagSettingsProviders.Elements)
				{
					LoadSettingsProviderXML(elSettingsProvider as MarkupTagElement);
				}
			}
			#endregion
			#region Main Menu Items
			UpdateSplashScreenStatus("Loading main menu items");

			MarkupTagElement tagMainMenuItems = (mvarRawMarkup.FindElement("ApplicationFramework", "MainMenu", "Items") as MarkupTagElement);
			if (tagMainMenuItems != null)
			{
				foreach (MarkupElement elItem in tagMainMenuItems.Elements)
				{
					MarkupTagElement tagItem = (elItem as MarkupTagElement);
					if (tagItem == null) continue;
					InitializeCommandBarItem(tagItem, null, null);
				}
			}

			UpdateSplashScreenStatus("Loading Quick Access Toolbar items");

			MarkupTagElement tagQuickAccessToolbarItems = (mvarRawMarkup.FindElement("ApplicationFramework", "QuickAccessToolbar", "Items") as MarkupTagElement);
			if (tagQuickAccessToolbarItems != null)
			{
				foreach (MarkupElement elItem in tagQuickAccessToolbarItems.Elements)
				{
					MarkupTagElement tagItem = (elItem as MarkupTagElement);
					if (tagItem == null) continue;

					QuickAccessToolbarItems.Add(CommandItemLoader.FromMarkup(tagItem));
				}
			}

			UpdateSplashScreenStatus("Loading command bars");

			MarkupTagElement tagCommandBars = (mvarRawMarkup.FindElement("ApplicationFramework", "CommandBars") as MarkupTagElement);
			if (tagCommandBars != null)
			{
				foreach (MarkupElement elCommandBar in tagCommandBars.Elements)
				{
					MarkupTagElement tagCommandBar = (elCommandBar as MarkupTagElement);
					if (tagCommandBar == null) continue;
					if (tagCommandBar.FullName != "CommandBar") continue;
					InitializeCommandBar(tagCommandBar);
				}
			}
			#endregion
			#region Languages
			UpdateSplashScreenStatus("Loading languages and translations");

			MarkupTagElement tagLanguages = (mvarRawMarkup.FindElement("ApplicationFramework", "Languages") as MarkupTagElement);
			if (tagLanguages != null)
			{
				foreach (MarkupElement elLanguage in tagLanguages.Elements)
				{
					MarkupTagElement tagLanguage = (elLanguage as MarkupTagElement);
					if (tagLanguage == null) continue;
					if (tagLanguage.FullName != "Language") continue;
					InitializeLanguage(tagLanguage);
				}

				MarkupAttribute attDefaultLanguageID = tagLanguages.Attributes["DefaultLanguageID"];
				if (attDefaultLanguageID != null)
				{
					mvarDefaultLanguage = mvarLanguages[attDefaultLanguageID.Value];
				}
			}

			UpdateSplashScreenStatus("Setting language");

			if (mvarDefaultLanguage == null)
			{
				mvarDefaultLanguage = new Language();
			}
			else
			{
				foreach (Command cmd in ((UIApplication)Application.Instance).Commands)
				{
					cmd.Title = mvarDefaultLanguage.GetCommandTitle(cmd.ID, cmd.ID);
				}
			}
			#endregion

			#region Plugins
			UpdateSplashScreenStatus("Loading plugins");

			MarkupTagElement tagPlugins = (mvarRawMarkup.FindElement("ApplicationFramework", "Plugins") as MarkupTagElement);
			if (tagPlugins != null)
			{
				foreach (MarkupElement elPlugin in tagPlugins.Elements)
				{
					MarkupTagElement tagPlugin = (elPlugin as MarkupTagElement);
					if (tagPlugin == null) continue;
					if (tagPlugin.FullName != "Plugin") continue;
					this.InitializePlugin(tagPlugin);
				}
			}
			#endregion

			// UpdateSplashScreenStatus("Finalizing configuration");
			// ConfigurationManager.Load();
			#endregion

			Title = DefaultLanguage?.GetStringTableEntry("Application.Title", ((UIApplication)Application.Instance).Title);
			OnAfterConfigurationLoaded(EventArgs.Empty);
		}

		private SettingsProvider LoadSettingsProviderXML(MarkupTagElement tag)
		{
			if (tag == null) return null;
			if (tag.FullName != "SettingsProvider") return null;

			MarkupAttribute attID = tag.Attributes["ID"];
			if (attID == null) return null;

			Guid id = new Guid(attID.Value);
			if (((UIApplication)Application.Instance).SettingsProviders.Contains(id))
				return null;

			CustomSettingsProvider csp = new CustomSettingsProvider();
			csp.ID = id;
			foreach (MarkupElement el in tag.Elements)
			{
				MarkupTagElement tag2 = (el as MarkupTagElement);
				if (tag2 == null) continue;
				if (tag2.FullName == "SettingsGroup")
				{
					SettingsGroup sg = new SettingsGroup();
					sg.Path = ParsePath(tag2.Elements["Path"] as MarkupTagElement);

					MarkupTagElement tagSettings = (tag2.Elements["Settings"] as MarkupTagElement);
					if (tagSettings != null)
					{
						foreach (MarkupElement el2 in tagSettings.Elements)
						{
							Setting s = LoadSettingXML(el2 as MarkupTagElement);
							if (s != null)
								sg.Settings.Add(s);
						}
					}
					csp.SettingsGroups.Add(sg);
				}
			}
			((UIApplication)Application.Instance).SettingsProviders.Add(csp);
			return csp;
		}

		private Setting LoadSettingXML(MarkupTagElement tag)
		{
			if (tag == null) return null;

			MarkupAttribute attSettingID = tag.Attributes["ID"];
			MarkupAttribute attSettingName = tag.Attributes["Name"];
			MarkupAttribute attSettingTitle = tag.Attributes["Title"];
			MarkupAttribute attSettingDescription = tag.Attributes["Description"];

			MarkupAttribute attDefaultValue = tag.Attributes["DefaultValue"];

			Setting s = null;
			switch (tag.FullName)
			{
				case "BooleanSetting":
				{
					s = new BooleanSetting(attSettingName?.Value, attSettingTitle?.Value);
					if (attDefaultValue != null)
						s.DefaultValue = bool.Parse(attDefaultValue.Value);
					break;
				}
				case "TextSetting":
				{
					s = new TextSetting(attSettingName?.Value, attSettingTitle?.Value);
					if (attDefaultValue != null)
						s.DefaultValue = attDefaultValue.Value;
					break;
				}
				case "FileSetting":
				{
					s = new FileSetting(attSettingName?.Value, attSettingTitle?.Value);
					if (attDefaultValue != null)
						s.DefaultValue = attDefaultValue.Value;
					break;
				}
				case "CommandSetting":
				{
					MarkupAttribute attCommandID = tag.Attributes["CommandID"];
					MarkupAttribute attStylePreset = tag.Attributes["StylePreset"];
					s = new CommandSetting(attSettingName?.Value, attSettingTitle?.Value, attCommandID?.Value);
					if (attStylePreset != null)
					{
						((CommandSetting)s).StylePreset = (ButtonStylePresets)Enum.Parse(typeof(ButtonStylePresets), attStylePreset.Value);
					}
					break;
				}
				case "RangeSetting":
				{
					s = new RangeSetting(attSettingName?.Value, attSettingTitle?.Value);
					MarkupAttribute attMinimumValue = tag.Attributes["MinimumValue"];
					if (attMinimumValue != null)
						((RangeSetting)s).MinimumValue = decimal.Parse(attMinimumValue.Value);

					MarkupAttribute attMaximumValue = tag.Attributes["MaximumValue"];
					if (attMaximumValue != null)
						((RangeSetting)s).MaximumValue = decimal.Parse(attMaximumValue.Value);

					if (attDefaultValue != null)
						((RangeSetting)s).DefaultValue = decimal.Parse(attDefaultValue.Value);
					break;
				}
				case "GroupSetting":
				{
					s = new GroupSetting(attSettingName?.Value, attSettingTitle?.Value);
					MarkupTagElement tagSettings = tag.Elements["Settings"] as MarkupTagElement;
					if (tagSettings != null)
					{
						foreach (MarkupElement el in tagSettings.Elements)
						{
							Setting s2 = LoadSettingXML(el as MarkupTagElement);
							if (s2 != null)
							{
								(s as GroupSetting).Options.Add(s2);
							}
						}
					}
					MarkupTagElement tagHeaderSettings = tag.Elements["HeaderSettings"] as MarkupTagElement;
					if (tagHeaderSettings != null)
					{
						foreach (MarkupElement el in tagHeaderSettings.Elements)
						{
							Setting s2 = LoadSettingXML(el as MarkupTagElement);
							if (s2 != null)
							{
								(s as GroupSetting).HeaderSettings.Add(s2);
							}
						}
					}
					break;
				}
			}

			if (s != null)
			{
				if (attSettingDescription != null)
					s.Description = attSettingDescription.Value;
			}
			return s;
		}

		private string[] ParsePath(MarkupTagElement tag)
		{
			if (tag == null) return null;
			if (tag.FullName != "Path") return null;

			List<string> path = new List<string>();
			foreach (MarkupElement el in tag.Elements)
			{
				MarkupTagElement tag2 = (el as MarkupTagElement);
				if (tag2 == null) continue;
				if (tag2.FullName != "Part") continue;

				path.Add(tag2.Value);
			}
			return path.ToArray();
		}

		public CustomPlugin.CustomPluginCollection CustomPlugins { get; } = new CustomPlugin.CustomPluginCollection();

		private void InitializeLanguage(MarkupTagElement tag)
		{
			MarkupAttribute attID = tag.Attributes["ID"];
			if (attID == null) return;

			Language lang = mvarLanguages[attID.Value];
			if (lang == null)
			{
				lang = new Language();
				lang.ID = attID.Value;
				mvarLanguages.Add(lang);
			}

			MarkupAttribute attTitle = tag.Attributes["Title"];
			if (attTitle != null)
			{
				lang.Title = attTitle.Value;
			}
			else
			{
				lang.Title = lang.ID;
			}

			MarkupTagElement tagStringTable = (tag.Elements["StringTable"] as MarkupTagElement);
			if (tagStringTable != null)
			{
				foreach (MarkupElement elStringTableEntry in tagStringTable.Elements)
				{
					MarkupTagElement tagStringTableEntry = (elStringTableEntry as MarkupTagElement);
					if (tagStringTableEntry == null) continue;
					if (tagStringTableEntry.FullName != "StringTableEntry") continue;

					MarkupAttribute attStringTableEntryID = tagStringTableEntry.Attributes["ID"];
					if (attStringTableEntryID == null) continue;

					MarkupAttribute attStringTableEntryValue = tagStringTableEntry.Attributes["Value"];
					if (attStringTableEntryValue == null) continue;

					lang.SetStringTableEntry(attStringTableEntryID.Value, attStringTableEntryValue.Value);
				}
			}

			MarkupTagElement tagCommands = (tag.Elements["Commands"] as MarkupTagElement);
			if (tagCommands != null)
			{
				foreach (MarkupElement elCommand in tagCommands.Elements)
				{
					MarkupTagElement tagCommand = (elCommand as MarkupTagElement);
					if (tagCommand == null) continue;
					if (tagCommand.FullName != "Command") continue;

					MarkupAttribute attCommandID = tagCommand.Attributes["ID"];
					if (attCommandID == null) continue;

					MarkupAttribute attCommandTitle = tagCommand.Attributes["Title"];
					if (attCommandTitle == null) continue;

					lang.SetCommandTitle(attCommandID.Value, attCommandTitle.Value);
				}
			}
		}

		public EventHandler BeforeConfigurationLoaded;
		private void OnBeforeConfigurationLoaded(EventArgs e)
		{
			BeforeConfigurationLoaded?.Invoke(this, e);
		}

		public EventHandler AfterConfigurationLoaded;
		private void OnAfterConfigurationLoaded(EventArgs e)
		{
			AfterConfigurationLoaded?.Invoke(this, e);
		}

		/// <summary>
		/// The event that is called the first time an application is started.
		/// </summary>
		public event EventHandler Startup;
		private void OnStartup(EventArgs e)
		{
			Startup?.Invoke(this, e);
		}

		private SplashScreenWindow splasher = null;
		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

		private void ShowSplashScreen()
		{
			sw.Reset();
			sw.Start();
			// if (LocalConfiguration.SplashScreen.Enabled)
			// {
			splasher = new SplashScreenWindow();
			splasher.Show();
			// }
		}
		internal void HideSplashScreen()
		{
			while (splasher == null)
			{
				// System.Threading.Thread.Sleep(500);
			}
			splasher.Hide();
			splasher = null;

			sw.Stop();
			Console.WriteLine("stopwatch: went from rip to ready in {0}", sw.Elapsed);
		}


		public event ApplicationActivatedEventHandler Activated;
		protected virtual void OnActivated(ApplicationActivatedEventArgs e)
		{
			if (e.FirstRun)
			{
				ShowSplashScreen();
				((UIApplication)Application.Instance).DoEvents();

				System.Threading.Thread t = new System.Threading.Thread(t_threadStart);
				t.Start();

				while (splasher != null)
				{
					((UIApplication)Application.Instance).DoEvents();
					System.Threading.Thread.Sleep(25); // don't remove this
				}
			}

			Activated?.Invoke(typeof(UIApplication), e);
		}

		private void t_threadStart(object obj)
		{
			InitializeXMLConfiguration();

			HideSplashScreen();
		}

		private void Application_MenuBar_Item_Click(object sender, EventArgs e)
		{
			CommandMenuItem mi = (sender as CommandMenuItem);
			if (mi == null)
				return;

			((UIApplication)Application.Instance).ExecuteCommand(mi.Name);
		}

		private List<Window> _windows = new List<Window>();
		private System.Collections.ObjectModel.ReadOnlyCollection<Window> _windowsRO = null;
		public System.Collections.ObjectModel.ReadOnlyCollection<Window> Windows
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
		internal void AddWindow(Window window)
		{
			_windows.Add(window);
		}

		private Dictionary<Context, List<MenuItem>> _listContextMenuItems = new Dictionary<Context, System.Collections.Generic.List<MenuItem>>();
		private Dictionary<Context, List<Command>> _listContextCommands = new Dictionary<Context, List<Command>>();

		protected override void OnContextAdded(ContextChangedEventArgs e)
		{
			if (!_listContextMenuItems.ContainsKey(e.Context))
			{
				_listContextMenuItems[e.Context] = new List<MenuItem>();
			}
			if (!_listContextCommands.ContainsKey(e.Context))
			{
				_listContextCommands[e.Context] = new List<Command>();
			}

			foreach (Command cmd in e.Context.Commands)
			{
				Command actual = ((UIApplication)Application.Instance).Commands[cmd.ID];
				if (actual != null)
				{
					for (int i = 0; i < cmd.Items.Count; i++)
					{
						if (!actual.Items.Contains(cmd.Items[i]))
						{
							cmd.Items[i].AddToCommandBar(actual, null);
						}
					}
				}
				else
				{
					_listContextCommands[e.Context].Add(cmd);
					((UIApplication)Application.Instance).Commands.Add(cmd);
				}
			}

			if (e.Context is UIContext)
			{
				foreach (CommandItem ci in ((UIContext)e.Context).MenuItems)
				{
					MenuItem[] mi = MenuItem.LoadMenuItem(ci, Application_MenuBar_Item_Click);
					foreach (Window w in ((UIApplication)Application.Instance).Windows)
					{
						int insertIndex = -1;
						if (ci.InsertAfterID != null)
						{
							insertIndex = w.MenuBar.Items.IndexOf(w.MenuBar.Items[ci.InsertAfterID]) + 1;
						}
						else if (ci.InsertBeforeID != null)
						{
							insertIndex = w.MenuBar.Items.IndexOf(w.MenuBar.Items[ci.InsertBeforeID]);
						}

						for (int i = 0; i < mi.Length; i++)
						{
							_listContextMenuItems[e.Context].Add(mi[i]);

							if (insertIndex != -1)
							{
								w.MenuBar.Items.Insert(insertIndex, mi[i]);
							}
							else
							{
								w.MenuBar.Items.Add(mi[i]);
							}
							insertIndex++;
						}
					}
				}
			}
			base.OnContextRemoved(e);
		}

		protected override void OnContextRemoved(ContextChangedEventArgs e)
		{
			if (_listContextMenuItems.ContainsKey(e.Context))
			{
				foreach (Window w in Windows)
				{
					foreach (MenuItem mi in _listContextMenuItems[e.Context])
					{
						w.MenuBar.Items.Remove(mi);
					}
				}
			}
			_listContextMenuItems[e.Context].Clear();

			foreach (Command cmd in _listContextCommands[e.Context])
			{
				Commands.Remove(cmd);
			}
		}
		public CommandItem.CommandItemCollection QuickAccessToolbarItems { get; } = new CommandItem.CommandItemCollection();


		public string ExpandRelativePath(string relativePath)
		{
			if (relativePath == null) relativePath = String.Empty;
			if (relativePath.StartsWith("~" + System.IO.Path.DirectorySeparatorChar.ToString()) || relativePath.StartsWith("~" + System.IO.Path.AltDirectorySeparatorChar.ToString()))
			{
				string[] potentialFileNames = EnumerateDataPaths();
				for (int i = potentialFileNames.Length - 1; i >= 0; i--)
				{
					potentialFileNames[i] = potentialFileNames[i] + System.IO.Path.DirectorySeparatorChar.ToString() + relativePath.Substring(2);
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

		public event EventHandler ApplicationExited;

		private void OnApplicationExited(EventArgs e)
		{
			foreach (SettingsProvider provider in ((UIApplication)Application.Instance).SettingsProviders)
			{
				provider.SaveSettings ();
			}

			if (ApplicationExited != null) ApplicationExited(null, e);
		}

		private void InitializeSettingsProfiles()
		{
			SettingsProfiles.Add(new SettingsProfile());
			SettingsProfiles[0].ID = SettingsProfile.AllUsersGUID;
			SettingsProfiles[0].Title = "(All Users)";

			SettingsProfiles.Add(new SettingsProfile());
			SettingsProfiles[1].ID = SettingsProfile.ThisUserGUID;
			SettingsProfiles[1].Title = "(This User)";

			string[] dataPaths = EnumerateDataPaths();
			for (int i = 0; i < dataPaths.Length; i++)
			{
				if (System.IO.File.Exists(dataPaths[i] + System.IO.Path.DirectorySeparatorChar.ToString() + "settings" + System.IO.Path.DirectorySeparatorChar.ToString() + "profiles.lst"))
				{
					string[] lines = System.IO.File.ReadAllLines(dataPaths[i] + System.IO.Path.DirectorySeparatorChar.ToString() + "settings" + System.IO.Path.DirectorySeparatorChar.ToString() + "profiles.lst");
					for (int j = 0; j < lines.Length; j++)
					{
						if (lines[j] == String.Empty || lines[j].StartsWith("#"))
							continue;

						string[] split = lines[j].Split(new char[] { '=' });
						if (split.Length == 2)
						{
							SettingsProfile profile = new SettingsProfile();
							profile.ID = new Guid(split[0].Trim());
							profile.Title = split[1].Trim();
							SettingsProfiles.Add(profile);
						}
					}
				}
			}
		}

		// [DebuggerNonUserCode()]
		protected override void InitializeInternal()
		{

			Type tKnownContexts = typeof(KnownContexts);
			System.Reflection.PropertyInfo[] pis = tKnownContexts.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			for (int i = 0; i < pis.Length; i++)
			{
				Context ctx = (Context)pis[i].GetValue(null, null);
				Contexts.Add(ctx);
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

			// configure UWT-provided features
			pis = typeof(KnownFeatures).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			for (int i = 0; i < pis.Length; i++)
			{
				Feature feature = (Feature)pis[i].GetValue(null, null);
				Features.Add(feature);
			}

			if (mvarEngine == null) throw new ArgumentNullException("Application.Engine", "No engines were found or could be loaded");

			Console.WriteLine("Using engine {0}", mvarEngine.GetType().FullName);
			mvarEngine.Initialize();

			InitializeSettingsProfiles();

			// after initialization, load option providers

			List<SettingsProvider> listOptionProviders = new List<SettingsProvider>();
			System.Collections.Specialized.StringCollection listOptionProviderTypeNames = new System.Collections.Specialized.StringCollection ();

			// load the already-known list
			foreach (SettingsProvider provider in ((UIApplication)Application.Instance).SettingsProviders)
			{
				// FIXME: why do we do this "twice" ?
				listOptionProviders.Add(provider);
				listOptionProviderTypeNames.Add(provider.GetType().FullName);
			}


			Type[] types = MBS.Framework.Reflection.GetAvailableTypes(new Type[] { typeof(SettingsProvider) });

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
							if (ex.InnerException.InnerException != null)
							{
								Console.WriteLine("^--- {0}", ex.InnerException.InnerException.Message);
								Console.WriteLine();
								Console.WriteLine(" *** STACK TRACE *** ");
								Console.WriteLine(ex.StackTrace);
								Console.WriteLine(" ******************* ");
								Console.WriteLine();
							}
						} catch (Exception ex) {
							Console.WriteLine ("error while loading SettingsProvider '" + type.FullName + "': " + ex.Message);
						}
					} else {
						Console.WriteLine ("skipping already loaded SettingsProvider '{0}'", type.FullName);
					}
				}
			}

			foreach (SettingsProvider provider in listOptionProviders)
			{
				if (provider is ApplicationSettingsProvider)
				{
					((UIApplication)Application.Instance).SettingsProviders.Add(provider);
					provider.LoadSettings();
				}
			}

			UserInterfacePlugin[] plugins = UserInterfacePlugin.Get();
			for (int i = 0; i < plugins.Length; i++)
			{
				Console.WriteLine("initializing plugin '{0}'", plugins[i].GetType().FullName);
				plugins[i].Initialize();

				if (plugins[i].Context != null)
					Contexts.Add(plugins[i].Context);
			}
		}

		public UIApplication()
		{
			CommandLine = new DefaultCommandLine();
		}

		protected override int StartInternal()
		{
			int exitCode = ((UIApplication)Application.Instance).Engine.Start(_waitForClose);
			ExitCode = exitCode;

			OnApplicationExited(EventArgs.Empty);

			return exitCode;
		}

		private Window _waitForClose = null;

		// [DebuggerNonUserCode()]
		public int Start(Window waitForClose)
		{
			_waitForClose = waitForClose;
			Console.WriteLine("Application_Start");
			if (waitForClose != null)
			{
				if (mvarEngine.IsControlDisposed(waitForClose))
					mvarEngine.CreateControl(waitForClose);

				waitForClose.Show();
			}
			int exitCode = Start();
			return exitCode;
		}

		protected override void StopInternal(int exitCode)
		{
			base.StopInternal(exitCode);
			mvarEngine.Stop(exitCode);
		}

		protected override void OnStopping(CancelEventArgs e)
		{
			base.OnStopping(e);

			if (mvarEngine == null)
			{
				e.Cancel = true;
				return; // why bother getting an engine? we're stopping...
			}
		}

		public void DoEvents()
		{
			mvarEngine?.DoEvents();
		}

		public T GetSetting<T>(string name, T defaultValue = default(T))
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
		public void SetSetting<T>(string name, T value)
		{
			SetSetting (name, (object)value);
		}

		public SettingsGroup FindSettingGroup(string name, out string realName, out string groupPath)
		{
			string[] namePath = name.Split (new char[] { ':' });
			realName = namePath [namePath.Length - 1];
			groupPath = String.Join (":", namePath, 0, namePath.Length - 1);

			foreach (SettingsProvider provider in SettingsProviders)
			{
				foreach (SettingsGroup group in provider.SettingsGroups)
				{
					string path = String.Join(":", group.Path);
					path = path.Replace(' ', '_');
					if (path.Equals(group.Path))
					{
						return group;
					}
				}
			}

			realName = null;
			groupPath = null;
			return null;
		}

		public object GetSetting(string name)
		{
			string realName = null;
			string groupPath = null;

			SettingsGroup group = FindSettingGroup(name, out realName, out groupPath);
			if (group == null)
				return null;

			if (group.Settings[realName] != null)
			{
				return group.Settings[realName].GetValue();
			}
			return null;
		}
		public void SetSetting(string name, object value)
		{
			string realName = null;
			string groupPath = null;

			SettingsGroup group = FindSettingGroup (name, out realName, out groupPath);
			if (group == null) return;
			if (group.Settings [realName] != null) {
				group.Settings [realName].SetValue (value);
			}
		}

		public Process Launch(Uri uri)
		{
			return Launch(uri.ToString());
		}
		/// <summarpublic / Launch the application represented by the given path.
		/// </summary>
		/// <public ame="path">Path.</param>
		public Process Launch(string path)
		{
			return Engine.LaunchApplication(path);
		}
		public Process Launch(string exename, string arguments)
		{
			return Engine.LaunchApplication(exename, arguments);
		}

		/// <summary>
		/// Displays the application's Help ipublic ystem native Help viewer, navigating to the appropriate <see cref="HelpTopicpublic  specified.
		/// </summary>
		public void ShowHelp(HelpTopic topic = null)
		{
			Engine.ShowHelp(topic);
		}

		public bool ShowSettingsDialog(string[] path = null)
		{
			SettingsDialog dlg = new SettingsDialog();
			if (dlg.ShowDialog(path) == DialogResult.OK)
			{
				return true;
			}
			return false;
		}
	}
}