using System;
using System.Collections.Generic;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class Mesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Mesh()
          : base("Mesh", "Mesh",
              "Create a Three.js mesh geometry",
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
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("JSON", "J", "Geometry JSON", GH_ParamAccess.item);
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
            MaterialWrapper material = null;

            // Reference the inputs
            DA.GetData(0, ref mesh);
            DA.GetData(1, ref name);
            DA.GetData(2, ref material);

            // If the meshes have quads, triangulate them
            if (!mesh.Faces.ConvertQuadsToTriangles())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Error triangulating quad meshes. Try triangulating quads before feeding into this component.");
            };

            // Fill the vertices list
            List<double> vertices = new List<double>();
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                vertices.Add(Math.Round(mesh.Vertices[i].X, 3) * -1);
                vertices.Add(Math.Round(mesh.Vertices[i].Z, 3));
                vertices.Add(Math.Round(mesh.Vertices[i].Y, 3));
            }

            // Create position object
            dynamic position = new ExpandoObject();
            position.itemSize = 3;
            position.type = "Float32Array";
            position.array = vertices;
            position.normalized = false;

            // Fill the normals list
            List<double> normals = new List<double>();
            for (int i = 0; i < mesh.Normals.Count; i++)
            {
                normals.Add(Math.Round(mesh.Normals[i].X, 3) * -1);
                normals.Add(Math.Round(mesh.Normals[i].Z, 3));
                normals.Add(Math.Round(mesh.Normals[i].Y, 3));
            }

            // Create normal object
            dynamic normal = new ExpandoObject();
            normal.itemSize = 3;
            normal.type = "Float32Array";
            normal.array = normals;
            normal.normalized = false;

            // Fill the uvs list
            //List<double> uvs = new List<double>();
            //for (int i = 0; i < mesh.TextureCoordinates.Count; i++)
            //{
            //    uvs.Add(Math.Round(mesh.TextureCoordinates[i].X, 3));
            //  uvs.Add(Math.Round(mesh.TextureCoordinates[i].Y, 3));
            //}

            // Create uv object
            //dynamic uv = new ExpandoObject();
            //uv.itemSize = 2;
            //uv.type = "Float32Array";
            //uv.array = uvs;
            //uv.normalized = false;

            // Create attributes object
            dynamic attributes = new ExpandoObject();
            attributes.position = position;
            attributes.normal = normal;
            //attributes.uv = uv;

            // Fill faces list
            List<int> faces = new List<int>();
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                faces.Add(mesh.Faces.GetFace(i).A);
                faces.Add(mesh.Faces.GetFace(i).B);
                faces.Add(mesh.Faces.GetFace(i).C);
            }

            // Create the index object
            dynamic index = new ExpandoObject();
            index.type = "Uint32Array";
            index.array = faces;

            // Create the data object
            dynamic data = new ExpandoObject();
            data.attributes = attributes;
            data.index = index;

            /// Add the bufferGeometry
            dynamic geometry = new ExpandoObject();
            geometry.uuid = Guid.NewGuid(); ;
            geometry.type = "BufferGeometry";
            geometry.data = data;

            /// Add the child
            dynamic child = new ExpandoObject();
            child.uuid = Guid.NewGuid();
            if (name.Length > 0)
                child.name = name;
            child.type = "Mesh";
            child.geometry = geometry.uuid;
            if (material != null)
                child.material = material.Material.uuid;
            child.matrix = new List<double> { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };
            child.castShadow = true;
            child.receiveShadow = true;

            // Wrap the objects in a wrapper object
            GeometryWrapper wrapper = null;
            if (material != null)
                wrapper = new GeometryWrapper(geometry, child, material.Material);
            else
                wrapper = new GeometryWrapper(geometry, child, null);


            // Create JSON string
            string JSON = JsonConvert.SerializeObject(wrapper);

            // Set outputs
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
                return Properties.Resources.Mesh;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("26c59fc0-17a3-4bc8-8158-ddedf5eeb9ba"); }
        }
    }
}