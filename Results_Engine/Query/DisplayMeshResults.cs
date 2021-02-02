/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Structure.Results;
using BH.oM.Structure.Elements;
using BH.oM.Graphics;
using System;
using BH.Engine.Graphics;
using BH.Engine.Geometry;
using BH.Engine.Base;
using BH.Engine.Analytical;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Applies colour to Mesh based on MeshResult, i.e stress or force etc, to a FEMesh.")]
        [Input("meshes", "Meshes to colour.")]
        [Input("meshResults", "The meshes' results to colour by.")]
        [Input("identifier", "The type of IAdapterId fragment to be used to extract the object identification, i.e. which fragment type to look for to find the identifier of the object. If no identifier is provided, the object will be scanned an IAdapterId to be used.")]
        [Input("caseFilter", "Which cases to colour by, default is all.")]
        [Input("meshResultDisplay", "Which kind of results to colour by.")]
        [Input("gradientOptions", "How to color the mesh, null defaults to `BlueToRed` with automatic range.")]
        [Output("results", "A List of Lists of RenderMeshes, where there is one List per provided mesh and one element per meshResult that matched that mesh.")]
        public static List<List<RenderMesh>> DisplayMeshResults(this IEnumerable<FEMesh> meshes, IEnumerable<MeshResult> meshResults,
                      Type identifier = null, List<string> caseFilter = null, MeshResultDisplay meshResultDisplay = MeshResultDisplay.SXX,
                      GradientOptions gradientOptions = null)
        {
            if (gradientOptions == null)
                gradientOptions = new GradientOptions();

            //Check if no identifier has been provided. If this is the case, identifiers i searched for on the obejcts
            identifier = meshes.ElementAt(0).FindIdentifier(identifier);

            List<FEMesh> meshList = meshes.ToList();

            // Order the MeshResults by FEMeshes
            List<List<MeshResult>> mappedResults = meshList.MapResults(meshResults, MapResultsBy.ObjectId, identifier, caseFilter);

            gradientOptions = gradientOptions.AutoRange(mappedResults.SelectMany(x => x.SelectMany(y => y.Results.Select(z => z.ResultToValue(meshResultDisplay)))));
            gradientOptions = gradientOptions.DefaultGradient("BlueToRed");
            gradientOptions = gradientOptions.CenteringOptions();

            List<List<RenderMesh>> result = new List<List<RenderMesh>>();

            for (int i = 0; i < meshList.Count; i++)
            {
                result.Add(new List<RenderMesh>());
                for (int j = 0; j < mappedResults[i].Count; j++)
                {
                    result[i].Add(meshList[i].DisplayMeshResults(mappedResults[i][j], identifier, meshResultDisplay, gradientOptions.Gradient, gradientOptions.From, gradientOptions.To));
                }
            }

            return result;
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/

        [Description("Gets the value type specified in MeshResultDisplay from the object r.")]
        [Output("value", "The value of the specified type.")]
        private static double ResultToValue(this MeshElementResult r, MeshResultDisplay prop)
        {
            if (r is MeshStress)
            {
                switch (prop)
                {
                    case MeshResultDisplay.SXX:
                        return (r as MeshStress).SXX;
                    case MeshResultDisplay.SYY:
                        return (r as MeshStress).SYY;
                    case MeshResultDisplay.SXY:
                        return (r as MeshStress).SXY;
                    case MeshResultDisplay.TXX:
                        return (r as MeshStress).TXX;
                    case MeshResultDisplay.TYY:
                        return (r as MeshStress).TYY;
                    case MeshResultDisplay.Principal_1:
                        return (r as MeshStress).Principal_1;
                    case MeshResultDisplay.Principal_2:
                        return (r as MeshStress).Principal_2;
                    case MeshResultDisplay.Principal_1_2:
                        return (r as MeshStress).Principal_1_2;
                    default:
                        break;
                }
            }
            else if (r is MeshForce)
            {
                switch (prop)
                {
                    case MeshResultDisplay.NXX:
                        return (r as MeshForce).NXX;
                    case MeshResultDisplay.NYY:
                        return (r as MeshForce).NYY;
                    case MeshResultDisplay.NXY:
                        return (r as MeshForce).NXY;
                    case MeshResultDisplay.MXX:
                        return (r as MeshForce).MXX;
                    case MeshResultDisplay.MYY:
                        return (r as MeshForce).MYY;
                    case MeshResultDisplay.MXY:
                        return (r as MeshForce).MXY;
                    case MeshResultDisplay.VX:
                        return (r as MeshForce).VX;
                    case MeshResultDisplay.VY:
                        return (r as MeshForce).VY;
                    default:
                        break;
                }
            }
            else if (r is MeshDisplacement)
            {
                switch (prop)
                {
                    case MeshResultDisplay.UXX:
                        return (r as MeshDisplacement).UXX;
                    case MeshResultDisplay.UYY:
                        return (r as MeshDisplacement).UYY;
                    case MeshResultDisplay.UZZ:
                        return (r as MeshDisplacement).UZZ;
                    case MeshResultDisplay.RXX:
                        return (r as MeshDisplacement).RXX;
                    case MeshResultDisplay.RYY:
                        return (r as MeshDisplacement).RYY;
                    case MeshResultDisplay.RZZ:
                        return (r as MeshDisplacement).RZZ;
                    default:
                        break;
                }
            }
            else if (r is MeshVonMises)
            {
                switch (prop)
                {
                    case MeshResultDisplay.S:
                        return (r as MeshVonMises).S;
                    case MeshResultDisplay.N:
                        return (r as MeshVonMises).N;
                    case MeshResultDisplay.M:
                        return (r as MeshVonMises).M;
                    default:
                        break;
                }

            }

            Engine.Reflection.Compute.RecordError(prop.ToString() + " is not present in the results.");
            return 0;
        }

        /***************************************************/

        [Description("Applies colour to a single Mesh based on a single MeshResult, i.e stress or force etc, to a FEMesh.")]
        [Output("renderMesh", "A coloured RenderMesh.")]
        private static RenderMesh DisplayMeshResults(this FEMesh mesh, MeshResult meshResult, Type identifier,
                                            MeshResultDisplay meshResultDisplay, Gradient gradient, double from, double to)
        {
            // Order the MeshNodeResults by the FEMesh Nodes
            List<List<MeshElementResult>> tempMappedElementResults = mesh.Nodes.MapResults(meshResult.Results, MapResultsBy.NodeId, identifier);
            // Get the relevant values into a list

            List<Vertex> verts = new List<Vertex>();
            List<Face> faces;

            switch (meshResult.Smoothing)
            {
                case MeshResultSmoothingType.None:
                    //  pair nodeValue as list<Dictionary<FaceId,nodeValue>>
                    //  all nodes are expected to have FaceIds
                    List<Dictionary<IComparable, double>> nodeValuePairs = tempMappedElementResults.Select(x => x.ToDictionary(y => y.MeshFaceId, y => y.ResultToValue(meshResultDisplay))).ToList();
                    //  put the Faces in a Dictionary<FaceId,Face>
                    Dictionary<object, Face> faceDictionaryResult = mesh.Faces.ToDictionary(x => x.FindFragment<IAdapterId>(identifier).Id, x => x.Geometry());
                    Dictionary<object, Face> faceDictionaryRefrence = mesh.Faces.ToDictionary(x => x.FindFragment<IAdapterId>(identifier).Id, x => x.Geometry());

                    // Add all verticies to a list with their colour and update the Faces
                    for (int k = 0; k < mesh.Nodes.Count; k++)
                    {
                        foreach (KeyValuePair<IComparable, double> FaceRelatedValue in nodeValuePairs[k])
                        {
                            verts.Add(new Vertex()
                            {
                                Point = mesh.Nodes[k].Position,
                                Color = gradient.Color(FaceRelatedValue.Value, from, to)
                            });
                            // Face management, faceResult points to verts and faceReference points to mesh.Nodes
                            Face faceResult = faceDictionaryResult[FaceRelatedValue.Key];
                            Face faceReference = faceDictionaryRefrence[FaceRelatedValue.Key];
                            Face newFace = new Face()
                            {
                                A = faceReference.A == k ? verts.Count - 1 : faceResult.A,
                                B = faceReference.B == k ? verts.Count - 1 : faceResult.B,
                                C = faceReference.C == k ? verts.Count - 1 : faceResult.C,
                                D = faceReference.IsQuad() ? faceReference.D == k ? verts.Count - 1 : faceResult.D : -1
                            };
                            faceDictionaryResult[FaceRelatedValue.Key] = newFace;
                        }
                    }
                    faces = faceDictionaryResult.Select(x => x.Value).ToList();
                    break;

                case MeshResultSmoothingType.ByPanel:
                case MeshResultSmoothingType.Global:
                    List<double> nodeValues = tempMappedElementResults.Select(x => x[0].ResultToValue(meshResultDisplay)).ToList();

                    // Add all verticies to a list with their colour
                    for (int k = 0; k < mesh.Nodes.Count; k++)
                    {
                        verts.Add(new Vertex()
                        {
                            Point = mesh.Nodes[k].Position,
                            Color = gradient.Color(nodeValues[k], from, to)
                        });
                    }
                    faces = mesh.Faces.Geometry().ToList();
                    break;

                case MeshResultSmoothingType.ByFiniteElementCentres:
                case MeshResultSmoothingType.BySelection:
                default:
                    Engine.Reflection.Compute.RecordError("Unsupported SmoothingType: " + meshResult.Smoothing.ToString() +
                                                      " detected, meshResult for ObjectId: " + meshResult.ObjectId.ToString() +
                                                      " and ResultCase: " + meshResult.ResultCase.ToString() + "will be returned empty.");
                    return new RenderMesh();
            }

            return new RenderMesh() { Vertices = verts, Faces = faces };
        }

        /***************************************************/

    }
}
