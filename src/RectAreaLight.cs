using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class RectAreaLight : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RectAreaLight class.
        /// </summary>
        public RectAreaLight()
          : base("RectAreaLight", "AreaLight",
              "An area light the shape of a rectangle.",
              "Triceratops", "Lights")
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
            pManager.AddRectangleParameter("Rectangle", "R", "The rectangle from which to build the light", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "C", "The color of the light", GH_ParamAccess.item, Color.White);
            pManager.AddNumberParameter("Intensity", "I", "The intensity of the light", GH_ParamAccess.item, 1);
            pManager.AddTextParameter("Name", "N", "The name of the light", GH_ParamAccess.item, "");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Helper", "H", "A helper to aid in positioning the light", GH_ParamAccess.list);
            pManager.AddGenericParameter("Light", "L", "The light object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            Rectangle3d rectangle = new Rectangle3d();
            Color color = Color.White;
            double intensity = 1;
            string name = "";

            // Reference the inputs
            DA.GetData(0, ref rectangle);
            DA.GetData(1, ref color);
            DA.GetData(2, ref intensity);
            DA.GetData(3, ref name);

            // Get matrix transform between starting plane and rectangle
            Point3d position = rectangle.Center;

            Vector3d vectorX = new Vector3d(-1, 0, 0);
            Vector3d vectorZ = new Vector3d(0, 0, 1);
            
            Plane zxPlane = new Plane(Point3d.Origin, vectorX, vectorZ);

            var plane = rectangle.Plane;
            var transform = Transform.PlaneToPlane(zxPlane, plane);
            float[] matrixArray = transform.ToFloatArray(false);
            List<double> l = new List<double>();
            
            foreach (float value in matrixArray)
            {
                decimal dec = new decimal(value);
                double dub = (double)dec;
                l.Add(Math.Round(dub, 6));
            }

            // Build the light object
            dynamic lightObject = new ExpandoObject();
            lightObject.Uuid = Guid.NewGuid();
            lightObject.Type = "RectAreaLight";
            if (name.Length > 0)
                lightObject.Name = name;
            lightObject.Color = Convert.ToInt32(color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"), 16); ;
            lightObject.Intensity = intensity;
            lightObject.Width = rectangle.Width;
            lightObject.Height = rectangle.Height;
            lightObject.Matrix = new List<double> { l[0] * -1, l[2], l[1], l[3], l[8] * -1, l[10], l[9], l[11], l[4] * -1, l[6], l[5], l[7],  l[12] * -1, l[14], l[13], l[15] };

            // Create a helper to aid in visualizing the light's behavior
            List<Curve> helper = new List<Curve>();

            helper.Add(rectangle.ToNurbsCurve());

            Point3d start = plane.Origin;
            Point3d end = start + plane.Normal;
            Line directionLine = new Line(start, end);
            helper.Add(directionLine.ToNurbsCurve());

            // Create object3d
            Object3d object3d = new Object3d(lightObject);

            // Set outputs
            DA.SetDataList(0, helper);
            DA.SetData(1, object3d);
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
                return Properties.Resources.Tri_RectAreaLight;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("aec577ad-3905-4ca5-9cdd-205a1d997108"); }
        }
    }
}