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
using BH.oM.Structure.Results;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BarForce> AbsoluteMaxForces(this IEnumerable<BarForce> forces)
        {

            List<BarForce> maxForces = new List<BarForce>();
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.FX)));
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.FY)));
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.FZ)));
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.MX)));
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.MY)));
            maxForces.Add(forces.MaxBy(x => Math.Abs(x.MZ)));

            return maxForces;
        }

        /***************************************************/

        public static List<BarForce> AbsoluteMaxEnvelopeByCase(this IEnumerable<BarForce> forces)
        {
            return forces.GroupByCase().Select(x => x.AbsoluteMaxEnvelope(false, true)).ToList();
        }

        /***************************************************/

        public static List<BarForce> AbsoluteMaxEnvelopeByObject(this IEnumerable<BarForce> forces)
        {
            return forces.GroupByObjectId().Select(x => x.AbsoluteMaxEnvelope(true, false)).ToList();
        }

        /***************************************************/

        public static BarForce AbsoluteMaxEnvelope(this IEnumerable<BarForce> forces, bool idFromFirst = false, bool caseFromFirst = false)
        {
            return new BarForce(
               idFromFirst ? forces.First().ObjectId : "",
               caseFromFirst ? forces.First().ResultCase : "",
               0,
               0,
               0,
               0,
               forces.AbsoluteMax(x => x.FX),
               forces.AbsoluteMax(x => x.FY),
               forces.AbsoluteMax(x => x.FZ),
               forces.AbsoluteMax(x => x.MX),
               forces.AbsoluteMax(x => x.MY),
               forces.AbsoluteMax(x => x.MZ)
               );
        }

        /***************************************************/

        public static double AbsoluteMax<T>(this IEnumerable<T> source, Func<T, double> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            bool first = true;
            double absMax = 0;
            double signedAbsMax = 0;
            foreach (var item in source)
            {
                if (first)
                {
                    signedAbsMax = selector(item);
                    absMax = Math.Abs(signedAbsMax);

                    first = false;
                }
                else
                {
                    double currentVal = selector(item);
                    double currentAbs = Math.Abs(currentVal);
                    if (currentAbs > absMax)
                    {
                        absMax = currentAbs;
                        signedAbsMax = currentVal;
                    }
                }
            }
            if (first) throw new InvalidOperationException("Sequence is empty.");
            return signedAbsMax;
        }

        /***************************************************/
    }
}






