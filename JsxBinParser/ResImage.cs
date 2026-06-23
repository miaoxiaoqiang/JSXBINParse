using System.Drawing;

namespace JsxBinParser
{
    internal static class ResImage
    {
        static ResImage()
        {
            Success = Properties.Resources.Success;
            Error = Properties.Resources.Error;
            Solve = Properties.Resources.Analyze;
            Init = Properties.Resources.Init;
        }

        public static Image Success
        {
            get;
        }

        public static Image Error
        {
            get;
        }

        public static Image Solve
        {
            get;
        }

        public static Image Init
        {
            get;
        }
    }
}
