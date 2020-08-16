using System;
using System.Collections.Generic;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Triceratops
{
    public class CubeTexture_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the CubeTexture_Component class.
        /// </summary>
        public CubeTexture_Component()
          : base("CubeTexture_Component", "Nickname",
              "Create a 6 sided texture for environment maps",
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
            pManager.AddTextParameter("Left", "L", "Left image (nx)", GH_ParamAccess.item);
            pManager.AddTextParameter("Front", "F", "Front image (pz)", GH_ParamAccess.item);
            pManager.AddTextParameter("Right", "R", "Right image (px)", GH_ParamAccess.item);
            pManager.AddTextParameter("Rear", "Re", "Rear image (nx)", GH_ParamAccess.item);
            pManager.AddTextParameter("Top", "T", "Top image (py)", GH_ParamAccess.item);
            pManager.AddTextParameter("Bottom", "B", "Bottom image (ny)", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Give the texture a name", GH_ParamAccess.item, "");
            pManager.AddGenericParameter("Settings", "S", "Texture map settings", GH_ParamAccess.item);
            pManager[7].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Cube Texture", "T", "A Triceratops cube texture object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            string left = "";
            string front = "";
            string right = "";
            string rear = "";
            string top = "";
            string bottom = "";
            string name = "";
            TextureSettings settings = null;

            // Reference the inputs
            DA.GetData(0, ref left);
            DA.GetData(1, ref front);
            DA.GetData(2, ref right);
            DA.GetData(3, ref rear);
            DA.GetData(4, ref top);
            DA.GetData(5, ref bottom);
            DA.GetData(6, ref name);
            if (!DA.GetData(7, ref settings))
                settings = null;

            List<string> paths = new List<string> { right, left, top, bottom, front, rear };

            // Create an image object
            CubeImage cubeImage = new CubeImage(paths);

            // Build the texture object
            dynamic texture = new ExpandoObject();
            texture.Uuid = Guid.NewGuid();
            texture.Name = name;
            texture.Mapping = 301;
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
            texture.Image = cubeImage.Uuid;

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
            Texture triTexture = new Texture(texture, cubeImage);

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
                return Properties.Resources.Tri_CubeTexture;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cbc3c5f4-1964-4a9e-b9ee-7d8a198e5a69"); }
        }
    }
}