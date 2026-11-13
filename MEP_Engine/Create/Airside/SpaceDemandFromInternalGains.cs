/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 * The BHoM is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3.0 of the License, or
 * (at your option) any later version.
 * The BHoM is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 * You should have received a copy of the GNU Lesser General Public License
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.
 */
using System;
using System.ComponentModel;
using BH.oM.MEP.Airside;
using BH.oM.MEP.Results;
namespace BH.Engine.MEP
{
    public static partial class Create
    {
        [Description("Converts positive internal gains to negative cooling demand. Adds separately declared signed sensible demand. Not a winter heat-loss model.")]
        public static SpaceConditioningDemand SpaceDemandFromInternalGains(SpaceLoad load, Guid spaceId,
            Guid designCaseId, AirState roomState, double additionalSignedSensibleDemand = 0,
            double additionalSignedMoistureDemand = 0)
        {
            if(load==null)throw new ArgumentNullException(nameof(load));
            if(spaceId==Guid.Empty || designCaseId==Guid.Empty || !spaceId.Equals(load.ObjectId))
                throw new ArgumentException("Non-empty identities and matching source space are required.");
            AirsideMath.State(roomState); AirsideMath.Finite(load.SensibleLoad,"SensibleLoad");AirsideMath.Finite(load.LatentLoad,"LatentLoad");
            if(load.SensibleLoad<0 || load.LatentLoad<0)throw new ArgumentException("Internal gains must be nonnegative.");
            AirsideMath.Finite(additionalSignedSensibleDemand,nameof(additionalSignedSensibleDemand));
            AirsideMath.Finite(additionalSignedMoistureDemand,nameof(additionalSignedMoistureDemand));
            double q=-load.SensibleLoad+additionalSignedSensibleDemand;
            double water=-load.LatentLoad/(2501000+1860*roomState.DryBulbTemperature)+additionalSignedMoistureDemand;
            AirsideMath.Finite(q,"assembled sensible demand");AirsideMath.Finite(water,"assembled moisture demand");
            return new SpaceConditioningDemand { SpaceId=spaceId,DesignCaseId=designCaseId,DesignCondition=SpaceDesignCondition.Cooling,
                SensibleConditioningDemand=q,MoistureConditioningDemand=water,
                DemandBasis="Internal gains plus explicitly supplied signed additions. Moisture conversion uses 2501000+1860*T_room J/kg; source criteria label: "+load.ResultCase };
        }
    }
}
