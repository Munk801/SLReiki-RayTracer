using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLReiki
{
    public abstract class SLRayPrimitive
    {
        // Color, Intersect with Ray, Normal
        public Color PrimitiveColor;

        public abstract float Intersect(SLRay currentRay);

        public abstract SLVector3f PointNormal(SLVector3f point);
    }
}
