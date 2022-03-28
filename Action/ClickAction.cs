using KAutoHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBroswer.Action
{
    class ClickAction
    {
        public static bool ClickByImage(IntPtr mainHandle, string subBmPath, int count = 3, int wait = 1000)
        {
            int currentCount = 0;
            var res = new System.Drawing.Point?();
            var subBitMap = ImageScanOpenCV.GetImage(subBmPath);
            while (true)
            {
                var captureHandle = (Bitmap)CaptureHelper.CaptureWindow(mainHandle);
                res = ImageScanOpenCV.FindOutPoint(captureHandle, subBitMap, 0.80);
                if (res.HasValue)
                {
                    AutoControl.SendClickOnPosition(mainHandle, res.Value.X, res.Value.Y);
                    return true;
                }

                if (++currentCount == count)
                {
                    return false;
                }
                Thread.Sleep(wait);
            }
        }

        public static bool ClickByImageOrImage(IntPtr windowHandle, string v1, string v2, int v3, int v4)
        {
            int currentCount = 0;
            var res = new System.Drawing.Point?();
            var subBitMap = ImageScanOpenCV.GetImage(v1);
            var subBitMap2 = ImageScanOpenCV.GetImage(v2);
            while (true)
            {
                var captureHandle = (Bitmap)CaptureHelper.CaptureWindow(windowHandle);
                res = ImageScanOpenCV.FindOutPoint(captureHandle, subBitMap);
                if (res.HasValue)
                {
                    AutoControl.SendClickOnPosition(windowHandle, res.Value.X, res.Value.Y);
                    return true;
                }
                res = ImageScanOpenCV.FindOutPoint(captureHandle, subBitMap2);
                if (res.HasValue)
                {
                    AutoControl.SendClickOnPosition(windowHandle, res.Value.X, res.Value.Y);
                    return true;
                }

                if (++currentCount == v3)
                {
                    return false;
                }
                Thread.Sleep(v4);
            }
        }

        public static void ClickByPosition(IntPtr windowHandle, int v1, int v2)
        {
            AutoControl.SendClickOnPosition(windowHandle, v1, v2);

        }
    }
}
