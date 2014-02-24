using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using xDev.Utils.Net;

namespace UncAccessConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (UncAccess unc = new UncAccess(@"\\localhost\UncTemp", null, "user", "user123"))
            {
                if(unc.LastError != 0)
                {
                    // The connection has failed
                    Console.WriteLine("Failed to connect to UNC path, LastError = " + unc.LastError);
                    return;
                }

                //Create the file. 
                var file = new FileInfo(@"\\localhost\UncTemp\test.txt");
                using (FileStream fs = file.Create())
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");

                    //Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
        }
    }
}
