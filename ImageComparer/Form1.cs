using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using static System.Net.WebRequestMethods;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using System.Collections;
using System.Text.RegularExpressions;


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
        const string TAG_PATH = "tag.dat";

        int currentIdx = -1;

        private Object thisLock = new object();

        string initialTitle;

        public Form1()
        {
            InitializeComponent();

            this.original_img_btn.Click += Original_Img_Btn_Click;
            this.initialTitle = this.Text;
            tag_box.Text = string.Empty;

            if (System.IO.File.Exists(TAG_PATH))
            {
                string text = System.IO.File.ReadAllText(TAG_PATH);

                char[] separator = { ',' };

                // Split the string by commas
                string[] parts = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                // Trim the whitespace around each element
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = parts[i].Trim();
                    freq_tag_box.Items.Add(parts[i]);
                }
            }
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

            original_pic.Image = System.Drawing.Image.FromFile(this.filteredOriginals[idx]);
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
            if (e.KeyCode == Keys.Delete && freq_tag_box.Focused)
            {
                if (freq_tag_box.SelectedItem != null)
                {
                    // Remove the selected item
                    freq_tag_box.Items.Remove(freq_tag_box.SelectedItem);
                    save_quick_tags();
                }
                return;
            }

            // Doesn't work on editing tag.
            if (tag_box.Focused  || add_tag_box.Focused || freq_tag_box.Focused)
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
            MessageBox.Show("Successfully saved");
        }

        private void saveTags()
        {
            if (filteredOriginals != null)
            {
                using (StreamWriter outputFile = new StreamWriter(getTag(this.filteredOriginals[currentIdx]), false))
                {
                    outputFile.Write(tag_box.Text);
                }
            }
        }

        private void tag_box_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Suppress the newline character
                e.Handled = true;

                saveTags();
                MessageBox.Show("Successfully saved");
            }
        }

        private void save_quick_tags()
        {
            var itmes = string.Join(", ", freq_tag_box.Items.OfType<string>());
            using (StreamWriter tagFile = new StreamWriter(TAG_PATH))
            {
                tagFile.Write(itmes);
            }
        }

        private void add_tag_box_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Suppress the newline character
                e.Handled = true;

                freq_tag_box.Items.Add(add_tag_box.Text);
                add_tag_box.Text = string.Empty;

                save_quick_tags();
            }
        }

        private void quick_tag(MouseEventArgs e)
        {
            var selectedItem = freq_tag_box.SelectedItem.ToString();
            bool add = (e.Button == MouseButtons.Left); // Set to 'true' for addition, 'false' for subtraction

            if (freq_tag_box.SelectedItem != null && tag_box.Text != string.Empty)
            {
                char[] separator = { ',' };

                // Split the string by commas
                var parts = tag_box.Text.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                          .Select(part => part.Trim())
                          .ToList();

                string pattern = @"\(([^\(\):]+):([\d.]+)\)|([^\(\):]+)";

                string updatedItem = null;

                int matchedIndex = -1;

                for (int i = 0; i < parts.Count; i++)
                {
                    string item = parts[i];
                    Match match = Regex.Match(item, pattern);
                    string tag = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[3].Value;
                    string weight = match.Groups[2].Value;

                    if (tag.Equals(selectedItem))
                    {
                        if (string.IsNullOrEmpty(weight))
                        {
                            updatedItem = $"({tag}:1.1)";
                        }
                        else
                        {
                            double newWeight = add ? double.Parse(weight) + 0.1 : double.Parse(weight) - 0.1;
                            updatedItem = $"({tag}:{newWeight})";
                        }
                        matchedIndex = i;
                        break;
                    }
                }

                if (e.Button == MouseButtons.Middle)
                {
                    parts.RemoveAt(matchedIndex);
                }
                else
                {
                    if (updatedItem != null)
                    {
                        parts[matchedIndex] = updatedItem;
                    }
                    else
                    {
                        int middleIndex = parts.Count / 2;
                        parts.Insert(middleIndex, selectedItem);
                    }
                }
                
                tag_box.Text = string.Join(", ", parts);
                saveTags();
            }
        }

        private void freq_tag_box_MouseUp(object sender, MouseEventArgs e)
        {
            quick_tag(e);
        }
    }
}
