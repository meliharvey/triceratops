using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Triceratops
{
    public class GeometryWrapper
    {
        public dynamic Geometry;
        public dynamic Child;
        public dynamic Material;

        public GeometryWrapper(dynamic geometry, dynamic child, dynamic material)
        {
            Geometry = geometry;
            Child = child;
            Material = material;
        }
    }

    public class MaterialWrapper
    {
        public dynamic Material;

        public MaterialWrapper(dynamic material)
        {
            Material = material;
        }
    }

    public class SceneWrapper
    {
        public dynamic ExportObject;

        public SceneWrapper(dynamic exportObject)
        {
            ExportObject = exportObject;
        }
    }
}