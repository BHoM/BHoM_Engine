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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Queries identifier of an object to be used to identify the object when processing verification results.")]
        [Input("obj", "Object to query the identifier from.")]
        [Output("identifier", "Identifier extracted from the input object.")]
        public static IComparable IIdentifier(this object obj)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not query identifier from a null object.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(Identifier), out result))
            {
                BH.Engine.Base.Compute.RecordError($"Identifier query failed because object of type {obj.GetType().Name} is currently not supported.");
                return null;
            }

            return (IComparable)result;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Queries identifier (BHoM_Guid) of IBHoMObject to be used to identify the object when processing verification results.")]
        [Input("obj", "IBHoMObject to query the identifier from.")]
        [Output("identifier", "Identifier (BHoM_Guid) extracted from the input IBHoMObject.")]
        public static IComparable Identifier(this IBHoMObject obj)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not query identifier from a null object.");
                return null;
            }

            return obj.BHoM_Guid;
        }

        /***************************************************/
    }
}
