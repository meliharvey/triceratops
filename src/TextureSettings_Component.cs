using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Triceratops
{
    public class TextureSettings_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MapSettings class.
        /// </summary>
        public TextureSettings_Component()
          : base("TextureSettings", "TextureSettings",
              "Scale, Repeat, and rotate textures",
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
            pManager.AddIntegerParameter("wrapS", "Ws", "Horizontal wrapping (0 = RepeatWrapping, 1 = ClampToEdgeWrapping, 2 = MirrorRepeatWrapping)", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("wrapT", "Wt", "Vertical wrapping (0 = RepeatWrapping, 1 = ClampToEdgeWrapping, 2 = MirrorRepeatWrapping)", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("repeatU", "Ru", "Texture horizontal scale (number of times texture repeats in U direction)", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("repeatV", "Rv", "Texture vertical scale (number of times texture repeats in V direction)", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("offsetU", "Ou", "Offset the texture in the U direction (0 to 1)", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("offsetV", "Ov", "Offset the texture in the V direction (0 to 1)", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("centerU", "Cu", "The center of texture rotation in the U direction (0 to 1)", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("centerV", "Cv", "The center of texture rotation in the V direction  (0 to 1)", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("rotation", "R", "Texture rotation angle (in degrees)", GH_ParamAccess.item, 0);
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
            int wrapS = 0;
            int wrapT = 0;
            double repeatU = 1;
            double repeatV = 1;
            double offsetU = 0;
            double offsetV = 0;
            double centerU = 0;
            double centerV = 0;
            double rotation = 0;

            DA.GetData(0, ref wrapS);
            DA.GetData(1, ref wrapT);
            DA.GetData(2, ref repeatU);
            DA.GetData(3, ref repeatV);
            DA.GetData(4, ref offsetU);
            DA.GetData(5, ref offsetV);
            DA.GetData(6, ref centerU);
            DA.GetData(7, ref centerV);
            DA.GetData(8, ref rotation);

            // Make a list for wrapping, first value is horizontal wrapping, second number is vertical
            List<int> wrap = new List<int>();

            // Add horizontl wrapping value
            switch (wrapS)
            {
                case 0:
                    wrap.Add(1000); // repeat
                    break;
                case 1:
                    wrap.Add(1001); // clamp to edge
                    break;
                case 2:
                    wrap.Add(1002); // repeat mirrored
                    break;
                default:
                    wrap.Add(1000);
                    break;
            }

            // Add vertical wrapping value
            switch (wrapT)
            {
                case 0:
                    wrap.Add(1000); // repeat
                    break;
                case 1:
                    wrap.Add(1001); // clamp to edge
                    break;
                case 2:
                    wrap.Add(1002); // repeat mirrored
                    break;
                default:
                    wrap.Add(1000);
                    break;
            }

            // Build the texture object
            TextureSettings textureSettings = new TextureSettings();
            textureSettings.Repeat = new List<double> { repeatU, repeatV };
            textureSettings.Offset = new List<double> { offsetU, offsetV };
            textureSettings.Center = new List<double> { centerU, centerV };
            textureSettings.Rotation = rotation * (Math.PI / 180);
            textureSettings.Wrap = wrap;

            DA.SetData(0, textureSettings);
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
                return Properties.Resources.Tri_TextureSettings;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7bbfc83b-d705-474e-bd66-e5f84755051e"); }
        }
    }
}