using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Triceratops
{
    public class SerializeObject : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SerializeObject()
          : base("SerializeObject", "Serialize",
              "Description",
              "Triceratops", "File Management")
        {
        }

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
            pManager.AddGenericParameter("Object3d", "O", "Serialize any Triceratops.Object3d", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Minify", "M", "Set to true to minifiy JSON", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Run", "R", "Run the serializer", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("JSON", "J", "Geometry JSON", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            Object3d object3d = null;
            bool minify = false;
            bool run = true;

            // Reference the inputs
            DA.GetData(0, ref object3d);
            DA.GetData(1, ref minify);
            DA.GetData(2, ref run);

            // If minify is true, formatting should be none
            Formatting formatting;
            if (minify)
                formatting = Formatting.None;
            else
                formatting = Formatting.Indented;

            string jsonString = "";

            // If set to run, run the serializer
            if (run)
            {
                // Create JSON string
                jsonString = JsonConvert.SerializeObject(object3d, formatting, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Set run to true to serialize the object");
            }
            
            // Set the outputs
            DA.SetData(0, jsonString);
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
                return Properties.Resources.Tri_ExportJSON;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("53d4f217-7389-4c3b-8fbb-a89d4b069789"); }
        }
    }
}