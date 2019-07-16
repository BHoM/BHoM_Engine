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


using BH.oM.Architecture.Theatron;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Humans.ViewQuality;

using BH.Engine.Architecture;
using System;
using System.Collections.Generic;
using System.IO;

namespace Engine_Test
{
    public class TheatronTest
    {
        public void Test()
        {

            var sType = BH.Engine.Architecture.Theatron.Create.StadiaTypeEnum(10);
            var sParams = BH.Engine.Architecture.Theatron.Create.StadiaParameters(7.5, 10, typeOfBowl: sType);
            sParams.TypeOfBowl = StadiaType.EightArc;
            var plan = BH.Engine.Architecture.Theatron.Create.PlanGeometry(sParams);
            var pParams1 = BH.Engine.Architecture.Theatron.Create.ProfileParameters(1);
            var pParams2 = BH.Engine.Architecture.Theatron.Create.ProfileParameters(1);
            pParams2.NumRows = 20;
            pParams2.Vomitory = true;
            pParams2.VomitoryStartRow = 5;
            pParams1.NumRows = 20;
            pParams1.Vomitory = true;
            pParams1.VomitoryStartRow = 5;

            List<ProfileParameters> parameters = new List<ProfileParameters> { pParams1, pParams2 };
            //SIMPLE profile in XZ plane focal point is the origin
            // fullProfile = Create.TheatronFullProfile(parameters);
            //profile in XZ plane focal point is the origin but distance from origin defined by the plan geometry
            var fullProfile = BH.Engine.Architecture.Theatron.Create.TheatronFullProfile(parameters, plan);

            var bowl = BH.Engine.Architecture.Theatron.Create.TheatronGeometry(plan, fullProfile, sParams, parameters);
            AValueAnalysisTest(bowl, sParams.ActivityArea);
        }
        public void AValueAnalysisTest(TheatronGeometry bowl, ActivityArea activityArea)
        {
            var settings = BH.Engine.Humans.ViewQuality.Create.AvalueSettings(ViewConeEnum.DynamicConeArea, true);
            var results = BH.Engine.Humans.ViewQuality.Query.AvalueAnalysis(bowl.Tiers3d[0].TierBlocks[0].Audience, settings, activityArea);
        }
        public void SutherLandHodgmanTest()
        {
            ////a 5 sided polyline
            //var clip = Geometry.Create.Polyline(new List<Point> { 

            //Geometry.Create.Point( 0.15127, 1.985252, 0),
            //Geometry.Create.Point( 2.390837, 4.912972, 0),
            //Geometry.Create.Point(6.715149, 7.422141, 0),
            //Geometry.Create.Point( 13.361243, 1.555223, 0),
            //Geometry.Create.Point( 6.021124, -1.413337, 0),
            //Geometry.Create.Point( 0.15127, 1.985252, 0),
            //   });
            ////a W shaped polyline
            //var subject = Geometry.Create.Polyline(new List<Point> {
            //    Geometry.Create.Point(4.468621, -0.202845, 0),
            //    Geometry.Create.Point(4.253897, 0.574251, 0),
            //    Geometry.Create.Point(4.039173, 1.351347, 0),
            //    Geometry.Create.Point(3.463167, 3.440645, 0),
            //    Geometry.Create.Point(2.887162, 5.529943, 0),
            //    Geometry.Create.Point(2.253215, 5.529943, 0),
            //    Geometry.Create.Point(1.619268, 5.529943, 0),
            //    Geometry.Create.Point(2.726971, 1.910311, 0),
            //    Geometry.Create.Point(3.834674, -1.709321, 0),
            //    Geometry.Create.Point(4.475437, -1.709321, 0),
            //    Geometry.Create.Point(5.116201, -1.709321, 0),
            //    Geometry.Create.Point(5.848989, 1.075274, 0),
            //    Geometry.Create.Point(6.581777, 3.859868, 0),
            //    Geometry.Create.Point(6.721518, 3.242963, 0),
            //    Geometry.Create.Point(6.861259, 2.626058, 0),
            //    Geometry.Create.Point(7.437264, 0.458369, 0),
            //    Geometry.Create.Point(8.01327, -1.709321, 0),
            //    Geometry.Create.Point(8.650625, -1.709321, 0),
            //    Geometry.Create.Point(9.28798, -1.709321, 0),
            //    Geometry.Create.Point(10.419541, 1.910311, 0),
            //    Geometry.Create.Point(11.551102, 5.529943, 0),
            //    Geometry.Create.Point(10.954647, 5.529943, 0),
            //    Geometry.Create.Point(10.358191, 5.529943, 0),
            //    Geometry.Create.Point(9.737878, 3.437237, 0),
            //    Geometry.Create.Point(9.117564, 1.344531, 0),
            //    Geometry.Create.Point(8.909657, 0.645826, 0),
            //    Geometry.Create.Point(8.701749, -0.052879, 0),
            //    Geometry.Create.Point(8.521109, 0.639009, 0),
            //    Geometry.Create.Point(8.340468, 1.330897, 0),
            //    Geometry.Create.Point(7.798546, 3.43042, 0),
            //    Geometry.Create.Point(7.256623, 5.529943, 0),
            //    Geometry.Create.Point(6.626085, 5.529943, 0),
            //    Geometry.Create.Point(5.995546, 5.529943, 0),
            //    Geometry.Create.Point(5.419541, 3.40997, 0),
            //    Geometry.Create.Point(4.843535, 1.289998, 0),
            //    Geometry.Create.Point(4.495887, -0.086962, 0),
            //    Geometry.Create.Point(4.468621, -0.202845, 0)

            //});
            ////the  expected result
            //var expectedresult = Geometry.Create.Polyline(new List<Point> {
            //    Geometry.Create.Point(4.843535, 1.289998, 0),
            //    Geometry.Create.Point(4.495887, -0.086962, 0),
            //    Geometry.Create.Point(4.468621, -0.202845, 0),
            //    Geometry.Create.Point(4.253897, 0.574251, 0),
            //    Geometry.Create.Point(4.039173, 1.351347, 0),
            //    Geometry.Create.Point(3.502262, 3.29884, 0),
            //    Geometry.Create.Point(2.965352, 5.246332, 0),
            //    Geometry.Create.Point(2.678094, 5.079652, 0),
            //    Geometry.Create.Point(2.390837, 4.912972, 0),
            //    Geometry.Create.Point(2.182718, 4.640903, 0),
            //    Geometry.Create.Point(1.974598, 4.368835, 0),
            //    Geometry.Create.Point(2.614177, 2.278889, 0),
            //    Geometry.Create.Point(3.253755, 0.188943, 0),
            //    Geometry.Create.Point(4.278598, -0.404431, 0),
            //    Geometry.Create.Point(5.303442, -0.997806, 0),
            //    Geometry.Create.Point(5.942609, 1.431031, 0),
            //    Geometry.Create.Point(6.581777, 3.859868, 0),
            //    Geometry.Create.Point(6.721518, 3.242963, 0),
            //    Geometry.Create.Point(6.861259, 2.626058, 0),
            //    Geometry.Create.Point(7.305098, 0.95575, 0),
            //    Geometry.Create.Point(7.748938, -0.714558, 0),
            //    Geometry.Create.Point(8.807826, -0.286312, 0),
            //    Geometry.Create.Point(9.866715, 0.141933, 0),
            //    Geometry.Create.Point(10.417745, 1.904568, 0),
            //    Geometry.Create.Point(10.968776, 3.667202, 0),
            //    Geometry.Create.Point(10.507983, 4.073972, 0),
            //    Geometry.Create.Point(10.04719, 4.480742, 0),
            //    Geometry.Create.Point(9.582377, 2.912637, 0),
            //    Geometry.Create.Point(9.117564, 1.344531, 0),
            //    Geometry.Create.Point(8.909657, 0.645826, 0),
            //    Geometry.Create.Point(8.701749, -0.052879, 0),
            //    Geometry.Create.Point(8.521109, 0.639009, 0),
            //    Geometry.Create.Point(8.340468, 1.330897, 0),
            //    Geometry.Create.Point(7.798546, 3.43042, 0),
            //    Geometry.Create.Point(7.256623, 5.529943, 0),
            //    Geometry.Create.Point(6.626085, 5.529943, 0),
            //    Geometry.Create.Point(5.995546, 5.529943, 0),
            //    Geometry.Create.Point(5.419541, 3.40997, 0),
            //    Geometry.Create.Point(4.843535, 1.289998, 0)


            //});
            //var expectedArea = expectedresult.Area();

            //var result = Geometry.Compute.ClipPolylines(subject, clip);
            //var area = result.Area();
            //var areadiff = Math.Abs(area - expectedArea) / expectedArea;

            //if (areadiff < 0.001)
            //{
            //    //for the avalue 0.1% is probably good enough
            //    throw new Exception("Incorrect result from SutherLandHodgman test");
            //}
            ////get the points in a file for a visual compare
            //using (StreamWriter writer = new StreamWriter("points.txt"))
            //{
            //    foreach (Point p in result.ControlPoints)
            //    {
            //        writer.WriteLine(string.Format("{{{0:0.000000},{1:0.000000},0}}", p.X,p.Y));
            //    }
            //}

        }
    }
}
