using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class Line : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Line()
          : base("Line", "Line",
              "Create a Three.js line geometry",
              "Triceratops", "Geometry")
        {
        }

        // Place in a partition
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Line", "L", "Line geometry", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Line name", GH_ParamAccess.item, "");
            pManager.AddGenericParameter("Line Material", "M", "Line material", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("JSON", "J", "The JSON string", GH_ParamAccess.item);
            pManager.AddGenericParameter("Geometry", "G", "The line geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declate variables
            Curve curve = null;
            string name = "";
            MaterialWrapper material = null;

            // Reference the inputs
            DA.GetData(0, ref curve);
            DA.GetData(1, ref name);
            DA.GetData(2, ref material);

            // Throw error message if curve cannot be converted to a polyline
            if (!curve.IsPolyline())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input must be polyine, curves are not allowed.");
                return;
            }

            // Convert curve to polyline
            Polyline polyline = null;
            curve.TryGetPolyline(out polyline);

            // Fill a list with the polyline's point coordinates
            List<double> array = new List<double>();
            for (int i = 0; i < polyline.Count; i++)
            {
                // Get point values
                var px = polyline.X[i];
                var py = polyline.Y[i];
                var pz = polyline.Z[i];

                // Add point values to array
                array.Add(Math.Round(px, 3) * -1);
                array.Add(Math.Round(pz, 3));
                array.Add(Math.Round(py, 3));
            }

            //Build the position object
            dynamic position = new ExpandoObject();
            position.itemSize = 3;
            position.type = "Float32Array";
            position.array = array;
            position.normalized = false;

            // Build the attribute object
            dynamic attributes = new ExpandoObject();
            attributes.position = position;

            // If LineDashedMaterial is used, add the lineDistance attribute
            if (material != null && string.Equals(material.Material.type, "LineDashedMaterial"))
            {
                // Create list to populate with lineDistances
                List<double> lineDistances = new List<double>();
                Point3d previousPt = new Point3d();

                // Loop over vertices and measure the distance from start to each vertex
                for (int i = 0; i < polyline.Count; i++)
                {
                    // Get point values
                    var px = polyline.X[i];
                    var py = polyline.Y[i];
                    var pz = polyline.Z[i];

                    // Distance to previous point
                    Point3d pt = new Point3d(px, py, pz);
                    if (i == 0)
                        lineDistances.Add(0);
                    else
                        lineDistances.Add(pt.DistanceTo(previousPt) + lineDistances[i - 1]);
                    previousPt = pt;
                }

                // Build the lineDistance object
                dynamic lineDistance = new ExpandoObject();
                lineDistance.type = "Float32Array";
                lineDistance.array = lineDistances;
                lineDistance.count = polyline.Count;
                lineDistance.itemSize = 1;

                // Add the lineDistance object to the attributes object
                attributes.lineDistance = lineDistance;
            }
                
            // Build the data object
            dynamic data = new ExpandoObject();
            data.attributes = attributes;

            // Build the geometry object
            dynamic geometry = new ExpandoObject();
            geometry.uuid = Guid.NewGuid();
            geometry.type = "BufferGeometry";
            geometry.data = data;

            // Build the child object
            dynamic child = new ExpandoObject();
            child.uuid = Guid.NewGuid();
            if (name.Length > 0) child.name = name;
            child.type = "Line";
            child.geometry = geometry.uuid;
            if (material != null) child.material = material.Material.uuid;
            child.matrix = new List<double> { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };

            // Fill the children list
            List<dynamic> children = new List<dynamic>();
            children.Add(child);

            // Wrap the objects in a wrapper object
            GeometryWrapper wrapper = null;
            if (material != null)
                wrapper = new GeometryWrapper(geometry, child, material.Material);
            else
                wrapper = new GeometryWrapper(geometry, child, null);

            // Serialize wrapper object
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
                return Properties.Resources.Line;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("251b9e7c-172f-4cfd-b786-7ac0892bd68d"); }
        }
    }
}