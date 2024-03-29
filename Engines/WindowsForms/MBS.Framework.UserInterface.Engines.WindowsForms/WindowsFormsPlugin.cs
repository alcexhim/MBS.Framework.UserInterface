//
//  WindowsFormsPlugin.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2020 Mike Becker
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
namespace MBS.Framework.UserInterface.Engines.WindowsForms
{
	public class WindowsFormsPlugin : EnginePlugin
	{
		public override Type EngineType => typeof(WindowsFormsEngine);

		public override string Title => "Windows Forms";

		public WindowsFormsPlugin()
		{
			ProvidedFeatures.Add(KnownFeatures.UWTPlatform);
		}

		protected override bool IsSupportedInternal()
		{
			// we do not support WinForms on linux coexisting with GTK, because it uses GTK2 and the GTKEngine uses GTK3
			return true; // return Environment.OSVersion.Platform == PlatformID.Win32NT;
		}
	}
}
