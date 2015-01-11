using System.IO;
using System.Linq;

namespace Ajubaa.IBModeler.Common
{
    public static class GetImageFilesHelper
    {
        public static string[] GetImageFilesFromLocation(string imageDirPath)
        {
            var filePaths = Directory.GetFiles(imageDirPath);
            if (filePaths == null || filePaths.Length <= 0)
                return null;

            var filePathList = filePaths.Where(x => x.ToLower().EndsWith(".jpg") || x.ToLower().EndsWith(".jpeg")).ToList();

            if (filePathList == null || filePathList.Count <= 0)
                return null;

            filePathList.Sort();
            return filePathList.ToArray();
        }
    }
}
