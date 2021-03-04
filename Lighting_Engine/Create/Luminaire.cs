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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Reflection.Attributes;
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

        [Description("Create a collection of luminaires along a curve based on a count and a target point.")]
        [Input("crv", "The line to place the luminaires on.")]
        [Input("count", "Number of luminaires to place along the curve.")]
        [Input("target", "The target point (all created luminaires will be oriented towards this point).")]
        [InputFromProperty("type")]
        [Input("name", "The name to apply to the luminaires (each name will include this plus the number of luminaire in the sequence).")]
        [Output("luminaires", "A collection of Luminaires created along the input line.")]
        public static List<Luminaire> Luminaires(this ICurve crv, int count, Point target, LuminaireType type = null, string name = "")
        {
            List<Luminaire> luminaires = new List<Luminaire>();
            for (int i = 0; i < count; i++)
            {
                Point pt = (crv.IPointAtLength(i * (crv.Length() / (count - 1))));
                Vector dir = BH.Engine.Geometry.Create.Vector(pt, target);
                Luminaire lum = Create.Luminaire(pt, dir, type, name + "_" + i.ToString());
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
            Luminaire luminaire = new Luminaire
            {
                Position = pt,
                Direction = orientation,
                LuminaireType = type,
                Name = name
            };
            return luminaire;
        }

    }
}


