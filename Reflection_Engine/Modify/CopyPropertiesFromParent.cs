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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Copy the properties from an object of a parent Type to an object of a child Type.")]
        [Input("childObject", "Object whose properties in common with `parentObject` will be copied from there.")]
        [Input("parentObject", "Object of a Type that is a super (parent) type of the type of childObject.")]
        public static void CopyPropertiesFromParent<P, C>(this C childObject, P parentObject) where C : IObject, P
        {
            if (childObject == null || parentObject == null)
                return;

            Type p = typeof(P);
            Type c = typeof(C);

            var parentProps = p.GetProperties();
            var childProps = c.GetProperties();

            foreach (var childProp in childProps)
            {
                var correspondingParentProp = parentProps.FirstOrDefault(pp => pp.Name == childProp.Name);
                if (correspondingParentProp == null)
                    continue;

                var correspondingParentPropValue = correspondingParentProp.GetValue(parentObject);
                childProp.SetValue(childObject, correspondingParentPropValue);
            }
        }

        /***************************************************/
    }
}






