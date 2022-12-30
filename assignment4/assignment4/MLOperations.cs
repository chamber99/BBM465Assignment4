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

        public void useRandomForest(double[][] learn, int[] classlabels, double[][] validate) {
            RandomForestLearning forestLearning = new RandomForestLearning();
            forestLearning.NumberOfTrees= 64;

            RandomForest forest = forestLearning.Learn(learn, classlabels);
            int[] prediction = forest.Decide(validate);
            double error = new ZeroOneLoss(classlabels).Loss(prediction);

            


        }


    }
}
