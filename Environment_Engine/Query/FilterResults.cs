/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Results.Mesh;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Environment.Results;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Filter a collection of Environment MeshResult objects to those which match the given object ID")]
        [Input("results", "A collection of Environment MeshResult objects to filter")]
        [Input("objectID", "The Object ID to filter by")]
        [Output("filteredResults", "The collection of Environment MeshResults which have the provided object ID")]
        public static List<MeshResult> FilterResultsByObjectID(this List<MeshResult> results, IComparable objectID)
        {
            return results.Where(x => x.ObjectId == objectID).ToList();
        }

        [Description("Filter a collection of Environment MeshResult objects to those which match the given result case")]
        [Input("results", "A collection of Environment MeshResult objects to filter")]
        [Input("resultCase", "The Result Case to filter by")]
        [Output("filteredResults", "The collection of Environment MeshResults which have the provided result case")]
        public static List<MeshResult> FilterResultsByResultCase(this List<MeshResult> results, IComparable resultCase)
        {
            return results.Where(x => x.ResultCase == resultCase).ToList();
        }

        [Description("Returns a collection of Environment Simulation Results by Profile Result Type")]
        [Input("results", "A collection of Simulation Results")]
        [Input("resultType", "The Profile Result Type filter")]
        [Output("simulationResults", "A collection of filtered simulation results")]
        public static List<SimulationResult> FilterResultsByResultType(this List<SimulationResult> results, ProfileResultType resultType)
        {
            List<SimulationResult> resultList = new List<SimulationResult>();

            foreach (SimulationResult sr in results)
            {
                List<ProfileResult> pResults = sr.SimulationResults.Where(x => x.Type == resultType).ToList();
                if (pResults.Count > 0)
                {
                    resultList.Add(new SimulationResult() {Name = sr.Name, SimulationResultType = sr.SimulationResultType, SimulationResults = pResults,});
                }
            }

            return resultList;
        }

        [Description("Returns a collection of Environment Simulation Results that match both Simulation Result Type and Profile Result Unit and Profile Result Type")]
        [Input("results", "A collection of Simulation Results")]
        [Input("simulationType", "The Simulation Result Type filter")]
        [Input("unit", "The Profile Result Unit filter")]
        [Input("resultType", "The Profile Result Type filter")]
        [Output("simulationResults", "A collection of filtered simulation results")]
        public static List<SimulationResult> FilterResultsByTypeUnitResultType(this List<SimulationResult> results, SimulationResultType simulationType, ProfileResultUnit unit, ProfileResultType resultType)
        {
            results = results.FilterResultsByTypeUnit(simulationType, unit);
            return results.FilterResultsByResultType(resultType);
        }

        [Description("Returns a collection of Environment Simulation Results by Simulation Result Type")]
        [Input("results", "A collection of Simulation Results")]
        [Input("type", "The Simulation Result Type filter")]
        [Output("simulationResults", "A collection of filtered simulation results")]
        public static List<SimulationResult> FilterResultsByType(this List<SimulationResult> results, SimulationResultType type)
        {
            return results.Where(x => x.SimulationResultType == type).ToList();
        }

        [Description("Returns a collection of Environment Simulation Results that match both Simulation Result Type and Profile Result Unit")]
        [Input("results", "A collection of Simulation Results")]
        [Input("simulationType", "The Simulation Result Type filter")]
        [Input("unit", "The Profile Result Unit filter")]
        [Output("simulationResults", "A collection of filtered simulation results")]
        public static List<SimulationResult> FilterResultsByTypeUnit(this List<SimulationResult> results, SimulationResultType simulationType, ProfileResultUnit unit)
        {
            results = results.FilterResultsByType(simulationType);
            return results.FilterResultsByUnit(unit);
        }

        [Description("Returns a collection of Environment Simulation Results by Profile Result Unit")]
        [Input("results", "A collection of Simulation Results")]
        [Input("unit", "The Profile Result Unit filter")]
        [Output("simulationResults", "A collection of filtered simulation results")]
        public static List<SimulationResult> FilterResultsByUnit(this List<SimulationResult> results, ProfileResultUnit unit)
        {
            List<SimulationResult> resultList = new List<SimulationResult>();

            foreach (SimulationResult sr in results)
            {
                List<ProfileResult> pResults = sr.SimulationResults.Where(x => x.Unit == unit).ToList();
                if (pResults.Count > 0)
                {
                    resultList.Add(new SimulationResult {Name = sr.Name, SimulationResultType = sr.SimulationResultType, SimulationResults = pResults,});
                }
            }

            return resultList;
        }
    }
}





