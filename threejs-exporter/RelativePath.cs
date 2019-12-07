using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace triceratops
{
    public class RelativePath : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RelativePath class.
        /// </summary>
        public RelativePath()
          : base("RelativePath", "Nickname",
              "Description",
              "Triceratops", "Scene")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Relative Path", "P", "The path of the GH document", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = null;

            if (OnPingDocument().FilePath != null)
            {
                path = System.IO.Path.GetDirectoryName(OnPingDocument().FilePath);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "You must save the Grasshopper file for this component to return a file path.");
            }

            DA.SetData(0, path);
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
                return Properties.Resources.RelativePath;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5ada270b-4da7-4bb5-be0c-3883a1dd3602"); }
        }
    }
}