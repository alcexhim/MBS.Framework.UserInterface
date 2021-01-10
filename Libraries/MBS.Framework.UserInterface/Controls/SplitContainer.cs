using System;

namespace MBS.Framework.UserInterface.Controls
{
	namespace Native
	{
		public interface ISplitContainerImplementation
		{
			int GetSplitterPosition();
			void SetSplitterPosition(int value);
		}
	}
	public class SplitContainer : SystemControl, IVirtualControlContainer
	{
		private int _OldSplitterPosition = 0;

		public Control[] GetAllControls()
		{
			Control[] ctls1 = Panel1.GetAllControls();
			Control[] ctls2 = Panel2.GetAllControls();

			Control[] ctls = new Control[ctls1.Length + ctls2.Length];
			Array.Copy(ctls1, 0, ctls, 0, ctls1.Length);
			Array.Copy(ctls2, 0, ctls, ctls1.Length, ctls2.Length);
			return ctls;
		}

		protected internal override void OnResizing(ResizingEventArgs e)
		{
			base.OnResizing(e);
		}
		public class SplitContainerPanel : Container
		{
			public new SplitContainer Parent { get; private set; } = null;

			private bool _Expanded = true;
			public bool Expanded
			{
				get { return _Expanded; }
				set
				{
					bool changed = (_Expanded != value);
					_Expanded = value;
					if (changed)
					{
						if (_Expanded)
						{
							Parent.mvarSplitterPosition = Parent._OldSplitterPosition;
						}
						else
						{
							Parent._OldSplitterPosition = Parent.SplitterPosition;
							if (this == Parent.Panel1)
							{
								Parent.mvarSplitterPosition = 0;
							}
							else if (this == Parent.Panel2)
							{
								if (Parent.Orientation == Orientation.Horizontal)
								{
									Parent.mvarSplitterPosition = (int)Parent.Size.Height;
								}
								else
								{
									Parent.mvarSplitterPosition = (int)Parent.Size.Width;
								}
							}
						}
						(Parent.ControlImplementation as Native.ISplitContainerImplementation)?.SetSplitterPosition(Parent.mvarSplitterPosition);
					}
				}
			}

			public SplitContainerPanel(SplitContainer parent)
			{
				Parent = parent;
			}
		}

		/// <summary>
		/// Gets the primary <see cref="SplitContainerPanel" /> for this <see cref="SplitContainer"/>. When <see cref="Orientation" /> is set to
		/// <see cref="Orientation.Vertical" />, this is the left panel; when <see cref="Orientation" /> is set to <see cref="Orientation.Horizontal" />,
		/// this is the top panel.
		/// </summary>
		/// <value>The primary <see cref="SplitContainerPanel" /> for this <see cref="SplitContainer" />.</value>
		public SplitContainerPanel Panel1 { get; private set; } = null;
		/// <summary>
		/// Gets the secondary <see cref="SplitContainerPanel" /> for this <see cref="SplitContainer"/>. When <see cref="Orientation" /> is set to
		/// <see cref="Orientation.Vertical" />, this is the right panel; when <see cref="Orientation" /> is set to <see cref="Orientation.Horizontal" />,
		/// this is the bottom panel.
		/// </summary>
		/// <value>The secondary <see cref="SplitContainerPanel" /> for this <see cref="SplitContainer" />.</value>
		public SplitContainerPanel Panel2 { get; private set; } = null;

		private Orientation mvarOrientation = Orientation.Horizontal;
		/// <summary>
		/// The orientation of the splitter in the SplitContainer. When vertical, panels are on the left and right; when
		/// horizontal, panels are on the top and bottom.
		/// </summary>
		/// <value>The orientation of the splitter in this <see cref="SplitContainer" />.</value>
		public Orientation Orientation { get { return mvarOrientation; } set { mvarOrientation = value; } }

		private int mvarSplitterPosition = 0;
		public int SplitterPosition
		{
			get
			{
				if (IsCreated)
				{
					Native.ISplitContainerImplementation impl = (ControlImplementation as Native.ISplitContainerImplementation);
					if (impl != null)
					{
						mvarSplitterPosition = impl.GetSplitterPosition();
					}
				}
				return mvarSplitterPosition;
			}
			set
			{
				_OldSplitterPosition = value;
				(ControlImplementation as Native.ISplitContainerImplementation)?.SetSplitterPosition(value);
				mvarSplitterPosition = value;
			}
		}

		public SplitContainer(Orientation orientation = Orientation.Horizontal)
		{
			mvarOrientation = orientation;
			Panel1 = new SplitContainerPanel(this);
			Panel2 = new SplitContainerPanel(this);
		}
	}
}

