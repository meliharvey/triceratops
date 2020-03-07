using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class HemisphereLight : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public HemisphereLight()
          : base("HempisphereLight", "HemiLight",
              "A light that uses sky or ground orientation to determine color.",
              "Triceratops", "Lights")
        {
        }

        // Place in a partition
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.primary;
            }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddColourParameter("Color", "C", "The color of the sky", GH_ParamAccess.item, Color.LightSkyBlue);
            pManager.AddColourParameter("GroundColor", "G", "The color of the ground", GH_ParamAccess.item, Color.Tan);
            pManager.AddNumberParameter("Intensity", "I", "The intensity of the light", GH_ParamAccess.item, 0.5);
            pManager.AddTextParameter("Name", "N", "The name of the light", GH_ParamAccess.item, "");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Light", "L", "The light object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            Color color = Color.LightSkyBlue;
            Color groundColor = Color.Tan;
            double intensity = 0.5;
            string name = "";

            // Reference the inputs
            DA.GetData(0, ref color);
            DA.GetData(1, ref groundColor);
            DA.GetData(2, ref intensity);
            DA.GetData(3, ref name);

            

            // Build the light object
            dynamic lightObject = new ExpandoObject();
            lightObject.Uuid = Guid.NewGuid();
            lightObject.Type = "HemisphereLight";
            if (name.Length > 0)
                lightObject.Name = name;
            lightObject.Color = new DecimalColor(color).Color;
            lightObject.GroundColor = new DecimalColor(groundColor).Color;
            lightObject.Intensity = intensity;
            lightObject.Matrix = new Matrix(new Point3d(0, 0, 1)).Array;

            // Create object3d
            Object3d object3d = new Object3d(lightObject);

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
                return Properties.Resources.Tri_HemisphereLight;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7c3728aa-ae2e-452e-9633-98c0b3493a66"); }
        }
    }
}