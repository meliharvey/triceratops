using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class PointsMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PointsMaterial class.
        /// </summary>
        public PointsMaterial()
          : base("PointsMaterial", "PointsMat",
              "Description",
              "Triceratops", "Materials")
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
            pManager.AddColourParameter("Color", "C", "The color of the points", GH_ParamAccess.item);
            pManager.AddNumberParameter("Size", "S", "The size of the points", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("SizeAttenuation", "A", "Point size changes with depth if true", GH_ParamAccess.item, true);
            pManager.AddGenericParameter("Map", "M", "A texture for the point", GH_ParamAccess.item);
            pManager.AddGenericParameter("AlphaMap", "Am", "A texture to apply as an alpha map", GH_ParamAccess.item);
            pManager.AddNumberParameter("AlphaTest", "At", "The softness of the alpha edge (0.0 to 1.0)", GH_ParamAccess.item, 0.5);
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Points Material", "M", "The points material object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            Color color = Color.White;
            double size = 1;
            bool sizeAttenuation = true;
            Texture map = null;
            Texture alphaMap = null;
            double alphaTest = 0.5;

            // Reference the inputs
            DA.GetData(0, ref color);
            DA.GetData(1, ref size);
            DA.GetData(2, ref sizeAttenuation);
            if(!DA.GetData(3, ref map))
                map = null;
            if(!DA.GetData(4, ref alphaMap))
                alphaMap = null;
            DA.GetData(5, ref alphaTest);
            
            // Build the material object
            dynamic material = new ExpandoObject();
            material.Uuid = Guid.NewGuid();
            material.Type = "PointsMaterial";
            material.Color = Convert.ToInt32(color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"), 16);
            material.Size = size;
            material.SizeAttenuation = sizeAttenuation;
            if (map != null)
                material.map = map.Data.Uuid;
            if (alphaMap != null)
                material.AlphaMap = alphaMap.Data.Uuid;
            material.transparent = false;
            material.AlphaTest = alphaTest;

            // Build the file object
            Material materialObject = new Material(material);
            if (map != null)
                materialObject.AddTexture(map);
            if (alphaMap != null)
                materialObject.AddTexture(alphaMap);

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
                return Properties.Resources.Tri_PointsMaterial;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7033b9fb-69d7-4404-a9bc-586c9ffbcb5e"); }
        }
    }
}