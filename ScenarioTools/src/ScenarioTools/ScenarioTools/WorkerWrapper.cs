using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ScenarioTools
{
    public enum WorkerStatus
    {
        Undefined = 0,
        Defined = 1,
        Working = 2,
        DoneWorking = 3
    }

    public class WorkerWrapper
    {
        #region Fields

        private WorkerStatus _status;
        private BackgroundWorker _worker;

        #endregion Fields

        /// <summary>
        /// Constructor -- Does not instantiate Worker member
        /// </summary>
        /// <returns></returns>
        public WorkerWrapper()
        {
            _status = WorkerStatus.Undefined;
        }

        #region Properties

        public BackgroundWorker Worker
        {
            get
            {
                return _worker;
            }
        }

        #endregion Properties

        #region Methods

        public bool Available()
        {
            if (_worker == null)
            {
                _status = WorkerStatus.Undefined;
            }
            else
            {
                if (_worker.IsBusy)
                {
                    _status = WorkerStatus.Working;
                }
            }
            if (_status == WorkerStatus.Undefined || _status == WorkerStatus.Defined || _status == WorkerStatus.DoneWorking)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InstantiateWorker()
        {
            if (Available())
            {
                if (_status == WorkerStatus.Defined || _status == WorkerStatus.DoneWorking)
                {
                    _worker.Dispose();
                    _worker = null;
                    _status = WorkerStatus.Undefined;
                }
                _worker = new BackgroundWorker();
                _status = WorkerStatus.Defined;
                _worker.WorkerReportsProgress = true;
                _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RunWorkerAsync(Object arg)
        {
            _status = WorkerStatus.Working;
            _worker.RunWorkerAsync(arg);
        }

        public void ReportProgress(int percentProgress)
        {
            _worker.ReportProgress(percentProgress);
        }


        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _status = WorkerStatus.DoneWorking;
        }


        #endregion Methods
    }
}
