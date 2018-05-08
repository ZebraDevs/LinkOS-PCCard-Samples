/***********************************************
 * CONFIDENTIAL AND PROPRIETARY 
 * 
 * The source code and other information contained herein is the confidential and exclusive property of
 * ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
 * This source code, and any other information contained herein, shall not be copied, reproduced, published, 
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
 * expressly permitted under such license agreement.
 * 
 * Copyright ZIH Corp. 2018
 * 
 * ALL RIGHTS RESERVED
 ***********************************************/

using DeveloperDemo_Card_Desktop.Utils;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Card.Graphics;
using Zebra.Sdk.Card.Graphics.Enumerations;

namespace DeveloperDemo_Card_Desktop.Demos.GraphicConversion {

    /// <summary>
    /// Interaction logic for GraphicConversionDemo.xaml
    /// </summary>
    public partial class GraphicConversionDemo : UserControl {

        public const string DefaultGraphicExtension = ".bmp";

        private GraphicConversionDemoViewModel viewModel;
        private PrinterManager printerManager;

        public GraphicConversionDemo(PrinterManager printerManager) {
            InitializeComponent();

            viewModel = DataContext as GraphicConversionDemoViewModel;
            this.printerManager = printerManager;
        }

        private void UpdateDisplayedDimensions(string path) {
            try {
                System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                viewModel.Width = img.Width;
                viewModel.Height = img.Height;
            } catch (Exception e) {
                MessageBoxHelper.ShowError($"Error updating image dimensions: {e.Message}");
            }
        }

        private void BrowseOriginalGraphicButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dialog = DialogHelper.CreateOpenImageFileDialog();
            if (dialog.ShowDialog() == true) {
                viewModel.OriginalGraphicFilename = dialog.FileName;
                UpdateDisplayedDimensions(dialog.FileName);
            }
        }

        private void BrowseConvertedGraphicButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.SaveFileDialog dialog = DialogHelper.CreateSaveImageFileDialog();
            if (dialog.ShowDialog() == true) {
                viewModel.ConvertedGraphicFilename = Path.Combine(Path.GetDirectoryName(dialog.FileName), Path.GetFileNameWithoutExtension(dialog.FileName)) + DefaultGraphicExtension;
            }
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e) {
            JobStatusControl.ClearLog();

            await Task.Run(() => {
                ZebraGraphics graphics = null;

                try {
                    graphics = new ZebraCardGraphics(null) {
                        PrinterModel = viewModel.SelectedPrinterModelInfo.PrinterModel
                    };

                    if (!Path.IsPathRooted(viewModel.OriginalGraphicFilename)) {
                        throw new ArgumentException("Original graphic filename must be an absolute path");
                    }

                    System.Drawing.Image image = ImageHelper.CreateImageFromFile(viewModel.OriginalGraphicFilename);
                    byte[] imageData = ImageHelper.ConvertImage(image);
                    
                    int width; // Width of final output image
                    int height; // Height of final output image

                    switch (viewModel.SelectedDimensionOption) {
                        case DimensionOption.Crop:
                            int croppedWidth = ConstrainWidth(viewModel.Width, viewModel.SelectedPrinterModelInfo.MaxWidth);
                            int croppedHeight = ConstrainHeight(viewModel.Height, viewModel.SelectedPrinterModelInfo.MaxHeight);
                            imageData = CropImage(graphics, imageData, croppedWidth, croppedHeight);

                            width = croppedWidth;
                            height = croppedHeight;
                            break;
                        case DimensionOption.Resize:
                            width = ConstrainWidth(viewModel.Width, viewModel.SelectedPrinterModelInfo.MaxWidth);
                            height = ConstrainHeight(viewModel.Height, viewModel.SelectedPrinterModelInfo.MaxHeight);

                            JobStatusControl.UpdateLog($"Resizing image to {width}x{height}...");
                            break;
                        case DimensionOption.Original:
                        default:
                            width = ConstrainWidth(image.Width, viewModel.SelectedPrinterModelInfo.MaxWidth);
                            height = ConstrainHeight(image.Height, viewModel.SelectedPrinterModelInfo.MaxHeight);

                            JobStatusControl.UpdateLog("Keeping current image dimensions unless they exceed the maximum model-specific width and height...");
                            break;
                    }

                    GraphicsFormat graphicsFormat = viewModel.SelectedGraphicsFormat;
                    MonochromeConversion monochromeConversionType = viewModel.SelectedGraphicsFormat.GetMonochromeConversion();
                    PrintType printType = viewModel.SelectedGraphicsFormat.GetPrintType();
                    OrientationType orientationType = OrientationType.Landscape;

                    JobStatusControl.UpdateLog($"Setting orientation to {orientationType}...");

                    graphics.Initialize(width, height, orientationType, printType, System.Drawing.Color.White);
                    graphics.DrawImage(imageData, 0, 0, width, height, RotationType.RotateNoneFlipNone);
                    ApplyMonochromeConversion(graphics, printType, monochromeConversionType);

                    JobStatusControl.UpdateLog($"Writing graphic file to path {viewModel.ConvertedGraphicFilename}...");

                    WriteToFile(viewModel.ConvertedGraphicFilename, graphics.CreateImage().ImageData);

                    JobStatusControl.UpdateLog("Finished converting graphic");
                } catch (Exception exception) {
                    string errorMessage = $"Error converting graphic: {exception.Message}";
                    JobStatusControl.UpdateLog(errorMessage);
                    MessageBoxHelper.ShowError(errorMessage);
                } finally {
                    if (graphics != null) {
                        graphics.Close();
                    }
                }
            });
        }

        private byte[] CropImage(ZebraGraphics graphics, byte[] imageData, int croppedWidth, int croppedHeight) {
            int xOffset = viewModel.XOffset < 0 ? 0 : viewModel.XOffset;
            int yOffset = viewModel.YOffset < 0 ? 0 : viewModel.YOffset;

            JobStatusControl.UpdateLog($"Cropping image from xOffset:{xOffset} yOffset:{yOffset} with width:{croppedWidth} and height:{croppedHeight}...");

            byte[] croppedImage = graphics.CropImage(imageData, xOffset, yOffset, croppedWidth, croppedHeight);

            JobStatusControl.UpdateLog("Finished cropping image");

            return croppedImage;
        }

        private int ConstrainWidth(int width, int? maxWidth) {
            if (width < 1) {
                JobStatusControl.UpdateLog("Width must be positive. Setting width to 1...");
                width = 1;
            } else if (maxWidth.HasValue && width > maxWidth.Value) {
                JobStatusControl.UpdateLog($"Specified width ({width}) is greater than the maximum width ({maxWidth.Value}). Setting width to maximum width...");
                width = maxWidth.Value;
            }
            return width;
        }

        private int ConstrainHeight(int height, int? maxHeight) {
            if (height < 1) {
                JobStatusControl.UpdateLog("Height must be positive. Setting height to 1...");
                height = 1;
            } else if (maxHeight.HasValue && height > maxHeight.Value) {
                JobStatusControl.UpdateLog($"Specified height ({height}) is greater than the maximum height ({maxHeight.Value}). Setting height to maximum height...");
                height = maxHeight.Value;
            }
            return height;
        }

        private void ApplyMonochromeConversion(ZebraGraphics graphics, PrintType printType, MonochromeConversion monochromeConversionType) {
            JobStatusControl.UpdateLog("Converting graphic...");

            if (printType != PrintType.MonoK && printType != PrintType.GrayDye) {
                switch (monochromeConversionType) {
                    case MonochromeConversion.Diffusion:
                        JobStatusControl.UpdateLog("Ignoring diffusion option for non-mono/gray format type...");
                        break;
                    case MonochromeConversion.HalfTone_6x6:
                    case MonochromeConversion.HalfTone_8x8:
                        JobStatusControl.UpdateLog("Ignoring halftone option for non-mono/gray format type...");
                        break;
                }
            } else {
                switch (monochromeConversionType) {
                    case MonochromeConversion.Diffusion:
                        graphics.MonochromeConverionType = MonochromeConversion.Diffusion;
                        JobStatusControl.UpdateLog("Applying diffusion algorithm...");
                        break;
                    case MonochromeConversion.HalfTone_6x6:
                        graphics.MonochromeConverionType = MonochromeConversion.HalfTone_6x6;
                        JobStatusControl.UpdateLog("Applying 6x6 halftone algorithm...");
                        break;
                    case MonochromeConversion.HalfTone_8x8:
                        graphics.MonochromeConverionType = MonochromeConversion.HalfTone_8x8;
                        JobStatusControl.UpdateLog("Applying 8x8 halftone algorithm...");
                        break;
                }
            }
        }

        private void WriteToFile(string path, byte[] bytes) {
            if (!System.IO.Path.IsPathRooted(path)) {
                throw new ArgumentException("Converted graphic filename must be an absolute path");
            }

            try {
                FileInfo fileInfo = new FileInfo(path);
                fileInfo.Directory.Create();
                File.WriteAllBytes(fileInfo.FullName, bytes);
            } catch (Exception e) {
                throw new ArgumentException($"Could not write file to path {path}: {e.Message}", e);
            }
        }
    }
}
