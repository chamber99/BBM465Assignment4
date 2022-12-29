using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using FCTH_Descriptor;
using CEDD_Descriptor;
using Emgu.CV.XFeatures2D;
using System.IO;
using Accord.Imaging;

namespace assignment4


{
    internal class ImageOperations
    {   
        public double[] CEDDTable = new double[144];
        public double[] FCTHTable = new double[192];

        List<GlobalImage> GlobalImages = new List<GlobalImage>();

        public ImageOperations() {
            
        }

        public void prepareImages(String training) {

            // searches the current directory and sub directory
            //int fCount = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
            // searches the current directory
            //int fCount = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length;


        }

        


        public void useHOG(Bitmap imageData) {
            HOGDescriptor getHOG = new HOGDescriptor();
            //getHOG.Compute();
        
        }


        public double[] useCEDD(Bitmap imageData) {
            CEDD_Descriptor.CEDD getCEDD = new CEDD_Descriptor.CEDD();
            double[] table = getCEDD.Apply(imageData);
            return table;
        }

        public double[] useFCTH(Bitmap imageData) {
            FCTH_Descriptor.FCTH getFCTH = new FCTH_Descriptor.FCTH();
            double[] table = getFCTH.Apply(imageData,2);
            return table;
        }

        public void useSIFT(Bitmap imageData) { 
            SIFT getSIFT = new SIFT();
            
            //getSIFT.DetectAndCompute();   
        }

        public void EmguCEDD() { 
            
        }


        public void useSURF(Bitmap imageData)
        {
            //SURF getSURF = new SURF();

        }




    }
}
