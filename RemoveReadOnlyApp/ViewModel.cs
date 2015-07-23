using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RemoveReadOnlyApp {
    public class ViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string propName = "") {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propName));
        }
        
        private ObservableCollection<string> _logList;
        public ObservableCollection<string> LogList {
            get { return _logList; }
            set { _logList = value; RaisePropertyChanged(); }
        }
        

        List<Watcher> _watchList;
        List<FileInfo> _filesToWatch;
        Dispatcher _dispatcher;

        public ViewModel(Dispatcher dispatcher) {
            _dispatcher = dispatcher;
            LogList = new ObservableCollection<string>();
            Log("Initializing application ...");

            _watchList = new List<Watcher>();
            _filesToWatch = new List<FileInfo>();

            if (Properties.Settings.Default.Files != null && Properties.Settings.Default.Files.Count > 0) {
                foreach (var filePath in Properties.Settings.Default.Files) {
                    if (!File.Exists(filePath)) {
                        Log("File not found: " + filePath);
                        continue;
                    }

                    _filesToWatch.Add(new FileInfo(filePath));
                }
            }

            if (Properties.Settings.Default.Folders != null && Properties.Settings.Default.Folders.Count > 0) {
                foreach (var folderPath in Properties.Settings.Default.Folders) {
                    if (!Directory.Exists(folderPath)) {
                        Log("Directory not found: " + folderPath);
                        continue;
                    }

                    var dir = new DirectoryInfo(folderPath);
                    foreach (var pattern in Properties.Settings.Default.FilePattern) {
                        var matches = dir.GetFiles(pattern);
                        _filesToWatch.AddRange(matches);
                    }
                }
            }
            
            Action<string> logAction = Log;
            foreach (var file in _filesToWatch) {
                Log("Adding watcher for: " + file.FullName);
                _watchList.Add(new Watcher(file, logAction));
            }
            
            Log(_watchList.Count + " Watchers are up and running");
        }

        public void Log(string message) {
            _dispatcher.Invoke(
                (Action)(() => {
                    LogList.Add(String.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));    
                })
            );            
        }

        public void Log(string message, string file) {
            _dispatcher.Invoke(
                (Action)(() => {
                    LogList.Add(String.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));
                })
            );
        }
    }
}
