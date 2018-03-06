using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Building Building(string name, double latitude, double longitude, double elevation)
        {
            return new Building
            {
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Elevation = elevation,
                Location = new oM.Geometry.Point()
            };
        }

        /***************************************************/
    }
}
