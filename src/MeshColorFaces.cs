using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Triceratops
{
    public class MeshColorFaces : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ColorMeshFace class.
        /// </summary>
        public MeshColorFaces()
          : base("MeshColorFaces", "ColorFace",
              "Color the faces of a mesh by unwelding edges.",
              "Triceratops", "Geometry")
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
            pManager.AddMeshParameter("Mesh", "M", "Mesh to color", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "C", "List of colors matching mesh faces", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Unwelded mesh with colored faces", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            Rhino.Geometry.Mesh mesh = null;
            List<Color> faceColors = new List<Color>();

            // Reference the inputs
            DA.GetData(0, ref mesh);
            if (!DA.GetDataList(1, faceColors))
                return;

            //Unweld the mesh so each face has its own unique set of vertices that can be colored
            mesh.Unweld(0, true);

            // A list of all vertex colors needs to be added first
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                mesh.VertexColors.SetColor(i, Color.CornflowerBlue);
            }

            // Loop through the list of vertex colors and replace them with the custom colors from the input
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                var vertexIndices = mesh.Faces.GetFace(i);

                mesh.VertexColors.SetColor(vertexIndices.A, faceColors[i]);
                mesh.VertexColors.SetColor(vertexIndices.B, faceColors[i]);
                mesh.VertexColors.SetColor(vertexIndices.C, faceColors[i]);
                mesh.VertexColors.SetColor(vertexIndices.D, faceColors[i]);
            }

            // Set the outputs
            DA.SetData(0, mesh);
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
                return Properties.Resources.MeshColorFaces;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cf894d52-9f46-41a0-acc3-ca592fc631f3"); }
        }
    }
}