using System.Windows.Forms;
using Potato64.Core;

namespace Potato64.UI
{
    public class ScanOptionsDialog : Form
    {
        private readonly CheckBox  _includeHidden;
        private readonly CheckBox  _includeSystem;
        private readonly CheckBox  _followSymlinks;
        private readonly NumericUpDown _minSize;
        private readonly ListBox   _excludeList;
        private readonly Button    _addExclude;
        private readonly Button    _removeExclude;
        private readonly Button    _btnScan;
        private readonly Button    _btnCancel;

        public ScanOptions Result { get; private set; } = new();

        public ScanOptionsDialog(ScanOptions current)
        {
            Text            = "Potato64 - Scan Options";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            Width           = 340;
            Height          = 320;
            StartPosition   = FormStartPosition.CenterParent;
            Font            = SystemFonts.DefaultFont;

            _includeHidden = new CheckBox
            {
                Text     = "Include hidden files",
                Checked  = current.IncludeHiddenFiles,
                Location = new System.Drawing.Point(12, 12),
                Width    = 280
            };

            _includeSystem = new CheckBox
            {
                Text     = "Include system files",
                Checked  = current.IncludeSystemFiles,
                Location = new System.Drawing.Point(12, 36),
                Width    = 280
            };

            _followSymlinks = new CheckBox
            {
                Text     = "Follow symbolic links",
                Checked  = current.FollowSymlinks,
                Location = new System.Drawing.Point(12, 60),
                Width    = 280
            };

            var lblMinSize = new Label
            {
                Text     = "Minimum file size (MB):",
                Location = new System.Drawing.Point(12, 90),
                Width    = 160
            };

            _minSize = new NumericUpDown
            {
                Minimum  = 0,
                Maximum  = 10_000,
                Value    = current.MinFileSizeBytes / 1_048_576,
                Location = new System.Drawing.Point(180, 87),
                Width    = 80
            };

            var lblExclude = new Label
            {
                Text     = "Exclude folders:",
                Location = new System.Drawing.Point(12, 120),
                Width    = 200
            };

            _excludeList = new ListBox
            {
                Location = new System.Drawing.Point(12, 138),
                Width    = 240,
                Height   = 80
            };

            foreach (var p in current.ExcludePaths)
                _excludeList.Items.Add(p);

            _addExclude = new Button
            {
                Text     = "Add",
                Location = new System.Drawing.Point(260, 138),
                Width    = 55
            };
            _addExclude.Click += OnAddExclude;

            _removeExclude = new Button
            {
                Text     = "Remove",
                Location = new System.Drawing.Point(260, 166),
                Width    = 55
            };
            _removeExclude.Click += OnRemoveExclude;

            _btnScan = new Button
            {
                Text         = "Scan",
                DialogResult = DialogResult.OK,
                Location     = new System.Drawing.Point(170, 240),
                Width        = 70
            };
            _btnScan.Click += OnScan;

            _btnCancel = new Button
            {
                Text         = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location     = new System.Drawing.Point(248, 240),
                Width        = 70
            };

            Controls.AddRange(new Control[]
            {
                _includeHidden, _includeSystem, _followSymlinks,
                lblMinSize, _minSize,
                lblExclude, _excludeList, _addExclude, _removeExclude,
                _btnScan, _btnCancel
            });

            AcceptButton = _btnScan;
            CancelButton = _btnCancel;
        }

        private void OnAddExclude(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                _excludeList.Items.Add(dialog.SelectedPath);
        }

        private void OnRemoveExclude(object? sender, EventArgs e)
        {
            if (_excludeList.SelectedIndex >= 0)
                _excludeList.Items.RemoveAt(_excludeList.SelectedIndex);
        }

        private void OnScan(object? sender, EventArgs e)
        {
            Result = new ScanOptions
            {
                IncludeHiddenFiles = _includeHidden.Checked,
                IncludeSystemFiles = _includeSystem.Checked,
                FollowSymlinks     = _followSymlinks.Checked,
                MinFileSizeBytes   = (long)_minSize.Value * 1_048_576,
                ExcludePaths       = _excludeList.Items
                                         .Cast<string>()
                                         .ToList()
            };
        }
    }
}