using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Triceratops
{
    public class MeshStandardMaterialMapsSettings : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshStandardMaterialMaps class.
        /// </summary>
        public MeshStandardMaterialMapsSettings()
          : base("MeshStandardMaterial Maps", "MeshStandardMaps",
              "Add textures to a mesh using map layers.",
              "Triceratops", "Materials")
        {
        }

        // Place in second partition
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
            pManager.AddGenericParameter("map", "M", "Add a texture map", GH_ParamAccess.item);
            pManager.AddGenericParameter("alphaMap", "A", "A texture to apply as an alpha map", GH_ParamAccess.item);
            pManager.AddNumberParameter("alphaTest", "At", "A texture to apply as an alpha map", GH_ParamAccess.item, 0.5);
            pManager.AddGenericParameter("bumpMap", "B", "A texture to apply as an alpha map", GH_ParamAccess.item);
            pManager.AddNumberParameter("bumpScale", "Bs", "A texture to apply as an alpha map", GH_ParamAccess.item, 1);
            pManager.AddGenericParameter("displacementMap", "D", "A texture to apply as an alpha map", GH_ParamAccess.item);
            pManager.AddNumberParameter("displacementScale", "Ds", "A texture to apply as an alpha map", GH_ParamAccess.item, 1);
            pManager.AddGenericParameter("normalMap", "N", "A texture to apply as an alpha map", GH_ParamAccess.item);
            pManager.AddGenericParameter("envMap", "E", "A texture to apply as an alpha map", GH_ParamAccess.item);
            pManager.AddNumberParameter("envMapIntensity", "Ei", "A texture to apply as an alpha map", GH_ParamAccess.item, 1);
            pManager.AddGenericParameter("roughnessMap", "R", "A texture to apply as an alpha map", GH_ParamAccess.item);
            pManager.AddGenericParameter("metalnessMap", "Mt", "A texture to apply as an alpha map", GH_ParamAccess.item);
            pManager.AddGenericParameter("aoMap", "AO", "A texture to apply as an alpha map", GH_ParamAccess.item);
            pManager.AddNumberParameter("aoMapIntensity", "AOi", "A texture to apply as an alpha map", GH_ParamAccess.item, 1);
            pManager.AddGenericParameter("emissiveMap", "Em", "A texture to apply as an alpha map", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[3].Optional = true;
            pManager[5].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
            pManager[10].Optional = true;
            pManager[11].Optional = true;
            pManager[12].Optional = true;
            pManager[14].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Material Maps", "M", "Threejs material maps", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Texture map = null;
            Texture alphaMap = null;
            double alphaTest = 0.5;
            Texture bumpMap = null;
            double bumpScale = 1;
            Texture displacementMap = null;
            double displacementScale = 1;
            Texture normalMap = null;
            Texture envMap = null;
            double envMapIntensity = 1;
            Texture roughnessMap = null;
            Texture metalnessMap = null;
            Texture aoMap = null;
            double aoMapIntensity = 1;
            Texture emissiveMap = null;


            if (!DA.GetData(0, ref map))
                map = null;
            if (!DA.GetData(1, ref alphaMap))
                alphaMap = null;
            DA.GetData(2, ref alphaTest);
            if (!DA.GetData(3, ref bumpMap))
                bumpMap = null;
            DA.GetData(4, ref bumpScale);
            if (!DA.GetData(5, ref displacementMap))
                displacementMap = null;
            DA.GetData(6, ref displacementScale);
            if (!DA.GetData(7, ref normalMap))
                normalMap = null;
            if (!DA.GetData(8, ref envMap))
                envMap = null;
            DA.GetData(9, ref envMapIntensity);
            if (!DA.GetData(10, ref roughnessMap))
                roughnessMap = null;
            if (!DA.GetData(11, ref metalnessMap))
                metalnessMap = null;
            if (!DA.GetData(12, ref aoMap))
                aoMap = null;
            DA.GetData(13, ref aoMapIntensity);
            if (!DA.GetData(14, ref emissiveMap))
                emissiveMap = null;

            MeshStandardMaterialMaps materialMaps = new MeshStandardMaterialMaps();
            materialMaps.Map = map;
            materialMaps.AlphaMap = alphaMap;
            materialMaps.AlphaTest = alphaTest;
            materialMaps.BumpMap = bumpMap;
            materialMaps.BumpScale = bumpScale;
            materialMaps.DisplacementMap = displacementMap;
            materialMaps.DisplacementScale = displacementScale;
            materialMaps.EnvMap = envMap;
            materialMaps.EnvMapIntensity = envMapIntensity;
            materialMaps.NormalMap = normalMap;
            materialMaps.RoughnessMap = roughnessMap;
            materialMaps.MetalnessMap = metalnessMap;
            materialMaps.AoMap = aoMap;
            materialMaps.AoMapIntensity = aoMapIntensity;
            materialMaps.EmissiveMap = emissiveMap;

            DA.SetData(0, materialMaps);
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
                return Properties.Resources.Tri_MeshStandardMaterialMaps;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("946e94a7-a1ba-45b1-bb50-bc02574f88da"); }
        }
    }
}