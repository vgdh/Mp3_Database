using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Toolkit.Mvvm.Input;
using Mp3_Database.Model;
using Mp3_Database.ViewModel;

namespace Mp3_Database.View
{
    public partial class MainWindow : Window
    {

        private static MainWindowViewModel vm = new();
        public MainWindow()
        {
            DataContext = vm;
            InitializeComponent();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewExistingSongs.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Artist", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));

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
            (vm.RemoveSelectedSongsFromDatabaseCommand as IRelayCommand).NotifyCanExecuteChanged();
        }

        private void ListViewNewSongs_Drop(object sender, DragEventArgs e)
        {
            vm.DropNewSongsCommand.Execute(e);
            (vm.RemoveeNewSongsFromDatabaseCommand as RelayCommand).NotifyCanExecuteChanged();
        }
    }


}
