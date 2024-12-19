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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Base;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Results;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Returns a list of BarForces, one for each component, that contain the maximum value for each components and its concurrent forces.")]
        [Input("forces", "The BarForce results to be considered.")]
        [Output("maxForces", "A list of BarForces, one for each component of the BarForce, containing the maximum force and its concurrent forces.")]
        public static List<BarForce> MaxForces(this IEnumerable<BarForce> forces)
        {
            if (forces.IsNullOrEmpty() || forces.Any(x => x.IsNull()))
                return null;

            List<BarForce> maxForces = new List<BarForce>();
            maxForces.Add(forces.MaxBy(x => x.FX));
            maxForces.Add(forces.MaxBy(x => x.FY));
            maxForces.Add(forces.MaxBy(x => x.FZ));
            maxForces.Add(forces.MaxBy(x => x.MX));
            maxForces.Add(forces.MaxBy(x => x.MY));
            maxForces.Add(forces.MaxBy(x => x.MZ));

            return maxForces;
        }

        /***************************************************/
        [Description("Returns a list of BarRequiredArea results, one for each component, that contain the maximum value for each required area and its concurrent required areas.")]
        [Input("results", "The BarRequiredArea results to be considered.")]
        [Output("maxResults", "A list of BarRequiredAreas, one for each component of the BarRequiredArea, containing the maximum required area and its concurrent required areas.")]
        public static List<BarRequiredArea> MaxBarRequiredArea(this IEnumerable<BarRequiredArea> results)
        {
            if (results.IsNullOrEmpty() || results.Any(x => x.IsNull()))
                return null;

            List<BarRequiredArea> maxResults = new List<BarRequiredArea>();
            maxResults.Add(results.MaxBy(x => x.Top));
            maxResults.Add(results.MaxBy(x => x.Bottom));
            maxResults.Add(results.MaxBy(x => x.Perimeter));
            maxResults.Add(results.MaxBy(x => x.Shear));
            maxResults.Add(results.MaxBy(x => x.Torsion));

            return maxResults;
        }

        /***************************************************/
        [Description("Returns a list of MeshRequiredAreas results, one for each component, that contain the maximum value for each required area and its concurrent required areas.")]
        [Input("results", "The MeshRequiredArea results to be considered.")]
        [Output("maxResults", "A list of MeshRequiredAreas, one for each component of the MeshRequiredAreas, containing the maximum required area and its concurrent required areas.")]
        public static List<MeshRequiredArea> MaxMeshRequiredArea(this IEnumerable<MeshRequiredArea> results)
        {
            if (results.IsNullOrEmpty() || results.Any(x => x.IsNull()))
                return null;

            List<MeshRequiredArea> maxResults = new List<MeshRequiredArea>();
            maxResults.Add(results.MaxBy(x => x.TopPrimary));
            maxResults.Add(results.MaxBy(x => x.TopSecondary));
            maxResults.Add(results.MaxBy(x => x.BottomPrimary));
            maxResults.Add(results.MaxBy(x => x.BottomSecondary));
            maxResults.Add(results.MaxBy(x => x.Shear));
            maxResults.Add(results.MaxBy(x => x.Torsion));

            return maxResults;
        }

        /***************************************************/
        [Description("Groups the BarForces by case and finds the maximum envelope returning a single enveloped BarForce for each case. The resulting BarForce will not contain concurrent forces.")]
        [Input("forces", "The BarForces to be considered.")]
        [Output("maxResults", "A list of BarForces, one for each case, enveloped to produce the maximum forces for that case.")]
        public static List<BarForce> MaxEnvelopeByCase(this IEnumerable<BarForce> forces)
        {
            return forces.IsNullOrEmpty() || forces.Any(x => x.IsNull()) ? null : forces.GroupByCase().Select(x => x.MaxEnvelope(false, true)).ToList();
        }

        /***************************************************/
        [Description("Groups the BarForces by id and finds the maximum envelope returning a single enveloped BarForce for each id. The resulting BarForce will not contain concurrent forces.")]
        [Input("forces", "The BarForces to be considered.")]
        [Output("maxResults", "A list of BarForces, one for each id, enveloped to produce the maximum forces for that id.")]
        public static List<BarForce> MaxEnvelopeByObject(this IEnumerable<BarForce> forces)
        {
            return forces.IsNullOrEmpty() || forces.Any(x => x.IsNull()) ? null : forces.GroupByObjectId().Select(x => x.MaxEnvelope(true, false)).ToList();
        }

        /***************************************************/
        [Description("Determines the maximum force in each component of the BarForce and returns a single BarForce object with the enveloped forces.")]
        [Input("forces", "The BarForces to be considered.")]
        [Input("idFromFirst", "True is the id from the first BarForce should be assigned to output BarForce. Otherwise an id will not be assigned.")]
        [Input("caseFromFirst", "True if the case from the first BarForce should be assigned to output Barforce. Otherwise a case will not be assigned.")]
        [Output("maxEnvelope", "A BarForce object containing the maximum enveloped forces in each of its components.")]
        public static BarForce MaxEnvelope(this IEnumerable<BarForce> forces, bool idFromFirst = false, bool caseFromFirst = false)
        {
            if (forces.IsNullOrEmpty() || forces.Any(x => x.IsNull()))
                return null;

            return new BarForce(
                idFromFirst ? forces.First().ObjectId : "",
                caseFromFirst ? forces.First().ResultCase : "",
                0,
                0,
                0,
                0,
                forces.Max(x => x.FX),
                forces.Max(x => x.FY),
                forces.Max(x => x.FZ),
                forces.Max(x => x.MX),
                forces.Max(x => x.MY),
                forces.Max(x => x.MZ)
                );
        }

        /***************************************************/
        [Description("Determines the maximum required in each component of the BarRequiredArea and returns a single BarRequiredArea object with the enveloped required areas.")]
        [Input("results", "The BarRequiredArea results to be considered.")]
        [Input("idFromFirst", "True is the id from the first BarRequiredArea should be assigned to output BarRequiredArea. Otherwise an id will not be assigned.")]
        [Input("caseFromFirst", "True if the case from the first BarRequiredArea should be assigned to output BarRequiredArea. Otherwise a case will not be assigned.")]
        [Input("materialFromFirst", "True if the material from the first BarRequiredArea should be assigned to output BarRequiredArea. Otherwise a material will not be assigned.")]
        [Output("maxEnvelope", "A BarRequiredArea object containing the enveloped required areas in each of its components.")]
        public static BarRequiredArea MaxEnvelope(this IEnumerable<BarRequiredArea> results, bool idFromFirst = false, bool caseFromFirst = false, bool materialFromFirst = false)
        {
            if (results.IsNullOrEmpty() || results.Any(x => x.IsNull()))
                return null;

            return new BarRequiredArea(
                idFromFirst ? results.First().ObjectId : "",
                caseFromFirst ? results.First().ResultCase : "",
                0,
                0,
                0,
                0,
                results.Max(x => x.Top),
                results.Max(x => x.Bottom),
                results.Max(x => x.Perimeter),
                results.Max(x => x.Shear),
                results.Max(x => x.Torsion),
                materialFromFirst ? results.First().MaterialName : ""
                );
        }

        /***************************************************/
        [Description("Determines the maximum required in each component of the MeshRequiredArea and returns a single MeshRequiredArea object with the enveloped required areas.")]
        [Input("results", "The MeshRequiredArea results to be considered.")]
        [Input("idFromFirst", "True is the id from the first MeshRequiredArea should be assigned to output MeshRequiredArea. Otherwise an id will not be assigned.")]
        [Input("caseFromFirst", "True if the case from the first MeshRequiredArea should be assigned to output MeshRequiredArea. Otherwise a case will not be assigned.")]
        [Input("materialFromFirst", "True if the material from the first MeshRequiredArea should be assigned to output MeshRequiredArea. Otherwise a material will not be assigned.")]
        [Output("maxEnvelope", "A MeshRequiredArea object containing the enveloped required areas in each of its components.")]
        public static MeshRequiredArea MaxEnvelope(this IEnumerable<MeshRequiredArea> results, bool idFromFirst = false, bool caseFromFirst = false, bool materialFromFirst = false)
        {
            if (results.IsNullOrEmpty() || results.Any(x => x.IsNull()))
                return null;

            return new MeshRequiredArea(
                idFromFirst ? results.First().ObjectId : "",
                0,
                0,
                caseFromFirst ? results.First().ResultCase : "",
                0,
                0,
                MeshResultLayer.Maximum,
                0,
                MeshResultSmoothingType.None,
                null,
                results.Max(x => x.TopPrimary),
                results.Max(x => x.TopSecondary),
                results.Max(x => x.BottomPrimary),
                results.Max(x => x.BottomSecondary),
                results.Max(x => x.Shear),
                results.Max(x => x.Torsion),
                materialFromFirst ? results.First().MaterialName : ""
                );
        }

        /***************************************************/
        [Description("Determines the maximum value of a property specified by the selector and returns the host object.")]
        [Input("source", "The objects to be considered.")]
        [Input("selector", "The property used to determine the maximum value.")]
        [Output("maxObj", "The source object containing the maximum value of the property specified.")]
        //TODO: Move these generic methods somewhere else
        public static T MaxBy<T, U>(this IEnumerable<T> source, Func<T, U> selector) where U : IComparable<U>
        {
            if (source == null) throw new ArgumentNullException("source");
            bool first = true;
            T maxObj = default(T);
            U maxKey = default(U);
            foreach (var item in source)
            {
                if (first)
                {
                    maxObj = item;
                    maxKey = selector(maxObj);
                    first = false;
                }
                else
                {
                    U currentKey = selector(item);
                    if (currentKey.CompareTo(maxKey) > 0)
                    {
                        maxKey = currentKey;
                        maxObj = item;
                    }
                }
            }
            if (first) throw new InvalidOperationException("Sequence is empty.");

            return maxObj;
        }

        /***************************************************/
    }
}






