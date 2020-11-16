/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using BH.Engine.Spatial;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a varying distributed load to be applied to Bar elements.")]
        [InputFromProperty("loadcase")]
        [InputFromProperty("group", "Objects")]
        [InputFromProperty("startPosition")]
        [InputFromProperty("endPosition")]
        [InputFromProperty("forceAtStart")]
        [InputFromProperty("forceAtEnd")]
        [InputFromProperty("momentAtStart")]
        [InputFromProperty("momentAtEnd")]
        [InputFromProperty("relativePositions")]
        [InputFromProperty("axis")]
        [InputFromProperty("projected")]
        [Input("name", "The name of the created load.")]
        [Output("barVarLoad", "The created BarVaryingDistributedLoad.")]
        public static BarVaryingDistributedLoad BarVaryingDistributedLoad(Loadcase loadcase, BHoMGroup<Bar> group, double startPosition = 0, Vector forceAtStart = null, Vector momentAtStart = null, double endPosition = 1, Vector forceAtEnd = null, Vector momentAtEnd = null, bool relativePositions = true, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            if ((forceAtStart == null || forceAtEnd == null) && (momentAtStart == null || momentAtEnd == null))
            {
                Engine.Reflection.Compute.RecordError("Bar varying load requires either the force at start and end and/or the moment at start and end to be defined");
                return null;
            }

            if (relativePositions && (startPosition < 0 || startPosition > 1 || endPosition < 0 || endPosition > 1))
            {
                Reflection.Compute.RecordError("Positions must exist between 0 and 1 (inclusive) for relative positions set to true.");
                return null;
            }

            return new BarVaryingDistributedLoad
            {
                Loadcase = loadcase,
                Objects = group,
                StartPosition = startPosition,
                EndPosition = endPosition,
                ForceAtStart = forceAtStart ?? new Vector(),
                ForceAtEnd = forceAtEnd ?? new Vector(),
                MomentAtStart = momentAtStart ?? new Vector(),
                MomentAtEnd = momentAtEnd ?? new Vector(),
                Projected = projected,
                RelativePositions = relativePositions,
                Axis = axis,
                Name = name
            };

        }

        /***************************************************/

        [Description("Creates a varying distributed load to be applied to Bar elements.")]
        [InputFromProperty("loadcase")]
        [Input("objects", "The collection of Bars the load should be applied to.")]
        [InputFromProperty("startPosition")]
        [InputFromProperty("endPosition")]
        [InputFromProperty("forceAtStart")]
        [InputFromProperty("forceAtEnd")]
        [InputFromProperty("momentAtStart")]
        [InputFromProperty("momentAtEnd")]
        [InputFromProperty("relativePositions")]
        [InputFromProperty("axis")]
        [InputFromProperty("projected")]
        [Input("name", "The name of the created load.")]
        [Output("barVarLoad", "The created BarVaryingDistributedLoad.")]
        public static BarVaryingDistributedLoad BarVaryingDistributedLoad(Loadcase loadcase, IEnumerable<Bar> objects, double startPosition = 0, Vector forceAtStart = null, Vector momentAtStart = null, double endPosition = 1, Vector forceAtEnd = null, Vector momentAtEnd = null, bool relativePositions = true, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return BarVaryingDistributedLoad(loadcase, new BHoMGroup<Bar> { Elements = objects.ToList() }, startPosition, forceAtStart, forceAtEnd, endPosition, forceAtEnd, momentAtEnd, relativePositions, axis, projected, name);

        }

        /***************************************************/

        [PreviousVersion("4.0", "BH.Engine.Structure.Create.BarVaryingDistributedLoad(BH.oM.Structure.Loads.Loadcase, BH.oM.Base.BHoMGroup<BH.oM.Structure.Elements.Bar>, System.Double, BH.oM.Geometry.Vector, BH.oM.Geometry.Vector, System.Double, BH.oM.Geometry.Vector, BH.oM.Geometry.Vector, BH.oM.Structure.Loads.LoadAxis, System.Boolean, System.String)")]
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
        [Output("barVarLoad", "The created BarVaryingDistributedLoads with bars grouped by length.")]
        public static List<BarVaryingDistributedLoad> BarVaryingDistributedLoadDistanceBothEnds(Loadcase loadcase, BHoMGroup<Bar> group, bool relativePositions, double startToStartDistance = 0, Vector forceAtStart = null, Vector momentAtStart = null, double endToEndDistance = 0, Vector forceAtEnd = null, Vector momentAtEnd = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "", double groupingTolerance = Tolerance.Distance)
        {
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

        [PreviousVersion("4.0", "BH.Engine.Structure.Create.BarVaryingDistributedLoad(BH.oM.Structure.Loads.Loadcase, System.Collections.Generic.IEnumerable<BH.oM.Structure.Elements.Bar>, System.Double, BH.oM.Geometry.Vector, BH.oM.Geometry.Vector, System.Double, BH.oM.Geometry.Vector, BH.oM.Geometry.Vector, BH.oM.Structure.Loads.LoadAxis, System.Boolean, System.String)")]
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
        [Output("barVarLoad", "The created BarVaryingDistributedLoads with bars grouped by length.")]
        public static List<BarVaryingDistributedLoad> BarVaryingDistributedLoadDistanceBothEnds(Loadcase loadcase, IEnumerable<Bar> objects, bool relativePositions, double startToStartDistance = 0, Vector forceAtStart = null, Vector momentAtStart = null, double endToEndDistance = 0, Vector forceAtEnd = null, Vector momentAtEnd = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "", double groupingTolerance = Tolerance.Distance)
        {
            return BarVaryingDistributedLoadDistanceBothEnds(loadcase, new BHoMGroup<Bar>() { Elements = objects.ToList() }, relativePositions, startToStartDistance, forceAtStart, forceAtEnd, endToEndDistance, forceAtEnd, momentAtEnd, axis, projected, name, groupingTolerance);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Groups bars by length, within a tolerance.")]
        [Input("bars", "The bars to group.")]
        [Input("tolerance", "Acceptable difference in length for each group")]
        [Output("barGroup", "The bars grouped, as a dictionary, with the key being the length and the value being the corresponding bars.")]
        private static Dictionary<double, List<Bar>> GroupBarsByLength(this IEnumerable<Bar> bars, double tolerance)
        {
            //Check that bars have valid geometry
            bars = bars.Where(x => x != null && x.StartNode != null && x.EndNode != null && x.StartNode.Position != null && x.EndNode.Position != null);

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

