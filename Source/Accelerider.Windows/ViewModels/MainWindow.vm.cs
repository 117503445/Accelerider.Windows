﻿using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Accelerider.Windows.Assets;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Common;
using Accelerider.Windows.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TransferingTaskList _downloadList;
        private TransferingTaskList _uploadList;
        private ICommand _feedbackCommand;


        public MainWindowViewModel(IUnityContainer container) : base(container)
        {
            FeedbackCommand = new RelayCommand(() => Process.Start(ConstStrings.IssueUrl));

            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);

            ConfigureTransferList();
        }

        public ICommand FeedbackCommand
        {
            get => _feedbackCommand;
            set => SetProperty(ref _feedbackCommand, value);
        }

        public TransferingTaskList DownloadList
        {
            get => _downloadList;
            set => SetProperty(ref _downloadList, value);
        }

        public TransferingTaskList UploadList
        {
            get => _uploadList;
            set => SetProperty(ref _uploadList, value);
        }


        public override void OnUnloaded(object view)
        {
            AcceleriderUser.OnExit();
        }


        private void ConfigureTransferList()
        {
            DownloadList = new TransferingTaskList(AcceleriderUser.GetDownloadingTasks().Select(task => new TransferingTaskViewModel(task)));
            UploadList = new TransferingTaskList(AcceleriderUser.GetUploadingTasks().Select(task => new TransferingTaskViewModel(task)));

            DownloadList.TransferedFileList = new TransferedFileList(AcceleriderUser.GetDownloadedFiles());
            UploadList.TransferedFileList = new TransferedFileList(AcceleriderUser.GetUploadedFiles());

            Container.RegisterInstance(TransferingTaskList.DownloadKey, DownloadList);
            Container.RegisterInstance(TransferingTaskList.UploadKey, UploadList);
        }
    }
}
