﻿using System;
using Xamarin.Forms.Platform.Android;
using Android.Webkit;
using Xamarin.Forms.Labs.Controls;

[assembly: Xamarin.Forms.ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace Xamarin.Forms.Labs.Controls
{
    public partial class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
        private Android.Webkit.WebView webView;

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged (e);

            this.webView = new Android.Webkit.WebView(this.Context);

            this.webView.Settings.JavaScriptEnabled = true;
//            this.InjectNativeFunctionScript ();

            this.webView.SetWebViewClient (new Client (this));
            this.webView.SetWebChromeClient(new ChromeClient());

            this.SetNativeControl (this.webView);

            Initialize();
        }
            
        partial void Inject(string script)
        {
            this.webView.LoadUrl(string.Format("javascript: {0}", script));
        }

        partial void Load(Uri uri)
        {
            if (uri != null)
            {
                this.webView.LoadUrl(uri.AbsoluteUri);
                this.InjectNativeFunctionScript ();
            }
        }

        private class Client : WebViewClient
        {
            private readonly WeakReference<HybridWebViewRenderer> webHybrid;

            public Client(HybridWebViewRenderer webHybrid)
            {
                this.webHybrid = new WeakReference<HybridWebViewRenderer>(webHybrid);
            }

            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
            {
                HybridWebViewRenderer hybrid;
                if (!this.webHybrid.TryGetTarget(out hybrid) || !hybrid.CheckRequest(url))
                {
                    view.LoadUrl(url);
                    hybrid.InjectNativeFunctionScript ();
                }

                return true;
            }
        }

        private class ChromeClient : WebChromeClient 
        {
            public override bool OnJsAlert(Android.Webkit.WebView view, string url, string message, JsResult result)
            {
                // the built-in alert is pretty ugly, you could do something different here if you wanted to
                return base.OnJsAlert(view, url, message, result);
            }
        }
    }
}

