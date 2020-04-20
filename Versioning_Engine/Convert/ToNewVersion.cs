/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Versioning
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string ToNewVersion(string json)
        {
            BsonDocument document;
            if (BsonDocument.TryParse(json, out document))
            {
                BsonDocument newDocument = ToNewVersion(document);
                if (newDocument != null)
                    return newDocument.ToJson();
                else
                    return json;
            }
            else
            {
                Reflection.Compute.RecordError("The string provided is not a supported json format");
                return json;
            }
        }

        /***************************************************/

        public static BsonDocument ToNewVersion(BsonDocument document, string version = "")
        {
            if (document == null)
                return null;

            // Get the current version of the BHoM if not provided
            if (version.Length == 0)
                version = GetCurrentAssemblyVersion();

            // Create a connection with the upgrader
            NamedPipeServerStream pipe = GetPipe(version);
            if (pipe == null)
                return null;

            // Send the document
            SendDocument(document, pipe);

            // Get the new version back
            return ReadDocument(pipe);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static NamedPipeServerStream GetPipe(string version)
        {
            // Return the pipe if already exists
            if (m_Pipes.ContainsKey(version))
                return m_Pipes[version];

            // Get the pipe name
            int processId = Process.GetCurrentProcess().Id;
            string pipeName = processId.ToString() + "_" + version;

            // Create the pipe
            NamedPipeServerStream pipe;
            try
            {
                pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut);
            }
            catch
            {
                BH.Engine.Reflection.Compute.RecordWarning("Failed to create a connection with BHoM Upgrader version " + version + ". The object will not be upgraded");
                return null;
            }

            // Find the upgrader file
            string upgraderName = "BHoMUpgrader" + version.Replace(".", "");
            string processFile = "bin\\" + upgraderName + "\\" + upgraderName + ".exe";
            if (!File.Exists(processFile))
            {
                string roamingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                processFile = Path.Combine(roamingFolder, "BHoM\\Assemblies", processFile);

                if (!File.Exists(processFile))
                {
                    Reflection.Compute.RecordWarning(processFile.Split(new char[] { '\\' }).Last() + " is missing. The object will not be upgraded");
                    return null;
                }
            }

            // Create the process for the upgrader
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(processFile, pipeName);
            //process.StartInfo.UseShellExecute = false;
            //process.StartInfo.CreateNoWindow = true;
            bool ok = process.Start();                

            // Waiting for pipe to connect
            pipe.WaitForConnection();
            
            // Store the pipe 
            m_Pipes[version] = pipe;

            return pipe;
        }

        /***************************************************/

        private static void SendDocument(BsonDocument document, PipeStream pipe)
        {
            BinaryWriter writer = new BinaryWriter(pipe);
            if (document == null)
                writer.Write(0);

            MemoryStream memory = new MemoryStream();
            BsonBinaryWriter memoryWriter = new BsonBinaryWriter(memory);
            BsonSerializer.Serialize(memoryWriter, typeof(BsonDocument), document);

            byte[] content = memory.ToArray();
            writer.Write(content.Length);

            writer.Write(content);
            writer.Flush();
        }

        /***************************************************/

        private static BsonDocument ReadDocument(PipeStream pipe)
        {
            BinaryReader reader = new BinaryReader(pipe);
            int contentSize = reader.ReadInt32();
            if (contentSize == 0)
                return null;

            byte[] content = new byte[contentSize];
            reader.Read(content, 0, contentSize);

            return BsonSerializer.Deserialize(content, typeof(BsonDocument)) as BsonDocument;
        }

        /***************************************************/

        private static string GetCurrentAssemblyVersion()
        {
            string version = "";

            // First try to get the assembly file version
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
            if (attributes.Length > 0)
            {
                AssemblyFileVersionAttribute attribute = attributes.First() as AssemblyFileVersionAttribute;
                if (attribute != null && attribute.Version != null)
                {
                    string[] split = attribute.Version.Split('.');
                    if (split.Length >= 2)
                        version = split[0] + "." + split[1];
                }
            }

            // Get the assembly version as a fallback
            if (version.Length == 0)
            {
                Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                version = currentVersion.Major + "." + currentVersion.Minor;
            }

            return version;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, NamedPipeServerStream> m_Pipes = new Dictionary<string, NamedPipeServerStream>();

        /***************************************************/
    }
}
