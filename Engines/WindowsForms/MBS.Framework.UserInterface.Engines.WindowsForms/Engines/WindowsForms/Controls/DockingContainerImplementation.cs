//
//  DockingContainerImplementation.cs
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
using MBS.Framework.UserInterface.Controls.Docking;
using MBS.Framework.UserInterface.Controls.Docking.Native;
using WeifenLuo.WinFormsUI.Docking;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Engines.WindowsForms.Controls
{
	[ControlImplementation(typeof(DockingContainer))]
	public class DockingContainerImplementation : WindowsFormsNativeImplementation, IDockingContainerNativeImplementation
	{
		public DockingContainerImplementation(Engine engine, DockingContainer control) : base(engine, control)
		{
		}

		public void ClearDockingItems()
		{
			WeifenLuo.WinFormsUI.Docking.DockPanel dp = ((Handle as WindowsFormsNativeControl).Handle as WeifenLuo.WinFormsUI.Docking.DockPanel);

		}

		public DockingItem GetCurrentItem()
		{
			// throw new NotImplementedException();
			return null;
		}

		public void InsertDockingItem(DockingItem item, int index)
		{
			WeifenLuo.WinFormsUI.Docking.DockPanel dp = ((Handle as WindowsFormsNativeControl).Handle as WeifenLuo.WinFormsUI.Docking.DockPanel);

			if (!item.ChildControl.IsCreated)
			{
				Console.WriteLine("child control of type {0} not yet created", item.ChildControl.GetType());

				bool created = Engine.CreateControl(item.ChildControl);
				if (!created) return;
			}

			NativeControl ncChild = (Engine.GetHandleForControl(item.ChildControl) as NativeControl);
			if (ncChild is WindowsFormsNativeControl)
			{
				System.Windows.Forms.Control wfcChild = (ncChild as WindowsFormsNativeControl).Handle;
				DockContent dcontent = new DockContent();
				dcontent.Text = item.Title;
				dcontent.Controls.Add(wfcChild);

				DockPane dpane = new DockPane(dcontent, DockingItemPlacementToDockState(item.Placement, item.AutoHide), true);
				dp.AddPane(dpane);
			}
		}

		public static DockState DockingItemPlacementToDockState(DockingItemPlacement placement, bool autoHide)
		{
			switch (placement)
			{
				case DockingItemPlacement.Bottom:
				{
					if (autoHide)
					{
						return DockState.DockBottomAutoHide;
					}
					else
					{
						return DockState.DockBottom;
					}
				}
				case DockingItemPlacement.Center:
				{
					return DockState.Document;
				}
				case DockingItemPlacement.Floating:
				{
					return DockState.Float;
				}
				case DockingItemPlacement.Left:
				{
					if (autoHide)
					{
						return DockState.DockLeftAutoHide;
					}
					else
					{
						return DockState.DockLeft;
					}
				}
				case DockingItemPlacement.None:
				{
					return DockState.Hidden;
				}
				case DockingItemPlacement.Right:
				{
					if (autoHide)
					{
						return DockState.DockRightAutoHide;
					}
					else
					{
						return DockState.DockRight;
					}
				}
				case DockingItemPlacement.Top:
				{
					if (autoHide)
					{
						return DockState.DockTopAutoHide;
					}
					else
					{
						return DockState.DockTop;
					}
				}
			}
			throw new NotSupportedException();
		}

		public void RemoveDockingItem(DockingItem item)
		{
			// throw new NotImplementedException();
		}

		public void SetCurrentItem(DockingItem item)
		{
			// throw new NotImplementedException();
		}

		public void SetDockingItem(int index, DockingItem item)
		{
			// throw new NotImplementedException();
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			WeifenLuo.WinFormsUI.Docking.DockPanel panel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
			panel.Dock = System.Windows.Forms.DockStyle.Fill;
			return new WindowsFormsNativeControl(panel);
		}
	}
}
