//
//  SyntaxRuleSets.cs
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
	public static class SyntaxLanguages
	{
		public static SyntaxLanguage CSharp { get; private set; } = null;
		public static SyntaxLanguage SQL { get; private set; } = null;

		static SyntaxLanguages()
		{
			CSharp = new SyntaxLanguage(new SyntaxColorRule[]
			{
				new SyntaxColorRule("//", null, null)
				{
					Type = SyntaxColorRuleTypes.Comment
				},
				new SyntaxColorRule(null, "internal", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "null", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "public", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "private", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "static", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				}
			}, new SyntaxKeyword[]
			{
				new SyntaxKeyword("int", SyntaxKeywordTypes.Primitive),
				new SyntaxKeyword("internal", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("null", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("public", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("private", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("static", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("void", SyntaxKeywordTypes.Primitive)
			})
			{ IsCaseSensitive = true };

			SQL = new SyntaxLanguage(new SyntaxColorRule[]
			{
				new SyntaxColorRule("--", null, null)
				{
					Type = SyntaxColorRuleTypes.Comment
				},
				new SyntaxColorRule(null, "SELECT", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "FROM", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "WHERE", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "ORDER", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "GROUP", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "BY", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "AND", null)
				{
					Type = SyntaxColorRuleTypes.Boolean
				},
				new SyntaxColorRule(null, "OR", null)
				{
					Type = SyntaxColorRuleTypes.Boolean
				},
				new SyntaxColorRule(null, "NOT", null)
				{
					Type = SyntaxColorRuleTypes.Boolean
				},
				new SyntaxColorRule(null, "END", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "DO", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "IF", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				},
				new SyntaxColorRule(null, "NULL", null)
				{
					Type = SyntaxColorRuleTypes.Keyword
				}
			}, new SyntaxKeyword[]
			{
				new SyntaxKeyword("SELECT", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("FROM", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("WHERE", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("ORDER", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("GROUP", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("BY", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("AND", SyntaxKeywordTypes.Boolean),
				new SyntaxKeyword("OR", SyntaxKeywordTypes.Boolean),
				new SyntaxKeyword("NOT", SyntaxKeywordTypes.Boolean),
				new SyntaxKeyword("NULL", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("END", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("DO", SyntaxKeywordTypes.Keyword),
				new SyntaxKeyword("IF", SyntaxKeywordTypes.Keyword)
			})
			{ IsCaseSensitive = false };
		}
	}
}
