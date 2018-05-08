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

using System.Collections.Generic;
using Zebra.Sdk.Card.Enumerations;

namespace DeveloperDemo_Card_Desktop.Demos.MultiJob {

    public class MultiJobControlViewModel : ViewModelBase {

        private int? jobId = null;
        private MultiJobNumber jobNumber;
        private string selectedSource = null;
        private string selectedDestination = null;
        private string selectedPrintOptimization = null;
        private int selectedQuantity = 1;
        private bool printFrontSide = false;
        private bool printBackSide = false;
        private PrintType frontSidePrintType;
        private PrintType backSidePrintType;
        private bool isFrontSideFullOverlay = false;
        private bool isBackSideFullOverlay = false;
        private string frontSideGraphicFilename = null;
        private string backSideGraphicFilename = null;
        private Dictionary<PrintType, string> frontSideGraphicFilenames = new Dictionary<PrintType, string>();
        private Dictionary<PrintType, string> backSideGraphicFilenames = new Dictionary<PrintType, string>();
        private bool magneticEncode = false;
        private string selectedCoercivityType = null;
        private string track1Data = "";
        private string track2Data = "";
        private string track3Data = "";

        public int? JobId {
            get => jobId;
            set {
                jobId = value;
                OnPropertyChanged();
            }
        }

        public MultiJobNumber JobNumber {
            get => jobNumber;
            set {
                jobNumber = value;
                OnPropertyChanged();
            }
        }

        public string SelectedSource {
            get => selectedSource;
            set {
                selectedSource = value;
                OnPropertyChanged();
            }
        }

        public string SelectedDestination {
            get => selectedDestination;
            set {
                selectedDestination = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPrintOptimization {
            get => selectedPrintOptimization;
            set {
                selectedPrintOptimization = value;
                OnPropertyChanged();
            }
        }

        public int SelectedQuantity {
            get => selectedQuantity;
            set {
                selectedQuantity = value;
                OnPropertyChanged();
            }
        }

        public bool PrintFrontSide {
            get => printFrontSide;
            set {
                printFrontSide = value;
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

        public PrintType BackSidePrintType {
            get => backSidePrintType;
            set {
                backSidePrintType = value;
                OnPropertyChanged();
            }
        }

        public bool IsFrontSideFullOverlay {
            get => isFrontSideFullOverlay;
            set {
                isFrontSideFullOverlay = value;
                OnPropertyChanged();
            }
        }

        public bool IsBackSideFullOverlay {
            get => isBackSideFullOverlay;
            set {
                isBackSideFullOverlay = value;
                OnPropertyChanged();
            }
        }

        public string FrontSideGraphicFilename {
            get => frontSideGraphicFilename;
            set {
                frontSideGraphicFilename = value;
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

        public Dictionary<PrintType, string> FrontSideGraphicFilenames {
            get => frontSideGraphicFilenames;
            set {
                frontSideGraphicFilenames = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<PrintType, string> BackSideGraphicFilenames {
            get => backSideGraphicFilenames;
            set {
                backSideGraphicFilenames = value;
                OnPropertyChanged();
            }
        }

        public bool MagneticEncode {
            get => magneticEncode;
            set {
                magneticEncode = value;
                OnPropertyChanged();
            }
        }

        public string SelectedCoercivityType {
            get => selectedCoercivityType;
            set {
                selectedCoercivityType = value;
                OnPropertyChanged();
            }
        }

        public string Track1Data {
            get => track1Data;
            set {
                track1Data = value;
                OnPropertyChanged();
            }
        }

        public string Track2Data {
            get => track2Data;
            set {
                track2Data = value;
                OnPropertyChanged();
            }
        }

        public string Track3Data {
            get => track3Data;
            set {
                track3Data = value;
                OnPropertyChanged();
            }
        }

        public bool HasValidFrontSide {
            get => PrintFrontSide && FrontSideGraphicFilenames.Count > 0;
        }

        public bool HasValidBackSide {
            get => PrintBackSide && BackSideGraphicFilenames.Count > 0;
        }

        public bool HasMagData {
            get => !string.IsNullOrEmpty(Track1Data) || !string.IsNullOrEmpty(Track2Data) || !string.IsNullOrEmpty(Track3Data);
        }

        public bool HasValidMagEncodeJob {
            get => MagneticEncode && SelectedCoercivityType != null && HasMagData;
        }

        public bool HasValidJob {
            get => HasValidFrontSide || HasValidBackSide || HasValidMagEncodeJob;
        }
    }
}
