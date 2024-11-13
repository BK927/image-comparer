using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

        private class UndoAction
        {
            public int Index { get; set; }
            public ImagePair ImagePair { get; set; }
            public string TempFilePath { get; set; }
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

        private readonly Dictionary<string, Image> imageCache = new Dictionary<string, Image>();
        private CancellationTokenSource loadingCancellation = new CancellationTokenSource();
        private const int PRELOAD_COUNT = 2;
        private readonly Stack<UndoAction> undoStack = new Stack<UndoAction>();

        public Form1()
        {
            InitializeComponent();
            SetupMessageFilter();
            SetupFileWatchers();
            LoadQuickTags();

            this.initialTitle = this.Text;
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // 임시 폴더 정리
            try
            {
                string tempFolder = Path.Combine(Path.GetTempPath(), "ImageComparerDeleted");
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"임시 파일 정리 중 오류: {ex.Message}");
            }

            // 기존 리소스 정리 코드...
            loadingCancellation.Cancel();
            foreach (var image in imageCache.Values)
            {
                image?.Dispose();
            }
            imageCache.Clear();
        }


        private void SetupMessageFilter()
        {
            var messageFilter = new MiddleMouseMessageFilter(quick_tag_box);
            messageFilter.MiddleMouseClick += async (s, e) => {
                Debug.WriteLine($"Middle click detected - Current index before delete: {currentIndex}");
                Debug.WriteLine($"Current image path: {imagePairs[currentIndex].ComparisonPath}");

                // 현재 마우스 위치의 컨트롤 확인
                Point cursorPos = Cursor.Position;
                Control control = this.GetChildAtPoint(this.PointToClient(cursorPos));
                Debug.WriteLine($"Clicked control: {control?.Name ?? "none"}");

                await DeleteCurrentImageAsync();
            };
            Application.AddMessageFilter(messageFilter);
        }

        private void SetupFileWatchers()
        {
            original_watcher.Created += async (s, e) => await RefreshImagesAsync(currentIndex);
            original_watcher.Deleted += async (s, e) => await RefreshImagesAsync(currentIndex);
            original_watcher.Renamed += async (s, e) => await RefreshImagesAsync(currentIndex);

            compare_watcher.Created += async (s, e) => await RefreshImagesAsync(currentIndex);
            compare_watcher.Deleted += async (s, e) => await RefreshImagesAsync(currentIndex);
            compare_watcher.Renamed += async (s, e) => await RefreshImagesAsync(currentIndex);

        }

        private string GetCacheKey(int index, string path)
        {
            return $"{index}_{path}";
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

        private async Task RefreshImagesAsync(int? indexToLoad = null)
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

            if (imagePairs.Any())
            {
                int indexToUse = indexToLoad ?? currentIndex;
                if (indexToUse < 0 || indexToUse >= imagePairs.Count)
                    indexToUse = 0;

                Debug.WriteLine($"이미지를 새로 고침했습니다. 인덱스 {indexToUse}의 이미지를 로드합니다.");
                await LoadImagePairAsync(indexToUse);
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
            {
                Debug.WriteLine($"Invalid index: {index}, Count: {imagePairs.Count}");
                return;
            }

            try
            {
                // 이전 작업 취소
                var oldCancellation = loadingCancellation;
                loadingCancellation = new CancellationTokenSource();
                oldCancellation?.Cancel();
                oldCancellation?.Dispose();

                currentIndex = index;
                var pair = imagePairs[index];

                // 파일 존재 여부 확인
                if (!File.Exists(pair.OriginalPath) || !File.Exists(pair.ComparisonPath))
                {
                    MessageBox.Show($"이미지 파일을 찾을 수 없습니다.\nOriginal: {pair.OriginalPath}\nComparison: {pair.ComparisonPath}",
                        "파일 없음", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // UI 업데이트 시작
                this.UseWaitCursor = true;

                // 이미지 로드
                await Task.WhenAll(
                    LoadImageAsync(original_pic, pair.OriginalPath, loadingCancellation.Token),
                    LoadImageAsync(compare_pic, pair.ComparisonPath, loadingCancellation.Token)
                );

                if (!loadingCancellation.Token.IsCancellationRequested)
                {
                    // 태그 처리
                    if (pair.TagFilePath != null && File.Exists(pair.TagFilePath))
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

                    this.Text = $"{initialTitle} - {pair.ComparisonPath} ({index + 1}/{imagePairs.Count})";

                    // 주변 이미지 미리 로딩
                    _ = PreloadImagesAsync(index, loadingCancellation.Token);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 로딩 중 오류: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.UseWaitCursor = false;
            }
        }

        private async Task PreloadImagesAsync(int currentIndex, CancellationToken token)
        {
            try
            {
                // 미리 로드할 인덱스 범위 계산
                var startIndex = Math.Max(0, currentIndex - PRELOAD_COUNT);
                var endIndex = Math.Min(imagePairs.Count - 1, currentIndex + PRELOAD_COUNT);

                var preloadIndices = Enumerable.Range(startIndex, endIndex - startIndex + 1)
                    .Where(i => i != currentIndex);  // 현재 인덱스 제외

                foreach (var index in preloadIndices)
                {
                    if (token.IsCancellationRequested)
                        return;

                    // 유효한 인덱스 재확인
                    if (index >= 0 && index < imagePairs.Count)
                    {
                        var pair = imagePairs[index];
                        if (pair != null)
                        {
                            await Task.WhenAll(
                                PreloadImageAsync(pair.OriginalPath, index, "original", token),
                                PreloadImageAsync(pair.ComparisonPath, index, "comparison", token)
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 미리 로딩 중 오류는 로그만 남기고 진행
                Debug.WriteLine($"Preload error: {ex.Message}");
            }
        }

        private async Task PreloadImageAsync(string path, int index, string type, CancellationToken token)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return;

            try
            {
                if (!imageCache.ContainsKey(path))
                {
                    byte[] imageBytes = await Task.Run(() => File.ReadAllBytes(path), token);
                    if (!token.IsCancellationRequested)
                    {
                        var image = await Task.Run(() => {
                            using (var ms = new MemoryStream(imageBytes))
                            {
                                return new Bitmap(Image.FromStream(ms, true, true));
                            }
                        }, token);

                        if (!token.IsCancellationRequested && image != null)
                        {
                            lock (imageCache)
                            {
                                if (!imageCache.ContainsKey(path))
                                {
                                    imageCache[path] = image;
                                }
                                else
                                {
                                    image.Dispose();
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // 취소된 경우 무시
            }
            catch (Exception)
            {
                // 미리 로딩 실패는 무시
            }
        }


        private void ClearCurrentImages()
        {
            try
            {
                var oldOriginal = original_pic.Image;
                var oldCompare = compare_pic.Image;

                original_pic.Image = null;
                compare_pic.Image = null;

                oldOriginal?.Dispose();
                oldCompare?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ClearCurrentImages error: {ex.Message}");
            }
        }

        // 캐시 관리
        private void CleanupCache(int currentIndex)
        {
            try
            {
                // 현재 표시 중인 이미지와 주변 이미지의 경로를 수집
                var pathsToKeep = new HashSet<string>();

                // 현재 인덱스의 이미지 쌍
                if (currentIndex >= 0 && currentIndex < imagePairs.Count)
                {
                    var currentPair = imagePairs[currentIndex];
                    pathsToKeep.Add(currentPair.OriginalPath);
                    pathsToKeep.Add(currentPair.ComparisonPath);
                }

                // 주변 이미지의 경로 수집
                for (int i = 1; i <= PRELOAD_COUNT; i++)
                {
                    int prevIndex = currentIndex - i;
                    int nextIndex = currentIndex + i;

                    if (prevIndex >= 0)
                    {
                        var prevPair = imagePairs[prevIndex];
                        pathsToKeep.Add(prevPair.OriginalPath);
                        pathsToKeep.Add(prevPair.ComparisonPath);
                    }

                    if (nextIndex < imagePairs.Count)
                    {
                        var nextPair = imagePairs[nextIndex];
                        pathsToKeep.Add(nextPair.OriginalPath);
                        pathsToKeep.Add(nextPair.ComparisonPath);
                    }
                }

                // 제거할 키 수집
                var keysToRemove = imageCache.Keys.Where(k => !pathsToKeep.Contains(k)).ToList();

                // 캐시에서 제거
                foreach (var key in keysToRemove)
                {
                    if (imageCache.TryGetValue(key, out var image))
                    {
                        image.Dispose();
                        imageCache.Remove(key);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"캐시 정리 중 오류: {ex.Message}");
            }
        }


        private void DisposeCache()
        {
            try
            {
                foreach (var image in imageCache.Values)
                {
                    image?.Dispose();
                }
                imageCache.Clear();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DisposeCache error: {ex.Message}");
            }
        }

        private async Task LoadImageAsync(PictureBox pictureBox, string path, CancellationToken token)
        {
            Debug.WriteLine($"LoadImageAsync 시작 - Path: {path}");
            try
            {
                // 캐시 확인
                Debug.WriteLine($"캐시 확인 - Path: {path}");

                Image imageToSet = null;

                if (imageCache.TryGetValue(path, out var cachedImage))
                {
                    Debug.WriteLine("캐시된 이미지 발견");
                    // 캐시된 이미지의 복사본 생성
                    imageToSet = new Bitmap(cachedImage);
                    Debug.WriteLine("캐시된 이미지 복제 완료");
                }
                else
                {
                    Debug.WriteLine("파일에서 이미지 로드 시작");
                    byte[] imageBytes = await Task.Run(() => {
                        Debug.WriteLine($"ReadAllBytes 시작 - {path}");
                        var bytes = File.ReadAllBytes(path);
                        Debug.WriteLine($"ReadAllBytes 완료 - 크기: {bytes.Length}");
                        return bytes;
                    }, token);

                    if (token.IsCancellationRequested) return;

                    Debug.WriteLine("이미지 생성 시작");
                    imageToSet = await Task.Run(() => {
                        try
                        {
                            using (var ms = new MemoryStream(imageBytes))
                            using (var tempImage = Image.FromStream(ms, true, true))
                            {
                                Debug.WriteLine($"이미지 생성 완료 - 크기: {tempImage.Width}x{tempImage.Height}");
                                // 캐시용 복사본 생성
                                var cacheImage = new Bitmap(tempImage);
                                // PictureBox용 복사본 생성
                                var displayImage = new Bitmap(tempImage);

                                // 캐시에 저장
                                lock (imageCache)
                                {
                                    imageCache[path] = cacheImage;
                                }

                                return displayImage;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"이미지 생성 중 오류: {ex.Message}");
                            throw;
                        }
                    }, token);
                }

                if (token.IsCancellationRequested)
                {
                    imageToSet?.Dispose();
                    return;
                }

                Debug.WriteLine("PictureBox에 이미지 설정 시작");
                var oldPicture = pictureBox.Image;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                // UI 스레드에서 이미지 설정
                if (pictureBox.InvokeRequired)
                {
                    await pictureBox.InvokeAsync(() =>
                    {
                        if (!token.IsCancellationRequested)
                        {
                            pictureBox.Image = imageToSet;
                            oldPicture?.Dispose();
                        }
                        else
                        {
                            imageToSet?.Dispose();
                        }
                    });
                }
                else
                {
                    if (!token.IsCancellationRequested)
                    {
                        pictureBox.Image = imageToSet;
                        oldPicture?.Dispose();
                    }
                    else
                    {
                        imageToSet?.Dispose();
                    }
                }

                Debug.WriteLine("LoadImageAsync 완료");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadImageAsync 오류: {ex.Message}\n{ex.StackTrace}");
                if (!token.IsCancellationRequested)
                {
                    throw new Exception($"이미지 '{path}' 로딩 실패: {ex.Message}", ex);
                }
            }
        }



        private Size CalculateTargetSize(Size originalSize, Size containerSize)
        {
            double ratioX = (double)containerSize.Width / originalSize.Width;
            double ratioY = (double)containerSize.Height / originalSize.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalSize.Width * ratio);
            int newHeight = (int)(originalSize.Height * ratio);

            return new Size(newWidth, newHeight);
        }

        private async Task DeleteCurrentImageAsync()
        {
            if (currentIndex < 0 || currentIndex >= imagePairs.Count)
                return;

            Debug.WriteLine($"Deleting image at index: {currentIndex}");

            try
            {
                // 파일 감시자 비활성화
                original_watcher.EnableRaisingEvents = false;
                compare_watcher.EnableRaisingEvents = false;

                var pair = imagePairs[currentIndex];

                // 이미지 파일을 임시 폴더로 이동
                string tempFolder = Path.Combine(Path.GetTempPath(), "ImageComparerDeleted");
                Directory.CreateDirectory(tempFolder);
                string tempFilePath = Path.Combine(tempFolder, Path.GetFileName(pair.ComparisonPath));

                // 동일한 이름의 임시 파일이 있으면 삭제
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }

                // 파일 이동
                File.Move(pair.ComparisonPath, tempFilePath);

                // Undo 정보를 스택에 저장
                var undoAction = new UndoAction
                {
                    Index = currentIndex,
                    ImagePair = pair,
                    TempFilePath = tempFilePath
                };

                undoStack.Push(undoAction);

                // 이미지 쌍 제거
                lock (lockObject)
                {
                    imagePairs.RemoveAt(currentIndex);
                }

                // 캐시에서 이미지 제거
                lock (imageCache)
                {
                    if (imageCache.ContainsKey(pair.OriginalPath))
                    {
                        imageCache[pair.OriginalPath]?.Dispose();
                        imageCache.Remove(pair.OriginalPath);
                    }
                    if (imageCache.ContainsKey(pair.ComparisonPath))
                    {
                        imageCache[pair.ComparisonPath]?.Dispose();
                        imageCache.Remove(pair.ComparisonPath);
                    }
                }

                // currentIndex 조정
                if (imagePairs.Count == 0)
                {
                    ClearImages();
                    currentIndex = -1;
                    Debug.WriteLine("이미지가 더 이상 없습니다. currentIndex를 -1로 설정합니다.");
                }
                else if (currentIndex >= imagePairs.Count)
                {
                    currentIndex = imagePairs.Count - 1;
                    Debug.WriteLine($"currentIndex를 마지막 인덱스로 조정: {currentIndex}");
                }

                Debug.WriteLine($"삭제 후 인덱스 {currentIndex}의 이미지를 로드합니다.");
                await LoadImagePairAsync(currentIndex);

                // 타이틀 업데이트
                this.Text = $"{initialTitle} - Viewing {currentIndex + 1}/{imagePairs.Count}";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 삭제 중 오류: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 파일 감시자 다시 활성화
                original_watcher.EnableRaisingEvents = true;
                compare_watcher.EnableRaisingEvents = true;
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

            if (e.KeyCode == Keys.Z && e.Control)
            {
                await UndoDeleteAsync();
                return;
            }

            // 기존 키 처리 로직...
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

            try
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        if (currentIndex > 0)
                        {
                            await LoadImagePairAsync(currentIndex - 1);
                        }
                        break;

                    case Keys.Right:
                        if (currentIndex < imagePairs.Count - 1)
                        {
                            await LoadImagePairAsync(currentIndex + 1);
                        }
                        break;

                    case Keys.Delete:
                        await DeleteCurrentImageAsync();
                        break;
                }

                InvalidateQuickTagBox();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KeyUp error: {ex.Message}");
            }
        }

        private async Task UndoDeleteAsync()
        {
            if (undoStack.Count == 0)
            {
                MessageBox.Show("실행 취소할 작업이 없습니다.", "실행 취소",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var undoAction = undoStack.Pop();

            try
            {
                // 파일을 원래 위치로 이동
                File.Move(undoAction.TempFilePath, undoAction.ImagePair.ComparisonPath);

                // 이미지 쌍을 리스트에 원래 위치에 삽입
                lock (lockObject)
                {
                    if (undoAction.Index >= 0 && undoAction.Index <= imagePairs.Count)
                    {
                        imagePairs.Insert(undoAction.Index, undoAction.ImagePair);
                    }
                    else
                    {
                        imagePairs.Add(undoAction.ImagePair);
                    }
                }

                // 캐시에서 이미지 제거 (혹시 남아있을 수 있으므로)
                lock (imageCache)
                {
                    if (imageCache.ContainsKey(undoAction.ImagePair.OriginalPath))
                    {
                        imageCache[undoAction.ImagePair.OriginalPath]?.Dispose();
                        imageCache.Remove(undoAction.ImagePair.OriginalPath);
                    }
                    if (imageCache.ContainsKey(undoAction.ImagePair.ComparisonPath))
                    {
                        imageCache[undoAction.ImagePair.ComparisonPath]?.Dispose();
                        imageCache.Remove(undoAction.ImagePair.ComparisonPath);
                    }
                }

                // currentIndex 조정
                if (currentIndex >= undoAction.Index)
                {
                    currentIndex++;
                }
                else
                {
                    currentIndex = undoAction.Index;
                }

                // 이미지 로드
                await LoadImagePairAsync(currentIndex);

                // 타이틀 업데이트
                this.Text = $"{initialTitle} - Viewing {currentIndex + 1}/{imagePairs.Count}";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 복원 중 오류: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        protected override async void OnMouseWheel(MouseEventArgs e)
        {
            try
            {
                base.OnMouseWheel(e);

                if (imagePairs.Count == 0) return;

                int targetIndex = currentIndex;
                if (e.Delta > 0 && currentIndex > 0)
                {
                    targetIndex = currentIndex - 1;
                }
                else if (e.Delta < 0 && currentIndex < imagePairs.Count - 1)
                {
                    targetIndex = currentIndex + 1;
                }
                else
                {
                    return; // 이동할 수 없는 경우 무시
                }

                // 인덱스 유효성 한번 더 체크
                if (targetIndex >= 0 && targetIndex < imagePairs.Count)
                {
                    await LoadImagePairAsync(targetIndex);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MouseWheel error: {ex.Message}");
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
                        Debug.WriteLine("=== Copying Images ===");
                        Debug.WriteLine($"Total images in pair: {imagePairs.Count}");

                        await Task.Run(() =>
                        {
                            foreach (var pair in imagePairs)
                            {
                                var destPath = Path.Combine(folderDialog.SelectedPath,
                                    Path.GetFileName(pair.ComparisonPath));
                                Debug.WriteLine($"Copying: {Path.GetFileName(pair.ComparisonPath)}");
                                File.Copy(pair.ComparisonPath, destPath, true);
                            }
                        });

                        MessageBox.Show($"Files copied successfully. Total: {imagePairs.Count}", "Success",
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
                try
                {
                    // 캐시된 이미지들 정리
                    foreach (var image in imageCache.Values)
                    {
                        image?.Dispose();
                    }
                    imageCache.Clear();

                    // PictureBox 이미지 정리
                    if (original_pic?.Image != null)
                    {
                        original_pic.Image.Dispose();
                        original_pic.Image = null;
                    }
                    if (compare_pic?.Image != null)
                    {
                        compare_pic.Image.Dispose();
                        compare_pic.Image = null;
                    }

                    // 기타 컴포넌트 정리
                    components?.Dispose();
                    loadingCancellation?.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Dispose 중 오류: {ex.Message}");
                }
            }
            base.Dispose(disposing);
        }
    }
}