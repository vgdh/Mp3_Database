using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp3_Database.Model
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
            using (mainEntities db = new mainEntities())
            {
                song s1 = new song()
                {
                    artist = "artist 1",
                    title = "title 1"
                };
                db.song.Add(s1);
                db.SaveChanges();
            }
        }

    }
}
