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

        public static BsonDocument ToNewVersion(BsonDocument document)
        {
            if (document == null)
                return null;

            // Get the current version of the BHoM
            Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            // Create a connection with the upgrader
            NamedPipeServerStream pipe = GetPipe(currentVersion.Major + "." + currentVersion.Minor);
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

            // Create the pipe
            NamedPipeServerStream pipe;
            try
            {
                pipe = new NamedPipeServerStream(version, PipeDirection.InOut);
            }
            catch(Exception e)
            {
                BH.Engine.Reflection.Compute.RecordError("Failed to create a connection with BHoM Upgrader version " + version + " Error: " + e.Message);
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
                    Reflection.Compute.RecordError("Cannot start process " + processFile.Split(new char[] { '\\' }).Last());
                    return null;
                }
            }

            // Create the process for the upgrader
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(processFile, version);
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
            MemoryStream memory = new MemoryStream();
            BsonBinaryWriter memoryWriter = new BsonBinaryWriter(memory);
            BsonSerializer.Serialize(memoryWriter, typeof(BsonDocument), document);
            byte[] content = memory.ToArray();

            BinaryWriter writer = new BinaryWriter(pipe);
            writer.Write(content.Length);

            writer.Write(content);
            writer.Flush();
        }

        /***************************************************/

        private static BsonDocument ReadDocument(PipeStream pipe)
        {
            BinaryReader reader = new BinaryReader(pipe);
            int contentSize = reader.ReadInt32();

            byte[] content = new byte[contentSize];
            reader.Read(content, 0, contentSize);

            return BsonSerializer.Deserialize(content, typeof(BsonDocument)) as BsonDocument;
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, NamedPipeServerStream> m_Pipes = new Dictionary<string, NamedPipeServerStream>();

        /***************************************************/
    }
}
