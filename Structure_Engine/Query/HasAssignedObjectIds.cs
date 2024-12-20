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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Loads;
using BH.oM.Base;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if the objects assigned to the IElementLoad all have an identifier matching the AdapterIdName assigned.\n" +
                     "Loads without correctly assigned ids to the objects can generally not be pushed through to structural packages.")]
        [Input("load", "The load to check for id assignment.")]
        [Input("adapterIdType", "The the type of AdapterId fragment to check for. For example RobotId for Autodesk Robot.")]
        [Output("result", "Returns true if all objects assigned to the load has an id matching the AdapterIdName assigned.")]
        public static bool HasAssignedObjectIds<T>(this IElementLoad<T> load, Type adapterIdType) where T : IBHoMObject
        {
            if (load.IsNull())
                return false;

            if (load.Objects == null || load.Objects.Elements == null || load.Objects.Elements.Any(x => x == null))
            {
                Base.Compute.RecordError("At least one of the provided objects assigned to the load is null. Id assignment could not be evaluated.");
                return false;
            }

            if (adapterIdType == null)
            {
                Base.Compute.RecordError("The provided AdapterId type is null. Could not check object id assignment for the load.");
                return false;
            }

            if (!typeof(IAdapterId).IsAssignableFrom(adapterIdType))
            {
                Base.Compute.RecordError($"The `{adapterIdType.Name}` is not a valid `{typeof(IAdapterId).Name}`.");
                return false;
            }
            return load.Objects.Elements.All(x => x.Fragments.Contains(adapterIdType));
        }

        /***************************************************/
    }
}





