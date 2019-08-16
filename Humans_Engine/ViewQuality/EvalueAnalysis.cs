/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Architecture.Theatron;
using BH.oM.Humans.ViewQuality;
using BH.Engine.Geometry;
using Accord.Collections;
using System;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Evaulate Evalues for a single Audience")]
        [Input("audience", "Audience to evalaute")]
        [Input("settings", "EvalueSettings to configure the evaluation")]
        [Input("activityArea", "ActivityArea to use in the evaluation")]
        public static List<Evalue> EvalueAnalysis(Audience audience, EvalueSettings settings, ActivityArea activityArea)
        {
            List<Evalue> results = EvaluateEvalue(audience, settings, activityArea);
            return results;
        }
        /***************************************************/
        [Description("Evaulate Evalues for a List of Audience")]
        [Input("audience", "Audience to evalaute")]
        [Input("settings", "EvalueSettings to configure the evaluation")]
        [Input("activityArea", "ActivityArea to use in the evaluation")]
        public static List<List<Evalue>> EvalueAnalysis(List<Audience> audience, EvalueSettings settings, ActivityArea activityArea)
        {
            List<List<Evalue>> results = new List<List<Evalue>>();
            foreach (Audience a in audience)
            {
                results.Add(EvaluateEvalue(a, settings, activityArea));
            }
            return results;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static List<Evalue> EvaluateEvalue(Audience audience, EvalueSettings settings, ActivityArea activityArea)
        {
            List<Evalue> results = new List<Evalue>();
            KDTree<Spectator> spectatorTree = SetKDTree(audience);
            foreach (Spectator s in audience.Spectators)
            {
                Vector rowVector = Geometry.Query.CrossProduct(Vector.ZAxis, s.Head.PairOfEyes.ViewDirection);
                Vector viewVector = s.Head.PairOfEyes.ViewDirection;
                if (settings.ViewType == EvalueViewEnum.ToPoint)
                {
                    viewVector = activityArea.ActivityFocalPoint - s.Head.PairOfEyes.ReferenceLocation;
                }
                results.Add(EvalueResult(s, rowVector, viewVector,activityArea.PlayingArea,settings));
            }
            return results;
        }
        private static Evalue EvalueResult(Spectator s, Vector rowVector, Vector viewVect, Polyline playingArea,EvalueSettings settings)
        {
            Evalue result = new Evalue();

            Vector viewY =Geometry.Query.CrossProduct(viewVect, rowVector);
            viewY = viewY.Normalise();
            
            Plane vertPln = Geometry.Create.Plane(s.Head.PairOfEyes.ReferenceLocation, rowVector);
            Plane horizPln = Geometry.Create.Plane(s.Head.PairOfEyes.ReferenceLocation, viewY);

            double htestAng = 0;
            double vtestAng = 0;
            Vector iComp = new Vector();
            Vector jComp = new Vector();
            Point Hp0 = new Point();
            Point Hp1 = new Point();
            Point Vp0 = new Point();
            Point Vp1 = new Point();
            //loop to get widest horizontal and vertical angles and vectors
            for (int i = 0; i < playingArea.ControlPoints.Count; i++)
            {
                Point controlPi = playingArea.ControlPoints[i];
                //need to test if points are the same
                iComp = controlPi - s.Head.PairOfEyes.ReferenceLocation;
                for (int j = 0; j < playingArea.ControlPoints.Count; j++)
                {
                    if (i != j)
                    {
                        Point controlPj = playingArea.ControlPoints[j];
                        jComp = controlPj - s.Head.PairOfEyes.ReferenceLocation;
                        htestAng = Geometry.Query.Angle(iComp, jComp, horizPln);
                        
                        vtestAng = Geometry.Query.Angle(iComp, jComp, vertPln);
                        if (htestAng > Math.PI) htestAng = Math.PI * 2 - htestAng;
                        if (vtestAng > Math.PI) vtestAng = Math.PI * 2 - vtestAng;
                        if (result.HorizViewAng < htestAng)
                        {
                            result.HorizViewAng = htestAng;
                            Hp0 = playingArea.ControlPoints[i];
                            Hp1 = playingArea.ControlPoints[j];
                        }
                        if (result.VertViewAng < vtestAng)
                        {
                            result.VertViewAng = vtestAng;
                            Vp0 = playingArea.ControlPoints[i];
                            Vp1 = playingArea.ControlPoints[j];
                        }
                    }
                }
            }
            result.HorizViewAng = result.HorizViewAng * 57.2958;
            result.VertViewAng = result.VertViewAng * 57.2958;
            result.VertViewVectors[0] = (Vp0 - s.Head.PairOfEyes.ReferenceLocation).Normalise();
            result.VertViewVectors[1] = (Vp1 - s.Head.PairOfEyes.ReferenceLocation).Normalise();
            result.HorizViewVectors[0] = (Hp0 - s.Head.PairOfEyes.ReferenceLocation).Normalise();
            result.HorizViewVectors[1] = (Hp1 - s.Head.PairOfEyes.ReferenceLocation).Normalise();

            if (settings.ViewType == EvalueViewEnum.ToPoint)
            {
                result.Torsion = Geometry.Query.Angle(s.Head.PairOfEyes.ViewDirection, viewVect) * 57.2958;
            }
            
            return result;
        }
        /***************************************************/
        
    }
}
