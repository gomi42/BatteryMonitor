using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BatteryMonitor
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        //////////////////////////////////////////////

        public event PropertyChangedEventHandler PropertyChanged;

        //////////////////////////////////////////////

        protected void NotifyPropertyChanged([CallerMemberName] string info = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        //////////////////////////////////////////////

        public void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return;
            }

            field = value;
            NotifyPropertyChanged(propertyName);
        }
    }
}
