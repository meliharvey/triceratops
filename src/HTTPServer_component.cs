using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Runtime.InteropServices;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Triceratops
{
    public class HTTPServer_component : GH_Component
    {
        // declare server variable outside of solve instance to ensure persistence
        SimpleHTTPServer server = null;

        /// <summary>
        /// Initializes a new instance of the HTTPServer class.
        /// </summary>
        public HTTPServer_component()
          : base("HTTPServer", "HTTP",
              "Description",
              "Triceratops", "File Management")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "P", "The path where the website's index.html is located.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Port", "N", "The port number on which to serve the website", GH_ParamAccess.item, 3000);
            pManager.AddBooleanParameter("Run", "R", "Start or stop the SimpleHTTPServer", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = null;
            int port = 3000;
            bool run = false;

            DA.GetData(0, ref path);
            DA.GetData(1, ref port);
            DA.GetData(2, ref run);

            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Grasshopper is running on: " + RuntimeInformation.OSDescription.ToString());

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Grasshopper is running on Mac");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) & !IsAdministrator())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "SimpleHTTPServer won't work unless Rhino is run as Administrator.");
                return;
            }

            if (run && server == null)
            {
                server = new SimpleHTTPServer(@"" + path, port);
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Server is running on port: " + port.ToString());
            }
            else if (run)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Server is running on port: " + port.ToString());
            }
            else if (server == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Toggle run to true to start a new server.");
            }
            else
            {
                server.Stop();
                server = null;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Stopped the SimpleHTTPServer.");
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Toggle run to true to start a new server.");
            }
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
                return Properties.Resources.Tri_HTTPServer;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f33bd33d-67bd-49dc-b82b-3adce674cb59"); }
        }

        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}