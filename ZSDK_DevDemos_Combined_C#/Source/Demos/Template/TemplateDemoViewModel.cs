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

namespace DeveloperDemo_Card_Desktop.Demos.Template {

    public class TemplateDemoViewModel : ViewModelBase {

        private string templateFilename;
        private string imageDirectory;
        private ObservableCollection<TemplateVariable> variableData = new ObservableCollection<TemplateVariable>();
        private ObservableCollection<TemplatePreview> templatePreviews = new ObservableCollection<TemplatePreview>();

        public string TemplateFilename {
            get => templateFilename;
            set {
                templateFilename = value;
                OnPropertyChanged();
            }
        }

        public string ImageDirectory {
            get => imageDirectory;
            set {
                imageDirectory = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TemplateVariable> VariableData {
            get => variableData;
        }

        public ObservableCollection<TemplatePreview> TemplatePreviews {
            get => templatePreviews;
        }
    }
}
