using BH.oM.Base;
using BH.oM.Verification.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

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

        public static object ValueFromSource(this object obj, IValueCondition valueCondition, bool errorIfNotFound = false)
        {
            return obj.IValueFromSource(valueCondition.ValueSource, errorIfNotFound);
        }

        /***************************************************/

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

            if (sourceName.Contains("."))
            {
                string[] props = sourceName.Split('.');

                Type fragmentType = BH.Engine.Base.Create.Type(sourceName, true);

                if (fragmentType != null)
                {
                    List<IFragment> allFragmentsOfType = bhomObj.Fragments.Where(fr => fragmentType.IsAssignableFrom(fr.GetType())).ToList();
                    List<object> values = allFragmentsOfType.Select(f => ValueFromSource(f, string.Join(".", props.Skip(1)))).ToList();
                    value = values.Count == 1 ? values.First() : values;
                }

            }
            else
            {
                // Try extracting the property using an Extension method.
                if (!BH.Engine.Base.Compute.TryRunExtensionMethod(bhomObj, sourceName, out value))
                    if (errorIfNotFound)
                        BH.Engine.Base.Compute.RecordError($"No property, CustomData or extension method named `{sourceName}` found for {bhomObj.GetType().Name}.");
            }

            return value;
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
