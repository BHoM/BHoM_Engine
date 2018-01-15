﻿using BH.oM.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Create 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PanelFreeForm PanelFreeForm(ISurface surface)
        {
            return new PanelFreeForm { Surface = surface };
        }

        /***************************************************/
    }
}
