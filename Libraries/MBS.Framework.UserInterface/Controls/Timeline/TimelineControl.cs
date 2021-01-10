using System;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Dragging;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface.Controls.Timeline
{
	public class TimelineControl : CustomControl
	{
		/// <summary>
		/// Timeline groups (e.g. layers, channels, etc.)
		/// </summary>
		/// <value></value>
		public TimelineGroup.TimelineGroupCollection Groups { get; } = new TimelineGroup.TimelineGroupCollection();

		private int _CurrentFrame = 0;
		public int CurrentFrame
		{
			get { return _CurrentFrame; }
			set
			{
				_CurrentFrame = value;

				Rectangle rectCursor = new Rectangle(GroupWidth + (value * FrameWidth) - FrameWidth - FrameWidth, 0, FrameWidth * 4, Size.Height);
				Invalidate(rectCursor);
			}
		}

		private int _GroupWidth = 128;
		public int GroupWidth { get { return _GroupWidth; } set { dm.DragLimitBounds = new Rectangle(_GroupWidth, -1, -1, -1); _GroupWidth = value; } }

		public int FrameWidth { get; set; } = 16;
		public int FrameHeight { get; set; } = 32;

		public event EventHandler<TimelineObjectActivatedEventArgs> ObjectActivated;
		protected virtual void OnObjectActivated(TimelineObjectActivatedEventArgs e)
		{
			ObjectActivated?.Invoke(this, e);
		}

		private DragManager dm = new DragManager();
		public TimelineControl()
		{
			dm.Register(this);
			dm.BeforeControlPaint += dm_BeforeControlPaint;
			dm.DragLimitBounds = new Rectangle(_GroupWidth, -1, -1, -1);
		}

		public TimelineGroup.TimelineGroupCollection SelectedGroups { get; } = new TimelineGroup.TimelineGroupCollection();
		public TimelineObject.TimelineObjectCollection SelectedObjects { get; } = new TimelineObject.TimelineObjectCollection(null);

		private DragOperation dragOperation = DragOperation.None;
		private int resizeMargin = 16;
		private double _OrigX = 0;
		protected internal override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Focus();

			TimelineItem item = HitTest(e.Location);
			dm.DrawSelection = (!(item is TimelineObject));

			if (e.ModifierKeys == Input.Keyboard.KeyboardModifierKey.None)
			{
				SelectedObjects.Clear();
			}
			if (item is TimelineGroup)
			{
				if (e.ModifierKeys == Input.Keyboard.KeyboardModifierKey.None)
				{
					SelectedGroups.Clear();
				}
				SelectedGroups.Add(item as TimelineGroup);
			}
			else if (item is TimelineObject)
			{
				SelectedGroups.Clear();
				SelectedGroups.Add((item as TimelineObject).Parent);

				double initX1 = FrameToPixel((item as TimelineObject).StartFrame);
				double initX2 = FrameToPixel((item as TimelineObject).EndFrame);

				dm.ObjectInitialX = initX1;
				SelectedObjects.Add(item as TimelineObject);
			}
		}
		protected internal override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (e.Buttons == MouseButtons.Primary)
			{
				for (int i = 0; i < SelectedObjects.Count; i++)
				{
					int c = SelectedObjects[i].Length;
					int startFrame = (int)(PixelToFrame(dm.ObjectDeltaX));
					int endFrame = (int)(PixelToFrame(dm.CurrentX));
					Console.WriteLine("startFrame = {0}", startFrame);

					int s = SelectedObjects[i].StartFrame;
					if (startFrame < 0)
						startFrame = 0;
					if (dragOperation == DragOperation.Move)
					{
						SelectedObjects[i].StartFrame = startFrame;
						SelectedObjects[i].Length = c;
					}
					else if (dragOperation == DragOperation.ResizeHorizontalEnd)
					{
						SelectedObjects[i].StartFrame = s;
						SelectedObjects[i].EndFrame = endFrame;
					}
					else if (dragOperation == DragOperation.ResizeHorizontalStart)
					{
						SelectedObjects[i].StartFrame = startFrame;
					}
				}
			}
			else
			{
				TimelineItem item = HitTest(e.Location);
				if (item is TimelineObject)
				{
					double initX1 = FrameToPixel((item as TimelineObject).StartFrame);
					double initX2 = FrameToPixel((item as TimelineObject).EndFrame);

					if (e.X >= initX1 && e.X <= initX1 + resizeMargin)
					{
						dragOperation = DragOperation.ResizeHorizontalStart;
					}
					else if (e.X >= initX2 - resizeMargin && e.X <= initX2)
					{
						dragOperation = DragOperation.ResizeHorizontalEnd;
					}
					else
					{
						dragOperation = DragOperation.Move;
					}

					switch (dragOperation)
					{
						case DragOperation.Move:
						{
							Cursor = Cursors.Move;
							break;
						}
						case DragOperation.ResizeHorizontalEnd:
						{
							Cursor = Cursors.ResizeE;
							break;
						}
						case DragOperation.ResizeHorizontalStart:
						{
							Cursor = Cursors.ResizeW;
							break;
						}
					}
				}
				else
				{
					Cursor = Cursors.Default;
				}
			}
		}

		private int PixelToFrame(double x)
		{
			return (int)((x - GroupWidth) / FrameWidth);
		}
		private double FrameToPixel(int frame)
		{
			return GroupWidth + (FrameWidth * frame);
		}

		protected internal override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			dm.DrawSelection = true;
			dragOperation = DragOperation.None;
		}
		protected internal override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			TimelineItem item = HitTest(e.Location);
			if (item is TimelineGroup)
			{
				if (e.X <= GroupWidth)
				{
					(item as TimelineGroup).Expanded = !(item as TimelineGroup).Expanded;
					Refresh();
				}
			}
			else if (item is TimelineObject)
			{
				OnObjectActivated(new TimelineObjectActivatedEventArgs(item as TimelineObject));
			}
		}

		public TimelineItem HitTest(Vector2D pt)
		{
			Rectangle rect = new Rectangle();
			return HitTest(pt, null, ref rect);
		}

		private TimelineItem HitTest(Vector2D pt, TimelineGroup group, ref Rectangle rectGroup)
		{
			TimelineGroup.TimelineGroupCollection gc = Groups;
			if (group != null)
			{
				gc = group.Groups;
			}

			TimelineGroup tg = HitTestGroup(pt, group, ref rectGroup);
			if (tg != null)
			{
				foreach (TimelineObject obj in tg.Objects)
				{
					double x1 = GroupWidth + (obj.StartFrame * FrameWidth);
					double x2 = GroupWidth + (obj.EndFrame * FrameWidth);

					if (pt.X >= x1 && pt.X <= x2)
					{
						return obj;
					}
				}
				return tg;
			}
			return null;
		}

		private TimelineGroup HitTestGroup(Vector2D pt, TimelineGroup group, ref Rectangle rectGroup)
		{
			if (group == null)
				rectGroup = new Rectangle(0, 0, Size.Width, FrameHeight);

			TimelineGroup.TimelineGroupCollection gc = Groups;
			if (group != null)
			{
				gc = group.Groups;
			}
			foreach (TimelineGroup g in gc)
			{
				rectGroup.Height = g.Height.GetValueOrDefault(FrameHeight);
				if (pt.Y >= rectGroup.Y && pt.Y <= rectGroup.Bottom)
				{
					return g;
				}
				else if (g.Expanded)
				{
					rectGroup.Y += rectGroup.Height;

					TimelineItem obj = HitTestGroup(pt, g, ref rectGroup);
					if (obj != null)
					{
						if (obj is TimelineGroup)
						{
							return obj as TimelineGroup;
						}
						else
						{
							return null;
						}
					}

					rectGroup.Y -= rectGroup.Height;
				}
				rectGroup.Y += rectGroup.Height;
			}
			return null;
		}

		private void dm_BeforeControlPaint(object sender, PaintEventArgs e)
		{
			for (int x = GroupWidth; x < Size.Width; x += FrameWidth)
			{
				e.Graphics.DrawLine(new Pen(SystemColors.WindowForeground.Alpha(0.2)), x, 0, x, Size.Height);
			}

			Rectangle rectGroup = new Rectangle(0, 0, GroupWidth, FrameHeight);
			foreach (TimelineGroup group in Groups)
			{
				RenderGroup(e, group, ref rectGroup);
			}

			// draw the cursor
			e.Graphics.DrawLine(new Pen(SystemColors.HighlightBackground), GroupWidth + (CurrentFrame * FrameWidth), 0, GroupWidth + (CurrentFrame * FrameWidth), Size.Height);
		}

		private void RenderGroup(PaintEventArgs e, TimelineGroup group, ref Rectangle rectGroup)
		{
			rectGroup.Height = group.Height.GetValueOrDefault(FrameHeight);

			Rectangle rectGroupText = rectGroup;
			rectGroupText.X += 16;
			rectGroupText.Y += 16;

			if (SelectedGroups.Contains(group))
			{
				e.Graphics.FillRectangle(new SolidBrush(SystemColors.HighlightBackground), rectGroup);
				e.Graphics.DrawText(group.Title, null, rectGroupText, new SolidBrush(SystemColors.HighlightForeground));
			}
			else
			{
				e.Graphics.FillRectangle(new SolidBrush(SystemColors.WindowBackground.Darken(0.8).Alpha(0.8)), rectGroup);
				e.Graphics.DrawText(group.Title, null, rectGroupText, new SolidBrush(SystemColors.WindowForeground));
			}

			// render group objects
			Rectangle rectObjects = new Rectangle(rectGroup.Width, rectGroup.Y, Size.Width - rectGroup.Width, rectGroup.Height);
			foreach (TimelineObject obj in group.Objects)
			{
				Rectangle rectObject = rectObjects;
				rectObject.X = rectGroup.Right + (obj.StartFrame * FrameWidth);
				rectObject.Width = (obj.Length * FrameWidth);

				RenderTimelineObject(e.Graphics, obj, rectObject);

				if (SelectedObjects.Contains(obj))
				{
					// e.Graphics.FillRectangle(new SolidBrush(SystemColors.HighlightBackground.Alpha(0.2)), rectObject);
					e.Graphics.DrawRectangle(new Pen(SystemColors.HighlightBackground/*, new Measurement(2, MeasurementUnit.Pixel)*/), rectObject);
				}
			}

			rectGroup.X += 32;
			rectGroup.Width -= 32;
			rectGroup.Y += rectGroup.Height;
			if (group.Expanded)
			{
				for (int i = 0; i < group.Groups.Count; i++)
				{
					RenderGroup(e, group.Groups[i], ref rectGroup);
				}
			}
			rectGroup.Width += 32;
			rectGroup.X -= 32;
		}

		protected virtual void RenderTimelineObject(Graphics g, TimelineObject obj, Rectangle rect)
		{
			g.FillRectangle(new SolidBrush(Colors.LightSteelBlue), rect);
			g.DrawRectangle(new Pen(Colors.SteelBlue), rect);
		}
	}
}
