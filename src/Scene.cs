using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Drawing;

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
            pManager.AddGenericParameter("Objects", "O", "Add all points, lines, meshes, and lights", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "N", "Give a name to the scene", GH_ParamAccess.item, "");
            pManager.AddGenericParameter("Background Color", "C", "The background color", GH_ParamAccess.item);
            pManager.AddGenericParameter("Background", "B", "The background image (CubeTexture)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Environment", "E", "The image that shows up in material reflections (CubeTexture)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Fog", "F", "Add fog to the scene", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Scene Object", "S", "Output scene", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variables
            List<Object3d> object3ds = new List<Object3d>();
            String name = "";
            Color backgroundColor = Color.LightGray;
            Texture background = null;
            Texture environment = null;
            

            // Reference the inputs
            DA.GetDataList(0, object3ds);
            DA.GetData(1, ref name);
            DA.GetData(2, ref backgroundColor);
            DA.GetData(3, ref background);
            DA.GetData(4, ref environment);

            /// Scene
            dynamic scene = new ExpandoObject();
            scene.Uuid = Guid.NewGuid();
            scene.Type = "Scene";
            scene.Name = name;
            scene.Matrix = new Matrix(Point3d.Origin).Array;
            scene.Children = new List<dynamic>();
            

            Object3d sceneObject = new Object3d(scene);

            // Create lists for ids to make sure there are no duplicates
            List<string> geometriesIds = new List<string>();
            List<string> materialsIds = new List<string>();
            List<string> texturesIds = new List<string>();
            List<string> imagesIds = new List<string>();

            // If there is a background, add it to the scene
            if (background != null)
            {
                scene.Background = background.Data.Uuid;

                if (!texturesIds.Contains(background.Data.Uuid.ToString()))
                {
                    sceneObject.AddTexture(background.Data);
                    texturesIds.Add(background.Data.Uuid.ToString());
                }

                if (!imagesIds.Contains(background.Image.Uuid.ToString()))
                {
                    sceneObject.AddImage(background.Image);
                    imagesIds.Add(background.Image.Uuid.ToString());
                }
            }
            else
            {
                scene.Background = new DecimalColor(backgroundColor).Color;
            }
                
            // If there is an environment, add it to the scene
            if (environment != null)
            {
                scene.Environment = environment.Data.Uuid;

                if (!texturesIds.Contains(environment.Data.Uuid.ToString()))
                {
                    sceneObject.AddTexture(environment.Data);
                    texturesIds.Add(environment.Data.Uuid.ToString());
                }

                if (!imagesIds.Contains(environment.Image.Uuid.ToString()))
                {
                    sceneObject.AddImage(environment.Image);
                    imagesIds.Add(environment.Image.Uuid.ToString());
                }
            }

            // Loop over all the threejs objects and add them to the scene
            foreach (Object3d object3d in object3ds)
            {
                // Add the objects to the scene as child objects
                scene.Children.Add(object3d.Object);

                // If Threejs object includes geometries list, add geometry objects to the scene
                if (object3d.Geometries != null)
                {
                    foreach (dynamic geometry in object3d.Geometries)
                    {
                        if (!geometriesIds.Contains(geometry.Uuid.ToString()))
                        {
                            sceneObject.AddGeometry(geometry);
                            geometriesIds.Add(geometry.Uuid.ToString());
                        }
                    }
                }

                // If Threejs object includes materials list, add material obects to the scene
                if (object3d.Materials != null)
                {
                    foreach (dynamic material in object3d.Materials)
                    {
                        if (!materialsIds.Contains(material.Uuid.ToString()))
                        {
                            sceneObject.AddMaterial(material);
                            materialsIds.Add(material.Uuid.ToString());
                        }
                    }
                }

                // If Threejs object includes textures list, add the textures to the scene
                if (object3d.Textures != null)
                {
                    foreach (dynamic texture in object3d.Textures)
                    {
                        if (!texturesIds.Contains(texture.Uuid.ToString()))
                        {
                            sceneObject.AddTexture(texture);
                            texturesIds.Add(texture.Uuid.ToString());
                        }
                    }
                }

                // If Threejs object includes images list, add the images to the scene
                if (object3d.Images != null)
                {
                    foreach (dynamic image in object3d.Images)
                    {
                        if (!imagesIds.Contains(image.Uuid.ToString()))
                        {
                            sceneObject.AddImage(image);
                            imagesIds.Add(image.Uuid.ToString());
                        }
                    }
                }
            }

            // Set the outputs
            DA.SetData(0, sceneObject);
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
                return Properties.Resources.Tri_Scene;
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