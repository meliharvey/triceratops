using System;
using System.Drawing;
using System.Collections.Generic;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class MeshStandardMaterial_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ExportGLTF class.
        /// </summary>
        public MeshStandardMaterial_Component()
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
            pManager.AddColourParameter("color", "C", "Material's color", GH_ParamAccess.item, Color.White);
            pManager.AddNumberParameter("opacity", "O", "Material's opacity (0 = transparent, 1 = opaque)", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("roughness", "R", "Material's roughness", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("metalness", "M", "Material's metalness", GH_ParamAccess.item, 0.5);
            pManager.AddColourParameter("emissive", "Ec", "Emissive color", GH_ParamAccess.item, Color.Black);
            pManager.AddNumberParameter("emissiveIntensity", "Ei", "Emissive intensity", GH_ParamAccess.item, 1);
            pManager.AddGenericParameter("wireframe", "W", "Display as wireframe", GH_ParamAccess.item);
            pManager.AddGenericParameter("maps", "M", "Add material maps", GH_ParamAccess.item);
            pManager[6].Optional = true;
            pManager[7].Optional = true;
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
            double opacity = 1;
            double roughness = 1;
            double metalness = 0.5;
            Color emmisiveColor = Color.Black;
            double emissiveIntensity = 1;
            WireframeSettings wireframe = null;
            MeshStandardMaterialMaps materialMaps = null;

            // Reference inputs
            DA.GetData(0, ref color);
            DA.GetData(1, ref opacity);
            DA.GetData(2, ref roughness);
            DA.GetData(3, ref metalness);
            DA.GetData(4, ref emmisiveColor);
            DA.GetData(5, ref emissiveIntensity);
            if (!DA.GetData(6, ref wireframe))
                wireframe = null;
            if (!DA.GetData(7, ref materialMaps))
                materialMaps = null;

            // Create the material object
            dynamic material = new ExpandoObject();
            material.Uuid = Guid.NewGuid();
            material.Type = "MeshStandardMaterial";
            material.Color = new DecimalColor(color).Color;
            material.Transparent = true;
            material.Opacity = opacity;
            material.Roughness = roughness;
            material.Metalness = metalness;
            material.Emissive = new DecimalColor(emmisiveColor).Color;
            material.EmissiveIntensity = emissiveIntensity;

            // If the wireframe is set to true, add wireframe attributes
            if (wireframe != null)
            {
                material.Wireframe = true;
                material.WireframeLinejoin = wireframe.WireframeLinejoin;
                material.WireframeLinewidth = wireframe.WireframeLinewidth;
            }

            // Build the file object
            Material materialObject = new Material(material);

            // If there are material maps, add them
            if (materialMaps != null)
            {
                if (materialMaps.Map != null)
                {
                    material.Map = materialMaps.Map.Data.Uuid;
                    materialObject.AddTexture(materialMaps.Map);
                }
                    
                if (materialMaps.AlphaMap != null)
                {
                    material.AlphaMap = materialMaps.AlphaMap.Data.Uuid;
                    material.AlphaTest = materialMaps.AlphaTest;
                    materialObject.AddTexture(materialMaps.AlphaMap);
                }

                if (materialMaps.BumpMap != null)
                {
                    material.BumpMap = materialMaps.BumpMap.Data.Uuid;
                    material.BumpScale = materialMaps.BumpScale;
                    materialObject.AddTexture(materialMaps.BumpMap);
                }

                if (materialMaps.DisplacementMap != null)
                {
                    material.DisplacementMap = materialMaps.DisplacementMap.Data.Uuid;
                    material.DisplacementScale = materialMaps.DisplacementScale;
                    materialObject.AddTexture(materialMaps.DisplacementMap);
                }

                if (materialMaps.NormalMap != null)
                {
                    material.NormalMap = materialMaps.NormalMap.Data.Uuid;
                    materialObject.AddTexture(materialMaps.NormalMap);
                }

                if (materialMaps.EnvMap != null)
                {
                    material.EnvMap = materialMaps.EnvMap.Data.Uuid;
                    material.EnvMapIntensity = materialMaps.EnvMapIntensity;
                    materialObject.AddTexture(materialMaps.EnvMap);
                }

                if (materialMaps.RoughnessMap != null)
                {
                    material.RoughnessMap = materialMaps.RoughnessMap.Data.Uuid;
                    materialObject.AddTexture(materialMaps.RoughnessMap);
                }

                if (materialMaps.MetalnessMap != null)
                {
                    material.MetalnessMap = materialMaps.MetalnessMap.Data.Uuid;
                    materialObject.AddTexture(materialMaps.MetalnessMap);
                }

                if (materialMaps.AoMap != null)
                {
                    material.AoMap = materialMaps.AoMap.Data.Uuid;
                    material.AoMapIntensity = materialMaps.AoMapIntensity;
                    materialObject.AddTexture(materialMaps.AoMap);
                }

                if (materialMaps.EmissiveMap != null)
                {
                    material.EmissiveMap = materialMaps.EmissiveMap.Data.Uuid;
                    materialObject.AddTexture(materialMaps.EmissiveMap);
                }
            }

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
                return Properties.Resources.Tri_MeshStandardMaterial;
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