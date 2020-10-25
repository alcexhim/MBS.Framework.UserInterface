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
		public static UserInterfacePlugin[] Get()
		{
			// _plugins = null; // should not be cached? // actually, yes it should...
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

				for (int i = 0; i < Application.CustomPlugins.Count; i++)
				{
					plugins.Add(Application.CustomPlugins[i]);
				}
				_plugins = plugins.ToArray();
			}
			return _plugins;
		}

		public static UserInterfacePlugin[] Get(Feature[] providedFeatures)
		{
			List<UserInterfacePlugin> list = new List<UserInterfacePlugin>();
			UserInterfacePlugin[] plugins = Get();
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
	}
}
