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

using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Spatial.ShapeProfiles.CellularOpenings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Distributes a series of openings along a centreline. Method will fit in as many openings along the curve as it can, starting from the start of the curve.")]
        [Input("opening", "CellularOpening do ditribute along curve.")]
        [Input("centreline", "Centreline curve to distribute the openings along.")]
        [Input("normal", "Normal direction of the element the openings belong to. The openings will be in a plane spanned by the tangent of the centreline and the normal vector.")]
        [Output("openingCurve", "The distributed opening curves.")]
        public static List<ICurve> DistributedOpeningCurves(this ICellularOpening opening, Line centreline, Vector normal)
        {
            if (opening == null || centreline == null || normal == null)
            {
                Base.Compute.RecordError("Unable to distribute curves due to null inputs.");
                return null;
            }

            ICurve openingCurve = opening.IOpeningCurve();

            Vector tan = centreline.Direction();

            Cartesian coordinateSystem = Engine.Geometry.Create.CartesianCoordinateSystem(centreline.Start, tan, normal);
            TransformMatrix orient = Engine.Geometry.Create.OrientationMatrixGlobalToLocal(coordinateSystem);

            ICurve orientedOpening = BH.Engine.Geometry.Modify.ITransform(openingCurve, orient);


            double endLength = opening.Spacing / 2 + opening.WidthWebPost;
            double max = centreline.Length() - endLength;
            double currCentre = endLength;

            List<ICurve> openingCurves = new List<ICurve>();
            while (currCentre < max)
            {
                openingCurves.Add(orientedOpening.ITranslate(tan * currCentre));
                currCentre += opening.Spacing;
            }
            return openingCurves;
        }

        /***************************************************/
    }
}
