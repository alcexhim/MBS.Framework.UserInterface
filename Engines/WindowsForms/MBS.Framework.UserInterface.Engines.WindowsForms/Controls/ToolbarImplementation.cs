//
//  ToolbarImplementation.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019
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
using System.Runtime.InteropServices;

using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls
{
	[ControlImplementation(typeof(Toolbar))]
	public class ToolbarImplementation : WindowsFormsNativeImplementation
	{
		public ToolbarImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			Toolbar ctl = (control as Toolbar);

			System.Windows.Forms.ToolStrip handle = new System.Windows.Forms.ToolStrip();

			foreach (ToolbarItem item in ctl.Items)
			{
				System.Windows.Forms.ToolStripItem hItem = null;
				if (item is ToolbarItemButton)
				{
					ToolbarItemButton tsb = (item as ToolbarItemButton);

					hItem = new System.Windows.Forms.ToolStripButton();
					(hItem as System.Windows.Forms.ToolStripButton).CheckOnClick = tsb.CheckOnClick;

					System.Drawing.Image iconWidget = null;
					string stockTypeID = Engine.StockTypeToString(tsb.StockType);

					string title = null;
					switch (tsb.DisplayStyle)
					{
						case ToolbarItemDisplayStyle.Default:
						case ToolbarItemDisplayStyle.Image:
						case ToolbarItemDisplayStyle.ImageAndText:
						{
							if (tsb.StockType != StockType.None)
							{
								try
								{
									string filename = ((UIApplication)Application.Instance).ExpandRelativePath(String.Format("~/Themes/{0}/Images/StockIcons/{1}.png", Theming.Theme.CurrentTheme.Name, stockTypeID));
									if (filename == null)
									{
										filename = ((UIApplication)Application.Instance).ExpandRelativePath(String.Format("~/Themes/{0}/Images/StockIcons/{1}.png", "Default", stockTypeID));
									}
									if (filename != null)
										iconWidget = System.Drawing.Image.FromFile(filename);
								}
								catch (System.IO.FileNotFoundException ex)
								{
									Console.WriteLine("stock icon image '{0}' not found for theme '{1}'", stockTypeID, "Default");
								}
							}
							else if (tsb.Image != null)
							{
								/*
								string filename = ((UIApplication)Application.Instance).ExpandRelativePath(String.Format("~/Themes/{0}/Images/StockIcons/{1}.png", Theming.Theme.CurrentTheme.Name, tsb.Image));
								if (filename == null)
								{
									filename = ((UIApplication)Application.Instance).ExpandRelativePath(String.Format("~/Themes/{0}/Images/StockIcons/{1}.png", "Default", tsb.Image));
								}
								if (filename != null)
									iconWidget = System.Drawing.Image.FromFile(filename);
								*/
								iconWidget = ((WindowsFormsNativeImage)tsb.Image).Handle;
							}
							if (iconWidget != null)
							{
								(hItem as System.Windows.Forms.ToolStripButton).Image = iconWidget;
							}
							break;
						}
					}
					switch (tsb.DisplayStyle)
					{
						case ToolbarItemDisplayStyle.Text:
						case ToolbarItemDisplayStyle.ImageAndText:
						{
							title = tsb.Title;
							if (tsb.StockType != StockType.None) {
								/*
								Internal.GTK.Structures.GtkStockItem stock = new Internal.GTK.Structures.GtkStockItem ();
								bool hasStock = Internal.GTK.Methods.GtkStock.gtk_stock_lookup (stockTypeID, ref stock);
								if (hasStock) {
									// fill info from GtkStockItem struct
									title = Marshal.PtrToStringAuto (stock.label);
								}
								*/
							}
							break;
						}
					}

					if (title != null) {
						title = title.Replace ("_", String.Empty);
					}
					hItem.Tag = item;
					hItem.Text = title;
					hItem.Click += hItem_Click;
				}
				else if (item is ToolbarItemSeparator)
				{
					hItem = new System.Windows.Forms.ToolStripSeparator();
				}
				if (hItem != null)
				{
					((UIApplication)Application.Instance).Engine.RegisterToolbarItemHandle(item, new WindowsFormsNativeToolStripItem(hItem));

					int index = ctl.Items.IndexOf(item);
					handle.Items.Insert(index, hItem);
				}
			}
			return new WindowsFormsNativeControl(handle);
		}

		void hItem_Click(object sender, EventArgs e)
		{
			ToolbarItemButton tsb = (sender as System.Windows.Forms.ToolStripButton).Tag as ToolbarItemButton;
			if (tsb != null) InvokeMethod(tsb, "OnClick", EventArgs.Empty);
		}

	}
}
