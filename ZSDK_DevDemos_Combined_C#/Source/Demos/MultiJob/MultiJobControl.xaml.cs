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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Card.Enumerations;

namespace DeveloperDemo_Card_Desktop.Demos.MultiJob {

    /// <summary>
    /// Interaction logic for MultiJobControl.xaml
    /// </summary>
    public partial class MultiJobControl : UserControl {

        private MultiJobControlViewModel viewModel;

        public MultiJobControlViewModel ViewModel {
            get => viewModel;
        }

        public MultiJobControl() {
            InitializeComponent();

            viewModel = DataContext as MultiJobControlViewModel;
        }

        private void BrowseFrontSideGraphicButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dialog = DialogHelper.CreateOpenImageFileDialog();
            if (dialog.ShowDialog() == true) {
                viewModel.FrontSideGraphicFilename = dialog.FileName;
            }
        }

        private void BrowseBackSideGraphicButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dialog = DialogHelper.CreateOpenImageFileDialog();
            if (dialog.ShowDialog() == true) {
                viewModel.BackSideGraphicFilename = dialog.FileName;
            }
        }

        private void AddFrontSideImageButton_Click(object sender, RoutedEventArgs e) {
            PrintType printType = viewModel.FrontSidePrintType;
            string graphicFilename = viewModel.FrontSideGraphicFilename;
            bool successfullyAdded = false;

            if (!Directory.Exists(graphicFilename) && File.Exists(graphicFilename)) {
                if (viewModel.FrontSideGraphicFilenames.ContainsKey(printType)) {
                    viewModel.FrontSideGraphicFilenames.Remove(printType);
                }

                viewModel.FrontSideGraphicFilenames.Add(printType, graphicFilename);
                successfullyAdded = true;
            }

            if (successfullyAdded) {
                MessageBox.Show($"{printType} image was added successfully.", "Front Side Graphic Added", MessageBoxButton.OK, MessageBoxImage.Information);
            } else {
                MessageBox.Show($"Unable to add {printType} image. Please check your file path and try again.", "Front Side Graphic Not Added", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddBackSideImageButton_Click(object sender, RoutedEventArgs e) {
            PrintType printType = viewModel.BackSidePrintType;
            string graphicFilename = viewModel.BackSideGraphicFilename;
            bool successfullyAdded = false;

            if (!Directory.Exists(graphicFilename) && File.Exists(graphicFilename)) {
                if (viewModel.BackSideGraphicFilenames.ContainsKey(printType)) {
                    viewModel.BackSideGraphicFilenames.Remove(printType);
                }

                viewModel.BackSideGraphicFilenames.Add(printType, graphicFilename);
                successfullyAdded = true;
            }

            if (successfullyAdded) {
                MessageBox.Show($"{printType} image was added successfully.", "Back Side Graphic Added", MessageBoxButton.OK, MessageBoxImage.Information);
            } else {
                MessageBox.Show($"Unable to add {printType} image. Please check your file path and try again.", "Back Side Graphic Not Added", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
