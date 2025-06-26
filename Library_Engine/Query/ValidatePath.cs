/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Versioning;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Validates that the provided string is a valid full library path, and attempts to upgrade the path if it is not. Returns the input path if valid, or failing to upgrade. Returns the upgraded path if an upgrade is possible.")]
        [Input("fullLibraryName", "The full library path to the particular Library to validate. Only full paths supported, not super paths or partial paths to libraries.")]
        [Output("path", "Returns the input path if valid or failing to upgrade. Returns the upgraded path if able to upgrade.")]
        public static string ValidatePath(this string fullLibraryName, string versionFrom = "")
        {
            List<string> fullLibraryNames = LibraryNames();
            if (fullLibraryNames.Contains(fullLibraryName))
                return fullLibraryName;
            string newName = fullLibraryName;
            string upgradedName;
            Dictionary<string, string> datasetUpgrades = Engine.Versioning.Query.DatasetToNewPaths();
            //Loops through the dataset upgrades recursively, making sure chained upgrades are captured
            while (datasetUpgrades.TryGetValue(newName, out upgradedName))
            {
                newName = upgradedName;
            }

            if (string.IsNullOrWhiteSpace(newName) || !fullLibraryNames.Contains(newName)) //No upgrade found, or the upgrade found is not valid.
            {
                Dictionary<string, string> messageForDeleted = Engine.Versioning.Query.DatasetToMessageForDeleted();
                string message;
                if (messageForDeleted.TryGetValue(fullLibraryName, out message) || //Try find message for deleted for provided name
 (!string.IsNullOrWhiteSpace(newName) && messageForDeleted.TryGetValue(newName, out message))) //If cant be found, and new name is not null, try finding message for deleted from new name
                {
                    BH.Engine.Base.Compute.RecordEvent(new VersioningEvent{OldDocument = fullLibraryName, NewDocument = "Dataset has been removed with the following message: " + message, OldVersion = string.IsNullOrWhiteSpace(versionFrom) ? "?.?" : versionFrom, NewVersion = Engine.Base.Query.BHoMVersion(), Message = "Dataset has been removed with the following message: " + message});
                    Engine.Base.Compute.RecordError(message);
                }
                else
                {
                    Engine.Base.Compute.RecordError($"The dataset {fullLibraryName} is not a valid full path library and no valid upgrade could be found.");
                }

                return fullLibraryName;
            }
            else
            {
                string newVersion = Engine.Base.Query.BHoMVersion();
                string oldVersion = string.IsNullOrWhiteSpace(versionFrom) ? "?.?" : versionFrom;
                string message = $"{fullLibraryName} from version {oldVersion} has been upgraded to {newName} (version {newVersion})";
                BH.Engine.Base.Compute.RecordEvent(new VersioningEvent{OldDocument = fullLibraryName, NewDocument = newName, OldVersion = oldVersion, NewVersion = newVersion, Message = message});
            }

            return newName;
        }

        /***************************************************/
    }
}