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
            this.btnCreateLink = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblSouce = new System.Windows.Forms.Label();
            this.lblTarget = new System.Windows.Forms.Label();
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
            this.btnBrowseSource.Location = new System.Drawing.Point(736, 34);
            this.btnBrowseSource.Name = "btnBrowseSource";
            this.btnBrowseSource.Size = new System.Drawing.Size(30, 24);
            this.btnBrowseSource.TabIndex = 2;
            this.btnBrowseSource.Text = "...";
            this.btnBrowseSource.UseVisualStyleBackColor = true;
            this.btnBrowseSource.Click += new System.EventHandler(this.btnBrowseSource_Click);
            // 
            // btnBrowseTarget
            // 
            this.btnBrowseTarget.Location = new System.Drawing.Point(736, 101);
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
            this.txtTarget.Size = new System.Drawing.Size(707, 24);
            this.txtTarget.TabIndex = 4;
            // 
            // btnCreateLink
            // 
            this.btnCreateLink.BackColor = System.Drawing.SystemColors.Control;
            this.btnCreateLink.Location = new System.Drawing.Point(23, 156);
            this.btnCreateLink.Name = "btnCreateLink";
            this.btnCreateLink.Size = new System.Drawing.Size(743, 40);
            this.btnCreateLink.TabIndex = 6;
            this.btnCreateLink.Text = "Create Link /J";
            this.btnCreateLink.UseVisualStyleBackColor = false;
            this.btnCreateLink.Click += new System.EventHandler(this.btnCreateLink_Click);
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.Location = new System.Drawing.Point(23, 231);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(743, 120);
            this.txtLog.TabIndex = 7;
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.lblSouce);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnCreateLink);
            this.Controls.Add(this.btnBrowseTarget);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.btnBrowseSource);
            this.Controls.Add(this.txtSource);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Button btnBrowseSource;
        private System.Windows.Forms.Button btnBrowseTarget;
        private System.Windows.Forms.TextBox txtTarget;
        private System.Windows.Forms.Button btnCreateLink;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblSouce;
        private System.Windows.Forms.Label lblTarget;
    }
}