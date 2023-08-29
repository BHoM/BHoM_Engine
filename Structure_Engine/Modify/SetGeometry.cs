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

using BH.Engine.Base;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Spatial.SettingOut;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Elements;
using BH.Engine.Geometry;
using BH.Engine.Structure;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Modifies the geometry of a Pile to be the provided curve. The Piles other properties are unaffected.")]
        [Input("pile", "The Pile to modify the geomerty of.")]
        [Input("curve", "The new geometry curve for the Pile. Note a Pile is defined by the TopNode and BottomNode, the start and the end of the curve will be used.")]
        [Output("element1D", "A IElement1D with the properties of 'element1D' and the location of 'curve'.")]
        public static Pile SetGeometry(this Pile pile, ICurve curve)
        {
            if (pile.IsNull())
                return null;

            Pile clone = pile.DeepClone();
            clone.TopNode.Position = curve.IStartPoint();
            clone.BottomNode.Position = curve.IEndPoint();

            return clone;
        }

        /******************************************/
    }
}




