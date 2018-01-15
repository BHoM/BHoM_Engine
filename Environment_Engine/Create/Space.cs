using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Elements;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        public static Space Space(string Name, string Number, IEnumerable<BuildingElement> BuildingElements)
        {
            return new Space
            {
                Name = Name,
                Number = Number,
                BuildingElements = BuildingElements.ToList()
            };
        }

        public static Space Space(string Name, string Number, Point Location, Storey Storey)
        {
            return new Space
            {
                Name = Name,
                Number = Number,
                Location = Location
            };
        }
    }
}
