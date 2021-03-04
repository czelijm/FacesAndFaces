using FacesApi.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FacesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacesController : ControllerBase
    {

        private readonly AzureFaceConfiguration _azureFaceConfiguration;

        public FacesController(AzureFaceConfiguration azureFaceConfiguration)
        {
            _azureFaceConfiguration = azureFaceConfiguration;
        }

        [HttpPost]
        public async Task<Tuple<List<byte[]>,Guid>> ReadFaces(Guid orderId) 
        {

            //return await ReadFacesUsingOpenCv(orderId);
            return await ReadFacesUsingAzureFaceApi(orderId);
  
        }


        private async Task<Tuple<List<byte[]>, Guid>> ReadFacesUsingAzureFaceApi(Guid orderId)
        {
            List<byte[]> facesCropped = null;
            //memory stream for storing image data,
            //Read data in 2048 size chunks
            using (var ms = new MemoryStream(2048))
            {
                await Request.Body.CopyToAsync(ms);
                if (ms.Length < 1) return new Tuple<List<byte[]>, Guid>(null, orderId);
                ////Image from ImageSharp
                //Image image = Image.Load(ms.ToArray());
                //image.Save("dumy.jpg");
                facesCropped = await UploadAndDetectFaces(new MemoryStream(ms.ToArray()));//It requries new memorystream object, in other case badrequest
                //return faces;
                return new Tuple<List<byte[]>, Guid>(facesCropped, orderId);
            }

        }
        //remenber to check Include prerelease checkbox
        public static IFaceClient Authenticate(string endpoint, string key) 
        {

            //if (Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
            //{
            //    return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
            //}
            //else 
            //{
            //    return null;
            //}
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };

        }

        private async Task<List<byte[]>> UploadAndDetectFaces(MemoryStream ms)
        {
            string subKey = _azureFaceConfiguration.AzureSubscriptionKey;
            string endPoint = _azureFaceConfiguration.AzureEndPoint;

            var client = Authenticate(endPoint, subKey);
            var faceList = new List<byte[]>();
            IList<DetectedFace> faces = null;
            Image image = Image.Load(ms.ToArray());
            
            try
            {
                
                faces = await client.Face.DetectWithStreamAsync(ms,true,false,null);
                int j = 0;
                foreach (var face in faces)
                {
                    var memoryStreamForImage = new MemoryStream();

                    //var zoom = 1.0;
                    //int h = (int)(face.FaceRectangle.Height / zoom);

                    ////Save image as file
                    //image.Clone(ctx =>
                    //    ctx.Crop(
                    //        new Rectangle(
                    //        face.FaceRectangle.Left,
                    //        face.FaceRectangle.Top,
                    //        face.FaceRectangle.Width,
                    //        face.FaceRectangle.Height
                    //    ))).Save("face" + j + ".jpg");

                    //Save image as memorystream
                    await image.Clone(ctx =>
                        ctx.Crop(
                            new Rectangle(
                            face.FaceRectangle.Left,
                            face.FaceRectangle.Top,
                            face.FaceRectangle.Width,
                            face.FaceRectangle.Height
                        ))).SaveAsJpegAsync(memoryStreamForImage);

                    faceList.Add(memoryStreamForImage.ToArray());
                    j++;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }

            return faceList;
        }

        private async Task<Tuple<List<byte[]>, Guid>> ReadFacesUsingOpenCv(Guid orderId)
        {
            //memory stream for storing image data,
            //Read data in 2048 size chunks
            using (var ms = new MemoryStream(2048))
            {
                await Request.Body.CopyToAsync(ms);
                if (ms.Length < 1) return new Tuple<List<byte[]>, Guid>(null, orderId);
                var faces = GetFaces(ms.ToArray());
                //return faces;
                return new Tuple<List<byte[]>, Guid>(faces, orderId);
            }

        }

        private List<byte[]> GetFaces(byte[] image)
        {
            Mat src = Cv2.ImDecode(image,ImreadModes.Color);
            //for veryfication purpose
            //save image on the host
            src.SaveImage("image.jpg",new ImageEncodingParam(ImwriteFlags.JpegProgressive,255));

            var file = Path.Combine(Directory.GetCurrentDirectory(),"CascadeFile", "haarcascade_frontalface_default.xml");
            var faceCascade = new CascadeClassifier();
            faceCascade.Load(file);
            var faces = faceCascade.DetectMultiScale(src, 1.1, 6, HaarDetectionTypes.DoRoughSearch, new OpenCvSharp.Size(60,60));

            var faceList = new List<byte[]>();
            var index = 0;

            foreach (var rect in faces)
            {
                //crop face image
                var faceImage = new Mat(src, rect);
                faceList.Add(faceImage.ToBytes(".jpg"));

                faceImage.SaveImage($"face{index++}.jpg", new ImageEncodingParam(ImwriteFlags.JpegProgressive, 255));               
            }

            return faceList;
        }
    }
}
