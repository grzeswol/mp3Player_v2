using System;
using System.IO;
using System.Windows.Forms;

namespace mp3Player_v2
{
    public partial class FolderSelectionForm : Form
    {
        private const string FolderPaths = "paths";

        public FolderSelectionForm()
        {
            InitializeComponent();
            if (File.Exists(FolderPaths))
            {
                LoadFolderPaths(FolderPaths);
            }
        }

        private void SaveFolderPaths(string folderPathName)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(folderPathName))
                {
                    foreach (string s in listBox1.Items)
                    {
                        stream.WriteLine(s);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Problem " + ex.Message);
                throw;
            }
        }


        private void LoadFolderPaths(string folderPathName)
        {
            listBox1.Items.Clear();
            try
            {
                using (StreamReader stream = new StreamReader(folderPathName))
                {
                    string line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        listBox1.Items.Add(line);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(@"Damaged file", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFolderPaths(FolderPaths);
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result != DialogResult.OK) return;
            listBox1.Items.Add(folderBrowserDialog1.SelectedPath);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }
    }
}
