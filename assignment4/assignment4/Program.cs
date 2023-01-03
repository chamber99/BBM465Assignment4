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
using CEDD_Descriptor;
using FCTH_Descriptor;
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
using System.Data;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.DecisionTrees;
using Accord.IO;
using Accord.Imaging.Filters;

public class Assignment4
{
    public static void Main(String[] args)
    {
        run(args);
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
        // Creating list structures to store image files.
        List<String> images_training = fetchImageFiles(path, "train");
        List<String> images_validation = fetchImageFiles(path, "val");

        if (modeName.ToLower().Equals("precompute"))
        {            
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

            // Generating precomputed csv files to store features and classes.
            generatePrecompute("CEDD", images_training, images_validation);
            generatePrecompute("FCTH", images_training, images_validation);
            generatePrecompute("SURF", images_training, images_validation);
        }
        else if (modeName.ToLower().Equals("trainval"))
        {
            // Checking all necessary files and regenerating the missing ones before processing.
            if (!File.Exists("..\\..\\..\\pre-computed\\precomputed_CEDD_train.csv"))
            {
                generateMissingFile("CEDD", "train", images_training, "precomputed_CEDD_train.csv");
            }
            if (!File.Exists("..\\..\\..\\pre-computed\\precomputed_CEDD_val.csv"))
            {
                generateMissingFile("CEDD", "val", images_validation, "precomputed_CEDD_val.csv");
            }
            if (!File.Exists("..\\..\\..\\pre-computed\\precomputed_FCTH_train.csv"))
            {
                generateMissingFile("FCTH", "train", images_training, "precomputed_FCTH_train.csv");
            }
            if (!File.Exists("..\\..\\..\\pre-computed\\precomputed_FCTH_val.csv"))
            {
                generateMissingFile("FCTH", "val", images_validation, "precomputed_FCTH_val.csv");
            }
            if (!File.Exists("..\\..\\..\\pre-computed\\precomputed_SURF_train.csv"))
            {
                generateMissingFile("SURF", "train", images_training, "precomputed_SURF_train.csv");
            }
            if (!File.Exists("..\\..\\..\\pre-computed\\precomputed_SURF_val.csv"))
            {
                generateMissingFile("SURF", "val", images_validation, "precomputed_SURF_val.csv");
            }
            // Training and testing the models.
            trainAndValidate("..\\..\\..\\pre-computed\\precomputed_CEDD_train.csv", "..\\..\\..\\pre-computed\\precomputed_CEDD_val.csv");
            trainAndValidate("..\\..\\..\\pre-computed\\precomputed_FCTH_train.csv", "..\\..\\..\\pre-computed\\precomputed_FCTH_val.csv");
            trainAndValidate("..\\..\\..\\pre-computed\\precomputed_SURF_train.csv", "..\\..\\..\\pre-computed\\precomputed_SURF_val.csv");

        }
    }
    public static List<String> fetchImageFiles(String path,String mode)
    {
        // This method fetches all paths of image files from both train and val directories.

        List<String> images = new List<String>();
        if (mode.Equals("train"))
        {
            string[] train_subdirectories = Directory.GetDirectories(path + "\\train");
            foreach (String s in train_subdirectories)
            {
                string[] folders = s.Split('\\');
                string className = folders[folders.Length - 1];
                foreach (String file in Directory.GetFiles(path + "\\train\\" + className))
                {
                    images.Add(file);
                }
            }
        }
        else if (mode.Equals("val"))
        {
            string[] val_subdirectories = Directory.GetDirectories(path + "\\val");
            foreach (String s in val_subdirectories)
            {
                string[] folders = s.Split('\\');
                string className = folders[folders.Length - 1];
                foreach (String file in Directory.GetFiles(path + "\\val\\" + className))
                {
                    images.Add(file);
                }
            }

        }
        return images;
    }



