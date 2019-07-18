using System;
namespace UniversalWidgetToolkit.Engines.GTK.Internal.GTK
{
	internal static class Structures
	{
		public struct GtkTreeIter
		{
			public int stamp;
			public IntPtr user_data;
			public IntPtr user_data2;
			public IntPtr user_data3;

			public static readonly GtkTreeIter Empty = new GtkTreeIter();
		}
		public struct GtkTargetEntry
		{
			public string target;
			public Constants.GtkTargetFlags flags;
			public uint info;
		}
		public struct GtkFileFilterInfo
		{
			public Constants.GtkFileFilterFlags contains;

			public string filename;
			public string uri;
			public string display_name;
			public string mime_type;
		}
	}
}
