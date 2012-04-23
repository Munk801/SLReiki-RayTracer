using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
namespace SLReiki
{
    class Program
    {
        // IMAGE RESOLUTIONS
        static int ImageWidth = 0;
        static int ImageHeight = 0;
        static float WIDTH_DIV_2;
        static float HEIGHT_DIV_2;

        // CAMERA
        static SLVector3f EYE = new SLVector3f();
        static SLVector3f SPOT = new SLVector3f();
        static SLVector3f UP = new SLVector3f();

        // SCREEN SETUP
        static float FOVY;
        static float ASPECT;

        // AMBIENT
        static float LaR;
        static float LaG;
        static float LaB;

        // TEST
        const float MATERIAL_DIFFUSE_COEFFICIENT = 0.5f;
        const float MATERIAL_REFLECTION_COEFFICIENT = 0.2f;  
        const float MATERIAL_SPECULAR_COEFFICIENT = 2.0f;
        const float MATERIAL_SPECULAR_POWER = 50.0f;  
        const float TINY = 0.0001f;          
        static Color BG_COLOR = Color.Black;

        const int MAX_DEPTH = 0; 
        static SLVector3f dir = new SLVector3f();

        // PRIMITIVES
        static List<SLRayPrimitive> RayObjects;


        // LIGHTS
        static List<SLLight> Lights;
        static Random random;

        // VIRTUAL SCREEN SETUP
        static SLVector3f right;
        static SLVector3f screenU;
        static SLVector3f screenV;
        static SLVector3f screenCenter;

        static void Main(string[] args)
        {
            RayObjects = new List<SLRayPrimitive>();
            Lights = new List<SLLight>();
            ReadScene("scene.urt");
            dir = EYE - SPOT;
            right = UP.Cross(dir);
            right.Normalize();
            screenCenter = dir.Cross(right);
            screenV = screenCenter * (float)Math.Tan(FOVY * 0.5f);
            screenU = right * screenV.Magnitude() * ASPECT;
            WIDTH_DIV_2 = ImageWidth / 2;
            HEIGHT_DIV_2 = ImageHeight / 2;
            random = new Random(01478650229);
            Bitmap outputImage = new Bitmap(ImageWidth, ImageHeight);
            // add some objects
            float distance = 20.0f;
            float offset = distance / 2.0f;
            for (int i = 0; i < 1000; i++)
            {
                float x = (float)(random.NextDouble() * distance) - offset;
                float y = (float)(random.NextDouble() * distance) - offset;
                float z = (float)(random.NextDouble() * 10.0f + 2.0f);
                Color c = Color.FromArgb(255, random.Next(255), random.Next(255), random.Next(255));
                SLSphere s = new SLSphere(new SLVector3f(x, y, z), (float)(random.NextDouble() + 0.5), c);
                RayObjects.Add(s);
            }
            int dotPeriod = ImageHeight / 10;
            Stopwatch stopwatch = new Stopwatch();
            System.Console.WriteLine("Rendering...\n");
            System.Console.WriteLine("|0%---100%|");
            stopwatch.Start();
            for (int j = 0; j < ImageHeight; j++)
            {
                if ((j % dotPeriod) == 0) System.Console.Write("*");
                for (int i = 0; i < ImageWidth; i++)
                {
                    // Go through each pixel to get the color
                    Color c = Render(i, j);
                    outputImage.SetPixel(i, j, c);
                }
            }
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string output = ts.Minutes.ToString() + "_" + ts.Seconds.ToString() + ".png";
            outputImage.Save(output);
        }

        static Color Render(int x, int y)
        {
            
            float sx = (x / WIDTH_DIV_2 - 1.0f);
            float sy =  (1.0f - y / HEIGHT_DIV_2);
            SLVector3f u = screenU * sx;
            SLVector3f v = screenV * sy;
            SLVector3f direction = u + v - dir;
            direction.Normalize();
            SLRay posRay = new SLRay(EYE, direction);
            return SetPixelColor(posRay, 0);
        }

