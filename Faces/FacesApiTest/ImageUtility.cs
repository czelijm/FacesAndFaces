using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacesApiTest
{
    class ImageUtility
    {

        public static async Task<byte[]> ConvertImageToByteArrayAsync(string imagePath) 
        {
            MemoryStream memoryStream = new MemoryStream();
            //shorter using
            using FileStream fileStream = new FileStream(imagePath, FileMode.Open);
            await fileStream.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        public static void ConvertByteArrayToImage(byte[] imageAsBytes, string fileName) 
        {
            using MemoryStream ms = new MemoryStream(imageAsBytes);
            Image image = Image.FromStream(ms);
            image.Save(fileName + ".jpg", ImageFormat.Jpeg);
        }

    }
}
