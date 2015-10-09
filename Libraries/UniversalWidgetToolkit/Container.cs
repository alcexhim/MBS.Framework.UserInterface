using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit
{
	public class Container : Control
	{
		public Container()
		{
			mvarControls = new ControlCollection(this);
		}

		private Control.ControlCollection mvarControls = null;
		public Control.ControlCollection Controls { get { return mvarControls; } }

		private Layout mvarLayout = null;
		/// <summary>
		/// The <see cref="Layout" /> used to arrange <see cref="Control" />s in this <see cref="Container" />.
		/// </summary>
		public Layout Layout { get { return mvarLayout; } set { mvarLayout = value; } }

		public Control HitTest(double x, double y)
		{
			foreach (Control ctl in mvarControls)
			{
				Rectangle rect = mvarLayout.GetControlBounds(ctl);
				if (rect.Contains(x, y)) return ctl;
			}
			return null;
		}
		public Control HitTest(Vector2D point)
		{
			return HitTest(point.X, point.Y);
		}

		public override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);
			foreach (Control ctl in mvarControls)
			{
				Application.Engine.CreateControl(ctl);
			}
		}
	}
}
