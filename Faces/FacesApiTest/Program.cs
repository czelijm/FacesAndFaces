using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FacesApiTest
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var imagePath = @"oscars-2017.jpg";
            String urlAddres = @"http://localhost:6001/api/Faces?orderId=3fa85f64-5717-4562-b3fc-2c963f66afa6";
            var bytes = await ImageUtility.ConvertImageToByteArrayAsync(imagePath);
            List<byte[]> faceList = null;//new List<byte[]>();
            //we will wrap our data, in this class
            var byteContent = new ByteArrayContent(bytes);
            //set the content of the bytearrays to media type
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            //we will use the HttpClient for sending the data to our API
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsync(urlAddres, byteContent)) 
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                   // faceList = JsonConvert.DeserializeObject<List<byte[]>>(apiResponse);
                    var tupleApiResponse = JsonConvert.DeserializeObject<
                            Tuple<List<byte[]>,Guid>
                        >(apiResponse);
                    faceList = tupleApiResponse.Item1;
                }
            }
            if (faceList.Count>0)
            {
                for (int i = 0; i < faceList.Count; i++)
                {
                    ImageUtility.ConvertByteArrayToImage(faceList[i],$"face{i}");
                }
            }

        }

    }
}
