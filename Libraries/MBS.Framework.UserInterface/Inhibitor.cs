//
//  Inhibitor.cs
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

namespace MBS.Framework.UserInterface
{
	public class Inhibitor
	{
		public class InhibitorCollection
			: System.Collections.ObjectModel.Collection<Inhibitor>
		{
			private UIApplication _parent = null;
			internal InhibitorCollection(UIApplication parent)
			{
				_parent = parent;
			}

			protected override void InsertItem(int index, Inhibitor item)
			{
				base.InsertItem(index, item);
				_parent.Engine.RegisterInhibitor(item);
			}
			protected override void RemoveItem(int index)
			{
				_parent.Engine.UnregisterInhibitor(this[index]);
				base.RemoveItem(index);
			}
			protected override void ClearItems()
			{
				for (int i = 0; i < Count; i++)
				{
					_parent.Engine.UnregisterInhibitor(this[i]);
				}
				base.ClearItems();
			}
		}

		public InhibitorType Type { get; } = InhibitorType.None;
		public string Message { get; } = null;
		public Window ParentWindow { get; } = null;

		public Inhibitor(InhibitorType type, string message, Window parentWindow = null)
		{
			Type = type;
			Message = message;
			ParentWindow = parentWindow;
		}
	}
}
