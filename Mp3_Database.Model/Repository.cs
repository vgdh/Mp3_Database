using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;

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

        public static void RemoveSongs(List<Song> songsList)
        {

            Db.Songs.RemoveRange(songsList);
            Db.SaveChanges();
        }
    }
}
