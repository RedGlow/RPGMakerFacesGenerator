using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RPGMakerFacesGenerator
{
    static class ImagesRegistry
    {
        private static Dictionary<Tuple<int, int, int, int>, Tuple<int, int, int, int>> metrics =
            new Dictionary<Tuple<int, int, int, int>, Tuple<int, int, int, int>>();
        private static bool initialized = false;

        private static void initialize()
        {
            // could be done with a static constructor, but they are usually harder to debug
            if (!initialized)
            {
                var fname = Path.Combine(getBaseDir(), "offsets.txt");
                using (var f = File.OpenText(fname))
                    for (; ; )
                    {
                        var line = f.ReadLine();
                        if (line == null)
                            break;
                        if (line.Length == 0)
                            continue;

                        var parts = line.Split(' ');
                        var frags = parts[0].Split('_');
                        var frag1 = int.Parse(frags[0]);
                        var frag2 = frags.Length < 2 ? -1 : int.Parse(frags[1]);
                        var frag3 = frags.Length < 3 ? -1 : int.Parse(frags[2]);
                        var frag4 = frags.Length < 4 ? -1 : int.Parse(frags[3]);
                        var x = int.Parse(parts[1]);
                        var y = int.Parse(parts[2]);
                        var width = int.Parse(parts[3]);
                        var height = int.Parse(parts[4]);

                        metrics[Tuple.Create(frag1, frag2, frag3, frag4)] = Tuple.Create(x, y, width, height);
                    }

                initialized = true;
            }
        }

        /// <summary>
        /// Get the full path of an image.
        /// </summary>
        /// <param name="frag1">First fragment.</param>
        /// <param name="frag2">Second fragment (optional).</param>
        /// <param name="frag3">Third fragment (optional).</param>
        /// <param name="frag4">Fourth fragment (optional).</param>
        /// <returns>The full path of the image.</returns>
        public static string GetFullPath(int frag1, int frag2 = -1, int frag3 = -1, int frag4 = -1)
        {
            initialize();

            var sb = new StringBuilder();

            // produce base path
            var basedir = getBaseDir();
            sb.Append(basedir);
            sb.Append(Path.DirectorySeparatorChar);

            // produce filename
            sb.Append(frag1.ToString());
            if(frag2 != -1)
            {
                sb.Append('_');
                sb.Append(frag2.ToString());
                if(frag3 != -1)
                {
                    sb.Append('_');
                    sb.Append(frag3.ToString());
                    if (frag4 != -1)
                    {
                        sb.Append('_');
                        sb.Append(frag4.ToString());
                    }
                }
            }
            sb.Append(".png");

            // return filename
            return sb.ToString();
        }

        /// <summary>
        /// Get the left offset of an image.
        /// </summary>
        /// <param name="frag1">First fragment.</param>
        /// <param name="frag2">Second fragment (optional).</param>
        /// <param name="frag3">Third fragment (optional).</param>
        /// <param name="frag4">Fourth fragment (optional).</param>
        /// <returns>The left offset of the image.</returns>
        public static int GetXOffset(int frag1, int frag2 = -1, int frag3 = -1, int frag4 = -1)
        {
            initialize();

            return getMetrics(frag1, frag2, frag3, frag4).Item1;
        }

        /// <summary>
        /// Get the top offset of an image.
        /// </summary>
        /// <param name="frag1">First fragment.</param>
        /// <param name="frag2">Second fragment (optional).</param>
        /// <param name="frag3">Third fragment (optional).</param>
        /// <param name="frag4">Fourth fragment (optional).</param>
        /// <returns>The left offset of the image.</returns>
        public static int GetYOffset(int frag1, int frag2 = -1, int frag3 = -1, int frag4 = -1)
        {
            initialize();

            return getMetrics(frag1, frag2, frag3, frag4).Item2;
        }

        /// <summary>
        /// Get the width of an image.
        /// </summary>
        /// <param name="frag1">First fragment.</param>
        /// <param name="frag2">Second fragment (optional).</param>
        /// <param name="frag3">Third fragment (optional).</param>
        /// <param name="frag4">Fourth fragment (optional).</param>
        /// <returns>The width of the image.</returns>
        public static int GetWidth(int frag1, int frag2 = -1, int frag3 = -1, int frag4 = -1)
        {
            initialize();

            return getMetrics(frag1, frag2, frag3, frag4).Item3;
        }

        /// <summary>
        /// Get the height of an image.
        /// </summary>
        /// <param name="frag1">First fragment.</param>
        /// <param name="frag2">Second fragment (optional).</param>
        /// <param name="frag3">Third fragment (optional).</param>
        /// <param name="frag4">Fourth fragment (optional).</param>
        /// <returns>The height of the image.</returns>
        public static int GetHeight(int frag1, int frag2 = -1, int frag3 = -1, int frag4 = -1)
        {
            initialize();

            return getMetrics(frag1, frag2, frag3, frag4).Item4;
        }

        private static Tuple<int, int, int, int> getMetrics(int frag1, int frag2, int frag3, int frag4)
        {
            return metrics[Tuple.Create(frag1, frag2, frag3, frag4)];
        }

        private static string getBaseDir()
        {
            var codebase = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            codebase = codebase.Substring(8);
            var basedir = Path.GetDirectoryName(codebase);
            return basedir + Path.DirectorySeparatorChar + "Data";
        }
    }
}
