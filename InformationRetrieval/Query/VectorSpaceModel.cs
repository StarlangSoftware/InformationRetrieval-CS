using InformationRetrieval.Document;
using InformationRetrieval.Index;

namespace InformationRetrieval.Query
{
    public class VectorSpaceModel
    {
        private double[] model;

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

        public double Get(int index)
        {
            return model[index];
        }

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