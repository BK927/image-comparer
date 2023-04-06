namespace WindowsFormsApplication5
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.ui_main = new System.Windows.Forms.TableLayoutPanel();
            this.uiTlp_Sub = new System.Windows.Forms.TableLayoutPanel();
            this.original_pic = new System.Windows.Forms.PictureBox();
            this.compare_pic = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.original_img_btn = new System.Windows.Forms.Button();
            this.orginal_img_txt_folder = new System.Windows.Forms.TextBox();
            this.original_Img_label = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.compare_img_btn = new System.Windows.Forms.Button();
            this.compare_txt_folder = new System.Windows.Forms.TextBox();
            this.compare_label = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.save_btn = new System.Windows.Forms.Button();
            this.tag_box = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ui_main.SuspendLayout();
            this.uiTlp_Sub.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.original_pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.compare_pic)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // ui_main
            // 
            this.ui_main.BackColor = System.Drawing.Color.Gray;
            this.ui_main.ColumnCount = 1;
            this.ui_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ui_main.Controls.Add(this.uiTlp_Sub, 0, 3);
            this.ui_main.Controls.Add(this.panel1, 0, 0);
            this.ui_main.Controls.Add(this.panel3, 0, 1);
            this.ui_main.Controls.Add(this.panel4, 0, 2);
            this.ui_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ui_main.Location = new System.Drawing.Point(0, 0);
            this.ui_main.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ui_main.Name = "ui_main";
            this.ui_main.RowCount = 4;
            this.ui_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.ui_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.ui_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.ui_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ui_main.Size = new System.Drawing.Size(1059, 778);
            this.ui_main.TabIndex = 0;
            // 
            // uiTlp_Sub
            // 
            this.uiTlp_Sub.BackColor = System.Drawing.Color.Gray;
            this.uiTlp_Sub.ColumnCount = 2;
            this.uiTlp_Sub.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTlp_Sub.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.uiTlp_Sub.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.uiTlp_Sub.Controls.Add(this.original_pic, 0, 0);
            this.uiTlp_Sub.Controls.Add(this.compare_pic, 1, 0);
            this.uiTlp_Sub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTlp_Sub.Location = new System.Drawing.Point(3, 224);
            this.uiTlp_Sub.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiTlp_Sub.Name = "uiTlp_Sub";
            this.uiTlp_Sub.Padding = new System.Windows.Forms.Padding(5);
            this.uiTlp_Sub.RowCount = 1;
            this.uiTlp_Sub.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTlp_Sub.Size = new System.Drawing.Size(1053, 550);
            this.uiTlp_Sub.TabIndex = 3;
            // 
            // original_pic
            // 
            this.original_pic.BackColor = System.Drawing.Color.DimGray;
            this.original_pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.original_pic.Location = new System.Drawing.Point(8, 9);
            this.original_pic.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.original_pic.Name = "original_pic";
            this.original_pic.Size = new System.Drawing.Size(515, 532);
            this.original_pic.TabIndex = 1;
            this.original_pic.TabStop = false;
            // 
            // compare_pic
            // 
            this.compare_pic.BackColor = System.Drawing.Color.DimGray;
            this.compare_pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compare_pic.Location = new System.Drawing.Point(529, 8);
            this.compare_pic.Name = "compare_pic";
            this.compare_pic.Size = new System.Drawing.Size(516, 534);
            this.compare_pic.TabIndex = 2;
            this.compare_pic.TabStop = false;
            this.compare_pic.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.compare_pic_LoadCompleted);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.original_img_btn);
            this.panel1.Controls.Add(this.orginal_img_txt_folder);
            this.panel1.Controls.Add(this.original_Img_label);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Margin = new System.Windows.Forms.Padding(5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1049, 45);
            this.panel1.TabIndex = 1;
            // 
            // original_img_btn
            // 
            this.original_img_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.original_img_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.original_img_btn.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.original_img_btn.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.original_img_btn.Location = new System.Drawing.Point(959, 4);
            this.original_img_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.original_img_btn.Name = "original_img_btn";
            this.original_img_btn.Size = new System.Drawing.Size(82, 39);
            this.original_img_btn.TabIndex = 4;
            this.original_img_btn.Text = "open";
            this.original_img_btn.UseVisualStyleBackColor = true;
            // 
            // orginal_img_txt_folder
            // 
            this.orginal_img_txt_folder.AllowDrop = true;
            this.orginal_img_txt_folder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.orginal_img_txt_folder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.orginal_img_txt_folder.Location = new System.Drawing.Point(185, 16);
            this.orginal_img_txt_folder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.orginal_img_txt_folder.Name = "orginal_img_txt_folder";
            this.orginal_img_txt_folder.ReadOnly = true;
            this.orginal_img_txt_folder.Size = new System.Drawing.Size(768, 18);
            this.orginal_img_txt_folder.TabIndex = 3;
            this.orginal_img_txt_folder.TextChanged += new System.EventHandler(this.orginal_img_txt_folder_TextChanged);
            this.orginal_img_txt_folder.DragDrop += new System.Windows.Forms.DragEventHandler(this.orginal_img_txt_folder_DragDrop);
            this.orginal_img_txt_folder.DragEnter += new System.Windows.Forms.DragEventHandler(this.orginal_img_txt_folder_DragEnter);
            // 
            // original_Img_label
            // 
            this.original_Img_label.Font = new System.Drawing.Font("Segoe UI Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.original_Img_label.ForeColor = System.Drawing.Color.White;
            this.original_Img_label.Location = new System.Drawing.Point(7, 9);
            this.original_Img_label.Name = "original_Img_label";
            this.original_Img_label.Size = new System.Drawing.Size(143, 29);
            this.original_Img_label.TabIndex = 2;
            this.original_Img_label.Text = "Original Images";
            this.original_Img_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.compare_img_btn);
            this.panel3.Controls.Add(this.compare_txt_folder);
            this.panel3.Controls.Add(this.compare_label);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(3, 58);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1053, 49);
            this.panel3.TabIndex = 4;
            // 
            // compare_img_btn
            // 
            this.compare_img_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.compare_img_btn.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compare_img_btn.Location = new System.Drawing.Point(961, 3);
            this.compare_img_btn.Name = "compare_img_btn";
            this.compare_img_btn.Size = new System.Drawing.Size(82, 43);
            this.compare_img_btn.TabIndex = 2;
            this.compare_img_btn.Text = "open";
            this.compare_img_btn.UseVisualStyleBackColor = true;
            this.compare_img_btn.Click += new System.EventHandler(this.compare_img_btn_Click);
            // 
            // compare_txt_folder
            // 
            this.compare_txt_folder.AllowDrop = true;
            this.compare_txt_folder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.compare_txt_folder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.compare_txt_folder.Location = new System.Drawing.Point(187, 17);
            this.compare_txt_folder.Name = "compare_txt_folder";
            this.compare_txt_folder.ReadOnly = true;
            this.compare_txt_folder.Size = new System.Drawing.Size(768, 18);
            this.compare_txt_folder.TabIndex = 1;
            this.compare_txt_folder.TextChanged += new System.EventHandler(this.compare_txt_folder_TextChanged);
            this.compare_txt_folder.DragDrop += new System.Windows.Forms.DragEventHandler(this.compare_txt_folder_DragDrop);
            this.compare_txt_folder.DragEnter += new System.Windows.Forms.DragEventHandler(this.compare_txt_folder_DragEnter);
            // 
            // compare_label
            // 
            this.compare_label.AutoSize = true;
            this.compare_label.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compare_label.ForeColor = System.Drawing.Color.White;
            this.compare_label.Location = new System.Drawing.Point(9, 12);
            this.compare_label.Name = "compare_label";
            this.compare_label.Size = new System.Drawing.Size(172, 23);
            this.compare_label.TabIndex = 0;
            this.compare_label.Text = "Images to compare";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.save_btn);
            this.panel4.Controls.Add(this.tag_box);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(3, 113);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1053, 104);
            this.panel4.TabIndex = 5;
            // 
            // save_btn
            // 
            this.save_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.save_btn.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.save_btn.Location = new System.Drawing.Point(983, 5);
            this.save_btn.Name = "save_btn";
            this.save_btn.Size = new System.Drawing.Size(62, 96);
            this.save_btn.TabIndex = 2;
            this.save_btn.Text = "save";
            this.save_btn.UseVisualStyleBackColor = true;
            this.save_btn.Click += new System.EventHandler(this.save_btn_Click);
            // 
            // tag_box
            // 
            this.tag_box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tag_box.Enabled = false;
            this.tag_box.HideSelection = false;
            this.tag_box.Location = new System.Drawing.Point(65, 5);
            this.tag_box.Multiline = true;
            this.tag_box.Name = "tag_box";
            this.tag_box.Size = new System.Drawing.Size(912, 96);
            this.tag_box.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Polite;
            this.label2.Location = new System.Drawing.Point(9, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Tags";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 778);
            this.Controls.Add(this.ui_main);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Image Comparer";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.ui_main.ResumeLayout(false);
            this.uiTlp_Sub.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.original_pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.compare_pic)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel ui_main;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label original_Img_label;
        private System.Windows.Forms.TextBox orginal_img_txt_folder;
        private System.Windows.Forms.TableLayoutPanel uiTlp_Sub;
        private System.Windows.Forms.PictureBox original_pic;
        private System.Windows.Forms.PictureBox compare_pic;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button compare_img_btn;
        private System.Windows.Forms.TextBox compare_txt_folder;
        private System.Windows.Forms.Label compare_label;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tag_box;
        private System.Windows.Forms.Button original_img_btn;
        private System.Windows.Forms.Button save_btn;
    }
}