        static void GetClosestIntersection(ref SLRay ray)
        {
            foreach(SLRayPrimitive primitive in RayObjects)
            {
                float d = primitive.Intersect(ray);

                if (d < ray.ClosestPointDistance && d > 0)
                {
                    ray.ClosestPrimitive = primitive;
                    ray.ClosestPointDistance = d;
                }
            }

            ray.ClosestPoint = ray.Origin + (ray.Direction * ray.ClosestPointDistance);
        }



        static void ReadScene(string path)
        {
            StreamReader file = new StreamReader(path);

            string delim = file.ReadLine();
            if (delim != "U5")
                return;
            // Get resolution
            string[] res = file.ReadLine().Split(' ');
            ImageWidth = Int32.Parse(res[0]);
            ImageHeight = Int32.Parse(res[1]);

            // Get Eye
            string[] eye = file.ReadLine().Split(' ');
            EYE = new SLVector3f(float.Parse(eye[0]), float.Parse(eye[1]), float.Parse(eye[2]));
 
            // Get Spot
            string[] spot = file.ReadLine().Split(' ');
            SPOT = new SLVector3f(float.Parse(spot[0]), float.Parse(spot[1]), float.Parse(spot[2]));
                
            // Get Up
            string[] up = file.ReadLine().Split(' ');
            UP = new SLVector3f(float.Parse(up[0]), float.Parse(up[1]), float.Parse(up[2]));

            // Get aspect
            string[] viewVol = file.ReadLine().Split(' ');
            FOVY = float.Parse(viewVol[0]);
            ASPECT = float.Parse(viewVol[1]);

            // Read in Ambient Values
            string[] ambient = file.ReadLine().Split(' ');
            LaR = float.Parse(ambient[0]);
            LaG = float.Parse(ambient[1]);
            LaB = float.Parse(ambient[2]);

            // Read in spheres
            while (!file.EndOfStream)
            {
                string[] line = file.ReadLine().Split(' ');
                string del = line[0];
                switch(del)
                {
                    case "l": // LIGHTS
                        SLVector3f pos = new SLVector3f(float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3]));
                        SLLight l = new SLLight(pos, float.Parse(line[4]), float.Parse(line[5]), float.Parse(line[6]));
                        Lights.Add(l);
                        break;
                    case "s": // SPHERES
                        SLSphere s = new SLSphere(new SLVector3f(float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3])), float.Parse(line[4]), 
                                                  Color.FromArgb((int)float.Parse(line[5])*255, (int)float.Parse(line[6])*255, (int)float.Parse(line[7])*255));
                        RayObjects.Add(s);       
                        break;
                    case "p": // PLANES
                        float offset;
                        if (float.Parse(line[1]) != 0.0f) offset = float.Parse(line[1]);
                        else if (float.Parse(line[2]) != 0.0f) offset = float.Parse(line[2]);
                        else offset = float.Parse(line[3]);
                        float pR = float.Parse(line[7]) * 255.0f;
                        float pG = float.Parse(line[8]) * 255.0f;
                        float pB = float.Parse(line[9]) * 255.0f;

