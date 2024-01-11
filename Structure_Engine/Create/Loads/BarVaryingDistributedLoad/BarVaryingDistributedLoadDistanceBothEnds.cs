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

using BH.oM.Geometry;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.Engine.Spatial;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousInputNames("startToStartDistance", "distanceFromA")]
        [PreviousInputNames("endToEndDistance", "distanceFromB")]
        [PreviousInputNames("forceAtStart", "forceA")]
        [PreviousInputNames("forceAtEnd", "forceB")]
        [PreviousInputNames("momentAtStart", "momentA")]
        [PreviousInputNames("momentAtEnd", "momentB")]
        [Description("Creates a varying distributed load to be applied to Bar elements.\n" +
                     "Creates the load measuring the distance from the start node to the start of the load and from the end node to the end of the load.\n" +
                     "Method will group Bars by their length, according to the tolerance, and return one load for each group.")]
        [InputFromProperty("loadcase")]
        [InputFromProperty("group", "Objects")]
        [Input("startToStartDistance", "Distance along each Bar from the StartNode to the start of the load.")]
        [Input("endToEndDistance", "Distance along each Bar from the EndNode to the end of the load.")]
        [InputFromProperty("relativePositions")]
        [InputFromProperty("forceAtStart")]
        [InputFromProperty("forceAtEnd")]
        [InputFromProperty("momentAtStart")]
        [InputFromProperty("momentAtEnd")]
        [InputFromProperty("axis")]
        [InputFromProperty("projected")]
        [Input("name", "The name of the created load.")]
        [Input("groupingTolerance", "The maximum difference in length between the Bars for the grouping.", typeof(Length))]
        [Output("barVarLoad", "The created BarVaryingDistributedLoads with bars grouped by length.")]
        public static List<BarVaryingDistributedLoad> BarVaryingDistributedLoadDistanceBothEnds(Loadcase loadcase, BHoMGroup<Bar> group, bool relativePositions, double startToStartDistance = 0, Vector forceAtStart = null, Vector momentAtStart = null, double endToEndDistance = 0, Vector forceAtEnd = null, Vector momentAtEnd = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "", double groupingTolerance = Tolerance.Distance)
        {
            if ((forceAtStart == null || forceAtEnd == null) && (momentAtStart == null || momentAtEnd == null))
            {
                Base.Compute.RecordError("BarVaryingDistributedLoad requires at least the force at start and end or the moment at start and end to be defined.");
                return null;
            }

            Dictionary<double, List<Bar>> barGroups = GroupBarsByLength(group.Elements, groupingTolerance);

            List<BarVaryingDistributedLoad> loads = new List<BarVaryingDistributedLoad>();

            foreach (KeyValuePair<double, List<Bar>> kvp in barGroups)
            {
                BHoMGroup<Bar> newGroup = new BHoMGroup<Bar> { Elements = kvp.Value, Name = group.Name };

                double endPosition;
                if (relativePositions)
                    endPosition = 1 - endToEndDistance;
                else
                    endPosition = kvp.Key - endToEndDistance;
                loads.Add(BarVaryingDistributedLoad(loadcase, newGroup, startToStartDistance, forceAtStart, momentAtStart, endPosition, forceAtEnd, momentAtEnd, relativePositions, axis, projected, name));
            }

            return loads.Where(x => x != null).ToList();

        }

        /***************************************************/

        [PreviousInputNames("startToStartDistance", "distFromA")]
        [PreviousInputNames("endToEndDistance", "distFromB")]
        [PreviousInputNames("forceAtStart", "forceA")]
        [PreviousInputNames("forceAtEnd", "forceB")]
        [PreviousInputNames("momentAtStart", "momentA")]
        [PreviousInputNames("momentAtEnd", "momentB")]
        [Description("Creates a varying distributed load to be applied to Bar elements.\n" +
                             "Creates the load measuring the distance from the start node to the start of the load and from the end node to the end of the load.\n" +
                             "Method will group Bars by their length, according to the tolerance, and return one load for each group.")]
        [InputFromProperty("loadcase")]
        [Input("objects", "The collection of Bars the load should be applied to.")]
        [Input("startToStartDistance", "Distance along each Bar from the StartNode to the start of the load.")]
        [Input("endToEndDistance", "Distance along each Bar from the EndNode to the end of the load.")]
        [InputFromProperty("relativePositions")]
        [InputFromProperty("forceAtStart")]
        [InputFromProperty("forceAtEnd")]
        [InputFromProperty("momentAtStart")]
        [InputFromProperty("momentAtEnd")]
        [InputFromProperty("axis")]
        [InputFromProperty("projected")]
        [Input("name", "The name of the created load.")]
        [Input("groupingTolerance", "The maximum difference in length between the Bars for the grouping.", typeof(Length))]
        [Output("barVarLoad", "The created BarVaryingDistributedLoads with bars grouped by length.")]
        public static List<BarVaryingDistributedLoad> BarVaryingDistributedLoadDistanceBothEnds(Loadcase loadcase, IEnumerable<Bar> objects, bool relativePositions, double startToStartDistance = 0, Vector forceAtStart = null, Vector momentAtStart = null, double endToEndDistance = 0, Vector forceAtEnd = null, Vector momentAtEnd = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "", double groupingTolerance = Tolerance.Distance)
        {
            BHoMGroup<Bar> group = new BHoMGroup<Bar>();
            if (objects == null)
                group = null;
            else
                group.Elements = objects.ToList();

            return BarVaryingDistributedLoadDistanceBothEnds(loadcase, group, relativePositions, startToStartDistance, forceAtStart, forceAtEnd, endToEndDistance, forceAtEnd, momentAtEnd, axis, projected, name, groupingTolerance);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Groups bars by length, within a tolerance.")]
        [Input("bars", "The bars to group.")]
        [Input("tolerance", "Acceptable difference in length for each group.")]
        [Output("barGroup", "The bars grouped, as a dictionary, with the key being the length and the value being the corresponding bars.")]
        private static Dictionary<double, List<Bar>> GroupBarsByLength(this IEnumerable<Bar> bars, double tolerance)
        {
            //Check that bars have valid geometry
            bars = bars.Where(x => x != null && x.Start != null && x.End != null && x.Start.Position != null && x.End.Position != null);

            Dictionary<double, List<Bar>> dict = new Dictionary<double, List<Bar>>();
            foreach (var group in bars.GroupBy(x => (int)Math.Round(x.Length() / tolerance)))
            {
                dict[group.Key * tolerance] = group.ToList();
            }
            return dict;
        }

        /***************************************************/

    }
}





