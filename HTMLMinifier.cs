using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using Sitecore.Diagnostics;
using System.Diagnostics;

namespace NTT.Minifier
{
    public class HTMLMinifier : IHttpModule
    {
        private static string[] _excludeExtensions;
        private static string[] _excludeKeywords;

        public static string[] ExcludeExtensions
        {
            get
            {
                if (_excludeExtensions == null || (_excludeExtensions != null && _excludeExtensions.Length < 1))
                {
                    string extensions = Settings.GetSetting("NTT.Minifier.ExcludeExtensions", ".css,.ashx,.png,.gif,.jpg");
                    _excludeExtensions = extensions.Split(',');
                }
                return _excludeExtensions;
            }
        }

        public static string[] ExcludeKeywords
        {
            get
            {
                if (_excludeKeywords == null || (_excludeKeywords != null && _excludeKeywords.Length < 1))
                {
                    string keywords = Settings.GetSetting("NTT.Minifier.ExcludeKeywords", "/sitecore");
                    _excludeKeywords = keywords.Split(',');
                }
                return _excludeKeywords;
            }
        }

        public void Init(HttpApplication context)
        {
            //Handle Post Release Request to handle page output
            context.PostReleaseRequestState += new EventHandler(context_PostReleaseRequestState);
        }

        protected void context_PostReleaseRequestState(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;

            //if debug is enabled do not minify
            if (app != null && !context.IsDebuggingEnabled)
            {
                //ignore /sitecore urls since it causes issues with sitecore desktop content editor
                //if HTMLContentType setting is true then process the file as long as it doesnt exist in the excluded extensions setting
                if ((Settings.GetBoolSetting("NTT.Minifier.HTMLContentType", false) && context.Response.ContentType == "text/html") && !ExcludeURL(app.Request.RawUrl.ToLower())
                    &&  ExcludeExtensions.Any(ext => ext != GetExtension(app.Request.Url.AbsoluteUri)))
                {
                    app.Response.Filter = new ResponseMemoryStream(app.Response.Filter);
                    Log.Info(String.Format("NTT Minifier URL:{0} - Content Type:{1} < MINIFIED", app.Request.RawUrl.ToLower(), context.Response.ContentType), this);
                }
            }
        }

        public void Dispose()
        {
        }

        private string GetExtension(string url)
        {
            //strip out query params (?d=1&c=ab), string out timestamps (?889990)
            Uri uri = new Uri(url);
            string pagePath = String.Format("{0}{1}{2}{3}", uri.Scheme, Uri.SchemeDelimiter, uri.Authority, uri.AbsolutePath);
            return Path.GetExtension(pagePath);
        }

        private bool ExcludeURL(string url)
        {
            foreach (string keyword in ExcludeKeywords)
            {
                if (Regex.IsMatch(url, @"(?<=^([^""]|""[^""]*"")*)" + keyword))
                    return true;
            }
            return false;
        }
    }
}
