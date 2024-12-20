/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Text;
using System.Threading.Tasks;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Geometry;
using BH.oM.Spatial;
using BH.oM.Lighting.Elements;

using BH.Engine.Geometry;
using BH.Engine.Spatial;

namespace BH.Engine.Lighting
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Create a collection of luminaires along a curve based on an exact and a target point.")]
        [Input("crv", "The line to place the luminaires on.")]
        [Input("exactSpacing", "Exact spacing between luminaires along the curve.")]
        [Input("target", "The target point (all created luminaires will be oriented towards this point).")]
        [Input("centered", "Center points on curve (if false, the first point will be at the start of the curve).")]
        [InputFromProperty("type")]
        [Input("name", "The name to apply to the luminaires (each name will include this plus the number of luminaires in the sequence).")]
        [Output("luminaires", "A collection of Luminaires created along the input curve.")]
        public static List<Luminaire> Luminaire(this ICurve crv, double exactSpacing, Point target, bool centered = true, LuminaireType type = null, string name = "")
        {
            ICurve trimmedCrv;

            double crvLen = crv.Length();
            if (exactSpacing == 0) return null;
            if (centered)
            {
                trimmedCrv = crv.IExtend(-0.5*(crvLen % exactSpacing), -0.5*(crvLen % exactSpacing));
            }
            else
            {
                trimmedCrv = crv.IExtend(0, -(crvLen % exactSpacing));
            }
            return Luminaire(trimmedCrv, exactSpacing, target, type, name);
        }

        /***************************************************/       
        
        [Description("Create a collection of luminaires along a curve based on a maximum spacing and a target point.")]
        [Input("crv", "The line to place the luminaires on.")]
        [Input("maxSpacing", "Exact spacing between luminaires along the curve.")]
        [Input("target", "The target point (all created luminaires will be oriented towards this point).")]
        [InputFromProperty("type")]
        [Input("name", "The name to apply to the luminaires (each name will include this plus the number of luminaires in the sequence).")]
        [Output("luminaires", "A collection of Luminaires created along the input curve.")]
        public static List<Luminaire> Luminaire(this ICurve crv, double maxSpacing, Point target, LuminaireType type = null, string name = "")
        {
            double crvLen = crv.Length();
            if (maxSpacing == 0) return null;
            int count = (int)Math.Ceiling(crvLen / maxSpacing) + 1;
            return Luminaire(crv, count, target, type, name);
        }

        /***************************************************/

        [Description("Create a collection of luminaires along a curve based on a count and a target point.")]
        [Input("crv", "The line to place the luminaires on.")]
        [Input("count", "Number of luminaires to place along the curve.")]
        [Input("target", "The target point (all created luminaires will be oriented towards this point).")]
        [InputFromProperty("type")]
        [Input("name", "The name to apply to the luminaires (each name will include this plus the number of luminaires in the sequence).")]
        [Output("luminaires", "A collection of Luminaires created along the input curve.")]
        public static List<Luminaire> Luminaire(this ICurve crv, int count, Point target, LuminaireType type = null, string name = "")
        {
            List<Luminaire> luminaires = new List<Luminaire>();
            List<Point> pts = crv.SamplePoints(count);
            for (int i = 0; i < pts.Count; i++)
            {
                Vector dir = BH.Engine.Geometry.Create.Vector(pts[i], target);
                Luminaire lum = Create.Luminaire(pts[i], dir, type, name + "_" + i.ToString());
                luminaires.Add(lum);
            }
            return luminaires;
        }

        /***************************************************/

        [Description("Create a collection of luminaires along a curve based on a maximum spacing and an orientation direction.")]
        [Input("crv", "The line to place the luminaires on.")]
        [Input("maxSpacing", "Maximum spacing between luminaires along the curve.")]
        [Input("dir", "The direction to orient created luminaires.")]
        [InputFromProperty("type")]
        [Input("name", "The name to apply to the luminaires (each name will include this plus the number of luminaire in the sequence).")]
        [Output("luminaires", "A collection of Luminaires created along the input curve.")]
        public static List<Luminaire> Luminaire(this ICurve crv, double maxSpacing, Vector dir, LuminaireType type = null, string name = "")
        {
            double crvLen = crv.Length();
            if (maxSpacing == 0) return null;
            int count = (int)Math.Ceiling(crvLen / maxSpacing) + 1;
            return Luminaire(crv, count, dir, type, name);
        }

        /***************************************************/

        [Description("Create a collection of luminaires along a curve based on a count and an orientation direction.")]
        [Input("crv", "The line to place the luminaires on.")]
        [Input("count", "Number of luminaires to place along the curve.")]
        [Input("dir", "The direction to orient created luminaires.")]
        [InputFromProperty("type")]
        [Input("name", "The name to apply to the luminaires (each name will include this plus the number of luminaire in the sequence).")]
        [Output("luminaires", "A collection of Luminaires created along the input curve.")]
        public static List<Luminaire> Luminaire(this ICurve crv, int count, Vector dir, LuminaireType type = null, string name = "")
        {
            List<Luminaire> luminaires = new List<Luminaire>();
            List<Point> pts = crv.SamplePoints(count);
            for (int i = 0; i < pts.Count; i++)
            {
                Luminaire lum = Create.Luminaire(pts[i], dir, type, name + "_" + i.ToString());
                luminaires.Add(lum);
            }
            return luminaires;
        }

        /***************************************************/

        [Description("Create a luminaire at a point based on directional orientation.")]
        [Input("pt", "The point to place the luminaire.")]
        [Input("orientation", "The direction the luminaire is pointed towards.")]
        [InputFromProperty("type")]
        [Input("name", "The name of the luminaire.")]
        [Output("luminaire", "The created luminaire.")]
        public static Luminaire Luminaire(this Point pt, Vector orientation = null, LuminaireType type = null, string name = "")
        {
            Basis basis = null;

            if (orientation != null && orientation.Length() != 0)
            {
                double angle = orientation.Angle(Vector.ZAxis);
                if (angle < Tolerance.Angle)
                    basis = Engine.Geometry.Create.Basis(Vector.XAxis, Vector.YAxis);
                else if (Math.Abs(Math.PI - angle) < Tolerance.Angle)
                    basis = Engine.Geometry.Create.Basis(Vector.XAxis.Reverse(), Vector.YAxis);
                else
                {
                    Vector x = orientation.CrossProduct(Vector.ZAxis).Project(Plane.XY).Normalise();
                    Vector y = orientation.CrossProduct(x).Normalise();
                    basis = Engine.Geometry.Create.Basis(x, y);
                }

            }
        
            Luminaire luminaire = new Luminaire
            {
                Position = pt,
                Orientation = basis,
                LuminaireType = type,
                Name = name
            };
            return luminaire;
        }

    }
}






