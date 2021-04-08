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
        #region Parameters

        private FilterInfoCollection videoDevices;
        private VideoCapabilities[] snapshotCapabilities;
        private List<VideoCaptureDevice> opened_videoDevices = new List<VideoCaptureDevice>();
        private List<VideoSourcePlayer> opened_videoPlayers = new List<VideoSourcePlayer>();
        private ArrayList listCamera = new ArrayList();
        public string pathFolder = Application.StartupPath + @"\ImageCapture\";

        private Stopwatch stopWatch = null;
        //private static bool needSnapshot = false;
        private BackgroundWorker bw = new BackgroundWorker();

        private static string _usbcamera;

        private readonly int DEVICES_CNT = 1;

        private int cameraProcessTime = 0;
        private int maxCameraProcessTime = 3 * 60; //Close camera when opening after 3 hours

        private int snMinTextLengh = 8;
        public string usbcamera
        {
            get { return _usbcamera; }
            set { _usbcamera = value; }
        }

        string iniPath = @"Config.ini";
        string ftpIp = string.Empty;
        string ftpPort = string.Empty;
        string ftpAccount = string.Empty;
        string ftpPsw = string.Empty;

        string nameCapture = string.Empty;

        int imageHeight;
        int imageWidth;

        Tools_CMD tools_CMD = new Tools_CMD();

        //private static object _thisLock = new object();

        #endregion

        #region Init

        public Form1()
        {
            InitializeComponent();
            //getListCameraUSB();
            //Disabled_Mode();
            Camera_ScreenShot_button.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Loadini();
            try
            {
                Disabled_Mode();
                Cursor.Current = Cursors.WaitCursor;
                GetListCameraUSB();

                //lock (_thisLock)
                //{
                //}

                //try
                //{
                //    OpenCamera();
                //    StartTimer();
                //    Enabled_Mode();
                //}
                //catch (Exception ex)
                //{
                //    Disabled_Mode();
                //}

                Cursor.Current = Cursors.Default;

            }
            catch
            { }
        }

        private void Loadini()
        {
            //FTP
            ftpIp = Tools_CMD.IniReadValue("FTP_Option", "FTP_IP", "", iniPath);
            ftpPort = Tools_CMD.IniReadValue("FTP_Option", "FTP_Port", "", iniPath);
            ftpAccount = Tools_CMD.IniReadValue("FTP_Option", "FTP_Account", "", iniPath);
            ftpPsw = Tools_CMD.IniReadValue("FTP_Option", "FTP_Password", "", iniPath);

            //Camera
            try
            {
                //MININUM_DEVICES_CNT = Convert.ToInt16(Tools_CMD.IniReadValue("Camera Setting", "MININUM_DEVICES_CNT", "", iniPath));
                imageHeight = Convert.ToInt16(Tools_CMD.IniReadValue("Camera Setting", "ImageHeight", "", iniPath));
                imageWidth = Convert.ToInt16(Tools_CMD.IniReadValue("Camera Setting", "ImageWidth", "", iniPath));
            }
            catch
            { }
        }

        #endregion

        #region Timer setting
        private void StartTimer()
        {
            // Call this procedure when the application starts.
            // Set to 1 second.
            timer1.Interval = 1000;

            // Enable timer.
            this.timer1.Start();
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

        #endregion

        #region Open scan camera
        private void OpenCamera()
        {
            try
            {
                usbcamera = listBox1.SelectedIndex.ToString();
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count >= DEVICES_CNT)
                {
                    //if device cnt large than setting, set setting device to one and let user choose(one camera capture mode)
                    bool isOneCameraCaptureMode = (videoDevices.Count > DEVICES_CNT) ? true : false;
                    string selectedItem = (string)listBox1.SelectedItem;
                    int loopCnt = 1;
                    // add all devices to combo
                    foreach (FilterInfo device in videoDevices)
                    {
                        if (isOneCameraCaptureMode && device.Name == selectedItem)
                        {
                            VideoCaptureDeviceInit(device, 1);
                            break;
                        }
                        else if (!isOneCameraCaptureMode)
                        {
                            VideoCaptureDeviceInit(device, loopCnt++);
                        }
                    }
                }
                else if (videoDevices.Count < DEVICES_CNT)
                {
                    throw new Exception("VideoDevices count not match devices count in setting file");
                }
                else
                {
                    throw new Exception("No camera devices found");
                }
                //videoDevice.SetCameraProperty(
                //    CameraControlProperty.Zoom,
                //    1,
                //    CameraControlFlags.Manual);

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                throw;
            }

        }

        private void VideoCaptureDeviceInit(FilterInfo device, int index)
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
                string[] sArray = resolution_size.Split(new string[] { ",", "{", "}" }, StringSplitOptions.RemoveEmptyEntries);
                for (int s = 0; s < sArray.Length; s++)
                {
                    _reSize += sArray[s].Remove(0, sArray[s].IndexOf("=") + 1);
                    if (s == 0)
                        _reSize += "x";
                }
                SizeList.Items.Add(_reSize);
                // System.Diagnostics.Debug.WriteLine(resolution_size);
            }
            v.VideoResolution = SelectResolution(v);
            SizeList.Text = imageWidth + "x" + imageHeight;
            LogInfo.Text += "Open camera " + device.Name + "\r\n";
            OpenVideoSource(v, index);
        }

        private VideoCapabilities SelectResolution(VideoCaptureDevice device)
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

        #region Capture snapshot
        //Delegate Untuk Capture, insert database, update ke grid 
        public delegate void CaptureSnapshotManifast(Bitmap image, int index);
        public void UpdateCaptureSnapshotManifast(Bitmap image, int index)
        {
            string SSN = string.Empty;
            LogInfo.Text = "";
            try
            {
                System.Windows.Forms.PictureBox pictureBox = (System.Windows.Forms.PictureBox)this.Controls["pictureBox" + index];
                //               needSnapshot = false;

                do
                {
                    Tools_CMD.InputBox("請刷入編號", "編號", false, ref SSN);

                    if ("".Equals(SSN))
                    {
                        MessageBox.Show("編號規則不正確");
                    }
                } while ("".Equals(SSN));

                pictureBox.Image = image;
                pictureBox.Update();

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

        #endregion

        #region Show enabled camera devices
        private void GetListCameraUSB()
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

            for (var i = 1; i <= DEVICES_CNT; i++)
            {
                System.Windows.Forms.PictureBox pictureBox = (System.Windows.Forms.PictureBox)this.Controls["pictureBox" + i];
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        #endregion

        #region VideoSource relate(open & close)

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

            LogInfo.Text += "Close all camera resources\r\n";
        }

        #endregion

        #region Wiget event
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenCamera();
                StartTimer();
                Enabled_Mode();
            }
            catch (Exception ex)
            {
                Disabled_Mode();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //needSnapshot = true;

            captureImg(sender, e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CloseCurrentVideoSource();
            Thread.Sleep(1000);
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sn_textBox.Text.Length >= snMinTextLengh)
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

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (System.Text.Encoding.UTF8.GetByteCount(new char[] { e.KeyChar }) > 1)
            {
                e.Handled = true;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            GetListCameraUSB();
        }

        private void SizeList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string[] _relist = SizeList.SelectedItem.ToString().Split('x');
            imageHeight = Convert.ToInt16(_relist[1]);
            imageWidth = Convert.ToInt16(_relist[0]);
            SizeList.Items.Clear();
            button1_Click(sender, e);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox1.Update();
        }

        #endregion

        #region Mode setting

        private void Enabled_Mode()
        {
            sn_textBox.Enabled = true;
            sn_textBox.Text = "";
            Camera_Start_button.Enabled = false;
            Camera_Stop_button.Enabled = true;
            Camera_ScreenShot_button.Enabled = true;
            //listBox1.Enabled = true;
        }

        private void Disabled_Mode()
        {
            sn_textBox.Enabled = false;
            sn_textBox.Text = "";
            Camera_Start_button.Enabled = true;
            Camera_Stop_button.Enabled = false;
            Camera_ScreenShot_button.Enabled = false;
            //listBox1.Enabled = false;
            pictureBox1.Image = null;
        }

        #endregion

        #region Image upload jobs
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
                UploadResult = Tools_CMD.UploadFile(nameCapture, pathFolder + nameCapture, ftpIp, ftpAccount, ftpPsw, progressBar1);
                if (UploadResult == true)
                {
                    LogInfo.Text += nameCapture + "圖檔上傳完成\r\n";
                    System.IO.File.Delete(pathFolder + nameCapture);
                    //pictureBox1.Image = null;
                }
            }
            catch (Exception eee)
            {
                LogInfo.Text += (eee.ToString());
            }
        }

        #endregion

    }
}
