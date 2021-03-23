namespace Camera_Record
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Camera_Start_button = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.Camera_ScreenShot_button = new System.Windows.Forms.Button();
            this.CameraList_comboBox = new System.Windows.Forms.ComboBox();
            this.videoSourcePlayer1 = new AForge.Controls.VideoSourcePlayer();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Open_Folder_button = new System.Windows.Forms.Button();
            this.Camera_Stop_button = new System.Windows.Forms.Button();
            this.Po_textBox = new System.Windows.Forms.TextBox();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // Camera_Start_button
            // 
            this.Camera_Start_button.Location = new System.Drawing.Point(325, 106);
            this.Camera_Start_button.Name = "Camera_Start_button";
            this.Camera_Start_button.Size = new System.Drawing.Size(108, 25);
            this.Camera_Start_button.TabIndex = 1;
            this.Camera_Start_button.Text = "Start";
            this.Camera_Start_button.UseVisualStyleBackColor = true;
            this.Camera_Start_button.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(443, 66);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(307, 212);
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // Camera_ScreenShot_button
            // 
            this.Camera_ScreenShot_button.Location = new System.Drawing.Point(325, 166);
            this.Camera_ScreenShot_button.Name = "Camera_ScreenShot_button";
            this.Camera_ScreenShot_button.Size = new System.Drawing.Size(108, 25);
            this.Camera_ScreenShot_button.TabIndex = 3;
            this.Camera_ScreenShot_button.Text = "Snapshoot";
            this.Camera_ScreenShot_button.UseVisualStyleBackColor = true;
            this.Camera_ScreenShot_button.Click += new System.EventHandler(this.button2_Click);
            // 
            // CameraList_comboBox
            // 
            this.CameraList_comboBox.FormattingEnabled = true;
            this.CameraList_comboBox.Location = new System.Drawing.Point(325, 66);
            this.CameraList_comboBox.Name = "CameraList_comboBox";
            this.CameraList_comboBox.Size = new System.Drawing.Size(112, 20);
            this.CameraList_comboBox.TabIndex = 4;
            // 
            // videoSourcePlayer1
            // 
            this.videoSourcePlayer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoSourcePlayer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(125)))), ((int)(((byte)(125)))));
            this.videoSourcePlayer1.Location = new System.Drawing.Point(12, 66);
            this.videoSourcePlayer1.Name = "videoSourcePlayer1";
            this.videoSourcePlayer1.Size = new System.Drawing.Size(307, 211);
            this.videoSourcePlayer1.TabIndex = 5;
            this.videoSourcePlayer1.Text = "videoSourcePlayer1";
            this.videoSourcePlayer1.VideoSource = null;
            this.videoSourcePlayer1.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.videoSourcePlayer1_NewFrame_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(440, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Image Capture Result";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Webcam";
            // 
            // Open_Folder_button
            // 
            this.Open_Folder_button.Location = new System.Drawing.Point(326, 197);
            this.Open_Folder_button.Name = "Open_Folder_button";
            this.Open_Folder_button.Size = new System.Drawing.Size(107, 23);
            this.Open_Folder_button.TabIndex = 8;
            this.Open_Folder_button.Text = "Open folder";
            this.Open_Folder_button.UseVisualStyleBackColor = true;
            this.Open_Folder_button.Click += new System.EventHandler(this.Browse_Target_Folder);
            // 
            // Camera_Stop_button
            // 
            this.Camera_Stop_button.Location = new System.Drawing.Point(326, 137);
            this.Camera_Stop_button.Name = "Camera_Stop_button";
            this.Camera_Stop_button.Size = new System.Drawing.Size(107, 23);
            this.Camera_Stop_button.TabIndex = 9;
            this.Camera_Stop_button.Text = "Stop";
            this.Camera_Stop_button.UseVisualStyleBackColor = true;
            this.Camera_Stop_button.Click += new System.EventHandler(this.button4_Click);
            // 
            // Po_textBox
            // 
            this.Po_textBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.Po_textBox.Location = new System.Drawing.Point(326, 227);
            this.Po_textBox.MaxLength = 50;
            this.Po_textBox.Name = "Po_textBox";
            this.Po_textBox.Size = new System.Drawing.Size(107, 22);
            this.Po_textBox.TabIndex = 10;
            this.Po_textBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Po_KeyUp);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItem3});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem7});
            this.menuItem1.Text = "File";
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 0;
            this.menuItem7.Text = "Open folder";
            this.menuItem7.Click += new System.EventHandler(this.Browse_Target_Folder);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4,
            this.menuItem5});
            this.menuItem2.Text = "Options";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.Text = "Camera resolution";
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.Text = "Camera properties";
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem6});
            this.menuItem3.Text = "Help";
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 0;
            this.menuItem6.Text = "None";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 294);
            this.Controls.Add(this.Po_textBox);
            this.Controls.Add(this.Camera_Stop_button);
            this.Controls.Add(this.Open_Folder_button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.videoSourcePlayer1);
            this.Controls.Add(this.CameraList_comboBox);
            this.Controls.Add(this.Camera_ScreenShot_button);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.Camera_Start_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Camera capture system";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Camera_Start_button;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button Camera_ScreenShot_button;
        private System.Windows.Forms.ComboBox CameraList_comboBox;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Open_Folder_button;
        private System.Windows.Forms.Button Camera_Stop_button;
        private System.Windows.Forms.TextBox Po_textBox;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem6;
    }
}