    public static void generateMissingFile(String descriptor,string mode,List<String> images,String filename)
    {
        // Generating missing files for model training and testing.
        Stopwatch stopwatch = new Stopwatch();
        StringBuilder builder = new StringBuilder();
        Console.WriteLine("{0} does not exist!", filename);
        Console.WriteLine("{0} features are being extracted for {1}...", descriptor, mode);
        if (!descriptor.Equals("SURF"))
        {
            stopwatch.Start();
            builder = extractGlobalFeatures(descriptor,images);
            createCSV("..\\..\\..\\pre-computed\\" + filename, builder.ToString());
            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            Console.WriteLine("Done.{0} is regenerated in {1} seconds.",filename, (int)timeSpan.TotalSeconds);
        }
        else
        {
            extractLocalFeatures(images, mode);   

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
            createCSV("..\\..\\..\\pre-computed\\" + trainingFile, builder_train.ToString());
            stopwatch.Stop();
            TimeSpan timeSpanTrain = stopwatch.Elapsed;
            Console.WriteLine("Done.{0} is regenerated in {1} seconds.",trainingFile,(int)timeSpanTrain.TotalSeconds);
            
            Console.WriteLine($"{algorithm} features are being extracted for val...");
            stopwatch.Restart();
            StringBuilder builder_val = extractGlobalFeatures(algorithm, images_val);
            createCSV("..\\..\\..\\pre-computed\\" + validatingFile, builder_val.ToString());
            stopwatch.Stop();
            TimeSpan timeSpanVal = stopwatch.Elapsed;
            Console.WriteLine("Done.{0} is regenerated in {1} seconds.", validatingFile, (int)timeSpanVal.TotalSeconds);


        }
        else if (algorithm.Equals("SURF"))
        {
            // Extracting SURF features.
            extractLocalFeatures(images_train,images_val);
        }
    }

    public static void extractLocalFeatures(List<String> images,String mode)
    {
        Stopwatch stopwatch = new Stopwatch();
        switch (mode)
        {
            case "train":
                // Regenerating the csv file for training images.
                Bitmap[] allTrainingImages = generateBitmaps(images);
                stopwatch.Start();
                var bag_of_visual_words = BagOfVisualWords.Create(numberOfWords: 400);
                bag_of_visual_words.Learn(allTrainingImages);
                StringBuilder trainingData = getFeatureVector(bag_of_visual_words, allTrainingImages,images);
                createCSV("..\\..\\..\\pre-computed\\" + "precomputed_SURF_train.csv", trainingData.ToString());
                stopwatch.Stop();
                TimeSpan timeSpanTrain = stopwatch.Elapsed;
                // Serializing and storing the bag of visual words for future use.
                Accord.IO.Serializer.Save(bag_of_visual_words,"..\\..\\..\\bag_of_visual_words.txt");
                Console.WriteLine("Done. precomputed_SURF_train.csv is regenerated in {0} seconds.", (int)timeSpanTrain.TotalSeconds);
                break;
            case "val":
                Bitmap[] allValidatingImages = generateBitmaps(images);
                stopwatch.Start();
                // Reconstructing the bag of visual words object to access dictionary
                var bagOfVisualWords = BagOfVisualWords.Load("..\\..\\..\\bag_of_visual_words.txt");
                StringBuilder validatingData = getFeatureVector(bagOfVisualWords, allValidatingImages,images);
                createCSV("..\\..\\..\\pre-computed\\" + "precomputed_SURF_val.csv", validatingData.ToString());
                stopwatch.Stop();
                TimeSpan timeSpanVal = stopwatch.Elapsed;
                Console.WriteLine("Done. precomputed_SURF_val.csv is regenerated in {0} seconds.", (int)timeSpanVal.TotalSeconds);
                break;
        }
    }
    public static void extractLocalFeatures(List<String> trainingImages,List<String> validatingImages) 
    {
        // This method extracts the local image features with the help of SURF feature descriptor.
        Stopwatch stopwatch = new Stopwatch();
        Bitmap[] allTrainingImages = generateBitmaps(trainingImages);

        // Extracting SURF features
        Console.WriteLine("SURF features are being extracted for train...");
        stopwatch.Start();
        var bag_of_visual_words = BagOfVisualWords.Create(numberOfWords: 400);
        // Clustering
        bag_of_visual_words.Learn(allTrainingImages);
        StringBuilder trainingData = getFeatureVector(bag_of_visual_words, allTrainingImages, trainingImages);
        createCSV("..\\..\\..\\pre-computed\\" + "precomputed_SURF_train.csv",trainingData.ToString());
        stopwatch.Stop();
        // Serializing and storing the bag of visual words for test samples.
        Accord.IO.Serializer.Save(bag_of_visual_words, "..\\..\\..\\bag_of_visual_words.txt");
        TimeSpan timeSpanTrain = stopwatch.Elapsed;
        Console.WriteLine("Done. precomputed_SURF_train.csv is regenerated in {0} seconds.", (int)timeSpanTrain.TotalSeconds);
        
        Bitmap[] allValidatingImages = generateBitmaps(validatingImages);
        Console.WriteLine("SURF features are being extracted for val...");
        stopwatch.Restart();
        // Converting validation data to a bag of visual words by using the dictionary that was generated by training part.
        StringBuilder validatingData = getFeatureVector(bag_of_visual_words, allValidatingImages, validatingImages);
        createCSV("..\\..\\..\\pre-computed\\" + "precomputed_SURF_val.csv", validatingData.ToString());
        stopwatch.Stop();
        TimeSpan timeSpanVal = stopwatch.Elapsed;
        Console.WriteLine("Done. precomputed_SURF_val.csv is regenerated in {0} seconds.", (int)timeSpanVal.TotalSeconds);

    }

