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
using System.Collections.Generic;
using MBS.Framework.Drawing;
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
			Font = Drawing.Font.FromFamily("Source Code Pro", new Measurement(12, MeasurementUnit.Point));

			acceptKeyModes[KeyboardKey.Escape] = SyntaxTextBox.AcceptKeyMode.Cancel;
			acceptKeyModes[KeyboardKey.Space] = SyntaxTextBox.AcceptKeyMode.Accept;
			acceptKeyModes[KeyboardKey.Enter] = SyntaxTextBox.AcceptKeyMode.Complete;
			acceptKeyModes[KeyboardKey.Tab] = SyntaxTextBox.AcceptKeyMode.Complete;
		}

		private AutoSuggestionWindow asw = null;

		private void BeforeASW()
		{
			// use gtk_text_view_get_iter_location

			int charIndex = this.SelectionStart;
			Rectangle rect = this.GetPositionFromCharIndex(charIndex);
			rect = new Rectangle(this.ClientToScreenCoordinates(rect.Location), rect.Size);

			// Vector2D curpos = this.GetCursorBounds().Location;
			asw.Location = new Vector2D(rect.X, rect.Bottom);

			string text = this.Text;

			asw.Suggestions.Clear();

			asw.Suggestions.AutoSort = false;
			foreach (SyntaxKeyword keyword in Language.Keywords)
			{
				asw.Suggestions.Add(keyword.Value);
			}
			asw.Suggestions.AutoSort = true;
			//}
		}

		public SyntaxLanguage Language { get; set; } = SyntaxLanguages.SQL;

		public static TextBoxStyleDefinition styleKeyword = new TextBoxStyleDefinition("Keyword") { ForegroundColor = Color.Parse("#719dcf") };
		public static TextBoxStyleDefinition styleBoolean = new TextBoxStyleDefinition("Boolean") { ForegroundColor = Color.Parse("#ff37f7") };

		protected internal override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);

			StyleDefinitions.Add(styleKeyword);
			StyleDefinitions.Add(styleBoolean);

			this.Text = "SELECT * FROM paster WHERE pasted_text = 'test';";
		}

		private bool _inhibit_changing = false;
		protected internal override void OnChanged(EventArgs e)
		{
			base.OnChanged(e);

			if (asw != null)
			{
				asw.FilterText = CurrentWord;
			}
			if (_inhibit_changing)
				return;

			if (Text.Length < 1)
				return;

			StyleAreas.Clear();
			foreach (SyntaxKeyword kw in Language.Keywords)
			{
				string keyword = kw.Value;
				TextBoxSearchResult[] results = FindAll(kw.Value);

				foreach (TextBoxSearchResult result in results)
				{
					TextBoxStyleArea area = new TextBoxStyleArea(kw.Type.TextBoxStyle, result.Start, result.Length);
					StyleAreas.Add(area);
				}
			}

			if (Text.Substring(Text.Length - 1, 1).Equals("\n"))
			{
				_inhibit_changing = true;
				Text = Text + "\t";
				_inhibit_changing = false;
			}
			else if (Text.Substring(Text.Length - 1, 1).Equals("}"))
			{
				if (Text.Substring(Text.Length - 2, 1).Equals("\t"))
				{
					// remove trailing tab
					_inhibit_changing = true;
					Text = Text.Substring(0, Text.Length - 2) + "}";
					_inhibit_changing = false;
				}
			}
		}

		private List<KeyboardKey> IgnoredKeys = new List<KeyboardKey>(new KeyboardKey[]
		{
			KeyboardKey.LMenu,
			KeyboardKey.RMenu,
			KeyboardKey.LControlKey,
			KeyboardKey.RControlKey,
			KeyboardKey.LShiftKey,
			KeyboardKey.RShiftKey,
			KeyboardKey.ArrowLeft,
			KeyboardKey.ArrowRight
		});

		protected internal override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (IgnoredKeys.Contains(e.Key))
			{
				return;
			}
			else if (e.Key == KeyboardKey.Back)
			{

			}
			else if (AcceptKeyMode(e.Key) == SyntaxTextBox.AcceptKeyMode.Cancel)
			{
				CancelASW();
			}
			else if (AcceptKeyMode(e.Key) == SyntaxTextBox.AcceptKeyMode.Accept)
			{
				if (AcceptASW())
				{
					asw.Visible = false;
					asw = null;
				}
			}
			else if (AcceptKeyMode(e.Key) == SyntaxTextBox.AcceptKeyMode.Complete)
			{
				if (AcceptASW())
				{
					asw.Visible = false;
					asw = null;

					e.Cancel = true;
				}
			}
			else if (e.Key == KeyboardKey.ArrowUp)
			{
				if ((asw?.Visible).GetValueOrDefault(false))
				{
					asw.SelectPrevious();
					e.Cancel = true;
					return;
				}
			}
			else if (e.Key == KeyboardKey.ArrowDown)
			{
				if ((asw?.Visible).GetValueOrDefault(false))
				{
					asw.SelectNext();
					e.Cancel = true;
					return;
				}
			}
			else
			{
				PresentASW();
			}
		}

		private Dictionary<KeyboardKey, AcceptKeyMode> acceptKeyModes = new Dictionary<KeyboardKey, AcceptKeyMode>();
		private AcceptKeyMode AcceptKeyMode(KeyboardKey key)
		{
			if (acceptKeyModes.ContainsKey(key))
				return acceptKeyModes[key];
			return SyntaxTextBox.AcceptKeyMode.Undefined;
		}

		private void PresentASW()
		{
			if (asw != null)
			{
				BeforeASW();
				if (asw.VisibleSuggestionCount > 0)
				{
					asw.Present();
				}
				else
				{
					asw.Hide();
				}
			}
			else
			{
				CreateASW();

				BeforeASW();
				if (asw.VisibleSuggestionCount > 0)
				{
					asw.Show();
					asw.Present();
				}
				else
				{
					asw.Hide();
				}
			}
		}

		private void CreateASW()
		{
			asw = new AutoSuggestionWindow();
			asw.PopupDirection = CardinalDirection.Bottom;
			asw.Owner = this;
			asw.lv.RowActivated += Lv_RowActivated;
			asw.Location = new Framework.Drawing.Vector2D(128, 300);
		}

		void Lv_RowActivated(object sender, ListViewRowActivatedEventArgs e)
		{
			AcceptASW();
		}

		private bool AcceptASW()
		{
			// close autosuggest window if we have, and put the suggested text
			if (asw != null)
			{
				if (!asw.Visible)
				{
					// return false;
				}

				if (asw.lv.SelectedRows.Count > 0)
				{
					string v = asw.lv.SelectedRows[0].RowColumns[0].Value?.ToString();
					if (v != null)
					{
						// FIXME: assumes cursor position is at end of line
						string pretext = String.Empty;
						int i = Text.LastIndexOf(' ') + 1;
						if (i > 0)
						{
							pretext = Text.Substring(0, i);
						}
						Text = pretext + v;
					}
				}

				BeforeASW();
				if (asw.Suggestions.Count == 0)
				{
					asw.Close();
					asw = null;
				}
				return true;
			}
			return false;
		}

		private void CancelASW()
		{
			if (asw != null)
			{
				asw.Close();
				asw = null;
			}
		}

	}
}
