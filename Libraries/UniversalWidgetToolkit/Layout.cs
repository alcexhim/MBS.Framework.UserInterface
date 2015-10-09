using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit
{
	public abstract class Layout
	{
		private bool mvarIgnoreControlPadding = false;
		/// <summary>
		/// Determines if <see cref="Control" />-specific <see cref="Padding" /> values should be ignored. Useful for
		/// developing a truly absolute <see cref="Layouts.AbsoluteLayout" />.
		/// </summary>
		public bool IgnoreControlPadding { get { return mvarIgnoreControlPadding; } set { mvarIgnoreControlPadding = value; } }

		private Dictionary<Control, Dimension2D> mvarMinimumSizes = new Dictionary<Control, Dimension2D>();
		public void SetControlMinimumSize(Control ctl, Dimension2D minimumSize)
		{
			mvarMinimumSizes[ctl] = minimumSize;
		}

		protected abstract Rectangle GetControlBoundsInternal(Control ctl);
		public Rectangle GetControlBounds(Control ctl)
		{
			if (ctl is Window) return (ctl as Window).Bounds;

			Rectangle rect = GetControlBoundsInternal(ctl);
			if (ctl.Parent != null)
			{
				rect.X += ctl.Parent.Padding.Left;
				rect.Y += ctl.Parent.Padding.Top;
			}

			if (!mvarIgnoreControlPadding)
			{
				rect.Width += (ctl.Padding.Left + ctl.Padding.Right);
				rect.Height += (ctl.Padding.Top + ctl.Padding.Bottom);
			}

			if (mvarMinimumSizes.ContainsKey(ctl))
			{
				if (rect.Width < mvarMinimumSizes[ctl].Width) rect.Width = mvarMinimumSizes[ctl].Width;
				if (rect.Height < mvarMinimumSizes[ctl].Height) rect.Height = mvarMinimumSizes[ctl].Height;
			}
			return rect;
		}

		protected abstract void ResetControlBoundsInternal(Control ctl = null);
		public void ResetControlBounds(Control ctl = null)
		{
			ResetControlBoundsInternal(ctl);
		}
	}
}
