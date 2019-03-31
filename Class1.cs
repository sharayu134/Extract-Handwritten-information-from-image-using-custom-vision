using ClassLibrary13;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSHttpClientSample
{
    static class Program
    {
        //Handwritten OCR

        //date pay rupee
        // Replace <Subscription Key> with your valid subscription key.
        const string subscriptionKey = "a02378056afc41c886ff93c7f5cbb17a";

        
        const string uriBase =
            "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/recognizeText";

        static void Main()
        {
        
            //input
            string imageFilePath = @"D:\as.jpg";

          
            //output
           ReadHandwrittenText(imageFilePath).Wait();

            Thread.Sleep(100000);




        }


        /// <param name="imageFilePath">The image file with handwritten text.</param>
        static async Task ReadHandwrittenText(string imageFilePath)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameter.
                string requestParameters = "mode=Handwritten";

                // Assemble the URI for the REST API method.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                
                string operationLocation;

               
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                // Adds the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // The first REST API method, Recognize Text, starts
                    // the async process to analyze the written text in the image.
                    response = await client.PostAsync(uri, content);
                }

                // The response header for the Recognize Text method contains the URI
                // of the second method, Get Recognize Text Operation Result, which
                // returns the results of the process in the response body.
                // The Recognize Text operation does not return anything in the response body.
                if (response.IsSuccessStatusCode)
                    operationLocation =
                        response.Headers.GetValues("Operation-Location").FirstOrDefault();
                else
                {
                    // Display the JSON error data.
                    string errorString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("\n\nResponse:\n{0}\n",
                        JToken.Parse(errorString).ToString());
                    return;
                }

                
                string contentString;
                int i = 0;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    response = await client.GetAsync(operationLocation);
                    contentString = await response.Content.ReadAsStringAsync();
                    ++i;


                }
                while (i < 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

                


                var oMycustomclassname = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(contentString);


                var lines = oMycustomclassname.recognitionResult.lines;

               

                StringBuilder date = new StringBuilder();

                StringBuilder pay = new StringBuilder();

                StringBuilder rupee = new StringBuilder(); //Amount


                foreach (var line in lines)
                {
                    int k = 0;
                    int counter = 0;
                    foreach (var word in line.words)
                    {

                        var txt = word.text;

                        if (txt == "PAY_" || txt =="pay" || txt.Contains("PAY") || txt.Contains("pay") ) {

                            foreach (var word3 in line.words) {

                                pay.Append(word3.text);
                            }

                        }

                        if (txt == "RUPEES" || txt == "rupees") {

                            foreach (var word4 in line.words)
                            {

                                rupee.Append(word4.text);
                            }

                        }





                        counter++;
                        int len = 0;
                        if (txt == "DATE") {
                            k = counter;
                            
                            foreach (var word2 in line.words)
                            {
                                len++;
                            }
                          
                        }

                        for (int g = counter; g < len ; g++) {
                            date.Append(line.words[g].text);
                        }
                    
                    }
                }


               Console.WriteLine(date);
                Console.WriteLine(pay);
           
                Console.WriteLine(rupee);
            }

            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        /// <summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}