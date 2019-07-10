/*
 * block file is part of the Buildings and Habitats object Model (BHoM)
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
 * along with block code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Geometry;
using BH.oM.Humans;
using BH.Engine.Geometry;
using BH.oM.Architecture.Theatron;
using System.Collections.Generic;
using System;
using BH.Engine.Base;
using System.Linq;

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

                FrontRow = Geometry.Create.Line(start.Origin,end.Origin),

            };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void SetBlockProfiles(ref SeatingBlock block, TierProfile sectionToMap, ProfileOrigin origin)
        {
            
            //first section
            //always mapping from the full profile base plane
            Point source = origin.Origin;
            Vector scaleVector = SetScaleVector(sectionToMap.SectionOrigin.Direction, block.Start, block.Vomitory);
            double angle = Geometry.Query.Angle(origin.Direction, block.Start.Direction, Plane.XY);
            var start = TransformProfile(sectionToMap, scaleVector, source, block.Start.Origin, angle);
            block.Sections.Add(start);

            //vomitory section no need for scalefactor
            scaleVector = SetScaleVector(sectionToMap.SectionOrigin.Direction, block.Vomitory, block.Vomitory);
            angle = Geometry.Query.Angle(origin.Direction, block.Vomitory.Direction, Plane.XY);
            var vom = TransformProfile(sectionToMap, scaleVector, source, block.Vomitory.Origin, angle);
            block.Sections.Add(vom);

            //end section
            scaleVector = SetScaleVector(sectionToMap.SectionOrigin.Direction, block.End, block.Vomitory);
            angle = Geometry.Query.Angle(origin.Direction, block.End.Direction, Plane.XY);
            var end = TransformProfile(sectionToMap, scaleVector, source, block.End.Origin, angle);
            block.Sections.Add(end);
            
        }
        /***************************************************/

        private static void SetTransitionProfiles(ref SeatingBlock block, TierProfile sectionToMap, ProfileOrigin origin, ProfileOrigin prevVomitory, ProfileOrigin nextVomitory)
        {
            //first section
            //always mapping from the full profile base plane
            Point source = origin.Origin;
            Vector scaleVector = SetScaleVector(sectionToMap.SectionOrigin.Direction, block.Start, prevVomitory);
            double angle = Geometry.Query.Angle(origin.Direction, block.Start.Direction, Plane.XY);
            var start = TransformProfile(sectionToMap, scaleVector, source, block.Start.Origin, angle);
            block.Sections.Add(start);

            //end section
            scaleVector = SetScaleVector(sectionToMap.SectionOrigin.Direction, block.End, nextVomitory);
            angle = Geometry.Query.Angle(origin.Direction, block.End.Direction, Plane.XY);
            var end = TransformProfile(sectionToMap, scaleVector, source, block.End.Origin, angle);
            block.Sections.Add(end);
        }

        /***************************************************/

            private static void SetEyesBasic(ref SeatingBlock block)
        {
            block.Audience = new Audience();
            int rows = block.Sections[0].EyePoints.Count;
            double rowL = 0;
            double seatW = block.SeatWidth;
            int seats = 0;
            double spacing = 0;
            int lastSection = block.Sections.Count - 1;
            Vector rowDir = block.Sections[lastSection].EyePoints[0] - block.Sections[0].EyePoints[0];
            rowDir = rowDir.Normalise();
            Vector viewDirection = Geometry.Query.CrossProduct(Vector.ZAxis, rowDir);
            double x, y, z;
            for (int i = 0; i < rows; i++)
            {
                rowL = Geometry.Query.Distance(block.Sections[0].EyePoints[i], block.Sections[lastSection].EyePoints[i]);  
                seats = (int)Math.Floor(rowL / seatW);
                spacing = rowL / seats;
                for (int j = 0; j < seats; j++)
                {
                    // startX + rowDir*j*spacing + 0.5 spacing
                    x = block.Sections[0].EyePoints[i].X + rowDir.X * j * spacing + rowDir.X * 0.5 * spacing;
                    y = block.Sections[0].EyePoints[i].Y + rowDir.Y * j * spacing + rowDir.Y * 0.5 * spacing;
                    z = block.Sections[0].EyePoints[i].Z;
                    block.Audience.Spectators.Add(Humans.Create.Spectator(Geometry.Create.Point(x, y, z), viewDirection));
                }
            }
        }


        /***************************************************/

        private static Vector SetScaleVector(Vector unscaled, ProfileOrigin current, ProfileOrigin next)
        {
            unscaled = unscaled.Normalise();
            double scaleAngle = Geometry.Query.Angle(current.Direction, next.Direction);
            double scalefactor = 1 / Math.Cos(scaleAngle);
            Vector scaleVector = unscaled*scalefactor;
            scaleVector = scaleVector + Vector.ZAxis;
            return scaleVector;
        }

        /***************************************************/

        private static SeatingBlock MirrorSeatingBlock(SeatingBlock original, SeatingBlockType stype)
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
                var mirroredprofile = MirrorTierYZ(profile);
                mirroredProfiles.Add(mirroredprofile);
            }
            //reverse the sections list
            mirroredProfiles.Reverse();
            mirrored.Sections = mirroredProfiles;
            //flip the start and end 
            var tempstart = mirrored.Start.DeepClone();
            var tempend = mirrored.End.DeepClone();
            mirrored.Start = tempend;
            mirrored.End = tempstart;
            return mirrored;
        }

        /***************************************************/

        private static void SetBlockFloorBasic(ref SeatingBlock block)
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
                int firstIndex = j * pointsPerSect;//first point index block section
                int nextFirstIndex = (j + 1) * pointsPerSect;//first point next section
                for (int i = 0; i < block.Sections[j].FloorPoints.Count - 1; i++)//not the last points
                {
                    faces.Add(Geometry.Create.Face(firstIndex + i, firstIndex + i + 1, nextFirstIndex + i + 1, nextFirstIndex + i));
                }
            }
            block.Floor = Geometry.Create.Mesh(vertices, faces);
        }

        /***************************************************/

        private static void SetBlockFloor(ref SeatingBlock block, ProfileParameters parameters)//include gaps for vomitories
        {

            bool vomitory = parameters.Vomitory;
            bool superR = parameters.SuperRiser;
            int vomStart = parameters.VomitoryStartRow;
            int superStart = parameters.SuperRiserStartRow;
            double aisleW = parameters.AisleWidth;
            if (!vomitory && !superR)
            {
                SetBlockFloorBasic(ref block);
                return;
            }
            if (vomitory && superR) vomStart = superStart;
            if (superR) vomStart = superStart;
            Vector rightShift = Geometry.Query.CrossProduct(Vector.ZAxis, block.Vomitory.Direction);
            rightShift = rightShift.Normalise();
            //half a 1.2 width vom
            rightShift = rightShift * -aisleW / 2;
            Vector leftShift = rightShift.DeepClone();

            leftShift = leftShift.Reverse();

            List<Point> temp = new List<Point>();

            //add the points to the mesh
            List<Point> vertices = new List<Point>();
            for (int j = 0; j < block.Sections.Count; j++)
            {
                temp = block.Sections[j].FloorPoints.Select(item => item.Clone()).ToList(); 

                if (j == 1)//on the vom
                {
                    //transfrom right
                    for (int p = 0; p < temp.Count; p++) temp[p] = temp[p] + rightShift;
                    vertices.AddRange(temp);
                    temp = block.Sections[j].FloorPoints.Select(item => item.Clone()).ToList();
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
                int firstIndex = j * pointsPerSect;//first point index block section
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

        private static SeatingBlock TransformSeatingBlock(SeatingBlock originalBlock, Point source, Point target, double angle)
        {
            SeatingBlock transformedBlock = originalBlock.DeepClone();
           
            var xRotate = Geometry.Create.RotationMatrix(source, Vector.ZAxis, angle);
            var xTrans = Geometry.Create.TranslationMatrix(target - source);

            TransformBlock(ref transformedBlock, xRotate);
            TransformBlock(ref transformedBlock, xTrans);
            return transformedBlock;
        }

        /***************************************************/

        private static void TransformBlock(ref SeatingBlock transformedBlock, TransformMatrix xTrans)
        {
            transformedBlock.Audience.Spectators.ForEach(p => p.Eye.Location.Transform(xTrans));
            transformedBlock.Audience.Spectators.ForEach(p => p.Eye.ViewDirection.Transform(xTrans));
            transformedBlock.Floor = transformedBlock.Floor.Transform(xTrans);
            transformedBlock.FrontRow= transformedBlock.FrontRow.Transform(xTrans);

        }
    }
}
