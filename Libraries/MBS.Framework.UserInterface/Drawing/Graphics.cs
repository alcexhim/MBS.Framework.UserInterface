using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.ObjectModels.Theming;
using MBS.Framework.UserInterface.ObjectModels.Theming.RenderingActions;
using MBS.Framework.UserInterface.Theming;

namespace MBS.Framework.UserInterface.Drawing
{
	public abstract class Graphics
	{
		public Rectangle ClipRectangle { get; private set; }

		public Graphics()
		{
		}
		public Graphics(Rectangle clipRectangle)
		{
			ClipRectangle = clipRectangle;
		}

		public static Graphics FromImage(Image image)
		{
			return ((UIApplication)Application.Instance).Engine.CreateGraphics(image);
		}

		protected abstract void DrawImageInternal(Image image, double x, double y, double width, double height);
		public void DrawImage(Image image, double x, double y)
		{
			DrawImage(image, x, y, image.Width, image.Height);
		}
		public void DrawImage(Image image, double x, double y, double width, double height)
		{
			DpiScale(ref x, ref y, ref width, ref height);
			DrawImageInternal(image, x, y, width, height);
		}

		protected abstract void DrawLineInternal(Pen pen, double x1, double y1, double x2, double y2);
		public void DrawLine(Pen pen, double x1, double y1, double x2, double y2)
		{
			DpiScale(ref x1, ref x2, ref y1, ref y2);
			DrawLineInternal(pen, x1, y1, x2, y2);
		}

		private Vector2D DpiScale(Vector2D point)
		{
			if (((UIApplication)Application.Instance).ShouldDpiScale)
			{
				double sf = Screen.Default.PrimaryMonitor.ScaleFactor;
				return new Vector2D(point.X * sf, point.Y * sf);
			}
			return point;
		}
		private Rectangle DpiScale(Rectangle rect)
		{
			if (((UIApplication)Application.Instance).ShouldDpiScale)
			{
				double sf = Screen.Default.PrimaryMonitor.ScaleFactor;
				return new Rectangle(rect.X * sf, rect.Y * sf, rect.Width * sf, rect.Height * sf);
			}
			return rect;
		}
		private void DpiScale(ref double x, ref double y, ref double w, ref double h)
		{
			if (((UIApplication)Application.Instance).ShouldDpiScale)
			{
				double sf = Screen.Default.PrimaryMonitor.ScaleFactor;
				x *= sf;
				y *= sf;
				w *= sf;
				h *= sf;
			}
		}

		protected abstract void DrawFocusInternal(double x, double y, double width, double height, Control styleReference);
		/// <summary>
		/// Renders a focus indicator on the rectangle determined by <paramref name="x" />, <paramref name="y" />,
		/// <paramref name="width" />, <paramref name="height" />.
		/// </summary>
		/// <param name="x">X origin of the rectangle.</param>
		/// <param name="y">Y origin of the rectangle.</param>
		/// <param name="width">Rectangle width.</param>
		/// <param name="height">Rectangle height.</param>
		/// <param name="styleReference">The control used as a reference for the focus rectangle style. For example,
		/// one could pass a TextBox to get a text box style focus rectangle, which looks different on certain themes.</param>
		public void DrawFocus(double x, double y, double width, double height, Control styleReference = null)
		{
			DrawFocusInternal(x, y, width, height, styleReference);
		}

		protected abstract void DrawRectangleInternal(Pen pen, double x, double y, double width, double height);
		public void DrawRectangle(Pen pen, double x, double y, double width, double height)
		{
			Rectangle rect2 = NormalizeRectangle(new Rectangle(x, y, width, height));
			rect2 = DpiScale(rect2);
			DrawRectangleInternal(pen, rect2.X, rect2.Y, rect2.Width, rect2.Height);
		}

		protected abstract void DrawPolygonInternal(Pen pen, Vector2D[] points);
		public void DrawPolygon(Pen pen, Vector2D[] points)
		{
			DrawPolygonInternal(pen, points);
		}

		protected abstract void FillPolygonInternal(Brush brush, Vector2D[] points);
		public void FillPolygon(Brush brush, Vector2D[] points)
		{
			FillPolygonInternal(brush, points);
		}

