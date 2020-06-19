using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;
///Algorithms Project
///Intelligent Scissors
///

namespace ImageQuantization
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }
    public struct vertix_data
    {
        public RGBPixel my_node;
        public double weight;
        public RGBPixel parent;
        public int pos;
        public int perent_pos;

        public vertix_data(RGBPixel my_node, double weight, RGBPixel parent, int poo, int parentp)
        {
            this.perent_pos = parentp;
            this.my_node = my_node;
            this.weight = weight;
            this.parent = parent;
            this.pos = poo;
        }
    }



    public struct Edge
    {

        public RGBPixel child;
        public int pos_in_myarr;


    }


    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        public static Hashtable clusterID;
        public static HashSet<Edge>[] graphs;
        public static vertix_data[] myarr;
        public static RGBPixel[] colours;
        public static int num_color;
        public static bool[] vistted;
        public static int num_cluster;
        public static LinkedList<RGBPixel> listcolor = new LinkedList<RGBPixel>();
        public static RGBPixel[] platte;
        public static RGBPixel[,,] histogram = new RGBPixel[256, 256, 256];


        //Graph[] Mygraph;

        public static int dist_colors(RGBPixel[,] colour_mat)      /* O(N^2) */   // Where N  is the image Width or Height
        {
            int height = GetHeight(colour_mat);                     //O(1)
            int width = GetWidth(colour_mat);                      //O(1)
            bool[,,] memo = new bool[256, 256, 256];                  //O(1)
            int count = 0;
            for (int i = 0; i < height; i++)                      //O(N^2)
            {
                for (int j = 0; j < width; j++)                  //O(N)
                {
                    if (!(memo[colour_mat[i, j].red, colour_mat[i, j].green, colour_mat[i, j].blue]))  //O(1)
                    {
                        memo[colour_mat[i, j].red, colour_mat[i, j].green, colour_mat[i, j].blue] = true;  //O(1)
                        count++;
                        listcolor.AddLast(colour_mat[i, j]); //O(1)

                    }

                }
            }
            num_color = count;  //O(1)
            colours = new RGBPixel[num_color]; //O(1)
            for (int i = 0; i < num_color; i++)  //O(d) where d is number of colours
            {
                colours[i] = listcolor.First.Value;   //O(1)
                listcolor.RemoveFirst();           //O(1)
            }
            return num_color;                              //O(1)
        }




        public static double eculidean_distance(RGBPixel color1, RGBPixel color2)         /* O(1) */
        {
            double ED;
            ED = ((color1.red - color2.red) * (color1.red - color2.red))                   // O(1)
                + ((color1.green - color2.green) * (color1.green - color2.green))
                + ((color1.blue - color2.blue) * (color1.blue - color2.blue));

            ED = Math.Sqrt(ED);                                                         // O(1)
            return ED;                                                                 // O(1)
        }


        public static void construct_list()
        {
            graphs = new HashSet<Edge>[num_color];
            for (int i = 0; i < num_color; i++)
            {
                graphs[i] = new HashSet<Edge>();
            }
            vertix_data parent;
            Edge e;
            for (int i = 0; i < num_color; i++)
            {
                parent = myarr[i];

                for (int j = 0; j < num_color; j++)
                {
                    if (parent.my_node.red == myarr[j].parent.red && parent.my_node.green == myarr[j].parent.green
                        && parent.my_node.blue == myarr[j].parent.blue)
                    {
                        e.child = myarr[j].my_node;
                        e.pos_in_myarr = myarr[j].pos;
                        graphs[parent.pos].Add(e);
                        e.child = parent.my_node;
                        e.pos_in_myarr = parent.pos;
                        graphs[myarr[j].pos].Add(e);
                    }
                }
            }
        }


        public static double MST()                       /*    O(E log V)    */   // Where V  is the  number of distenct colors  & E is number of edges 
        {

            myarr = new vertix_data[num_color];             //O(1)
            bool[] visit = new bool[num_color];             //O(1)
            double sum = 0;
            MinHeap heap = new MinHeap(num_color);         //O(1)
            for (int i = 0; i < num_color; i++)               // O(V log V)
            {
                vertix_data r = new vertix_data();       //O(1)
                if (i == 0) r.weight = 0;                //O(1)
                else
                {
                    r.weight = double.MaxValue;         //O(1)
                }
                r.my_node = colours[i];                  //O(1)
                r.pos = i;                              //O(1)
                visit[i] = false;                       //O(1)
                myarr[i] = r;                           //O(1)
                heap.insert(r);                         //O(log V)
            }

            while (!heap.isEmpty())   //heapnotnull                                                        //O(E log V)
            {
                vertix_data node = heap.extractMin(); //fromheap                                           //O(log V)
                visit[node.pos] = true;
                sum += node.weight;
                for (int i = 0; i < num_color; i++)                                                         //O(E log V)
                {
                    if (visit[i]) continue;                                                                 //O(1)
                    else
                    {
                        double weight = eculidean_distance(node.my_node, colours[i]);                       //O(1)
                        if (myarr[i].weight > weight)                                                       //O(1)
                        {
                            vertix_data data = new vertix_data();                                           //O(1) 
                            data.weight = weight;                                                           //O(1)
                            data.parent = node.my_node;                                                     //O(1)
                            data.pos = i;                                                                   //O(1)
                            data.perent_pos = node.pos;
                            data.my_node = myarr[i].my_node;                                                //O(1)
                            heap.decrease(data, myarr[i].pos);                                              //O(log V)
                            myarr[i] = data;                                                                //O(1)
                        }

                    }

                }

            }
            return sum;
        }







        public static void k_cluster(int k)
        {
            num_cluster = k;
            Edge e1 = new Edge();
            Edge e2 = new Edge();
            int pe1 = 0, pe2 = 0;

            double max = -1;
            for (int i = 0; i < k - 1; i++)
            {
                int pos = 0;
                max = -1;
                for (int j = 0; j < num_color; j++)
                {
                    if (myarr[j].weight > max)
                    {
                        max = myarr[j].weight;
                        pe1 = myarr[j].perent_pos;
                        e1.child = myarr[j].my_node;
                        e1.pos_in_myarr = myarr[j].pos;
                        pos = j;
                        pe2 = myarr[j].pos;
                        e2.child = myarr[j].parent;
                        e2.pos_in_myarr = myarr[j].perent_pos;
                    }

                }
                myarr[pos].weight = -1;
                graphs[pe1].Remove(e1);
                graphs[pe2].Remove(e2);
            }

        }

        public static RGBPixel bfs(vertix_data element, int id)
        {

            Queue<vertix_data> queue = new Queue<vertix_data>();
            queue.Enqueue(element);
         
            int red = 0, green = 0, blue = 0;
            int num = 0;
            while (queue.Count != 0)
            {
                vertix_data node = queue.Dequeue();
                red += node.my_node.red;
                green += node.my_node.green;
                blue += node.my_node.blue;
                num++;
                vistted[node.pos] = true;
                if (graphs[node.pos].Count != 0)
                {
                    foreach (var v in graphs[node.pos])
                    {

                        if (!(vistted[v.pos_in_myarr]))
                        {

                            queue.Enqueue(myarr[v.pos_in_myarr]);
                        }
                    }
                }

            }
            blue /= num;
            red /= num;
            green /= num;
            RGBPixel final;
            final.red = (byte)red;
            final.green = (byte)green;
            final.blue = (byte)blue;
            return final;
        }



        public static void plate()
        {
            platte = new RGBPixel[num_cluster];
            
            vistted = new bool[num_color];

            int index = 0;
            for (int i = 0; i < graphs.Length; i++)
            {
                if (!vistted[i])
                {
                    platte[index] = bfs(myarr[i], index);
                    index++;
                }
            }
            vistted = new bool[num_color];
            index = 0;
            for (int i = 0; i < graphs.Length; i++)
            {
                if (!vistted[i])
                {
                    new_bfs(myarr[i], index,platte[index]);
                    index++;
                }
            }
        }
        public static void new_bfs (vertix_data element, int id, RGBPixel new_pallet)
        {

            Queue<vertix_data> queue = new Queue<vertix_data>();
            queue.Enqueue(element);


            while (queue.Count != 0)
            {
                vertix_data node = queue.Dequeue();
                histogram[node.my_node.red, node.my_node.blue, node.my_node.green]=new_pallet;
                vistted[node.pos] = true;
                if (graphs[node.pos].Count != 0)
                {
                    foreach (var v in graphs[node.pos])
                    {

                        if (!(vistted[v.pos_in_myarr]))
                        {
                            queue.Enqueue(myarr[v.pos_in_myarr]);
                        }
                    }
                }

            }

        }




        public static void final_image(RGBPixel[,] image)
        {
            int height = GetHeight(image);
            int width = GetWidth(image);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                   
                    image[i, j] = histogram[image[i,j].red, image[i, j].blue,image[i, j].green];

                }
            }

        }















        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }



        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <reurns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }



        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }




        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }



        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];


            // Create Filter in Spatial Domain:
            //=================================
            //make the filter ODD size
            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];

            //Compute Filter in Spatial Domain :
            //==================================
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
                Sum1 += Filter[y + HalfSize];
            }
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                Filter[y + HalfSize] /= Sum1;
            }

            //Filter Original Image Vertically:
            //=================================
            int ii, jj;
            RGBPixelD Sum;
            RGBPixel Item1;
            RGBPixelD Item2;

            for (int j = 0; j < Width; j++)
                for (int i = 0; i < Height; i++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int y = -HalfSize; y <= HalfSize; y++)
                    {
                        ii = i + y;
                        if (ii >= 0 && ii < Height)
                        {
                            Item1 = ImageMatrix[ii, j];
                            Sum.red += Filter[y + HalfSize] * Item1.red;
                            Sum.green += Filter[y + HalfSize] * Item1.green;
                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
                        }
                    }
                    VerFiltered[i, j] = Sum;
                }

            //Filter Resulting Image Horizontally:
            //===================================
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int x = -HalfSize; x <= HalfSize; x++)
                    {
                        jj = j + x;
                        if (jj >= 0 && jj < Width)
                        {
                            Item2 = VerFiltered[i, jj];
                            Sum.red += Filter[x + HalfSize] * Item2.red;
                            Sum.green += Filter[x + HalfSize] * Item2.green;
                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
                        }
                    }
                    Filtered[i, j].red = (byte)Sum.red;
                    Filtered[i, j].green = (byte)Sum.green;
                    Filtered[i, j].blue = (byte)Sum.blue;
                }

            return Filtered;
        }


    }
}