    public static Bitmap[] generateBitmaps(List<String> images)
    {
        // Converting images to bitmaps and returning all bitmaps.
        Bitmap[] bitmaps = new Bitmap[images.Count];
        int index = 0;
        foreach (String fileName in images)
        {
            bitmaps[index++] = new Bitmap(System.Drawing.Image.FromFile(fileName));
        }
        return bitmaps;
    }

    public static StringBuilder getFeatureVector(BagOfVisualWords visualWords, Bitmap[] images,List<String> imageFiles)
    {
        // Creating header part for csv file.
        int index = 0;
        StringBuilder builder = new StringBuilder();
        String header = "";
        for(int i = 0; i < 400; i++)
        {
            header += $"f{i},";
        }
        header += "label\n";
        builder.Append(header);
        // Generating feature vectors using the vocabulary.
        foreach (Bitmap bitmap in images)
        {

            double[] values = visualWords.GetFeatureVector(bitmap);

            foreach (double value in values)
            {
                // Adding values to the builder.
                builder.Append(value.ToString() + ",");
            }
            String fileName = imageFiles[index];
            int len = fileName.Split('\\').Length;
            builder.Append($"{fileName.Split('\\')[len - 2]}\n");
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
        // Making readOnly attribute false for future writes. 
        fileinfo.IsReadOnly = false;
        csvCreator.Close();
    }

    public static void trainAndValidate(String pathTrain,String pathVal)
    {        
        // Creating a one-vs-one multi-class SVM learning algorithm 
        var svm = new MulticlassSupportVectorLearning<Linear>()
        {
            // using LIBLINEAR's L2-loss SVC dual for each SVM
            Learner = (p) => new LinearDualCoordinateDescent()
            {
                Loss = Loss.L2
            }
        };

        // Creating forest learning algorithm
        RandomForestLearning forestLearning = new RandomForestLearning();

        String trainingFile = pathTrain.Split('\\')[pathTrain.Split('\\').Length - 1];
        String validatingFile = pathVal.Split('\\')[pathVal.Split('\\').Length - 1];
        // Reading training files
        string[] lines = System.IO.File.ReadAllLines(pathTrain);
        double[][] featuresTrain = new double[lines.Length - 1][];
        int[] classesTrain = new int[lines.Length - 1];
        //Generating features and classes 
        fetchFeaturesAndClasses(lines,classesTrain,featuresTrain);
        List<int> indexes = checkConstants(featuresTrain);
        // Dropping all columns with constant values
        featuresTrain = removeColumn(featuresTrain, indexes);

        Stopwatch stopwatch = new Stopwatch();
        Console.WriteLine("Training with {0}", trainingFile);

        // Start training
        stopwatch.Start();
        var supportVectorMachine = svm.Learn(featuresTrain, classesTrain);
        var randomForest = forestLearning.Learn(featuresTrain, classesTrain);
        stopwatch.Stop();
        TimeSpan timeSpanTrain = stopwatch.Elapsed;
        Console.WriteLine("Done in {0} seconds.", (int)timeSpanTrain.TotalSeconds);

        // Reading validation files.
        lines = System.IO.File.ReadAllLines(pathVal);
        double[][] featuresVal = new double[lines.Length - 1][];
        int[] classesVal = new int[lines.Length - 1];
        // Generating featues and classes.
        fetchFeaturesAndClasses(lines,classesVal,featuresVal);
        // Dropping all columns with constant values
        featuresVal = removeColumn(featuresVal, indexes);

        // Printing results.
        Console.WriteLine("Testing with {0} {1} samples", validatingFile, featuresVal.Rows<double>());
        int[] svm_predicted = supportVectorMachine.Decide(featuresVal);
        int[] rf_predicted = randomForest.Decide(featuresVal);
        Console.WriteLine("Random Forest {0}", generateResults(classesVal, rf_predicted));
        Console.WriteLine("SVM           {0}", generateResults(classesVal, svm_predicted));
        Console.WriteLine("--------------------------------------------------------------");


    }
    public static void fetchFeaturesAndClasses(String[] lines, int[] classes, double[][] features)
    {
        // Fetching all values from files and creating features and classes.
        assignment4.Constants constants = new assignment4.Constants();
        int index = 0;
        bool headerControl = false;
        foreach (string line in lines)
        {
            if (!headerControl)
            {
                headerControl = true;
                continue;
            }
            // Splitting line by comma
            string[] splitLine = line.Split(',');
            double[] doubles = convertLinetoDoubles(splitLine);
            features[index] = doubles;
            classes[index] = constants.getClasses()[splitLine[splitLine.Length - 1]];
            index++;
        }
    }
    public static String generateResults(int[] classes, int[] predicted)
    {
        // Creating a general confusion matrix for multiclass evaluation.
        var confusionMatrix = new GeneralConfusionMatrix(classes: 15, expected: classes, predicted: predicted);
        // Generating a confusion matrix for each class in order to evaluate performance.
        ConfusionMatrix[] allMatrices = confusionMatrix.PerClassMatrices;
        double totalTruePositives = 0;
        double totalFalsePositives = 0;
        double totalFalseNegatives = 0;
        double totalTrueNegatives = 0;

        foreach (ConfusionMatrix matrix in allMatrices)
        {

            totalTruePositives += matrix.ActualPositives;
            totalTrueNegatives += matrix.ActualNegatives;
            totalFalsePositives += matrix.FalsePositives;
            totalFalseNegatives += matrix.FalseNegatives;
        }
        // Calculating final scores.
        double tpr = totalTruePositives / (totalTruePositives + totalFalseNegatives);
        double fpr = totalFalsePositives / (totalFalsePositives + totalTrueNegatives);
        double f1 = (2 * totalTruePositives) / (2 * totalTruePositives + totalFalsePositives + totalFalseNegatives);

        String results = $"| TPR {Math.Round(tpr,3)} | FPR {Math.Round(fpr,3)} | F1 {Math.Round(f1,3)} |";
        return results;
    }



    public static double[] convertLinetoDoubles(String[] line)
    {
        // Converting a line to a double feature array.
        double[] features = new double[line.Length - 1];
        int index = 0;
        for(int i = 0; i < line.Length-1; i++)
        {
            features[index++] = Convert.ToDouble(line[i]);
        }
        return features;
    }


    public static List<int> checkConstants(double[][] target) {

        //Finding columns which have constant values.

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
        // Removing columns which are at specified indexes from the target matrix.
        double[,] doubles = target.ToMatrix();
        doubles = Matrix.Remove<double>(doubles, null, indexes.ToArray());

        target = doubles.ToJagged();
        return target;


    }

    public static StringBuilder extractGlobalFeatures(String algorithm,List<String> images)
    {
        // In this method,global features are extracted and the content for the related csv files is created.
        StringBuilder builder = new StringBuilder();
        String header = "";
        if (algorithm.Equals("CEDD"))
        {   
            //For CEDD descriptor,there are 144 features.Header is created here for csv files.
            for(int i = 0; i < 144; i++)
            {
                header += $"f{i},";   
            }
            header += "label\n";
            // Appending the header to the builder.
            builder.Append(header);

            foreach(string image in images)
            {
                // Extracting features for each image in the list.
                double[] values = useCEDD(new Bitmap(System.Drawing.Image.FromFile(image)));
                foreach (double value in values)
                {
                    //Combining extracted features with comma.
                    builder.Append(value.ToString() + ",");
                }
                int length = image.Split('\\').Length;
                String className = image.Split('\\')[length - 2];
                // Appending label at the end of the line.
                builder.Append(className + "\n");

            }
        }
        else if (algorithm.Equals("FCTH"))
        {
            //For FCTH descriptor,there are 144 features.Header is created here for csv files.
            for (int i = 0; i < 192; i++)
            {
                header += $"f{i},";
            }
            header += "label\n";
            builder.Append(header);

            foreach (string image in images)
            {
                // Extracting features for each image in the list.
                double[] values = useFCTH(new Bitmap(System.Drawing.Image.FromFile(image)));
                foreach (double value in values)
                {
                    // Combining features with comma.
                    builder.Append(value.ToString() + ",");
                }
                int length = image.Split('\\').Length;
                String className = image.Split('\\')[length - 2];
                // Appending label name at the end of the list.
                builder.Append(className + "\n");

            }
        }
        return builder;

    }
    public static double[] useCEDD(Bitmap imageData)
    {   
        // Extracting CEDD features.
        CEDD_Descriptor.CEDD getCEDD = new CEDD_Descriptor.CEDD();
        double[] table = getCEDD.Apply(imageData);
        return table;
    }
    public static double[] useFCTH(Bitmap imageData)
    {
        // Extracting FCTH features.
        FCTH_Descriptor.FCTH getFCTH = new FCTH_Descriptor.FCTH();
        double[] table = getFCTH.Apply(imageData, 1);
        return table;
    }




}