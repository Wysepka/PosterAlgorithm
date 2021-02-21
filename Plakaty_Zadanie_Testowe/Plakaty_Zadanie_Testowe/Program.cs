using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

//============================================//
/// <summary>
/// Github Link: https://github.com/Wysepka/PosterAlgorithm
/// </summary>
//============================================//
namespace Plakaty_Zadanie_Testowe
{
    #region Main Program
    class Program
    {

        //Static file destination
        static string fileDestination = @"";
        //Write File Destination
        static string writeFileDestination = @"";

        //Main program method
        static void Main(string[] args)
        {
            BlockAssigment blockAssigment = new BlockAssigment();

            ReadInputData(blockAssigment);

            int postersCalculatedV1 = AlgorithmCalculation(blockAssigment);

            PrintAlgorithmResult(postersCalculatedV1);

            WriteResultToFile(blockAssigment, postersCalculatedV1);
        }

        private static void ReadInputData(BlockAssigment blockAssigment)
        {
            //Input by User
            blockAssigment.ReadBlocksFromConsole();

            //Input by File
            //blockAssigment.ReadingFileToBlocksArray(fileDestination);
        }

        private static int AlgorithmCalculation(BlockAssigment blockAssigment)
        {
            return blockAssigment.CalculatePostersV1();
            //return blockAssigment.CalculatePostersV2NestedLists();
        }

        private static void PrintAlgorithmResult(int result)
        {
            Console.WriteLine(result);
        }

        private static void WriteResultToFile(BlockAssigment blockAssigment, int postersCalculatedV1)
        {
            blockAssigment.WriteResultToOutFile(postersCalculatedV1, Path.Combine(writeFileDestination + "resultV1.out"));
            //blockAssigment.WriteResultToOutFile(postersCalculatedV2,Path.Combine(writeFileDestination+"resultV2.out"));
        }
    }

    #endregion

    //Block Assigment class from which all calculation takes place
    //BlockAssigment gathers all Methods and Data
    public class BlockAssigment
    {
        //Block array to write information about blocks length etc...
        Block[] blocks;

        //Blocks height, used for V1 algorithm implementation
        //Used List instead of Array, because, 
        //(friend told me that (non-nested) lists are quicker(//more comfortable))
        List<int> blocksHeight;

        //Clear constructor;
        public BlockAssigment()
        {

        }

        #region Reading Files to Block Array

        //Reading StringText to Block Array class from a string
        //Disadvantage of using method below it only calculates
        //Blocks that are lesser than 10
        public void ReadingStringToBlocksArray(string textFile)
        {
            int blocksCount = int.Parse(textFile[0].ToString());
            blocks = new Block[blocksCount];
            string textFileTrimmed;

            textFileTrimmed = Regex.Replace(textFile, @"\s+", string.Empty);

            int blocksIterator = 0;
            for (int i = 1; i < textFileTrimmed.Length; i += 2)
            {
                int blockWidth = int.Parse(textFileTrimmed[i].ToString());
                int blockHeight = int.Parse(textFileTrimmed[i + 1].ToString());

                Console.WriteLine("Parsing width:{0}, height:{1}, block:{2} ", blockWidth, blockHeight, blocksIterator);

                blocks[blocksIterator] = new Block(blockWidth, blockHeight);
                blocksIterator++;
            }

            for (int i = 0; i < blocks.Length; i++)
            {
                Console.WriteLine("Block: {0} , Width: {1} , Height: {2}", i.ToString(), blocks[i].Width.ToString(), blocks[i].Height.ToString());
            }

        }

        //Reading File to BlocksArray
        //Implemented second option when Width or Height of block
        //has two digits
        public void ReadingFileToBlocksArray(string fileDestination)
        {
            using (StreamReader sr = File.OpenText(fileDestination))
            {
                //Reading first line of Text file to initialize Array
                string blocksCountString = sr.ReadLine();
                int blocksCountInt = int.Parse(blocksCountString);

                blocks = new Block[blocksCountInt];
                blocksHeight = new List<int>();

                //Console.WriteLine("Blocks Count String: " + blocksCountString);
                string blocksData = "";
                int blocksIterator = 0;

                //Reading one line at a time
                while ((blocksData = sr.ReadLine()) != null)
                {
                    int blockWidth, blockHeight;

                    AssignBlocksParamsFromStringInput(blocksData,out blockWidth,out blockHeight);

                    //Assigning block Array
                    blocks[blocksIterator] = new Block(blockWidth, blockHeight);
                    blocksHeight.Add(blockHeight);

                    //Console.WriteLine("Block Iterator: {0} , W:{1} , H:{2} " , blocksIterator , dimensions[0] , dimensions[1]);

                    blocksIterator++;
                }
                //Console.WriteLine("Blocks Array Assigned Count:{0}", blocksIterator);
            }
        }

