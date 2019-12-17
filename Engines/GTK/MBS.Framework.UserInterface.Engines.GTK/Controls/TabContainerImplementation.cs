using System;
using MBS.Framework.Collections.Generic;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Native;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(TabContainer))]
	public class TabContainerImplementation : GTKNativeImplementation, ITabContainerControlImplementation
	{
		public TabContainerImplementation(Engine engine, Control control) : base(engine, control)
		{
		}
		static TabContainerImplementation()
		{
			create_window_d = new Func<IntPtr, IntPtr, int, int, IntPtr, IntPtr>(create_window);
		}

		static Random rnd = new Random();

		private static BidirectionalDictionary<TabPage, IntPtr> _TabPageHandles = new BidirectionalDictionary<TabPage, IntPtr>();
		private static void RegisterTabPage(TabPage page, IntPtr handle)
		{
			_TabPageHandles.Add(page, handle);
		}
		private static TabPage GetTabPageByHandle(IntPtr handle)
		{
			if (_TabPageHandles.ContainsValue2(handle))
			{
				return _TabPageHandles.GetValue1(handle);
			}
			return null;
		}

		public void NotebookAppendPage(TabContainer ctl, IntPtr handle, TabPage page, int indexAfter = -1)
		{
			Container tabControlContainer = new Container();
			tabControlContainer.Layout = new BoxLayout(Orientation.Horizontal, 8);

			Label lblTabText = new Label(page.Text);
			lblTabText.WordWrap = WordWrapMode.Never;

			tabControlContainer.Controls.Add(lblTabText, new BoxLayout.Constraints(true, true, 8));

			System.Reflection.FieldInfo fiParent = tabControlContainer.GetType().BaseType.GetField("mvarParent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			fiParent.SetValue(tabControlContainer, page);

			foreach (Control ctlTabButton in ctl.TabTitleControls)
			{
				tabControlContainer.Controls.Add(ctlTabButton);
			}

			Engine.CreateControl(tabControlContainer);
			IntPtr hTabLabel = (Engine.GetHandleForControl(tabControlContainer) as GTKNativeControl).Handle;

			ContainerImplementation cimpl = new ContainerImplementation(Engine, page);
			cimpl.CreateControl(page);
			IntPtr container = (cimpl.Handle as GTKNativeControl).Handle;

			string rndgroupname = ctl.GroupName;
			if (rndgroupname == null)
			{
				rndgroupname = Application.ID.ToString() + "_TabContainer_" + (rnd.Next().ToString());
			}
			Internal.GTK.Methods.GtkNotebook.gtk_notebook_set_group_name(handle, rndgroupname);

			if (indexAfter == -1)
			{
				int index = Internal.GTK.Methods.GtkNotebook.gtk_notebook_append_page(handle, container, hTabLabel);
				IntPtr hTabPage = Internal.GTK.Methods.GtkNotebook.gtk_notebook_get_nth_page(handle, index);
				RegisterTabPage(page, hTabPage);

				(ctl.ControlImplementation as TabContainerImplementation).SetTabPageDetachable(handle, hTabPage, page.Detachable);
				(ctl.ControlImplementation as TabContainerImplementation).SetTabPageReorderable(handle, hTabPage, page.Reorderable);

				Internal.GTK.Methods.GtkWidget.gtk_widget_show_all (hTabLabel);
			}
			else
			{
			}
		}

		public void ClearTabPages()
		{
			if (!Control.IsCreated)
				return;

			IntPtr handle = (Engine.GetHandleForControl(Control) as GTKNativeControl).Handle;
			int pageCount = Internal.GTK.Methods.GtkNotebook.gtk_notebook_get_n_pages(handle);
			for (int i = 0; i < pageCount; i++)
			{
				Internal.GTK.Methods.GtkNotebook.gtk_notebook_remove_page(handle, i);
			}
		}

		public void SetTabPageReorderable(TabPage page, bool value)
		{
			IntPtr? hptrParent = (page.Parent?.ControlImplementation.Handle as GTKNativeControl)?.Handle;
			IntPtr? hptr = (page.ControlImplementation.Handle as GTKNativeControl)?.Handle;
			SetTabPageReorderable(hptrParent, hptr, value);
		}
		public void SetTabPageDetachable(TabPage page, bool value)
		{
			IntPtr? hptrParent = (page.Parent?.ControlImplementation.Handle as GTKNativeControl)?.Handle;
			IntPtr? hptr = (page.ControlImplementation.Handle as GTKNativeControl)?.Handle;
			SetTabPageDetachable(hptrParent, hptr, value);
		}
		public void SetTabPageReorderable(IntPtr? hptrParent, IntPtr? hptr, bool value)
		{
			Internal.GTK.Methods.GtkNotebook.gtk_notebook_set_tab_reorderable(hptrParent.GetValueOrDefault(), hptr.GetValueOrDefault(), value);
		}
		public void SetTabPageDetachable(IntPtr? hptrParent, IntPtr? hptr, bool value)
		{
			Internal.GTK.Methods.GtkNotebook.gtk_notebook_set_tab_detachable(hptrParent.GetValueOrDefault(), hptr.GetValueOrDefault(), value);
		}

		public void InsertTabPage(int index, TabPage item)
		{
			if (!Control.IsCreated)
				return;

			IntPtr handle = (Engine.GetHandleForControl(Control) as GTKNativeControl).Handle;
			NotebookAppendPage((Control as TabContainer), handle, item, index);
		}

		public void RemoveTabPage(TabPage tabPage)
		{
			throw new NotImplementedException();
		}

		private static Func<IntPtr, IntPtr, int, int, IntPtr, IntPtr> create_window_d = null;
		private static IntPtr /*GtkNotebook*/ create_window(IntPtr /*GtkNotebook*/ notebook, IntPtr /*GtkWidget*/ page, int x, int y, IntPtr user_data)
		{
			TabContainer tbsParent = ((Application.Engine as GTKEngine).GetControlByHandle(notebook) as TabContainer);
			TabPage tabPage = GetTabPageByHandle(page);
			if (tbsParent == null) return IntPtr.Zero;

			TabPageDetachedEventArgs ee = new TabPageDetachedEventArgs(tbsParent, tabPage);
			InvokeMethod(tbsParent, "OnTabPageDetached", new object[] { ee });

			if (ee.Handled)
			{
				return IntPtr.Zero; // replace with GetControlHandle...
			}


			string groupName = Internal.GTK.Methods.GtkNotebook.gtk_notebook_get_group_name(notebook);

			Window window = new Window();
			window.Layout = new BoxLayout(Orientation.Vertical);
			window.Text = Internal.GTK.Methods.GtkNotebook.gtk_notebook_get_tab_label_text(notebook, page);
			window.Location = new Framework.Drawing.Vector2D(x, y);
			window.StartPosition = WindowStartPosition.Manual;

			TabContainer tbs = new TabContainer();
			tbs.TabPageDetached += tbsOurWindow_TabPageDetached;
			window.Controls.Add(tbs, new BoxLayout.Constraints(true, true));

			Application.Engine.CreateControl(tbs);
			Application.Engine.CreateControl(window);

			Internal.GTK.Methods.GtkNotebook.gtk_notebook_set_group_name((Application.Engine.GetHandleForControl(tbs) as GTKNativeControl).Handle, groupName);
			return (Application.Engine.GetHandleForControl(tbs) as GTKNativeControl).Handle;
		}

		static void tbsOurWindow_TabPageDetached(object sender, TabPageDetachedEventArgs e)
		{
			TabContainer parent = (sender as TabContainer);
			if (parent.TabPages.Count == 0)
			{
				parent.ParentWindow.Close();
			}
		}


		protected override NativeControl CreateControlInternal(Control control)
		{
			TabContainer ctl = (control as TabContainer);
			IntPtr handle = Internal.GTK.Methods.GtkNotebook.gtk_notebook_new();

			foreach (TabPage tabPage in ctl.TabPages)
			{
				NotebookAppendPage(ctl, handle, tabPage);
			}

			Internal.GObject.Methods.g_signal_connect(handle, "create_window", create_window_d, IntPtr.Zero);

			return new GTKNativeControl(handle);
		}
	}
}
