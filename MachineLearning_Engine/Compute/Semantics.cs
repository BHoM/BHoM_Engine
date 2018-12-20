/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System;
using System.IO;
using java.util;
using java.io;
using edu.stanford.nlp.pipeline;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.MachineLearning
{
    public static partial class Compute
    {
        /****************************************/
        /****  Public Methods                ****/
        /****************************************/

        /// <summary>
        /// Analyse a paragraph of text looking for semantic relationships between words and sentences.
        /// </summary>
        /// <param name="text">Paragraph to analyse as string</param>
        /// <param name="annotators">Annotators to extract from the analysis as list of strings. See <see cref="https://stanfordnlp.github.io/CoreNLP/annotators.html"/> for a complete list.</param>
        /// <returns>A json string containing annotated text</returns>
        public static string Semantics(string text, IEnumerable<string> annotators = null)
        {
            Properties props = new Properties();
            string finalAnnotators = "";
            if (annotators != null) { finalAnnotators = String.Join(", ", annotators.ToArray()); }
            finalAnnotators += "tokenize, ssplit, pos, lemma, ner, parse, dcoref, sentiment";
            props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, parse, dcoref, sentiment"); 
            props.setProperty("ner.useSUTime", "0");
            props.setProperty("thread", Environment.ProcessorCount.ToString());

            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            string jarRoot = "C:\\Users\\" + username + "\\AppData\\Roaming\\BHoM\\stanford-corenlp-3.8.0";
            if (!System.IO.Directory.Exists(jarRoot)) { throw new System.IO.FileNotFoundException("Please download stanford-corenlp-3.8.0 from https://stanfordnlp.github.io/CoreNLP/index.html#download and place it in" + jarRoot); }
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


        /****************************************/
    }
}
