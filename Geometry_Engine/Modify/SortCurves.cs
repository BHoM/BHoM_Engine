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

using BH.Engine.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PolyCurve SortCurves(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            if (curve.Curves.Count < 2)
                return curve;

            List<ICurve> pending = curve.Curves.ToList();
            PolyCurve result = new PolyCurve { Curves = new List<ICurve> { pending[0] } };
            pending.RemoveAt(0);

            double sqTol = tolerance * tolerance;

            while (pending.Count > 0)
            {
                Point start1 = result.StartPoint();
                Point end1 = result.EndPoint();
                bool foundNext = false;

                for (int i = 0; i < pending.Count; i++)
                {
                    Point start2 = pending[i].IStartPoint();
                    Point end2 = pending[i].IEndPoint();
 
                    if (end1.SquareDistance(start2) < sqTol)
                    {
                        result.Curves.Add(pending[i]);
                        pending.RemoveAt(i);
                        foundNext = true;
                        break;
                    }
                    else if (end1.SquareDistance(end2) < sqTol)
                    {
                        result.Curves.Add(pending[i].IFlip());
                        pending.RemoveAt(i);
                        foundNext = true;
                        break;
                    }
                    else if (start1.SquareDistance(end2) < sqTol)
                    {
                        result.Curves.Insert(0, pending[i]);
                        pending.RemoveAt(i);
                        foundNext = true;
                        break;
                    }
                    else if (start1.SquareDistance(start2) < sqTol)
                    {
                        result.Curves.Insert(0, pending[i].IFlip());
                        pending.RemoveAt(i);
                        foundNext = true;
                        break;
                    }
                }

                if (!foundNext)
                    throw new Exception("PolyCurve with unconnected subcurves cannot have them sorted");
            }

            return result;
        }

        /***************************************************/
    }
}





