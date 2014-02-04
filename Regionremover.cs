using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegionRemover
{
    public partial class Regionremover : Form
    {
        private string _folderName = "";

        public Regionremover()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog Dialog = new OpenFileDialog { Filter = "All Files|*.*", Title = "Select C# project file", RestoreDirectory = true })
            {
                if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fileName = Dialog.FileName;
                    _folderName = Path.GetDirectoryName(fileName);
                }
            }
        }

        private void replaceRegionsInFolder(String folder)
        {
            string[] filePaths = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);

            int counter = 0;
            string currentFile = string.Empty;
            string currentLine = string.Empty;
            string updatedLine = string.Empty;

            foreach (string file in filePaths)
            {
                currentFile = File.ReadAllText(file);
                counter++;
                Debug.Print("Found in " + file);

                Boolean hasRegion = false;

                using (StreamReader streamReader = new StreamReader(file))
                {
                    while (!streamReader.EndOfStream)
                    {
                        currentLine = streamReader.ReadLine();

                        if (currentLine.IndexOf("#region", 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                        {
                            hasRegion = true;
                            break;
                        }

                        if (currentLine.IndexOf("#endregion", 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                        {
                            hasRegion = true;
                            break;
                        }
                    }
                }

                if (hasRegion)
                {
                    currentFile = currentFile.Replace(currentLine, "");

                    // If file is ReadOnly then remove that attribute.
                    FileAttributes attributes = File.GetAttributes(file);

                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(file, attributes ^ FileAttributes.ReadOnly);
                    }

                    using (StreamWriter streamWriter = new StreamWriter(file))
                    {
                        streamWriter.Write(currentFile);
                    }
                }
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (_folderName != null && _folderName.Length > 0)
            {
                replaceRegionsInFolder(_folderName);
                _folderName = "";
                MessageBox.Show("All C# regions are removed!");
            }
            else
            {
                MessageBox.Show("Please select a projectFile");
            }
        }
    }
}
