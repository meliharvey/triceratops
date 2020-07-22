using System;
using System.Collections.Generic;
using System.Dynamic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Triceratops
{
    public class UserData_Component : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public UserData_Component()
          : base("UserData", "UserData",
              "Create custom properties as key, value pairs.",
              "Triceratops", "Geometry")
        {
        }

        // Place in a partition
        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.quarternary;
            }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Keys", "K", "A list of keys (one branch per object)", GH_ParamAccess.list);
            pManager.AddGenericParameter("Values", "V", "A list of values (one branch per object)", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("UserData", "U", "Custom properties as UserData objects", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            List<string> keys = new List<string>();
            List<object> values = new List<object>();

            // Reference the inputs
            if (!DA.GetDataList(0, keys))
                return;
            if (!DA.GetDataList(1, values))
                return;

            // Make sure same number of keys and values
            if (keys.Count != values.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There must must be the same number of keys and values per object.");
            }

            // Create a dictionary
            IDictionary<string, object> properties = new ExpandoObject();
            for (int i = 0; i < keys.Count; i++)
            {
                if (values[i] is GH_Boolean)
                {
                    GH_Boolean n = (GH_Boolean)values[i];
                    bool value = n.Value;
                    properties[keys[i]] = value;
                }
                else if (values[i] is GH_Number)
                {
                    GH_Number n = (GH_Number)values[i];
                    double value = n.Value;
                    properties[keys[i]] = value;
                }
                else if (values[i] is GH_Integer)
                {
                    GH_Integer n = (GH_Integer)values[i];
                    int value = n.Value;
                    properties[keys[i]] = value;
                }
                else if (values[i] is GH_String)
                {
                    GH_String n = (GH_String)values[i];
                    string value = n.Value;
                    properties[keys[i]] = value;
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Item of type [{values[i].GetType()}] is not supported. Only support GH_Bool, GH_Number, GH_Integer, and GH_String.");
                }
            }

            // Wrap in a custom class
            UserData userData = new UserData(properties);

            // Set output references
            DA.SetData(0, userData);
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
                return Properties.Resources.Tri_UserData;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6c6d7872-14d7-41d0-9a33-8a2e28417d84"); }
        }
    }
}