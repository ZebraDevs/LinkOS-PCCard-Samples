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

namespace DeveloperDemo_Card_Desktop.Demos.SmartCard {

    public class SmartCardDemoViewModel : ViewModelBase {

        private string selectedSource;
        private string selectedDestination;
        private string selectedEncoderType;

        private ObservableCollection<string> sources = new ObservableCollection<string>();
        private ObservableCollection<string> destinations = new ObservableCollection<string>();
        private ObservableCollection<string> encoderTypes = new ObservableCollection<string>();

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

        public string SelectedEncoderType {
            get => selectedEncoderType;
            set {
                selectedEncoderType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Sources {
            get => sources;
        }

        public ObservableCollection<string> Destinations {
            get => destinations;
        }

        public ObservableCollection<string> EncoderTypes {
            get => encoderTypes;
        }
    }
}
