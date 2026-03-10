using System.Drawing;
using System.Windows.Forms;

namespace DiskMover
{
    partial class MainForm
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtSource = new System.Windows.Forms.TextBox();
            this.btnBrowseSource = new System.Windows.Forms.Button();
            this.btnBrowseTarget = new System.Windows.Forms.Button();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.lblSouce = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
            this.btnCreateGlobalFolder = new System.Windows.Forms.Button();
            this.rbLinkOnly = new System.Windows.Forms.RadioButton();
            this.rbMoveOnly = new System.Windows.Forms.RadioButton();
            this.rbMoveAndLink = new System.Windows.Forms.RadioButton();
            this.lblSelectAction = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSource
            // 
            this.txtSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSource.Location = new System.Drawing.Point(23, 33);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(707, 24);
            this.txtSource.TabIndex = 1;
            // 
            // btnBrowseSource
            // 
            this.btnBrowseSource.Location = new System.Drawing.Point(736, 33);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(30, 24);
            this.btnBrowseSource.TabIndex = 2;
            this.btnBrowseSource.Text = "...";
            this.btnBrowseSource.UseVisualStyleBackColor = true;
            this.btnBrowseSource.Click += new System.EventHandler(this.btnBrowseSource_Click);
            // 
            // btnBrowseTarget
            // 
            this.btnBrowseTarget.Location = new System.Drawing.Point(595, 100);
            this.btnBrowseTarget.Name = "btnBrowseTarget";
            this.btnBrowseTarget.Size = new System.Drawing.Size(30, 24);
            this.btnBrowseTarget.TabIndex = 5;
            this.btnBrowseTarget.Text = "...";
            this.btnBrowseTarget.UseVisualStyleBackColor = true;
            this.btnBrowseTarget.Click += new System.EventHandler(this.btnBrowseTarget_Click);
            // 
            // txtTarget
            // 
            this.txtTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTarget.Location = new System.Drawing.Point(23, 100);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(566, 24);
            this.txtTarget.TabIndex = 4;
            // 
            // btnExecute
            // 
            this.btnExecute.BackColor = System.Drawing.SystemColors.Control;
            this.btnExecute.Location = new System.Drawing.Point(23, 252);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(743, 48);
            this.btnExecute.TabIndex = 6;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = false;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // lblSouce
            // 
            this.lblSouce.AutoSize = true;
            this.lblSouce.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSouce.Location = new System.Drawing.Point(20, 13);
            this.lblSouce.Name = "lblSouce";
            this.lblSouce.Size = new System.Drawing.Size(76, 13);
            this.lblSouce.TabIndex = 8;
            this.lblSouce.Text = "Source Folder:";
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTarget.Location = new System.Drawing.Point(20, 80);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(73, 13);
            this.lblTarget.TabIndex = 9;
            this.lblTarget.Text = "Target Folder:";
            // 
            // btnCreateGlobalFolder
            // 
            this.btnCreateGlobalFolder.Location = new System.Drawing.Point(631, 99);
            this.btnCreateGlobalFolder.Name = "btnCreateGlobalFolder";
            this.btnCreateGlobalFolder.Size = new System.Drawing.Size(135, 25);
            this.btnCreateGlobalFolder.TabIndex = 10;
            this.btnCreateGlobalFolder.Text = "Create Target Folder";
            this.btnCreateGlobalFolder.UseVisualStyleBackColor = true;
            this.btnCreateGlobalFolder.Click += new System.EventHandler(this.btnCreateGlobalFolder_Click);
            // 
            // rbLinkOnly
            // 
            this.rbLinkOnly.AutoSize = true;
            this.rbLinkOnly.Location = new System.Drawing.Point(24, 186);
            this.rbLinkOnly.Name = "rbLinkOnly";
            this.rbLinkOnly.Size = new System.Drawing.Size(67, 17);
            this.rbLinkOnly.TabIndex = 11;
            this.rbLinkOnly.Text = "Link only";
            this.rbLinkOnly.UseVisualStyleBackColor = true;
            // 
            // rbMoveOnly
            // 
            this.rbMoveOnly.AutoSize = true;
            this.rbMoveOnly.Location = new System.Drawing.Point(25, 209);
            this.rbMoveOnly.Name = "rbMoveOnly";
            this.rbMoveOnly.Size = new System.Drawing.Size(74, 17);
            this.rbMoveOnly.TabIndex = 12;
            this.rbMoveOnly.Text = "Move only";
            this.rbMoveOnly.UseVisualStyleBackColor = true;
            // 
            // rbMoveAndLink
            // 
            this.rbMoveAndLink.AutoSize = true;
            this.rbMoveAndLink.Checked = true;
            this.rbMoveAndLink.Location = new System.Drawing.Point(24, 163);
            this.rbMoveAndLink.Name = "rbMoveAndLink";
            this.rbMoveAndLink.Size = new System.Drawing.Size(96, 17);
            this.rbMoveAndLink.TabIndex = 13;
            this.rbMoveAndLink.TabStop = true;
            this.rbMoveAndLink.Text = "Move and Link";
            this.rbMoveAndLink.UseVisualStyleBackColor = true;
            // 
            // lblSelectAction
            // 
            this.lblSelectAction.AutoSize = true;
            this.lblSelectAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectAction.Location = new System.Drawing.Point(22, 147);
            this.lblSelectAction.Name = "lblSelectAction";
            this.lblSelectAction.Size = new System.Drawing.Size(73, 13);
            this.lblSelectAction.TabIndex = 14;
            this.lblSelectAction.Text = "Select Action:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 322);
            this.Controls.Add(this.lblSelectAction);
            this.Controls.Add(this.rbMoveAndLink);
            this.Controls.Add(this.rbMoveOnly);
            this.Controls.Add(this.rbLinkOnly);
            this.Controls.Add(this.btnCreateGlobalFolder);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.lblSouce);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnBrowseTarget);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.btnBrowseSource);
            this.Controls.Add(this.txtSource);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void BtnCreateLink_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.Button btnBrowseTarget;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label lblSouce;
        private System.Windows.Forms.Label lblTarget;
        private Button btnCreateGlobalFolder;
        private RadioButton rbLinkOnly;
        private RadioButton rbMoveOnly;
        private RadioButton rbMoveAndLink;
        private Label lblSelectAction;
    }
}