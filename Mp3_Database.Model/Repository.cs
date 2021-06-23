using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mp3_Database.Model
{

    public static class Repository
    {
        private static readonly Db DbContext = new Db("./database.sqlite");

        public static List<Song> GetAllSongs()
        {
            return DbContext.Songs.ToList();
        }

        public static void AddSongs(IEnumerable<Song> songsList)
        {
            DbContext.Songs.AddRange(songsList);
            DbContext.SaveChanges();
        }

        public static void AddSong(Song song)
        {
            DbContext.Songs.Add(song);
            DbContext.SaveChanges();
        }

        public static void RemoveSongs(IEnumerable<Song> songsList)
        {
            List<Song> sList = new List<Song>();
            foreach (var song in songsList)
            {
                var foundSong = DbContext.Songs.FirstOrDefault(x => x.Artist == song.Artist && x.Title == song.Title);
                if (foundSong != null)
                    sList.Add(foundSong);
            }

            if (sList.Count != 0)
            {
                DbContext.Songs.RemoveRange(sList);
                DbContext.SaveChanges();

                foreach (var song in songsList)
                    song.ExistEarlier = false;
            }


        }
    }
}
