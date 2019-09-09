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
using MBS.Framework.Drawing;
using UniversalWidgetToolkit.Drawing;
using UniversalWidgetToolkit.Input.Keyboard;
using UniversalWidgetToolkit.Input.Mouse;

namespace UniversalWidgetToolkit.Controls.HexEditor
{
	public class HexEditorControl : CustomControl
	{
		private int editOffset = 0;

		public int HorizontalCellSpacing { get; set; } = 4;
		public int VerticalCellSpacing { get; set; } = 4;

		private byte[] mvarData = new byte[4096];
		public byte[] Data { get { return mvarData; } set { mvarData = value; Refresh(); } }

		public HexEditorPosition SelectionStart
		{
			get { return new HexEditorPosition(mvarSelectionStart, selectedNybble); }
			set
			{
				if (value.ByteIndex < 0 || value.ByteIndex >= Data.Length)
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

				Refresh();
				OnSelectionChanged(EventArgs.Empty);
			}
		}

		public event EventHandler SelectionChanged;
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			SelectionChanged?.Invoke(this, e);
		}

		public int mvarSelectionStart = 0; // where the selection starts

		private int selectedNybble = 0; // 0 = first nybble, 1 = second nybble

		// The maximum number of cells we can display in a single line
		private int mvarMaxDisplayWidth = 16;

		// offsets for data display
		private int xoffset = 0;
		private int yoffset = 16;

		public HexEditorBackspaceBehavior BackspaceBehavior { get; set; } = HexEditorBackspaceBehavior.EraseNybble;

		public HexEditorControl()
		{
			HighlightAreas = new HexEditorHighlightArea.HexEditorHighlightAreaCollection(this);
		}

		public HexEditorHighlightArea.HexEditorHighlightAreaCollection HighlightAreas { get; private set; } = null;

		int PositionGutterWidth = 72;
		int TextAreaWidth = 128;

