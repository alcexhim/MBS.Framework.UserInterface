using System;
using System.Collections.Generic;
using MBS.Framework.Drawing;

namespace MBS.Framework.UserInterface.Controls.Timeline
{
	public class TimelineObject : TimelineItem, ISupportsExtraData
	{
		public class TimelineObjectCollection
			: System.Collections.ObjectModel.Collection<TimelineObject>
		{
			private TimelineGroup _Parent = null;
			public TimelineObjectCollection(TimelineGroup parent)
			{
				_Parent = parent;
			}
			protected override void ClearItems()
			{
				if (_Parent != null)
				{
					// required because this is also used for SelectedObjects and we don't want to clear the parent just because we deselect an item
					for (int i = 0; i < Count; i++)
					{
						this[i].Parent = null;
					}
				}
				base.ClearItems();
			}
			protected override void InsertItem(int index, TimelineObject item)
			{
				if (_Parent != null)
				{
					// required because this is also used for SelectedObjects and we don't want to clear the parent just because we deselect an item
					item.Parent = _Parent;
				}
				base.InsertItem(index, item);
			}
			protected override void RemoveItem(int index)
			{
				if (_Parent != null)
				{
					// required because this is also used for SelectedObjects and we don't want to clear the parent just because we deselect an item
					this[index].Parent = null;
				}
				base.RemoveItem(index);
			}

		}

		public TimelineObject(int startFrame, int length)
		{
			StartFrame = startFrame;
			Length = length;
		}

		public int StartFrame { get; set; }
		public int EndFrame { get; set; }
		public int Length {  get { return EndFrame - StartFrame; } set { EndFrame = StartFrame + value; } }

		public Color BackgroundColor { get; set; } = Color.Empty;
		public TimelineGroup Parent { get; private set; }

		private Dictionary<string, object> _ExtraData = new Dictionary<string, object>();
		public T GetExtraData<T>(string key, T defaultValue = default(T))
		{
			if (_ExtraData.ContainsKey(key) && _ExtraData[key] is T)
				return (T)_ExtraData[key];
			return defaultValue;
		}

		public void SetExtraData<T>(string key, T value)
		{
			_ExtraData[key] = value;
		}

		public object GetExtraData(string key, object defaultValue = null)
		{
			if (_ExtraData.ContainsKey(key))
				return _ExtraData[key];
			return defaultValue;
		}

		public void SetExtraData(string key, object value)
		{
			_ExtraData[key] = value;
		}
	}
}
