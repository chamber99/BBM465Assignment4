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

public class Assignment4
{
    public static void Main(String[] args)
    {

        String path = "";
        String mode = "";

        if (args.Length > 0)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string argument = args[i].ToLower();
                switch (argument)
                {
                    case "-dataset":
                        path = args[i + 1];
                        break;
                    case "-mode":
                        mode = args[i + 1];
                        break;
                }
            }
        }
        if (mode.Equals("precompute"))
        {
            List<String> images_training = new List<String>();
            List<String> images_validation = new List<String>();


            /*Reading phishIRIS_DL_Dataset...
            1313 images were found in train folder
            1539 images were found in val folder
            14 classes exist
            FCTH features are being extracted for train...
            Done.precomputed_FCTH_train.csv is regenerated in XX seconds
            FCTH features are being extracted for val...
            Done.precomputed_FCTH_val.csv is regenerated in XX seconds
            CEDD features are being extracted for train..Done precomputed_CEDD_train.csv is regenerated in XX seconds
            CEDD features are being extracted for val...
            Done precomputed_CEDD_val.csv is regenerated in XX seconds
            SIFT features are being extracted for train...
            Done precomputed_SIFT_train.csv is regenerated in XX seconds
            SIFT features are being extracted for val...
            Done precomputed_SIFT_val.csv is regenerated in XX seconds*/





            //Console.WriteLine("Path: " + path + " Mode: " + mode);
            //Console.WriteLine(Directory.Exists(path) ? "Path is valid. " : "Path is invalid.");


            String dataset_name = path.Trim('\\', '.');



            Console.WriteLine("Reading " + dataset_name + "...");

            int train_count = Directory.GetFiles(path + "\\train", "*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg")).ToList().Count;
            Console.WriteLine(train_count + " images found in train folder");
            int val_count = Directory.GetFiles(path + "\\val", "*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg") || s.EndsWith(".jpeg")).ToList().Count;
            Console.WriteLine(val_count + " images found in val folder");

            String[] dirs = Directory.GetDirectories(path + "\\train", "*", SearchOption.AllDirectories);
            HashSet<String> classSet = new HashSet<string>();
            foreach (String dir in dirs)
            {
                classSet.Add(dir.Trim('\\','.').Trim('\\').Replace("phishIRIS_DL_Dataset", "").Replace("train","").Replace("val","").Trim('\\'));
            }
                       
            Console.WriteLine(String.Join(",", classSet));

            int class_count = classSet.Count;
            Console.WriteLine(class_count + " classes exist.");

            ImageOperations img = new ImageOperations(path + "\\train", path + "\\val");

            // searches the current directory and sub directory


            //int fCount = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
            //string[] allfiles = Directory.GetFiles("C:\\Users\\berka\\Desktop\\Assignment 4\\phishIRIS_DL_Dataset", "*.*", SearchOption.AllDirectories);

            string[] train_subdirectories = Directory.GetDirectories(path + "\\train");
            string[] val_subdirectories = Directory.GetDirectories(path + "\\val");



            foreach (String s in train_subdirectories)
            {
                string[] folders = s.Split('\\');
                string className = folders[folders.Length - 1];
                //Console.WriteLine(className);

                foreach (String file in Directory.GetFiles(path + "\\train\\" + className))
                {
                    //Console.WriteLine(file);
                    //GlobalImage image = new GlobalImage(new Bitmap(file),className);        
                    images_training.Add(file);
                }


            }

            foreach (String s in val_subdirectories)
            {
                string[] folders = s.Split('\\');
                string className = folders[folders.Length - 1];
                //Console.WriteLine(className);

                foreach (String file in Directory.GetFiles(path + "\\val\\" + className))
                {
                    //Console.WriteLine(file);
                    //GlobalImage image = new GlobalImage(new Bitmap(file),className);        
                    images_validation.Add(file);
                }
            }



            Console.WriteLine();

            

            StringBuilder builder_cedd = new StringBuilder();
            StringBuilder builder_fcth = new StringBuilder();

            foreach (String image in images_training)
            {

                double[] values_cedd = img.useCEDD(new Bitmap(image));
                double[] values_fcth = img.useFCTH(new Bitmap(image));

                foreach (double value in values_cedd)
                {
                    builder_cedd.Append(value.ToString() + ",");
                }

                foreach (double value in values_fcth)
                {
                    builder_fcth.Append(value.ToString() + ",");

                }

                builder_cedd.Remove(builder_cedd.Length - 1, 1);
                builder_fcth.Remove(builder_fcth.Length - 1, 1);

                int len = image.Split('\\').Length;

                String val = image.Split('\\')[len - 2];

                builder_cedd.Append($" {val}\n");
                builder_fcth.Append(" " + val + "\n");

            }

            String csv_cedd = builder_cedd.ToString();
            String csv_fcth = builder_fcth.ToString();

            StreamWriter precomputed = new StreamWriter(path + "\\precomputed_CEDD_train.csv");
            StreamWriter precomputed_fcth = new StreamWriter(path + "\\precomputed_FCTH_train.csv");

            precomputed.Write(csv_cedd);
            precomputed_fcth.Write(csv_fcth);

            precomputed.Close();
            precomputed_fcth.Close();
        }


    }
}