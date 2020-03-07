using System;
using System.Collections.Generic;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Triceratops
{
    public class MeshNormalMaterial : GH_Component
    {
        /// <summary>C:\Users\dee\Desktop\Triceratops\src\MeshNormalMaterial.cs
        /// Initializes a new instance of the MeshNormalMaterial class.
        /// </summary>
        public MeshNormalMaterial()
          : base("MeshNormalMaterial", "NormalMat",
              "Create a MeshNormalMaterial.",
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
            pManager.AddBooleanParameter("Wireframe", "W", "Display as wireframe", GH_ParamAccess.item, false);
            pManager.AddTextParameter("WireframeLineJoin", "J", "Style of wireframe joins ('round', 'miter', or 'bevel'", GH_ParamAccess.item, "round");
            pManager.AddNumberParameter("WireframeLineWidth", "L", "The width of the wireframe line", GH_ParamAccess.item, 1);
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
            bool wireframe = false;
            string wireframeLinejoin = "round";
            double wireframeLinewidth = 1;

            // Reference the inputs
            DA.GetData(0, ref wireframe);
            DA.GetData(1, ref wireframeLinejoin);
            DA.GetData(2, ref wireframeLinewidth);

            // Create material object
            dynamic material = new ExpandoObject();
            material.Uuid = Guid.NewGuid();
            material.Type = "MeshNormalMaterial";
            material.Wireframe = wireframe;
            material.WireframeLinejoin = wireframeLinejoin;
            material.WireframeLinewidth = wireframeLinewidth;

            // Build the file object
            Material materialObject = new Material(material);

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
                return Properties.Resources.Tri_MeshNormalMaterial;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d35efd52-3d80-4eac-865f-b3f8597214e3"); }
        }
    }
}