//
//  CommandBinding.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2021 Mike Becker's Software
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
using System.Collections.Generic;
using System.Text;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;
using UniversalEditor.ObjectModels.Markup;

namespace MBS.Framework.UserInterface
{
	public class CommandBinding
	{
		public class CommandBindingCollection
			: System.Collections.ObjectModel.Collection<CommandBinding>
		{
			private struct COMMAND_BINDING_KEY
			{
				public KeyboardKey Key;
				public KeyboardModifierKey ModifierKeys;
				public MouseButtons MouseButtons;
				public Guid? ContextID;

				public COMMAND_BINDING_KEY(KeyboardKey key, KeyboardModifierKey modifierKeys = KeyboardModifierKey.None, Guid? contextID = null)
				{
					Key = key;
					ModifierKeys = modifierKeys;
					MouseButtons = MouseButtons.None;
					ContextID = contextID;
				}
				public COMMAND_BINDING_KEY(MouseButtons buttons, KeyboardModifierKey modifierKeys = KeyboardModifierKey.None, Guid? contextID = null)
				{
					Key = KeyboardKey.None;
					ModifierKeys = modifierKeys;
					MouseButtons = buttons;
					ContextID = contextID;
				}
			}

			public CommandBinding[] this[string commandID]
			{
				get
				{
					if (!_ItemsByCommand.ContainsKey(commandID))
					{
						return new CommandBinding[0];
					}
					return _ItemsByCommand[commandID].ToArray();
				}
			}

			private Dictionary<string, List<CommandBinding>> _ItemsByCommand = new Dictionary<string, List<CommandBinding>>();

			private Dictionary<COMMAND_BINDING_KEY, CommandBinding> _ItemsByKey = new Dictionary<COMMAND_BINDING_KEY, CommandBinding>();
			public CommandBinding this[KeyboardKey key, KeyboardModifierKey modifierKeys = KeyboardModifierKey.None, Guid? contextID = null]
			{
				get
				{
					COMMAND_BINDING_KEY _key = new COMMAND_BINDING_KEY(key, modifierKeys, contextID);
					if (_ItemsByKey.ContainsKey(_key))
					{
						return _ItemsByKey[_key];
					}
					return null;
				}
			}
			public CommandBinding this[MouseButtons buttons, KeyboardModifierKey modifierKeys = KeyboardModifierKey.None, Guid? contextID = null]
			{
				get
				{
					COMMAND_BINDING_KEY _key = new COMMAND_BINDING_KEY(buttons, modifierKeys, contextID);
					if (_ItemsByKey.ContainsKey(_key))
					{
						return _ItemsByKey[_key];
					}
					return null;
				}
			}

			public bool Remove(KeyboardKey key, KeyboardModifierKey modifierKeys = KeyboardModifierKey.None, Guid? contextID = null)
			{
				COMMAND_BINDING_KEY _key = new COMMAND_BINDING_KEY(key, modifierKeys, contextID);
				if (_ItemsByKey.ContainsKey(_key))
				{
					if (_ItemsByCommand.ContainsKey(_ItemsByKey[_key].CommandID))
					{
						List<CommandBinding> list = new List<CommandBinding>();
						foreach (CommandBinding binding in _ItemsByCommand[_ItemsByKey[_key].CommandID])
						{
							if (binding.Key == key && binding.ModifierKeys == modifierKeys && binding.ContextID == contextID)
							{
								// remove the item from the list - do nothing
							}
							else
							{
								list.Add(binding);
							}
						}
						_ItemsByCommand[_ItemsByKey[_key].CommandID] = list;
					}

					_ItemsByCommand.Remove(_ItemsByKey[_key].CommandID);
					Remove(_ItemsByKey[_key]);
					return true;
				}
				return false;
			}
			public bool Remove(MouseButtons buttons, KeyboardModifierKey modifierKeys = KeyboardModifierKey.None, Guid? contextID = null)
			{
				COMMAND_BINDING_KEY _key = new COMMAND_BINDING_KEY(buttons, modifierKeys, contextID);
				if (_ItemsByKey.ContainsKey(_key))
				{
					Remove(_ItemsByKey[_key]);
					return true;
				}
				return false;
			}

			public CommandBinding Add(string commandID, MouseButtons buttons, KeyboardModifierKey modifierKeys, Guid? contextID = null)
			{
				CommandBinding cb = new CommandBinding(commandID, buttons, modifierKeys, contextID);
				Add(cb);
				return cb;
			}
			public CommandBinding Add(string commandID, KeyboardKey key, KeyboardModifierKey modifierKeys, Guid? contextID = null)
			{
				CommandBinding cb = new CommandBinding(commandID, key, modifierKeys, contextID);
				Add(cb);
				return cb;
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				_ItemsByKey.Clear();
			}
			protected override void InsertItem(int index, CommandBinding item)
			{
				base.InsertItem(index, item);
				if (item.MouseButtons == MouseButtons.None)
				{
					_ItemsByKey[new COMMAND_BINDING_KEY(item.Key, item.ModifierKeys, item.ContextID)] = item;
				}
				else if (item.Key == KeyboardKey.None)
				{
					_ItemsByKey[new COMMAND_BINDING_KEY(item.MouseButtons, item.ModifierKeys, item.ContextID)] = item;
				}

				if (!_ItemsByCommand.ContainsKey(item.CommandID))
				{
					_ItemsByCommand[item.CommandID] = new List<CommandBinding>();
				}
				_ItemsByCommand[item.CommandID].Add(item);
			}
			protected override void RemoveItem(int index)
			{
				CommandBinding item = this[index];
				if (item.MouseButtons == MouseButtons.None)
				{
					COMMAND_BINDING_KEY _key = new COMMAND_BINDING_KEY(item.Key, item.ModifierKeys, item.ContextID);
					_ItemsByKey.Remove(_key);
				}
				else if (item.Key == KeyboardKey.None)
				{
					COMMAND_BINDING_KEY _key = new COMMAND_BINDING_KEY(item.MouseButtons, item.ModifierKeys, item.ContextID);
					_ItemsByKey.Remove(_key);
				}

				if (_ItemsByCommand.ContainsKey(item.CommandID))
				{
					_ItemsByCommand[item.CommandID].Remove(item);
				}
				base.RemoveItem(index);
			}
		}

