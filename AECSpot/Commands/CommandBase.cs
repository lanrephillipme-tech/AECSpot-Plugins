using AECSpot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace AECSpot
{
    public abstract class CommandBase : ICommand
    {
        #region Command Elements

        public ViewModelBase viewModel { get; set; }

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        { return true; }

        public abstract void Execute(object parameter);

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        #endregion Command Elements

        /// <summary>
        /// Convert List of objects to obsdervable collection
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="ListToBeConverted">
        /// List of type <see cref="T"> where t is type of list </see>/&gt;
        /// </param>
        /// <returns> </returns>
        public static ObservableCollection<T> ConvertListToObservableCollection<T>(List<T> ListToBeConverted)
        {
            ObservableCollection<T> values = new ObservableCollection<T>();
            if (null != ListToBeConverted)
            {
                foreach (var item in ListToBeConverted)
                {
                    values.Add(item);
                }
            }
            return values;
        }
    }
}