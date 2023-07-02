//
//  DockingTabPopupWindow.cs
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
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.Docking.Impl
{
	internal class DockingTabPopupWindow : Window
	{

		private DockingTabContainer _container = null;
		public DockingTabPopupWindow(DockingTabContainer container)
		{
			Layout = new BoxLayout(Orientation.Vertical);
			_container = container;
		}

		protected override void OnClosing(WindowClosingEventArgs e)
		{
			e.Cancel = true;
			_container.ClosePopupWindow();
		}
	}

}
