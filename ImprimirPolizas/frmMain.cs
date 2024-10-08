﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeviceId;
using DeviceId.Windows;
using IronPdf;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PdfiumViewer;
using Segment.Analytics;
using Segment.Serialization;
using static ImprimirPolizas.ScTools;

namespace ImprimirPolizas
{
    public partial class frmMain : Form
    {
        private int[] options = new int[6];
        private readonly string downloadFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "descargas"
        );
        public CancellationTokenSource cts; // Token para cancelar las tareas

        private enum IconState
        {
            Loading = 0,
            Ready = 1,
            Error = 2,
        }

        private enum BranchIcon
        {
            Moto = 3,
            Car = 4,
            Document = 5,
            House = 6
        }

        static readonly Configuration SegmentConfig = new Configuration(
            "3agF8DVa82eZaBk1GYxaiYqhAi2f9fv1",
            flushAt: 20,
            flushInterval: 30
        );
        Analytics analytics = new Analytics(SegmentConfig);

        string deviceId = new DeviceIdBuilder()
            .OnWindows(windows => windows.AddWindowsDeviceId())
            .ToString();

        List<Task> _printTasks;
        bool _hasFailed = false;

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            analytics.Track("app_open", new JsonObject { ["anonymousId"] = deviceId, });
            EnableWhenReady(); // Verificar disponibilidad servidor
            // Habilitar opciones iniciales
            options[0] = (int)ScTools.DownloadOpt.policy;
            options[1] = (int)ScTools.DownloadOpt.policyCard;

            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text += $" - v{assemblyVersion}";

