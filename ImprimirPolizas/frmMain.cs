using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using PdfiumViewer;
using Segment.Analytics;
using Segment.Serialization;
using static ImprimirPolizas.ScTools;

namespace ImprimirPolizas
{
    public partial class frmMain : Form
    {
        private int[] options = new int[5];
        private readonly string downloadFolder = Path.Combine(Directory.GetCurrentDirectory(), "descargas");
        private enum IconState
        {
            Loading = 0,
            Ready = 1,
            Error = 2,
        }
        static readonly Configuration SegmentConfig = new Configuration("3agF8DVa82eZaBk1GYxaiYqhAi2f9fv1",
        flushAt: 20,
        flushInterval: 30);
        Analytics analytics = new Analytics(SegmentConfig);

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            analytics.Track("app_open");
            EnableWhenReady(); // Verificar disponibilidad servidor
            // Habilitar opciones iniciales
            options[0] = (int)ScTools.DownloadOpt.policy;
            options[1] = (int)ScTools.DownloadOpt.policyCard;
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
                if (ctrl.Equals(lblStatus) || ctrl.Equals(label1)) continue;
                if (ctrl is PictureBox) continue;
                // Si es un groupbox, recorrer controles
                if (ctrl is GroupBox)
                {
                    foreach (Control ctrlGB in ctrl.Controls)
                    {
                        if (ctrlGB is PictureBox) continue;
                        ctrlGB.Enabled = setEnabled;
                    }
                }
                else
                {
                    ctrl.Enabled = setEnabled;
                }
            }
        }

        private void ResetAllStatus()
        {
            // Resetear iconos y estado esperando 5 seg
            // De forma asíncrona sin bloquear UI
            Task.Run(async () =>
            {
                await Task.Delay(4000);
                pbPolicy.Image = null;
                pbCard.Image = null;
                pbPayment.Image = null;
                pbCoupons.Image = null;
                pbInvoice.Image = null;
                lblStatus.Text = "Listo";
                lblStatus.ForeColor = Color.Green;
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
            using (var doc = PdfDocument.Load(filePath))
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
            }
            else
            {
                EnableControls(groupBox2, true);
                EnableBtnPrint();
            }
        }
        private async Task NotifyInvoiceRequired(string policyNumber)
        {
            if (!await ScTools.requiresInvoice(policyNumber)) return;
            GetFocused(); // traer ventana al frente
            string actualAction = rbPrint.Checked ? "imprimir" : "descargar";
            DialogResult result = MessageBox.Show("La categoría del socio es R.I o Monotributo\n" +
                            $"Desea {actualAction} la Factura?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            if (result == DialogResult.No) return;
            ScTools.DownloadOpt opt = ScTools.DownloadOpt.invoice;
            try
            {
                ChangeCheckFromTask(chkInvoice, true);
                SetIconStatus(ScTools.DownloadOpt.invoice, IconState.Loading);
                await ScTools.DownloadDocAsync(policyNumber, 1, opt, downloadFolder);
                if (rbPrint.Checked)
                {
                    ChangeStatusFromTask("Imprimiendo " + ScTools.GetOptionName(opt) + "...");
                    string filePath = Path.Combine(downloadFolder, ScTools.GetFileName(policyNumber, opt));
                    PrintPDF(filePath);
                }
                SetIconStatus(opt, IconState.Ready);
            }
            catch (Exception ex)
            {
                SetIconStatus(opt, IconState.Error);
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

        }
        private async void BtnPrint_Click(object sender, EventArgs e)
        {
            string pcNumber = txtPolicy.Text;
            lblStatus.ForeColor = Color.Black;
            btnPrint.Text = "Aguarde...";
            EnableControls(this, false); // deshabilitar mientras carga
            bool hasFailed = false;
            List<Task> printTasks = new List<Task>();
            if (!chkInvoice.Checked) // Verifica solo si no fue seleccionada la opción Factura
            {
                // Se agrega una tarea a la lista que debe ser esperada para habilitar
                // de nuevo el formulario y el botón imprimir/descargar
                printTasks.Add(Task.Run(async () => await NotifyInvoiceRequired(pcNumber)));
            }
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == 0)
                    continue;
                ScTools.DownloadOpt opt = (ScTools.DownloadOpt)options[i];
                lblStatus.Text = "Descargando archivos...";
                printTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            SetIconStatus(opt, IconState.Loading);
                            await ScTools.DownloadDocAsync(pcNumber, 1, opt, downloadFolder);
                            if (rbPrint.Checked)
                            {
                                ChangeStatusFromTask("Imprimiendo " + ScTools.GetOptionName(opt) + "...");
                                string filePath = Path.Combine(downloadFolder, ScTools.GetFileName(pcNumber, opt));
                                PrintPDF(filePath);
                            }
                            SetIconStatus(opt, IconState.Ready);
                            // Se actualizan las estadísticas pero sin esperar respuesta
                            _ = ScTools.UpdateStats(opt == ScTools.DownloadOpt.policy, rbPrint.Checked);
                            TrackAction(opt, rbPrint.Checked); // Send Analytics
                        }
                        catch (Exception ex)
                        {
                            SetIconStatus(opt, IconState.Error);
                            if (!hasFailed)
                            {
                                hasFailed = true;
                                MessageBox.Show(
                                    ex.Message,
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error
                                );
                            }
                        }
                    })
                );
            }
            // Esperar que terminen todas las tasks
            await Task.WhenAll(printTasks);
            btnPrint.Text = rbPrint.Checked ? "IMPRIMIR" : "DESCARGAR";
            lblStatus.Text = hasFailed ? "Error" : "Listo";
            lblStatus.ForeColor = hasFailed ? Color.Red : Color.Green;
            if (!hasFailed && !lnkDownloads.Visible) lnkDownloads.Visible = true;
            EnableControls(this, true);
            ResetAllStatus(); // Reset de iconos y estado
        }
        private void OnFrameChanged(object sender, EventArgs args)
        {
        }

        private void SetIconStatus(ScTools.DownloadOpt opt, IconState icon)
        {
            var img = iconsList.Images[(int)icon];
            if (icon == IconState.Loading) img = Properties.Resources.loading;
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
                if (!(ctrl is CheckBox)) continue;
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

        private void BtnPrint_EnabledChanged(object sender, EventArgs e) { }

        private void RbPrint_CheckedChanged(object sender, EventArgs e)
        {
            if(rbPrint.Checked) btnPrint.Text = "IMPRIMIR";
        }

        private void RbDownload_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDownload.Checked) btnPrint.Text = "DESCARGAR";
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Directory.Exists(downloadFolder)) Directory.Delete(downloadFolder, true);
        }

        private void LnkDownloads_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            analytics.Track("btn_download_click");
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
            if(btnPrint.Text != "Aguarde...") EnableBtnPrint();
            if (chkInvoice.Checked)
            {
                options[4] = (int)ScTools.DownloadOpt.invoice;
            }
            else
            {
                options[4] = 0;
            }
        }

        private void TrackAction(ScTools.DownloadOpt opt,  bool isPrinting)
        {
            string eventName = isPrinting ? "print_" : "download_";
            eventName += GetDocName(opt);
            analytics.Track(eventName, new JsonObject
            {
                ["document"] = GetDocName(opt),
            });
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

    }
}
