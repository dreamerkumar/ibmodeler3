using System;
using System.Collections.Generic;

namespace Ajubaa.Common.MatrixManipulations
{
    public class MatrixDataProcessor
    {
        /// <summary>
        /// tested satisfactorily on 19-May-2008
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="intRowNumber"></param>
        /// <returns></returns>
        public static double[] GetMatrixRow(double[,] matrix, int intRowNumber)
        {
            if (intRowNumber < 0)
                throw new Exception("GetMatrixRow::Passed row number is less than zero.");
            if (matrix.GetLength(0) < intRowNumber + 1)
                throw new Exception("GetMatrixRow::Index out of range. The matrix does not contain the passed row number");

            int intColumnCount = matrix.GetLength(1);
            double[] returnMatrix = new double[intColumnCount];
            for (int intCtr = 0; intCtr < intColumnCount; intCtr++)
                returnMatrix[intCtr] = matrix[intRowNumber,intCtr];
            return returnMatrix;
        }
        /// <summary>
        /// tested satisfactorily on 19-May-2008
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="intColumnNumber"></param>
        /// <returns></returns>
        public static double[] GetMatrixColumn(double[,] matrix, int intColumnNumber)
        {
            if (intColumnNumber < 0)
                throw new Exception("GetMatrixColumn::Passed column number is less than zero.");
            if (matrix.GetLength(1) < intColumnNumber + 1)
                throw new Exception("GetMatrixColumn::Index out of range. The matrix does not contain the passed column number");

            int intRowCount = matrix.GetLength(0);
            double[] returnMatrix = new double[intRowCount];
            for (int intCtr = 0; intCtr < intRowCount; intCtr++)
                returnMatrix[intCtr] = matrix[intCtr,intColumnNumber];
            return returnMatrix;
        }
        /// <summary>
        /// tested satisfactorily on 19-May-2008
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static double GetSumOfArrayProducts(double[] arr1, double[] arr2)
        {
            int intArrLength = arr1.Length;
            if (intArrLength < 1)
                throw new Exception("GetSumOfArrayProducts::passed array length less than 1");
            if (intArrLength != arr2.Length)
                throw new Exception("GetSumOfArrayProducts::passed arrays are not of the same length");
            double dblReturnVal = 0.0f;
            for (int intCtr = 0; intCtr < intArrLength; intCtr++)
            {
                dblReturnVal+= arr1[intCtr] * arr2[intCtr];
            }
            return dblReturnVal;
        }
        /// <summary>
        /// tested satisfactorily on 19-May-2008
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static double[,] MultiplyMatrices(double[,] matrix1, double[,] matrix2)
        {
            if (matrix1.GetLength(0) <= 0 || matrix2.GetLength(0) <= 0)
                throw new Exception("MultiplyMatrices::One or more of the input matrices does not have any rows");
            if (matrix1.GetLength(1) <= 0 || matrix1.GetLength(1) <= 0)
                throw new Exception("MultiplyMatrices::One or more of the input matrices does not have any columns");
            if (matrix1.GetLength(1) != matrix2.GetLength(0))
                throw new Exception("MultiplyMatrices::The columns on the first matrix should match the rows on the second");

            int intRows = matrix1.GetLength(0);
            int intColumns = matrix2.GetLength(1);

            double[,] returnedMatrix = new double[intRows,intColumns];

            for (int intRowCtr = 0; intRowCtr < intRows; intRowCtr++)
            {
                for (int intColumnCtr = 0; intColumnCtr < intColumns; intColumnCtr++)
                {
                    returnedMatrix[intRowCtr,intColumnCtr] = GetSumOfArrayProducts(GetMatrixRow(matrix1, intRowCtr), GetMatrixColumn(matrix2, intColumnCtr));
                }
            }
            return returnedMatrix;
        }
        /// <summary>
        /// tested satisfactorily on 23-May-2008
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double GetDeterminant(double[,] matrix)
        {
            int intTotalRows = matrix.GetLength(0);
            int intTotalCols = matrix.GetLength(1);

            if (intTotalRows != intTotalCols)
                throw new Exception("GetDeterminant: Determinant cannot be evaluated. Non square matrix");
            if(intTotalRows < 2)
                throw new Exception("GetDeterminant: Determinant cannot be evaluated. Matrix has less than two rows/columns.");

            if (intTotalRows == 2)
                return (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]);
            else
            {
                int intSignVal = 1;
                double dblReturnVal = 0.0;
                for (int intRow = 0; intRow < intTotalRows; intRow++)
                {
                    dblReturnVal+=  intSignVal * matrix[intRow,0] * GetDeterminant(GetMatrixExcludingThisRowAndColumn(matrix, intRow, 0));
                    intSignVal = intSignVal * -1;
                }
                return dblReturnVal;
            }
        }
        /// <summary>
        /// tested satisfactorily on 23-May-2008
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="intRowNumber"></param>
        /// <param name="intColumnNumber"></param>
        /// <returns></returns>
        private static double[,] GetMatrixExcludingThisRowAndColumn(double[,] matrix, int intRowNumber, int intColumnNumber)
        {
            int intTotalRows = matrix.GetLength(0);
            int intTotalCols = matrix.GetLength(1);

            if (intTotalRows < 2 || intTotalCols < 2)
                throw new Exception("GetMatrixExcludingThisRowAndColumn: Rows and columns cannot be less than two.");            
            if(intRowNumber >= intTotalRows || intRowNumber < 0)
                throw new Exception("GetMatrixExcludingThisRowAndColumn: passed row number is invalid");
            if (intColumnNumber >= intTotalCols || intColumnNumber < 0)
                throw new Exception("GetMatrixExcludingThisRowAndColumn: passed column number is invalid");

            double[,] newmatrix = new double[intTotalRows - 1, intTotalCols - 1];

            int intNewMatrixRow = 0;            
            for (int intRow = 0; intRow < intTotalRows; intRow++)
            {
                if (intRow == intRowNumber)
                    continue;

                int intNewMatrixCol = 0;
                for (int intCol = 0; intCol < intTotalCols; intCol++)
                {
                    if (intCol == intColumnNumber)
                        continue;

                    newmatrix[intNewMatrixRow, intNewMatrixCol] = matrix[intRow, intCol];

                    intNewMatrixCol++;
                }

                intNewMatrixRow++;
            }

            return newmatrix;
        }

