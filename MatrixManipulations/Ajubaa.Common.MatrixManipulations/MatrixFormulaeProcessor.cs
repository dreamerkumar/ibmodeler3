using System;
using System.Text;

namespace Ajubaa.Common.MatrixManipulations
{
    public class MatrixFormulaeProcessor
    {
        public static string[] GetMatrixRow(string[,] matrix, int intRowNumber)
        {
            if (intRowNumber < 0)
                throw new Exception("GetMatrixRow::Passed row number is less than zero.");
            if (matrix.GetLength(0) < intRowNumber + 1)
                throw new Exception("GetMatrixRow::Index out of range. The matrix does not contain the passed row number");

            int intColumnCount = matrix.GetLength(1);
            string[] returnMatrix = new string[intColumnCount];
            for (int intCtr = 0; intCtr < intColumnCount; intCtr++)
                returnMatrix[intCtr] = matrix[intRowNumber,intCtr];
            return returnMatrix;
        }

        public static string[] GetMatrixColumn(string[,] matrix, int intColumnNumber)
        {
            if (intColumnNumber < 0)
                throw new Exception("GetMatrixColumn::Passed column number is less than zero.");
            if (matrix.GetLength(1) < intColumnNumber + 1)
                throw new Exception("GetMatrixColumn::Index out of range. The matrix does not contain the passed column number");

            int intRowCount = matrix.GetLength(0);
            string[] returnMatrix = new string[intRowCount];
            for (int intCtr = 0; intCtr < intRowCount; intCtr++)
                returnMatrix[intCtr] = matrix[intCtr,intColumnNumber];
            return returnMatrix;
        }

        public static string GetSumOfArrayProducts(string[] arr1, string[] arr2)
        {
            int intArrLength = arr1.Length;
            if (intArrLength < 1)
                throw new Exception("GetSumOfArrayProducts::passed array length less than 1");
            if (intArrLength != arr2.Length)
                throw new Exception("GetSumOfArrayProducts::passed arrays are not of the same length");
            string strReturnVal = string.Empty;
            for (int intCtr = 0; intCtr < intArrLength; intCtr++)
            {
                if (string.IsNullOrEmpty(arr1[intCtr]) || string.IsNullOrEmpty(arr2[intCtr]))
                    throw new Exception("Blank strings are not allowed for matrix values");

                
                if (arr1[intCtr] != "0" && arr2[intCtr] != "0")
                {
                    int intSign = ExtractSign(ref arr1[intCtr]) * ExtractSign(ref arr2[intCtr]);                    
                    
                    string strVal = string.Empty;
                    if (arr1[intCtr] == "1")
                        strVal = arr2[intCtr];
                    else if (arr2[intCtr] == "1")
                        strVal = arr1[intCtr];
                    else
                    {
                        if(arr1[intCtr].ToUpper().StartsWith("SIN") 
                            || arr1[intCtr].ToUpper().StartsWith("COS") 
                            || arr1[intCtr].ToUpper().StartsWith("TAN") 
                            || arr1[intCtr].ToUpper().StartsWith("COT") 
                            || arr1[intCtr].ToUpper().StartsWith("SEC") 
                            || arr1[intCtr].ToUpper().StartsWith("COSEC"))
                            
                            strVal = arr2[intCtr] + "*" + arr1[intCtr];
                        else
                            strVal = arr1[intCtr] + "*" + arr2[intCtr];
                    }

                    string strSign = string.Empty;
                    if (intSign == 1)
                        strSign = "+";
                    else if (intSign == -1)
                        strSign = "-";
                    else
                        throw new Exception("Invalid sign");

                    if (strSign == "-")//- sign is always appended
                        strReturnVal += strSign;
                    else
                    {
                        if (!string.IsNullOrEmpty(strReturnVal))//append only if other values exist
                            strReturnVal += strSign;
                    }
                    strReturnVal += strVal;
                }
            }

            if (string.IsNullOrEmpty(strReturnVal))
                strReturnVal = "0";
            return strReturnVal;
        }

        private static int ExtractSign(ref string strVal)
        {
            int intSign = 1;
            if (strVal.StartsWith("-"))
                intSign = -1;
            strVal = strVal.Trim(new char[] { '+', '-', ' ' });
            return intSign;
        }

        public static string[,] MultiplyMatrices(string[,] matrix1, string[,] matrix2)
        {
            if (matrix1.GetLength(0) <= 0 || matrix2.GetLength(0) <= 0)
                throw new Exception("MultiplyMatrices::One or more of the input matrices does not have any rows");
            if (matrix1.GetLength(1) <= 0 || matrix1.GetLength(1) <= 0)
                throw new Exception("MultiplyMatrices::One or more of the input matrices does not have any columns");
            if (matrix1.GetLength(1) != matrix2.GetLength(0))
                throw new Exception("MultiplyMatrices::The columns on the first matrix should match the rows on the second");

            int intRows = matrix1.GetLength(0);
            int intColumns = matrix2.GetLength(1);

            string[,] returnedMatrix = new string[intRows,intColumns];

            for (int intRowCtr = 0; intRowCtr < intRows; intRowCtr++)
            {
                for (int intColumnCtr = 0; intColumnCtr < intColumns; intColumnCtr++)
                {
                    returnedMatrix[intRowCtr,intColumnCtr] = GetSumOfArrayProducts(GetMatrixRow(matrix1, intRowCtr), GetMatrixColumn(matrix2, intColumnCtr));
                }
            }
            return returnedMatrix;
        }

        public static string WriteMatrixData(string[,] matrix)
        {
            var objSb = new StringBuilder();
            var intRows = matrix.GetLength(0);
            var intColumns = matrix.GetLength(1);
            for(var intCtr = 0; intCtr < intRows; intCtr++)
            {
                for(var intColNo = 0; intColNo < intColumns; intColNo++)
                {
                    objSb.Append(matrix[intCtr, intColNo]);
                    objSb.Append(" ");
                }
                objSb.AppendLine();
            }
            return objSb.ToString();
        }
    }
}
