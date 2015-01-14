using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using NDepend.Helpers.FileDirectoryPath;

namespace ScenarioTools.Util
{
    public enum FileType
    {
        Shapefile = 0,
        Smpfile = 1,
        SecondarySmpfile = 2
    }

    public static class FileUtil
    {

        public static string Absolute2Relative(string filePath, string dirPath)
        {
            string result;
            result = filePath;
            if (filePath != "" && Directory.Exists(dirPath))
            {
                if (Path.IsPathRooted(filePath))
                {
                    result = GetRelativePath(filePath, dirPath);
                }
            }
            return result;
        }

        public static string AddTrailingBackslash(string directoryPath)
        {
            string returnString = directoryPath;
            string bSlash = "\\";
            if (!directoryPath.EndsWith(bSlash))
            {
                returnString = directoryPath + bSlash;
            }
            return returnString;
        }

        public static string FindFile(FileType fileType, OpenFileDialog openFileDialog,
                                      string defaultFilename)
        {
            string fileName = "";
            string folder = "";
            string path = "";
            openFileDialog.RestoreDirectory = true;
            switch (fileType)
            {
                case FileType.Shapefile:
                    openFileDialog.Filter = "Shapefiles (*.shp)|*.shp";
                    break;
                case FileType.Smpfile:
                    openFileDialog.Filter = "Time series file (*.smp)|*.smp|All Files|*.*";
                    break;
            }
            openFileDialog.FileName = defaultFilename;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
            }
            return path;
        }

        public static string[] GetFileContents(string filePath)
        {
            try
            {
                // Open the file.
                StreamReader sr = File.OpenText(filePath);

                // Read the contents into a list.
                List<string> fileContents = new List<string>();
                while (!sr.EndOfStream)
                {
                    fileContents.Add(sr.ReadLine());
                }

                // Close the file.
                sr.Close();

                // Return the file contents as an array.
                return fileContents.ToArray();
            }
            catch
            {
                return new string[0];
            }
        }

        public static string GetFileLengthString(double fileLength)
        {
            string fileSize;
            if (fileLength < 1024.0)
            {
                fileSize = (int)fileLength + " bytes";
            }
            else
            {
                fileLength /= 1024.0;
                if (fileLength < 1024.0)
                {
                    fileSize = String.Format("{0:0.00}", fileLength) + " KB";
                }
                else
                {
                    fileLength /= 1024.0;
                    if (fileLength < 1024.0)
                    {
                        fileSize = String.Format("{0:0.00}", fileLength) + " MB";
                    }
                    else
                    {
                        fileLength /= 1024.0;
                        fileSize = String.Format("{0:0.00}", fileLength) + " GB";
                    }
                }
            }

            // Return the result.
            return fileSize;
        }

        /// <summary>
        /// Return a relative path name, given a file pathname and a 
        /// directory path relative to which the result relative 
        /// pathname is to be referenced.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="directoryPath"></param>
        /// <returns>Relative path</returns>
        public static string GetRelativePath(string filePath, string directoryPath)
        {
            if (filePath == null)
            {
                return "";
            }
            if (filePath == "")
            {
                return "";
            }
            string pathRoot = Path.GetPathRoot(filePath);
            if (string.IsNullOrEmpty(pathRoot))
            {
                if (pathRoot == null)
                {
                    // filePath is null
                    return "";
                }
                else
                {
                    // filePath is already a relative path or empty
                    return filePath;
                }
            }
            string relativePath = filePath;
            if (directoryPath != "")
            {
                string dir = AddTrailingBackslash(directoryPath);
                if (Directory.Exists(dir))
                {
                    // Ned TODO: Need code to handle exceptions: maybe try...catch
                    Uri fileUri = new Uri(filePath);
                    Uri directoryUri = new Uri(dir);
                    Uri relative = directoryUri.MakeRelativeUri(fileUri);
                    relativePath = Uri.UnescapeDataString(relative.ToString());
                    relativePath = relativePath.Replace("/", "\\");
                }
            }
            return relativePath;
        }

        public static bool IsValidFilename(string name)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars(); //returns invalid characters
            for (int i = 0; i < invalidChars.Length; i++)
            {
                if (name.Contains(Convert.ToString(invalidChars[i])))
                {
                    return false;
                }
            }
            return true;
        }

        public static string Relative2Absolute(string relativePath, string directoryPath)
        {
            if (relativePath == null)
            {
                return "";
            }
            if (relativePath == "")
            {
                return "";
            }
            string absolutePath;
            string pathRoot = Path.GetPathRoot(relativePath);
            if (string.IsNullOrEmpty(pathRoot))
            {
                // relativePath really is a relative path
                if (relativePath != null)
                {
                    string relPathDot = relativePath;
                    if (!relativePath.StartsWith("."))
                    {
                        relPathDot = ".\\" + relativePath;
                    }
                    NDepend.Helpers.FileDirectoryPath.FilePathRelative relPath
                        =  new FilePathRelative(relPathDot);
                    if (Directory.Exists(directoryPath))
                    {
                        NDepend.Helpers.FileDirectoryPath.DirectoryPathAbsolute absDir
                            = new DirectoryPathAbsolute(directoryPath);
                        NDepend.Helpers.FileDirectoryPath.FilePathAbsolute absPath
                            = relPath.GetAbsolutePathFrom(absDir);
                        absolutePath = absPath.Path;
                    }
                    else
                    {
                        absolutePath = relativePath;
                    }
                }
                else
                {
                    absolutePath = "";
                }
            }
            else
            {
                // relativePath already is an absolute path
                absolutePath = relativePath;
            }
            return absolutePath;
        }

        public static void WriteFile(string path, string[] contents)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                try
                {
                    for (int i = 0; i < contents.Length; i++)
                    {
                        sw.WriteLine(contents[i]);
                    }
                }
                catch { }

                if (sw != null)
                {
                    try
                    {
                        sw.Close();
                    }
                    catch { }
                }
            }
        }

    }
}
