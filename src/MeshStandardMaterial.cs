using System;
using System.Drawing;
using System.Collections.Generic;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class MeshStandardMaterial : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ExportGLTF class.
        /// </summary>
        public MeshStandardMaterial()
          : base("MeshStandardMaterial", "StandardMat",
              "Create a MeshStandardMaterial.",
              "Triceratops", "Materials")
        {
        }

        // Place in second partition
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
            pManager.AddColourParameter("Color", "C", "Material's color", GH_ParamAccess.item, Color.CornflowerBlue);
            pManager.AddNumberParameter("Opacity", "O", "Material's opacity (0 = transparent, 1 = opaque)", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Roughness", "R", "Material's roughness", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Metalness", "M", "Material's metalness", GH_ParamAccess.item, 0.5);
            pManager.AddColourParameter("Emissive", "Ec", "Emissive color", GH_ParamAccess.item, Color.Black);
            pManager.AddNumberParameter("EmissiveIntensity", "Ei", "Emissive intensity", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Wireframe", "W", "Display as wireframe", GH_ParamAccess.item, false);
            pManager.AddTextParameter("WireframeLineJoin", "Wj", "Style of wireframe joins ('round', 'miter', or 'bevel'", GH_ParamAccess.item, "round");
            pManager.AddNumberParameter("WireframeLineWidth", "Wl", "The width of the wireframe line", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("JSON", "J", "JSON string", GH_ParamAccess.item);
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
            double opacity = 1;
            double roughness = 1;
            double metalness = 0.5;
            Color emissive = Color.Black;
            double emissiveIntensity = 1;
            bool wireframe = false;
            string wireframeLinejoin = "round";
            double wireframeLinewidth = 1;

            // Reference inputs
            DA.GetData(0, ref color);
            DA.GetData(1, ref opacity);
            DA.GetData(2, ref roughness);
            DA.GetData(3, ref metalness);
            DA.GetData(4, ref emissive);
            DA.GetData(5, ref emissiveIntensity);
            DA.GetData(6, ref wireframe);
            DA.GetData(7, ref wireframeLinejoin);
            DA.GetData(8, ref wireframeLinewidth);

            // Create the material object
            dynamic material = new ExpandoObject();
            material.uuid = Guid.NewGuid();
            material.type = "MeshStandardMaterial";
            material.color = Convert.ToInt32(color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"), 16);
            material.transparent = true;
            material.opacity = opacity;
            material.roughness = roughness;
            material.metalness = metalness;
            material.emissive = Convert.ToInt32(emissive.R.ToString("X2") + emissive.G.ToString("X2") + emissive.B.ToString("X2"), 16);
            material.emissiveIntensity = emissiveIntensity;
            material.wireframe = wireframe;
            material.wireframeLinejoin = wireframeLinejoin;
            material.wireframeLinewidth = wireframeLinewidth;

            /// Wrap the material
            MaterialWrapper wrapper = new MaterialWrapper(material);

            string JSON = JsonConvert.SerializeObject(material);

            DA.SetData(0, JSON);
            DA.SetData(1, wrapper);
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
                return Properties.Resources.MeshStandardMaterial;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("52c8142d-9f07-4ceb-aca5-daef08d10a5c"); }
        }
    }
}