        //Reading Input from Console, and assigning 
        //BlocksHeight list with heights applied by user
        public void ReadBlocksFromConsole()
        {
            int numberOfBlocks = int.Parse(Console.ReadLine());
            blocksHeight = new List<int>();

            for (int i = 0; i < numberOfBlocks; i++)
            {
                string readLine = Console.ReadLine();
                int blockWidth = 0;
                int blockHeight = 0;

                AssignBlocksParamsFromStringInput(readLine,out blockWidth,out blockHeight);

                blocksHeight.Add(blockHeight);
            }
        }

        //Assigning Block Width,Height from string input
        //Seperating int(Chars) from whitespaces
        private void AssignBlocksParamsFromStringInput(string input, out int width, out int height)
        {
            List<char> dimensionsChar = new List<char>();
            width = 0;
            height = 0;
            for (int i = 0; i < input.Length; i++)
            {
                //When Whitespace is met, Char[] is Parsed to Int
                if (char.IsWhiteSpace(input[i]))
                {
                    string numberString = new string(dimensionsChar.ToArray());
                    int number = int.Parse(numberString);
                    width = number;
                    dimensionsChar = new List<char>();
                }
                //Else, char numbers are added
                else dimensionsChar.Add(input[i]);
            }
            string number2String = new string(dimensionsChar.ToArray());
            int number2 = int.Parse(number2String);
            height = number2;
        }

        #endregion

        /// <summary>
        /// The Most Basic Algorithm implementation.
        /// Very similiar as the main described in PDF file.
        /// Also similiar as Top - to - Down, version,
        /// Link - Explanation: https://i.imgur.com/GZRuet2.png
        /// </summary>
        /// <returns></returns>
        #region Calculating Posters V1

        public int CalculatePostersV1()
        {
            int postersUsed = 0;
            Stack<int> blocksStacked = new Stack<int>();

            for (int i = 0; i < blocksHeight.Count; i++)
            {
                while (blocksStacked.Count > 0 && blocksStacked.Peek() > blocksHeight[i])
                {
                    blocksStacked.Pop();
                    postersUsed++;
                }
                if (blocksStacked.Count == 0 || blocksStacked.Peek() < blocksHeight[i])
                {
                    blocksStacked.Push(blocksHeight[i]);
                }
            }
            postersUsed += blocksStacked.Count;

            return postersUsed;

        }

        #endregion

        /// <summary>
        /// The downside of this version is Data complication
        /// It can be helpfull for, more advanced implementations
        /// Such as nested, data in blocks or saving data for further
        /// Algorithm takes time because of nested lists,
        /// and multiple loops
        /// </summary>
        /// <returns></returns>
        #region Calculating Posters V2 Methods

        //Second method to calculate Posters Count similiar to Algorithm
        //mentioned in pdf file "od ziemi do góry", 
        //added nested list implementation
        //Such feature allows to split array into groups
        //Explanation: - Link: https://i.imgur.com/3UTkZA9.png
        public int CalculatePostersV2NestedLists()
        {
            int postersUsed = 0;

            List<List<int>> blocksList = WriteBlocksToNestedList();

            //Condition checks if blockslist has any values
            while (CheckPostersAppliedCondition(blocksList))
            {
                int numbersCount = 0;
                for (int i = 0; i < blocksList.Count; i++)
                {

                    int lowestHeight = ReturnSmallestValue(blocksList[i]);

                    for (int j = 0; j < blocksList[i].Count; j++)
                    {
                        blocksList[i][j] -= lowestHeight;
                        numbersCount++;
                    }

                    postersUsed++;
                }

                blocksList = SortNestedList(blocksList);
                //PrintNestedIntList(blocksList);
                //Console.WriteLine("Progress 0/100 | {0}% |" , (float)numbersCount * 100f/(float)blocks.Length);
            }
            return postersUsed;
        }

