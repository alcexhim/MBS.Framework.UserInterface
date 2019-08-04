using System;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Layouts;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[ControlImplementation(typeof(TabContainer))]
	public class TabContainerImplementation : GTKNativeImplementation
	{
		public TabContainerImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		public static void NotebookAppendPage(Engine engine, TabContainer ctl, IntPtr handle, TabPage page, int indexAfter = -1)
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

			engine.CreateControl(tabControlContainer);
			IntPtr hTabLabel = (engine.GetHandleForControl(tabControlContainer) as GTKNativeControl).Handle;

			ContainerImplementation cimpl = new ContainerImplementation(engine, page);
			cimpl.CreateControl(page);
			IntPtr container = (cimpl.Handle as GTKNativeControl).Handle;

			if (indexAfter == -1)
			{
				Internal.GTK.Methods.GtkNotebook.gtk_notebook_append_page(handle, container, hTabLabel);
				Internal.GTK.Methods.GtkWidget.gtk_widget_show_all (hTabLabel);
			}
			else
			{
			}
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			TabContainer ctl = (control as TabContainer);
			IntPtr handle = Internal.GTK.Methods.GtkNotebook.gtk_notebook_new();

			foreach (TabPage tabPage in ctl.TabPages)
			{
				NotebookAppendPage(Engine, ctl, handle, tabPage);
			}

			return new GTKNativeControl(handle);
		}
	}
}
