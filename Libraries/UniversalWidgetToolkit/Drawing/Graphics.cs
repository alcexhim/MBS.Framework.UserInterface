using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.ObjectModels.Theming;
using UniversalWidgetToolkit.ObjectModels.Theming.RenderingActions;
using UniversalWidgetToolkit.Theming;

namespace UniversalWidgetToolkit.Drawing
{
	public abstract class Graphics
	{
		protected abstract void DrawLineInternal(double x1, double y1, double x2, double y2);
		public void DrawLine(double x1, double y1, double x2, double y2)
		{
			DrawLineInternal(x1, y1, x2, y2);
		}

		public void DrawRectangle(double x, double y, double width, double height)
		{
			DrawLine(x, y, x + width, y);
			DrawLine(x, y, x, y + height);
			DrawLine(x, y + height, x + width, y + height);
			DrawLine(x + width, y, x + width, y + height);
		}
		public void DrawRectangle(Rectangle rect)
		{
			DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void Clear(Color color)
		{
			
		}

		/// <summary>
		/// Draws the specified <see cref="ThemeComponent" /> in the specified state.
		/// </summary>
		/// <param name="componentID"></param>
		/// <param name="stateID"></param>
		public void DrawThemeComponent(ThemeComponentReference paramz, Control component, Dictionary<string, object> variables = null)
		{
			if (variables == null) variables = new Dictionary<string,object>();

			ThemeComponent tc = ThemeManager.CurrentTheme.GetComponent(paramz.ComponentID);
			DrawThemeComponent(tc, component, paramz.StateID, variables);
		}
		public void DrawThemeComponent(ThemeComponent tc, Control component, Guid stateID, Dictionary<string, object> variables = null)
		{
			if (tc.InheritsComponent != null)
			{
				DrawThemeComponent(tc.InheritsComponent, component, stateID, variables);
			}

			foreach (Rendering rendering in tc.Renderings)
			{
				if (rendering.States.Count == 0 || rendering.States.Contains(stateID))
				{
					// we can use this rendering
					DrawRendering(rendering, component, variables);
				}
			}
		}

		private void DrawRendering(Rendering rendering, Control component, Dictionary<string, object> variables)
		{
			foreach (RenderingAction action in rendering.Actions)
			{
				DrawRenderingAction(action, component, variables);
			}
		}

		private void DrawRenderingAction(RenderingAction action, Control component, Dictionary<string, object> variables)
		{
			if (variables == null) variables = new Dictionary<string, object>();
			Dictionary<string, object> dict = new Dictionary<string, object>();

			foreach (KeyValuePair<string, object> kvp in variables)
			{
				dict.Add(kvp.Key, kvp.Value);
			}

			Rectangle bounds = component.Parent.Layout.GetControlBounds(component);
			bounds = new Rectangle(0, 0, bounds.Width, bounds.Height);

			dict.Add("Component.Width", bounds.Width);
			dict.Add("Component.Height", bounds.Height);

			/*
			if (component is System.Windows.Forms.ToolStripDropDownMenu)
			{
				System.Windows.Forms.ToolStripDropDownMenu tsddm = (component as System.Windows.Forms.ToolStripDropDownMenu);
				if (tsddm.OwnerItem != null)
				{
					dict.Add("Component.Parent.Width", tsddm.OwnerItem.Width);
					dict.Add("Component.Parent.Height", tsddm.OwnerItem.Height);
				}
			}
			if (component is System.Windows.Forms.ToolStripSplitButton)
			{
				dict.Add("Component.ButtonWidth", (component as System.Windows.Forms.ToolStripSplitButton).ButtonBounds.Width);
				dict.Add("Component.DropDownButtonWidth", (component as System.Windows.Forms.ToolStripSplitButton).DropDownButtonBounds.Width);
			}
			*/

			if (action is RectangleRenderingAction)
			{
				RectangleRenderingAction act = (action as RectangleRenderingAction);

				double x = act.X.Evaluate(dict) + bounds.X;
				double y = act.Y.Evaluate(dict) + bounds.Y;
				double w = act.Width.Evaluate(dict);
				double h = act.Height.Evaluate(dict);

				if (act.Fill != null)
				{
					FillRectangle(BrushFromFill(act.Fill, new System.Drawing.RectangleF(x, y, w, h)), x, y, w, h);
				}
				if (act.Outline != null)
				{
					if (act.Outline is SolidOutline)
					{
						DrawRectangle(PenFromOutline(act.Outline), x, y, w - 1, h - 1);
					}
					else if (act.Outline is ThreeDOutline)
					{
						ThreeDOutline threed = (act.Outline as ThreeDOutline);

						Color lightColor = Color.Empty;
						Color darkColor = Color.Empty;

						switch (threed.Type)
						{
							case ThreeDOutlineType.Inset:
							{
								lightColor = Color.FromString(threed.DarkColor);
								darkColor = Color.FromString(threed.LightColor);
								break;
							}
							case ThreeDOutlineType.Outset:
							{
								lightColor = Color.FromString(threed.LightColor);
								darkColor = Color.FromString(threed.DarkColor);
								break;
							}
						}

						System.Drawing.Pen lightPen = new System.Drawing.Pen(lightColor, act.Outline.Width);
						System.Drawing.Pen darkPen = new System.Drawing.Pen(darkColor, act.Outline.Width);

						DrawLine(lightPen, x, y, x + w, y);
						DrawLine(lightPen, x, y, x, y + h);
						DrawLine(darkPen, x + w - 1, y, x + w - 1, y + h - 1);
						DrawLine(darkPen, x, y + h - 1, x + w - 1, y + h - 1);
					}
				}
			}
			else if (action is LineRenderingAction)
			{
				LineRenderingAction act = (action as LineRenderingAction);

				double x1 = act.X1.Evaluate(dict) + bounds.X;
				double y1 = act.Y1.Evaluate(dict) + bounds.Y;
				double x2 = act.X2.Evaluate(dict);
				double y2 = act.Y2.Evaluate(dict);

				if (act.Outline != null)
				{
					DrawLine(PenFromOutline(act.Outline), x1, y1, x2, y2);
				}
			}
			else if (action is TextRenderingAction)
			{
				TextRenderingAction act = (action as TextRenderingAction);

				int x = (int)Math.Round(act.X.Evaluate(dict) + bounds.X);
				int y = (int)Math.Round(act.Y.Evaluate(dict) + bounds.Y);
				int width = (int)Math.Round(act.Width.Evaluate(dict));
				int height = (int)Math.Round(act.Height.Evaluate(dict));
				Color color = Color.FromString(act.Color);
				string value = act.Value.ReplaceVariables(dict);

				// graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

				/*
				Font font = SystemFonts.DefaultFont;
				if (act.Font != null)
				{
					font = Font.FromString(act.Font);
				}

				System.Windows.Forms.TextRenderer.DrawText(graphics, value, font, new System.Drawing.Rectangle(x, y, width, height), color, System.Windows.Forms.TextFormatFlags.Left);
				*/
			}
		}

		private Brush BrushFromFill(Fill fill, Rectangle bounds)
		{
			if (fill is SolidFill)
			{
				SolidFill fil = (fill as SolidFill);
				return new SolidBrush(Color.FromString(fil.Color));
			}
			else if (fill is LinearGradientFill)
			{
				LinearGradientFill fil = (fill as LinearGradientFill);

				LinearGradientBrush brush = new LinearGradientBrush(rect, LinearGradientFillOrientationToLinearGradientMode(fil.Orientation));
				if (fil.ColorStops.Count > 0)
				{
					List<System.Drawing.Color> colorList = new List<System.Drawing.Color>();
					List<float> positionList = new List<float>();

					for (int i = 0; i < fil.ColorStops.Count; i++)
					{
						colorList.Add(ColorFromString(fil.ColorStops[i].Color));
						positionList.Add(FloatFromString(fil.ColorStops[i].Position));
					}

					System.Drawing.Drawing2D.ColorBlend blend = new System.Drawing.Drawing2D.ColorBlend(fil.ColorStops.Count);
					blend.Colors = colorList.ToArray();
					blend.Positions = positionList.ToArray();
					brush.InterpolationColors = blend;
				}
				return brush;
			}
			return null;
		}
	}
}