        /// <summary>
        /// solves an equation using cramer's rule. 
        /// the solution array will contain the values of the variables in the sequence of their positions
        /// make sure the equations are arranged properly before calling this function. 
        /// In the below example, a, b and c are the coefficients and the k values are the equation constants
        /// a1X + b1Y + c1Z = k1
        /// a2X + b2Y + c2Z = k2
        /// a3X + b3Y + c3Z = k3
        /// passed testing on may 27, 2008
        /// </summary>
        /// <param name="coefficientmatrix"></param>
        /// <param name="equationconstants"></param>
        /// <returns></returns>
        public static double[] SolveEquation(double[,] coefficientmatrix, double[] equationconstants)
        {
            var intTotalRows = coefficientmatrix.GetLength(0);
            if (intTotalRows != equationconstants.Length)
                throw new Exception("SolveEquation: The number of equation constants should be equal to the number of equations");

            var dblDet = GetDeterminant(coefficientmatrix);
            if (dblDet == 0.0)
                throw new Exception("SolveEquation: Cannot solve equation. Determinant value is zero. This may be due to the absence of unique equations.");

            var variablevalues = new double[intTotalRows];
            for (var intVariable = 0; intVariable < intTotalRows; intVariable++)
            {
                variablevalues[intVariable] = GetDeterminant(GetMatrixWithThisReplacedColumn(coefficientmatrix, equationconstants, intVariable))/dblDet;
            }

            return variablevalues;
        }
        /// <summary>
        /// passed testing on may 27, 2008
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="newcolumn"></param>
        /// <param name="intColumnPos"></param>
        /// <returns></returns>
        private static double[,] GetMatrixWithThisReplacedColumn(double[,] matrix, IList<double> newcolumn, int intColumnPos)
        {
            var intTotalRows = matrix.GetLength(0);
            var intTotalColumns = matrix.GetLength(1);
            var returnmatrix = new double[intTotalRows, intTotalColumns];
            
            if (intColumnPos < 0 || intColumnPos >= matrix.GetLength(1))
                throw new Exception("ReplaceColumn: Invalid column number passed.");
            if (intTotalRows != newcolumn.Count)
                throw new Exception("ReplaceColumn: The length of the new column is not equal to the number of rows of the matrix.");

            for (var intRowNo = 0; intRowNo < intTotalRows; intRowNo++)
            {
                for (var intColNo = 0; intColNo < intTotalColumns; intColNo++)
                {
                    if(intColNo == intColumnPos)
                        returnmatrix[intRowNo, intColNo] = newcolumn[intRowNo];
                    else
                        returnmatrix[intRowNo, intColNo] = matrix[intRowNo, intColNo];
                }
            }

            return returnmatrix;
        }
    }
}
