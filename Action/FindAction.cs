﻿using KAutoHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBroswer.Action
{
    class FindAction
    {
        public static Point? FindByImage(IntPtr mainHandle, string subBmPath, int count = 3, int wait = 1000)
        {
            int currentCount = 0;
            var res = new System.Drawing.Point?();
            var subBitMap = ImageScanOpenCV.GetImage(subBmPath);
            while (true)
            {
                var captureHandle = (Bitmap)CaptureHelper.CaptureWindow(mainHandle);
                res = ImageScanOpenCV.FindOutPoint(captureHandle, subBitMap);
                if (res.HasValue)
                {
                    AutoControl.SendClickOnPosition(mainHandle, res.Value.X, res.Value.Y);
                    return res;
                }

                if (++currentCount == count)
                {
                    return res;
                }
                Thread.Sleep(wait);
            }
        }

        internal static bool FindByImageNorImage(IntPtr mainHandle, string subBmPath, string subBmPath1, int count, int wait)
        {
            int currentCount = 0;
            var res = new System.Drawing.Point?();
            var subBitMap = ImageScanOpenCV.GetImage(subBmPath);
            var subBitMap1 = ImageScanOpenCV.GetImage(subBmPath);
            while (true)
            {
                var captureHandle = (Bitmap)CaptureHelper.CaptureWindow(mainHandle);
                res = ImageScanOpenCV.FindOutPoint(captureHandle, subBitMap);
                if (res.HasValue)
                {
                    AutoControl.SendClickOnPosition(mainHandle, res.Value.X, res.Value.Y);
                    return true;
                }

                res = ImageScanOpenCV.FindOutPoint(captureHandle, subBitMap);
                if (res.HasValue)
                {
                    AutoControl.SendClickOnPosition(mainHandle, res.Value.X, res.Value.Y);
                    return false;
                }

                if (++currentCount == count)
                {
                    return false;
                }
                Thread.Sleep(wait);
            }
        }
    }
}
