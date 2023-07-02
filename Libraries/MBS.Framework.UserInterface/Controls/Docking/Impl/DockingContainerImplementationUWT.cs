using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls.Docking.Native;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.Docking.Impl
{
	[ControlImplementation(typeof(DockingContainerControl))]
	public class DockingContainerImplementationUWT : CustomImplementation, IDockingContainerNativeImplementation
	{
		public DockingContainerImplementationUWT(Engine engine, Control control) : base(engine, control)
		{
		}


		private DockingDockContainer _ddc = null;

		public override ControlImplementation GetNativeImplementation()
		{
			return _ddc.ControlImplementation;
		}
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

		protected override Cursor GetCursorInternal()
		{
			throw new NotImplementedException();
		}

		protected override string GetTooltipTextInternal()
		{
			return String.Empty;
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
			// _ddc.tbsCenterPanel.TabPages.Clear();
		}

		public void InsertDockingItem(DockingItem item, int index)
		{
			InsertDockingItemRecursive(item, index, null);
		}

		protected internal override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);
			Application.Instance.EventFilters.Add(new EventFilter(_MouseEvent, EventFilterType.MouseDown | EventFilterType.MouseMove | EventFilterType.MouseUp));
		}

		private bool _MouseEvent(EventFilterType type, ref EventArgs e)
		{
			Console.WriteLine("filter mouse event : hi !");
			return false;
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
				tab.SetExtraData<DockingWindow>("dw", item as DockingWindow);
				FindOrCreateParentControl(tab, parent, item.Placement);

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
						TabPage tab = new TabPage();
						tab.Layout = new BoxLayout(Orientation.Vertical);
						tab.Text = item.Title;
						tab.Controls.Add(ddc, new BoxLayout.Constraints(true, true));
						RegisterDockingItemTabPage(tab, item);
						break;
					}
				}
			}
		}

		private void FindOrCreateParentControl(TabPage tab, DockingDockContainer parent, DockingItemPlacement placement)
		{
			switch (placement)
			{
				case DockingItemPlacement.Left:
				{
					if (parent.Controls[0] is DockingTabContainer)
					{
						DockingTabContainer tbs = (parent.Controls[0] as DockingTabContainer);
						parent.Controls.Remove(tbs);

						DockingSplitContainer dsc = new DockingSplitContainer();
						dsc.Orientation = Orientation.Vertical;

						DockingTabContainer tbs1 = new DockingTabContainer();
						tbs1.TabPosition = TabPosition.Bottom;
						dsc.Panel1.Controls.Add(new DockingPanelTitleBar(tbs1), new BoxLayout.Constraints(false, true));

						dsc.Panel1.Controls.Add(tbs1, new BoxLayout.Constraints(true, true));

						dsc.Panel2.Controls.Add(tbs, new BoxLayout.Constraints(true, true));
						parent.Controls.Add(dsc, new BoxLayout.Constraints(true, true));
						tbs1.TabPages.Add(tab);
					}
					else if (parent.Controls[0] is DockingSplitContainer)
					{
						DockingSplitContainer dsc = (parent.Controls[0] as DockingSplitContainer);
						DockingTabContainer tbs1 = (dsc.Panel1.Controls[1] as DockingTabContainer);
						tbs1.TabPages.Add(tab);
					}
					break;
				}
				case DockingItemPlacement.Right:
				{
					if (parent.Controls[0] is DockingTabContainer)
					{
						DockingTabContainer tbs = (parent.Controls[0] as DockingTabContainer);
						parent.Controls.Remove(tbs);

						DockingSplitContainer dsc = new DockingSplitContainer();
						dsc.Orientation = Orientation.Vertical;

						DockingTabContainer tbs1 = new DockingTabContainer();
						tbs1.TabPosition = TabPosition.Bottom;
						dsc.Panel1.Controls.Add(new DockingPanelTitleBar(tbs1), new BoxLayout.Constraints(false, true));

						dsc.Panel1.Controls.Add(tbs1, new BoxLayout.Constraints(true, true));

						dsc.Panel2.Controls.Add(tbs, new BoxLayout.Constraints(true, true));
						parent.Controls.Add(dsc, new BoxLayout.Constraints(true, true));
						tbs1.TabPages.Add(tab);
					}
					else if (parent.Controls[0] is DockingSplitContainer)
					{
						DockingSplitContainer dsc = (parent.Controls[0] as DockingSplitContainer);
						DockingTabContainer tbs1 = (dsc.Panel1.Controls[1] as DockingTabContainer);
						tbs1.TabPages.Add(tab);
					}
					break;
				}
				case DockingItemPlacement.Bottom:
				{
					if (parent.Controls[0] is DockingTabContainer)
					{
						DockingTabContainer tbs = (parent.Controls[0] as DockingTabContainer);
						parent.Controls.Remove(tbs);

						DockingSplitContainer dsc = new DockingSplitContainer();
						dsc.Orientation = Orientation.Horizontal;

						DockingTabContainer tbs1 = new DockingTabContainer();
						tbs1.TabPosition = TabPosition.Bottom;
						dsc.Panel2.Controls.Add(new DockingPanelTitleBar(tbs1), new BoxLayout.Constraints(false, true));
						dsc.Panel2.Controls.Add(tbs1, new BoxLayout.Constraints(true, true));

						dsc.Panel1.Controls.Add(tbs, new BoxLayout.Constraints(true, true));
						parent.Controls.Add(dsc, new BoxLayout.Constraints(true, true));
						tbs1.TabPages.Add(tab);
					}
					else if (parent.Controls[0] is DockingSplitContainer)
					{
						DockingTabContainer tbs1 = null;
						DockingSplitContainer dsc = (parent.Controls[0] as DockingSplitContainer);
						if (dsc.Orientation == Orientation.Vertical)
						{
							// huh, we already have a vertical SplitContainer
							// must be a left- or right-docked item
							if (dsc.Panel1.Controls[1] is DockingSplitContainer)
							{
							}
							else if (dsc.Panel1.Controls[1] is DockingTabContainer)
							{
								// left-docked item
								if (dsc.Panel2.Controls[0] is DockingTabContainer)
								{
									DockingTabContainer tbs = (dsc.Panel2.Controls[0] as DockingTabContainer);
									dsc.Panel2.Controls.Remove(tbs);

									DockingSplitContainer dsc1 = new DockingSplitContainer();
									dsc1.Orientation = Orientation.Horizontal;
									dsc1.Panel1.Controls.Add(tbs, new BoxLayout.Constraints(true, true));

									tbs1 = new DockingTabContainer();
									tbs1.SplitContainerPanel = dsc1.Panel2;
									tbs1.TabPosition = TabPosition.Bottom;
									dsc1.Panel2.Controls.Add(new DockingPanelTitleBar(tbs1), new BoxLayout.Constraints(false, true));
									dsc1.Panel2.Controls.Add(tbs1, new BoxLayout.Constraints(true, true));

									dsc.Panel2.Controls.Add(dsc1, new BoxLayout.Constraints(true, true));
								}
								else if (dsc.Panel2.Controls.Count > 1 && dsc.Panel2.Controls[1] is DockingTabContainer)
								{
									tbs1 = (dsc.Panel2.Controls[1] as DockingTabContainer);
								}
							}
							else if (dsc.Panel2.Controls[0] is DockingSplitContainer)
							{
								tbs1 = ((dsc.Panel2.Controls[0] as SplitContainer).Panel2.Controls[1] as DockingTabContainer);
							}
						}
						else
						{
							tbs1 = (dsc.Panel2.Controls[1] as DockingTabContainer);
						}

						if (tbs1 != null)
						{
							tbs1.TabPages.Add(tab);
						}
						else
						{
							Console.Error.WriteLine("tbs1 is NULL");
						}
					}
					break;
				}
				case DockingItemPlacement.Center:
				{
					DockingTabContainer tbs = null;
					if (parent.Controls[0] is DockingTabContainer)
					{
						tbs = (parent.Controls[0] as DockingTabContainer);
					}
					else if (parent.Controls[0] is DockingSplitContainer)
					{
						DockingSplitContainer dsc = (parent.Controls[0] as DockingSplitContainer);
						if (dsc.Panel1.Controls[0] is DockingTabContainer)
						{
							tbs = (dsc.Panel1.Controls[0] as DockingTabContainer);
						}
						else if (dsc.Panel2.Controls[0] is DockingTabContainer)
						{
							tbs = (dsc.Panel2.Controls[0] as DockingTabContainer);
						}
						else if (dsc.Panel2.Controls[0] is DockingSplitContainer)
						{
							DockingSplitContainer dsc1 = dsc.Panel2.Controls[0] as DockingSplitContainer;
							if (dsc1.Panel1.Controls[0] is DockingTabContainer)
							{
								tbs = (dsc1.Panel1.Controls[0] as DockingTabContainer);
							}
							else if (dsc1.Panel2.Controls[0] is DockingTabContainer)
							{
								tbs = (dsc1.Panel2.Controls[0] as DockingTabContainer);
							}
						}
					}

					if (tbs != null)
						tbs.TabPages.Add(tab);
					break;
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

		internal TabPage _CurrentTabPage = null;

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
			// FIXME: this crashes after DockingContainerControl::HideWindowListPopup
			// ------ if we close a newly-created tab (e.g. create a file, then Ctrl+W)
			TabPage tab = GetTabPageForDockingItem(item);
			tab.Parent.SelectedTab = tab;
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

		protected override void SetMarginInternal(Padding value)
		{
			(Handle as CustomNativeControl).Handle.Margin = value;
		}
	}
}
