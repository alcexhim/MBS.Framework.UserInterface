//
//  AutoSuggestWindow.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2021 Mike Becker's Software
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
using MBS.Framework.UserInterface.Controls.ListView;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.SyntaxTextBox
{
	public class AutoSuggestionWindow : PopupWindow
	{
		public class SuggestionCollection
			: System.Collections.Generic.List<string>
		{
			private AutoSuggestionWindow _parent = null;
			public SuggestionCollection(AutoSuggestionWindow parent)
			{
				_parent = parent;
			}

			public bool AutoSort { get; set; } = true;

			public new void Clear()
			{
				base.Clear();
				_parent.lv.Model.Rows.Clear();
			}
			public new void Add(string value)
			{
				base.Add(value);
				_parent.lv.Model.Rows.Add(new TreeModelRow(new TreeModelRowColumn[] { new TreeModelRowColumn(_parent.lv.Model.Columns[0], value) }));

				if (AutoSort)
					Sort();
			}
		}

		public ListViewControl lv = null;
		public TabContainer tabs = null;

		public SuggestionCollection Suggestions { get; private set; } = null;

		private string _FilterText = null;
		public string FilterText
		{
			get
			{
				return _FilterText;
			}
			set
			{
				_FilterText = value;
				UpdateList();
			}
		}

		public void UpdateList()
		{
			lv.Model.Rows.Clear();

			for (int i = 0; i < Suggestions.Count; i++)
			{
				if (ShouldFilter(Suggestions[i]))
				{
					lv.Model.Rows.Add(new TreeModelRow(new TreeModelRowColumn[] { new TreeModelRowColumn(lv.Model.Columns[0], Suggestions[i]) }));
				}
			}

			if (lv.Model.Rows.Count > 0)
			{
				lv.SelectedRows.Clear();
				lv.SelectedRows.Add(lv.Model.Rows[0]);
			}
		}

		private bool ShouldFilter(string v)
		{
			SyntaxTextBoxControl owner = (SyntaxTextBoxControl)Owner;
			if (!owner.Language.IsCaseSensitive)
			{
				return String.IsNullOrEmpty(FilterText) || v.ToLower().StartsWith(FilterText.ToLower());
			}
			return String.IsNullOrEmpty(FilterText) || v.StartsWith(FilterText);
		}

		public int VisibleSuggestionCount
		{
			get
			{
				if (!IsCreated)
				{
					return Suggestions.Count;
				}
				UpdateList();
				return lv.Model.Rows.Count;
			}
		}

		public AutoSuggestionWindow()
		{
			Suggestions = new SuggestionCollection(this);
			this.Layout = new BoxLayout(Orientation.Vertical);

			lv = new ListViewControl();
			lv.Model = new DefaultTreeModel(new Type[] { typeof(string) });
			lv.Columns.Add(new ListViewColumn("Item", new CellRenderer[]
			{
				new CellRendererText(lv.Model.Columns[0])
			}));
			lv.HeaderStyle = ColumnHeaderStyle.None;

			this.Controls.Add(lv, new BoxLayout.Constraints(true, true));

			tabs = new TabContainer();
			tabs.TabPages.Add(new TabPage() { Text = "Common" });
			tabs.TabPages.Add(new TabPage() { Text = "All" });
			this.Controls.Add(tabs, new BoxLayout.Constraints(false, false));
		}

		protected internal override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);
			lv.Size = new MBS.Framework.Drawing.Dimension2D(400, 200);
		}

		public void SelectPrevious()
		{
			lv.SelectedIndex--;
		}
		public void SelectNext()
		{
			lv.SelectedIndex++;
		}
	}
}
