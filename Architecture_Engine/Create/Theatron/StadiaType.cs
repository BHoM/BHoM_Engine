/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Architecture.Theatron;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create an enumerator for predfined stadia types.")]
        [Input("typeNum", "0 = EightArc, 1 = Orthogonal, 2 = NoCorners, 3 = Circular, 4 = FourArc, 5 = Oblong, 6 = undefined")]
        public static StadiaType StadiaTypeEnum(int typeNum)
        {
            var enumCount = StadiaType.GetNames(typeof(StadiaType)).Length;

            //last name is undefined so total possible types is enumCount-2
            if (typeNum > enumCount-2) typeNum = 0;

            StadiaType value = (StadiaType)typeNum;
            
            return value;
        }

        /***************************************************/
    }
}
