using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AECSpot
{
    public class SelectAllCommand : CommandBase
    {

        public override void Execute(object parameter)
        {
            var treeItems = (IList)parameter;
            if (treeItems == null) return;
            foreach (var treeItem in treeItems)
            {
                ((TreeViewElement)treeItem).IsChecked = true;
            }

        }

    }
}
