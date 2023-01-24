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

using BH.Engine.Serialiser.Objects.MemberMapConventions;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static void RegisterClassMap(Type type)
        {
            if (!BsonClassMap.IsClassMapRegistered(type) && !m_TypesRegistered.Contains(type.FullName))
            {
                try
                {
                    if (type.IsEnum)
                    {
                        MethodInfo generic = m_CreateEnumSerializer.MakeGenericMethod(type);
                        generic.Invoke(null, null);
                    }
                    else if (!type.IsGenericTypeDefinition)
                    { 
                        BsonClassMap cm = new BsonClassMap(type);
                        cm.AutoMap();

                        BsonClassMap.RegisterClassMap(cm);

                        BsonSerializer.RegisterDiscriminatorConvention(type, new GenericDiscriminatorConvention()); 
                    }
                    else
                        BsonSerializer.RegisterDiscriminatorConvention(type, new GenericDiscriminatorConvention());

                    m_TypesRegistered.Add(type.FullName);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }

        /*******************************************/

        public static void RegisterClassMaps(this Assembly assembly)
        {
            if (assembly != null)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!(type.IsAbstract && type.IsSealed) && (type.IsEnum || typeof(IObject).IsAssignableFrom(type)))
                        RegisterClassMap(type);
                }
            }
        }
        

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        private static void CreateEnumSerializer<T>() where T : struct, Enum
        {
            try
            {
                BsonSerializer.RegisterSerializer(typeof(T), new EnumSerializer<T>(BsonType.String));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }


        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private static MethodInfo m_CreateEnumSerializer = typeof(Compute).GetMethod("CreateEnumSerializer", BindingFlags.NonPublic | BindingFlags.Static);
        private static HashSet<string> m_TypesRegistered = new HashSet<string>();

        /*******************************************/
    }
}



