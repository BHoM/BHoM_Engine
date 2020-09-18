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

using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Environment;
using BH.oM.Geometry;

using BH.oM.Physical.Constructions;

using System.Collections.Generic;
using System.Linq;
using BH.oM.Environment.Fragments;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.Engine.Base;
using System;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Panel> SetPanelConstructionByType(this List<Panel> panels, IConstruction newConstruction, PanelType panelType)

        {
            List<Panel> clones = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());
# modifiedPanels=List<clones>
            string constructionType="Wall"
                Console.WriteLine("Which Construction type do you want?")
                constructionType = Console.ReadLine()

                foreach string in modifiedPanels
                {
                if string = constructionType
                    bool true
                else bool false
                    }
                

        


                    /*List all the objects being passed to the method
Filter out all objects which don't match the specified Type (and store these for later)
For the remaining objects, assign the given Construction to them
Join the two lists back together and return them.*/
                    return clones;
        }
    }
}

