using Accord.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment4
{
    internal class Constants
    {
        public Dictionary<string, int> classes;

        public Constants()
        {
            // All brands are assigned to a constant integer in order to be able to be used in models.
            classes= new Dictionary<string, int>();
            classes.Add("adobe", 0);
            classes.Add("alibaba", 1);
            classes.Add("amazon", 2);
            classes.Add("apple", 3);
            classes.Add("boa", 4);
            classes.Add("chase", 5);
            classes.Add("dhl", 6);
            classes.Add("dropbox", 7);
            classes.Add("facebook", 8);
            classes.Add("linkedin", 9);
            classes.Add("microsoft", 10);
            classes.Add("other", 11);
            classes.Add("paypal", 12);
            classes.Add("wellsfargo", 13);
            classes.Add("yahoo", 14);
        }

        // Getter for outside usage.
        public Dictionary<string,int> getClasses()
        {
            return this.classes;
        }

    }
}
