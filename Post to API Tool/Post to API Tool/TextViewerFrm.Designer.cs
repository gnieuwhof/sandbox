﻿
namespace Post_to_API_Tool
{
    partial class TextViewerFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.CloseBtn = new System.Windows.Forms.Button();
            this.ResponseTxt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CloseBtn
            // 
            this.CloseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CloseBtn.Location = new System.Drawing.Point(650, 556);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(150, 35);
            this.CloseBtn.TabIndex = 0;
            this.CloseBtn.Text = "&Close";
            this.CloseBtn.UseVisualStyleBackColor = true;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // ResponseTxt
            // 
            this.ResponseTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResponseTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ResponseTxt.Location = new System.Drawing.Point(12, 12);
            this.ResponseTxt.Multiline = true;
            this.ResponseTxt.Name = "ResponseTxt";
            this.ResponseTxt.ReadOnly = true;
            this.ResponseTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ResponseTxt.Size = new System.Drawing.Size(808, 538);
            this.ResponseTxt.TabIndex = 1;
            // 
            // TextViewerFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 603);
            this.Controls.Add(this.ResponseTxt);
            this.Controls.Add(this.CloseBtn);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "TextViewerFrm";
            this.ShowIcon = false;
            this.Text = "Post to API Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ResponseViewerFrm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ResponseViewerFrm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.TextBox ResponseTxt;
    }
}