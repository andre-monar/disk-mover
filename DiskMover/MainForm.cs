using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskMover

{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            string selectedPath = FolderFileDialog.ShowDialog(this, "Select source folder or file:");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                txtSource.Text = selectedPath;
            }
        }

        private void btnBrowseTarget_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select target folder:";
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtTarget.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnCreateLink_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtSource.Text) || string.IsNullOrWhiteSpace(txtTarget.Text))
            {
                MessageBox.Show("Please select both source and target folders.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate source folder/file exists
            if (!System.IO.Directory.Exists(txtSource.Text) && !System.IO.File.Exists(txtSource.Text))
            {
                MessageBox.Show("Source file or folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Extract the last folder name from source path
            string sourcePath = txtSource.Text.TrimEnd('\\'); // Remove "\" if any
            string folderOrFileName = System.IO.Path.GetFileName(sourcePath); // Gets folder/file name (from path)

            string targetBase = txtTarget.Text.TrimEnd('\\'); // Remove "\" if any
            string fullTargetPath = System.IO.Path.Combine(targetBase, folderOrFileName); // Combine target base with folder/file name

            // Check if target path already exists
            if (System.IO.Directory.Exists(fullTargetPath) || System.IO.File.Exists(fullTargetPath))
            {
                MessageBox.Show($"A file or folder already exists at the target location:\r\n{fullTargetPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create symbolic link (mklink)
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c mklink /J \"{fullTargetPath}\" \"{txtSource.Text}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
                process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;


                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Update log
                txtLog.Clear();
                txtLog.AppendText($"Command: mklink /J \"{fullTargetPath}\" \"{txtSource.Text}\"\r\n");

                if (process.ExitCode == 0)
                {
                    txtLog.AppendText("✅ Link created successfully:\r\n");
                    txtLog.AppendText(output);
                    SystemSounds.Asterisk.Play();
                }
                else
                {
                    txtLog.AppendText($"❌ Error creating link:\r\n");
                    txtLog.AppendText(error);
                    SystemSounds.Hand.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreateGlobalFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select where to create the DiskMover Links folder:";
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;

                    // Create the global links folder
                    string globalFolderPath = System.IO.Path.Combine(selectedPath, "DiskMover Links");

                    try
                    {
                        // Check if the global folder already exists
                        if (!System.IO.Directory.Exists(globalFolderPath))
                        {
                            System.IO.Directory.CreateDirectory(globalFolderPath);
                            MessageBox.Show($"Global links folder created at:\r\n{globalFolderPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Global links folder already exists at:\r\n{globalFolderPath}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while creating the global links folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}