using BH.oM.Geometry;
using BH.oM.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.SVG
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string SVGObjectToString(this SVGObject svgObject)
        {
            string geometryString = "<g __Style__>" + Environment.NewLine;

            geometryString.Replace("__Style__", SVGStyleToString(svgObject.Style));

            for (int i = 0; i < svgObject.Geometry.Count; i++)
            {
                geometryString += Convert.IToSVG(svgObject.Geometry[i]) + Environment.NewLine;
            }
            geometryString += "</g>" + Environment.NewLine;

            return geometryString;
        }

        //public static string ToSVGString(this List<SVGObject> svgObject)
        //{
        //    string geometryString = "<g __Style__>" + System.Environment.NewLine;

        //    geometryString.Replace("__Style__", BH.Engine.SVG.Create.ToSVGString(svgObject.Style));

        //    for (int i = 0; i < svgObject.Geometry.Count; i++)
        //    {
        //        geometryString += Convert.IToSVG(svgObject.Geometry[i]);
        //    }
        //    geometryString += "</g>";

        //    return geometryString;
        //}

    }
}
