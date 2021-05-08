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
		public ListViewControl lv = null;

		public AutoSuggestionWindow()
		{
			this.Layout = new BoxLayout(Orientation.Vertical);

			lv = new ListViewControl();
			lv.Model = new DefaultTreeModel(new Type[] { typeof(string) });
			lv.Columns.Add(new ListViewColumnText(lv.Model.Columns[0], "Item"));
			lv.HeaderStyle = ColumnHeaderStyle.None;

			this.Controls.Add(lv, new BoxLayout.Constraints(true, true));
		}

		protected internal override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);
			lv.Size = new MBS.Framework.Drawing.Dimension2D(400, 200);
		}

	}
}
