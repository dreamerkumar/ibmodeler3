using System;
using System.IO;
using Ajubaa.Common;

namespace Ajubaa.IBModeler.Help
{
    [Serializable]
    public class HideScreenStartDialogFlags
    {
        public bool BackgroundScreen { get; set; }

        public bool ClickInputs { get; set; }

        //public bool AutoConfigure { get; set; }

        public bool CreateReady { get; set; }

        public bool MeshCreated { get; set; }

        //public bool AddSkinDialog { get; set; }

        private static HideScreenStartDialogFlags _flags;
        public static HideScreenStartDialogFlags Flags
        {
            get
            {
                if(_flags == null)
                {
                    var filePath = GetFilePath();
                    if(File.Exists(filePath)) _flags = XmlSerializerHelper.Deserialize<HideScreenStartDialogFlags>(filePath);
                    if(_flags == null) _flags = new HideScreenStartDialogFlags();
                }
                return _flags;
            }
            set
            {
                _flags = value;
                if(_flags != null)
                {
                    var filePath = GetFilePath();
                    XmlSerializerHelper.Serialize(_flags, filePath);
                }
            }
        }

        private static string GetFilePath()
        {
            var outputPath = ExecutionDirInfoHelper.GetExecutionPath();
            return string.Format(@"{0}\HideScreenStartDialogFlags.xml", outputPath);
        }
    }
}
