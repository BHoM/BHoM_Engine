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

using BH.oM.Structure.Constraints;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LinkConstraint LinkConstraint(string name, List<bool> fixity)
        {
            return new LinkConstraint
            {
                XtoX = fixity[0],
                YtoY = fixity[1],
                ZtoZ = fixity[2],
                XtoYY = fixity[3],
                XtoZZ = fixity[4],
                YtoXX = fixity[5],
                YtoZZ = fixity[6],
                ZtoXX = fixity[7],
                ZtoYY = fixity[8],
                XXtoXX = fixity[9],
                YYtoYY = fixity[10],
                ZZtoZZ = fixity[11],
                Name = name
            };
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintFixed(string name = "Fixed")
        {
            return new LinkConstraint
            {
                XtoX   = true,
                YtoY   = true,
                ZtoZ   = true,
                XtoYY  = true,
                XtoZZ  = true,
                YtoXX  = true,
                YtoZZ  = true,
                ZtoXX  = true,
                ZtoYY  = true,
                XXtoXX = true,
                YYtoYY = true,
                ZZtoZZ = true,
                Name = name
            };
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintPinned(string name = "Pinned")
        {

            bool[] fixities = new bool[12];

            for (int i = 0; i < 9; i++)
            {
                fixities[i] = true;
            }

            LinkConstraint constr = LinkConstraint(name, fixities.ToList());
            return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintXYPlane(string name = "xy-Plane")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.XtoX = true;
                constr.XtoZZ = true;
                constr.YtoY = true;
                constr.YtoZZ = true;
                constr.ZZtoZZ = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintYZPlane(string name = "yz-Plane")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.YtoY = true;
                constr.YtoXX = true;
                constr.ZtoZ = true;
                constr.ZtoXX = true;
                constr.XXtoXX = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintZXPlane(string name = "zx-Plane")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.XtoX = true;
                constr.XtoYY = true;
                constr.ZtoZ = true;
                constr.ZtoYY = true;
                constr.YYtoYY = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintXYPlanePin(string name = "xy-Plane Pin")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.XtoX = true;
                constr.XtoZZ = true;
                constr.YtoY = true;
                constr.YtoZZ = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintYZPlanePin(string name = "yz-Plane Pin")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.YtoY = true;
                constr.YtoXX = true;
                constr.ZtoZ = true;
                constr.ZtoXX = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintZXPlanePin(string name = "zx-Plane Pin")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.XtoX = true;
                constr.XtoYY = true;
                constr.ZtoZ = true;
                constr.ZtoYY = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintXPlate(string name = "x-Plate")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.XtoX = true;
                constr.XtoYY = true;
                constr.XtoZZ = true;
                constr.YYtoYY = true;
                constr.ZZtoZZ = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintYPlate(string name = "y-Plate")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.YtoY = true;
                constr.YtoXX = true;
                constr.YtoZZ = true;
                constr.XXtoXX = true;
                constr.ZZtoZZ = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintYPlateZPlate(string name = "z-Plate")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.ZtoZ = true;
                constr.ZtoXX = true;
                constr.ZtoYY = true;
                constr.XXtoXX = true;
                constr.YYtoYY = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintXPlatePin(string name = "x-Plate Pin")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.XtoX = true;
                constr.XtoYY = true;
                constr.XtoZZ = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintYPlatePin(string name = "y-Plate Pin")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.YtoY = true;
                constr.YtoXX = true;
                constr.YtoZZ = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/

        public static LinkConstraint LinkConstraintZPlatePin(string name = "z-Plate Pin")
        {
                LinkConstraint constr = new LinkConstraint();
                constr.ZtoZ = true;
                constr.ZtoXX = true;
                constr.ZtoYY = true;
                constr.Name = name;
                return constr;
        }

        /***************************************************/
    }
}

