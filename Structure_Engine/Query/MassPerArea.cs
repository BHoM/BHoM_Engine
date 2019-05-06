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

using BH.oM.Structure.SurfaceProperties;
using BH.oM.Reflection.Attributes;
using System;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double MassPerArea(this ConstantThickness constantThickness)
        {
            return constantThickness.Thickness * constantThickness.Material.Density;
        }

        /***************************************************/

        [NotImplemented]
        public static double MassPerArea(this Ribbed ribbedProperty)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static double MassPerArea(this Waffle ribbedProperty)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static double MassPerArea(this LoadingPanelProperty loadingPanelProperty)
        {
            return 0;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IMassPerArea(this ISurfaceProperty property)
        {
            return MassPerArea(property as dynamic);
        }

        /***************************************************/
    }
}