		public CommandBinding(string commandID, MouseButtons buttons, KeyboardModifierKey modifierKeys = KeyboardModifierKey.None, Guid? contextID = null)
		{
			CommandID = commandID;
			MouseButtons = buttons;
			ModifierKeys = modifierKeys;
			ContextID = contextID;
		}
		public CommandBinding(string commandID, KeyboardKey key, KeyboardModifierKey modifierKeys = KeyboardModifierKey.None, Guid? contextID = null)
		{
			CommandID = commandID;
			Key = key;
			ModifierKeys = modifierKeys;
			ContextID = contextID;
		}

		public string CommandID { get; set; } = null;
		public MouseButtons MouseButtons { get; set; } = MouseButtons.None;
		public KeyboardModifierKey ModifierKeys { get; set; } = KeyboardModifierKey.None;
		public KeyboardKey Key { get; set; } = KeyboardKey.None;
		public Guid? ContextID { get; set; } = null;

		public bool Match(KeyEventArgs e)
		{
			return e.Key == Key && e.ModifierKeys == ModifierKeys;
		}
		public bool Match(MouseEventArgs e)
		{
			return e.Buttons == MouseButtons && e.ModifierKeys == ModifierKeys;
		}

		public static CommandBinding FromXML(MarkupTagElement tag)
		{
			if (tag == null) return null;
			if (tag.FullName != "CommandBinding") return null;

			MarkupAttribute attCommandID = tag.Attributes["CommandID"];
			MarkupAttribute attContextID = tag.Attributes["ContextID"];
			MarkupAttribute attKey = tag.Attributes["Key"];
			MarkupAttribute attModifierKeys = tag.Attributes["ModifierKeys"];
			MarkupAttribute attMouseButtons = tag.Attributes["MouseButtons"];

			MouseButtons mb = MouseButtons.None;
			KeyboardModifierKey kmk = KeyboardModifierKey.None;
			KeyboardKey key = KeyboardKey.None;

			Guid? ctxid = null;
			if (attContextID != null)
			{
				ctxid = new Guid(attContextID.Value);
			}

			if (attModifierKeys != null)
			{
				kmk = (KeyboardModifierKey)Enum.Parse(typeof(KeyboardModifierKey), attModifierKeys.Value);
			}
			if (attMouseButtons != null)
			{
				mb = (MouseButtons)Enum.Parse(typeof(MouseButtons), attMouseButtons.Value);
				return new CommandBinding(attCommandID.Value, mb, kmk, ctxid);
			}
			else if (attKey != null)
			{
				key = (KeyboardKey)Enum.Parse(typeof(KeyboardKey), attKey.Value);
				return new CommandBinding(attCommandID.Value, key, kmk, ctxid);
			}
			return null;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (Key == KeyboardKey.None && MouseButtons == MouseButtons.None)
			{
				sb.Append(base.ToString());
			}
			else if (Key == KeyboardKey.None)
			{
				sb.Append("[mouse] ");
				sb.Append(CommandBinding.GetString(MouseButtons, ModifierKeys));
			}
			else if (MouseButtons == MouseButtons.None)
			{
				sb.Append("[keyboard] ");
				sb.Append(CommandBinding.GetString(Key, ModifierKeys));
			}
			return sb.ToString();
		}

		public static string GetModifierString(KeyboardModifierKey modifierKeys)
		{
			return modifierKeys.ToString().Replace(", ", " + ").Replace("Control", "Ctrl");
		}
		public static string GetString(MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			StringBuilder sb = new StringBuilder();
			if (modifierKeys != KeyboardModifierKey.None)
			{
				sb.Append(GetModifierString(modifierKeys));
				sb.Append(" + ");
			}
			sb.Append(buttons);
			return sb.ToString();
		}
		public static string GetString(KeyboardKey key, KeyboardModifierKey modifierKeys)
		{
			StringBuilder sb = new StringBuilder();
			if (modifierKeys != KeyboardModifierKey.None)
			{
				sb.Append(GetModifierString(modifierKeys));
				sb.Append(" + ");
			}
			sb.Append(key.ToString().Replace("LControlKey", "Left Ctrl").Replace("LMenu", "Left Alt").Replace("LShiftKey", "Left Shift").Replace("RControlKey", "Right Ctrl").Replace("RMenu", "Right Alt").Replace("RShiftKey", "Right Shift"));
			return sb.ToString();
		}
	}
}
