//
//  SyntaxTextBox.cs
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
using MBS.Framework.UserInterface.Controls.ListView;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.SyntaxTextBox
{
	public class SyntaxTextBoxControl : TextBox
	{
		public SyntaxTextBoxControl()
		{
			Multiline = true;
		}

		private AutoSuggestionWindow asw = null;

		private void BeforeASW()
		{
			string text = this.Text;

			if (text.EndsWith("public "))
			{
				asw.lv.Model.Rows.Add(new TreeModelRow(new TreeModelRowColumn[] { new TreeModelRowColumn(asw.lv.Model.Columns[0], "class") }));
			}
			else
			{
				asw.lv.Model.Rows.Add(new TreeModelRow(new TreeModelRowColumn[] { new TreeModelRowColumn(asw.lv.Model.Columns[0], "public") }));
				asw.lv.Model.Rows.Add(new TreeModelRow(new TreeModelRowColumn[] { new TreeModelRowColumn(asw.lv.Model.Columns[0], "private") }));
				asw.lv.Model.Rows.Add(new TreeModelRow(new TreeModelRowColumn[] { new TreeModelRowColumn(asw.lv.Model.Columns[0], "internal") }));
			}
		}

		protected internal override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Key == KeyboardKey.LMenu || e.Key == KeyboardKey.RMenu || e.Key == KeyboardKey.LControlKey || e.Key == KeyboardKey.RControlKey || e.Key == KeyboardKey.LShiftKey || e.Key == KeyboardKey.RShiftKey)
			{

			}
			else if (e.Key == KeyboardKey.Enter)
			{
				AcceptASW();

				// determine if we need to indent
				// if (CodeFormatter.ShouldIndent(...))
				SelectedText = "\t";
				e.Cancel = true;
			}
			else if (e.Key == KeyboardKey.ArrowUp)
			{
			}
			else if (e.Key == KeyboardKey.ArrowDown)
			{
			}
			else
			{
				if (asw != null)
				{
					BeforeASW();
					asw.Present();
				}
				else
				{
					CreateASW();

					BeforeASW();
					asw.Show();
					asw.Present();
				}
			}
		}

		private void CreateASW()
		{
			asw = new AutoSuggestionWindow();
			asw.PopupDirection = CardinalDirection.Bottom;
			asw.Owner = this;
			asw.lv.RowActivated += Lv_RowActivated;
		}

		void Lv_RowActivated(object sender, ListViewRowActivatedEventArgs e)
		{
			AcceptASW();
		}

		private void AcceptASW()
		{
			// close autosuggest window if we have, and put the suggested text
			if (asw != null)
			{
				if (asw.lv.SelectedRows.Count > 0)
				{
					string v = asw.lv.SelectedRows[0].RowColumns[0].Value?.ToString();
					if (v != null)
					{
						Text += v;
					}
				}

				asw.Close();
				asw = null;
			}
		}

	}
}

