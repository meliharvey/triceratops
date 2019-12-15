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
            pManager.AddColourParameter("Color", "C", "Add a color", GH_ParamAccess.item, Color.CornflowerBlue);
            pManager.AddBooleanParameter("Wireframe", "W", "Display as wireframe", GH_ParamAccess.item, false);
            pManager.AddTextParameter("WireframeLineJoin", "Wj", "Style of wireframe joins ('round', 'miter', or 'bevel'", GH_ParamAccess.item, "round");
            pManager.AddNumberParameter("WireframeLineWidth", "Wl", "The width of the wireframe line", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("JSON", "J", "Material's JSON string", GH_ParamAccess.item);
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
            bool wireframe = false;
            string wireframeLinejoin = "round";
            double wireframeLinewidth = 1;

            // Reference the inputs
            DA.GetData(0, ref color);
            DA.GetData(1, ref wireframe);
            DA.GetData(2, ref wireframeLinejoin);
            DA.GetData(3, ref wireframeLinewidth);

            // Build the material object
            dynamic material = new ExpandoObject();
            material.uuid = Guid.NewGuid();
            material.type = "MeshBasicMaterial";
            material.color = Convert.ToInt32(color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"), 16);

            // If the wireframe is set to true, add wireframe attributes
            if (wireframe)
            {
                material.wireframe = wireframe;
                material.wireframeLinejoin = wireframeLinejoin;
                material.wireframeLinewidth = wireframeLinewidth;
            }
            
            //Wrap the material
            MaterialWrapper wrapper = new MaterialWrapper(material);

            // Serialize
            string JSON = JsonConvert.SerializeObject(material);

            // Set the outputs
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
                return Properties.Resources.MeshBasicMaterial;
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