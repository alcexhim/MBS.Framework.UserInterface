using System;
using System.Collections.Generic;
using System.Text;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.ListView;
using MBS.Framework.UserInterface.DataFormats.Layout.Glade;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Layouts;
using MBS.Framework.UserInterface.ObjectModels.Layout;
using UniversalEditor;
using UniversalEditor.Accessors;

namespace MBS.Framework.UserInterface
{
	public class ContainerLayoutLoader
	{
		private Container container = null;
		public ContainerLayoutLoader(Container container)
		{
			this.container = container;
		}

		public void InitContainerLayout(Container.ContainerLayoutAttribute wla)
		{
			Accessor acc = null;
			if (wla.ResourceType != null)
			{
				System.IO.Stream st = wla.ResourceType.Assembly.GetManifestResourceStream(wla.PathName);
				if (st == null)
				{
					Console.WriteLine("container layout stream not found: [{0}]'{1}'", wla.ResourceType.FullName, wla.PathName);
					return;
				}
				acc = new StreamAccessor(st);
			}
			else
			{
				string fileName = ((UIApplication)Application.Instance).ExpandRelativePath(wla.PathName);
				if (fileName == null)
				{
					Console.WriteLine("container layout file not found: '{0}'", wla.PathName);
					return;
				}
				acc = new FileAccessor(fileName);
			}

			LoadFromMarkup(acc, wla.ClassName);

			System.Reflection.FieldInfo[] fis = container.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			foreach (System.Reflection.FieldInfo fi in fis)
			{
				if (fi.FieldType.IsSubclassOf(typeof(Control)))
				{
					// see if we have a control by that name in the list
					Control ctl = container.GetControlByID(fi.Name);
					if (ctl != null)
					{
						if (fi.FieldType == ctl.GetType())
						{
							fi.SetValue(container, ctl);
						}
						else
						{
							Console.Error.WriteLine("field type mismatch");
						}
					}
				}
			}

			System.Reflection.MethodInfo[] mis = container.GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			foreach (System.Reflection.MethodInfo mi in mis)
			{
				object[] atts = mi.GetCustomAttributes(typeof(EventHandlerAttribute), false);
				foreach (object att in atts)
				{
					if (att is EventHandlerAttribute)
					{
						EventHandlerAttribute eha = (att as EventHandlerAttribute);
						Control ctl = container.GetControlByID(eha.ControlName);
						if (ctl == null) continue;

						System.Reflection.EventInfo ei = ctl.GetType().GetEvent(eha.EventName);
						if (ei != null)
						{
							Delegate delg = Delegate.CreateDelegate(ei.EventHandlerType, container, mi.Name);
							ei.AddEventHandler(ctl, delg);
						}
					}
				}
			}
		}

		private void CreateForPropertyOrLocalRef<T>(LayoutItem item, Func<LayoutItem, T> func)
		{
			bool found = false;
			System.Reflection.FieldInfo[] fis = container.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			foreach (System.Reflection.FieldInfo fi in fis)
			{
				if (fi.FieldType.IsAssignableFrom(typeof(T)))
				{
					// see if we have a control by that name in the list
					if (fi.Name == item.ID)
					{
						fi.SetValue(container, func(item));
						found = true;
					}
				}
			}

			if (!found)
			{
				_localRefs[item.ID] = func(item);
			}
		}

		private Image CreateImage(LayoutItem item)
		{
			Image image = ImageFromGtkImage(item);
			return image;
		}

