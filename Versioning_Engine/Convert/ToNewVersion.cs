/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Versioning;
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
                Base.Compute.RecordError("The string provided is not a supported json format");
                return json;
            }
        }

        /***************************************************/

        public static BsonDocument ToNewVersion(this BsonDocument document, string version = "")
        {
            if (document == null)
                return null;

            if (document.Contains("_t") && document["_t"].ToString() == "DBNull")
                return null;

            if (version == "")
                version = document.Version();

            // Keep a description of the old document
            string oldDocument = Compute.VersioningKey(document);
            string noUpdateMessage = null;
            bool wasUpdated = false;

            // Get the list of upgraders to call
            List<string> versions = Query.UpgradersToCall(version);

            bool versionWithPipes = false;

            lock (m_versioningLock)
            {
                if (versionWithPipes)
                {
                    // Call all the upgraders in sequence
                    for (int i = 0; i < versions.Count; i++)
                    {
                        // Create a connection with the upgrader
                        NamedPipeServerStream pipe = GetPipe(versions[i]);
                        if (pipe == null)
                            return document;

                        // Send the document
                        SendDocument(document, pipe);

                        // Get the new version back
                        BsonDocument result = ReadDocument(pipe);
                        if (result != null)
                        {
                            if (result.Contains("_t") && result["_t"] == "NoUpdate")
                            {
                                if (result.Contains("Message"))
                                {
                                    noUpdateMessage = result["Message"].ToString();
                                    Engine.Base.Compute.RecordError(noUpdateMessage);
                                }
                            }
                            else if (document != result)
                            {
                                wasUpdated = true;
                                document = result;
                            }
                        }
                    }

                    // Record the fact that a document needed to be upgraded
                    if (wasUpdated || noUpdateMessage != null)
                    {
                        string newDocument = noUpdateMessage != null ? null : Compute.VersioningKey(document);
                        string newVersion = Engine.Base.Query.BHoMVersion();
                        string oldVersion = string.IsNullOrWhiteSpace(version) ? "?.?" : version;
                        string message = noUpdateMessage ?? $"{oldDocument} from version {oldVersion} has been upgraded to {newDocument} (version {newVersion})";

                        BH.Engine.Base.Compute.RecordEvent(new VersioningEvent
                        {
                            OldDocument = oldDocument,
                            NewDocument = newDocument,
                            OldVersion = oldVersion,
                            NewVersion = newVersion,
                            Message = message
                        });
                    }
                }
                else
                {
                    // Call all the upgraders in sequence
                    for (int i = 0; i < versions.Count; i++)
                    {
                        BsonDocument result = null;
                        try
                        {
                            //Get pre-compiled upgrader method for the version
                            Func<BsonDocument, BsonDocument> upgrader = GetUpgraderMethod(versions[i]);
                            result = upgrader(document);    //Upgrade
                        }
                        catch (Exception e)
                        {
                            Engine.Base.Compute.RecordError(e.Message);
                            if (e.GetType().Name == "NoUpdateException")    //No update -> no point in trying the rest
                                break;
                        }

                        if (result != null && document != result)
                        {
                            wasUpdated = true;
                            document = result;
                        }
                    }

                    if (wasUpdated)
                    {
                        string newDocument = noUpdateMessage != null ? null : Compute.VersioningKey(document);
                        string newVersion = Engine.Base.Query.BHoMVersion();
                        string oldVersion = string.IsNullOrWhiteSpace(version) ? "?.?" : version;
                        string message = noUpdateMessage ?? $"{oldDocument} from version {oldVersion} has been upgraded to {newDocument} (version {newVersion})";

                        BH.Engine.Base.Compute.RecordEvent(new VersioningEvent
                        {
                            OldDocument = oldDocument,
                            NewDocument = newDocument,
                            OldVersion = oldVersion,
                            NewVersion = newVersion,
                            Message = message
                        });
                    }
                }
            }

            return document;
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
                BH.Engine.Base.Compute.RecordWarning("Failed to create a connection with BHoM Upgrader version " + version + ". The object will not be upgraded");
                return null;
            }

            // Find the upgrader file
            string upgraderName = "BHoMUpgrader" + version.Replace(".", "");
            string processFile = upgraderName + "\\" + upgraderName + ".exe";
            if (!File.Exists(processFile))
            {
                processFile = Path.Combine(BH.Engine.Base.Query.BHoMFolderUpgrades(), processFile);

                if (!File.Exists(processFile))
                {
                    Base.Compute.RecordWarning(processFile.Split(new char[] { '\\' }).Last() + " is missing. The object will not be upgraded");
                    return null;
                }
            }

            // Create the process for the upgrader
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(processFile, pipeName);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
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

        private static Func<BsonDocument, BsonDocument> GetUpgraderMethod(string version)
        {
            // Return the pipe if already exists
            if (m_CompiledUpgraders.ContainsKey(version))
                return m_CompiledUpgraders[version];


            // Find the upgrader file
            string upgraderName = "BHoMUpgrader" + version.Replace(".", "");
            string processFile = upgraderName + "\\" + upgraderName + ".exe";
            if (!File.Exists(processFile))
            {
                processFile = Path.Combine(BH.Engine.Base.Query.BHoMFolderUpgrades(), processFile);

                if (!File.Exists(processFile))
                {
                    Base.Compute.RecordWarning(processFile.Split(new char[] { '\\' }).Last() + " is missing. The object will not be upgraded");
                    return null;
                }
            }

            Assembly converterAssembly = Assembly.LoadFrom(processFile);

            if (m_BaseConverter == null)
            {
                Assembly upgraderAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == "BHoMUpgrader");
                if (upgraderAssembly == null)
                {
                    string baseUpgraderName = "BHoMUpgrader" + version.Replace(".", "");
                    string baseProcessFile = upgraderName + "\\" + "BHoMUpgrader" + ".dll";
                    if (!File.Exists(baseProcessFile))
                    {
                        baseProcessFile = Path.Combine(BH.Engine.Base.Query.BHoMFolderUpgrades(), baseProcessFile);

                        if (!File.Exists(baseProcessFile))
                        {
                            Base.Compute.RecordWarning(baseProcessFile.Split(new char[] { '\\' }).Last() + " is missing. The object will not be upgraded");
                            return null;
                        }
                    }
                    upgraderAssembly = Assembly.LoadFrom(baseProcessFile);
                }
                Type upgraderType = upgraderAssembly.GetTypes().First(x => x.Name == "Upgrader");
                m_Upgrader = Activator.CreateInstance(upgraderType);
                m_UpgraderMethod = upgraderType.GetMethod("Upgrade", BindingFlags.NonPublic | BindingFlags.Instance);

                Type baseConverterType = upgraderAssembly.GetTypes().First(x => x.Name == "Converter");
                m_BaseConverter = Activator.CreateInstance(baseConverterType);
            }

            Type converterType = converterAssembly.GetTypes().First(x => m_BaseConverter.GetType().IsAssignableFrom(x));
            dynamic converter = Activator.CreateInstance(converterType);

            Func<BsonDocument, BsonDocument> func = CreateUpgradeFunction(converter as dynamic, m_BaseConverter as dynamic);
            m_CompiledUpgraders[version] = func;
            return func;
        }

        /***************************************************/

        private static Func<BsonDocument, BsonDocument> CreateUpgradeFunction<T, TBase>(T converter, TBase baseConverter) where T : class where TBase : class
        {
            Func<BsonDocument, TBase, BsonDocument> initFunc = (Func<BsonDocument, TBase, BsonDocument>)Delegate.CreateDelegate(typeof(Func<BsonDocument, TBase, BsonDocument>), m_Upgrader, m_UpgraderMethod);
            return x => initFunc(x, converter as TBase);
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, NamedPipeServerStream> m_Pipes = new Dictionary<string, NamedPipeServerStream>();

        private static object m_versioningLock = new object();

        /***************************************************/

        private static Dictionary<string, Func<BsonDocument, BsonDocument>> m_CompiledUpgraders = new Dictionary<string, Func<BsonDocument, BsonDocument>>();

        private static object m_Upgrader = null;

        private static dynamic m_BaseConverter = null;

        private static MethodInfo m_UpgraderMethod = null;

        /***************************************************/
    }
}



