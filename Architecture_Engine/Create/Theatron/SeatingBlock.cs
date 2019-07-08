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

using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Architecture.Theatron;
using System.Collections.Generic;
using System;
using BH.Engine.Base;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SeatingBlock SeatingBlock(ProfileOrigin start, ProfileOrigin vom, ProfileOrigin end, SeatingBlockType t, double seatWidth, double aisleWidth)
        {
            return new SeatingBlock
            {
                Start = start,

                Vomitory = vom,

                End = end,

                SeatWidth = seatWidth,

                TypeOfSeatingBlock = t,

                AisleWidth = aisleWidth,

            };
        }

        /***************************************************/

        public static void SetTransitionBlock(ref SeatingBlock block, TierProfile startToMap, TierProfile endToMap, ProfileOrigin origin)
        {
            //transition block from neighbours
            
            //first section
            double scalefactor = 1.0;
            double angle = Geometry.Query.Angle(startToMap.SectionOrigin.Direction, block.Start.Direction, Plane.XY);
            var s = Create.mapTierToPlane(startToMap, scalefactor, block.Start.Origin, angle);
            block.Sections.Add(s);

            //last section
            angle = Geometry.Query.Angle(endToMap.SectionOrigin.Direction, block.End.Direction,  Plane.XY);
            var e = Create.mapTierToPlane(endToMap, scalefactor, block.End.Origin, angle);
            block.Sections.Add(e);
        }

        /***************************************************/

        public static void SetBlockProfiles(ref SeatingBlock block, TierProfile sectionToMap, ProfileOrigin origin)
        {
            //first section
            double scalefactor = setScaleFactor(block.Start, block.Vomitory);
            double angle = Geometry.Query.Angle(origin.Direction, block.Start.Direction,Plane.XY);
            var start = Create.mapTierToPlane(sectionToMap, scalefactor, (block.Start.Origin), angle);
            block.Sections.Add(start);

            //vomitory section no need for scalefactor
            scalefactor = 1;
            angle = Geometry.Query.Angle(origin.Direction, block.Vomitory.Direction, Plane.XY);
            var vom = Create.mapTierToPlane(sectionToMap, scalefactor, (block.Vomitory.Origin), angle);
            block.Sections.Add(vom);

            //end section
            scalefactor = setScaleFactor(block.End, block.Vomitory);
            angle = Geometry.Query.Angle(origin.Direction, block.End.Direction, Plane.XY);
            var end = Create.mapTierToPlane(sectionToMap, scalefactor, (block.End.Origin), angle);
            block.Sections.Add(end);
        }

        /***************************************************/

        public static SeatingBlock MirrorSeatingBlock(SeatingBlock original, SeatingBlockType stype)
        {
            SeatingBlock mirrored = original.DeepClone();
            mirrored.TypeOfSeatingBlock = stype;
            List<TierProfile> mirroredProfiles = new List<TierProfile>();
            //mirror the origins
            mirrored.Start.Origin.Y = -mirrored.Start.Origin.Y;
            mirrored.Start.Direction.Y = -mirrored.Start.Direction.Y;
            mirrored.Vomitory.Origin.Y = -mirrored.Vomitory.Origin.Y;
            mirrored.Vomitory.Direction.Y = -mirrored.Vomitory.Direction.Y;
            mirrored.End.Origin.Y = -mirrored.End.Origin.Y;
            mirrored.End.Direction.Y = -mirrored.End.Direction.Y;
            //mirror the points and profile
            foreach (TierProfile profile in mirrored.Sections)
            {
                var mirroredprofile = Create.mirrorTierYZ(profile);
                mirroredProfiles.Add(mirroredprofile);
            }
            mirrored.Sections = mirroredProfiles;
            return mirrored;
        }

        /***************************************************/

        public static void setBlockFloorBasic(ref SeatingBlock block)
        {
            List<Point> vertices = new List<Point>();
            int pointsPerSect = block.Sections[0].FloorPoints.Count;
            //add all the vertices
            for (int j = 0; j < block.Sections.Count; j++)
            {
                for (int i = 0; i < block.Sections[j].FloorPoints.Count; i++)
                {
                    vertices.Add(block.Sections[j].FloorPoints[i]);
                }
            }
            //make the faces
            List<Face> faces = new List<Face>();
            for (int j = 0; j < block.Sections.Count - 1; j++)//not the last section
            {
                int firstIndex = j * pointsPerSect;//first point index this section
                int nextFirstIndex = (j + 1) * pointsPerSect;//first point next section
                for (int i = 0; i < block.Sections[j].FloorPoints.Count - 1; i++)//not the last points
                {
                    faces.Add(Geometry.Create.Face(firstIndex + i, firstIndex + i + 1, nextFirstIndex + i + 1, nextFirstIndex + i));
                }
            }
            block.Floor = Geometry.Create.Mesh(vertices,faces);
        }

        /***************************************************/

        public static void setBlockFloor(ref SeatingBlock block, ProfileParameters parameters)//include gaps for vomitories
        {
            
            bool vomitory = parameters.Vomitory;
            bool superR = parameters.SuperRiser;
            int vomStart = parameters.VomitoryStartRow;
            int superStart = parameters.SuperRiserStartRow;
            double aisleW = parameters.AisleWidth;
            if (!vomitory && !superR)
            {
                setBlockFloorBasic(ref block);
                return;
            }
            if (vomitory && superR) vomStart = superStart;
            if (superR) vomStart = superStart;
            Vector rightShift = Geometry.Query.CrossProduct(Vector.ZAxis, block.Vomitory.Direction);
            rightShift.Normalise();
            //half a 1.2 width vom
            rightShift = rightShift * aisleW / 2;
            Vector leftShift = rightShift.DeepClone();
            
            leftShift.Reverse();
            
            List<Point> temp = new List<Point>();

            //add the points to the mesh
            List<Point> vertices = new List<Point>();
            for (int j = 0; j < block.Sections.Count; j++)
            {
                temp = new List<Point>(block.Sections[j].FloorPoints);

                if (j == 1)//on the vom
                {
                    //transfrom right
                    for(int p=0;p<temp.Count;p++) temp[p] = temp[p] + rightShift;
                    vertices.AddRange(temp);
                    temp = new List<Point>(block.Sections[j].FloorPoints);
                    //transfrom left
                    for (int p = 0; p < temp.Count; p++) temp[p] = temp[p] + leftShift;
                }
                vertices.AddRange(temp);

            }
            int pointsPerSect = block.Sections[0].FloorPoints.Count;
            int row = 0;
            //make the faces
            List<Face> faces = new List<Face>();
            for (int j = 0; j < block.Sections.Count; j++)//not the last section
            {
                int firstIndex = j * pointsPerSect;//first point index this section
                int nextFirstIndex = (j + 1) * pointsPerSect;//first point next section
                for (int i = 0; i < block.Sections[j].FloorPoints.Count - 1; i++)//not the last points
                {
                    if (j == 1 && i % 2 == 0) row++;
                    if (row < vomStart || row > vomStart + 8)
                    {

                        faces.Add(Geometry.Create.Face(firstIndex + i, firstIndex + i + 1, nextFirstIndex + i + 1, nextFirstIndex + i));
                    }
                }
            }
            block.Floor = Geometry.Create.Mesh(vertices, faces);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double setScaleFactor(ProfileOrigin current, ProfileOrigin next)
        {
            double scaleAngle = Geometry.Query.Angle(current.Direction, next.Direction);
            double scalefactor = 1 / Math.Cos(scaleAngle);
            return scalefactor;
        }
    }
}
