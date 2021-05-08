using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Internal.Windows.UWP
{
	internal static class Methods
	{
		private const string LIBRARY_FILENAME_COMBASE = "combase";

		/// <summary>
		/// Gets the activation factory for the specified runtime class.
		/// </summary>
		/// <param name="activatableClassId">The ID of the activatable class.</param>
		/// <param name="iid">The reference ID of the interface.</param>
		/// <param name="factory">The activation factory.</param>
		/// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
		[DllImport(LIBRARY_FILENAME_COMBASE)]
		public static extern int /*HRESULT*/ RoGetActivationFactory(string activatableClassId, int /*REFIID*/ iid, IntPtr /* void** */ factory);
	}
}
