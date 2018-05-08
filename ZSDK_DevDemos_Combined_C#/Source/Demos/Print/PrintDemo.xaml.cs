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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Card.Containers;
using Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Card.Graphics;
using Zebra.Sdk.Card.Graphics.Enumerations;
using Zebra.Sdk.Card.Job;
using static DeveloperDemo_Card_Desktop.UserControls.JobStatusControl;

namespace DeveloperDemo_Card_Desktop.Demos.Print {

    /// <summary>
    /// Interaction logic for PrintDemo.xaml
    /// </summary>
    public partial class PrintDemo : UserControl {

        private PrinterManager printerManager;
        private PrintDemoViewModel viewModel;

        public PrintDemo(PrinterManager printerManager) {
            InitializeComponent();

            this.printerManager = printerManager;
            viewModel = DataContext as PrintDemoViewModel;
        }

        private void BrowseFrontSideGraphicButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dialog = DialogHelper.CreateOpenImageFileDialog();
            if (dialog.ShowDialog() == true) {
                viewModel.FrontSideGraphicFilename = dialog.FileName;
            }
        }

        private void BrowseFrontSideOverlayGraphicButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dialog = DialogHelper.CreateOpenImageFileDialog();
            if (dialog.ShowDialog() == true) {
                viewModel.FrontSideOverlayGraphicFilename = dialog.FileName;
            }
        }

        private void BrowseBackSideGraphicButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dialog = DialogHelper.CreateOpenImageFileDialog();
            if (dialog.ShowDialog() == true) {
                viewModel.BackSideGraphicFilename = dialog.FileName;
            }
        }

        private async void PrintButton_Click(object sender, RoutedEventArgs e) {
            ZebraCardGraphics graphics = null;
            int? jobId = null;
            CardSource? cardSource = null;

            JobStatusControl.ClearLog();
            await printerManager.PerformAction("Sending print job to printer...", (zebraCardPrinter, connection) => {
                if (printerManager.IsPrinterReady(zebraCardPrinter, JobStatusControl)) {
                    graphics = new ZebraCardGraphics(zebraCardPrinter);

                    List<GraphicsInfo> graphicsData = new List<GraphicsInfo>();

                    if (viewModel.PrintFrontSide) {
                        byte[] frontSideGraphicData = ImageHelper.ConvertImage(ImageHelper.CreateImageFromFile(viewModel.FrontSideGraphicFilename));
                        graphics.Initialize(0, 0, OrientationType.Landscape, viewModel.FrontSidePrintType, System.Drawing.Color.White);
                        graphics.DrawImage(frontSideGraphicData, 0, 0, 0, 0, RotationType.RotateNoneFlipNone);
                        graphicsData.Add(BuildGraphicsInfo(graphics.CreateImage(), CardSide.Front, viewModel.FrontSidePrintType));
                        graphics.Clear();
                    }

                    if (viewModel.PrintFrontSideOverlay) {
                        if (viewModel.FrontSideOverlayGraphicFilename != null) {
                            byte[] frontSideOverlayGraphicData = ImageHelper.ConvertImage(ImageHelper.CreateImageFromFile(viewModel.FrontSideOverlayGraphicFilename));
                            graphics.Initialize(0, 0, OrientationType.Landscape, PrintType.Overlay, System.Drawing.Color.White);
                            graphics.DrawImage(frontSideOverlayGraphicData, 0, 0, 0, 0, RotationType.RotateNoneFlipNone);
                            graphicsData.Add(BuildGraphicsInfo(graphics.CreateImage(), CardSide.Front, PrintType.Overlay));
                            graphics.Clear();
                        } else {
                            graphicsData.Add(BuildGraphicsInfo(null, CardSide.Front, PrintType.Overlay));
                            graphics.Clear();
                        }
                    }

                    if (viewModel.PrintBackSide) {
                        byte[] backSideGraphicData = ImageHelper.ConvertImage(ImageHelper.CreateImageFromFile(viewModel.BackSideGraphicFilename));
                        graphics.Initialize(0, 0, OrientationType.Landscape, PrintType.MonoK, System.Drawing.Color.White);
                        graphics.DrawImage(backSideGraphicData, 0, 0, 0, 0, RotationType.RotateNoneFlipNone);
                        graphicsData.Add(BuildGraphicsInfo(graphics.CreateImage(), CardSide.Back, PrintType.MonoK));
                        graphics.Clear();
                    }

                    cardSource = (CardSource)Enum.Parse(typeof(CardSource), zebraCardPrinter.GetJobSettingValue(ZebraCardJobSettingNames.CARD_SOURCE));

                    jobId = zebraCardPrinter.Print(viewModel.Quantity, graphicsData);
                }
            }, (exception) => {
                string errorMessage = $"Error printing card: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                JobStatusControl.UpdateLog(errorMessage);
            }, () => {
                if (graphics != null) {
                    graphics.Close();
                }
            });

            if (jobId.HasValue && cardSource.HasValue) {
                await JobStatusControl.StartPolling(printerManager.Printer, new JobInfo(jobId.Value, cardSource.Value));
            }
        }

        private GraphicsInfo BuildGraphicsInfo(ZebraCardImageI zebraCardImage, CardSide side, PrintType printType) {
            return new GraphicsInfo {
                GraphicData = zebraCardImage ?? null,
                GraphicType = zebraCardImage != null ? GraphicType.BMP : GraphicType.NA,
                Side = side,
                PrintType = printType
            };
        }
    }
}
