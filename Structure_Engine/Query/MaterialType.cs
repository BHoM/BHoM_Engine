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


using BH.oM.Structure.MaterialFragments;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the material type from the MaterialFragment. For a Steel material this will always return type Steel.")]
        [Input("materialFragment", "The structural MaterialFragment to get the type from.")]
        [Output("type", "The material type of the provided material.")]
        public static MaterialType MaterialType(this Steel materialFragment)
        {
            return materialFragment.IsNull() ? oM.Structure.MaterialFragments.MaterialType.Undefined : oM.Structure.MaterialFragments.MaterialType.Steel;
        }

        /***************************************************/

        [Description("Gets the material type from the MaterialFragment. For a Concrete material this will always return type Concrete.")]
        [Input("materialFragment", "The structural MaterialFragment to get the type from.")]
        [Output("type", "The material type of the provided material.")]
        public static MaterialType MaterialType(this Concrete materialFragment)
        {
            return materialFragment.IsNull() ? oM.Structure.MaterialFragments.MaterialType.Undefined : oM.Structure.MaterialFragments.MaterialType.Concrete;
        }

        /***************************************************/

        [Description("Gets the material type from the MaterialFragment. For a Timber material this will always return type Timber.")]
        [Input("materialFragment", "The structural MaterialFragment to get the type from.")]
        [Output("type", "The material type of the provided material.")]
        public static MaterialType MaterialType(this Timber materialFragment)
        {
            return materialFragment.IsNull() ? oM.Structure.MaterialFragments.MaterialType.Undefined : oM.Structure.MaterialFragments.MaterialType.Timber;
        }

        /***************************************************/

        [Description("Gets the material type from the MaterialFragment. For a Aluminium material this will always return type Aluminium.")]
        [Input("materialFragment", "The structural MaterialFragment to get the type from.")]
        [Output("type", "The material type of the provided material.")]
        public static MaterialType MaterialType(this Aluminium materialFragment)
        {
            return materialFragment.IsNull() ? oM.Structure.MaterialFragments.MaterialType.Undefined : oM.Structure.MaterialFragments.MaterialType.Aluminium;
        }

        /***************************************************/

        [Description("Gets the material type from the MaterialFragment. For a GenericIsotropicMaterial material this will always return type Undefined.")]
        [Input("materialFragment", "The structural MaterialFragment to get the type from.")]
        [Output("type", "The material type of the provided material.")]
        public static MaterialType MaterialType(this GenericIsotropicMaterial materialFragment)
        {
            return materialFragment.IsNull() ? oM.Structure.MaterialFragments.MaterialType.Undefined : oM.Structure.MaterialFragments.MaterialType.Undefined;
        }

        /***************************************************/

        [Description("Gets the material type from the MaterialFragment. For a GenericOrthotropicMaterial material this will always return type Undefined.")]
        [Input("materialFragment", "The structural MaterialFragment to get the type from.")]
        [Output("type", "The material type of the provided material.")]
        public static MaterialType MaterialType(this GenericOrthotropicMaterial materialFragment)
        {
            return materialFragment.IsNull() ? oM.Structure.MaterialFragments.MaterialType.Undefined : oM.Structure.MaterialFragments.MaterialType.Undefined;
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets the material type from the MaterialFragment.")]
        [Input("materialFragment", "The structural MaterialFragment to get the type from.")]
        [Output("type", "The material type of the provided material.")]
        public static MaterialType IMaterialType(this IMaterialFragment materialFragment)
        {
            return materialFragment.IsNull() ? oM.Structure.MaterialFragments.MaterialType.Undefined : MaterialType(materialFragment as dynamic);
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static MaterialType MaterialType(this IMaterialFragment materialFragment)
        {
            return oM.Structure.MaterialFragments.MaterialType.Undefined;
        }

        /***************************************************/
    }
}





