using ScanCode.Properties;
using Ookii.Dialogs.Wpf;
using Microsoft.Win32;
using System.IO;
using System;

namespace ScanCode.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileDialogHelper
    {
        public static string DefaultDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Settings.Default.DefaultDirectory) || !Directory.Exists(Settings.Default.DefaultDirectory))
                {
                    UpdateDefaultDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "quang"));
                    Directory.CreateDirectory(Settings.Default.DefaultDirectory);
                }
                return Settings.Default.DefaultDirectory;
            }
        }

        public static void UpdateDefaultDirectory(string directory)
        {
            if (directory.Equals(Settings.Default.DefaultDirectory))
            {
                return;
            }
            Settings.Default.DefaultDirectory = directory;
            Settings.Default.Save();
        }

        /// <summary>
        /// Open folder browser dialog
        /// </summary>
        /// <param name="startupPath"></param>
        /// <returns></returns>
        public static VistaOpenFileDialog OpenFileDialog(string filter = null, bool checkFileExists = true, bool checkPathExists = true, string startupPath = null)
        {
            return new VistaOpenFileDialog
            {
                CheckFileExists = checkFileExists,
                CheckPathExists = checkPathExists,
                Filter = filter,
            };
        }

        public static SaveFileDialog CreateSaveAsExcelFileDialog(string defaultFileName)
        {
            return new SaveFileDialog
            {
                DefaultExt = Constants.ExcelExtension,
                FileName = defaultFileName,
                Filter = Constants.ExcelFilter,
                InitialDirectory = DefaultDirectory
            };
        }

    }
}
