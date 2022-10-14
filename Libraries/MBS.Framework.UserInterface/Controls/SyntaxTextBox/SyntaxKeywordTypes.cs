//
//  EmptyClass.cs
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
	public static class SyntaxKeywordTypes
	{
		public static SyntaxKeywordType Primitive { get; } = new SyntaxKeywordType(new Guid("{e5493708-2d81-437c-98d8-40d62f6f68df}"));
		public static SyntaxKeywordType Keyword { get; } = new SyntaxKeywordType(new Guid("{ef764a43-44eb-478e-9355-29282513aaa9}")) { TextBoxStyle = SyntaxTextBoxControl.styleKeyword };
		public static SyntaxKeywordType Snippet { get; } = new SyntaxKeywordType(new Guid("{e3983de0-67a9-4716-b793-898c4c500475}"));
		public static SyntaxKeywordType Boolean { get; } = new SyntaxKeywordType(new Guid("{daf06356-0b5c-409d-84d6-9fca44e438f4}")) { TextBoxStyle = SyntaxTextBoxControl.styleBoolean };
	}
}
