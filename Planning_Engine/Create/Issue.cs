using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using BH.oM.Testing;
using BH.oM.Planning;

namespace BH.Engine.Planning
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Issue Issue(string repoName, int number, string title = "", List<string> labels = null)
        {
            return new Issue
            {
                RepoName = repoName,
                Number = number,
                Title = title,
                Labels = (labels == null) ? new List<string>() : labels,
                Url = @"https://github.com/BuroHappoldEngineering/" + repoName + "/issues/" + number
            };
        }

        /***************************************************/

        public static Issue Issue(string repoName, int number, string title = "", List<string> labels = null, string milestone = "", string creator = "", List<string> assignees = null)
        {
            return new Issue
            {
                RepoName = repoName,
                Number = number,
                Title = title,
                Labels = (labels == null) ? new List<string>() : labels,
                MilestoneName = milestone,
                Creator = creator,
                Assignees = (assignees == null) ? new List<string>() : assignees,
                Url = @"https://github.com/BuroHappoldEngineering/" + repoName + "/issues/" + number
            };
        }

        /***************************************************/
    }
}
