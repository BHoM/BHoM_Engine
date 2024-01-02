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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using BH.Engine.Base;
using BH.oM.Physical.Materials;

namespace BH.Engine.Matter
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Merges the Properties of the target and source by adding all properties on the source to the target. Checks for duplicate type/namespaces and resolves any duplicates found depending on settings provided.")]
        [Input("target", "The material to merge the properties of the source onto. The Returned material will take name and other properties from the target.")]
        [Input("source", "The source Material to grab proeprties from.")]
        [Input("prioritiseSource", "Controls if target or source should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, source is prioritised, if false, target is prioritised.")]
        [Input("uniquePerNamespace", "If true, the method is checking for similarity of MaterialProperties on the target and source based on namespace. If false, this check is instead done on exact type.")]
        [Output("material", "Target material with Properties from the Source merged onto it.")]
        public static Material CombineMaterials(this Material target, Material source, bool prioritiseSource, bool uniquePerNamespace)
        {
            if (target == null)
            {
                Base.Compute.RecordError($"{nameof(target)} {nameof(Material)} is null. Unable to merge properties of source onto target. Null returned.");
                return null;
            }

            if (source == null)
            {
                Base.Compute.RecordWarning($"{nameof(source)} {nameof(Material)} is null. Unmodified target material returned.");
                return target;
            }

            Material targetClone = target.ShallowClone();   //Clone the target to be returned
            targetClone.Properties = new List<IMaterialProperties>(target.Properties);  //Clone the target list

            if (double.IsNaN(targetClone.Density))  //If density on target is NaN, use density from source no matter the setting
                targetClone.Density = source.Density;
            else if (prioritiseSource && !double.IsNaN(source.Density)) //Density of target is not NaN, use source density if it is not NaN and if setting to prioritise source is true
                    targetClone.Density = source.Density;
            
            if (!uniquePerNamespace)    //If not unique by namespace, check properties by type
            {
                List<Type> targetTypes = target.Properties.Select(x => x.GetType()).ToList();   //Get out existing types on the target
                foreach (IMaterialProperties property in source.Properties)
                {
                    Type propType = property.GetType(); //Type of the property on the source to be added
                    if (prioritiseSource)  //If true, source object should take priority if a duplicate is found
                    {
                        if (targetTypes.Contains(propType)) //If type exists
                        {
                            targetClone.Properties.RemoveAll(x => x.GetType() == propType); //Remove all instances of the type
                        }
                        targetClone.Properties.Add(property);   //Add property from source
                    }
                    else    //else (prioritiseMap is false) -> only add property if a property of that type does not already exist
                    {
                        if (!targetTypes.Contains(propType))    //If property not already existing
                        {
                            targetClone.Properties.Add(property);   //Add
                        }
                    }
                }
            }
            else    //If unique by namespace
            {
                HashSet<string> nameSpaces = new HashSet<string>(target.Properties.Select(x => x.GetType().Namespace));     //Get out namespaces of all Proeprties on the target
                foreach (IMaterialProperties property in source.Properties)
                {
                    string propertyNamespace = property.GetType().Namespace;    //Namespace of Property to be added
                    if (prioritiseSource) //Prioritise source
                    {
                        if (nameSpaces.Contains(propertyNamespace)) //Target contains items from the namespace
                        {
                            targetClone.Properties.RemoveAll(x => x.GetType().Namespace == propertyNamespace);  //Remove all instances in the namespace
                        }
                        targetClone.Properties.Add(property);   //Add the property
                    }
                    else    //Prioritise target
                    {
                        if (!nameSpaces.Contains(propertyNamespace))    //Target does not contain a property in the namespace evaluated
                        {
                            targetClone.Properties.Add(property);   //Add
                        }
                    }
                }
            }
            return targetClone;
        }

        /***************************************************/
    }
}


