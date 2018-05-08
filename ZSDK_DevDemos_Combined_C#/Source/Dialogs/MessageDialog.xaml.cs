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

using System.Windows;

namespace DeveloperDemo_Card_Desktop.Dialogs {

    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window {

        private MessageDialogViewModel viewModel;

        public delegate void PositiveButtonClickedHandler();
        public delegate void NegativeButtonClickedHandler();

        public event PositiveButtonClickedHandler PositiveButtonClicked;
        public event NegativeButtonClickedHandler NegativeButtonClicked;

        public string Message {
            get => viewModel.Message;
            set {
                viewModel.Message = value;
            }
        }

        public string PositiveButtonText {
            get => viewModel.PositiveButtonText;
            set {
                viewModel.PositiveButtonText = value;
            }
        }

        public string NegativeButtonText {
            get => viewModel.NegativeButtonText;
            set {
                viewModel.NegativeButtonText = value;
            }
        }

        public bool HasNegativeButton {
            get => viewModel.HasNegativeButton;
            set {
                viewModel.HasNegativeButton = value;
            }
        }

        public MessageDialog() {
            InitializeComponent();

            viewModel = DataContext as MessageDialogViewModel;
        }

        private void NegativeButton_Click(object sender, RoutedEventArgs e) {
            if (NegativeButtonClicked != null) {
                NegativeButtonClicked.Invoke();
            }
            Close();
        }

        private void PositiveButton_Click(object sender, RoutedEventArgs e) {
            if (PositiveButtonClicked != null) {
                PositiveButtonClicked.Invoke();
            }
            Close();
        }
    }
}
