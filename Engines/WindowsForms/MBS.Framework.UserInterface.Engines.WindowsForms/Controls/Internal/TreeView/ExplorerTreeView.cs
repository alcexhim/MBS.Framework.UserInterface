using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls.Internal.TreeView
{
	internal class ExplorerTreeView : System.Windows.Forms.TreeView
	{
		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		private extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName,
											string pszSubIdList);

		public ExplorerTreeView()
		{
			HotTracking = true;
			HideSelection = false;
			this.DrawMode = TreeViewDrawMode.OwnerDrawAll;
		}

		protected override void CreateHandle()
		{
			base.CreateHandle();

			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				if (Environment.OSVersion.Version.Major >= 6)
				{
					SetWindowTheme(this.Handle, "explorer", null);
				}
			}

			base.SetStyle(ControlStyles.UserPaint, true);
		}


		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);

			Theming.Theme.CurrentTheme.DrawListViewBackground(e.Graphics, new System.Drawing.Rectangle(0, 0, Width, Height));
			for (int i = 0; i < Nodes.Count; i++)
			{
				DrawNodeRecursive(e.Graphics, Nodes[i], (Nodes[i] == currentHotHit) ? TreeNodeStates.Hot : TreeNodeStates.Default);
			}
			lastHotHit = currentHotHit;
		}
		private void DrawNodeRecursive(System.Drawing.Graphics g, TreeNode node, TreeNodeStates state)
		{
			OnDrawNode(new DrawTreeNodeEventArgs(g, node, node.Bounds, state));
			if (node.IsExpanded)
			{
				for (int i = 0; i < node.Nodes.Count; i++)
				{
					DrawNodeRecursive(g, node.Nodes[i], (node.Nodes[i] == currentHotHit) ? TreeNodeStates.Hot : TreeNodeStates.Default);
				}
			}
		}

		private TreeNode lastHotHit = null;
		private TreeNode currentHotHit = null;

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			TreeViewHitTestInfo tvh = HitTest(e.Location);
			if (lastHotHit != tvh.Node)
			{
				currentHotHit = tvh.Node;
			}

			if (lastHotHit != null)
				Invalidate(lastHotHit.Bounds);
			if (currentHotHit != null)
				Invalidate(currentHotHit.Bounds);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			System.Drawing.Rectangle? bounds1 = lastHotHit?.Bounds;
			lastHotHit = null;
			System.Drawing.Rectangle? bounds2 = currentHotHit?.Bounds;
			currentHotHit = null;

			if (bounds1 != null)
				Invalidate(bounds1.Value);
			if (bounds2 != null)
				Invalidate(bounds2.Value);

			Invalidate();
		}

		protected override void OnDrawNode(DrawTreeNodeEventArgs e)
		{
			if (e.Node != null)
			{
				if (e.Bounds == System.Drawing.Rectangle.Empty)
					return;

				UserInterface.Theming.ControlState state = (e.State == TreeNodeStates.Hot ? UserInterface.Theming.ControlState.Hover : UserInterface.Theming.ControlState.Normal);
				if (e.Node.Nodes.Count > 0)
				{
					Theming.Theme.CurrentTheme.DrawListViewTreeGlyph(e.Graphics, new System.Drawing.Rectangle(e.Bounds.X - 16, e.Bounds.Y, 16, 16), state, e.Node.IsExpanded);
				}
				Theming.Theme.CurrentTheme.DrawListItemBackground(e.Graphics, new System.Drawing.Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1), state, e.Node == SelectedNode, Focused);

				TextRenderer.DrawText(e.Graphics, e.Node.Text, Font, e.Bounds.Location, ForeColor);
			}
		}
	}
}
