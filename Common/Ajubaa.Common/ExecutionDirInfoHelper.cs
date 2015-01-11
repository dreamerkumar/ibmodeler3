using System;
using System.IO;

namespace Ajubaa.Common
{
    public class ExecutionDirInfoHelper
    {
        /// <summary>
        /// Gets the directory of the project that started the execution
        /// </summary>
        /// <returns></returns>
        public static string GetStartProjectDirectoryPath()
        {
            var assemblyDirInfo = new DirectoryInfo(GetExecutionPath());
            return assemblyDirInfo.Parent.Parent.FullName;
        }

        public static string GetExecutionPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetInputDirPath()
        {
            return GetStartProjectDirectoryPath() + @"\Input";
        }

        public static string GetOutputDirPath()
        {
            return GetStartProjectDirectoryPath() + @"\Output";
        }

        public static string GetTimeStampBasedFolderName()
        {
            return DateTime.Now.ToString("dd_MMMM_yyyy_HH_mm_ss");
        }

        public static string CreateUniqueOutputPath()
        {
            var outputFolder = GetOutputDirPath() + @"\" + GetTimeStampBasedFolderName();
            Directory.CreateDirectory(outputFolder);
            return outputFolder;
        }

        public static string GetLogFilePath()
        {
            return string.Format(@"{0}\{1}", GetOutputDirPath(), "log.txt");
        }

        public static string GetLogFilePath(string logFileName)
        {
            return string.Format(@"{0}\{1}", GetOutputDirPath(), logFileName);
        }

        public static string GetLogFilePath(string outputDirPath, string logFileName)
        {
            return string.Format(@"{0}\{1}", outputDirPath, logFileName);
        }
    }
}
