using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoveReadOnlyApp {
    public class Watcher {
        FileSystemWatcher _watcher;
        Action<string> _logAction;

        public FileInfo File { get; set; }
        public DirectoryInfo Folder { get; set; }


        public Watcher(FileInfo fileToWatch, Action<string> logAction) {
            File = fileToWatch;
            if (!File.Exists)
                throw new FileNotFoundException("Could not find file: " + File.FullName);

            _logAction = logAction;

            _watcher = new FileSystemWatcher() { 
                Path = File.DirectoryName,
                NotifyFilter = NotifyFilters.Attributes,
                Filter = File.Name,
                EnableRaisingEvents = true
            };

            _watcher.Changed += new FileSystemEventHandler(OnFileAttributesChanged);

            removeReadOnlyFlag();
        }

        
        private void OnFileAttributesChanged(object sender, FileSystemEventArgs e) {
            _logAction(String.Format(
                "Changed detected: [ {0} ] {1}", 
                File.GetAttributeString(),
                File.FullName
            ));

            removeReadOnlyFlag();
        }

        private void removeReadOnlyFlag() {
            if (File.IsReadOnly()) {
                File.IsReadOnly = false;
                _logAction("Removed read-only flag for: " + File.FullName);                
            }
        }
    }
}
