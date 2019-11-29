using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace Mp3_Database.Model
{

    public static class Repository
    {
        private static mainEntities db = new mainEntities();
        public static ObservableCollection<Song> GetAllSongs
        {
            get
            {
                db.Songs.Load();
                return db.Songs.Local;
            }
        }

        public static void AddSongs(IEnumerable<Song> songsList)
        {
            using (var db = new mainEntities())
            {
                db.Songs.AddRange(songsList);
                db.SaveChanges();
            }
        }

        public static void RemoveSongs(List<Song> songsList)
        {

            db.Songs.RemoveRange(songsList);
            db.SaveChanges();
        }
    }
}
