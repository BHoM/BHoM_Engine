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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Verification.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Extracts a value from an object based on the instruction embedded in the provided " + nameof(IValueSource) + ".")]
        [Input("obj", "Object to extract the value from.")]
        [Input("valueSource", "Object defining how to extract the value from the input object.")]
        [Input("errorIfNotFound", "If true, error will be raised in case the value could not be found, otherwise not.")]
        [Output("value", "Value extracted from the input object based on the provided instruction.")]
        public static object IValueFromSource(this object obj, IValueSource valueSource, bool errorIfNotFound = false)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not extract value from a null object.");
                return null;
            }

            if (valueSource == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not extract value based on a null value source.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(ValueFromSource), new object[] { valueSource, errorIfNotFound }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Extraction failed because value source of type {valueSource.GetType().Name} is currently not supported.");
                return null;
            }

            return result;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Extracts a value from an object based on the instruction embedded in the provided " + nameof(IValueCondition) + ".")]
        [Input("obj", "Object to extract the value from.")]
        [Input("valueCondition", "Condition containing an object defining how to extract the value from the input object.")]
        [Input("errorIfNotFound", "If true, error will be raised in case the value could not be found, otherwise not.")]
        [Output("value", "Value extracted from the input object based on the provided instruction.")]
        public static object ValueFromSource(this object obj, IValueCondition valueCondition, bool errorIfNotFound = false)
        {
            return obj.IValueFromSource(valueCondition.ValueSource, errorIfNotFound);
        }

        /***************************************************/

        [Description("Extracts a value from an object based on the instruction embedded in the provided " + nameof(PropertyValueSource) + ".")]
        [Input("obj", "Object to extract the value from.")]
        [Input("valueSource", "Object defining how to extract the value from the input object.")]
        [Input("errorIfNotFound", "If true, error will be raised in case the value could not be found, otherwise not.")]
        [Output("value", "Value extracted from the input object based on the provided instruction.")]
        public static object ValueFromSource(this object obj, PropertyValueSource valueSource, bool errorIfNotFound = false)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not extract value from a null object.");
                return null;
            }

            if (valueSource == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not extract value based on a null value source.");
                return null;
            }

            return obj.ValueFromSource(valueSource.PropertyName, errorIfNotFound);
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        // Re-written from BH.Engine.Reflection.Query.PropertyValue for additional features.
        // Imporantly, if this does not find the value in any property or CustomData, then it invokes RunExtensionMethod
        // with the last segment of the source path (segments = separated by dots).
        private static object ValueFromSource(this object obj, string sourceName, bool errorIfNotFound = false)
        {
            // If source name not set, compare entire object
            if (obj == null || sourceName == null)
                return obj;

            // Try get the value from nested properties
            if (sourceName.Contains("."))
            {
                string[] props = sourceName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string innerProp in props)
                {
                    obj = obj.ValueFromSource(innerProp);
                    if (obj == null)
                        break;
                }

                return obj;
            }

            // Get the object if indexing applied
            if (m_IndexedPropertyPattern.IsMatch(sourceName))
            {
                // Check if there is no nested or open brackets or any characters between brackets
                char expected = '[';
                bool startNewBracket = false;
                foreach (char c in sourceName)
                {
                    if (c == '[')
                    {
                        if (expected != '[')
                            return InvalidPropertyError(sourceName);
                        else
                            expected = ']';

                        startNewBracket = false;
                    }
                    else if (c == ']')
                    {
                        if (expected != ']')
                            return InvalidPropertyError(sourceName);
                        else
                            expected = '[';

                        startNewBracket = true;
                    }
                    else if (startNewBracket)
                        return InvalidPropertyError(sourceName);
                }

                if (expected != '[')
                    return InvalidPropertyError(sourceName);

                int nameCount = sourceName.IndexOf('[');
                string propName = sourceName.Substring(0, nameCount);
                obj = obj.ValueFromSource(propName);
                if (obj == null)
                    return IndexingFailedError(sourceName, propName);

                foreach (string match in m_IndexerPattern.Matches(sourceName).Cast<Match>().Select(x => x.Value))
                {
                    string trimmed = match.Trim('[', ']');
                    if (obj is IList list)
                    {
                        int index;
                        if (int.TryParse(trimmed, out index) && index >= 0 && list.Count > index)
                            obj = list[index];
                        else
                            return IndexingFailedError(sourceName, trimmed);
                    }
                    else if (obj is IDictionary dictionary)
                    {
                        if (dictionary.Contains(trimmed))
                        {
                            obj = dictionary[trimmed];
                            continue;
                        }

                        int asInt;
                        if (int.TryParse(trimmed, out asInt) && dictionary.Contains(asInt))
                        {
                            obj = dictionary[asInt];
                            continue;
                        }

                        double asDouble;
                        if (double.TryParse(trimmed, out asDouble) && dictionary.Contains(asDouble))
                        {
                            obj = dictionary[asDouble];
                            continue;
                        }

                        return IndexingFailedError(sourceName, trimmed);
                    }
                    else
                        return IndexingFailedError(sourceName, trimmed);
                }

                return obj;
            }

            // Try get value from property
            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(sourceName);
            if (prop != null)
                return prop.GetValue(obj);

            // If source name is type, get type
            if (sourceName == "Type")
                return obj.GetType();

            // Finally try fallback methods (currently implemented for IBHoMObject)
            return GetValue(obj as dynamic, sourceName, errorIfNotFound);
        }

        /***************************************************/

        private static object IndexingFailedError(string sourceName, string index)
        {
            BH.Engine.Base.Compute.RecordError($"Indexing of {sourceName} failed on {index}.");
            return null;
        }

        /***************************************************/

        private static object InvalidPropertyError(string sourceName)
        {
            BH.Engine.Base.Compute.RecordError($"{sourceName} is not a valid property name.");
            return null;
        }

        /***************************************************/

        private static object GetValue(this IBHoMObject bhomObj, string sourceName, bool errorIfNotFound = false)
        {
            object value = null;

            // Try get the value from CustomData in general
            if (bhomObj.CustomData.TryGetValue(sourceName, out value))
                return value;

            Type fragmentType = BH.Engine.Base.Create.Type(sourceName, true);
            if (fragmentType != null)
            {
                List<IFragment> allFragmentsOfType = bhomObj.Fragments.Where(fr => fragmentType.IsAssignableFrom(fr.GetType())).ToList();
                if (allFragmentsOfType.Count == 1)
                    return allFragmentsOfType[0];
                else if (allFragmentsOfType.Count > 1)
                {
                    BH.Engine.Base.Compute.RecordError($"Value extraction failed due to ambiguity: {allFragmentsOfType.Count} fragments of type {sourceName} found. BHoM_Guid: {bhomObj.BHoM_Guid}");
                    return null;
                }
            }

            return GetValue((object)bhomObj, sourceName, errorIfNotFound);
        }

        /***************************************************/

        private static object GetValue(this object obj, string sourceName, bool errorIfNotFound)
        {
            // Try extracting the property using an Extension method.
            object value = null;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, sourceName, out value))
                if (errorIfNotFound)
                    BH.Engine.Base.Compute.RecordError($"No property, CustomData or extension method named `{sourceName}` found for {obj.GetType().Name}.");

            return value;
        }


        /***************************************************/
        /****               Private Fields              ****/
        /***************************************************/

        private static readonly Regex m_IndexedPropertyPattern = new Regex("^[a-zA-Z0-9]+\\[[^\\]]+\\]");
        private static readonly Regex m_IndexerPattern = new Regex("\\[[^\\]]+\\]");

        /***************************************************/
    }
}

