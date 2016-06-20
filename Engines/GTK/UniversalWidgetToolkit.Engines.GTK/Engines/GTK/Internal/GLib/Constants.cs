using System;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.GLib
{
	internal class Constants
	{
		public class GVariantType
		{

			static GVariantType ()
			{
				_Boolean = Methods.g_variant_type_new("b");
				_Byte = Methods.g_variant_type_new("y");
				_Int16 = Methods.g_variant_type_new("n");
				_UInt16 = Methods.g_variant_type_new("q");
				_Int32 = Methods.g_variant_type_new("i");
				_UInt32 = Methods.g_variant_type_new("u");
				_Int64 = Methods.g_variant_type_new("x");
				_UInt64 = Methods.g_variant_type_new("t");
				_Double = Methods.g_variant_type_new("d");
				_String = Methods.g_variant_type_new("s");
				_ObjectPath = Methods.g_variant_type_new("o");
				_Signature = Methods.g_variant_type_new("g");
				_Variant = Methods.g_variant_type_new("v");
				_Handle = Methods.g_variant_type_new("h");
				_Unit = Methods.g_variant_type_new("()");
				_Any = Methods.g_variant_type_new("*");
				_Basic = Methods.g_variant_type_new("?");
				_Maybe = Methods.g_variant_type_new("m*");
				_Array = Methods.g_variant_type_new("a*");
				_Tuple = Methods.g_variant_type_new("r");
				_DictEntry = Methods.g_variant_type_new("{?*}");
				_Dictionary = Methods.g_variant_type_new("a{?*}");
				_StringArray = Methods.g_variant_type_new("as");
				_ObjectPathArray = Methods.g_variant_type_new("ao");
				_ByteString = Methods.g_variant_type_new("ay");
				_ByteStringArray = Methods.g_variant_type_new("aay");
				_VarDict = Methods.g_variant_type_new("a{sv}");
			}

			private static IntPtr /*GVariantType*/ _Boolean = IntPtr.Zero;
			/// <summary>
			/// The type of a value that can be either <see cref="true" />  or <see cref="false" />.
			/// </summary>
			public static IntPtr /*GVariantType*/ Boolean { get { return _Boolean; } }

			private static IntPtr /*GVariantType*/ _Byte = IntPtr.Zero;
			/// <summary>
			/// The type of an integer value that can range from 0 to 255.
			/// </summary>
			public static IntPtr /*GVariantType*/ Byte { get { return _Byte; } }

			private static IntPtr /*GVariantType*/ _Int16 = IntPtr.Zero;
			/// <summary>
			/// The type of an integer value that can range from -32768 to 32767.
			/// </summary>
			public static IntPtr /*GVariantType*/ Int16 { get { return _Int16; } }

			private static IntPtr /*GVariantType*/ _UInt16 = IntPtr.Zero;
			/// <summary>
			/// The type of an integer value that can range from 0 to 65535.
			/// </summary>
			/// <remarks>There were about this many people living in Toronto in the 1870s.</remarks>
			public static IntPtr /*GVariantType*/ UInt16 { get { return _UInt16; } }

			private static IntPtr /*GVariantType*/ _Int32 = IntPtr.Zero;
			/// <summary>
			/// The type of an integer value that can range from -2147483648 to 2147483647.
			/// </summary>
			public static IntPtr /*GVariantType*/ Int32 { get { return _Int32; } }

			private static IntPtr /*GVariantType*/ _UInt32 = IntPtr.Zero;
			/// <summary>
			/// The type of an integer value that can range from 0 to 4294967295.
			/// </summary>
			/// <remarks>That's one number for everyone who was around in the late 1970s.</remarks>
			public static IntPtr /*GVariantType*/ UInt32 { get { return _UInt32; } }

			private static IntPtr /*GVariantType*/ _Int64 = IntPtr.Zero;
			/// <summary>
			/// The type of an integer value that can range from -9223372036854775808 to 9223372036854775807.
			/// </summary>
			public static IntPtr /*GVariantType*/ Int64 { get { return _Int64; } }

			private static IntPtr /*GVariantType*/ _UInt64 = IntPtr.Zero;
			/// <summary>
			/// The type of an integer value that can range from 0 to 18446744073709551616.
			/// </summary>
			/// <remarks>That's a really big number, but a Rubik's cube can have a bit more than twice as many possible positions.</remarks>
			public static IntPtr /*GVariantType*/ UInt64 { get { return _UInt64; } }

			private static IntPtr /*GVariantType*/ _Double = IntPtr.Zero;
			/// <summary>
			/// The type of a double precision IEEE754 floating point number.
			/// </summary>
			/// <remarks>
			/// These guys go up to about 1.80e308 (plus and minus) but miss out on some numbers in between.  In any case, that's far greater than
			/// the estimated number of fundamental particles in the observable universe.
			/// </remarks>
			public static IntPtr /*GVariantType*/ Double { get { return _Double; } }

			private static IntPtr /*GVariantType*/ _String = IntPtr.Zero;
			/// <summary>
			/// The type of a string.  "" is a string.  %NULL is not a string.
			/// </summary>
			public static IntPtr /*GVariantType*/ String { get { return _String; } }

			private static IntPtr /*GVariantType*/ _ObjectPath = IntPtr.Zero;
			/// <summary>
			/// The type of a D-Bus object reference.  These are strings of a specific format used to identify objects at a given destination on the bus.
			/// </summary>
			/// <remarks>
			/// If you are not interacting with D-Bus, then there is no reason to make use of this type.  If you are, then the D-Bus specification contains a
			/// precise description of valid object paths.
			/// </remarks>
			public static IntPtr /*GVariantType*/ ObjectPath { get { return _ObjectPath; } }

			private static IntPtr /*GVariantType*/ _Signature = IntPtr.Zero;
			/// <summary>
			/// The type of a D-Bus type signature.  These are strings of a specific format used as type signatures for D-Bus methods and messages.
			/// </summary>
			/// <remarks>
			/// If you are not interacting with D-Bus, then there is no reason to make use of this type.  If you are, then the D-Bus specification contains a
			/// precise description of valid signature strings.
			/// </remarks>
			public static IntPtr /*GVariantType*/ Signature { get { return _Signature; } }

			private static IntPtr /*GVariantType*/ _Variant = IntPtr.Zero;
			/// <summary>
			/// The type of a box that contains any other value (including another variant).
			/// </summary>
			public static IntPtr /*GVariantType*/ Variant { get { return _Variant; } }

			private static IntPtr /*GVariantType*/ _Handle = IntPtr.Zero;
			/// <summary>
			/// The type of a 32bit signed integer value, that by convention, is used as an index into an array of file descriptors that are sent alongside
			/// a D-Bus message.
			/// </summary>
			/// <remarks>If you are not interacting with D-Bus, then there is no reason to make use of this type.</remarks>
			public static IntPtr /*GVariantType*/ Handle { get { return _Handle; } }

			private static IntPtr /*GVariantType*/ _Unit = IntPtr.Zero;
			/// <summary>
			/// The empty tuple type.  Has only one instance.  Known also as "triv" or "void".
			/// </summary>
			public static IntPtr /*GVariantType*/ Unit { get { return _Unit; } }

			private static IntPtr /*GVariantType*/ _Any = IntPtr.Zero;
			/// <summary>
			/// An indefinite type that is a supertype of every type (including itself).
			/// </summary>
			public static IntPtr /*GVariantType*/ Any { get { return _Any; } }

			private static IntPtr /*GVariantType*/ _Basic = IntPtr.Zero;
			/// <summary>
			/// An indefinite type that is a supertype of every basic (ie: non-container) type.
			/// </summary>
			public static IntPtr /*GVariantType*/ Basic { get { return _Basic; } }

			private static IntPtr /*GVariantType*/ _Maybe = IntPtr.Zero;
			/// <summary>
			/// An indefinite type that is a supertype of every maybe type.
			/// </summary>
			public static IntPtr /*GVariantType*/ Maybe { get { return _Maybe; } }

			private static IntPtr /*GVariantType*/ _Array = IntPtr.Zero;
			/// <summary>
			/// An indefinite type that is a supertype of every array type.
			/// </summary>
			public static IntPtr /*GVariantType*/ Array { get { return _Array; } }

			private static IntPtr /*GVariantType*/ _Tuple = IntPtr.Zero;
			/// <summary>
			/// An indefinite type that is a supertype of every tuple type, regardless of the number of items in the tuple.
			/// </summary>
			public static IntPtr /*GVariantType*/ Tuple { get { return _Tuple; } }

			private static IntPtr /*GVariantType*/ _DictEntry = IntPtr.Zero;
			/// <summary>
			/// An indefinite type that is a supertype of every dictionary entry type.
			/// </summary>
			public static IntPtr /*GVariantType*/ DictEntry { get { return _DictEntry; } }

			private static IntPtr /*GVariantType*/ _Dictionary = IntPtr.Zero;
			/// <summary>
			/// An indefinite type that is a supertype of every dictionary type -- that is, any array type that has an element type equal to any dictionary entry type.
			/// </summary>
			public static IntPtr /*GVariantType*/ Dictionary { get { return _Dictionary; } }

			private static IntPtr /*GVariantType*/ _StringArray = IntPtr.Zero;
			/// <summary>
			/// The type of an array of strings.
			/// </summary>
			public static IntPtr /*GVariantType*/ StringArray { get { return _StringArray; } }

			private static IntPtr /*GVariantType*/ _ObjectPathArray = IntPtr.Zero;
			/// <summary>
			/// The type of an array of object paths.
			/// </summary>
			public static IntPtr /*GVariantType*/ ObjectPathArray { get { return _ObjectPathArray; } }

			private static IntPtr /*GVariantType*/ _ByteString = IntPtr.Zero;
			/// <summary>
			/// The type of an array of bytes.  This type is commonly used to pass around strings that may not be valid utf8.  In that case, the convention is that the
			/// nul terminator character should be included as the last character in the array.
			/// </summary>
			public static IntPtr /*GVariantType*/ ByteString { get { return _ByteString; } }

			private static IntPtr /*GVariantType*/ _ByteStringArray = IntPtr.Zero;
			/// <summary>
			/// The type of an array of byte strings (an array of arrays of bytes).
			/// </summary>
			public static IntPtr /*GVariantType*/ ByteStringArray { get { return _ByteStringArray; } }

			private static IntPtr /*GVariantType*/ _VarDict = IntPtr.Zero;
			/// <summary>
			/// The type of a dictionary mapping strings to variants (the ubiquitous "a{sv}" type). Since: 2.30
			/// </summary>
			public static IntPtr /*GVariantType*/ VarDict { get { return _VarDict; } }

		}
	}
}

