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

using BH.oM.Structural;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.oM.Project
{
    public enum FilterOption
    {
        Name = 0,
        Guid,
        Property,
        UserData
    }

    public class ObjectFilter : ObjectFilter<BHoMObject>
    {
        public ObjectFilter(Instance project) : base(project) { }

        public ObjectFilter() : this(Instance.Active) { }

        public ObjectFilter(List<BHoMObject> objects)
        {
            m_Data = objects;
        }

        public ObjectFilter<BHoMObject> OfClass(Type t)
        {
            List<BHoMObject> result = new List<BHoMObject>();
            foreach (object obj in m_Data)
            {
                if (t.IsAssignableFrom(obj.GetType()))
                {
                    result.Add(obj as BHoMObject);
                }
            }
            return new ObjectFilter(result);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ObjectFilter<T> : IEnumerable<T>, IEnumerable where T : IObject
    {
        private Instance m_Project;
        protected List<T> m_Data;
        //private int m_UniqueNumber;           //Never used
        //private FilterOption m_Option;        //Never used
        //private string m_Name;                //Never used

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        public ObjectFilter(Instance project)
        {
            m_Project = project;
            m_Data = FilterClass(m_Project.Objects);
        }

        // <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        public ObjectFilter(IEnumerable<T> data)
        {
            m_Data = data.ToList();
        }

        public ObjectFilter() : this(Instance.Active) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        internal ObjectFilter(Instance project, List<T> list)
        {
            m_Project = project;
            m_Data = new List<T>();
            for(int i = 0; i < list.Count;i++)
            {
                m_Data.Add(list[i]);
            }
        }

        private List<T> FilterClass(IEnumerable<BHoMObject> list)
        {
            List<T> result = new List<T>();
            foreach (BHoMObject obj in list)
            {
                if (typeof(T).IsAssignableFrom(obj.GetType()))
                {
                    result.Add((T)(object)obj);
                }
            }
            return result;
        }

        public ObjectFilter<T> Implements(Type t)
        {
            List<T> result = new List<T>();
            foreach (IObject obj in m_Data)
            {
                if (t.IsAssignableFrom(obj.GetType()))
                {
                    result.Add((T)(object)obj);
                }
            }
            return new ObjectFilter<T>(m_Project, result);
        }

        /// <summary>
        /// Create a dictionary based on a defined unqiue key, Note: if duplicate keys exists on the first one found will be added
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="option"></param>
        /// <returns>A key, value pair based on the input option</returns>
        public Dictionary<TKey,T> ToDictionary<TKey>(string propertyName, FilterOption option)
        {
            Dictionary<TKey, T> result = new Dictionary<TKey, T>();
            for (int i = 0; i < m_Data.Count; i++)
            {
                if (m_Data[i] != null)
                {
                    TKey key = GetKey<TKey>(m_Data[i], propertyName, option);

                    T value = default(T);
                    if (key != null && !result.TryGetValue(key, out value))
                    {
                        result.Add(key, m_Data[i]);
                    }
                }
            }
            return result;
        }
        internal static TKey GetKey<TKey>(T obj, string name, FilterOption option)
        {
            if (obj != null)
            {
                switch (option)
                {
                    case FilterOption.Name:
                        return (TKey)(object)obj.Name;
                    case FilterOption.Guid:
                        if (typeof(TKey) == typeof(string))
                        {
                            return (TKey)(object)obj.BHoM_Guid.ToString();
                        }
                        else
                        {
                            return (TKey)(object)obj.BHoM_Guid;
                        }
                    case FilterOption.Property:
                        System.Reflection.PropertyInfo pInfo = obj.GetType().GetProperty(name);
                        if (pInfo != null)
                        {
                            return (TKey)(object)pInfo.GetValue(obj);
                        }
                        break;
                    case FilterOption.UserData:
                        object keyResult = null;
                        if (obj.CustomData.TryGetValue(name, out keyResult))
                        {
                            if (keyResult.GetType() != typeof(TKey))
                            {
                                System.Reflection.MethodInfo parseMethod = typeof(TKey).GetMethod("Parse", new Type[] { typeof(string) });
                                if (parseMethod != null)
                                {
                                    obj.CustomData[name] = (obj.CustomData[name] = parseMethod.Invoke(null, new object[] { keyResult.ToString() }));
                                    return (TKey)(object)obj.CustomData[name];
                                }
                            }
                            else
                            {
                                return (TKey)(object)keyResult;
                            }

                        }
                        return default(TKey);
                }
            }
            return default(TKey);
        }
   
        public IEnumerator GetEnumerator()
        {
            return m_Data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return m_Data.GetEnumerator();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BHoMObjectEnum : IEnumerator
    {
        /// <summary></summary>
        public BHoMObject[] _BHoMOjects;

        int position = -1;

        /// <summary></summary>
        /// <param name="list"></param>
        public BHoMObjectEnum(BHoMObject[] list)
        {
            _BHoMOjects = list;
        }

        /// <summary></summary>
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary></summary>
        public BHoMObject Current
        {
            get
            {
                try
                {
                    return _BHoMOjects[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary></summary>
        public bool MoveNext()
        {
            position++;
            return (position < _BHoMOjects.Length);
        }

        /// <summary></summary>
        public void Reset()
        {
            position = -1;
        }
    }
}
