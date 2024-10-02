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
using BH.oM.Structure.Elements;
using NUnit.Framework;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Data;
using System.Linq;

namespace BH.Tests.Engine.Serialiser
{

    public class SystemTypesToFromJson
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/


        [Test]
        [Description("Tests serialisation and deserialisation of a list of Enums and a single Enum.")]
        public void SystemTypesToFromJsonListEnums()
        {
            CustomObject custom = new CustomObject();
            List<object> enums = new List<object> { BarFEAType.Axial, BH.oM.Structure.Loads.CaseType.Envelope };

            custom.CustomData["list"] = enums;
            custom.CustomData["single"] = BarFEAType.Axial;

            string json = BH.Engine.Serialiser.Convert.ToJson(custom);

            CustomObject retObj = BH.Engine.Serialiser.Convert.FromJson(json) as CustomObject;

            retObj.Should().NotBeNull();

            retObj.CustomData.Should().NotBeNull();
            retObj.CustomData.Should().ContainKey("single");
            retObj.CustomData["single"].Should().Be(BarFEAType.Axial);
            retObj.CustomData.Should().ContainKey("list");
            List<object> retList = retObj.CustomData["list"] as List<object>;
            retList.Should().NotBeNull();
            retList.Should().Contain(BarFEAType.Axial);
            retList.Should().Contain(BH.oM.Structure.Loads.CaseType.Envelope);
        }

        [Test]
        [Description("Tests serialisation and deserialisation of a Color object.")]
        public void SystemTypesToFromJsonColor()
        {
            System.Drawing.Color color = System.Drawing.Color.Turquoise;

            string json = BH.Engine.Serialiser.Convert.ToJson(color);
            object retObj = BH.Engine.Serialiser.Convert.FromJson(json);

            retObj.Should().BeOfType(typeof(System.Drawing.Color));

            System.Drawing.Color retColor = (System.Drawing.Color)retObj;
            retColor.A.Should().Be(color.A);
            retColor.R.Should().Be(color.R);
            retColor.G.Should().Be(color.G);
            retColor.B.Should().Be(color.B);
        }

        [Test]
        [Description("Tests serialisation and deserialisation of a Color object as a property of CustomData.")]
        public void SystemTypesToFromJsonColorObjectProperty()
        {
            CustomObject custom = new CustomObject();
            System.Drawing.Color color = System.Drawing.Color.Turquoise;
            custom.CustomData["Colour"] = color;

            string json = BH.Engine.Serialiser.Convert.ToJson(custom);
            CustomObject retObj = BH.Engine.Serialiser.Convert.FromJson(json) as CustomObject;

            retObj.Should().NotBeNull();
            retObj.CustomData.Should().NotBeNull();
            retObj.CustomData.Should().ContainKey("Colour");
            object retColObj = retObj.CustomData["Colour"];
            retColObj.Should().NotBeNull();

            retColObj.Should().BeOfType(typeof(System.Drawing.Color));

            System.Drawing.Color retColor = (System.Drawing.Color)retColObj;
            retColor.A.Should().Be(color.A);
            retColor.R.Should().Be(color.R);
            retColor.G.Should().Be(color.G);
            retColor.B.Should().Be(color.B);
        }

