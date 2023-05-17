using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Threading;

namespace Change_resolution
{
    internal class Program
    {
        static void ANiceTouch(string Syntax, bool GoBack)
        {
            if (GoBack)
                Console.SetCursorPosition(0, 3);

            foreach (char ch in Syntax)
            {
                Console.Write(ch);
                Thread.Sleep(15);
            }
            Console.Write("\n\r\t\r");
        }
        static void Main(string[] args)
        {
            ChangeDPI();

            return;

            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Console.CursorVisible = false;

            string sourceDirectory = "";

            bool Reverse = false;

            bool Restart = true;

            int Width = 0;

            int Height = 0;

            while (Restart)
            {
                Console.Clear();

                Console.SetCursorPosition(0, 0);

                ANiceTouch("Please Enter The Directory Of The Folder:", false);

                sourceDirectory = Console.ReadLine();

                ANiceTouch("Please Enter the new Width of the image", true);

                Width = int.Parse(Console.ReadLine());

                ANiceTouch("Please Enter the new Height of the image", true);

                Height = int.Parse(Console.ReadLine());

                Restart = false;

                try
                {
                    ChangeResolution(sourceDirectory, Width, Height, Reverse);
                }
                catch (Exception ex)
                {
                    ANiceTouch("Error: This Path Is Not Of Legal Form\n", false);

                    ANiceTouch("The Program Is Going To Restart Itself Now", false);

                    Thread.Sleep(3000);

                    Restart = true;
                }
            }

            Console.WriteLine();

            ANiceTouch("Do You Want To Open The Folder?\n\nPress Enter If You Do\n\nTo Continue Press Any Other Button", false);

            ConsoleKeyInfo keyinfo = Console.ReadKey();

            if (keyinfo.Key == ConsoleKey.Enter)
            {
                // Start a new process for the file explorer
                Process fileExplorer = new Process();
                fileExplorer.StartInfo.FileName = sourceDirectory;
                fileExplorer.StartInfo.UseShellExecute = true;
                fileExplorer.Start();

                // Refresh the file explorer window
                fileExplorer.Refresh();
            }

            Console.WriteLine();

            ANiceTouch("Do You Regret Your Actions And Want To Go Back?\n\nPress Enter If You Do\n\nTo Leave Press Any Other Button\t\t", true);

            keyinfo = Console.ReadKey();

            if (keyinfo.Key == ConsoleKey.Enter)
            {
                Reverse = true;
                ChangeResolution(sourceDirectory, Width, Height, Reverse);
            }

            Console.WriteLine();
        }
        static void ChangeResolution(string sourceDirectory, int Width, int Height, bool Reverse)
        {
            List<string> folders = new List<string>(Directory.GetDirectories(sourceDirectory));

            foreach (string folder in folders)
                ChangeResolution(folder, Width, Height, Reverse);

            string NewFolderPath = Path.Combine(sourceDirectory, "Images Reduced");

            // Get all files in the source directory
            List<string> files = new List<string>(Directory.GetFiles(sourceDirectory));

            int NameIndex = 0;

            string FileName = "";

            foreach (string file in files)
            {
                // Load the image from a file
                Image originalImage = Image.FromFile(file);

                // Create a new Bitmap object with the desired width and height
                Bitmap resizedImage = new Bitmap(Width, Height);

                // Create a Graphics object from the Bitmap
                Graphics graphics = Graphics.FromImage(resizedImage);

                // Set the interpolation mode to HighQualityBicubic to ensure high quality resizing
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // Draw the original image onto the resized Bitmap using the Graphics object
                graphics.DrawImage(originalImage, 0, 0, Width, Height);

                if (!Directory.Exists(NewFolderPath))
                {
                    // Create the new directory
                    Directory.CreateDirectory(NewFolderPath);
                }

                FileName = file.Substring(sourceDirectory.Length, file.Length - sourceDirectory.Length - 4);

                // Save the resized image to a file
                resizedImage.Save(NewFolderPath + $"\\{FileName}  Lower Resolution.jpg");

                // Dispose of the Graphics object and the original image
                graphics.Dispose();
                originalImage.Dispose();
            }
        }
        static void ChangeDPI()
        {
            Console.WriteLine("Please Enter The Directory");

            // Load the image from a file
            Bitmap originalImage = new Bitmap(Console.ReadLine());

            // Set the desired DPI
            int dpi = 40;

            // Set the image resolution
            originalImage.SetResolution(dpi, dpi);

            // Get the JPEG codec info
            ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders().First(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

            // Set the encoder parameters to adjust the quality and compression level
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 90L);

            Console.WriteLine("Please Enter The Directory You Want To Save The Image");

            // Save the image with the new DPI and encoder parameters
            originalImage.Save(Console.ReadLine(), jpegCodec, encoderParams);

            // Dispose of the original image
            originalImage.Dispose();
        }
    }
}
