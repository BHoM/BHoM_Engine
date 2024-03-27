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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        [Description("Queries the control points of the geometrical representation of an IElement0D. Always returns the point location due to zero-dimensionality of an IElement0D.")]
        [Input("element0D", "The IElement0D with the geometry to get the control points from.")]
        [Output("cPoints", "The control points of the geometrical representation of an IElement0D.")]
        public static List<Point> ControlPoints(this IElement0D element0D)
        {
            return new List<Point> { element0D.IGeometry() };
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [PreviousInputNames("element1D", "bar,edges")]
        [Description("Queries the control points of the one dimensional representation of the IElement1D.")]
        [Input("element1D", "The IElement1D with the geometry to get the control points from.")]
        [Output("cPoints", "The control points of the defining geometry for the one dimensional representation.")]
        public static List<Point> ControlPoints(this IElement1D element1D)
        {
            return Engine.Geometry.Query.IControlPoints(element1D.IGeometry());
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [PreviousInputNames("element2D", "panel,opening")]
        [Description("Queries the control points of the element curve representation of the IElement2D.")]
        [Input("element2D", "The IElement2D with the element curves to get the control points from.")]
        [Input("externalOnly", "Controls if the control points from the internal elements are queried as well.")]
        [Output("cPoints", "The control points of all the defining geometry for the element curve representation.")]
        public static List<Point> ControlPoints(this IElement2D element2D, bool externalOnly = false)
        {
            List<Point> pts = Engine.Geometry.Query.ControlPoints(element2D.OutlineCurve());
            if (!externalOnly)
            {
                foreach (IElement2D e in element2D.IInternalElements2D())
                {
                    pts.AddRange(e.ControlPoints());
                }
            }
            return pts;
        }


        /******************************************/
        /****             IElement             ****/
        /******************************************/

        [Description("Queries the control points of the geometrical representation of an IElement.")]
        [Input("element", "The IElement with the geometry to get the control points from.")]
        [Output("cPoints", "The control points of the geometrical representation of an IElement.")]
        public static List<Point> IControlPoints(this IElement element)
        {
            return ControlPoints(element as dynamic);
        }

        /******************************************/
    }
}




