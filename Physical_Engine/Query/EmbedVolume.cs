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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Physical.Reinforcement;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.Engine.Spatial;
using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.Engine.Base;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines whether the Piles are within the volume of the Piles that are embed in to the pile cap.")]
        [Input("padFoundation", "The PadFoundation that defines the extents for the Piles to be located.")]
        [Input("piles", "The Piles to calculate the embeded volume.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("embedVolume", "True if all Piles are contained within the boundary and depth of the PileCap. False if one or more Piles are not.")]
        public static double EmbedVolume(this PadFoundation padFoundation, List<Pile> piles, double tolerance = Tolerance.MicroDistance)
        {
            if (padFoundation.IsNull())
                return 0;
            else if (piles.IsNullOrEmpty())
                return 0;
            else if (piles.Any(x => x.IsNull()))
                return 0;

            if (!IsWithinPileCap(padFoundation, piles, tolerance))
            {
                Base.Compute.RecordError("The list of Piles must be within the extents and depth of the pile cap,.");
                return 0;
            }

            // Get the top of the piles
            List<Point> tops = piles.Select(p => p.Location.IControlPoints().OrderBy(pt => pt.Z).Last()).ToList();

            Plane bottom = padFoundation.Location.FitPlane();
            if (bottom.Normal.Z < 0)
                bottom.Normal *= -1;

            bottom.Origin -= bottom.Normal * padFoundation.Construction.IThickness();

            double embedVolume = 0;

            //For each pile, get the intersection with the bottom surface, get the line extending in to the pile cap and calculate the solid volume
            for (int i = 0; i < tops.Count; i++)
            {
                Pile embedPile = piles[i].ShallowClone();
                Point top = tops[i];
                Point projected = top.Project(bottom);
                if (top.Z > projected.Z)
                {
                    embedPile = (Pile)embedPile.SetGeometry(new Line { Start = projected, End = top });
                    embedVolume += embedPile.SolidVolume();
                }
            }

            return embedVolume;
        }

        /***************************************************/

    }
}

