using System;
using System.Linq;
using System.Windows.Forms;
using Potato64.Core;

namespace Potato64.UI.Controls {
    public class DirectoryTreeView : UserControl {
        private TreeView _tree;
        public event Action<FolderEntry>? FolderSelected;
        public DirectoryTreeView() {
            _tree = new TreeView { Dock = DockStyle.Fill };
            _tree.AfterSelect += (s, e) => { if (e.Node?.Tag is FolderEntry f) FolderSelected?.Invoke(f); };
            this.Controls.Add(_tree);
        }
        public void LoadResult(ScanResult res) {
            _tree.Nodes.Clear();
            var root = new TreeNode(res.RootPath) { Tag = res.RootFolder };
            _tree.Nodes.Add(root);
            AddNodes(root, res.RootFolder);
            root.Expand();
        }
        private void AddNodes(TreeNode node, FolderEntry folder) {
            foreach (var s in folder.SubFolders.OrderByDescending(x => x.TotalBytes)) {
                var subNode = new TreeNode(s.Name) { Tag = s };
                node.Nodes.Add(subNode);
                AddNodes(subNode, s);
            }
        }
        public void Clear() => _tree.Nodes.Clear();
    }
}