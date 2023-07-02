//
//  DockingTabContainer.cs
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
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.Docking.Impl
{
	internal class DockingTabContainer : TabContainer
	{
		public DockingTabContainer()
		{
			// this.TabPosition = TabPosition.Bottom;
			GroupName = "UwtDockingTabContainer";
			this.Scrollable = true;
		}

		public IControlContainer OldParent { get; internal set; }
		internal SplitContainer.SplitContainerPanel SplitContainerPanel { get; set; }

		private DockingTabPopupWindow _popupWindow = null;
		internal void OpenPopupWindow()
		{
			if (_popupWindow == null)
			{
				_popupWindow = new DockingTabPopupWindow(this);
				_popupWindow.Show();
			}

			SplitContainerPanel.Expanded = false;

			if (SelectedTab != null)
			{
				_popupWindow.Text = SelectedTab.Text;

				Control ctl = SelectedTab.Controls[0];
				SelectedTab.Controls.Remove(ctl);

				_popupWindow.Controls.Add(ctl, new BoxLayout.Constraints(true, true));
			}

			_popupWindow.Present();
		}
		internal void MovePopupWindow(Vector2D location)
		{
			if (_popupWindow != null)
			{
				Console.WriteLine("moving the popup window");
				_popupWindow.Bounds = new Rectangle(ClientToScreenCoordinates(location), new Dimension2D(-1, -1));
			}
		}
		internal void ClosePopupWindow()
		{
			_popupWindow.Hide();
			SplitContainerPanel.Expanded = true;

			Control ctl = _popupWindow.Controls[0];
			_popupWindow.Controls.Remove(ctl);
			SelectedTab.Controls.Add(ctl, new BoxLayout.Constraints(true, true));
		}

		internal void BeginMovePopupWindow(Vector2D location)
		{
			_popupWindow.BeginMove(new Vector2D(192, 64));
		}
	}

}
