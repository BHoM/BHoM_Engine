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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.MEP.Fragments;
using BH.oM.Geometry;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Returns an MEP Geometry Fragment which can be applied to an MEP object to provide geometric information")]
        [Input("geometry", "A BHoM Geometry object, any geometry which implements the IGeometry interface, such as Curves, Surfaces, etc. - default null")]
        [Output("geometryFragment", "An MEP Geometry Fragment")]
        public static GeometryFragment GeometryFragment(IGeometry geometry = null)
        {
            return new GeometryFragment
            {
                Geometry = geometry,
            };
        }
        /***************************************************/
    }
}