                        SLPlane p = new SLPlane(new SLVector3f(float.Parse(line[4]), float.Parse(line[5]), float.Parse(line[6])), offset,
                            Color.FromArgb((int)pR, (int)pG, (int)pB));
                        RayObjects.Add(p);
                        break;
                }
            }

        }

        static Color SetPixelColor(SLRay ray, int depth)
        {
            // Get closest intersection IF ANY
            GetClosestIntersection(ref ray);

            // Return if the ray didn't hit anything
            if (ray.ClosestPointDistance >= SLRay.Max_Distance || ray.ClosestPrimitive == null)
                return BG_COLOR;

            // Set ambient light
            float r = LaR * ray.ClosestPrimitive.PrimitiveColor.R;
            float g = LaG * ray.ClosestPrimitive.PrimitiveColor.G;
            float b = LaB * ray.ClosestPrimitive.PrimitiveColor.B;
            

            // Add contributions of each light
            SLVector3f normal = ray.ClosestPrimitive.PointNormal(ray.ClosestPoint);
            SLVector3f viewDir = -ray.Direction;

            foreach (SLLight light in Lights)
            {
                SLVector3f lightDir = new SLVector3f();
                float lightDistance;

                // Find light direction and distance
                lightDir = light.position - ray.ClosestPoint;

                lightDistance = lightDir.Magnitude();
                lightDir.Normalize();

                SLRay pointToLight = new SLRay(ray.ClosestPoint + (lightDir * TINY), lightDir);
                pointToLight.ClosestPointDistance = lightDistance;           // IMPORTANT: We only want it to trace as far as the light!
                GetClosestIntersection(ref pointToLight);
                if (pointToLight.ClosestPrimitive != null)                 // We hit something -- ignore this light entirely
                    continue;

                // DIFFUSE LIGHTING
                float lDotN = normal.Dot(lightDir);

                // Clamp
                if (lDotN <= 0)
                    continue;

                float diffColorContrR = (light.color.R * lDotN)/255.0f;
                float diffColorContrG = (light.color.G * lDotN)/ 255.0f;
                float diffColorContrB = (light.color.B * lDotN)/ 255.0f;

                // Add this light's diffuse contribution to our running totals
                r += diffColorContrR * ray.ClosestPrimitive.PrimitiveColor.R;
                g += diffColorContrG * ray.ClosestPrimitive.PrimitiveColor.G;
                b += diffColorContrB * ray.ClosestPrimitive.PrimitiveColor.B;


                if (MATERIAL_SPECULAR_COEFFICIENT > TINY)
                {
                    // Specular component - dot product of light's reflection vector and viewer direction
                    // Direction to the viewer is simply negative of the ray direction
                    SLVector3f lightReflectionDir = normal * (lDotN * 2) - lightDir;
                    float specularFactor = viewDir.Dot(lightReflectionDir);
                    if (specularFactor > 0)
                    {
                        // To get smaller, sharper highlights we raise it to a power and multiply it
                        specularFactor = MATERIAL_SPECULAR_COEFFICIENT * (float)Math.Pow(specularFactor, MATERIAL_SPECULAR_POWER);

                        // Add the specular contribution to our running totals
                        r += MATERIAL_SPECULAR_COEFFICIENT * specularFactor * ray.ClosestPrimitive.PrimitiveColor.R;
                        g += MATERIAL_SPECULAR_COEFFICIENT * specularFactor * ray.ClosestPrimitive.PrimitiveColor.G;
                        b += MATERIAL_SPECULAR_COEFFICIENT * specularFactor * ray.ClosestPrimitive.PrimitiveColor.B;
                    }
                }

                //// Now do reflection, unless we're too deep
                if (depth < MAX_DEPTH && MATERIAL_REFLECTION_COEFFICIENT > TINY)
                {
                    // Set up the reflected ray - notice we move the origin out a tiny bit again
                    SLVector3f reflectedDir = ray.Direction.Reflection(normal);
                    SLRay reflectionRay = new SLRay(ray.ClosestPoint + reflectedDir * TINY, reflectedDir);

                    // And trace!
                    Color reflectionCol = SetPixelColor(reflectionRay, depth + 1);

                    // Add reflection results to running totals, scaling by reflect coeff.
                    r += MATERIAL_REFLECTION_COEFFICIENT * reflectionCol.R;
                    g += MATERIAL_REFLECTION_COEFFICIENT * reflectionCol.G;
                    b += MATERIAL_REFLECTION_COEFFICIENT * reflectionCol.B;
                }
            }
            // Clamp
            if (r > 255) r = 255;
            if (g > 255) g = 255;
            if (b > 255) b = 255;

            // Take care of NaN
            if (r < 0) r = 0;
            if (g < 0) g = 0;
            if (b < 0) b = 0;

            return (Color.FromArgb(255, (int)r, (int)g, (int)b));
        }
    }
}
