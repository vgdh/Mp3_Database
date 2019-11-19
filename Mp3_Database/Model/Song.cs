//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Mp3_Database.Model
//{
//    public class song : INotifyPropertyChanged
//    {
//        private string _authorName;
//        private string _songName;
//        private int price;

//        public int id { get; set; }

//        public string author_name
//        {
//            get { return _authorName; }
//            set
//            {
//                _authorName = value;
//                OnPropertyChanged("author_name");
//            }
//        }
//        public string song_name
//        {
//            get { return _songName; }
//            set
//            {
//                _songName = value;
//                OnPropertyChanged("song_name");
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//        public void OnPropertyChanged([CallerMemberName]string prop = "")
//        {
//            if (PropertyChanged != null)
//                PropertyChanged(this, new PropertyChangedEventArgs(prop));
//        }
//    }
//}
