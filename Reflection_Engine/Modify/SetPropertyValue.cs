﻿using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool SetPropertyValue(this object obj, string propName, object value)
        {
            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propName);

            if (prop != null)
            {
                prop.SetValue(obj, value);
                return true;
            }

            return false;
        }

        /***************************************************/

        public static bool SetPropertyValue(this List<BHoMObject> objects, Type objectType, string propName, object value)
        {
            PropertyInfo propInfo = objectType.GetProperty(propName);
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

        /***************************************************/
    }
}
