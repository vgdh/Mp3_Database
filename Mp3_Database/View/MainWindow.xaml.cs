using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Mp3_Database.Model;
using Mp3_Database.ViewModel;

namespace Mp3_Database.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.ListViewExistingSongs.ItemsSource = Repository.GetAllSongs;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewExistingSongs.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Artist", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));

            this.DataContext = new MainWindowViewModel();
        }

        //private void ListViewNewSongs_OnDrop(object sender, DragEventArgs e)
        //{
        //    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        //    List<string> filePathsList = GetFilePathsRecursive(files);
        //    ObservableCollection<Song> newSongsList = CreateListOfSongs(filePathsList);
        //    ListViewNewSongs.ItemsSource = newSongsList;
        //    DuplicateSongCount.Content = newSongsList.Count(song => song.ExistEarlier == true);
        //}

        //private List<string> GetFilePathsRecursive(string[] files)
        //{
        //    List<string> filePathsListForReturn = new List<string>();

        //    foreach (string file in files)
        //    {
        //        if (Directory.Exists(file))
        //        {
        //            string[] filePathsInDir = Directory.GetFiles(file, "*", SearchOption.AllDirectories);
        //            filePathsListForReturn.AddRange(GetFilePathsRecursive(filePathsInDir));
        //        }
        //        else
        //        {
        //            if (file.EndsWith(".mP3", StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                filePathsListForReturn.Add(file);
        //            }
        //        }
        //    }

        //    return filePathsListForReturn;
        //}

        //private ObservableCollection<Song> CreateListOfSongs(List<string> filesList)
        //{
        //    ObservableCollection<Song> songs = new ObservableCollection<Song>();
        //    foreach (var filePath in filesList)
        //    {
        //        using (var mp3 = new Mp3(filePath))
        //        {
        //            Id3Tag tag = null;
        //            Id3Tag tag_1x = null;
        //            Id3Tag tag_2x = null;

        //            try
        //            {
        //                tag_1x = mp3.GetTag(Id3TagFamily.Version1X);
        //            }
        //            catch (Exception e)
        //            {
        //                Logging(e);
        //            }

        //            try
        //            {
        //                tag_2x = mp3.GetTag(Id3TagFamily.Version2X);
        //            }
        //            catch (Exception e)
        //            {
        //                Logging(e);
        //            }


        //            if (tag_2x != null)
        //            {
        //                tag = tag_2x;
        //            }
        //            else
        //            {
        //                tag = tag_1x;
        //            }

        //            if (string.IsNullOrEmpty(tag.Title))
        //            {
        //                MessageBox.Show($"Трек {filePath} не содержит в себе название трека в MP3 TAG");
        //            }
        //            else
        //            {
        //                var unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        //                //Чтение даты из времени создания файла в папке
        //                //var unixTime = (int)(File.GetCreationTimeUtc(filePath) - new DateTime(1970, 1, 1)).TotalSeconds;
        //                string artist;


        //                if (tag.Artists.Value.Count > 0 && tag.Artists.Value[0].Length < 50)
        //                {
        //                    artist = tag.Artists.Value[0];
        //                }
        //                else if (!string.IsNullOrEmpty(tag.Band.Value))
        //                {
        //                    artist = tag.Band.Value;
        //                }
        //                else
        //                {
        //                    artist = "";
        //                }

        //                Song newSong = new Song(artist, tag.Title, unixTime, filePath);

        //                if (Repository.Songs.Count(xx => xx.Title == newSong.Title & xx.Artist == newSong.Artist) > 0)
        //                {
        //                    newSong.ExistEarlier = true;
        //                }

        //                songs.Add(newSong);
        //            }
        //        }
        //    }
        //    return songs;
        //}

        //private static void Logging(Exception e)
        //{
        //    string filePath;
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(e.ToString());
        //    File.AppendAllText("log.txt", sb.ToString());
        //    sb.Clear();
        //}

        //private void Button_Click_CopySongs(object sender, RoutedEventArgs e)
        //{
        //    if (ListViewNewSongs.ItemsSource != null)
        //    {
        //        foreach (Song song in ListViewNewSongs.ItemsSource)
        //        {
        //            if (Repository.Songs.Count(x => x.Title == song.Title & x.Artist == song.Artist) == 0)
        //            {
        //                CopyNewSongsToOutputFolder(song);
        //                Repository.Songs.Add(song);
        //            }
        //        }
        //        Repository.SaveChanges();
        //    }
        //    Process.Start(@".\\MP3_Output\\");
        //}

        //private void CopyNewSongsToOutputFolder(Song song)
        //{
        //    if (!Directory.Exists(".\\MP3_Output"))
        //    {
        //        Directory.CreateDirectory(".\\MP3_Output");
        //    }

        //    string fileName = $"{song.Artist} - {song.Title}.mp3";

        //    string regexSearch = new string(System.IO.Path.GetInvalidFileNameChars());
        //    Regex r = new Regex($"[{Regex.Escape(regexSearch)}]");
        //    fileName = r.Replace(fileName, "");

        //    File.Copy(song.FilePath, $".\\MP3_Output\\{fileName}", overwrite: true);
        //}

        //private void Button_Click_RemoveSongFromDatabase(object sender, RoutedEventArgs e)
        //{
        //    var songs = ListViewExistingSongs.SelectedItems.Cast<Song>().ToList();
        //    if (songs.Count > 0)
        //    {
        //        Repository.Songs.RemoveRange(songs);
        //        Repository.SaveChanges();
        //    }

        //}

        //private void Button_Click_AddSongsToDatabase(object sender, RoutedEventArgs e)
        //{
        //    if (ListViewNewSongs.ItemsSource != null)
        //    {
        //        foreach (Song song in ListViewNewSongs.ItemsSource)
        //        {
        //            if (Repository.Songs.Count(x => x.Title == song.Title & x.Artist == song.Artist) == 0)
        //            {
        //                Repository.Songs.Add(song);
        //            }
        //        }
        //        Repository.SaveChanges();
        //    }

        //    MessageBox.Show("Дреки добалены в базу данных");
        //}


        

        private void ListViewExistingSongs_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = (ListView) sender;
            ((MainWindowViewModel) (((Grid) listView.Parent).DataContext)).SelectedExistedSongs =
                ListViewExistingSongs.SelectedItems.Cast<Song>().ToList();

        }
    }


}
