using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.ML;

namespace assignment4
{
    internal class MLOperations
    {
        public MLOperations()
        {

        }

        public void useKNN() { 
            Emgu.CV.ML.KNearest knn = new KNearest();
            
        
        }

        public void useLSVM() { 
           Emgu.CV.ML.SVM svm = new SVM();
            //svm.Train();
            //svm.Predict();
        }

        public void useRandomForest() {
            Emgu.CV.ML.RTrees rTrees = new Emgu.CV.ML.RTrees();
            //rTrees.Train();
            //rTrees.Predict();

        }


    }
}
