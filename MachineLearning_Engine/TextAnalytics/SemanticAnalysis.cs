using System;
using System.Collections.Generic;
using System.IO;
using java.util;
using java.io;
using edu.stanford.nlp.pipeline;
using Console = System.Console;
using Newtonsoft.Json;

namespace BH.Engine.MachineLearning
{
    public static partial class Analyse
    {
        public static string Semantics(string text)
        {
            //tokenize == separate
            //sssplit == split into difference sentences
            // pos == part of speech identification
            // lemma == part of lemma identification
            // ner == name entity recognition
            // parse == Consituency parsing
            Properties props = new Properties();
            props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, parse, dcoref, sentiment"); // Hard coded for now for lack of proper documentation to provide to the user, 
            props.setProperty("ner.useSUTime", "0");                                                        // But should be exposed as query
            props.setProperty("thread", Environment.ProcessorCount.ToString()); // TODO Not working in Gh

            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            string jarRoot = "C:\\Users\\" + username + "\\AppData\\Roaming\\BHoM\\stanford-corenlp-3.8.0";
            if (!System.IO.Directory.Exists(jarRoot)) { throw new System.IO.FileNotFoundException("Please download stanford-corenlp-3.8.0 from https://stanfordnlp.github.io/CoreNLP/index.html#download"); }
            string curDir = Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(jarRoot);
            StanfordCoreNLP pipeline = new StanfordCoreNLP(props);
            Directory.SetCurrentDirectory(curDir);

            Annotation annotation = new Annotation(text);
            pipeline.annotate(annotation);

            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            pipeline.jsonPrint(annotation, new PrintWriter(stream));

            return stream.ToString();
        }

        public static List<object> ParseAnnotation(string stream, string key)
        {
            dynamic deJson = JsonConvert.DeserializeObject(stream);
            List<object> value = new List<object>();
            for (int j = 0; j < deJson["sentences"].Count; j++)
            {
                value.Add(deJson["sentences"][j][key]);
            }
            return value;
        }

        public static List<string> getSentences(this string stream)
        {
            dynamic deJson = JsonConvert.DeserializeObject(stream);
            List<string> values = new List<string>();
            //string key = "sentenceText";
            for (int j = 0; j < deJson["sentences"].Count; j++)
            {
                string value = "";
                for (int k = 0; k < deJson["sentences"][j]["tokens"].Count; k++)
                {
                    value += deJson["sentences"][j]["tokens"][k]["originalText"];
                    value += ' ';
                }
                values.Add(value);
            }
            return values;
        }
    }
}
