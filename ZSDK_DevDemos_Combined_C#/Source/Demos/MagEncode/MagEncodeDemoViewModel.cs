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

namespace DeveloperDemo_Card_Desktop.Demos.MagEncode {

    public class MagEncodeDemoViewModel : ViewModelBase {

        private MagEncodeJobType selectedJobType;
        private string selectedSource;
        private string selectedDestination;
        private string selectedCoercivityType;
        private ObservableCollection<MagEncodeJobType> jobTypes = new ObservableCollection<MagEncodeJobType>();
        private ObservableCollection<string> sources = new ObservableCollection<string>();
        private ObservableCollection<string> destinations = new ObservableCollection<string>();
        private ObservableCollection<string> coercivityTypes = new ObservableCollection<string>();
        private string track1Data = "";
        private string track2Data = "";
        private string track3Data = "";
        private bool verifyEncoding = true;

        public MagEncodeDemoViewModel() {
            jobTypes.Add(MagEncodeJobType.Read);
            jobTypes.Add(MagEncodeJobType.Write);

            selectedJobType = jobTypes[0];
        }

        public MagEncodeJobType SelectedJobType {
            get => selectedJobType;
            set {
                selectedJobType = value;
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

        public string SelectedCoercivityType {
            get => selectedCoercivityType;
            set {
                selectedCoercivityType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MagEncodeJobType> JobTypes {
            get => jobTypes;
        }

        public ObservableCollection<string> Sources {
            get => sources;
        }

        public ObservableCollection<string> Destinations {
            get => destinations;
        }

        public ObservableCollection<string> CoercivityTypes {
            get => coercivityTypes;
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

        public bool VerifyEncoding {
            get => verifyEncoding;
            set {
                verifyEncoding = value;
                OnPropertyChanged();
            }
        }
    }
}
