using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Mp3_Database.Model;
using Mp3_Database.View;
namespace Mp3_Database.ViewModel
{

    public class MainWindowViewModel : ViewModelBase
    {
        public void MainViewModel()
        {

        }

        public ObservableCollection<Song> ExistingSongs
        {
            get
            {
                return Repository.GetAllSongs;
            }
        }

        public List<Song> SelectedExistedSongs { get; set; } = new List<Song>();

        RelayCommand _removeSongsFromDatabaseCommand;
        public ICommand RemoveSongsCommand
        {
            get
            {
                if (_removeSongsFromDatabaseCommand == null)
                    _removeSongsFromDatabaseCommand = new RelayCommand(ExecuteAddSongsToDatabaseCommand, CanExecuteAddSongsToDatabaseCommand);
                return _removeSongsFromDatabaseCommand;
            }
        }

        public void ExecuteAddSongsToDatabaseCommand()
        {
            Repository.RemoveSongs(SelectedExistedSongs);
        }

        public bool CanExecuteAddSongsToDatabaseCommand()
        {
            if (SelectedExistedSongs == null || SelectedExistedSongs.Count < 1)
                return false;
            return true;
        }


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
                        headerClicked.Column.HeaderTemplate =
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

    }
}
