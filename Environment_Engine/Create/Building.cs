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
        public static Building Building(string Name, double Latitude, double Longitude, double Elevation)
        {
            return new Building()
            {
                Name = Name,
                Latitude = Latitude,
                Longitude = Longitude,
                Elevation = Elevation,
                Location = new oM.Geometry.Point()
                {
                    X = 0,
                    Y = 0,
                    Z = 0,
                }
            };
        }
    }
}
