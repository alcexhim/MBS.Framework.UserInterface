//
//  KeyEvent.cs
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
using System.ComponentModel;

namespace UniversalWidgetToolkit.Input.Keyboard
{
	public delegate void KeyEventHandler(object sender, KeyEventArgs e);
	public class KeyEventArgs : CancelEventArgs
	{
		private const ulong KEYS_MODIFIER_MASK = 0xFFFFFFFFFFFF0000;

		public KeyboardKey Key { get; set; }
		public KeyboardModifierKey ModifierKeys { get; set; }

		public KeyboardKey KeyData { get { return (KeyboardKey)((uint)Key & KEYS_MODIFIER_MASK); } }

		public int KeyCode { get; set; }
		public int HardwareKeyCode { get; set; }
	}
}