		public HexEditorHitTestInfo HitTest(double x, double y)
		{
			int byteIndex = -1, nybbleIndex = -1;
			Rectangle rectPosition = new Rectangle(xoffset, yoffset, PositionGutterWidth, 24);
			Rectangle rectCell = new Rectangle(rectPosition.X + rectPosition.Width + HorizontalCellSpacing, yoffset, 24, 24);

			for (int i = 0; i < mvarData.Length; i++)
			{
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
					return new HexEditorHitTestInfo(byteIndex, nybbleIndex);
				}

				rectCell.X += rectCell.Width + HorizontalCellSpacing;

				if (((i + 1) % mvarMaxDisplayWidth) == 0)
				{
					rectCell.X = rectPosition.X + rectPosition.Width + HorizontalCellSpacing;
					rectCell.Y += rectCell.Height + VerticalCellSpacing;
					rectPosition.Y += rectPosition.Height + VerticalCellSpacing;
				}
			}
			return new HexEditorHitTestInfo(byteIndex, nybbleIndex);
		}

		protected internal override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Focus();

			HexEditorHitTestInfo index = HitTest(e.X, e.Y);
			if (index.ByteIndex > -1)
			{
				mvarSelectionStart = index.ByteIndex;
				selectedNybble = index.NybbleIndex;

				SelectionStart = SelectionStart;
				Refresh();
			}
		}
		protected internal override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
		}

		protected internal override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			// we should learn how to do this mathematically...
			char next = '\0';

			switch (e.Key)
			{
				case KeyboardKey.Back:
				{
					if (BackspaceBehavior == HexEditorBackspaceBehavior.EraseByte)
					{
						if (mvarSelectionStart > 0)
						{
							Data[mvarSelectionStart - 1] = 0x0;
							mvarSelectionStart--;

							SelectionStart = SelectionStart; // to fire events
							Refresh();
							e.Cancel = true;
						}
					}
					else if (BackspaceBehavior == HexEditorBackspaceBehavior.EraseNybble)
					{
						if (selectedNybble == 0)
						{
							string curhex = Data[mvarSelectionStart - 1].ToString("X").PadLeft(2, '0');
							Data[mvarSelectionStart - 1] = Byte.Parse(curhex.Substring(0, 1) + '0', System.Globalization.NumberStyles.HexNumber);
							mvarSelectionStart--;
							selectedNybble = 1;
						}
						else if (selectedNybble == 1)
						{
							string curhex = Data[mvarSelectionStart].ToString("X").PadLeft(2, '0');
							Data[mvarSelectionStart] = Byte.Parse('0' + curhex.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
							selectedNybble = 0;
						}

						SelectionStart = SelectionStart; // to fire events
						Refresh();
						e.Cancel = true;
					}
					return;
				}
				case KeyboardKey.ArrowLeft:
				{
					if (selectedNybble == 1)
					{
						selectedNybble = 0;
					}
					else if (selectedNybble == 0)
					{
						mvarSelectionStart--;
						selectedNybble = 1;
					}

					if (mvarSelectionStart < 0)
					{
						mvarSelectionStart = 0;
						selectedNybble = 0;
					}

					SelectionStart = SelectionStart; // to fire events
					Refresh();
					e.Cancel = true;
					break;
				}
				case KeyboardKey.ArrowRight:
				{
					if (selectedNybble == 1)
					{
						mvarSelectionStart++;
						selectedNybble = 0;
					}
					else if (selectedNybble == 0)
					{
						selectedNybble = 1;
					}

					if (mvarSelectionStart >= Data.Length)
						mvarSelectionStart = Data.Length - 1;

					SelectionStart = SelectionStart; // to fire events
					Refresh();
					e.Cancel = true;
					break;
				}
				case KeyboardKey.ArrowUp:
				{
					if (mvarSelectionStart - mvarMaxDisplayWidth >= 0)
					{
						mvarSelectionStart -= mvarMaxDisplayWidth;
					}

					SelectionStart = SelectionStart; // to fire events
					Refresh();
					e.Cancel = true;
					break;
				}
				case KeyboardKey.ArrowDown:
				{
					mvarSelectionStart += mvarMaxDisplayWidth;

					if (mvarSelectionStart >= Data.Length)
						mvarSelectionStart = Data.Length - 1;

					SelectionStart = SelectionStart; // to fire events
					Refresh();
					e.Cancel = true;
					break;
				}
				case KeyboardKey.Home:
				{
					mvarSelectionStart = 0;
					selectedNybble = 0;

					SelectionStart = SelectionStart; // to fire events
					Refresh();
					e.Cancel = true;
					break;
				}
				case KeyboardKey.End:
				{
					mvarSelectionStart = Data.Length - 1;
					selectedNybble = 1;

					SelectionStart = SelectionStart; // to fire events
					Refresh();
					e.Cancel = true;
					break;
				}
			}

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

			if (next != '\0')
			{
				string curhex = Data[mvarSelectionStart].ToString("X").PadLeft(2, '0');
				if (selectedNybble == 0)
				{
					Data[mvarSelectionStart] = Byte.Parse(next.ToString() + curhex.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
					selectedNybble = 1;
				}
				else if (selectedNybble == 1)
				{
					Data[mvarSelectionStart] = Byte.Parse(curhex.Substring(0, 1) + next.ToString(), System.Globalization.NumberStyles.HexNumber);

					mvarSelectionStart++;
					selectedNybble = 0;
				}

				Refresh();
			}
		}

		protected internal override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			mvarMaxDisplayWidth = (int)((this.Size.Width - PositionGutterWidth - TextAreaWidth) / 24);

			Font font = Font.FromFamily("Monospace", 14.0);

			Vector2D textOffset = new Vector2D(4, 16);

			Rectangle rectPosition = new Rectangle(xoffset, yoffset, 72, 24);
			Rectangle rectCell = new Rectangle(rectPosition.X + rectPosition.Width + HorizontalCellSpacing, yoffset, 24, 24);

			for (int i = 0; i < mvarData.Length; i++)
			{
				Rectangle rectPositionText = new Rectangle(rectPosition.X + textOffset.X, rectPosition.Y + textOffset.Y, rectPosition.Width - (textOffset.X * 2), rectPosition.Height - (textOffset.Y * 2));
				Rectangle rectCellText = new Rectangle(rectCell.X + textOffset.X, rectCell.Y + textOffset.Y, rectCell.Width - (textOffset.X * 2), rectCell.Height - (textOffset.Y * 2));
				Rectangle rectNybbleCell = new Rectangle(rectCell.X + ((rectCell.Width / 2) * selectedNybble), rectCell.Y, rectCell.Width / 2, rectCell.Height);

				foreach (HexEditorHighlightArea area in HighlightAreas)
				{
					if (i >= area.Start && i < (area.Start + area.Length))
					{
						e.Graphics.FillRectangle(new SolidBrush(area.Color), rectCell);
					}
				}

				if ((i % mvarMaxDisplayWidth) == 0)
				{
					// print the offset of data, once per line
					string strOffset = i.ToString("X").PadLeft(8, '0');
					e.Graphics.DrawText(strOffset, font, rectPositionText, Brushes.Black);
				}

				if (i == mvarSelectionStart)
				{
					e.Graphics.DrawRectangle(new Pen(Colors.LightSteelBlue), rectCell);
					e.Graphics.FillRectangle(new SolidBrush(Colors.LightSteelBlue), rectNybbleCell);
				}

				string hex = mvarData[i].ToString("X").PadLeft(2, '0');
				e.Graphics.DrawText(hex, font, rectCellText, Brushes.Black);
				rectCell.X += rectCell.Width + HorizontalCellSpacing;

				if (((i + 1) % mvarMaxDisplayWidth) == 0)
				{
					rectCell.X = rectPosition.X + rectPosition.Width + HorizontalCellSpacing;
					rectCell.Y += rectCell.Height + VerticalCellSpacing;
					rectPosition.Y += rectPosition.Height + VerticalCellSpacing;
				}

				if (rectPosition.Y > this.Size.Height)
				{
					break;
				}
			}
		}
	}
}
