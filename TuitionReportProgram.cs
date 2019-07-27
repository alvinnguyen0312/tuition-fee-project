using System;
using System.Xml;
using System.Xml.XPath;

namespace Project2Tuition
{
    class TuitionReportProgram
    {
        private const string _XML_FILE = "tuition-fees.xml";
        static void Main(string[] args)
        {
            try
            {
                // Load XML file into the DOM
                XmlDocument doc = new XmlDocument();
                doc.Load(_XML_FILE);

                // Create an XPathNavigator object for performing XPath queries
                XPathNavigator nav = doc.CreateNavigator();

                //Display Header
                displayHeader();
                bool validCommand = true;
                bool validInput = true;
                do
                {
                    Console.Write("Enter 'R' to select a region, 'F' to select a field-of-study or 'T' to select a $ range: ");
                    char input = Console.ReadKey().KeyChar;

                    if (input == 'R' || input == 'r')
                    {
                        //Display all regions
                        displayRegions(nav);
                        do
                        {
                            validInput = true;
                            Console.Write("Enter a region #: ");
                            string regString = Console.ReadLine();
                            int regNum = 0;
                            if (!int.TryParse(regString, out int res))
                            {
                                Console.WriteLine("Input must be a number.");
                                validInput = false;
                            }
                            else
                            {
                                regNum = int.Parse(regString);
                                displayTuitionByStudyField(nav, regNum);
                            }                            
                        } while (!validInput);
                    }
                    else if (input == 'F' || input == 'f')
                    {
                        //Display all fields of study
                        displayStudyFields(nav);
                        Console.WriteLine();
                        do
                        {
                            validInput = true;
                            Console.Write("Enter a field #: ");
                            string fldString = Console.ReadLine();
                            int fldNum = 0;
                            if (!int.TryParse(fldString, out int res))
                            {
                                Console.WriteLine("Input must be a number.");
                                validInput = false;
                            }
                            else
                            {
                                fldNum = int.Parse(fldString);
                                displayTuitionByRegion(nav, fldNum);
                            }
                        } while (!validInput);
                    }
                    else if (input == 'T' || input == 't')
                    {
                        int minAmount = 0;
                        int maxAmount = 0;
                        string year = "";
                        do
                        {
                            validInput = true;
                            Console.Write("\nEnter the minimum tuition amount or press enter for no minimum: $");
                            string minString = Console.ReadLine();
                            if (minString != "" && !int.TryParse(minString, out int res1))
                            {
                                Console.WriteLine("Input must be a number.");
                                validInput = false;
                            }
                            else
                            {
                                minAmount = (minString == "") ? 0 : int.Parse(minString);
                                if (minAmount < 0)
                                {
                                    Console.WriteLine("Min limit must be non-negative.");
                                    validInput = false;
                                }
                            }
                        } while (!validInput);
                        do
                        {
                            validInput = true;
                            Console.Write("\nEnter the maximum tuition amount or press enter for no minimum: $");
                            string maxString = Console.ReadLine();
                            if (maxString != "" && !int.TryParse(maxString, out int res2))
                            {
                                Console.WriteLine("Input must be a number.");
                                validInput = false;
                            }
                            else
                            {
                                maxAmount = (maxString == "") ? 0 : int.Parse(maxString);
                                if (maxAmount < 0)
                                {
                                    Console.WriteLine("Max limit must be non-negative.");
                                    validInput = false;
                                }
                                if ((minAmount > maxAmount) && maxAmount != 0 )
                                {
                                    Console.WriteLine("The minimum tuition amount must not be greater that the maximum tuition amount.");
                                    validInput = false;
                                }
                            }

                        } while (!validInput);
                        do
                        {
                            validInput = true;
                            Console.Write("\nEnter a year (2016 or 2017): ");
                            year = Console.ReadLine();
                            if (year != "2016" && year != "2017")
                            {
                                Console.WriteLine("The year must be either 2016 or 2017.");
                                validInput = false;
                            }
                        } while (!validInput);

                        displayTuitionByRange(nav, minAmount, maxAmount, year);
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid command. Please try again!\n");
                        validCommand = false;
                    }
                } while (!validCommand);
            }
            catch (XmlException err)
            {
                Console.WriteLine("\nXML ERROR: " + err.Message);
            }
            catch (Exception err)
            {
                Console.WriteLine("\nERROR: " + err.Message);
            }
        }
        // display header function
        private static void displayHeader()
        {
            Console.WriteLine("Canadian Undergraduate Tuition Fees");
            Console.WriteLine("===================================\n");
        }

