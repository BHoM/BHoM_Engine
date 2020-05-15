/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.Engine.Serialiser.Objects;
using BH.Engine.Serialiser.Objects.MemberMapConventions;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static void RegisterClassMap(Type type)
        {
            try
            {
                if (type.Name.StartsWith("Tree"))
                    Console.WriteLine("Here");

                BsonClassMap cm = new BsonClassMap(type);
                cm.AutoMap();
                cm.SetDiscriminator(type.FullName);
                cm.SetDiscriminatorIsRequired(true);
                cm.SetIgnoreExtraElements(false);   // It would have been nice to use cm.MapExtraElementsProperty("CustomData") but it doesn't work for inherited properties
                cm.SetIdMember(null);

                BsonClassMap.RegisterClassMap(cm);

                BsonSerializer.RegisterDiscriminatorConvention(type, new GenericDiscriminatorConvention());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        /*******************************************/
    }
}
