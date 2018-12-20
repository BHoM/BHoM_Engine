/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Results;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BarForce> MinForces(IEnumerable<BarForce> forces)
        {

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

        public static List<BarForce> MinEnvelopeByCase(IEnumerable<BarForce> forces)
        {
            return forces.GroupByCase().Select(x => x.MinEnvelope(false, true)).ToList();
        }

        /***************************************************/

        public static List<BarForce> MinEnvelopeByObject(IEnumerable<BarForce> forces)
        {
            return forces.GroupByObjectId().Select(x => x.MinEnvelope(true, false)).ToList();
        }

        /***************************************************/

        public static BarForce MinEnvelope(this IEnumerable<BarForce> forces, bool idFromFirst = false, bool caseFromFirst = false)
        {
            return new BarForce()
            {
                ObjectId = idFromFirst ? forces.First().ObjectId : "",
                ResultCase = caseFromFirst ? forces.First().ResultCase : "",
                FX = forces.Min(x => x.FX),
                FY = forces.Min(x => x.FY),
                FZ = forces.Min(x => x.FZ),
                MX = forces.Min(x => x.MX),
                MY = forces.Min(x => x.MY),
                MZ = forces.Min(x => x.MZ),
            };
        }

        /***************************************************/

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