		public void LoadFromMarkup(Accessor acc, string className = null, string id = null)
		{
			GladeXMLDataFormat xml = new GladeXMLDataFormat();
			LayoutObjectModel layout = new LayoutObjectModel();
			Document.Load(layout, xml, acc);

			foreach (LayoutItem item in layout.Items)
			{
				if (item.ClassName == "GtkTreeStore" || item.ClassName == "GtkListStore")
				{
					CreateForPropertyOrLocalRef(item, CreateTreeModel);
					continue;
				}
				else if (item.ClassName == "GtkAdjustment")
				{
					CreateForPropertyOrLocalRef(item, CreateAdjustment);
					continue;
				}
				else if (item.ClassName == "GtkImage")
				{
					CreateForPropertyOrLocalRef(item, CreateImage);
					continue;
				}
			}

			// I really don't want to loop twice, but sometimes GtkTreeStore / GtkListStore gets created AFTER the controls that reference them, breaking things
			bool textSet = false;
			foreach (LayoutItem item in layout.Items)
			{
				if (className != null && (item.ClassName != className)) continue;
				if (id != null && (item.ID != null && item.ID != id)) continue;

				if (item.ClassName == "GtkListStore" || item.ClassName == "GtkTreeStore"
					|| item.ClassName == "GtkAdjustment")
				{
					continue;
				}

				LayoutItemProperty pTitle = item.Properties["title"];
				if (pTitle != null && !textSet)
				{
					container.Text = pTitle.Value;
					textSet = true;
				}

				LayoutItem itemBox = item.Items.FirstOfClassName(new string[] { "GtkBox", "GtkGrid" });
				if (itemBox == null)
				{
					Console.WriteLine("warning: layout designer did not specify a container; using GtkBox");
					itemBox = item;
				}

				LayoutItem itemHeaderBar = item.Items.FirstOfClassName(new string[] { "GtkHeaderBar" });
				if (itemHeaderBar != null && container is Window)
				{
					string title = itemHeaderBar.Properties["title"]?.Value ?? String.Empty;
					string subtitle = itemHeaderBar.Properties["subtitle"]?.Value ?? String.Empty;
					(container as Window).Text = title;
					(container as Window).DocumentFileName = subtitle;
				}
				RecursiveLoadContainer(layout, itemBox, container);

				LayoutItemProperty pDefaultWidth = item.Properties["default_width"];
				LayoutItemProperty pDefaultHeight = item.Properties["default_height"];
				if (pDefaultWidth != null)
				{
					container.Size.Width = Int32.Parse(pDefaultWidth.Value);
				}
				if (pDefaultHeight != null)
				{
					container.Size.Height = Int32.Parse(pDefaultHeight.Value);
				}
			}
		}

		private struct GtkAdjustment
		{
			public double Lower;
			public double Upper;
			public double StepIncrement;
			public double PageIncrement;
		}

		private GtkAdjustment CreateAdjustment(LayoutItem item)
		{
			GtkAdjustment adj = new GtkAdjustment();

			if (item.Properties["lower"] != null)
				adj.Lower = double.Parse(item.Properties["lower"].Value);

			if (item.Properties["upper"] != null)
				adj.Upper = double.Parse(item.Properties["upper"].Value);

			if (item.Properties["step_increment"] != null)
				adj.StepIncrement = double.Parse(item.Properties["step_increment"].Value);

			if (item.Properties["page_increment"] != null)
				adj.PageIncrement = double.Parse(item.Properties["page_increment"].Value);
			return adj;
		}

		private object GetPropertyOrLocalRef(string id)
		{
			bool found = false;
			System.Reflection.FieldInfo[] fis = container.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			foreach (System.Reflection.FieldInfo fi in fis)
			{
				// see if we have a control by that name in the list
				if (fi.Name == id)
				{
					object obj = fi.GetValue(container);
					return obj;
				}
			}

			if (!found)
			{
				if (_localRefs.ContainsKey(id))
					return _localRefs[id];
			}
			return null;
		}

		private DefaultTreeModel CreateTreeModel(LayoutItem item)
		{
			List<Type> types = new List<Type>();

			LayoutItem columns = item.Items.FirstOfClassName("columns");
			if (columns == null)
			{
				Console.WriteLine("warning: cannot create TreeModel, columns == null");
				return null;
			}

			LayoutItem rows = item.Items.FirstOfClassName("data");

			for (int j = 0; j < columns.Items.Count; j++)
			{
				switch (columns.Items[j].ClassName)
				{
					case "gchar": types.Add(typeof(sbyte)); break;
					case "guchar": types.Add(typeof(byte)); break;
					case "gboolean": types.Add(typeof(bool)); break;
					case "gshort": types.Add(typeof(short)); break;
					case "gushort": types.Add(typeof(ushort)); break;
					case "gint": types.Add(typeof(int)); break;
					case "guint": types.Add(typeof(uint)); break;
					case "glong": types.Add(typeof(long)); break;
					case "gulong": types.Add(typeof(ulong)); break;
					case "gint64": types.Add(typeof(long)); break;
					case "guint64": types.Add(typeof(ulong)); break;
					case "gfloat": types.Add(typeof(float)); break;
					case "gdouble": types.Add(typeof(double)); break;
					case "gchararray": types.Add(typeof(string)); break;
					case "gpointer": types.Add(typeof(IntPtr)); break;
					default: types.Add(typeof(string)); break;
				}
			}

			DefaultTreeModel dtm = new DefaultTreeModel(types.ToArray());
			if (rows != null)
			{
				for (int j = 0; j < rows.Items.Count; j++)
				{
					// rows
					TreeModelRow row = new TreeModelRow();
					for (int k = 0; k < rows.Items[j].Items.Count; k++)
					{
						// cols
						// oh god make it stop
						LayoutItem col = rows.Items[j].Items[k];
						row.RowColumns.Add(new TreeModelRowColumn(dtm.Columns[Int32.Parse(col.Attributes["id"].Value)], col.Value.Parse(types[k])));
					}
					dtm.Rows.Add(row);
				}
			}
			return dtm;
		}

