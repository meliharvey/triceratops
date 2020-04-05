using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Triceratops
{
    public class Wireframe_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Wireframe class.
        /// </summary>
        public Wireframe_Component()
          : base("wireframe", "wireframe",
              "Change any material to a wireframe",
              "Triceratops", "Materials")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("wireframeLineJoin", "J", "Style of wireframe joins ('round', 'miter', or 'bevel'", GH_ParamAccess.item, "round");
            pManager.AddIntegerParameter("wireframeLineWidth", "L", "The width of the wireframe line", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Wireframe Settings", "W", "Wireframe settings (plug into any mesh material)", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string wireframeLinejoin = "round";
            int wireframeLinewidth = 1;

            DA.GetData(0, ref wireframeLinejoin);
            DA.GetData(1, ref wireframeLinewidth);

            WireframeSettings wireframe = new WireframeSettings();
            wireframe.WireframeLinejoin = wireframeLinejoin;
            wireframe.WireframeLinewidth = wireframeLinewidth;

            DA.SetData(0, wireframe);
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
                return Properties.Resources.Tri_Wireframe;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4ccb1e30-a284-4ac5-a143-40bf1b59f495"); }
        }
    }
}