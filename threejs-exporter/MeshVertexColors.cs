using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace triceratops
{
    public class MeshVertexColors : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MeshVertexColors()
          : base("MeshVertexColors", "MeshVertex",
              "Mesh colored with vertex colors.",
              "Triceratops", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "G", "Mesh geometry", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "N", "Name of mesh", GH_ParamAccess.list, "");
            pManager.AddGenericParameter("Material", "M", "Threejs material", GH_ParamAccess.item);
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
            List<Rhino.Geometry.Mesh> meshes = new List<Rhino.Geometry.Mesh>();
            List<string> names = new List<string>();
            MaterialWrapper material = null;

            if (!DA.GetDataList(0, meshes))
                return;
            if (!DA.GetDataList(1, names))
                return;
            DA.GetData(2, ref material);

            List<dynamic> bufferGeometries = new List<dynamic>();
            List<dynamic> children = new List<dynamic>();

            int counter = 0;

            //Loop through each mesh and create a geometry object, and schene child object for it.
            foreach (Rhino.Geometry.Mesh mesh in meshes)
            {

                // Triangulate all quads.
                // bufferGeometry doesn't read quad mesh faces.
                if (!mesh.Faces.ConvertQuadsToTriangles())
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Error triangulating quad meshes. Try triangulating quads before feeding into this component.");
                };

                dynamic attributes = new ExpandoObject();
                dynamic position = new ExpandoObject();
                dynamic color = new ExpandoObject();
                dynamic normal = new ExpandoObject();
                dynamic uv = new ExpandoObject();
                dynamic index = new ExpandoObject();

                Guid guid = Guid.NewGuid();

                List<double> vertices = new List<double>();
                List<double> colors = new List<double>();
                for (int i = 0; i < mesh.Vertices.Count; i++)
                {
                    vertices.Add(Math.Round(mesh.Vertices[i].X, 3) * -1);
                    vertices.Add(Math.Round(mesh.Vertices[i].Z, 3));
                    vertices.Add(Math.Round(mesh.Vertices[i].Y, 3));

                    colors.Add((float)mesh.VertexColors[i].R/255);
                    colors.Add((float)mesh.VertexColors[i].G/255);
                    colors.Add((float)mesh.VertexColors[i].B/255);
                }

                List<double> normals = new List<double>();
                for (int i = 0; i < mesh.Normals.Count; i++)
                {
                    normals.Add(Math.Round(mesh.Normals[i].X, 3) * -1);
                    normals.Add(Math.Round(mesh.Normals[i].Z, 3));
                    normals.Add(Math.Round(mesh.Normals[i].Y, 3));
                }

                List<double> uvs = new List<double>();
                for (int i = 0; i < mesh.TextureCoordinates.Count; i++)
                {
                    uvs.Add(Math.Round(mesh.TextureCoordinates[i].X, 3));
                    uvs.Add(Math.Round(mesh.TextureCoordinates[i].Y, 3));
                }

                position.itemSize = 3;
                position.type = "Float32Array";
                position.array = vertices;
                position.normalized = false;

                color.itemSize = 3;
                color.type = "Float32Array";
                color.array = colors;

                normal.itemSize = 3;
                normal.type = "Float32Array";
                normal.array = normals;
                normal.normalized = false;

                uv.itemSize = 2;
                uv.type = "Float32Array";
                uv.array = uvs;
                uv.normalized = false;

                attributes.position = position;
                attributes.color = color;
                attributes.normal = normal;
                attributes.uv = uv;

                List<int> faces = new List<int>();

                for (int i = 0; i < mesh.Faces.Count; i++)
                {
                    faces.Add(mesh.Faces.GetFace(i).A);
                    faces.Add(mesh.Faces.GetFace(i).B);
                    faces.Add(mesh.Faces.GetFace(i).C);
                }

                index.type = "Uint32Array";
                index.array = faces;

                dynamic data = new ExpandoObject();
                data.attributes = attributes;
                data.index = index;

                /// Add the bufferGeometry
                dynamic bufferGeometry = new ExpandoObject();
                bufferGeometry.uuid = guid;
                bufferGeometry.type = "BufferGeometry";
                bufferGeometry.data = data;

                bufferGeometries.Add(bufferGeometry);

                /// Add the child
                dynamic child = new ExpandoObject();
                child.uuid = Guid.NewGuid();

                if (names.Count == 0)
                    child.name = "";
                else if (names.Count == 1)
                    child.name = names[0];
                else
                    child.name = names[counter];

                child.type = "Mesh";
                child.geometry = guid;
                child.material = material.Material.uuid;
                child.matrix = new List<double> { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };

                children.Add(child);

                counter += 1;
            }

            material.Material.vertexColors = 2;
            material.Material.color = "0xffffff";

            /// Wrap the bufferGeometries and children to the wrapper
            MeshWrapper wrapper = new MeshWrapper(bufferGeometries, children, material.Material);

            string JSON = JsonConvert.SerializeObject(wrapper);

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
                return Properties.Resources.MeshVertexColors;
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