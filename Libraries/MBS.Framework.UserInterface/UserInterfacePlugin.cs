//
//  Plugin.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using UniversalEditor.ObjectModels.PropertyList;

namespace MBS.Framework.UserInterface
{
	public class UserInterfacePlugin : Plugin
	{
		public PropertyListObjectModel Configuration { get; set; } = new PropertyListObjectModel();
		public Context Context { get; protected set; }

		private static UserInterfacePlugin[] _plugins = null;
		public static UserInterfacePlugin[] Get(bool resetCache = false)
		{
			if (resetCache)
			{
				_plugins = null; // should not be cached? // actually, yes it should...

				// 2020-12-12 20:54 by beckermj
				// ACTUALLY, it depends on whether the configuration needs to be persisted across calls to Get()
				// 				[it does] and whether the list of plugins needs to be reloaded when CustomPlugins is modified
				//				[it shouldn't, but it does] .
				//
				// The safest way we can handle this RIGHT NOW is to prevent any plugin from being loaded until after CustomPlugins
				// is initialized by UIApplication.
				//
				// We call Plugins.Get(new Feature[] { KnownFeatures.UWTPlatform }) to retrieve the available User Interface plugins
				// that supply the UWT Platform implementation (e.g. GTK, Windows Forms, etc.) - and this causes CustomPlugins to not
				// load properly since it loads AFTER this initial call to Plugins.Get().
				//
				// So I add ed a resetCache parameter that when specified TRUE will clear the cache and then return the appropriate
				// plugin. This way we can continue caching future calls to Get() without missing out on CustomPlugins.
			}

			if (_plugins == null)
			{
				Type[] types = MBS.Framework.Reflection.GetAvailableTypes(new Type[] { typeof(UserInterfacePlugin) });
				List<UserInterfacePlugin> plugins = new List<UserInterfacePlugin>();
				for (int i = 0; i < types.Length; i++)
				{
					try
					{
						if (types[i] == typeof(CustomPlugin)) continue;

						UserInterfacePlugin plg = (UserInterfacePlugin)types[i].Assembly.CreateInstance(types[i].FullName);
						plugins.Add(plg);
					}
					catch (Exception ex)
					{
					}
				}

				for (int i = 0; i < ((UIApplication)Application.Instance).CustomPlugins.Count; i++)
				{
					plugins.Add(((UIApplication)Application.Instance).CustomPlugins[i]);
				}
				_plugins = plugins.ToArray();

				if (resetCache)
				{
					_plugins = null;
					return plugins.ToArray();
				}
			}
			return _plugins;
		}

		public static UserInterfacePlugin[] Get(Feature[] providedFeatures, bool resetCache = false)
		{
			List<UserInterfacePlugin> list = new List<UserInterfacePlugin>();
			UserInterfacePlugin[] plugins = Get(resetCache);
			for (int i = 0; i < plugins.Length; i++)
			{
				if (!plugins[i].IsSupported())
					continue;

				for (int j = 0; j < providedFeatures.Length; j++)
				{
					if (plugins[i].ProvidedFeatures.Contains(providedFeatures[j]))
						list.Add(plugins[i]);
				}
			}
			return list.ToArray();
		}

		public static UserInterfacePlugin Get(Guid id)
		{
			UserInterfacePlugin[] plugins = Get();
			for (int i = 0; i < plugins.Length; i++)
			{
				if (plugins[i].ID == id)
					return plugins[i];
			}
			return null;
		}

		protected virtual void UpdateMenuItemsInternal()
		{
		}
		public void UpdateMenuItems()
		{
			UpdateMenuItemsInternal();
		}
	}
}
