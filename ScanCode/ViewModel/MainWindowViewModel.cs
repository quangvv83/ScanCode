using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ScanCode.Data;
using ScanCode.Enums;
using ScanCode.Interfaces;
using ScanCode.Properties;
using ScanCode.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace ScanCode.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IWorkflowReader _workflowReader;
        private readonly IProjectEvaluator _projectEvaluator;
        private readonly ILogger _logger;

        public MainWindowViewModel(IWorkflowReader reader, IProjectEvaluator evaluator, ILogger logger, IMessenger messenger) : base(messenger)
        {
            _workflowReader = reader;
            _projectEvaluator = evaluator;
            _logger = logger;
        }

        #region Binding properties

        private string _scannedTime;
        public string ScannedTime
        {
            get => _scannedTime;
            set
            {
                if (_scannedTime == value)
                {
                    return;
                }

                _scannedTime = value;
                RaisePropertyChanged(nameof(ScannedTime));
            }
        }

        private string _effortRange;
        public string EffortRange
        {
            get => _effortRange;
            set
            {
                if (_effortRange == value)
                {
                    return;
                }

                _effortRange = value;
                RaisePropertyChanged(nameof(EffortRange));
            }
        }

        private ViewStatus _screenStatus = ViewStatus.NewScan;
        public ViewStatus ScreenStatus
        {
            get => _screenStatus;
            set
            {
                if (_screenStatus == value)
                {
                    return;
                }

                _screenStatus = value;
                RaisePropertyChanged(nameof(ScreenStatus));

                if (ScreenStatus == ViewStatus.ProjectData)
                {
                    WindowBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F9F9F9"));
                }
                else
                {
                    WindowBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                }
            }
        }

        private bool _isCalculating;
        public bool IsCalculating
        {
            get => _isCalculating;
            set
            {
                if (_isCalculating == value)
                {
                    return;
                }

                _isCalculating = value;
                RaisePropertyChanged(nameof(IsCalculating));
            }
        }

        private int _totalActivities;
        public int TotalActivities
        {
            get => _totalActivities;
            set
            {
                if (_totalActivities == value)
                {
                    return;
                }

                _totalActivities = value;
                RaisePropertyChanged(nameof(TotalActivities));
            }
        }

        private int _totalWorkflows;
        public int TotalWorkflows
        {
            get => _totalWorkflows;
            set
            {
                if (_totalWorkflows == value)
                {
                    return;
                }

                _totalWorkflows = value;
                RaisePropertyChanged(nameof(TotalWorkflows));
            }
        }

        private bool _scanListEmpty;
        public bool ScanListEmpty
        {
            get => _scanListEmpty;
            set
            {
                if (_scanListEmpty == value)
                {
                    return;
                }

                _scanListEmpty = value;
                RaisePropertyChanged(nameof(ScanListEmpty));
            }
        }

        private string _addButtonTitle;
        public string AddButtonTitle
        {
            get => _addButtonTitle;
            set
            {
                if (_addButtonTitle == value)
                {
                    return;
                }

                _addButtonTitle = value;
                RaisePropertyChanged(nameof(AddButtonTitle));
            }
        }

        private string _logFolder;
        public string LogFolder
        {
            get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.LogPath);
            set
            {
                if (_logFolder == value)
                {
                    return;
                }

                _logFolder = value;
                RaisePropertyChanged(nameof(LogFolder));
            }
        }

        private string _droppedFilePath;
        public string DroppedFilePath
        {
            get => _droppedFilePath;
            set
            {
                if (_droppedFilePath == value)
                {
                    return;
                }

                _droppedFilePath = value;
                RaisePropertyChanged(nameof(DroppedFilePath));

                // dropped => add to scan list
                ReadProjectInfo(DroppedFilePath);
            }
        }

        private string _loadingScreenText;
        public string LoadingScreenText
        {
            get => _loadingScreenText;
            set
            {
                if (_loadingScreenText == value)
                {
                    return;
                }

                _loadingScreenText = value;
                RaisePropertyChanged(nameof(LoadingScreenText));
            }
        }

        private SolidColorBrush _windowBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        public SolidColorBrush WindowBackgroundColor
        {
            get => _windowBackgroundColor;
            set
            {
                if (_windowBackgroundColor == value)
                {
                    return;
                }

                _windowBackgroundColor = value;
                RaisePropertyChanged(nameof(WindowBackgroundColor));
            }
        }

        private ObservableCollection<ProjectData> _projectList = new ObservableCollection<ProjectData>();
        public ObservableCollection<ProjectData> ProjectList
        {
            get => _projectList;
            set
            {
                Set(() => ProjectList, ref _projectList, value);
            }
        }

        private ObservableCollection<ProjectInfo> _scanList = new ObservableCollection<ProjectInfo>();
        public ObservableCollection<ProjectInfo> ScanList
        {
            get => _scanList;
            set
            {
                Set(() => ScanList, ref _scanList, value);
            }
        }

        private ObservableCollection<ProjectInfo> _errorList = new ObservableCollection<ProjectInfo>();
        public ObservableCollection<ProjectInfo> ErrorList
        {
            get => _errorList;
            set
            {
                Set(() => ErrorList, ref _errorList, value);
            }
        }

        #endregion

        #region Binding commands

        private RelayCommand _chooseProjectCommand;
        public RelayCommand ChooseProjectCommand
        {
            get
            {
                if (_chooseProjectCommand == null)
                {
                    _chooseProjectCommand = new RelayCommand(() =>
                    {
                        try
                        {
                            // choose project
                            ChooseProject();
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                            _logger.Error(this, ex.Message, ex);
                            MessageBox.Show(Application.Current.MainWindow, ex.Message, Resources.Error_Title, MessageBoxButton.OK);
                        }
                    });
                }
                return _chooseProjectCommand;
            }
        }

        private RelayCommand _addMoreCommand;
        public RelayCommand AddMoreCommand
        {
            get
            {
                if (_addMoreCommand == null)
                {
                    _addMoreCommand = new RelayCommand(() =>
                    {
                        try
                        {
                            // Add project
                            ChooseProject();
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                            _logger.Error(this, ex.Message, ex);
                            MessageBox.Show(Application.Current.MainWindow, ex.Message, Resources.Error_Title, MessageBoxButton.OK);
                        }
                    });
                }
                return _addMoreCommand;
            }
        }

        private RelayCommand<ProjectInfo> _removeProjectCommand;
        public RelayCommand<ProjectInfo> RemoveProjectCommand
        {
            get
            {
                if (_removeProjectCommand == null)
                {
                    _removeProjectCommand = new RelayCommand<ProjectInfo>((ProjectInfo pi) =>
                    {
                        try
                        {
                            // Remove project
                            ScanList.Remove(pi);
                            RefreshScanList(ScanList);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                            _logger.Error(this, ex.Message, ex);
                            MessageBox.Show(Application.Current.MainWindow, ex.Message, Resources.Error_Title, MessageBoxButton.OK);
                        }
                    });
                }
                return _removeProjectCommand;
            }
        }

        private RelayCommand _newScanCommand;
        public RelayCommand NewScanCommand
        {
            get
            {
                if (_newScanCommand == null)
                {
                    _newScanCommand = new RelayCommand(() =>
                    {
                        try
                        {
                            ScanList.Clear();
                            ScreenStatus = ViewStatus.NewScan;
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                            _logger.Error(this, ex.Message, ex);
                            MessageBox.Show(Application.Current.MainWindow, ex.Message, Resources.Error_Title, MessageBoxButton.OK);
                        }
                    },
                    () => { return ScreenStatus != ViewStatus.NewScan; });
                }
                return _newScanCommand;
            }
        }

        private RelayCommand _startScanCommand;
        public RelayCommand StartScanCommand
        {
            get
            {
                if (_startScanCommand == null)
                {
                    _startScanCommand = new RelayCommand(() =>
                    {
                        if (ScanListEmpty)
                        {
                            return;
                        }

                        ProjectList.Clear();
                        ErrorList.Clear();
                        IsCalculating = true;
                        LoadingScreenText = Resources.AnalyzingProject_Label;
                        ScannedTime = DateTime.Now.ToString(Constants.ScanTimeFormat);
                        bool anyError = false;

                        Task.Factory.StartNew(() =>
                        {
                            foreach (var item in ScanList)
                            {
                                try
                                {
                                    // scan project
                                    _workflowReader.Scan(Path.GetDirectoryName(item.Location));

                                    // calculate project point
                                    ProjectData projectData = _projectEvaluator.CalculateProjectPoint(_workflowReader.ScanResult, new ComplexityParam
                                    {
                                        ConditionNumber = _workflowReader.ConditionNumber,
                                        WorkflowNumber = _workflowReader.Workflows.Count,
                                    });
                                    projectData.ProjectName = _workflowReader.ProjectName;
                                    projectData.ScanResult = new List<ActivityRawData>(_workflowReader.ScanResult);

                                    Application.Current.Dispatcher.Invoke(() => ProjectList.Add(projectData));
                                    projectData.PropertyChanged += (sender, e) =>
                                    {
                                        RefreshProjectSummary(ProjectList);
                                    };
                                }
                                catch (Exception e)
                                {
                                    anyError = true;
                                    Application.Current.Dispatcher.Invoke(() => ErrorList.Add(item));
                                    Trace.TraceError(e.ToString());

                                    if (e.InnerException == null)
                                    {
                                        _logger.Error(this, e.Message, e);
                                    }
                                    else
                                    {
                                        _logger.Error(this, e.Message, e.InnerException);
                                    }
                                }
                            }
                        }).ContinueWith((task) =>
                        {
                            // project's summary
                            RefreshProjectSummary(ProjectList);

                            if (anyError)
                            {
                                ScreenStatus = ViewStatus.ScanError;
                            }
                            else
                            {
                                ScreenStatus = ViewStatus.ProjectData;
                            }
                            IsCalculating = false;

                            if (task.IsFaulted)
                            {
                                Trace.TraceError(task.Exception?.InnerException?.ToString());
                                _logger.Error(this, task.Exception?.InnerException?.Message, task.Exception?.InnerException);
                                MessageBox.Show(Application.Current.MainWindow, task.Exception?.InnerException?.Message, Resources.Error_Title, MessageBoxButton.OK);
                            }
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                    },
                    () => { return true; });
                }
                return _startScanCommand;
            }
        }

        private RelayCommand _tryAgainCommand;
        public RelayCommand TryAgainCommand
        {
            get
            {
                if (_tryAgainCommand == null)
                {
                    _tryAgainCommand = new RelayCommand(() =>
                    {
                        try
                        {
                            ScreenStatus = ViewStatus.ScanList;
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                            _logger.Error(this, ex.Message, ex);
                            MessageBox.Show(Application.Current.MainWindow, ex.Message, Resources.Error_Title, MessageBoxButton.OK);
                        }
                    });
                }
                return _tryAgainCommand;
            }
        }

        private RelayCommand<string> _openFolderCommand;
        public RelayCommand<string> OpenFolderCommand
        {
            get
            {
                if (_openFolderCommand == null)
                {
                    _openFolderCommand = new RelayCommand<string>((folderPath) =>
                    {
                        try
                        {
                            Process.Start(folderPath);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                            _logger.Error(this, ex.Message, ex);
                            MessageBox.Show(Application.Current.MainWindow, ex.Message, Resources.Error_Title, MessageBoxButton.OK);
                        }
                    });
                }
                return _openFolderCommand;
            }
        }

        #endregion

        #region private functions

        private void ChooseProject()
        {
            var browserDialog = FileDialogHelper.OpenFileDialog(Constants.ProjectFilter);
            bool? flag = browserDialog.ShowDialog();
            if (flag == null || !flag.Value)
            {
                return;
            }

            // add project to scan list
            ReadProjectInfo(browserDialog.FileName);
        }

        private void ReadProjectInfo(string projectPath)
        {
            if (!File.Exists(projectPath) || Path.GetFileName(projectPath) != Constants.ProjectFileName)
            {
                MessageBox.Show(Resources.ProjectFile_Alert, Resources.Error_Title, MessageBoxButton.OK);
                return;
            }

            // add project to scan list
            var projectInfo = new ProjectInfo
            {
                Location = projectPath,
                Name = _workflowReader.ReadProjectInfo(projectPath).Name,
            };
            if (!ScanList.Contains(projectInfo))
            {
                ScanList.Add(projectInfo);
                RefreshScanList(ScanList);
            }
            ScreenStatus = ViewStatus.ScanList;
        }

        private void RefreshScanList(IList<ProjectInfo> list)
        {
            foreach (var item in list)
            {
                item.Index = list.IndexOf(item) + 1;
            }
            if (ScanList.Any())
            {
                ScanListEmpty = false;
                AddButtonTitle = Resources.AddMore_Label;
            }
            else
            {
                ScanListEmpty = true;
                AddButtonTitle = Resources.AddProject_Label;
            }
        }

        private void RefreshProjectSummary(ObservableCollection<ProjectData> projectList)
        {
            EffortRange = _projectEvaluator.GetProjectLevel(projectList.Where(p => p.Enabled).Sum(p => p.TotalEffort)).EffortRange;
            TotalActivities = projectList.Where(p => p.Enabled).Sum(p => p.TotalActivities);
            TotalWorkflows = projectList.Where(p => p.Enabled).Sum(p => p.TotalWorkflows);
        }

        #endregion

    }
}