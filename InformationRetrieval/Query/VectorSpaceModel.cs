using InformationRetrieval.Document;
using InformationRetrieval.Index;

namespace InformationRetrieval.Query
{
    public class VectorSpaceModel
    {
        private double[] model;

        /// <summary>
        /// Constructor for the VectorSpaceModel class. Calculates the normalized tf-idf vector of a single document.
        /// </summary>
        /// <param name="termFrequencies">Term frequencies in the document</param>
        /// <param name="documentFrequencies">Document frequencies of terms.</param>
        /// <param name="documentSize">Number of documents in the collection</param>
        /// <param name="termWeighting">Term weighting scheme applied in term frequency calculation.</param>
        /// <param name="documentWeighting">Document weighting scheme applied in document frequency calculation.</param>
        public VectorSpaceModel(int[] termFrequencies, int[] documentFrequencies, int documentSize,
            TermWeighting termWeighting, DocumentWeighting documentWeighting)
        {
            double sum = 0;
            model = new double[termFrequencies.Length];
            for (var i = 0; i < termFrequencies.Length; i++)
            {
                model[i] = Weighting(termFrequencies[i], documentFrequencies[i], documentSize, termWeighting,
                    documentWeighting);
                sum += model[i] * model[i];
            }

            for (var i = 0; i < termFrequencies.Length; i++)
            {
                model[i] /= System.Math.Sqrt(sum);
            }
        }

        /// <summary>
        /// Returns the tf-idf value for a column at position index
        /// </summary>
        /// <param name="index">Position of the column</param>
        /// <returns>tf-idf value for a column at position index</returns>
        public double Get(int index)
        {
            return model[index];
        }

        /// <summary>
        /// Calculates the cosine similarity between this document vector and the given second document vector.
        /// </summary>
        /// <param name="secondModel">Document vector of the second document.</param>
        /// <returns>Cosine similarity between this document vector and the given second document vector.</returns>
        public double CosineSimilarity(VectorSpaceModel secondModel)
        {
            var sum = 0.0;
            if (model.Length != secondModel.model.Length)
            {
                return 0.0;
            }
            else
            {
                for (var i = 0; i < model.Length; i++)
                {
                    sum += model[i] * secondModel.model[i];
                }
            }

            return sum;
        }
        
        /// <summary>
        /// Calculates tf-idf value of a single word (column) of the document vector.
        /// </summary>
        /// <param name="termFrequency">Term frequency of this word in the document</param>
        /// <param name="documentFrequency">Document frequency of this word.</param>
        /// <param name="documentSize">Number of documents in the collection</param>
        /// <param name="termWeighting">Term weighting scheme applied in term frequency calculation.</param>
        /// <param name="documentWeighting">Document weighting scheme applied in document frequency calculation.</param>
        /// <returns>tf-idf value of a single word (column) of the document vector.</returns>
        public static double Weighting(double termFrequency, double documentFrequency, int documentSize,
            TermWeighting termWeighting, DocumentWeighting documentWeighting)
        {
            double multiplier1 = 1, multiplier2 = 1;
            switch (termWeighting)
            {
                case TermWeighting.NATURAL:
                    multiplier1 = termFrequency;
                    break;
                case TermWeighting.LOGARITHM:
                    if (termFrequency > 0)
                        multiplier1 = 1 + System.Math.Log(termFrequency);
                    else
                        multiplier1 = 0;
                    break;
                case TermWeighting.BOOLE:
                    if (termFrequency > 0)
                    {
                        multiplier1 = 1;
                    }
                    else
                    {
                        multiplier1 = 0;
                    }

                    break;
            }

            switch (documentWeighting)
            {
                case DocumentWeighting.NO_IDF:
                    multiplier2 = 1;
                    break;
                case DocumentWeighting.IDF:
                    multiplier2 = System.Math.Log(documentSize / (documentFrequency + 0.0));
                    break;
                case DocumentWeighting.PROBABILISTIC_IDF:
                    if (documentSize > 2 * documentFrequency)
                    {
                        multiplier2 = System.Math.Log((documentSize - documentFrequency) / (documentFrequency + 0.0));
                    }
                    else
                    {
                        multiplier2 = 0.0;
                    }

                    break;
            }

            return multiplier1 * multiplier2;
        }
    }
}