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

using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.Engine.Geometry;
using BH.oM.Physical.Elements;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [ToBeRemoved("3.2", "Was used for incode purposes of defaulting elements, a task which is now achived by providing a ICurve to the SetOutlineElement1D.")]
        [Description("Creates a valid IElement1D which can be assigned to the ISurface.")]
        [Input("surface", "The 2-dimensional element which a corresponding valid IElement1D is to be gotten from.")]
        [Input("curve", "The geometrical location of the new IElement1D as a ICurve.")]
        [Output("element1D", "a valid IElement1D of a type which can be assigned to the ISurface.")]
        public static IElement1D NewElement1D(this oM.Physical.Elements.ISurface surface, ICurve curve)
        {
            return curve.DeepClone();
        }

        /***************************************************/

        [ToBeRemoved("3.2", "Was used for incode purposes of defaulting elements, a task which is now achived by providing a ICurve to the SetOutlineElement1D.")]
        [Description("Creates a valid IElement1D which can be assigned to the IOpening.")]
        [Input("surface", "The 2-dimensional element which a corresponding valid IElement1D is to be gotten from.")]
        [Input("curve", "The geometrical location of the new IElement1D as a ICurve.")]
        [Output("element1D", "a valid IElement1D of a type which can be assigned to the IOpening.")]
        public static IElement1D NewElement1D(this IOpening surface, ICurve curve)
        {
            return curve.DeepClone();
        }

        /***************************************************/
        
    }
}