        //Sorting NestedList, taking blocks list after each iteration
        //This method calculates groups and assigns them back to main List
        //Could be done in different manner, such as Dynamic Solving
        private List<List<int>> SortNestedList(List<List<int>> blocksList)
        {
            List<List<int>> sortedList = new List<List<int>>();
            for (int i = 0; i < blocksList.Count; i++)
            {
                List<int> insideList = new List<int>();
                for (int j = 0; j < blocksList[i].Count; j++)
                {
                    if(blocksList[i][j] == 0)
                    {
                        if (insideList != null && insideList.Count > 0)
                        {
                            sortedList.Add(insideList);
                            insideList = new List<int>();
                        }
                    }
                    else
                    {
                        insideList.Add(blocksList[i][j]);
                    }
                }
                if (insideList != null && insideList.Count > 0) sortedList.Add(insideList);
            }
            return sortedList;
        }

        private bool CheckPostersAppliedCondition(List<List<int>> blockList)
        {
            bool keepChecking = false;
            for (int i = 0; i < blockList.Count; i++)
            {
                for (int j = 0; j < blockList[i].Count; j++)
                {
                    if (blockList[i][j] > 0) keepChecking = true;
                }
            }
            return keepChecking;
        }

        //Returning smallest value from Group of blocks
        private int ReturnSmallestValue(List<int> blockList)
        {
            int smallestValue = int.MaxValue;

            for (int i = 0; i < blockList.Count; i++)
            {
                if (blockList[i] < smallestValue) smallestValue = blockList[i];
            }
            return smallestValue;
        }

        private static void PrintNestedIntList(List<List<int>> blocksList)
        {
            Console.WriteLine("=======================================");
            for (int i = 0; i < blocksList.Count; i++)
            {
                Console.WriteLine("Block: {0} , Values: ", i.ToString());

                for (int j = 0; j < blocksList[i].Count; j++)
                {
                    Console.WriteLine(", {0}", blocksList[i][j]);
                }
            }
        }

        //Writing blocks to nested list
        //Used at the beggining to write from Blocks Array
        private List<List<int>> WriteBlocksToNestedList()
        {
            List<List<int>> nestedBlocks = new List<List<int>>();
            List<int> blockListAdder = new List<int>();

            bool optimizeTreeshold = false;
            for (int i = 0; i < blocks.Length; i++)
            {
                /*
                if (optimizeTreeshold && i+1 < blocks.Length && blocks[i].Height < blocks[i+1].Height)
                {
                    nestedBlocks.Add(blockListAdder);
                    blockListAdder = new List<int>();
                    optimizeTreeshold = false;
                }
                else */ if (blocks[i].Height > 0)
                {
                    blockListAdder.Add(blocks[i].Height);
                    if (blockListAdder.Count > 100) optimizeTreeshold = true;
                }
                else
                {
                    nestedBlocks.Add(blockListAdder);
                    blockListAdder = new List<int>();
                    optimizeTreeshold = false;
                }
            }

            if (blockListAdder.Count > 0) nestedBlocks.Add(blockListAdder);

            return nestedBlocks;
        }

        #endregion

        #region Writing Result to File

        public void WriteResultToOutFile(int postersCount , string fileName)
        {
            using(StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine(postersCount.ToString());
            }
        }

        #endregion

        //Block class for assigning each value from File
        public class Block
        {
            int width;
            int height;
            //-----------------------------------------//
            //--------------- Accesors ----------------//
            public int Width { get { return width; } }
            public int Height { get { return height; } }
            //-----------------------------------------//
            //-------------- Constructor --------------//
            public Block(int width, int height)
            {
                this.width = width;
                this.height = height;
            }

        }
    }


    /// <summary>
    ///Debug Helper class for printing Blocks array 
    /// </summary>
    public static class DebugHelper
    {
        public static void PrintBlockArray(BlockAssigment.Block [] blocks)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                Console.WriteLine("Blocks: W:{0} , H:{1} ", blocks[i].Width , blocks[i].Height);
            }

        }

    }
}