//
//  GladeXMLDataFormat.cs
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
using UniversalEditor;
using UniversalEditor.DataFormats.Markup.XML;
using UniversalEditor.ObjectModels.Markup;
using MBS.Framework.UserInterface.ObjectModels.Layout;

namespace MBS.Framework.UserInterface.DataFormats.Layout.Glade
{
	public class GladeXMLDataFormat : XMLDataFormat
	{
		protected override void BeforeLoadInternal(Stack<ObjectModel> objectModels)
		{
			base.BeforeLoadInternal(objectModels);
			objectModels.Push(new MarkupObjectModel());
		}
		protected override void AfterLoadInternal(Stack<ObjectModel> objectModels)
		{
			base.AfterLoadInternal(objectModels);

			MarkupObjectModel mom = (objectModels.Pop() as MarkupObjectModel);
			LayoutObjectModel layout = (objectModels.Pop() as LayoutObjectModel);

			MarkupTagElement tagInterface = (mom.Elements["interface"] as MarkupTagElement);
			if (tagInterface == null) throw new InvalidDataFormatException();

			foreach (MarkupElement el in tagInterface.Elements)
			{
				MarkupTagElement tag = (el as MarkupTagElement);
				if (tag == null) continue;

				if (tag.Name == "object")
				{
					LayoutItem item = LoadLayoutItem(tag);
					layout.Items.Add(item);
				}
			}
		}

