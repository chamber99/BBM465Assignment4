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
            generatePrecompute("SURF", images_training, images_validation);



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
        csvCreator.Close();
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