        [Test]
        [Description("Tests serialisation and deserialisation of a DateTimeOffset object as a property of CustomData.")]
        public void SystemTypesToFromJsonDateTimeOffsetObjectProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(DateTimeOffset.UtcNow);
        }


        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a TimeSpan object as a property of CustomData.")]
        public void SystemTypesToFromJsonTimeSpanObjectProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(TimeSpan.FromSeconds(43543));
        }


        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a Regex object as a property of CustomData.")]
        public void SystemTypesToFromJsonRegexObjectProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(new Regex("/d*"));
        }
        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of an IntPtr object as a property of CustomData.")]
        public void SystemTypesToFromJsonIntPtrObjectProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(new IntPtr(34234));
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a Guid object as a property of CustomData.")]
        public void SystemTypesToFromJsonGuidProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(new Guid("E5DD4655-3A14-47E2-A19C-A042EE62EF42"));
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a Dictionary object.")]
        public void SystemTypesToFromJsonDictionary()
        {
            Dictionary<string, double> dict = new Dictionary<string, double>()
            {
                ["test1"] = 4,
                ["test2"] = 5
            };

            string json = BH.Engine.Serialiser.Convert.ToJson(dict);
            object ret = BH.Engine.Serialiser.Convert.FromJson(json);
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a Dictionary object as a property of CustomData.")]
        public void SystemTypesToFromJsonDictionaryAsProperty()
        {
            Dictionary<string, double> dict = new Dictionary<string, double>()
            {
                ["test1"] = 4,
                ["test2"] = 5
            };

            SystemTypesToFromJsonCustomDataProperty(dict);
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a ReadOnlyCollection object as a property of CustomData.")]
        public void SystemTypesToFromJsonReadOnlyCollectionAsProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(new ReadOnlyCollection<int>(new List<int> { 5, 6, 7 }));
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a SortedDictionary object.")]
        public void SystemTypesToFromJsonSortedDictionary()
        {
            SortedDictionary<double, double> dict = new SortedDictionary<double, double>();

            dict.Add(432.3, 21312);
            dict.Add(342.2, 12312);

            string json = BH.Engine.Serialiser.Convert.ToJson(dict);
            object ret = BH.Engine.Serialiser.Convert.FromJson(json);

            CheckDictionary(ret, dict);
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a SortedDictionary object as a property of CustomData.")]
        public void SystemTypesToFromJsonSortedDictionaryAsProperty()
        {
            SortedDictionary<double, double> dict = new SortedDictionary<double, double>();

            dict.Add(432.3, 21312);
            dict.Add(342.2, 12312);

            SystemTypesToFromJsonCustomDataProperty(dict);
        }

        [Test]
        [Description("Tests serialisation and deserialisation of an array as a property of CustomData.")]
        public void SystemTypesToFromJsonArrayAsProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(new int[] { 6, 7, 8 });
        }

        [Test]
        [Description("Tests serialisation and deserialisation of a Tuple with two objects as a property of CustomData.")]
        public void SystemTypesToFromJsonTuple2Property()
        {
            SystemTypesToFromJsonCustomDataProperty(new Tuple<int, string>(5, "test"));
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a Tuple with three objects as a property of CustomData.")]
        public void SystemTypesToFromJsonTuple3Property()
        {
            SystemTypesToFromJsonCustomDataProperty(new Tuple<int, string, double>(5, "test", 234.2));
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a Tuple with four objects as a property of CustomData.")]
        public void SystemTypesToFromJsonTuple4Property()
        {
            SystemTypesToFromJsonCustomDataProperty(new Tuple<int, string, double, string>(5, "test", 423.3, "asd"));
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a Tuple with five objects as a property of CustomData.")]
        public void SystemTypesToFromJsonTuple5Property()
        {
            SystemTypesToFromJsonCustomDataProperty(new Tuple<int, string, double, double, double>(5, "test", 34534.3, 0.423, 0.2432));
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a Tuple with five different system types as a property of CustomData.")]
        public void SystemTypesToFromJsonTuple5OtherSystemTypesProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(new Tuple<DateTimeOffset, TimeSpan, Regex, IntPtr, System.Drawing.Color>(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(43543), new Regex("/d*"), new IntPtr(982374), System.Drawing.Color.FromArgb(3)));
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a HashSet object as a property of CustomData.")]
        public void SystemTypesToFromJsonHashSetObjectProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(new HashSet<int>(new List<int> { 5, 6, 7 }));
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a 2D array as a property .")]
        public void SystemTypesToFromJson2dArrayProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(new double[,] { { 4.3, 4.4 }, { 7.6, 2.2 } });
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a list of Node objects as a property of CustomData.")]
        public void SystemTypesToFromJsonListObjectProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(new List<Node> { new Node(), new Node() });
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a ReadOnlyCollection as a property of CustomData.")]
        public void SystemTypesToFromJsonReadOnlyCollectionObjectProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(new ReadOnlyCollection<Node>(new List<Node> { new Node(), new Node() }));
        }

        /***************************************************/

        // Commented out because it is not functioning as intended for CI 
        //[Test]
        //public void SystemTypesToFromJsonBitMapObjectProperty()
        //{
        //    SystemTypesToFromJsonCustomDataProperty(RandomBitmap(1));       
        //}

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a DataTable object as a property of CustomData.")]
        public void SystemTypesToFromJsonDataTableProperty()
        {
            DataTable table = new DataTable();
            table.Columns.Add("col1", typeof(double));
            table.Columns.Add("col2", typeof(string));

            table.Rows.Add(4.2, "test");
            table.Rows.Add(1.2, "sdgfljksdfh");
            table.Rows.Add(2.00, "i");
            SystemTypesToFromJsonCustomDataProperty(table);
        }

        /***************************************************/

        [Test]
        [Description("Tests serialisation and deserialisation of a DateTime object as a property of CustomData.")]
        public void SystemTypesToFromJsonDateTimeProperty()
        {
            SystemTypesToFromJsonCustomDataProperty(System.DateTime.Parse("2023-05-10 14:40:52.531").ToUniversalTime());
        }

        /***************************************************/

        [Description("Helper method to test serialisation and deserialisation of various types as properties of CustomData.")]
        private void SystemTypesToFromJsonCustomDataProperty<T>(T value)
        {
            string keyItem = "item";
            string keyList = "list";
            string keyGenList = "genList";

            string keyDict1 = "dict1";
            string keyDict2 = "dict2";
            string keyDict3 = "dict3";
            string keyDict4 = "dict4";
            string keyDict5 = "dict5";

            CustomObject custom = new CustomObject();
            custom.CustomData[keyItem] = value;
            custom.CustomData[keyList] = new List<object> { value };
            custom.CustomData[keyGenList] = new List<T> { value };


            var dict1 = new Dictionary<string, object> { { "key", value } };
            var dict2 = new Dictionary<decimal, object> { { 4.4m, value } };
            var dict3 = new Dictionary<string, T> { { "key", value } };
            var dict4 = new Dictionary<T, string> { { value, "key" } };
            var dict5 = new Dictionary<object, object> { { value, value } };

            custom.CustomData[keyDict1] = dict1;
            custom.CustomData[keyDict2] = dict2;
            custom.CustomData[keyDict3] = dict3;
            custom.CustomData[keyDict4] = dict4;
            custom.CustomData[keyDict5] = dict5;

            string json = BH.Engine.Serialiser.Convert.ToJson(custom);

            CustomObject retCustom = BH.Engine.Serialiser.Convert.FromJson(json) as CustomObject;

            retCustom.Should().NotBeNull();
            retCustom.CustomData.Should().NotBeNull();
            retCustom.CustomData.Should().ContainKey(keyItem);
            object retValue = retCustom.CustomData[keyItem];
            retValue.Should().NotBeNull();

            retValue.Should().BeOfType(typeof(T));

            T retOffset = (T)retValue;

            EquivalentCheckList(retOffset, value);

            retCustom.CustomData.Should().ContainKey(keyList);
            object listVal = retCustom.CustomData[keyList];
            listVal.Should().NotBeNull();

            listVal.Should().BeOfType(typeof(List<object>));

            List<object> retList = (List<object>)listVal;
            retList.Count.Should().Be(1);
            EquivalentCheckList(retList[0], value);


            retCustom.CustomData.Should().ContainKey(keyGenList);
            object genListVal = retCustom.CustomData[keyGenList];
            genListVal.Should().NotBeNull();

            genListVal.Should().BeOfType(typeof(List<T>));

            List<T> retGenList = (List<T>)genListVal;
            retGenList.Count.Should().Be(1);
            EquivalentCheckList(retGenList[0], value);

            CheckDictionary(retCustom, keyDict1, dict1);
            CheckDictionary(retCustom, keyDict2, dict2);
            CheckDictionary(retCustom, keyDict3, dict3);
            CheckDictionary(retCustom, keyDict4, dict4);
            CheckDictionary(retCustom, keyDict5, dict5);
        }

        /***************************************************/

        [Description("Helper method to check the equality of dictionaries after deserialisation.")]
        private void CheckDictionary<T1, T2>(CustomObject retCustom, string key, IDictionary<T1, T2> val)
        {
            retCustom.CustomData.Should().ContainKey(key);
            object dictVal = retCustom.CustomData[key];
            CheckDictionary(dictVal, val);
        }

        /***************************************************/

        [Description("Helper method to check the equality of dictionaries after deserialisation.")]
        private void CheckDictionary<T1, T2>(object dictVal, IDictionary<T1, T2> val)
        {
            dictVal.Should().NotBeNull();

            dictVal.Should().BeOfType(val.GetType());

            IDictionary<T1, T2> retDict = (IDictionary<T1, T2>)dictVal;
            retDict.Count.Should().Be(val.Count);
            EquivalentCheckList(retDict.Keys.ToArray()[0], val.Keys.ToArray()[0]);
            EquivalentCheckList(retDict.Values.ToArray()[0], val.Values.ToArray()[0]);
        }

        /***************************************************/

        [Description("Helper method to check the equality of lists after deserialisation.")]
        private static void EquivalentCheckList(object val, object refVal)
        {
            if (refVal == null)
                return;

            val.Should().BeOfType(refVal.GetType());

            if ((refVal is IEnumerable) && !(refVal is string))
            {
                List<object> valEnum = ((IEnumerable)val).Cast<object>().ToList();
                List<object> refEnumerable = ((IEnumerable)refVal).Cast<object>().ToList();

                valEnum.Count.Should().Be(refEnumerable.Count);
                for (int i = 0; i < valEnum.Count; i++)
                {
                    EquivalentCheckList(valEnum[i], refEnumerable[i]);
                }
            }
            else if (val is Bitmap)
            {
                Bitmap bitMap1 = (Bitmap)val;
                Bitmap bitMap2 = (Bitmap)refVal;
                bitMap1.Should().BeEquivalentTo(bitMap2, options => options.Excluding(o => o.Palette.Flags));   //Flags are randomly populated, hence need to exclude from comparison
            }
            else
            {
                val.Should().BeEquivalentTo(refVal);
            }
        }

        /***************************************************/

        [Description("Generates a random Bitmap for testing purposes.")]
        private static Bitmap RandomBitmap(int index)
        {
            Bitmap bitmap = new Bitmap(80, 20, PixelFormat.Format24bppRgb);

            // 2. Get access to the raw bitmap data
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

            // 3. Generate RGB noise and write it to the bitmap's buffer.
            // Note that we are assuming that data.Stride == 3 * data.Width for simplicity/brevity here.
            byte[] noise = new byte[data.Width * data.Height * 3];
            new Random(index).NextBytes(noise);
            Marshal.Copy(noise, 0, data.Scan0, noise.Length);

            bitmap.UnlockBits(data);

            // 4. Save as JPEG and convert to Base64
            MemoryStream jpegStream = new MemoryStream();

            bitmap.Save(jpegStream, ImageFormat.Bmp);

            return new Bitmap(jpegStream);
        }

        /***************************************************/
    }
}



