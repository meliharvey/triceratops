using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Triceratops
{
    public class WriteText_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SaveCSV class.
        /// </summary>
        public WriteText_Component()
          : base("Write Text File", "WriteTxt",
              "Write a json or text file.",
              "Triceratops", "File Management")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "P", "Path to file directory", GH_ParamAccess.item);
            pManager.AddTextParameter("File Name", "N", "CSV file name", GH_ParamAccess.item);
            pManager.AddGenericParameter("Data", "D", "Data as a list of strings", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Write", "W", "Write the JSON", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Full Path", "P", "The full file path of the CSV", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            string path = null;
            string name = null;
            string jsonString = null;
            bool write = false;

            // Reference the inputs
            DA.GetData(0, ref path);
            DA.GetData(1, ref name);
            DA.GetData(2, ref jsonString);
            DA.GetData(3, ref write);

            // Create filepath
            string filePath = path + "\\" + name;

            // Write the JSON only when the run input is true
            if (write)
                File.WriteAllText(filePath, jsonString);

            // If the write input is not true, trigger a warning
            else
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Toggle to true to export JSON");
                
            // Set the outputs
            DA.SetData(0, filePath);
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
                return Properties.Resources.Tri_TextFile;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8bc3d185-7333-4633-8057-a72500a9d971"); }
        }
    }
}