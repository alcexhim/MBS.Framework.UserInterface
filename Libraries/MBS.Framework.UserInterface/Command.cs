using System;
using MBS.Framework.UserInterface.Input.Keyboard;
using UniversalEditor.ObjectModels.Markup;

namespace MBS.Framework.UserInterface
{
	public class Command
	{
		public class CommandCollection
			: System.Collections.ObjectModel.Collection<Command>
		{
			public Command this[string ID]
			{
				get
				{
					foreach (Command command in this)
					{
						if (command.ID == ID) return command;
					}
					return null;
				}
			}
		}

		public Command()
		{

		}
		public Command(string id, string title, CommandItem[] items = null)
		{
			ID = id;
			Title = title;
			if (items != null)
			{
				for (int i = 0; i < items.Length; i++)
				{
					Items.Add(items[i]);
				}
			}
		}

		private bool mvarEnableTearoff = false;
		public bool EnableTearoff { get { return mvarEnableTearoff; } set { mvarEnableTearoff = value; } }

		private bool mvarChecked = false;
		/// <summary>
		/// Determines whether this command displays as checked.
		/// </summary>
		public bool Checked { get { return mvarChecked; } set { mvarChecked = value; } }
		
		private string mvarID = String.Empty;
		/// <summary>
		/// The ID of the command, used to reference it in <see cref="CommandReferenceCommandItem"/>.
		/// </summary>
		public string ID { get { return mvarID; } set { mvarID = value; } }
		
		private string mvarTitle = String.Empty;
		/// <summary>
		/// The title of the command (including mnemonic prefix, if applicable).
		/// </summary>
		public string Title { get { return mvarTitle; } set { mvarTitle = value; } }

		private string mvarDefaultCommandID = String.Empty;
		public string DefaultCommandID { get { return mvarDefaultCommandID; } set { mvarDefaultCommandID = value; } }

		private Shortcut mvarShortcut = null;
		public Shortcut Shortcut { get { return mvarShortcut; } set { mvarShortcut = value; } }
		
		private StockType mvarStockType = StockType.None;
		/// <summary>
		/// A <see cref="StockType"/> that represents a predefined, platform-themed command.
		/// </summary>
		public StockType StockType { get { return mvarStockType; } set { mvarStockType = value; } }
		
		private string mvarImageFileName = String.Empty;
		/// <summary>
		/// The file name of the image to be displayed on the command.
		/// </summary>
		public string ImageFileName { get { return mvarImageFileName; } set { mvarImageFileName = value; } }
		
		
		private CommandItem.CommandItemCollection mvarItems = new CommandItem.CommandItemCollection();
		/// <summary>
		/// The child <see cref="CommandItem"/>s that are contained within this <see cref="Command"/>.
		/// </summary>
		public CommandItem.CommandItemCollection Items { get { return mvarItems; } }
		
		/// <summary>
		/// The event that is fired when the command is executed.
		/// </summary>
		public event EventHandler Executed;

		/// <summary>
		/// Determines whether this <see cref="Command" /> is enabled in all <see cref="CommandBar" />s and <see cref="MenuBar" />s
		/// that reference it.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// Determines whether this <see cref="Command" /> is visible in all <see cref="CommandBar" />s and <see cref="MenuBar" />s
		/// that reference it.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		public bool Visible { get; set; }

		/// <summary>
		/// Executes this <see cref="Command"/>.
		/// </summary>
		public void Execute()
		{
			if (Executed != null) Executed(this, EventArgs.Empty);
		}

		public override string ToString()
		{
			return String.Format("{0} [{1}]", ID, Title);
		}

		public static Command FromMarkup(MarkupTagElement tagCommand)
		{
			Command cmd = new Command();
			cmd.ID = tagCommand.Attributes["ID"]?.Value;

			MarkupAttribute attDefaultCommandID = tagCommand.Attributes["DefaultCommandID"];
			if (attDefaultCommandID != null)
			{
				cmd.DefaultCommandID = attDefaultCommandID.Value;
			}

			MarkupAttribute attCommandStockType = tagCommand.Attributes["StockType"];
			if (attCommandStockType != null)
			{
				StockType stockType = StockType.None;
				string[] names = Enum.GetNames(typeof(StockType));
				int[] values = (int[])Enum.GetValues(typeof(StockType));
				for (int i = 0; i < names.Length; i++)
				{
					if (names[i].Equals(attCommandStockType.Value))
					{
						stockType = (StockType)values[i];
						break;
					}
				}
				cmd.StockType = stockType;
			}

			MarkupAttribute attTitle = tagCommand.Attributes["Title"];
			if (attTitle != null)
			{
				cmd.Title = attTitle.Value;
			}
			else
			{
				cmd.Title = cmd.ID;
			}

			MarkupAttribute attEnabled = tagCommand.Attributes["Enabled"];
			if (attEnabled != null)
			{
				cmd.Enabled = (attEnabled.Value.ToLower() == "true");
			}

			MarkupTagElement tagShortcut = (tagCommand.Elements["Shortcut"] as MarkupTagElement);
			if (tagShortcut != null)
			{
				MarkupAttribute attModifiers = tagShortcut.Attributes["Modifiers"];
				MarkupAttribute attKey = tagShortcut.Attributes["Key"];
				if (attKey != null)
				{
					KeyboardModifierKey modifiers = KeyboardModifierKey.None;
					if (attModifiers != null)
					{
						string[] strModifiers = attModifiers.Value.Split(new char[] { ',' });
						foreach (string strModifier in strModifiers)
						{
							switch (strModifier.Trim().ToLower())
							{
								case "alt":
								{
									modifiers |= KeyboardModifierKey.Alt;
									break;
								}
								case "control":
								{
									modifiers |= KeyboardModifierKey.Control;
									break;
								}
								case "meta":
								{
									modifiers |= KeyboardModifierKey.Meta;
									break;
								}
								case "shift":
								{
									modifiers |= KeyboardModifierKey.Shift;
									break;
								}
								case "super":
								{
									modifiers |= KeyboardModifierKey.Super;
									break;
								}
							}
						}
					}

					KeyboardKey value = KeyboardKey.None;
					if (!Enum.TryParse<KeyboardKey>(attKey.Value, out value))
					{
						Console.WriteLine("ue: ui: unable to parse keyboard key '{0}'", attKey.Value);
					}

					cmd.Shortcut = new Shortcut(value, modifiers);
				}
			}

			MarkupTagElement tagItems = (tagCommand.Elements["Items"] as MarkupTagElement);
			if (tagItems != null)
			{
				foreach (MarkupElement el in tagItems.Elements)
				{
					MarkupTagElement tag = (el as MarkupTagElement);
					if (tag == null) continue;

					Application.InitializeCommandBarItem(tag, cmd, null);
				}
			}
			return cmd;
		}
	}
}

