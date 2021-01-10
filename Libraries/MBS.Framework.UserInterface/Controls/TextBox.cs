using System;
using MBS.Framework.UserInterface.Controls.Native;
using MBS.Framework.UserInterface.Input.Keyboard;

namespace MBS.Framework.UserInterface.Controls
{
	namespace Native
	{
		public interface ITextBoxImplementation
		{
			void InsertText(string content);

			int GetSelectionStart();
			void SetSelectionStart(int pos);
			int GetSelectionLength();
			void SetSelectionLength(int len);

			string GetSelectedText();
			void SetSelectedText(string text);

			bool IsEditable();
			void SetEditable(bool value);

			HorizontalAlignment GetTextAlignment();
			void SetTextAlignment(HorizontalAlignment value);
		}
	}
	public class TextBox : SystemControl
	{
		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="Control.ContextMenu" /> for this <see cref="TextBox" /> should be merged with the system native context menu.
		/// </summary>
		/// <value><c>true</c> if the ContextMenu for this TextBox should be merged ; otherwise, <c>false</c>.</value>
		public bool MergeContextMenu { get; set; } = true;

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

		public DefaultTreeModel CompletionModel { get; } = new DefaultTreeModel(new Type[] { typeof(string) });

		private HorizontalAlignment _TextAlignment = HorizontalAlignment.Default;
		public HorizontalAlignment TextAlignment
		{
			get
			{
				ITextBoxImplementation impl = (ControlImplementation as ITextBoxImplementation);
				if (impl != null && IsCreated)
				{
					return impl.GetTextAlignment();
				}
				return _TextAlignment;
			}
			set
			{
				_TextAlignment = value;
				(ControlImplementation as ITextBoxImplementation)?.SetTextAlignment(value);
			}
		}

		private bool mvarUseSystemPasswordChar = false;

		public void Insert(string content)
		{
			(ControlImplementation as Native.ITextBoxImplementation).InsertText(content);
		}

		public bool UseSystemPasswordChar { get { return mvarUseSystemPasswordChar; } set { mvarUseSystemPasswordChar = value; } }

		private bool mvarEditable = true;
		/// <summary>
		/// Determines if text in this <see cref="TextBox" /> may be edited.
		/// </summary>
		/// <value><c>true</c> if text may be edited; otherwise, <c>false</c>.</value>
		public bool Editable
		{
			get
			{
				if (!IsCreated)
					return mvarEditable;

				ITextBoxImplementation impl = (ControlImplementation as ITextBoxImplementation);
				if (impl != null)
					mvarEditable = impl.IsEditable();

				return mvarEditable;
			}
			set
			{
				if (IsCreated)
					(ControlImplementation as ITextBoxImplementation)?.SetEditable(value);

				mvarEditable = value;
			}
		}

		/// <summary>
		/// Determines whether this <see cref="TextBox"/> has been changed by the user.
		/// </summary>
		/// <value><c>true</c> if is changed by user; otherwise, <c>false</c>.</value>
		public bool IsChangedByUser { get; private set; }

		public void ResetChangedByUser()
		{
			IsChangedByUser = false;
		}

		protected internal override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Cancel) return;
			
			this.IsChangedByUser = true;
		}

		public event EventHandler Changed;
		protected internal virtual void OnChanged(EventArgs e)
		{
			if (Changed != null)
				Changed (this, e);
		}

		private string mvarSelectedText = null;
		public string SelectedText
		{
			get
			{
				ITextBoxImplementation impl = (ControlImplementation as ITextBoxImplementation);
				if (impl != null)
					mvarSelectedText = impl.GetSelectedText();

				return mvarSelectedText;
			}
			set
			{
				(ControlImplementation as ITextBoxImplementation)?.SetSelectedText(value);
			}
		}

		private int mvarSelectionStart = 0;
		public int SelectionStart
		{
			get
			{
				ITextBoxImplementation impl = (ControlImplementation as ITextBoxImplementation);
				if (impl != null)
					mvarSelectionStart = impl.GetSelectionStart();

				return mvarSelectionStart;
			}
			set
			{
				(ControlImplementation as ITextBoxImplementation)?.SetSelectionStart(value);
			}
		}
		private int mvarSelectionLength = 0;
		public int SelectionLength
		{
			get
			{
				ITextBoxImplementation impl = (ControlImplementation as ITextBoxImplementation);
				if (impl != null)
					mvarSelectionLength = impl.GetSelectionLength();

				return mvarSelectionLength;
			}
			set
			{
				(ControlImplementation as ITextBoxImplementation)?.SetSelectionLength(value);
			}
		}
	}
}

