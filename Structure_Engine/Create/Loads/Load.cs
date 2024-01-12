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

using BH.oM.Geometry;
using BH.oM.Structure.Loads;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.Engine.Base;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Method used to create a load from a specified type as well as range of variables.")]
        [Input("type", "Specifies the type of load that should be created.")]
        [InputFromProperty("loadcase")]
        [Input("magnitude", "Should be a list of 1-6 doubles, depending on load type. \n" +
                            "-For load types with a single value, such as temprature loads, only the first value is used. \n" +
                            "-For load types with a single vector, such as Area UDLs, the first three numbers are used, and assumed to be the x, y and z component of this vector. \n" +
                            "-For load types with two vectors, such as bar UDLs, the first three values will be assumed to be the force vector, and the last three the moment vector. \n" +
                            "Quantity varies with load type.")]
        [Input("groupName", "Value will be given to the object group on the load as well as to the load itself.")]
        [InputFromProperty("axis")]
        [InputFromProperty("isProjected", "Projected")]
        [Input("units", "Scales the magnitude inputs to SI depending on the provided value. Accepted values are N, Newton, Newtons, kN, kilo newton, kilo newtons, C, Celcius, K, Kelvin.")]
        [Output("load", "The created Load.")]
        public static ILoad Load(LoadType type, Loadcase loadcase, List<double> magnitude, string groupName, LoadAxis axis, bool isProjected, string units = "kN")
        {
            if (magnitude.IsNullOrEmpty() || String.IsNullOrEmpty(groupName))
                return null;

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
                    Base.Compute.RecordError("Unrecognised unit type.");
                    return null;
            }

            Vector force = null;
            Vector moment = null;
            double mag;

            if (magnitude.Count < 1)
            {
                Base.Compute.RecordError("At least magnitude value is required, please check inputs.");
                return null;
            }

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
                        return new GravityLoad { Loadcase = loadcase, GravityDirection = force, Objects = group, Name = groupName };
                    }
                case LoadType.PointLoad:
                    {
                        BHoMGroup<Node> group = new BHoMGroup<Node>() { Name = groupName };
                        return PointLoad(loadcase, group, force, moment, axis, groupName);
                    }
                case LoadType.PointDisplacement:
                    {
                        BHoMGroup<Node> group = new BHoMGroup<Node>() { Name = groupName };
                        return PointDisplacement(loadcase, group, force, moment, axis, groupName);
                    }
                case LoadType.PointVelocity:
                    {
                        BHoMGroup<Node> group = new BHoMGroup<Node>() { Name = groupName };
                        return PointVelocity(loadcase, group, force, moment, axis, groupName);
                    }
                case LoadType.PointAcceleration:
                    {
                        BHoMGroup<Node> group = new BHoMGroup<Node>() { Name = groupName };
                        return PointAcceleration(loadcase, group, force, moment, axis, groupName);
                    }
                case LoadType.BarUniformLoad:
                    {
                        BHoMGroup<Bar> group = new BHoMGroup<Bar>() { Name = groupName };
                        return BarUniformlyDistributedLoad(loadcase, group, force, moment, axis, isProjected, groupName);
                    }

                case LoadType.BarTemperature:
                    {
                        BHoMGroup<Bar> group = new BHoMGroup<Bar>() { Name = groupName };
                        return new BarUniformTemperatureLoad { Loadcase = loadcase, TemperatureChange = mag, Objects = group, Axis = axis, Projected = isProjected, Name = groupName };
                    }
                case LoadType.AreaUniformLoad:
                    {
                        BHoMGroup<IAreaElement> group = new BHoMGroup<IAreaElement>() { Name = groupName };
                        return AreaUniformlyDistributedLoad(loadcase, force, group, axis, isProjected, groupName);
                    }
                case LoadType.BarVaryingLoad:
                case LoadType.BarPointLoad:
                case LoadType.AreaTemperature:
                case LoadType.Pressure:
                case LoadType.Geometrical:
                default:
                    Base.Compute.RecordError("Load type not implemented.");
                    return null;
            }
        }

        /***************************************************/

    }
}





