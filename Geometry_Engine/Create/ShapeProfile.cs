/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.ObjectModel;
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Geometry;
using System;
using BH.Engine.Reflection;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISectionProfile ISectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            if (height < flangeThickness * 2 + rootRadius * 2 || height <= flangeThickness * 2)
            {
                InvalidRatioError("height","flangeThickness and rootRadius");
                return null;
            }

            if (width < webthickness + rootRadius * 2 + toeRadius * 2)
            {
                InvalidRatioError("width", "webthickness, rootRadius and toeRadius");
                return null;
            }

            if (toeRadius > flangeThickness)
            {
                InvalidRatioError("toeRadius", "flangeThickness");
                return null;
            }

            if (height <= 0 || width <= 0 || webthickness <= 0 || flangeThickness<= 0 || rootRadius< 0 ||toeRadius< 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null; 
            }

            List<ICurve> curves = IProfileCurves(flangeThickness, width, flangeThickness, width, webthickness, height - 2 * flangeThickness, rootRadius, toeRadius,0);
            return new ISectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/

        public static BoxProfile BoxProfile(double height, double width, double thickness, double outerRadius, double innerRadius)
        {
            if (thickness >= height / 2)
            {
                InvalidRatioError("thickness", "height");
                return null;
            }

            if (thickness >= width / 2)
            {
                InvalidRatioError("thickness", "width");
                return null;
            }

            if (outerRadius > height / 2)
            {
                InvalidRatioError("outerRadius", "height");
                return null;
            }

            if (outerRadius > width / 2)
            {
                InvalidRatioError("outerRadius", "width");
                return null;
            }

            if (innerRadius * 2 > width - thickness * 2)
            {
                InvalidRatioError("innerRadius","width and thickness");
                return null;
            }

            if (innerRadius * 2 > height - thickness * 2)
            {
                InvalidRatioError("innerRadius", "height and thickness");
                return null;
            }

            //if (Math.Sqrt(2) * thickness <= Math.Sqrt(2) * outerRadius - outerRadius - Math.Sqrt(2) * innerRadius + innerRadius)
            //{
            //    InvalidRatioError("thickness", "outerRadius and innerRadius");
            //    return null;
            //}

            //if (height <= 0 || width <= 0 || thickness <= 0 || outerRadius < 0 || innerRadius < 0)
            //{
            //    Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
            //    return null;
            //}

            List<ICurve> curves = BoxProfileCurves(width, height, thickness, thickness, innerRadius, outerRadius);
            return new BoxProfile(height, width, thickness, outerRadius, innerRadius, curves);
        }

        /***************************************************/

        public static AngleProfile AngleProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalZ = false, bool mirrorAboutLocalY = false)
        {
            if (height < flangeThickness + rootRadius + toeRadius)
            {
                InvalidRatioError("height", "flangeThickness, rootRadius and toeRadius");
                return null;
            }

            if (width < webthickness + rootRadius + toeRadius)
            {
                InvalidRatioError("width", "webthickness, rootRadius and toeRadius");
                return null;
            }

            if (flangeThickness < toeRadius)
            {
                InvalidRatioError("flangeThickness", "toeRadius");
                return null;
            }

            if (webthickness < toeRadius)
            {
                InvalidRatioError("webthickness", "toeRadius");
                return null;
            }

            if (height <= 0 || width <= 0 || webthickness <= 0 || flangeThickness <= 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = AngleProfileCurves(width, height, flangeThickness, webthickness, rootRadius, toeRadius);

            if (mirrorAboutLocalZ)
                curves = curves.MirrorAboutLocalZ();
            if (mirrorAboutLocalY)
                curves = curves.MirrorAboutLocalY();

            return new AngleProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ, mirrorAboutLocalY, curves);
        }

        /***************************************************/

        public static ChannelProfile ChannelProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalZ = false)
        {
            if (height < flangeThickness * 2 + rootRadius * 2 || height <= flangeThickness * 2)
            {
                InvalidRatioError("height", "flangeThickness and rootRadius");
                return null;
            }

            if (width < webthickness + rootRadius + toeRadius)
            {
                InvalidRatioError("width", "webthickness, toeRadius and rootRadius");
                return null;
            }

            if (flangeThickness < toeRadius)
            {
                InvalidRatioError("flangeThickness", "toeRadius");
                return null;
            }

            if (height <= 0 || width <= 0 || webthickness <= 0 || flangeThickness <= 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = ChannelProfileCurves(height, width, webthickness, flangeThickness, rootRadius, toeRadius);

            if (mirrorAboutLocalZ)
                curves = curves.MirrorAboutLocalZ();

            return new ChannelProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ, curves);
        }

        /***************************************************/

        public static CircleProfile CircleProfile(double diameter)
        {
            if (diameter <= 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }
            List<ICurve> curves = CircleProfileCurves(diameter / 2);
            return new CircleProfile(diameter, curves);
        }

        /***************************************************/

        public static FabricatedBoxProfile FabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            if (height < topFlangeThickness + botFlangeThickness + 2 * Math.Sqrt(2) * weldSize || height <= topFlangeThickness + botFlangeThickness)
            {
                InvalidRatioError("height", "topFlangeThickness, botFlangeThickness and weldSize");
                return null;
            }

            if (width < webThickness * 2 + 2 * Math.Sqrt(2) * weldSize || width <= webThickness * 2)
            {
                InvalidRatioError("width", "webThickness and weldSize");
                return null;
            }

            if (height <= 0 || width <= 0 || webThickness <= 0 || topFlangeThickness <= 0 || botFlangeThickness <= 0 || weldSize < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = FabricatedBoxProfileCurves(width, height, webThickness, topFlangeThickness, botFlangeThickness, weldSize);
            return new FabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, weldSize, curves);
        }

        /***************************************************/

        public static GeneralisedFabricatedBoxProfile GeneralisedFabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness = 0.0, double botFlangeThickness = 0.0, double topCorbelWidth = 0.0, double botCorbelWidth = 0.0)
        {
            if (webThickness >= width / 2)
            {
                InvalidRatioError("webThickness", "width");
                return null;
            }

            if (height <= topFlangeThickness + botFlangeThickness)
            {
                InvalidRatioError("height","topFlangeThickness and botFlangeThickness");
                return null;
            }

            if (height <= 0 || width <= 0 || webThickness <= 0 || topFlangeThickness <= 0 || botFlangeThickness <= 0 || topCorbelWidth < 0 || botCorbelWidth < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = GeneralisedFabricatedBoxProfileCurves(height, width, webThickness, topFlangeThickness, botFlangeThickness, topCorbelWidth, topCorbelWidth, botCorbelWidth, botCorbelWidth);
            return new GeneralisedFabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, topCorbelWidth, topCorbelWidth, botCorbelWidth, botCorbelWidth, curves);
        }

        /***************************************************/

        public static KiteProfile KiteProfile(double width1, double angle1, double thickness)
        {
            if ((width1 * Math.Sin(angle1 / 2) / Math.Sqrt(2)) / (Math.Sin(Math.PI * 0.75 - (angle1 / 2))) <= thickness)
            {
                InvalidRatioError("thickness", "width and angle1");
                return null;
            }

            if (width1 <= 0 || angle1 <= 0 || thickness <= 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = KiteProfileCurves(width1, angle1, thickness);
            return new KiteProfile(width1, angle1, thickness, curves);
        }

        /***************************************************/

        public static FabricatedISectionProfile FabricatedISectionProfile(double height, double topFlangeWidth, double botFlangeWidth, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            if (height < topFlangeThickness + botFlangeThickness + 2 * Math.Sqrt(2) * weldSize || height <= topFlangeThickness + botFlangeThickness)
            {
                InvalidRatioError("height","topFlangeThickness, botFlangeThickness and weldSize");
                return null;
            }

            if (botFlangeWidth < webThickness + 2 * Math.Sqrt(2) * weldSize)
            {
                InvalidRatioError("botFlangeWidth", "webThickness and weldSize");
                return null;
            }

            if (topFlangeWidth < webThickness + 2 * Math.Sqrt(2) * weldSize)
            {
                InvalidRatioError("topFlangeWidth", "webThickness and weldSize");
                return null;
            }

            if (height <= 0 || topFlangeWidth <= 0 || botFlangeWidth <= 0 || webThickness <= 0 || topFlangeThickness <= 0 || botFlangeThickness <= 0 || weldSize < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = IProfileCurves(topFlangeThickness, topFlangeWidth, botFlangeThickness, botFlangeWidth, webThickness, height - botFlangeThickness - topFlangeThickness,0,0,weldSize);
            return new FabricatedISectionProfile(height, topFlangeWidth, botFlangeWidth, webThickness, topFlangeThickness, botFlangeThickness, weldSize, curves);
        }

        /***************************************************/

        [Description("Creates a single FreeFormProfile from a collection of ICurves. \n" +
                     "Checks if it's a valid profile and attempts to fix;  \n" + 
                     " - coplanarity with eachother, by projecting onto the plane of the curve with the biggest area. \n" + 
                     " - coplanarity with XYPlane, by rotating the curves to align. \n" + 
                     " - curves on the XYPlane, by translating from one controlpoint to the origin. \n" +
                     "Checks if it's a valid profile; If they're closed, Not zero area, curve curve intersections, selfintersections. ")]
        public static FreeFormProfile FreeFormProfile(IEnumerable<ICurve> edges)
        {
            IEnumerable<ICurve> result = edges.ToList();
            
            List<Point> cPoints = edges.SelectMany(x => x.IControlPoints()).ToList();

            // Any Point not on the XY-Plane
            if (cPoints.Any(x => Math.Abs(x.Z) > Tolerance.Distance))
            {
                // Is Planar
                Plane plane = Compute.FitPlane(cPoints);
                bool failedProject = false;
                if (cPoints.Any(x => x.Distance(plane) > Tolerance.Distance))
                {
                    Reflection.Compute.RecordWarning("The Profiles curves are not Planar");
                    try
                    {
                        // Get biggest contributing curve and fit a plane to it
                        plane = Compute.IJoin(edges.ToList()).OrderBy(x => x.Area()).Last().ControlPoints().FitPlane();

                        result = edges.Select(x => x.IProject(plane)).ToList();
                        Reflection.Compute.RecordWarning("The Profiles curves have been projected onto a plane fitted through the biggest curve's control points.");
                        cPoints = result.SelectMany(x => x.IControlPoints()).ToList();
                    }
                    catch
                    {
                        failedProject = true;
                    }
                }

                if (!failedProject)
                {
                    // Is Coplanar with XY
                    if (plane.Normal.IsParallel(oM.Geometry.Vector.ZAxis) == 0)
                    {
                        Reflection.Compute.RecordWarning("The Profiles curves are not coplanar with the XY-Plane. Automatic orientation has occured.");

                        plane.Normal = plane.Normal.Z > 0 ? plane.Normal : -plane.Normal;
                        double rad = plane.Normal.Angle(oM.Geometry.Vector.ZAxis);

                        Line axis = plane.PlaneIntersection(oM.Geometry.Plane.XY);
                        result = result.Select(x => x.IRotate(axis.Start, axis.TangentAtParameter(0), rad)).ToList();
                        cPoints = result.SelectMany(x => x.IControlPoints()).ToList();
                    }

                    // Is on XY
                    if (cPoints.FirstOrDefault().Distance(oM.Geometry.Plane.XY) > Tolerance.Distance)
                    {
                        Reflection.Compute.RecordWarning("The Profiles curves are not on the XY-Plane. Automatic translation has occured.");
                        Point p = cPoints.FirstOrDefault();
                        Vector v = new oM.Geometry.Vector() { X = p.X, Y = p.Y, Z = p.Z };

                        result = result.Select(x => x.ITranslate(-v)).ToList();
                    }
                }
            }

            if (!result.Any(x => x.ISubParts().Any(y => y is NurbsCurve)))
            {
                // Join the curves
                List<PolyCurve> joinedCurves = Compute.IJoin(result.ToList()).ToList();

                // Is Closed
                if (joinedCurves.Any(x => !x.IsClosed()))
                    Reflection.Compute.RecordWarning("The Profiles curves does not form closed curves");

                // Has non-zero area
                if (joinedCurves.Any(x => x.IArea() < Tolerance.Distance))
                    Reflection.Compute.RecordWarning("One or more of the profile curves have close to zero area.");

                if (!joinedCurves.Any(x => x.ISubParts().Any(y => y is Ellipse)))
                {
                    // Check curve curve Intersections
                    bool intersects = false;
                    for (int i = 0; i < joinedCurves.Count - 1; i++)
                    {
                        for (int j = i + 1; j < joinedCurves.Count; j++)
                        {
                            if (joinedCurves[i].CurveIntersections(joinedCurves[j]).Count > 0)
                            {
                                intersects = true;
                                break;
                            }
                        }
                        if (intersects)
                            break;
                    }
                    if (intersects)
                        Engine.Reflection.Compute.RecordWarning("The Profiles curves are intersecting eachother.");
                }
                // Is SelfInteersecting
                if (joinedCurves.Any(x => x.IsSelfIntersecting()))
                    Engine.Reflection.Compute.RecordWarning("One or more of the Profiles curves is intersecting itself.");

            }

            return new FreeFormProfile(result);
        }

        /***************************************************/

        public static RectangleProfile RectangleProfile(double height, double width, double cornerRadius)
        {
            if (cornerRadius > height / 2)
            {
                InvalidRatioError("cornerRadius", "height");
                return null;
            }

            if (cornerRadius > width / 2)
            {
                InvalidRatioError("cornerRadius", "width");
                return null;
            }

            if (height <= 0 || width <= 0 || cornerRadius < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }
            List<ICurve> curves = RectangleProfileCurves(width, height, cornerRadius);
            return new RectangleProfile(height, width, cornerRadius, curves);
        }

        /***************************************************/

        public static TSectionProfile TSectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalY = false)
        {
            if (height < flangeThickness + rootRadius)
            {
                InvalidRatioError("height", "flangeThickness and rootRadius");
                return null;
            }

            if (width < webthickness + 2 * rootRadius + 2 * toeRadius)
            {
                InvalidRatioError("width", "webThickess, rootRadius and toeRadius");
                return null;
            }

            if (toeRadius > flangeThickness)
            {
                InvalidRatioError("toeTadius", "flangeThickness");
                return null;
            }

            if (height <= 0 || width <= 0 || webthickness <= 0 || flangeThickness <= 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = TeeProfileCurves(flangeThickness, width, webthickness, height - flangeThickness, rootRadius, toeRadius);

            if (mirrorAboutLocalY)
                curves = curves.MirrorAboutLocalY();

            return new TSectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalY, curves);
        }

        /***************************************************/

        public static GeneralisedTSectionProfile GeneralisedTSectionProfile(double height, double webThickness, double leftOutstandWidth, double leftOutstandThickness, double rightOutstandWidth, double rightOutstandThickness, bool mirrorAboutLocalY = false)
        {
            if (height < leftOutstandThickness)
            {
                InvalidRatioError("height", "leftOutstandThickness");
                return null;
            }

            if (height < rightOutstandThickness)
            {
                InvalidRatioError("height", "rightOutstandThickness");
                return null;
            }

            if (leftOutstandThickness <= 0 && leftOutstandWidth > 0 || leftOutstandWidth <= 0 && leftOutstandThickness > 0)
            {
                InvalidRatioError("leftOutstandThickness","leftOutstandWidth");
                return null;
            }

            if (rightOutstandThickness <= 0 && rightOutstandWidth > 0 || rightOutstandWidth <= 0 && rightOutstandThickness > 0)
            {
                InvalidRatioError("rightOutstandThickness", "rightOutstandWidth");
                return null;
            }

            if (height <= 0 || webThickness <= 0 || leftOutstandThickness < 0 || leftOutstandWidth < 0 || rightOutstandThickness < 0 || rightOutstandWidth < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = GeneralisedTeeProfileCurves(height, webThickness, leftOutstandWidth, leftOutstandThickness, rightOutstandWidth, rightOutstandThickness);

            if (mirrorAboutLocalY)
                curves = curves.MirrorAboutLocalY();

            return new GeneralisedTSectionProfile(height, webThickness, leftOutstandWidth, leftOutstandThickness, rightOutstandWidth, rightOutstandThickness,mirrorAboutLocalY, curves);
        }

        /***************************************************/

        public static TubeProfile TubeProfile(double diameter, double thickness)
        {
            if (thickness >= diameter / 2)
            {
                InvalidRatioError("diameter", "thickness");
                return null;
            }

            //if (diameter <= 0 || thickness <= 0)
            //{
            //    Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
            //    return null;
            //}

            List<ICurve> curves = TubeProfileCurves(diameter / 2, thickness);
            return new TubeProfile(diameter, thickness, curves);
        }

        /***************************************************/

        public static TaperedProfile TaperedProfile(List<decimal> positions, List<IProfile> profiles)
        {
            if (positions.Count != profiles.Count)
            {
                Engine.Reflection.Compute.RecordError("Number of positions and profiles provided are not equal");
                return null;
            }
            else if (positions.Exists((decimal d) => { return d > 1; }) || positions.Exists((decimal d) => { return d < 0; }))
            {
                Engine.Reflection.Compute.RecordError("Positions must exist between 0 and 1 (inclusive)");
                return null;
            }
            else if (!positions.Contains(0) || !positions.Contains(1))
            {
                Engine.Reflection.Compute.RecordError("Start and end profile must be provided");
                return null;
            }

            SortedDictionary<decimal, IProfile> profileDict = new SortedDictionary<decimal, IProfile>();

            for (int i = 0; i < positions.Count; i++)
            {
                profileDict[positions[i]] = profiles[i];
            }

            return new TaperedProfile(profileDict);
        }

        /***************************************************/

        public static TaperedProfile TaperedProfile(IProfile startProfile, IProfile endProfile)
        {
            SortedDictionary<decimal, IProfile> profileDict = new SortedDictionary<decimal, IProfile>();

            profileDict.Add(0, startProfile);
            profileDict.Add(1, endProfile);

            return new TaperedProfile(profileDict);
        }

        /***************************************************/

        [NotImplemented]
        public static ZSectionProfile ZSectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            throw new NotImplementedException();
            //TODO: Section curves for z-profile
            //List<ICurve> curves = ZProfileCurves(flangeThickness, width, webthickness, height - flangeThickness, rootRadius, toeRadius);
            //return new ZSectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> MirrorAboutLocalY(this List<ICurve> curves)
        {
            Plane plane = oM.Geometry.Plane.XZ;
            return curves.Select(x => x.IMirror(plane)).ToList();
        }

        /***************************************************/

        private static List<ICurve> MirrorAboutLocalZ(this List<ICurve> curves)
        {
            Plane plane = oM.Geometry.Plane.YZ;
            return curves.Select(x => x.IMirror(plane)).ToList();
        }

        /***************************************************/

        private static void InvalidRatioError(string first, string second)
        {
            Engine.Reflection.Compute.RecordError("The ratio of the " + first + " in relation to the " + second + " makes section inconceivable");
        }

        /***************************************************/
    }
}
