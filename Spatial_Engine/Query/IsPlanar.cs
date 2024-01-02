/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        [Description("Checks whether all control points of an element lie in a single plane.")]
        [Input("element0D", "Element to evaluate.")]
        [Input("tolerance", "Tolerance used in numerical processing of planarity.")]
        [Output("planar", "True if all control points of an element lie in a single plane.")]
        public static bool IsPlanar(this IElement0D element0D, double tolerance = Tolerance.Distance)
        {
            return true;
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Checks whether all control points of an element lie in a single plane.")]
        [Input("element1D", "Element to evaluate.")]
        [Input("tolerance", "Tolerance used in numerical processing of planarity.")]
        [Output("planar", "True if all control points of an element lie in a single plane.")]
        public static bool IsPlanar(this IElement1D element1D, double tolerance = Tolerance.Distance)
        {
            return element1D.IGeometry().IIsPlanar(tolerance);
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Checks whether all control points of an element lie in a single plane.")]
        [Input("element2D", "Element to evaluate.")]
        [Input("externalOnly", "If true, only the external outline of an element is evaluated.")]
        [Input("tolerance", "Tolerance used in numerical processing of planarity.")]
        [Output("planar", "True if all control points of an element lie in a single plane.")]
        public static bool IsPlanar(this IElement2D element2D, bool externalOnly = false, double tolerance = Tolerance.Distance)
        {
            return element2D.ControlPoints(externalOnly).IsCoplanar(tolerance);
        }


        /******************************************/
        /****            Interfaces            ****/
        /******************************************/

        [Description("Checks whether all control points of an element lie in a single plane.")]
        [Input("element", "Element to evaluate.")]
        [Input("externalOnly", "If true, only the external outline of an element is evaluated.")]
        [Input("tolerance", "Tolerance used in numerical processing of planarity.")]
        [Output("planar", "True if all control points of an element lie in a single plane.")]
        public static bool IIsPlanar(this IElement element, bool externalOnly = false, double tolerance = Tolerance.Distance)
        {
            return IsPlanar(element as dynamic, externalOnly, tolerance);
        }


        /******************************************/
        /****         Private methods          ****/
        /******************************************/

        private static bool IsPlanar(this IElement0D element0D, bool externalOnly = false, double tolerance = Tolerance.Distance)
        {
            return element0D.IsPlanar(tolerance);
        }

        /******************************************/

        private static bool IsPlanar(this IElement1D element1D, bool externalOnly = false, double tolerance = Tolerance.Distance)
        {
            return element1D.IsPlanar(tolerance);
        }

        /******************************************/
    }
}



