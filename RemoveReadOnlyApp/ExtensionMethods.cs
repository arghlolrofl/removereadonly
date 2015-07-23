using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoveReadOnlyApp {
    public static class ExtensionMethods {
        public static bool IsReadOnly(this FileInfo file) {
            return ((File.GetAttributes(file.FullName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
        }

        public static string GetAttributeString(this FileInfo file) {
            // check whether a file is hidden
            bool isHidden = ((File.GetAttributes(file.FullName) & FileAttributes.Hidden) == FileAttributes.Hidden);

            // check whether a file has archive attribute
            bool isArchive = ((File.GetAttributes(file.FullName) & FileAttributes.Archive) == FileAttributes.Archive);

            // check whether a file is system file
            bool isSystem = ((File.GetAttributes(file.FullName) & FileAttributes.System) == FileAttributes.System);

            string result = String.Empty;
            result += file.IsReadOnly() ? 'r' : '-';
            result += isHidden ? 'h' : '-';
            result += isArchive ? 'a' : '-';
            result += isSystem ? 's' : '-';

            return result;
        }
    }
}
