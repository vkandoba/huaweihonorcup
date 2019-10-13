using System.Drawing;
using Borderer;

namespace BordererTests
{
    public class TrainImage
    {
        public string Name { get; set; }
        public Bitmap Image { get; set; }
        public Bitmap Original { get; set; }
        public ImageParameters Param { get; set; }
    }
}