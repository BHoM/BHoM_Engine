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

using BH.Engine.Reflection;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base.Attributes;
using MongoDB.Bson;
using System.Text.RegularExpressions;
using BH.oM.Versioning;
using ICSharpCode.Decompiler.CSharp.Syntax;
using BH.Engine.Versioning.Objects;

namespace BH.Engine.Versioning
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Provide a string representation of a method as it used for versioning by the PreviousVersion attribute.")]
        [Input("declaringType", "Type in which the method is declared. You can use just the name of the type or include a (part of the) namespace in front of it.")]
        [Input("methodName", "Name of the method. It has to be the exact string. If the method is a constructor, you can leave this input blank.")]
        [Output("keys", "String representation for each method that matches the input filters.")]
        public static void LoadVersioningAssemblies(string folder = "")
        {
            List<Assembly> result = new List<Assembly>();
            if (string.IsNullOrEmpty(folder))
                folder = BH.Engine.Base.Query.BHoMFolderUpgrades();

            List<Assembly> assemblies = BH.Engine.Base.Compute.LoadAllAssemblies(folder, "", true);

            foreach(Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                
                foreach (Type type in types)
                {
                    // ToNew upgraders
                    UpgraderAttribute upgradeAttribute = type.GetCustomAttribute<UpgraderAttribute>();
                    if (upgradeAttribute != null)
                    {
                        string version = upgradeAttribute.MajorVersion + "." + upgradeAttribute.MinorVersion;
                        if (!Global.CustomUpgrades.ContainsKey(version))
                            Global.CustomUpgrades[version] = new Dictionary<string, CustomVersioningMethod>();
                        Dictionary<string, CustomVersioningMethod> container = Global.CustomUpgrades[version];

                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                        {
                            foreach (VersioningTargetAttribute targetAttribute in method.GetCustomAttributes<VersioningTargetAttribute>())
                                container[targetAttribute.Target] = Delegate.CreateDelegate(typeof(CustomVersioningMethod), method) as CustomVersioningMethod;
                        }
                    }

                    // ToOld downgraders
                    DowngraderAttribute downgradeAttribute = type.GetCustomAttribute<DowngraderAttribute>();
                    if (downgradeAttribute != null)
                    {
                        string version = downgradeAttribute.MajorVersion + "." + downgradeAttribute.MinorVersion;
                        if (!Global.CustomUpgrades.ContainsKey(version))
                            Global.CustomUpgrades[version] = new Dictionary<string, CustomVersioningMethod>();
                        Dictionary<string, CustomVersioningMethod> container = Global.CustomDowngrades[version];

                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                        {
                            foreach (VersioningTargetAttribute targetAttribute in method.GetCustomAttributes<VersioningTargetAttribute>())
                                container[targetAttribute.Target] = Delegate.CreateDelegate(typeof(CustomVersioningMethod), method) as CustomVersioningMethod;
                        }
                    }
                }
            }
            
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/


        /***************************************************/
    }
}





