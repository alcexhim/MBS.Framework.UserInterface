using System;

using UniversalWidgetToolkit.Input.Keyboard;

namespace UniversalWidgetToolkit.Controls
{
	public class TextBox : SystemControl
	{
		private int mvarMaxLength = -1;
		public int MaxLength { get { return mvarMaxLength; } set { mvarMaxLength = value; } }

		private int mvarWidthChars = -1;
		public int WidthChars { get { return mvarWidthChars; } set { mvarWidthChars = value; } }

		private bool mvarMultiline = false;
		/// <summary>
		/// Determines whether this <see cref="TextBox"/> supports multi-line editing.
		/// </summary>
		/// <value><c>true</c> if multiline; otherwise, <c>false</c>.</value>
		public bool Multiline {  get { return mvarMultiline;  } set { mvarMultiline = value; } }

		private HorizontalAlignment mvarHorizontalAlignment = HorizontalAlignment.Default;
		public HorizontalAlignment HorizontalAlignment {  get { return mvarHorizontalAlignment;  } set { mvarHorizontalAlignment = value; } }

		private VerticalAlignment mvarVerticalAlignment = VerticalAlignment.Default;
		public VerticalAlignment VerticalAlignment {  get { return mvarVerticalAlignment;  } set { mvarVerticalAlignment = value; } }

		private bool mvarUseSystemPasswordChar = false;
		public bool UseSystemPasswordChar { get { return mvarUseSystemPasswordChar; } set { mvarUseSystemPasswordChar = value; } }

		private bool mvarEditable = true;
		/// <summary>
		/// Determines if text in this <see cref="TextBox" /> may be edited.
		/// </summary>
		/// <value><c>true</c> if text may be edited; otherwise, <c>false</c>.</value>
		public bool Editable { get { return mvarEditable; } set { mvarEditable = value; } }

		/// <summary>
		/// Determines whether this <see cref="TextBox"/> has been changed by the user.
		/// </summary>
		/// <value><c>true</c> if is changed by user; otherwise, <c>false</c>.</value>
		public bool IsChangedByUser { get; private set; }

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			this.IsChangedByUser = true;
		}

		public event EventHandler Changed;
		protected virtual void OnChanged(EventArgs e)
		{
			if (Changed != null)
				Changed (this, e);
		}
	}
}

