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

using BH.oM.Geometry;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        public static ILoad Load(LoadType type, Loadcase loadCase, List<double> magnitude, string groupName, LoadAxis axis, bool isProjected, string units = "kN")
        {
            units = units.ToUpper();
            units = units.Replace(" ", "");

            double sFac = 1;

            switch (units)
            {
                case "N":
                case "NEWTON":
                case "NEWTONS":
                    sFac = 1;
                    break;
                case "KN":
                case "KILONEWTON":
                case "KILONEWTONS":
                    sFac = 1000;
                    break;
                case "C":
                case "CELSIUS":
                case "K":
                case "KELVIN":
                    sFac = 1;
                    break;
                default:
                    throw new ArgumentException("Unrecognised unit type");
            }

            Vector force = null;
            Vector moment = null;
            double mag;

            if (magnitude.Count < 1)
                throw new ArgumentException("Need at least one maginute value");

            mag = magnitude[0] * sFac;

            if (magnitude.Count > 2)
                force = new Vector() { X = magnitude[0], Y = magnitude[1], Z = magnitude[2] } * sFac;

            if (magnitude.Count > 5)
                moment = new Vector() { X = magnitude[3], Y = magnitude[4], Z = magnitude[5] } * sFac;

            switch (type)
            {
                case LoadType.Selfweight:
                    {
                        BHoMGroup<BHoMObject> group = new BHoMGroup<BHoMObject>() { Name = groupName };
                        return GravityLoad(loadCase, force, group, groupName);
                    }
                case LoadType.PointForce:
                    {
                        BHoMGroup<Node> group = new BHoMGroup<Node>() { Name = groupName };
                        return PointForce(loadCase, group, force, moment, axis, groupName);
                    }
                case LoadType.PointDisplacement:
                    {
                        BHoMGroup<Node> group = new BHoMGroup<Node>() { Name = groupName };
                        return PointDisplacement(loadCase, group, force, moment, axis, groupName);
                    }
                case LoadType.PointVelocity:
                    {
                        BHoMGroup<Node> group = new BHoMGroup<Node>() { Name = groupName };
                        return PointVelocity(loadCase, group, force, moment, axis, groupName);
                    }
                case LoadType.PointAcceleration:
                    {
                        BHoMGroup<Node> group = new BHoMGroup<Node>() { Name = groupName };
                        return PointAcceleration(loadCase, group, force, moment, axis, groupName);
                    }
                case LoadType.BarUniformLoad:
                    {
                        BHoMGroup<Bar> group = new BHoMGroup<Bar>() { Name = groupName };
                        return BarUniformlyDistributedLoad(loadCase, group, force, moment, axis, isProjected, groupName);
                    }

                case LoadType.BarTemperature:
                    {
                        BHoMGroup<Bar> group = new BHoMGroup<Bar>() { Name = groupName };
                        return BarTemperatureLoad(loadCase, mag, group, axis, isProjected, groupName);
                    }
                case LoadType.AreaUniformLoad:
                    {
                        BHoMGroup<IAreaElement> group = new BHoMGroup<IAreaElement>() { Name = groupName };
                        return AreaUniformlyDistributedLoad(loadCase, force, group, axis, isProjected, groupName);
                    }
                case LoadType.BarVaryingLoad:
                case LoadType.BarPointLoad:
                case LoadType.AreaTemperature:
                case LoadType.Pressure:
                case LoadType.Geometrical:
                default:
                    throw new NotImplementedException("Load type not implemented");
            }
        }




        /***************************************************/


        /***************************************************/


    }
}

