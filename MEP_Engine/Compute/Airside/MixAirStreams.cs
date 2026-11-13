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
        public static AirStream MixAirStreams(IEnumerable<AirStream> streams)
        {
            if (streams == null) throw new ArgumentNullException(nameof(streams));
            double mass = 0, mw = 0, mh = 0, p = double.NaN;
            foreach (AirStream stream in streams)
            {
                AirsideMath.Stream(stream, true);
                if (stream.DryAirMassFlowRate == 0) continue;
                if (double.IsNaN(p)) p = stream.State.Pressure;
                else AirsideMath.SamePressure(p, stream.State.Pressure);
                double m = stream.DryAirMassFlowRate;
                mass += m; mw += m * stream.State.HumidityRatio;
                mh += m * AirsideMath.H(stream.State.DryBulbTemperature, stream.State.HumidityRatio);
            }
            AirsideMath.Finite(mass, nameof(mass));
            if (mass <= 0) throw new ArgumentException("Mixing requires positive total flow.");
            double w = mw / mass, h = mh / mass;
            return new AirStream { State = AirsideMath.NewState(AirsideMath.TFromHW(h, w), w, p), DryAirMassFlowRate = mass };
        }
    }
}
