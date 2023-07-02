//
//  DockingDockContainer.cs
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
using System.Collections.Generic;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.Docking.Impl
{
	internal class DockingDockContainer : Container
	{
		private void tbs_SelectedTabChanged(object sender, TabContainerSelectedTabChangedEventArgs e)
		{
			(_dcc?.ControlImplementation as DockingContainerImplementationUWT)._CurrentTabPage = e.NewTab;
			Reflection.InvokeMethod(_dcc, "OnSelectionChanged", new object[] { e });
		}

		protected override IVirtualControlContainer GetControlParent()
		{
			return _dcc;
		}

		private Menu _DockingContainerContextMenu = null;

		private void _DockingContainerContextMenu_Close(object sender, EventArgs e)
		{
			DockingWindow dw = _DockingContainerContextMenu.GetExtraData<DockingWindow>("dw");
			((UIApplication)Application.Instance).ExecuteCommand("DockingContainerContextMenu_Close", new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Item", dw) });
		}
		private void _DockingContainerContextMenu_CloseAllButThis(object sender, EventArgs e)
		{
			((UIApplication)Application.Instance).ExecuteCommand("DockingContainerContextMenu_CloseAllButThis");
		}
		private void _DockingContainerContextMenu_CloseAll(object sender, EventArgs e)
		{
			((UIApplication)Application.Instance).ExecuteCommand("DockingContainerContextMenu_CloseAll");
		}

		private void tbs_BeforeTabContextMenu(object sender, BeforeTabContextMenuEventArgs e)
		{
			e.ContextMenuCommandID = "DockingWindowTabPageContextMenu";
		}

		private DockingContainerControl _dcc = null;


		public DockingDockContainer(DockingContainerControl dcc)
		{
			_dcc = dcc;
			Layout = new BoxLayout(Orientation.Vertical);

			DockingTabContainer tbsCenterPanel = new DockingTabContainer();
			tbsCenterPanel.SelectedTabChanged += tbs_SelectedTabChanged;
			tbsCenterPanel.BeforeTabContextMenu += tbs_BeforeTabContextMenu;

			Controls.Add(tbsCenterPanel, new BoxLayout.Constraints(true, true));
		}
	}

}
