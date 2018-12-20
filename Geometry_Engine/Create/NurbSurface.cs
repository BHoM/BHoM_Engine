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

using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [NotImplemented]
        public static NurbsSurface NurbsSurface(IEnumerable<Point> controlPoints, int degree = 3)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsSurface NurbsSurface(IEnumerable<Point> controlPoints, IEnumerable<double> weights, int degree = 3)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsSurface NurbsSurface(IEnumerable<Point> controlPoints, IEnumerable<double> weights, IEnumerable<double> uKnots, IEnumerable<double> vKnots)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [NotImplemented]
        public static NurbsSurface RandomNurbsSurface(int seed = -1, BoundingBox box = null, int minNbCPs = 4, int maxNbCPs = 20)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsSurface RandomNurbsSurface(Random rnd, BoundingBox box = null, int minNbCPs = 4, int maxNbCPs = 20)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
    }
}
