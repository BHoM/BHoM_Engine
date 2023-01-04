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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Tries to find an IAdapterId on the object.")]
        [Input("o", "The object to search for an IAdapterId.")]
        [Output("identifier", "First plausible identifier present on the object.")]
        public static Type FindIdentifier(this IBHoMObject o)
        {
            if (o == null)
            {
                Base.Compute.RecordError("Provided object is null. Cannot extract identifier.");
                return null;
            }
            Type adapterIdType = o.Fragments.FirstOrDefault(fr => fr is IAdapterId)?.GetType();
            if (adapterIdType == null)
            {
                return null;
            }
            else
            {
                Base.Compute.RecordNote($"Auto-generated Identifier as {adapterIdType.Name}.");
                return adapterIdType;
            }
        }

        /***************************************************/

        [Description("Tries to find a AdapterIdType on the object if no input is provided.")]
        [Output("adapterIdType", "First plausible identifier present on the object or provided.")]
        public static Type FindIdentifier(this IBHoMObject o, Type adapterIdType)
        {
            if (adapterIdType == null)
            {
                Type identifier = o.FindIdentifier();
                if(identifier == null)
                    Base.Compute.RecordError("No Identifier found.");
                return identifier;
            }
            else if (!typeof(IAdapterId).IsAssignableFrom(adapterIdType))
            {
                Base.Compute.RecordError("The provided adapterIdType need to be a type of IAdapterId.");
                return null;
            }
            return adapterIdType;
        }

        /***************************************************/

    }
}



