/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.MEP.Airside;
using BH.oM.MEP.Results;

namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        public static HeatRecoveryResult HeatRecovery(AirStream supplyInlet, AirStream extractInlet,
            HeatRecoveryDefinition definition, Guid designCaseId = default(Guid))
        {
            AirsideMath.Stream(supplyInlet); AirsideMath.Stream(extractInlet);
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            AirsideMath.Fraction(definition.SensibleEffectiveness, nameof(definition.SensibleEffectiveness));
            AirsideMath.Fraction(definition.LatentEffectiveness, nameof(definition.LatentEffectiveness));
            var s = supplyInlet.State; var e = extractInlet.State;
            double ms = supplyInlet.DryAirMassFlowRate, me = extractInlet.DryAirMassFlowRate;
            double cs = ms * AirsideMath.Cp(s.HumidityRatio), ce = me * AirsideMath.Cp(e.HumidityRatio);
            double sensible = definition.SensibleEffectiveness * Math.Min(cs, ce) * (e.DryBulbTemperature - s.DryBulbTemperature);
            double water = definition.LatentEffectiveness * Math.Min(ms, me) * (e.HumidityRatio - s.HumidityRatio);
            double ts = s.DryBulbTemperature + sensible / cs;
            double ws = s.HumidityRatio + water / ms, we = e.HumidityRatio - water / me;
            var so = AirsideMath.NewState(ts, ws, s.Pressure);
            double total = ms * (Query.Enthalpy(so) - Query.Enthalpy(s));
            double he = Query.Enthalpy(e) - total / me;
            var eo = AirsideMath.NewState(AirsideMath.TFromHW(he, we), we, e.Pressure);
            double minT = Math.Min(s.DryBulbTemperature, e.DryBulbTemperature) - 1e-6;
            double maxT = Math.Max(s.DryBulbTemperature, e.DryBulbTemperature) + 1e-6;
            if (eo.DryBulbTemperature < minT || eo.DryBulbTemperature > maxT)
                throw new NotSupportedException("Specified independent sensible/latent effectiveness gives an unsupported temperature crossover.");
            var r = new HeatRecoveryResult { HeatRecoveryDefinitionId = definition.BHoM_Guid, DesignCaseId = designCaseId,
                SupplyProcess = new AirProcessResult { DesignCaseId = designCaseId, Inlet = AirsideMath.Copy(supplyInlet),
                    Outlet = new AirStream { State = so, DryAirMassFlowRate = ms }, HeatTransferToAir = total, MoistureTransferToAir = water },
                ExtractProcess = new AirProcessResult { DesignCaseId = designCaseId, Inlet = AirsideMath.Copy(extractInlet),
                    Outlet = new AirStream { State = eo, DryAirMassFlowRate = me }, HeatTransferToAir = -total, MoistureTransferToAir = -water } };
            r.CalculationMessages.Add("Idealised balanced-energy two-stream model; minimum inlet capacity rate for sensible effectiveness and minimum dry-air flow for moisture effectiveness. No condensation, frost, leakage, carryover or bypass controls. SectionId is unresolved until an orchestrator maps the two recovery sections.");
            return r;
        }
    }
}
