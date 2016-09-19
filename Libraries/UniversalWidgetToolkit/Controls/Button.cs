using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Controls
{
	public enum ButtonBorderStyle
	{
		None,
		Half,
		Normal
	}
	public class Button : Control
	{
		public class ButtonCollection
			: System.Collections.ObjectModel.Collection<Button>
		{
		}

		public Button()
		{
		}
		public Button(string text, DialogResult responseValue = DialogResult.None)
			: this(text, (int)responseValue)
		{
		}
		public Button(string text, int responseValue = (int)DialogResult.None)
		{
			this.Text = text;
			mvarResponseValue = responseValue;
		}
		public Button(ButtonStockType type, DialogResult responseValue = DialogResult.None)
			: this(type, (int)responseValue)
		{
		}
		public Button(ButtonStockType type, int responseValue = (int)DialogResult.None)
		{
			mvarStockType = type;
			mvarResponseValue = responseValue;
		}

		private ButtonBorderStyle mvarBorderStyle = ButtonBorderStyle.Normal;
		public ButtonBorderStyle BorderStyle { get { return mvarBorderStyle; } set { mvarBorderStyle = value; Application.Engine.UpdateControlProperties (this); } }

		private ButtonStockType mvarStockType = ButtonStockType.None;
		public ButtonStockType StockType { get { return mvarStockType; } set { mvarStockType = value; } }

		private int mvarResponseValue = 0;
		/// <summary>
		/// The response value used when this <see cref="Button" /> is added to a <see cref="Dialog" />.
		/// </summary>
		/// <value>The response value.</value>
		public int ResponseValue { get { return mvarResponseValue; } set { mvarResponseValue = value; } }
	}
}
