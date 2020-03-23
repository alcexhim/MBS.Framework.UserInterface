using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls.Internal.CustomListView
{
    public class ListViewHitTestInfo
    {
        private ListViewItem mvarItem = null;
        public ListViewItem Item { get { return mvarItem; } }

        public ListViewHitTestInfo(ListViewItem item)
        {
            mvarItem = item;
        }
    }
}
