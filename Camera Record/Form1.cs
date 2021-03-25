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

        string iniPath = @"Config.ini";
        string FTP_IP = string.Empty;
        string FTP_Port = string.Empty;
        string FTP_Account = string.Empty;
        string FTP_Password = string.Empty;
        string OPID = string.Empty;
        string SN = string.Empty;

        private readonly int MININUM_DEVICES_CNT = 2;
        private int cameraProcessTime = 0;
        private int maxCameraProcessTime = 3 * 60; //Close camera when opening after 3 hours

        public Form1()
        {
            InitializeComponent();
            //getListCameraUSB();
            Disabled_Mode();
        }

        private static string _usbcamera;
        public string usbcamera
        {
            get { return _usbcamera; }
            set { _usbcamera = value; }
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

                        //v.SnapshotResolution = v.VideoCapabilities[0]; //It selects the default size
                        //int a = v.VideoCapabilities.Length;
                        //for (int i = 0; i < v.VideoCapabilities.Length; i++)
                        //{
                        //    string resolution = "Resolution Number " + Convert.ToString(i);
                        //    string resolution_size = v.VideoCapabilities[i].FrameSize.ToString();

                        //    System.Diagnostics.Debug.WriteLine("resolution , resolution_size>> " + resolution + "" + resolution_size);
                        //}
                        v.VideoResolution = selectResolution(v);
                        OpenVideoSource(v, loopCnt++);
                    }
                }
                else
                {
                    MessageBox.Show("Camera devices found");
                }
                //videoDevice.SetCameraProperty(
                //    CameraControlProperty.Zoom,
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
                if (cap.FrameSize.Height == 1080)
                    return cap;
                if (cap.FrameSize.Width == 1920)
                    return cap;
            }
            return device.VideoCapabilities.Last();
        }

        #endregion
        //Delegate Untuk Capture, insert database, update ke grid 
        public delegate void CaptureSnapshotManifast(Bitmap image, int index);
        public void UpdateCaptureSnapshotManifast(Bitmap image, int index)
        {
            try
            {
                System.Windows.Forms.PictureBox pictureBox = (System.Windows.Forms.PictureBox)this.Controls["pictureBox" + index];
                needSnapshot = false;
                pictureBox.Image = image;
                pictureBox.Update();

                string nameCapture = sn_textBox.Text + "_" + "camera" + index + "_" +
                    DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp";

                if (!Directory.Exists(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }
                pictureBox.Image.Save(pathFolder + nameCapture, ImageFormat.Bmp);
            }

            catch (Exception ex){
                LogInfo.Text = "Please wait for camera ready.";
            }
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
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CloseCurrentVideoSource();
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
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sn_textBox.Enabled = false;
                captureImg(sender, e);
                sn_textBox.Enabled = true;
                sn_textBox.Text = "";
                sn_textBox.Focus();
            }
        }

        private void _TimersTimer_Elapsed(object sender, EventArgs e)
        {
            if (cameraProcessTime++ > maxCameraProcessTime)
            {
                this.CloseCurrentVideoSource();
                cameraProcessTime = 0;
                this.timer1.Stop();
            }
        }
    }
}
