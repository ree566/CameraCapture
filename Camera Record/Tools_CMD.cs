using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Net;
using System.Data;
using System.Drawing;
using System.Threading;

namespace Camera_Record
{
    class Tools_CMD
    {
        public static List<string> Keys = new List<string>();
        public static List<string> Keys_Lines = new List<string>();
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        #region Configini
        public static void IniWriteValue(string Section, string Key, string Value, string inipath)
        {
            if (inipath.Contains("\\"))
            {
                WritePrivateProfileString(Section, Key, Value, inipath);
            }
            else
            {
                WritePrivateProfileString(Section, Key, Value, Application.StartupPath + "\\" + inipath);
            }

        }
        public static string IniReadValue(string Section, string Key, string def_val, string inipath)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = 0;
            if (inipath.Contains("\\"))
            {
                i = GetPrivateProfileString(Section, Key, def_val, temp, 255, inipath);
            }
            else
            {
                i = GetPrivateProfileString(Section, Key, def_val, temp, 255, Application.StartupPath + "\\" + inipath);
            }
            return temp.ToString();
        }
        #endregion
        #region FTP ZIP Upload
        internal bool ftpZIPupload(string fileName, string uploadUrl, string UserName, string Password)
        {
            Stream requestStream = null;
            FileStream fileStream = null;
            FtpWebResponse uploadResponse = null;
            try
            {
                FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(uploadUrl + fileName); 
                uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;//設定Method上傳檔案
                uploadRequest.Proxy = null;
                if (UserName.Length > 0)//如果需要帳號登入
                {
                    uploadRequest.Credentials = new NetworkCredential(UserName, Password); //設定帳號
                }

                requestStream = uploadRequest.GetRequestStream();
                fileStream = File.Open(AppDomain.CurrentDomain.BaseDirectory + "FTPUpload\\" + fileName, FileMode.Open);
                byte[] buffer = new byte[1024];
                int bytesRead;
                while (true)
                {//開始上傳資料流
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    requestStream.Write(buffer, 0, bytesRead);
                }
                requestStream.Close();
                uploadResponse = (FtpWebResponse)uploadRequest.GetResponse();
                return true;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            finally
            {
                if (uploadResponse != null)
                    uploadResponse.Close();
                if (fileStream != null)
                    fileStream.Close();
                if (requestStream != null)
                    requestStream.Close();
            }
        }
        #endregion
        #region TEXTBOX
        public static DialogResult InputBox(string title, string promptText, bool password, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            textBox.Text = "";

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;
            textBox.UseSystemPasswordChar = password;
            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.OK;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            textBox.CharacterCasing = CharacterCasing.Upper;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
        #endregion
        #region FTP ZIP Download      
        internal bool FTP_ZIP_Download(string fileName, string uploadUrl, string UserName, string Password)
        {

            //FtpWebRequest
            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(uploadUrl + fileName);
            NetworkCredential ftpCredential = new NetworkCredential(UserName, Password);
            ftpRequest.Credentials = ftpCredential;
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            try
            {
               //FtpWebResponse
               FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
               //Get Stream From FtpWebResponse
               Stream ftpStream = ftpResponse.GetResponseStream();
            
                using (FileStream fileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\FTPDownload\\" + fileName, FileMode.Create))
                {
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        fileStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }
                }
                ftpStream.Close();
                ftpResponse.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Download Error：" + ex.Message);
                return false;
            }
            Thread.Sleep(2000);
            return true;       
        }
        #endregion
        #region FTP log Upload
        internal bool ftpupload(string fileName, string strLogFilePath, string uploadUrl, string UserName, string Password)
        {
           try
                {
                    // Get the object used to communicate with the server.
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl + fileName);
                    request.Method = WebRequestMethods.Ftp.UploadFile;

                    // This example assumes the FTP site uses anonymous logon.
                    request.Credentials = new NetworkCredential(UserName, Password);

                    // Copy the contents of the file to the request stream.
                    byte[] fileContents;
                using (StreamReader sourceStream = new StreamReader(strLogFilePath))
                {
                    fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                }

                    request.ContentLength = fileContents.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return true;
                }                 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            
        }
        #endregion
        #region FTP log Download      
        internal bool FTP_log_Download(string fileName, string uploadUrl, string UserName, string Password)
        {

            //FtpWebRequest
            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(uploadUrl + fileName);
            NetworkCredential ftpCredential = new NetworkCredential(UserName, Password);
            ftpRequest.Credentials = ftpCredential;
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            try
            {
                //FtpWebResponse
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                //Get Stream From FtpWebResponse
                Stream ftpStream = ftpResponse.GetResponseStream();

                using (FileStream fileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\log\\" + fileName, FileMode.Create))
                {
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        fileStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }
                }
                ftpStream.Close();
                ftpResponse.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Download Error：" + ex.Message);
                return false;
            }
            Thread.Sleep(2000);
            return true;
        }
        #endregion
    }
}
