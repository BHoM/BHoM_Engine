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
using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MaterialType MaterialType(this Steel materialFragment)
        {
            return oM.Structure.MaterialFragments.MaterialType.Steel;
        }

        /***************************************************/

        public static MaterialType MaterialType(this Concrete materialFragment)
        {
            return oM.Structure.MaterialFragments.MaterialType.Concrete;
        }

        /***************************************************/

        public static MaterialType MaterialType(this Timber materialFragment)
        {
            return oM.Structure.MaterialFragments.MaterialType.Timber;
        }

        /***************************************************/

        public static MaterialType MaterialType(this Aluminium materialFragment)
        {
            return oM.Structure.MaterialFragments.MaterialType.Aluminium;
        }

        /***************************************************/

        public static MaterialType MaterialType(this GenericIsotropicMaterial materialFragment)
        {
            return oM.Structure.MaterialFragments.MaterialType.Undefined;
        }

        /***************************************************/

        public static MaterialType MaterialType(this GenericOrthotropicMaterial materialFragment)
        {
            return oM.Structure.MaterialFragments.MaterialType.Undefined;
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        public static MaterialType IMaterialType(this IMaterialFragment materialFragment)
        {
            return MaterialType(materialFragment as dynamic);
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
