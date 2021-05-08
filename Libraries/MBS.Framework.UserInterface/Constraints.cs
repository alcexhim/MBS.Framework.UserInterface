using System;

namespace MBS.Framework.UserInterface
{
	/// <summary>
	/// Constraints for layouts.
	/// </summary>
	public abstract class Constraints
	{
		public bool HorizontalExpand { get; set; } = false;
		public bool VerticalExpand { get; set; } = false;
	}
}
