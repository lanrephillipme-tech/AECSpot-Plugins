using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AECSpot
{
    public abstract class ViewModelBase : INotifyPropertyChanged

    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

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