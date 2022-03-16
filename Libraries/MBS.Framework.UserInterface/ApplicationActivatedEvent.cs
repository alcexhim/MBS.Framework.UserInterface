//
//  ApplicationActivatedEvent.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
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

namespace MBS.Framework.UserInterface
{
	public class ApplicationActivatedEventArgs : EventArgs
	{
		public bool FirstRun { get; } = true;
		public CommandLine CommandLine { get; } = null;
		public int ExitCode { get; set; } = 0;
		public ApplicationActivationType ActivationType { get; } = ApplicationActivationType.Launch;

		public ApplicationActivatedEventArgs(bool firstRun, ApplicationActivationType activationType, CommandLine commandLine)
		{
			FirstRun = firstRun;
			ActivationType = activationType;
			CommandLine = commandLine;
		}
	}
	public delegate void ApplicationActivatedEventHandler(object sender, ApplicationActivatedEventArgs e);
}
