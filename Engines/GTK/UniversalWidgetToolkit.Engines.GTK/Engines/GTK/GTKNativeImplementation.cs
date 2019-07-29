//
//  GTKNativeImplementation.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 
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
using UniversalWidgetToolkit.Input.Keyboard;
using UniversalWidgetToolkit.Input.Mouse;

namespace UniversalWidgetToolkit.Engines.GTK
{
	public abstract class GTKNativeImplementation : NativeImplementation
	{
		public GTKNativeImplementation(Engine engine, Control control) : base(engine, control)
		{

		}

		internal virtual void RegisterDragSourceGTK(IntPtr handle, Internal.GDK.Constants.GdkModifierType modifiers, Internal.GTK.Structures.GtkTargetEntry[] targets, Internal.GDK.Constants.GdkDragAction actions)
		{
			Internal.GTK.Methods.GtkDragSource.gtk_drag_source_set(handle, modifiers, targets, targets.Length, actions);
		}
		internal virtual void RegisterDropTargetGTK(IntPtr handle, Internal.GDK.Constants.GdkModifierType modifiers, Internal.GTK.Structures.GtkTargetEntry[] targets, Internal.GDK.Constants.GdkDragAction actions)
		{
			Internal.GTK.Methods.GtkDragDest.gtk_drag_dest_set(handle, Internal.GTK.Constants.GtkDestDefaults.All, targets, targets.Length, actions);
		}
		protected override void RegisterDropTargetInternal(Control control, DragDrop.DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			Internal.GDK.Constants.GdkModifierType modifiers = GTKEngine.KeyboardModifierKeyToGdkModifierType(modifierKeys) | GTKEngine.MouseButtonsToGdkModifierType(buttons);
			
			IntPtr handle = Engine.GetHandleForControl(control);
			if (handle == IntPtr.Zero) return;
			
			RegisterDropTargetGTK(handle, modifiers, GTKEngine.DragDropTargetToGtkTargetEntry(targets), GTKEngine.DragDropEffectToGdkDragAction(actions));
		}
		protected override void RegisterDragSourceInternal(Control control, DragDrop.DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			Internal.GDK.Constants.GdkModifierType modifiers = GTKEngine.KeyboardModifierKeyToGdkModifierType(modifierKeys) | GTKEngine.MouseButtonsToGdkModifierType(buttons);
			
			IntPtr handle = Engine.GetHandleForControl(control);
			if (handle == IntPtr.Zero) return;
			
			RegisterDragSourceGTK(handle, modifiers, GTKEngine.DragDropTargetToGtkTargetEntry(targets), GTKEngine.DragDropEffectToGdkDragAction(actions));
		}

		protected override void OnCreated (EventArgs e)
		{
			base.OnCreated (e);

			IntPtr hCtrl = (Handle as GTKNativeControl).Handle;
			IntPtr hStyleContext = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context (hCtrl);
			foreach (ControlStyleClass cls in Control.Style.Classes) {
				Internal.GTK.Methods.GtkStyleContext.gtk_style_context_add_class (hStyleContext, cls.Value);
			}
		}
	}
}
