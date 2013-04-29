using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGMakerFacesGenerator
{
    public class FacePart
    {
        public string Filename { get; private set; }
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int Frag1 { get; private set; }
        public int Frag2 { get; private set; }
        public int Frag3 { get; private set; }
        public int Frag4 { get; private set; }

        public HSVData HSVData { get; private set; }

        public FacePart(HSVData hsvData, int frag1, int frag2 = -1, int frag3 = -1, int frag4 = -1)
        {
            HSVData = hsvData;
            Frag1 = frag1;
            Frag2 = frag2;
            Frag3 = frag3;
            Frag4 = frag4;
            Filename = ImagesRegistry.GetFullPath(frag1, frag2, frag3, frag4);
            XOffset = ImagesRegistry.GetXOffset(frag1, frag2, frag3, frag4);
            YOffset = ImagesRegistry.GetYOffset(frag1, frag2, frag3, frag4);
            Width = ImagesRegistry.GetWidth(frag1, frag2, frag3, frag4);
            Height = ImagesRegistry.GetHeight(frag1, frag2, frag3, frag4);
        }
    }
}
