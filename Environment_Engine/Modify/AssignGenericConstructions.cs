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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.Engine.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Constructions;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Assign generic constructions to a collection of Environment Panels based on their panel type. This will load a dataset of Generic Constructions and attempt to assign each generic construction to the panel based on the panel type. For example, panel type wall will be assigned the 'generic_wall' construction. WARNING: This will overwrite constructions hosted on objects passed.")]
        [Input("panel", "A collection of Environment Panels to assign constructions to")]
        [Input("assignOpenings", "Flag to determine whether to assign generic constructions to openings hosted by the panels at the same time. Default is false, meaning openings will not be updated. If set to true then openings will receive generic constructions as well")]
        [Output("panel", "A collection of Environment Panels with the generic constructions assigned")]
        public static Panel AssignGenericConstructions(this Panel panel, bool assignOpenings = false)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot assign the generic constructions to a null panel.");
                return null;
            }

            Panel cloned = panel.DeepClone<Panel>();

            List<Construction> constructions = BH.Engine.Library.Query.Library("GenericConstructions").Select(x => x as Construction).ToList();
            if (constructions.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError("The dataset for generic Environment constructions did not exist within your datasets. Generic Constructions cannot be assigned.");
                return cloned;
            }

            if (cloned.Construction != null)
                BH.Engine.Base.Compute.RecordWarning(string.Format("The construction for the panel with GUID {0} was not null and will be replaced with a generic construction", cloned.BHoM_Guid));
            else
                BH.Engine.Base.Compute.RecordNote(string.Format("The construction for the panel with GUID {0} was automatically assigned a generic construction", cloned.BHoM_Guid));

            switch (cloned.Type)
            {
                case PanelType.Floor:
                case PanelType.FloorExposed:
                case PanelType.FloorInternal:
                case PanelType.FloorRaised:
                case PanelType.SlabOnGrade:
                case PanelType.UndergroundSlab:
                    cloned.Construction = constructions.Where(x => x.Name == "generic_floor").FirstOrDefault();
                    break;
                case PanelType.Ceiling:
                case PanelType.UndergroundCeiling:
                case PanelType.Roof:
                    cloned.Construction = constructions.Where(x => x.Name == "generic_ceiling").FirstOrDefault();
                    break;
                case PanelType.WallInternal:
                    cloned.Construction = constructions.Where(x => x.Name == "generic_partition").FirstOrDefault();
                    break;
                default:
                    cloned.Construction = constructions.Where(x => x.Name == "generic_wall").FirstOrDefault();
                    break;
            }

            if(assignOpenings)
            {
                for(int x = 0; x < cloned.Openings.Count; x++)
                    cloned.Openings[x] = cloned.Openings[x].AssignGenericConstructions();
            }

            return cloned;
        }

        [Description("Assign generic constructions to a collection of Environment Openings based on their opening type. This will load a dataset of Generic Constructions and attempt to assign each generic construction to the opening based on the opening type. For example, opening type Window will be assigned the 'generic_window' construction. WARNING: This will overwrite constructions hosted on objects passed.")]
        [Input("opening", "A collection of Environment Openings to assign constructions to")]
        [Output("opening", "A collection of Environment Openings with the generic constructions assigned")]
        public static Opening AssignGenericConstructions(this Opening opening)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot assign the generic constructions to a null opening.");
                return null;
            }

            Opening cloned = opening.DeepClone<Opening>();

            List<Construction> constructions = BH.Engine.Library.Query.Library("GenericConstructions").Select(x => x as Construction).ToList();
            if (constructions.Count == 0)
            {
                BH.Engine.Base.Compute.RecordError("The dataset for generic Environment constructions did not exist within your datasets. Generic Constructions cannot be assigned.");
                return cloned;
            }

            if (cloned.OpeningConstruction != null)
                BH.Engine.Base.Compute.RecordWarning(string.Format("The construction for the opening with GUID {0} was not null and will be replaced with a generic construction", cloned.BHoM_Guid));
            else
                BH.Engine.Base.Compute.RecordNote(string.Format("The construction for the opening with GUID {0} was automatically assigned a generic window construction", cloned.BHoM_Guid));

            switch (cloned.Type)
            {
                case OpeningType.CurtainWall:
                case OpeningType.Door:
                case OpeningType.Frame:
                case OpeningType.Glazing:
                case OpeningType.Hole:
                case OpeningType.Rooflight:
                case OpeningType.RooflightWithFrame:
                case OpeningType.Undefined:
                case OpeningType.VehicleDoor:
                case OpeningType.Window:
                case OpeningType.WindowWithFrame:
                    cloned.OpeningConstruction = constructions.Where(x => x.Name == "generic_window").FirstOrDefault();
                    break;
                default:
                    cloned.OpeningConstruction = constructions.Where(x => x.Name == "generic_window").FirstOrDefault();
                    break;
            }

            return cloned;
        }
    }
}




