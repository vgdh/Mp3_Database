using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Mp3_Database.Model
{

    public static class Repository
    {
        private static readonly Db DbContext = new Db("./database.sqlite");

        public static List<Song> GetAllSongs()
        {
            return DbContext.Songs.AsNoTracking().ToList();
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

        public static Song FindSong(Song song)
        {
            return FindSong(song.Artist, song.Title);
        }

        public static Song FindSong(string artist, string title)
        {
            return DbContext.Songs.FirstOrDefault(x => x.Artist == artist && x.Title == title);
        }

        public static bool RemoveSong(Song song)
        {
            var foundSong = DbContext.Songs.FirstOrDefault(x => x.Artist == song.Artist && x.Title == song.Title);
            if (foundSong != null)
            {
                DbContext.Songs.Remove(foundSong);
                return true;
            }
            else
                return false;
        }
        
        /// <summary>
        /// Удаляет песни из базы и возвращает лист удаленых объектов
        /// </summary>
        /// <param name="songsList"></param>
        /// <returns></returns>
        public static IEnumerable<Song> RemoveSongs(IEnumerable<Song> songsList)
        {
            List<Song> removedSongs = new();
            foreach (var item in songsList)
            {
                Song foundedSong = DbContext.Songs.FirstOrDefault(s => s.Artist == item.Artist && s.Title == item.Title);
                if (foundedSong != null)
                    removedSongs.Add(foundedSong);
            }

            DbContext.Songs.RemoveRange(removedSongs);
            DbContext.SaveChanges();
            return removedSongs;
        }
    }
}
