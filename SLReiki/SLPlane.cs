using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SLReiki
{
    class SLPlane : SLRayPrimitive
    {
        public SLVector3f pNormal;
        public float Offset;

        public SLPlane(SLVector3f normal, float offset, Color color)
        {
            pNormal = normal;
            Offset = offset;
            PrimitiveColor = color;
        }

        public override float Intersect(SLRay currentRay)
        {
            float normDotRay = pNormal.Dot(currentRay.Direction);
            if (normDotRay == 0)
                return -1;

            float d = -(pNormal.Dot(currentRay.Origin) - Offset) / normDotRay;

            if (d < 0)
                return -1;
            else
                return d;
        }

        public override SLVector3f PointNormal(SLVector3f point)
        {
            return pNormal;
        }
    }
}
