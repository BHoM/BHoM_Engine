using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.DataManipulation
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Dictionary<int, List<string>> FolderContent(string directory, string searchKey = "*", int depth = 1)
        {
            Dictionary<int, List<string>> tree = new Dictionary<int, List<string>>();

            List<string> toVisit = new List<string> { directory };
            for (int i = 0; i < depth; i++)
            {
                tree.Add(i, new List<string>());
                List<string> nextStop = new List<string>();
                foreach (string dir in toVisit)
                {
                    tree[i].AddRange(System.IO.Directory.GetFiles(dir, searchKey).ToList());
                    nextStop.AddRange(System.IO.Directory.GetDirectories(dir).ToList());
                }
                toVisit = nextStop;
            }
            return tree;
        }

        /***************************************************/
    }
}