		private LayoutItem LoadLayoutItem(MarkupTagElement tag, MarkupTagElement tagPacking = null, MarkupTagElement tagAttributes = null)
		{
			LayoutItem item = new LayoutItem();

			MarkupAttribute attClass = tag.Attributes["class"];
			if (attClass != null) item.ClassName = attClass.Value;

			MarkupAttribute attId = tag.Attributes["id"];
			if (attId != null) item.ID = attId.Value;

			foreach (MarkupElement el in tag.Elements)
			{
				MarkupTagElement tag1 = (el as MarkupTagElement);
				if (tag1 == null) continue;

				switch (tag1.Name)
				{
					case "child":
					{
						MarkupAttribute attChildType = tag1.Attributes["type"];
						MarkupAttribute attInternalChildType = tag1.Attributes["internal-child"];

						MarkupTagElement tagObject = (tag1.Elements["object"] as MarkupTagElement);
						MarkupTagElement tagPacking1 = (tag1.Elements["packing"] as MarkupTagElement);
						MarkupTagElement tagAttributes1 = (tag1.Elements["attributes"] as MarkupTagElement);
						if (tagObject != null)
						{
							LayoutItem itemChild = LoadLayoutItem(tagObject, tagPacking1, tagAttributes1);
							if (attChildType != null)
							{
								itemChild.ChildType = attChildType.Value;
							}
							if (attInternalChildType != null)
							{
								itemChild.InternalType = attInternalChildType.Value;
							}
							item.Items.Add(itemChild);
						}
						break;
					}
					case "attributes":
					{
						foreach (MarkupElement elAttr in tag1.Elements)
						{
							MarkupTagElement tagAttr = (elAttr as MarkupTagElement);
							if (tagAttr == null) continue;

							MarkupAttribute attName = tagAttr.Attributes["name"];
							MarkupAttribute attValue = tagAttr.Attributes["value"];

							if (attName == null) continue;

							string value = null;
							if (attValue != null)
							{
								value = attValue.Value;
							}
							else
							{
								value = tagAttr.Value;
							}

							item.Attributes.Add(attName.Value, value);
						}
						break;
					}
					case "property":
					{
						LayoutItemProperty property = new LayoutItemProperty();
						MarkupAttribute attName = tag1.Attributes["name"];
						if (attName != null) property.Name = attName.Value;
						property.Value = tag1.Value;
						item.Properties.Add(property);
						break;
					}
					case "style":
					{
						for (int i = 0; i < tag1.Elements.Count; i++)
						{
							MarkupTagElement tagStyleClass = tag1.Elements[i] as MarkupTagElement;
							if (tagStyleClass == null) continue;
							if (tagStyleClass.FullName != "class") continue;

							MarkupAttribute attStyleClassName = tagStyleClass.Attributes["name"];
							if (attStyleClassName == null) continue;

							item.StyleClasses.Add(attStyleClassName.Value);
						}
						break;
					}
					case "action-widgets":
					{
						for (int i = 0; i < tag1.Elements.Count; i++)
						{
							MarkupTagElement tagActionWidget = tag1.Elements[i] as MarkupTagElement;
							if (tagActionWidget == null) continue;
							if (tagActionWidget.FullName != "action-widget") continue;

							MarkupAttribute attResponse = tagActionWidget.Attributes["response"];
							if (attResponse == null) continue;

							LayoutItemProperty actionWidget = new LayoutItemProperty();
							actionWidget.Name = tagActionWidget.Value;
							actionWidget.Value = attResponse.Value;
							item.ActionWidgets.Add(actionWidget);
						}
						break;
					}
				}
			}

			if (tagPacking != null)
			{
				foreach (MarkupElement el in tagPacking.Elements)
				{
					MarkupTagElement tagPackingProperty = (el as MarkupTagElement);
					if (tagPackingProperty == null) continue;
					if (!tagPackingProperty.Name.Equals("property")) continue;

					MarkupAttribute attName = tagPackingProperty.Attributes["name"];
					if (attName == null) continue;

					item.PackingProperties.Add(attName.Value, tagPackingProperty.Value);
				}
			}
			if (tagAttributes != null)
			{
				foreach (MarkupElement elAttr in tagAttributes.Elements)
				{
					MarkupTagElement tagAttr = (elAttr as MarkupTagElement);
					if (tagAttr == null) continue;

					MarkupAttribute attName = tagAttr.Attributes["name"];
					MarkupAttribute attValue = tagAttr.Attributes["value"];

					if (attName == null) continue;

					string value = null;
					if (attValue != null)
					{
						value = attValue.Value;
					}
					else
					{
						value = tagAttr.Value;
					}

					item.Attributes.Add(attName.Value, value);
				}
			}

			MarkupTagElement tagColumns = tag.Elements["columns"] as MarkupTagElement;
			if (tagColumns != null)
			{
				LayoutItem columns = new LayoutItem();
				columns.ClassName = "columns";
				foreach (MarkupElement elColumn in tagColumns.Elements)
				{
					LayoutItem column = new LayoutItem();

					MarkupTagElement tagColumn = (elColumn as MarkupTagElement);
					if (tagColumn == null) continue;
					if (tagColumn.FullName != "column") continue;

					MarkupAttribute type = tagColumn.Attributes["type"];
					if (type != null)
					{
						column.ClassName = type.Value;
					}

					columns.Items.Add(column);
				}
				item.Items.Add(columns);
			}
			MarkupTagElement tagData = tag.Elements["data"] as MarkupTagElement;
			if (tagData != null)
			{
				LayoutItem data = new LayoutItem();
				data.ClassName = "data";
				foreach (MarkupElement elRow in tagData.Elements)
				{
					MarkupTagElement tagRow = (elRow as MarkupTagElement);
					if (tagRow == null) continue;
					if (tagRow.FullName != "row") continue;

					LayoutItem row = new LayoutItem();
					row.ClassName = "row";

					for (int i = 0; i < tagRow.Elements.Count; i++)
					{
						MarkupTagElement tagCol = tagRow.Elements[i] as MarkupTagElement;
						if (tagCol == null) continue;
						if (tagCol.FullName != "col") continue;

						LayoutItem col = new LayoutItem();
						col.ClassName = "col";

						MarkupAttribute id = tagCol.Attributes["id"];
						if (id != null)
						{
							col.Attributes.Add("id", id.Value);
						}
						col.Value = tagCol.Value;

						row.Items.Add(col);
					}

					data.Items.Add(row);
				}
				item.Items.Add(data);
			}
			return item;
		}

		protected override void BeforeSaveInternal(Stack<ObjectModel> objectModels)
		{
			base.BeforeSaveInternal(objectModels);

			LayoutObjectModel layout = (objectModels.Pop() as LayoutObjectModel);
			MarkupObjectModel mom = new MarkupObjectModel();

			objectModels.Push(mom);
		}
	}
}
