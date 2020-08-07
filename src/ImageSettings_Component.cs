using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Triceratops
{
    public class ImageSettings_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ImageSettings_Component class.
        /// </summary>
        public ImageSettings_Component()
          : base("ImageSettings", "ImgSettings",
              "Set saturation and lightness of an texture image",
              "Triceratops", "Materials")
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
            pManager.AddNumberParameter("saturation", "S", "Image saturation (from 0 to 1 where 0 is completely desaturated and 1 is unchanged)", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("contrast", "C", "Image contrast (from -1 to 1, where -1 is least contrast, 1 is highest contrast, 0 is unchanged)", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("lightness", "L", "Image lightness (from-1 to 1, where -1 is darkest, 1 is lightest, 0 is unchanged)", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("TextureSettings", "S", "Settings for textures", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double saturation = 1;
            double contrast = 0;
            double lightness = 0;

            DA.GetData(0, ref saturation);
            DA.GetData(1, ref contrast);
            DA.GetData(2, ref lightness);

            // Build the image settings object
            ImageSettings imageSettings = new ImageSettings();
            imageSettings.Saturation = saturation;
            imageSettings.Contrast = contrast;
            imageSettings.Lightness = lightness;

            DA.SetData(0, imageSettings);
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
                return Properties.Resources.Tri_ImageSettings;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4e4d5eb2-bf23-4eec-b2cc-6593d69213f9"); }
        }
    }
}