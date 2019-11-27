using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
        public MainWindow()
        {
            InitializeComponent();


            using (var db = new mainEntities1())
            {
                //db.Songs.Add(new Song("1", "2", 123));
                //db.SaveChanges();
                db.Songs.Load();
                this.ListViewExistingSongs.ItemsSource = db.Songs.Local;
            }
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
                    string[] filePathsInDir = Directory.GetFiles(file);
                    filePathsListForReturn.AddRange(GetFilePathsRecursive(filePathsInDir));
                }
                else
                {
                    if (file.EndsWith(".mP3",StringComparison.CurrentCultureIgnoreCase))
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
                   
                    if (string.IsNullOrEmpty(tag.Band) || string.IsNullOrEmpty(tag.Title))
                    {
                        MessageBox.Show($"Трек {filePath} не содержит в себе MP3 tag исполнителя или название трека");
                    }
                    else
                    {
                        Song newSong = new Song(tag.Band, tag.Title, (int)DateTime.Now.Ticks);
                        songs.Add(newSong);
                    }
                }
            }
            return songs;
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

    }
}
