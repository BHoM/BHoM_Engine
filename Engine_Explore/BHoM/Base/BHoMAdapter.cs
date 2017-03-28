using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Base
{
    public class BHoMAdapter : IAdapter
    {
        public delegate List<object> PullFunction(string query = "", string config = "");
        public delegate bool PushFunction(IEnumerable<object> data, bool overwrite = true, string config = "");
        public delegate bool ExecuteFunction(string command, string config = "");
        public delegate bool DeleteFunction(string command, string config = "");


        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Dictionary<string, PullFunction> PullFunctions { get; set; } = new Dictionary<string, PullFunction>();

        public Dictionary<string, PushFunction> PushFunctions { get; set; } = new Dictionary<string, PushFunction>();

        public Dictionary<string, ExecuteFunction> ExecuteFunctions { get; set; } = new Dictionary<string, ExecuteFunction>();

        public Dictionary<string, ExecuteFunction> DeleteFunctions { get; set; } = new Dictionary<string, ExecuteFunction>();

        public List<string> ErrorLog { get; set; } = new List<string>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public BHoMAdapter() {}


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public virtual List<object> Pull(string query, string config = "")
        {
            if (PullFunctions.ContainsKey(query))
                return PullFunctions[query](query, config);
            else
                return null;
        }

        /***************************************************/

        public virtual bool Push(IEnumerable<object> data, bool overwrite = true, string config = "")
        {
            bool ok = true;

            foreach (IGrouping<Type, object> group in data.GroupBy(x => x.GetType()))
            {
                string typeName = group.Key.Name;
                if (PushFunctions.ContainsKey(typeName))
                    ok &= PushFunctions[typeName](group, overwrite, config);
                else
                    ok = false;
            }

            return ok;
        }

        /***************************************************/

        public virtual bool Delete(string filter = "", string config = "")
        {
            if (DeleteFunctions.ContainsKey(filter))
                return DeleteFunctions[filter](filter, config);
            else
                return false;
        }

        /***************************************************/

        public virtual bool Execute(string command, string config = "")
        {
            if (ExecuteFunctions.ContainsKey(command))
                return ExecuteFunctions[command](command, config);
            else
                return false;
        }

    }
}
