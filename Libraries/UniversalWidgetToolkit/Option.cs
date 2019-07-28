//
//  Option.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
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

namespace UniversalWidgetToolkit
{
	public class RangeOption : Option<double>
	{
		public double MinimumValue { get; set; } = 0.0;
		public double MaximumValue { get; set; } = 0.0;

		public RangeOption(string title, double defaultValue = 0.0, double minimumValue = 0.0, double maximumValue = 0.0) : base(title, defaultValue) {
			MinimumValue = minimumValue;
			MaximumValue = maximumValue;
		}
	}
	public class GroupOption : Option
	{
		public Option.OptionCollection Options { get; } = new Option.OptionCollection();

		public GroupOption(string title, Option[] options = null)
		{
			Title = title;
			if (options != null) {
				foreach (Option option in options) {
					Options.Add (option);
				}
			}
		}
	}
	public class BooleanOption : Option<bool>
	{
		public BooleanOption(string title, bool defaultValue = false) : base(title, defaultValue)
		{
		}
	}
	public abstract class ChoiceOption : TextOption
	{
		protected ChoiceOption(string title, string defaultValue) : base(title, defaultValue)
		{
		}
	}
	public class ChoiceOption<T> : ChoiceOption
	{
		public ChoiceOption(string title, ChoiceOptionValue defaultValue = null, ChoiceOptionValue[] values = null) : base (title, null)
		{
			if (values == null) {
				values = new ChoiceOptionValue[0];
			}
			if (defaultValue != null) {
				if (defaultValue.Value != null) {
					base.DefaultValue = defaultValue.Value.ToString ();
				} else {
					base.DefaultValue = null;
				}
			}
			
			foreach (ChoiceOptionValue value in values) {
				ValidValues.Add (value);
			}
		}

		public class ChoiceOptionValue
		{
			public class ChoiceOptionValueCollection
				: System.Collections.ObjectModel.Collection<ChoiceOptionValue>
			{
			}

			public string Title { get; set; } = String.Empty;
			public T Value { get; set; } = default(T);

			public ChoiceOptionValue(string title, T value) 
			{
				Title = title;
				Value = value;
			}
		}

		public ChoiceOptionValue.ChoiceOptionValueCollection ValidValues { get; } = new ChoiceOptionValue.ChoiceOptionValueCollection();
		public ChoiceOptionValue SelectedValue { get; set; } = null;

		public bool RequireSelectionFromList { get; set; } = true;
	}
	public class TextOption : Option<string>
	{
		public TextOption(string title, string defaultValue = "") : base(title, defaultValue)
		{
		}
	}
	public abstract class Option<T> : Option
	{
		public T DefaultValue { get; set; } = default(T);
		public T Value { get; set; } = default(T);

		public void Reset()
		{
			Value = DefaultValue;
		}

		public Option(string title, T defaultValue = default(T))
		{
			Title = title;
			DefaultValue = defaultValue;
		}
	}
	public class Option
	{
		public string Title { get; set; } = String.Empty;

		public class OptionCollection
			: System.Collections.ObjectModel.Collection<Option>
		{
		}

		protected Option()
		{
		}
	}
}

