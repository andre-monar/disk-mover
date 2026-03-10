using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace DiskMover

{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            // Verify admin rights for link creation
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                var result = MessageBox.Show("Warning: This application requires administrator privileges to create symbolic links and move files.", "Elevation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    // Restart as admin
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.UseShellExecute = true;
                    startInfo.WorkingDirectory = Environment.CurrentDirectory;
                    startInfo.FileName = Application.ExecutablePath;
                    startInfo.Verb = "runas";

                    Process.Start(startInfo);
                    Application.Exit();
                }
                else
                {
                    // User chose not to run as admin, close the application
                    MessageBox.Show("The application will now close. Please run as administrator to use the application.", "Exiting", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
            }
            else
            {
                // Warning about moving system folders/backup
                MessageBox.Show("This tool can move and create links for files and folders between disks. Please note:\n\n" +
                    " - Do NOT move system folders (Windows, System32, ProgramFiles)\n" +
                    " - Do NOT move folders containing running programs or DLLs\n" +
                    " - Backup important data before moving", "Security Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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

        private void btnExecute_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtSource.Text) || string.IsNullOrWhiteSpace(txtTarget.Text))
            {
                MessageBox.Show("Please select both source and target folders.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate source folder/file exists
            if (!Directory.Exists(txtSource.Text) && !File.Exists(txtSource.Text))
            {
                MessageBox.Show("Source file or folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Extract the last folder name from source path
            string sourcePath = txtSource.Text.TrimEnd('\\'); // Remove "\" if any
            string folderOrFileName = Path.GetFileName(sourcePath); // Gets folder/file name (from path)

            string targetBase = txtTarget.Text.TrimEnd('\\'); // Remove "\" if any
            string fullTargetPath = Path.Combine(targetBase, folderOrFileName); // Combine target base with folder/file name

            // Check if target path already exists
            if (Directory.Exists(fullTargetPath) || File.Exists(fullTargetPath))
            {
                MessageBox.Show($"A file or folder already exists at the target location:\r\n{fullTargetPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (rbMoveAndLink.Checked)
            {
                moveAndLink(sourcePath, fullTargetPath);
            }
            else if (rbLinkOnly.Checked)
            {
                linkOnly(sourcePath, fullTargetPath);
            }
            else if (rbMoveOnly.Checked)
            {
                moveOnly(sourcePath, fullTargetPath);
            }
        }
        
        private void linkOnly(string source, string target)
        {
            // Create symbolic link (mklink)
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "cmd.exe";
                // Treat file/folder separately
                string arguments;
                if (Directory.Exists(source))
                {
                    arguments = $"/c mklink /J \"{target}\" \"{source}\""; // folder
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                }
                else
                {
                    arguments = $"/c mklink \"{target}\" \"{source}\""; // file
                }

                process.StartInfo.Arguments = arguments;
                process.Start();
                process.WaitForExit();


                if (process.ExitCode == 0)
                {
                    MessageBox.Show($"Link created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string errorMessage = process.StandardError.ReadToEnd();
                    MessageBox.Show($"Failed to create link. Error: {errorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void moveOnly(string source, string target)
        {
            try
            {
                // Check if it's a directory or file and move accordingly
                if (Directory.Exists(source)) // folder
                {
                    // Check if it's different drives
                    if (Path.GetPathRoot(source) != Path.GetPathRoot(target))
                    {
                        // If different drives, we need to copy and delete instead of move
                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(source, target, true); // Custom method to copy directories
                        Directory.Delete(source, true); // Delete original after copying
                    }
                    else
                    {
                        // If same drive, we can move directly
                        Directory.Move(source, target);
                    }

                }
                else if (File.Exists(source)) // file
                {
                    // Check if it's different drives
                    if (Path.GetPathRoot(source) != Path.GetPathRoot(target))
                    {
                        // If different drives, we need to copy and delete instead of move
                        File.Copy(source, target, true); // Copy file
                        File.Delete(source); // Delete original after copying
                    }
                    else
                    {
                        // If same drive, we can move directly
                        File.Move(source, target);
                    }
                }
                else // Redundant check for source existence, added for safety before move operation
                {
                    MessageBox.Show("Source file or folder does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Moved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while moving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void moveAndLink(string source, string target)
        {
            try
            {
                if (!CanCreateLink(target))
                {
                    return;
                }

                // First move the source to the target location
                moveOnly(source, target);
                linkOnly(target, source); // Link from new location back to original source path
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while moving and linking: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CanCreateLink(string target)
        {
            try
            {
                // Attempt to create a temp file
                string testFile = Path.Combine(Path.GetDirectoryName(target), "diskMover_test.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);

                // Verify if it's NTFS and supports symbolic links
                DriveInfo drive = new DriveInfo(Path.GetPathRoot(target));
                if (drive.DriveFormat != "NTFS")
                {
                    MessageBox.Show("The target drive does not support symbolic links (requires NTFS).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return true;
            } 
            catch
            {
                return false; // If any exception occurs, we assume we cannot create links
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
                        txtTarget.Text = globalFolderPath; // Set the target path to the new global folder
                        // Check if the global folder already exists
                        if (!Directory.Exists(globalFolderPath))
                        {
                            Directory.CreateDirectory(globalFolderPath);
                            
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