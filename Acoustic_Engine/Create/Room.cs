using BH.oM.Acoustic;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public static Room Room(List<Point> points, double area, double volume)
        {
            return new Room()
            {
                Area = area,
                Volume = volume,
                Samples = points.Select(x => Create.Receiver(x)).ToList()
            };
        }

        /***************************************************/

        public static Room Room(List<Receiver> receivers, double area, double volume)
        {
            return new Room()
            {
                Area = area,
                Volume = volume,
                Samples = receivers
            };
        }
    }
}
