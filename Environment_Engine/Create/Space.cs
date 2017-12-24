﻿using BH.oM.Environmental.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Space Space(List<Polyline> contours)
        {
            return new Space { Contours = contours };
        }

        /***************************************************/
    }
}
