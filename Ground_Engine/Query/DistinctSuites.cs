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

using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Data;
using BH.oM.Geometry;
using BH.oM.Ground;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;
using BH.Engine.Data;
using BH.Engine.Geometry;
using BH.oM.Data.Requests;


namespace BH.Engine.Ground
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a distinct list of suites from a Borehole using the sample reference, test name or top depth (in order of priority).")]
        [Input("borehole", "The Borehole from which to count the number of suites.")]
        [Output("suites", "A distinct list of suites from the Borehole provided.")]
        public static List<string> DistinctSuites(this Borehole borehole)
        {
            bool reference = true;
            bool testName = false;
            bool depth = false;

            List<string> suites = new List<string>();
            List<double> depths = new List<double>();

            foreach(ContaminantSample contaminantSample in borehole.ContaminantSamples)
            {
                if (reference)
                {
                    List<IContaminantProperty> contaminantProperties = contaminantSample.ContaminantProperties;
                    FilterRequest filterRequest = Data.Create.FilterRequest(typeof(ContaminantReference), "");
                    IEnumerable<IBHoMObject> contaminantReferences = Data.Compute.FilterData(filterRequest, contaminantProperties);
                    if (contaminantReferences.Any(x => x == null) || contaminantReferences.Count() != 0)
                    {
                        ContaminantReference contaminantReference = (ContaminantReference)contaminantReferences.First();
                        if (contaminantReference.Reference != "")
                            suites.Add(contaminantReference.Reference);
                        else
                        {
                            testName = true;
                            reference = false;
                        }
                    }
                    else
                    {
                        testName = true;
                        reference = false;
                    }
                }    
                
                if(testName)
                {
                    List<IContaminantProperty> contaminantProperties = contaminantSample.ContaminantProperties;
                    FilterRequest filterRequest = Data.Create.FilterRequest(typeof(TestProperties), "");
                    IEnumerable<IBHoMObject> testProperties = Data.Compute.FilterData(filterRequest, contaminantProperties);
                    if (!testProperties.Any(x => x == null) || testProperties.Count() != 0)
                    {
                        TestProperties testProperty = (TestProperties)testProperties.First();
                        if (testProperty.Name != "")
                            suites.Add(testProperty.Name);
                        else
                        {
                            testName = false;
                            depth = true;
                        }
                    }    
                    else
                    {
                        testName = false;
                        depth = true;
                    }
                }    
                
                if(depth)
                {
                    depths.Add(contaminantSample.Top);
                }

                if(!reference && !testName && !depth)
                {
                    Engine.Base.Compute.RecordError("Neither the ContaminantReference.Reference, TestProperties.Name or Top depth fields contain any data. \n" +
                        "Therefore, the number of suites cannot be counted.");
                }
            }

            if (depth)
                suites = depths.Distinct().Select(x => x.ToString()).ToList();
            else
                suites = suites.Distinct().ToList();

            return suites;
        }
    }
}



