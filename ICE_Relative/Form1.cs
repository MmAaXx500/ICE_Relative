using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Xml;
using System.Threading;

namespace ICE_Relative
{
    public partial class Form1 : Form
    {

        public bool processing = false;
        public string[] FileList;

        public Form1()
        {
            InitializeComponent();

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(BackgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker1_ProgressChanged);
        }
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            FixTheFiles(worker, e);
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            processing = false;
            pictureBox1.Image = ICE_Relative.Properties.Resources.DragAndDrop;
        }

        void FixTheFiles(BackgroundWorker worker, DoWorkEventArgs e)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }

            List<SpjFiles> spjlist = new List<SpjFiles>();
            for (int i = 0; i < FileList.Length; i++)
            {
                if (FileList[i].Substring(FileList[i].Length - 4) == ".spj")
                {
                    spjlist.Add(new SpjFiles { Path = FileList[i] });
                }
            }

            int filesCount = spjlist.Count();
            int processedFilesCount = 0;
            foreach (var currentFilePath in spjlist) //Files
            {
                worker.ReportProgress((int)((float)processedFilesCount / (float)filesCount * 100));

                XmlDocument xml = new XmlDocument();
                xml.Load(currentFilePath.Path);

                XmlElement root = xml.DocumentElement;
                XmlNodeList imgs = root.SelectNodes("//stitchProject/sourceImages/sourceImage");

                foreach (XmlNode xmlPicPath in imgs) //Paths
                {
                    string xmlImgPath = xmlPicPath.Attributes["filePath"].Value;
                    xmlPicPath.Attributes["filePath"].Value = ".\\" + xmlImgPath.Substring(xmlImgPath.LastIndexOf('\\'));
                }

                xml.Save(currentFilePath.Path);

                processedFilesCount++;
            }
            worker.ReportProgress((int)((float)processedFilesCount / (float)filesCount * 100));
        }

        private void DDStart(object sender, DragEventArgs e)
        {
            if (processing == false)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void DDEnd(object sender, DragEventArgs e)
        {
            FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            backgroundWorker1.RunWorkerAsync();
            processing = true;

            e.Effect = DragDropEffects.None;
            pictureBox1.Image = ICE_Relative.Properties.Resources.DragAndDrop_Process;
        }

        private void LabelStartDD(object sender, DragEventArgs e)
        {
            if (processing == false)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void LabelEndDD(object sender, DragEventArgs e)
        {
            FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            backgroundWorker1.RunWorkerAsync();
            processing = true;

            e.Effect = DragDropEffects.None;
            pictureBox1.Image = ICE_Relative.Properties.Resources.DragAndDrop_Process;
        }
    }

    public class SpjFiles
    {
        public string Path { get; set; }
    }
}
