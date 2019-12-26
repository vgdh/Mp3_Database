using System.Collections;
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
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewExistingSongs.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Artist", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));

        }



       

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


        #region Сортировка по клику на столбец

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }
                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction, ((ListView)sender).ItemsSource);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate=
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }
        private void Sort(string sortBy, ListSortDirection direction, IEnumerable itemSource)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(itemSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
        #endregion


        private void ListViewExistingSongs_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            ((MainWindowViewModel)(((Grid)listView.Parent).DataContext)).SelectedExistedSongs =
                ListViewExistingSongs.SelectedItems.Cast<Song>().ToList();

        }
    }


}