		public void DrawRectangle(Pen pen, Rectangle rect)
		{
			DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
		}

		protected abstract void FillRectangleInternal(Brush brush, double x, double y, double width, double height);
		public void FillRectangle(Brush brush, double x, double y, double width, double height)
		{
			Rectangle rect2 = NormalizeRectangle(new Rectangle(x, y, width, height));
			rect2 = DpiScale(rect2);
			FillRectangleInternal(brush, rect2.X, rect2.Y, rect2.Width, rect2.Height);
		}
		public void FillRectangle(Brush brush, Rectangle rect)
		{
			FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
		}

		private Rectangle NormalizeRectangle(Rectangle input)
		{
			double x = input.X;
			double y = input.Y;
			double width = input.Width;
			double height = input.Height;

			if (width < 0)
			{
				x = x + width;
				width = -width;
			}
			if (height < 0)
			{
				y = y + height;
				height = -height;
			}
			return new Rectangle(x, y, width, height);
		}

		public void Clear(Color color)
		{
			if (ClipRectangle != null)
			{
				FillRectangle(new SolidBrush(color), new Rectangle(0, 0, ClipRectangle.Width, ClipRectangle.Height));
			}
		}

		/// <summary>
		/// Draws the specified <see cref="ThemeComponent" /> in the specified state.
		/// </summary>
		/// <param name="componentID"></param>
		/// <param name="stateID"></param>
		public void DrawThemeComponent(ThemeComponentReference paramz, Control component, Dictionary<string, object> variables = null)
		{
			if (variables == null) variables = new Dictionary<string, object>();

			ThemeComponent tc = ThemeManager.CurrentTheme.GetComponent(paramz.ComponentID);
			DrawThemeComponent(tc, component, paramz.StateID, variables);
		}
		public void DrawThemeComponent(ThemeComponent tc, Control component, Guid stateID, Dictionary<string, object> variables = null)
		{
			if (tc == null) return;
			if (tc.InheritsComponent != null)
			{
				DrawThemeComponent(tc.InheritsComponent, component, stateID, variables);
			}

			foreach (ThemeRendering rendering in tc.Renderings)
			{
				if (rendering.States.Count == 0 || rendering.States.Contains(stateID))
				{
					// we can use this rendering
					DrawRendering(rendering, component, variables);
				}
			}
		}

		private void DrawRendering(ThemeRendering rendering, Control component, Dictionary<string, object> variables)
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
						FillRectangle(BrushFromFill(act.Fill, new Rectangle(x, y, w, h)), x, y, w, h);
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

							Pen lightPen = new Pen(lightColor, new Measurement(act.Outline.Width, MeasurementUnit.Pixel));
							Pen darkPen = new Pen(darkColor, new Measurement(act.Outline.Width, MeasurementUnit.Pixel));

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

					Font font = SystemFonts.MenuFont;
					/*
					if (act.Font != null)
					{
						font = Font.FromFamily(act.Font, 8);
					}
					*/
					DrawText(value, font, new Rectangle(x, y, width, height), new SolidBrush(color), act.HorizontalAlignment, act.VerticalAlignment);
				}
			}
		}

		protected abstract void DrawTextInternal(string value, Font font, Vector2D location, Dimension2D size, Brush brush, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment);
		public void DrawText(string value, Font font, Vector2D location, Brush brush, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, VerticalAlignment verticalAlignment = VerticalAlignment.Top)
		{
			location = DpiScale(location);
			DrawTextInternal(value, font, location, null, brush, horizontalAlignment, verticalAlignment);
		}
		public void DrawText(string value, Font font, Rectangle rectangle, Brush brush, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, VerticalAlignment verticalAlignment = VerticalAlignment.Top)
		{
			rectangle = DpiScale(rectangle);
			DrawTextInternal(value, font, rectangle.Location, rectangle.Size, brush, horizontalAlignment, verticalAlignment);
		}

		private Pen PenFromOutline(Outline outline)
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

		private LinearGradientBrushOrientation LinearGradientFillOrientationToLinearGradientBrushOrientation(LinearGradientFillOrientation orientation)
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
	}
}
