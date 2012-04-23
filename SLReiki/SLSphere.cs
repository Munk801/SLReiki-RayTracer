using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLReiki
{
    /// <summary>
    /// Represents a sphere primitive.  derives from a ray primitive which provides abstract classes for intersection and finding a point normal
    /// </summary>
    class SLSphere : SLRayPrimitive
    {
        SLVector3f position;
        float radius;

        public SLSphere(SLVector3f pos, float rad, Color col)
        {
            position = pos;
            radius = rad;
            PrimitiveColor = col;
        }

        public override float Intersect(SLRay currentRay)
        {
            SLVector3f e = currentRay.Origin;
            SLVector3f o = position;
            SLVector3f d = currentRay.Direction;
            SLVector3f rayToSphereOrigin = o - e;

            float b = rayToSphereOrigin.Dot(d);

            float h = radius * radius + b * b - rayToSphereOrigin.x * rayToSphereOrigin.x - rayToSphereOrigin.y * rayToSphereOrigin.y - rayToSphereOrigin.z * rayToSphereOrigin.z;

            // Early out
            if (h < 0)
                return -1;

            h = b - (float)Math.Sqrt(h);

            if (h < 0)
                return -1;
            else return h;
        }

        public override SLVector3f PointNormal(SLVector3f point)
        {
            SLVector3f pNormal = (point - position);
            pNormal.Normalize();
            return pNormal;
        }
    }
}
