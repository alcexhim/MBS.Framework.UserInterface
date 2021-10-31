using System;
namespace MBS.Framework.UserInterface
{
	public static class ExtensionMethods
	{
		/// <summary>
		/// Translates the specified translation key <paramref name="key" />
		/// using <see cref="Language.GetStringTableEntry" />.
		/// </summary>
		/// <returns>The translated string.</returns>
		/// <param name="any">Any object.</param>
		/// <param name="key">The translation key.</param>
		public static string _(this object any, string key, string defaultValue = null)
		{
			return ((UIApplication)Application.Instance).DefaultLanguage.GetStringTableEntry(key, defaultValue);
		}
	}
}
