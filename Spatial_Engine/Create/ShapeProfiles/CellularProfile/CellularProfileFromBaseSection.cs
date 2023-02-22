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
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Spatial.ShapeProfiles.CellularOpenings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static CellularProfile CellularProfileFromBaseSection(ISectionProfile baseProfile, ICellularOpening opening)
        {
            double openingCutHeight = opening.IHeight();

            double webHeight = baseProfile.Height - 2 * (baseProfile.FlangeThickness + baseProfile.RootRadius);

            if (webHeight <= openingCutHeight)
            {
                Base.Compute.RecordError("Height of web needs to be larger or equal to the height of the opening.");
            }

            double topHeight = (baseProfile.Height - openingCutHeight / 2) / 2;

            TSectionProfile openingProfile = Create.TSectionProfile(topHeight, baseProfile.Width, baseProfile.WebThickness, baseProfile.FlangeThickness, baseProfile.RootRadius, baseProfile.ToeRadius);

            double totalHeight = openingCutHeight / 2 + opening.SpacerHeight + baseProfile.Height;

            ISectionProfile solidProfile = Create.ISectionProfile(totalHeight, baseProfile.Width, baseProfile.WebThickness, baseProfile.FlangeThickness, baseProfile.RootRadius, baseProfile.ToeRadius);


            BoundingBox boundsOpeningProfile = Geometry.Query.Bounds(openingProfile.Edges.Select(x => BH.Engine.Geometry.Query.IBounds(x)).ToList());

            double move = totalHeight / 2 - boundsOpeningProfile.Max.Y;
            Vector translationVector = new Vector { Y = move };
            List<ICurve> edges = openingProfile.Edges.Select(x => Engine.Geometry.Modify.ITranslate(x, translationVector)).ToList();

            Plane mirrorPlane = Plane.XZ;
            return new CellularProfile(baseProfile, openingProfile, solidProfile, opening, edges.Concat(edges.Select(x => x.IMirror(mirrorPlane))));

        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double IHeight(this ICellularOpening opening)
        {
            return Height(opening as dynamic);
        }

        /***************************************************/

        private static double Height(this HexagonalOpening opening)
        {
            return opening.Height;
        }

        /***************************************************/

        private static double Height(this CircularOpening opening)
        {
            return opening.Diameter;
        }

        /***************************************************/

        private static double Height(this SinusoidalOpening opening)
        {
            return opening.Height;
        }

        /***************************************************/
    }
}
