using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface.Controls
{
	public class Label : SystemControl
	{

		public Label()
		{
		}
		public Label(string text)
		{
			this.Text = text;
		}

		public bool UseMnemonic { get; set; } = true;

		public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Default;
		public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Default;
		public WordWrapMode WordWrap { get; set; } = WordWrapMode.Default;
	}
}
