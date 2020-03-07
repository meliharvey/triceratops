using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class MeshVertexColors : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MeshVertexColors()
          : base("MeshVertexColors", "MeshVertex",
              "Create a Three.js mesh geometry with vertex colors.",
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
            pManager.AddMeshParameter("Mesh", "G", "Mesh geometry", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Name of mesh", GH_ParamAccess.item, "");
            pManager.AddGenericParameter("Material", "M", "Threejs material", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Decimal Accuracy", "D", "The number of decimal points in accuracy (impacts export size)", GH_ParamAccess.item, 3);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Geometry", "G", "Mesh geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            Rhino.Geometry.Mesh mesh = null;
            string name = "";
            Material material = null;
            int decimalAccuracy = 3;

            // Reference the inputs
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref name);
            DA.GetData(2, ref material);
            DA.GetData(3, ref decimalAccuracy);

            // If the meshes have quads, triangulate them
            if (!mesh.Faces.ConvertQuadsToTriangles())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Error triangulating quad meshes. Try triangulating quads before feeding into this component.");
            };

            // Get the center of the object so the bufferGeometry can be given a local coordinates
            Point3d center = mesh.GetBoundingBox(true).Center;

            /// Add the bufferGeometry
            BufferGeometry geometry = new BufferGeometry(mesh, center, decimalAccuracy, true);

            /// Add the child
            dynamic meshObject = new ExpandoObject();
            meshObject.Uuid = Guid.NewGuid();
            if (name.Length > 0)
                meshObject.name = name;
            meshObject.Type = "Mesh";
            meshObject.Geometry = geometry.Uuid;
            meshObject.Material = material.Data.Uuid;
            meshObject.Matrix = new Matrix(center).Array;
            meshObject.CastShadow = true;
            meshObject.ReceiveShadow = true;

            // Set the material to use vertex colors
            material.Data.vertexColors = 2;
            material.Data.color = "0xffffff";

            // Create line object
            Object3d object3d = new Object3d(meshObject);
            object3d.AddGeometry(geometry);

            // If there is a material, add the material, textures, and images
            if (material != null)
            {
                object3d.AddMaterial(material.Data);
                if (material.Textures != null)
                    foreach (dynamic texture in material.Textures)
                        object3d.AddTexture(texture);
                if (material.Images != null)
                    foreach (dynamic image in material.Images)
                        object3d.AddImage(image);
            }

            // Set output references
            DA.SetData(0, object3d);
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
                return Properties.Resources.Tri_MeshVertexColor;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4adc22c1-7726-4187-8085-5d69f036bb51"); }
        }
    }
}