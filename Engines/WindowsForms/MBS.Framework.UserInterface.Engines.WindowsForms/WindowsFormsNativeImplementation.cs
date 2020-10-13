using System;
using System.Collections.Generic;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface.Engines.WindowsForms
{
	public abstract class WindowsFormsNativeImplementation : NativeImplementation
	{
		public WindowsFormsNativeImplementation (Engine engine, Control control) : base(engine, control)
		{
		}
		protected void InvokeIfRequired(System.Windows.Forms.Control ctl, Delegate method, params object[] args)
		{
			if (ctl.InvokeRequired)
			{
				ctl.Invoke(method, args);
			}
			else
			{
				method.DynamicInvoke(args);
			}
		}

		protected override void InvalidateInternal(int x, int y, int width, int height)
		{
			if (Handle is WindowsFormsNativeControl)
				(Handle as WindowsFormsNativeControl).Handle.Invalidate(new System.Drawing.Rectangle(x, y, width, height));
		}

		protected override void DestroyInternal()
		{
			if (Control is Dialog)
			{
				System.Windows.Forms.Form handle = ((Handle as WindowsFormsNativeControl).GetNamedHandle("dialog") as System.Windows.Forms.Form);
				handle.Close();
			}
			else if (Handle is WindowsFormsNativeDialog)
			{
				if ((Handle as WindowsFormsNativeDialog)?.Form != null)
				{
					(Handle as WindowsFormsNativeDialog)?.Form.Close();
				}
				else if ((Handle as WindowsFormsNativeDialog)?.Handle != null)
				{
					(Handle as WindowsFormsNativeDialog)?.Handle.Dispose();
				}
			}
			else
			{
				if ((Handle as WindowsFormsNativeControl).Handle is System.Windows.Forms.Form)
				{
					((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.Form).Close();
				}
				else
				{
					(Handle as WindowsFormsNativeControl).Handle.Dispose();
				}
			}
		}

		protected override bool SupportsEngineInternal(Type engineType)
		{
			return (engineType == typeof(WindowsFormsEngine));
		}

		protected override bool HasFocusInternal()
		{
			return ((Handle as WindowsFormsNativeControl).Handle).Focused;
		}

		protected override Dimension2D GetControlSizeInternal()
		{
			return new Framework.Drawing.Dimension2D((Handle as WindowsFormsNativeControl).Handle.Size.Width, ((Handle as WindowsFormsNativeControl).Handle.Size.Height));
		}
		protected override void SetControlSizeInternal(Dimension2D value)
		{
			(Handle as WindowsFormsNativeControl).Handle.Size = new System.Drawing.Size((int)value.Width, (int)value.Height);
		}

		protected override Cursor GetCursorInternal()
		{
			throw new NotImplementedException();
		}

		protected override string GetTooltipTextInternal()
		{
			throw new NotImplementedException();
		}

		private struct DRAGDROPDATA
		{
			public DragDropTarget[] targets;
			public DragDropEffect actions;
			public MouseButtons buttons;
			public KeyboardModifierKey modifierKeys;
		}
		private Dictionary<Control, List<DRAGDROPDATA>> _DDTargets = new Dictionary<Control, List<DRAGDROPDATA>>();

		protected override void RegisterDragSourceInternal(Control control, DragDropTarget[] targets, DragDropEffect actions, Input.Mouse.MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			if (!_DDTargets.ContainsKey(control))
			{
				System.Windows.Forms.Control ctl = (Engine.GetHandleForControl(control) as WindowsFormsNativeControl).Handle;
				ctl.MouseMove += control_MouseMove_DragSource;
				_DDTargets[control] = new List<DRAGDROPDATA>();
			}

			DRAGDROPDATA ddd = new DRAGDROPDATA();
			ddd.targets = targets;
			ddd.actions = actions;
			ddd.buttons = buttons;
			ddd.modifierKeys = modifierKeys;
			_DDTargets[control].Add(ddd);
		}

		private void dobj_DataRequest(DragDropDataRequestEventArgs e)
		{


		}

		private void control_MouseMove_DragSource(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				System.Windows.Forms.Control ctl = (sender as System.Windows.Forms.Control);

				List<DRAGDROPDATA> ddd = _DDTargets[ctl.Tag as Control];

				System.Windows.Forms.DataObject dobj = new System.Windows.Forms.DataObject();
				// dobj.Control = ctl.Tag as Control;
				// dobj.DataRequest += dobj_DataRequest;

				System.Windows.Forms.DragDropEffects effects = System.Windows.Forms.DragDropEffects.None;
				for (int i = 0; i < ddd.Count; i++)
				{
					if ((ddd[i].actions & DragDropEffect.Copy) == DragDropEffect.Copy) effects |= System.Windows.Forms.DragDropEffects.Copy;
					if ((ddd[i].actions & DragDropEffect.Link) == DragDropEffect.Link) effects |= System.Windows.Forms.DragDropEffects.Link;
					if ((ddd[i].actions & DragDropEffect.Move) == DragDropEffect.Move) effects |= System.Windows.Forms.DragDropEffects.Move;
					if ((ddd[i].actions & DragDropEffect.Scroll) == DragDropEffect.Scroll) effects |= System.Windows.Forms.DragDropEffects.Scroll;

					for (int j = 0; j < ddd[i].targets.Length; j++)
					{
						if (ddd[i].targets[j].Type == DragDropTargetTypes.FileList)
						{
							System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();
							dobj.SetFileDropList(sc);
						}
					}
				}

				ctl.DoDragDrop(dobj, effects);
			}
		}

		protected override void RegisterDropTargetInternal(Control control, DragDropTarget[] targets, DragDropEffect actions, Input.Mouse.MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
		}

		protected override void SetControlVisibilityInternal(bool visible)
		{
			if (Handle is Win32NativeControl)
			{
				Internal.Windows.Methods.ShowWindow((Handle as Win32NativeControl).Handle, visible ? Internal.Windows.Constants.ShowWindowCommand.Show : Internal.Windows.Constants.ShowWindowCommand.Hide);
				return;
			}
			else if (Handle is WindowsFormsNativeControl)
			{
				(Handle as WindowsFormsNativeControl).Handle.Visible = visible;
				return;
			}
			throw new NotSupportedException();
		}
		protected override bool IsControlVisibleInternal()
		{
			if (Handle is Win32NativeControl)
			{
				return Internal.Windows.Methods.IsWindowVisible((Handle as Win32NativeControl).Handle);
			}
			else if (Handle is WindowsFormsNativeControl)
			{
				return (Handle as WindowsFormsNativeControl).Handle.Visible;
			}
			throw new NotSupportedException();
		}

		protected override void SetCursorInternal(Cursor value)
		{
			// TODO: Implement cursors
			// System.Windows.Forms.Cursor.Current = _HCursors[value];
		}

		protected override void SetFocusInternal()
		{
			if (Handle is WindowsFormsNativeControl)
			{
				(Handle as WindowsFormsNativeControl).Handle.Focus();
			}
		}

		protected override void SetTooltipTextInternal(string value)
		{
			throw new NotImplementedException();
		}

		protected override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);

			SetupCommonEvents();
		}

		private void SetupCommonEvents()
		{
			System.Windows.Forms.Control ctl = ((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.Control);
			if (ctl == null)
				return;

			ctl.Text = Control.Text;

			ctl.Click += ctl_Click;
			ctl.MouseDown += ctl_MouseDown;
			ctl.MouseMove += ctl_MouseMove;
			ctl.MouseUp += ctl_MouseUp;
			ctl.MouseEnter += ctl_MouseEnter;
			ctl.MouseLeave += ctl_MouseLeave;
			ctl.KeyUp += ctl_KeyUp;
			ctl.KeyDown += ctl_KeyDown;
			ctl.PreviewKeyDown += ctl_PreviewKeyDown;
		}

		void ctl_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == System.Windows.Forms.Keys.Left || e.KeyCode == System.Windows.Forms.Keys.Right || e.KeyCode == System.Windows.Forms.Keys.Up || e.KeyCode == System.Windows.Forms.Keys.Down)
			{
				e.IsInputKey = true;
			}
		}


		protected override void SetControlTextInternal(Control control, string text)
		{
			if (text == null) text = String.Empty;
			if (Control.IsCreated)
			{
				if ((Handle as WindowsFormsNativeControl).Handle != null)
				{
					InvokeIfRequired((Handle as WindowsFormsNativeControl).Handle, new Action<System.Windows.Forms.Control, string>(delegate (System.Windows.Forms.Control ctl, string value)
					{
						ctl.Text = value.Replace('_', '&');
					}), new object[] { (Handle as WindowsFormsNativeControl).Handle, text });
				}
			}
		}

		void ctl_Click(object sender, EventArgs e)
		{
			InvokeMethod(this, "OnClick", new object[] { e });
		}
		void ctl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Input.Mouse.MouseEventArgs ee = new Input.Mouse.MouseEventArgs(e.X, e.Y, WindowsFormsEngine.SWFMouseButtonsToMouseButtons(e.Button), WindowsFormsEngine.SWFKeysToKeyboardModifierKey(System.Windows.Forms.Control.ModifierKeys));
			InvokeMethod(this, "OnMouseDown", new object[] { ee });

			if (ee.Handled)
				return;

			if (ee.Buttons == MouseButtons.Secondary)
			{
				// default implementation - display a context menu if we have one set
				// moved this up here to give us a chance to add a context menu if we don't have one associated yet
				OnBeforeContextMenu(ee);

				if (Control.ContextMenu != null)
				{
					Menu menu = Control.ContextMenu;

					System.Windows.Forms.ContextMenuStrip hMenu = (Engine as WindowsFormsEngine).BuildContextMenuStrip(menu);
					foreach (MenuItem mi in menu.Items)
					{
						RecursiveApplyMenuItemVisibility(mi);
					}
					hMenu.Show(System.Windows.Forms.Cursor.Position);

					OnAfterContextMenu(ee);
				}
			}
		}

		private void RecursiveApplyMenuItemVisibility(MenuItem mi)
		{
			if (mi == null)
				return;

			System.Windows.Forms.ToolStripItem hMi = ((Engine as WindowsFormsEngine).GetHandleForMenuItem(mi) as WindowsFormsNativeMenuItem).Handle as System.Windows.Forms.ToolStripItem;
			// hMi.Enabled = mi.Enabled;
			hMi.Visible = mi.Visible;

			if (mi is CommandMenuItem)
			{
				foreach (MenuItem mi1 in (mi as CommandMenuItem).Items)
				{
					RecursiveApplyMenuItemVisibility(mi1);
				}
			}
		}
		void ctl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Input.Mouse.MouseEventArgs ee = new Input.Mouse.MouseEventArgs(e.X, e.Y, WindowsFormsEngine.SWFMouseButtonsToMouseButtons(e.Button), WindowsFormsEngine.SWFKeysToKeyboardModifierKey(System.Windows.Forms.Control.ModifierKeys));
			InvokeMethod(this, "OnMouseMove", new object[] { ee });
		}
		void ctl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Input.Mouse.MouseEventArgs ee = new Input.Mouse.MouseEventArgs(e.X, e.Y, WindowsFormsEngine.SWFMouseButtonsToMouseButtons(e.Button), WindowsFormsEngine.SWFKeysToKeyboardModifierKey(System.Windows.Forms.Control.ModifierKeys));
			InvokeMethod(this, "OnMouseUp", new object[] { ee });
		}
		void ctl_MouseEnter(object sender, EventArgs e)
		{
			Input.Mouse.MouseEventArgs ee = new Input.Mouse.MouseEventArgs(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, WindowsFormsEngine.SWFMouseButtonsToMouseButtons(System.Windows.Forms.Control.MouseButtons), WindowsFormsEngine.SWFKeysToKeyboardModifierKey(System.Windows.Forms.Control.ModifierKeys));
			InvokeMethod(this, "OnMouseEnter", new object[] { ee });
		}
		void ctl_MouseLeave(object sender, EventArgs e)
		{
			Input.Mouse.MouseEventArgs ee = new Input.Mouse.MouseEventArgs(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, WindowsFormsEngine.SWFMouseButtonsToMouseButtons(System.Windows.Forms.Control.MouseButtons), WindowsFormsEngine.SWFKeysToKeyboardModifierKey(System.Windows.Forms.Control.ModifierKeys));
			InvokeMethod(this, "OnMouseLeave", new object[] { ee });
		}
		void ctl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			Input.Keyboard.KeyEventArgs ee = WindowsFormsEngine.SWFKeyEventArgsToKeyEventArgs(e);
			InvokeMethod(this, "OnKeyDown", new object[] { ee });

			if (ee.Cancel)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}
		void ctl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			Input.Keyboard.KeyEventArgs ee = WindowsFormsEngine.SWFKeyEventArgsToKeyEventArgs(e);
			InvokeMethod(this, "OnKeyUp", new object[] { ee });

			if (ee.Cancel)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		protected override double GetAdjustmentValueInternal(Orientation orientation)
		{
			return 0.0;
		}
		protected override void SetAdjustmentValueInternal(Orientation orientation, double value)
		{
		}

		protected override Dimension2D GetScrollBoundsInternal()
		{
			return Dimension2D.Empty;
		}
		protected override void SetScrollBoundsInternal(Dimension2D bounds)
		{
		}

		protected override AdjustmentScrollType GetAdjustmentScrollTypeInternal(Orientation orientation)
		{
			// FIXME: not implemented
			return AdjustmentScrollType.Never;
		}
		protected override void SetAdjustmentScrollTypeInternal(Orientation orientation, AdjustmentScrollType value)
		{
			// FIXME: not implemented
		}

		protected override HorizontalAlignment GetHorizontalAlignmentInternal()
		{
			// FIXME: not implemented
			return HorizontalAlignment.Default;
		}
		protected override void SetHorizontalAlignmentInternal(HorizontalAlignment value)
		{
			// FIXME: not implemented
		}
		protected override VerticalAlignment GetVerticalAlignmentInternal()
		{
			// FIXME: not implemented
			return VerticalAlignment.Default;
		}
		protected override void SetVerticalAlignmentInternal(VerticalAlignment value)
		{
			// FIXME: not implemented
		}

		protected override void UpdateControlFontInternal(Font font)
		{
			System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
			if (font.Italic)
				style |= System.Drawing.FontStyle.Italic;
			if (font.Weight >= FontWeights.Bold)
				style |= System.Drawing.FontStyle.Bold;
			(Handle as WindowsFormsNativeControl).Handle.Font = new System.Drawing.Font(new System.Drawing.FontFamily(font.FamilyName), (int)font.Size, style);
		}

		protected override IControlContainer GetParentControlInternal()
		{
			if ((Handle as WindowsFormsNativeControl).Handle.Parent == null)
				return null;

			Control ctl = (Engine as WindowsFormsEngine).GetControlByHandle((Handle as WindowsFormsNativeControl).Handle.Parent);
			if (ctl is IControlContainer)
				return ctl as IControlContainer;
			return new WindowsFormsNativeControlContainer((Handle as WindowsFormsNativeControl).Handle.Parent);
		}
		protected override Rectangle GetControlBoundsInternal()
		{
			return new Rectangle((Handle as WindowsFormsNativeControl).Handle.Bounds.X, (Handle as WindowsFormsNativeControl).Handle.Bounds.Y, (Handle as WindowsFormsNativeControl).Handle.Bounds.Width, (Handle as WindowsFormsNativeControl).Handle.Bounds.Height);
		}
		protected override void SetControlBoundsInternal(Rectangle bounds)
		{
			base.SetControlBoundsInternal(bounds);

			(Handle as WindowsFormsNativeControl).Handle.SetBounds((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
		}

	}
}
