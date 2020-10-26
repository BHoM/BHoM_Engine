using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Diffing;
using BH.Engine.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        public static Dictionary<Guid, IBHoMObject> DiffEntities(List<IBHoMObject> entities, DiffConfig diffConfig)
        {
            
            Dictionary<Guid, IBHoMObject> replaceMap = new Dictionary<Guid, IBHoMObject>();
            
            foreach (IBHoMObject entityA in entities)
            {
                foreach (IBHoMObject entityB in entities)
                {
                    if(entityA.GetType() == entityB.GetType())
                    {
                        Dictionary<string, Tuple<object, object>> modifiedProps = Diffing.Query.DifferentProperties(entityA, entityB, diffConfig);
                        if (modifiedProps == null)
                        {
                            //matched entities
                            replaceMap[entityA.BHoM_Guid] = entityB;
                            break;
                        }
                    }
                }
                
            }
            return replaceMap;
        }
    }
}
