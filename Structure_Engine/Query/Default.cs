/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Physical.Materials;
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

        [Description("Extracts a default material from the material datasets for a specific material type.")]
        [Input("materialType","The material type to extract a default value for.")]
        [Output("material","The default material of the type provided.")]
        public static IMaterialFragment Default(this MaterialType materialType)
        {
            string libraryName = "Structure\\Materials";
            string matName = null;
            switch (materialType)
            {

                case oM.Structure.MaterialFragments.MaterialType.Steel:
                    matName = "S355";
                    break;
                case oM.Structure.MaterialFragments.MaterialType.Concrete:
                    matName = "C30/37";
                    break;
                case oM.Structure.MaterialFragments.MaterialType.Rebar:
                    matName = "B500B";
                    break;
                case oM.Structure.MaterialFragments.MaterialType.Aluminium:
                case oM.Structure.MaterialFragments.MaterialType.Timber:
                case oM.Structure.MaterialFragments.MaterialType.Cable:
                case oM.Structure.MaterialFragments.MaterialType.Tendon:
                case oM.Structure.MaterialFragments.MaterialType.Glass:
                default:
                    Base.Compute.RecordWarning("Could not find default material of type " + materialType);
                    return null;
            }

            if (matName != null)
                return Library.Query.Match(libraryName, matName, true, true) as IMaterialFragment;

            return null;
        }

    }
}






