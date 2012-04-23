using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLReiki
{
    public class SLRay
    {
        public SLVector3f Origin;
        public SLVector3f Direction;
        public const float Max_Distance = 1000.0f;
        public SLRayPrimitive ClosestPrimitive;
        public float ClosestPointDistance;
        public SLVector3f ClosestPoint;

        public SLRay(SLVector3f o, SLVector3f d)
        {
            Origin = o;
            Direction = d;
            ClosestPointDistance = Max_Distance;
            ClosestPrimitive = null;

        }
    }
}
