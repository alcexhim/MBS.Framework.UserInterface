using System;
using System.Collections.Generic;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls.Docking.Native;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.Docking
{
	[ControlImplementation(typeof(DockingContainerControl))]
	public class DockingContainerImplementationUWT : CustomImplementation, IDockingContainerNativeImplementation
	{
		public DockingContainerImplementationUWT(Engine engine, Control control) : base(engine, control)
		{
		}

		private class DockingDockContainer : Container
		{
			internal DockingTabContainer tbsTopPanel = null;
			internal DockingTabContainer tbsBottomPanel = null;
			internal DockingTabContainer tbsLeftPanel = null;
			internal DockingTabContainer tbsRightPanel = null;
			internal DockingTabContainer tbsCenterPanel = null;

			internal DockingSplitContainer scTopCenterTOP = null;
			internal DockingSplitContainer scCenterBottomCENTERBOTTOM = null;
			internal DockingSplitContainer scLeftCenterLEFT = null;
			internal DockingSplitContainer scCenterRightRIGHT = null;

			private void tbs_SelectedTabChanged(object sender, TabContainerSelectedTabChangedEventArgs e)
			{
				(_dcc?.ControlImplementation as DockingContainerImplementationUWT)._CurrentTabPage = e.NewTab;
				InvokeMethod(_dcc, "OnSelectionChanged", new object[] { e });
			}

			private Menu _DockingContainerContextMenu = null;

			private void _DockingContainerContextMenu_Close(object sender, EventArgs e)
			{

			}
			private void _DockingContainerContextMenu_CloseAllButThis(object sender, EventArgs e)
			{

			}
			private void _DockingContainerContextMenu_CloseAll(object sender, EventArgs e)
			{

			}

			private void tbs_BeforeTabContextMenu(object sender, BeforeTabContextMenuEventArgs e)
			{
				if (_DockingContainerContextMenu == null)
				{
					_DockingContainerContextMenu = new Menu();
					_DockingContainerContextMenu.Items.AddRange(new MenuItem[]
					{
						new CommandMenuItem("_Close", null, _DockingContainerContextMenu_Close),
						new CommandMenuItem("Close All But T_his", null, _DockingContainerContextMenu_CloseAllButThis),
						new CommandMenuItem("C_lose All", null, _DockingContainerContextMenu_CloseAll)
					});
				}
				e.ContextMenu = _DockingContainerContextMenu;
			}

			private DockingContainerControl _dcc = null;

			public DockingDockContainer(DockingContainerControl dcc)
			{
				_dcc = dcc;
				Layout = new BoxLayout(Orientation.Vertical);

				tbsTopPanel = new DockingTabContainer();
				tbsTopPanel.SelectedTabChanged += tbs_SelectedTabChanged;
				tbsTopPanel.BeforeTabContextMenu += tbs_BeforeTabContextMenu;
				tbsBottomPanel = new DockingTabContainer();
				tbsBottomPanel.SelectedTabChanged += tbs_SelectedTabChanged;
				tbsBottomPanel.BeforeTabContextMenu += tbs_BeforeTabContextMenu;
				tbsLeftPanel = new DockingTabContainer();
				tbsLeftPanel.SelectedTabChanged += tbs_SelectedTabChanged;
				tbsLeftPanel.BeforeTabContextMenu += tbs_BeforeTabContextMenu;
				tbsRightPanel = new DockingTabContainer();
				tbsRightPanel.SelectedTabChanged += tbs_SelectedTabChanged;
				tbsRightPanel.BeforeTabContextMenu += tbs_BeforeTabContextMenu;
				tbsCenterPanel = new DockingTabContainer();
				tbsCenterPanel.SelectedTabChanged += tbs_SelectedTabChanged;
				tbsCenterPanel.BeforeTabContextMenu += tbs_BeforeTabContextMenu;

				scTopCenterTOP = new DockingSplitContainer();
				scTopCenterTOP.Panel1.Layout = new BoxLayout(Orientation.Vertical);
				scTopCenterTOP.Panel1.Controls.Add(new DockingPanelTitleBar(), new BoxLayout.Constraints(false, true));
				scTopCenterTOP.Panel1.Controls.Add(tbsTopPanel, new BoxLayout.Constraints(true, true));

				scCenterBottomCENTERBOTTOM = new DockingSplitContainer();
				scCenterBottomCENTERBOTTOM.Panel1.Layout = new BoxLayout(Orientation.Vertical);
				scCenterBottomCENTERBOTTOM.Panel1.Controls.Add(new DockingPanelTitleBar(), new BoxLayout.Constraints(false, true));
				scCenterBottomCENTERBOTTOM.Panel1.Controls.Add(tbsCenterPanel, new BoxLayout.Constraints(true, true));
				scCenterBottomCENTERBOTTOM.Panel2.Layout = new BoxLayout(Orientation.Vertical);
				scCenterBottomCENTERBOTTOM.Panel2.Controls.Add(new DockingPanelTitleBar(), new BoxLayout.Constraints(false, true));
				scCenterBottomCENTERBOTTOM.Panel2.Controls.Add(tbsBottomPanel, new BoxLayout.Constraints(true, true));

				scTopCenterTOP.Panel2.Layout = new BoxLayout(Orientation.Vertical);
				scTopCenterTOP.Panel2.Controls.Add(scCenterBottomCENTERBOTTOM, new BoxLayout.Constraints(true, true));

				scLeftCenterLEFT = new DockingSplitContainer();
				scLeftCenterLEFT.Panel1.Layout = new BoxLayout(Orientation.Vertical);
				scLeftCenterLEFT.Panel1.Controls.Add(new DockingPanelTitleBar(), new BoxLayout.Constraints(false, true));
				scLeftCenterLEFT.Panel1.Controls.Add(tbsLeftPanel, new BoxLayout.Constraints(true, true));
				scLeftCenterLEFT.Orientation = Orientation.Vertical;

				scCenterRightRIGHT = new DockingSplitContainer();
				scCenterRightRIGHT.Orientation = Orientation.Vertical;
				scCenterRightRIGHT.Panel1.Layout = new BoxLayout(Orientation.Vertical);
				scCenterRightRIGHT.Panel1.Controls.Add(scTopCenterTOP, new BoxLayout.Constraints(true, true));
				scCenterRightRIGHT.Panel2.Layout = new BoxLayout(Orientation.Vertical);
				scCenterRightRIGHT.Panel2.Controls.Add(new DockingPanelTitleBar(), new BoxLayout.Constraints(false, true));
				scCenterRightRIGHT.Panel2.Controls.Add(tbsRightPanel, new BoxLayout.Constraints(true, true));

				scLeftCenterLEFT.Panel2.Layout = new BoxLayout(Orientation.Vertical);
				scLeftCenterLEFT.Panel2.Controls.Add(scCenterRightRIGHT, new BoxLayout.Constraints(true, true));

				Controls.Add(scLeftCenterLEFT, new BoxLayout.Constraints(true, true));
			}
		}
		private class DockingTabContainer : TabContainer
		{
			public DockingTabContainer()
			{
				// this.TabPosition = TabPosition.Bottom;
				GroupName = "UwtDockingTabContainer";
			}
		}
		private class DockingSplitContainer : SplitContainer
		{
			public DockingSplitContainer()
			{
				this.Panel1.Layout = new BoxLayout(Orientation.Vertical);

				this.Panel2.Layout = new BoxLayout(Orientation.Vertical);

				this.SplitterPosition = 100;
			}
		}
		private class DockingPanelTitleBar : Container
		{
			private Label lblTitleBar = null;
			private Button cmdOptions = null;
			private Button cmdClose = null;

			public DockingPanelTitleBar()
			{
				this.Layout = new BoxLayout(Orientation.Horizontal);

				lblTitleBar = new Label();
				lblTitleBar.HorizontalAlignment = HorizontalAlignment.Left;
				lblTitleBar.VerticalAlignment = VerticalAlignment.Middle;
				lblTitleBar.Text = "Title bar for docking widget";
				this.Controls.Add(lblTitleBar, new BoxLayout.Constraints(true, true));

				cmdOptions = new Button();
				cmdOptions.Text = "O";
				this.Controls.Add(cmdOptions, new BoxLayout.Constraints(false, false));

				cmdClose = new Button();
				cmdClose.Text = "X";
				this.Controls.Add(cmdClose, new BoxLayout.Constraints(false, false));
			}
		}

		private DockingDockContainer _ddc = null;
		protected override NativeControl CreateControlInternal(Control control)
		{
			DockingContainerControl dcc = (control as DockingContainerControl);
			DockingDockContainer ddc = new DockingDockContainer(dcc);
			_ddc = ddc;

			for (int i = 0; i < dcc.Items.Count; i++)
			{
				InsertDockingItem(dcc.Items[i], dcc.Items.Count - 1);
			}
			return new CustomNativeControl(ddc);
		}

		protected override Dimension2D GetControlSizeInternal()
		{
			return (Handle as CustomNativeControl).Handle.Size;
		}

		protected override Cursor GetCursorInternal()
		{
			throw new NotImplementedException();
		}

		protected override string GetTooltipTextInternal()
		{
			throw new NotImplementedException();
		}

		protected override bool HasFocusInternal()
		{
			return _ddc.Focused;
		}

		protected override bool IsControlVisibleInternal()
		{
			throw new NotImplementedException();
		}

		protected override void RegisterDragSourceInternal(Control control, DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			throw new NotImplementedException();
		}

		protected override void RegisterDropTargetInternal(Control control, DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			throw new NotImplementedException();
		}

		protected override void SetControlVisibilityInternal(bool visible)
		{
			throw new NotImplementedException();
		}

		protected override void SetCursorInternal(Cursor value)
		{
			throw new NotImplementedException();
		}

		protected override void SetFocusInternal()
		{
			throw new NotImplementedException();
		}

		protected override void SetTooltipTextInternal(string value)
		{
			throw new NotImplementedException();
		}

		public void ClearDockingItems()
		{
			_ddc.tbsTopPanel.TabPages.Clear();
			_ddc.tbsLeftPanel.TabPages.Clear();
			_ddc.tbsRightPanel.TabPages.Clear();
			_ddc.tbsBottomPanel.TabPages.Clear();
			_ddc.tbsCenterPanel.TabPages.Clear();
		}

		public void InsertDockingItem(DockingItem item, int index)
		{
			InsertDockingItemRecursive(item, index, null);
		}

		private void InsertDockingItemRecursive(DockingItem item, int index, DockingDockContainer parent = null)
		{
			if (parent == null)
			{
				parent = _ddc;
			}

			if (item is DockingWindow)
			{
				TabPage tab = new TabPage(item.Title);
				tab.Layout = new BoxLayout(Orientation.Horizontal);
				tab.Controls.Add((item as DockingWindow).ChildControl, new BoxLayout.Constraints(true, true));
				tab.Detachable = true;
				tab.Reorderable = true;
				switch (item.Placement)
				{
					case DockingItemPlacement.Center:
					{
						parent.tbsCenterPanel.TabPages.Add(tab);
						break;
					}
					case DockingItemPlacement.Left:
					{
						parent.tbsLeftPanel.TabPages.Add(tab);
						break;
					}
					case DockingItemPlacement.Bottom:
					{
						parent.tbsBottomPanel.TabPages.Add(tab);
						break;
					}
					case DockingItemPlacement.Right:
					{
						parent.tbsRightPanel.TabPages.Add(tab);
						break;
					}
					case DockingItemPlacement.Top:
					{
						parent.tbsTopPanel.TabPages.Add(tab);
						break;
					}
				}

				RegisterDockingItemTabPage(tab, item);
			}
			else if (item is DockingContainer)
			{
				DockingDockContainer ddc = new DockingDockContainer(Control as DockingContainerControl);
				DockingContainer dc = (item as DockingContainer);
				for (int i = 0; i < dc.Items.Count; i++)
				{
					InsertDockingItemRecursive(dc.Items[i], dc.Items.Count - 1, ddc);
				}

				switch (item.Placement)
				{
					case DockingItemPlacement.Center:
					{
						DockingTabContainer tbsCenter = (parent.scCenterBottomCENTERBOTTOM.Panel1.Controls[1] as DockingTabContainer);

						TabPage tab = new TabPage();
						tab.Layout = new BoxLayout(Orientation.Vertical);
						tab.Text = item.Title;
						tab.Controls.Add(ddc, new BoxLayout.Constraints(true, true));
						tbsCenter.TabPages.Add(tab);

						_DockingItemsForTabPage[tab] = item;
						break;
					}
					case DockingItemPlacement.Left:
					{
						DockingTabContainer tbsLeft = (parent.scLeftCenterLEFT.Panel1.Controls[1] as DockingTabContainer);
						parent.scLeftCenterLEFT.Panel1.Controls.Remove(tbsLeft);

						DockingSplitContainer sc = new DockingSplitContainer();
						sc.Orientation = Orientation.Vertical;
						sc.Panel1.Controls.Add(tbsLeft, new BoxLayout.Constraints(true, true));
						sc.Panel2.Controls.Add(ddc, new BoxLayout.Constraints(true, true));
						parent.scLeftCenterLEFT.Panel1.Controls.Add(sc, new BoxLayout.Constraints(true, true));
						break;
					}
					case DockingItemPlacement.Bottom:
					{
						DockingTabContainer tbsLeft = (parent.scCenterBottomCENTERBOTTOM.Panel2.Controls[1] as DockingTabContainer);
						parent.scCenterBottomCENTERBOTTOM.Panel2.Controls.Remove(tbsLeft);

						DockingSplitContainer sc = new DockingSplitContainer();
						sc.Orientation = Orientation.Vertical;
						sc.Panel1.Controls.Add(tbsLeft, new BoxLayout.Constraints(true, true));
						sc.Panel2.Controls.Add(ddc, new BoxLayout.Constraints(true, true));
						parent.scCenterBottomCENTERBOTTOM.Panel2.Controls.Add(sc, new BoxLayout.Constraints(true, true));
						break;
					}
					case DockingItemPlacement.Right:
					{
						DockingTabContainer tbsLeft = (parent.scCenterRightRIGHT.Panel2.Controls[1] as DockingTabContainer);
						parent.scCenterRightRIGHT.Panel2.Controls.Remove(tbsLeft);

						DockingSplitContainer sc = new DockingSplitContainer();
						sc.Orientation = Orientation.Vertical;
						sc.Panel1.Controls.Add(tbsLeft, new BoxLayout.Constraints(true, true));
						sc.Panel2.Controls.Add(ddc, new BoxLayout.Constraints(true, true));
						parent.scCenterRightRIGHT.Panel2.Controls.Add(sc, new BoxLayout.Constraints(true, true));
						break;
					}
					case DockingItemPlacement.Top:
					{
						DockingTabContainer tbsLeft = (parent.scTopCenterTOP.Panel1.Controls[1] as DockingTabContainer);
						parent.scLeftCenterLEFT.Panel1.Controls.Remove(tbsLeft);

						DockingSplitContainer sc = new DockingSplitContainer();
						sc.Orientation = Orientation.Vertical;
						sc.Panel1.Controls.Add(ddc, new BoxLayout.Constraints(true, true));
						sc.Panel2.Controls.Add(tbsLeft, new BoxLayout.Constraints(true, true));
						parent.scTopCenterTOP.Panel1.Controls.Add(sc, new BoxLayout.Constraints(true, true));
						break;
					}
				}
			}
		}

		public void RemoveDockingItem(DockingItem item)
		{
			if (!_TabPagesForDockingItem.ContainsKey(item))
				return;

			TabPage tabPage = _TabPagesForDockingItem[item];
			tabPage.Parent.TabPages.Remove(tabPage);
		}

		public void SetDockingItem(int index, DockingItem item)
		{
			throw new NotImplementedException();
		}

		private TabPage _CurrentTabPage = null;

		private Dictionary<TabPage, DockingItem> _DockingItemsForTabPage = new Dictionary<TabPage, DockingItem>();
		private Dictionary<DockingItem, TabPage> _TabPagesForDockingItem = new Dictionary<DockingItem, TabPage>();

		private void RegisterDockingItemTabPage(TabPage tab, DockingItem item)
		{
			_DockingItemsForTabPage[tab] = item;
			_TabPagesForDockingItem[item] = tab;
		}

		public DockingItem GetCurrentItem()
		{
			if (_CurrentTabPage == null)
				return null;
			return _DockingItemsForTabPage[_CurrentTabPage];
		}

		public void SetCurrentItem(DockingItem item)
		{
			throw new NotImplementedException();
		}

		public void UpdateDockingItemName(DockingItem item, string text)
		{
			TabPage tab = GetTabPageForDockingItem(item);
			tab.Name = text;
		}

		public void UpdateDockingItemTitle(DockingItem item, string text)
		{
			TabPage tab = GetTabPageForDockingItem(item);
			tab.Text = text;
		}

		private TabPage GetTabPageForDockingItem(DockingItem item)
		{
			return (_TabPagesForDockingItem.ContainsKey(item) ? _TabPagesForDockingItem[item] : null);
		}
	}
}
