//
//  SyntaxRule.cs
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
	public class SyntaxColorRule
	{
		public class SyntaxColorRuleCollection
			: System.Collections.ObjectModel.Collection<SyntaxColorRule>
		{

		}

		public string Prefix { get; set; } = null;
		public string Content { get; set; } = null;
		public string Suffix { get; set; } = null;

		public SyntaxColorRuleType Type { get; set; }

		public SyntaxColorRule(string prefix, string content, string suffix)
		{
			Prefix = prefix;
			Content = content;
			Suffix = suffix;
		}
	}
}
