//
//  ProgressDialog.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2021 Mike Becker's Software
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
using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface.Dialogs
{
	[ContainerLayout("~/Dialogs/ProgressDialog.glade")]
	public class ProgressDialog : Window
	{
		private Label lblStatus1;
		private Label lblStatus2;
		private ProgressBar pb;
		private Label lblTimeElapsed;
		private Label lblTimeElapsedLabel;
		private Label lblTimeRemaining;
		private Label lblTimeRemainingLabel;
		private Button cmdPause;
		private Button cmdCancel;

		private DateTime _shownTime = DateTime.Now;

		public event EventHandler ThreadStart;

		public bool AutoClose { get; set; } = true;
		private bool _EnablePause = false;
		public bool EnablePause
		{
			get
			{
				lock (this)
				{
					if (cmdPause != null && cmdPause.IsCreated)
					{
						return cmdPause.Enabled;
					}
				}
				return _EnablePause;
			}
			set
			{
				lock (this)
				{
					if (cmdPause != null && cmdPause.IsCreated)
					{
						cmdPause.Enabled = value;
					}
				}
				_EnablePause = value;
			}
		}
		public bool Paused { get; private set; } = false;

		protected virtual void OnThreadStart(EventArgs e)
		{
			ThreadStart?.Invoke(this, e);
		}

		private void _t_ThreadStart()
		{
			lock (this)
			{
				_shownTime = DateTime.Now;
			}
			OnThreadStart(EventArgs.Empty);

			if (AutoClose)
			{
				Close();
			}
		}

		private System.Threading.Thread _t = null;

		[EventHandler(nameof(cmdPause), nameof(Button.Click))]
		private void cmdPause_Click(object sender, EventArgs e)
		{
			lock (this)
			{
				Paused = !Paused;
				if (Paused)
				{
					cmdPause.Text = "Resume";
				}
				else
				{
					cmdPause.Text = "Pause";
				}
			}
		}

		protected internal override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);

			cmdPause.Enabled = _EnablePause;
		}

		protected internal override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			_t = new System.Threading.Thread(_t_ThreadStart);
			_t.Start();
		}

		public void Update(string statusLine1, string statusLine2, double progressValue, double progressMinimum = 0, double progressMaximum = 100)
		{
			lock (this)
			{
				lblTimeRemainingLabel.Visible = false;
				lblTimeRemaining.Visible = false;

				lblStatus1.Text = statusLine1;
				lblStatus2.Text = statusLine2;

				pb.Minimum = progressMinimum;
				pb.Maximum = progressMaximum;
				pb.Value = progressValue;

				lblTimeElapsed.Text = (DateTime.Now - _shownTime).ToString();
			}
		}
	}
}
