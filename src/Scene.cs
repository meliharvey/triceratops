using System;
using System.Collections.Generic;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace triceratops
{
    public class Scene : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Scene()
          : base("Compile Scene", "Scene",
              "Add objects to scene.",
              "Triceratops", "Scene")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Geometry", "G", "Mesh inputs", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("JSON", "J", "Output scene as JSON", GH_ParamAccess.item);
            pManager.AddGenericParameter("Scene", "S", "Output scene", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<MeshWrapper> geometries = new List<MeshWrapper>();

            DA.GetDataList(0, geometries);

            /// Metadata
            dynamic metadata = new ExpandoObject();
            metadata.version = 4.3;
            metadata.type = "Object";
            metadata.generator = "TriceratopsExporter";

            /// Scene
            dynamic scene = new ExpandoObject();
            scene.uuid = Guid.NewGuid();
            scene.type = "Scene";
            scene.name = "";
            scene.matrix = new List<double> { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };
            scene.children = new List<dynamic>();

            List<dynamic> materials = new List<dynamic>();
            List<string> materialIds = new List<string>();

            dynamic exportObject = new ExpandoObject();
            exportObject.metadata = metadata;
            exportObject.geometries = new List<dynamic>();

            /// Loop over all of the imported geometries to add to object
            foreach (MeshWrapper geometry in geometries)
            {
                Console.WriteLine(geometry.Material.uuid.ToString());

                foreach (dynamic child in geometry.Children)
                    scene.children.Add(child);

                foreach (dynamic bufferGeometry in geometry.Geometry)
                    exportObject.geometries.Add(bufferGeometry);

                /// If material doesn't exist yet, add it
                if (!materialIds.Contains(geometry.Material.uuid.ToString()))
                {
                    materials.Add(geometry.Material);
                    materialIds.Add(geometry.Material.uuid.ToString());
                }
            }

            exportObject.materials = materials;
            exportObject.@object = scene;

            string JSON = JsonConvert.SerializeObject(exportObject);

            /// Wrap the bufferGeometries and children to the wrapper
            SceneWrapper wrapper = new SceneWrapper(exportObject);

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
                return Properties.Resources.Scene;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("548efd68-0fd0-4927-aa39-4ddde8337068"); }
        }
    }
}