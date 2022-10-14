//
//  SyntaxColors.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2022 Mike Becker's Software
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
namespace MBS.Framework.UserInterface.Controls.SyntaxTextBox
{
	public static class SyntaxColorRuleTypes
	{
		public static SyntaxColorRuleType Comment { get; } = new SyntaxColorRuleType(new Guid("{592338b1-0d02-45de-8966-a4eae55497e5}"));
		public static SyntaxColorRuleType Keyword { get; } = new SyntaxColorRuleType(new Guid("{b30c9c70-2230-4683-88f7-ea981c38df8f}"));
		public static SyntaxColorRuleType Boolean { get; } = new SyntaxColorRuleType(new Guid("{3e5a1f3f-27c9-448d-8b0a-d38e61fec4fb}"));
	}
}
