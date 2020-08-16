using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class PointLight_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PointLight_Component()
          : base("PointLight", "PtLight",
              "A light that casts rays from a single point.",
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
            pManager.AddPointParameter("Position", "P", "Location of point light", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "C", "The color of the light", GH_ParamAccess.item, Color.White);
            pManager.AddNumberParameter("Intensity", "I", "The intensity of the light", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Distance", "D", "This distance the light travels (default is infinity)", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Decay", "E", "The amount the light diminishes with distance", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("CastShadow", "S", "Set to true to cast shadows", GH_ParamAccess.item, false);
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
            Point3d position = new Point3d(0,10,0);
            Color color = Color.White;
            double intensity = 1;
            double distance = 0;
            double decay = 1;
            Boolean castShadow = false;
            string name = "";

            // Reference the inputs
            DA.GetData(0, ref position);
            DA.GetData(1, ref color);
            DA.GetData(2, ref intensity);
            DA.GetData(3, ref distance);
            DA.GetData(4, ref decay);
            DA.GetData(5, ref castShadow);
            DA.GetData(6, ref name);

            // Build the light object
            dynamic lightObject = new ExpandoObject();
            lightObject.Uuid = Guid.NewGuid();
            lightObject.Type = "PointLight";
            if (name.Length > 0)
                lightObject.Name = name;
            lightObject.Color = Convert.ToInt32(color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"), 16); ;
            lightObject.Intensity = intensity;
            lightObject.CastShadow = castShadow;
            lightObject.Distance = distance;
            lightObject.Decay = decay;
            lightObject.Matrix = new Matrix(position).Array;

            // Create a helper to aid in visualizing the light's behavior
            List<Curve> helper = new List<Curve>();
            Circle circle1 = new Circle(Plane.WorldXY, position, 0.5);
            Circle circle2 = new Circle(Plane.WorldYZ, position, 0.5);
            Circle circle3 = new Circle(Plane.WorldZX, position, 0.5);

            helper.Add(circle1.ToNurbsCurve());
            helper.Add(circle2.ToNurbsCurve());
            helper.Add(circle3.ToNurbsCurve());

            var lineX0 = new Rhino.Geometry.Line(position, position + new Point3d(1, 0, 0));
            var lineX1 = new Rhino.Geometry.Line(position, position + new Point3d(-1, 0, 0));
            var lineY0 = new Rhino.Geometry.Line(position, position + new Point3d(0, 1, 0));
            var lineY1 = new Rhino.Geometry.Line(position, position + new Point3d(0, -1, 0));
            var lineZ0 = new Rhino.Geometry.Line(position, position + new Point3d(0, 0, 1));
            var lineZ1 = new Rhino.Geometry.Line(position, position + new Point3d(0, 0, -1));

            helper.Add(lineX0.ToNurbsCurve());
            helper.Add(lineX1.ToNurbsCurve());
            helper.Add(lineY0.ToNurbsCurve());
            helper.Add(lineY1.ToNurbsCurve());
            helper.Add(lineZ0.ToNurbsCurve());
            helper.Add(lineZ1.ToNurbsCurve());

            // Create light
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
                return Properties.Resources.Tri_PointLight;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("facbe8c4-6a7b-4d30-b86e-53fb7513b26d"); }
        }
    }
}