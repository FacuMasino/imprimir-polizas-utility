using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows;

namespace ImprimirPolizas
{
    internal static class ScTools
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
            paymentProof = 5,
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
            string folderPath
        )
        {
            string fileName = GetFileName(pcNumber, opt);
            try
            {
                using (var client = new WebClient())
                {
                    Debug.Print(Path.Combine(folderPath, fileName));
                    await client.DownloadFileTaskAsync(
                        new Uri(
                            $"{_baseUrl}getBinaryAnnual?pcN={pcNumber}&vhN={vhNumber}&opt={(int)opt}"
                        ),
                        Path.Combine(folderPath, fileName)
                    );
                    Debug.Print(
                        $"{_baseUrl}getBinaryAnnual?pcN={pcNumber}&vhN={vhNumber}&opt={(int)opt}"
                    );
                }
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
                case DownloadOpt.paymentProof:
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

        public static async Task<bool> requiresInvoice(string policyNumber)
        {
            string baseUrl = $"{_baseUrl}requiresInvoice?pcN={policyNumber}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var resData = await content.ReadAsStringAsync();
                            //MessageBox.Show(resData);
                            return ((bool)JObject.Parse(resData)["requiresInvoice"]) || false;
                        }
                    }
                }
            } catch {
                return false;
            }
        }

        public static string GetOptionName(DownloadOpt opt)
        {
            switch (opt)
            {
                case ScTools.DownloadOpt.policy:
                    return "Frente de póliza";
                case ScTools.DownloadOpt.paymentProof:
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

        public static async Task UpdateStats(bool isPolicy, bool isPrint)
        {
            string statType = isPrint ? "newprint" : "newdownload";
            string policyParam = isPolicy ? "true" : "false";
            string baseUrl = $"{_baseUrl}stats/{statType}?isPolicy={policyParam}";
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
