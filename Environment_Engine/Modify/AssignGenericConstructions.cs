using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.Engine.Base;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Constructions;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Assign generic constructions to a collection of Environment Panels based on their panel type. This will load a dataset of Generic Constructions and attempt to assign each generic construction to the panel based on the panel type. For example, panel type wall will be assigned the 'generic_wall' construction. WARNING: This will overwrite constructions hosted on objects passed.")]
        [Input("panels", "A collection of Environment Panels to assign constructions to")]
        [Input("assignOpenings", "Flag to determine whether to assign generic constructions to openings hosted by the panels at the same time. Default is false, meaning openings will not be updated. If set to true then openings will receive generic constructions as well")]
        [Output("panels", "A collection of Environment Panels with the generic constructions assigned")]
        public static Panel AssignGenericConstructions(this Panel panel, bool assignOpenings = false)
        {
            Panel cloned = panel.DeepClone<Panel>();

            List<Construction> constructions = BH.Engine.Library.Query.Library("GenericConstructions").Select(x => x as Construction).ToList();
            if (constructions.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("The dataset for generic Environment constructions did not exist within your datasets. Generic Constructions cannot be assigned.");
                return cloned;
            }

            if (cloned.Construction != null)
                BH.Engine.Reflection.Compute.RecordWarning(string.Format("The construction for the panel with GUID {0} was not null and will be replaced with a generic construction", cloned.BHoM_Guid));
            else
                BH.Engine.Reflection.Compute.RecordNote(string.Format("The construction for the panel with GUID {0} was automatically assigned a generic construction", cloned.BHoM_Guid));


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
        [Input("openings", "A collection of Environment Openings to assign constructions to")]
        [Output("openings", "A collection of Environment Openings with the generic constructions assigned")]
        public static Opening AssignGenericConstructions(this Opening opening)
        {
            Opening cloned = opening.DeepClone<Opening>();

            List<Construction> constructions = BH.Engine.Library.Query.Library("GenericConstructions").Select(x => x as Construction).ToList();
            if (constructions.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("The dataset for generic Environment constructions did not exist within your datasets. Generic Constructions cannot be assigned.");
                return cloned;
            }

            if (cloned.OpeningConstruction != null)
                BH.Engine.Reflection.Compute.RecordWarning(string.Format("The construction for the opening with GUID {0} was not null and will be replaced with a generic construction", cloned.BHoM_Guid));
            else
                BH.Engine.Reflection.Compute.RecordNote(string.Format("The construction for the opening with GUID {0} was automatically assigned a generic construction", cloned.BHoM_Guid));

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
