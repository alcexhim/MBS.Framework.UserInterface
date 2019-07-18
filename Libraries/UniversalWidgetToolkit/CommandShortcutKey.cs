using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Input.Keyboard;

namespace UniversalWidgetToolkit
{
	public class CommandShortcutKey
	{
		private KeyboardModifierKey mvarModifiers = KeyboardModifierKey.None;
		public KeyboardModifierKey Modifiers { get { return mvarModifiers; } set { mvarModifiers = value; } }

		private KeyboardKey mvarValue = KeyboardKey.None;
		public KeyboardKey Value { get { return mvarValue; } set { mvarValue = value; } }

		public bool CompareTo(KeyboardKey keyData)
		{
			// first look at modifier keys
			// if (!(((value.Modifiers & CommandShortcutKeyModifiers.Alt) == CommandShortcutKeyModifiers.Alt)
			//	&& ((keyData & KeyboardModifierKey.Alt) == KeyboardModifierKey.Alt))) return false;

			return true;
		}

		public CommandShortcutKey()
			: this(KeyboardKey.None, KeyboardModifierKey.None)
		{
		}
		public CommandShortcutKey(KeyboardKey value, KeyboardModifierKey modifiers = KeyboardModifierKey.None)
		{
			mvarValue = value;
			mvarModifiers = modifiers;
		}
	}
}
