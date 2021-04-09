using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using AForge.Video;
using System.Diagnostics;
using AForge.Video.DirectShow;
using System.Collections;
using System.IO;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Globalization;
using System.Net;
using AForge.Controls;
using System.Timers;

namespace Camera_Record
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCapabilities[] snapshotCapabilities;
        private List<VideoCaptureDevice> opened_videoDevices = new List<VideoCaptureDevice>();
        private List<VideoSourcePlayer> opened_videoPlayers = new List<VideoSourcePlayer>();
        private ArrayList listCamera = new ArrayList();
        public string pathFolder = Application.StartupPath + @"\ImageCapture\";
        private Stopwatch stopWatch = null;
        private static bool needSnapshot = false;
        private BackgroundWorker bw = new BackgroundWorker();

        private static string _usbcamera;

        private int MININUM_DEVICES_CNT;
        private int cameraProcessTime = 0;
        private int maxCameraProcessTime = 3 * 60; //Close camera when opening after 3 hours
        public string usbcamera
        {
            get { return _usbcamera; }
            set { _usbcamera = value; }
        }

        string iniPath = @"Config.ini";
        string FTP_IP = string.Empty;
        string FTP_Port = string.Empty;
        string FTP_Account = string.Empty;
        string FTP_Password = string.Empty;
        string OPID = string.Empty;
        string SN = string.Empty;
        string nameCapture = string.Empty;

        int imageHeight;
        int imageWidth;


        Tools_CMD tools_CMD = new Tools_CMD();



        public Form1()
        {
            InitializeComponent();
            //getListCameraUSB();
            Disabled_Mode();
        }

        private void StartTimer()
        {
            // Call this procedure when the application starts.
            // Set to 1 second.
            timer1.Interval = 1000;

            // Enable timer.
            this.timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenCamera();
            Enabled_Mode();
            StartTimer();
        }

        #region Open Scan Camera
        private void OpenCamera()
        {
            try
            {
                usbcamera = listBox1.SelectedIndex.ToString();
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == MININUM_DEVICES_CNT)
                {
                    int loopCnt = 1;
                    // add all devices to combo
                    foreach (FilterInfo device in videoDevices)
                    {
                        
                        listCamera.Add(device.Name);
                        VideoCaptureDevice v = new VideoCaptureDevice(device.MonikerString);
                        opened_videoDevices.Add(v);

                        snapshotCapabilities = v.SnapshotCapabilities;
                        if (snapshotCapabilities.Length == 0)
                        {
                            MessageBox.Show("Camera Capture Not supported");
                        }

                        v.SnapshotResolution = v.VideoCapabilities[0]; //It selects the default size
                        int a = v.VideoCapabilities.Length;
                        for (int i = 0; i < v.VideoCapabilities.Length; i++)
                        {

                            string _reSize = string.Empty;
                            string resolution_size = v.VideoCapabilities[i].FrameSize.ToString();
                            string[] sArray = resolution_size.Split(new string[] {",","{","}" } , StringSplitOptions.RemoveEmptyEntries);
                            for (int s = 0; s < sArray.Length; s++)
                            {
                                _reSize += sArray[s].Remove(0, sArray[s].IndexOf("=") + 1);
                                if (s == 0)
                                    _reSize += "x";                               
                            }
                            SizeList.Items.Add(_reSize);
                           // System.Diagnostics.Debug.WriteLine(resolution_size);
                        }
                        v.VideoResolution = selectResolution(v);
                        SizeList.Text = imageWidth + "x" + imageHeight;
                        OpenVideoSource(v, loopCnt++);
                    }
                }
                else
                {
                    MessageBox.Show("Camera devices found");
                }
                //videoDevice.SetCameraProperty(
                 //   CameraControlProperty..Zoom,
                //    1,
                //    CameraControlFlags.Manual);

            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }

        }

        private VideoCapabilities selectResolution(VideoCaptureDevice device)
        {
            foreach (var cap in device.VideoCapabilities)
            {
                if (cap.FrameSize.Height == imageHeight)
                  if (cap.FrameSize.Width == imageWidth)
                    return cap;
            }
            return device.VideoCapabilities.Last();
        }

        #endregion
        //Delegate Untuk Capture, insert database, update ke grid 
        public delegate void CaptureSnapshotManifast(Bitmap image, int index);
        public void UpdateCaptureSnapshotManifast(Bitmap image, int index)
        {
            string SSN = string.Empty;
            LogInfo.Text = "";
            try
            {
                System.Windows.Forms.PictureBox pictureBox = (System.Windows.Forms.PictureBox)this.Controls["pictureBox" + index];
                needSnapshot = false;
                pictureBox.Image = image;
                pictureBox.Update();
                Tools_CMD.InputBox("請刷入編號", "編號", false,ref SSN);

                nameCapture = sn_textBox.Text + "_" + SSN + "_" +
                    DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp";

                if (!Directory.Exists(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }
                pictureBox.Image.Save(pathFolder + nameCapture, ImageFormat.Bmp);
                this.Image_upload.Dispose();
                this.Image_upload.WorkerSupportsCancellation = true;
                this.progressBar1.Value = 0;
                Image_upload.RunWorkerAsync();
            }

            catch { }           
        }

        public void OpenVideoSource(IVideoSource source, int index)
        {
            try
            {
                VideoSourcePlayer player = (VideoSourcePlayer)this.Controls["videoSourcePlayer" + index];

                // set busy cursor
                this.Cursor = Cursors.WaitCursor;

                // stop current video source
                CloseCurrentVideoSource(player);

                // start new video source
                player.VideoSource = source;
                player.Start();
                opened_videoPlayers.Add(player);

                // reset stop watch
                stopWatch = null;


                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region
        private void getListCameraUSB()
        {
            listBox1.Items.Clear();
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count != 0)
            {
                // add all devices to combo
                foreach (FilterInfo device in videoDevices)
                {
                    listBox1.Items.Add(device.Name);
                }
            }
            else
            {
                listBox1.Items.Add("No DirectShow devices found");
            }

            listBox1.SelectedIndex = 0;

            for (var i = 1; i <= MININUM_DEVICES_CNT; i++)
            {
                System.Windows.Forms.PictureBox pictureBox = (System.Windows.Forms.PictureBox)this.Controls["pictureBox" + i];
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        public void CloseCurrentVideoSource(VideoSourcePlayer player)
        {
            try
            {

                if (player.VideoSource != null)
                {
                    player.SignalToStop();

                    // wait ~ 3 seconds
                    for (int i = 0; i < 30; i++)
                    {
                        if (!player.IsRunning)
                            break;
                        System.Threading.Thread.Sleep(100);
                    }

                    if (player.IsRunning)
                    {
                        player.Stop();
                    }

                    player.VideoSource = null;
                }

            }

            catch { }
        }
        #endregion

        public void CloseCurrentVideoSource()
        {
            Disabled_Mode();
            Thread.Sleep(500);
            opened_videoPlayers.ForEach(player =>
            {
                CloseCurrentVideoSource(player);
            });
            this.opened_videoDevices.Clear();
            this.opened_videoPlayers.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //needSnapshot = true;

            captureImg(sender, e);
        }

        private void captureImg(object sender, EventArgs e)
        {
            foreach (var v in opened_videoPlayers.Select((player, index) => new { player, index }))
            {
                this.Invoke(new CaptureSnapshotManifast(UpdateCaptureSnapshotManifast), v.player.GetCurrentVideoFrame(), v.index + 1);
            }
        }

        private void videoSourcePlayer1_NewFrame_1(object sender, ref Bitmap image)
        {
            try
            {
                DateTime now = DateTime.Now;
                Graphics g = Graphics.FromImage(image);

                // paint current time
                SolidBrush brush = new SolidBrush(Color.Red);
                g.DrawString(now.ToString(), this.Font, brush, new PointF(5, 5));
                brush.Dispose();
                //if (needSnapshot)
                //{
                //    System.Diagnostics.Debug.WriteLine((sender as VideoSourcePlayer)?.Name);
                //    System.Diagnostics.Debug.WriteLine(image.GetHashCode());

                //    this.Invoke(new CaptureSnapshotManifast(UpdateCaptureSnapshotManifast), image, (sender as VideoSourcePlayer)?.Name);

                //}
                g.Dispose();
            }
            catch
            { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCurrentVideoSource();
            Tools_CMD.IniWriteValue("Camera Setting", "ImageHeight", imageHeight.ToString(), iniPath); 
            Tools_CMD.IniWriteValue("Camera Setting", "ImageWidth", imageWidth.ToString(), iniPath);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CloseCurrentVideoSource();
            Thread.Sleep(1000);
        }

        private void Enabled_Mode()
        {
            sn_textBox.Enabled = true;
            Camera_Start_button.Enabled = false;
            Camera_Stop_button.Enabled = true;
            Camera_ScreenShot_button.Enabled = true;
            listBox1.Enabled = true;
        }

        private void Disabled_Mode()
        {
            sn_textBox.Enabled = false;
            Camera_Start_button.Enabled = true;
            Camera_Stop_button.Enabled = false;
            Camera_ScreenShot_button.Enabled = false;
            listBox1.Enabled = false;
            pictureBox1.Image = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Loadini();
            try
            {
                Disabled_Mode();
                Cursor.Current = Cursors.WaitCursor;
                getListCameraUSB();
                OpenCamera();
                StartTimer();
                Cursor.Current = Cursors.Default;
                Enabled_Mode();        
            }
            catch
            { }
        }

        private void Loadini()
        {
            //FTP
            FTP_IP = Tools_CMD.IniReadValue("FTP_Option", "FTP_IP", "", iniPath);
            FTP_Port = Tools_CMD.IniReadValue("FTP_Option", "FTP_Port", "", iniPath);
            FTP_Account = Tools_CMD.IniReadValue("FTP_Option", "FTP_Account", "", iniPath);
            FTP_Password = Tools_CMD.IniReadValue("FTP_Option", "FTP_Password", "", iniPath);

            //Camera
            try
            {
                MININUM_DEVICES_CNT = Convert.ToInt16(Tools_CMD.IniReadValue("Camera Setting", "MININUM_DEVICES_CNT", "", iniPath));
                imageHeight = Convert.ToInt16(Tools_CMD.IniReadValue("Camera Setting", "ImageHeight", "", iniPath));
                imageWidth = Convert.ToInt16(Tools_CMD.IniReadValue("Camera Setting", "ImageWidth", "", iniPath));
            }
            catch
            { }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sn_textBox.Text.Length == 10)
                {
                    sn_textBox.Enabled = false;
                    captureImg(sender, e);
                    sn_textBox.Enabled = true;
                    sn_textBox.Text = "";
                    sn_textBox.Focus();
                }
                else
                {
                    MessageBox.Show("SN規則不正確");
                    sn_textBox.Clear();
                    sn_textBox.Focus();
                }
            }
        }

        private void _TimersTimer_Elapsed(object sender, EventArgs e)
        {
            if (cameraProcessTime++ > maxCameraProcessTime)
            {
                this.CloseCurrentVideoSource();
                cameraProcessTime = 0;
                this.timer1.Stop();
                pictureBox1.Image = null;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            getListCameraUSB();
        }
    
        private void Image_upload_DoWork(object sender, DoWorkEventArgs e)
        {
            Form.CheckForIllegalCrossThreadCalls = false;
            bool UploadResult = false;
            if (Image_upload.CancellationPending)//如果被中斷...
                e.Cancel = true;

            this.Image_upload.WorkerReportsProgress = true;
            BackgroundWorker worker = (BackgroundWorker)sender;
            try
            {
                UploadResult = Tools_CMD.UploadFile(nameCapture, pathFolder+nameCapture ,FTP_IP, FTP_Account,FTP_Password, progressBar1);
                if (UploadResult == true)
                {
                    LogInfo.Text += nameCapture + "\r\n圖檔上傳完成";
                    pictureBox1.Image = null;
                }
            }
            catch (Exception eee)
            {
                LogInfo.Text += (eee.ToString());
            }
        }

        private void SizeList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string[] _relist = SizeList.SelectedItem.ToString().Split('x');
            imageHeight = Convert.ToInt16(_relist[1]);
            imageWidth = Convert.ToInt16(_relist[0]);
            SizeList.Items.Clear();
            button1_Click(sender,e);
        }

        private void setting_button_Click(object sender, EventArgs e)
        {
            Form s = new Setting();
            this.Hide();
            s.ShowDialog();
        }
    }
}
