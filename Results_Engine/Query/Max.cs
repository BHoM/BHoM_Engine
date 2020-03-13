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

        public static List<BarForce> MaxForces(this IEnumerable<BarForce> forces)
        {

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

        public static List<BarForce> MaxEnvelopeByCase(this IEnumerable<BarForce> forces)
        {
            return forces.GroupByCase().Select(x => x.MaxEnvelope(false, true)).ToList();
        }

        /***************************************************/

        public static List<BarForce> MaxEnvelopeByObject(this IEnumerable<BarForce> forces)
        {
            return forces.GroupByObjectId().Select(x => x.MaxEnvelope(true, false)).ToList();
        }

        /***************************************************/

        public static BarForce MaxEnvelope(this IEnumerable<BarForce> forces, bool idFromFirst = false, bool caseFromFirst = false)
        {
            return new BarForce()
            {
                ObjectId = idFromFirst ? forces.First().ObjectId : "",
                ResultCase = caseFromFirst ? forces.First().ResultCase : "",
                FX = forces.Max(x => x.FX),
                FY = forces.Max(x => x.FY),
                FZ = forces.Max(x => x.FZ),
                MX = forces.Max(x => x.MX),
                MY = forces.Max(x => x.MY),
                MZ = forces.Max(x => x.MZ),
            };
        }

        /***************************************************/

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

