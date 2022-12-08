using System.Collections.Generic;
using System.IO;
using InformationRetrieval.Index;

namespace InformationRetrieval.Document
{
    public class DiskCollection : AbstractCollection
    {
        public DiskCollection(string directory, Parameter parameter) : base(directory, parameter)
        {
        }

        private bool NotCombinedAllIndexes(int[] currentIdList)
        {
            foreach (var id in currentIdList)
            {
                if (id != -1)
                {
                    return true;
                }
            }

            return false;
        }

        private List<int> SelectIndexesWithMinimumTermIds(int[] currentIdList)
        {
            var result = new List<int>();
            var min = int.MaxValue;
            foreach (var id in currentIdList)
            {
                if (id != -1 && id < min)
                {
                    min = id;
                }
            }

            for (var i = 0; i < currentIdList.Length; i++)
            {
                if (currentIdList[i] == min)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        protected void CombineMultipleInvertedIndexesInDisk(string collectionName, string tmpName, int blockCount)
        {
            StreamReader[] files;
            int[] currentIdList;
            PostingList[] currentPostingLists;
            currentIdList = new int[blockCount];
            currentPostingLists = new PostingList[blockCount];
            files = new StreamReader[blockCount];
            var printWriter = new StreamWriter(collectionName + "-postings.txt");
            for (var i = 0; i < blockCount; i++)
            {
                files[i] = new StreamReader("tmp-" + tmpName + i + "-postings.txt");
                var line = files[i].ReadLine();
                var items = line.Split(" ");
                currentIdList[i] = int.Parse(items[0]);
                line = files[i].ReadLine();
                currentPostingLists[i] = new PostingList(line);
            }

            while (NotCombinedAllIndexes(currentIdList))
            {
                var indexesToCombine = SelectIndexesWithMinimumTermIds(currentIdList);
                var mergedPostingList = currentPostingLists[indexesToCombine[0]];
                for (var i = 1; i < indexesToCombine.Count; i++)
                {
                    mergedPostingList = mergedPostingList.Union(currentPostingLists[indexesToCombine[i]]);
                }

                mergedPostingList.WriteToFile(printWriter, currentIdList[indexesToCombine[0]]);
                foreach (var i in indexesToCombine)
                {
                    var line = files[i].ReadLine();
                    if (line != null)
                    {
                        var items = line.Split(" ");
                        currentIdList[i] = int.Parse(items[0]);
                        line = files[i].ReadLine();
                        currentPostingLists[i] = new PostingList(line);
                    }
                    else
                    {
                        currentIdList[i] = -1;
                    }
                }
            }

            for (var i = 0; i < blockCount; i++)
            {
                files[i].Close();
            }

            printWriter.Close();
        }

        protected void CombineMultiplePositionalIndexesInDisk(string collectionName, int blockCount)
        {
            StreamReader[] files;
            int[] currentIdList;
            PositionalPostingList[] currentPostingLists;
            currentIdList = new int[blockCount];
            currentPostingLists = new PositionalPostingList[blockCount];
            files = new StreamReader[blockCount];
            var printWriter = new StreamWriter(collectionName + "-positionalPostings.txt");
            for (var i = 0; i < blockCount; i++)
            {
                files[i] = new StreamReader("tmp-" + i + "-positionalPostings.txt");
                var line = files[i].ReadLine();
                var items = line.Split(" ");
                currentIdList[i] = int.Parse(items[0]);
                currentPostingLists[i] = new PositionalPostingList(files[i], int.Parse(items[1]));
            }

            while (NotCombinedAllIndexes(currentIdList))
            {
                var indexesToCombine = SelectIndexesWithMinimumTermIds(currentIdList);
                var mergedPostingList = currentPostingLists[indexesToCombine[0]];
                for (var i = 1; i < indexesToCombine.Count; i++)
                {
                    mergedPostingList = mergedPostingList.Union(currentPostingLists[indexesToCombine[i]]);
                }

                mergedPostingList.WriteToFile(printWriter, currentIdList[indexesToCombine[0]]);
                foreach (var i in indexesToCombine)
                {
                    var line = files[i].ReadLine();
                    if (line != null)
                    {
                        string[] items = line.Split(" ");
                        currentIdList[i] = int.Parse(items[0]);
                        currentPostingLists[i] = new PositionalPostingList(files[i], int.Parse(items[1]));
                    }
                    else
                    {
                        currentIdList[i] = -1;
                    }
                }
            }

            for (var i = 0; i < blockCount; i++)
            {
                files[i].Close();
            }

            printWriter.Close();
        }
    }
}