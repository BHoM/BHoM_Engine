/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/
        
        [Description("Queries the centre of weight for the homogeneous geometrical representation of an IElement0D. Always returns the point location due to zero-dimensionality of an IElement0D.")]
        [Input("element0D", "The IElement0D with the geometry to get the centre of weight of.")]
        [Output("centroid", "The Point at the centre of weight for the homogeneous geometrical representation of the IElement0D.")]
        public static Point Centroid(this IElement0D element0D, double tolerance = Tolerance.Distance)
        {
            return element0D.IGeometry();
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [NotImplemented]
        [Description("Queries the centre of weight for a IElement1Ds ICurve representation.")]
        [Input("element1D", "The IElement1D with the geometry to get the centre of weight of. The IElement1D will be considered homogeneous.")]
        [Output("centroid", "The Point at the centre of weight for the homogeneous geometrical representation of the IElement1D.")]
        public static Point Centroid(this IElement1D element1D)
        {
            //TODO: find a proper centre of weight of a curve (not an average of control points)
            throw new NotImplementedException();
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Queries the centre of area for a IElement2Ds surface representation. For an IElement2D with homogeneous material and thickness this will also be the centre of weight.")]
        [Input("element2D", "The IElement2D with the geometry to get the centre of area of.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance")]
        [Output("centroid", "The Point at the centre of area for the homogeneous geometrical representation of the IElement2D.")]
        public static Point Centroid(this IElement2D element2D, double tolerance = Tolerance.Distance)
        {
            return Engine.Geometry.Query.Centroid(new List<ICurve> { element2D.OutlineCurve() }, element2D.InternalOutlineCurves(), tolerance);
        }


        /******************************************/
        /****   Public Methods - Interfaces    ****/
        /******************************************/
        
        [Description("Queries the centre of weight for the homogeneous geometrical representation of an IElement.")]
        [Input("element", "The IElement with the geometry to get the centre of mass of.")]
        [Input("tolerance", "Distance tolerance used in geometry processing, default set to BH.oM.Geometry.Tolerance.Distance")]
        [Output("centroid", "The Point at the centre of weight for the homogeneous geometrical representation of an IElement.")]
        public static Point ICentroid(this IElement element, double tolerance = Tolerance.Distance)
        {
            return Centroid(element as dynamic, tolerance);
        }

        /******************************************/
    }
}



