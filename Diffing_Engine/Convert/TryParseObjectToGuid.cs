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
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using BH.oM.Base.Attributes;
using System.ComponentModel;


namespace BH.Engine.Diffing
{
    public static partial class Convert
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        public static bool TryParseObjectToGuid(this object obj, out Guid guid)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot parse a null object to a GUID.");
                guid = Guid.Empty;
                return false;
            }

            guid = Guid.Empty;

            // If it's a Guid, extract the Id from there. 
            // Note: at this stage we cannot check if the provided GUID belongs to an existing Stream or not.
            if (!Guid.TryParse(obj.ToString(), out guid))
            {
                // Check if it's a StreamPointer, and extract the StreamId from there.
                var sP = obj as StreamPointer;
                if (sP != null)
                {
                    guid = sP.StreamID;
                    return true;
                }

                // Check if it's a Revision, and extract the StreamId from there.
                var rev = obj as Revision;
                if (rev != null)
                {
                    guid = rev.StreamID;
                    return true;
                }

                return false;
            }

            return true;

        }

        ///***************************************************/

    }
}





