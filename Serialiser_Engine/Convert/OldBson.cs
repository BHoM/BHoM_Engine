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
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Data;
using MongoDB.Bson.Serialization.Conventions;
using BH.Engine.Serialiser.BsonSerializers;
using BH.Engine.Serialiser.MemberMapConventions;
using BH.Engine.Serialiser.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System.Diagnostics;
using BH.Engine.Serialiser.Objects.MemberMapConventions;
using System.Reflection;
using BH.Engine.Serialiser.Objects;
using System.Drawing;
using System.Text.RegularExpressions;
using BH.Engine.Versioning;
using System.Collections;

namespace BH.Engine.Serialiser
{
    public static partial class Convert
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static BsonDocument ToOldBson(this object obj)
        {
            RegisterTypes();

            if (obj is string)
            {
                BsonDocument document;
                BsonDocument.TryParse(obj as string, out document);
                return document;
            }
            else
            {
                BsonDocument document = obj.ToBsonDocument();
                if (document != null)
                    document.AddVersion();
                return document;
            }
                
        }

        /*******************************************/

        public static object FromOldBson(BsonDocument bson)
        {
            RegisterTypes();

            // Patch for handling the case where a string is a top object - will need proper review in next quarter
            if (bson.Contains("_t") && bson["_t"] == "System.String" && bson.Contains("_v"))
                return bson["_v"].AsString;

            bson.Remove("_id");

            object obj = BsonSerializer.Deserialize(bson, typeof(object));
            if (obj is ExpandoObject)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>(obj as ExpandoObject);
                CustomObject co = new CustomObject();
                if (dic.ContainsKey("Name"))
                {
                    co.Name = dic["Name"] as string;
                    dic.Remove("Name");
                }
                if (dic.ContainsKey("Tags"))
                {
                    co.Tags = new HashSet<string>(((List<object>)dic["Tags"]).Cast<string>());
                    dic.Remove("Tags");
                }
                if (dic.ContainsKey("BHoM_Guid"))
                {
                    co.BHoM_Guid = (Guid)dic["BHoM_Guid"];
                    dic.Remove("BHoM_Guid");
                }
                co.CustomData = dic;
                return co;
            }
            else
                return obj;
        }

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        static Convert()
        {
            RegisterTypes();
        }

        /*******************************************/

        private static void RegisterTypes()
        {
            lock (m_RegisterTypesLock)
            {
                if (m_TypesRegistered)
                    return;

                RegisterPacks();

                RegisterSerializers();

                RegisterClassMaps();

                m_TypesRegistered = true;
            }
        }

        /*******************************************/

        private static void RegisterPacks()
        {
            // Fix the ImmutableTypeClassMapConvention by replacing it with our own version
            // For the content of the default pack, check https://github.com/mongodb/mongo-csharp-driver/blob/14e046f23640ff9257c4edf53065b9a6768254d4/src/MongoDB.Bson/Serialization/Conventions/DefaultConventionPack.cs
            ConventionRegistry.Remove("__defaults__");
            ConventionPack defaultPack = new ConventionPack();
            defaultPack.Add(new ReadWriteMemberFinderConvention());
            defaultPack.Add(new NamedIdMemberConvention(new[] { "Id", "id", "_id" }));
            defaultPack.Add(new NamedExtraElementsMemberConvention(new[] { "ExtraElements" }));
            defaultPack.Add(new IgnoreExtraElementsConvention(false));
            defaultPack.Add(new ImmutableTypeClassMapConventionFixed());
            defaultPack.Add(new NamedParameterCreatorMapConvention());
            defaultPack.Add(new StringObjectIdIdGeneratorConvention());
            defaultPack.Add(new LookupIdGeneratorConvention());
            ConventionRegistry.Register("__defaults__", defaultPack, x => x is object);

            // Define the conventions   
            var pack = new ConventionPack();
            pack.Add(new BHoMDefaultClassMapConvention());
            pack.Add(new ImmutableBHoMClassMapConvention());
            pack.Add(new ImmutableBHoMCreatorMapConvention());
            pack.Add(new BHoMDictionaryConvention());
            ConventionRegistry.Register("BHoM Conventions", pack, x => x is object);

            var pack2 = new ConventionPack();
            pack2.Add(new BHoMEnumConvention());
            ConventionRegistry.Register("Enum Conventions", pack2, x => x.GetType().IsEnum);
        }

        /*******************************************/

        private static void RegisterSerializers()
        {
            try
            {
                BsonSerializer.RegisterSerializer(typeof(object), new BH_ObjectSerializer());
                BsonSerializer.RegisterSerializer(typeof(System.Drawing.Color), new ColourSerializer());
                BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));
                BsonSerializer.RegisterSerializer(typeof(CustomObject), new CustomObjectSerializer());
                BsonSerializer.RegisterSerializer(typeof(FragmentSet), new BHoMCollectionSerializer<FragmentSet, IFragment>());
                BsonSerializer.RegisterSerializer(typeof(Enum), new EnumSerializer());
                BsonSerializer.RegisterSerializer(typeof(DataTable), new DataTableSerialiser());
                BsonSerializer.RegisterSerializer(typeof(Bitmap), new BitmapSerializer());
                BsonSerializer.RegisterSerializer(typeof(IntPtr), new IntPtrSerializer());
                BsonSerializer.RegisterSerializer(typeof(Regex), new BsonSerializers.RegexSerializer());

                var typeSerializer = new TypeSerializer();
                BsonSerializer.RegisterSerializer(typeof(Type), typeSerializer);
                BsonSerializer.RegisterSerializer(Type.GetType("System.RuntimeType"), typeSerializer);

                var methodBaseSerializer = new MethodBaseSerializer();
                BsonSerializer.RegisterSerializer(typeof(MethodBase), methodBaseSerializer);
                BsonSerializer.RegisterSerializer(typeof(MethodInfo), methodBaseSerializer);

                BsonDefaults.DynamicDocumentSerializer = new CustomObjectSerializer();

                BsonSerializer.RegisterDiscriminatorConvention(typeof(IBHoMObject), new BHoMObjectDiscriminatorConvention());
                BsonSerializer.RegisterDiscriminatorConvention(typeof(IObject), new BHoMObjectDiscriminatorConvention());
            }
            catch (Exception)
            {
                Debug.WriteLine("Problem with initialisation of the Bson Serializer");
            }
        }

        /*******************************************/

        private static void RegisterClassMaps()
        {
            BH.Engine.Base.Query.BHoMTypeList().ForEach(x => Compute.RegisterClassMap(x));
            Compute.RegisterClassMap(typeof(System.Drawing.Color));
            Compute.RegisterClassMap(typeof(MethodInfo));
            Compute.RegisterClassMap(typeof(ConstructorInfo));
            Compute.RegisterClassMap(typeof(Bitmap));
            Compute.RegisterClassMap(typeof(IntPtr));
            Compute.RegisterClassMap(typeof(Regex));
        }


        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private static bool m_TypesRegistered = false;
        private static readonly object m_RegisterTypesLock = new object();

        /*******************************************/
    }
}

