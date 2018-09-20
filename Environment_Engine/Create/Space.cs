using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Architecture.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Space Space(string name, string number)
        {
            return new Space
            {
                Name = name,
                Number = number,
            };
        }

        /***************************************************/

        public static Space Space(string number)
        {
            return new Space
            {
                Number = number,
            };
        }

        /***************************************************/

        public static Space Space(Point location)
        {
            return new Space
            {
                Location = location,
            };
        }

        /***************************************************/

        public static Space Space(string name, Point location)
        {
            return new Space
            {
                Name = name,
                Location = location,
            };
        }

        /***************************************************/

        public static Space Space(string name, string number, Point location)
        {
            return new Space
            {
                Name = name,
                Number = number,
                Location = location,
            };
        }
    }
}
