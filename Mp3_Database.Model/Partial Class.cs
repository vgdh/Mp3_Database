using System;
using System.ComponentModel;

namespace Mp3_Database.Model
{
    partial class Song : INotifyPropertyChanged
    {
        public Song() { }

        public Song(string artist, string title, int addTime)
        {
            this.Artist = artist;
            this.Title = title;
            this.Add_time = addTime;
        }

        public Song(string artist, string title, int addTime, string filePath)
            : this(artist, title, addTime)
        {
            this.FilePath = filePath;
        }


        public string FilePath { get; set; }

        private bool _existEarlier;
        public bool ExistEarlier
        {
            get { return _existEarlier; }
            set
            {
                if (_existEarlier != value)
                {
                    _existEarlier = value;
                    OnPropertyChanged(nameof(ExistEarlier));
                }
            }
        }

        public DateTime AddTimeDateTime => (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(this.Add_time);
        public bool IsSelected { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
