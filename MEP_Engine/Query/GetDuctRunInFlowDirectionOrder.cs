/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using System.ComponentModel;

using BH.oM.Geometry;
using BH.oM.MEP.System;
using BH.oM.MEP.System.Fittings;
using BH.oM.MEP.Fixtures;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.Linq;
using System;
using BH.oM.Base;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/
        [Description("")]
        [Input("ductsAndFittings", "List of ducts and fittings")]
        public static List<BHoMObject> GetDuctRunInFlowDirectionOrder(List<Fitting> fittings, List<Duct> ducts, Duct firstDuct)
        {
            List<Fitting> ConnectedFittings = new List<Fitting>();

            //Ideally only two fitting connections...
            foreach (Fitting fitting in fittings)
            {
                fitting.ConnectionObjects = new List<Duct>();

                foreach (var fittingConnection in fitting.ConnectionsLocation)
                {
                    foreach (var duct in ducts)
                    {
                        if ((duct.StartPoint.X == fittingConnection.X && duct.StartPoint.Y == fittingConnection.Y && duct.StartPoint.Z == fittingConnection.Z) ||
                            (duct.EndPoint.X == fittingConnection.X && duct.EndPoint.Y == fittingConnection.Y && duct.EndPoint.Z == fittingConnection.Z))
                        {
                            fitting.ConnectionObjects.Add(duct);
                        }
                    }
                }
                ConnectedFittings.Add(fitting);
            }


            List<BHoMObject> ductRunFinal = new List<BHoMObject>();


            ductRunFinal.Add(firstDuct);

            for (int i = 0; i <= ducts.Count; i++)
            {
                foreach (Duct duct in ducts)
                {
                    if (ductRunFinal.Contains(duct))
                    {
                        foreach (Fitting fitting in ConnectedFittings)
                        {
                            if (fitting.ConnectionObjects[0] == duct)
                            {
                                if (!ductRunFinal.Contains(fitting))
                                {
                                    ductRunFinal.Insert(ductRunFinal.IndexOf(duct), (Fitting)fitting);
                                    if (!ductRunFinal.Contains(fitting.ConnectionObjects[1]))
                                    {
                                        ductRunFinal.Insert(ductRunFinal.IndexOf(fitting), (Duct)fitting.ConnectionObjects[1]);
                                    }
                                }

                            }
                            if (fitting.ConnectionObjects[1] == duct)
                            {
                                if (!ductRunFinal.Contains(fitting))
                                {
                                    ductRunFinal.Insert(ductRunFinal.IndexOf(duct), (Fitting)fitting);
                                    if (!ductRunFinal.Contains(fitting.ConnectionObjects[0]))
                                    {
                                        ductRunFinal.Insert(ductRunFinal.IndexOf(fitting), (Duct)fitting.ConnectionObjects[0]);
                                    }
                                }

                            }
                        }


                    }


                }

            }


            return ductRunFinal;
            /***************************************************/
        }

    }
}



