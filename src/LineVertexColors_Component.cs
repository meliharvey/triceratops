using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class LineVertexColors_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public LineVertexColors_Component()
          : base("Line Vertex Colors", "LineVertexColors",
              "Create a Three.js line geometry with colored vertices",
              "Triceratops", "Geometry")
        {
        }

        // Place in a partition
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Line", "L", "Line geometry", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Line name", GH_ParamAccess.item, "");
            pManager.AddGenericParameter("Line Material", "M", "Line material", GH_ParamAccess.item);
            pManager.AddColourParameter("Colors", "C", "Vertex colors (count must match number of vertices)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Decimal Accuracy", "D", "The number of decimal points in accuracy (impacts export size)", GH_ParamAccess.item, 3);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Geometry", "G", "The line geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declate variables
            Curve curve = null;
            string name = "";
            Material material = null;
            List<Color> colors = new List<Color>();
            int decimalAccuracy = 3;

            // Reference the inputs
            DA.GetData(0, ref curve);
            DA.GetData(1, ref name);
            DA.GetData(2, ref material);
            if (!DA.GetDataList(3, colors))
                return;
            DA.GetData(4, ref decimalAccuracy);

            // Throw error message if curve cannot be converted to a polyline
            if (!curve.IsPolyline())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input must be polyine, curves are not allowed.");
                return;
            }

            // Convert curve to polyline
            Polyline polyline;
            curve.TryGetPolyline(out polyline);

            // Get the center of the object so the bufferGeometry can be given local coordinates
            Point3d center = polyline.BoundingBox.Center;

            // Produce a buffer geometry from the polyline
            dynamic geometry = new BufferGeometry(polyline, center, decimalAccuracy);
            geometry.AddVertexColors(colors);

            // If dashed material is used add the line distance to the buffer geometry
            if (material != null && string.Equals(material.Data.Type, "LineDashedMaterial"))
                geometry.AddLineDistance();

            //Create a threejs object definition
            dynamic lineObject = new ExpandoObject();
            lineObject.Uuid = Guid.NewGuid();
            lineObject.Name = name;
            lineObject.Type = "Line";
            lineObject.Geometry = geometry.Uuid;
            lineObject.Matrix = new Matrix(center).Array;

            // Create file object
            Object3d object3d = new Object3d(lineObject);
            object3d.AddGeometry(geometry);

            // If there is a material, add the material, textures, and images
            if (material != null)
            {
                lineObject.Material = material.Data.Uuid;

                material.Data.vertexColors = 2;
                object3d.AddMaterial(material.Data);

                if (material.Textures != null)
                    foreach (dynamic texture in material.Textures)
                        object3d.AddTexture(texture);
                if (material.Images != null)
                    foreach (dynamic image in material.Images)
                        object3d.AddImage(image);
            }
            else
            {
                Guid uuid = Guid.NewGuid();
                lineObject.Material = uuid;

                // Build the material object
                dynamic lineBasicMaterial = new ExpandoObject();
                lineBasicMaterial.Uuid = uuid;
                lineBasicMaterial.Type = "LineBasicMaterial";
                lineBasicMaterial.vertexColors = 2;
                object3d.AddMaterial(lineBasicMaterial);
            }

            // Set outputs
            DA.SetData(0, object3d);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.Tri_LineVertexColors;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c8e41d1a-f630-41e0-9c9b-c7b126f48c4f"); }
        }
    }
}