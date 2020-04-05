using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Triceratops
{
    public class MetaData
    {
        public double version = 4.3;
        public string type;
        public string generator = "Triceratops Exporter";

        public MetaData(string Type)
        {
            type = Type;
        }
    }

    public class DecimalColor
    {
        public int Color;

        public DecimalColor(Color color)
        {
            Color = Convert.ToInt32(color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"), 16);
        }
    }

    public class Matrix
    {
        public List<double> Array;

        public Matrix(Point3d center)
        {
            Array = new List<double> { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, center.X * -1, center.Z, center.Y, 1 };
        }
    }

    public class WireframeSettings
    {
        public string WireframeLinejoin;
        public int WireframeLinewidth;

        public WireframeSettings()
        {
            WireframeLinejoin = "round";
            WireframeLinewidth = 1;
        }
    }

    public class Fog
    {
        public string Type;
        public int Color;
        public double Near;
        public double Far;

        public Fog(Color color, double near, double far)
        {
            Type = "Fog";
            Color = new DecimalColor(color).Color;
            Near = near;
            Far = far;
        }
    }

    public class Image
    {
        public Guid Uuid;
        public string Url;

        public Image(string path)
        {
            // Get image type
            string imageType = Path.GetExtension(path).Remove(0, 1);

            // Read image
            Bitmap bmp = new Bitmap(path);

            // Convert the image to a 64 bit representation
            byte[] imageBytes = ImageToByte(bmp);
            string base64ImageString = Convert.ToBase64String(imageBytes);

            // Product image object
            string url = "data:image/" + imageType + ";base64," + base64ImageString;

            Uuid = Guid.NewGuid(); ;
            Url = url;
        }

        // Convert bitmap to a byte array
        private static byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        // Push value within min/max bounds
        private static void WithinMinMaxBounds(ref double value, double min, double max)
        {
            // Ensure the value falls between 0 and 1
            if (value < min)
                value = min;

            if (value > max)
                value = max;
        }

        // Desaturate the image
        private static void DesaturateBitmap(ref Bitmap bmp, double saturation)
        {
            // Ensure the saturation value falls between 0 and 1
            WithinMinMaxBounds(ref saturation, 0, 1);

            // Get the height and width
            int height = bmp.Height;
            int width = bmp.Width;

            // Loop over image pixels and desaturate values
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color color = bmp.GetPixel(i, j);

                    int a = color.A;
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;

                    double avg = (r + g + b) / 3;
                    double desaturation = avg * (1 - saturation);

                    double newR = (r * saturation) + desaturation;
                    double newG = (g * saturation) + desaturation;
                    double newB = (b * saturation) + desaturation;

                    int newRint = Convert.ToInt32(Math.Round(newR));
                    int newGint = Convert.ToInt32(Math.Round(newG));
                    int newBint = Convert.ToInt32(Math.Round(newB));

                    bmp.SetPixel(i, j, Color.FromArgb(a, newRint, newGint, newBint));
                }
            }
        }

        // Lighten or darken the image
        private static void LightenDarkenBitmap(ref Bitmap bmp, double lightness)
        {
            // Make sure the lightness value falls between 0 and 2
            WithinMinMaxBounds(ref lightness, 0, 2);

            // Ensure the value falls between 0 and 1
            Math.Max(lightness, 0);
            Math.Min(lightness, 2);

            // Get the height and width
            int height = bmp.Height;
            int width = bmp.Width;

            // Loop over image pixels and desaturate values
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color color = bmp.GetPixel(i, j);

                    int a = color.A;
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;

                    double newR;
                    double newG;
                    double newB;

                    if (lightness <= 1)
                    {
                        newR = r * lightness;
                        newG = g * lightness;
                        newB = b * lightness;
                    }
                    else
                    {
                        newR = r + ((255 - r) * (lightness - 1));
                        newG = g + ((255 - g) * (lightness - 1));
                        newB = b + ((255 - b) * (lightness - 1));
                    }

                    int newRint = Convert.ToInt32(Math.Round(newR));
                    int newGint = Convert.ToInt32(Math.Round(newG));
                    int newBint = Convert.ToInt32(Math.Round(newB));

                    bmp.SetPixel(i, j, Color.FromArgb(a, newRint, newGint, newBint));
                }
            }
        }
    }


    public class CubeImage
    {
        public Guid Uuid;
        public List<string> Url;

        public CubeImage(List<string> paths)
        {
            Uuid = Guid.NewGuid(); ;
            Url = new List<string>();

            foreach (string path in paths)
            {
                // Get image type
                string imageType = Path.GetExtension(path).Remove(0, 1);

                string imageString = "data:image/" + imageType + ";base64," + BitmapToBase64String(new Bitmap(path));

                Url.Add(imageString);
            }
        }

        // Convert bitmap to a base 64 string
        private string BitmapToBase64String(Bitmap bmp)
        {
            byte[] imageBytes = ImageToByte(bmp);
            string base64ImageString = Convert.ToBase64String(imageBytes);
            return base64ImageString;
        }

        // Convert bitmap to a byte array
        private static byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
    }

    public class Texture
    {
        public dynamic Data;
        public dynamic Image;

        public Texture(dynamic texture, dynamic image)
        {
            Data = texture;
            Image = image;
        }
    }

    public class TextureSettings
    {
        public List<double> Repeat;
        public List<double> Offset;
        public List<double> Center;
        public double Rotation;
        public List<int> Wrap;

        public TextureSettings()
        {

        }
    }

    public class MeshStandardMaterialMaps
    {
        public Texture Map;
        public Texture AlphaMap;
        public double AlphaTest;
        public Texture BumpMap;
        public double BumpScale;
        public Texture DisplacementMap;
        public double DisplacementScale;
        public Texture EnvMap;
        public double EnvMapIntensity;
        public Texture NormalMap;
        public Texture RoughnessMap;
        public Texture MetalnessMap;
        public Texture AoMap;
        public double AoMapIntensity;
        public Texture EmissiveMap;

        public MeshStandardMaterialMaps()
        {

        }
    }

    public class Material
    {
        public dynamic Data;
        public List<dynamic> Textures;
        public List<dynamic> Images;

        public Material(dynamic material)
        {
            Data = material;
        }

        public void AddTexture(Texture texture)
        {
            if (Textures == null)
                Textures = new List<dynamic>();
            if (Images == null)
                Images = new List<dynamic>();

            Textures.Add(texture.Data);
            Images.Add(texture.Image);
        }
    }

    public class Position
    {
        public int ItemSize;
        public string Type;
        public List<double> Array;
        public bool? Normalized = null;

        public Position(Mesh mesh, Point3d center, int decimalAccuracy)
        {
            ItemSize = 3;
            Type = "Float32Array";
            Array = new List<double>();

            // Fill the vertices list
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                double px = Math.Round(mesh.Vertices[i].X - center.X, decimalAccuracy);
                double py = Math.Round(mesh.Vertices[i].Y - center.Y, decimalAccuracy);
                double pz = Math.Round(mesh.Vertices[i].Z - center.Z, decimalAccuracy);

                Array.Add(px * -1);
                Array.Add(pz);
                Array.Add(py);
            }
        }

        public Position(Polyline polyline, Point3d center, int decimalAccuracy)
        {
            ItemSize = 3;
            Type = "Float32Array";
            Array = new List<double>();

            // Fill a list with the polyline's point coordinates
            for (int i = 0; i < polyline.Count; i++)
            {
                // Get point values
                var px = Math.Round(polyline.X[i] - center.X, decimalAccuracy);
                var py = Math.Round(polyline.Y[i] - center.Y, decimalAccuracy);
                var pz = Math.Round(polyline.Z[i] - center.Z, decimalAccuracy);

                // Add point values to array
                Array.Add(px * -1);
                Array.Add(pz);
                Array.Add(py);
            }
        }

        public Position(List<Point3d> points, Point3d center, int decimalAccuracy)
        {
            ItemSize = 3;
            Type = "Float32Array";
            Array = new List<double>();

            foreach (Point3d point in points)
            {
                double px = Math.Round(point.X - center.X, decimalAccuracy);
                double py = Math.Round(point.Y - center.Y, decimalAccuracy);
                double pz = Math.Round(point.Z - center.Z, decimalAccuracy);

                Array.Add(px * -1);
                Array.Add(pz);
                Array.Add(py);
            }
        }
    }

    public class ColorArray
    {
        public int ItemSize;
        public string Type;
        public List<double> Array;
        public bool? Normalized = null;

        public ColorArray(Mesh mesh)  
        {
            ItemSize = 3;
            Type = "Float32Array";
            Array = new List<double>();

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                double colorR = Math.Round((float)mesh.VertexColors[i].R / 255, 3);
                double colorG = Math.Round((float)mesh.VertexColors[i].G / 255, 3);
                double colorB = Math.Round((float)mesh.VertexColors[i].B / 255, 3);

                Array.Add(colorR);
                Array.Add(colorG);
                Array.Add(colorB);
            }
        }

        public ColorArray(List<Color> colors)
        {
            ItemSize = 3;
            Type = "Float32Array";
            Array = new List<double>();

            for (int i = 0; i < colors.Count; i++)
            {
                double colorR = Math.Round((float)colors[i].R / 255, 3);
                double colorG = Math.Round((float)colors[i].G / 255, 3);
                double colorB = Math.Round((float)colors[i].B / 255, 3);

                Array.Add(colorR);
                Array.Add(colorG);
                Array.Add(colorB);
            }
        }
    }

    public class Normal
    {
        public int ItemSize;
        public string Type;
        public List<double> Array;
        public bool? Normalized = null;

        public Normal(Rhino.Geometry.Mesh mesh, int decimalAccuracy)
        {
            ItemSize = 3;
            Type = "Float32Array";

            // Fill the normals list
            Array = new List<double>();
            for (int i = 0; i < mesh.Normals.Count; i++)
            {
                Array.Add(Math.Round(mesh.Normals[i].X, decimalAccuracy) * -1);
                Array.Add(Math.Round(mesh.Normals[i].Z, decimalAccuracy));
                Array.Add(Math.Round(mesh.Normals[i].Y, decimalAccuracy));
            }

            // Fill the uvs list
            List<double> uvs = new List<double>();
            for (int i = 0; i < mesh.TextureCoordinates.Count; i++)
            {
                uvs.Add(Math.Round(mesh.TextureCoordinates[i].X, 3));
                uvs.Add(Math.Round(mesh.TextureCoordinates[i].Y, 3));
            }
        }
    }

    public class Uv
    {
        public int ItemSize;
        public string Type;
        public List<double> Array;
        public bool? Normalized = null;

        public Uv(Rhino.Geometry.Mesh mesh)
        {
            ItemSize = 2;
            Type = "Float32Array";

            // Fill the uvs list
            Array = new List<double>();
            for (int i = 0; i < mesh.TextureCoordinates.Count; i++)
            {
                Array.Add(Math.Round(mesh.TextureCoordinates[i].X, 3));
                Array.Add(Math.Round(mesh.TextureCoordinates[i].Y, 3));
            }
        }
    }

    public class IndexObject
    {
        //public int ItemSize;
        public string Type;
        public List<int> Array;
        public bool? Normalized = null;

        public IndexObject(Mesh mesh)
        {
            // Fill faces list
            Array = new List<int>();
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                Array.Add(mesh.Faces.GetFace(i).A);
                Array.Add(mesh.Faces.GetFace(i).B);
                Array.Add(mesh.Faces.GetFace(i).C);
            }

            // Specify the array type based on the highest index value in the list
            if (Array.Max() <= 255)
                Type = "Uint8Array";
            else if (Array.Max() <= 65635)
                Type = "Uint16Array";
            else
                Type = "Uint32Array";
        }
    }

    /// <summary>
    /// Initializes a new instance of the BufferGeometry class.
    /// </summary>
    public class BufferGeometry
    {
        public Guid Uuid;
        public string Type = "BufferGeometry";
        public dynamic Data;

        public BufferGeometry(Mesh mesh, Point3d center, int decimalAccuracy, bool vertexColors = false)
        {
            // Add a Guid
            Uuid = Guid.NewGuid();

            // If the meshes have quads, triangulate them
            mesh.Faces.ConvertQuadsToTriangles();

            // Create attributes object
            dynamic attributes = new ExpandoObject();
            attributes.Position = new Position(mesh, center, decimalAccuracy);
            attributes.Normal = new Normal(mesh, decimalAccuracy);
            attributes.Uv = new Uv(mesh);
            if (vertexColors)
                attributes.Color = new ColorArray(mesh);

            // Create index object
            dynamic Index = new IndexObject(mesh);

            Data = new ExpandoObject();
            Data.Attributes = attributes;
            Data.Index = Index;
        }

        public BufferGeometry(Polyline polyline, Point3d center, int decimalAccuracy, bool vertexColors = false)
        {
            // Add a Guid
            Uuid = Guid.NewGuid();

            // Create attributes object
            dynamic attributes = new ExpandoObject();
            attributes.Position = new Position(polyline, center, decimalAccuracy);

            Data = new ExpandoObject();
            Data.attributes = attributes;
        }

        public BufferGeometry(List<Point3d> points, Point3d center, int decimalAccuracy)
        {
            // Add a Guid
            Uuid = Guid.NewGuid();

            // Create attributes object
            dynamic attributes = new ExpandoObject();
            attributes.Position = new Position(points, center, decimalAccuracy);

            Data = new ExpandoObject();
            Data.attributes = attributes;
        }

        public void AddVertexColors(List<Color> colors)
        {
            ColorArray colorArray = new ColorArray(colors);
            Data.attributes.Color = colorArray;
        }

        public void AddLineDistance()
        {
            
            // Create list to populate with lineDistances
            List<double> lineDistances = new List<double>();
            Point3d previousPt = new Point3d();
            double previousDistance = 0;
            double distance = 0;

            // Loop over vertices and measure the distance from start to each vertex
            for (int i = 0; i < Data.attributes.Position.Array.Count; i += 3)
            {
                // Get point values
                var px = Data.attributes.Position.Array[i];
                var py = Data.attributes.Position.Array[i + 1];
                var pz = Data.attributes.Position.Array[i + 2];

                // Distance to previous point
                Point3d pt = new Point3d(px, py, pz);
                if (i == 0)
                {
                    lineDistances.Add(0);
                }   
                else
                {
                    distance = pt.DistanceTo(previousPt) + previousDistance;
                    lineDistances.Add(distance);
                }
                
                previousPt = pt;
                previousDistance = distance;
            }

            dynamic lineDistance = new ExpandoObject();
            lineDistance.type = "Float32Array";
            lineDistance.itemSize = 1;
            lineDistance.count = lineDistances.Count / 3;
            lineDistance.array = lineDistances;

            Data.attributes.lineDistance = lineDistance;
        }
    }


    public class Object3d
    {
        public MetaData Metadata;
        public dynamic @Object;
        public List<BufferGeometry> Geometries;
        public List<dynamic> Materials;
        public List<dynamic> Textures;
        public List<dynamic> Images;

        public Object3d(dynamic object3d)
        {
            Metadata = new MetaData("Object3d");
            @Object = object3d;
        }

        public void AddGeometry(BufferGeometry geometry)
        {
            // If geometries does not have a list, add an empty list
            if (Geometries == null)
                Geometries = new List<BufferGeometry>();

            // Add the specified geometry
            Geometries.Add(geometry);
        }

        public void AddMaterial(dynamic material)
        {
            // If geometries does not have a list, add an empty list
            if (Materials == null)
                Materials = new List<dynamic>();

            // Add the specified geometry
            Materials.Add(material);
        }

        public void AddTexture(dynamic texture)
        {
            // If geometries does not have a list, add an empty list
            if (Textures == null)
                Textures = new List<dynamic>();

            // Add the specified geometry
            Textures.Add(texture);
        }

        public void AddImage(dynamic image)
        {
            // If images does not have a list, add an empty list
            if (Images == null)
                Images = new List<dynamic>();

            // Add the specified image
            Images.Add(image);
        }
    }
}