using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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
                        try
                        {
                            HttpClient n_cli = new HttpClient();

                            var bookResponse = n_cli.GetAsync(bookurl).Result;
                            string bookName = bookResponse.RequestMessage.RequestUri.Segments[bookResponse.RequestMessage.RequestUri.Segments.Length - 1];

                            DownloadFile(bookurl, string.Concat(localFolder, HttpUtility.UrlDecode(bookName)));
                            Console.WriteLine("{0} downloaded", bookName);
                        }
                        catch (TaskCanceledException ex)
                        {

                        }
                        catch (Exception e)
                        {

                        }
                    });
               
                Console.Write("Book Download Complete");
            }
        }

        private static string ShortenName(string file)
        {
            if (file.Length > 250)
            {
                string extension = file.Substring(file.LastIndexOf('.'), 3);
                file = string.Concat(file.Substring(0, 50), ".", extension);
            }

            return file;
          
            
        }

        public static void DownloadFile(string url, string filePath)
        {
            WebClient cli = new WebClient();

            cli.DownloadFile(url, ShortenName(filePath));
        }

     
    }
}