		private Control RecursiveLoadControl(LayoutObjectModel layout, LayoutItem item)
		{
			Control ctl = null;
			switch (item.ClassName)
			{
				case "GtkFrame":
				{
					ctl = new GroupBox();
					RecursiveLoadContainer(layout, item.Items[0], ctl as Container);
					ctl.Text = item.Items[1].Properties["label"].Value;
					break;
				}
				case "GtkFileChooserButton":
				{
					ctl = new FileChooserButton();
					(ctl as FileChooserButton).DialogTitle = item.Properties["title"]?.Value;
					(ctl as FileChooserButton).DialogMode = Dialogs.FileDialogMode.Open;
					if (item.Properties["action"] != null)
					{
						switch (item.Properties["action"].Value)
						{
							case "save":
							{
								(ctl as FileChooserButton).DialogMode = Dialogs.FileDialogMode.Save;
								break;
							}
							case "select-folder":
							{
								(ctl as FileChooserButton).DialogMode = Dialogs.FileDialogMode.SelectFolder;
								break;
							}
							case "create-folder":
							{
								(ctl as FileChooserButton).DialogMode = Dialogs.FileDialogMode.CreateFolder;
								break;
							}
						}
					}
					break;
				}
				case "GtkAlignment":
				case "GtkScrolledWindow":
				{
					if (item.Items.Count > 0)
					{
						ctl = RecursiveLoadControl(layout, item.Items[0]);
					}
					break;
				}
				case "GtkButtonBox":
				{
					break;
				}
				case "GtkComboBox":
				{
					ctl = new ComboBox();
					if (item.Properties["has_entry"] != null)
					{
						(ctl as ComboBox).ReadOnly = item.Properties["has_entry"].Value != "True";
					}
					else
					{
						(ctl as ComboBox).ReadOnly = true;
					}
					if (item.Properties["model"] != null)
					{
						(ctl as ComboBox).Model = GetPropertyOrLocalRef(item.Properties["model"].Value) as TreeModel;
					}
					break;
				}
				case "GtkSpinButton":
				{
					ctl = new NumericTextBox();
					if (item.Properties["adjustment"] != null)
					{
						GtkAdjustment adj = (GtkAdjustment)GetPropertyOrLocalRef(item.Properties["adjustment"].Value);
						(ctl as NumericTextBox).Minimum = adj.Lower;
						(ctl as NumericTextBox).Maximum = adj.Upper;
						(ctl as NumericTextBox).SmallIncrement = adj.StepIncrement;
						(ctl as NumericTextBox).LargeIncrement = adj.PageIncrement;
					}
					if (item.Properties["digits"] != null)
					{
						(ctl as NumericTextBox).DecimalPlaces = Int32.Parse(item.Properties["digits"].Value);
					}
					break;
				}
				case "GtkButton":
				{
					ctl = new Button();
					if (item.StyleClasses.Contains("suggested-action"))
					{
						(ctl as Button).StylePreset = CommandStylePreset.Suggested;
					}
					else if (item.StyleClasses.Contains("destructive-action"))
					{
						(ctl as Button).StylePreset = CommandStylePreset.Destructive;
					}
					if (item.Properties["label"] != null)
					{
						ctl.Text = item.Properties["label"].Value;
					}
					if (item.Properties["image"] != null)
					{
						(ctl as Button).Image = (GetPropertyOrLocalRef(item.Properties["image"].Value) as Image);
					}
					if ((item.Properties["use_stock"] != null) && (item.Properties["use_stock"].Value.Equals("True")))
					{
						(ctl as Button).StockType = (StockType)((UIApplication)Application.Instance).Engine.StockTypeFromString(item.Properties["label"].Value);
						switch ((ctl as Button).StockType)
						{
							case StockType.Cancel:
							{
								(ctl as Button).ResponseValue = (int)DialogResult.Cancel;
								break;
							}
						}
					}
					break;
				}
				case "GtkSearchEntry":
				case "GtkEntry":
				case "GtkTextView":
				{
					ctl = new TextBox();
					if (item.ClassName == "GtkTextView")
					{
						(ctl as TextBox).Multiline = true;
					}

					if (item.Properties["editable"] != null)
					{
						(ctl as TextBox).Editable = (item.Properties["editable"].Value == "True");
					}
					if (item.Properties["xalign"] != null)
					{
						decimal xalign = Decimal.Parse(item.Properties["xalign"].Value);
						if (xalign < 0.5M)
						{
							(ctl as TextBox).TextAlignment = HorizontalAlignment.Left;
						}
						else if (xalign == 0.5M)
						{
							(ctl as TextBox).TextAlignment = HorizontalAlignment.Center;
						}
						else if (xalign > 0.5M)
						{
							(ctl as TextBox).TextAlignment = HorizontalAlignment.Right;
						}
					}
					break;
				}
				case "GtkLabel":
				case "GtkAccelLabel":
				{
					ctl = new Label();

					if (item.Properties["use_markup"] != null)
					{
						ctl.UseMarkup = (item.Properties["use_markup"].Value == "True");
					}
					if (item.Properties["label"] != null)
					{
						ctl.Text = UnescapeHTMLIf(item.Properties["label"].Value, ctl.UseMarkup);
					}
					if (item.Attributes["font-desc"] != null)
					{
						ctl.Font = Font.Parse(item.Attributes["font-desc"].Value);
					}
					if (item.Attributes["scale"] != null)
					{
						ctl.Attributes.Add("scale", Double.Parse(item.Attributes["scale"].Value));
					}
					if (item.Attributes["weight"] != null)
					{
						double weight = 400;
						if (!Double.TryParse(item.Attributes["weight"].Value, out weight))
						{
							switch (item.Attributes["weight"].Value)
							{
								case "bold": weight = 700; break;
								default: Console.WriteLine("uwt: containerlayout: warning: value '{0}' for font-weight not supported", item.Attributes["weight"].Value); break;
							}
						}
						ctl.Attributes.Add("weight", weight);
						if (ctl.Font != null)
						{
							ctl.Font.Weight = weight;
						}
						else
						{
							ctl.Font = new Font();
							ctl.Font.Weight = weight;
						}
					}
					if (item.Properties["wrap"] != null)
					{
						(ctl as Label).WordWrap = (item.Properties["wrap"].Value == "True") ? WordWrapMode.Always : WordWrapMode.Never;
					}
					if (item.Properties["width_chars"] != null)
					{
						if (Int32.TryParse(item.Properties["width_chars"].Value, out int wc))
						{
							(ctl as Label).WidthChars = wc;
						}
					}
					if (item.Properties["xalign"] != null)
					{
						double align = Double.Parse(item.Properties["xalign"].Value);
						if (align >= 0 && align < 0.25)
						{
							(ctl as Label).HorizontalAlignment = HorizontalAlignment.Left;
						}
						else if (align > 0.75)
						{
							(ctl as Label).HorizontalAlignment = HorizontalAlignment.Right;
						}
						else
						{
							(ctl as Label).HorizontalAlignment = HorizontalAlignment.Center;
						}
					}
					break;
				}
				case "GtkCheckButton":
				{
					ctl = new CheckBox();
					if (item.Properties["label"] != null)
					{
						ctl.Text = item.Properties["label"].Value;
					}
					break;
				}
				case "GtkImage":
				{
					ctl = new Controls.PictureFrame();
					(ctl as Controls.PictureFrame).Image = ImageFromGtkImage(item);
					break;
				}
				case "GtkPaned":
				{
					ctl = new SplitContainer();

					Orientation orientation = Orientation.Vertical;
					LayoutItemProperty propOrientation = item.Properties["orientation"];
					if (propOrientation != null)
					{
						switch (propOrientation.Value.ToLower())
						{
							case "vertical": orientation = Orientation.Horizontal; break;
							case "horizontal": orientation = Orientation.Vertical; break;
						}
					}
					(ctl as SplitContainer).Orientation = orientation;

					LayoutItemProperty propPosition = item.Properties["position"];
					if (propPosition != null)
					{
						(ctl as SplitContainer).SplitterPosition = Int32.Parse(propPosition.Value);
					}

					// only two children here
					if (item.Items.Count > 0)
					{
						RecursiveLoadContainer(layout, item.Items[0], (ctl as SplitContainer).Panel1);
					}
					if (item.Items.Count > 1)
					{
						RecursiveLoadContainer(layout, item.Items[1], (ctl as SplitContainer).Panel2);
					}
					break;
				}
				case "GtkNotebook":
				{
					ctl = new TabContainer();
					for (int i = 0; i < item.Items.Count; i += 2)
					{
						LayoutItem itemContent = item.Items[i];
						if (i + 1 < item.Items.Count)
						{
							LayoutItem itemTab = item.Items[i + 1];
							if (itemTab.ClassName == "GtkLabel")
							{
								TabPage tabPage = new TabPage();
								if (itemTab.Properties["label"] != null)
								{
									tabPage.Text = itemTab.Properties["label"].Value;
								}
								RecursiveLoadContainer(layout, itemContent, tabPage);
								(ctl as TabContainer).TabPages.Add(tabPage);
							}
						}
						else
						{
							// single tab, but no content, so we add placeholder
							if (itemContent.ClassName == "GtkLabel")
							{
								TabPage tabPage = new TabPage();
								if (itemContent.Properties["label"] != null)
								{
									tabPage.Text = itemContent.Properties["label"].Value;
								}
								tabPage.Layout = new BoxLayout(Orientation.Vertical);
								tabPage.Controls.Add(new Label(), new BoxLayout.Constraints(true, true));
								(ctl as TabContainer).TabPages.Add(tabPage);
							}
						}
					}
					break;
				}
				case "GtkTreeSelection":
				{
					// intentionally ignored
					break;
				}
				case "GtkIconView":
				case "GtkTreeView":
				{
					ctl = new ListViewControl();
					if (item.Properties["model"] != null)
					{
						DefaultTreeModel tm = GetPropertyOrLocalRef(item.Properties["model"].Value) as DefaultTreeModel;
						(ctl as ListViewControl).Model = tm;
					}

					(ctl as ListViewControl).HeaderStyle = ColumnHeaderStyle.Clickable;
					if (item.Properties["headers_visible"] != null)
					{
						if ("False".Equals(item.Properties["headers_visible"].Value))
						{
							(ctl as ListViewControl).HeaderStyle = ColumnHeaderStyle.None;
						}
					}
					else if (item.Properties["headers_clickable"] != null)
					{
						if ("False".Equals(item.Properties["headers_clickable"].Value))
						{
							(ctl as ListViewControl).HeaderStyle = ColumnHeaderStyle.Nonclickable;
						}
					}

					foreach (LayoutItem item2 in item.Items)
					{
						if (item2.ClassName == "GtkTreeViewColumn")
						{
							TreeModelColumn col = null;
							if (item2.Items.Count > 0)
							{
								if (item2.Items[0].Attributes["text"] != null)
								{
									int colindex = Int32.Parse(item2.Items[0].Attributes["text"].Value);
									col = (ctl as ListViewControl).Model?.Columns[colindex];
								}
							}
							ListViewColumn ch = new ListViewColumnText(col, item2.Properties["title"]?.Value);
							(ctl as ListViewControl).Columns.Add(ch);
						}
						else if (item2.ClassName == "GtkTreeSelection" && item2.InternalType == "selection")
						{
							LayoutItemProperty propMode = item2.Properties["mode"];
							if (propMode != null)
							{
								switch (propMode.Value)
								{
									case "none":
									{
										(ctl as ListViewControl).SelectionMode = SelectionMode.None;
										break;
									}
									case "multiple":
									{
										(ctl as ListViewControl).SelectionMode = SelectionMode.Multiple;
										break;
									}
									case "browse":
									{
										(ctl as ListViewControl).SelectionMode = SelectionMode.Browse;
										break;
									}
									case "single":
									{
										(ctl as ListViewControl).SelectionMode = SelectionMode.Single;
										break;
									}
									default:
									{
										// if the property is there, but is invalid, SelectionMode gets set to None
										(ctl as ListViewControl).SelectionMode = SelectionMode.None;
										break;
									}
								}
							}
							else
							{
								// if the property isn't there at all, SelectionMode gets set to Single
								(ctl as ListViewControl).SelectionMode = SelectionMode.Single;
							}
						}
					}
					break;
				}
				case "GtkBox":
				{
					ctl = new Container();
					RecursiveLoadContainer(layout, item, (ctl as Container));
					break;
				}
				case "GtkGrid":
				{
					ctl = new Container();
					RecursiveLoadContainer(layout, item, (ctl as Container));
					break;
				}
				case "GtkExpander":
				{
					ctl = new Disclosure();
					RecursiveLoadContainer(layout, item, (ctl as Container));
					break;
				}
				case "GtkDrawingArea":
				{
					LayoutItemProperty name = item.Properties["name"];
					if (name != null)
					{
						string className = name.Value;
						Type classType = MBS.Framework.Reflection.FindType(className);
						if (classType != null)
						{
							ctl = (classType.Assembly.CreateInstance(classType.FullName) as Control);
						}
					}
					break;
				}
				case "GtkToolbar":
				{
					ctl = new Toolbar();
					for (int i = 0; i < item.Items.Count; i++)
					{
						switch (item.Items[i].ClassName)
						{
							case "GtkToolButton":
							{
								LayoutItemProperty propStockId = item.Items[i].Properties["stock_id"];
								if (propStockId != null)
								{
									StockType stockType = ((UIApplication)Application.Instance).Engine.StockTypeFromString(propStockId.Value);
									ToolbarItemButton tsb = new ToolbarItemButton(item.Items[i].ID, stockType);
									(ctl as Toolbar).Items.Add(tsb);
								}
								break;
							}
							case "GtkSeparatorToolItem":
							{
								(ctl as Toolbar).Items.Add(new ToolbarItemSeparator());
								break;
							}
						}
					}
					break;
				}
			}

			if (ctl != null)
			{
				if (item.ID != null)
					ctl.Name = item.ID;

				if (item.Properties["sensitive"] != null)
				{
					ctl.Enabled = (item.Properties["sensitive"].Value != "False");
				}
				if (item.Properties["visible"] != null)
				{
					ctl.Visible = (item.Properties["visible"].Value == "True");
				}

				int width_request = -1, height_request = -1;
				if (item.Properties["width_request"] != null)
				{
					width_request = Int32.Parse(item.Properties["width_request"].Value);
				}
				if (item.Properties["height_request"] != null)
				{
					height_request = Int32.Parse(item.Properties["height_request"].Value);
				}
				ctl.MinimumSize = new Dimension2D(width_request, height_request);

				int margin_left = 0, margin_right = 0, margin_top = 0, margin_bottom = 0;
				margin_top = Int32.Parse(item.Properties["margin_top"]?.Value ?? "0");
				margin_bottom = Int32.Parse(item.Properties["margin_bottom"]?.Value ?? "0");
				margin_left = Int32.Parse(item.Properties["margin_left"]?.Value ?? "0");
				margin_right = Int32.Parse(item.Properties["margin_right"]?.Value ?? "0");
				ctl.Margin = new Padding(margin_top, margin_bottom, margin_left, margin_right);
				
				if (item.Properties["valign"] != null)
				{
					ctl.VerticalAlignment = ParseVerticalAlignment(item.Properties["valign"].Value);
				}

				for (int i = 0; i < item.StyleClasses.Count; i++)
				{
					ctl.Style.Classes.Add(item.StyleClasses[i]);
				}
			}
			else
			{
				Console.Error.WriteLine("uwt: ContainerLayout: control class '" + item.ClassName + "' not handled");
			}
			return ctl;
		}

