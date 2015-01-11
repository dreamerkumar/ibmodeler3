using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace Ajubaa.IBModeler.Processor
{
    /// <summary>
    /// return value for create mesh processor
    /// </summary>
    [Serializable]
    public class ModelMeshAndPositionNeighbors
    {
        private MeshGeometry3D _meshGeometry;

        /// <summary>
        /// model mesh
        /// </summary>
        internal MeshGeometry3D MeshGeometry
        {
            set
            {
                _meshGeometry = value;
                //required special handling for wpf object
                //freeze it so that it can be read by calling threads
                if(_meshGeometry.CanFreeze)
                    _meshGeometry.Freeze();
            }
        }

        public MeshGeometry3D GetModifiableMesh()
        {
            //required special handling for wpf object
            //when returned by a thread, _meshGeometry will always be read only (see freeze on the setter)
            //return a modifiable copy
            var memory = new MemoryStream();
            XamlWriter.Save(_meshGeometry, memory);
            memory.Flush();
            memory.Seek(0, SeekOrigin.Begin);
            return XamlReader.Load(memory) as MeshGeometry3D;
        }

        /// <summary>
        /// used for smoothening of the model
        /// </summary>
        public HashSet<int>[] PositionNeighbors { get; set; }
    }
}
