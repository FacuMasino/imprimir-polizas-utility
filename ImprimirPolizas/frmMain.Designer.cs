namespace ImprimirPolizas
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPolicy = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pbPayment = new System.Windows.Forms.PictureBox();
            this.pbCard = new System.Windows.Forms.PictureBox();
            this.pbPolicy = new System.Windows.Forms.PictureBox();
            this.rbDownload = new System.Windows.Forms.RadioButton();
            this.rbPrint = new System.Windows.Forms.RadioButton();
            this.chkPaymentProof = new System.Windows.Forms.CheckBox();
            this.chkPolicyCard = new System.Windows.Forms.CheckBox();
            this.chkPolicy = new System.Windows.Forms.CheckBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.iconsList = new System.Windows.Forms.ImageList(this.components);
            this.lnkDownloads = new System.Windows.Forms.LinkLabel();
            this.chkCoupons = new System.Windows.Forms.CheckBox();
            this.pbCoupons = new System.Windows.Forms.PictureBox();
            this.chkInvoice = new System.Windows.Forms.CheckBox();
            this.pbInvoice = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPayment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPolicy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoupons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInvoice)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPolicy);
            this.groupBox1.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(99, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Número de Póliza";
            this.groupBox1.Enter += new System.EventHandler(this.GroupBox1_Enter);
            // 
            // txtPolicy
            // 
            this.txtPolicy.Font = new System.Drawing.Font("Roboto", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPolicy.Location = new System.Drawing.Point(36, 25);
            this.txtPolicy.MaxLength = 17;
            this.txtPolicy.Name = "txtPolicy";
            this.txtPolicy.Size = new System.Drawing.Size(203, 30);
            this.txtPolicy.TabIndex = 0;
            this.txtPolicy.WordWrap = false;
            this.txtPolicy.TextChanged += new System.EventHandler(this.TxtPolicy_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pbInvoice);
            this.groupBox2.Controls.Add(this.pbCoupons);
            this.groupBox2.Controls.Add(this.pbPayment);
            this.groupBox2.Controls.Add(this.pbCard);
            this.groupBox2.Controls.Add(this.pbPolicy);
            this.groupBox2.Controls.Add(this.rbDownload);
            this.groupBox2.Controls.Add(this.rbPrint);
            this.groupBox2.Controls.Add(this.chkInvoice);
            this.groupBox2.Controls.Add(this.chkCoupons);
            this.groupBox2.Controls.Add(this.chkPaymentProof);
            this.groupBox2.Controls.Add(this.chkPolicyCard);
            this.groupBox2.Controls.Add(this.chkPolicy);
            this.groupBox2.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(99, 133);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(270, 237);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Seleccione opciones";
            // 
            // pbPayment
            // 
            this.pbPayment.Location = new System.Drawing.Point(195, 92);
            this.pbPayment.Name = "pbPayment";
            this.pbPayment.Size = new System.Drawing.Size(25, 25);
            this.pbPayment.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPayment.TabIndex = 6;
            this.pbPayment.TabStop = false;
            // 
            // pbCard
            // 
            this.pbCard.Location = new System.Drawing.Point(195, 61);
            this.pbCard.Name = "pbCard";
            this.pbCard.Size = new System.Drawing.Size(25, 25);
            this.pbCard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbCard.TabIndex = 5;
            this.pbCard.TabStop = false;
            // 
            // pbPolicy
            // 
            this.pbPolicy.Location = new System.Drawing.Point(195, 30);
            this.pbPolicy.Name = "pbPolicy";
            this.pbPolicy.Size = new System.Drawing.Size(25, 25);
            this.pbPolicy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPolicy.TabIndex = 3;
            this.pbPolicy.TabStop = false;
            // 
            // rbDownload
            // 
            this.rbDownload.AutoSize = true;
            this.rbDownload.Enabled = false;
            this.rbDownload.Location = new System.Drawing.Point(97, 207);
            this.rbDownload.Name = "rbDownload";
            this.rbDownload.Size = new System.Drawing.Size(127, 22);
            this.rbDownload.TabIndex = 2;
            this.rbDownload.Text = "Solo descargar";
            this.rbDownload.UseVisualStyleBackColor = true;
            this.rbDownload.CheckedChanged += new System.EventHandler(this.RbDownload_CheckedChanged);
            // 
            // rbPrint
            // 
            this.rbPrint.AutoSize = true;
            this.rbPrint.Checked = true;
            this.rbPrint.Enabled = false;
            this.rbPrint.Location = new System.Drawing.Point(9, 207);
            this.rbPrint.Name = "rbPrint";
            this.rbPrint.Size = new System.Drawing.Size(82, 22);
            this.rbPrint.TabIndex = 1;
            this.rbPrint.TabStop = true;
            this.rbPrint.Text = "Imprimir";
            this.rbPrint.UseVisualStyleBackColor = true;
            this.rbPrint.CheckedChanged += new System.EventHandler(this.RbPrint_CheckedChanged);
            // 
            // chkPaymentProof
            // 
            this.chkPaymentProof.AutoSize = true;
            this.chkPaymentProof.Enabled = false;
            this.chkPaymentProof.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPaymentProof.Location = new System.Drawing.Point(8, 94);
            this.chkPaymentProof.Name = "chkPaymentProof";
            this.chkPaymentProof.Size = new System.Drawing.Size(186, 23);
            this.chkPaymentProof.TabIndex = 0;
            this.chkPaymentProof.Text = "Comprobante de Pago";
            this.chkPaymentProof.UseVisualStyleBackColor = true;
            this.chkPaymentProof.CheckedChanged += new System.EventHandler(this.ChkPaymentProof_CheckedChanged);
            // 
            // chkPolicyCard
            // 
            this.chkPolicyCard.AutoSize = true;
            this.chkPolicyCard.Checked = true;
            this.chkPolicyCard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPolicyCard.Enabled = false;
            this.chkPolicyCard.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPolicyCard.Location = new System.Drawing.Point(8, 63);
            this.chkPolicyCard.Name = "chkPolicyCard";
            this.chkPolicyCard.Size = new System.Drawing.Size(160, 23);
            this.chkPolicyCard.TabIndex = 0;
            this.chkPolicyCard.Text = "Tarjeta Circulación";
            this.chkPolicyCard.UseVisualStyleBackColor = true;
            this.chkPolicyCard.CheckedChanged += new System.EventHandler(this.ChkPolicyCard_CheckedChanged);
            // 
            // chkPolicy
            // 
            this.chkPolicy.AutoSize = true;
            this.chkPolicy.Checked = true;
            this.chkPolicy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPolicy.Enabled = false;
            this.chkPolicy.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPolicy.Location = new System.Drawing.Point(8, 30);
            this.chkPolicy.Name = "chkPolicy";
            this.chkPolicy.Size = new System.Drawing.Size(141, 23);
            this.chkPolicy.TabIndex = 0;
            this.chkPolicy.Text = "Frente de Póliza";
            this.chkPolicy.UseVisualStyleBackColor = true;
            this.chkPolicy.CheckedChanged += new System.EventHandler(this.ChkPolicy_CheckedChanged);
            // 
            // btnPrint
            // 
            this.btnPrint.Enabled = false;
            this.btnPrint.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Location = new System.Drawing.Point(162, 376);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(131, 43);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "IMPRIMIR";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.EnabledChanged += new System.EventHandler(this.BtnPrint_EnabledChanged);
            this.btnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(68, 424);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(393, 18);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "estado";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 424);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Estado:";
            // 
            // iconsList
            // 
            this.iconsList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iconsList.ImageStream")));
            this.iconsList.TransparentColor = System.Drawing.Color.Transparent;
            this.iconsList.Images.SetKeyName(0, "loading.gif");
            this.iconsList.Images.SetKeyName(1, "checkIcon");
            this.iconsList.Images.SetKeyName(2, "errorIcon");
            // 
            // lnkDownloads
            // 
            this.lnkDownloads.AutoSize = true;
            this.lnkDownloads.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkDownloads.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(22)))), ((int)(((byte)(105)))));
            this.lnkDownloads.Location = new System.Drawing.Point(355, 424);
            this.lnkDownloads.Name = "lnkDownloads";
            this.lnkDownloads.Size = new System.Drawing.Size(106, 18);
            this.lnkDownloads.TabIndex = 5;
            this.lnkDownloads.TabStop = true;
            this.lnkDownloads.Text = "Ver Descargas";
            this.lnkDownloads.Visible = false;
            this.lnkDownloads.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LnkDownloads_LinkClicked);
            // 
            // chkCoupons
            // 
            this.chkCoupons.AutoSize = true;
            this.chkCoupons.Enabled = false;
            this.chkCoupons.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCoupons.Location = new System.Drawing.Point(8, 126);
            this.chkCoupons.Name = "chkCoupons";
            this.chkCoupons.Size = new System.Drawing.Size(151, 23);
            this.chkCoupons.TabIndex = 0;
            this.chkCoupons.Text = "Cupones de pago";
            this.chkCoupons.UseVisualStyleBackColor = true;
            this.chkCoupons.CheckedChanged += new System.EventHandler(this.chkCoupons_CheckedChanged);
            // 
            // pbCoupons
            // 
            this.pbCoupons.Location = new System.Drawing.Point(195, 124);
            this.pbCoupons.Name = "pbCoupons";
            this.pbCoupons.Size = new System.Drawing.Size(25, 25);
            this.pbCoupons.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbCoupons.TabIndex = 6;
            this.pbCoupons.TabStop = false;
            // 
            // chkInvoice
            // 
            this.chkInvoice.AutoSize = true;
            this.chkInvoice.Enabled = false;
            this.chkInvoice.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkInvoice.Location = new System.Drawing.Point(8, 159);
            this.chkInvoice.Name = "chkInvoice";
            this.chkInvoice.Size = new System.Drawing.Size(82, 23);
            this.chkInvoice.TabIndex = 0;
            this.chkInvoice.Text = "Factura";
            this.chkInvoice.UseVisualStyleBackColor = true;
            this.chkInvoice.CheckedChanged += new System.EventHandler(this.chkInvoice_CheckedChanged);
            // 
            // pbInvoice
            // 
            this.pbInvoice.Location = new System.Drawing.Point(195, 157);
            this.pbInvoice.Name = "pbInvoice";
            this.pbInvoice.Size = new System.Drawing.Size(25, 25);
            this.pbInvoice.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbInvoice.TabIndex = 6;
            this.pbInvoice.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 451);
            this.Controls.Add(this.lnkDownloads);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Imprimir Pólizas";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPayment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPolicy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoupons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInvoice)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPolicy;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkPolicy;
        private System.Windows.Forms.CheckBox chkPaymentProof;
        private System.Windows.Forms.CheckBox chkPolicyCard;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbPrint;
        private System.Windows.Forms.RadioButton rbDownload;
        private System.Windows.Forms.PictureBox pbPolicy;
        private System.Windows.Forms.ImageList iconsList;
        private System.Windows.Forms.PictureBox pbPayment;
        private System.Windows.Forms.PictureBox pbCard;
        private System.Windows.Forms.LinkLabel lnkDownloads;
        private System.Windows.Forms.PictureBox pbCoupons;
        private System.Windows.Forms.CheckBox chkCoupons;
        private System.Windows.Forms.PictureBox pbInvoice;
        private System.Windows.Forms.CheckBox chkInvoice;
    }
}

