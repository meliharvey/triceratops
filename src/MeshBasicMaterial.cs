using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class MeshBasicMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshBasicMaterial class.
        /// </summary>
        public MeshBasicMaterial()
          : base("MeshBasicMaterial", "BasicMat",
              "Create a MeshBasicMaterial.",
              "Triceratops", "Materials")
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
            pManager.AddColourParameter("Color", "C", "Add a color", GH_ParamAccess.item, Color.White);
            pManager.AddGenericParameter("Wireframe", "W", "Display as wireframe", GH_ParamAccess.item);
            pManager.AddGenericParameter("Maps", "M", "Add a texture map", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "M", "Threejs material", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            Color color = Color.White;
            WireframeSettings wireframe = null;
            Texture map = null;

            // Reference the inputs
            DA.GetData(0, ref color);
            if (!DA.GetData(1, ref wireframe))
                wireframe = null;
            if (!DA.GetData(2, ref map))
                map = null;

            // Build the material object
            dynamic material = new ExpandoObject();
            material.Uuid = Guid.NewGuid();
            material.Type = "MeshBasicMaterial";
            material.Color = new DecimalColor(color).Color;

            // If the material has map
            if (map != null)
                material.Map = map.Data.Uuid;

            // If the wireframe is set to true, add wireframe attributes
            if (wireframe != null)
            {
                material.Wireframe = true;
                material.WireframeLinejoin = wireframe.WireframeLinejoin;
                material.WireframeLinewidth = wireframe.WireframeLinewidth;
            }

            // Build the file object
            Material materialObject = new Material(material);
            if (map != null)
                materialObject.AddTexture(map);

            // Set the outputs
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
                return Properties.Resources.Tri_MeshBasicMaterial;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("87793bf9-80e8-44e7-aa29-59c7c138511b"); }
        }
    }
}