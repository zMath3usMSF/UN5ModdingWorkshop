using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UN5ModdingWorkshop
{
    public class Filelist
    {
        public static void LoadDirectory(string path, TreeView treeView)
        {
            path = Path.Combine(path, "DATA/ROFS/");
            treeView.Nodes.Clear();

            DirectoryInfo rootDirectory = new DirectoryInfo(path);

            TreeNode rootNode = new TreeNode(rootDirectory.Name);
            rootNode.Tag = rootDirectory.FullName;

            treeView.Nodes.Add(rootNode);

            LoadDirectoryNodes(rootDirectory, rootNode);

            treeView.Nodes[0].Expand();

            //treeView.ExpandAll();
        }

        private static void LoadDirectoryNodes(DirectoryInfo directory, TreeNode parentNode)
        {
            // Pastas
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                TreeNode node = new TreeNode(dir.Name);
                node.Tag = dir.FullName;

                parentNode.Nodes.Add(node);

                LoadDirectoryNodes(dir, node);
            }

            // Arquivos
            foreach (FileInfo file in directory.GetFiles())
            {
                TreeNode node = new TreeNode(file.Name);
                node.Tag = file.FullName;

                parentNode.Nodes.Add(node);
            }
        }

    }
}
