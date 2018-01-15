using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Geometry
{
    public class Line : BHoMGeometry, ICurve
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Point Start { get; set; } = new Point();

        public Point End { get; set; } = new Point();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public Line() { }

        /***************************************************/

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
