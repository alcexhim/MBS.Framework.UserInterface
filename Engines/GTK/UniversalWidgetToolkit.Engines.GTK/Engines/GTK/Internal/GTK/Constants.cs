using System;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.GTK
{
	internal static class Constants
	{
		public enum GtkWindowType
		{
			TopLevel = 0,
			Popup = 1
		}
		public enum GtkOrientation
		{
			Horizontal = 0,
			Vertical = 1
		}

		/// <summary>
		/// Prebuilt sets of buttons for the dialog. If none of these choices are appropriate, simply use %GTK_BUTTONS_NONE
		/// then call gtk_dialog_add_buttons().
		/// </summary>
		/// <remarks>
		/// Please note that %GTK_BUTTONS_OK, %GTK_BUTTONS_YES_NO and %GTK_BUTTONS_OK_CANCEL are discouraged by the
		/// <ulink url="http://library.gnome.org/devel/hig-book/stable/">GNOME HIG</ulink>.
		/// </remarks>
		public enum GtkButtonsType
		{
			/// <summary>
			/// no buttons at all
			/// </summary>
			None,
			/// <summary>
			/// an OK button
			/// </summary>
			OK,
			/// <summary>
			/// a Close button
			/// </summary>
			Close,
			/// <summary>
			/// a Cancel button
			/// </summary>
			Cancel,
			/// <summary>
			/// Yes and No buttons
			/// </summary>
			YesNo,
			/// <summary>
			/// OK and Cancel buttons
			/// </summary>
			OKCancel
		}

		/// <summary>
		/// Flags used to influence dialog construction.
		/// </summary>
		[Flags()]
		public enum GtkDialogFlags
		{
			/// <summary>
			/// No flags
			/// </summary>
			None = 0,
			/// <summary>
			/// Make the constructed dialog modal, see <see cref="Methods.gtk_window_set_modal" />
			/// </summary>
			Modal = 1,
			/// <summary>
			/// Destroy the dialog when its parent is destroyed, see <see cref="Methods.gtk_window_set_destroy_with_parent" />
			/// </summary>
			DestroyWithParent = 2,
			/// <summary>
			/// Create dialog with actions in header bar instead of action area. Since 3.12.
			/// </summary>
			UseHeaderBar = 4
		}

		/// <summary>
		/// The type of message being displayed in the dialog.
		/// </summary>
		public enum GtkMessageType
		{
			/// <summary>
			/// Informational message
			/// </summary>
			Info,
			/// <summary>
			/// Non-fatal warning message
			/// </summary>
			Warning,
			/// <summary>
			/// Question requiring a choice
			/// </summary>
			Question,
			/// <summary>
			/// Fatal error message
			/// </summary>
			Error,
			/// <summary>
			/// None of the above, doesn't get an icon
			/// </summary>
			Other
		}

		/// <summary>
		/// justification for label and maybe other widgets (text?)
		/// </summary>
		public enum GtkJustification
		{
			Left,
			Right,
			Center,
			Fill
		}

		/// <summary>
		/// Predefined values for use as response ids in gtk_dialog_add_button(). All predefined values are negative, GTK+ leaves positive values for
		/// application-defined response ids.
		/// </summary>
		public enum GtkResponseType
		{
			/// <summary>
			/// Returned if an action widget has no response id, or if the dialog gets programmatically hidden or destroyed
			/// </summary>
			None = -1,
			/// <summary>
			/// Generic response id, not used by GTK+ dialogs
			/// </summary>
			Reject = -2,
			/// <summary>
			/// Generic response id, not used by GTK+ dialogs
			/// </summary>
			Accept = -3,
			/// <summary>
			/// Returned if the dialog is deleted
			/// </summary>
			DeleteEvent = -4,
			/// <summary>
			/// Returned by OK buttons in GTK+ dialogs
			/// </summary>
			OK = -5,
			/// <summary>
			/// Returned by Cancel buttons in GTK+ dialogs
			/// </summary>
			Cancel = -6,
			/// <summary>
			/// Returned by Close buttons in GTK+ dialogs
			/// </summary>
			Close = -7,
			/// <summary>
			/// Returned by Yes buttons in GTK+ dialogs
			/// </summary>
			Yes = -8,
			/// <summary>
			/// Returned by No buttons in GTK+ dialogs
			/// </summary>
			No = -9,
			/// <summary>
			/// Returned by Apply buttons in GTK+ dialogs
			/// </summary>
			Apply = -10,
			/// <summary>
			/// Returned by Help buttons in GTK+ dialogs
			/// </summary>
			Help = -11
		}

		public enum GtkPackType
		{
			Start,
			End
		}

		/// <summary>
		/// Indicates the relief to be drawn around a GtkButton.
		/// </summary>
		public enum GtkReliefStyle
		{
			/// <summary>
			/// Draw a normal relief.
			/// </summary>
			Normal,
			/// <summary>
			/// A half relief.
			/// </summary>
			Half,
			/// <summary>
			/// No relief.
			/// </summary>
			None
		}

		/// <summary>
		/// Describes whether a #GtkFileChooser is being used to open existing files or to save to a possibly new file.
		/// </summary>
		public enum GtkFileChooserAction
		{
			/// <summary>
			/// Indicates open mode.  The file chooser will only let the user pick an existing file.
			/// </summary>
			Open,
			/// <summary>
			/// Indicates save mode.  The file chooser will let the user pick an existing file, or type in a new filename.
			/// </summary>
			Save,
			/// <summary>
			/// Indicates an Open mode for selecting folders.  The file chooser will let the user pick an existing folder.
			/// </summary>
			SelectFolder,
			/// <summary>
			/// Indicates a mode for creating a new folder.  The file chooser will let the user name an existing or new folder.
			/// </summary>
			CreateFolder
		}

		public enum GtkLicense
		{
			Unknown,
			Custom,

			GPL20,
			GPL30,

			LGPL21,
			LGPL30,

			BSD,
			MITX11,

			Artistic
		}

		public enum GtkAlign
		{
			Fill,
			Start,
			End,
			Center,
			Baseline
		}

		public enum GtkPrintOperationResult
		{
			Error,
			Apply,
			Cancel,
			InProgress
		}

		public enum GtkPrintOperationAction
		{
			/// <summary>
			/// Show the print dialog.
			/// </summary>
			PrintDialog,
			/// <summary>
			/// Start to print without showing the print dialog, based on the current print settings.
			/// </summary>
			Print,
			/// <summary>
			/// Show the print preview.
			/// </summary>
			Preview,
			/// <summary>
			/// Export to a file. This requires the export-filename property to be set.
			/// </summary>
			Export
		}

		[Flags()]
		public enum GtkTargetFlags : uint
		{
			/// <summary>
			/// If this is set, the target will only be selected for drags within a single application.
			/// </summary>
			SameApp = 1 << 0,
			/// <summary>
			/// If this is set, the target will only be selected for drags within a single widget.
			/// </summary>
			SameWidget = 1 << 1,
			/// <summary>
			/// If this is set, the target will not be selected for drags within a single application.
			/// </summary>
			OtherApp = 1 << 2,
			/// <summary>
			/// If this is set, the target will not be selected for drags withing a single widget.
			/// </summary>
			OtherWidget = 1 << 3
		}


		/// <summary>
		/// These flags indicate what parts of a <see cref="Structures.GtkFileFilterInfo"/> struct are filled or need to be filled.
		/// </summary>
		[Flags()]
		public enum GtkFileFilterFlags
		{
			/// <summary>
			/// the filename of the file being tested
			/// </summary>
			FileName = 1 << 0,
			/// <summary>
			/// the URI for the file being tested
			/// </summary>
			FilterURI = 1 << 1,
			/// <summary>
			/// the string that will be used to display the file in the file chooser
			/// </summary>
			DisplayName = 1 << 2,
			/// <summary>
			/// the mime type of the file
			/// </summary>
			MIMEType = 1 << 3
		}

		/// <summary>
		/// Specifies the various types of action that will be taken on behalf of the user for a drag destination site.
		/// </summary>
		[Flags()]
		public enum GtkDestDefaults
		{
			/// <summary>
			/// If set for a widget, GTK+, during a drag over this widget will check if the drag matches this widget’s list of possible targets and actions. GTK+ will then call gdk_drag_status() as appropriate.
			/// </summary>
			Motion = 1 << 0,
			/// <summary>
			/// If set for a widget, GTK+ will draw a highlight on this widget as long as a drag is over this widget and the widget drag format and action are acceptable.
			/// </summary>
			Highlight = 1 << 1,
			/// <summary>
			/// If set for a widget, when a drop occurs, GTK+ will will check if the drag matches this widget’s list of possible targets and actions. If so, GTK+ will call gtk_drag_get_data() on behalf of the widget. Whether or not the drop is successful, GTK+ will call gtk_drag_finish(). If the action was a move, then if the drag was successful, then TRUE will be passed for the delete parameter to gtk_drag_finish().
			/// </summary>
			Drop = 1 << 2,
			/// <summary>
			/// If set, specifies that all default actions should be taken.
			/// </summary>
			All = Motion | Highlight | Drop
		}

		/// <summary>
		/// Used to control what selections users are allowed to make.
		/// </summary>
		public enum GtkSelectionMode
		{
			/// <summary>
			/// No selection is possible.
			/// </summary>
			None,
			/// <summary>
			/// Zero or one element may be selected.
			/// </summary>
			Single,
			/// <summary>
			/// Exactly one element is selected. In some circumstances, such as initially or during a search operation, it’s possible for no element to be selected with GTK_SELECTION_BROWSE. What is really enforced is that the user can’t deselect a currently selected element except by selecting another element.
			/// </summary>
			Browse,
			/// <summary>
			/// Any number of elements may be selected. The Ctrl key may be used to enlarge the selection, and Shift key to select between the focus and the child pointed to. Some widgets may also allow Click-drag to select a range of elements.
			/// </summary>
			Multiple
		}
		/// <summary>
		/// Window placement can be influenced using this enumeration. 
		/// </summary>
		public enum GtkWindowPosition
		{
			/// <summary>
			/// No influence is made on placement.
			/// </summary>
			None,
			/// <summary>
			/// Windows should be placed in the center of the screen.
			/// </summary>
			Center,
			/// <summary>
			/// Windows should be placed at the current mouse position.
			/// </summary>
			Mouse,
			/// <summary>
			/// Keep window centered as it changes size, etc.
			/// </summary>
			/// <remarks>Note that using CenterAlways is almost always a bad idea. It won’t necessarily work well with all window managers or on all windowing systems.</remarks>
			CenterAlways,
			/// <summary>
			/// Center the window on its transient parent (see <see cref="Methods.gtk_window_set_transient_for" />).
			/// </summary>
			CenterOnParent
		}

		public enum GtkAttachOptions
		{
			Expand = 1 << 0,
			Shrink = 1 << 1,
			Fill = 1 << 2
		}
		public enum GtkStyleProviderPriority
		{
			Fallback = 1,
			Theme = 200,
			Settings = 400,
			Application = 600,
			User = 800
		}
	}
}

