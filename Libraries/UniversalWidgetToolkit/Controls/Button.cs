using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Controls
{
	public class Button : Control
	{
		public Button()
		{
		}
		public Button(StockButtonType type)
		{
			mvarStockType = type;
		}

		private StockButtonType mvarStockType = StockButtonType.None;
		public StockButtonType StockType { get { return mvarStockType; } set { mvarStockType = value; } }
	}
}
