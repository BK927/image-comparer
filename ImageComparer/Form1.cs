using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace ImageComparer
{
    public partial class Form1 : Form
    {
        List<string> originalImgs = null;
        List<string> comparisonImgs = null;
        List<string> filteredOriginals = null;
        List<string> filteredComparions = null;
        HashSet<string> tagDic = null;
        string delReserved = null;
        const string TAG_FILE_PATH = "tag.dat";
        const string TAG_PATTERN = @"\(([^\(\):]+):([\d.]+)\)|([^\(\):]+)";

        int currentIdx = -1;

        private Object thisLock = new object();

        string initialTitle;

        public Form1()
        {
            InitializeComponent();
            // redraw quick_tag_box whenever tagLayoutPanel changed
            InitializeTagLayout();

            // Middle button delete
            MiddleMouseMessageFilter messageFilter = new MiddleMouseMessageFilter(quick_tag_box);
            messageFilter.MiddleMouseClick += MessageFilter_MiddleMouseClick;
            System.Windows.Forms.Application.AddMessageFilter(messageFilter);

            // ListBox coloring
            quick_tag_box.DrawMode = DrawMode.OwnerDrawFixed;
            quick_tag_box.DrawItem += quick_taglist_DrawItem;


            this.original_img_btn.Click += Original_Img_Btn_Click;
            this.initialTitle = this.Text;

            if (System.IO.File.Exists(TAG_FILE_PATH))
            {
                string text = System.IO.File.ReadAllText(TAG_FILE_PATH);

                char[] separator = { ',' };

                // Split the string by commas
                string[] parts = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                // Trim the whitespace around each element
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = parts[i].Trim();
                    quick_tag_box.Items.Add(parts[i]);
                }
            }
        }

        private void InitializeTagLayout()
        {
            FlowLayoutPanel originalFlowLayoutPanel = tagLayoutPanel;

            // Create a new instance of CustomFlowLayoutPanel
            TagFlowLayoutPanel customFlowLayoutPanel = new TagFlowLayoutPanel();

            // Set the properties of the CustomFlowLayoutPanel to match the original FlowLayoutPanel
            customFlowLayoutPanel.Location = originalFlowLayoutPanel.Location;
            customFlowLayoutPanel.Size = originalFlowLayoutPanel.Size;
            customFlowLayoutPanel.Anchor = originalFlowLayoutPanel.Anchor;
            customFlowLayoutPanel.Dock = originalFlowLayoutPanel.Dock;
            customFlowLayoutPanel.ChildTextChanged += InvalidateQuickTagBox;

            // Add any child controls from the original FlowLayoutPanel to the CustomFlowLayoutPanel
            foreach (Control control in originalFlowLayoutPanel.Controls)
            {
                customFlowLayoutPanel.Controls.Add(control);
            }

            // Replace the original FlowLayoutPanel with the CustomFlowLayoutPanel on the form
            this.Controls.Remove(originalFlowLayoutPanel);
            this.Controls.Add(customFlowLayoutPanel);
        }

        private void MessageFilter_MiddleMouseClick(object sender, MouseEventArgs e)
        {
             deleteImg();
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

        private string getTagFile(string path)
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
            var tagPath = getTagFile(this.filteredOriginals[idx]);

            if (tagPath != null)
            {
                var tags = splitTagsToList(System.IO.File.ReadAllText(tagPath));
                update_tags(tags);
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
            if (e.KeyCode == Keys.Delete && quick_tag_box.Focused)
            {
                if (quick_tag_box.SelectedItem != null)
                {
                    // Remove the selected item
                    quick_tag_box.Items.Remove(quick_tag_box.SelectedItem);
                    save_quick_tags();
                }
                return;
            }

            // Doesn't work on editing tag.
            if (add_quick_tag_box.Focused || quick_tag_box.Focused || add_tag_box.Focused)
                return;

            if (e.KeyCode == Keys.Left)
            {
                flipToPrevImg();
            }

            if (e.KeyCode == Keys.Right)
            {
                flipToNextImg();
            }

            if (e.KeyCode == Keys.Delete)
            {
                deleteImg();
            }

            InvalidateQuickTagBox(sender, e);
        }

        private void deleteImg()
        {
            var tmp = compare_pic.Image;
            if (tmp == null)
                return;

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

        private void saveTags()
        {
            if (filteredOriginals != null)
            {
                using (StreamWriter outputFile = new StreamWriter(getTagFile(this.filteredOriginals[currentIdx]), false))
                {
                    string text = string.Join(", ", tagLayoutPanel.Controls.OfType<Button>().Select(button => button.Text));
                    outputFile.Write(text);
                }
            }
        }

        private void save_quick_tags()
        {
            var itmes = string.Join(", ", quick_tag_box.Items.OfType<string>());
            using (StreamWriter tagFile = new StreamWriter(TAG_FILE_PATH))
            {
                tagFile.Write(itmes);
            }
        }

        private void add_quick_tag_box_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Suppress the newline character
                e.Handled = true;

                quick_tag_box.Items.Add(add_quick_tag_box.Text.Replace('_', ' '));
                add_quick_tag_box.Text = string.Empty;

                save_quick_tags();
            }
        }

        private void quick_tag_click(MouseEventArgs e)
        {
            if (quick_tag_box.SelectedItem == null)
                return;

            var selectedItem = quick_tag_box.SelectedItem.ToString();
            bool add = (e.Button == MouseButtons.Left); // Set to 'true' for addition, 'false' for subtraction

            if (tagLayoutPanel.Controls.Count > 0)
            {
                char[] separator = { ',' };

                // Split the string by commas
                var parts = tagLayoutPanel.Controls.OfType<Button>().Select(button => button.Text).ToList();

                string updatedItem = null;

                int matchedIndex = -1;


                for (int i = 0; i < parts.Count; i++)
                {
                    string item = parts[i];
                    Match match = Regex.Match(item, TAG_PATTERN);
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
                            updatedItem = newWeight == 1.0 ? tag : $"({tag}:{newWeight})";
                        }
                        matchedIndex = i;
                        break;
                    }
                }

                // Do not anything if there's no matched tag.
                if (updatedItem == null && e.Button == MouseButtons.Middle)
                {
                    return;
                }

                // If matched tag exist. Delete it.
                if (updatedItem != null && e.Button == MouseButtons.Middle)
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

                update_tags(parts);
            }
        }

        private void quick_tag_box_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                quick_tag_click(e);
            }
            
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (quick_tag_box.Focused)
                return;

            if (e.Delta > 0)
            {
                flipToPrevImg();
            }
            else if (e.Delta < 0)
            {
                flipToNextImg();
            }
        }

        protected void flipToPrevImg()
        {
            if (currentIdx >= 1)
            {
                this.currentIdx--;
                showImage(this.currentIdx);
            }
        }
        protected void flipToNextImg()
        {
            if (currentIdx < this.filteredOriginals.Count - 1)
            {
                this.currentIdx++;
                showImage(this.currentIdx);
            }
        }

        private void quick_tag_box_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            quick_tag_click(e);
        }

        private void quick_taglist_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            ListBox listBox = (ListBox)sender;
            var tags = getTags();

            // Change the background color based on the item index, state or other conditions
            Color backgroundColor;
            if (tags.Contains(quick_tag_box.Items[e.Index].ToString()))
            {
                backgroundColor = Color.LightBlue;
            }
            else
            {
                backgroundColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.Highlight : SystemColors.Window;
            }
            using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            }

            Brush textColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected ? SystemBrushes.HighlightText : SystemBrushes.ControlText;
            string itemText = listBox.Items[e.Index].ToString();
            e.Graphics.DrawString(itemText, e.Font, textColor, e.Bounds, StringFormat.GenericDefault);

            e.DrawFocusRectangle();
        }

        private List<string> splitTagsToList(string text)
        {
            char[] separator = { ',' };

            return text.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                          .Select(part => part.Trim())
                          .ToList();
        }

        private void update_tags(List<string> list)
        {
            var tagBox = add_tag_box;
            tagLayoutPanel.Controls.Clear();
            foreach (string tag in list)
            {
                var btn = new Button();
                btn.Text = tag;
                btn.AutoSize = true;
                btn.Click += tagBtn_Click;
                // Set the btn's text color
                btn.ForeColor = Color.FromArgb(255, 255, 255); // White

                // Set the btn's background color
                btn.BackColor = Color.FromArgb(26, 115, 232); // Blue

                // Set the btn's font size and style
                btn.Font = new Font(btn.Font.FontFamily, 12, FontStyle.Bold);

                // Add a border to the btn
                //btn.FlatStyle = FlatStyle.Flat;
                //btn.FlatAppearance.BorderSize = 2;
                btn.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255); // White

                // Add a hover effect to change the btn's background color
                btn.MouseEnter += (sender, e) =>
                {
                    ((Button)sender).BackColor = Color.FromArgb(15, 84, 193); // Darker Blue
                };
                btn.MouseLeave += (sender, e) =>
                {
                    ((Button)sender).BackColor = Color.FromArgb(26, 115, 232); // Original Blue
                };


                tagLayoutPanel.Controls.Add(btn);
            }
            tagLayoutPanel.Controls.Add(tagBox);

            // Auto save to tag.Dat
            saveTags();
        }

        private void tagBtn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                tagLayoutPanel.Controls.Remove(btn);
                btn.Dispose();
            }

            // Auto save to tag.Dat
            saveTags();
        }

        private List<string> getTags()
        {
            return tagLayoutPanel.Controls.OfType<Button>().Select(button => button.Text).ToList();
        }

        private void InvalidateQuickTagBox(object sender, EventArgs e)
        {
            quick_tag_box.Invalidate();
        }

        private void add_tag_box_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && (add_tag_box.Text != null || add_tag_box.Text != ""))
            {
                string text = add_tag_box.Text.Replace('_', ' ');
                // Suppress the newline character
                e.Handled = true;

                var tags = getTags();

                if (tags.Contains(text))
                    return;

                // Insert a new tag in the middle of the list
                int middleIndex = tags.Count / 2;
                tags.Insert(middleIndex, text);
                update_tags(tags);
                add_tag_box.Text = null;
            }
        }

        private void original_pic_Paint(object sender, PaintEventArgs e)
        {
            InvalidateQuickTagBox(sender, e);
        }

        private void compare_pic_Paint(object sender, PaintEventArgs e)
        {
            InvalidateQuickTagBox(sender, e);
        }
    }
}
