using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Potato64.Core;
using Potato64.Shell;
using Potato64.UI.Controls;

namespace Potato64.UI {
    public class MainForm : Form {
        private ScanResult? _lastResult;
        private ScanOptions _scanOptions = new();
        private CancellationTokenSource? _cts;
        private readonly ShellIconCache _iconCache = new();

        private TextBox _txtPath = null!;
        private Button _btnScan = null!;
        private FileTypePanel _fileTypePanel = null!;
        private DirectoryTreeView _treeView = null!;
        private FileListView _fileListView = null!;
        private DiskUsageBar _usageBar = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        public MainForm() {
            this.Text = "Potato64";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            try { if (File.Exists("assets/potato64.ico")) this.Icon = new Icon("assets/potato64.ico"); } catch { }

            InitializeLayout();
            _txtPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private void InitializeLayout() {
            // Top Bar
            var topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10), BackColor = Color.FromArgb(230, 230, 230) };
            topPanel.Controls.Add(new Label { Text = "PATH:", AutoSize = true, Margin = new Padding(0, 7, 0, 0) });
            _txtPath = new TextBox { Width = 400 };
            topPanel.Controls.Add(_txtPath);
            
            var btnBrowse = new Button { Text = "...", Width = 40 };
            btnBrowse.Click += (s, e) => { using var fbd = new FolderBrowserDialog(); if (fbd.ShowDialog() == DialogResult.OK) _txtPath.Text = fbd.SelectedPath; };
            topPanel.Controls.Add(btnBrowse);

            _btnScan = new Button { Text = "SCAN", Width = 100, FlatStyle = FlatStyle.Flat, BackColor = Color.White };
            _btnScan.Click += (s, e) => StartScan();
            topPanel.Controls.Add(_btnScan);
            this.Controls.Add(topPanel);

            // Bottom
            _usageBar = new DiskUsageBar { Dock = DockStyle.Bottom, Height = 60 };
            this.Controls.Add(_usageBar);

            var ss = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel("Ready");
            ss.Items.Add(_statusLabel);
            this.Controls.Add(ss);

            // Left
            var left = new Panel { Dock = DockStyle.Left, Width = 250, BorderStyle = BorderStyle.FixedSingle };
            _fileTypePanel = new FileTypePanel(_iconCache) { Dock = DockStyle.Fill };
            _fileTypePanel.ExtensionSelected += OnExtensionSelected;
            left.Controls.Add(_fileTypePanel);
            this.Controls.Add(left);

            // Right (Tree + List)
            var right = new Panel { Dock = DockStyle.Fill };
            _treeView = new DirectoryTreeView { Dock = DockStyle.Top, Height = 300 };
            _treeView.FolderSelected += OnFolderSelected;
            _fileListView = new FileListView(_iconCache) { Dock = DockStyle.Fill };
            right.Controls.Add(_fileListView);
            right.Controls.Add(_treeView);
            this.Controls.Add(right);
        }

        private async void StartScan() {
            if (!Directory.Exists(_txtPath.Text)) return;
            _btnScan.Enabled = false;
            _cts = new CancellationTokenSource();
            var scanner = new Scanner(new ScanOptions { RootPath = _txtPath.Text });
            scanner.ProgressChanged += p => { if (this.IsHandleCreated) this.BeginInvoke(new Action(() => _statusLabel.Text = p)); };
            try {
                _lastResult = await scanner.ScanAsync(_cts.Token);
                _fileTypePanel.LoadResult(_lastResult);
                _treeView.LoadResult(_lastResult);
                _fileListView.LoadFiles(_lastResult.RootFolder.Files);
                _usageBar.TotalBytes = _lastResult.DriveTotal;
                _usageBar.UsedBytes = _lastResult.TotalBytes;
                _statusLabel.Text = "Scan Complete.";
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { _btnScan.Enabled = true; }
        }

        private void OnFolderSelected(FolderEntry f) => _fileListView.LoadFiles(f.Files);
        private void OnExtensionSelected(string ext) {
            if (_lastResult == null) return;
            _fileListView.LoadFiles(CollectFiles(_lastResult.RootFolder, ext));
        }
        private List<FileEntry> CollectFiles(FolderEntry f, string e) {
            var l = f.Files.Where(x => x.Extension.Equals(e, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var s in f.SubFolders) l.AddRange(CollectFiles(s, e));
            return l;
        }
    }
}