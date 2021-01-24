using System;
using System.ComponentModel;





namespace Lockscreen_Swap
{
    class ClassImages
    {
        public string name { get; set; }

        public ClassImages(string name)
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
