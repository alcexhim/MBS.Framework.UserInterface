//
//  SettingValue.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2020 Mike Becker's Software
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

namespace MBS.Framework.UserInterface
{
	public class SettingsValue
	{
		public class SettingsValueCollection : System.Collections.ObjectModel.Collection<SettingsValue>
		{
			Dictionary<Guid, SettingsValue> _itemsByGuid = new Dictionary<Guid, SettingsValue>();

			public int Count { get { return _itemsByGuid.Count; } }

			public bool Contains(Guid scopeId)
			{
				return _itemsByGuid.ContainsKey(scopeId);
			}

			public void Add(Guid scopeId, object value)
			{
				SettingsValue item = new SettingsValue();
				item.ScopeId = scopeId;
				item.Value = value;
				Add(item);
			}
			public SettingsValue this[Guid scopeId]
			{
				get
				{
					if (_itemsByGuid.ContainsKey(scopeId))
						return _itemsByGuid[scopeId];
					return null;
				}
			}
		}

		public Guid ScopeId { get; set; }
		public object Value { get; set; } = null;

	}
}
