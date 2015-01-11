using System.IO;
using System.Linq;
using System.Management;

namespace Ajubaa.IBModeler.UI.Main
{
    public static class HardwareId
    {
        public static string GetHardWareId()
        {
            return GetUniqueId("C");
        }

        private static string GetUniqueId(string drive)
        {
            if (drive == string.Empty)
            {
                //Find first drive
                foreach (var compDrive in DriveInfo.GetDrives().Where(compDrive => compDrive.IsReady))
                {
                    drive = compDrive.RootDirectory.ToString();
                    break;
                }
            }

            if (drive.EndsWith(":\\"))
            {
                //C:\ -> C
                drive = drive.Substring(0, drive.Length - 2);
            }

            var volumeSerial = GetVolumeSerial(drive);
            var cpuId = GetCpuid();

            //Mix them up and remove some useless 0's
            return cpuId.Substring(13) + cpuId.Substring(1, 4) + volumeSerial + cpuId.Substring(4, 4);
        }

        private static string GetVolumeSerial(string drive)
        {
            var disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            disk.Get();

            var volumeSerial = disk["VolumeSerialNumber"].ToString();
            disk.Dispose();

            return volumeSerial;
        }

        private static string GetCpuid()
        {
            var cpuInfo = "";
            var managClass = new ManagementClass("win32_processor");
            var managCollec = managClass.GetInstances();

            //Get only the first CPU's ID
            foreach (var managObj in managCollec)
            {
                cpuInfo = managObj.Properties["processorID"].Value.ToString();
                break;
            }

            return cpuInfo;
        }
    }
}
