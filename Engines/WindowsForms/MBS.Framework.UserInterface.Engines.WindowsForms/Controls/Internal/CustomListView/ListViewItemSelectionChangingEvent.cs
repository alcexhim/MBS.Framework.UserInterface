using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls.Internal.CustomListView
{
    public delegate void ListViewItemSelectionChangedEventHandler(object sender, ListViewItemSelectionChangedEventArgs e);
    public class ListViewItemSelectionChangedEventArgs : CancelEventArgs
    {
        private ListViewItem mvarItem = null;
        public ListViewItem Item { get { return mvarItem; } }

        public ListViewItemSelectionChangedEventArgs(ListViewItem item)
        {
            mvarItem = item;
        }
    }
}