            pbBranch.Image = iconsList.Images[(int)BranchIcon.Document];
        }

        private async void EnableWhenReady()
        {
            // Deshabilitar controles
            lblStatus.AutoEllipsis = true; // Habilita truncado de texto
            lblStatus.Text = $"Conectando {ScTools.BaseUrl}...";
            EnableControls(groupBox1, false);
            bool isAvailable = await ScTools.IsAvailable();
            if (isAvailable)
            {
                // Si el server está disponible, habilitar
                lblStatus.Text = "Listo";
                lblStatus.ForeColor = Color.Green;
                EnableControls(groupBox1, true);
            }
            else
            {
                lblStatus.Text = "Error";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show(
                    "Error al conectar con ScTools",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void EnableControls(Control control, bool setEnabled)
        {
            foreach (Control ctrl in control.Controls)
            {
                // Si es label de status, saltar
                if (ctrl.Equals(lblStatus) || ctrl.Equals(label1) || ctrl.Equals(btnPrint))
                    continue;
                if (ctrl is PictureBox)
                    continue;
                // Si es un groupbox, recorrer controles
                if (ctrl is GroupBox)
                {
                    foreach (Control ctrlGB in ctrl.Controls)
                    {
                        if (ctrlGB is PictureBox)
                            continue;
                        ctrlGB.Enabled = setEnabled;
                    }
                }
                else
                {
                    ctrl.Enabled = setEnabled;
                }
            }
        }

        // Deshabilita los checkbox que no corresppondan a la rama ingresada
        private void SetCheckboxByBranch()
        {
            switch (GetBranchNumber(txtPolicy.Text))
            {
                case "21":
                    chkMercosur.Enabled = false; // FIX: Por ahora no esta habilitado para motos
                    chkMercosur.Checked = false;
                    break;
                case "07":
                    chkPolicyCard.Enabled = false;
                    chkPolicyCard.Checked = false;
                    chkInvoice.Enabled = false;
                    chkInvoice.Checked = false;
                    chkMercosur.Enabled = false;
                    chkMercosur.Checked = false;
                    break;
                case "06":
                    chkPolicyCard.Enabled = false;
                    chkPolicyCard.Checked = false;
                    chkInvoice.Enabled = false;
                    chkInvoice.Checked = false;
                    chkMercosur.Enabled = false;
                    chkMercosur.Checked = false;
                    break;
                default:
                    break;
            }
        }

        private void ResetAllStatus(bool wait = true)
        {
            // Resetear iconos y estado esperando 5 seg
            // De forma asíncrona sin bloquear UI
            Task.Run(async () =>
            {
                if (wait)
                {
                    await Task.Delay(4000);
                }
                pbPolicy.Image = null;
                pbCard.Image = null;
                pbPayment.Image = null;
                pbCoupons.Image = null;
                pbInvoice.Image = null;
                pbMercosur.Image = null;
            });
        }

        private void PrintPDF(string filePath, int fromPage = 1, int toPage = 1)
        {
            // Create the printer settings for our printer
            var printerSettings = new PrinterSettings
            {
                FromPage = fromPage,
                ToPage = toPage,
                Copies = 1
            };

            // Configuración de páginas
            var pageSettings = new PageSettings(printerSettings)
            {
                Margins = new Margins(0, 0, 0, 0),
            };
            foreach (PaperSize paperSize in printerSettings.PaperSizes)
            {
                if (paperSize.PaperName == "A4")
                {
                    pageSettings.PaperSize = paperSize;
                    break;
                }
            }

            // Cargar PDF
            // using se asegura que el archivo sea cerrado incluso si hay un error
            using (var doc = PdfiumViewer.PdfDocument.Load(filePath))
            {
                using (var printDocument = doc.CreatePrintDocument())
                {
                    printDocument.PrinterSettings = printerSettings;
                    printDocument.DefaultPageSettings = pageSettings;
                    printDocument.PrintController = new StandardPrintController();
                    printDocument.Print(); // Imprime con la impresora predeterminada
                }
            }
        }

        private void GroupBox1_Enter(object sender, EventArgs e) { }

        private void TxtPolicy_TextChanged(object sender, EventArgs e)
        {
            if (txtPolicy.TextLength < 17)
            {
                EnableControls(groupBox2, false);
                btnPrint.Enabled = false;
                pbBranch.Image = iconsList.Images[(int)BranchIcon.Document];
            }
            else
            {
                SetBranchIcon();
                EnableControls(groupBox2, true);
                EnableBtnPrint();
                SetCheckboxByBranch(); // Verificar que checkboxes corresponden
            }
        }

        private void SetBranchIcon()
        {
            switch (GetBranchNumber(txtPolicy.Text))
            {
                case "01":
                    pbBranch.Image = iconsList.Images[(int)BranchIcon.Car];
                    break;
                case "21":
                    pbBranch.Image = iconsList.Images[(int)BranchIcon.Moto];
                    break;
                case "07":
                    pbBranch.Image = iconsList.Images[(int)BranchIcon.House];
                    break;
                default:
                    pbBranch.Image = iconsList.Images[(int)BranchIcon.Document];
                    break;
            }
        }

        private bool branchHasInvoice(string policyNumber)
        {
            switch (GetBranchNumber(policyNumber))
            {
                case "01":
                    return true;
                case "21":
                    return true;
                default:
                    return false;
            }
        }

        private async Task NotifyInvoiceRequired(string policyNumber)
        {
            ScTools.DownloadOpt opt = ScTools.DownloadOpt.invoice;

            if (!branchHasInvoice(policyNumber))
                return;

            try
            {
                if (!await ScTools.requiresInvoice(policyNumber, cts.Token))
                    return;

                GetFocused(); // traer ventana al frente
                string actualAction = rbPrint.Checked ? "imprimir" : "descargar";
                DialogResult result = MessageBox.Show(
                    "La categoría del socio es R.I o Monotributo\n"
                        + $"Desea {actualAction} la Factura?",
                    "Aviso",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly
                );
                if (result == DialogResult.No)
                    return;

                cts.Token.ThrowIfCancellationRequested();
                ChangeCheckFromTask(chkInvoice, true);
                SetIconStatus(ScTools.DownloadOpt.invoice, IconState.Loading);
                await ScTools.DownloadDocAsync(policyNumber, 1, opt, downloadFolder, cts.Token);
                if (rbPrint.Checked)
                {
                    ChangeStatusFromTask("Imprimiendo " + ScTools.GetOptionName(opt) + "...");
                    string filePath = Path.Combine(
                        downloadFolder,
                        ScTools.GetFileName(policyNumber, opt)
                    );
                    PrintPDF(filePath);
                }
                SetIconStatus(opt, IconState.Ready);
            }
            catch (OperationCanceledException)
            {
                return; // ignorar si se cancela
            }
            catch (Exception ex)
            {
                if (rbPrint.Checked)
                {
                    SetIconStatus(opt, IconState.Error);
                }
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnPrint_Click(object sender, EventArgs e)
        {
            if (btnPrint.Text == "CANCELAR")
            {
                cts.Cancel(); // Cancelar todo
                EnableControls(this, true); // rehabilitar controles
                return;
            }
            string pcNumber = txtPolicy.Text;
            lblStatus.ForeColor = Color.Black;
            EnableControls(this, false); // deshabilitar controles
            btnPrint.Text = "CANCELAR";
            _hasFailed = false;

            // Resetear token si ya se usó
            cts?.Dispose();
            // Crea nuevo token
            cts = new CancellationTokenSource();

            _printTasks = new List<Task>();
            if (!chkInvoice.Checked) // Verifica solo si no fue seleccionada la opción Factura
            {
                // Se agrega una tarea a la lista que debe ser esperada para habilitar
                // de nuevo el formulario y el botón imprimir/descargar
                _printTasks.Add(
                    Task.Run(async () => await NotifyInvoiceRequired(pcNumber), cts.Token)
                );
            }

            await GetPrintDocs(pcNumber);

            // Esperar que terminen todas las tasks
            await Task.WhenAll(_printTasks);
            btnPrint.Text = rbPrint.Checked ? "IMPRIMIR" : "DESCARGAR";
            lblStatus.Text = _hasFailed ? "Error" : "Listo";
            lblStatus.ForeColor = _hasFailed ? Color.Red : Color.Green;
            lblStatus.Refresh();

            if (cts.IsCancellationRequested)
            {
                lblStatus.Text = "Operación Cancelada.";
                lblStatus.ForeColor = Color.Red;
            }
            if (!_hasFailed && !cts.IsCancellationRequested && !lnkDownloads.Visible)
            {
                lnkDownloads.Visible = true;
                lnkLblCopyDocs.Visible = true;
            }
            ResetAllStatus(!cts.IsCancellationRequested); // Reset de iconos y estado
            EnableControls(this, true); // rehabilitar controles
            SetCheckboxByBranch(); // Deshabilitar los que no correspondan
        }

        private async Task GetPrintDocs(string pcNumber)
        {
            PolicyDocs pcDocs = new PolicyDocs();
            if (IsCarPolicy(pcNumber))
            {
                lblStatus.Text = "Buscando documentos...";
                try
                {
                    pcDocs = await ScTools.GetPolicyDocs(pcNumber, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    ChangeStatusFromTask("Operación Cancelada.");
                    return;
                }
                catch // Si da error el servidor, dejar pcDocs null para que use el otro método de descarga
                {
                    pcDocs = null;
                }
            }
            else
            {
                pcDocs = null;
            }

            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == 0)
                    continue;
                ScTools.DownloadOpt opt = (ScTools.DownloadOpt)options[i];
                lblStatus.Text = "Descargando archivos...";
                _printTasks.Add(
                    Task.Run(
                        async () =>
                        {
                            try
                            {
                                cts.Token.ThrowIfCancellationRequested();

                                SetIconStatus(opt, IconState.Loading);

                                await TryDownload(pcDocs, pcNumber, opt, downloadFolder, cts.Token);

                                if (rbPrint.Checked)
                                {
                                    ChangeStatusFromTask(
                                        "Imprimiendo " + ScTools.GetOptionName(opt) + "..."
                                    );
                                    string filePath = Path.Combine(
                                        downloadFolder,
                                        ScTools.GetFileName(pcNumber, opt)
                                    );
                                    PrintPDF(filePath);
                                }

                                SetIconStatus(opt, IconState.Ready);
                                // Se actualizan las estadísticas pero sin esperar respuesta
                                _ = ScTools.UpdateStats(
                                    opt == ScTools.DownloadOpt.policy,
                                    rbPrint.Checked,
                                    cts.Token
                                );
                                TrackAction(opt, rbPrint.Checked); // Send Analytics
                            }
                            catch (OperationCanceledException)
                            {
                                ChangeStatusFromTask("Operación Cancelada.");
                            }
                            catch (Exception ex)
                            {
                                SetIconStatus(opt, IconState.Error);
                                if (!_hasFailed) // Muestra el mensaje de error solo la 1er vez que falla
                                {
                                    _hasFailed = true;
                                    MessageBox.Show(
                                        ex.Message,
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error
                                    );
                                }
                            }
                        },
                        cts.Token
                    )
                );
            }
        }

        private async Task TryDownload(
            PolicyDocs pcDocs,
            string pcNumber,
            DownloadOpt opt,
            string downloadFolder,
            CancellationToken ct
        )
        {
            bool firstTryFail = false;

            // Si no está definido no es ramo Automotor o dio error la nueva API de documentos
            if (pcDocs != null)
            {
                try
                {
                    await ScTools.DownloadDocAsync(
                        pcDocs.GetIdByOption(opt),
                        pcNumber,
                        opt,
                        downloadFolder,
                        ct
                    );
                    return;
                }
                catch
                {
                    Debug.Print("Falló el primer intento de descarga.");
                    firstTryFail = true;
                }
            }

            if (firstTryFail || pcDocs == null)
            {
                await ScTools.DownloadDocAsync(pcNumber, 1, opt, downloadFolder, ct);
            }
        }

        private void SetIconStatus(ScTools.DownloadOpt opt, IconState icon)
        {
            var img = iconsList.Images[(int)icon];
            if (icon == IconState.Loading)
                img = Properties.Resources.loading;
            switch (opt)
            {
                case ScTools.DownloadOpt.policy:
                    pbPolicy.Image = img;
                    break;
                case ScTools.DownloadOpt.policyCard:
                    pbCard.Image = img;
                    break;
                case ScTools.DownloadOpt.paymentReceipt:
                    pbPayment.Image = img;
                    break;
                case ScTools.DownloadOpt.coupons:
                    pbCoupons.Image = img;
                    break;
                case ScTools.DownloadOpt.invoice:
                    pbInvoice.Image = img;
                    break;
                case ScTools.DownloadOpt.mercosur:
                    pbMercosur.Image = img;
                    break;
                default:
                    break;
            }
        }

        private void OpenDownloadsFolder()
        {
            System.Diagnostics.Process.Start("explorer.exe", @downloadFolder);
        }

        private void ChangeStatusFromTask(string text)
        {
            lblStatus.Invoke(
                (
                    new MethodInvoker(
                        delegate
                        {
                            lblStatus.Text = text;
                        }
                    )
                )
            );
        }

        private void ChangeCheckFromTask(CheckBox chk, bool check)
        {
            chk.Invoke(
                (
                    new MethodInvoker(
                        delegate
                        {
                            chk.Checked = check;
                        }
                    )
                )
            );
        }

        // Permite traer la ventana al frente desde
        // un subproceso iniciado por una Task.
        private void GetFocused()
        {
            this.Invoke(
                (
                    new MethodInvoker(
                        delegate
                        {
                            this.Activate();
                        }
                    )
                )
            );
        }

        private void ChkPolicy_CheckedChanged(object sender, EventArgs e)
        {
            EnableBtnPrint();
            if (chkPolicy.Checked)
            {
                options[0] = (int)ScTools.DownloadOpt.policy;
            }
            else
            {
                options[0] = 0;
            }
        }

        private void ChkPolicyCard_CheckedChanged(object sender, EventArgs e)
        {
            EnableBtnPrint();
            if (chkPolicyCard.Checked)
            {
                options[1] = (int)ScTools.DownloadOpt.policyCard;
            }
            else
            {
                options[1] = 0;
            }
        }

        private void ChkPaymentProof_CheckedChanged(object sender, EventArgs e)
        {
            EnableBtnPrint();
            if (chkPaymentProof.Checked)
            {
                options[2] = (int)ScTools.DownloadOpt.paymentReceipt;
            }
            else
            {
                options[2] = 0;
            }
        }

        private void EnableBtnPrint()
        {
            int totalChecked = 0;
            //if (sender.Equals(btnPrint)) return; // Si se deshabilita, no comprobar nada

            if (txtPolicy.Text.Length < 17)
                return;
            foreach (Control ctrl in groupBox2.Controls)
            {
                // Si no es CheckBox, continuar con otro
                if (!(ctrl is CheckBox))
                    continue;
                if (((CheckBox)ctrl).Checked)
                {
                    totalChecked++;
                }
            }

            if (totalChecked == 0 || txtPolicy.Text.Length < 17)
            {
                btnPrint.Enabled = false;
            }
            else
            {
                btnPrint.Enabled = true;
            }
        }

        private void RbPrint_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPrint.Checked)
                btnPrint.Text = "IMPRIMIR";
        }

        private void RbDownload_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDownload.Checked)
                btnPrint.Text = "DESCARGAR";
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Directory.Exists(downloadFolder))
                Directory.Delete(downloadFolder, true);
        }

        private void LnkDownloads_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            analytics.Track("btn_download_click", new JsonObject { ["anonymousId"] = deviceId, });
            OpenDownloadsFolder();
        }

        private void chkCoupons_CheckedChanged(object sender, EventArgs e)
        {
            EnableBtnPrint();
            if (chkCoupons.Checked)
            {
                options[3] = (int)ScTools.DownloadOpt.coupons;
            }
            else
            {
                options[3] = 0;
            }
        }

        private void chkInvoice_CheckedChanged(object sender, EventArgs e)
        {
            if (btnPrint.Text != "Aguarde...")
                EnableBtnPrint();
            if (chkInvoice.Checked)
            {
                options[4] = (int)ScTools.DownloadOpt.invoice;
            }
            else
            {
                options[4] = 0;
            }
        }

        private void TrackAction(ScTools.DownloadOpt opt, bool isPrinting)
        {
            string eventName = isPrinting ? "print_" : "download_";
            eventName += GetDocName(opt);
            analytics.Track(
                eventName,
                new JsonObject { ["anonymousId"] = deviceId, ["document"] = GetDocName(opt), }
            );
        }

        private string GetDocName(ScTools.DownloadOpt opt)
        {
            string docName;
            switch (opt)
            {
                case DownloadOpt.policy:
                    docName = "policy";
                    break;
                case DownloadOpt.paymentReceipt:
                    docName = "paymentReceipt";
                    break;
                case DownloadOpt.policyCard:
                    docName = "policyCard";
                    break;
                case DownloadOpt.coupons:
                    docName = "coupons";
                    break;
                case DownloadOpt.invoice:
                    docName = "invoice";
                    break;
                default:
                    docName = "unknown";
                    break;
            }
            return docName;
        }

        private void lnkLblCopyDocs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EnableControls(this, false);
            lblStatus.ForeColor = Color.Black;
            lblStatus.Text = "Copiando documentos...";
            lblStatus.Refresh();
            ConvertDownloadedDocs();
            CopyDownloadedDocs();
            EnableControls(this, true);
            lblStatus.Text = "Listo";
            lblStatus.ForeColor = Color.Green;
        }

        private void ConvertDownloadedDocs()
        {
            lblStatus.Text = "Copiando documentos...";
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == 0)
                    continue;

                ScTools.DownloadOpt opt = (ScTools.DownloadOpt)options[i];
                string filePath = Path.Combine(
                    downloadFolder,
                    ScTools.GetFileName(txtPolicy.Text, opt)
                );

                if (new FileInfo(filePath).Length == 0)
                {
                    // Archivo vacío
                    continue;
                }

                ConvertPdf(filePath);
            }
        }

        private void CopyDownloadedDocs()
        {
            StringCollection docsPathList = new StringCollection();

            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == 0)
                    continue;

                ScTools.DownloadOpt opt = (ScTools.DownloadOpt)options[i];
                string filePath = Path.Combine(
                    downloadFolder,
                    ScTools.GetFileName(txtPolicy.Text, opt)
                );

                if (new FileInfo(filePath).Length == 0)
                {
                    // Archivo vacío
                    continue;
                }

                docsPathList.Add(filePath + ".png");
            }

            Clipboard.SetFileDropList(docsPathList);
        }

        private void ConvertPdf(string filePath)
        {
            IronPdf.PdfDocument pdf = IronPdf.PdfDocument.FromFile(filePath);
            IEnumerable<int> pageIndexes = Enumerable.Range(0, 1);
            pdf.RasterizeToImageFiles(
                filePath + ".png",
                pageIndexes,
                2550,
                1950,
                IronPdf.Imaging.ImageType.Png,
                300
            );
        }

        private void chkMercosur_CheckedChanged(object sender, EventArgs e)
        {
            if (btnPrint.Text != "Aguarde...")
                EnableBtnPrint();
            if (chkMercosur.Checked)
            {
                options[5] = (int)ScTools.DownloadOpt.mercosur;
            }
            else
            {
                options[5] = 0;
            }
        }
    }
}
