using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;

namespace Triceratops
{
    public class DirectionalLight_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DirectionalLight class.
        /// </summary>
        public DirectionalLight_Component()
          : base("DirectionalLight", "DirLight",
              "A light casting parallel rays across a scene.",
              "Triceratops", "Lights")
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
            pManager.AddPointParameter("Position", "P", "Location of point light", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "C", "The color of the light", GH_ParamAccess.item, Color.White);
            pManager.AddNumberParameter("Intensity", "I", "The intensity of the light", GH_ParamAccess.item, 0.5);
            pManager.AddTextParameter("Name", "N", "The name of the light", GH_ParamAccess.item, "");
            pManager.AddBooleanParameter("CastShadow", "S", "Set to true to cast shadows", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Shadow Top", "St", "The top extents of the shadow", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Shadow Bottom", "Sb", "The bottom extents of the shadow", GH_ParamAccess.item, -10);
            pManager.AddNumberParameter("Shadow Right", "Sr", "The right extents of the shadow", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Shadow Left", "Sl", "The left extents of the shadow", GH_ParamAccess.item, -10);
            pManager.AddNumberParameter("Shadow Near", "Sn", "The nearest to the light that shadows will be cast", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Shadow Far", "Sf", "The farthest from the light that shadows will be cast", GH_ParamAccess.item, 100);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Helper", "H", "A helper to aid in positioning the light", GH_ParamAccess.list);
            pManager.AddGenericParameter("Light", "L", "The light object", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            Point3d position = new Point3d(0, 10, 10);
            Color color = Color.White;
            double intensity = 0.5;
            string name = "";
            Boolean castShadow = false;
            double shadowTop = 10;
            double shadowBottom = -10;
            double shadowRight = 10;
            double shadowLeft = -10;
            double shadowNear = 0;
            double shadowFar = 100;

            // Reference the inputs
            DA.GetData(0, ref position);
            DA.GetData(1, ref color);
            DA.GetData(2, ref intensity);
            DA.GetData(3, ref name);
            DA.GetData(4, ref castShadow);
            DA.GetData(5, ref shadowTop);
            DA.GetData(6, ref shadowBottom);
            DA.GetData(7, ref shadowRight);
            DA.GetData(8, ref shadowLeft);
            DA.GetData(9, ref shadowNear);
            DA.GetData(10, ref shadowFar);

            // Build the shadow camera
            dynamic camera = new ExpandoObject();
            camera.Uuid = Guid.NewGuid();
            camera.Type = "OrthographicCamera";
            camera.Top = shadowTop;
            camera.Bottom = shadowBottom;
            camera.Right = shadowRight;
            camera.Left = shadowLeft;
            camera.Near = shadowNear;
            camera.Far = shadowFar;

            dynamic shadow = new ExpandoObject();
            shadow.camera = camera;

            // Build the light object
            dynamic lightObject = new ExpandoObject();
            lightObject.Uuid = Guid.NewGuid();
            lightObject.Type = "DirectionalLight";
            if (name.Length > 0)
                lightObject.Name = name;
            lightObject.Color = new DecimalColor(color).Color;
            lightObject.Intensity = intensity;
            lightObject.Matrix = new Matrix(position).Array;
            lightObject.CastShadow = castShadow;
            lightObject.Shadow = shadow;

            // Create a helper to aid in visualizing the light's behavior
            List<Curve> helper = new List<Curve>();

            Circle circle1 = new Circle(Plane.WorldXY, position, 0.5);
            Circle circle2 = new Circle(Plane.WorldYZ, position, 0.5);
            Circle circle3 = new Circle(Plane.WorldZX, position, 0.5);

            helper.Add(circle1.ToNurbsCurve());
            helper.Add(circle2.ToNurbsCurve());
            helper.Add(circle3.ToNurbsCurve());

            var line = new Rhino.Geometry.Line(position, new Point3d(0, 0, 0));
            helper.Add(line.ToNurbsCurve());

            // If it casts a shadow, create a shadow helper
            if (castShadow)
            {
                // Create the plane's x vector
                Vector3d vectorX = new Vector3d(-position);
                vectorX.Rotate(90 * (Math.PI / 180), Vector3d.ZAxis);
                vectorX.Z = 0;

                // Create the plane's y vector
                Vector3d vectorY = new Vector3d(-position);
                vectorY.Rotate(-90 * (Math.PI / 180), vectorX);

                Plane plane = new Plane(position, vectorX, vectorY);
                Interval xSize = new Interval(-shadowRight, -shadowLeft);
                Interval ySize = new Interval(shadowBottom, shadowTop);
                Interval zSize = new Interval(shadowNear, shadowFar);
                Box shadowBox = new Box(plane, xSize, ySize, zSize);
                Brep brep = shadowBox.ToBrep();

                for (int i = 0; i < brep.Edges.Count; i++)
                {
                    helper.Add(brep.Edges[i].EdgeCurve);
                }
            }

            // Create object3d
            Object3d object3d = new Object3d(lightObject);

            // Set outputs
            DA.SetDataList(0, helper);
            DA.SetData(1, object3d);
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
                return Properties.Resources.Tri_DirectionalLight;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3e9a0115-184b-4c48-93a2-2fafda6c9ab7"); }
        }
    }
}