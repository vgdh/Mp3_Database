using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace Mp3_Database.Model
{

    public static class Repository
    {
        public static ObservableCollection<Song> GetAllSongs
        {
            get
            {
                using (var db = new mainEntities())
                {
                        db.Songs.Load();
                        return db.Songs.Local;
                }
            }
        }

        public static void AddSongs(IList<Song> songsList)
        {
            using (var db = new mainEntities())
            {
                db.Songs.AddRange(songsList);
                db.SaveChanges();
            }
        }
    }
}
