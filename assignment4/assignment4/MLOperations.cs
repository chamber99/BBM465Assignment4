using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.ML;
using Accord.MachineLearning.DecisionTrees;
using Accord.Math.Optimization.Losses;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Analysis;
using Accord.Statistics;

namespace assignment4
{
    internal class MLOperations
    {
        public MLOperations()
        {

        }

        public void useKNN() {
            Accord.MachineLearning.KNearestNeighbors knn = new Accord.MachineLearning.KNearestNeighbors();
            //knn.Learn();

        }

        public void useLSVM(int input) {
            SupportVectorMachine svm = new SupportVectorMachine(input);
            
        }

        public void useRandomForest(double[][] learn, int[] classlabels, double[][] validate, int[] predictionlabels) {
            RandomForestLearning forestLearning = new RandomForestLearning();
            

            RandomForest forest = forestLearning.Learn(learn, classlabels);
            int[] prediction = forest.Decide(validate);
            double error = new ZeroOneLoss(predictionlabels).Loss(prediction);
            double acc = new AccuracyLoss(predictionlabels).Loss(prediction);

            Console.WriteLine("Error : " + error);
            Console.WriteLine("Accuracy : " + acc);

            Console.WriteLine("Labels size: " + classlabels.Length + " Prediction size: " + prediction.Length);

            var cm = new GeneralConfusionMatrix(classes: 15, expected: predictionlabels, predicted: prediction);

            int[,] matrix = cm.Matrix;
            /*double truePositives = 0;
            double falseNegatives = 0;

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if(i == j)
                    {
                        truePositives += Convert.ToDouble(matrix[i, j]);
                    }
                    else
                    {
                        falseNegatives += Convert.ToDouble(matrix[i, j]);

                    }
                }

            }
            Console.WriteLine("TPR : {0}",truePositives/(truePositives+falseNegatives));
            Console.WriteLine("FPR : {0}", truePositives / (truePositives + falseNegatives));
            Console.WriteLine("F1 : {0}", truePositives / (truePositives + falseNegatives));*/

            // Micro approach was used Berkayım işte burada sıçtık ..... 
            ConfusionMatrix[] allMatrices = cm.PerClassMatrices;
            double totalTruePositives = 0;
            double totalFalsePositives = 0;
            double totalFalseNegatives = 0;
            double totalTrueNegatives = 0;

            foreach (ConfusionMatrix confusionMatrix in allMatrices)
            {

                totalTruePositives += confusionMatrix.ActualPositives;
                totalTrueNegatives += confusionMatrix.ActualNegatives;
                totalFalsePositives += confusionMatrix.FalsePositives;
                totalFalseNegatives += confusionMatrix.FalseNegatives;
            }

            Console.WriteLine("TPR {0}", totalTruePositives / (totalTruePositives + totalFalseNegatives));
            Console.WriteLine("FPR {0}", totalFalsePositives / (totalFalsePositives + totalTrueNegatives));
            Console.WriteLine("F1 {0}", (2 * totalTruePositives) / (2 * totalTruePositives + totalFalsePositives + totalFalseNegatives));







        }


    }
}
