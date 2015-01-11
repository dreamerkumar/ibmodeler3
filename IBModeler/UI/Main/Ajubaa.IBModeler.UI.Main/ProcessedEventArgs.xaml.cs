using System;
using Ajubaa.IBModeler.Processor;

namespace Ajubaa.IBModeler.UI.Main
{
    internal class ProcessedEventArgs : EventArgs
    {
        /// <summary>
        /// original contract for creating the model
        /// </summary>
        public CreateMeshContract CreateMeshContract { get; set; }

        /// <summary>
        /// The created model mesh and position neighbors that can be used for smoothening
        /// </summary>
        public ModelMeshAndPositionNeighbors ModelMeshAndPositionNeighbors { get; set; }

    }
}