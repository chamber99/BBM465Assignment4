using assignment4;
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Accord.MachineLearning;
using Accord.Statistics;
using Emgu.CV.XFeatures2D;
using OpenTK.Audio.OpenAL;
using System.Security.Policy;
using System.Linq;
using System.Diagnostics;
using Accord.Imaging;
using Accord.Vision;
using static Accord.Imaging.Filters.StereoAnaglyph;
using Accord.Math;
using Emgu.CV;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Analysis;
using OpenTK.Graphics.ES20;
using Emgu.CV.Structure;

public class Assignment4
{
    public static void Main(String[] args)
    {
        run(args);
        //readCSVFile("../../../precomputed/precomputed_SURF_train.csv", "../../../precomputed/precomputed_SURF_val.csv");

        /*MLOperations ops = new MLOperations();
        Console.WriteLine("CEDD");

        int[] class_cedd = prepareClassNames("CEDD","train");
        int[] pred_cedd = prepareClassNames("CEDD","val");

        double[][] cedd_train = prepareMLInputs("CEDD", "train");
        double[][] cedd_val = prepareMLInputs("CEDD", "val");





        List<int> indexes = checkConstants(cedd_train);
        Console.Write("indexes: ");
        foreach (int i in indexes) {
            Console.Write(i + " ");
        
        }
        Console.WriteLine();


        cedd_train = removeColumn(cedd_train, indexes);
        cedd_val = removeColumn(cedd_val, indexes);
        

        ops.useRandomForest(cedd_train, class_cedd, cedd_val,pred_cedd);

        Console.WriteLine("FCTH");

        int[] class_FCTH = prepareClassNames("FCTH","train");
        int[] pred_FCTH = prepareClassNames("FCTH", "val");
        double[][] FCTH_train = prepareMLInputs("FCTH", "train");
        double[][] FCTH_val = prepareMLInputs("FCTH", "val");

        List<int> indexes_fcth = checkConstants(FCTH_train);
        Console.Write("indexes: ");
        foreach (int i in indexes_fcth)
        {
            Console.Write(i + " ");

        }
        Console.WriteLine();


        FCTH_train = removeColumn(FCTH_train, indexes_fcth);
        FCTH_val = removeColumn(FCTH_val, indexes_fcth);



        ops.useRandomForest(FCTH_train, class_FCTH, FCTH_val,pred_FCTH);*/




    }

    public static void run(String[] arguments)
    {
        // Evaluating the command line arguments.
        String path = "";
        String mode = "";
        if (arguments.Length > 0)
        {   
            for (int i = 0; i < arguments.Length; i++)
            {
                string argument = arguments[i].ToLower();
                switch (argument)
                {
                    case "-dataset":
                        // Getting relative path of the dataset.
                        path = arguments[i + 1];
                        break;
                    case "-mode":
                        // Getting the mode of operation.
                        mode = arguments[i + 1];
                        break;
                }
            }
            startOperation(mode, path);


        }
        else
        {
            Console.WriteLine("No argument provided!");
        }
    }

    public static void startOperation(String modeName,String path)
    {
        if (modeName.ToLower().Equals("precompute"))
        {
            // Creating list structures to store image files.
            List<String> images_training = new List<String>();
            List<String> images_validation = new List<String>();
            
            // Printing dataset name to the console.
            String dataset_name = path.Trim('\\', '.');
            Console.WriteLine("Reading " + dataset_name + "...");

            // Calculating number of image files.
            int train_count = Directory.GetFiles(path + "\\train", "*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg")).ToList().Count;
            Console.WriteLine(train_count + " images found in train folder");
            int val_count = Directory.GetFiles(path + "\\val", "*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg")).ToList().Count;
            Console.WriteLine(val_count + " images found in val folder");

            // Calculating number of distinct classes.
            String[] dirs = Directory.GetDirectories(path + "\\train", "*", SearchOption.AllDirectories);
            HashSet<String> classSet = new HashSet<string>();
            foreach (String dir in dirs)
            {
                classSet.Add(dir.Trim('\\', '.').Trim('\\').Replace("phishIRIS_DL_Dataset", "").Replace("train", "").Replace("val", "").Trim('\\'));
            }
            int class_count = classSet.Count;
            Console.WriteLine(class_count + " classes exist.");

            // Fetching the names of all image files between lines 300-324.

            string[] train_subdirectories = Directory.GetDirectories(path + "\\train");
            string[] val_subdirectories = Directory.GetDirectories(path + "\\val");
            
