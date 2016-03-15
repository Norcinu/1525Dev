
namespace PDTUtils.MVVM.Models
{
    class ScreenshotModel
    {
        public int ImageID { get; set; }
        public string FileName { get; set; }

        public ScreenshotModel()
        {
            this.ImageID = -1;
            this.FileName = "";
        }

        public ScreenshotModel(int id, string filename)
        {
            this.ImageID = id;
            this.FileName = filename;
        }
    }
}
