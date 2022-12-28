using assignment4;
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

public class Assignment4{
    public static void Main(String[] args){

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
        if (mode.Equals("precompute")) { 
        
        
        
        
        }

        List<String> images_training = new List<String>();
        List<GlobalImage> images_validation = new List<GlobalImage>();

        

        Console.WriteLine("Path: "+path + " Mode: " + mode);
        Console.WriteLine(Directory.Exists(path) ? "Path is valid. ": "Path is invalid.");
        
        MLOperations ml = new MLOperations();
        ImageOperations img = new ImageOperations(path + "\\train", path + "\\val");
            
        // searches the current directory and sub directory


        //int fCount = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
        //string[] allfiles = Directory.GetFiles("C:\\Users\\berka\\Desktop\\Assignment 4\\phishIRIS_DL_Dataset", "*.*", SearchOption.AllDirectories);

        string[] train_subdirectories = Directory.GetDirectories(path + "\\train");
        string[] val_subdirectories = Directory.GetDirectories(path + "\\val");



        foreach(String s in train_subdirectories) {
            string[] folders = s.Split('\\');
            string className = folders[folders.Length -1];
            //Console.WriteLine(className);

            foreach(String file in Directory.GetFiles(path + "\\train\\" + className)){
                //Console.WriteLine(file);
                //GlobalImage image = new GlobalImage(new Bitmap(file),className);        
                images_training.Add(file);
            }


        }

        Console.WriteLine();

        foreach (String s in val_subdirectories) {
            string[] folders = s.Split('\\');
            string className = folders[folders.Length - 1];
            //Console.WriteLine(className);

            foreach (String file in Directory.GetFiles(path + "\\val\\" + className))
            {
                //Console.WriteLine(file);
                GlobalImage image = new GlobalImage(new Bitmap(file), className);
                images_validation.Add(image);
            }




        }

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
            builder_fcth.Append(" "+ val + "\n");

        }

        String csv = builder_cedd.ToString();
        String csv_fcth = builder_fcth.ToString();

        StreamWriter precomputed = new StreamWriter(path + "\\precomputed_CEDD_train.csv");
        StreamWriter precomputed_fcth = new StreamWriter(path + "\\precomputed_FCTH_train.csv");

        precomputed.Write(csv);
        precomputed_fcth.Write(csv_fcth);

        precomputed.Close();
        precomputed_fcth.Close();










    }
    }