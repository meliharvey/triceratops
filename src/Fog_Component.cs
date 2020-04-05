using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Triceratops
{
    public class Fog_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Fog_Component()
          : base("Fog", "Fog",
              "Creates scene fog",
              "Triceratops", "Scene")
        {
        }

        // Place in a partition
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.quarternary;
            }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddColourParameter("Color", "C", "The color of the fog", GH_ParamAccess.item, Color.White);
            pManager.AddNumberParameter("Near", "N", "The distance at which the fog starts obscuring objects", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Far", "F", "The distance where fog completely obscures the scene", GH_ParamAccess.item, 100);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Fog", "F", "A Triceratops fog object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Color color = Color.White;
            double near = 10;
            double far = 100;

            DA.GetData(0, ref color);
            DA.GetData(1, ref near);
            DA.GetData(2, ref far);

            Fog fogObject = new Fog(color, near, far);

            // Set output references
            DA.SetData(0, fogObject);

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
                return Properties.Resources.Tri_Fog;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("47799da7-9249-44a0-8bbb-dc8345ce4cdf"); }
        }
    }
}