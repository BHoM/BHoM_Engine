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
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Structure.Requests;
using BH.oM.Data.Requests;
using BH.oM.Structure.Results;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Generates a IResultRequest of the appropriate type based on provided type as well as optional ids, cases and divisions."+
            "\n As a user, please have a look at BarResultRequest, MeshResultRequest, NodeResultRequest and GlobalResultRequest instead for greater control.")]
        [Input("type", "Type of result.")]
        [Input("ids", "Object ids to extract results for.")]
        [Input("cases", "Cases to extract results for.")]
        [Input("divisions", "Only used for bar results. Sets how many points should be looked at for the result extraction.")]
        [Output("resultRequest", "A resultRequest matching the inputs provided.")]
        public static IResultRequest IResultRequest(Type type, IEnumerable<object> ids = null, IEnumerable<object> cases = null, int divisions = 5)
        {
            IResultRequest request = null;

            if (typeof(BarResult).IsAssignableFrom(type))
            {
                BarResultType resType = BarResultType.BarForce;

                if (type == typeof(BarForce))
                    resType = BarResultType.BarForce;
                else if (type == typeof(BarDeformation))
                    resType = BarResultType.BarDeformation;
                else if (type == typeof(BarStress))
                    resType = BarResultType.BarStress;
                else if (type == typeof(BarStrain))
                    resType = BarResultType.BarStrain;
                else if (type == typeof(BarDisplacement))
                    resType = BarResultType.BarDisplacement;
                else if (type == typeof(BarModeShape))
                    resType = BarResultType.BarModeShape;
                else
                    Base.Compute.RecordWarning("Did not find exact type. Assuming " + resType);

                request = new BarResultRequest { Divisions = divisions, DivisionType = DivisionType.EvenlyDistributed, ResultType = resType };
            }
            else if (typeof(MeshResult).IsAssignableFrom(type) || typeof(MeshElementResult).IsAssignableFrom(type))
            {
                MeshResultType resType = MeshResultType.Forces;

                if (type == typeof(MeshForce))
                    resType = MeshResultType.Forces;
                else if (type == typeof(MeshStress))
                    resType = MeshResultType.Stresses;
                else if (type == typeof(MeshVonMises))
                    resType = MeshResultType.VonMises;
                else if (type == typeof(MeshDisplacement))
                    resType = MeshResultType.Displacements;
                else if (type == typeof(MeshModeShape))
                    resType = MeshResultType.MeshModeShape;
                else
                    Base.Compute.RecordWarning("Did not find exact type. Assuming " + resType);

                request = new MeshResultRequest { ResultType = resType };

            }
            else if (typeof(StructuralGlobalResult).IsAssignableFrom(type))
            {
                GlobalResultType resType = GlobalResultType.Reactions;

                if (type == typeof(GlobalReactions))
                    resType = GlobalResultType.Reactions;
                else if (type == typeof(ModalDynamics))
                    resType = GlobalResultType.ModalDynamics;
                else
                    Base.Compute.RecordWarning("Did not find exact type. Assuming " + resType);

                request = new GlobalResultRequest { ResultType = resType };
            }
            else if (typeof(NodeResult).IsAssignableFrom(type))
            {
                NodeResultType resType = NodeResultType.NodeReaction;

                if (type == typeof(NodeReaction))
                    resType = NodeResultType.NodeReaction;
                else if (type == typeof(NodeDisplacement))
                    resType = NodeResultType.NodeDisplacement;
                else if (type == typeof(NodeAcceleration))
                    resType = NodeResultType.NodeAcceleration;
                else if (type == typeof(NodeVelocity))
                    resType = NodeResultType.NodeVelocity;
                else if (type == typeof(NodeModalMass))
                    resType = NodeResultType.NodeModalMass;
                else if (type == typeof(NodeModeShape))
                    resType = NodeResultType.NodeModeShape;
                else
                    Base.Compute.RecordWarning("Did not find exact type. Assuming " + resType);

                request = new NodeResultRequest { ResultType = resType };
            }
            else
            {
                return null;
            }


            if (ids != null)
                request.ObjectIds = ids.ToList();

            if (cases != null)
                request.Cases = cases.ToList();

            return request;

        }

        /***************************************************/
    }
}






