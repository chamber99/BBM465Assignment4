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

        foreach(String image in images_training)
        {
            StringBuilder builder = new StringBuilder();
            double[] values = img.useCEDD(new Bitmap(image));

            foreach (double value in values)
            {
                builder.Append(value.ToString() + ",");
            }

            builder.Remove(builder.Length - 1, 1);
            int len = image.Split('\\').Length;

            builder.Append($" {image.Split('\\')[len-2]}\n");

            String csv = builder.ToString();

            StreamWriter precomputed = new StreamWriter(path + "\\precomputed_CEDD_train.csv");

            precomputed.Write(path + "\\precomputed_CEDD_train.csv", csv);

            precomputed.Close();




        }










    }
    }