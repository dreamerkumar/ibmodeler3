namespace Ajubaa.IBModeler.UI.Main
{
    public class ModelSettingsEventArgs
    {
        public int MeshDensity { get; set; }

        public bool SaveProcessedImages { get; set; }

        public bool AnalyzeCutOutData { get; set; }

        /// <summary>
        /// sets a maximum angle upto which we should process the images
        /// any images beyond this will not be used in creating the mesh
        /// </summary>
        public int MaxAngleOfImageToProcessInDegress { get; set; }
    }
}
