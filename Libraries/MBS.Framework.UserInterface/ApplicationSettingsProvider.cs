//
//  ApplicationSettingsProvider.cs
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
using UniversalEditor.ObjectModels.Markup;
using UniversalEditor.DataFormats.Markup.XML;
using UniversalEditor.Accessors;
using UniversalEditor;

namespace MBS.Framework.UserInterface
{
	/// <summary>
	/// Represents a <see cref="SettingsProvider" /> that controls settings for the entire <see cref="UIApplication" />
	/// (i.e., is added to <see cref="UIApplication.SettingsProviders"/> collection).
	/// </summary>
	public class ApplicationSettingsProvider : SettingsProvider
	{
		public virtual string FileName { get { return null; } }

		protected override void InitializeInternal()
		{
			base.InitializeInternal();

			foreach (SettingsGroup grp in SettingsGroups)
			{
				foreach (Setting s in grp.Settings)
				{
					s.SetValue((Application.Instance as UIApplication).GetSetting(s.ID));
				}
			}
		}

		protected override void LoadSettingsInternal()
		{
			try
			{
				string settingsDir = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				settingsDir += System.IO.Path.DirectorySeparatorChar.ToString() + "settings";

				string fileName = FileName;
				if (fileName == null)
				{
					fileName = this.GetType().FullName;
					fileName = fileName.Replace('.', System.IO.Path.DirectorySeparatorChar);
				}
				fileName = settingsDir + System.IO.Path.DirectorySeparatorChar.ToString() + ID.ToString("B") + ".xml";

				MarkupObjectModel mom = new MarkupObjectModel();
				XMLDataFormat xdf = new XMLDataFormat();
				FileAccessor fa = new FileAccessor(fileName);

				Document.Load(mom, xdf, fa);

				MarkupTagElement tagSettings = (mom.FindElementUsingSchema("urn:net.alcetech.schemas.MBS.Framework.UserInterface.Settings", "settings") as MarkupTagElement);
				if (tagSettings == null) return;

				foreach (MarkupElement elGroup in tagSettings.Elements)
				{
					MarkupTagElement tagGroup = (elGroup as MarkupTagElement);
					LoadGroup(tagGroup);
				}
			}
			catch (System.IO.DirectoryNotFoundException ex)
			{
			}
		}

		private void LoadGroup(MarkupTagElement tagGroup)
		{
			foreach (MarkupElement elSetting in tagGroup.Elements)
			{
				MarkupTagElement tagSetting = (elSetting as MarkupTagElement);
				if (tagSetting == null)
					continue;

				MarkupAttribute attID = tagSetting.Attributes["id"];
				if (attID == null)
					continue;

				object value = null;

				MarkupAttribute attValue = tagSetting.Attributes["value"];
				if (attValue != null)
					value = attValue.Value;

				Guid groupID = new Guid(tagGroup.Attributes["id"].Value);
				Guid settingID = new Guid(attID.Value);

				SettingsGroups[groupID].Settings[settingID].SetValue(value);
				((UIApplication)Application.Instance).SetSetting(settingID, value);
			}
		}

		protected override void SaveSettingsInternal()
		{
			string settingsDir = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			settingsDir += System.IO.Path.DirectorySeparatorChar.ToString() + "settings";

			string fileName = FileName;
			if (fileName == null)
			{
				fileName = this.GetType().FullName;
				fileName = fileName.Replace('.', System.IO.Path.DirectorySeparatorChar);
			}
			fileName = settingsDir + System.IO.Path.DirectorySeparatorChar.ToString() + ID.ToString("B") + ".xml";

			string dir = System.IO.Path.GetDirectoryName(fileName);
			if (!System.IO.Directory.Exists(dir))
			{
				System.IO.Directory.CreateDirectory(dir);
			}

			MarkupObjectModel mom = new MarkupObjectModel();
			XMLDataFormat xdf = new XMLDataFormat();
			FileAccessor fa = new FileAccessor(fileName, true, true);

			MarkupTagElement tagSettings = new MarkupTagElement();
			tagSettings.FullName = "uwt:settings";
			tagSettings.Attributes.Add("xmlns:uwt", "urn:net.alcetech.schemas.MBS.Framework.UserInterface.Settings");

			foreach (SettingsGroup group in SettingsGroups)
			{
				if (group.Settings.Count == 0)
					continue;

				MarkupTagElement tagGroup = new MarkupTagElement();
				tagGroup.FullName = "group";
				if (group.ID != Guid.Empty)
				{
					tagGroup.Attributes.Add("id", group.ID.ToString("B"));
				}
				// tagGroup.Attributes.Add("name", String.Join(":", group.Path).Replace(" ", "_"));

				foreach (Setting setting in group.Settings)
				{
					MarkupTagElement tagSetting = new MarkupTagElement();
					tagSetting.FullName = "setting";
					if (setting.ID == Guid.Empty)
						continue;

					tagSetting.Attributes.Add("id", setting.ID.ToString("B"));
					object value = setting.GetValue();
					if (value != null)
					{
						tagSetting.Attributes.Add("value", value.ToString());
					}
					tagGroup.Elements.Add(tagSetting);
				}
				tagSettings.Elements.Add(tagGroup);
			}

			mom.Elements.Add(tagSettings);

			Document.Save(mom, xdf, fa);
		}
	}
}
