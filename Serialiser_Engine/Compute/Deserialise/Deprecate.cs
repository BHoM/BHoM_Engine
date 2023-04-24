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

using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/
        public static object DeserialiseDeprecate(this BsonDocument doc, ref bool failed)
        {
            BsonDocument newDoc = Versioning.Convert.ToNewVersion(doc);

            if (newDoc == null || newDoc.Equals(doc))
            {
                /*string newType = newDoc["_t"].AsString;
                if (newType.Contains("[["))
                {
                    // If the object is of a generic type, try one last time by ensuring that the assembly name is provided
                    try
                    {
                        string baseType = newType.Split(new char[] { '[' }).First();
                        List<Type> matchingTypes = BH.Engine.Base.Create.AllTypes(baseType, true);
                        if (matchingTypes.Count == 1)
                        {
                            string assemblyName = matchingTypes.First().Assembly?.FullName;
                            newDoc["_t"] += "," + assemblyName;
                            return Convert.FromBson(newDoc);
                        }
                    }
                    catch { }
                }*/
                failed = true;
                Engine.Base.Compute.RecordWarning("The type " + doc["_t"] + " is unknown -> data returned as custom objects.");
                return DeserialiseCustomObject(doc, ref failed, null, "", true);
            }
            else
            {
                failed = false;
                return IDeserialise(newDoc, ref failed, "", true);
            }
        }

        /*******************************************/
    }
}
