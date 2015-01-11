using System;
using System.Xml;
using Ajubaa.IBModeler.ImgToObj;
using System.IO;

namespace Ajubaa.IBModeler.InstructionFileHandler
{
    public class XmlInstructionFileHandler
    {
        /// <summary>
        /// should have the create mold specifications in the xml file if this function is called
        /// </summary>
        /// <param name="strXmlFilePath"></param>
        public static void ReadAndApplyInstructions(string strXmlFilePath)
        {
            var dtStart = DateTime.Now;
            var doc = new XmlDocument();
            doc.Load(strXmlFilePath);
            //Get the current directory info from the xml file path
            strXmlFilePath = strXmlFilePath.Replace(@"/", @"\");
            var strCurrDir = strXmlFilePath.Substring(0, strXmlFilePath.LastIndexOf(@"\") + 1);
            var objMoldNode = doc.DocumentElement.SelectSingleNode("Mold");
            if (objMoldNode == null)
                throw new Exception("The required node 'Mold' was not found");
            var strMoldFilePath = CommonFunctions.GetFilePathInfo(objMoldNode, "Name", strCurrDir, true);
            var blnCreateNewMold = false;
            var strDiscardExistingFile = CommonFunctions.GetAttributeValue(objMoldNode, "DiscardExistingFile", false);
            if (strDiscardExistingFile.ToLower().Trim() == "true")
                blnCreateNewMold = true;
            else
            {//check if a mold file already exists
                if (!File.Exists(strMoldFilePath))
                    blnCreateNewMold = true;
            }
            if (blnCreateNewMold)
            {
                CreateNewMold(strMoldFilePath, objMoldNode, "PointDensity", false);
            }

            ReadAndApplyInstructions(doc, strCurrDir, strMoldFilePath);
            var dtEnd = DateTime.Now;
            var diff = dtEnd - dtStart;
        }

        public static void CreateNewMold(string strMoldFilePath, XmlNode objMoldNode, string strPointDensityAttribute, bool blnPtDensityRequired)
        {
            //Check if point density has been supplied
            var strPtDensity = CommonFunctions.GetAttributeValue(objMoldNode, strPointDensityAttribute, blnPtDensityRequired);
            if (!string.IsNullOrEmpty(strPtDensity))
            {
                uint uintPtDensity;
                if (!UInt32.TryParse(strPtDensity, out uintPtDensity))
                    throw new Exception("An invalid value exists for the PointDensity attribute. An unsigned int value was expected.");
                if (uintPtDensity < 3)
                    throw new Exception("The PointDensity should have a value greater than 2");
                MoldFileCreator.CreateNewMoldFile(strMoldFilePath, uintPtDensity);
            }
            else
                MoldFileCreator.CreateNewMoldFile(strMoldFilePath);
        }

        public static void ReadAndApplyInstructions(XmlDocument doc, string strCurrDir, string strMoldFilePath)
        {
            //var dblMaxDataLossPercent = AnalyzeDataLossHandler.AnalyzeDataLosses(doc.DocumentElement.SelectSingleNode("Images"), doc.DocumentElement.SelectSingleNode("AnalyzeDataLoss"), strCurrDir);
            //ApplyImageHandler.ApplyImageInstructions(doc.DocumentElement.SelectSingleNode("Images"), strCurrDir, strMoldFilePath);
        }
    }
}
