﻿//
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
	public class Plugin
	{
		public virtual string Title { get; set; } = null;
		public Feature.FeatureCollection ProvidedFeatures { get; } = new Feature.FeatureCollection();
		public PropertyListObjectModel Configuration { get; set; } = new PropertyListObjectModel();

		public bool Initialized { get; private set; } = false;
		public void Initialize()
		{
			if (Initialized)
				return;

			InitializeInternal();
			Initialized = true;
		}

		public Guid ID { get; set; } = Guid.Empty;
		public Context Context { get; protected set; }

		protected virtual void InitializeInternal()
		{
			// this method intentionally left blank
		}

		private static Plugin[] _plugins = null;
		public static Plugin[] Get()
		{
			// _plugins = null; // should not be cached? // actually, yes it should...
			if (_plugins == null)
			{
				Type[] types = MBS.Framework.Reflection.GetAvailableTypes(new Type[] { typeof(Plugin) });
				List<Plugin> plugins = new List<Plugin>();
				for (int i = 0; i < types.Length; i++)
				{
					try
					{
						if (types[i] == typeof(CustomPlugin)) continue;

						Plugin plg = (Plugin)types[i].Assembly.CreateInstance(types[i].FullName);
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

		public static Plugin[] Get(Feature[] providedFeatures)
		{
			List<Plugin> list = new List<Plugin>();
			Plugin[] plugins = Get();
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

		public static Plugin Get(Guid id)
		{
			Plugin[] plugins = Get();
			for (int i = 0; i < plugins.Length; i++)
			{
				if (plugins[i].ID == id)
					return plugins[i];
			}
			return null;
		}

		protected virtual bool IsSupportedInternal()
		{
			return true;
		}
		public bool IsSupported()
		{
			return IsSupportedInternal();
		}
	}
}
