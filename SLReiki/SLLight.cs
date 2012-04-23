using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SLReiki
{
    /// <summary>
    /// Represents the light in a scene.  A light only has a position and color*intensity*.
    /// </summary>
    public class SLLight
    {
        public SLVector3f position;
        public Color color;

        public SLLight( SLVector3f pos, float red = 1.0f, float green = 1.0f, float blue = 1.0f)
        {
            position = pos;
            color = Color.FromArgb((int)(red * 255.0f), (int)(green * 255.0f), (int)(blue * 255.0f));
        }
    }
}
