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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****         public Methods - Vectors          ****/
        /***************************************************/

        public static List<List<Plane>> ClusterCoplanar(this List<Plane> planes, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            List<List<Plane>> planeClusters = new List<List<Plane>>();
            foreach (Plane p in planes)
            {
                bool coplanar = false;
                foreach (List<Plane> pp in planeClusters)
                {
                    if (p.IsCoplanar(pp[0], distanceTolerance, angleTolerance))
                    {
                        pp.Add(p);
                        coplanar = true;
                        break;
                    }
                }

                if (!coplanar)
                    planeClusters.Add(new List<Plane> { p });
            }

            return planeClusters;
        }


        /***************************************************/
        /****          public Methods - Curves          ****/
        /***************************************************/

        public static List<List<Polyline>> ClusterCoplanar(this List<Polyline> curves, double distanceTolerance = Tolerance.Distance)
        {
            List<List<Polyline>> curveClusters = new List<List<Polyline>>();
            foreach (Polyline p in curves)
            {
                bool coplanar = false;
                foreach (List<Polyline> pp in curveClusters)
                {
                    if (p.IsCoplanar(pp[0], distanceTolerance))
                    {
                        pp.Add(p);
                        coplanar = true;
                        break;
                    }
                }

                if (!coplanar)
                    curveClusters.Add(new List<Polyline> { p });
            }

            return curveClusters;
        }

        /***************************************************/

        public static List<List<PolyCurve>> ClusterCoplanar(this List<PolyCurve> curves, double distanceTolerance = Tolerance.Distance)
        {
            List<List<PolyCurve>> curveClusters = new List<List<PolyCurve>>();
            foreach (PolyCurve p in curves)
            {
                bool coplanar = false;
                foreach (List<PolyCurve> pp in curveClusters)
                {
                    if (p.IsCoplanar(pp[0], distanceTolerance))
                    {
                        pp.Add(p);
                        coplanar = true;
                        break;
                    }
                }

                if (!coplanar)
                    curveClusters.Add(new List<PolyCurve> { p });
            }

            return curveClusters;
        }

        /***************************************************/

        public static List<List<ICurve>> IClusterCoplanar(this List<ICurve> curves, double distanceTolerance = Tolerance.Distance)
        {
            List<List<ICurve>> curveClusters = new List<List<ICurve>>();
            foreach (ICurve p in curves)
            {
                bool coplanar = false;
                foreach (List<ICurve> pp in curveClusters)
                {
                    if (p.IIsCoplanar(pp[0], distanceTolerance))
                    {
                        pp.Add(p);
                        coplanar = true;
                        break;
                    }
                }

                if (!coplanar)
                    curveClusters.Add(new List<ICurve> { p });
            }

            return curveClusters;
        }

        /***************************************************/
    }
}




