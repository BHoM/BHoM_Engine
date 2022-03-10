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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using System.Linq;
using System.Reflection;
using BH.oM.Geometry;
using BH.oM.Structure.Results;
using BH.oM.Graphics;
using System;
using BH.Engine.Graphics;
using BH.Engine.Geometry;
using BH.Engine.Base;
using BH.Engine.Analytical;
using BH.oM.Analytical.Results;
using BH.oM.Analytical.Elements;
using BH.oM.Graphics.Colours;
using BH.oM.Quantities.Attributes;
 

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [PreviousVersion("5.1", "BH.Engine.Results.Query.DisplayMeshResults(System.Collections.Generic.IEnumerable<BH.oM.Analytical.Elements.IMesh<BH.oM.Analytical.Elements.INode, BH.oM.Analytical.Elements.IFace>>, System.Collections.Generic.IEnumerable<BH.oM.Analytical.Results.IMeshResult<BH.oM.Analytical.Results.IMeshElementResult>>, System.Type, System.Collections.Generic.List<System.String>, System.String, BH.oM.Graphics.Colours.GradientOptions)")]
        [PreviousInputNames("objectIdentifier", "identifier")]
        [PreviousInputNames("filter", "caseFilter")]
        [Description("Applies colour to IMesh based on MeshResult.")]
        [Input("meshes", "Meshes to colour.")]
        [Input("meshResults", "The MeshResults to colour by.")]
        [Input("objectIdentifier", "Should either be a string specifying what property on the object that should be used to map the objects to the results, or a type of IAdapterId fragment to be used to extract the object identification, i.e. which fragment type to look for to find the identifier of the object. If no identifier is provided, the object will be scanned an IAdapterId to be used.")]
        [Input("filter", "Optional filter for the results. If nothing is provided, all results will be used.")]
        [Input("meshResultDisplay", "Which kind of results to colour by.")]
        [Input("gradientOptions", "How to color the mesh, null defaults to `BlueToRed` with automatic range.")]
        [MultiOutput(0, "results", "A List of Lists of RenderMeshes, where there is one List per provided mesh and one element per meshResult that matched that mesh.")]
        [MultiOutput(1, "gradientOptions", "The gradientOptions that were used to colour the meshes.")]
        public static Output<List<List<RenderMesh>>, GradientOptions> DisplayMeshResults<TNode, TFace>(this IEnumerable<IMesh<TNode, TFace>> meshes, IEnumerable<IMeshResult<IMeshElementResult>> meshResults,
                      object objectIdentifier = null, ResultFilter filter = null, string meshResultDisplay = "", GradientOptions gradientOptions = null)
            where TNode : INode
            where TFace : IFace
        {
            if (meshes == null || meshes.Count() < 1)
            {
                Engine.Base.Compute.RecordError("No meshes found. Make sure that your meshes are input correctly.");
                return new Output<List<List<RenderMesh>>, GradientOptions>
                {
                    Item1 = new List<List<RenderMesh>>(),
                    Item2 = gradientOptions
                };
            }
            if (meshResults == null || meshResults.Count() < 1)
            {
                Engine.Base.Compute.RecordError("No results found. Make sure that your results are input correctly.");
                return new Output<List<List<RenderMesh>>, GradientOptions>
                {
                    Item1 = new List<List<RenderMesh>>(),
                    Item2 = gradientOptions
                };
            }

            if (gradientOptions == null)
                gradientOptions = new GradientOptions();

            List<IMesh<TNode, TFace>> meshList = meshes.ToList();

            // Map the MeshResults to Meshes
            List<List<IMeshResult<IMeshElementResult>>> mappedResults = meshList.MapResults(meshResults, "ObjectId", objectIdentifier, filter);
            //Get tuple with result name, property selector function and quantity attribute
            var resPropSelectorAndQuantity = meshResults.First().ResultItemValueProperty(meshResultDisplay);
            Func<IResultItem, double> resultPropertySelector = resPropSelectorAndQuantity?.Item2;

            if (resultPropertySelector == null)
            {
                return new Output<List<List<RenderMesh>>, GradientOptions>
                {
                    Item1 = new List<List<RenderMesh>>(),
                    Item2 = gradientOptions
                };
            }
            
            gradientOptions = gradientOptions.ApplyGradientOptions(mappedResults.SelectMany(x => x.SelectMany(y => y.Results.Select(resultPropertySelector))));

            //If unset, set name of gradient options to match property and unit
            if (string.IsNullOrWhiteSpace(gradientOptions.Name))
            {
                gradientOptions.Name = resPropSelectorAndQuantity.Item1;
                QuantityAttribute quantity = resPropSelectorAndQuantity.Item3;
                if (quantity != null)
                    gradientOptions.Name += $" [{quantity.SIUnit}]";
            }

            List<List<RenderMesh>> result = new List<List<RenderMesh>>();

            for (int i = 0; i < meshList.Count; i++)
            {
                result.Add(new List<RenderMesh>());
                for (int j = 0; j < mappedResults[i].Count; j++)
                {
                    result[i].Add(meshList[i].DisplayMeshResults(mappedResults[i][j], objectIdentifier, resultPropertySelector, gradientOptions.Gradient, gradientOptions.LowerBound, gradientOptions.UpperBound));
                }
            }

            return new Output<List<List<RenderMesh>>, GradientOptions>
            {
                Item1 = result,
                Item2 = gradientOptions
            };
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/

        [Description("Applies colour to a single IMesh based on a single MeshResult, i.e stress or force etc.")]
        [Output("renderMesh", "A coloured RenderMesh.")]
        private static RenderMesh DisplayMeshResults<TNode, TFace>(this IMesh<TNode, TFace> mesh, IMeshResult<IMeshElementResult> meshResult, object identifier,
                                             Func<IResultItem, double> propertyFuction, IGradient gradient, double from, double to)
            where TNode : INode
            where TFace : IFace
        {
            if (mesh?.Nodes == null || mesh?.Faces == null || mesh.Nodes.Count < 1 || mesh.Faces.Count < 1)
            {
                Engine.Base.Compute.RecordError("A mesh is null or invalid. Cannot display results for this mesh.");
                return null;
            }
            if (meshResult?.Results == null || meshResult.Results.Count < 1)
            {
                Engine.Base.Compute.RecordError("A result is null or invalid. Cannot display results for this mesh.");
                return null;
            }

            // Get the relevant values into a list

            List<RenderPoint> verts = new List<RenderPoint>();
            List<Face> faces;

            object smoothing = Base.Query.PropertyValue(meshResult, "Smoothing");
            MeshResultSmoothingType smoothingType = MeshResultSmoothingType.None;
            if (smoothing is MeshResultSmoothingType)
                smoothingType = (MeshResultSmoothingType)smoothing;

            // Order the MeshNodeResults by the IMesh Nodes or faces depending on smoothing
            List<List<IMeshElementResult>> tempMappedElementResults;
            if (smoothingType == MeshResultSmoothingType.ByFiniteElementCentres)
                tempMappedElementResults = mesh.Faces.MapResults(meshResult.Results, nameof(IMeshElementResult.MeshFaceId), identifier);
            else
                tempMappedElementResults = mesh.Nodes.MapResults(meshResult.Results, nameof(IMeshElementResult.NodeId), identifier);

            switch (smoothingType)
            {
                case MeshResultSmoothingType.None:
                    //  pair nodeValue as list<Dictionary<FaceId,nodeValue>>
                    //  all nodes are expected to have FaceIds
                    List<Dictionary<IComparable, double>> nodeValuePairs = tempMappedElementResults.Select(x => x.ToDictionary(y => y.MeshFaceId, y => propertyFuction(y))).ToList();
                    //  put the Faces in a Dictionary<FaceId,Face>
                    Func<IBHoMObject, string> faceIdentifierFunc = ObjectIdentifier(mesh.Faces.First(), identifier);
                    Dictionary<string, Face> faceDictionaryResult = mesh.Faces.ToDictionary(x => faceIdentifierFunc(x), x => x.Geometry());
                    Dictionary<string, Face> faceDictionaryRefrence = new Dictionary<string, Face>(faceDictionaryResult);

                    // Add all verticies to a list with their colour and update the Faces
                    for (int k = 0; k < mesh.Nodes.Count; k++)
                    {
                        foreach (KeyValuePair<IComparable, double> faceRelatedValue in nodeValuePairs[k])
                        {
                            verts.Add(new RenderPoint()
                            {
                                Point = mesh.Nodes[k].Position,
                                Colour = gradient.Color(faceRelatedValue.Value, from, to)
                            });
                            // Face management, faceResult points to verts and faceReference points to mesh.Nodes
                            Face faceResult = faceDictionaryResult[faceRelatedValue.Key.ToString()];
                            Face faceReference = faceDictionaryRefrence[faceRelatedValue.Key.ToString()];
                            Face newFace = new Face()
                            {
                                A = faceReference.A == k ? verts.Count - 1 : faceResult.A,
                                B = faceReference.B == k ? verts.Count - 1 : faceResult.B,
                                C = faceReference.C == k ? verts.Count - 1 : faceResult.C,
                                D = faceReference.IsQuad() ? faceReference.D == k ? verts.Count - 1 : faceResult.D : -1
                            };
                            faceDictionaryResult[faceRelatedValue.Key.ToString()] = newFace;
                        }
                    }
                    faces = faceDictionaryResult.Select(x => x.Value).ToList();
                    break;

                case MeshResultSmoothingType.ByPanel:
                case MeshResultSmoothingType.Global:
                    List<double> nodeValues = tempMappedElementResults.Select(x => propertyFuction(x[0])).ToList();

                    // Add all verticies to a list with their colour
                    for (int k = 0; k < mesh.Nodes.Count; k++)
                    {
                        verts.Add(new RenderPoint()
                        {
                            Point = mesh.Nodes[k].Position,
                            Colour = gradient.Color(nodeValues[k], from, to)
                        });
                    }
                    faces = mesh.Faces.Geometry().ToList();
                    break;

                case MeshResultSmoothingType.ByFiniteElementCentres:
                    //For smoothening by face centres, the same colour is displayed for the whole face.
                    //This is acheived by adding duplicate nodes (one for each face that connected to a node) and give
                    //All nodes that belong to a face the same colour

                    //Get out face results
                    List<double> faceValues = tempMappedElementResults.Select(x => propertyFuction(x[0])).ToList();
                    faces = new List<Face>();
                    for (int i = 0; i < mesh.Faces.Count; i++)
                    {
                        //Get colour for the face
                        System.Drawing.Color colour = gradient.Color(faceValues[i], from, to);
                        //Store face indecies for newly added vertecies
                        List<int> faceIndecies = new List<int>();
                        int vertsCount = verts.Count;
                        foreach (int nodeIndex in mesh.Faces[i].NodeListIndices)
                        {
                            verts.Add(new RenderPoint()
                            {
                                Point = mesh.Nodes[nodeIndex].Position,
                                Colour = colour
                            });
                            faceIndecies.Add(vertsCount++);
                        }
                        //Add new face pointing at the newly added vertex indecies
                        faces.Add(new Face
                        {
                            A = faceIndecies[0],
                            B = faceIndecies[1],
                            C = faceIndecies[2],
                            D = faceIndecies.Count > 3 ? faceIndecies[3] : -1
                        });
                        
                    }

                    break;
                case MeshResultSmoothingType.BySelection:
                default:
                    string msg = $"Unsupported SmoothingType: {smoothingType} detected, meshResult for ObjectId: {meshResult.ObjectId}";
                    if (meshResult is ICasedResult)
                        msg += $" and ResultCase: {(meshResult as ICasedResult).ResultCase}";

                    msg += " will be returned empty.";
                    Engine.Base.Compute.RecordError(msg);
                    return new RenderMesh();
            }

            return new RenderMesh() { Vertices = verts, Faces = faces };
        }

        /***************************************************/

    }
}