		private Image ImageFromGtkImage(LayoutItem item)
		{
			int size = 16;
			if (item.Properties["pixel_size"] != null)
			{
				size = Int32.Parse(item.Properties["pixel_size"].Value);
			}
			else if (item.Properties["icon_size"] != null)
			{
				int sizePreset = Int32.Parse(item.Properties["icon_size"].Value);
				switch (sizePreset)
				{
					case 6: // dialog
						{
							size = 48;
							break;
						}
				}
			}
			if (item.Properties["icon_name"] != null)
			{
				return Image.FromName(item.Properties["icon_name"].Value, size);
			}
			else if (item.Properties["stock"] != null)
			{
				return Image.FromName(item.Properties["stock"].Value, size);
			}
			return null;
		}

		private VerticalAlignment ParseVerticalAlignment(string value)
		{
			switch (value.ToLower())
			{
				case "start": return VerticalAlignment.Top;
				case "end": return VerticalAlignment.Bottom;
			}
			return VerticalAlignment.Default;
		}

		private string UnescapeHTML(string value)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == '&')
				{
					i++;
					StringBuilder sbc = new StringBuilder();
					while (value[i] != ';')
					{
						sbc.Append(value[i]);
						i++;
					}

					switch (sbc.ToString())
					{
						case "lt":
						{
							sb.Append('<');
							break;
						}
						case "gt":
						{
							sb.Append('>');
							break;
						}
						case "nbsp":
						{
							sb.Append(' ');
							break;
						}
						case "copy":
						{
							sb.Append('©');
							break;
						}
					}

					continue;
				}
				else if (value[i] == '%')
				{
					string psz = (value[i + 1].ToString() + value[i + 2].ToString());
					int index = Int32.Parse(psz);
					sb.Append((char)index);
				}
				else
				{
					sb.Append(value[i]);
				}
			}
			return sb.ToString();
		}
		private string UnescapeHTMLIf(string value, bool useMarkup)
		{
			if (useMarkup)
				return UnescapeHTML(value);
			return value;
		}

		private Dictionary<string, object> _localRefs = new Dictionary<string, object>();

		private void RecursiveLoadContainer(LayoutObjectModel layout, LayoutItem item, Container container)
		{
			double width = 0.0, height = 0.0;
			if (item.Properties["default_width"] != null)
			{
				width = Double.Parse(item.Properties["default_width"].Value);
			}
			if (item.Properties["default_height"] != null)
			{
				height = Double.Parse(item.Properties["default_height"].Value);
			}
			container.Size = new Dimension2D(width, height);

			int margin_left = 0, margin_right = 0, margin_top = 0, margin_bottom = 0;
			margin_top = Int32.Parse(item.Properties["margin_top"]?.Value ?? "0");
			margin_bottom = Int32.Parse(item.Properties["margin_bottom"]?.Value ?? "0");
			margin_left = Int32.Parse(item.Properties["margin_left"]?.Value ?? "0");
			margin_right = Int32.Parse(item.Properties["margin_right"]?.Value ?? "0");
			container.Margin = new Padding(margin_top, margin_bottom, margin_left, margin_right);

			switch (item.ClassName)
			{
				case "GtkBox":
				{
					// layout is a BoxLayout
					Orientation orientation = Orientation.Horizontal;
					LayoutItemProperty propOrientation = item.Properties["orientation"];
					if (propOrientation != null)
					{
						switch (propOrientation.Value.ToLower())
						{
							case "vertical": orientation = Orientation.Vertical; break;
							case "horizontal": orientation = Orientation.Horizontal; break;
						}
					}
					if (item.Properties["default_height"] != null)
					{
						height = Double.Parse(item.Properties["default_height"].Value);
					}
					container.Layout = new BoxLayout(orientation);
					if (item.Properties["spacing"] != null)
					{
						if (Int32.TryParse(item.Properties["spacing"].Value, out int p))
						{
							(container.Layout as BoxLayout).Spacing = p;
						}
					}
					if (item.Properties["homogeneous"] != null)
					{
						(container.Layout as BoxLayout).Homogeneous = (item.Properties["homogeneous"].Value == "True");
					}
					break;
				}
				case "GtkGrid":
				{
					// layout is a GridLayout
					container.Layout = new GridLayout();

					if (item.Properties["row_homogeneous"] != null)
					{
						(container.Layout as GridLayout).RowHomogeneous = (item.Properties["row_homogeneous"].Value == "True");
					}
					if (item.Properties["column_homogeneous"] != null)
					{
						(container.Layout as GridLayout).ColumnHomogeneous = (item.Properties["column_homogeneous"].Value == "True");
					}
					break;
				}
			}

			foreach (LayoutItem item2 in item.Items)
			{
				Control control = RecursiveLoadControl(layout, item2);
				if (item2.ChildType != null)
				{
					// do not add it to the collection
					if (container is Disclosure && item2.ChildType == "label")
					{
						(container as Disclosure).Text = item2.Properties["label"]?.Value;
					}
					continue;
				}

				if (container is Dialog && item2.ClassName == "GtkButtonBox")
				{
					foreach (LayoutItem itemButton in item2.Items)
					{
						Button button = (RecursiveLoadControl(layout, itemButton) as Button);
						if (button != null)
						{
							(container as Dialog).Buttons.Add(button);
						}
					}
					continue;
				}
				if (control != null)
				{
					container.Controls.Add(control);

					LayoutItemProperty propPadding = item2.PackingProperties["padding"];
					if (propPadding != null)
					{
						control.Padding = new Padding(Int32.Parse(propPadding.Value));
					}

					if (container.Layout == null)
					{
						container.Layout = new BoxLayout(Orientation.Vertical);
						container.Layout.SetControlConstraints(control, new BoxLayout.Constraints(true, true));
					}
					else if (container.Layout is BoxLayout)
					{
						LayoutItemProperty propExpand = item2.PackingProperties["expand"];
						bool expand = (propExpand != null && propExpand.Value == "True");
						LayoutItemProperty propFill = item2.PackingProperties["fill"];
						bool fill = (propFill != null && propFill.Value == "True");

						BoxLayout.PackType packType = BoxLayout.PackType.Start;
						LayoutItemProperty propPackType = item2.PackingProperties["pack_type"];
						if (propPackType != null)
						{
							switch (propPackType.Value.ToLower())
							{
								case "start": packType = BoxLayout.PackType.Start; break;
								case "end": packType = BoxLayout.PackType.End; break;
							}
						}

						int padding = 0;
						container.Layout.SetControlConstraints(control, new BoxLayout.Constraints(expand, fill, padding, packType));
					}
					else if (container.Layout is GridLayout)
					{
						LayoutItemProperty propLeftAttach = item2.PackingProperties["left_attach"];
						int left_attach = 0;
						if (propLeftAttach != null) Int32.TryParse(propLeftAttach.Value, out left_attach);

						LayoutItemProperty propTopAttach = item2.PackingProperties["top_attach"];
						int top_attach = 0;
						if (propTopAttach != null) Int32.TryParse(propTopAttach.Value, out top_attach);

						LayoutItemProperty propWidth = item2.PackingProperties["width"];
						int width_attach = 1;
						if (propWidth != null) Int32.TryParse(propWidth.Value, out width_attach);

						LayoutItemProperty propHeight = item2.PackingProperties["height"];
						int height_attach = 1;
						if (propHeight != null) Int32.TryParse(propHeight.Value, out height_attach);

						container.Layout.SetControlConstraints(container.Controls[container.Controls.Count - 1], new GridLayout.Constraints(top_attach, left_attach, height_attach, width_attach));
					}

					LayoutItemProperty propHExpand = item2.Properties["hexpand"];
					LayoutItemProperty propVExpand = item2.Properties["vexpand"];
					if (container.Layout != null && (propHExpand != null || propVExpand != null))
					{
						Constraints constraints = container.Layout.GetControlConstraints(container.Controls[container.Controls.Count - 1]);
						constraints.HorizontalExpand = (propHExpand != null && propHExpand.Value == "True");
						constraints.VerticalExpand = (propVExpand != null && propVExpand.Value == "True");

						if (constraints is GridLayout.Constraints)
						{
							ExpandMode expand = ExpandMode.None;
							if (constraints.HorizontalExpand) expand |= ExpandMode.Horizontal;
							if (constraints.VerticalExpand) expand |= ExpandMode.Vertical;
							(constraints as GridLayout.Constraints).Expand = expand;
						}
					}
				}
				else
				{
					Console.Error.WriteLine("uwt: ContainerLayout: could not load control with class name '" + item2.ClassName + "'");
				}
			}
		}

		
	}
}
