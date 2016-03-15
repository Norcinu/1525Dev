using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        MainWindow mainWindow = new MainWindow();
        public LoadingWindow()
        {
            InitializeComponent();
        }
        
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            
            worker.RunWorkerAsync();
        }
        
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (mainWindow.FullyLoaded)
            {
                try
                {
                    this.Dispatcher.Invoke((DelegateWindow)ShowMainWindow, new object[] { mainWindow, this });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                Thread.Sleep(100);
            }
        }
        
        public delegate void DelegateWindow(Window window, Window current);
        public void ShowMainWindow(Window window, Window current)
        {
            current.Close();
            window.Show();
        }
        
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PbStatus.Value = e.ProgressPercentage;
        }
    }
}
