using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLReiki
{
    public class SLVector3f
    {
        public float x, y, z;

        // Default Constructor
        public SLVector3f()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }

        public SLVector3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // Default Destructor
        ~SLVector3f()
        {
        }

        #region Operator Overloaders
        public static SLVector3f operator +(SLVector3f a, SLVector3f b)
        {
            return new SLVector3f(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static SLVector3f operator -(SLVector3f a, SLVector3f b)
        {
            return new SLVector3f(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static SLVector3f operator -(SLVector3f a)
        {
            return new SLVector3f(-a.x, -a.y, -a.z);
        }

        public static SLVector3f operator *(SLVector3f a, float x)
        {
            return new SLVector3f(a.x * x, a.y * x, a.z * x);
        }

        public static SLVector3f operator /(SLVector3f a, float x)
        {
            return new SLVector3f(a.x / x, a.y / x, a.z / x);
        }
        #endregion 

        public float Dot(SLVector3f b)
        {
            return (this.x * b.x + this.y * b.y + this.z * b.z);
        }

        public SLVector3f Cross(SLVector3f v)
        {
            return new SLVector3f(this.y * v.z - this.z * v.y,
                                  this.z * v.x - this.x * v.z,
                                  this.x * v.y - this.y * v.x);
        }

        public void Normalize()
        {
            float norm = (float)(1.0f / (Math.Sqrt(this.Dot(this))));
            this.x *= norm;
            this.y *= norm;
            this.z *= norm;
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public SLVector3f Reflection(SLVector3f normal)
        {
            SLVector3f negVector = -this;
            SLVector3f reflectedDir = normal * (2.0f * negVector.Dot(normal));
            return reflectedDir;
        }
    }
}
