using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageComparer
{
    public partial class Form1 : Form
    {
        private class ImagePair
        {
            public string OriginalPath { get; set; }
            public string ComparisonPath { get; set; }
            public string TagFilePath { get; set; }
        }

        private readonly List<ImagePair> imagePairs = new List<ImagePair>();
        private readonly HashSet<string> tagDictionary = new HashSet<string>();
        private static readonly string TAG_FILE_PATH = Path.Combine(
            Application.StartupPath, "tags.dat");
        private static readonly Regex TAG_PATTERN = new Regex(
            @"\(([^\(\):]+):([\d.]+)\)|([^\(\):]+)", RegexOptions.Compiled);

        private int currentIndex = -1;
        private readonly object lockObject = new object();
        private string deletionReserved;
        private readonly string initialTitle;
        private bool isLoadingImages;

        public Form1()
        {
            InitializeComponent();
            SetupMessageFilter();
            SetupFileWatchers();
            LoadQuickTags();

            this.initialTitle = this.Text;
        }

        private void SetupMessageFilter()
        {
            var messageFilter = new MiddleMouseMessageFilter(quick_tag_box);
            messageFilter.MiddleMouseClick += async (s, e) => await DeleteCurrentImageAsync();
            Application.AddMessageFilter(messageFilter);
        }

        private void SetupFileWatchers()
        {
            original_watcher.Created += async (s, e) => await RefreshImagesAsync();
            original_watcher.Deleted += async (s, e) => await RefreshImagesAsync();
            original_watcher.Renamed += async (s, e) => await RefreshImagesAsync();

            compare_watcher.Created += async (s, e) => await RefreshImagesAsync();
            compare_watcher.Deleted += async (s, e) => await RefreshImagesAsync();
            compare_watcher.Renamed += async (s, e) => await RefreshImagesAsync();
        }

        private void LoadQuickTags()
        {
            try
            {
                if (File.Exists(TAG_FILE_PATH))
                {
                    string[] tags = File.ReadAllText(TAG_FILE_PATH)
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim())
                        .ToArray();

                    quick_tag_box.BeginUpdate();
                    quick_tag_box.Items.AddRange(tags);
                    quick_tag_box.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"태그 로딩 중 오류: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void Original_Img_Btn_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new VistaFolderBrowserDialog())
            {
                folderDialog.Description = "원본 이미지 폴더 선택";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    orginal_img_txt_folder.Text = folderDialog.SelectedPath;
                    await RefreshImagesAsync();
                }
            }
        }

        private async Task RefreshImagesAsync()
        {
            if (isLoadingImages ||
                string.IsNullOrEmpty(orginal_img_txt_folder.Text) ||
                string.IsNullOrEmpty(compare_txt_folder.Text))
                return;

            try
            {
                isLoadingImages = true;
                UseWaitCursor = true;

                await Task.Run(() =>
                {
                    var originalFiles = GetImageFiles(orginal_img_txt_folder.Text);
                    var compareFiles = GetImageFiles(compare_txt_folder.Text);
                    var tagFiles = Directory.GetFiles(orginal_img_txt_folder.Text, "*.txt")
                        .Select(f => Path.GetFileNameWithoutExtension(f))
                        .ToHashSet();

                    lock (lockObject)
                    {
                        imagePairs.Clear();
                        foreach (var originalFile in originalFiles)
                        {
                            var fileName = Path.GetFileName(originalFile);
                            var comparePath = compareFiles
                                .FirstOrDefault(f => Path.GetFileName(f) == fileName);

                            if (comparePath != null)
                            {
                                var baseName = Path.GetFileNameWithoutExtension(originalFile);
                                var tagPath = tagFiles.Contains(baseName)
                                    ? Path.ChangeExtension(originalFile, "txt")
                                    : null;

                                imagePairs.Add(new ImagePair
                                {
                                    OriginalPath = originalFile,
                                    ComparisonPath = comparePath,
                                    TagFilePath = tagPath
                                });
                            }
                        }
                    }
                });

                if (imagePairs.Any())
                {
                    await LoadImagePairAsync(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 로딩 중 오류: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoadingImages = false;
                UseWaitCursor = false;
            }
        }

        private IEnumerable<string> GetImageFiles(string path)
        {
            return Directory.GetFiles(path)
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".png", StringComparison.OrdinalIgnoreCase));
        }

        private async Task LoadImagePairAsync(int index)
        {
            if (index < 0 || index >= imagePairs.Count)
                return;

            try
            {
                currentIndex = index;
                var pair = imagePairs[index];

                await Task.WhenAll(
                    LoadImageAsync(original_pic, pair.OriginalPath),
                    LoadImageAsync(compare_pic, pair.ComparisonPath)
                );

                if (pair.TagFilePath != null)
                {
                    var tags = await Task.Run(() =>
                        File.ReadAllText(pair.TagFilePath)
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim())
                            .ToList());

                    UpdateTags(tags);
                }
                else
                {
                    UpdateTags(new List<string>());
                }

                this.Text = $"{initialTitle} - {pair.ComparisonPath}" +
                    $"({index + 1}/{imagePairs.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 로딩 중 오류: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadImageAsync(PictureBox pictureBox, string path)
        {
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var image = await Task.Run(() => Image.FromStream(stream));
                    var oldImage = pictureBox.Image;
                    // PictureBox의 SizeMode를 Zoom으로 설정
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox.Image = image;

                    oldImage?.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"이미지 '{path}' 로딩 실패: {ex.Message}");
            }
        }

        private async Task DeleteCurrentImageAsync()
        {
            if (currentIndex < 0 || currentIndex >= imagePairs.Count)
                return;

            try
            {
                var pair = imagePairs[currentIndex];
                var oldImage = compare_pic.Image;
                compare_pic.Image = null;
                oldImage?.Dispose();

                lock (lockObject)
                {
                    deletionReserved = pair.ComparisonPath;
                    imagePairs.RemoveAt(currentIndex);
                }

                if (imagePairs.Count == 0)
                {
                    ClearImages();
                    return;
                }

                if (currentIndex >= imagePairs.Count)
                {
                    currentIndex = imagePairs.Count - 1;
                }

                await LoadImagePairAsync(currentIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 삭제 중 오류: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearImages()
        {
            var oldOriginal = original_pic.Image;
            var oldCompare = compare_pic.Image;

            original_pic.Image = null;
            compare_pic.Image = null;

            oldOriginal?.Dispose();
            oldCompare?.Dispose();

            currentIndex = -1;
            this.Text = initialTitle;
        }

        private void UpdateTags(List<string> tags)
        {
            tagLayoutPanel.SuspendLayout();
            foreach (Control control in tagLayoutPanel.Controls)
            {
                if (control != add_tag_box)
                    control.Dispose();
            }
            tagLayoutPanel.Controls.Clear();

            foreach (string tag in tags)
            {
                var button = CreateTagButton(tag);
                tagLayoutPanel.Controls.Add(button);
            }

            tagLayoutPanel.Controls.Add(add_tag_box);
            tagLayoutPanel.ResumeLayout();

            SaveTags();
            InvalidateQuickTagBox();
        }

        private Button CreateTagButton(string text)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = true,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(26, 115, 232),
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            button.FlatAppearance.BorderColor = Color.White;
            button.Click += (s, e) =>
            {
                tagLayoutPanel.Controls.Remove(button);
                button.Dispose();
                SaveTags();
            };

            button.MouseEnter += (s, e) =>
                button.BackColor = Color.FromArgb(15, 84, 193);
            button.MouseLeave += (s, e) =>
                button.BackColor = Color.FromArgb(26, 115, 232);

            return button;
        }

        private void SaveTags()
        {
            if (currentIndex < 0 || currentIndex >= imagePairs.Count)
                return;

            try
            {
                var pair = imagePairs[currentIndex];
                if (pair.TagFilePath != null)
                {
                    string tags = string.Join(", ",
                        tagLayoutPanel.Controls.OfType<Button>()
                            .Select(b => b.Text));
                    File.WriteAllText(pair.TagFilePath, tags);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"태그 저장 중 오류: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveQuickTags()
        {
            try
            {
                var tags = string.Join(", ", quick_tag_box.Items.Cast<string>());
                File.WriteAllText(TAG_FILE_PATH, tags);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"빠른 태그 저장 중 오류: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override async void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.Delete && quick_tag_box.Focused)
            {
                if (quick_tag_box.SelectedItem != null)
                {
                    quick_tag_box.Items.Remove(quick_tag_box.SelectedItem);
                    SaveQuickTags();
                }
                return;
            }

            if (add_quick_tag_box.Focused || quick_tag_box.Focused || add_tag_box.Focused)
                return;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (currentIndex > 0)
                        await LoadImagePairAsync(currentIndex - 1);
                    break;

                case Keys.Right:
                    if (currentIndex < imagePairs.Count - 1)
                        await LoadImagePairAsync(currentIndex + 1);
                    break;

                case Keys.Delete:
                    await DeleteCurrentImageAsync();
                    break;
            }

            InvalidateQuickTagBox();
        }

        protected override async void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta > 0 && currentIndex > 0)
            {
                await LoadImagePairAsync(currentIndex - 1);
            }
            else if (e.Delta < 0 && currentIndex < imagePairs.Count - 1)
            {
                await LoadImagePairAsync(currentIndex + 1);
            }
        }

        private async void compare_pic_LoadCompleted(object sender,
            System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (deletionReserved != null)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        lock (lockObject)
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                                deletionReserved,
                                Microsoft.VisualBasic.FileIO.UIOption.AllDialogs,
                                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                            deletionReserved = null;
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"파일 삭제 중 오류: {ex.Message}", "오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void quick_tag_box_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                ProcessQuickTag(e);
            }
        }

        private void ProcessQuickTag(MouseEventArgs e)
        {
            if (quick_tag_box.SelectedItem == null)
                return;

            var selectedItem = quick_tag_box.SelectedItem.ToString();
            bool isAdd = (e.Button == MouseButtons.Left);

            var currentTags = tagLayoutPanel.Controls.OfType<Button>()
                .Select(button => button.Text)
                .ToList();

            var matchedTag = currentTags.Select((tag, index) => new { Tag = tag, Index = index })
                .FirstOrDefault(t =>
                {
                    var match = TAG_PATTERN.Match(t.Tag);
                    var tagText = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[3].Value;
                    return tagText.Equals(selectedItem);
                });

            if (e.Button == MouseButtons.Middle)
            {
                if (matchedTag != null)
                {
                    currentTags.RemoveAt(matchedTag.Index);
                    UpdateTags(currentTags);
                }
                return;
            }

            string updatedTag;
            if (matchedTag != null)
            {
                var match = TAG_PATTERN.Match(matchedTag.Tag);
                var tagText = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[3].Value;
                var weight = match.Groups[2].Success ? double.Parse(match.Groups[2].Value) : 1.0;

                weight = isAdd ? weight + 0.1 : weight - 0.1;
                updatedTag = weight == 1.0 ? tagText : $"({tagText}:{weight:F1})";

                currentTags[matchedTag.Index] = updatedTag;
            }
            else
            {
                var insertIndex = currentTags.Count / 2;
                currentTags.Insert(insertIndex, selectedItem);
            }

            UpdateTags(currentTags);
        }

        private void add_quick_tag_box_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
                return;

            e.Handled = true;
            var newTag = add_quick_tag_box.Text.Replace('_', ' ').Trim();

            if (string.IsNullOrWhiteSpace(newTag))
                return;

            if (!quick_tag_box.Items.Contains(newTag))
            {
                quick_tag_box.Items.Add(newTag);
                SaveQuickTags();
            }

            add_quick_tag_box.Text = string.Empty;
        }

        private void add_tag_box_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
                return;

            e.Handled = true;
            var newTag = add_tag_box.Text.Replace('_', ' ').Trim();

            if (string.IsNullOrWhiteSpace(newTag))
                return;

            var currentTags = tagLayoutPanel.Controls.OfType<Button>()
                .Select(b => b.Text)
                .ToList();

            if (!currentTags.Contains(newTag))
            {
                var insertIndex = currentTags.Count / 2;
                currentTags.Insert(insertIndex, newTag);
                UpdateTags(currentTags);
            }

            add_tag_box.Text = string.Empty;
        }

        private void quick_taglist_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            var currentTags = tagLayoutPanel.Controls.OfType<Button>()
                .Select(b =>
                {
                    var match = TAG_PATTERN.Match(b.Text);
                    return match.Groups[1].Success ? match.Groups[1].Value : match.Groups[3].Value;
                })
                .ToHashSet();

            string itemText = quick_tag_box.Items[e.Index].ToString();
            Color backgroundColor;

            if (currentTags.Contains(itemText))
            {
                backgroundColor = Color.LightBlue;
            }
            else
            {
                backgroundColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                    ? SystemColors.Highlight
                    : SystemColors.Window;
            }

            using (var brush = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            var textColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                ? SystemBrushes.HighlightText
                : SystemBrushes.ControlText;

            e.Graphics.DrawString(itemText, e.Font, textColor,
                e.Bounds, StringFormat.GenericDefault);

            e.DrawFocusRectangle();
        }

        private void InvalidateQuickTagBox()
        {
            if (!IsDisposed && !quick_tag_box.IsDisposed)
            {
                quick_tag_box.Invalidate();
            }
        }

        private async void copyCurrentWorksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new VistaFolderBrowserDialog())
            {
                folderDialog.Description = "Select destination folder";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        UseWaitCursor = true;
                        await Task.Run(() =>
                        {
                            foreach (var pair in imagePairs)
                            {
                                var destPath = Path.Combine(folderDialog.SelectedPath,
                                    Path.GetFileName(pair.ComparisonPath));
                                File.Copy(pair.ComparisonPath, destPath, true);
                            }
                        });
                        MessageBox.Show("Files copied successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying files: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        UseWaitCursor = false;
                    }
                }
            }
        }

        private async void copyImagesToReworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(orginal_img_txt_folder.Text))
            {
                MessageBox.Show("Please select original images folder first.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var folderDialog = new VistaFolderBrowserDialog())
            {
                folderDialog.Description = "Select destination folder for rework images";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        UseWaitCursor = true;
                        await Task.Run(() =>
                        {
                            var originalFiles = GetImageFiles(orginal_img_txt_folder.Text);
                            var comparisonFiles = imagePairs.Select(p =>
                                Path.GetFileName(p.ComparisonPath)).ToHashSet();

                            foreach (var originalFile in originalFiles)
                            {
                                if (!comparisonFiles.Contains(Path.GetFileName(originalFile)))
                                {
                                    var destPath = Path.Combine(folderDialog.SelectedPath,
                                        Path.GetFileName(originalFile));
                                    File.Copy(originalFile, destPath, true);
                                }
                            }
                        });
                        MessageBox.Show("Rework files copied successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying rework files: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        UseWaitCursor = false;
                    }
                }
            }
        }

        private void quick_tag_box_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ProcessQuickTag(e);
        }

        private void orginal_img_txt_folder_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(orginal_img_txt_folder.Text))
                {
                    original_watcher.Path = orginal_img_txt_folder.Text;
                    original_watcher.EnableRaisingEvents = true;
                    _ = RefreshImagesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting folder: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void orginal_img_txt_folder_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files?.Length > 0 && Directory.Exists(files[0]))
            {
                orginal_img_txt_folder.Text = files[0];
            }
        }

        private void orginal_img_txt_folder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private async void compare_img_btn_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new VistaFolderBrowserDialog())
            {
                folderDialog.Description = "Select comparison folder";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    compare_txt_folder.Text = folderDialog.SelectedPath;
                    await RefreshImagesAsync();
                }
            }
        }

        private void compare_txt_folder_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(compare_txt_folder.Text))
                {
                    compare_watcher.Path = compare_txt_folder.Text;
                    compare_watcher.EnableRaisingEvents = true;
                    _ = RefreshImagesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting folder: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void compare_txt_folder_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files?.Length > 0 && Directory.Exists(files[0]))
            {
                compare_txt_folder.Text = files[0];
            }
        }

        private void compare_txt_folder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void PictureBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    string ext = Path.GetExtension(files[0]).ToLower();
                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private async void Original_PictureBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files?.Length > 0)
                {
                    string filePath = files[0];
                    string folderPath = Path.GetDirectoryName(filePath);

                    // 폴더 경로 설정
                    orginal_img_txt_folder.Text = folderPath;

                    // 현재 이미지를 드롭된 이미지로 설정
                    await LoadImagePairByFileNameAsync(Path.GetFileName(filePath));
                }
            }
        }

        private async void Compare_PictureBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files?.Length > 0)
                {
                    string filePath = files[0];
                    string folderPath = Path.GetDirectoryName(filePath);

                    // 폴더 경로 설정
                    compare_txt_folder.Text = folderPath;

                    // 현재 이미지를 드롭된 이미지로 설정
                    await LoadImagePairByFileNameAsync(Path.GetFileName(filePath));
                }
            }
        }

        // 파일 이름으로 이미지 쌍을 찾아 로드하는 메서드 추가
        private async Task LoadImagePairByFileNameAsync(string fileName)
        {
            try
            {
                var pair = imagePairs.FirstOrDefault(p =>
                    Path.GetFileName(p.OriginalPath) == fileName ||
                    Path.GetFileName(p.ComparisonPath) == fileName);

                if (pair != null)
                {
                    int index = imagePairs.IndexOf(pair);
                    await LoadImagePairAsync(index);
                }
                else
                {
                    // 이미지 쌍을 찾지 못한 경우 새로고침
                    await RefreshImagesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                original_pic.Image?.Dispose();
                compare_pic.Image?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}