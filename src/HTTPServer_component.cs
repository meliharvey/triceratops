using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

using Grasshopper.Kernel;

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
            bool run = false;

            DA.GetData(0, ref path);
            DA.GetData(1, ref run);

            // AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Grasshopper is running on: " + RuntimeInformation.OSDescription.ToString());

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component only runs on Windows.");
                return;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) & !IsAdministrator())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component won't work unless Rhino is run as Administrator.");
                return;
            }

            int port = FreeTcpPort();

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Port " + port.ToString() + " is already being used. Try another port.");
                    return;
                }
            }

            if (run && server == null)
            {
                server = new SimpleHTTPServer(@"" + path, port);
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Server is running on port: " + port.ToString());
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Toggle back to false before closing document or server will stay running.");
                System.Diagnostics.Process.Start("http://localhost:" + port.ToString());
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

        static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}