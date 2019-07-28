//
//  OptionPanel.cs
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

namespace UniversalWidgetToolkit
{
	public class OptionGroup : IComparable<OptionGroup>
	{
		public class OptionGroupCollection
			: System.Collections.ObjectModel.Collection<OptionGroup>
		{
			public OptionGroup Add(string path, Option[] options)
			{
				string[] paths = new string[0];
				if (!String.IsNullOrEmpty (path)) {
					paths = path.Split (new char[] { ':' });
				}
				return Add (paths, options);
			}
			public OptionGroup Add(string[] path, Option[] options)
			{
				OptionGroup grp = new OptionGroup();
				grp.Path = path;
				if (options != null) {
					foreach (Option option in options) {
						grp.Options.Add (option);
					}
				}
				Add (grp);
				return grp;
			}
		}

		public OptionGroup()
		{
		}
		public OptionGroup(string path, Option[] options)
		{
			string[] paths = new string[0];
			if (!String.IsNullOrEmpty (path)) {
				paths = path.Split (new char[] { ':' });
			}
			Path = paths;
			foreach (Option option in options)
			{
				Options.Add (option);
			}
		}
		public OptionGroup(string[] paths, Option[] options)
		{
			Path = paths;
			foreach (Option option in options)
			{
				Options.Add (option);
			}
		}

		public int CompareTo(OptionGroup other)
		{
			string xpath = String.Join (":", this.GetPath ());
			string ypath = String.Join (":", other.GetPath ());
			return xpath.CompareTo (ypath);
		}

		public string[] GetPath()
		{
			if (Path == null) return new string[0];
			return Path;
		}

		public string[] Path { get; set; } = null;
		public string Title
		{
			get
			{
				if (Path.Length > 0) return Path[Path.Length - 1];
				return null;
			}
		}
		public Option.OptionCollection Options { get; } = new Option.OptionCollection();

		public override string ToString ()
		{
			return String.Join (":", Path);
		}
	}
}

