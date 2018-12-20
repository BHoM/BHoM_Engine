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
using BH.oM.Structure.Properties.Surface;
using BH.oM.Structure.Properties.Section;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISurfaceProperty ApplyModifiers(this ISurfaceProperty prop, double f11 =1, double f12=1, double f22=1, double m11=1, double m12=1, double m22=1, double v13=1, double v23=1, double mass=1, double weight=1)
        {
            ISurfaceProperty clone = prop.GetShallowClone() as ISurfaceProperty;

            double[] modifiers = new double[] { f11, f12, f22, m11, m12, m22, v13, v23, mass, weight };

            clone.CustomData["Modifiers"] = modifiers;

            return clone;
        }

        /***************************************************/

        public static ISectionProperty ApplyModifiers(this ISectionProperty prop, double area = 1, double iy = 1, double iz = 1, double j = 1, double asy = 1, double asz = 1)
        {
            ISectionProperty clone = prop.GetShallowClone() as ISectionProperty;

            double[] modifiers = new double[] { area, iy, iz, j, asy, asz };

            clone.CustomData["Modifiers"] = modifiers;

            return clone;
        }

        /***************************************************/
    }
}
