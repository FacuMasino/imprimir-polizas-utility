using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutoUpdaterDotNET;
using Newtonsoft.Json.Linq;

namespace ImprimirPolizas
{
    public static class ScTools
    {
        private static string _baseUrl = "https://sctools-production.up.railway.app/";

        //static private string _baseUrl = "http://127.0.0.1:3100/";

        public static string BaseUrl
        {
            get { return _baseUrl; }
        }

        public enum DownloadOpt
        {
            policy = 1,
            policyCard = 2,
            paymentTC = 3,
            paymentReceipt = 5,
            coupons = 6,
            invoice = 7,
        }

        // Verifica que el servidor este disponible
        public async static Task<bool> IsAvailable()
        {
            try
            {
                // Get Request, esperar que cargue la página
                HttpClient req = new HttpClient();
                var content = await req.GetAsync(_baseUrl);
                if (content.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static async Task DownloadDocAsync(
            string pcNumber,
            int vhNumber,
            DownloadOpt opt,
            string folderPath,
            CancellationToken cancellationToken
        )
        {
            string fileName = GetFileName(pcNumber, opt);
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var client = new WebClient())
                {
                    var tcs = new TaskCompletionSource<bool>();

                    using (cancellationToken.Register(client.CancelAsync))
                    {
                        client.DownloadFileCompleted += (sender, args) =>
                        {
                            if (args.Cancelled)
                            {
                                tcs.TrySetCanceled();
                            }
                            else if (args.Error != null)
                            {
                                tcs.TrySetException(args.Error);
                            }
                            else
                            {
                                tcs.TrySetResult(true);
                            }
                        };

                        client.DownloadFileAsync(
                            new Uri(
                                $"{_baseUrl}getBinaryAnnual?pcN={pcNumber}&vhN={vhNumber}&opt={(int)opt}"
                            ),
                            Path.Combine(folderPath, fileName)
                        );

                        Debug.Print(
                            $"{_baseUrl}getBinaryAnnual?pcN={pcNumber}&vhN={vhNumber}&opt={(int)opt}"
                        );

                        await tcs.Task;
                    }
                }
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.RequestCanceled)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ex)
                when (ex.InnerException is WebException exWeb
                    && exWeb.Status == WebExceptionStatus.RequestCanceled
                )
            {
                throw new OperationCanceledException();
            }
            catch (TaskCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception e)
            {
                string msg = e.Message + "\n" + e.StackTrace;
                if (e.InnerException is WebException)
                {
                    WebException ex = (WebException)e;
                    var webResponse = ex.Response.GetResponseStream();
                    msg =
                        webResponse != null
                            ? new StreamReader(webResponse).ReadToEnd()
                            : "Sin información";
                }
                throw (
                    new Exception($"Error en la descarga.\n" + $"Descripción del error:\n{msg}", e)
                );
            }
        }

