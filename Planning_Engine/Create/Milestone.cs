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

using BH.oM.Planning;
using System;

namespace BH.Engine.Planning
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Milestone Milestone(string name, DateTimeOffset dueOn, ItemState state)
        {
            return new Milestone
            {
                Name = name,
                DueOn = dueOn,
                State = state
            };
        }

        /***************************************************/

        public static Milestone Milestone(string name, int year, int month, int day, string description = "", ItemState state = ItemState.Open)
        {
            DateTimeOffset dueOn = new DateTimeOffset(year, month, day, 23, 59, 59, TimeSpan.Zero);

            return new Milestone
            {
                Name = name,
                DueOn = dueOn,
                Description = description,
                State = state
            };

        }

        /***************************************************/

    }
}






