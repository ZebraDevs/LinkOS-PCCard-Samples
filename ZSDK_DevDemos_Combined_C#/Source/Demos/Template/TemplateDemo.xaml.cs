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

using DeveloperDemo_Card_Desktop.Dialogs;
using DeveloperDemo_Card_Desktop.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Card.Containers;
using Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Card.Job.Template;
using static DeveloperDemo_Card_Desktop.UserControls.JobStatusControl;

namespace DeveloperDemo_Card_Desktop.Demos.Template {

    /// <summary>
    /// Interaction logic for TemplateDemo.xaml
    /// </summary>
    public partial class TemplateDemo : UserControl {

        private static readonly List<string> SupportedImageExtensions = new List<string> { ".bmp", ".jpg", ".jpeg", ".png" };

        private TemplateDemoViewModel viewModel;
        private PrinterManager printerManager;
        private ZebraCardTemplate zebraCardTemplate = new ZebraCardTemplate(null);

        public TemplateDemo(PrinterManager printerManager) {
            InitializeComponent();

            viewModel = DataContext as TemplateDemoViewModel;
            this.printerManager = printerManager;

            TabControl.SelectionChanged += TabControl_SelectionChanged;
        }

        private void SaveTemplateImages(string imageDirectory) {
            List<string> existingTemplateImageFiles = zebraCardTemplate.GetTemplateImageNames();

            List<FileInfo> imageFileInfoList = new DirectoryInfo(imageDirectory).EnumerateFiles().Where(f => SupportedImageExtensions.Contains(f.Extension.ToLower())).ToList();

            foreach (FileInfo imageFileInfo in imageFileInfoList) {
                try {
                    if (existingTemplateImageFiles.Contains(imageFileInfo.Name)) {
                        zebraCardTemplate.DeleteTemplateImage(imageFileInfo.Name);
                    }

                    byte[] templateImageData = File.ReadAllBytes(imageFileInfo.FullName);
                    zebraCardTemplate.SaveTemplateImage(imageFileInfo.Name, templateImageData);

                    Application.Current.Dispatcher.Invoke(() => {
                        JobStatusControl.UpdateLog($"Saving image file {imageFileInfo.Name}...");
                    });
                } catch (Exception e) {
                    string errorMessage = $"Error saving template image file {imageFileInfo.Name}: {e.Message}";
                    JobStatusControl.UpdateLog(errorMessage);
                    MessageBoxHelper.ShowError(errorMessage);
                }
            }
        }

        private TemplateJob CreateTemplateJob() {
            Dictionary<string, string> fieldDataMap = new Dictionary<string, string>();
            foreach (TemplateVariable templateVariable in viewModel.VariableData) {
                if (!string.IsNullOrEmpty(templateVariable.Value)) {
                    fieldDataMap.Add(templateVariable.Variable, templateVariable.Value);
                }
            }

            string templateName = Path.GetFileNameWithoutExtension(viewModel.TemplateFilename);
            return zebraCardTemplate.GenerateTemplateJob(templateName, fieldDataMap);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (PreviewTab.IsSelected) {
                viewModel.TemplatePreviews.Clear();

                IndeterminateProgressDialog indeterminateProgressDialog = new IndeterminateProgressDialog("Refreshing preview...");

                Task.Run(() => {
                    Application.Current.Dispatcher.Invoke(() => {
                        indeterminateProgressDialog.ShowDialog();
                    });
                });

                Task.Run(() => {
                    try {
                        if (!Path.IsPathRooted(viewModel.TemplateFilename)) {
                            throw new ArgumentException("Template filename must be an absolute path");
                        }

                        if (viewModel.ImageDirectory != null && !Path.IsPathRooted(viewModel.ImageDirectory)) {
                            throw new ArgumentException("Image directory must be an absolute path");
                        }

                        List<GraphicsInfo> graphicsData = CreateTemplateJob().GraphicsData;
                        foreach (GraphicsInfo graphicsInfo in graphicsData) {
                            Application.Current.Dispatcher.Invoke(() => {
                                viewModel.TemplatePreviews.Add(new TemplatePreview {
                                    Label = $"{graphicsInfo.Side} ({graphicsInfo.PrintType})",
                                    ImageData = graphicsInfo.GraphicData?.ImageData
                                });
                            });
                        }
                    } catch (Exception exception) {
                        MessageBoxHelper.ShowError($"Error getting template preview: {exception.Message}");

                        Application.Current.Dispatcher.Invoke(() => {
                            TabControl.SelectedItem = TemplateTab;
                        });
                    } finally {
                        Application.Current.Dispatcher.Invoke(() => {
                            indeterminateProgressDialog.Close();
                        });
                    }
                });
            }
        }

        private async void PrintButton_Click(object sender, RoutedEventArgs e) {
            int? jobId = null;
            CardSource? cardSource = null;

            JobStatusControl.ClearLog();

            await printerManager.PerformAction("Sending print job to printer...", (zebraCardPrinter, connection) => {
                if (printerManager.IsPrinterReady(zebraCardPrinter, JobStatusControl)) {
                    TemplateJob templateJob = CreateTemplateJob();
                    if (templateJob.JobInfo.CardDestination.HasValue) {
                        if (templateJob.JobInfo.CardDestination.Value == CardDestination.Eject && zebraCardPrinter.HasLaminator()) {
                            templateJob.JobInfo.CardDestination = CardDestination.LaminatorAny;
                        }
                    }

                    cardSource = templateJob.JobInfo.CardSource;
                    jobId = zebraCardPrinter.PrintTemplate(1, templateJob);
                }
            }, (exception) => {
                string errorMessage = $"Error printing card: {exception.Message}";
                MessageBoxHelper.ShowError(errorMessage);
                JobStatusControl.UpdateLog(errorMessage);
            });

            if (jobId.HasValue && cardSource.HasValue) {
                await JobStatusControl.StartPolling(printerManager.Printer, new JobInfo(jobId.Value, cardSource.Value));
            }
        }

        private void BrowseTemplateButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dialog = DialogHelper.CreateOpenTemplateFileDialog();
            if (dialog.ShowDialog() == true) {
                try {
                    viewModel.TemplateFilename = dialog.FileName;
                    string templateName = Path.GetFileNameWithoutExtension(dialog.FileName);

                    if (zebraCardTemplate.GetTemplateFileNames().Contains(templateName)) {
                        zebraCardTemplate.DeleteTemplateFileData(templateName);
                    }
                    zebraCardTemplate.SaveTemplateFileData(templateName, File.ReadAllText(dialog.FileName));

                    viewModel.VariableData.Clear();
                    foreach (string templateField in zebraCardTemplate.GetTemplateFields(templateName)) {
                        viewModel.VariableData.Add(new TemplateVariable {
                            Variable = templateField
                        });
                    }

                    SaveTemplateImages(new FileInfo(dialog.FileName).Directory.FullName);
                } catch (Exception exception) {
                    viewModel.TemplateFilename = "";
                    string errorMessage = "Error reading selected template"; 
                    if(!string.IsNullOrEmpty(exception.Message)) {
                        errorMessage += ": " + exception.Message;
                    }
                    MessageBoxHelper.ShowError(errorMessage);
                }
            }
        }

        private void BrowseImageDirectoryButton_Click(object sender, RoutedEventArgs e) {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog {
                ShowNewFolderButton = false,
                Description = "Please select the directory where your template image files are stored."
            }) {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    viewModel.ImageDirectory = dialog.SelectedPath;
                    SaveTemplateImages(dialog.SelectedPath);
                }
            }
        }
    }
}
