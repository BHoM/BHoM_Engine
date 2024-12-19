/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Elements;
using NUnit.Framework;

namespace BH.Tests.Engine.Base.Compute
{
    public class TryRunExtensionMethodTests
    {
        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void BarDoubleDoubleDouble()
        {
            Bar bar = new Bar();
            double dbl = 123;
            string expected = "BH.Engine.TestHelper.Compute.ExtensionMethodToCallHelper(BH.oM.Structure.Elements.Bar, System.Double, System.Double, System.Double)";
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(bar, dbl, dbl, dbl);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void BarBarDoubleDouble()
        {
            Bar bar = new Bar();
            double dbl = 123;
            string expected = "BH.Engine.TestHelper.Compute.ExtensionMethodToCallHelper(BH.oM.Structure.Elements.Bar, BH.oM.Structure.Elements.Bar, System.Double, System.Double)";
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(bar, bar, dbl, dbl);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void BarBarBarDouble()
        {
            Bar bar = new Bar();
            double dbl = 123;
            string expected = "BH.Engine.TestHelper.Compute.ExtensionMethodToCallHelper(BH.oM.Structure.Elements.Bar, System.Object, System.Object, System.Object)";
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(bar, bar, bar, dbl);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void BarBarBarBar()
        {
            Bar bar = new Bar();
            string expected = "BH.Engine.TestHelper.Compute.ExtensionMethodToCallHelper(BH.oM.Structure.Elements.Bar, System.Object, System.Object, System.Object)";
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(bar, bar, bar, bar);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void BarBarPanelDouble()
        {
            Bar bar = new Bar();
            Panel panel = new Panel();
            double dbl = 123;
            string expected = "BH.Engine.TestHelper.Compute.ExtensionMethodToCallHelper(BH.oM.Structure.Elements.Bar, System.Object, BH.oM.Structure.Elements.Panel, System.Object)";
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(bar, bar, panel, dbl);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void PanelDoubleDoubleDouble()
        {
            Panel panel = new Panel();
            double dbl = 123;
            string expected = "BH.Engine.TestHelper.Compute.ExtensionMethodToCallHelper(BH.oM.Dimensional.IElement2D, System.Double, System.Double, System.Double)";
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(panel, dbl, dbl, dbl);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void BarNullPanelNull()
        {
            Bar bar = new Bar();
            Panel panel = new Panel();
            string expected = "BH.Engine.TestHelper.Compute.ExtensionMethodToCallHelper(BH.oM.Structure.Elements.Bar, System.Object, BH.oM.Structure.Elements.Panel, System.Object)";
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(bar, null, panel, null);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void PanelPanelPanelDouble()
        {
            Panel panel = new Panel();
            double dbl = 123;
            string expected = null;
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(panel, panel, panel, dbl);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void BarDoubleNullNull()
        {
            Bar bar = new Bar();
            double dbl = 123;
            string expected = null;
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(bar, dbl, null, null);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void BarNullNullBar()
        {
            Bar bar = new Bar();
            string expected = null;
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(bar, null, null, bar);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void BarNullNullNull()
        {
            Bar bar = new Bar();
            string expected = null;
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(bar, null, null, null);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void NullDoubleDoubleDouble()
        {
            Bar bar = new Bar();
            double dbl = 123;
            string expected = null;
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(null, dbl, dbl, dbl);
            Assert.AreEqual(result, expected);
        }

        [Test]
        [Description("Tests Compute.TryRunExtensionMethod by calling IExtensionMethodToCallHelper interface method with objects of types provided in name of this method." +
                     "\n Underlying ExtensionMethodToCallHelper method to which the call is dispatched should return the value set in 'expected' variable.")]
        public void NullNullNullNull()
        {
            Bar bar = new Bar();
            string expected = null;
            string result = BH.Engine.TestHelper.Compute.IExtensionMethodToCallHelper(null, null, null, null);
            Assert.AreEqual(result, expected);
        }
    }
}
