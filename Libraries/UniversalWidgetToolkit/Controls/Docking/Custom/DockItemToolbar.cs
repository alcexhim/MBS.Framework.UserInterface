// 
// DockItemToolbar.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using Gtk;

using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Components.Docking
{
	public class DockItemToolbar
	{
		DockItem parentItem;
		Gtk.Widget frame;
		Box box;
		DockPositionType position;
		bool empty = true;
		CustomFrame topFrame;
		
		internal DockItemToolbar (DockItem parentItem, DockPositionType position)
		{
			this.parentItem = parentItem;

			topFrame = new CustomFrame ();
			topFrame.SetPadding (3,3,3,3);

/*			switch (position) {
				case PositionType.Top:
					frame.SetMargins (0, 0, 1, 1); 
					frame.SetPadding (0, 2, 2, 0); 
					break;
				case PositionType.Bottom:
					frame.SetMargins (0, 1, 1, 1);
					frame.SetPadding (2, 2, 2, 0); 
					break;
				case PositionType.Left:
					frame.SetMargins (0, 1, 1, 0);
					frame.SetPadding (0, 0, 2, 2); 
					break;
				case PositionType.Right:
					frame.SetMargins (0, 1, 0, 1);
					frame.SetPadding (0, 0, 2, 2); 
					break;
			}*/

			this.position = position;
			if (position == DockPositionType.Top || position == DockPositionType.Bottom)
				box = new HBox (false, 3);
			else
				box = new VBox (false, 3);
			box.Show ();
//			frame = box;
			frame = topFrame;
			topFrame.Add (box);

//			topFrame.GradientBackround = true;

			box.Accessible.SetShouldIgnore (false);
			box.Accessible.Role = Atk.Role.ToolBar;

			UpdateAccessibilityLabel ();
		}

		internal void UpdateAccessibilityLabel ()
		{
			string name = "";
			switch (position) {
			case DockPositionType.Bottom:
				name = Core.GettextCatalog.GetString ("Bottom {0} pad toolbar", parentItem.Label);
				break;

			case DockPositionType.Left:
				name = Core.GettextCatalog.GetString ("Left {0} pad toolbar", parentItem.Label);
				break;

			case DockPositionType.Right:
				name = Core.GettextCatalog.GetString ("Right {0} pad toolbar", parentItem.Label);
				break;

			case DockPositionType.Top:
				name = Core.GettextCatalog.GetString ("Top {0} pad toolbar", parentItem.Label);
				break;
			}

			box.Accessible.SetCommonAttributes ("padtoolbar", name, "");
		}

		internal void SetStyle (DockVisualStyle style)
		{
			topFrame.BackgroundColor = style.PadBackgroundColor.Value.ToGdkColor ();
		}

		internal Atk.Object Accessible {
			get {
				return box.Accessible;
			}
		}

		public DockItem DockItem {
			get { return parentItem; }
		}
		
		internal Widget Container {
			get { return frame; }
		}
		
		public DockPositionType Position {
			get { return this.position; }
		}
		
		public void Add (Control widget)
		{
			Add (widget, false);
		}
		
		public void Add (Control widget, bool fill)
		{
			Add (widget, fill, -1);
		}
		
		public void Add (Control widget, bool fill, int padding)
		{
			Add (widget, fill, padding, -1);
		}
		
		void Add (Control control, bool fill, int padding, int index)
		{
			int defaultPadding = 3;

			Widget widget = control;
			if (widget is Button) {
				((Button)widget).Relief = ReliefStyle.None;
				((Button)widget).FocusOnClick = false;
				defaultPadding = 0;
			}
			else if (widget is Entry) {
				((Entry)widget).HasFrame = false;
			}
			else if (widget is ComboBox) {
				((ComboBox)widget).HasFrame = false;
			}
			else if (widget is VSeparator)
				((VSeparator)widget).HeightRequest = 10;
			
			if (padding == -1)
				padding = defaultPadding;
			
			box.PackStart (widget, fill, fill, (uint)padding);
			if (empty) {
				empty = false;
				frame.Show ();
			}
			if (index != -1) {
				Box.BoxChild bc = (Box.BoxChild) box [widget];
				bc.Position = index;
			}
		}
		
		public void Insert (Control w, int index)
		{
			Add (w, false, 0, index);
		}
		
		public void Remove (Control widget)
		{
			box.Remove (widget);
		}
		
		public bool Visible {
			get {
				return empty || frame.Visible;
			}
			set {
				frame.Visible = value;
			}
		}
		
		public bool Sensitive {
			get { return frame.Sensitive; }
			set { frame.Sensitive = value; }
		}
		
		public void ShowAll ()
		{
			frame.ShowAll ();
		}
		
		public Control[] Children {
			get { return box.Children.Select (child => (Control)child).ToArray (); }
		}
	}
	
	public class DockToolButton : Control
	{
		public ImageView Image {
			get { return (ImageView)button.Image; }
			set { button.Image = value; }
		}

		public string TooltipText {
			get { return button.TooltipText; }
			set { button.TooltipText = value; }
		}

		public string Label {
			get { return button.Label; }
			set { button.Label = value; }
		}

		public bool Sensitive {
			get { return button.Sensitive; }
			set { button.Sensitive = value; }
		}

		Gtk.Button button;

		public DockToolButton (string stockId) : this (stockId, null)
		{
		}
		
		public DockToolButton (string stockId, string label)
		{
			button = new Button ();
			Label = label;

			Image = new ImageView (stockId, IconSize.Menu);
			Image.Show ();
		}

		protected override object CreateNativeWidget<T> ()
		{
			return button;
		}

		public event EventHandler Clicked {
			add {
				button.Clicked += value;
			}
			remove {
				button.Clicked -= value;
			}
		}

		public class DockToolButtonImage : Control
		{
			ImageView image;
			internal DockToolButtonImage (ImageView image)
			{
				this.image = image;
			}

			protected override object CreateNativeWidget<T> ()
			{
				return image;
			}

			public static implicit operator Gtk.Widget (DockToolButtonImage d)
			{
				return d.GetNativeWidget<Gtk.Widget> ();
			}

			public static implicit operator DockToolButtonImage (ImageView d)
			{
				return new DockToolButtonImage (d);
			}
		}
	}
}

