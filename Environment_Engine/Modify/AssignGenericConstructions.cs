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
        [Description("Assign generic constructions to a collection of Environment Panels based on their panel type. This will load a dataset of Generic Constructions and attempt to assign each generic construction to the panel based on the panel type. For example, panel type wall will be assigned the 'generic_wall' construction.")]
        [Input("panels", "A collection of Environment Panels to assign constructions to")]
        [Output("panels", "A collection of Environment Panels with the generic constructions assigned")]
        public static List<Panel> AssignGenericConstructions(this List<Panel> panels)
        {
            List<Panel> cloned = new List<Panel>(panels.Select(x => x.DeepClone<Panel>()).ToList());

            List<Construction> constructions = BH.Engine.Library.Query.Library("GenericConstructions").Select(x => x as Construction).ToList();
            if (constructions.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("The dataset for generic Environment constructions did not exist within your datasets. Generic Constructions cannot be assigned.");
                return cloned;
            }

            foreach(Panel p in cloned)
            {
                switch(p.Type)
                {
                    case PanelType.Floor:
                    case PanelType.FloorExposed:
                    case PanelType.FloorInternal:
                    case PanelType.FloorRaised:
                    case PanelType.SlabOnGrade:
                    case PanelType.UndergroundSlab:
                        p.Construction = constructions.Where(x => x.Name == "generic_floor").FirstOrDefault();
                        break;
                    case PanelType.Ceiling:
                    case PanelType.UndergroundCeiling:
                    case PanelType.Roof:
                        p.Construction = constructions.Where(x => x.Name == "generic_ceiling").FirstOrDefault();
                        break;
                    case PanelType.WallInternal:
                        p.Construction = constructions.Where(x => x.Name == "generic_partition").FirstOrDefault();
                        break;
                    default:
                        p.Construction = constructions.Where(x => x.Name == "generic_wall").FirstOrDefault();
                        break;
                }
            }

            return cloned;
        }
    }
}
