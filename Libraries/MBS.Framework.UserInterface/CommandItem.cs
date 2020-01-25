using System;
namespace MBS.Framework.UserInterface
{
	public abstract class CommandItem
	{
		public string InsertAfterID { get; set; } = null;
		public string InsertBeforeID { get; set; } = null;

		public class CommandItemCollection
			: System.Collections.ObjectModel.Collection<CommandItem>
		{
			public int IndexOf(string value)
			{
				for (int i = 0; i < Count; i++)
				{
					if (this[i] is CommandReferenceCommandItem)
					{
						if ((this[i] as CommandReferenceCommandItem).CommandID.Equals(value))
							return i;
					}
				}
				return -1;
			}
		}
	}
	public class CommandReferenceCommandItem : CommandItem
	{
		private string mvarCommandID = String.Empty;
		public string CommandID { get { return mvarCommandID; } set { mvarCommandID = value; } }
		
		public CommandReferenceCommandItem(string commandID)
		{
			mvarCommandID = commandID;
		}
	}
	public class CommandPlaceholderCommandItem : CommandItem
	{
		private string mvarPlaceholderID = String.Empty;
		public string PlaceholderID { get { return mvarPlaceholderID; } set { mvarPlaceholderID = value; } }

		public CommandPlaceholderCommandItem(string placeholderID)
		{
			mvarPlaceholderID = placeholderID;
		}
	}
	public class SeparatorCommandItem : CommandItem
	{
	}
}

