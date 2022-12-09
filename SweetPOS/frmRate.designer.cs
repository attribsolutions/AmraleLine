namespace SweetPOS
{
    partial class frmRate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRate));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dtSDate = new System.Windows.Forms.DateTimePicker();
            this.lblEffectiveFrom = new System.Windows.Forms.Label();
            this.txtNewVat = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.lblNewVat = new System.Windows.Forms.Label();
            this.cboItem = new System.Windows.Forms.ComboBox();
            this.txtNewRate = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.lblNewRate = new System.Windows.Forms.Label();
            this.txtPreviousVat = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.lblPrevoiusVat = new System.Windows.Forms.Label();
            this.txtpreviousRate = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.lblPreviousRate = new System.Windows.Forms.Label();
            this.lblItem = new System.Windows.Forms.Label();
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSaveClose = new System.Windows.Forms.Button();
            this.btnSaveNew = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelEx2 = new DevComponents.DotNetBar.PanelEx();
            this.panelEx3 = new DevComponents.DotNetBar.PanelEx();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblHeading = new System.Windows.Forms.Label();
            this.panelEx4 = new DevComponents.DotNetBar.PanelEx();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panelEx1.SuspendLayout();
            this.panelEx3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(12, 83);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(710, 397);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.dtSDate);
            this.tabPage1.Controls.Add(this.lblEffectiveFrom);
            this.tabPage1.Controls.Add(this.txtNewVat);
            this.tabPage1.Controls.Add(this.lblNewVat);
            this.tabPage1.Controls.Add(this.cboItem);
            this.tabPage1.Controls.Add(this.txtNewRate);
            this.tabPage1.Controls.Add(this.lblNewRate);
            this.tabPage1.Controls.Add(this.txtPreviousVat);
            this.tabPage1.Controls.Add(this.lblPrevoiusVat);
            this.tabPage1.Controls.Add(this.txtpreviousRate);
            this.tabPage1.Controls.Add(this.lblPreviousRate);
            this.tabPage1.Controls.Add(this.lblItem);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(702, 368);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            // 
            // dtSDate
            // 
            this.dtSDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtSDate.Location = new System.Drawing.Point(118, 23);
            this.dtSDate.Name = "dtSDate";
            this.dtSDate.Size = new System.Drawing.Size(107, 22);
            this.dtSDate.TabIndex = 0;
            // 
            // lblEffectiveFrom
            // 
            this.lblEffectiveFrom.AutoSize = true;
            this.lblEffectiveFrom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.lblEffectiveFrom.Location = new System.Drawing.Point(17, 23);
            this.lblEffectiveFrom.Name = "lblEffectiveFrom";
            this.lblEffectiveFrom.Size = new System.Drawing.Size(93, 16);
            this.lblEffectiveFrom.TabIndex = 13;
            this.lblEffectiveFrom.Text = "Effective From";
            // 
            // txtNewVat
            // 
            // 
            // 
            // 
            this.txtNewVat.Border.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(163)))), ((int)(((byte)(165)))));
            this.txtNewVat.Border.Class = "TextBoxBorder";
            this.txtNewVat.Location = new System.Drawing.Point(117, 206);
            this.txtNewVat.Name = "txtNewVat";
            this.txtNewVat.Size = new System.Drawing.Size(108, 22);
            this.txtNewVat.TabIndex = 5;
            this.txtNewVat.Text = "0.00";
            this.txtNewVat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNewVat.Validating += new System.ComponentModel.CancelEventHandler(this.Numeric_Validating);
            this.txtNewVat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.string_Keypress);
            // 
            // lblNewVat
            // 
            this.lblNewVat.AutoSize = true;
            this.lblNewVat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.lblNewVat.Location = new System.Drawing.Point(17, 212);
            this.lblNewVat.Name = "lblNewVat";
            this.lblNewVat.Size = new System.Drawing.Size(65, 16);
            this.lblNewVat.TabIndex = 11;
            this.lblNewVat.Text = "New VAT";
            // 
            // cboItem
            // 
            this.cboItem.FormattingEnabled = true;
            this.cboItem.Location = new System.Drawing.Point(117, 59);
            this.cboItem.Name = "cboItem";
            this.cboItem.Size = new System.Drawing.Size(253, 24);
            this.cboItem.TabIndex = 1;
            this.cboItem.SelectedIndexChanged += new System.EventHandler(this.cboItem_SelectedIndexChanged);
            // 
            // txtNewRate
            // 
            // 
            // 
            // 
            this.txtNewRate.Border.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(163)))), ((int)(((byte)(165)))));
            this.txtNewRate.Border.Class = "TextBoxBorder";
            this.txtNewRate.Location = new System.Drawing.Point(117, 139);
            this.txtNewRate.Name = "txtNewRate";
            this.txtNewRate.Size = new System.Drawing.Size(108, 22);
            this.txtNewRate.TabIndex = 3;
            this.txtNewRate.Text = "0.00";
            this.txtNewRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNewRate.Validating += new System.ComponentModel.CancelEventHandler(this.Numeric_Validating);
            this.txtNewRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.string_Keypress);
            // 
            // lblNewRate
            // 
            this.lblNewRate.AutoSize = true;
            this.lblNewRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.lblNewRate.Location = new System.Drawing.Point(17, 140);
            this.lblNewRate.Name = "lblNewRate";
            this.lblNewRate.Size = new System.Drawing.Size(67, 16);
            this.lblNewRate.TabIndex = 9;
            this.lblNewRate.Text = "New Rate";
            // 
            // txtPreviousVat
            // 
            // 
            // 
            // 
            this.txtPreviousVat.Border.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(163)))), ((int)(((byte)(165)))));
            this.txtPreviousVat.Border.Class = "TextBoxBorder";
            this.txtPreviousVat.Enabled = false;
            this.txtPreviousVat.Location = new System.Drawing.Point(118, 169);
            this.txtPreviousVat.Name = "txtPreviousVat";
            this.txtPreviousVat.Size = new System.Drawing.Size(107, 22);
            this.txtPreviousVat.TabIndex = 4;
            this.txtPreviousVat.Text = "0.00";
            this.txtPreviousVat.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPreviousVat.Validating += new System.ComponentModel.CancelEventHandler(this.Numeric_Validating);
            // 
            // lblPrevoiusVat
            // 
            this.lblPrevoiusVat.AutoSize = true;
            this.lblPrevoiusVat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.lblPrevoiusVat.Location = new System.Drawing.Point(17, 172);
            this.lblPrevoiusVat.Name = "lblPrevoiusVat";
            this.lblPrevoiusVat.Size = new System.Drawing.Size(91, 16);
            this.lblPrevoiusVat.TabIndex = 7;
            this.lblPrevoiusVat.Text = "Previous VAT";
            // 
            // txtpreviousRate
            // 
            this.txtpreviousRate.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtpreviousRate.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            // 
            // 
            // 
            this.txtpreviousRate.Border.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(163)))), ((int)(((byte)(165)))));
            this.txtpreviousRate.Border.Class = "TextBoxBorder";
            this.txtpreviousRate.Enabled = false;
            this.txtpreviousRate.Location = new System.Drawing.Point(118, 103);
            this.txtpreviousRate.Name = "txtpreviousRate";
            this.txtpreviousRate.Size = new System.Drawing.Size(107, 22);
            this.txtpreviousRate.TabIndex = 2;
            this.txtpreviousRate.Text = "0.00";
            this.txtpreviousRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtpreviousRate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AutoComplete_KeyDown);
            this.txtpreviousRate.Validating += new System.ComponentModel.CancelEventHandler(this.Numeric_Validating);
            // 
            // lblPreviousRate
            // 
            this.lblPreviousRate.AutoSize = true;
            this.lblPreviousRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.lblPreviousRate.Location = new System.Drawing.Point(17, 105);
            this.lblPreviousRate.Name = "lblPreviousRate";
            this.lblPreviousRate.Size = new System.Drawing.Size(93, 16);
            this.lblPreviousRate.TabIndex = 5;
            this.lblPreviousRate.Text = "Previous Rate";
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(73)))), ((int)(((byte)(125)))));
            this.lblItem.Location = new System.Drawing.Point(17, 59);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(73, 16);
            this.lblItem.TabIndex = 3;
            this.lblItem.Text = "Item Name";
            // 
            // panelEx1
            // 
            this.panelEx1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.Controls.Add(this.btnClear);
            this.panelEx1.Controls.Add(this.btnSaveClose);
            this.panelEx1.Controls.Add(this.btnSaveNew);
            this.panelEx1.Controls.Add(this.btnClose);
            this.panelEx1.Location = new System.Drawing.Point(0, 47);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(734, 24);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.panelEx1.Style.BackColor2.Color = System.Drawing.Color.LightSteelBlue;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.BorderSide = DevComponents.DotNetBar.eBorderSide.None;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 1;
            // 
            // btnClear
            // 
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(175)))), ((int)(((byte)(5)))));
            this.btnClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(36)))), ((int)(((byte)(62)))));
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.Location = new System.Drawing.Point(289, -1);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(131, 25);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clea&r";
            this.btnClear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSaveClose
            // 
            this.btnSaveClose.FlatAppearance.BorderSize = 0;
            this.btnSaveClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(175)))), ((int)(((byte)(5)))));
            this.btnSaveClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.btnSaveClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(36)))), ((int)(((byte)(62)))));
            this.btnSaveClose.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveClose.Image")));
            this.btnSaveClose.Location = new System.Drawing.Point(152, -1);
            this.btnSaveClose.Name = "btnSaveClose";
            this.btnSaveClose.Size = new System.Drawing.Size(131, 25);
            this.btnSaveClose.TabIndex = 1;
            this.btnSaveClose.Text = "Save && Cl&ose";
            this.btnSaveClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveClose.UseVisualStyleBackColor = true;
            this.btnSaveClose.Click += new System.EventHandler(this.btnSaveClose_Click);
            // 
            // btnSaveNew
            // 
            this.btnSaveNew.FlatAppearance.BorderSize = 0;
            this.btnSaveNew.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(175)))), ((int)(((byte)(5)))));
            this.btnSaveNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.btnSaveNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveNew.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(36)))), ((int)(((byte)(62)))));
            this.btnSaveNew.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveNew.Image")));
            this.btnSaveNew.Location = new System.Drawing.Point(14, -1);
            this.btnSaveNew.Name = "btnSaveNew";
            this.btnSaveNew.Size = new System.Drawing.Size(131, 25);
            this.btnSaveNew.TabIndex = 0;
            this.btnSaveNew.Text = "Save && Ne&w";
            this.btnSaveNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveNew.UseVisualStyleBackColor = true;
            this.btnSaveNew.Click += new System.EventHandler(this.btnSaveNew_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(175)))), ((int)(((byte)(5)))));
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(36)))), ((int)(((byte)(62)))));
            this.btnClose.Image = global::SweetPOS.Properties.Resources.stop;
            this.btnClose.Location = new System.Drawing.Point(647, -1);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(70, 25);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "&Close";
            this.btnClose.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelEx2
            // 
            this.panelEx2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelEx2.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx2.Location = new System.Drawing.Point(0, 46);
            this.panelEx2.Name = "panelEx2";
            this.panelEx2.Size = new System.Drawing.Size(734, 2);
            this.panelEx2.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx2.Style.BackColor1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panelEx2.Style.BackColor2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panelEx2.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx2.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx2.Style.BorderSide = DevComponents.DotNetBar.eBorderSide.None;
            this.panelEx2.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx2.Style.GradientAngle = 90;
            this.panelEx2.TabIndex = 5;
            // 
            // panelEx3
            // 
            this.panelEx3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelEx3.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx3.Controls.Add(this.pictureBox1);
            this.panelEx3.Controls.Add(this.lblHeading);
            this.panelEx3.Location = new System.Drawing.Point(0, 0);
            this.panelEx3.Name = "panelEx3";
            this.panelEx3.Size = new System.Drawing.Size(734, 46);
            this.panelEx3.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx3.Style.BackColor1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.panelEx3.Style.BackColor2.Color = System.Drawing.Color.SteelBlue;
            this.panelEx3.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx3.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx3.Style.BorderSide = DevComponents.DotNetBar.eBorderSide.None;
            this.panelEx3.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx3.Style.GradientAngle = 180;
            this.panelEx3.TabIndex = 6;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(14, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(39, 31);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblHeading.Location = new System.Drawing.Point(59, 8);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(141, 25);
            this.lblHeading.TabIndex = 0;
            this.lblHeading.Text = "Rate Details";
            // 
            // panelEx4
            // 
            this.panelEx4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelEx4.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx4.Location = new System.Drawing.Point(0, 71);
            this.panelEx4.Name = "panelEx4";
            this.panelEx4.Size = new System.Drawing.Size(734, 3);
            this.panelEx4.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx4.Style.BackColor1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(36)))), ((int)(((byte)(62)))));
            this.panelEx4.Style.BackColor2.Color = System.Drawing.SystemColors.Control;
            this.panelEx4.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx4.Style.BorderSide = DevComponents.DotNetBar.eBorderSide.None;
            this.panelEx4.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx4.Style.GradientAngle = 90;
            this.panelEx4.TabIndex = 7;
            // 
            // frmRate
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(734, 492);
            this.Controls.Add(this.panelEx1);
            this.Controls.Add(this.panelEx4);
            this.Controls.Add(this.panelEx3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panelEx2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRate";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmRate_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frmItem_KeyPress);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panelEx1.ResumeLayout(false);
            this.panelEx3.ResumeLayout(false);
            this.panelEx3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private DevComponents.DotNetBar.PanelEx panelEx1;
        private DevComponents.DotNetBar.PanelEx panelEx2;
        private DevComponents.DotNetBar.PanelEx panelEx3;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblItem;
        private DevComponents.DotNetBar.Controls.TextBoxX txtpreviousRate;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblPreviousRate;
        private System.Windows.Forms.Label lblPrevoiusVat;
        private DevComponents.DotNetBar.Controls.TextBoxX txtPreviousVat;
        private System.Windows.Forms.Button btnSaveNew;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSaveClose;
        private DevComponents.DotNetBar.PanelEx panelEx4;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblNewRate;
        private DevComponents.DotNetBar.Controls.TextBoxX txtNewRate;
        private System.Windows.Forms.ComboBox cboItem;
        private System.Windows.Forms.Label lblEffectiveFrom;
        private DevComponents.DotNetBar.Controls.TextBoxX txtNewVat;
        private System.Windows.Forms.Label lblNewVat;
        private System.Windows.Forms.DateTimePicker dtSDate;
    }
}