using System;
using System.Collections.ObjectModel;
using System.ComponentModel;





namespace Lockscreen_Swap
{
    class ClassTime : INotifyPropertyChanged
    {
        public int frTime { get; set; }
        public string frName { get; set; }

        public ClassTime(int frTime, string frName)
        {
            this.frTime = frTime;
            this.frName = frName;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}