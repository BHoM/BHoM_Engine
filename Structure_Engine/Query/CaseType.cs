/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Loads;
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

        [ToBeRemoved("3.1", "Should not need this method as it was historically used to easy type switch via enum instead of type switching. Method not in use in any toolkit, and its usecase should be solved by the 'as dynamic' dispatching.")]
        [Description("Gets the CaseType for the Loadcase. This will always return the type Simple for a Loadcase.")]
        public static CaseType CaseType(this Loadcase loadcase)
        {
            return oM.Structure.Loads.CaseType.Simple;
        }

        /***************************************************/

        [ToBeRemoved("3.1", "Should not need this method as it was historically used to easy type switch via enum instead of type switching. Method not in use in any toolkit, and its usecase should be solved by the 'as dynamic' dispatching.")]
        [Description("Gets the CaseType for the LoadCombination. This will always return the type Combination for a LoadCombination.")]
        public static CaseType CaseType(this LoadCombination loadCombination)
        {
            return oM.Structure.Loads.CaseType.Combination;
        }

        /***************************************************/
    }
}




