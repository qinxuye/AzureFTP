using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace FTPServerRole.AzureMngmntRESTLib
{
    class AzureMngmntHandler
    {
        private static string getValueFromAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key] as string;
        }

        static string SubscriptionId
        {
            get
            {
                return getValueFromAppSettings("SubscriptionId");
            }
        }

        static string CertificateThumbprintName
        {
            get
            {
                return getValueFromAppSettings("CertificateThumbprintName");
            }
        }

        static string ServiceName
        {
            get
            {
                return getValueFromAppSettings("ServiceName");
            }
        }

        static public X509Certificate LookupCertificate(string ThumbprintSettingName)
        {
            //GetConfigurationValue() doesn't seem to retrieve the value from Certificates
            //So I added it to the Settings tab
            string Thumbprint = RoleEnvironment.GetConfigurationSettingValue(ThumbprintSettingName);

            X509Store certificateStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            certificateStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certs = certificateStore.Certificates.Find(X509FindType.FindByThumbprint, Thumbprint, false);

            return certs[0];
        }

        static public string GetDeploymentInfo()
        {
            string x_ms_version = "2009-10-01";

            string DeploymentSlot = "Production";

            string RequestUri = "https://management.core.windows.net/" + SubscriptionId + "/services/hostedservices/"
                                + ServiceName + "/deploymentslots/" + DeploymentSlot;
            HttpWebRequest RestRequest = (HttpWebRequest)HttpWebRequest.Create(RequestUri);

            NameValueCollection RequestHeaders = new NameValueCollection();
            RequestHeaders.Add("x-ms-version", x_ms_version);

            X509Certificate cert = LookupCertificate(CertificateThumbprintName);
            RestRequest.ClientCertificates.Add(cert);

            RestRequest.Method = "GET";
            WebResponse RestResponse = default(WebResponse);
            RestRequest.ContentType = "text/xml";

            if (RequestHeaders != null)
            {
                RestRequest.Headers.Add(RequestHeaders);
            }

            RestResponse = RestRequest.GetResponse();

            string ResponseBody = string.Empty;

            if (RestResponse != null)
            {
                using (StreamReader RestResponseStream = new StreamReader(RestResponse.GetResponseStream(), true))
                {
                    // Deployment DeploymentConfiguration = (Deployment)xmls.Deserialize(RestResponseStream);
                    ResponseBody = RestResponseStream.ReadToEnd();
                    RestResponseStream.Close();
                }
            }


            return ResponseBody;
        }

        static public string GetServiceConfig(string DeploymentInfoXML)
        {
            //get the service configuration out of the deployment configuration
            XElement DeploymentInfo = XElement.Parse(DeploymentInfoXML);
            string EncodedServiceConfig = (from element in DeploymentInfo.Elements()
                                           where element.Name.LocalName.Trim().ToLower() == "configuration"
                                           select (string)element.Value).Single();

            string CurrentServiceConfigText = System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(EncodedServiceConfig));

            return CurrentServiceConfigText;
        }

        static public string GetInstanceCount(string ServiceConfigXML, string RoleName)
        {
            //make the service config queryable
            XElement XServiceConfig = XElement.Parse(ServiceConfigXML);

            XElement WebRoleElement = (from element in XServiceConfig.Elements()
                                       where element.Attribute("name").Value == RoleName
                                       select element).Single();

            string CurrentInstanceCount = (from childelement in WebRoleElement.Elements()
                                           where childelement.Name.LocalName.Trim().ToLower() == "instances"
                                           select (string)childelement.Attribute("count").Value).FirstOrDefault();
            return CurrentInstanceCount;

        }

        public static string ChangeInstanceCount(string ServiceConfigXML, string RoleName, string Newcount)
        {
            string returnConfig = default(string);
            XElement XServiceConfig = XElement.Parse(ServiceConfigXML);

            XElement WebRoleElement = (from element in XServiceConfig.Elements()
                                       where element.Attribute("name").Value == RoleName
                                       select element).Single();

            XElement InstancesElement = (from childelement in WebRoleElement.Elements()
                                         where childelement.Name.LocalName.Trim().ToLower() == "instances"
                                         select childelement).Single();

            InstancesElement.SetAttributeValue("count", Newcount.ToString());

            StringBuilder xml = new StringBuilder();
            XServiceConfig.Save(new StringWriter(xml));
            returnConfig = xml.ToString();

            return returnConfig;
        }

        public static void GetServiceInfo()
        {
            string x_ms_version = "2009-10-01";


            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(@"https://management.core.windows.net/b32ae60e-39d6-4b26-b14e-67b4b95ab9da/services/hostedservices");


            NameValueCollection RequestHeaders = new NameValueCollection();
            RequestHeaders.Add("x-ms-version", x_ms_version);


            //X509Certificate cert = X509Certificate.CreateFromCertFile(CertificatePath);
            X509Certificate cert = LookupCertificate(CertificateThumbprintName);

            request.Method = "GET";
            WebResponse resp = default(WebResponse);
            request.ContentType = "text/xml";
            request.ClientCertificates.Add(cert);
            if (RequestHeaders != null)
            {
                request.Headers.Add(RequestHeaders);
            }

            resp = request.GetResponse();

            string ReturnBody = string.Empty;

            if (resp != null)
            {

                using (StreamReader sr = new StreamReader(resp.GetResponseStream(), true))
                {
                    ReturnBody = sr.ReadToEnd();
                    sr.Close();
                }
            }
        }

        static public bool ChangeConfigFile(string ConfigXML)
        {

            return ChangeConfigFile(AzureMngmntHandler.SubscriptionId,
                            AzureMngmntHandler.CertificateThumbprintName,
                            AzureMngmntHandler.ServiceName, "Production",
                            ConfigXML);
        }
        //following this structure
        //   https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deployments/<deployment-name>/?comp=config

        static public bool ChangeConfigFile(String SubscriptionID, String CertThumbprint, String SvcName, String DeploymentSlots, String ConfigXML)
        {

            bool result = true;

            try
            {
                string changeConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                <ChangeConfiguration xmlns=""http://schemas.microsoft.com/windowsazure"">
                                    <Configuration>{0}</Configuration>
                                </ChangeConfiguration>";

                string requestUrl = "https://management.core.windows.net/" + SubscriptionID +
                                    "/services/hostedservices/" + SvcName + "/deploymentslots/" +
                                    DeploymentSlots + "/?comp=config";

                string configData = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigXML));
                String requestBody = string.Format(changeConfig, configData);

                string x_ms_version = "2009-10-01";
                string ReturnBody = string.Empty;


                WebResponse resp = null;
                NameValueCollection RequestHeaders = new NameValueCollection();
                RequestHeaders.Add("x-ms-version", x_ms_version);

                //X509Certificate2 cert = new X509Certificate2();
                //cert.Import(CertificatePath);
                X509Certificate cert = LookupCertificate(CertThumbprint);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
                request.Method = "POST";
                request.ClientCertificates.Add(cert);
                request.ContentType = "application/xml";
                request.ContentLength = Encoding.UTF8.GetBytes(requestBody).Length;
                if (RequestHeaders != null)
                {
                    request.Headers.Add(RequestHeaders);
                }

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(requestBody);
                    sw.Close();
                }
                resp = request.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

    }
}
