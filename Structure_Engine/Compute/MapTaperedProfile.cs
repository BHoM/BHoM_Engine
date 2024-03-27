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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.SectionProperties;
using BH.Engine.Base;
using BH.Engine.Spatial;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Maps a TaperedProfile to a series of sequential Bars by interpolating the profiles at the startNode and endNode of each bar using a polynomial defined by the interpolationOrder. " +
            "For nonlinear profiles a concave profile is achieved by setting the larger profile at the smallest position. To achieve a convex profile, the larger profile must be at the largest position.")]
        [Input("bars", "The Bars in sequential order for the TaperedProfile to be mapped to.")]
        [Input("section", "The section containing the TaperedProfile to be mapped to the series of Bars.")]
        [Output("bars", "The Bars with interpolated SectionProperties based on the TaperedProfile provided.")]
        public static List<Bar> MapTaperedProfile(List<Bar> bars, IGeometricalSection section)
        {
            if (bars.Any(x => x.IsNull()) || section.IsNull())
                return null;

            List<Bar> newBars = bars.ShallowClone();

            if (!(section.SectionProfile is TaperedProfile))
            {
                Base.Compute.RecordError("Section provided does not contain a TaperedProfile.");
                foreach (Bar newBar in newBars)
                {
                    newBar.SectionProperty = section;
                }
            }
            else
            {
                List<TaperedProfile> mappedTaperedProfiles = MapTaperedProfile(bars, section.SectionProfile as TaperedProfile);
                List<IGeometricalSection> sections = mappedTaperedProfiles.Select(x => Create.SectionPropertyFromProfile(x, section.Material)).ToList();
                for (int i = 0; i < sections.Count; i++)
                {
                    sections[i].Name = section.Name + "-s" + i;
                    newBars[i].SectionProperty = sections[i];
                }
            }

            return newBars;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<TaperedProfile> MapTaperedProfile(List<Bar> bars, TaperedProfile taperedProfile)
        {
            //Check profiles have the same shape
            if (taperedProfile.Profiles.Values.Any(x => x.Shape != taperedProfile.Profiles.Values.First().Shape))
            {
                Base.Compute.RecordError("MapTaperedProfile does not support TaperedProfiles with different ShapeProfiles.");
                return null;
            }

            //Get geometry of Bars
            List<Line> lines = bars.Select(x => x.Centreline()).ToList();

            //Checks bars form a single line
            List<Polyline> centrelines = lines.Join();
            if (lines.Join().Count > 1)
            {
                Base.Compute.RecordError("Bars provided do not form a single continuous line.");
                return null;
            }


            Polyline centreline = centrelines[0];

            //Check bars are in order
            List<Point> midpoints = lines.Select(x => x.PointAtParameter(0.5)).ToList();
            List<Point> orderedMidpoints = midpoints.SortAlongCurve(centreline);

            if (!midpoints.SequenceEqual(orderedMidpoints))
            {
                Base.Compute.RecordError("Bars provided are not sorted.");
                return null;
            }

            List<double> originalPositions = new List<double>(taperedProfile.Profiles.Keys);
            List<IProfile> originalProfiles = new List<IProfile>(taperedProfile.Profiles.Values);

            List<TaperedProfile> taperedProfiles = new List<TaperedProfile>();

            //For each bar interpolate the profiles as necessary and create a TaperedProfile 
            foreach (Bar bar in bars)
            {
                double startPosition = centreline.ParameterAtPoint(bar.Start.Position);
                double endPosition = centreline.ParameterAtPoint(bar.End.Position);
                double newLength = endPosition - startPosition;

                List<double> positions = new List<double>(originalPositions);
                if (!positions.Contains(startPosition))
                    positions.Add(startPosition);
                if (!positions.Contains(endPosition))
                    positions.Add(endPosition);
                positions.Sort();

                int startIndex = positions.IndexOf(startPosition);
                int endIndex = positions.IndexOf(endPosition);

                //These are required to create the new TaperedProfile mapped to the Bar
                List<IProfile> newProfiles = new List<IProfile>();
                List<double> newPositions = new List<double>();
                List<int> newInterpolationOrders = GetInterpolationOrder(taperedProfile, positions);


                //Cycle through the positions in the extents of the Bar to get the newProfile and newPosition
                for (int i = startIndex; i < endIndex + 1; i++)
                {
                    double position = positions[i];
                    IProfile newProfile;
                    double newPosition = 0;

                    if (position == 0)
                    {
                        //If the position is the same as the start of the TaperedProfile
                        taperedProfile.Profiles.TryGetValue(0, out newProfile);
                        newPosition = 0;
                    }
                    else if (position == 1)
                    {
                        //If the position is the same as the end of the TaperedProfile
                        taperedProfile.Profiles.TryGetValue(1, out newProfile);
                        newPosition = (1 - startPosition) / newLength;
                    }
                    else if (taperedProfile.Profiles.ContainsKey(position))
                    {
                        //If the position is equal to an existing position in the TaperedProfile
                        taperedProfile.Profiles.TryGetValue(position, out newProfile);
                        newPosition = (positions[i] - startPosition) / newLength;
                    }
                    else
                    {
                        //If the position is between existing positions in the TaperedProfile
                        IProfile preProfile;
                        IProfile postProfile;

                        double prePosition = positions[i - 1];
                        taperedProfile.Profiles.TryGetValue(prePosition, out preProfile);
                        //If the both new positions are between original positions 
                        if (preProfile == null)
                        {
                            taperedProfile.Profiles.TryGetValue(positions[i - 2], out preProfile);
                            prePosition = positions[i - 2];
                        }

                        double postPosition = positions[i + 1];
                        taperedProfile.Profiles.TryGetValue(postPosition, out postProfile);
                        //If the both new positions are between original positions 
                        if (postProfile == null)
                        {
                            taperedProfile.Profiles.TryGetValue(positions[i + 2], out postProfile);
                            postPosition = positions[i + 2];
                        }

                        double interpolationPosition = (position - prePosition) / (postPosition - prePosition);

                        //One less interpolationOrder than positions/profiles
                        int interpolationIndex = Math.Min(i, newInterpolationOrders.Count - 1);
                        int newInterpolationOrder = newInterpolationOrders[interpolationIndex];

                        newProfile = Spatial.Compute.IInterpolateProfile(preProfile, postProfile, interpolationPosition, newInterpolationOrder);
                    }
                    newProfiles.Add(newProfile);
                    newPositions.Add(position);
                }
                if (endIndex == 1 || endIndex - startIndex == 1)
                    taperedProfiles.Add(Spatial.Create.TaperedProfile(newPositions, newProfiles, newInterpolationOrders.GetRange(startIndex, 1)));
                else
                    taperedProfiles.Add(Spatial.Create.TaperedProfile(newPositions, newProfiles, newInterpolationOrders.GetRange(startIndex, (endIndex - startIndex))));
            }

            return taperedProfiles;
        }

        /***************************************************/

        private static List<int> GetInterpolationOrder(TaperedProfile taperedProfile, List<double> newPositions)
        {
            List<int> interpolationOrders = new List<int>();
            List<double> originalPositions = new List<double>(taperedProfile.Profiles.Keys);
            for (int i = 0; i < newPositions.Count - 1; i++)
            {
                bool containsPosition = taperedProfile.Profiles.ContainsKey(newPositions[i]);
                bool containsNextPosition = taperedProfile.Profiles.ContainsKey(newPositions[i + 1]);
                if (containsPosition)
                {
                    interpolationOrders.Add(taperedProfile.InterpolationOrder[originalPositions.IndexOf(newPositions[i])]);
                }
                else if (!containsPosition && !containsNextPosition)
                {
                    interpolationOrders.Add(taperedProfile.InterpolationOrder[originalPositions.IndexOf(newPositions[i - 1])]);
                }
                else if (containsNextPosition)
                {
                    if (taperedProfile.Profiles.ContainsKey(newPositions[i - 1]))
                    {
                        interpolationOrders.Add(taperedProfile.InterpolationOrder[originalPositions.IndexOf(newPositions[i - 1])]);
                    }
                    else
                    {
                        interpolationOrders.Add(taperedProfile.InterpolationOrder[originalPositions.IndexOf(newPositions[i - 2])]);
                    }
                }
            }

            return interpolationOrders;
        }

        /***************************************************/

    }
}


