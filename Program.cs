using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Stopwatch st = new Stopwatch();

            List<string> listRU = new List<string>();
            List<string> listEN = new List<string>();

            List<string> NewlistRU = new List<string>();
            List<string> NewlistEN = new List<string>();

            List<double> RuIndex = new List<double>();
            List<double> EnIndex = new List<double>();


            string pathRU, pathEN;

            bool bull = false;

            do
            {
                Console.Write("Welcome to Petrenko-Golzman String program v 1.0\n\n\nAre you going to analyze your txt files (Y / N) ?\n");
                Console.Write("Your answer: ");
                string answer = Console.ReadLine();

                if (answer.ToUpper() == "Y")
                {
                    Console.Write("\nEnter path to your RU file (in format {diskname}:\\...\\{filename}.txt)\nAs an example - C:\\Users\\{username}\\Desktop\\{filename}.txt: ");
                    pathRU = Console.ReadLine();
                    //pathRU = @"C:\Users\SS\Desktop\textRU.txt";
                    Console.Write("\n\nEnter path to your EN file (in format {diskname}:\\...\\{filename}.txt)\nAs an example - C:\\Users\\{username}\\Desktop\\{filename}.txt: ");
                    pathEN = Console.ReadLine();
                    //pathEN = @"C:\Users\SS\Desktop\textEN.txt";

                    st.Start();

                    //call methods to write lines from files
                    listRU = GetRU(pathRU);
                    listEN = GetEN(pathEN);
                    
                    bull = true;
                }
                else if (answer.ToUpper() == "N")
                {
                    st.Start();
                    //set RU text here
                    listRU.Add("Не выходи из комнаты, не совершай ошибку.");

                    //set EN text here
                    listEN.Add("hello |EN");

                    bull = true;
                }
                else
                {
                    Console.Write("Wrong input!");
                    Console.ReadLine();
                    Console.Clear();
                }
            } while (!bull);

            //call method to clean lines from punctuation marks

            //P.S we have to make different methods for RU and EN lists, as we do not delete '|' char
            //we can face with this char in russian list, so program will pass it.
            //that is why we have to add another method for RU list, but i didn't
            //anyway we can use part of StringCleaner method with if statement if we want to check ru text separately

            //test string
            listRU.Add("Не выходи из комнаты, не совершай ошибку лялялялялляляляляляляля.");

            NewlistRU = StringCleaner(listRU);
            NewlistEN = StringCleaner(listEN);

            //call methods to count petrenko's index
            RuIndex = RuCount(NewlistRU);
            EnIndex = EnCount(NewlistEN);

            //test examples
            listEN.Add("example");
            EnIndex.Add(17968.5);
            listEN.Add("kek");
            EnIndex.Add(17968.5);
            listEN.Add("no");
            EnIndex.Add(17968.5);

            for (int i = 0; i < RuIndex.Count; i++)
            {
                for (int j = 0; j < EnIndex.Count; j++)
                {
                    if (RuIndex[i] == EnIndex[j])
                    {
                        Console.WriteLine("\n\nLine {0} match line {1}", listRU[i], listEN[j]);
                    }
                }
            }

            st.Stop();
            Console.WriteLine("\n\n{0} ms", st.ElapsedMilliseconds);
            Console.ReadLine();

        }

        //collect RU lines from file
        static List<string> GetRU(string pathRU)
        {
            List<string> listRU = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(pathRU, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                        listRU.Add(line);
                }
                return listRU;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\n" + ex.Message);
                Console.ReadLine();
                Console.Clear();
                return null;
            }
        }

        //collect EN lines from file
        static List<string> GetEN(string pathEN)
        {
            List<string> listEN = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader((string)pathEN, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                        listEN.Add(line);
                }
                return listEN;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\n" + ex.Message);
                Console.ReadLine();
                Console.Clear();
                return null;
            }
        }

        //delete punctuation marks, white spaces (digits optional)
        static List<string> StringCleaner(List<string> list)
        {
            List<string> newList = new List<string>();

            string empty = "";
            string newLine;
            foreach (var line in list)
            {
                //remove punctuation
                newLine = line;
                newLine = new Regex(@"[\p{P}-[|]]").Replace(newLine, empty);
                for (int i = 0; i < newLine.Length; i++)
                    if (/*char.IsPunctuation(newLine[i]) || */ newLine[i].ToString() == "\t"/* || char.IsDigit(newLine[i])*/)
                        newLine = newLine.Replace(newLine[i].ToString(), empty);

                //remove white spaces
                newLine = new Regex(@"\s+").Replace(newLine, empty);

                newList.Add(newLine.ToLower());
            }
            return newList;
        }

        //count index-petrenko for RU text
        static List<double> RuCount(List<string> RUlist)
        {
            List<double> listIndex = new List<double>();

            double a1 = 0.5;
            double index, result;

            foreach (var line in RUlist)
            {
                index = ((2 * a1 + 1 * (line.Length - 1)) / 2) * line.Length;
                result = index * line.Length;
                listIndex.Add(result);
            }
            return listIndex;
        }

        //count index-petrenko for EN text
        static List<double> EnCount(List<string> ENlist)
        {
            List<double> listIndex = new List<double>();

            string[] cut;
            double a1 = 0.5;
            double indexMAIN, indexCOMM, result;

            foreach (var line in ENlist)
            {
                cut = line.Split(new char[] { '|' }); //here we can face with situation when several '|' chars meet in the text (or none).
                                                      //I belive that this kind of char is rare so there is no need to check the amount of whitespaces after it
                                                      //(to make sure that no more than five words stands after it)
                                                      //if we need this kind of check we have to clean whitespaces only inside methods where we count petrenko's index
                                                      //and make if-statement to check amount '|' chars in line and than check amount of words after the last '|' char.
                                                      //here I use simple way cause in my texts i put '|' char at the end of english lines
                                                      //(not more than 5 words after '|' char).
                                                      
                indexMAIN = ((2 * a1 + 1 * (cut[0].Length - 1)) / 2) * cut[0].Length;
                indexCOMM = ((2 * a1 + 1 * (cut[1].Length - 1)) / 2) * cut[1].Length;
                result = indexMAIN * cut[0].Length + indexCOMM * cut[1].Length;
                listIndex.Add(result);
            }
            return listIndex;
        }
    }
}
