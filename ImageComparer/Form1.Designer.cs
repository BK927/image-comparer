﻿namespace ImageComparer
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
            this.header_panel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.original_img_btn = new System.Windows.Forms.Button();
            this.original_Img_label = new System.Windows.Forms.Label();
            this.orginal_img_txt_folder = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.compare_img_btn = new System.Windows.Forms.Button();
            this.compare_label = new System.Windows.Forms.Label();
            this.compare_txt_folder = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.save_btn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tag_box = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.add_tag_box = new System.Windows.Forms.TextBox();
            this.freq_tag_box = new System.Windows.Forms.ListBox();
            this.ui_main.SuspendLayout();
            this.uiTlp_Sub.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.original_pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.compare_pic)).BeginInit();
            this.header_panel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.ui_main.Controls.Add(this.uiTlp_Sub, 0, 1);
            this.ui_main.Controls.Add(this.header_panel, 0, 0);
            this.ui_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ui_main.Location = new System.Drawing.Point(0, 0);
            this.ui_main.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ui_main.Name = "ui_main";
            this.ui_main.RowCount = 2;
            this.ui_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.ui_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ui_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
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
            this.uiTlp_Sub.Location = new System.Drawing.Point(3, 234);
            this.uiTlp_Sub.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiTlp_Sub.Name = "uiTlp_Sub";
            this.uiTlp_Sub.Padding = new System.Windows.Forms.Padding(5);
            this.uiTlp_Sub.RowCount = 1;
            this.uiTlp_Sub.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiTlp_Sub.Size = new System.Drawing.Size(1053, 540);
            this.uiTlp_Sub.TabIndex = 3;
            // 
            // original_pic
            // 
            this.original_pic.BackColor = System.Drawing.Color.DimGray;
            this.original_pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.original_pic.Location = new System.Drawing.Point(8, 9);
            this.original_pic.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.original_pic.Name = "original_pic";
            this.original_pic.Size = new System.Drawing.Size(515, 522);
            this.original_pic.TabIndex = 1;
            this.original_pic.TabStop = false;
            // 
            // compare_pic
            // 
            this.compare_pic.BackColor = System.Drawing.Color.DimGray;
            this.compare_pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compare_pic.Location = new System.Drawing.Point(529, 8);
            this.compare_pic.Name = "compare_pic";
            this.compare_pic.Size = new System.Drawing.Size(516, 524);
            this.compare_pic.TabIndex = 2;
            this.compare_pic.TabStop = false;
            this.compare_pic.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.compare_pic_LoadCompleted);
            // 
            // header_panel
            // 
            this.header_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.header_panel.ColumnCount = 2;
            this.header_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.header_panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.header_panel.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.header_panel.Controls.Add(this.panel4, 1, 0);
            this.header_panel.Location = new System.Drawing.Point(3, 3);
            this.header_panel.Name = "header_panel";
            this.header_panel.RowCount = 1;
            this.header_panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.header_panel.Size = new System.Drawing.Size(1053, 224);
            this.header_panel.TabIndex = 6;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(887, 218);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.original_img_btn);
            this.panel2.Controls.Add(this.original_Img_label);
            this.panel2.Controls.Add(this.orginal_img_txt_folder);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(881, 49);
            this.panel2.TabIndex = 0;
            // 
            // original_img_btn
            // 
            this.original_img_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.original_img_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.original_img_btn.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.original_img_btn.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.original_img_btn.Location = new System.Drawing.Point(796, 4);
            this.original_img_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.original_img_btn.Name = "original_img_btn";
            this.original_img_btn.Size = new System.Drawing.Size(82, 39);
            this.original_img_btn.TabIndex = 4;
            this.original_img_btn.Text = "open";
            this.original_img_btn.UseVisualStyleBackColor = true;
            // 
            // original_Img_label
            // 
            this.original_Img_label.Font = new System.Drawing.Font("Segoe UI Black", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.original_Img_label.ForeColor = System.Drawing.Color.White;
            this.original_Img_label.Location = new System.Drawing.Point(23, 10);
            this.original_Img_label.Name = "original_Img_label";
            this.original_Img_label.Size = new System.Drawing.Size(143, 29);
            this.original_Img_label.TabIndex = 2;
            this.original_Img_label.Text = "Original Images";
            this.original_Img_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // orginal_img_txt_folder
            // 
            this.orginal_img_txt_folder.AllowDrop = true;
            this.orginal_img_txt_folder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.orginal_img_txt_folder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.orginal_img_txt_folder.Location = new System.Drawing.Point(181, 17);
            this.orginal_img_txt_folder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.orginal_img_txt_folder.Name = "orginal_img_txt_folder";
            this.orginal_img_txt_folder.ReadOnly = true;
            this.orginal_img_txt_folder.Size = new System.Drawing.Size(609, 18);
            this.orginal_img_txt_folder.TabIndex = 3;
            this.orginal_img_txt_folder.TextChanged += new System.EventHandler(this.orginal_img_txt_folder_TextChanged);
            this.orginal_img_txt_folder.DragDrop += new System.Windows.Forms.DragEventHandler(this.orginal_img_txt_folder_DragDrop);
            this.orginal_img_txt_folder.DragEnter += new System.Windows.Forms.DragEventHandler(this.orginal_img_txt_folder_DragEnter);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.compare_img_btn);
            this.panel1.Controls.Add(this.compare_label);
            this.panel1.Controls.Add(this.compare_txt_folder);
            this.panel1.Location = new System.Drawing.Point(3, 58);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(881, 49);
            this.panel1.TabIndex = 1;
            // 
            // compare_img_btn
            // 
            this.compare_img_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.compare_img_btn.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compare_img_btn.Location = new System.Drawing.Point(796, 3);
            this.compare_img_btn.Name = "compare_img_btn";
            this.compare_img_btn.Size = new System.Drawing.Size(82, 43);
            this.compare_img_btn.TabIndex = 2;
            this.compare_img_btn.Text = "open";
            this.compare_img_btn.UseVisualStyleBackColor = true;
            this.compare_img_btn.Click += new System.EventHandler(this.compare_img_btn_Click);
            // 
            // compare_label
            // 
            this.compare_label.AutoSize = true;
            this.compare_label.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compare_label.ForeColor = System.Drawing.Color.White;
            this.compare_label.Location = new System.Drawing.Point(3, 13);
            this.compare_label.Name = "compare_label";
            this.compare_label.Size = new System.Drawing.Size(172, 23);
            this.compare_label.TabIndex = 0;
            this.compare_label.Text = "Images to compare";
            // 
            // compare_txt_folder
            // 
            this.compare_txt_folder.AllowDrop = true;
            this.compare_txt_folder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.compare_txt_folder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.compare_txt_folder.Location = new System.Drawing.Point(181, 13);
            this.compare_txt_folder.Name = "compare_txt_folder";
            this.compare_txt_folder.ReadOnly = true;
            this.compare_txt_folder.Size = new System.Drawing.Size(609, 18);
            this.compare_txt_folder.TabIndex = 1;
            this.compare_txt_folder.TextChanged += new System.EventHandler(this.compare_txt_folder_TextChanged);
            this.compare_txt_folder.DragDrop += new System.Windows.Forms.DragEventHandler(this.compare_txt_folder_DragDrop);
            this.compare_txt_folder.DragEnter += new System.Windows.Forms.DragEventHandler(this.compare_txt_folder_DragEnter);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.save_btn);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.tag_box);
            this.panel3.Location = new System.Drawing.Point(3, 113);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(881, 104);
            this.panel3.TabIndex = 2;
            // 
            // save_btn
            // 
            this.save_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.save_btn.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.save_btn.Location = new System.Drawing.Point(796, 3);
            this.save_btn.Name = "save_btn";
            this.save_btn.Size = new System.Drawing.Size(82, 98);
            this.save_btn.TabIndex = 2;
            this.save_btn.Text = "save";
            this.save_btn.UseVisualStyleBackColor = true;
            this.save_btn.Click += new System.EventHandler(this.save_btn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Black", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Polite;
            this.label2.Location = new System.Drawing.Point(61, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Tags";
            // 
            // tag_box
            // 
            this.tag_box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tag_box.Enabled = false;
            this.tag_box.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tag_box.HideSelection = false;
            this.tag_box.Location = new System.Drawing.Point(181, 3);
            this.tag_box.Multiline = true;
            this.tag_box.Name = "tag_box";
            this.tag_box.Size = new System.Drawing.Size(609, 98);
            this.tag_box.TabIndex = 1;
            this.tag_box.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tag_box_KeyPress);
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.Controls.Add(this.add_tag_box);
            this.panel4.Controls.Add(this.freq_tag_box);
            this.panel4.Location = new System.Drawing.Point(896, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(154, 218);
            this.panel4.TabIndex = 1;
            // 
            // add_tag_box
            // 
            this.add_tag_box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.add_tag_box.Location = new System.Drawing.Point(3, 189);
            this.add_tag_box.Name = "add_tag_box";
            this.add_tag_box.Size = new System.Drawing.Size(145, 25);
            this.add_tag_box.TabIndex = 1;
            this.add_tag_box.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.add_tag_box_KeyPress);
            // 
            // freq_tag_box
            // 
            this.freq_tag_box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.freq_tag_box.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.freq_tag_box.FormattingEnabled = true;
            this.freq_tag_box.ItemHeight = 20;
            this.freq_tag_box.Location = new System.Drawing.Point(4, 0);
            this.freq_tag_box.Name = "freq_tag_box";
            this.freq_tag_box.Size = new System.Drawing.Size(145, 184);
            this.freq_tag_box.TabIndex = 0;
            this.freq_tag_box.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.freq_tag_box_MouseDoubleClick);
            this.freq_tag_box.MouseUp += new System.Windows.Forms.MouseEventHandler(this.freq_tag_box_MouseUp);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 778);
            this.Controls.Add(this.ui_main);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Image Comparer";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.ui_main.ResumeLayout(false);
            this.uiTlp_Sub.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.original_pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.compare_pic)).EndInit();
            this.header_panel.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
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
        private System.Windows.Forms.Label original_Img_label;
        private System.Windows.Forms.TextBox orginal_img_txt_folder;
        private System.Windows.Forms.TableLayoutPanel uiTlp_Sub;
        private System.Windows.Forms.PictureBox original_pic;
        private System.Windows.Forms.PictureBox compare_pic;
        private System.Windows.Forms.Button compare_img_btn;
        private System.Windows.Forms.TextBox compare_txt_folder;
        private System.Windows.Forms.Label compare_label;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tag_box;
        private System.Windows.Forms.Button original_img_btn;
        private System.Windows.Forms.Button save_btn;
        private System.Windows.Forms.TableLayoutPanel header_panel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox add_tag_box;
        private System.Windows.Forms.ListBox freq_tag_box;
    }
}

