using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace DiskMover

{
    public partial class MainForm : Form
    {
        private string lastMoveSource;
        private string lastMoveTarget;

        private string lastMoveLinkSource;
        private string lastMoveLinkTarget;

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
                MessageBox.Show("⚠️ EXPERIMENTAL TOOL - USE AT YOUR OWN RISK ⚠️\n\n" +
                    "This tool can move and create links for files and folders between disks. Please note:\n\n" +
                    "This is still an experimental tool and may cause data loss!\n" +
                    "Move only files and folders that contain NON-CRITICAL data.\n"+
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
                moveAndLink(sourcePath, fullTargetPath, true);
            }
            else if (rbMoveOnly.Checked)
            {
                moveOnly(sourcePath, fullTargetPath, true);
            }
            else if (rbLinkOnly.Checked)
            {
                linkOnly(sourcePath, fullTargetPath);
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

        private void moveOnly(string source, string target, bool moveButton = false)
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

                // If used by the Move button, save for potential undo
                if (moveButton)
                {
                    lastMoveSource = source;
                    lastMoveTarget = target;
                    btnUndoMove.Visible = true;
                }
                MessageBox.Show("Moved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while moving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void moveAndLink(string source, string target, bool moveLinkButton = false)
        {
            try
            {
                if (!CanCreateLink(target))
                {
                    return;
                }

                // First move the source to the target location
                moveOnly(source, target);

                // If used by the Move and Link button, save for potential undo.
                // Save before creating the link, because if link creation fails, we want to be able to undo the move without worrying about the link.
                if (moveLinkButton)
                {
                    lastMoveLinkSource = source;
                    lastMoveLinkTarget = target;
                    btnUndoMoveLink.Visible = true;
                }
                linkOnly(target, source);  // Link from new location back to original source path

                
                
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

        private void btnDeleteLinks_Click(object sender, EventArgs e)
        {
            try
            {
                // Determines start folder
                string initialFolder = null;
                // if txtTarget.Text valid, use 
                if (!string.IsNullOrWhiteSpace(txtTarget.Text) && Directory.Exists(txtTarget.Text))
                {
                    initialFolder = txtTarget.Text;
                }
                string selectedPath = FolderFileDialog.ShowDialog(this, "Select folder containing links to delete:", initialFolder);

                if (string.IsNullOrEmpty(selectedPath))
                {
                    return;
                }

                // Verify if it's a link
                bool isLink = false;
                try
                {
                    FileAttributes attributes = File.GetAttributes(selectedPath);
                    isLink = (attributes & System.IO.FileAttributes.ReparsePoint) == System.IO.FileAttributes.ReparsePoint;
                }
                catch
                {
                    isLink = false;
                }

                if (!isLink)
                {
                    var confirm = MessageBox.Show("This doesn't appear to be symbolic link or junction.\n\nDelete anyway?",
                        "Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirm != DialogResult.Yes)
                        return;
                }

                var result = MessageBox.Show($"Are you sure you want to delete this link?\n\n{selectedPath}",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (Directory.Exists(selectedPath))
                    {
                        Directory.Delete(selectedPath);
                        MessageBox.Show("Link folder deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (File.Exists(selectedPath))
                        {
                            File.Delete(selectedPath);
                            MessageBox.Show("Link file deleted successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting link: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUndoMove_Click(object sender, EventArgs e)
        {
            try
            {
                // Redundant checks
                if (string.IsNullOrEmpty(lastMoveSource) || string.IsNullOrEmpty(lastMoveTarget))
                {
                    MessageBox.Show("No move operation to undo.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                // Undo
                var result = MessageBox.Show($"Undo last move operation?\n\nLast move: \n{lastMoveSource} -> {lastMoveTarget}",
                    "Confirm Undo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    moveOnly(lastMoveTarget, lastMoveSource);
                    MessageBox.Show("Undo completed!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lastMoveSource = null;
                    lastMoveTarget = null;
                    btnUndoMove.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during undo: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUndoMoveLink_Click(object sender, EventArgs e)
        {
            try
            {
                // Redundant checks
                if (string.IsNullOrEmpty(lastMoveLinkSource) || string.IsNullOrEmpty(lastMoveLinkTarget))
                {
                    MessageBox.Show("No move operation to undo.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Undo
                var result = MessageBox.Show($"Undo last move and link operation?\n\nLast move and link: \n{lastMoveLinkSource} -> {lastMoveLinkTarget}",
                        "Confirm Undo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Check if the original source path is still a link (which it should be if the link creation succeeded). If it's not a link, it means the link creation failed and we should not proceed with the undo to avoid potential data loss.
                    if (Directory.Exists(lastMoveLinkSource) || File.Exists(lastMoveLinkSource))
                    {
                        FileAttributes attributes = File.GetAttributes(lastMoveLinkSource);
                        bool isLink = (attributes & System.IO.FileAttributes.ReparsePoint) == System.IO.FileAttributes.ReparsePoint;
                        if (isLink)
                        {
                            // If it's a link, we can just delete it to remove the link from the original source path.
                            if (Directory.Exists(lastMoveLinkSource))
                            {
                                Directory.Delete(lastMoveLinkSource);
                            }
                            else if (File.Exists(lastMoveLinkSource))
                            {
                                File.Delete(lastMoveLinkSource);
                            }
                        }
                        else
                        {
                            // If not a link, we should not proceed with the undo because it means the link creation failed and we want to keep the original file/folder for safety.
                            MessageBox.Show("The original source path is not a link. Undo cannot proceed to avoid potential data loss.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    moveOnly(lastMoveLinkTarget, lastMoveLinkSource);
                    // No need for explicit link deletion because moving back will remove the link from the target location.
                    // If the link still exists, it means the move back failed and we want to keep the link for safety.
                    MessageBox.Show("Undo completed!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lastMoveLinkSource = null;
                    lastMoveLinkTarget = null;
                    btnUndoMoveLink.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during undo: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}