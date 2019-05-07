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

        [Description("Poissons Ratio of the material. Can be either a double or a vector depending on the material type")]
        [Output("ν", "Poissons Ratio of the material. Double for Isotropic material, Vector for orthotropic")]
        public static object PoissonsRatio(this Material material)
        {
            return material.StructuralMaterialFragment()?.IPoissonsRatio();
        }

        /***************************************************/

        [Description("Poissons Ratio for a isotropic material")]
        [Output("ν", "Poissons Ratio of the material. 0 if material does not contain any structural material fragments")]
        public static double PoissonsRatioIsotropic(this Material material)
        {
            IIsotropic fragment = material.IsotropicMaterialFragment();
            return fragment != null ? fragment.PoissonsRatio : double.NaN;
        }

        /***************************************************/

        [Description("Poissons Ratio for a orthotropic material")]
        [Output("ν", "Poissons Ratio of the material. null if material does not contain any structural material fragments")]
        public static Vector PoissonsRatioOrthotropic(this Material material)
        {
            return material.OrthotropicMaterialFragment()?.PoissonsRatio;
        }


        /***************************************************/
        /**** Public Methods  - Interace                ****/
        /***************************************************/

        [Description("Gets Poissons Ratio from the material fragment")]
        public static object IPoissonsRatio(this IStructuralMaterial materialFragment)
        {
            return _PoissonsRatio(materialFragment as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Private extension method added to alow for dynamic casting")]
        private static double _PoissonsRatio(this IIsotropic materialFragment)
        {
            return materialFragment.PoissonsRatio;
        }

        /***************************************************/

        [Description("Private extension method added to alow for dynamic casting")]
        private static Vector _PoissonsRatio(this IOrthotropic materialFragment)
        {
            return materialFragment.PoissonsRatio;
        }

        /***************************************************/
    }
}
