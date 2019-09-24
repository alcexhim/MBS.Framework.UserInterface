using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls
{
	public class ContainerImplementation : WindowsFormsNativeImplementation
	{
		public ContainerImplementation(Engine engine, Container control) : base(engine, control)
		{
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			System.Windows.Forms.Panel hContainer = null;
			Container container = (control as Container);

			Layout layout = container.Layout;
			if (container.Layout == null) layout = new Layouts.AbsoluteLayout();

			if (layout is BoxLayout)
			{
				BoxLayout box = (layout as BoxLayout);
				hContainer = new System.Windows.Forms.TableLayoutPanel();
			}
			else if (layout is AbsoluteLayout)
			{
				AbsoluteLayout abs = (layout as AbsoluteLayout);
				hContainer = new System.Windows.Forms.Panel();
			}
			else if (layout is GridLayout)
			{
				hContainer = new System.Windows.Forms.TableLayoutPanel();
			}

			if (hContainer != null)
			{
				foreach (Control ctl in container.Controls)
				{
					bool ret = Engine.CreateControl(ctl);
					if (!ret) continue;

					ApplyLayout(hContainer, ctl, layout);
				}
			}

			return new WindowsFormsNativeControl(hContainer);
		}

		private void ApplyLayout(System.Windows.Forms.Panel hContainer, Control ctl, Layout layout)
		{
			System.Windows.Forms.Control ctlNative = (Engine.GetHandleForControl(ctl) as WindowsFormsNativeControl).Handle;

			if (layout is BoxLayout)
			{
				BoxLayout box = (layout as BoxLayout);
				BoxLayout.Constraints c = (box.GetControlConstraints(ctl) as BoxLayout.Constraints);
				if (c == null) c = new BoxLayout.Constraints();

				int padding = c.Padding == 0 ? ctl.Padding.All : c.Padding;

				switch (c.PackType)
				{
					case BoxLayout.PackType.Start:
					case BoxLayout.PackType.End:
					{
						System.Windows.Forms.AnchorStyles anchor = System.Windows.Forms.AnchorStyles.None;
						if (c.Expand)
						{
							anchor = System.Windows.Forms.AnchorStyles.None;
						}
						else
						{
							anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
						}
						if (c.Fill)
						{
							anchor |= System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
						}
						ctlNative.Anchor = anchor;

						(hContainer as System.Windows.Forms.TableLayoutPanel).Controls.Add(ctlNative, 0, 0);
						// (hContainer as System.Windows.Forms.TableLayoutPanel).SetRowSpan(ctl
						break;
					}
				}
			}
			else if (layout is AbsoluteLayout)
			{
				AbsoluteLayout.Constraints constraints = (layout.GetControlConstraints(ctl) as AbsoluteLayout.Constraints);
				if (constraints == null) constraints = new AbsoluteLayout.Constraints(0, 0, 0, 0);
				hContainer.Controls.Add(ctlNative);
				ctlNative.Location = new System.Drawing.Point(constraints.X, constraints.Y);
				ctlNative.Size = new System.Drawing.Size(constraints.Width, constraints.Height);
			}
			else if (layout is GridLayout)
			{
				GridLayout.Constraints constraints = (layout.GetControlConstraints(ctl) as GridLayout.Constraints);
				if (constraints != null)
				{
					(hContainer as System.Windows.Forms.TableLayoutPanel).Controls.Add(ctlNative, constraints.Column, constraints.Row);
					(hContainer as System.Windows.Forms.TableLayoutPanel).SetColumnSpan(ctlNative, constraints.ColumnSpan);
					(hContainer as System.Windows.Forms.TableLayoutPanel).SetRowSpan(ctlNative, constraints.RowSpan);

					switch (constraints.Expand)
					{
					}
				}
			}
			else
			{
				hContainer.Controls.Add(ctlNative);
			}
		}




		protected override Dimension2D GetControlSizeInternal()
		{
			throw new System.NotImplementedException();
		}

		protected override Cursor GetCursorInternal()
		{
			throw new System.NotImplementedException();
		}

		protected override string GetTooltipTextInternal()
		{
			throw new System.NotImplementedException();
		}

		protected override void RegisterDragSourceInternal(Control control, DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			throw new System.NotImplementedException();
		}

		protected override void RegisterDropTargetInternal(Control control, DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			throw new System.NotImplementedException();
		}

		protected override void SetControlVisibilityInternal(bool visible)
		{
			throw new System.NotImplementedException();
		}

		protected override void SetCursorInternal(Cursor value)
		{
			throw new System.NotImplementedException();
		}

		protected override void SetFocusInternal()
		{
			throw new System.NotImplementedException();
		}

		protected override void SetTooltipTextInternal(string value)
		{
			throw new System.NotImplementedException();
		}
	}
}