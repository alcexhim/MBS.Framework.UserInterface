using System;
using MBS.Framework.Drawing;

namespace MBS.Framework.UserInterface
{
	public partial class Container : Control, IControlContainer
	{
		public Container()
		{
			mvarControls = new ControlCollection(this);
		}
		public Container(Layout layout) : this()
		{
			Layout = layout;
		}

		protected internal override void OnCreating(EventArgs e)
		{
			base.OnCreating(e);

			object[] atts = this.GetType().GetCustomAttributes(typeof(ContainerLayoutAttribute), false);
			if (atts.Length > 0)
			{
				// there can be only one ContainerLayoutAttribute applied to a Container
				ContainerLayoutAttribute wla = (atts[0] as ContainerLayoutAttribute);
				ContainerLayoutLoader cll = new ContainerLayoutLoader(this);
				cll.InitContainerLayout(wla);
			}
		}

		public virtual Control[] GetAllControls()
		{
			System.Collections.Generic.List<Control> list = new System.Collections.Generic.List<Control>();
			foreach (Control ctl in this.Controls)
			{
				if (ctl is IVirtualControlContainer)
				{
					Control[] childControls = ((IVirtualControlContainer)ctl).GetAllControls();
					foreach (Control ctlChild in childControls)
					{
						list.Add(ctlChild);
					}
				}
				list.Add(ctl);
			}
			return list.ToArray();
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

		public T GetControlByID<T>(string ID) where T : Control
		{
			return (GetControlByID(ID) as T);
		}
		public Control GetControlByID(string ID, bool recurse = true)
		{
			Control[] ctls = this.GetAllControls();
			foreach (Control ctl in ctls)
			{
				if (ctl.Name == ID) return ctl;
				/*
				if (recurse)
				{
					if (ctl is Container)
					{
						Control ctl2 = (ctl as Container).GetControlByID(ID, recurse);
						if (ctl2 != null) return ctl2;
					}
					else if (ctl is TabContainer)
					{
						TabContainer tbs = (ctl as TabContainer);
						foreach (TabPage page in tbs.TabPages)
						{
							Control ctl2 = (page as Container).GetControlByID(ID, recurse);
							if (ctl2 != null) return ctl2;
						}
					}
				}
				*/
			}
			return null;
		}
	}
}
