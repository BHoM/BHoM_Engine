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
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static CustomObject CustomObject(Dictionary<string, object> data, string name = "")
        {
            return new CustomObject
            {
                CustomData = new Dictionary<string, object>(data),
                Name = name
            };
        }

        /***************************************************/

        public static CustomObject CustomObject(List<string> propertyNames, List<object> propertyValues, string name = "")
        {
            CustomObject result = new CustomObject();

            if (propertyNames.Count == propertyValues.Count)
            {
                for (int i = 0; i < propertyValues.Count; i++)
                {
                    string propName = propertyNames[i];
                    object propValue = propertyValues[i];

                    if (propName == "Name" && propValue is string)
                        result.Name = propValue as string;
                    else if (propName == "BHoM_Guid" && propValue is System.Guid)
                        result.BHoM_Guid = (System.Guid)propValue;
                    else if (propName == "Tags")
                        SetTags(result, propValue as dynamic);
                    else if (propName == "Fragments")
                        SetFragments(result, propValue as dynamic);
                    else
                        result.CustomData.Add(propName, propValue);
                }     
            }
            else
                throw new System.Exception("The list of property names must be the same length as the list of property values when creating a Custon object.");

            if (!string.IsNullOrEmpty(name))
                result.Name = name;

            return result;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void SetFragments<T>(CustomObject custom, IEnumerable<T> fragments)
        {
            List<IFragment> valids = fragments.OfType<IFragment>().ToList();
            if (valids.Count == fragments.Count())
                custom.Fragments = new FragmentSet(valids);
            else
                SetFragments(custom, fragments as object);
        }

        /***************************************************/

        private static void SetFragments(CustomObject custom, IFragment fragment)
        {
            custom.Fragments.Add(fragment);
        }

        /***************************************************/

        private static void SetFragments(CustomObject custom, object fragments)
        {
            Engine.Base.Compute.RecordWarning("The Fragments property should be a List<IFragment>. It will be added in CustomData instead.");
            custom.CustomData["Fragments"] = fragments;
        }

        /***************************************************/

        private static void SetTags<T>(CustomObject custom, IEnumerable<T> tags)
        {
            List<string> valids = tags.OfType<string>().ToList();
            if (valids.Count == tags.Count())
                custom.Tags = new HashSet<string>(valids);
            else
                SetTags(custom, tags as object);
        }

        /***************************************************/

        private static void SetTags(CustomObject custom, string tag)
        {
            custom.Tags.Add(tag);
        }

        /***************************************************/

        private static void SetTags(CustomObject custom, object tags)
        {
            Engine.Base.Compute.RecordWarning("The Tags property should be a List<string>. It will be added in CustomData instead.");
            custom.CustomData["Tags"] = tags;
        }

        /***************************************************/
    }
}




