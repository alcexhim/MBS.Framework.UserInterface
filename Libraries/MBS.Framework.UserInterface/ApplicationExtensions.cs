//
//  ApplicationExtensions.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2020 Mike Becker's Software
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
using UniversalEditor;
using UniversalEditor.Accessors;
using UniversalEditor.DataFormats.Markup.XML;
using UniversalEditor.ObjectModels.Markup;
using UniversalEditor.ObjectModels.PropertyList;

namespace MBS.Framework.UserInterface
{
	public static class ApplicationExtensions
	{
		public static void InitializePlugin(this UIApplication app, MarkupTagElement tag)
		{
			CustomPlugin plugin = new CustomPlugin();
			plugin.ID = new Guid(tag.Attributes["ID"]?.Value);
			plugin.Title = tag.Attributes["Title"]?.Value;

			MarkupTagElement tagProvidedFeatures = tag.Elements["ProvidedFeatures"] as MarkupTagElement;
			if (tagProvidedFeatures != null)
			{
				for (int i = 0; i < tagProvidedFeatures.Elements.Count; i++)
				{
					MarkupTagElement tagProvidedFeature = (tagProvidedFeatures.Elements[i] as MarkupTagElement);
					if (tagProvidedFeature == null) continue;
					if (tagProvidedFeature.FullName != "ProvidedFeature") continue;

					string featureId = tagProvidedFeature.Attributes["FeatureID"]?.Value;
					if (featureId == null) continue;

					plugin.ProvidedFeatures.Add(new Feature(new Guid(featureId), tagProvidedFeature.Attributes["Title"]?.Value));
				}
			}

			MarkupTagElement tagConfiguration = tag.Elements["Configuration"] as MarkupTagElement;
			if (tagConfiguration != null)
			{
				MarkupObjectModel cfg = new MarkupObjectModel();
				cfg.Elements.Add(tagConfiguration);

				PropertyListObjectModel plom = new PropertyListObjectModel();
				MemoryAccessor ma = new MemoryAccessor();
				Document.Save(cfg, new XMLDataFormat(), ma);
				ma.Position = 0;

				Document.Load(plom, new UniversalEditor.DataFormats.PropertyList.XML.XMLPropertyListDataFormat(), ma);
				plugin.Configuration = plom;
			}

			app.CustomPlugins.Add(plugin);
		}

	}
}
