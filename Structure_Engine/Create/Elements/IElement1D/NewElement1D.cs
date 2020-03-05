/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Dimensional;
using BH.oM.Common;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a new Element1D, appropriate to the input type. For this case the appropriate type for the Opening will be a new Edge, in the position provided. \n" +
                     "Method required for any IElement2D.")]
        [Input("opening", "Opening just used to determain the appropriate type of IElement1D to create.")]
        [Input("curve", "The position of the new IElement1D, i.e. the position of the returned Edge.")]
        [Output("edge", "The created Edge in the position provided as a IElement1D.")]
        public static IElement1D NewElement1D(this Opening opening, ICurve curve)
        {
            return new Edge { Curve = curve.IClone() };
        }

        /***************************************************/

        [Description("Creates a new Element1D, appropriate to the input type. For this case the appropriate type for the Panel will be a new Edge, in the position provided. \n" +
                     "Method required for any IElement2D.")]
        [Input("panel", "Panel just used to determain the appropriate type of IElement1D to create.")]
        [Input("curve", "The position of the new IElement1D, i.e. the position of the returned Edge.")]
        [Output("edge", "The created Edge in the position provided as a IElement1D.")]
        public static IElement1D NewElement1D(this Panel panel, ICurve curve)
        {
            return new Edge { Curve = curve.IClone() };
        }

        /***************************************************/
    }
}

