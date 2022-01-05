/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson;
using BH.oM.Base;

namespace BH.Engine.Serialiser.Conventions
{
    /// <summary>
    /// Ensures the SetIgnoreExtraElements, Discriminator and IdMember is correctly set for generic types
    /// Important for generic types that gets registred by a different mechanism compared to the one in compute
    /// for example if a generic type is the value type of a Dictionary
    /// </summary>
    public class BHoMDefaultClassMapConvention : ConventionBase, IClassMapConvention
    {
        /*******************************************/
        /**** Interface Methods                 ****/
        /*******************************************/

        public void Apply(BsonClassMap classMap)
        {
            Type classMapType = classMap.ClassType;

            if (!classMapType.IsGenericType)
                return;

            if (!typeof(IObject).IsAssignableFrom(classMapType))
                return;

            classMap.SetDiscriminator(classMapType.FullName);
            classMap.SetDiscriminatorIsRequired(true);
            classMap.SetIgnoreExtraElements(true);
            classMap.SetIdMember(null);
        }


        /*******************************************/

    }
}


