/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;
using System.ComponentModel;
using BH.oM.Base;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using BH.Engine.Reflection;
using System.Reflection;
using System.Text.RegularExpressions;
using BH.oM.Reflection.Attributes;
namespace BH.Engine.Serialiser
{
    public static partial class Convert
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        [Description("Convert a list of BHoM methods to Markdown for documentation.")]
        public static string MethodsToMarkdown(this List<MethodBase> methods, string fileName = "MarkdownOutput", string filePath = null, bool toFile = false)
        {
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fullDoc = new StringWriter();

            foreach (var method in methods)
            {
                string methodDefinition = MethodToMarkdown(method);
                fullDoc.Write(methodDefinition);
                fullDoc.WriteLine();
            }
            string outputName = fileName + ".md";

            if (filePath == null)
            {
                filePath = defaultPath;
            }

            if (toFile == true)
            {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(filePath, outputName)))
                {
                    outputFile.Write(fullDoc.ToString());
                }
            }
            return fullDoc.ToString();
        }
        [Description("Convert a BHoM method to Markdown for documentation.")]
        public static string MethodToMarkdown(this MethodBase method)
        {
            var methodString = new StringWriter();

            string methodFullName = method.Path();

            string methodName = method.Name;
            string methodDescription = method.Description();

            methodString.WriteLine("### {0}", methodName);
            methodString.WriteLine("<details>");
            methodString.WriteLine("<summary>");
            methodString.WriteLine("<i>{0}</i>\n", methodFullName);
            methodString.WriteLine("{0}", methodDescription);
            methodString.WriteLine("</summary>");
            methodString.WriteLine();

            Dictionary<string, string> inputParameters = method.InputDescriptions();
            methodString.WriteLine("#### Inputs");
            methodString.WriteLine("____");
            foreach (var input in inputParameters)
            {
                string inputName = input.Key;
                string inputDescription = input.Value;
                
                
                methodString.WriteLine("- **{0}**", inputName);
                string parsedInputDescription = parseInputs(inputDescription);
                methodString.Write(parsedInputDescription);


            }

            methodString.WriteLine();
            methodString.WriteLine("#### Outputs");
            methodString.WriteLine("____");
            string outputName = method.OutputName();
            string outputDescription = method.OutputDescription();
            
            methodString.WriteLine("- **{0}**", outputName);
            string[] subOutputDescriptions = Regex.Split(outputDescription, @"(?<!:)\r\n");
            foreach (var subOutputDescription in subOutputDescriptions)
            {
                string parsedOutputDescription = parseInputs(subOutputDescription);
                methodString.Write(parsedOutputDescription);
            }
            methodString.WriteLine("</details>");
            return methodString.ToString();
        }

        private static string parseInputs (string inputToParse)
        {
            StringWriter sw = new StringWriter();
            string enumString = null;
            List<string> enumValues = new List<string>();

            // Find enums if they exist
            if (inputToParse.Contains("Enum values:") == true)
            {
                List<string> splitAtEnums = Regex.Split(inputToParse, "Enum values:").ToList();
                // some kind of check to make sure there are only 2 elements in the split list...
                if (splitAtEnums.Count != 2)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Method description contains multiple enums, which are not currently supported.");
                }
                else
                {
                    enumString = splitAtEnums.Last();
                    enumValues = Regex.Split(enumString, "\r\n-").Where(s => s != string.Empty).ToList();
                    inputToParse = splitAtEnums.First();
                }
            }
            // Find line breaks that are not due to term/definition separation
            string[] subDescriptions = Regex.Split(inputToParse, @"(?<!:)\r\n");
            int i = 1; // set initial indentation level for sublist
            string prependIndent = new string((char)32, 3 * i); // white space for bullet points
            foreach (var subDescription in subDescriptions)
            {
                // find term/definitions
                if (subDescription.Contains(":\r\n") == true)
                {
                    List<string> termDef = Regex.Split(subDescription, @":\r\n").ToList();
                    string term = termDef[0];
                    termDef.RemoveAt(0); // remove term from term/definition pair
                    if (term.StartsWith("This is a BH.") == true)
                    {
                        sw.WriteLine(prependIndent + "- _{0}_", term);
                    }
                    else
                    {
                        sw.WriteLine(prependIndent + "- {0}", term);
                    }
                    if (termDef.Count == 1) // there should only be one definition --- no nested defs supported
                    {
                        List<string> definitions = Regex.Split(termDef[0], "\n").Where(s => s != string.Empty).ToList();
                        int j = i + 1; // set indentation for subsublist
                        prependIndent = new string((char)32, 3 * j);
                        foreach (var def in definitions)
                        {
                            if (def.StartsWith("This is a BH.") == true)
                            {
                                sw.WriteLine(prependIndent + "- _{0}_", def);
                            }
                            else
                            {
                                sw.WriteLine(prependIndent + "- {0}", def);
                            }
                            // Check if enum description reached
                            if ((def.Contains("This enum") == true) && (enumString != null))
                            {
                                sw.WriteLine(prependIndent + "- Enum values:");
                                int k = j + 1; // set indentation for subsubsublist
                                prependIndent = new string((char)32, 3 * k);
                                foreach (var enumValue in enumValues)
                                {
                                    sw.WriteLine(prependIndent + "- {0}", enumValue);
                                }
                            }
                        }
                    }
                    else
                    {
                        BH.Engine.Reflection.Compute.RecordWarning("Method description contains nested definitions, which are currently unhandled.");
                    }
                }
                else
                {
                    // find newlines and split by that delimiter into bullet points
                    List<string> descriptionAttributes = Regex.Split(subDescription, "\n").Where(s => s != string.Empty).ToList();
                    foreach (var descriptionAttribute in descriptionAttributes)
                    {
                        if (descriptionAttribute.StartsWith("This is a BH.") == true)
                        {
                            sw.WriteLine(prependIndent + "- _{0}_", descriptionAttribute);
                        }
                        else
                        {
                            sw.WriteLine(prependIndent + "- {0}", descriptionAttribute);
                        }
                    }
                }
            }
            return sw.ToString();
        }
        /*******************************************/
    }
}

