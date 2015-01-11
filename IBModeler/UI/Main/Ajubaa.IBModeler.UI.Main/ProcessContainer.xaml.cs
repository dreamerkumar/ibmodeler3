using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Ajubaa.Common;
using Ajubaa.IBModeler.Processor;

namespace Ajubaa.IBModeler.UI.Main
{
    internal delegate void ProcessedEventHandler(object sender, ProcessedEventArgs args);
    internal delegate void SuccessfullyCanceledEventHandler(object sender, EventArgs args);

    /// <summary>
    /// Interaction logic for ProcessContainer.xaml
    /// </summary>
    public partial class ProcessContainer
    {
        internal event ProcessedEventHandler ProcessedEvent;
        internal event SuccessfullyCanceledEventHandler SuccessfullyCancelled;

        private static BackgroundWorker _bw;
        const string ProcessingMessage = "Processing {0} of {1} images....";
        private int _totalImageCount;
        private CreateMeshContract _contract;
        
        public ProcessContainer(CreateMeshContract contract)
        {
            InitializeComponent();

            InitiateModelCreation(contract);
        }

        private void InitiateModelCreation(CreateMeshContract contract)
        {
            _contract = contract;
            _totalImageCount = contract.ClickInputs.Angles.Length;

            //set the carousel display
            //_imageCarousel.ImagePaths = new List<string>();
            //var angles = contract.ClickInputs.Angles;
            //for (var index = 0; index < angles.Length; index++)
            //{
            //    //var clickInput = contract.ClickInputs.ImageClickInputDetailsList[index];
            //    //var imagePath = string.Format(@"{0}\{1}", contract.ImageFolder, clickInput.ImageName);
            //    //_imageCarousel.ImagePaths.Add(imagePath);
            //}
            
            _bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _bw.DoWork += BwDoWork;
            _bw.ProgressChanged += BwProgressChanged;
            _bw.RunWorkerCompleted += BwRunWorkerCompleted;
            _bw.WorkerSupportsCancellation = true;
            
            _bw.RunWorkerAsync(contract);
            
            ProcessedImageCountDisplay.Text = string.Format(ProcessingMessage, 1, _totalImageCount);
        }

        private static void BwDoWork(object sender, DoWorkEventArgs e)
        {
            var contract = (CreateMeshContract)e.Argument;
            var logger = new Logger(contract.LogFilePath);

            var angles = contract.ClickInputs.Angles;

            if (CheckCancellation(e))
                return;

            contract.PercentExtraWidth = MainProcessor.GetExtraPadding(contract);
            
            if (CheckCancellation(e))
                return;

            Stream moldDataPtr = null;
            for (var index = 0; index < angles.Length; index++)
            {
                var angleInDegrees = angles[index]*180.0/Math.PI;
                if (angleInDegrees <= contract.MaxAngleOfImageToProcessInDegress)
                {
                    CreateMeshProcessor.ApplyImage(contract, index, ref moldDataPtr, logger);
                    if (CheckCancellation(e))
                        return;
                    _bw.ReportProgress(index);
                }
            }

            if (CheckCancellation(e))
                return;

            _bw.ReportProgress(angles.Length + 1);
            e.Result = CreateMeshProcessor.CreateModelFromMold(contract.PtDensity, moldDataPtr, logger);
            
            //clean up
            if (moldDataPtr != null) moldDataPtr.Dispose();
            if(MainProcessor.FirstImageParams != null) MainProcessor.FirstImageParams.Dispose();
        }

        private static bool CheckCancellation(DoWorkEventArgs e)
        {
            if (_bw.CancellationPending)
            {
                e.Cancel = true;
                return true;
            }
            return false;
        }

        private void BwProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //if (e.ProgressPercentage + 1 < _totalImageCount)
                //_imageCarousel.SetSelectedIndex(e.ProgressPercentage + 1);

            if (e.ProgressPercentage + 1 >= _totalImageCount)
            {
                ProcessedImageCountDisplay.Text = "Creating model....";
            }
            else
                ProcessedImageCountDisplay.Text = string.Format(ProcessingMessage, e.ProgressPercentage + 2, _totalImageCount);
        }

        private void BwRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                InvokeSuccessfullyCancelled(new EventArgs());
                return;
            }
            var modelMeshAndPositionNeighbors = (ModelMeshAndPositionNeighbors)e.Result;
            var args = new ProcessedEventArgs { ModelMeshAndPositionNeighbors = modelMeshAndPositionNeighbors, CreateMeshContract = _contract };
            InvokeProcessedEvent(args);
        }

        internal void InvokeProcessedEvent(ProcessedEventArgs args)
        {
            var handler = ProcessedEvent;
            if (handler != null) handler(this, args);
        }

        internal void InvokeSuccessfullyCancelled(EventArgs args)
        {
            var handler = SuccessfullyCancelled;
            if (handler != null) handler(this, args);
        }

        public void Cancel()
        {
            if(_bw.IsBusy)
                _bw.CancelAsync();
        }
    }
}
