
namespace PostToAPITool
{
    partial class MainFrm
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
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.PayloadTxt = new System.Windows.Forms.TextBox();
            this.ControllerCmb = new System.Windows.Forms.ComboBox();
            this.EndpointCmb = new System.Windows.Forms.ComboBox();
            this.ControllerLbl = new System.Windows.Forms.Label();
            this.EndpointLbl = new System.Windows.Forms.Label();
            this.CallAPIBtn = new System.Windows.Forms.Button();
            this.StatusCodeLbl = new System.Windows.Forms.Label();
            this.TokenAcquireTimeLbl = new System.Windows.Forms.Label();
            this.ShowTokenLbl = new System.Windows.Forms.Label();
            this.HostLbl = new System.Windows.Forms.Label();
            this.HostCmb = new System.Windows.Forms.ComboBox();
            this.BytesReceivedLbl = new System.Windows.Forms.Label();
            this.ShowResponeLbl = new System.Windows.Forms.Label();
            this.AutoOpenResponseChk = new System.Windows.Forms.CheckBox();
            this.AutoFormatChk = new System.Windows.Forms.CheckBox();
            this.MethodLbl = new System.Windows.Forms.Label();
            this.MethodCmb = new System.Windows.Forms.ComboBox();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusStrip
            // 
            this.StatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel,
            this.ToolStripProgressBar});
            this.StatusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.StatusStrip.Location = new System.Drawing.Point(0, 346);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.StatusStrip.Size = new System.Drawing.Size(738, 22);
            this.StatusStrip.TabIndex = 11;
            this.StatusStrip.Text = "StatusStrip";
            // 
            // ToolStripStatusLabel
            // 
            this.ToolStripStatusLabel.Name = "ToolStripStatusLabel";
            this.ToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // ToolStripProgressBar
            // 
            this.ToolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.ToolStripProgressBar.Name = "ToolStripProgressBar";
            this.ToolStripProgressBar.Size = new System.Drawing.Size(131, 16);
            this.ToolStripProgressBar.Step = 0;
            // 
            // PayloadTxt
            // 
            this.PayloadTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PayloadTxt.Font = new System.Drawing.Font("Lucida Console", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PayloadTxt.ForeColor = System.Drawing.Color.Navy;
            this.PayloadTxt.Location = new System.Drawing.Point(9, 41);
            this.PayloadTxt.Margin = new System.Windows.Forms.Padding(2);
            this.PayloadTxt.Multiline = true;
            this.PayloadTxt.Name = "PayloadTxt";
            this.PayloadTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PayloadTxt.Size = new System.Drawing.Size(720, 258);
            this.PayloadTxt.TabIndex = 6;
            this.PayloadTxt.Enter += new System.EventHandler(this.PayloadTxt_Enter);
            this.PayloadTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PayloadTxt_KeyDown);
            this.PayloadTxt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PayloadTxt_KeyUp);
            // 
            // ControllerCmb
            // 
            this.ControllerCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ControllerCmb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ControllerCmb.FormattingEnabled = true;
            this.ControllerCmb.Location = new System.Drawing.Point(258, 8);
            this.ControllerCmb.Margin = new System.Windows.Forms.Padding(2);
            this.ControllerCmb.Name = "ControllerCmb";
            this.ControllerCmb.Size = new System.Drawing.Size(151, 23);
            this.ControllerCmb.TabIndex = 3;
            this.ControllerCmb.SelectedIndexChanged += new System.EventHandler(this.ControllerCmb_SelectedIndexChanged);
            // 
            // EndpointCmb
            // 
            this.EndpointCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EndpointCmb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EndpointCmb.FormattingEnabled = true;
            this.EndpointCmb.Location = new System.Drawing.Point(450, 8);
            this.EndpointCmb.Margin = new System.Windows.Forms.Padding(2);
            this.EndpointCmb.Name = "EndpointCmb";
            this.EndpointCmb.Size = new System.Drawing.Size(151, 23);
            this.EndpointCmb.TabIndex = 5;
            // 
            // ControllerLbl
            // 
            this.ControllerLbl.AutoSize = true;
            this.ControllerLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ControllerLbl.Location = new System.Drawing.Point(236, 12);
            this.ControllerLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ControllerLbl.Name = "ControllerLbl";
            this.ControllerLbl.Size = new System.Drawing.Size(18, 15);
            this.ControllerLbl.TabIndex = 2;
            this.ControllerLbl.Text = "C:";
            // 
            // EndpointLbl
            // 
            this.EndpointLbl.AutoSize = true;
            this.EndpointLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EndpointLbl.ForeColor = System.Drawing.SystemColors.WindowText;
            this.EndpointLbl.Location = new System.Drawing.Point(428, 12);
            this.EndpointLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.EndpointLbl.Name = "EndpointLbl";
            this.EndpointLbl.Size = new System.Drawing.Size(18, 15);
            this.EndpointLbl.TabIndex = 4;
            this.EndpointLbl.Text = "E:";
            // 
            // CallAPIBtn
            // 
            this.CallAPIBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CallAPIBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CallAPIBtn.Location = new System.Drawing.Point(615, 307);
            this.CallAPIBtn.Margin = new System.Windows.Forms.Padding(2);
            this.CallAPIBtn.Name = "CallAPIBtn";
            this.CallAPIBtn.Size = new System.Drawing.Size(112, 32);
            this.CallAPIBtn.TabIndex = 10;
            this.CallAPIBtn.Text = "&Call API...";
            this.CallAPIBtn.UseVisualStyleBackColor = true;
            this.CallAPIBtn.Click += new System.EventHandler(this.CallAPIBtn_Click);
            // 
            // StatusCodeLbl
            // 
            this.StatusCodeLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusCodeLbl.AutoSize = true;
            this.StatusCodeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusCodeLbl.Location = new System.Drawing.Point(9, 323);
            this.StatusCodeLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.StatusCodeLbl.Name = "StatusCodeLbl";
            this.StatusCodeLbl.Size = new System.Drawing.Size(89, 17);
            this.StatusCodeLbl.TabIndex = 9;
            this.StatusCodeLbl.Text = "Status Code:";
            // 
            // TokenAcquireTimeLbl
            // 
            this.TokenAcquireTimeLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TokenAcquireTimeLbl.AutoSize = true;
            this.TokenAcquireTimeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TokenAcquireTimeLbl.Location = new System.Drawing.Point(9, 303);
            this.TokenAcquireTimeLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TokenAcquireTimeLbl.Name = "TokenAcquireTimeLbl";
            this.TokenAcquireTimeLbl.Size = new System.Drawing.Size(198, 17);
            this.TokenAcquireTimeLbl.TabIndex = 7;
            this.TokenAcquireTimeLbl.Text = "Token aquired time:  00:00:00";
            // 
            // ShowTokenLbl
            // 
            this.ShowTokenLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ShowTokenLbl.AutoSize = true;
            this.ShowTokenLbl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ShowTokenLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShowTokenLbl.ForeColor = System.Drawing.Color.Blue;
            this.ShowTokenLbl.Location = new System.Drawing.Point(217, 302);
            this.ShowTokenLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ShowTokenLbl.Name = "ShowTokenLbl";
            this.ShowTokenLbl.Size = new System.Drawing.Size(79, 17);
            this.ShowTokenLbl.TabIndex = 8;
            this.ShowTokenLbl.Text = "show token";
            this.ShowTokenLbl.Click += new System.EventHandler(this.ShowTokenLbl_Click);
            // 
            // HostLbl
            // 
            this.HostLbl.AutoSize = true;
            this.HostLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HostLbl.Location = new System.Drawing.Point(6, 12);
            this.HostLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HostLbl.Name = "HostLbl";
            this.HostLbl.Size = new System.Drawing.Size(19, 15);
            this.HostLbl.TabIndex = 0;
            this.HostLbl.Text = "H:";
            // 
            // HostCmb
            // 
            this.HostCmb.BackColor = System.Drawing.SystemColors.Window;
            this.HostCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HostCmb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HostCmb.FormattingEnabled = true;
            this.HostCmb.Location = new System.Drawing.Point(29, 8);
            this.HostCmb.Margin = new System.Windows.Forms.Padding(2);
            this.HostCmb.Name = "HostCmb";
            this.HostCmb.Size = new System.Drawing.Size(188, 23);
            this.HostCmb.TabIndex = 1;
            // 
            // BytesReceivedLbl
            // 
            this.BytesReceivedLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BytesReceivedLbl.AutoSize = true;
            this.BytesReceivedLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BytesReceivedLbl.Location = new System.Drawing.Point(344, 303);
            this.BytesReceivedLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.BytesReceivedLbl.Name = "BytesReceivedLbl";
            this.BytesReceivedLbl.Size = new System.Drawing.Size(105, 17);
            this.BytesReceivedLbl.TabIndex = 12;
            this.BytesReceivedLbl.Text = "Bytes received:";
            // 
            // ShowResponeLbl
            // 
            this.ShowResponeLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ShowResponeLbl.AutoSize = true;
            this.ShowResponeLbl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ShowResponeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShowResponeLbl.ForeColor = System.Drawing.Color.Blue;
            this.ShowResponeLbl.Location = new System.Drawing.Point(506, 303);
            this.ShowResponeLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ShowResponeLbl.Name = "ShowResponeLbl";
            this.ShowResponeLbl.Size = new System.Drawing.Size(96, 17);
            this.ShowResponeLbl.TabIndex = 13;
            this.ShowResponeLbl.Text = "show respone";
            this.ShowResponeLbl.Click += new System.EventHandler(this.ShowResponeLbl_Click);
            // 
            // AutoOpenResponseChk
            // 
            this.AutoOpenResponseChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AutoOpenResponseChk.AutoSize = true;
            this.AutoOpenResponseChk.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AutoOpenResponseChk.Location = new System.Drawing.Point(346, 322);
            this.AutoOpenResponseChk.Margin = new System.Windows.Forms.Padding(2);
            this.AutoOpenResponseChk.Name = "AutoOpenResponseChk";
            this.AutoOpenResponseChk.Size = new System.Drawing.Size(207, 21);
            this.AutoOpenResponseChk.TabIndex = 14;
            this.AutoOpenResponseChk.Text = "Auto-open Response Viewer";
            this.AutoOpenResponseChk.UseVisualStyleBackColor = true;
            // 
            // AutoFormatChk
            // 
            this.AutoFormatChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AutoFormatChk.AutoSize = true;
            this.AutoFormatChk.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AutoFormatChk.Location = new System.Drawing.Point(220, 322);
            this.AutoFormatChk.Margin = new System.Windows.Forms.Padding(2);
            this.AutoFormatChk.Name = "AutoFormatChk";
            this.AutoFormatChk.Size = new System.Drawing.Size(101, 21);
            this.AutoFormatChk.TabIndex = 15;
            this.AutoFormatChk.Text = "Auto-format";
            this.AutoFormatChk.UseVisualStyleBackColor = true;
            this.AutoFormatChk.CheckedChanged += new System.EventHandler(this.AutoFormatChk_CheckedChanged);
            // 
            // MethodLbl
            // 
            this.MethodLbl.AutoSize = true;
            this.MethodLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MethodLbl.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MethodLbl.Location = new System.Drawing.Point(620, 12);
            this.MethodLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MethodLbl.Name = "MethodLbl";
            this.MethodLbl.Size = new System.Drawing.Size(21, 15);
            this.MethodLbl.TabIndex = 16;
            this.MethodLbl.Text = "M:";
            // 
            // MethodCmb
            // 
            this.MethodCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MethodCmb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MethodCmb.FormattingEnabled = true;
            this.MethodCmb.Location = new System.Drawing.Point(645, 9);
            this.MethodCmb.Margin = new System.Windows.Forms.Padding(2);
            this.MethodCmb.Name = "MethodCmb";
            this.MethodCmb.Size = new System.Drawing.Size(82, 23);
            this.MethodCmb.TabIndex = 17;
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 368);
            this.Controls.Add(this.MethodCmb);
            this.Controls.Add(this.MethodLbl);
            this.Controls.Add(this.AutoFormatChk);
            this.Controls.Add(this.AutoOpenResponseChk);
            this.Controls.Add(this.ShowResponeLbl);
            this.Controls.Add(this.BytesReceivedLbl);
            this.Controls.Add(this.HostCmb);
            this.Controls.Add(this.HostLbl);
            this.Controls.Add(this.ShowTokenLbl);
            this.Controls.Add(this.TokenAcquireTimeLbl);
            this.Controls.Add(this.StatusCodeLbl);
            this.Controls.Add(this.CallAPIBtn);
            this.Controls.Add(this.EndpointLbl);
            this.Controls.Add(this.ControllerLbl);
            this.Controls.Add(this.EndpointCmb);
            this.Controls.Add(this.ControllerCmb);
            this.Controls.Add(this.PayloadTxt);
            this.Controls.Add(this.StatusStrip);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(754, 251);
            this.Name = "MainFrm";
            this.ShowIcon = false;
            this.Text = "Post to API Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFrm_FormClosing);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.TextBox PayloadTxt;
        private System.Windows.Forms.ComboBox ControllerCmb;
        private System.Windows.Forms.ComboBox EndpointCmb;
        private System.Windows.Forms.Label ControllerLbl;
        private System.Windows.Forms.Label EndpointLbl;
        private System.Windows.Forms.Button CallAPIBtn;
        private System.Windows.Forms.Label StatusCodeLbl;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel;
        private System.Windows.Forms.Label TokenAcquireTimeLbl;
        private System.Windows.Forms.Label ShowTokenLbl;
        private System.Windows.Forms.Label HostLbl;
        private System.Windows.Forms.ComboBox HostCmb;
        private System.Windows.Forms.Label BytesReceivedLbl;
        private System.Windows.Forms.Label ShowResponeLbl;
        private System.Windows.Forms.CheckBox AutoOpenResponseChk;
        private System.Windows.Forms.ToolStripProgressBar ToolStripProgressBar;
        private System.Windows.Forms.CheckBox AutoFormatChk;
        private System.Windows.Forms.Label MethodLbl;
        private System.Windows.Forms.ComboBox MethodCmb;
    }
}

