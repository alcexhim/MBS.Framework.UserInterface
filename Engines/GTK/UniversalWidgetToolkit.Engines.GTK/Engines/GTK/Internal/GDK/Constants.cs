using System;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.GDK
{
	internal static class Constants
	{
		[Flags()]
		public enum GdkModifierType
		{
			None = 0,
			Shift    = 1 << 0,
			Lock     = 1 << 1,
			Control  = 1 << 2,
			Alt     = 1 << 3, // Mod1
			Mod2     = 1 << 4,
			Mod3     = 1 << 5,
			Mod4     = 1 << 6,
			Mod5     = 1 << 7,
			Button1  = 1 << 8,
			Button2  = 1 << 9,
			Button3  = 1 << 10,
			Button4  = 1 << 11,
			Button5  = 1 << 12,

			ModifierReserved13  = 1 << 13,
			ModifierReserved14  = 1 << 14,
			ModifierReserved15  = 1 << 15,
			ModifierReserved16  = 1 << 16,
			ModifierReserved17  = 1 << 17,
			ModifierReserved18  = 1 << 18,
			ModifierReserved19  = 1 << 19,
			ModifierReserved20  = 1 << 20,
			ModifierReserved21  = 1 << 21,
			ModifierReserved22  = 1 << 22,
			ModifierReserved23  = 1 << 23,
			ModifierReserved24  = 1 << 24,
			ModifierReserved25  = 1 << 25,

			/// <summary>
			/// The next few modifiers are used by XKB, so we skip to the end. Bits 15 - 25 are currently unused. Bit 29 is used internally.
			/// </summary>
			Super    = 1 << 26,
			Hyper    = 1 << 27,
			Meta     = 1 << 28,

			Reserved29  = 1 << 29,

			Release  = 1 << 30,

			/// <summary>
			/// Combination of GDK_SHIFT_MASK..GDK_BUTTON5_MASK + GDK_SUPER_MASK + GDK_HYPER_MASK + GDK_META_MASK + GDK_RELEASE_MASK
			/// </summary>
			GDK_MODIFIER_MASK = 0x5c001fff
		}
	}
}

