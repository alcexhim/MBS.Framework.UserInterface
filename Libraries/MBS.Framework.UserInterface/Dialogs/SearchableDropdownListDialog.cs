//
//  SearchableDropdownListDialog.cs
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
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.ListView;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Dialogs
{
	public class SearchableDropdownListDialog : CustomDialog
	{
		private Button cmdReset = null;
		private Button cmdNone = null;
		private TextBox txtSearch = null;
		private ListViewControl lv = null;

		private Container ctSearchAndShowAll = null;
		private DefaultTreeModel tm = null;

		public SearchableDropdownListDialog()
		{
			this.Decorated = false;
			this.Layout = new BoxLayout(Orientation.Vertical);


			this.ctSearchAndShowAll = new Container();
			this.ctSearchAndShowAll.Layout = new BoxLayout(Orientation.Horizontal);

			this.txtSearch = new TextBox();
			this.txtSearch.Changed += txtSearch_Changed;
			this.txtSearch.KeyDown += txtSearch_KeyDown;
			this.ctSearchAndShowAll.Controls.Add(this.txtSearch, new BoxLayout.Constraints(true, true));

			this.cmdReset = new Button();
			this.cmdReset.Text = "_Reset";
			this.cmdReset.Click += cmdReset_Click;
			this.cmdReset.BorderStyle = ButtonBorderStyle.None;
			this.ctSearchAndShowAll.Controls.Add(this.cmdReset, new BoxLayout.Constraints(false, false));

			this.cmdNone = new Button();
			this.cmdNone.Text = "_None";
			this.cmdNone.Click += cmdNone_Click;
			this.cmdNone.BorderStyle = ButtonBorderStyle.None;
			this.ctSearchAndShowAll.Controls.Add(this.cmdNone, new BoxLayout.Constraints(false, false));

			this.Controls.Add(ctSearchAndShowAll, new BoxLayout.Constraints(false, true));

			this.tm = new DefaultTreeModel(new Type[] { typeof(string), typeof(string) });

			this.lv = new ListViewControl();
			lv.Columns.Add(new ListViewColumnText(tm.Columns[0], "Name"));
			lv.Columns.Add(new ListViewColumnText(tm.Columns[1], "Description"));
			lv.RowActivated += this.lv_RowActivated;
			this.lv.Model = tm;
			this.Controls.Add(this.lv, new BoxLayout.Constraints(true, true));

			this.MinimumSize = new Dimension2D(300, 200);

			StartPosition = WindowStartPosition.Manual;
		}

		public event EventHandler SelectionChanged;
		public bool AutoClose { get; set; } = true;

		protected internal override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);
			UpdateSearch();
		}

		/*
		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);
			if (mvarAutoClose) this.Close();
		}
		*/

		/*
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			UpdateSearch();
		}
		*/

		protected virtual void UpdateSearchInternal(string query)
		{
		}
		protected void UpdateSearch()
		{
			tm.Rows.Clear();
			UpdateSearchInternal(txtSearch.Text);

			if (tm.Rows.Count == 1)
			{
				lv.SelectedRows.Clear();
				lv.SelectedRows.Add(tm.Rows[0]);
			}
			// lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		protected void AddRow(TreeModelRow row)
		{
			tm.Rows.Add(row);
		}

		private void txtSearch_Changed(object sender, EventArgs e)
		{
			UpdateSearch();
		}

		protected virtual void SelectRow(TreeModelRow row)
		{
		}

		private void txtSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == KeyboardKey.Enter)
			{
				e.Cancel = true;
				if (lv.SelectedRows.Count != 1) return;

				SelectRow(lv.SelectedRows[0]);
				if (SelectionChanged != null) SelectionChanged(this, e);

				Close();
			}
			else if (e.Key == KeyboardKey.Escape)
			{
				e.Cancel = true;

				// already handled by GTK? but what about other platforms
				Close();
			}
		}

		public event EventHandler ResetList;

		protected TreeModelRowColumn CreateColumn(int index, string value)
		{
			return new TreeModelRowColumn(tm.Columns[index], value);
		}

		private void cmdReset_Click(object sender, EventArgs e)
		{
			ResetList?.Invoke(this, e);
			UpdateSearch();
		}
		private void cmdNone_Click(object sender, EventArgs e)
		{
			SelectRow(null);
			SelectionChanged?.Invoke(this, e);
			Close();
		}
		private void lv_RowActivated(object sender, ListViewRowActivatedEventArgs e)
		{
			// if (lv.SelectedItems.Count != 1) return;

			SelectRow(e.Row);
			SelectionChanged?.Invoke(this, e);

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
