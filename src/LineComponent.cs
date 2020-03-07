using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class LineComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public LineComponent()
          : base("Line", "Line",
              "Create a Three.js line geometry",
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
            int decimalAccuracy = 3;

            // Reference the inputs
            DA.GetData(0, ref curve);
            DA.GetData(1, ref name);
            DA.GetData(2, ref material);
            DA.GetData(3, ref decimalAccuracy);

            // Throw error message if curve cannot be converted to a polyline
            if (!curve.IsPolyline())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input must be polyine, curves are not allowed.");
                return;
            }
            // Convert curve to polyline
            Polyline polyline = null;
            curve.TryGetPolyline(out polyline);

            // Get the center of the object so the bufferGeometry can be given local coordinates
            Point3d center = polyline.BoundingBox.Center;

            // Produce a buffer geometry from the polyline
            dynamic geometry = new BufferGeometry(polyline, center, decimalAccuracy);

            // If dashed material is used add the line distance to the buffer geometry
            if (material != null && string.Equals(material.Data.Type, "LineDashedMaterial"))
                geometry.AddLineDistance();

            //Create a threejs object definition
            dynamic lineObject = new ExpandoObject();
            lineObject.Uuid = Guid.NewGuid();
            lineObject.Name = name;
            lineObject.Type = "Line";
            lineObject.Geometry = geometry.Uuid;
            if (material != null)
                lineObject.Material = material.Data.Uuid;
            lineObject.Matrix = new Matrix(center).Array;

            // Create file object
            Object3d object3d = new Object3d(lineObject);
            object3d.AddGeometry(geometry);

            // If there is a material, add the material, textures, and images
            if (material != null)
            {
                object3d.AddMaterial(material.Data);
                if (material.Textures != null)
                    foreach (dynamic texture in material.Textures)
                        object3d.AddTexture(texture);
                if (material.Images != null)
                    foreach (dynamic image in material.Images)
                        object3d.AddImage(image);
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
                return Properties.Resources.Tri_Line;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("251b9e7c-172f-4cfd-b786-7ac0892bd68d"); }
        }
    }
}