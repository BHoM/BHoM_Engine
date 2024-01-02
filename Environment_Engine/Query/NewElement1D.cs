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

using BH.oM.Environment.Elements;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [ToBeRemoved("3.2", "Was used for incode purposes of defaulting elements, a task which is now achived by providing a ICurve to the SetOutlineElement1D.")]
        [Description("Returns an instance of an IElement1D from the given object")]
        [Input("opening", "An Environmental Opening object")]
        [Input("curve", "The curve to clone")]
        [Output("element", "A 1D element object")]
        public static IElement1D NewElement1D(this Opening opening, ICurve curve)
        {
            return curve.DeepClone();
        }

        /***************************************************/

        [ToBeRemoved("3.2", "Was used for incode purposes of defaulting elements, a task which is now achived by providing a ICurve to the SetOutlineElement1D.")]
        [Description("Returns an instance of an IElement1D from the given object")]
        [Input("panel", "An Environmental Panel object")]
        [Input("curve", "The curve to clone")]
        [Output("element", "A 1D element object")]
        public static IElement1D NewElement1D(this Panel panel, ICurve curve)
        {
            return curve.DeepClone();
        }

        /***************************************************/

        [ToBeRemoved("3.2", "Was used for incode purposes of defaulting elements, a task which is now achived by providing a ICurve to the SetOutlineElement1D.")]
        [Description("Returns an instance of an IElement1D from the given object")]
        [Input("space", "An Environment Space object")]
        [Input("curve", "The curve to clone")]
        [Output("element", "A 1D element object")]
        public static IElement1D NewElement1D(this Space space, ICurve curve)
        {
            return curve.DeepClone();
        }
    }
}