        private static void displayRegions(XPathNavigator nav)
        {
            Console.WriteLine();
            Console.WriteLine("Select a region by number as shown below...");
            XPathNodeIterator noteIt = nav.Select("//region/@description");
            int i = 0;
            while (noteIt.MoveNext())
            {                
                i++;
                if (i % 2 != 0)
                {
                    Console.Write(String.Format("{0,2}. {1,-50}", i, noteIt.Current.Value));
                }
                else
                {
                    Console.Write(String.Format("{0,2}. {1,-10}\n", i, noteIt.Current.Value));
                }
                
            }
        }
        private static void displayTuitionByStudyField(XPathNavigator nav, int Reg)
        {
            string querySelectedRegion = $"//region[{Reg}]/@description";
            string queryAllFieldCodes = $"//series[@region-code = {Reg}]/@field-code";
            XPathNodeIterator selectedIt = nav.Select(querySelectedRegion);
            XPathNodeIterator fieldCodeIt = nav.Select(queryAllFieldCodes);
            while (selectedIt.MoveNext())
            {
                string title = $"\nTuitions in {selectedIt.Current.Value} by Field-of-Study";
                Console.WriteLine(title);
                foreach (var i in title)
                {
                    Console.Write("-");
                }
                Console.Write("\n");
            }
            Console.WriteLine(String.Format("{0,70} {1,19} {2,19}", "Field-of-Study", "Tuition $, 2016", "Tuition $, 2017"));
            int count = 0;
            while (fieldCodeIt.MoveNext())
            {                               
                string queryAllFields = $"//field[{fieldCodeIt.Current.Value}]/@description";
                XPathNodeIterator fieldIt = nav.Select(queryAllFields);
                XPathNodeIterator tuitionIT2016 = nav.Select($"//series[@field-code={fieldCodeIt.Current.Value} and @region-code={Reg}]/tuition[@year = 2016]/text()");
                XPathNodeIterator tuitionIT2017 = nav.Select($"//series[@field-code={fieldCodeIt.Current.Value} and @region-code={Reg}]/tuition[@year = 2017]/text()");
                while (fieldIt.MoveNext())
                {
                    Console.Write(String.Format("{0,70}", $"{fieldIt.Current.Value}"));
                    count++;
                    while(tuitionIT2016.MoveNext() && tuitionIT2017.MoveNext())
                    {
                        Console.Write(String.Format("{0,20} {1,20}",
                                         int.TryParse(tuitionIT2016.Current.Value, out int res2016) ? res2016.ToString("#,##0") : "", 
                                     $"{(int.TryParse(tuitionIT2017.Current.Value, out int res2017) ? res2017.ToString("#,##0") : "")}\n"));
                    }
                }
            }
            Console.WriteLine($"{count} matches found.");
        }

        private static void displayStudyFields(XPathNavigator nav)
        {
            Console.WriteLine();
            Console.WriteLine("Select a field by number as shown below...");
            XPathNodeIterator noteIt = nav.Select("//field/@description");
            int i = 0;
            while (noteIt.MoveNext())
            {
                i++;
                if (i % 2 != 0)
                {
                    Console.Write(String.Format("{0,2}. {1,-60}", i, noteIt.Current.Value));
                }
                else
                {
                    Console.Write(String.Format("{0,2}. {1,-50}\n", i, noteIt.Current.Value));
                }

            }
        }

        private static void displayTuitionByRegion (XPathNavigator nav, int Fld)
        {
            string querySelectedField = $"//field[{Fld}]/@description";
            string queryAllRegionCodes = $"//series[@field-code = {Fld}]/@region-code";
            XPathNodeIterator selectedFieldIt = nav.Select(querySelectedField);
            XPathNodeIterator regionCodeIt = nav.Select(queryAllRegionCodes);
            while (selectedFieldIt.MoveNext())
            {
                string title = $"\nTuitions for {selectedFieldIt.Current.Value} by Region";
                Console.WriteLine(title);
                foreach (var i in title)
                {
                    Console.Write("-");
                }
                Console.Write("\n");
            }
            Console.WriteLine(String.Format("{0,60} {1,19} {2,19}", "Region", "Tuition $, 2016", "Tuition $, 2017"));
            int count = 0;
            while (regionCodeIt.MoveNext())
            {
                string queryAllRegions = $"//region[@code={regionCodeIt.Current.Value}]/@description"; // because the region code is not in order so I cant use region[{regionCodeIt.Current.Value}]
                XPathNodeIterator regionIt = nav.Select(queryAllRegions);
                XPathNodeIterator tuitionIT2016 = nav.Select($"//series[@region-code={regionCodeIt.Current.Value} and @field-code={Fld}]/tuition[@year = 2016]/text()");
                XPathNodeIterator tuitionIT2017 = nav.Select($"//series[@region-code={regionCodeIt.Current.Value} and @field-code={Fld}]/tuition[@year = 2017]/text()");
                while (regionIt.MoveNext())
                {
                    Console.Write(String.Format("{0,60}", $"{regionIt.Current.Value}"));
                    count++;
                    while (tuitionIT2016.MoveNext() && tuitionIT2017.MoveNext())
                    {
                        Console.Write(String.Format("{0,20} {1,20}", 
                                     int.TryParse(tuitionIT2016.Current.Value, out int res2016) ? res2016.ToString("#,##0") : "", 
                                     $"{(int.TryParse(tuitionIT2017.Current.Value, out int res2017) ? res2017.ToString("#,##0") : "")}\n"));
                    }
                }
            }
            Console.WriteLine($"{count} matches found.");
        }

        private static void displayTuitionByRange(XPathNavigator nav, int min, int max, string year)
        {
            string title = "";
            if (min == 0 && max == 0)
            {
                title = $"\nAll Tuitions for {year}";
            }
            else if (max == 0)
            {
                title = $"\nTuitions At or Above ${min.ToString("#,##0")} in {year}";
            }
            else if (min == 0)
            {                
                title = $"\nTuitions At or Below ${max.ToString("#,##0")} in {year}";
            }
            else
            {
                title = $"\nTuitions Between ${min.ToString("#,##0")} and ${max.ToString("#,##0")} in {year}";
            }
            Console.WriteLine(title);
            foreach (var i in title)
            {
                Console.Write("-");
            }
            Console.Write("\n");

            string queryRegionCodes = (max != 0) ? $"//series/tuition[(text() < {max} and text() > {min}) and @year={year}]/ancestor::series/@region-code" :
                                                   $"//series/tuition[ text() > {min} and @year={year}]/ancestor::series/@region-code";
            string queryFieldCodes =  (max != 0) ? $"//series/tuition[(text() < {max} and text() > {min}) and @year={year}]/ancestor::series/@field-code" :
                                                   $"//series/tuition[ text() > {min} and @year={year}]/ancestor::series/@field-code";
            string queryTuitionAmount = (max != 0) ?  $"//series/tuition[(text() < {max} and text() > {min}) and @year={year}]" :
                                                      $"//series/tuition[text() > {min} and @year={year}]";
            if(min == 0 & max == 0)
            {
                queryRegionCodes = $"//series/tuition[@year={year}]/ancestor::series/@region-code";

                queryFieldCodes = $"//series/tuition[@year={year}]/ancestor::series/@field-code";

                queryTuitionAmount = $"//series/tuition[@year={year}]";
            }
            XPathNodeIterator fieldCodeIt = nav.Select(queryFieldCodes);
            XPathNodeIterator regionCodeIt = nav.Select(queryRegionCodes);
            XPathNodeIterator tuitionCodeIt = nav.Select(queryTuitionAmount);
           
            Console.WriteLine(String.Format("{0,25} {1,45} {2,10}", "Region", "Field-of-Study", "Tuition $"));
            int count = 0;
            while (tuitionCodeIt.MoveNext() && regionCodeIt.MoveNext() && fieldCodeIt.MoveNext())
            {
                string queryRegionNameByCode = $"//region[@code={regionCodeIt.Current.Value}]/@description";
                string queryFieldNameByCode = $"//field[{fieldCodeIt.Current.Value}]/@description";
                XPathNodeIterator fieldNameByCodeIt = nav.Select(queryFieldNameByCode);
                XPathNodeIterator regionNameByCodeIt = nav.Select(queryRegionNameByCode);
                count++;
                while (fieldNameByCodeIt.MoveNext() && regionNameByCodeIt.MoveNext())
                {
                    Console.WriteLine(String.Format("{0,25} {1,45} {2,10}",regionNameByCodeIt.Current.Value.Substring(0, regionNameByCodeIt.Current.Value.Length >= 25 ? 25 : regionNameByCodeIt.Current.Value.Length), 
                                                                           fieldNameByCodeIt.Current.Value.Substring(0, fieldNameByCodeIt.Current.Value.Length >= 45 ? 45 : fieldNameByCodeIt.Current.Value.Length), 
                                                                           (int.TryParse(tuitionCodeIt.Current.Value, out int result)) ? result.ToString("#,##0") : ""));
                }
            }
            Console.WriteLine($"{count} matches found.");

        }
    }

    
}
