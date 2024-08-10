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

        /// <summary>
        /// In single pass in memory indexing, the index files are merged to get the final index file. This method
        /// checks if all parallel index files are combined or not.
        /// </summary>
        /// <param name="currentIdList">Current pointers for the terms in parallel index files. currentIdList[0] is the current term
        ///                     in the first index file to be combined, currentIdList[2] is the current term in the second
        ///                     index file to be combined etc.</param>
        /// <returns>True, if all merge operation is completed, false otherwise.</returns>
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

        /// <summary>
        /// In single pass in memory indexing, the index files are merged to get the final index file. This method
        /// identifies the indexes whose terms to be merged have the smallest term id. They will be selected and
        /// combined in the next phase.
        /// </summary>
        /// <param name="currentIdList">Current pointers for the terms in parallel index files. currentIdList[0] is the current term
        ///                     in the first index file to be combined, currentIdList[2] is the current term in the second
        ///                     index file to be combined etc.</param>
        /// <returns>An array list of indexes for the index files, whose terms to be merged have the smallest term id.</returns>
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

        /// <summary>
        /// In single pass in memory indexing, the index files are merged to get the final index file. This method
        /// implements the merging algorithm. Reads the index files in parallel and at each iteration merges the posting
        /// lists of the smallest term and put it to the merged index file. Updates the pointers of the indexes accordingly.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="tmpName">Temporary name of the index files.</param>
        /// <param name="blockCount">Number of index files to be merged.</param>
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

        /// <summary>
        /// In single pass in memory indexing, the index files are merged to get the final index file. This method
        /// implements the merging algorithm. Reads the index files in parallel and at each iteration merges the positional
        /// posting lists of the smallest term and put it to the merged index file. Updates the pointers of the indexes accordingly.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="blockCount">Number of index files to be merged.</param>
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