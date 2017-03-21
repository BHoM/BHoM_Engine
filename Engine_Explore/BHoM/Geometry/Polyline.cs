using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Geometry
{
    public class Polyline : ICurve
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<Point> ControlPoints { get; set; } = new List<Point>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public Polyline() { }

        /***************************************************/

        public Polyline(IEnumerable<Point> points)
        {
            ControlPoints = points.ToList();
        }

        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
