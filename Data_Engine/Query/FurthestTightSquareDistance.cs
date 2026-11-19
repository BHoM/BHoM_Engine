/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Data.Collections;
using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the furthest possible distance between two vectors confined in respective tight DomainBoxes." +
                     "i.e. assume that the data in the DomainBoxes coincide with every side of the box and get a worst-case distance between their data.")]
        [Input("box1", "Box to evaluate furthest possible square distance from.")]
        [Input("box2", "Box to evaluate furthest possible square distance from.")]
        [Output("sqDist", "The furthest possible square distance between two vectors confined in the respective tight DomainBoxes.")]
        public static double FurthestTightSquareDistance(this DomainBox box1, DomainBox box2)
        {
            if (box1 == null || box2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the furthest square distance if either domain box is null.");
                return 0;
            }

            // Find in which axis the distance between the outer bounderies of the boxes is the least
            int index = 0;
            double min = double.MaxValue;
            for (int i = 0; i < box1.Domains.Length; i++)
            {
                double temp = ShortestEdgeDistance(box1.Domains[i], box2.Domains[i]);
                if (temp < min)
                {
                    min = temp;
                    index = i;
                }
            }

            double sq = min * min;

            for (int i = 0; i < box1.Domains.Length; i++)
            {
                if (i != index)
                {
                    double d = FurthestEdgeDistance(box1.Domains[i], box2.Domains[i]);
                    sq += d * d;
                }
            }

            return sq;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double ShortestEdgeDistance(Domain a, Domain b)
        {
            return Math.Min(
                Math.Min(Math.Abs(a.Min - b.Min), Math.Abs(a.Min - b.Max)),
                Math.Min(Math.Abs(a.Max - b.Min), Math.Abs(a.Max - b.Max)));
        }

        /***************************************************/

    }
}
