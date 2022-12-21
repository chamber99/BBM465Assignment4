using assignment4;
using System;
using System.IO;

public class Assignment4{
    public static void Main(String[] args)
    {
        String training = args[0];
        String validation = args[1];

        Console.WriteLine(training + " " + validation);
        


        Console.WriteLine("hehe");
        Console.WriteLine("Why does this thing have autocomplete in it?");
        Console.WriteLine("GITIGNORE PLS");
        Console.WriteLine("testing");
        

        ImageOperations image = new ImageOperations();


        // searches the current directory and sub directory
        int fCount = Directory.GetFiles("C:\\Users\\berka\\Desktop\\Assignment 4\\phishIRIS_DL_Dataset", "*", SearchOption.AllDirectories).Length;
        Console.WriteLine("Count :" + fCount);


    }
}