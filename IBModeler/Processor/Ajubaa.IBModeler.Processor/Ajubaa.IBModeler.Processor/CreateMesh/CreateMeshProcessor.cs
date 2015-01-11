using System.IO;
using Ajubaa.Common;
using Ajubaa.IBModeler.ImageAlterations;
using Ajubaa.IBModeler.PtsToPolygons;
using Ajubaa.SurfaceSmoother.FullModelSmoother;

namespace Ajubaa.IBModeler.Processor
{
    public static class CreateMeshProcessor
    {
        public static ModelMeshAndPositionNeighbors CreateMesh(CreateMeshContract inputParams)
        {
            var logger = new Logger(inputParams.LogFilePath);

            Stream moldDataStream;
            logger.Log("Start applying images");
            ApplyImages(inputParams, out moldDataStream);
            logger.Log("End applying images");

            return CreateModelFromMold(inputParams.PtDensity, moldDataStream, logger);
        }

        public static void ApplyImages(CreateMeshContract inputParams, out Stream moldData)
        {
            var logger = new Logger(inputParams.LogFilePath);

            var angles = inputParams.ClickInputs.Angles;

            Stream moldDataPtr = null;
            for (var index = 0; index < angles.Length; index++)
            {
                ApplyImage(inputParams, index, ref moldDataPtr, logger);
            }
            moldData = moldDataPtr;
        }

        public static void ApplyImage(CreateMeshContract inputParams, int index, ref Stream moldDataPtr, Logger logger)
        {
            MainProcessor.ApplyImage(inputParams, index, ref moldDataPtr, logger); 
        }

        public static ModelMeshAndPositionNeighbors CreateModelFromMold(int ptDensity, Stream moldDataStream, Logger logger)
        {
            var createModelInfo = new CreateModelInfo
            {
                MoldData = moldDataStream,
                Minx = 1, Maxx = ptDensity,
                Miny = 1, Maxy = ptDensity,
                Minz = 1, Maxz = ptDensity
            };

            logger.Log("Start model creation from mold points");
            var ptsToPolygons = new PointsToPolygons(createModelInfo);
            var model = ptsToPolygons.Process();
            logger.Log("End model creation from mold points.");

            if (moldDataStream != null)
                moldDataStream.Close();

            var neighboringIndices = model == null? null : PaulBourkeSmoother.GetPositionNeighbors(model.Positions.Count, model.TriangleIndices);

            return new ModelMeshAndPositionNeighbors
            {
                MeshGeometry =model, 
                PositionNeighbors = neighboringIndices
            };
        }
    }
}
