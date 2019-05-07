/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Physical.Materials;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Youngs modulus of the material. Can be either a double or a vector depending on the material type")]
        [Output("E", "Youngs modulus of the material. Double for Isotropic material, Vector for orthotropic")]
        public static object YoungsModulus(this Material material)
        {
            return material.StructuralMaterialFragment()?.IYoungsModulus();
        }

        /***************************************************/

        [Description("Youngs modulus for a isotropic material")]
        [Output("E", "Youngs modulus of the material. NaN if material does not contain any structural material fragments")]
        public static double YoungsModulusIsotropic(this Material material)
        {
            IIsotropic fragment = material.IsotropicMaterialFragment();
            return fragment != null ? fragment.YoungsModulus : double.NaN;
        }

        /***************************************************/

        [Description("Youngs modulus for a orthotropic material")]
        [Output("E", "Youngs modulus of the material. null if material does not contain any structural material fragments")]
        public static Vector YoungsModulusOrthotropic(this Material material)
        {
            return material.OrthotropicMaterialFragment()?.YoungsModulus;
        }


        /***************************************************/
        /**** Public Methods  - Interace                ****/
        /***************************************************/

        [Description("Gets youngs modulus from the material fragment")]
        public static object IYoungsModulus(this IStructuralMaterial materialFragment)
        {
            return _YoungsModulus(materialFragment as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Private extension method added to alow for dynamic casting")]
        private static double _YoungsModulus(this IIsotropic materialFragment)
        {
            return materialFragment.YoungsModulus;
        }

        /***************************************************/

        [Description("Private extension method added to alow for dynamic casting")]
        private static Vector _YoungsModulus(this IOrthotropic materialFragment)
        {
            return materialFragment.YoungsModulus;
        }

        /***************************************************/
    }
}
