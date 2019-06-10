/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Set the value of a property with a given name for an object")]
        [Input("obj", "object to set the value for")]
        [Input("propName", "name of the property to set the value of")]
        [Input("value", "new value of the property")]
        [Output("result", "New object with its property changed to the new value")]
        public static BHoMObject PropertyValue(this BHoMObject obj, string propName, object value)
        {
            BHoMObject newObject = obj.GetShallowClone() as BHoMObject;
            newObject.SetPropertyValue(propName, value);
            return newObject;
        }

        /***************************************************/

        public static bool SetPropertyValue(this object obj, string propName, object value)
        {
            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propName);

            if (prop != null)
            {
                if (value.GetType() != prop.PropertyType && value.GetType().GenericTypeArguments.Length > 0 && prop.PropertyType.GenericTypeArguments.Length > 0)
                {
                    value = Modify.CastGeneric(value as dynamic, prop.PropertyType.GenericTypeArguments[0]);
                }
                if (value.GetType() != prop.PropertyType)
                {
                    ConstructorInfo constructor = prop.PropertyType.GetConstructor(new Type[] { value.GetType() });
                    if (constructor != null)
                        value = constructor.Invoke(new object[] { value });
                }

                prop.SetValue(obj, value);
                return true;
            }
            else if (obj is IBHoMObject)
            {
                IBHoMObject bhomObj = obj as IBHoMObject;
                if (bhomObj == null) return false;

                if (!(bhomObj is CustomObject))
                    Compute.RecordWarning("The objects does not contain any property with the name " + propName + ". The value is being set as custom data");

                bhomObj.CustomData[propName] = value;
                return true;
            }

            return false;
        }

        /***************************************************/

        public static bool SetPropertyValue(this List<IBHoMObject> objects, Type objectType, string propName, object value)
        {
            PropertyInfo propInfo = objectType.GetProperty(propName);

            if (propInfo == null)
            {
                Compute.RecordWarning("No property with the provided name found. The value is being set as custom data");
                foreach (IBHoMObject obj in objects)
                    obj.CustomData[propName] = value;
                return true;
            }
            else
            {
                Action<object, object> setProp = (Action<object, object>)Delegate.CreateDelegate(typeof(Action<object, object>), propInfo.GetSetMethod());

                if (value is IList && value.GetType() != propInfo.PropertyType)
                {
                    IList values = ((IList)value);

                    // Check that the two lists are of equal length
                    if (objects.Count != values.Count)
                        return false;

                    // Set their property
                    for (int i = 0; i < values.Count; i++)
                        setProp(objects[i], values[i]);
                }
                else
                {
                    // Set the same property to all objects
                    foreach (object obj in objects)
                        setProp(obj, value);
                }

                return true;
            }

        }


        /***************************************************/
    }
}