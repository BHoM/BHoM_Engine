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
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Methods - Curves                  ****/
        /***************************************************/

        [NotImplemented]
        public static NurbsCurve ToNurbsCurve(this Arc arc)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve ToNurbsCurve(this Circle circle)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve ToNurbsCurve(this Ellipse ellipse)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve ToNurbsCurve(this Line line)
        {
            throw new NotImplementedException();

        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve ToNurbsCurve(this NurbsCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve ToNurbsCurve(this PolyCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve ToNurbsCurve(this Polyline curve)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Interaces                ****/
        /***************************************************/

        [NotImplemented]
        public static NurbsCurve IToNurbsCurve(this ICurve geometry)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
    }
}
