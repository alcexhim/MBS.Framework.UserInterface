using System;

using UniversalWidgetToolkit.Input.Keyboard;

namespace UniversalWidgetToolkit
{
	public class Shortcut
	{
		private KeyboardKey mvarKey = KeyboardKey.None;
		public KeyboardKey Key { get { return mvarKey; } set { mvarKey = value; } }

		private KeyboardModifierKey mvarModifierKeys = KeyboardModifierKey.None;
		public KeyboardModifierKey ModifierKeys { get { return mvarModifierKeys; } set { mvarModifierKeys = value; } }

		public Shortcut (KeyboardKey key, KeyboardModifierKey modifierKeys)
		{
			mvarKey = key;
			mvarModifierKeys = modifierKeys;
		}
	}
}

