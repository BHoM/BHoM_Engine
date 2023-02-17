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
using BH.Engine.Base;
using System.Collections;

namespace BH.oM.Base
{
    // Must be unpackable
    public class Container<T> : BHoMObject, IContainer
    {
        public T? SomeObject { get; set; }
        public IEnumerable<T> ListOfObjects { get; set; } = new List<T>();
        public IEnumerable<IEnumerable<T>> ListOfLists { get; set; } = new List<List<T>>();
    }

    // We want to support this, it should get the values of the dictionary.
    public class DictionaryContainer<T> : Container<T>
    {
        public Dictionary<string, T> Dictionary { get; set; } = new Dictionary<string, T>();
    }

    // We want to support this, it should get the values of the dictionary and flatten them.
    public class DictionaryListContainer<T> : Container<T>
    {
        public Dictionary<string, IEnumerable<T>> DictionaryOfLists { get; set; } = new Dictionary<string, IEnumerable<T>>();
    }

    // Not supported.
    public class ListOfDictionariesContainer<T> : Container<T>
    {
        public IEnumerable<Dictionary<string, T>> ListOfDictionaries { get; set; } = new List<Dictionary<string, T>>();
    }

    // Not supported.
    public class ListOfListOfListContainer<T> : Container<T>
    {
        public IEnumerable<IEnumerable<IEnumerable<T>>> ListOfListOfLists { get; set; } = new List<List<List<T>>>();
    }
}