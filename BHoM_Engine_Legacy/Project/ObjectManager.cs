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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Structural.Loads;
using BH.oM.Project;

namespace BH.oM.Base
{
    /// <summary>
    /// Object manager Class.
    /// Used to add objects to the project where a unique identifier other than a Guid is required. Just inputting the BHoMObject type will default the key to the object name.
    /// </summary>
    /// <typeparam name="TValue">Type of BHoMObject</typeparam>
    public class ObjectManager<TValue> : ObjectManager<string, TValue> where TValue : IObject
    {
        /// <summary>
        /// Initialises a new object manager where the BHoM object name is used as the default key
        /// </summary>
        public ObjectManager(Instance project) : base(project, "", FilterOption.Name)
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        public ObjectManager() : this (Instance.Active) { }         
    }


    /// <summary>
    /// Object manager Class.
    /// Used to add objects to the project where a unique identifier other than a Guid is required
    /// </summary>
    /// <typeparam name="TKey">Type of unique identifier</typeparam>
    /// <typeparam name="TValue">Type of BHoMObject</typeparam>
    public class ObjectManager<TKey, TValue> : IEnumerable<TValue> where TValue : IObject
    {
        Instance m_Project;
        Dictionary<TKey, TValue> m_Data;
        FilterOption m_Option;
        string m_Name;
        int m_UniqueNumber;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="option"></param>
        public ObjectManager(string name, FilterOption option) : this (Instance.Active, name, option)  {  }

        /// <summary>
        /// Initialises a new object manager based on the input name and option
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name">Name of the BHoM Property or userdata name</param>
        /// <param name="option">Fliter option defines the type of key to be used</param>
        public ObjectManager(Instance project, string name, FilterOption option)
        {
            Initialise(project, name, option);
        }
        
        protected void Initialise(Instance project, string name, FilterOption option)
        {
            m_Project = project;      
            m_Data = new ObjectFilter<TValue>(project).ToDictionary<TKey>(name, option);            
            m_Option = option;
            m_Name = name;
        }

        public void AddRange(IEnumerable<TValue> items)
        {
            TKey key = default(TKey);
            foreach (TValue item in items)
            {
                switch (m_Option)
                {
                    case FilterOption.Guid:
                        key = (TKey)(object)item.BHoM_Guid;
                        break;
                    case FilterOption.Name:
                        key = (TKey)(object)item.Name;
                        break;
                    case FilterOption.Property:
                        key = (TKey)(object)item.GetType().GetProperty(m_Name).GetValue(item);
                        break;
                    case FilterOption.UserData:
                        object data = null;
                        if (item.CustomData.TryGetValue(m_Name, out data))
                        {
                            key = (TKey)data;
                        }
                        break;
                }
                if (key != null)
                {
                    Add(key, item);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TValue Add(TKey key, TValue value)
        {
            TValue result = default(TValue);

            if (!m_Data.TryGetValue(key, out result))
            {
                m_Data.Add(key, value);
                m_Project.AddObject(value as BHoMObject);
                if (m_UniqueNumber > 0) m_UniqueNumber++;
                result = value;

                switch (m_Option)
                {
                    case FilterOption.Guid:
                        break;
                    case FilterOption.Name:
                        value.Name = key.ToString();
                        break;
                    case FilterOption.Property:
                        value.GetType().GetProperty(m_Name).SetValue(value, key);
                        break;
                    case FilterOption.UserData:
                        object objValue = null;
                        if (value.CustomData.TryGetValue(m_Name, out objValue))
                        {
                            value.CustomData[m_Name] = key;
                        }
                        else
                        {
                            value.CustomData.Add(m_Name, key);
                        }

                        break;
                }
            }    
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetUniqueNumber()
        {
            if (m_UniqueNumber == 0)
            {
                if (m_Data.Count > 0 && m_Data.Values.First().GetType() == typeof(Int32))
                {
                    foreach (TKey value in m_Data.Keys)
                    {
                        int key = int.Parse(value.ToString());
                        m_UniqueNumber = Math.Max(key, m_UniqueNumber);
                    }
                }
                else
                {
                    foreach (TKey value in m_Data.Keys)
                    {
                        int key = 0;
                        int.TryParse(key.ToString(), out key);                       
                        m_UniqueNumber = Math.Max(key, m_UniqueNumber);
                    }
                }
            }
            return ++m_UniqueNumber;
        }

        /// <summary>
        /// Lookup object which corresponds to the input key. Note: if the key does not exists nothing will be returned
        /// </summary>
        /// <param name="key">object key</param>
        /// <returns>object corresponding to key</returns>
        public TValue this[TKey key]
        {
            get
            {
                return TryLookup(key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<TValue> GetRange(IEnumerable<TKey> keys)
        {
            List<TValue> values = new List<TValue>();
            foreach (TKey key in keys)
            {
                values.Add(TryLookup(key));
            }
            return values;
        }

        /// <summary>
        /// Safe method for looking up value in dictionary
        /// </summary>
        /// <param name="key">object key</param>
        /// <returns>if key exists it will return the object, null otherwise</returns>
        public TValue TryLookup(TKey key)
        {
            TValue result = default(TValue);
            m_Data.TryGetValue(key, out result);
            return result;
        }

        /// <summary>
        /// Remove and object from the manager with the matching key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKey key)
        {
            TValue result = default(TValue);
            if (m_Data.TryGetValue(key, out result))
            {
                m_Data.Remove(key);
                m_Project.RemoveObject(result.BHoM_Guid);
            }
        }

        public List<TKey> Keys
        {
            get
            {
                return m_Data.Keys.ToList();
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            return m_Data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Data.Values.GetEnumerator();
        }

        /// <summary>
        /// Number of objects in the collection
        /// </summary>
        public int Count
        {
            get
            {
                return m_Data.Count;
            }
        }       
    }
}
