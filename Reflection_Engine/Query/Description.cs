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

using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BH.Engine.Base;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Return the custom description of a C# class member (e.g. property, method, field)")]
        public static string Description(this MemberInfo member, bool addTypeDescription = true)
        {
            if(member == null)
            {
                Base.Compute.RecordWarning("Cannot query the description of a null member info object. An empty string will be returned instead.");
                return "";
            }

            DescriptionAttribute descriptionAttribute = member.GetCustomAttribute<DescriptionAttribute>();

            // Classification attribute not queried for methods - in that case it is processed per input/output, not the method itself
            ClassificationAttribute classification = null;
            if (!(member is MethodInfo))
                classification = member.GetCustomAttribute<ClassificationAttribute>();

            string desc = "";
            if (descriptionAttribute != null && !string.IsNullOrWhiteSpace(descriptionAttribute.Description))
                desc = descriptionAttribute.Description + Environment.NewLine;

            if (addTypeDescription && member is PropertyInfo && (typeof(IObject).IsAssignableFrom(((PropertyInfo)member).PropertyType) || classification != null))
            {
                desc += ((PropertyInfo)member).PropertyType.Description(classification) + Environment.NewLine;
            }

            return desc;
        }

        /***************************************************/

        [Description("Return the custom description of a C# method argument")]
        public static string Description(this ParameterInfo parameter, bool addTypeDescription = true)
        {
            if(parameter == null)
            {
                Base.Compute.RecordWarning("Cannot query the description of a null parameter object. An empty string will be returned instead.");
                return "";
            }

            IEnumerable<InputAttribute> inputDesc = parameter.Member.GetCustomAttributes<InputAttribute>().Where(x => x.Name == parameter.Name);
            ClassificationAttribute classification = parameter.Member.GetCustomAttributes<ClassificationAttribute>().FirstOrDefault(x => x.Name == parameter.Name);
            string desc = "";
            if (inputDesc.Count() > 0)
            {
                desc = inputDesc.First().Description + Environment.NewLine;

                if (classification == null)
                    classification = inputDesc.First().Quantity;
            }
            else
            {
                //If no input descs are found, check if inputFromProperty descs can be found
                IEnumerable<InputFromProperty> inputFromPropDesc = parameter.Member.GetCustomAttributes<InputFromProperty>().Where(x => x.InputName == parameter.Name);

                //Only valid for engine type methods
                MethodInfo methodInfo = parameter.Member as MethodInfo;
                if (inputFromPropDesc.Count() > 0 && methodInfo != null && methodInfo.DeclaringType != null)
                {
                    Type returnType = methodInfo.ReturnType.UnderlyingType().Type;
                    if (returnType != null)
                    {
                        //Try to find matching proeprty type, matching both name and type
                        PropertyInfo prop = returnType.GetProperty(inputFromPropDesc.First().PropertyName, parameter.ParameterType);

                        //If found return description of property
                        if (prop != null)
                            return prop.Description();
                    }
                }
            }
            if (addTypeDescription && parameter.ParameterType != null)
            {
                desc += parameter.ParameterType.Description(classification);
            }
            return desc;
        }

        /***************************************************/

        [Description("Return the custom description of a C# class")]
        public static string Description(this Type type)
        {
            return Description(type, null);
        }

        /***************************************************/

        [Description("Return the custom description of a C# class")]
        public static string Description(this Type type, ClassificationAttribute classification)
        {
            if (type == null)
            {
                Base.Compute.RecordWarning("Cannot query the description of a null type. An empty string will be returned instead.");
                return "";
            }

            DescriptionAttribute attribute = type.GetCustomAttribute<DescriptionAttribute>();

            string desc = "";

            //If a quantity attribute is present, this is used to generate the default description
            if (classification != null)
            {
                desc += classification.IDescription();
                desc += " (as a " + type.ToText(type.Namespace.StartsWith("BH.")) + ")";
                return desc;
            }

            //Add the default description
            desc += "This is a " + type.ToText(type.Namespace.StartsWith("BH."));

            if (attribute != null)
            {
                desc += ":" + Environment.NewLine;
                desc += attribute.Description;
            }

            //If type is enum, list options with descriptions
            if (type.IsEnum)
                desc += EnumItemDescription(type);

            Type innerType = type;

            while (typeof(IEnumerable).IsAssignableFrom(innerType) && innerType.IsGenericType)
                innerType = innerType.GenericTypeArguments.First();

            if (innerType.IsInterface)
            {
                desc += Environment.NewLine;
                desc += "This can be of the following types: ";
                List<Type> t = innerType.ImplementingTypes();
                int m = Math.Min(15, t.Count);

                for (int i = 0; i < m; i++)
                    desc += $"{t[i].ToText()}, ";

                if (t.Count > m)
                    desc += "and more...";
                else
                    desc = desc.Remove(desc.Length - 2, 2);

                return desc;
            }

            return desc;

        }

        /***************************************************/

        [Description("Return the custom description of a C# element such as Type, MemberInfo, and ParamaterInfo")]
        [Input("item", "This item can either be a Type, a MemberInfo, or a ParamaterInfo")]
        public static string IDescription(this object item)
        {
            if (item is ParameterInfo)
                return Description(item as ParameterInfo);
            else if (item is Type)
                return Description(item as Type);
            else if (item is MemberInfo)
                return Description(item as MemberInfo);
            else
                return "";
        }

        /***************************************************/

        [Description("Return the custom description of a classification attribute.")]
        [Input("classification", "Classification attribute to be queried for description.")]
        public static string IDescription(this ClassificationAttribute classification)
        {
            return Description(classification as dynamic);
        }

        /***************************************************/

        [Description("Return the custom description of a quantity attribute.")]
        [Input("quantity", "Quantity attribute to be queried for description.")]
        public static string Description(this QuantityAttribute quantity)
        {
            if(quantity == null)
            {
                Base.Compute.RecordWarning("Cannot query the description of a null quantity attribute. An empty string will be returned isntead.");
                return "";
            }

            return "This is a " + quantity.GetType().Name + " [" + quantity.SIUnit + "]";
        }

        /***************************************************/

        [Description("Return the description of a folder path attribute.")]
        [Input("folderPath", "Folder path attribute to be queried for description.")]
        public static string Description(this FolderPathAttribute folderPath)
        {
            return "This is a folder path.";
        }

        /***************************************************/

        [Description("Return the description of a file path attribute.")]
        [Input("filePath", "File path attribute to be queried for description.")]
        public static string Description(this FilePathAttribute filePath)
        {
            string description = "This is a file path.";
            if (filePath.FileExtensions != null && filePath.FileExtensions.Length != 0)
                description += $" It supports files with following extensions: {string.Join(", ", filePath.FileExtensions)}.";

            return description;
        }


        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        [Description("Fallback returning an empty string in case a type-specific Description method is missing for a given subtype of InputClassificationAttribute.")]
        [Input("classification", "Input classification attribute to be queried for description.")]
        private static string Description(this ClassificationAttribute classification)
        {
            return "";
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Lists all the enum options and their descriptions.")]
        private static string EnumItemDescription(this Type type)
        {
            FieldInfo[] fields = type.GetFields();
            string desc = Environment.NewLine + "Enum values:";

            int m = Math.Min(fields.Length, 20);

            for (int i = 0; i < m; i++)
            {
                FieldInfo field = fields[i];

                //Skip the value option
                if (field.Name == "value__")
                    continue;

                desc += Environment.NewLine;
                desc += "-" + field.Name;

                DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

                if (attribute != null)
                    desc += ": " + attribute.Description;
            }

            if (fields.Length > m)
                desc += Environment.NewLine + "-...And more";

            return desc;
        }

        /***************************************************/
    }
}




