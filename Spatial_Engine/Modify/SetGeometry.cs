/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Spatial.SettingOut;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        [Description("Modifies the geometry of a IElement0D to be the provided point's. The IElement0Ds other properties are unaffected.")]
        [Input("element0D", "The IElement0D to modify the geometry of.")]
        [Input("point", "The new point geometry for the IElement0D.")]
        [Output("element0D", "A IElement0D with the properties of 'element0D' and the location of 'point'.")]
        public static IElement0D ISetGeometry(this IElement0D element0D, Point point)
        {
            return Base.Compute.RunExtensionMethod(element0D, "SetGeometry", new object[] { point }) as IElement0D;
        }

        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Modifies the geometry of a IElement1D to be the provided curve. The IElement1Ds other properties are unaffected.")]
        [Input("element1D", "The IElement1D to modify the geomerty of.")]
        [Input("curve", "The new geometry curve for the IElement1D.")]
        [Output("element1D", "A IElement1D with the properties of 'element1D' and the location of 'curve'.")]
        public static IElement1D ISetGeometry(this IElement1D element1D, ICurve curve)
        {
            return Base.Compute.RunExtensionMethod(element1D, "SetGeometry", new object[] { curve }) as IElement1D;
        }

        /******************************************/

        [Description("Modifies the geometry of a Grid to be the provided curve.")]
        [Input("grid", "The Grid to modify the geomerty of.")]
        [Input("curve", "The new geometry curve for the Grid.")]
        [Output("Grid", "A Grid with the curve updated.")]
        public static Grid SetGeometry(this Grid grid, ICurve curve)
        {
            Grid clone = grid.ShallowClone();
            clone.Curve = curve.DeepClone();
            return clone;
        }

        /******************************************/
    }
}



