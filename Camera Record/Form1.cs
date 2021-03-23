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

namespace Camera_Record
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        private VideoCapabilities[] snapshotCapabilities;
        private ArrayList listCamera = new ArrayList();

        public string pathFolder = Application.StartupPath + @"\ImageCapture\";

        private Stopwatch stopWatch = null;
        private static bool needSnapshot = false;

        private BackgroundWorker bw = new BackgroundWorker();

        public Form1()
        {
            InitializeComponent();
            getListCameraUSB();
            Disabled_Mode();
        }

        private static string _usbcamera;
        public string usbcamera
        {
            get { return _usbcamera; }
            set { _usbcamera = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenCamera();
            Enabled_Mode();
        }

        #region Open Scan Camera
        private void OpenCamera()
        {
            try
            {
                usbcamera = CameraList_comboBox.SelectedIndex.ToString();
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count != 0)
                {
                    // add all devices to combo
                    foreach (FilterInfo device in videoDevices)
                    {
                        listCamera.Add(device.Name);
                    }
                }
                else
                {
                    MessageBox.Show("Camera devices found");
                }

                videoDevice = new VideoCaptureDevice(videoDevices[Convert.ToInt32(usbcamera)].MonikerString);
                //videoDevice.SetCameraProperty(
                //    CameraControlProperty.Zoom,
                //    1,
                //    CameraControlFlags.Manual);
                snapshotCapabilities = videoDevice.SnapshotCapabilities;
                if (snapshotCapabilities.Length == 0)
                {
                    MessageBox.Show("Camera Capture Not supported");
                }

                videoDevice.SnapshotResolution = videoDevice.VideoCapabilities[0]; //It selects the default size
                int a = videoDevice.VideoCapabilities.Length;
                for (int i = 0; i < videoDevice.VideoCapabilities.Length; i++)
                {

                    string resolution = "Resolution Number " + Convert.ToString(i);
                    string resolution_size = videoDevice.VideoCapabilities[i].FrameSize.ToString();

                    System.Diagnostics.Debug.WriteLine("resolution , resolution_size>> " + resolution + "" + resolution_size);
                }
                videoDevice.VideoResolution = selectResolution(videoDevice);
                OpenVideoSource(videoDevice);
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
        public delegate void CaptureSnapshotManifast(Bitmap image);
        public void UpdateCaptureSnapshotManifast(Bitmap image)
        {
            try
            {
                needSnapshot = false;
                pictureBox2.Image = image;
                pictureBox2.Update();


                string namaImage = "sampleImage";
                string nameCapture = namaImage + "_" + Po_textBox.Text + "_" +
                    DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp";

                if (Directory.Exists(pathFolder))
                {
                    pictureBox2.Image.Save(pathFolder + nameCapture, ImageFormat.Bmp);
                }
                else
                {
                    Directory.CreateDirectory(pathFolder);
                    pictureBox2.Image.Save(pathFolder + nameCapture, ImageFormat.Bmp);
                }

            }

            catch { }

            Po_textBox.Text = "";
        }

        public void OpenVideoSource(IVideoSource source)
        {
            try
            {
                // set busy cursor
                this.Cursor = Cursors.WaitCursor;

                // stop current video source
                CloseCurrentVideoSource();

                // start new video source
                videoSourcePlayer1.VideoSource = source;
                videoSourcePlayer1.Start();

                // reset stop watch
                stopWatch = null;


                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void getListCameraUSB()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count != 0)
            {
                // add all devices to combo
                foreach (FilterInfo device in videoDevices)
                {
                    CameraList_comboBox.Items.Add(device.Name);
                }
            }
            else
            {
                CameraList_comboBox.Items.Add("No DirectShow devices found");
            }

            CameraList_comboBox.SelectedIndex = 0;

            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public void CloseCurrentVideoSource()
        {
            Disabled_Mode();
            try
            {

                if (videoSourcePlayer1.VideoSource != null)
                {
                    videoSourcePlayer1.SignalToStop();

                    // wait ~ 3 seconds
                    for (int i = 0; i < 30; i++)
                    {
                        if (!videoSourcePlayer1.IsRunning)
                            break;
                        System.Threading.Thread.Sleep(100);
                    }

                    if (videoSourcePlayer1.IsRunning)
                    {
                        videoSourcePlayer1.Stop();
                    }

                    videoSourcePlayer1.VideoSource = null;
                }
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            needSnapshot = true;
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
                if (needSnapshot)
                {
                    this.Invoke(new CaptureSnapshotManifast(UpdateCaptureSnapshotManifast), image);
                }
                g.Dispose();
            }
            catch
            { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit?", "My Application", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
            }

            MessageBox.Show("Wait for three seconds...");

            Thread.Sleep(500);

            CloseCurrentVideoSource();
        }

        private void Browse_Target_Folder(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(pathFolder);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.CloseCurrentVideoSource();
        }

        private void Enabled_Mode()
        {
            Camera_ScreenShot_button.Enabled = true;
            Po_textBox.Enabled = true;
        }

        private void Disabled_Mode()
        {
            Camera_ScreenShot_button.Enabled = false;
            Po_textBox.Enabled = false;
        }

        public void Po_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Do something
                e.Handled = true;

                needSnapshot = true;

                System.Diagnostics.Debug.WriteLine("Triggering");

            }
        }
    }
}
