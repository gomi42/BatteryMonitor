namespace BatteryMonitor
{
    internal class ErrorViewModel :ViewModelBase
    {
        string text;

        public string Text
        {
            get => text;
            set
            {
                SetProperty(ref text, value);
                NotifyPropertyChanged(nameof(ShowError));
            }
        }

        public bool ShowError
        {
            get
            {
                return !string.IsNullOrEmpty(text);
            }
        }

    }
}
