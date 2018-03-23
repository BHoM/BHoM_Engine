using BH.oM.Structural.Properties;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConcreteSection SetReinforcement(this ConcreteSection section, IEnumerable<Reinforcement> reinforcement)
        {
            ConcreteSection clone = section.GetShallowClone() as ConcreteSection;
            clone.Reinforcement = reinforcement.ToList();
            return clone;
        }

        /***************************************************/

        public static ConcreteSection AddReinforcement(this ConcreteSection section, IEnumerable<Reinforcement> reinforcement)
        {
            ConcreteSection clone = section.GetShallowClone() as ConcreteSection;

            if (clone.Reinforcement == null)
                clone.Reinforcement = new List<Reinforcement>();

            clone.Reinforcement.AddRange(reinforcement);

            return clone;
        }

        /***************************************************/
    }
}
