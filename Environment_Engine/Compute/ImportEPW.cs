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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Collections;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Environment.Climate;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Imports an epw-file from a given file path")]
        [Input("filePath", "The file path of the epw-file")]
        [Output("epwFile", "The imported weather-file object")]
        public static WeatherFile ImportEPW(string filePath)
        {
            // Declare a weatherfile object to which attribtues can be passed
            WeatherFile epw = new WeatherFile();

            // Check that file passed exists, and exit if it doesn't while passing a record of your failure to the Error Log
            if (!File.Exists(filePath))
            {
                BH.Engine.Base.Compute.RecordError("File not found. Did you give the correct path?");
                return null;
            }

            // Read all lines of file passed as strings
            string[] lines = System.IO.File.ReadAllLines(filePath);

            // Check to see if the file passed contains the requisite number of lines to parse as an EPW (> 8760)
            if (lines.Count() < 8760)
            {
                BH.Engine.Base.Compute.RecordError("The file passed has less than 8760 lines. Are you sure it's an EPW?");
                return null;
            }

            // Get location data from the EPW file header and pass to EPW object
            List<String> header = lines[0].Split(',').ToList();

            // Check that file header is capable of being parsed as location data
            if (header.Count() < 10)
            {
                BH.Engine.Base.Compute.RecordError("The file passed has an odd header structure. Are you sure it's an EPW?");
                return null;
            }

            // Load location (and UTC offset) data
            epw.CityName = header[1];
            epw.StateName = header[2];
            epw.Country = header[3];
            epw.WeatherSource = header[4];
            epw.WMOID = header[5];
            epw.SpaceTime = new SpaceTime();
            epw.SpaceTime.Location.Latitude = System.Convert.ToDouble(header[6]);
            epw.SpaceTime.Location.Longitude = System.Convert.ToDouble(header[7]);
            epw.SpaceTime.Location.UtcOffset = System.Convert.ToDouble(header[8]);
            epw.SpaceTime.Location.Elevation = System.Convert.ToDouble(header[9]);

            // Create a list of lists from the last 8760 lines of the file, each sublist representing values for an hour of the year
            List<List<String>> rawData = new List<List<String>>();
            for (int i = lines.Count() - 8760; i < lines.Count(); ++i)
            {
                rawData.Add(lines[i].Split(',').ToList());
            }

            // Transpose the list of lists into a list of annual hourly variables
            List<List<String>> data = BH.Engine.Data.Modify.TransposeRectangularCollection(rawData);

            try
            {
                // Populate the EPW object with the hourly variables data
                epw.Year = data[0].ConvertAll(x => System.Convert.ToInt32(x)).ToList();
                epw.Month = data[1].ConvertAll(x => System.Convert.ToInt32(x)).ToList();
                epw.Day = data[2].ConvertAll(x => System.Convert.ToInt32(x)).ToList();
                epw.Hour = data[3].ConvertAll(x => System.Convert.ToInt32(x) - 1).ToList();
                epw.Minute = data[4].ConvertAll(x => System.Convert.ToInt32(x) - 60).ToList();

                epw.UncertaintyFlag = data[5]; //A list of string

                epw.DryBulbTemperature = ConvertData(data, 6, "Dry Bulb Temperature");
                epw.DewPointTemperature = ConvertData(data, 7, "Dew Point Temperature");
                epw.RelativeHumidity = ConvertData(data, 8, "Relative Humidity");
                epw.AtmosphericStationPressure = ConvertData(data, 9, "Atmospheric Station Pressure");
                epw.ExtraterrestrialHorizontalRadiation = ConvertData(data, 10, "Extraterrestrial Horizontal Radiation");
                epw.ExtraterrestrialDirectNormalRadiation = ConvertData(data, 11, "Extraterrestrial Direct Normal Radiation");
                epw.HorizontalInfraredRadiationIntensity = ConvertData(data, 12, "Horizontal Infrared Radiation Intensity");
                epw.GlobalHorizontalRadiation = ConvertData(data, 13, "Global Horizontal Radiation");
                epw.DirectNormalRadiation = ConvertData(data, 14, "Direct Normal Radiation");
                epw.DiffuseHorizontalRadiation = ConvertData(data, 15, "Diffuse Horizontal Radiation");
                epw.GlobalHorizontalIlluminance = ConvertData(data, 16, "Global Horizontal Illuminance");
                epw.DirectNormalIlluminance = ConvertData(data, 17, "Direct Normal Illuminance");
                epw.DiffuseHorizontalIlluminance = ConvertData(data, 18, "Diffuse Horizontal Illuminance");
                epw.ZenithLuminance = ConvertData(data, 19, "Zenith Luminance");
                epw.WindDirection = ConvertData(data, 20, "Wind Direction");
                epw.WindSpeed = ConvertData(data, 21, "Wind Speed");
                epw.TotalSkyCover = ConvertData(data, 22, "Total Sky Cover");
                epw.OpaqueSkyCover = ConvertData(data, 23, "Opaque Sky Cover");
                epw.Visibility = ConvertData(data, 24, "Visibility");
                epw.CeilingHeight = ConvertData(data, 25, "Ceiling Height");
                epw.PresentWeatherObservation = ConvertData(data, 26, "Present Weather Observation");
                epw.PresentWeatherCodes = ConvertData(data, 27, "Present Weather Codes");
                epw.PrecipitableWater = ConvertData(data, 28, "Precipitable Water");
                epw.AerosolOpticalDepth = ConvertData(data, 29, "Aerosol Optical Depth");
                epw.SnowDepth = ConvertData(data, 30, "Snow Depth");
                epw.DaysSinceLastSnowfall = ConvertData(data, 31, "Days Since Last Snowfall");
                epw.Albedo = ConvertData(data, 32, "Albedo");
                epw.LiquidPrecipitationDepth = ConvertData(data, 33, "Liquid Precipitation Depth");
                epw.LiquidPrecipitationQuantity = ConvertData(data, 34, "Liquid Precipitation Quantity");

            }
            catch (Exception e)
            {
                BH.Engine.Base.Compute.RecordError(e.ToString());
                return null;
            }

            return epw;
        }

        private static List<double> ConvertData(List<List<string>> data, int index, string dataName)
        {
            //Handle the conversion of the data and error handling all at once

            if ((data.Count - 1) < index)
            {
                BH.Engine.Base.Compute.RecordWarning("No data for " + dataName + " could be extracted from the EPW");
                return new List<double>();
            }

            try
            {
                return data[index].ConvertAll(x => System.Convert.ToDouble(x)).ToList();
            }
            catch (Exception e)
            {
                BH.Engine.Base.Compute.RecordError("An error occurred in converting the " + dataName + " data. The error was\n" + e.ToString());
                return new List<double>();
            }
        }
    }
}





