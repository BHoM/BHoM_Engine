using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Building Building(string name, double latitude, double longitude, double elevation, BH.oM.Geometry.Point location)
        {
            return new Building
            {
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Elevation = elevation,
                Location = location,
            };
        }

        public static Building Building(string name, double latitude, double longitude, double elevation)
        {
            return new Building
            {
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Elevation = elevation,
            };
        }

        public static Building Building(string name)
        {
            return new Building
            {
                Name = name,
            };
        }

        public static Building Building(double latitude, double longitude)
        {
            return new Building
            {
                Latitude = latitude,
                Longitude = longitude,
            };
        }

        public static Building Building(double elevation)
        {
            return new Building
            {
                Elevation = elevation,
            };
        }

        public static Building Building(BH.oM.Geometry.Point location)
        {
            return new Building
            {
                Location = location,
            };
        }

        public static Building Building(double elevation, double latitude, double longitude, BH.oM.Geometry.Point location)
        {
            return new Building
            {
                Elevation = elevation,
                Latitude = latitude,
                Longitude = longitude,
                Location = location,
            };
        }

        /***************************************************/
    }
}
