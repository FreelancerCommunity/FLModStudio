﻿namespace FreelancerModStudio
{
    using System.ComponentModel;
    using System.Windows.Forms;

    partial class FrmFileType
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(FrmFileType));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.fileTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pathLabel = new System.Windows.Forms.Label();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            // cancelButton
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = null;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;

            // okButton
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = null;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;

            // fileTypeComboBox
            this.fileTypeComboBox.AccessibleDescription = null;
            this.fileTypeComboBox.AccessibleName = null;
            resources.ApplyResources(this.fileTypeComboBox, "fileTypeComboBox");
            this.fileTypeComboBox.BackgroundImage = null;
            this.fileTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fileTypeComboBox.Font = null;
            this.fileTypeComboBox.FormattingEnabled = true;
            this.fileTypeComboBox.Name = "fileTypeComboBox";

            // label1
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";

            // pathLabel
            this.pathLabel.AccessibleDescription = null;
            this.pathLabel.AccessibleName = null;
            resources.ApplyResources(this.pathLabel, "pathLabel");
            this.pathLabel.Font = null;
            this.pathLabel.Name = "pathLabel";

            // pathTextBox
            this.pathTextBox.AccessibleDescription = null;
            this.pathTextBox.AccessibleName = null;
            resources.ApplyResources(this.pathTextBox, "pathTextBox");
            this.pathTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.pathTextBox.BackgroundImage = null;
            this.pathTextBox.Font = null;
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.ReadOnly = true;

            // frmFileType
            this.AcceptButton = this.okButton;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.fileTypeComboBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmFileType";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmFileTypeFormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Button cancelButton;
        private Button okButton;
        private ComboBox fileTypeComboBox;
        private Label label1;
        private Label pathLabel;
        private TextBox pathTextBox;
    }
}