using System;
using System.Collections.Generic;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
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
            // Declare variables
            List<GeometryWrapper> geometries = new List<GeometryWrapper>();

            // Reference the inputs
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

            // Create the export object
            dynamic exportObject = new ExpandoObject();
            exportObject.metadata = metadata;
            exportObject.geometries = new List<dynamic>();

            // Create a list for materials, and track ids to ensure no duplicates
            List<dynamic> materials = new List<dynamic>();
            List<string> materialIds = new List<string>();

            /// Loop over all of the imported geometries to add to object
            foreach (GeometryWrapper geometry in geometries)
            {
                // Add the child objects
                scene.children.Add(geometry.Child);
                exportObject.geometries.Add(geometry.Geometry);

                if (geometry.Material != null)
                /// If a material exists and it hasn't been added to the scene yet, add it to the scene 
                    if (!materialIds.Contains(geometry.Material.uuid.ToString()))
                    {
                        materials.Add(geometry.Material);
                        materialIds.Add(geometry.Material.uuid.ToString());
                    }
            }

            // Add materials and object to the export object
            exportObject.materials = materials;
            exportObject.@object = scene;

            // Create a JSON string
            string JSON = JsonConvert.SerializeObject(exportObject);

            /// Wrap the bufferGeometries and children to the wrapper
            SceneWrapper wrapper = new SceneWrapper(exportObject);

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