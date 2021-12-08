using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Native;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface.Engines.GTK3.Controls
{
	[ControlImplementation(typeof(TextBox))]
	public class TextBoxImplementation : GTKNativeImplementation, ITextBoxImplementation
	{
		public TextBoxImplementation(Engine engine, Control control) : base(engine, control)
		{
			TextBox_Changed_Handler = new Internal.GObject.Delegates.GCallback(TextBox_Changed);
			TextBuffer_Changed_Handler = new Internal.GObject.Delegates.GCallbackV1I(TextBuffer_Changed);

			populate_popup_d = new Action<IntPtr, IntPtr, IntPtr>(populate_popup);
		}

		public void InsertText(string content)
		{
			TextBox ctl = (Control as TextBox);
			if (ctl.Multiline)
			{
				IntPtr hScrolledWindow = (Engine.GetHandleForControl(Control) as GTKNativeControl).Handle;
				IntPtr hList = Internal.GTK.Methods.GtkContainer.gtk_container_get_children(hScrolledWindow);
				IntPtr hTextBox = Internal.GLib.Methods.g_list_nth_data(hList, 0);

				IntPtr hBuffer = Internal.GTK.Methods.GtkTextView.gtk_text_view_get_buffer(hTextBox);
				Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_insert_at_cursor(hBuffer, content, content.Length);
			}
		}

		protected override string GetControlTextInternal(Control control)
		{
			IntPtr handle = (Engine.GetHandleForControl(control) as GTKNativeControl).Handle;

			GType typeEntry = Internal.GTK.Methods.GtkEntry.gtk_entry_get_type();
			bool isTextBox = Internal.GObject.Methods.G_TYPE_CHECK_INSTANCE_TYPE(handle, typeEntry);

			TextBox ctl = (control as TextBox);
			/*
			if (textboxChanged.ContainsKey (handle)) {
				if (!textboxChanged [handle])
					return null;
			}
			*/
			// textboxChanged [handle] = false;

			string value = String.Empty;

			if (ctl.Multiline)
			{
				// handle points to the ScrolledWindow
				IntPtr hTextBox = (Engine.GetHandleForControl(control) as GTKNativeControl).GetNamedHandle("TextBox");
				IntPtr hBuffer = Internal.GTK.Methods.GtkTextView.gtk_text_view_get_buffer(hTextBox);

				Internal.GTK.Structures.GtkTextIter hStartIter = new Internal.GTK.Structures.GtkTextIter();
				Internal.GTK.Structures.GtkTextIter hEndIter = new Internal.GTK.Structures.GtkTextIter();

				Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_start_iter(hBuffer, ref hStartIter);
				Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_end_iter(hBuffer, ref hEndIter);

				value = Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_text(hBuffer, ref hStartIter, ref hEndIter, true);
			}
			else
			{
				ushort textLength = Internal.GTK.Methods.GtkEntry.gtk_entry_get_text_length(handle);
				if (textLength > 0)
				{
					try
					{
						value = Internal.GTK.Methods.GtkEntry.gtk_entry_get_text(handle);
					}
					catch (Exception ex)
					{
						Console.Error.WriteLine(ex.Message);
					}
				}
			}
			return value;
		}
		protected override void SetControlTextInternal(Control control, string text)
		{
			if (text == null)
				text = String.Empty;

			TextBox ctl = (control as TextBox);
			if (ctl.Multiline)
			{
				IntPtr hTextBox = (Engine.GetHandleForControl(control) as GTKNativeControl).GetNamedHandle("TextBox");
				IntPtr hBuffer = Internal.GTK.Methods.GtkTextView.gtk_text_view_get_buffer(hTextBox);

				text = text.Replace('\0', ' ');

				int len = System.Text.Encoding.UTF8.GetByteCount(text);
				Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_set_text(hBuffer, text, len);
			}
			else
			{
				// this isn't working.. why not?
				IntPtr hTextBox = (Engine.GetHandleForControl(control) as GTKNativeControl).Handle;
				Internal.GTK.Methods.GtkEntry.gtk_entry_set_text(hTextBox, text);
			}
		}

		private Action<IntPtr /*GtkTextView*/, IntPtr /*GtkWidget*/, IntPtr> populate_popup_d;
		private void populate_popup(IntPtr /*GtkTextView*/ text_view, IntPtr /*GtkWidget*/ popup, IntPtr user_data)
		{

		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			Contract.Assert(control is TextBox);

			TextBox ctl = (control as TextBox);
			ctl.CompletionModel.TreeModelChanged += CompletionModel_TreeModelChanged;

			IntPtr handle = IntPtr.Zero;
			if (ctl.Multiline)
			{
				handle = Internal.GTK.Methods.GtkTextView.gtk_text_view_new();
			}
			else
			{
				if (ctl.UsageHint == TextBoxUsageHint.Search)
				{
					handle = Internal.GTK.Methods.GtkSearchEntry.gtk_search_entry_new();
					Internal.GObject.Methods.g_signal_connect(handle, "search-changed", TextBox_Changed_Handler);
				}
				else
				{
					handle = Internal.GTK.Methods.GtkEntry.gtk_entry_new();
					Internal.GObject.Methods.g_signal_connect(handle, "changed", TextBox_Changed_Handler);
				}
			}

			IntPtr /*GtkEntryCompletion*/ hCompletion = Internal.GTK.Methods.GtkEntryCompletion.gtk_entry_completion_new();
			Internal.GTK.Methods.GtkEntry.gtk_entry_set_completion(handle, hCompletion);

			Internal.GTK.Methods.GtkEntryCompletion.gtk_entry_completion_set_model(hCompletion, (Engine.CreateTreeModel(ctl.CompletionModel) as GTKNativeTreeModel).Handle);
			Internal.GTK.Methods.GtkEntryCompletion.gtk_entry_completion_set_text_column(hCompletion, 0);


			string ctlText = ctl.Text;
			if (ctl.Multiline)
			{
				IntPtr hTextTagTable = Internal.GTK.Methods.GtkTextTagTable.gtk_text_tag_table_new();
				IntPtr hBuffer = Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_new(hTextTagTable);
				Internal.GTK.Methods.GtkTextView.gtk_text_view_set_buffer(handle, hBuffer);
				Internal.GTK.Methods.GtkTextView.gtk_text_view_set_wrap_mode(handle, Internal.GTK.Constants.GtkWrapMode.Word);
				Internal.GTK.Methods.GtkTextView.gtk_text_view_set_editable(handle, ctl.Editable);

				Internal.GObject.Methods.g_signal_connect(handle, "populate_popup", populate_popup_d);

				_TextBoxForBuffer[hBuffer] = ctl;

				if (ctlText != null)
				{
					Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_set_text(hBuffer, ctlText, ctlText.Length);
				}
				Internal.GObject.Methods.g_signal_connect(hBuffer, "changed", TextBuffer_Changed_Handler);

				IntPtr hHAdjustment = Internal.GTK.Methods.GtkAdjustment.gtk_adjustment_new(0, 0, 100, 1, 10, 10);
				IntPtr hVAdjustment = Internal.GTK.Methods.GtkAdjustment.gtk_adjustment_new(0, 0, 100, 1, 10, 10);

				IntPtr hScrolledWindow = Internal.GTK.Methods.GtkScrolledWindow.gtk_scrolled_window_new(hHAdjustment, hVAdjustment);

				if (ctl.BorderStyle == ControlBorderStyle.None)
				{
					// do nothing
				}
				else if (ctl.BorderStyle == ControlBorderStyle.Default)
				{
					// on GTK, default is do nothing
				}
				else
				{
					// GTK does not distinguish between FixedSingle and Fixed3D (that I know of)
					IntPtr hStyleCtx = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(hScrolledWindow);
					Internal.GTK.Methods.GtkStyleContext.gtk_style_context_add_class(hStyleCtx, "frame");
				}

				Internal.GTK.Methods.GtkContainer.gtk_container_add(hScrolledWindow, handle);
				return new GTKNativeControl(hScrolledWindow, new KeyValuePair<string, IntPtr>[]
				{
					new KeyValuePair<string, IntPtr>("ScrolledWindow", hScrolledWindow),
					new KeyValuePair<string, IntPtr>("TextBox", handle)
				});
			}
			else
			{
				if (ctl.MaxLength > -1)
				{
					Internal.GTK.Methods.GtkEntry.gtk_entry_set_max_length(handle, ctl.MaxLength);
				}
				if (ctl.WidthChars > -1)
				{
					Internal.GTK.Methods.GtkEntry.gtk_entry_set_width_chars(handle, ctl.WidthChars);
				}

				switch (ctl.TextAlignment)
				{
					case HorizontalAlignment.Left:
					{
						Internal.GTK.Methods.GtkEntry.gtk_entry_set_alignment(handle, 0.0f);
						break;
					}
					case HorizontalAlignment.Center:
					{
						Internal.GTK.Methods.GtkEntry.gtk_entry_set_alignment(handle, 0.5f);
						break;
					}
					case HorizontalAlignment.Right:
					{
						Internal.GTK.Methods.GtkEntry.gtk_entry_set_alignment(handle, 1.0f);
						break;
					}
				}

				if (ctl.BorderStyle == ControlBorderStyle.None)
				{
					Internal.GTK.Methods.GtkEntry.gtk_entry_set_has_frame(handle, false);
				}
				else
				{
					Internal.GTK.Methods.GtkEntry.gtk_entry_set_has_frame(handle, true);
				}

				Internal.GTK.Methods.GtkEntry.gtk_entry_set_activates_default(handle, true);
				Internal.GTK.Methods.GtkEntry.gtk_entry_set_visibility(handle, !ctl.UseSystemPasswordChar);
				Internal.GTK.Methods.GtkEditable.gtk_editable_set_editable(handle, ctl.Editable);

				if (ctlText != null)
				{
					Internal.GTK.Methods.GtkEntry.gtk_entry_set_text(handle, ctlText);
				}
				return new GTKNativeControl(handle, new KeyValuePair<string, IntPtr>[]
				{
					new KeyValuePair<string, IntPtr>("TextBox", handle)
				});
			}
		}

		void CompletionModel_TreeModelChanged(object sender, TreeModelChangedEventArgs e)
		{
			if (e.Action == TreeModelChangedAction.Add)
			{
				DefaultTreeModel coll = (sender as DefaultTreeModel);
				(((UIApplication)Application.Instance).Engine as GTK3Engine).UpdateTreeModel(coll, e);
			}
		}



		private Dictionary<IntPtr, bool> textboxChanged = new Dictionary<IntPtr, bool>();
		private Internal.GObject.Delegates.GCallback TextBox_Changed_Handler;
		private Internal.GObject.Delegates.GCallbackV1I TextBuffer_Changed_Handler;
		private void TextBox_Changed(IntPtr handle, IntPtr data)
		{
			TextBox ctl = Control as TextBox;
			if (ctl == null)
				return;

			textboxChanged[handle] = true;
			InvokeMethod(ctl, "OnChanged", EventArgs.Empty);
		}

		private Dictionary<IntPtr, TextBox> _TextBoxForBuffer = new Dictionary<IntPtr, TextBox>();
		private void TextBuffer_Changed(IntPtr handle)
		{
			if (_TextBoxForBuffer.ContainsKey(handle))
			{
				TextBox ctl = _TextBoxForBuffer[handle];
				if (ctl == null)
					return;

				textboxChanged[handle] = true;
				InvokeMethod(ctl, "OnChanged", EventArgs.Empty);
			}
		}

		public int GetSelectionStart()
		{
			TextBox ctl = (Control as TextBox);
			if (ctl.Multiline)
			{
				IntPtr hScrolledWindow = (Engine.GetHandleForControl(Control) as GTKNativeControl).Handle;
				IntPtr hList = Internal.GTK.Methods.GtkContainer.gtk_container_get_children(hScrolledWindow);
				IntPtr hTextBox = Internal.GLib.Methods.g_list_nth_data(hList, 0);

				IntPtr hBuffer = Internal.GTK.Methods.GtkTextView.gtk_text_view_get_buffer(hTextBox);
				Internal.GTK.Structures.GtkTextIter iterStart = new Internal.GTK.Structures.GtkTextIter();
				Internal.GTK.Structures.GtkTextIter iterEnd = new Internal.GTK.Structures.GtkTextIter();
				bool success = Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_selection_bounds(hBuffer, ref iterStart, ref iterEnd);
				if (success)
				{
					return Internal.GTK.Methods.GtkTextIter.gtk_text_iter_get_offset(ref iterStart);
				}
				return 0;
			}
			return -1;
		}
		public void SetSelectionStart(int pos)
		{
			throw new NotImplementedException();
		}
		public int GetSelectionLength()
		{
			TextBox ctl = (Control as TextBox);
			if (ctl.Multiline)
			{
				IntPtr hScrolledWindow = (Engine.GetHandleForControl(Control) as GTKNativeControl).Handle;
				IntPtr hList = Internal.GTK.Methods.GtkContainer.gtk_container_get_children(hScrolledWindow);
				IntPtr hTextBox = Internal.GLib.Methods.g_list_nth_data(hList, 0);

				IntPtr hBuffer = Internal.GTK.Methods.GtkTextView.gtk_text_view_get_buffer(hTextBox);
				Internal.GTK.Structures.GtkTextIter iterStart = new Internal.GTK.Structures.GtkTextIter();
				Internal.GTK.Structures.GtkTextIter iterEnd = new Internal.GTK.Structures.GtkTextIter();
				bool success = Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_selection_bounds(hBuffer, ref iterStart, ref iterEnd);
				if (success)
				{
					int start = Internal.GTK.Methods.GtkTextIter.gtk_text_iter_get_offset(ref iterStart);
					int end = Internal.GTK.Methods.GtkTextIter.gtk_text_iter_get_offset(ref iterEnd);
					return end - start;
				}
			}
			return 0;
		}
		public void SetSelectionLength(int len)
		{
			throw new NotImplementedException();
		}

		public string GetSelectedText()
		{
			TextBox ctl = (Control as TextBox);
			if (ctl.Multiline)
			{
				IntPtr hScrolledWindow = (Engine.GetHandleForControl(Control) as GTKNativeControl).Handle;
				IntPtr hList = Internal.GTK.Methods.GtkContainer.gtk_container_get_children(hScrolledWindow);
				IntPtr hTextBox = Internal.GLib.Methods.g_list_nth_data(hList, 0);

				IntPtr hBuffer = Internal.GTK.Methods.GtkTextView.gtk_text_view_get_buffer(hTextBox);
				Internal.GTK.Structures.GtkTextIter iterStart = new Internal.GTK.Structures.GtkTextIter();
				Internal.GTK.Structures.GtkTextIter iterEnd = new Internal.GTK.Structures.GtkTextIter();
				bool success = Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_selection_bounds(hBuffer, ref iterStart, ref iterEnd);
				if (success)
				{
					string value = Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_text(hBuffer, ref iterStart, ref iterEnd, true);
					return value;
				}
			}
			return null;
		}

		public void SetSelectedText(string text)
		{
			TextBox ctl = (Control as TextBox);
			if (ctl.Multiline)
			{
				IntPtr hScrolledWindow = (Engine.GetHandleForControl(Control) as GTKNativeControl).Handle;
				IntPtr hList = Internal.GTK.Methods.GtkContainer.gtk_container_get_children(hScrolledWindow);
				IntPtr hTextBox = Internal.GLib.Methods.g_list_nth_data(hList, 0);

				IntPtr hBuffer = Internal.GTK.Methods.GtkTextView.gtk_text_view_get_buffer(hTextBox);
				Internal.GTK.Structures.GtkTextIter iterStart = new Internal.GTK.Structures.GtkTextIter();
				Internal.GTK.Structures.GtkTextIter iterEnd = new Internal.GTK.Structures.GtkTextIter();
				bool success = Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_selection_bounds(hBuffer, ref iterStart, ref iterEnd);

				Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_delete(hBuffer, ref iterStart, ref iterEnd);
				if (!String.IsNullOrEmpty(text))
				{
					Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_insert(hBuffer, ref iterStart, text, text.Length);
				}
			}
		}

		private bool _IsEditable = true;
		public bool IsEditable()
		{
			if (Handle == null)
				return _IsEditable;

			IntPtr handle = (Handle as GTKNativeControl).GetNamedHandle("TextBox");
			if ((Control as TextBox).Multiline)
			{
				return Internal.GTK.Methods.GtkTextView.gtk_text_view_get_editable(handle);
			}
			else
			{
				return Internal.GTK.Methods.GtkEditable.gtk_editable_get_editable(handle);
			}
		}
		public void SetEditable(bool value)
		{
			_IsEditable = value;
			if (Handle == null)
				return;

			IntPtr handle = (Handle as GTKNativeControl).GetNamedHandle("TextBox");
			if ((Control as TextBox).Multiline)
			{
				Internal.GTK.Methods.GtkTextView.gtk_text_view_set_editable(handle, value);
			}
			else
			{
				Internal.GTK.Methods.GtkEditable.gtk_editable_set_editable(handle, value);
			}
		}

		public HorizontalAlignment GetTextAlignment()
		{
			IntPtr handle = (Handle as GTKNativeControl).GetNamedHandle("TextBox");
			float value = Internal.GTK.Methods.GtkEntry.gtk_entry_get_alignment(handle);
			if (value < 0.4f)
			{
				return HorizontalAlignment.Left;
			}
			else if (value > 0.4f || value < 0.6f)
			{
				return HorizontalAlignment.Center;
			}
			else if (value > 0.6f)
			{
				return HorizontalAlignment.Right;
			}
			return HorizontalAlignment.Default;
		}
		public void SetTextAlignment(HorizontalAlignment value)
		{
			IntPtr handle = (Handle as GTKNativeControl).GetNamedHandle("TextBox");
			switch (value)
			{
				case HorizontalAlignment.Left:
				{
					Internal.GTK.Methods.GtkEntry.gtk_entry_set_alignment(handle, 0.0f);
					break;
				}
				case HorizontalAlignment.Center:
				{
					Internal.GTK.Methods.GtkEntry.gtk_entry_set_alignment(handle, 0.5f);
					break;
				}
				case HorizontalAlignment.Right:
				{
					Internal.GTK.Methods.GtkEntry.gtk_entry_set_alignment(handle, 1.0f);
					break;
				}
			}
		}





		internal Internal.GTK.Structures.GtkTextIter GetStartIter()
		{
			IntPtr hBuffer = (Handle as GTKNativeControl).GetNamedHandle("TextBuffer");
			if (hBuffer == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBuffer' is NULL");
			}

			Internal.GTK.Structures.GtkTextIter iter = new Internal.GTK.Structures.GtkTextIter();
			Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_start_iter(hBuffer, ref iter);
			return iter;
		}
		internal Internal.GTK.Structures.GtkTextIter GetEndIter()
		{
			IntPtr hBuffer = (Handle as GTKNativeControl).GetNamedHandle("TextBuffer");
			if (hBuffer == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBuffer' is NULL");
			}

			Internal.GTK.Structures.GtkTextIter iter = new Internal.GTK.Structures.GtkTextIter();
			Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_end_iter(hBuffer, ref iter);
			return iter;
		}
		internal Internal.GTK.Structures.GtkTextIter GetIterAtPosition(int column)
		{
			IntPtr hBuffer = (Handle as GTKNativeControl).GetNamedHandle("TextBuffer");
			if (hBuffer == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBuffer' is NULL");
			}

			Internal.GTK.Structures.GtkTextIter iter = new Internal.GTK.Structures.GtkTextIter();
			Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_iter_at_offset(hBuffer, ref iter, column);
			return iter;
		}
		internal Internal.GTK.Structures.GtkTextIter GetIterAtLine(int line, int column = 0)
		{
			IntPtr hBuffer = (Handle as GTKNativeControl).GetNamedHandle("TextBuffer");
			if (hBuffer == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBuffer' is NULL");
			}

			Internal.GTK.Structures.GtkTextIter iter = new Internal.GTK.Structures.GtkTextIter();
			Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_iter_at_line_offset(hBuffer, ref iter, line, column);
			return iter;
		}

		internal MBS.Framework.Drawing.Rectangle GetIterLocation(Internal.GTK.Structures.GtkTextIter iter)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GDK.Structures.GdkRectangle rect = new Internal.GDK.Structures.GdkRectangle();
			Internal.GTK.Methods.GtkTextView.gtk_text_view_get_iter_location(handle, ref iter, ref rect);
			return new Framework.Drawing.Rectangle(rect.x, rect.y, rect.width, rect.height);
		}

		private MBS.Framework.Collections.Generic.HandleDictionary<TextBoxStyleDefinition> _StyleDefinitions = new Collections.Generic.HandleDictionary<TextBoxStyleDefinition>();

		public void ClearStyleDefinitions()
		{
			IntPtr hBuffer = (Handle as GTKNativeControl).GetNamedHandle("TextBuffer");
			if (hBuffer == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBuffer' is NULL");
			}

			IntPtr hTagTable = Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_tag_table(hBuffer);
		}

		public void AddStyleDefinition(TextBoxStyleDefinition item)
		{
			IntPtr hTextBox = (Handle as GTKNativeControl).GetNamedHandle("TextBox");
			if (hTextBox == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBox' is NULL");
			}

			IntPtr hBuffer = Internal.GTK.Methods.GtkTextView.gtk_text_view_get_buffer(hTextBox);
			if (hBuffer == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBuffer' is NULL");
			}

			IntPtr hTagTable = Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_tag_table(hBuffer);
			IntPtr hTag = Internal.GTK.Methods.GtkTextTag.gtk_text_tag_new(item.Name);

			if (item.BackgroundColor != Color.Empty)
			{
				Internal.GObject.Methods.g_object_set_property(hTag, "background-rgba", (Engine as GTK3Engine).ColorToGDKRGBA(item.BackgroundColor));
				Internal.GObject.Methods.g_object_set_property(hTag, "background-set", true);
			}
			if (item.ForegroundColor != Color.Empty)
			{
				Internal.GObject.Methods.g_object_set_property(hTag, "foreground-rgba", (Engine as GTK3Engine).ColorToGDKRGBA(item.ForegroundColor));
				Internal.GObject.Methods.g_object_set_property(hTag, "foreground-set", true);
			}

			Internal.GObject.Methods.g_object_set_property(hTag, "editable-set", true);
			Internal.GObject.Methods.g_object_set_property(hTag, "editable", item.Editable);

			_StyleDefinitions.Add(hTag, item);
			Internal.GTK.Methods.GtkTextTagTable.gtk_text_tag_table_add(hTagTable, hTag);
		}

		public void RemoveStyleDefinition(TextBoxStyleDefinition item)
		{
			IntPtr hBuffer = (Handle as GTKNativeControl).GetNamedHandle("TextBuffer");
			if (hBuffer == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBuffer' is NULL");
			}

			IntPtr hTagTable = Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_tag_table(hBuffer);
			Internal.GTK.Methods.GtkTextTagTable.gtk_text_tag_table_remove(hTagTable, _StyleDefinitions.GetHandle(item));
			_StyleDefinitions.Remove(item);
		}

		public void ClearStyleAreas()
		{

		}

		public void AddStyleArea(TextBoxStyleArea item)
		{
			IntPtr hTextBox = (Handle as GTKNativeControl).GetNamedHandle("TextBox");
			if (hTextBox == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBox' is NULL");
			}

			IntPtr hBuffer = Internal.GTK.Methods.GtkTextView.gtk_text_view_get_buffer(hTextBox);
			if (hBuffer == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBuffer' is NULL");
			}

			IntPtr hTag = _StyleDefinitions.GetHandle(item.Style);
			if (hTag == IntPtr.Zero)
				return;

			Internal.GTK.Structures.GtkTextIter iterStart = new Internal.GTK.Structures.GtkTextIter(), iterEnd = new Internal.GTK.Structures.GtkTextIter();
			Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_iter_at_offset(hBuffer, ref iterStart, item.Start);
			Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_get_iter_at_offset(hBuffer, ref iterEnd, item.Start + item.Length);

			Internal.GTK.Methods.GtkTextBuffer.gtk_text_buffer_apply_tag(hBuffer, hTag, ref iterStart, ref iterEnd);
		}

		public void RemoveStyleArea(TextBoxStyleArea item)
		{
			IntPtr hBuffer = (Handle as GTKNativeControl).GetNamedHandle("TextBuffer");
			if (hBuffer == IntPtr.Zero)
			{
				Console.Error.WriteLine("uwt: SyntaxTextBox: named handle 'TextBuffer' is NULL");
			}

			throw new NotImplementedException();
		}
	}
}
