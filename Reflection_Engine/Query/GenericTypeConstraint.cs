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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        [Description("Obtains the type constraint for the provided generic type.")]
        [Input("type", "The generic type to obtain a type constraint for.")]
        [Output("constraint", "The generic type constraint for the provided type.")]
        public static Type GenericTypeConstraint(this Type type)
        {
            if(type == null)
            {
                Base.Compute.RecordError("Cannot query the generic type constraints when the type is null.");
                return null;
            }

            return ResolveGeneric(type, new Dictionary<Type, Type>());
        }

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/

        // Replaces all the generic parameters found in the type by a concrete type that satisfies their constraints (the first one if there are several).
        // 'resolving' holds the generic parameters currently being resolved, each mapped to the type to use if it is met again inside its own constraint.
        // It is read-only here: ResolveConstraint is the only place that adds to it, in a copy. See there for why.
        private static Type ResolveGeneric(Type type, IReadOnlyDictionary<Type, Type> resolving)
        {
            if (!type.ContainsGenericParameters)
                return type;

            if (type.IsGenericParameter)
            {
                if (resolving.TryGetValue(type, out Type known))
                    return known;

                // `where T : struct` is not listed in the constraints (only System.ValueType is), and System.ValueType itself does not satisfy it
                if ((type.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
                    return StructConstraint(type, resolving);

                Type constraint = type.GetGenericParameterConstraints().FirstOrDefault();
                if (constraint == null)
                    return typeof(object);

                return ResolveConstraint(type, constraint, typeof(object), resolving);
            }

            if (type.HasElementType)    // Arrays, references, pointers
            {
                Type element = ResolveGeneric(type.GetElementType(), resolving);
                if (type.IsArray)
                    return type.GetArrayRank() == 1 && type == type.GetElementType().MakeArrayType() ? element.MakeArrayType() : element.MakeArrayType(type.GetArrayRank());
                else if (type.IsByRef)
                    return element.MakeByRefType();
                else
                    return element.MakePointerType();
            }

            Type[] arguments = type.GetGenericArguments().Select(x => ResolveGeneric(x, resolving)).ToArray();
            return type.GetGenericTypeDefinition().MakeGenericType(arguments);
        }

        /*******************************************/

        // Resolves the 'constraint' of the generic 'parameter' into a concrete type.
        //
        // What: the constraint can mention the parameter it constrains, directly or nested (T : IComparable<T>, T : IContainer<List<T>>).
        // Resolving that inner 'parameter' means resolving its constraint again, which mentions it again, and so on without end.
        // To stop that, 'parameter' is mapped to 'replacement' while the constraint is being resolved:
        // ResolveGeneric returns the replacement as soon as it meets 'parameter', instead of looking at its constraint again.
        //
        // The replacement depends on the caller:
        // - ordinary parameter: object, only a placeholder to break the loop (T : IComparable<T> gives IComparable<object>)
        // - struct parameter: a candidate value type being tried (the caller then checks that the candidate satisfies the constraint)
        //
        private static Type ResolveConstraint(Type parameter, Type constraint, Type replacement, IReadOnlyDictionary<Type, Type> resolving)
        {
            Dictionary<Type, Type> inner = resolving.ToDictionary(x => x.Key, x => x.Value);
            inner[parameter] = replacement;
            return ResolveGeneric(constraint, inner);
        }

        /*******************************************/

        // Finds a value type that satisfies the struct constraint and all the other constraints of the generic parameter
        private static Type StructConstraint(Type type, IReadOnlyDictionary<Type, Type> resolving)
        {
            Type[] otherConstraints = type.GetGenericParameterConstraints().Where(x => x != typeof(ValueType)).ToArray();

            foreach (Type candidate in m_StructCandidates)
            {
                try
                {
                    if (otherConstraints.All(x => ResolveConstraint(type, x, candidate, resolving).IsAssignableFrom(candidate)))
                        return candidate;
                }
                catch (ArgumentException)
                {
                    // This candidate cannot be used to build the constraint, try the next one
                }
            }

            return typeof(double);   // No better option: the callers guard the construction of the generic method
        }

        /*******************************************/
        /**** Private Fields                    ****/
        /*******************************************/

        private static readonly Type[] m_StructCandidates = new Type[] { typeof(double), typeof(int), typeof(bool), typeof(DateTime), typeof(Guid), typeof(long), typeof(char) };

        /*******************************************/
    }
}

