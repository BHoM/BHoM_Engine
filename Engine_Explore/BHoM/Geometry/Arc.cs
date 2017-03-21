using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Geometry
{
    public class Arc : ICurve
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Point Start { get; set; } = new Point();

        public Point End { get; set; } = new Point();

        public Point Centre { get; set; } = new Point();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public Arc() { }

        /***************************************************/

        public Arc(Point start, Point end, Point centre)
        {
            Start = start;
            End = end;
            Centre = centre;
        }

        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
