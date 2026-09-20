using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AECSpot
{
    public class ApplyLineStyleCommand : CommandBase
    {
        public ApplyLineStyleCommand(ViewModelBase ApplyLinesChangesViewModel)
        {
            this.viewModel = ApplyLinesChangesViewModel;
        }

        public override void Execute(object parameter)
        {
            var vm = (ApplyLinesToElementsViewModel)viewModel;
            vm.ConfirmApllyStyle();
        }
    }
}