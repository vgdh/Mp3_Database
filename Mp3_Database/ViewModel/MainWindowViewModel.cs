using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Mp3_Database.Model;

namespace Mp3_Database.ViewModel
{

    public class MainWindowViewModel : ViewModelBase
    {
        public void MainViewModel()
        {

        }
        public ObservableCollection<Song> Songs => Repository.GetAllSongs;


        //public IList<Song> SelectedExistedSongs { get; set; }

        //RelayCommand _addSongsToDatabaseCommand;
        //public ICommand AddClient
        //{
        //    get
        //    {
        //        if (_addSongsToDatabaseCommand == null)
        //            _addSongsToDatabaseCommand = new RelayCommand(ExecuteAddSongsToDatabaseCommand, CanExecuteAddSongsToDatabaseCommand);
        //        return _addSongsToDatabaseCommand;
        //    }
        //}

        //public void ExecuteAddSongsToDatabaseCommand()
        //{
        //    Repository.AddSongs(songs);
        //    Clients.Add(_currentClient);
        //    CurrentClient = null;
        //}

        //public bool CanExecuteAddSongsToDatabaseCommand()
        //{
        //    if (string.IsNullOrEmpty(CurrentClient.FirstName) ||
        //        string.IsNullOrEmpty(CurrentClient.LastName))
        //        return false;
        //    return true;
        //}

    }
}
