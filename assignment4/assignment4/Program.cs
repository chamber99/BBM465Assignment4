using assignment4;
using System;
using System.IO;
using System.Drawing;

public class Assignment4{
    public static void Main(String[] args)
    {
        String training = args[0];
        String validation = args[1];

        Console.WriteLine(training + " " + validation);
        MLOperations ml = new MLOperations();
        ImageOperations img = new ImageOperations("train","validate");

        


        Console.WriteLine("hehe");
        Console.WriteLine("Why does this thing have autocomplete in it?");
        Console.WriteLine("GITIGNORE PLS");
        Console.WriteLine("testing");
        

        


        // searches the current directory and sub directory
        int fCount = Directory.GetFiles("C:\\Users\\berka\\Desktop\\Assignment 4\\phishIRIS_DL_Dataset", "*", SearchOption.AllDirectories).Length;
        string[] allfiles = Directory.GetFiles("C:\\Users\\berka\\Desktop\\Assignment 4\\phishIRIS_DL_Dataset", "*.*", SearchOption.AllDirectories);

        for (int i = 0; i < allfiles.Length; i++){
            Console.WriteLine(allfiles[i]);
            img.useCEDD(new Bitmap(allfiles[i]));
            img.useFCTH(new Bitmap(allfiles[i]));
            break;
        }
        
        for(int i = 0; i<img.CEDDTable.Length; i++)
        {
            Console.Write(img.CEDDTable[i] + " ");
            
        }
        Console.WriteLine();

        for (int i = 0; i < img.FCTHTable.Length; i++)
        {
            Console.Write(img.FCTHTable[i] + " ");

        }
        Console.WriteLine();

        Console.WriteLine("Garavel");

        img.useCEDD(new Bitmap("C:\\Users\\berka\\Desktop\\garavel.jpeg"));
        for (int i = 0; i < img.CEDDTable.Length; i++)
        {
            Console.Write(img.CEDDTable[i] + " ");

        }
        Console.WriteLine();

        img.useFCTH(new Bitmap("C:\\Users\\berka\\Desktop\\garavel.jpeg"));
        for (int i = 0; i < img.FCTHTable.Length; i++)
        {
            Console.Write(img.FCTHTable[i] + " ");

        }





        Console.WriteLine("Count :" + fCount);


    }
}