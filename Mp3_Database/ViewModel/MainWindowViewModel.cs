using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Id3;
using Mp3_Database.Model;
using Mp3_Database.View;

namespace Mp3_Database.ViewModel
{

    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public void MainViewModel()
        {

        }

        public ObservableCollection<Song> ExistingSongs
        {
            get
            {
                return Repository.GetAllSongs();
            }
        }

        public List<Song> SelectedExistedSongs { get; set; } = new List<Song>();

        public ObservableCollection<Song> NewSongsList { get; set; } = new ObservableCollection<Song>();
        public bool OneDirectoryDroped { get; set; } = false;

        private int _duplicateSongCount;
        public int DuplicateSongCount
        {
            get { return _duplicateSongCount; }
            set
            {
                if (_duplicateSongCount != value)
                {
                    _duplicateSongCount = value;
                    OnPropertyChanged(nameof(DuplicateSongCount));
                }
            }
        }


        private string _oneDirectoryName = "MP3_Output";
        public string OneDirectoryName 
        {
            get { return _oneDirectoryName; }
            set
            {
                if (_oneDirectoryName != value)
                {
                    _oneDirectoryName = value;
                    OnPropertyChanged("OneDirectoryName");
                    OnPropertyChanged(nameof(OneDirectoryName));
                }
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        #region RemoveSongCommand
        RelayCommand _removeSongsFromDatabaseCommand;
        public ICommand RemoveSongsFromDatabaseCommand
        {
            get
            {
                if (_removeSongsFromDatabaseCommand == null)
                    _removeSongsFromDatabaseCommand = new RelayCommand(ExecuteRemoveSongsFromDatabaseCommand, CanExecuteRemoveSongsFromDatabaseCommand);
                return _removeSongsFromDatabaseCommand;
            }
        }

        public void ExecuteRemoveSongsFromDatabaseCommand()
        {
            Repository.RemoveSongs(SelectedExistedSongs);
        }

        public bool CanExecuteRemoveSongsFromDatabaseCommand()
        {
            if (SelectedExistedSongs == null || SelectedExistedSongs.Count < 1)
                return false;
            return true;
        }
        #endregion

        #region CopyNewSongsComand
        RelayCommand _copyNewSongsCommand;
        public ICommand CopyNewSongsCommand
        {
            get
            {
                if (_copyNewSongsCommand == null)
                    _copyNewSongsCommand = new RelayCommand(ExecuteCopyNewSongsCommand, CanExecuteCopyNewSongsCommand);
                return _copyNewSongsCommand;
            }
        }

        public void ExecuteCopyNewSongsCommand()
        {
            if (NewSongsList.Count == 0) return;
            string outputDir = string.Empty;

            if (OneDirectoryDroped)
            {
                outputDir = OneDirectoryName;
            }
            else
            {
                outputDir = "MP3_Output";
            }




            foreach (Song song in NewSongsList)
            {
                if (ExistingSongs.Count(sng => sng.Artist == song.Artist & sng.Title == song.Title) == 0)
                {
                    CopyNewSongsToOutputFolder(song, outputDir);
                    Repository.AddSong(song);
                    song.ExistEarlier = true;
                }
            }

            if (Directory.Exists($".\\{outputDir}"))
            {
                Process.Start($".\\{outputDir}\\");
            }
            else
            {
                MessageBox.Show("Что то пошло не так и не создалась папка для файлов");
            }
        }

        public bool CanExecuteCopyNewSongsCommand()
        {
            if (NewSongsList == null || NewSongsList.Count < 1 || NewSongsList.Count(song => song.ExistEarlier == false) == 0)
                return false;
            return true;
        }

        private void CopyNewSongsToOutputFolder(Song song, string folder)
        {
            if (!Directory.Exists($".\\{folder}"))
            {
                Directory.CreateDirectory($".\\{folder}");
            }

            string fileName = $"{song.Artist} - {song.Title}.mp3";

            string regexSearch = new string(System.IO.Path.GetInvalidFileNameChars());
            Regex r = new Regex($"[{Regex.Escape(regexSearch)}]");
            fileName = r.Replace(fileName, "");

            File.Copy(song.FilePath, $".\\{folder}\\{fileName}", overwrite: true);
            UpdateDuplicateSongCounter();
        }
        #endregion

        #region AddToDatabaseComand
        RelayCommand _onlyAddToDatabaseSongsCommand;
        public ICommand OnlyAddToDatabaseCommand
        {
            get
            {
                if (_onlyAddToDatabaseSongsCommand == null)
                    _onlyAddToDatabaseSongsCommand = new RelayCommand(ExecuteOnlyAddToDatabaseCommand, CanExecuteOnlyAddToDatabaseCommand);
                return _onlyAddToDatabaseSongsCommand;
            }
        }

        public void ExecuteOnlyAddToDatabaseCommand()
        {
            Repository.AddSongs(NewSongsList.Where(x => x.ExistEarlier == false));
            foreach (var song in NewSongsList)
            {
                song.ExistEarlier = true;
            }
            UpdateDuplicateSongCounter();
        }

        public bool CanExecuteOnlyAddToDatabaseCommand()
        {
            if (NewSongsList == null || NewSongsList.Count < 1 || NewSongsList.Count(song => song.ExistEarlier == false) == 0)
                return false;
            return true;
        }
        #endregion

        #region DropCommand
        private RelayCommand<DragEventArgs> _dropNewSongsCommand;
        public RelayCommand<DragEventArgs> DropNewSongsCommand
        {
            get
            {
                return _dropNewSongsCommand ?? (_dropNewSongsCommand = new RelayCommand<DragEventArgs>(Drop));
            }
        }

        private void Drop(DragEventArgs e)
        {

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null && files.Length == 1) // если это одна папка отметим это чтобы переименовать выходную папку
            {
                if (Directory.Exists(files[0]))
                {
                    OneDirectoryDroped = true;
                    OneDirectoryName = Path.GetFileName(files[0]);
                }
                else
                {
                    OneDirectoryDroped = false;
                }
            }

            List<string> filePathsList = GetFilePathsRecursive(files);
            ObservableCollection<Song> newSongsList = CreateListOfSongs(filePathsList);
            NewSongsList.Clear();
            foreach (var song in newSongsList)
            {
                NewSongsList.Add(song);
            }
            UpdateDuplicateSongCounter();
        }

        private void UpdateDuplicateSongCounter()
        {
            DuplicateSongCount = NewSongsList.Count(x => x.ExistEarlier);
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

        private ObservableCollection<Song> CreateListOfSongs(List<string> filesList)
        {
            ObservableCollection<Song> songs = new ObservableCollection<Song>();
            foreach (var filePath in filesList)
            {
                using (var mp3 = new Mp3(filePath))
                {
                    Id3Tag tag = null;
                    Id3Tag tag_1x = null;
                    Id3Tag tag_2x = null;

                    try
                    {
                        tag_1x = mp3.GetTag(Id3TagFamily.Version1X);
                    }
                    catch (Exception e)
                    {
                        Logging(e);
                    }

                    try
                    {
                        tag_2x = mp3.GetTag(Id3TagFamily.Version2X);
                    }
                    catch (Exception e)
                    {
                        Logging(e);
                    }


                    if (tag_2x != null)
                    {
                        tag = tag_2x;
                    }
                    else
                    {
                        tag = tag_1x;
                    }

                    if (string.IsNullOrEmpty(tag.Title))
                    {
                        MessageBox.Show($"Трек {filePath} не содержит в себе название трека в MP3 TAG");
                    }
                    else
                    {
                        var unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                        //Чтение даты из времени создания файла в папке
                        //var unixTime = (int)(File.GetCreationTimeUtc(filePath) - new DateTime(1970, 1, 1)).TotalSeconds;
                        string artist;


                        if (tag.Artists.Value.Count > 0 && tag.Artists.Value[0].Length < 50)
                        {
                            artist = tag.Artists.Value[0];
                        }
                        else if (!string.IsNullOrEmpty(tag.Band.Value))
                        {
                            artist = tag.Band.Value;
                        }
                        else
                        {
                            artist = "";
                        }

                        Song newSong = new Song(artist, tag.Title, unixTime, filePath);

                        if (ExistingSongs.Count(xx => xx.Title == newSong.Title & xx.Artist == newSong.Artist) > 0)
                        {
                            newSong.ExistEarlier = true;
                        }

                        songs.Add(newSong);
                    }
                }
            }
            return songs;
        }
   
        private static void Logging(Exception e)
        {
            string filePath;
            StringBuilder sb = new StringBuilder();
            sb.Append(e.ToString());
            File.AppendAllText("log.txt", sb.ToString());
            sb.Clear();
        }
        
        #endregion




    }
}
