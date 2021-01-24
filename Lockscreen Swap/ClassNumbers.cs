using System;
using System.ComponentModel;





namespace Lockscreen_Swap
{
    class ClassNumbers
    {
        public string name { get; set; }

        public ClassNumbers(string name)
        {
            this.name = name;
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
