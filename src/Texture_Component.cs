using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Dynamic;
using Newtonsoft.Json;

namespace Triceratops
{
    public class Texture_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Texture class.
        /// </summary>
        public Texture_Component()
          : base("Texture", "Texture",
              "A texture object",
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
            pManager.AddTextParameter("Image Path", "P", "The full path to the image", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Give the texture a name", GH_ParamAccess.item, "");
            pManager.AddGenericParameter("Settings", "S", "Texture map settings", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Texture", "T", "A Triceratops texture object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            string path = "";
            string name = "";
            TextureSettings settings = null;

            // Reference the inputs
            DA.GetData(0, ref path);
            DA.GetData(1, ref name);
            if(!DA.GetData(2, ref settings))
                settings = null;

            // Create an image object
            Image image = new Image(path);

            // Build the texture object
            dynamic texture = new ExpandoObject();
            texture.Uuid = Guid.NewGuid();
            texture.Name = name;
            texture.Mapping = 300;
            texture.Repeat = new List<double> { 1, 1 };
            texture.Offset = new List<double> { 0, 0 };
            texture.Center = new List<double> { 0, 0 };
            texture.Rotation = 0;
            texture.Wrap = new List<int> { 1000, 1000 }; ;
            //texture.Format = 1023;
            //texture.Type = 1009;
            //texture.Encoding = 3000;
            //texture.MinFilter = 1008;
            //texture.MagFilter = 1006;
            //texture.Anisotropy = 1;
            //texture.FlipY = true;
            //texture.PremultiplyAlpha = false;
            //texture.UnpackAlignment = 4;
            texture.Image = image.Uuid;

            // If there are texture settings, apply them
            if (settings != null)
            {
                texture.Repeat = settings.Repeat;
                texture.Offset = settings.Offset;
                texture.Center = settings.Center;
                texture.Rotation = settings.Rotation;
                texture.Wrap = settings.Wrap;
            }

            // Build the texture
            Texture triTexture = new Texture(texture, image);

            // Set the outputs
            DA.SetData(0, triTexture);
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
                return Properties.Resources.Tri_Texture;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e4677c34-5ba8-4316-9628-6db0a4e59b6f"); }
        }
    }
}