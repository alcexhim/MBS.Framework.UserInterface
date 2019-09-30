//
//  TabContainerImplementation.cs
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
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Native;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Engines.WindowsForms.Controls
{
	public class TabContainerImplementation : WindowsFormsNativeImplementation, ITabContainerControlImplementation
	{
		public TabContainerImplementation(Engine engine, TabContainer control)
			: base(engine, control)
		{
		}

		public void ClearTabPages()
		{
			(Control as TabContainer).TabPages.Clear();
		}

		public void InsertTabPage(int index, TabPage item)
		{
			(Control as TabContainer).TabPages.Insert(index, item);
		}

		public void RemoveTabPage(TabPage tabPage)
		{
			(Control as TabContainer).TabPages.Remove(tabPage);
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			TabContainer ctl = (control as TabContainer);

			System.Windows.Forms.TabControl tbs = new System.Windows.Forms.TabControl();
			return new WindowsFormsNativeControl(tbs);
		}
	}
}
