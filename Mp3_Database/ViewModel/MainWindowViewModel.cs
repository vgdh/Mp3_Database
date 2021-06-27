using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Id3;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Mp3_Database.Model;

namespace Mp3_Database.ViewModel
{

    public class MainWindowViewModel : ObservableObject, INotifyPropertyChanged
    {
        public ObservableCollection<Song> ExistingSongs { get; set; }
        public ObservableCollection<Song> NewSongsList { get; set; }
        public bool OneDirectoryDroped { get; set; } = false;


        public MainWindowViewModel()
        {
            RemoveSelectedSongsFromDatabaseCommand = new RelayCommand<object>(ExecuteRemoveSelectedSongsFromDatabaseCommand, CanExecuteRemoveSelectedSongsFromDatabaseCommand);
            RemoveeNewSongsFromDatabaseCommand = new RelayCommand(ExecuteRemoveSongsFromDatabaseCommand, CanExecuteRemoveSongsFromDatabaseCommand);
            OnlyAddToDatabaseCommand = new RelayCommand(ExecuteOnlyAddToDatabaseCommand, CanExecuteOnlyAddToDatabaseCommand);
            CopyNewSongsCommand = new RelayCommand(ExecuteCopyNewSongsCommand, CanExecuteCopyNewSongsCommand);

            DropNewSongsCommand = new RelayCommand<DragEventArgs>(Drop);

            ExistingSongs = new ObservableCollection<Song>(Repository.GetAllSongs());
            ExistingSongs.CollectionChanged += SongListsChangedEvent;

            NewSongsList = new();
            NewSongsList.CollectionChanged += SongListsChangedEvent;
        }

        private void SongListsChangedEvent(object sender, EventArgs e)
        {
            (RemoveeNewSongsFromDatabaseCommand as RelayCommand).NotifyCanExecuteChanged();
            (OnlyAddToDatabaseCommand as RelayCommand).NotifyCanExecuteChanged();
            (CopyNewSongsCommand as RelayCommand).NotifyCanExecuteChanged();

        }


        private int _duplicateSongCount;
        public int DuplicateSongCount
        {
            get => _duplicateSongCount;
            set
            {
                if (_duplicateSongCount != value)
                {
                    _duplicateSongCount = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }


        #region Удалить выделенные существующие песни из бызы
        public ICommand RemoveSelectedSongsFromDatabaseCommand { get; }
        public void ExecuteRemoveSelectedSongsFromDatabaseCommand(object _songs)
        {
            var items = (IList)_songs;
            var songs = items?.Cast<Song>().ToList();
            Repository.RemoveSongs(songs);
            foreach (var item in songs)
                ExistingSongs.Remove(item);

            UpdateDuplicateSongCounter();
        }
        public bool CanExecuteRemoveSelectedSongsFromDatabaseCommand(object _songs)
        {
            var items = (IList)_songs;
            var songs = items?.Cast<Song>().ToList();
            if (songs == null || songs.Count() < 1)
                return false;
            return true;
        }
        #endregion

        #region Удалить песни в списке новые из бызы
        public ICommand RemoveeNewSongsFromDatabaseCommand { get; }
        public void ExecuteRemoveSongsFromDatabaseCommand()
        {
            foreach (var item in Repository.RemoveSongs(NewSongsList))
            {
                Song found = ExistingSongs.FirstOrDefault(s => s.Artist == item.Artist && s.Title == item.Title);
                ExistingSongs.Remove(found);
            }
            NewSongsList.Clear();
        }

        public bool CanExecuteRemoveSongsFromDatabaseCommand()
        {
            if (NewSongsList.Count < 1)
                return false;
            return true;
        }
        #endregion

        #region Копировать новые песни в базу и в папку
        public ICommand CopyNewSongsCommand { get; }
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

            List<Song> newSongs = new List<Song>();
            foreach (Song song in NewSongsList)
            {
                if (!ExistingSongs.Any(eSong => eSong.Artist == song.Artist & eSong.Title == song.Title))
                    newSongs.Add(song);
            }

            foreach (var song in newSongs)
            {
                Task.Run(() =>
                {
                    CopyNewSongsToOutputFolder(song, outputDir);
                });
                song.ExistEarlier = true;
                ExistingSongs.Add(song);
            }

            Repository.AddSongs(newSongs);

            if (Directory.Exists($".\\{outputDir}"))
            {
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = $".\\{outputDir}\\";
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                }
            }
            else
            {
                MessageBox.Show("Что то пошло не так и не создалась папка для файлов");
            }

            UpdateDuplicateSongCounter();
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
        }
        #endregion

        #region Только добавить треки в базу
        public ICommand OnlyAddToDatabaseCommand { get; }
        public void ExecuteOnlyAddToDatabaseCommand()
        {
            Repository.AddSongs(NewSongsList.Where(x => x.ExistEarlier == false));
            foreach (var song in NewSongsList)
            {
                song.ExistEarlier = true;
            }
        }

        public bool CanExecuteOnlyAddToDatabaseCommand()
        {
            if (NewSongsList == null || NewSongsList.Count < 1 || NewSongsList.Count(song => song.ExistEarlier == false) == 0)
                return false;
            return true;
        }
        #endregion

        #region DropCommand
        public RelayCommand<DragEventArgs> DropNewSongsCommand { get; }
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
        #endregion

        /// <summary>
        /// Проверить есть ли новые треки в базе и отметить их
        /// </summary>
        private void UpdateDuplicateSongCounter()
        {
            DuplicateSongCount = 0;

            if (NewSongsList == null || NewSongsList.Count == 0)
                return;

            foreach (var item in NewSongsList)
            {
                if (ExistingSongs.Any(s => s.Artist == item.Artist && s.Title == item.Title))
                {
                    item.ExistEarlier = true;
                    DuplicateSongCount++;
                }
                else
                    item.ExistEarlier = false;
            }
        }

        /// <summary>
        /// Рекурсивно получить список путей файлов 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
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
                        filePathsListForReturn.Add(file);
                }
            }
            return filePathsListForReturn;
        }


        //https://github.com/mono/taglib-sharp

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
            StringBuilder sb = new StringBuilder();
            sb.Append(e.ToString());
            File.AppendAllText("log.txt", sb.ToString());
            sb.Clear();
        }

    }
}
