using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        public static Color ColourFromObject(object colourObject)
        {
            Color color = new Color();
            if (colourObject is Color)
            {
                color = (Color)colourObject;
            }
            if (colourObject is string)
            {
                string colourString = System.Convert.ToString(colourObject);

                if (colourString.Contains("Name"))
                {
                    //"{Name=ffe6484d, ARGB=(255, 230, 72, 77)}"
                    string[] parts = colourString.Split(',');
                    color = System.Drawing.Color.FromName(parts[0].Replace("Name=", ""));
                }
                if (colourString.Contains("ARGB"))
                {
                    //"{Name=ffe6484d, ARGB=(255, 230, 72, 77)}"
                    string[] parts = colourString.Split(',');
                    int a = 0;
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    int.TryParse(parts[1].Replace("ARGB=(", ""), out a);
                    int.TryParse(parts[2], out r);
                    int.TryParse(parts[3], out g);
                    int.TryParse(parts[4].Replace("", ""), out b);
                    color = System.Drawing.Color.FromArgb(a, r, g, b);
                }

                if (colourString.Contains("#"))
                {
                    color = ColorTranslator.FromHtml(colourString);
                }
            }

            return color;
        }
    }
}