            // Train images
            foreach (String s in train_subdirectories)
            {
                string[] folders = s.Split('\\');
                string className = folders[folders.Length - 1];
                foreach (String file in Directory.GetFiles(path + "\\train\\" + className))
                {      
                    images_training.Add(file);
                }
            }

            // Val images
            foreach (String s in val_subdirectories)
            {
                string[] folders = s.Split('\\');
                string className = folders[folders.Length - 1];
                foreach (String file in Directory.GetFiles(path + "\\val\\" + className))
                {       
                    images_validation.Add(file);
                }
            }

            // Generating precomputed csv files to store features.
            generatePrecompute("CEDD", images_training, images_validation);
            generatePrecompute("FCTH", images_training, images_validation);
            //generatePrecompute("SURF", images_training, images_validation);



        }
    }

    public static void generatePrecompute(String algorithm,List<String> images_train,List<String> images_val)
    {
        // Creating a stopwatch object to calculate time taken for each algorithm.
        Stopwatch stopwatch = new Stopwatch();

        if(algorithm.Equals("CEDD") || algorithm.Equals("FCTH")){
            // Names of the output files.
            String trainingFile = "precomputed_" + algorithm + "_train.csv";
            String validatingFile = "precomputed_" + algorithm + "_val.csv";
            
            Console.WriteLine($"{algorithm} features are being extracted for train...");
            stopwatch.Start();
            
            // Extracting global features using specified algorithm and writing results to csv files.
            StringBuilder builder_train = extractGlobalFeatures(algorithm, images_train);
            createCSV("..\\..\\..\\precomputed\\" + trainingFile, builder_train.ToString());
            stopwatch.Stop();
            TimeSpan timeSpanTrain = stopwatch.Elapsed;
            Console.WriteLine("Done. {0} is regenerated in {1} seconds.",trainingFile,(int)timeSpanTrain.TotalSeconds);
            
            Console.WriteLine($"{algorithm} features are being extracted for val...");
            stopwatch.Restart();
            StringBuilder builder_val = extractGlobalFeatures(algorithm, images_val);
            createCSV("..\\..\\..\\precomputed\\" + validatingFile, builder_val.ToString());
            stopwatch.Stop();
            TimeSpan timeSpanVal = stopwatch.Elapsed;
            Console.WriteLine("Done. {0} is regenerated in {1} seconds.", validatingFile, (int)timeSpanVal.TotalSeconds);


        }
        else if (algorithm.Equals("SURF"))
        {

            extractLocalFeatures(images_train,images_val);
        }
    }

    public static void extractLocalFeatures(List<String> trainingImages,List<String> validatingImages) 
    {
        Stopwatch stopwatch = new Stopwatch();
        Bitmap[] allTrainingImages = new Bitmap[trainingImages.Count];
        int index = 0;
        foreach (String fileName in trainingImages)
        {
            allTrainingImages[index++] = new Bitmap(System.Drawing.Image.FromFile(fileName));
        }

        Console.WriteLine("SURF features are being extracted for train...");
        stopwatch.Start();
        var bag_of_visual_words = BagOfVisualWords.Create(numberOfWords: 400);
        bag_of_visual_words.Learn(allTrainingImages);
        StringBuilder trainingData = getFeatureVector(bag_of_visual_words, allTrainingImages, trainingImages);
        createCSV("..\\..\\..\\precomputed\\" + "precomputed_SURF_train.csv",trainingData.ToString());
        stopwatch.Stop();
        TimeSpan timeSpanTrain = stopwatch.Elapsed;
        Console.WriteLine("Done. precomputed_SURF_train.csv is regenerated in {0} seconds.", (int)timeSpanTrain.TotalSeconds);



        index = 0;
        Bitmap[] allValidatingImages = new Bitmap[validatingImages.Count];
        foreach (String fileName in validatingImages)
        {
            allValidatingImages[index++] = new Bitmap(System.Drawing.Image.FromFile(fileName));
        }

        Console.WriteLine("SURF features are being extracted for val...");
        stopwatch.Restart();
        StringBuilder validatingData = getFeatureVector(bag_of_visual_words, allValidatingImages, validatingImages);
        createCSV("..\\..\\..\\precomputed\\" + "precomputed_SURF_val.csv", validatingData.ToString());
        stopwatch.Stop();
        TimeSpan timeSpanVal = stopwatch.Elapsed;
        Console.WriteLine("Done. precomputed_SURF_val.csv is regenerated in {0} seconds.", (int)timeSpanVal.TotalSeconds);

    }
    public static StringBuilder getFeatureVector(BagOfVisualWords visualWords, Bitmap[] images,List<String> imageFiles)
    {
        int index = 0;
        StringBuilder builder = new StringBuilder();
        foreach (Bitmap bitmap in images)
        {

            double[] values = visualWords.GetFeatureVector(bitmap);

            foreach (double value in values)
            {
                builder.Append(value.ToString() + ",");
            }
            builder.Remove(builder.Length - 1, 1);
            String fileName = imageFiles[index];
            int len = fileName.Split('\\').Length;
            builder.Append($" {fileName.Split('\\')[len - 2]}\n");
            index++;
        }

        return builder;



    }

    public static void createCSV(String path,String content)
    {
        // Creating new csv files or overriding the content of existed ones using StreamWriter.  
        StreamWriter csvCreator = new StreamWriter(path);
        csvCreator.Write(content);
        File.SetAttributes(path, FileAttributes.Normal);
        FileInfo fileinfo = new FileInfo(path);
        fileinfo.IsReadOnly = false;

        csvCreator.Close();
    }

    public static void readCSVFile(String pathTrain,String pathVal)
    {
        assignment4.Constants constants = new assignment4.Constants();
        string[] lines = System.IO.File.ReadAllLines(pathTrain);
        double[][] features = new double[lines.Length][];
        int[] classes = new int[lines.Length];
        int index = 0;
        foreach (string line in lines)
        {   
            // Splitting line by whitespace
            string[] splitLine = line.Split(null);
            double[] doubles = Array.ConvertAll(splitLine[0].Split(','), new Converter<string, double>(Double.Parse));
            features[index] = doubles;
            classes[index] = constants.getClasses()[splitLine[1]];
            index++;
        }
        // Create a one-vs-one multi-class SVM learning algorithm 
        var teacher = new MulticlassSupportVectorLearning<Linear>()
        {
            // using LIBLINEAR's L2-loss SVC dual for each SVM
            Learner = (p) => new LinearDualCoordinateDescent()
            {
                Loss = Loss.L2
            }
        };

        // Learn a machine
        var machine = teacher.Learn(features,classes);
        index = 0;
        lines = System.IO.File.ReadAllLines(pathVal);
        features = new double[lines.Length][];
        classes = new int[lines.Length];    
        foreach (string line in lines)
        {
            // Splitting line by whitespace
            string[] splitLine = line.Split(null);
            double[] doubles = Array.ConvertAll(splitLine[0].Split(','), new Converter<string, double>(Double.Parse));
            features[index] = doubles;
            classes[index] = constants.getClasses()[splitLine[1]];
            index++;
        }
        index = 0;
        // Obtain class predictions for each sample
        int[] predictedM = machine.Decide(features);

        foreach(int i in predictedM) {
            Console.WriteLine("predicted {0} + normal {1}", i, classes[index++]);
        }

        var cm = new GeneralConfusionMatrix(classes: 15, expected: classes, predicted: predictedM);

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
        double totalFalsePositives = 0 ;
        double totalFalseNegatives = 0;
        double totalTrueNegatives = 0;

        foreach(ConfusionMatrix confusionMatrix in allMatrices)
        {

            totalTruePositives += confusionMatrix.ActualPositives;
            totalTrueNegatives += confusionMatrix.ActualNegatives;
            totalFalsePositives += confusionMatrix.FalsePositives;
            totalFalseNegatives += confusionMatrix.FalseNegatives;
        }

        Console.WriteLine("TPR {0}",totalTruePositives/(totalTruePositives+totalFalseNegatives));
        Console.WriteLine("FPR {0}", totalFalsePositives / (totalFalsePositives + totalTrueNegatives));
        Console.WriteLine("F1 {0}", (2 * totalTruePositives) / (2 * totalTruePositives + totalFalsePositives + totalFalseNegatives));


        MLOperations ops = new MLOperations();

        


    }

    public static double[][] prepareMLInputs(String algorithm, String mode)
    {
        string[] lines = null;
        double[][] output = null;

        if (algorithm.Equals("CEDD"))
        {
            if (mode.Equals("train"))
            {
                output = new double[1313][];
                lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_CEDD_train.csv");
            }
            else
            {
                output = new double[1539][];
                lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_CEDD_val.csv");
            }
        }

        else if (algorithm.Equals("FCTH"))
        {
            if (mode.Equals("train"))
            {
                output = new double[1313][];
                lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_FCTH_train.csv");
            }
            else
            {
                output = new double[1539][];
                lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_FCTH_val.csv");
            }
        }
        else if (algorithm.Equals("SURF"))
        {
            if (mode.Equals("train"))
            {
                output = new double[1313][];
                lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_SURF_train.csv");
            }
            else
            {
                output = new double[1539][];
                lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_SURF_val.csv");
            }
        }
        else
        {
            return null;
        }

        for (int i = 0; i < output.Length; i++)
        {
            String line = lines[i];
            string[] splitLine = line.Split(null);
            double[] doubles = Array.ConvertAll(splitLine[0].Split(','), new Converter<string, double>(Double.Parse));
            output[i] = doubles;
        }

        return output;
    }

    public static int[] prepareClassNames(String algorithm,String mode)
    {
        // we only need the classes from training.
        int[] classes = mode.Equals("train") ? new int[1313] : new int[1539];
        string[] lines = null;

        if (algorithm.Equals("CEDD"))
        {
            if(mode.Equals("train"))
            lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_CEDD_train.csv");
            else
                lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_CEDD_val.csv");
        }
        else if (algorithm.Equals("FCTH"))
        {
            if(mode.Equals("train"))
            lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_FCTH_train.csv");
            else
                lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_FCTH_val.csv");
        }
        else if (algorithm.Equals("SURF"))
        {
            if (mode.Equals("train"))
                lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_SURF_train.csv");
            else
                lines = File.ReadAllLines("..\\..\\..\\precomputed\\precomputed_SURF_val.csv");
        }
        else
        {
            return null;
        }

        for (int i = 0; i < classes.Length; i++)
        {
            if (lines[i].Contains("adobe"))
                classes[i] = 0;
            else if (lines[i].Contains("alibaba"))
                classes[i] = 1;
            else if (lines[i].Contains("amazon"))
                classes[i] = 2;
            else if (lines[i].Contains("apple"))
                classes[i] = 3;
            else if (lines[i].Contains("boa"))
                classes[i] = 4;
            else if (lines[i].Contains("chase"))
                classes[i] = 5;
            else if (lines[i].Contains("dhl"))
                classes[i] = 6;
            else if (lines[i].Contains("dropbox"))
                classes[i] = 7;
            else if (lines[i].Contains("facebook"))
                classes[i] = 8;
            else if (lines[i].Contains("linkedin"))
                classes[i] = 9;
            else if (lines[i].Contains("microsoft"))
                classes[i] = 10;
            else if (lines[i].Contains("other"))
                classes[i] = 11;
            else if (lines[i].Contains("paypal"))
                classes[i] = 12;
            else if (lines[i].Contains("wellsfargo"))
                classes[i] = 13;
            else if (lines[i].Contains("yahoo"))
                classes[i] = 14;
            else
                classes[i] = -1;
        }
        return classes;
    }

    public static List<int> checkConstants(double[][] target) {

        List<int> indexes = new List<int>();
        double[] variences = Measures.Variance(target);

        int index = 0;
        foreach (double d in variences) {
            if (d == 0) {
                indexes.Add(index);
            }
            index++;
        }

       
        return indexes;


    }

    
    
    public static double[][] removeColumn (double[][] target, List<int> indexes) {

        double[,] doubles = target.ToMatrix();
        doubles = Matrix.Remove<double>(doubles, null, indexes.ToArray());

        target = doubles.ToArray();

        Console.WriteLine(target.Length + " " + target[1].Length);
        return target;


    }



    public static StringBuilder extractGlobalFeatures(String algorithm,List<String> images)
    {
        StringBuilder builder = new StringBuilder();
        ImageOperations imageOperations = new ImageOperations();
        foreach (String image in images)
        {
            double[] values;
            if (algorithm.Equals("CEDD"))
            {
                values = imageOperations.useCEDD(new Bitmap(System.Drawing.Image.FromFile(image)));
            }
            else
            {
                values = imageOperations.useFCTH(new Bitmap(System.Drawing.Image.FromFile(image)));
            }

            foreach (double value in values)
            {
                builder.Append(value.ToString() + ",");
            }
            builder.Remove(builder.Length - 1, 1);
            int length = image.Split('\\').Length;
            String className = image.Split('\\')[length - 2];
            builder.Append(" " + className + "\n");

        }
        return builder;

    }






}