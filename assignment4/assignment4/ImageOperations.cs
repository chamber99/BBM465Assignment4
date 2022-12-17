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

namespace assignment4


{
    internal class ImageOperations
    {   
        double[] CEDDTable = new double[144];
        double[] FCTHTable = new double[192];




        public ImageOperations() { 
              


        }

        public void useHOG(Bitmap imageData) {
            HOGDescriptor getHOG = new HOGDescriptor();

        
        }


        public void useCEDD(Bitmap imageData) {
            CEDD_Descriptor.CEDD getCEDD = new CEDD_Descriptor.CEDD();
            CEDDTable = getCEDD.Apply(imageData);


        }

        public void useFCTH(Bitmap imageData) {
            FCTH_Descriptor.FCTH getFCTH = new FCTH_Descriptor.FCTH();
            FCTHTable = getFCTH.Apply(imageData,2);

        }

        public void useSIFT(Bitmap imageData) { 
            SIFT getSIFT = new SIFT();
            //getSIFT.DetectAndCompute();   
        }

        public void useSURF(Bitmap imageData)
        {
            //SURF getSURF = new SURF();

        }




    }
}
