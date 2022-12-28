using assignment4;
using System;
using System.IO;
using System.Drawing;

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

        //path = path.Replace('/', '\\');

        Console.WriteLine(path);
        Console.WriteLine(mode);

        Console.WriteLine(Directory.Exists(path) ? "Path found ": "Path invalid.");
        


        
        MLOperations ml = new MLOperations();
        ImageOperations img = new ImageOperations(path + "\\train", path + "\\validate");
            
        // searches the current directory and sub directory


        int fCount = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
        //string[] allfiles = Directory.GetFiles("C:\\Users\\berka\\Desktop\\Assignment 4\\phishIRIS_DL_Dataset", "*.*", SearchOption.AllDirectories);

        string[] train_subdirectories = Directory.GetDirectories(path + "\\train");
        string[] val_subdirectories = Directory.GetDirectories(path + "\\val");

        foreach(String s in train_subdirectories) { 
            Console.WriteLine(s);
        }
        foreach (String s in val_subdirectories) { 
            Console.WriteLine(s);
        }



        Console.WriteLine("Count :" + fCount);


        }
    }