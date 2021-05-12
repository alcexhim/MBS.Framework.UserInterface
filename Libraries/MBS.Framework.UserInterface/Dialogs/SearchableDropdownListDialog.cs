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

			this.lv = new ListViewControl();
			this.lv.Model = new DefaultTreeModel(new Type[] { typeof(string), typeof(string) });

			lv.Columns.Add(new ListViewColumn("Name", new CellRenderer[]
			{
				new CellRendererText(lv.Model.Columns[0])
			}));
			lv.Columns.Add(new ListViewColumn("Description", new CellRenderer[]
			{
				new CellRendererText(lv.Model.Columns[1])
			}));
			lv.RowActivated += this.lv_RowActivated;
			this.Controls.Add(this.lv, new BoxLayout.Constraints(true, true));

			this.MinimumSize = new Dimension2D(300, 200);

			StartPosition = WindowStartPosition.Manual;
		}

		public event EventHandler SelectionChanged;
		public bool AutoClose { get; set; } = true;

		public bool ShowSearch {  get { return txtSearch.Visible; } set { txtSearch.Visible = value; } }
		public string Query
		{
			get { return txtSearch.Text; }
			set { txtSearch.Text = value; }
		}

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

		private string[] _Columns = null;
		public string[] Columns
		{
			get { return _Columns; }
			set
			{
				_Columns = value;

				Type[] types = new Type[value.Length];
				for (int i = 0; i < value.Length; i++)
				{
					types[i] = typeof(string);
				}
				lv.Model = new DefaultTreeModel(types);

				lv.Columns.Clear();
				for (int i = 0; i < value.Length; i++)
				{
					lv.Columns.Add(new ListViewColumn(value[i], new CellRenderer[] { new CellRendererText(lv.Model.Columns[i]) }));
				}
			}
		}

		public event EventHandler<UpdateDropDownListEventArgs> Update;

		protected virtual void UpdateSearchInternal(string query)
		{
			UpdateDropDownListEventArgs e = new UpdateDropDownListEventArgs(query);
			Update?.Invoke(this, e);

			if (!e.Cancel)
			{
				for (int i = 0; i < e.Items.Count; i++)
				{
					TreeModelRow row = new TreeModelRow();
					for (int j = 0; j < e.Items[i].Length; j++)
					{
						row.RowColumns.Add(new TreeModelRowColumn(lv.Model.Columns[j], e.Items[i][j]));
					}
					AddRow(row);
				}
			}
		}
		protected void UpdateSearch()
		{
			lv.Model.Rows.Clear();
			UpdateSearchInternal(txtSearch.Text);

			if (lv.Model.Rows.Count == 1)
			{
				lv.SelectedRows.Clear();
				lv.SelectedRows.Add(lv.Model.Rows[0]);
			}
			// lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		protected void AddRow(TreeModelRow row)
		{
			lv.Model.Rows.Add(row);
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
			return new TreeModelRowColumn(lv.Model.Columns[index], value);
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
