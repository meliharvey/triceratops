using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class Points : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Point class.
        /// </summary>
        public Points()
          : base("Points", "Points",
              "Create a Three.js points object",
              "Triceratops", "Geometry")
        {
        }

        // Place in a partition
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.tertiary;
            }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Point cloud", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "N", "Point cloud name", GH_ParamAccess.item, "");
            pManager.AddGenericParameter("Points Material", "M", "Points material", GH_ParamAccess.item);
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
            List<Point3d> points = new List<Point3d>();
            string name = "";
            Material material = null;
            int decimalAccuracy = 3;

            // Reference the inputs
            if(!DA.GetDataList(0, points))
                return;
            DA.GetData(1, ref name);
            DA.GetData(2, ref material);
            DA.GetData(3, ref decimalAccuracy);

            // Get the center of the bounding box to create local coordinates
            PointCloud pointCloud = new PointCloud(points);
            Point3d center = pointCloud.GetBoundingBox(true).Center;

            // Build the customColor objet
            dynamic customColor = new ExpandoObject();
            customColor.ItemSize = 3;
            customColor.Type = "Float32Array";
            customColor.Array = new List<double>();
            customColor.Normalized = false;

            // Build the size object
            dynamic size = new ExpandoObject();
            size.Itemsize = 1;
            size.Type = "Float32Array";
            size.Array = new List<double>();
            size.Normalized = false;

            // Build the attributes object
            //dynamic attributes = new ExpandoObject();
            //attributes.position = position;
            //attributes.customColor = customColor;
            //attributes.size = size;

            // Create the buffer geometry
            BufferGeometry geometry = new BufferGeometry(points, center, decimalAccuracy);

            // Build the child object
            dynamic pointsObject = new ExpandoObject();
            pointsObject.Uuid = Guid.NewGuid();
            pointsObject.Type = "Points";
            pointsObject.Layers = 1;
            pointsObject.Matrix = new Matrix(center).Array;
            pointsObject.Geometry = geometry.Uuid;
            if (material != null)
                pointsObject.Material = material.Data.Uuid;

            // Create line object
            Object3d object3d = new Object3d(pointsObject);
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

            // Set output references
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
                return Properties.Resources.Tri_Points;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3c10af1f-ffa5-46de-adf0-af1fd9ae78b5"); }
        }
    }
}