/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Versioning;
using BH.oM.Dimensional;
using BH.oM.Structure.Elements;
using System.Reflection;

namespace BH.Engine.TestHelper
{
    public static partial class Compute
    {
        public static string ExtensionMethodToCallHelper(this Bar a, double b, double c, double d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper(this Bar a, Bar b, double c, double d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper(this Bar a, object b, double c, double d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper(this Bar a, object b, object c, object d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper(this Bar a, object b, Panel c, object d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper(this IElement2D a, double b, double c, double d)
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string ExtensionMethodToCallHelper()
        {
            return MethodBase.GetCurrentMethod().VersioningKey();
        }

        public static string IExtensionMethodToCallHelper(this object a, object b, object c, object d)
        {
            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(a, nameof(ExtensionMethodToCallHelper), new object[] { b, c, d }, out result))
            {
                BH.Engine.Base.Compute.RecordError("Extension method not found.");
                return null;
            }
            else
                return (string)result;
        }
    }
}
