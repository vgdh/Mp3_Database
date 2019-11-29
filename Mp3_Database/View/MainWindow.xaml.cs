using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Mp3_Database.Model;

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
            this.ListViewExistingSongs.ItemsSource = Repository.GetAllSongs;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewExistingSongs.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Artist", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
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
        //GridViewColumnHeader _lastHeaderClicked = null;
        //ListSortDirection _lastDirection = ListSortDirection.Ascending;

        //#region Сортировка по клику на столбец
        //void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        //{
        //    var headerClicked = e.OriginalSource as GridViewColumnHeader;
        //    ListSortDirection direction;

        //    if (headerClicked != null)
        //    {
        //        if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
        //        {
        //            if (headerClicked != _lastHeaderClicked)
        //            {
        //                direction = ListSortDirection.Ascending;
        //            }
        //            else
        //            {
        //                if (_lastDirection == ListSortDirection.Ascending)
        //                {
        //                    direction = ListSortDirection.Descending;
        //                }
        //                else
        //                {
        //                    direction = ListSortDirection.Ascending;
        //                }
        //            }
        //            var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
        //            var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

        //            Sort(sortBy, direction, ((ListView)sender).ItemsSource);

        //            if (direction == ListSortDirection.Ascending)
        //            {
        //                headerClicked.Column.HeaderTemplate =
        //                  Resources["HeaderTemplateArrowUp"] as DataTemplate;
        //            }
        //            else
        //            {
        //                headerClicked.Column.HeaderTemplate =
        //                  Resources["HeaderTemplateArrowDown"] as DataTemplate;
        //            }

        //            // Remove arrow from previously sorted header
        //            if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
        //            {
        //                _lastHeaderClicked.Column.HeaderTemplate = null;
        //            }

        //            _lastHeaderClicked = headerClicked;
        //            _lastDirection = direction;
        //        }
        //    }
        //}
        //private void Sort(string sortBy, ListSortDirection direction, IEnumerable itemSource)
        //{
        //    ICollectionView dataView = CollectionViewSource.GetDefaultView(itemSource);

        //    dataView.SortDescriptions.Clear();
        //    SortDescription sd = new SortDescription(sortBy, direction);
        //    dataView.SortDescriptions.Add(sd);
        //    dataView.Refresh();
        //}
        //#endregion

    }


}
