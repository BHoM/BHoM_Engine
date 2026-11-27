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
    [Description("Key identifying an extension method to call: the name of the method and the runtime types of all its arguments (null for null arguments).\n" +
        "Only holds the types, never the arguments themselves, so the same key can be used both to look up and to store a method.")]
    internal readonly struct ExtensionMethodKey : IEquatable<ExtensionMethodKey>
    {
        // From the main object and the additional arguments
        public ExtensionMethodKey(string methodName, object target, object[] parameters)
        {
            int parameterCount = parameters?.Length ?? 0;
            Type[] types = new Type[1 + parameterCount];
            types[0] = target.GetType();
            for (int i = 0; i < parameterCount; i++)
                types[i + 1] = parameters[i]?.GetType();

            m_MethodName = methodName;
            m_Types = types;
            m_Hash = Hash(methodName, types);
        }

        // From all the arguments in one array, the first being the main object
        public ExtensionMethodKey(string methodName, object[] arguments)
        {
            Type[] types = new Type[arguments.Length];
            for (int i = 0; i < arguments.Length; i++)
                types[i] = arguments[i]?.GetType();

            m_MethodName = methodName;
            m_Types = types;
            m_Hash = Hash(methodName, types);
        }

        [Input("other", "The key to compare with.")]
        public bool Equals(ExtensionMethodKey other)
        {
            if (m_Hash != other.m_Hash || m_Types.Length != other.m_Types.Length || !string.Equals(m_MethodName, other.m_MethodName))
                return false;

            for (int i = 0; i < m_Types.Length; i++)
            {
                if (m_Types[i] != other.m_Types[i])
                    return false;
            }

            return true;
        }

        [Input("obj", "The object to compare with.")]
        public override bool Equals(object obj)
        {
            return obj is ExtensionMethodKey && Equals((ExtensionMethodKey)obj);
        }

        public override int GetHashCode()
        {
            return m_Hash;
        }

        private static int Hash(string methodName, Type[] types)
        {
            int hash = methodName.GetHashCode();
            for (int i = 0; i < types.Length; i++)
                hash = hash * 31 + (types[i]?.GetHashCode() ?? 0);
            return hash;
        }

        private readonly string m_MethodName;
        private readonly Type[] m_Types;
        private readonly int m_Hash;
    }
}