        public static async Task DownloadDocAsync(
            string docId,
            string pcNumber,
            DownloadOpt opt,
            string folderPath,
            CancellationToken cancellationToken
        )
        {
            string fileName = GetFileName(pcNumber, opt);
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var client = new WebClient())
                {
                    var tcs = new TaskCompletionSource<bool>();

                    using (cancellationToken.Register(client.CancelAsync))
                    {
                        client.DownloadFileCompleted += (sender, args) =>
                        {
                            if (args.Cancelled)
                            {
                                tcs.TrySetCanceled();
                            }
                            else if (args.Error != null)
                            {
                                tcs.TrySetException(args.Error);
                            }
                            else
                            {
                                tcs.TrySetResult(true);
                            }
                        };

                        client.DownloadFileAsync(
                            new Uri($"{_baseUrl}getBinaryV2?docId={docId}&opt={(int)opt}"),
                            Path.Combine(folderPath, fileName)
                        );

                        Debug.Print($"{_baseUrl}getBinaryV2?docId={docId}&opt={(int)opt}");

                        await tcs.Task;
                    }
                }
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.RequestCanceled)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ex)
                when (ex.InnerException is WebException exWeb
                    && exWeb.Status == WebExceptionStatus.RequestCanceled
                )
            {
                throw new OperationCanceledException();
            }
            catch (TaskCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception e)
            {
                string msg = e.Message + "\n" + e.StackTrace;
                if (e.InnerException is WebException)
                {
                    WebException ex = (WebException)e;
                    var webResponse = ex.Response.GetResponseStream();
                    msg =
                        webResponse != null
                            ? new StreamReader(webResponse).ReadToEnd()
                            : "Sin información";
                }
                throw (
                    new Exception($"Error en la descarga.\n" + $"Descripción del error:\n{msg}", e)
                );
            }
        }

        public static string GetFileName(string pcNumber, DownloadOpt opt)
        {
            string fileName = pcNumber + "_";
            switch (opt)
            {
                case DownloadOpt.policy:
                    fileName += "FrentePoliza.pdf";
                    break;
                case DownloadOpt.paymentReceipt:
                    fileName += "ComprobantePago.pdf";
                    break;
                case DownloadOpt.policyCard:
                    fileName += "TarjetaCirculacion.pdf";
                    break;
                case DownloadOpt.coupons:
                    fileName += "CuponesDePago.pdf";
                    break;
                case DownloadOpt.invoice:
                    fileName += "Factura.pdf";
                    break;
                default:
                    fileName += ".pdf";
                    break;
            }
            return fileName;
        }

        public static async Task<bool> requiresInvoice(
            string policyNumber,
            CancellationToken ctsToken
        )
        {
            string baseUrl = $"{_baseUrl}requiresInvoice?pcN={policyNumber}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(baseUrl, ctsToken))
                    {
                        using (HttpContent content = res.Content)
                        {
                            if (res.StatusCode != HttpStatusCode.OK)
                            {
                                throw new Exception(
                                    "Error en la solicitud al servidor, no se pudo obtener la condición fiscal."
                                );
                            }
                            var resData = await content.ReadAsStringAsync();
                            // MessageBox.Show(resData);
                            return ((bool)JObject.Parse(resData)["requiresInvoice"]) || false;
                        }
                    }
                }
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.RequestCanceled)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ex)
                when (ex.InnerException is WebException exWeb
                    && exWeb.Status == WebExceptionStatus.RequestCanceled
                )
            {
                throw new OperationCanceledException();
            }
            catch (TaskCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetOptionName(DownloadOpt opt)
        {
            switch (opt)
            {
                case ScTools.DownloadOpt.policy:
                    return "Frente de póliza";
                case ScTools.DownloadOpt.paymentReceipt:
                    return "Comprobante de pago";
                case ScTools.DownloadOpt.policyCard:
                    return "Tarjeta Seguro Obligatorio";
                case ScTools.DownloadOpt.coupons:
                    return "Cupones de pago";
                case ScTools.DownloadOpt.invoice:
                    return "Factura";
                default:
                    return "Desconocido";
            }
        }

        public static async Task<PolicyDocs> GetPolicyDocs(
            string policyNumber,
            CancellationToken ctsToken
        )
        {
            PolicyDocs pcDocs = new PolicyDocs();

            string baseUrl = $"{_baseUrl}getDocsId?policyNumber={policyNumber}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(baseUrl, ctsToken))
                    {
                        using (HttpContent content = res.Content)
                        {
                            if (res.StatusCode != HttpStatusCode.OK)
                            {
                                throw new Exception(
                                    "Error al obtener los documentos de la póliza."
                                );
                            }
                            var resData = await content.ReadAsStringAsync();

                            JObject jsonData = JObject.Parse(resData);
                            pcDocs.PolicyId = jsonData["Policy"].ToString();
                            pcDocs.PaymentCupon = jsonData["PaymentCoupon"]?.ToString();
                            pcDocs.SummaryId = jsonData["PolicySummary"].ToString();
                            pcDocs.CardId = jsonData["InsuranceCardMandatory"].ToString();
                            return pcDocs;
                        }
                    }
                }
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.RequestCanceled)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ex)
                when (ex.InnerException is WebException exWeb
                    && exWeb.Status == WebExceptionStatus.RequestCanceled
                )
            {
                throw new OperationCanceledException();
            }
            catch (TaskCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool IsCarPolicy(string policyNumber)
        {
            if (string.IsNullOrWhiteSpace(policyNumber))
            {
                throw new ArgumentException("No se puede ingresar un número de póliza vacío");
            }

            // Dividir el número de póliza en segmentos separados por '-'
            string[] segments = policyNumber.Split('-');

            // Comprobar que hay al menos 4 segmentos (ej: "01-05-21-30115997")
            if (segments.Length < 4)
            {
                throw new FormatException("Invalid policy number format.");
            }

            // Obtener nro de Rama
            string branch = segments[2];

            if (branch.Equals("01"))
            {
                return true;
            }

            return false;
        }

        public static async Task UpdateStats(
            bool isPolicy,
            bool isPrint,
            CancellationToken ctsToken
        )
        {
            string statType = isPrint ? "newprint" : "newdownload";
            string policyParam = isPolicy ? "true" : "false";
            string baseUrl = $"{_baseUrl}stats/{statType}?isPolicy={policyParam}";
            ctsToken.ThrowIfCancellationRequested();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var resData = await content.ReadAsStringAsync();
                            Debug.WriteLine(resData);
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
}
