//
//  HexEditorControl.cs
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

using MBS.Framework;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface.Controls.HexEditor
{
	public class HexEditorControl : CustomControl
	{
		private int editOffset = 0;

		public int HorizontalCellSpacing { get; set; } = 4;
		public int VerticalCellSpacing { get; set; } = 4;

		private byte[] mvarData = new byte[4096];
		public byte[] Data
		{
			get { return mvarData; }
			set
			{
				System.ComponentModel.CancelEventArgs ee = new System.ComponentModel.CancelEventArgs();
				OnChanging(ee);
				if (ee.Cancel)
					return;
				mvarData = value; SelectionStart = SelectionStart;
				OnChanged(EventArgs.Empty);
			}
		}

		public HexEditorHitTestSection SelectedSection { get; private set; } = HexEditorHitTestSection.Hexadecimal;

		public HexEditorPosition SelectionStart
		{
			get { return new HexEditorPosition(mvarSelectionStart, selectedNybble); }
			set
			{
				bool changed = (mvarSelectionStart != value.ByteIndex || selectedNybble != value.NybbleIndex);
				if (changed)
				{
					if (value.ByteIndex < 0 || ((EnableInsert && value.ByteIndex > Data.Length) || (!EnableInsert && value.ByteIndex >= Data.Length)))
					{
						Console.WriteLine(String.Format("Selection start must be between the bounds of zero and length of data ({0}) minus one", Data.Length));
					}
					else
					{
						mvarSelectionStart = value.ByteIndex;
					}

					if (value.NybbleIndex < 0 || value.NybbleIndex > 1)
					{
						Console.WriteLine(String.Format("Selected nybble must be either 0 or 1, not {0}", value.NybbleIndex));
					}
					else
					{
						selectedNybble = value.NybbleIndex;
					}

					// ensure the window gets scrolled to the line on which the current selection resides
					Rectangle cellRect = GetCellRect(SelectionStart.ByteIndex);
					// scroll to the start of the current selection
					if (cellRect.Y < VerticalAdjustment.Value)
					{
						VerticalAdjustment.Value = cellRect.Y;
					}
					else if (cellRect.Y > VerticalAdjustment.Value + Size.Height - LineHeight)
					{
						VerticalAdjustment.Value = cellRect.Y - Size.Height + LineHeight;
					}

					cursorBlinking = true;
					Refresh();
					OnSelectionChanged(EventArgs.Empty);
				}
			}
		}

		private bool _AlternateRowHighlight = true;
		/// <summary>
		/// Gets or sets a value indicating whether every other row in this <see cref="HexEditorControl" /> is highlighted with an alternate background color.
		/// </summary>
		/// <value><c>true</c> if alternating rows should be highlighted with an alternate background color; otherwise, <c>false</c>.</value>
		public bool AlternateRowHighlight { get { return _AlternateRowHighlight; } set { _AlternateRowHighlight = value; Refresh(); } }

		private bool _HighlightCurrentLine = true;
		/// <summary>
		/// Gets or sets a value indicating whether the line containing the currently-selected cell in this <see cref="HexEditorControl" /> is highlighted.
		/// </summary>
		/// <value><c>true</c> if the current line should be highlighted; otherwise, <c>false</c>.</value>
		public bool HighlightCurrentLine { get { return _HighlightCurrentLine; } set { _HighlightCurrentLine = value; Refresh(); } }

		private int GetLineIndex(int byteIndex)
		{
			return byteIndex % CellsPerLine;
		}
		private MBS.Framework.Drawing.Rectangle GetCellRect(int byteIndex)
		{
			Rectangle rectPosition = new Rectangle(xoffset, yoffset, PositionGutterWidth, CellSize);
			Rectangle rectCell = new Rectangle(rectPosition.X + rectPosition.Width + HorizontalCellSpacing, yoffset, CellSize, CellSize);
			for (int i = 0; i < byteIndex; i++)
			{
				rectCell.X += rectCell.Width + HorizontalCellSpacing;

				if (((i + 1) % CellsPerLine) == 0)
				{
					rectCell.X = rectPosition.X + rectPosition.Width + HorizontalCellSpacing;
					rectCell.Y += rectCell.Height + VerticalCellSpacing;
					rectPosition.Y += rectPosition.Height + VerticalCellSpacing;
				}
			}
			return rectCell;
		}

		private HexEditorPosition mvarSelectionLength = new HexEditorPosition(0, 0);
		public HexEditorPosition SelectionLength
		{
			get { return mvarSelectionLength; }
			set
			{
				mvarSelectionLength = value;

				Refresh();
				OnSelectionChanged(EventArgs.Empty);
			}
		}

		public event EventHandler SelectionChanged;
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			SelectionChanged?.Invoke(this, e);
		}

		public int mvarSelectionStart = 0;

		private int selectedNybble = 0; // 0 = first nybble, 1 = second nybble

		// offsets for data display
		private const int xoffset = 0;
		private const int yoffset = 16 + 16;


		public int CellSize { get { return 24; } }
		/// <summary>
		/// Gets the maximum number of cells that can be displayed in a single line.
		/// </summary>
		/// <value>The maximum number of cells that can be displayed in a single line.</value>
		public int CellsPerLine
		{
			get
			{
				int maxDisplayWidth = (int)(PageWidth / CellSize);
				if (maxDisplayWidth < 1) maxDisplayWidth = 1;// prevent division by zero
				return maxDisplayWidth;  // PageWidth / (ByteWidth + HorizontalCellSpacing + HorizontalCellSpacing);
			}
		}
		public int CellsPerColumn { get { return (PageHeight / (CellSize + VerticalCellSpacing + VerticalCellSpacing)) + 1; } }
		public int BytesPerPage {  get { return CellsPerLine * CellsPerColumn; } }

		public int PageWidth { get { return (int)(this.Size.Width - PositionGutterWidth - TextAreaWidth - 128 - HexAsciiMargin); } }
		public int PageHeight { get { return (int)this.Size.Height; } }

		public int NLines { get { return (int)Math.Round((double)Data.Length / CellsPerLine) + 1; } }
		public int LineHeight { get { return (CellSize + VerticalCellSpacing); } }

		/// <summary>
		/// Gets or sets a value indicating whether this
		/// <see cref="HexEditorControl"/> allows inserting new data.
		/// </summary>
		/// <value><c>true</c> if inserting data is allowed; otherwise, <c>false</c>.</value>
		public bool EnableInsert { get; set; } = true;

		public HexEditorBackspaceBehavior BackspaceBehavior { get; set; } = HexEditorBackspaceBehavior.EraseNybble;
		/// <summary>
		/// Gets or sets a value indicating whether the backspace or delete key clears a byte instead of deleting it.
		/// </summary>
		/// <value><c>true</c> if backspace deletes; otherwise, <c>false</c>.</value>
		public bool BackspaceDeletes { get; set; } = true;

		public HexEditorControl()
		{
			HighlightAreas = new HexEditorHighlightArea.HexEditorHighlightAreaCollection(this);
			MinimumSize = new Framework.Drawing.Dimension2D(320, 240);
		}

		public HexEditorHighlightArea.HexEditorHighlightAreaCollection HighlightAreas { get; private set; } = null;

		int PositionGutterWidth = 72;
		int TextAreaWidth = 128;

		protected internal override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);

			_tCursorBlinkThread = new System.Threading.Thread(_tCursorBlinkThread_ThreadStart);
			_tCursorBlinkThread.Start();
		}

		public HexEditorHitTestInfo HitTest(double x, double y)
		{
			int byteIndex = -1, nybbleIndex = -1;
			Rectangle rectPosition = new Rectangle(xoffset, yoffset, PositionGutterWidth, CellSize);
			Rectangle rectCell = new Rectangle(rectPosition.X + rectPosition.Width + HorizontalCellSpacing, yoffset, CellSize, CellSize);

			for (int i = 0; i < mvarData.Length + 1; i++)
			{
				Rectangle rectChar = new Rectangle(this.Size.Width - TextAreaWidth - PositionGutterWidth + ((i % CellsPerLine) * 8), rectCell.Y, 8, CellSize);

				if (x >= rectCell.X && x <= rectCell.Right && y >= rectCell.Y && y <= rectCell.Bottom)
				{
					byteIndex = i;
					if (x >= (rectCell.X + (rectCell.Width / 2)))
					{
						nybbleIndex = 1;
					}
					else
					{
						nybbleIndex = 0;
					}
					return new HexEditorHitTestInfo(byteIndex, nybbleIndex, HexEditorHitTestSection.Hexadecimal);
				}
				else if (x >= rectChar.X && x <= rectChar.Right && y >= rectChar.Y && y <= rectChar.Bottom)
				{
					byteIndex = i;
					nybbleIndex = 0;
					return new HexEditorHitTestInfo(byteIndex, nybbleIndex, HexEditorHitTestSection.ASCII);
				}

				rectCell.X += rectCell.Width + HorizontalCellSpacing;

				if (((i + 1) % CellsPerLine) == 0)
				{
					rectCell.X = rectPosition.X + rectPosition.Width + HorizontalCellSpacing;
					rectCell.Y += rectCell.Height + VerticalCellSpacing;
					rectPosition.Y += rectPosition.Height + VerticalCellSpacing;
				}
			}
			return new HexEditorHitTestInfo(byteIndex, nybbleIndex, HexEditorHitTestSection.Hexadecimal);
		}

		protected internal override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Focus();

			if (e.Buttons == MouseButtons.Primary || SelectionLength == 0)
			{
				HexEditorHitTestInfo index = HitTest(e.X, e.Y);
				if (index.ByteIndex > -1)
				{
					SelectionStart = new HexEditorPosition(index.ByteIndex, index.NybbleIndex);
					SelectionLength = new HexEditorPosition(0, 0);
					SelectedSection = index.Section;
					Refresh();
				}
			}
		}
		protected internal override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (e.Buttons == MouseButtons.Primary)
			{
				HexEditorHitTestInfo index = HitTest(e.X, e.Y);
				if (index.ByteIndex > -1)
				{
					if (index.ByteIndex >= Data.Length)
					{
						SelectionLength = Data.Length - SelectionStart;
						SelectionLength = new HexEditorPosition(SelectionLength.ByteIndex, 1);
					}
					else
					{
						SelectionLength = new HexEditorPosition(index.ByteIndex - mvarSelectionStart + 1, index.NybbleIndex);
					}
				}
			}

			if (e.X > this.Size.Width - TextAreaWidth - PositionGutterWidth)
			{
				Cursor = Cursors.Text;
			}
			else
			{
				Cursor = Cursors.Default;
			}
		}

		private bool shouldSwitchSectionOnFocus = false;

		protected internal override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);

			if (shouldSwitchSectionOnFocus)
			{
				SelectedSection = HexEditorHitTestSection.Hexadecimal;
				shouldSwitchSectionOnFocus = false;
			}
		}

		private bool ShouldBackspaceDelete(KeyboardModifierKey modifierKeys)
		{
			return ((BackspaceDeletes && ((modifierKeys & KeyboardModifierKey.Shift) != KeyboardModifierKey.Shift))
				|| (BackspaceDeletes && ((modifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)));
		}

		protected internal override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			// we should learn how to do this mathematically...
			char next = '\0';

			switch (e.Key)
			{
				case KeyboardKey.Tab:
				{
					if (SelectedSection == HexEditorHitTestSection.ASCII)
					{
						// do nothing; let the owner handle the tab to next window
						// but we should switch the section on the next focus
						shouldSwitchSectionOnFocus = true;
					}
					else if (SelectedSection == HexEditorHitTestSection.Hexadecimal)
					{
						SelectedSection = HexEditorHitTestSection.ASCII;
						e.Cancel = true;
					}
					return;
				}
				case KeyboardKey.Back:
				{
					if (!Editable || SelectionStart < 0)
					{
						return;
					}

					if (SelectionLength > 1)
					{
						if (ShouldBackspaceDelete(e.ModifierKeys))
						{
							byte[] old = Data;
							byte[] _new = new byte[old.Length - SelectionLength];
							Array.Copy(old, 0, _new, 0, SelectionStart);
							Array.Copy(old, SelectionStart + SelectionLength, _new, SelectionStart, old.Length - SelectionStart - SelectionLength);
							Data = _new;

							SelectionLength = 0;
							Refresh();
							e.Cancel = true;
						}
					}
					else
					{
						if (BackspaceBehavior == HexEditorBackspaceBehavior.EraseByte || SelectedSection == HexEditorHitTestSection.ASCII)
						{
							// ASCII section ALWAYS erases a whole byte
							if (SelectionStart > 0)
							{
								if (ShouldBackspaceDelete(e.ModifierKeys))
								{
									if (mvarSelectionLength == 0)
									{
										ArrayExtensions.Array_RemoveAt<byte>(ref mvarData, mvarSelectionStart - 1, mvarSelectionLength + 1);
										mvarSelectionStart--;
									}
									else
									{
										ArrayExtensions.Array_RemoveAt<byte>(ref mvarData, SelectionStart.ByteIndex, SelectionLength);
										if (SelectionLength < 0)
										{
											SelectionStart += SelectionLength;
										}
									}
								}
								else // if (BackspaceBehavior == HexEditorBackspaceBehavior.EraseByte)
								{
									Data[SelectionStart - 1] = 0x0;
								}
								SelectionLength = 0;
								e.Cancel = true;
							}
						}
						else if (BackspaceBehavior == HexEditorBackspaceBehavior.EraseNybble)
						{
							if (SelectionStart.ByteIndex == 0 && SelectionStart.NybbleIndex == 0)
							{
								return;
							}
							if (SelectionStart.NybbleIndex == 0)
							{
								string curhex = Data[mvarSelectionStart - 1].ToString("X").PadLeft(2, '0');
								Data[mvarSelectionStart - 1] = Byte.Parse(curhex.Substring(0, 1) + '0', System.Globalization.NumberStyles.HexNumber);

								if (ShouldBackspaceDelete(e.ModifierKeys))
								{
									ArrayExtensions.Array_RemoveAt<byte>(ref mvarData, mvarSelectionStart - 1);
									SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex, 0);
								}
								else
								{
									SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex, 1);
								}

								SelectionStart--;
							}
							else if (SelectionStart.NybbleIndex == 1)
							{
								if (SelectionStart < Data.Length)
								{
									string curhex = Data[mvarSelectionStart].ToString("X").PadLeft(2, '0');
									Data[mvarSelectionStart] = Byte.Parse('0' + curhex.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
								}
								selectedNybble = 0;
							}
							SelectionLength = 0;

							e.Cancel = true;
						}
					}
					return;
				}
				case KeyboardKey.ArrowLeft:
				{
					if (SelectedSection == HexEditorHitTestSection.Hexadecimal)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							SelectionLength -= 0.5;
						}
						else
						{
							SelectionLength = 0;
							if (SelectionStart.NybbleIndex == 1)
							{
								SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex, 0);
							}
							else if (selectedNybble == 0)
							{
								SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex - 1, 1);
							}

							if (SelectionStart < 0)
							{
								SelectionStart = 0;
							}
						}
					}
					else
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							SelectionLength -= 1;
						}
						else
						{
							SelectionLength = 0;
							SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex - 1, 0);

							if (SelectionStart < 0)
							{
								SelectionStart = 0;
							}
						}
					}
					e.Cancel = true;
					break;
				}
				case KeyboardKey.ArrowRight:
				{
					if (SelectedSection == HexEditorHitTestSection.Hexadecimal)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							SelectionLength += 0.5;
						}
						else
						{
							SelectionLength = 0;
							if (selectedNybble == 1)
							{
								SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex + 1, 0);
							}
							else if (selectedNybble == 0)
							{
								SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex, 1);
							}

							int end = Editable ? Data.Length : Data.Length - 1;
							if (SelectionStart > end)
							{
								SelectionStart = new HexEditorPosition(end, 1);
							}
						}
					}
					else
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							SelectionLength += 1;
						}
						else
						{
							SelectionLength = 0;
							SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex + 1, 0);

							int end = Editable ? Data.Length : Data.Length - 1;
							if (mvarSelectionStart > end)
							{
								SelectionStart = new HexEditorPosition(end, 0);
							}
						}
					}
					e.Cancel = true;
					break;
				}
				case KeyboardKey.ArrowUp:
				{
					if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
					{
						SelectionLength -= CellsPerLine;
					}
					else
					{
						SelectionLength = 0;
						if (mvarSelectionStart - CellsPerLine >= 0)
						{
							SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex - CellsPerLine, SelectionStart.NybbleIndex);
						}
					}
					e.Cancel = true;
					break;
				}
				case KeyboardKey.ArrowDown:
				{
					if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
					{
						SelectionLength += CellsPerLine;
					}
					else
					{
						SelectionLength = 0;
						SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex + CellsPerLine, SelectionStart.NybbleIndex);

						if (SelectionStart >= Data.Length)
							SelectionStart = Data.Length - 1;

					}
					e.Cancel = true;
					break;
				}
				case KeyboardKey.Home:
				{
					// let's try something different
					SelectionLength = 0;
					if ((e.ModifierKeys & KeyboardModifierKey.Control) == KeyboardModifierKey.Control)
					{
						SelectionStart = 0;
					}
					else
					{
						SelectionStart = (int)((double)mvarSelectionStart / (double)CellsPerLine) * CellsPerLine;
					}
					e.Cancel = true;
					break;
				}
				case KeyboardKey.End:
				{
					// let's try something different
					SelectionLength = 0;
					if ((e.ModifierKeys & KeyboardModifierKey.Control) == KeyboardModifierKey.Control)
					{
						SelectionStart = new HexEditorPosition(Data.Length - 1, 1);
					}
					else
					{
						SelectionStart = new HexEditorPosition(Math.Min(Data.Length - 1, (((int)((double)mvarSelectionStart / (double)CellsPerLine) * CellsPerLine) + CellsPerLine) - 1), 1);
					}
					e.Cancel = true;
					break;
				}
			}

			if (Editable)
			{
				if (SelectedSection == HexEditorHitTestSection.Hexadecimal)
				{
					if ((int)e.Key >= (int)KeyboardKey.D0 && (int)e.Key <= (int)KeyboardKey.D9)
					{
						next = (char)((int)'0' + ((int)e.Key - (int)KeyboardKey.D0));
					}
					else if ((int)e.Key >= (int)KeyboardKey.NumPad0 && (int)e.Key <= (int)KeyboardKey.NumPad9)
					{
						next = (char)((int)'0' + ((int)e.Key - (int)KeyboardKey.NumPad0));
					}
					else if ((int)e.Key >= (int)KeyboardKey.A && (int)e.Key <= (int)KeyboardKey.F)
					{
						next = (char)((int)'A' + ((int)e.Key - (int)KeyboardKey.A));
					}
				}
				else if (SelectedSection == HexEditorHitTestSection.ASCII)
				{
					if (((int)e.Key >= (int)KeyboardKey.D0 && (int)e.Key <= (int)KeyboardKey.D9) && e.ModifierKeys == KeyboardModifierKey.None)
					{
						next = (char)((int)'0' + ((int)e.Key - (int)KeyboardKey.D0));
					}
					else if ((int)e.Key >= (int)KeyboardKey.NumPad0 && (int)e.Key <= (int)KeyboardKey.NumPad9)
					{
						next = (char)((int)'0' + ((int)e.Key - (int)KeyboardKey.NumPad0));
					}
					else if ((int)e.Key >= (int)KeyboardKey.A && (int)e.Key <= (int)KeyboardKey.Z)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = (char)((int)'A' + ((int)e.Key - (int)KeyboardKey.A));
						}
						else
						{
							next = (char)((int)'a' + ((int)e.Key - (int)KeyboardKey.A));
						}
					}
					else if (e.Key == KeyboardKey.Space)
					{
						next = ' ';
					}
					else if (e.Key == KeyboardKey.Tilde)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '~';
						}
						else
						{
							next = '`';
						}
					}
					else if (e.Key == KeyboardKey.Backslash)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '|';
						}
						else
						{
							next = '\\';
						}
					}
					else if (e.Key == KeyboardKey.Pipe)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '|';
						}
						else
						{
							next = '\\';
						}
					}
					else if (e.Key == KeyboardKey.Question)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '?';
						}
						else
						{
							next = '/';
						}
					}
					else if (e.Key == KeyboardKey.OpenBrackets)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '{';
						}
						else
						{
							next = '[';
						}
					}
					else if (e.Key == KeyboardKey.CloseBrackets)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '}';
						}
						else
						{
							next = ']';
						}
					}
					else if (e.Key == KeyboardKey.Minus)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '_';
						}
						else
						{
							next = '-';
						}
					}
					else if (e.Key == KeyboardKey.Plus)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '+';
						}
						else
						{
							next = '=';
						}
					}
					else if (e.Key == KeyboardKey.Semicolon)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = ':';
						}
						else
						{
							next = ';';
						}
					}
					else if (e.Key == KeyboardKey.Quotes)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '"';
						}
						else
						{
							next = '\'';
						}
					}
					else if (e.Key == KeyboardKey.Period)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '>';
						}
						else
						{
							next = '.';
						}
					}
					else if (e.Key == KeyboardKey.Comma)
					{
						if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
						{
							next = '<';
						}
						else
						{
							next = ',';
						}
					}
					else if ((e.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
					{
						// special case certain !@#$% we're having trouble with
						switch (e.Key)
						{
							case KeyboardKey.D0: next = ')'; break;
							case KeyboardKey.D1: next = '!'; break;
							case KeyboardKey.D2: next = '@'; break;
							case KeyboardKey.D3: next = '#'; break;
							case KeyboardKey.D4: next = '$'; break;
							case KeyboardKey.D5: next = '%'; break;
							case KeyboardKey.D6: next = '^'; break;
							case KeyboardKey.D7: next = '&'; break;
							case KeyboardKey.D8: next = '*'; break;
							case KeyboardKey.D9: next = '('; break;
						}
					}
				}
			}

			if (next != '\0')
			{
				System.ComponentModel.CancelEventArgs ee = new System.ComponentModel.CancelEventArgs();
				OnChanging(ee);
				if (ee.Cancel)
					return;

				if (SelectionStart.ByteIndex >= Data.Length)
				{
					if (EnableInsert)
					{
						Array.Resize<byte>(ref mvarData, mvarData.Length + 1);
					}
				}

				if (SelectedSection == HexEditorHitTestSection.Hexadecimal)
				{
					string curhex = Data[mvarSelectionStart].ToString("X").PadLeft(2, '0');
					mvarSelectionLength = 0;
					if (SelectionStart.NybbleIndex == 0)
					{
						Data[SelectionStart.ByteIndex] = Byte.Parse(next.ToString() + curhex.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
						SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex, 1);
					}
					else if (SelectionStart.NybbleIndex == 1)
					{
						Data[mvarSelectionStart] = Byte.Parse(curhex.Substring(0, 1) + next.ToString(), System.Globalization.NumberStyles.HexNumber);

						SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex + 1, 0);
					}
				}
				else
				{
					Data[SelectionStart.ByteIndex] = (byte)next;
					SelectionStart = new HexEditorPosition(SelectionStart.ByteIndex + 1, 0);
				}

				OnChanged(EventArgs.Empty);
				e.Cancel = true;
			}
		}

		public event System.ComponentModel.CancelEventHandler Changing;
		protected virtual void OnChanging(System.ComponentModel.CancelEventArgs e)
		{
			Changing?.Invoke(this, e);
		}

		public event EventHandler Changed;
		protected virtual void OnChanged(EventArgs e)
		{
			Changed?.Invoke(this, e);
		}

		private bool cursorBlinking = true;
		public bool BlinkCursor { get; set; } = true;

		private System.Threading.Thread _tCursorBlinkThread = null;
		private void _tCursorBlinkThread_ThreadStart()
		{
			while (!((UIApplication)Application.Instance).Exited)
			{
				System.Threading.Thread.Sleep(((UIApplication)Application.Instance).Engine.SystemSettings.CursorBlinkTime);
				cursorBlinking = !cursorBlinking;

				if (IsCreated)
					Invalidate(); // replace with call to Invalidate(Rect)
			}
		}

		protected internal override void OnUnrealize(EventArgs e)
		{
			base.OnUnrealize(e);
			if (_tCursorBlinkThread != null)
				_tCursorBlinkThread.Abort();
		}

		public bool Editable { get; set; } = true;

		private static readonly int HexAsciiMargin = 24;

		private static readonly Vector2D textOffset = new Vector2D(4, 16);

		private Pen pSelectionBorderUnfocused = new Pen(Colors.Gray);
		private Brush bSelectionBorderUnfocused = new SolidBrush(Colors.Gray);
		private Brush bSelectionBackgroundUnfocused = new SolidBrush(Colors.LightGray);

		protected internal override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.Clear(SystemColors.WindowBackground);
			ScrollBounds = new MBS.Framework.Drawing.Dimension2D(0, NLines * LineHeight);

			Brush bOffsetColor = new SolidBrush(SystemColors.HighlightBackground);

			
			Pen pSelectionBorderFocused = new Pen(SystemColors.HighlightBackground);
			Brush bSelectionBorderFocused = new SolidBrush(SystemColors.HighlightBackground);
			Brush bSelectionBackgroundFocused = new SolidBrush(SystemColors.HighlightBackground);


			Brush bForeColor = new SolidBrush(SystemColors.WindowForeground);

			Font font = SystemFonts.Monospace;

			int start = 0; //(int)(VerticalAdjustment.Value / LineHeight) * CellsPerLine;
			if (start < 0) start = 0;

			int end = mvarData.Length;
			if (Editable)
				end++;

			Rectangle rectPosition = new Rectangle(xoffset, yoffset, PositionGutterWidth, CellSize);
			Rectangle rectCell = new Rectangle(rectPosition.X + rectPosition.Width, yoffset, CellSize, CellSize);

			for (int i = 0; i < CellsPerLine; i++)
			{
				e.Graphics.DrawText(i.ToString("X").PadLeft(2, '0'), font, new Rectangle(rectCell.X + textOffset.X + (i * (CellSize + HorizontalCellSpacing)), VerticalAdjustment.Value + yoffset - 8, CellSize, CellSize), bOffsetColor);
			}

			Console.WriteLine("render cell range: {0} (0x{1}) to {2} (0x{3})", start, start.ToString("x"), end, end.ToString("x"));
			for (int i = start; i < end; i++)
			{
				Rectangle rectPositionText = new Rectangle(rectPosition.X + textOffset.X, rectPosition.Y + textOffset.Y, rectPosition.Width - (textOffset.X * 2), rectPosition.Height - (textOffset.Y * 2));
				Rectangle rectCellText = new Rectangle(rectCell.X + textOffset.X, rectCell.Y + textOffset.Y, rectCell.Width - (textOffset.X * 2), rectCell.Height - (textOffset.Y * 2));
				Rectangle rectFirstNybbleCell = new Rectangle(rectCell.X, rectCell.Y, rectCell.Width / 2, rectCell.Height);
				Rectangle rectNybbleCell = new Rectangle(rectCell.X + ((rectCell.Width / 2) * selectedNybble), rectCell.Y, rectCell.Width / 2, rectCell.Height);

				Rectangle rectChar = new Rectangle(this.Size.Width - TextAreaWidth - PositionGutterWidth + ((i % CellsPerLine) * 8), rectCell.Y, 8, rectCell.Height);
				Rectangle rectCharText = new Rectangle(rectChar.X, rectChar.Y + textOffset.Y, rectChar.Width, rectChar.Height);

				bool isFirstCellInLine = (i % CellsPerLine) == 0;
				if (isFirstCellInLine && AlternateRowHighlight && (int)(((double)i / CellsPerLine) % 2) == 0)
				{
					e.Graphics.FillRectangle(new SolidBrush(Colors.DarkGray.Alpha(0.2)), new Rectangle(rectCell.X, rectCell.Y, this.Size.Width, rectCell.Height));
				}

				bool hasAreaFill = false;
				for (int j = HighlightAreas.Count - 1;  j > -1;  j--)
				{
					HexEditorHighlightArea area = HighlightAreas[j];
					if (i >= area.Start && i < (area.Start + area.Length))
					{
						if (area.BackColor != Color.Empty)
						{
							e.Graphics.FillRectangle(new SolidBrush(area.BackColor), rectCell);
							e.Graphics.FillRectangle(new SolidBrush(area.BackColor), rectChar);

							if (area.ForeColor != Color.Empty)
							{
								if ((bForeColor as SolidBrush).Color != area.ForeColor)
								{
									bForeColor = new SolidBrush(area.ForeColor);
								}
							}
							else
							{
								if ((bForeColor as SolidBrush).Color != SystemColors.WindowForeground)
								{
									bForeColor = new SolidBrush(SystemColors.WindowForeground);
								}
							}
						}
						else
						{
							if ((bForeColor as SolidBrush).Color != SystemColors.WindowForeground)
							{
								bForeColor = new SolidBrush(SystemColors.WindowForeground);
							}
						}
						hasAreaFill = true;
						break;
					}
					else
					{
						if ((bForeColor as SolidBrush).Color != SystemColors.WindowForeground)
						{
							bForeColor = new SolidBrush(SystemColors.WindowForeground);
						}
					}
				}

				int selstart = mvarSelectionStart;
				int sellength = mvarSelectionLength.ByteIndex;
				if (sellength < 0)
				{
					sellength = Math.Abs(sellength);
					selstart = mvarSelectionStart - sellength;
				}
				if (HighlightCurrentLine && i == selstart)
				{
					e.Graphics.FillRectangle(new SolidBrush(SystemColors.HighlightBackground.Alpha(0.2)), new Rectangle(0, rectCell.Y, this.Size.Width, rectCell.Height));
				}

				if (i >= selstart && i <= (selstart + sellength - 1))
				{
					if (mvarSelectionLength.NybbleIndex == 1 || i < (selstart + sellength - 1))
					{
						if (!hasAreaFill)
						{
							e.Graphics.FillRectangle(bSelectionBackgroundFocused, rectCell);
						}
						e.Graphics.DrawRectangle(SelectedSection == HexEditorHitTestSection.Hexadecimal ? pSelectionBorderFocused : pSelectionBorderUnfocused, rectCell);
					}
					else if (mvarSelectionLength.NybbleIndex == 0)
					{
						if (!hasAreaFill)
						{
							e.Graphics.FillRectangle(SelectedSection == HexEditorHitTestSection.Hexadecimal ? bSelectionBackgroundFocused : bSelectionBackgroundUnfocused, rectFirstNybbleCell);
						}
						e.Graphics.DrawRectangle(SelectedSection == HexEditorHitTestSection.Hexadecimal ? pSelectionBorderFocused : pSelectionBorderUnfocused, rectFirstNybbleCell);
					}
					bForeColor = new SolidBrush(SystemColors.HighlightForeground);
				}
				else
				{
					bForeColor = new SolidBrush(SystemColors.WindowForeground);
				}

				if ((i % CellsPerLine) == 0)
				{
					// print the offset of data, once per line
					string strOffset = i.ToString("X").PadLeft(8, '0');
					e.Graphics.DrawText(strOffset, font, rectPositionText, bOffsetColor);
				}

				if (i == mvarSelectionStart)
				{
					e.Graphics.DrawRectangle(SelectedSection == HexEditorHitTestSection.Hexadecimal ? pSelectionBorderFocused : pSelectionBorderUnfocused, rectCell);
					if (!BlinkCursor || cursorBlinking || !Editable)
						e.Graphics.FillRectangle(SelectedSection == HexEditorHitTestSection.Hexadecimal ? bSelectionBorderFocused : bSelectionBorderUnfocused, new Rectangle(rectNybbleCell.X, rectNybbleCell.Bottom - 4, rectNybbleCell.Width, 4));
				}

				if (i < mvarData.Length)
				{
					string hex = mvarData[i].ToString("X").PadLeft(2, '0');
					e.Graphics.DrawText(hex, font, rectCellText, bForeColor);
				}

				rectCell.X += rectCell.Width + HorizontalCellSpacing;

				if (mvarData.Length > 0 && i < end)
				{
					if (i >= selstart && i <= selstart + sellength)
					{
						if (i < selstart + sellength)
						{
							// highlight
							e.Graphics.FillRectangle(SelectedSection == HexEditorHitTestSection.ASCII ? bSelectionBackgroundFocused : bSelectionBackgroundUnfocused, new Rectangle(rectChar.X, rectChar.Y, rectChar.Width, rectChar.Height));
						}
						if (i == selstart)
						{
							// cursor
							Rectangle cursorRect = new Rectangle(rectChar.X, rectChar.Bottom - 4, rectChar.Width, 4);
							e.Graphics.FillRectangle(SelectedSection == HexEditorHitTestSection.ASCII ? bSelectionBorderFocused : bSelectionBorderUnfocused, cursorRect);
						}
					}

					if (i < mvarData.Length)
					{
						if (!Char.IsControl((char)mvarData[i])) // (mvarData[i] > 31 && mvarData[i] < 128)
						{
							e.Graphics.DrawText(((char)mvarData[i]).ToString(), font, rectCharText, bForeColor);
						}
						else
						{
							e.Graphics.DrawText(".", font, rectCharText, bForeColor);
						}
					}
				}

				if (((i + 1) % CellsPerLine) == 0)
				{
					rectCell.X = rectPosition.X + rectPosition.Width;
					rectCell.Y += rectCell.Height + VerticalCellSpacing;
					rectPosition.Y += rectPosition.Height + VerticalCellSpacing;
				}

				if (rectPosition.Y > this.Size.Height + VerticalAdjustment.Value)
				{
					break;
				}
			}
		}
	}
}
