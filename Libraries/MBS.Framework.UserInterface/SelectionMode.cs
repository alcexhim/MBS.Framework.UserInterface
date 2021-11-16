//
//  SelectionMode.cs - specify which types of selection a user is allowed to make
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019-2021 Mike Becker's Software
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
	public enum SelectionMode
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
}
