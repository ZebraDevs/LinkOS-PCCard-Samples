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

namespace DeveloperDemo_Card_Desktop.Demos.MultiJob {

    public class MultiJobDemoViewModel : ViewModelBase {

        private bool isPrintOptimizationAvailable;
        private bool hasDualSidedPrintCapability;
        private bool hasMagneticEncoder;
        private ObservableCollection<string> sources = new ObservableCollection<string>();
        private ObservableCollection<string> destinations = new ObservableCollection<string>();
        private ObservableCollection<string> printOptimizations = new ObservableCollection<string>();
        private ObservableCollection<int> quantities = new ObservableCollection<int>();
        private ObservableCollection<PrintType> printTypes = new ObservableCollection<PrintType>();
        private ObservableCollection<string> coercivityTypes = new ObservableCollection<string>();
        private string jobStatus1 = "";
        private string jobStatus2 = "";
        private string jobStatus3 = "";
        private string jobStatus4 = "";
        private string cancellableJobId = null;
        private bool isSendJobsButtonEnabled = true;

        public MultiJobDemoViewModel() {
            quantities.Add(1);
            quantities.Add(2);
            quantities.Add(3);
            quantities.Add(4);
            quantities.Add(5);
        }

        public bool IsPrintOptimizationAvailable {
            get => isPrintOptimizationAvailable;
            set {
                isPrintOptimizationAvailable = value;
                OnPropertyChanged();
            }
        }

        public bool HasDualSidedPrintCapability {
            get => hasDualSidedPrintCapability;
            set {
                hasDualSidedPrintCapability = value;
                OnPropertyChanged();
            }
        }

        public bool HasMagneticEncoder {
            get => hasMagneticEncoder;
            set {
                hasMagneticEncoder = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Sources {
            get => sources;
        }

        public ObservableCollection<string> Destinations {
            get => destinations;
        }

        public ObservableCollection<string> PrintOptimizations {
            get => printOptimizations;
        }

        public ObservableCollection<int> Quantities {
            get => quantities;
        }

        public ObservableCollection<PrintType> PrintTypes {
            get => printTypes;
        }

        public ObservableCollection<string> CoercivityTypes {
            get => coercivityTypes;
        }

        public string JobStatus1 {
            get => jobStatus1;
            set {
                jobStatus1 = value;
                OnPropertyChanged();
            }
        }

        public string JobStatus2 {
            get => jobStatus2;
            set {
                jobStatus2 = value;
                OnPropertyChanged();
            }
        }

        public string JobStatus3 {
            get => jobStatus3;
            set {
                jobStatus3 = value;
                OnPropertyChanged();
            }
        }

        public string JobStatus4 {
            get => jobStatus4;
            set {
                jobStatus4 = value;
                OnPropertyChanged();
            }
        }

        public string CancellableJobId {
            get => cancellableJobId;
            set {
                cancellableJobId = value;
                OnPropertyChanged();
            }
        }

        public bool IsSendJobsButtonEnabled {
            get => isSendJobsButtonEnabled;
            set {
                isSendJobsButtonEnabled = value;
                OnPropertyChanged();
            }
        }
    }
}
