using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using ScanCode.Interfaces;
using ScanCode.Services;

namespace ScanCode.ViewModel
{
    public class ViewModelLocator
    {
        private static bool _initialized = false;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IWorkflowReader, WorkflowReader>();
            SimpleIoc.Default.Register<IProjectEvaluator, Evaluator>();
            SimpleIoc.Default.Register<ILogger, LogService>();

            var messenger = Messenger.Default;
            SimpleIoc.Default.Register(() =>
            {
                var workflowReader = ServiceLocator.Current.GetInstance<IWorkflowReader>();
                var evaluator = ServiceLocator.Current.GetInstance<IProjectEvaluator>();
                var logger = ServiceLocator.Current.GetInstance<ILogger>();
                return new MainWindowViewModel(workflowReader, evaluator, logger, messenger);
            });
        }

        public MainWindowViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainWindowViewModel>(); }
        }
        
        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<MainWindowViewModel>();
        }
    }
}
