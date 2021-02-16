using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Framework.UserInterface.Drawing
{
	public class Font
	{
		private string mvarFamilyName = null;
		public string FamilyName { get { return mvarFamilyName; } set { mvarFamilyName = value; } }

		private string mvarFaceName = null;
		public string FaceName { get { return mvarFaceName; } set { mvarFaceName = value; } }

		private double? mvarSize = null;
		public double? Size { get { return mvarSize; } set { mvarSize = value; } }

		private bool mvarItalic = false;
		public bool Italic { get { return mvarItalic; } set { mvarItalic = value; } }

		private double? mvarWeight = null;
		public double? Weight { get { return mvarWeight; } set { mvarWeight = value; } }

		public static Font FromFamily(string familyName, double size, double? weight = null)
		{
			Font font = new Font();
			font.FaceName = familyName;
			font.FamilyName = familyName;
			font.Size = size;
			font.Weight = weight;
			return font;
		}
		public static Font FromFont(Font font, double size, double? weight = null)
		{
			Font font2 = new Font();
			font2.FaceName = font.FaceName;
			font2.FamilyName = font.FamilyName;
			font2.Size = size;
			font2.Weight = weight;
			return font2;
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (mvarFamilyName);

			sb.Append (' ');
			sb.Append (mvarSize.ToString ());
			sb.Append (' ');
			sb.Append (mvarFaceName);
			return sb.ToString ();
		}

		public static Font Parse(string value)
		{
			string[] pieces = value.Split(new char[] { ' ' });
			string name = null, style = null;
			int size = 10;

			if (pieces.Length >= 3)
			{
				name = pieces[0];
				style = pieces[1];
				size = Int32.Parse(pieces[2]);
			}
			else if (pieces.Length == 2)
			{
				name = pieces[0];
				size = Int32.Parse(pieces[1]);
			}
			return Font.FromFamily(name, size, FontWeightFromStyle(style));
		}

		private static double? FontWeightFromStyle(string style)
		{
			switch (style)
			{
				case "Thin": return FontWeights.Thin;
				case "Ultra-Light": return FontWeights.UltraLight;
				case "Light": return FontWeights.Light;
				case "Semi-Light": return FontWeights.SemiLight;
				case "Book": return FontWeights.Book;
				case "Medium": return FontWeights.Medium;
				case "Semi-Bold": return FontWeights.SemiBold;
				case "Bold": return FontWeights.Bold;
				case "Ultra-Bold": return FontWeights.UltraBold;
				case "Heavy": return FontWeights.Heavy;
				case "Ultra-Heavy": return FontWeights.UltraHeavy;
				case null: return FontWeights.Normal;
			}
			return null;
		}
	}
}
