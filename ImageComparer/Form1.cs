using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using static System.Net.WebRequestMethods;

namespace WindowsFormsApplication5
{
    public partial class Form1 : Form
    {
        List<string> originalImgs = null;
        List<string> comparisonImgs = null;
        List<string> filteredOriginals = null;
        List<string> filteredComparions = null;
        HashSet<string> tagDic = null;
        string delReserved = null;

        int currentIdx = -1;

        private Object thisLock = new object();

        string initialTitle;

        public Form1()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.AllowDrop = true;

            this.original_img_btn.Click += Original_Img_Btn_Click;
            this.initialTitle = this.Text;
        }

        private void Original_Img_Btn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                orginal_img_txt_folder.Text = fbd.SelectedPath;
            }
        }

        private void compare_img_btn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                compare_txt_folder.Text = fbd.SelectedPath;
            }
        }

        private List<string> loadimages(string path)
        {
            string[] files = Directory.GetFiles(path);
            return files.Where(x => x.IndexOf(".jpg", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                       x.IndexOf(".png", StringComparison.OrdinalIgnoreCase) >= 0)
                           .Select(x => x).ToList();
        }

        private HashSet<string> loadTags(string path)
        {
            string[] files = Directory.GetFiles(path);
            var list =  files.Where(x => x.IndexOf(".txt", StringComparison.OrdinalIgnoreCase) >= 0)
                           .Select(x => x).ToList();
            string[] baseNames = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                baseNames[i] = Path.GetFileNameWithoutExtension(list[i]);
            }

            return baseNames.ToHashSet();
        }

        private string getTag(string path)
        {
            if (tagDic == null)
                return null;

            if (tagDic.Contains(Path.GetFileNameWithoutExtension(path))){
                return Path.ChangeExtension(path, "txt");
            }

            return null;
        }

        private List<string> compareLists(List<string> list1, List<string> list2)
        {
            var duplicates = list1.Where(path1 => list2.Any(path2 => Path.GetFileNameWithoutExtension(path1) == Path.GetFileNameWithoutExtension(path2))).ToList();
            return duplicates;
        }

        private void showImage(int idx)
        {
            this.currentIdx = idx;

            original_pic.Image = Image.FromFile(this.filteredOriginals[idx]);
            original_pic.SizeMode = PictureBoxSizeMode.Zoom;
            compare_pic.LoadAsync(this.filteredComparions[idx]);
            compare_pic.SizeMode = PictureBoxSizeMode.Zoom;
            var tagPath = getTag(this.filteredOriginals[idx]);

            if (tagPath != null)
            {
                tag_box.Text = System.IO.File.ReadAllText(tagPath);
                tag_box.Enabled = true;
            }
            else
            {
                tag_box.Text = null;
                tag_box.Enabled = false;
            }

            this.Text = $"{initialTitle} - {this.filteredComparions[idx]}({(idx+1).ToString()}/{this.filteredOriginals.Count.ToString()})";
        }

        private void startSlide()
        {
            if (this.originalImgs == null || this.comparisonImgs == null)
                return;

            this.filteredOriginals = compareLists(this.originalImgs, this.comparisonImgs);
            this.filteredComparions = compareLists(this.comparisonImgs, this.originalImgs);

            if (this.filteredOriginals.Count > 0)
            {
                showImage(0);
            }
        }

        private bool checkIsFolder(string path)
        {
            FileAttributes attr = System.IO.File.GetAttributes(path);
            return attr.HasFlag(FileAttributes.Directory);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                saveTags();
            }

            // Doesn't work on editing tag.
            if (tag_box.Focused)
                return;

            if (e.KeyCode == Keys.Left)
            {
                if (currentIdx >= 1)
                {
                    this.currentIdx--;
                    showImage(this.currentIdx);
                }
            }

            if (e.KeyCode == Keys.Right)
            {
                if (currentIdx < this.filteredOriginals.Count - 1)
                {
                    this.currentIdx++;
                    showImage(this.currentIdx);
                }
            }

            if (e.KeyCode == Keys.Delete)
            {
                var tmp = compare_pic.Image;
                compare_pic.Image = null;
                tmp.Dispose();

                lock (thisLock)
                {
                    delReserved = filteredComparions[currentIdx];
                }

                filteredOriginals.RemoveAt(currentIdx);
                filteredComparions.RemoveAt(currentIdx);

                if (filteredComparions.Count <= 0)
                {
                    originalImgs = null;
                    comparisonImgs = null;
                    filteredOriginals = null;
                    filteredComparions = null;
                    currentIdx = -1;
                    tagDic = null;

                    original_pic.Image = null;
                    compare_pic.Image = null;
                    return;
                }

                //if (currentIdx == filteredComparions.Count - 1)
                //{
                //    currentIdx--;
                //}
                //else
                //{
                //    currentIdx++;
                //}
                showImage(currentIdx);
            }
        }

        private void orginal_img_txt_folder_DragDrop(object sender, DragEventArgs e)
        {
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (checkIsFolder(filePaths[0]))
                orginal_img_txt_folder.Text = filePaths[0];
        }

        private void orginal_img_txt_folder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void compare_txt_folder_DragDrop(object sender, DragEventArgs e)
        {
            string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (checkIsFolder(filePaths[0]))
                compare_txt_folder.Text = filePaths[0];
        }

        private void compare_txt_folder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void orginal_img_txt_folder_TextChanged(object sender, EventArgs e)
        {
            this.originalImgs = loadimages(orginal_img_txt_folder.Text);
            this.tagDic = loadTags(orginal_img_txt_folder.Text);
            startSlide();
        }

        private void compare_txt_folder_TextChanged(object sender, EventArgs e)
        {
            this.comparisonImgs = loadimages(compare_txt_folder.Text);
            startSlide();
        }

        private void compare_pic_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (delReserved != null)
            {
                lock (thisLock)
                {
                    // System.IO.File.Delete(delReserved);
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(delReserved,
                    Microsoft.VisualBasic.FileIO.UIOption.AllDialogs,
                    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    delReserved = null;
                }
            }
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            saveTags();
        }

        private void saveTags()
        {
            if (filteredOriginals != null)
            {
                using (StreamWriter outputFile = new StreamWriter(getTag(this.filteredOriginals[currentIdx]), false))
                {
                    outputFile.Write(tag_box.Text);
                }
                MessageBox.Show("Successfully saved");
            }
        }
    }
}
