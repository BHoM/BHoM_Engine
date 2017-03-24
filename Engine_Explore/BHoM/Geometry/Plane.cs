using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Geometry
{
    public class Plane : BHoMGeometry
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Point Origin { get; set; } = new Point();

        public Vector Normal { get; set; } = new Vector(0,0,1);


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public Plane() { }

        /***************************************************/

        public Plane(Point origin)
        {
            Origin = origin;
        }

        /***************************************************/

        public Plane(Point origin, Vector normal)
        {
            Origin = origin;
            Normal = normal;
        }

        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
