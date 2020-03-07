using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class AmbientLight : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public AmbientLight()
          : base("AmbientLight", "AmbientLight",
              "A light that illuminates from all directions.",
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
            pManager.AddColourParameter("Color", "C", "The color of the light", GH_ParamAccess.item, Color.White);
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
            Color color = Color.White;
            double intensity = 0.5;
            string name = "";

            // Reference the inputs
            DA.GetData(0, ref color);
            DA.GetData(1, ref intensity);
            DA.GetData(2, ref name);

            // Build the light object
            dynamic lightObject = new ExpandoObject();
            lightObject.Uuid = Guid.NewGuid();
            lightObject.Type = "AmbientLight";
            if (name.Length > 0)
                lightObject.Name = name;
            lightObject.Color = Convert.ToInt32(color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"), 16); ;
            lightObject.Intensity = intensity;
            lightObject.Matrix = new Matrix(new Point3d(0, 0, 0)).Array;

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
                return Properties.Resources.Tri_AmbientLight;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("52792682-7b0c-475a-be74-04d8c4d97944"); }
        }
    }
}