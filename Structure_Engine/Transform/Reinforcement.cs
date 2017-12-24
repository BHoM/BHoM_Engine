using BH.oM.Structural.Properties;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Transform
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConcreteSection SetReinforcement(this ConcreteSection section, IEnumerable<Reinforcement> reinforcement)
        {
            section.Reinforcement = reinforcement.ToList();

            return section;
        }

        /***************************************************/

        public static ConcreteSection AddReinforcement(this ConcreteSection section, IEnumerable<Reinforcement> reinforcement)
        {

            if (section.Reinforcement == null)
                section.Reinforcement = new List<Reinforcement>();

            section.Reinforcement.AddRange(reinforcement);

            return section;
        }
    }
}
