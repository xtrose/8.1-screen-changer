using System;
using System.ComponentModel;





namespace Lockscreen_Swap
{
    class ClassSongs
    {
        public int id { get; set; }
        public string name { get; set; }

        public ClassSongs(int id, string name)
        {
            this.id = id;
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
