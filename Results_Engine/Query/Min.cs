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
        [Description("Returns a list of BarForces, one for each component, that contain the minimum value for each components and its concurrent forces.")]
        [Input("forces", "The BarForce results to be considered.")]
        [Output("minForces", "A list of BarForces, one for each component of the BarForce, containing the minimum force and its concurrent forces.")]
        public static List<BarForce> MinForces(this IEnumerable<BarForce> forces)
        {
            if (forces.IsNullOrEmpty() || forces.Any(x => x.IsNull()))
                return null;

            List<BarForce> minForces = new List<BarForce>();
            minForces.Add(forces.MinBy(x => x.FX));
            minForces.Add(forces.MinBy(x => x.FY));
            minForces.Add(forces.MinBy(x => x.FZ));
            minForces.Add(forces.MinBy(x => x.MX));
            minForces.Add(forces.MinBy(x => x.MY));
            minForces.Add(forces.MinBy(x => x.MZ));

            return minForces;
        }

        /***************************************************/
        [Description("Returns a list of BarRequiredArea results, one for each component, that contain the minimum value for each required area and its concurrent required areas.")]
        [Input("results", "The BarRequiredArea results to be considered.")]
        [Output("minResults", "A list of BarRequiredAreas, one for each component of the BarRequiredArea, containing the minimum required area and its concurrent required areas.")]
        public static List<BarRequiredArea> MinBarRequiredArea(this IEnumerable<BarRequiredArea> results)
        {
            if (results.IsNullOrEmpty() || results.Any(x => x.IsNull()))
                return null;

            List<BarRequiredArea> minResults = new List<BarRequiredArea>();
            minResults.Add(results.MinBy(x => x.Top));
            minResults.Add(results.MinBy(x => x.Bottom));
            minResults.Add(results.MinBy(x => x.Perimeter));
            minResults.Add(results.MinBy(x => x.Shear));
            minResults.Add(results.MinBy(x => x.Torsion));

            return minResults;
        }

        /***************************************************/
        [Description("Returns a list of MeshRequiredAreas results, one for each component, that contain the minimum value for each required area and its concurrent required areas.")]
        [Input("results", "The MeshRequiredArea results to be considered.")]
        [Output("minResults", "A list of MeshRequiredAreas, one for each component of the MeshRequiredAreas, containing the minimum required area and its concurrent required areas.")]
        public static List<MeshRequiredArea> MinMeshRequiredArea(this IEnumerable<MeshRequiredArea> results)
        {
            if (results.IsNullOrEmpty() || results.Any(x => x.IsNull()))
                return null;

            List<MeshRequiredArea> minResults = new List<MeshRequiredArea>();
            minResults.Add(results.MinBy(x => x.TopPrimary));
            minResults.Add(results.MinBy(x => x.TopSecondary));
            minResults.Add(results.MinBy(x => x.BottomPrimary));
            minResults.Add(results.MinBy(x => x.BottomSecondary));
            minResults.Add(results.MinBy(x => x.Shear));
            minResults.Add(results.MinBy(x => x.Torsion));

            return minResults;
        }

        /***************************************************/
        [Description("Groups the BarForces by case and finds the minimum envelope returning a single enveloped BarForce for each case. The resulting BarForce will not contain concurrent forces.")]
        [Input("forces", "The BarForces to be considered.")]
        [Output("minResults", "A list of BarForces, one for each case, enveloped to produce the minimum forces for that case.")]
        public static List<BarForce> MinEnvelopeByCase(this IEnumerable<BarForce> forces)
        {
            return forces.IsNullOrEmpty() || forces.Any(x => x.IsNull()) ? null : forces.GroupByCase().Select(x => x.MinEnvelope(false, true)).ToList();
        }

        /***************************************************/
        [Description("Groups the BarForces by id and finds the minimum envelope returning a single enveloped BarForce for each id. The resulting BarForce will not contain concurrent forces.")]
        [Input("forces", "The BarForces to be considered.")]
        [Output("minResults", "A list of BarForces, one for each id, enveloped to produce the minimum forces for that id.")]
        public static List<BarForce> MinEnvelopeByObject(this IEnumerable<BarForce> forces)
        {
            return forces.IsNullOrEmpty() || forces.Any(x => x.IsNull()) ? null : forces.GroupByObjectId().Select(x => x.MinEnvelope(true, false)).ToList();
        }

        /***************************************************/
        [Description("Determines the minimum force in each component of the BarForce and returns a single BarForce object with the enveloped forces.")]
        [Input("forces", "The BarForces to be considered.")]
        [Input("idFromFirst", "True is the id from the first BarForce should be assigned to output BarForce. Otherwise an id will not be assigned.")]
        [Input("caseFromFirst", "True if the case from the first BarForce should be assigned to output Barforce. Otherwise a case will not be assigned.")]
        [Output("minEnvelope", "A BarForce object containing the minimum enveloped forces in each of its components.")]
        public static BarForce MinEnvelope(this IEnumerable<BarForce> forces, bool idFromFirst = false, bool caseFromFirst = false)
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
               forces.Min(x => x.FX),
               forces.Min(x => x.FY),
               forces.Min(x => x.FZ),
               forces.Min(x => x.MX),
               forces.Min(x => x.MY),
               forces.Min(x => x.MZ)
               );
        }
        /***************************************************/
        [Description("Determines the minimum required in each component of the BarRequiredArea and returns a single BarRequiredArea object with the enveloped required areas.")]
        [Input("results", "The BarRequiredArea results to be considered.")]
        [Input("idFromFirst", "True is the id from the first BarRequiredArea should be assigned to output BarRequiredArea. Otherwise an id will not be assigned.")]
        [Input("caseFromFirst", "True if the case from the first BarRequiredArea should be assigned to output BarRequiredArea. Otherwise a case will not be assigned.")]
        [Input("materialFromFirst", "True if the material from the first BarRequiredArea should be assigned to output BarRequiredArea. Otherwise a material will not be assigned.")]
        [Output("maxEnvelope", "A BarRequiredArea object containing the enveloped required areas in each of its components.")]
        public static BarRequiredArea MinEnvelope(this IEnumerable<BarRequiredArea> results, bool idFromFirst = false, bool caseFromFirst = false, bool materialFromFirst = false)
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
                results.Min(x => x.Top),
                results.Min(x => x.Bottom),
                results.Min(x => x.Perimeter),
                results.Min(x => x.Shear),
                results.Min(x => x.Torsion),
                materialFromFirst ? results.First().MaterialName : ""
                );
        }

        /***************************************************/
        [Description("Determines the minimum required in each component of the MeshRequiredArea and returns a single MeshRequiredArea object with the enveloped required areas.")]
        [Input("results", "The MeshRequiredArea results to be considered.")]
        [Input("idFromFirst", "True is the id from the first MeshRequiredArea should be assigned to output MeshRequiredArea. Otherwise an id will not be assigned.")]
        [Input("caseFromFirst", "True if the case from the first MeshRequiredArea should be assigned to output MeshRequiredArea. Otherwise a case will not be assigned.")]
        [Input("materialFromFirst", "True if the material from the first MeshRequiredArea should be assigned to output MeshRequiredArea. Otherwise a material will not be assigned.")]
        [Output("maxEnvelope", "A MeshRequiredArea object containing the enveloped required areas in each of its components.")]
        public static MeshRequiredArea MinEnvelope(this IEnumerable<MeshRequiredArea> results, bool idFromFirst = false, bool caseFromFirst = false, bool materialFromFirst = false)
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
                MeshResultLayer.Minimum,
                0,
                MeshResultSmoothingType.None,
                null,
                results.Min(x => x.TopPrimary),
                results.Min(x => x.TopSecondary),
                results.Min(x => x.BottomPrimary),
                results.Min(x => x.BottomSecondary),
                results.Min(x => x.Shear),
                results.Min(x => x.Torsion),
                materialFromFirst ? results.First().MaterialName : ""
                );
        }


        /***************************************************/
        [Description("Determines the minimum value of a property specified by the selector and returns the host object.")]
        [Input("source", "The objects to be considered.")]
        [Input("selector", "The property used to determine the minimum value.")]
        [Output("minObj", "The source object containing the minimum value of the property specified.")]
        //TODO: Move these generic methods somewhere else
        public static T MinBy<T, U>(this IEnumerable<T> source, Func<T, U> selector) where U : IComparable<U>
        {
            if (source == null) throw new ArgumentNullException("source");
            bool first = true;
            T minObj = default(T);
            U minKey = default(U);
            foreach (var item in source)
            {
                if (first)
                {
                    minObj = item;
                    minKey = selector(minObj);
                    first = false;
                }
                else
                {
                    U currentKey = selector(item);
                    if (currentKey.CompareTo(minKey) < 0)
                    {
                        minKey = currentKey;
                        minObj = item;
                    }
                }
            }
            if (first) throw new InvalidOperationException("Sequence is empty.");
            return minObj;
        }

        /***************************************************/
    }
}




