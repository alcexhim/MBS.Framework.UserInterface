//
//  DockingPanelTitleBar.cs
//
//  Author:
//       beckermj <>
//
//  Copyright (c) 2023 ${CopyrightHolder}
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
using System.Diagnostics.Contracts;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Input.Mouse;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.Docking.Impl
{
	internal class DockingPanelTitleBar : Container
	{
		private Label lblTitleBar = null;
		private Button cmdOptions = null;
		private Button cmdClose = null;

		private DockingTabContainer _tabContainer = null;

		protected internal override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
			e.Graphics.FillRectangle(new Drawing.SolidBrush(MBS.Framework.Drawing.Color.FromRGBAByte(255, 255, 255, 32)), new Rectangle(0, 0, Size.Width, Size.Height));
			e.Handled = true;
		}
		protected internal override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			//e.Graphics.DrawPolygon(new Drawing.Pen(Color.FromRGBAByte(0, 0, 0), new Measurement(1.0, MeasurementUnit.Pixel), Drawing.PenStyle.Solid), new Vector2D[] { new Vector2D(0, 0), new Vector2D(0, 16), new Vector2D(16, 0) });
		}

		public DockingPanelTitleBar(DockingTabContainer tabContainer)
		{
			MinimumSize = new Dimension2D(-1, 16);

			Contract.Requires(tabContainer != null);

			this.Layout = new BoxLayout(Orientation.Horizontal);

			lblTitleBar = new Label();
			lblTitleBar.Font = Drawing.Font.FromFont(SystemFonts.MenuFont, new Measurement(8, MeasurementUnit.Point));
			lblTitleBar.HorizontalAlignment = HorizontalAlignment.Left;
			lblTitleBar.VerticalAlignment = VerticalAlignment.Middle;
			lblTitleBar.Text = "Title bar for docking widget";
			lblTitleBar.Margin = new Padding(8);
			this.Controls.Add(lblTitleBar, new BoxLayout.Constraints(true, true));

			cmdOptions = new Button();
			cmdOptions.TooltipText = "Options";
			cmdOptions.Click += cmdOptions_Click;
			cmdOptions.BorderStyle = ButtonBorderStyle.None;
			cmdOptions.Image = Drawing.Image.FromName("gtk-preferences", 16);
			this.Controls.Add(cmdOptions, new BoxLayout.Constraints(false, false));

			cmdClose = new Button();
			cmdClose.TooltipText = "Close";
			cmdClose.Click += CmdClose_Click;
			cmdClose.BorderStyle = ButtonBorderStyle.None;
			cmdClose.Image = Drawing.Image.FromName("gtk-close", 16);
			this.Controls.Add(cmdClose, new BoxLayout.Constraints(false, false));

			_tabContainer = tabContainer;
			_tabContainer.SelectedTabChanged += _tabContainer_SelectedTabChanged;
			if (_tabContainer.TabPages.Count > 0)
			{
				lblTitleBar.Text = _tabContainer.TabPages[0].Text;
			}

			MouseDown += _MouseDown;
			lblTitleBar.MouseDown += _MouseDown;

			MouseMove += _MouseMove;
			lblTitleBar.MouseMove += _MouseMove;

			MouseUp += _MouseUp;
			lblTitleBar.MouseUp += _MouseUp;
		}

		void cmdOptions_Click(object sender, EventArgs e)
		{
			Menu menu = new Menu();
			menu.Items.Add(new CommandMenuItem("_Floating"));
			menu.Items.Add(new CommandMenuItem("Doc_kable"));
			menu.Items.Add(new CommandMenuItem("_Tabbed Document"));
			menu.Items.Add(new CommandMenuItem("_Auto Hide"));
			menu.Items.Add(new CommandMenuItem("_Hide"));
			menu.Show(cmdOptions, Gravity.BottomLeft, Gravity.TopLeft);
		}


		void CmdClose_Click(object sender, EventArgs e)
		{
		}


		void _tabContainer_SelectedTabChanged(object sender, TabContainerSelectedTabChangedEventArgs e)
		{
			TabContainer tabContainer = (sender as TabContainer);
			lblTitleBar.Text = e.NewTab?.Text;
		}


		protected internal override void OnCreated(EventArgs e)
		{
			Contract.Requires(_tabContainer != null);
			base.OnCreated(e);

			if (_tabContainer.TabPages.Count > 0)
			{
				lblTitleBar.Text = _tabContainer.TabPages[0].Text;
			}
		}

		private Vector2D _initialDragPoint = new Vector2D();

		private void _MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Buttons == MouseButtons.Primary)
			{
				_initialDragPoint = e.Location;
			}
			//_tabContainer.BeginMovePopupWindow(e.Location);
			//Seat.Grab(SeatCapabilities.AllPointing);
		}
		private int dragMargin = 16;
		private bool _isOpened = false;
		private void _MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Buttons == MouseButtons.Primary)
			{
				Vector2D diff = e.Location - _initialDragPoint;
				if (_isOpened || (Math.Abs(diff.X) > dragMargin && Math.Abs(diff.Y) > dragMargin))
				{
					if (!_isOpened)
					{
						_tabContainer.OpenPopupWindow();
						_tabContainer.BeginMovePopupWindow(ClientToWindowCoordinates(e.Location));
					}
					_isOpened = true;
					//_tabContainer.MovePopupWindow(e.Location);
				}
				Console.WriteLine("mouse moved whilst dragging : {0}", diff);
			}
		}
		private void _MouseUp(object sender, MouseEventArgs e)
		{
			_isOpened = false;
			//Seat.Release();
		}
	}

}
