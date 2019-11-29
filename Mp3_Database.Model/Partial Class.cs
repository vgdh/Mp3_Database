using System;

namespace Mp3_Database.Model
{
    partial class Song
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
        public bool ExistEarlier { get; set; } = false;
        public DateTime AddTimeDateTime => (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(this.Add_time);
    }
}
