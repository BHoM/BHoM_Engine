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

using BH.Engine.Reflection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BH.Tests.Engine.Reflection.Query
{
    public class GenericConstraintTests
    {
        /***************************************************/
        /**** Tests: GenericTypeConstraint              ****/
        /***************************************************/

        [TestCase(nameof(Samples.Unconstrained))]
        [TestCase(nameof(Samples.Struct))]
        [TestCase(nameof(Samples.StructAndInterface))]
        [TestCase(nameof(Samples.Unmanaged))]
        [TestCase(nameof(Samples.ClassAndNew))]
        [TestCase(nameof(Samples.SelfReferencing))]
        [Description("The type returned for the generic parameter must satisfy its constraints, which means the generic method can be made from it.")]
        public void GenericTypeConstraintSatisfiesConstraints(string methodName)
        {
            MethodInfo method = Method(methodName);
            Type[] types = method.GetGenericArguments().Select(x => x.GenericTypeConstraint()).ToArray();

            Assert.DoesNotThrow(() => method.MakeGenericMethod(types));
        }

        /***************************************************/

        [Test]
        public void GenericTypeConstraintUnconstrainedIsObject()
        {
            Assert.AreEqual(typeof(object), Parameter(nameof(Samples.Unconstrained)).GenericTypeConstraint());
        }

        /***************************************************/

        [Test]
        public void GenericTypeConstraintStructIsValueType()
        {
            Assert.IsTrue(Parameter(nameof(Samples.Struct)).GenericTypeConstraint().IsValueType);
        }

        /***************************************************/

        [Test]
        public void GenericTypeConstraintSelfReferencingUsesObject()
        {
            Assert.AreEqual(typeof(IComparable<object>), Parameter(nameof(Samples.SelfReferencing)).GenericTypeConstraint());
        }

        /***************************************************/

        [Test]
        [Description("Constraint that is itself another generic parameter: resolved through that parameter's own constraint.")]
        public void GenericTypeConstraintOtherParameterIsResolved()
        {
            Type constraint = null;
            Assert.DoesNotThrow(() => constraint = Parameter(nameof(Samples.ConstrainedToParameter)).GenericTypeConstraint());
            Assert.AreEqual(typeof(object), constraint);
        }

        /***************************************************/

        [Test]
        [Description("Constraint that is a generic type built on the parameter itself, nested in another generic type.")]
        public void GenericTypeConstraintNestedSelfReferenceIsResolved()
        {
            Type constraint = null;
            Assert.DoesNotThrow(() => constraint = Parameter(nameof(Samples.NestedSelfReferencing)).GenericTypeConstraint());
            Assert.AreEqual(typeof(Samples.IContainer<List<object>>), constraint);
        }

        /***************************************************/
        /**** Tests: MakeFromGeneric                    ****/
        /***************************************************/

        [TestCase(nameof(Samples.Unconstrained))]
        [TestCase(nameof(Samples.Struct))]
        [TestCase(nameof(Samples.StructAndInterface))]
        [TestCase(nameof(Samples.Unmanaged))]
        [TestCase(nameof(Samples.ClassAndNew))]
        [TestCase(nameof(Samples.SelfReferencing))]
        [Description("The type made from the generic parameter must satisfy its constraints, which means the generic method can be made from it.")]
        public void MakeFromGenericTypeSatisfiesConstraints(string methodName)
        {
            MethodInfo method = Method(methodName);
            Type[] types = method.GetGenericArguments().Select(x => x.MakeFromGeneric()).ToArray();

            Assert.DoesNotThrow(() => method.MakeGenericMethod(types));
        }

        /***************************************************/

        [Test]
        public void MakeFromGenericTypeAgreesWithGenericTypeConstraint()
        {
            foreach (string name in new string[] { nameof(Samples.Unconstrained), nameof(Samples.Struct), nameof(Samples.StructAndInterface), nameof(Samples.Unmanaged), nameof(Samples.ClassAndNew), nameof(Samples.SelfReferencing) })
            {
                Type parameter = Parameter(name);
                Assert.AreEqual(parameter.GenericTypeConstraint(), parameter.MakeFromGeneric(), name);
            }
        }

        /***************************************************/

        [Test]
        public void MakeFromGenericTypeConstructedType()
        {
            Type listOfT = typeof(Samples).GetMethod(nameof(Samples.ListOfParameter)).ReturnType;
            Assert.AreEqual(typeof(List<object>), listOfT.MakeFromGeneric());
        }

        /***************************************************/

        [Test]
        public void MakeFromGenericTypeNonGenericIsUnchanged()
        {
            Assert.AreEqual(typeof(string), typeof(string).MakeFromGeneric());
        }

        /***************************************************/
        /**** Tests: method level                       ****/
        /***************************************************/

        [TestCase(nameof(Samples.Struct))]
        [TestCase(nameof(Samples.StructAndInterface))]
        [TestCase(nameof(Samples.Unmanaged))]
        [TestCase(nameof(Samples.ClassAndNew))]
        [TestCase(nameof(Samples.SelfReferencing))]
        [Description("ParametersWithConstraints must not throw, and must return parameters that are not generic any more.")]
        public void ParametersWithConstraintsAreConcrete(string methodName)
        {
            ParameterInfo[] parameters = null;
            Assert.DoesNotThrow(() => parameters = Method(methodName).ParametersWithConstraints());
            Assert.IsTrue(parameters.All(x => !x.ParameterType.ContainsGenericParameters));
        }

        /***************************************************/

        [Test]
        [Description("Constraints that no type found can satisfy: the parameters of the generic method are returned and nothing is thrown.")]
        public void ParametersWithConstraintsUnsatisfiableFallsBackToGenericMethod()
        {
            ParameterInfo[] parameters = null;
            Assert.DoesNotThrow(() => parameters = Method(nameof(Samples.Unsatisfiable)).ParametersWithConstraints());
            Assert.AreEqual(1, parameters.Length);
            Assert.IsTrue(parameters[0].ParameterType.IsGenericParameter);
        }

        /***************************************************/

        [Test]
        public void MakeFromGenericMethodConcrete()
        {
            MethodInfo made = Method(nameof(Samples.Struct)).MakeFromGeneric();
            Assert.IsFalse(made.ContainsGenericParameters);
        }

        /***************************************************/

        [Test]
        [Description("Constraints that no type found can satisfy: the generic method is returned unchanged and nothing is thrown.")]
        public void MakeFromGenericMethodUnsatisfiableIsUnchanged()
        {
            MethodInfo method = Method(nameof(Samples.Unsatisfiable));
            MethodInfo made = null;
            Assert.DoesNotThrow(() => made = method.MakeFromGeneric());
            Assert.AreEqual(method, made);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static MethodInfo Method(string name)
        {
            return typeof(Samples).GetMethod(name);
        }

        /***************************************************/

        // The generic parameter that is the first generic argument of the method
        private static Type Parameter(string methodName)
        {
            return Method(methodName).GetGenericArguments()[0];
        }

        /***************************************************/
        /**** Sample methods                            ****/
        /***************************************************/

        public static class Samples
        {
            public interface IContainer<X> { }
            public class Base { }
            public interface IMarker { }

            public static void Unconstrained<T>(T value) { }

            public static void Struct<T>(T value) where T : struct { }

            public static void StructAndInterface<T>(T value) where T : struct, IComparable<T> { }

            public static void Unmanaged<T>(T value) where T : unmanaged { }

            public static void ClassAndNew<T>(T value) where T : class, new() { }

            public static void SelfReferencing<T>(T value) where T : IComparable<T> { }

            public static void NestedSelfReferencing<T>(T value) where T : IContainer<List<T>> { }

            public static void ConstrainedToParameter<T, U>(T value, U other) where T : U { }

            public static List<T> ListOfParameter<T>(T value) { return null; }

            public static void Unsatisfiable<T>(T value) where T : Base, IMarker { }
        }

        /***************************************************/
    }
}

