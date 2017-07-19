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
    public class GenericAdapter
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<string> ErrorLog { get; set; }

        public ILink Adapter { get; set; }


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GenericAdapter(ILink adapter)
        {
            Adapter = adapter;
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Push(IEnumerable<object> data, string tag = "", Dictionary<string, string> config = null)
        {
            foreach (KeyValuePair<Type, List<object>> kvp in data.GroupByType())
            {
                Type type = kvp.Key;
                List<object> group = kvp.Value;

                // First push all properties
                Dictionary<Type, List<object>> properties = group.GetPropertyObjects(type);
                Push(properties.Values, tag, config);

                // Then pull all relevant objects
                List<object> existing = Pull(new List<IQuery> { new BaseQuery(type, tag) }, config);

                // Delete the old objects that are not in the new set
                IEqualityComparer<object> comparer = EqualityComparer<object>.Default; //TODO: We need to have proper equality comparer for object (probably directly on BHoMObject and Geometry
                Adapter.Delete(existing.Except(data, comparer));

                // Update the objects that already exist
                Adapter.Update(existing.Union(data, comparer), data.Union(existing, comparer));

                //Create the objects that don't exist already
                Adapter.Create(data.Except(existing, comparer));
            }

            return true;
        }

        /***************************************************/

        public List<object> Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null)
        {
            // TODO: Two things are wrong with the code below:
            //   - The information necessary to add the properties back into the objects are not available
            //   - We can clearly see that a single tag would not be enough for objects that are shared properties between different tagged groups
            // SO:
            //   - The pull code needs to be into read of the adapter
            //   - For tag to work at all, we need a list of them, not just one value

            return Adapter.Read(query, config);


            /*// Only works with base query for now
            if (!(query.Count() == 1 && query.First() is BaseQuery))
                throw new NotImplementedException();

            BaseQuery q = query.First() as BaseQuery;
            Type type = q.Type;
            string tag = q.Tag;

            // Get all properties
            Dictionary<Type, List<object>> properties = new Dictionary<Type, List<object>>();
            foreach (Type propType in type.GetPropertyTypes())
                properties[propType] = Pull(new List<IQuery> { new BaseQuery(propType, tag) }, config);

            // Get the objects themselves
            List<object> objects = new List<object>();
            foreach(object obj in Adapter.Read(query, config))
            {
                // Add all properties back into the object
            }*/

        }

        /***************************************************/

        public bool Execute(string command, Dictionary<string, string> config = null)
        {
            return Adapter.Execute(command, config);
        }

        /***************************************************/

        public bool Delete(List<object> objects, Dictionary<string, string> config = null)
        {
            return Adapter.Delete(objects, config);
        }
  
    }
}
