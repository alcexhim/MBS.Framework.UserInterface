using System;
using MBS.Framework.Drawing;
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

			void ClearStyleDefinitions();
			void AddStyleDefinition(TextBoxStyleDefinition item);
			void RemoveStyleDefinition(TextBoxStyleDefinition item);

			void ClearStyleAreas();
			void AddStyleArea(TextBoxStyleArea item);
			void RemoveStyleArea(TextBoxStyleArea item);

			int GetCharIndexFromPosition(Vector2D pt);
			int GetFirstCharIndexOfCurrentLine();
			int GetFirstCharIndexFromLine(int lineIndex);
			Rectangle GetPositionFromCharIndex(int charIndex);
		}
	}
	public class TextBoxSearchResult
	{
		public int Start { get; }
		public int Length { get; }
		public string Value { get; }

		public TextBoxSearchResult(string value, int start, int length)
		{
			Value = value;
			Start = start;
			Length = length;
		}
	}
	public class TextBoxStyleDefinition
	{
		public class TextBoxStyleDefinitionCollection
			: System.Collections.ObjectModel.Collection<TextBoxStyleDefinition>
		{
			private TextBox _Parent = null;
			public TextBoxStyleDefinitionCollection(TextBox parent)
			{
				_Parent = parent;
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				(_Parent.ControlImplementation as Native.ITextBoxImplementation).ClearStyleDefinitions();
			}
			protected override void InsertItem(int index, TextBoxStyleDefinition item)
			{
				base.InsertItem(index, item);
				(_Parent.ControlImplementation as Native.ITextBoxImplementation).AddStyleDefinition(item);
			}
			protected override void RemoveItem(int index)
			{
				(_Parent.ControlImplementation as Native.ITextBoxImplementation).RemoveStyleDefinition(this[index]);
				base.RemoveItem(index);
			}
		}

		public string Name { get; } = null;
		public Color BackgroundColor { get; set; } = Color.Empty;
		public Color ForegroundColor { get; set; } = Color.Empty;
		public bool Editable { get; set; } = true;

		public TextBoxStyleDefinition(string name)
		{
			Name = name;
		}
	}
	public class TextBoxStyleArea
	{
		public class TextBoxStyleAreaCollection
			: System.Collections.ObjectModel.Collection<TextBoxStyleArea>
		{
			private TextBox _Parent = null;
			public TextBoxStyleAreaCollection(TextBox parent)
			{
				_Parent = parent;
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				(_Parent.ControlImplementation as Native.ITextBoxImplementation).ClearStyleAreas();
			}
			protected override void InsertItem(int index, TextBoxStyleArea item)
			{
				base.InsertItem(index, item);
				(_Parent.ControlImplementation as Native.ITextBoxImplementation).AddStyleArea(item);
			}
			protected override void RemoveItem(int index)
			{
				(_Parent.ControlImplementation as Native.ITextBoxImplementation).RemoveStyleArea(this[index]);
				base.RemoveItem(index);
			}
		}

		public int Start { get; set; } = 0;
		public int Length { get; set; } = 0;
		public TextBoxStyleDefinition Style { get; set; } = null;

		public TextBoxStyleArea(TextBoxStyleDefinition definition, int start, int length)
		{
			Style = definition;
			Start = start;
			Length = length;
		}
	}
	public enum TextBoxUsageHint
	{
		None,
		Search
	}
	public class TextBox : SystemControl
	{
		public TextBox()
		{
			StyleDefinitions = new TextBoxStyleDefinition.TextBoxStyleDefinitionCollection(this);
			StyleAreas = new TextBoxStyleArea.TextBoxStyleAreaCollection(this);
		}
		public TextBoxStyleDefinition.TextBoxStyleDefinitionCollection StyleDefinitions { get; }
		public TextBoxStyleArea.TextBoxStyleAreaCollection StyleAreas { get; }

		public TextBoxUsageHint UsageHint { get; set; } = TextBoxUsageHint.None;

		public TextBoxSearchResult[] FindAll(string value)
		{
			System.Collections.Generic.List<TextBoxSearchResult> results = new System.Collections.Generic.List<TextBoxSearchResult>();
			for (int i = 0; (i + value.Length) < Text.Length; i++)
			{
				if (Text.Substring(i, value.Length) == value)
				{
					results.Add(new TextBoxSearchResult(value, i, value.Length));
				}
			}
			return results.ToArray();
		}

		public string LastCompleteWord
		{
			get
			{
				if (Words.Length == 1)
				{
					return Words[0];
				}
				else if (Words.Length > 1)
				{
					return Words[Words.Length - 1];
				}
				return Text;
			}
		}

		public string[] Words
		{
			get
			{
				return Text.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			}
		}
		public string CurrentWord
		{
			get
			{
				int indexOfSpace = Text.LastIndexOf(' ');
				if (indexOfSpace == -1)
					return Text;
				return Text.Substring(indexOfSpace + 1);
			}
		}

		/// <summary>
		/// Retrieves the index of the character nearest to the specified location.
		/// </summary>
		/// <returns>The zero-based character index at the specified location.</returns>
		/// <param name="pt">The location to search.</param>
		public int GetCharIndexFromPosition(Vector2D pt)
		{
			return ((this.ControlImplementation as Native.ITextBoxImplementation)?.GetCharIndexFromPosition(pt)).GetValueOrDefault(-1);
		}


		public Rectangle GetPositionFromCharIndex(int charIndex)
		{
			return ((this.ControlImplementation as Native.ITextBoxImplementation)?.GetPositionFromCharIndex(charIndex)).GetValueOrDefault(Rectangle.Empty);
		}

		/// <summary>
		/// Retrieves the index of the first character of the current line.
		/// </summary>
		/// <returns>The zero-based character index in the current line.</returns>
		public int GetFirstCharIndexOfCurrentLine()
		{
			return ((this.ControlImplementation as Native.ITextBoxImplementation)?.GetFirstCharIndexOfCurrentLine()).GetValueOrDefault(-1);
		}

		/// <summary>
		/// Retrieves the index of the first character of the specified line.
		/// </summary>
		/// <returns>The zero-based character index in the specified line.</returns>
		public int GetFirstCharIndexFromLine(int index)
		{
			return ((this.ControlImplementation as Native.ITextBoxImplementation)?.GetFirstCharIndexFromLine(index)).GetValueOrDefault(-1);
		}

		public string GetNewLineString()
		{
			return System.Environment.NewLine;
		}

		public string[] Lines
		{
			get
			{
				string[] lines = Text.Split(new string[] { GetNewLineString() }, StringSplitOptions.None);
				return lines;
			}
		}
		public string CurrentLine
		{
			get { return Lines[CurrentLineIndex]; }
		}

		public int CurrentLineIndex
		{
			get
			{
				if (Multiline)
				{
					string[] lines = Text.Split(new string[] { GetNewLineString() }, StringSplitOptions.None);
					int selstart = 0;
					for (int i = 0; i < lines.Length; i++)
					{
						selstart += lines[i].Length;
						if (selstart >= SelectionStart)
						{
							return i;
						}
					}
				}
				return 0;
			}
		}

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
		public bool Multiline { get { return mvarMultiline; } set { mvarMultiline = value; } }

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
				Changed(this, e);
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
