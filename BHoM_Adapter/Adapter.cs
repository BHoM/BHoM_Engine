using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using BH.Engine.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Adapter.Queries;

namespace BH.Adapter
{
    public class Adapter
    {
        public Adapter(ILink link)
        {
            Link = link;
        }


        public bool Push(IEnumerable<object> data, string tag = "", Dictionary<string, string> config = null)
        {
            foreach (KeyValuePair<Type, List<object>> kvp in data.GroupByType())
            {
                Type type = kvp.Key;
                List<object> group = kvp.Value;

                // First push all properties
                Dictionary<Type, List<object>> properties = group.GetPropertyObjects(type);
                Push(properties.Values, tag, config);

                //Then pull all relevant objects
                List<object> existing = Pull(new List<IQuery> { new BaseQuery(type, tag) }, config);

                // Delete the old objects that are not in the new set
                IEqualityComparer<object> comparer = EqualityComparer<object>.Default; //TODO: We need to have proper equality comparer for object (probably directly on BHoMObject and Geometry
                Link.Delete(existing.Except(data, comparer));

                //Update the objects that already exist
                Link.Update(existing.Union(data, comparer), data.Union(existing, comparer));

                //Create the objects that don't exist already
                Link.Create(data.Except(existing, comparer));
            }

            return true;
        }

        public List<object> Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null)
        {
                return new List<object>();
        }

        public bool Execute(string command, Dictionary<string, string> config = null)
        {
            return false;
        }

        public bool Delete(string filter = "", Dictionary<string, string> config = null)
        {
            return false;
        }

        List<string> ErrorLog { get; set; }

        ILink Link { get; set; }
    }
}
