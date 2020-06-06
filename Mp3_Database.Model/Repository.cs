using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace Mp3_Database.Model
{

    public static class Repository
    {
        private static readonly mainEntities Db = new mainEntities();

        public static ObservableCollection<Song> GetAllSongs()
        {
            Db.Songs.Load();
            return Db.Songs.Local;
        }

        public static void AddSongs(IEnumerable<Song> songsList)
        {
            Db.Songs.AddRange(songsList);
            Db.SaveChanges();
        }

        public static void AddSong(Song song)
        {
            Db.Songs.Add(song);
            Db.SaveChanges();
        }

        public static void RemoveSongs(IEnumerable<Song> songsList)
        {
            List<Song> sList = new List<Song>();
            foreach (var song in songsList)
            {
                var foundSong = Db.Songs.FirstOrDefault(x => x.Artist == song.Artist && x.Title == song.Title);
                if (foundSong != null)
                    sList.Add(foundSong);
            }

            if (sList.Count != 0)
            {
                Db.Songs.RemoveRange(sList);
                Db.SaveChanges();

                foreach (var song in songsList)
                    song.ExistEarlier = false;
            }


        }
    }
}
