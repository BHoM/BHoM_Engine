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

using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.Engine.Versioning;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static IDynamicPropertyProvider DeserialiseIDynamicPropertyProvider(this BsonValue bson, Type type, string version, bool isUpgraded)
        {
            if (bson.IsBsonNull)
                return null;
            else if (!bson.IsBsonDocument)
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise an IDynamicPropertyProvider object and received " + bson.ToString() + " instead.");
                return null;
            }

            BsonDocument doc = bson.AsBsonDocument;
            IDynamicPropertyProvider result;

            try
            {
                result = Activator.CreateInstance(type) as IDynamicPropertyProvider;
            }
            catch (Exception e)
            {
                if (!isUpgraded && TryUpgrade(doc, version, out object upgrade))
                {
                    return upgrade as IDynamicPropertyProvider;
                }
                else
                {
                    BH.Engine.Base.Compute.RecordError(e, $"Cannot deserialise a {nameof(IDynamicPropertyProvider)} that is also IImmutable");
                    return DeserialiseDeprecatedCustomObject(doc, version, false);
                }
            }
            
            try
            {
                foreach (BsonElement item in doc)
                {
                    if (item.Name.StartsWith("_"))
                        continue;

                    object value = item.Value.IDeserialise(version, isUpgraded);

                    object success;
                    bool found = Base.Compute.TryRunExtensionMethod(result, "TrySetProperty", new object[] { item.Name, value }, out success);

                    if (!found || !(bool)success)
                    {
                        if (!isUpgraded)
                        {
                            return DeserialiseDeprecate(doc, version, isUpgraded) as IDynamicPropertyProvider;
                        }
                        else
                        {
                            Base.Compute.RecordError($"Unable to set property {item.Name} to object of type {value?.GetType().Name ?? "uknown type"}.");
                            return DeserialiseDeprecatedCustomObject(doc, version, false);
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                if (!isUpgraded)
                    return DeserialiseDeprecate(doc, version, isUpgraded) as IDynamicPropertyProvider;
                else
                    return DeserialiseDeprecatedCustomObject(doc, version);
            }
        }
    }
}


