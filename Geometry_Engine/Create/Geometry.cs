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
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random IGeometry based on a seed. If no seed is provided, a random one will be generated. If Box is provided, the resuling geometry will be contained within the box.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the goemetry.")]
        [Output("geom", "The generated random IGeometry.")]
        public static IGeometry RandomGeometry(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = NextSeed();
            Random rnd = new Random(seed);
            return RandomGeometry(rnd, box);
        }

        /***************************************************/

        [Description("Creates a random IGeometry using the provided Random class. If Box is provided, the resuling geometry will be contained within the box.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the goemetry.")]
        [Output("geom", "The generated random IGeometry.")]
        public static IGeometry RandomGeometry(Random rnd, BoundingBox box = null)
        {
            int nb = rnd.Next(13);
            switch(nb)
            {
                case 0:
                    return RandomArc(rnd, box);
                case 1:
                    return RandomCircle(rnd, box);
                case 2:
                    return RandomEllipse(rnd, box);
                case 3:
                    return RandomLine(rnd, box);
                case 4:
                    return RandomPolyline(rnd, box);
                case 5:
                    return RandomExtrusion(rnd, box);
                case 6:
                    return RandomLoft(rnd, box);
                case 7:
                    return RandomPipe(rnd, box);
                case 8:
                    return RandomPolySurface(rnd, box);
                case 9:
                    return RandomMesh(rnd, box);
                case 10:
                    return RandomCompositeGeometry(rnd, box);
                case 11:
                    return RandomPlane(rnd, box);
                default:
                    return RandomPoint(rnd, box);
            }
        }

        /***************************************************/

        [Description("Creates a random IGeometry of the specified type using the provided Random class. If Box is provided, the resuling geometry will be contained within the box.")]
        [Input("type", "Type of geometry to create. Must be a type of IGeometry.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("box", "Optional containing box. The geometry created will be limited to the bounding box. If no box is provided, values between 0 and 1 will be used when generating properties for the goemetry.")]
        [Output("geom", "The generated random IGeometry.")]
        public static IGeometry RandomGeometry(Type type, Random rnd, BoundingBox box = null)
        {
            if (!typeof(IGeometry).IsAssignableFrom(type))
                return null;

            switch (type.Name)
            {
                case "Arc":
                    return RandomArc(rnd, box);
                case "BoundingBox":
                    return RandomBoundingBox(rnd, box);
                case "Circle":
                    return RandomCircle(rnd, box);
                case "CompositeGeometry":
                    return RandomCompositeGeometry(rnd, box);
                case "Ellipse":
                    return RandomEllipse(rnd, box);
                case "Extrusion":
                    return RandomExtrusion(rnd, box);
                case "ICurve":
                    return RandomCurve(rnd, box);
                case "IGeometry":
                    return RandomGeometry(rnd, box);
                case "ISurface":
                    return RandomSurface(rnd, box);
                case "Line":
                    return RandomLine(rnd, box);
                case "Loft":
                    return RandomLoft(rnd, box);
                case "Mesh":
                    return RandomMesh(rnd, box);
                case "NurbsCurve":
                    return RandomNurbsCurve(rnd, box);
                case "NurbsSurface":
                    return RandomNurbsSurface(rnd, box);
                case "Pipe":
                    return RandomPipe(rnd, box);
                case "Plane":
                    return RandomPlane(rnd, box);
                case "Point":
                    return RandomPoint(rnd, box);
                case "PolyCurve":
                    return RandomPolyCurve(rnd, box);
                case "Polyline":
                    return RandomPolyline(rnd, box);
                case "Quaternion":
                    return RandomQuaternion(rnd);
                case "PolySurface":
                    return RandomPolySurface(rnd, box);
                case "TransformMatrix":
                    return RandomMatrix(rnd);
                case "Vector":
                    return RandomVector(rnd, box);
                default:
                    return null;
            }
        }
        
        /***************************************************/
    }
}



