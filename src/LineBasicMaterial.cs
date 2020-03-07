using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class LineBasicMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the LineBasicMaterial class.
        /// </summary>
        public LineBasicMaterial()
          : base("LineBasicMaterial", "LineBasicMat",
              "Create a Three.js LineBasicMaterial",
              "Triceratops", "Materials")
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
            pManager.AddColourParameter("Color", "C", "The line color", GH_ParamAccess.item, Color.White);
            pManager.AddNumberParameter("LineWidth", "W", "Line width", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Line Material", "M", "The line material object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Color color = Color.White;
            double linewidth = 1;

            DA.GetData(0, ref color);
            DA.GetData(1, ref linewidth);

            // Build the material object
            dynamic material = new ExpandoObject();
            material.Uuid = Guid.NewGuid();
            material.Type = "LineBasicMaterial";
            material.Color = new DecimalColor(color).Color;
            material.Linewidth = linewidth;

            // Build the file object
            Material materialObject = new Material(material);

            // Set output references
            DA.SetData(0, materialObject);
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
                return Properties.Resources.Tri_LineBasicMaterial;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3492cbee-96f5-4793-9906-bc52a25c0fe8"); }
        }
    }
}