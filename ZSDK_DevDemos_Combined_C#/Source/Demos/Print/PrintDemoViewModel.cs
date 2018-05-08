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
using Zebra.Sdk.Card.Enumerations;

namespace DeveloperDemo_Card_Desktop.Demos.Print {

    public class PrintDemoViewModel : ViewModelBase {

        private bool printFrontSide;
        private bool printFrontSideOverlay;
        private bool printBackSide;
        private PrintType frontSidePrintType;
        private ObservableCollection<PrintType> frontSidePrintTypes = new ObservableCollection<PrintType>();
        private string frontSideGraphicFilename;
        private string frontSideOverlayGraphicFilename;
        private string backSideGraphicFilename;
        private ObservableCollection<int> quantities = new ObservableCollection<int>();
        private int quantity;

        public PrintDemoViewModel() {
            quantities.Add(1);
            quantities.Add(2);
            quantities.Add(3);
            quantities.Add(4);
            quantities.Add(5);

            quantity = quantities[0];

            frontSidePrintTypes.Add(PrintType.Color);
            frontSidePrintTypes.Add(PrintType.MonoK);
        }

        public bool PrintFrontSide {
            get => printFrontSide;
            set {
                printFrontSide = value;
                OnPropertyChanged();
            }
        }

        public bool PrintFrontSideOverlay {
            get => printFrontSideOverlay;
            set {
                printFrontSideOverlay = value;
                OnPropertyChanged();
            }
        }

        public bool PrintBackSide {
            get => printBackSide;
            set {
                printBackSide = value;
                OnPropertyChanged();
            }
        }

        public PrintType FrontSidePrintType {
            get => frontSidePrintType;
            set {
                frontSidePrintType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PrintType> FrontSidePrintTypes {
            get => frontSidePrintTypes;
        }

        public string FrontSideGraphicFilename {
            get => frontSideGraphicFilename;
            set {
                frontSideGraphicFilename = value;
                OnPropertyChanged();
            }
        }

        public string FrontSideOverlayGraphicFilename {
            get => frontSideOverlayGraphicFilename;
            set {
                frontSideOverlayGraphicFilename = value;
                OnPropertyChanged();
            }
        }

        public string BackSideGraphicFilename {
            get => backSideGraphicFilename;
            set {
                backSideGraphicFilename = value;
                OnPropertyChanged();
            }
        }

        public int Quantity {
            get => quantity;
            set {
                quantity = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> Quantities {
            get => quantities;
        }
    }
}
