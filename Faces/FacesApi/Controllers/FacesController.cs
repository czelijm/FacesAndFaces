using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
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
        [HttpPost]
        public async Task<List<byte[]>> ReadFaces() 
        {
            //memory stream for storing image data,
            //Read data in 2048 size chunks
            using (var ms = new MemoryStream(2048))
            {
                await Request.Body.CopyToAsync(ms);
                var faces = GetFaces(ms.ToArray());
                return faces;
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
            var faces = faceCascade.DetectMultiScale(src, 1.1, 6, HaarDetectionTypes.DoRoughSearch, new Size(60,60));

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
