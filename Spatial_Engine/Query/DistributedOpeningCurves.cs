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
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Quantities.Attributes;
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

        [Description("Distributes a series of openings along a centreline from the start point.")]
        [Input("opening", "CellularOpening to distribute along curve.")]
        [Input("centreline", "Centreline curve to distribute the openings along.")]
        [Input("normal", "Normal direction of the element the openings belong to.")]
        [Input("tolerance", "Tolerance used for checking how many openings that can be fitted along the centreline.", typeof(Length))]
        [Output("openingCurve", "The distributed opening curves.")]
        public static List<ICurve> DistributedOpeningCurves(this ICellularOpening opening, Line centreline, Vector normal, double tolerance = Tolerance.Distance)
        {
            if (opening == null || centreline == null || normal == null)
            {
                Base.Compute.RecordError("Unable to distribute curves due to null inputs.");
                return null;
            }

            ICurve openingCurve = opening.IOpeningCurve();

            Vector tan = centreline.Direction(tolerance);

            Cartesian coordinateSystem = Engine.Geometry.Create.CartesianCoordinateSystem(centreline.Start, tan, normal);
            TransformMatrix orient = Engine.Geometry.Create.OrientationMatrixGlobalToLocal(coordinateSystem);

            ICurve orientedOpening = BH.Engine.Geometry.Modify.ITransform(openingCurve, orient);

            double length = centreline.Length();
            double distributionLength = length - opening.WidthWebPost;  //Will require one extra endpost on the end compared to spacing distribution
            int count = (int)Math.Floor((distributionLength + tolerance) / opening.Spacing);    //Number of openings that can be fitted
            distributionLength = count * opening.Spacing;   //The length along the element where openings can be fitted
            double endAdditional = (length - distributionLength) / 2;   //Leftover on each side
            double currCentre = endAdditional + opening.Spacing / 2;

            List<ICurve> openingCurves = new List<ICurve>();
            for (int i = 0; i < count; i++)
            {
                openingCurves.Add(orientedOpening.ITranslate(tan * currCentre));
                currCentre += opening.Spacing;
            }

            return openingCurves;
        }

        /***************************************************/
    }
}

