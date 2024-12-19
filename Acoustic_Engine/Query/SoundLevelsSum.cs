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

using BH.oM.Acoustic;
using System;
using System.Collections.Generic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double SoundLevelsSum(this List<double> spl)
        {
            double SPL = 0;
            for (int i = 0; i < spl.Count; i++)
                SPL = (10 * Math.Log10(Math.Pow(10, SPL / 10) + Math.Pow(10, spl[i] / 10)));
            return SPL;
        }
        
        /***************************************************/

        public static SoundLevel SoundLevelsSum(this List<SoundLevel> spl)
        {
            SoundLevel totalLevel = new SoundLevel();
            for (int i = 0; i < spl.Count; i++)
                totalLevel += spl[i];
            return totalLevel;
        }

        /***************************************************/
    }
}






