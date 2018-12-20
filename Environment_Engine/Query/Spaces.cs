/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System.Linq;
using System.Collections.Generic;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Space> Spaces(this List<IBHoMObject> bhomObjects)
        {
            List<Space> spaces = new List<Space>();

            foreach(IBHoMObject obj in bhomObjects)
            {
                if (obj is Space)
                    spaces.Add(obj as Space);
            }

            return spaces;
        }

        /***************************************************/

        public static List<Space> Spaces(this List<List<BuildingElement>> besAsSpace)
        {
            List<Space> spaces = new List<Space>();

            for (int x = 0; x < besAsSpace.Count; x++)
                spaces.Add(besAsSpace[x].Space(x.ToString(), x.ToString()));

            return spaces;
        }

        /***************************************************/

        public static Space Space(this List<BuildingElement> space)
        {
            Point spaceCentre = space.SpaceCentre();
            string xName = spaceCentre.X.ToString().Length > 3 ? spaceCentre.X.ToString().Substring(0, 3) : spaceCentre.X.ToString();
            string yName = spaceCentre.Y.ToString().Length > 3 ? spaceCentre.Y.ToString().Substring(0, 3) : spaceCentre.Y.ToString();
            string zName = spaceCentre.Z.ToString().Length > 3 ? spaceCentre.Z.ToString().Substring(0, 3) : spaceCentre.Z.ToString();
            string spaceName = xName + "-" + yName + "-" + zName;
            return Create.Space(spaceName, spaceName, space.SpaceCentre());
        }

        /***************************************************/

        public static Space Space(this List<BuildingElement> space, string spaceNumber, string spaceName)
        {
            return Create.Space(spaceName, spaceNumber, space.SpaceCentre());
        }

        /***************************************************/

        public static Space Space(this List<BuildingElement> space, int spaceNumber, string spaceName)
        {
            return Space(space, spaceNumber.ToString(), spaceName);
        }

        /***************************************************/

        public static List<List<BuildingElement>> SpacesNotClosed(this List<List<BuildingElement>> spaces)
        {
            List<List<BuildingElement>> spacesNotClosed = new List<List<BuildingElement>>();

            foreach(List<BuildingElement> space in spaces)
            {
                if (!space.IsClosed())
                    spacesNotClosed.Add(space);
            }

            return spacesNotClosed;
        }

        /***************************************************/

        public static List<List<BuildingElement>> ClosedSpaces(this List<List<BuildingElement>> spaces)
        {
            List<List<BuildingElement>> closedSpaces = new List<List<BuildingElement>>();

            foreach(List<BuildingElement> space in spaces)
            {
                if (space.IsClosed())
                    closedSpaces.Add(space);
            }

            return closedSpaces;
        }

        /***************************************************/

       

        /***************************************************/

        public static List<List<BuildingElement>> JoinSpaces(this List<List<BuildingElement>> spaces, List<List<BuildingElement>> spacesExtra)
        {
            spaces.AddRange(spacesExtra);
            return spaces;
        }

        /***************************************************/

        public static List<List<BuildingElement>> AddSpaces(this List<List<BuildingElement>> spaces, List<BuildingElement> space)
        {
            spaces.Add(space);
            return spaces;
        }

        public static List<string> UniqueSpaceNames(this List<BuildingElement> elements)
        {
            List<string> spaceNames = new List<string>();

            foreach (BuildingElement be in elements)
            {
                if (be.CustomData.ContainsKey("SpaceID") && be.CustomData["SpaceID"] != null)
                    spaceNames.Add(be.CustomData["SpaceID"].ToString());
                if(be.CustomData.ContainsKey("AdjacentSpaceID") && be.CustomData["AdjacentSpaceID"] != null)
                    spaceNames.Add(be.CustomData["AdjacentSpaceID"].ToString());
            }

            return spaceNames.Where(x => !x.Equals("-1")).Distinct().ToList();
        }
    }
}
