/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using System;
using System.ComponentModel;

namespace BH.Engine.Base
{
    [Description("Key identifying a compiled extension method taking only the main object: the type of the object and the name of the method.")]
    internal readonly struct SingleArgumentKey : IEquatable<SingleArgumentKey>
    {
        public SingleArgumentKey(Type type, string methodName)
        {
            m_Type = type;
            m_MethodName = methodName;
            m_Hash = type.GetHashCode() * 31 + methodName.GetHashCode();
        }

        [Input("other", "The key to compare with.")]
        public bool Equals(SingleArgumentKey other)
        {
            return m_Hash == other.m_Hash && m_Type == other.m_Type && string.Equals(m_MethodName, other.m_MethodName);
        }

        [Input("obj", "The object to compare with.")]
        public override bool Equals(object obj)
        {
            return obj is SingleArgumentKey && Equals((SingleArgumentKey)obj);
        }

        public override int GetHashCode()
        {
            return m_Hash;
        }

        private readonly Type m_Type;
        private readonly string m_MethodName;
        private readonly int m_Hash;
    }
}
