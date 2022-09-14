//
//  GraphicsExtensions.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2022 Mike Becker's Software
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
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.ObjectModels.Theming;
using MBS.Framework.UserInterface.ObjectModels.Theming.RenderingActions;

namespace MBS.Framework.UserInterface.Theming
{
	public static class GraphicsExtensions
	{
		/// <summary>
		/// Draws the specified <see cref="ThemeComponent" /> in the state defined by the
		/// <see cref="ThemeComponentReference"/> specified by <paramref name="paramz" />.
		/// </summary>
		/// <param name="component"></param>
		public static void DrawThemeComponent(this Graphics g, ThemeComponentReference paramz, Control component, Dictionary<string, object> variables = null)
		{
			if (variables == null) variables = new Dictionary<string, object>();

			ThemeComponent tc = ThemeManager.CurrentTheme.GetComponent(paramz.ComponentID);
			g.DrawThemeComponent(tc, component, paramz.StateID, variables);
		}

		/// <summary>
		/// Draws the specified <see cref="ThemeComponent" /> in the specified state.
		/// </summary>
		/// <param name="component"></param>
		/// <param name="stateID"></param>
		public static void DrawThemeComponent(this Graphics g, ThemeComponent tc, Control component, Guid stateID, Dictionary<string, object> variables = null)
		{
			if (tc == null) return;
			if (tc.InheritsComponent != null)
			{
				g.DrawThemeComponent(tc.InheritsComponent, component, stateID, variables);
			}

			foreach (ThemeRendering rendering in tc.Renderings)
			{
				if (rendering.States.Count == 0 || rendering.States.Contains(stateID))
				{
					// we can use this rendering
					g.DrawRendering(rendering, component, variables);
				}
			}
		}

		private static void DrawRendering(this Graphics g, ThemeRendering rendering, Control component, Dictionary<string, object> variables)
		{
			foreach (RenderingAction action in rendering.Actions)
			{
				g.DrawRenderingAction(action, component, variables);
			}
		}
		private static Brush BrushFromFill(Fill fill, Rectangle bounds)
		{
			if (fill is SolidFill)
			{
				SolidFill fil = (fill as SolidFill);
				return new SolidBrush(Color.FromString(fil.Color));
			}
			else if (fill is LinearGradientFill)
			{
				LinearGradientFill fil = (fill as LinearGradientFill);

				LinearGradientBrush brush = new LinearGradientBrush(bounds, LinearGradientFillOrientationToLinearGradientBrushOrientation(fil.Orientation));
				if (fil.ColorStops.Count > 0)
				{
					for (int i = 0; i < fil.ColorStops.Count; i++)
					{
						Color color = Color.FromString(fil.ColorStops[i].Color);
						Measurement measurement = Measurement.Parse(fil.ColorStops[i].Position);
						brush.ColorStops.Add(color, measurement);
					}
				}
				return brush;
			}
			return null;
		}


		private static Pen PenFromOutline(Outline outline)
		{
			if (outline is SolidOutline)
			{
				Pen pen = new Pen(Color.FromString((outline as SolidOutline).Color), new Measurement(outline.Width, MeasurementUnit.Pixel));
				return pen;
			}
			else
			{
				Console.WriteLine("uwt-theme: PenFromOutline: outline used to create pen must be SolidOutline");
				return Pens.Black;
			}
		}

		private static LinearGradientBrushOrientation LinearGradientFillOrientationToLinearGradientBrushOrientation(LinearGradientFillOrientation orientation)
		{
			switch (orientation)
			{
				case LinearGradientFillOrientation.BackwardDiagonal: return LinearGradientBrushOrientation.BackwardDiagonal;
				case LinearGradientFillOrientation.ForwardDiagonal: return LinearGradientBrushOrientation.ForwardDiagonal;
				case LinearGradientFillOrientation.Horizontal: return LinearGradientBrushOrientation.Horizontal;
				case LinearGradientFillOrientation.Vertical: return LinearGradientBrushOrientation.Vertical;
			}
			return LinearGradientBrushOrientation.Horizontal;
		}

		private static void DrawRenderingAction(this Graphics g, RenderingAction action, Control component, Dictionary<string, object> variables)
		{
			if (variables == null) variables = new Dictionary<string, object>();
			Dictionary<string, object> dict = new Dictionary<string, object>();

			foreach (KeyValuePair<string, object> kvp in variables)
			{
				dict.Add(kvp.Key, kvp.Value);
			}

			if (component.Parent is IControlContainer)
			{
				Rectangle bounds = (component.Parent as IControlContainer).Layout.GetControlBounds(component);

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
						g.FillRectangle(BrushFromFill(act.Fill, new Rectangle(x, y, w, h)), x, y, w, h);
					}
					if (act.Outline != null)
					{
						if (act.Outline is SolidOutline)
						{
							g.DrawRectangle(PenFromOutline(act.Outline), x, y, w - 1, h - 1);
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

							Pen lightPen = new Pen(lightColor, new Measurement(act.Outline.Width, MeasurementUnit.Pixel));
							Pen darkPen = new Pen(darkColor, new Measurement(act.Outline.Width, MeasurementUnit.Pixel));

							g.DrawLine(lightPen, x, y, x + w, y);
							g.DrawLine(lightPen, x, y, x, y + h);
							g.DrawLine(darkPen, x + w - 1, y, x + w - 1, y + h - 1);
							g.DrawLine(darkPen, x, y + h - 1, x + w - 1, y + h - 1);
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
						g.DrawLine(PenFromOutline(act.Outline), x1, y1, x2, y2);
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

					Font font = SystemFonts.MenuFont;
					/*
					if (act.Font != null)
					{
						font = Font.FromFamily(act.Font, 8);
					}
					*/
					g.DrawText(value, font, new Rectangle(x, y, width, height), new SolidBrush(color), act.HorizontalAlignment, act.VerticalAlignment);
				}
			}
		}

	}
}
