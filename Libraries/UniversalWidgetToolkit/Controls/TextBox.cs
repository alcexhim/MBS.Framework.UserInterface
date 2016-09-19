using System;

namespace UniversalWidgetToolkit
{
	public class TextBox : Control
	{
		private int mvarMaxLength = -1;
		public int MaxLength { get { return mvarMaxLength; } set { mvarMaxLength = value; } }

		private int mvarWidthChars = -1;
		public int WidthChars { get { return mvarWidthChars; } set { mvarWidthChars = value; } }

		private bool mvarUseSystemPasswordChar = false;
		public bool UseSystemPasswordChar { get { return mvarUseSystemPasswordChar; } set { mvarUseSystemPasswordChar = value; } }

		private bool mvarEditable = true;
		/// <summary>
		/// Determines if text in this <see cref="TextBox" /> may be edited.
		/// </summary>
		/// <value><c>true</c> if text may be edited; otherwise, <c>false</c>.</value>
		public bool Editable { get { return mvarEditable; } set { mvarEditable = value; } }

		public event EventHandler Changed;
		public virtual void OnChanged(EventArgs e)
		{
			if (Changed != null)
				Changed (this, e);
		}
	}
}

