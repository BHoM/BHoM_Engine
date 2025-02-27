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
    public static partial class Compute
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Tries to extract a value from an object based on the instruction embedded in the provided " + nameof(IValueSource) + ".")]
        [Input("obj", "Object to extract the value from.")]
        [Input("valueSource", "Object defining how to extract the value from the input object.")]
        [MultiOutput(0, "found", "True if value source exists in the input object (i.e. value could be extracted from the object), otherwise false.")]
        [MultiOutput(1, "value", "Value extracted from the input object based on the provided instruction.")]
        public static Output<bool, object> ITryGetValueFromSource(this object obj, IValueSource valueSource)
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
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(TryGetValueFromSource), new object[] { valueSource }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Extraction failed because value source of type {valueSource.GetType().Name} is currently not supported.");
                return ValueNotFound();
            }

            return result as Output<bool, object>;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Tries to extract a value from an object based on the instruction embedded in the provided " + nameof(IValueCondition) + ".")]
        [Input("obj", "Object to extract the value from.")]
        [Input("valueCondition", "Condition containing information about how to extract the value from the input object.")]
        [MultiOutput(0, "found", "True if value source exists in the input object (i.e. value could be extracted from the object), otherwise false.")]
        [MultiOutput(1, "value", "Value extracted from the input object based on the provided instruction.")]
        public static Output<bool, object> TryGetValueFromSource(this object obj, IValueCondition valueCondition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not extract value from a null object.");
                return null;
            }

            if (valueCondition?.ValueSource == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not extract value based on a null value source.");
                return null;
            }

            return obj.ITryGetValueFromSource(valueCondition.ValueSource);
        }

        /***************************************************/

        [Description("Tries to extract a value from an object based on the instruction embedded in the provided " + nameof(PropertyValueSource) + ".")]
        [Input("obj", "Object to extract the value from.")]
        [Input("valueSource", "Object defining how to extract the value from the input object.")]
        [MultiOutput(0, "found", "True if value source exists in the input object (i.e. value could be extracted from the object), otherwise false.")]
        [MultiOutput(1, "value", "Value extracted from the input object based on the provided instruction.")]
        public static Output<bool, object> TryGetValueFromSource(this object obj, PropertyValueSource valueSource)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not extract value from a null object.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(valueSource?.PropertyName))
            {
                BH.Engine.Base.Compute.RecordError("Could not extract value based on an empty value source.");
                return null;
            }

            return obj.TryGetValueFromSource(valueSource.PropertyName);
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        // Re-written from BH.Engine.Reflection.Query.PropertyValue for additional features.
        // Imporantly, if this does not find the value in any property or CustomData, then it invokes RunExtensionMethod
        // with the last segment of the source path (segments = separated by dots).
        private static Output<bool, object> TryGetValueFromSource(this object obj, string sourceName)
        {
            if (obj is Output<bool, object> objAsOutput)
                obj = objAsOutput.Item2;

            // If source name not set, compare entire object
            if (obj == null || sourceName == null)
                return ValueNotFound();

            // Try get the value from nested properties
            if (sourceName.Contains("."))
            {
                string[] props = sourceName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string innerProp in props)
                {
                    obj = obj.TryGetValueFromSource(innerProp);
                    if (obj == null)
                        break;
                }

                return ValueFound(obj);
            }

            // Get the object if indexing applied
            if (sourceName.Contains('[') || sourceName.Contains(']'))
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
                obj = obj.TryGetValueFromSource(propName);
                if (obj == null)
                    return ValueNotFound();

                foreach (string match in m_IndexerPattern.Matches(sourceName).Cast<Match>().Select(x => x.Value))
                {
                    string trimmed = match.Trim('[', ']');
                    if (obj is IList list)
                    {
                        int index;
                        if (int.TryParse(trimmed, out index) && index >= 0 && list.Count > index)
                            obj = list[index];
                        else
                            return ValueNotFound();
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

                        return ValueNotFound();
                    }
                    else
                        return ValueNotFound();
                }

                return ValueFound(obj);
            }

            // Try get value from property
            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(sourceName);
            if (prop != null)
                return ValueFound(prop.GetValue(obj));

            // If source name is type, get type
            if (sourceName == "Type")
                return ValueFound(obj.GetType());

            // Finally try fallback methods (currently implemented for IBHoMObject)
            return GetValue(obj as dynamic, sourceName);
        }

        /***************************************************/

        private static Output<bool, object> InvalidPropertyError(string sourceName)
        {
            BH.Engine.Base.Compute.RecordNote($"{sourceName} is not a valid property, indexer or method name.");
            return ValueNotFound();
        }

        /***************************************************/

        private static Output<bool, object> GetValue(this IBHoMObject bhomObj, string sourceName)
        {
            object value = null;

            // Try get the value from CustomData in general
            if (bhomObj.CustomData.TryGetValue(sourceName, out value))
                return ValueFound(value);

            Type fragmentType = BH.Engine.Base.Create.Type(sourceName, true);
            if (fragmentType != null)
            {
                List<IFragment> allFragmentsOfType = bhomObj.Fragments.Where(fr => fragmentType.IsAssignableFrom(fr.GetType())).ToList();
                if (allFragmentsOfType.Count == 1)
                    return ValueFound(allFragmentsOfType[0]);
                else if (allFragmentsOfType.Count > 1)
                {
                    BH.Engine.Base.Compute.RecordNote($"Value extraction failed due to ambiguity: {allFragmentsOfType.Count} fragments of type {sourceName} found. BHoM_Guid: {bhomObj.BHoM_Guid}");
                    return ValueNotFound();
                }
            }

            return GetValue((object)bhomObj, sourceName);
        }

        /***************************************************/

        private static Output<bool, object> GetValue(this object obj, string sourceName)
        {
            // Try extracting the property using an Extension method.
            object value;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, sourceName, out value))
            {
                BH.Engine.Base.Compute.RecordNote($"No property, CustomData or extension method named `{sourceName}` found for {obj.GetType().Name}.");
                return ValueNotFound();
            }

            return ValueFound(value);
        }

        /***************************************************/

        [Description("Called when the method terminated correctly and value was found.")]
        private static Output<bool, object> ValueFound(object value)
        {
            if (value is Output<bool, object> valueAsOutput)
                value = valueAsOutput.Item2;

            return new Output<bool, object> { Item1 = true, Item2 = value };
        }

        /***************************************************/

        [Description("Called when the method terminated correctly, but value not found.")]
        private static Output<bool, object> ValueNotFound()
        {
            return new Output<bool, object> { Item1 = false, Item2 = null };
        }


        /***************************************************/
        /****               Private Fields              ****/
        /***************************************************/

        private static readonly Regex m_IndexerPattern = new Regex("\\[[^\\]]+\\]");

        /***************************************************/
    }
}
