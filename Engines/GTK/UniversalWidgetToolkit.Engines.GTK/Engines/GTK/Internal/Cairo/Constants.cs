using System;
namespace UniversalWidgetToolkit.Engines.GTK.Internal.Cairo
{
	internal static class Constants
	{
		public enum CairoStatus
		{
			Unknown = -1,
			Success = 0,

			NoMemory,
			InvalidRestore,
			InvalidPopGroup,
			NoCurrentPoint,
			InvalidMatrix,
			InvalidStatus,
			NullPointer,
			InvalidString,
			InvalidPathData,
			ReadError,
			WriteError,
			SurfaceFinished,
			SurfaceTypeMismatch,
			PatternTypeMismatch,
			InvalidContent,
			InvalidFormat,
			InvalidVisual,
			FileNotFound,
			InvalidDash,
			InvalidDSCComment,
			InvalidIndex,
			ClipNotRepresentable,
			TempFileError,
			InvalidStride,
			FontTypeMismatch,
			UserFontImmutable,
			UserFontError,
			NegativeCount,
			InvalidClusters,
			InvalidSlant,
			InvalidWeight,
			InvalidSize,
			UserFontNotImplemented,
			DeviceTypeMismatch,
			DeviceError,
			InvalidMeshConstruction,
			DeviceFinished,
			JBIG2GlobalMissing,
			PNGError,
			FreeTypeError,
			Win32GDIError,
			TagError,

			LastStatus
		}
		public enum CairoFontSlant
		{
			Normal,
			Italic,
			Oblique
		}
		public enum CairoFontWeight
		{
			Normal,
			Bold
		}
	}
}
