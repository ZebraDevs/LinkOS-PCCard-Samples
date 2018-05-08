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

using System.Collections.ObjectModel;
using Zebra.Sdk.Card.Graphics.Enumerations;

namespace DeveloperDemo_Card_Desktop.Demos.GraphicConversion {

    public class GraphicConversionDemoViewModel : ViewModelBase {

        private const int MAX_WIDTH_ZXP7_AND_ZC = 1006;
        private const int MAX_WIDTH_ZXP8_AND_ZXP9 = 1024;
        private const int MAX_HEIGHT_ZXP7_AND_ZC = 640;
        private const int MAX_HEIGHT_ZXP8_AND_ZXP9 = 648;

        private string originalGraphicFilename;
        private string convertedGraphicFilename;
        private GraphicsFormat selectedGraphicsFormat;
        private ObservableCollection<GraphicsFormat> graphicsFormats = new ObservableCollection<GraphicsFormat>();
        private PrinterModelInfo selectedPrinterModelInfo;
        private ObservableCollection<PrinterModelInfo> printerModels = new ObservableCollection<PrinterModelInfo>();
        private DimensionOption selectedDimensionOption;
        private ObservableCollection<DimensionOption> dimensionOptions = new ObservableCollection<DimensionOption>();
        private int width;
        private int height;
        private int xOffset;
        private int yOffset;

        public GraphicConversionDemoViewModel() {
            graphicsFormats.Add(GraphicsFormat.GrayHalfTone8x8);
            graphicsFormats.Add(GraphicsFormat.GrayHalfTone6x6);
            graphicsFormats.Add(GraphicsFormat.GrayDiffusion);
            graphicsFormats.Add(GraphicsFormat.MonoHalfTone8x8);
            graphicsFormats.Add(GraphicsFormat.MonoHalfTone6x6);
            graphicsFormats.Add(GraphicsFormat.MonoDiffusion);
            graphicsFormats.Add(GraphicsFormat.Color);

            printerModels.Add(new PrinterModelInfo(PrinterModel.ZXPSeries7, "ZXP7", MAX_WIDTH_ZXP7_AND_ZC, MAX_HEIGHT_ZXP7_AND_ZC));
            printerModels.Add(new PrinterModelInfo(PrinterModel.ZXPSeries8, "ZXP8", MAX_WIDTH_ZXP8_AND_ZXP9, MAX_HEIGHT_ZXP8_AND_ZXP9));
            printerModels.Add(new PrinterModelInfo(PrinterModel.ZXPSeries9, "ZXP9", MAX_WIDTH_ZXP8_AND_ZXP9, MAX_HEIGHT_ZXP8_AND_ZXP9));
            printerModels.Add(new PrinterModelInfo(PrinterModel.ZC100, "ZC100", MAX_WIDTH_ZXP7_AND_ZC, MAX_HEIGHT_ZXP7_AND_ZC));
            printerModels.Add(new PrinterModelInfo(PrinterModel.ZC150, "ZC150", MAX_WIDTH_ZXP7_AND_ZC, MAX_HEIGHT_ZXP7_AND_ZC));
            printerModels.Add(new PrinterModelInfo(PrinterModel.ZC300, "ZC300", MAX_WIDTH_ZXP7_AND_ZC, MAX_HEIGHT_ZXP7_AND_ZC));
            printerModels.Add(new PrinterModelInfo(PrinterModel.ZC350, "ZC350", MAX_WIDTH_ZXP7_AND_ZC, MAX_HEIGHT_ZXP7_AND_ZC));

            selectedPrinterModelInfo = printerModels[0];

            dimensionOptions.Add(DimensionOption.Original);
            dimensionOptions.Add(DimensionOption.Resize);
            dimensionOptions.Add(DimensionOption.Crop);
        }

        public string OriginalGraphicFilename {
            get => originalGraphicFilename;
            set {
                originalGraphicFilename = value;
                OnPropertyChanged();
            }
        }

        public string ConvertedGraphicFilename {
            get => convertedGraphicFilename;
            set {
                convertedGraphicFilename = value;
                OnPropertyChanged();
            }
        }

        public GraphicsFormat SelectedGraphicsFormat {
            get => selectedGraphicsFormat;
            set {
                selectedGraphicsFormat = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GraphicsFormat> GraphicsFormats {
            get => graphicsFormats;
        }

        public PrinterModelInfo SelectedPrinterModelInfo {
            get => selectedPrinterModelInfo;
            set {
                selectedPrinterModelInfo = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PrinterModelInfo> PrinterModels {
            get => printerModels;
        }

        public DimensionOption SelectedDimensionOption {
            get => selectedDimensionOption;
            set {
                selectedDimensionOption = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DimensionOption> DimensionOptions {
            get => dimensionOptions;
        }

        public int Width {
            get => width;
            set {
                width = value;
                OnPropertyChanged();
            }
        }

        public int Height {
            get => height;
            set {
                height = value;
                OnPropertyChanged();
            }
        }

        public int XOffset {
            get => xOffset;
            set {
                xOffset = value;
                OnPropertyChanged();
            }
        }

        public int YOffset {
            get => yOffset;
            set {
                yOffset = value;
                OnPropertyChanged();
            }
        }
    }
}
