using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Id3;

namespace Mp3_Database
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private mainEntities1 db = new mainEntities1();

        public MainWindow()
        {
            InitializeComponent();
            db.Songs.Load();
            this.ListViewExistingSongs.ItemsSource = db.Songs.Local;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewExistingSongs.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Artist", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
        }

        private void ListViewNewSongs_OnDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> filePathsList = GetFilePathsRecursive(files);
            ListViewNewSongs.ItemsSource = CreateListOfSongs(filePathsList);
        }

        private List<string> GetFilePathsRecursive(string[] files)
        {
            List<string> filePathsListForReturn = new List<string>();

            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    string[] filePathsInDir = Directory.GetFiles(file, "*", SearchOption.AllDirectories);
                    filePathsListForReturn.AddRange(GetFilePathsRecursive(filePathsInDir));
                }
                else
                {
                    if (file.EndsWith(".mP3", StringComparison.CurrentCultureIgnoreCase))
                    {
                        filePathsListForReturn.Add(file);
                    }
                }
            }

            return filePathsListForReturn;
        }

        static ObservableCollection<Song> CreateListOfSongs(List<string> filesList)
        {
            ObservableCollection<Song> songs = new ObservableCollection<Song>();
            foreach (var filePath in filesList)
            {
                using (var mp3 = new Mp3(filePath))
                {
                    Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
                    Id3Tag tag1 = mp3.GetTag(Id3TagFamily.Version1X);

                    if (string.IsNullOrEmpty(tag.Title) || string.IsNullOrEmpty(tag.Artists.Value[0]))
                    {
                        MessageBox.Show($"Трек {filePath} MP3 tag не содержит в себе название трека");
                    }
                    else
                    {
                        var unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

                        Song newSong = new Song(tag.Artists.Value[0], tag.Title, unixTime, filePath);
                        songs.Add(newSong);
                    }
                }
            }
            return songs;
        }

        private void Button_Click_CopySongs(object sender, RoutedEventArgs e)
        {
            if (ListViewNewSongs.ItemsSource != null)
            {
                foreach (Song song in ListViewNewSongs.ItemsSource)
                {
                    if (db.Songs.Count(x => x.Title == song.Title & x.Artist == song.Artist) == 0)
                    {
                        CopyNewSongsToOutputFolder(song);
                        db.Songs.Add(song);
                    }
                }
                db.SaveChanges();
            }
            Process.Start(@".\\MP3_Output\\");
        }

        private void CopyNewSongsToOutputFolder(Song song)
        {
            if (!Directory.Exists(".\\MP3_Output"))
            {
                Directory.CreateDirectory(".\\MP3_Output");
            }

            File.Copy(song.FilePath, $".\\MP3_Output\\{song.Artist} - {song.Title}.mp3", overwrite: true);
        }

        private void Button_Click_RemoveSongFromDatabase(object sender, RoutedEventArgs e)
        {
            var songs = ListViewExistingSongs.SelectedItems.Cast<Song>().ToList();
            if (songs.Count > 0)
            {
                db.Songs.RemoveRange(songs);
                db.SaveChanges();
            }

        }

        private void Button_Click_AddSongsToDatabase(object sender, RoutedEventArgs e)
        {
            if (ListViewNewSongs.ItemsSource != null)
            {
                foreach (Song song in ListViewNewSongs.ItemsSource)
                {
                    if (db.Songs.Count(x => x.Title == song.Title & x.Artist == song.Artist) == 0)
                    {
                        db.Songs.Add(song);
                    }
                }
                db.SaveChanges();
            }

            MessageBox.Show("Дреки добалены в базу данных");
        }
    }

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

        public DateTime AddTimeDateTime => (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(this.Add_time);
    }
}
