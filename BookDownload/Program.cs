using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookDownload
{
    class Program
    {
        static void Main(string[] args)
        {

            HttpClient client = new HttpClient();

            using (client = new HttpClient())
            {
                Console.WriteLine("Getting Book Catalog");
                HttpResponseMessage response = client.GetAsync("http://www.mssmallbiz.com/ericligman/Key_Shorts/MSFTFreeEbooks.txt").Result;

                response.EnsureSuccessStatusCode();

                string result = response.Content.ReadAsStringAsync().Result;

                string[] books = result.Replace('\r',' ').Split('\n').Skip(1).ToArray();
                string localFolder = "c:\\temp\\";

                Console.WriteLine("Starting Book Download");
                CancellationToken token = new CancellationToken(false);
                Parallel.ForEach(books, new ParallelOptions { CancellationToken = token }, (bookurl) =>
                {
                    
                    var bookResponse = client.GetAsync(bookurl).Result;
                    string bookName = bookResponse.RequestMessage.RequestUri.Segments[bookResponse.RequestMessage.RequestUri.Segments.Length - 1];
                    DownloadFile(bookurl, string.Concat(localFolder,bookName));
                    Console.WriteLine("{0} downloaded",bookName);
                });

                Console.Write("Book Download Complete");
            }
        }


        public static void DownloadFile(string url, string filePath)
        {
            WebClient cli = new WebClient();
            cli.DownloadFile(new Uri(url), filePath);
            
        }

     
    }
}
