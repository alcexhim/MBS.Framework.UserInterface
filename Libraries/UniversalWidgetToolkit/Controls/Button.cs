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
		public Button()
		{
		}
		public Button(StockType type)
		{
			mvarStockType = type;
		}

		private ButtonBorderStyle mvarBorderStyle = ButtonBorderStyle.Normal;
		public ButtonBorderStyle BorderStyle { get { return mvarBorderStyle; } set { mvarBorderStyle = value; Application.Engine.UpdateControlProperties (this); } }

		private StockType mvarStockType = StockType.None;
		public StockType StockType { get { return mvarStockType; } set { mvarStockType = value; } }
	}
}
