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
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Base
{
	public static partial class Query
	{
		/***************************************************/
		/**** Public Methods                            ****/
		/***************************************************/

		[Description("Get all the Properties and Sub properties of the given object in their full name form.")]
		[Input("obj", "Object to get the properties from.")]
		[Input("maxDepth", "(Optional, defaults to 100) Maximum property nesting level.")]
		public static HashSet<string> GetAllPropertyFullNames(this object obj, int maxDepth = 100)
		{
			if (obj == null || maxDepth < 1)
				return new HashSet<string>();

			HashSet<string> allPropertyFullNames = new HashSet<string>();
			GetAllPropertyFullNames(obj, obj.GetType().FullName, 0, ref allPropertyFullNames, maxDepth);

			return allPropertyFullNames ?? new HashSet<string>();
		}

		/***************************************************/

		[Description("Get all the Properties and Sub properties of the given type in their full name form.")]
		[Input("type", "Type to get the properties from.")]
		[Input("maxDepth", "(Optional, defaults to 100) Maximum property nesting level.")]
		[Input("cached", "(Optional, defaults to true) If true, caches the FullNames found for a type," +
			"so that if the same type is encountered again in the same session the computation is faster.")]
		public static HashSet<string> GetAllPropertyFullNames(this Type type, int maxDepth = 100, bool cached = true)
		{
			if (maxDepth < 1)
				return new HashSet<string>();

			HashSet<string> allPropertyFullNames = null;

			if (cached && m_cachedPropertyFullNames.TryGetValue(type, out allPropertyFullNames))
				return allPropertyFullNames;

			allPropertyFullNames = new HashSet<string>();

			PropertyNameTree(type.FullName, type.GetProperties(), 0, maxDepth, ref allPropertyFullNames);

			// Results for the same type are cached for performance, if required.
			if (cached)
				m_cachedPropertyFullNames[type] = allPropertyFullNames;

			return allPropertyFullNames;
		}

		/***************************************************/
		/**** Private Methods                           ****/
		/***************************************************/

		private static void PropertyNameTree(string toCombine, PropertyInfo[] properties, int currentDepth, int maxDepth, ref HashSet<string> allPropertyFullNames)
		{
			if (currentDepth >= maxDepth)
				return;

			foreach (var property in properties)
			{
				string res = toCombine + "." + property.Name;
				var subProps = property.PropertyType.GetProperties();

				allPropertyFullNames.Add(res);

				PropertyNameTree(res, subProps, currentDepth + 1, maxDepth, ref allPropertyFullNames);
			}
		}

		/***************************************************/

		private static void GetAllPropertyFullNames(object obj, string nameToCombine, int nestingLevel, ref HashSet<string> result, int maxDepth = 100)
		{
			if (nestingLevel > maxDepth || obj == null || result == null)
				return;

			Type objectType = obj.GetType();

			if (!typeof(IObject).IsAssignableFrom(objectType))
				return;

			var props = objectType.GetProperties();

			foreach (var prop in props)
			{
				// Get only the readable, declared properties.
				if (!prop.CanRead || !typeof(IObject).IsAssignableFrom(prop.DeclaringType))
					return;

				string propertyPath = nameToCombine + "." + prop.Name;

				result.Add(propertyPath);

				GetAllPropertyFullNames(prop.GetValue(obj), propertyPath, nestingLevel + 1, ref result, maxDepth);
			}
		}

		/***************************************************/
		/**** Private Fields                            ****/
		/***************************************************/

		private static Dictionary<Type, HashSet<string>> m_cachedPropertyFullNames = new Dictionary<System.Type, System.Collections.Generic.HashSet<string>>();

	}
}




