using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp3_Database.Model
{
    public class Db : DbContext
    {
        public string DatabasePath { get; }
        public Db(string dbName)
        {
            DatabasePath = dbName;
            Database.EnsureCreated();
        }


        public DbSet<Song> Songs { get; set; }

        // The following configures EF to create a Sqlite database file as `C:\blogging.db`.
        // For Mac or Linux, change this to `/tmp/blogging.db` or any other absolute path.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DatabasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>()
                .ToTable("Songs");


            modelBuilder.Entity<Song>()
                .Ignore(p => p.FilePath)
                .Ignore(p => p.AddTimeDateTime)
                .Ignore(p => p.ExistEarlier)
                .Ignore(p => p.IsSelected);

            modelBuilder.Entity<Song>()
                .Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("integer");

            modelBuilder.Entity<Song>()
                .Property(p => p.Artist)
                .HasColumnName("Artist")
                .HasColumnType("text");

            modelBuilder.Entity<Song>()
                .Property(p => p.Title)
                .HasColumnName("Title")
                .HasColumnType("text");

            modelBuilder.Entity<Song>()
                .Property(p => p.Add_time)
                .HasColumnName("Add_time")
                .HasColumnType("integer");
        }
    }

    public class Song : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public int Add_time { get; set; }


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
