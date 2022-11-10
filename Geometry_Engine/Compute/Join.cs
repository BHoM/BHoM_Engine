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

using BH.oM.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****                Join curves                ****/
        /***************************************************/

        public static List<PolyCurve> Join(this List<PolyCurve> curves, double tolerance = Tolerance.Distance)
        {

            List<PolyCurve> sections = new List<PolyCurve>();

            foreach(PolyCurve curve in curves)
            {
                foreach(ICurve section in curve.ISubParts())
                {
                    sections.Add(new PolyCurve { Curves = new List<ICurve> { section } });
                }
            }

            double sqTol = tolerance * tolerance;
            int counter = 0;
            while (counter < sections.Count)
            {
                for (int j = counter + 1; j < sections.Count; j++)
                {
                    if (sections[j].IStartPoint().SquareDistance(sections[counter].IStartPoint()) <= sqTol)
                    {
                        sections[counter].Curves = sections[counter].Curves.Select(c => c.IFlip()).ToList();
                        sections[counter].Curves.Reverse();
                        sections[j].Curves.InsertRange(0, sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IStartPoint().SquareDistance(sections[counter].IEndPoint()) <= sqTol)
                    {
                        sections[j].Curves.InsertRange(0, sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IEndPoint().SquareDistance(sections[counter].IStartPoint()) <= sqTol)
                    {
                        sections[j].Curves.AddRange(sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IEndPoint().SquareDistance(sections[counter].IEndPoint()) <= sqTol)
                    {
                        sections[counter].Curves = sections[counter].Curves.Select(c => c.IFlip()).ToList();
                        sections[counter].Curves.Reverse();
                        sections[j].Curves.AddRange(sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                }
                counter++;
            }
            return sections;
        }

        /***************************************************/

        public static List<Polyline> Join(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            List<Polyline> sections = lines.Select(l => new Polyline { ControlPoints = l.ControlPoints() }).ToList();
            return Compute.Join(sections, tolerance);
        }

        /***************************************************/

        public static List<Polyline> Join(this List<Polyline> curves, double tolerance = Tolerance.Distance)
        {
            List<Polyline> sections = curves.Select(l => new Polyline { ControlPoints = new List<Point>(l.ControlPoints) }).ToList();

            double sqTol = tolerance * tolerance;
            int counter = 0;
            while (counter < sections.Count)
            {
                for (int j = counter + 1; j < sections.Count; j++)
                {
                    if (sections[j].ControlPoints[0].SquareDistance(sections[counter].ControlPoints[0]) <= sqTol)
                    {
                        List<Point> aPts = sections[counter].ControlPoints.Skip(1).ToList();
                        aPts.Reverse();
                        sections[j].ControlPoints.InsertRange(0, aPts);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].ControlPoints[0].SquareDistance(sections[counter].ControlPoints.Last()) <= sqTol)
                    {
                        List<Point> aPts = sections[counter].ControlPoints;
                        aPts.AddRange(sections[j].ControlPoints.Skip(1).ToList());
                        sections[j].ControlPoints = aPts;
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].ControlPoints.Last().SquareDistance(sections[counter].ControlPoints[0]) <= sqTol)
                    {
                        sections[j].ControlPoints.AddRange(sections[counter].ControlPoints.Skip(1).ToList()); ;
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].ControlPoints.Last().SquareDistance(sections[counter].ControlPoints.Last()) <= sqTol)
                    {
                        sections[counter].ControlPoints.Reverse();
                        sections[j].ControlPoints.AddRange(sections[counter].ControlPoints.Skip(1).ToList()); ;
                        sections.RemoveAt(counter--);
                        break;
                    }
                }
                counter++;
            }
            return sections;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<PolyCurve> IJoin(this List<ICurve> curves, double tolerance = Tolerance.Distance)
        {
            if (curves == null)
                return null;

            List<PolyCurve> sections = new List<PolyCurve>();
            
            foreach(ICurve curve in curves)
            {
                if (curve is PolyCurve)
                    sections.Add(curve as PolyCurve);
                else
                    sections.Add(new PolyCurve { Curves = curve.ISubParts().ToList() });
            }

            if (sections.Count < 2)
                return sections;

            double sqTol = tolerance * tolerance;
            int counter = 0;
            while (counter < sections.Count)
            {
                for (int j = counter + 1; j < sections.Count; j++)
                {
                    if (sections[j].IStartPoint().SquareDistance(sections[counter].IStartPoint()) <= sqTol)
                    {
                        sections[counter].Curves = sections[counter].Curves.Select(c => c.IFlip()).ToList();
                        sections[counter].Curves.Reverse();
                        sections[j].Curves.InsertRange(0, sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IStartPoint().SquareDistance(sections[counter].IEndPoint()) <= sqTol)
                    {
                        sections[j].Curves.InsertRange(0, sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IEndPoint().SquareDistance(sections[counter].IStartPoint()) <= sqTol)
                    {
                        sections[j].Curves.AddRange(sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IEndPoint().SquareDistance(sections[counter].IEndPoint()) <= sqTol)
                    {
                        sections[counter].Curves = sections[counter].Curves.Select(c => c.IFlip()).ToList();
                        sections[counter].Curves.Reverse();
                        sections[j].Curves.AddRange(sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                }
                counter++;
            }
            return sections;
        }

        /***************************************************/
        /****                Join meshes                ****/
        /***************************************************/

        [Description("Joins a list of Meshes into a single Mesh. Disjointed Meshes are allowed to be joined into one mesh.")]
        [Input("meshes", "The meshes to join.")]
        [Input("mergeVertices", "If true, duplicate vertices will be merged. If false, duplicate vertices will be kept.")]
        [Input("tolerance", "Only used if mergeVertices is true. The maximum allowable distance between two vertices for them to be deemed the same vertex.", typeof(Length))]
        [Output("mesh", "The joined meshes as a single mesh.")]
        public static Mesh Join(this List<Mesh> meshes, bool mergeVertices = false, double tolereance = Tolerance.Distance)
        {
            if (meshes == null || meshes.Count == 0)   //No meshes provided, return null
                return null;

            Mesh returnMesh = meshes[0];    //Set to first mesh as starting point

            for (int i = 1; i < meshes.Count; i++)  //Add on the rest
            {
                Mesh mesh = meshes[i];
                int vertexCount = returnMesh.Vertices.Count;

                returnMesh.Vertices.AddRange(mesh.Vertices);
                foreach (Face face in mesh.Faces)
                {
                    returnMesh.Faces.Add(new Face
                    {
                        A = face.A + vertexCount,
                        B = face.B + vertexCount,
                        C = face.C + vertexCount,
                        D = face.D == -1 ? -1 : face.D + vertexCount,
                    });
                }
            }

            if (mergeVertices)
            {
                returnMesh = returnMesh.MergedVertices(tolereance);
            }

            return returnMesh;
        }

        /***************************************************/
    }
}



