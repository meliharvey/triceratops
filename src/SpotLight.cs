using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class SpotLight : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SpotLight class.
        /// </summary>
        public SpotLight()
          : base("SpotLight", "SpotLight",
              "A spotlight pointing at the origin.",
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
            pManager.AddNumberParameter("Angle", "A", "The angle of the light cone extent from the light direciton (radians)", GH_ParamAccess.item, 0.35);
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
            Point3d position = new Point3d(0, 10, 0);
            Color color = Color.White;
            double intensity = 1;
            double angle = 0.35;
            double distance = 0;
            double decay = 1;
            Boolean castShadow = false;
            string name = "";

            // Reference the inputs
            DA.GetData(0, ref position);
            DA.GetData(1, ref color);
            DA.GetData(2, ref intensity);
            DA.GetData(3, ref angle);
            DA.GetData(4, ref distance);
            DA.GetData(5, ref decay);
            DA.GetData(6, ref castShadow);
            DA.GetData(7, ref name);

            // Build the light object
            dynamic lightObject = new ExpandoObject();
            lightObject.Uuid = Guid.NewGuid();
            lightObject.Type = "SpotLight";
            if (name.Length > 0)
                lightObject.Name = name;
            lightObject.Color = Convert.ToInt32(color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"), 16); ;
            lightObject.Intensity = intensity;
            lightObject.Angle = (Math.PI / 180) * angle;
            lightObject.CastShadow = castShadow;
            lightObject.Distance = distance;
            lightObject.Decay = decay;
            lightObject.Matrix = new Matrix(position).Array;

            // Create a helper to aid in visualizing the light's behavior
            List<Curve> helper = new List<Curve>();

            var line = new Rhino.Geometry.Line(position, new Point3d(0, 0, 0));
            helper.Add(line.ToNurbsCurve());

            Vector3d vector = new Vector3d(new Point3d(0, 0, 0) - position);
            Plane plane = new Plane(position, vector);
            double radius = vector.Length * Math.Tan((Math.PI / 180) * angle);
            Cone cone = new Cone(plane, vector.Length, radius);

            Brep brep = cone.ToBrep(false);
            for (int i = 0; i < brep.Edges.Count; i++)
            {
                helper.Add(brep.Edges[i].EdgeCurve);
            }

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
                return Properties.Resources.Tri_SpotLight;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("06cb0a80-4a30-4225-bdd5-bbdfe91cc6d9"); }
        }
    }
}