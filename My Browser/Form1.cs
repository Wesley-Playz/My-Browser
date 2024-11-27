using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using CefSharp;
using CefSharp.WinForms;
using System.Runtime.CompilerServices;
using ElectronNET.API.Entities;

namespace CSharpBrowser
{
    public partial class Form1 : Form
    {
        private bool isUrlTextBoxClickedOnce = false;
        private bool isUrlTextBoxSelectedAll = false;

        public ChromiumWebBrowser browser;

        public Form1()
        {
            InitializeComponent();
            WireUpEvents();
            InitializeBrowser();
        }

        private void WireUpEvents()
        {
            btnBack.Click += BtnBack_Click;
            btnForward.Click += BtnForward_Click;
            btnReload.Click += BtnReload_Click;
            btnNewTab.Click += BtnNewTab_Click;
            btnCloseTab.Click += BtnCloseTab_Click;
            urlTextBox.KeyDown += UrlTextBox_KeyDown;
            urlTextBox.Click += UrlTextBox_Click;
            urlTextBox.LostFocus += UrlTextBox_LostFocus; // Reset on focus loss
        }

        private void InitializeBrowser()
        {
            AddNewTab("https://websites.30-seven.cc/internal.html");
        }

        private void AddContextMenuToWebView(WebView2 webView)
        {
            webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                webView.CoreWebView2.ContextMenuRequested += async (sender, args) =>
                {
                    args.Handled = true;

                    var deferral = args.GetDeferral();
                    try
                    {
                        var menu = new ContextMenuStrip();

                        // Open in New Tab
                        var openInNewTab = new ToolStripMenuItem("Open in New Tab");
                        openInNewTab.Click += (o, ev) =>
                        {
                            if (args.ContextMenuTarget.LinkUri != null)
                            {
                                AddNewTab(args.ContextMenuTarget.LinkUri.ToString());
                            }
                        };
                        menu.Items.Add(openInNewTab);

                        // Open Image in New Tab (Contextual)
                        if (args.ContextMenuTarget.Kind == CoreWebView2ContextMenuTargetKind.Image)
                        {
                            var openImageInNewTab = new ToolStripMenuItem("Open Image in New Tab");
                            openImageInNewTab.Click += (o, ev) =>
                            {
                                if (args.ContextMenuTarget.SourceUri != null)
                                {
                                    AddNewTab(args.ContextMenuTarget.SourceUri.ToString());
                                }
                            };
                            menu.Items.Add(openImageInNewTab);
                        }

                        // Add a horizontal line (separator)
                        menu.Items.Add(new ToolStripSeparator());

                        // Add Editing Buttons
                        var undo = new ToolStripMenuItem("Undo");
                        undo.Click += async (o, ev) =>
                        {
                            await webView.CoreWebView2.ExecuteScriptAsync("document.execCommand('undo');");
                        };
                        undo.Enabled = await IsCommandEnabled(webView, "undo");
                        menu.Items.Add(undo);

                        var redo = new ToolStripMenuItem("Redo");
                        redo.Click += async (o, ev) =>
                        {
                            await webView.CoreWebView2.ExecuteScriptAsync("document.execCommand('redo');");
                        };
                        redo.Enabled = await IsCommandEnabled(webView, "redo");
                        menu.Items.Add(redo);

                        var cut = new ToolStripMenuItem("Cut");
                        cut.Click += async (o, ev) =>
                        {
                            await webView.CoreWebView2.ExecuteScriptAsync("document.execCommand('cut');");
                        };
                        cut.Enabled = await IsCommandEnabled(webView, "cut");
                        menu.Items.Add(cut);

                        var copy = new ToolStripMenuItem("Copy");
                        copy.Click += async (o, ev) =>
                        {
                            await webView.CoreWebView2.ExecuteScriptAsync("document.execCommand('copy');");
                        };
                        copy.Enabled = await IsCommandEnabled(webView, "copy");
                        menu.Items.Add(copy);

                        var paste = new ToolStripMenuItem("Paste");
                        paste.Click += async (o, ev) =>
                        {
                            await webView.CoreWebView2.ExecuteScriptAsync("document.execCommand('paste');");
                        };
                        paste.Enabled = await IsCommandEnabled(webView, "paste");
                        menu.Items.Add(paste);

                        var delete = new ToolStripMenuItem("Delete");
                        delete.Click += async (o, ev) =>
                        {
                            await webView.CoreWebView2.ExecuteScriptAsync("document.execCommand('delete');");
                        };
                        delete.Enabled = await IsCommandEnabled(webView, "delete");
                        menu.Items.Add(delete);

                        var selectAll = new ToolStripMenuItem("Select All");
                        selectAll.Click += async (o, ev) =>
                        {
                            await webView.CoreWebView2.ExecuteScriptAsync("document.execCommand('selectAll');");
                        };
                        menu.Items.Add(selectAll);

                        var checkSpelling = new ToolStripMenuItem("Check Spelling");
                        checkSpelling.Click += async (o, ev) =>
                        {
                            await webView.CoreWebView2.ExecuteScriptAsync("document.execCommand('spellcheck');");
                        };
                        checkSpelling.Enabled = await IsCommandEnabled(webView, "spellcheck");
                        menu.Items.Add(checkSpelling);

                        // Add a horizontal line (separator)
                        menu.Items.Add(new ToolStripSeparator());

                        // View Page Source
                        var viewPageSource = new ToolStripMenuItem("View Page Source");
                        viewPageSource.Click += async (o, ev) =>
                        {
                            string pageSource = await webView.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML");
                            pageSource = System.Web.HttpUtility.HtmlDecode(pageSource.Trim('"').Replace("\\n", Environment.NewLine).Replace("\\\"", "\""));

                            var sourceForm = new Form
                            {
                                Text = "Page Source",
                                Width = 800,
                                Height = 600
                            };
                            var sourceTextBox = new TextBox
                            {
                                Multiline = true,
                                Dock = DockStyle.Fill,
                                ScrollBars = ScrollBars.Vertical,
                                Text = pageSource
                            };
                            sourceForm.Controls.Add(sourceTextBox);
                            sourceForm.Show();
                        };
                        menu.Items.Add(viewPageSource);

                        // Inspect Element
                        var inspect = new ToolStripMenuItem("Inspect");
                        inspect.Click += async (o, ev) =>
                        {
                            await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("DevTools.show", "{}");
                        };
                        menu.Items.Add(inspect);

                        // Show the menu
                        menu.Show(Cursor.Position);
                    }
                    finally
                    {
                        deferral.Complete();
                    }
                };
            };
        }

        // Helper function to check if a command is enabled
        private async Task<bool> IsCommandEnabled(WebView2 webView, string command)
        {
            var result = await webView.CoreWebView2.ExecuteScriptAsync($"document.queryCommandEnabled('{command}')");
            return result == "true";
        }


        private void AddNewTab(string url)
        {
            var tabPage = new TabPage("New Tab");
            var webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            webView.NavigationCompleted += (sender, args) =>
            {
                if (tabControl1.SelectedTab == tabPage)
                {
                    urlTextBox.Text = webView.Source.ToString();
                }
                tabPage.Text = webView.CoreWebView2?.DocumentTitle ?? "Loading...";
            };

            tabControl1.SelectedIndexChanged += (s, e) =>
            {
                var currentWebView = GetCurrentWebView();
                if (currentWebView != null)
                {
                    urlTextBox.Text = currentWebView.Source.ToString();
                }
            };

            AddContextMenuToWebView(webView);

            webView.Source = new Uri(url);
            tabPage.Controls.Add(webView);
            tabControl1.TabPages.Add(tabPage);
            tabControl1.SelectedTab = tabPage;

            webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                EnableAdBlocking(webView);
                EnableRequestFiltering(webView);
            };

            webView.GotFocus += WebView_GotFocus;
        }

        private void EnableRequestFiltering(WebView2 webView)
        {
            webView.CoreWebView2InitializationCompleted += (sender, args) =>
            {
                webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);

                webView.CoreWebView2.WebResourceRequested += (s, e) =>
                {
                    var blockedDomains = new[]
                    {
                "doubleclick.net",
                "googleadservices.com",
                "googlesyndication.com",
                "youtube.com",
                "amazon-adsystem.com",
                "adservice.google.com",
                "facebook.com/ads",
                "adnxs.com",
                "bing.com/ads"
            };

                    var uri = e.Request.Uri.ToLower();
                    if (blockedDomains.Any(domain => uri.Contains(domain)))
                    {
                        e.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(
                            null, 403, "Blocked by AdBlock", "Content-Type: text/plain");
                    }
                };
            };
        }


        private async void EnableAdBlocking(WebView2 webView)
        {
            webView.CoreWebView2InitializationCompleted += async (sender, args) =>
            {
                // Enable network features for DevTools Protocol
                await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.enable", "{}");

                // Block URLs known for serving ads
                var blockedUrls = new[]
                {
                    "*.doubleclick.net/*",
                    "*.googleadservices.com/*",
                    "*.googlesyndication.com/*",
                    "*.googlevideo.com/*",
                    "*.youtube.com/api/stats/*",
                    "*.youtube.com/youtubei/v1/log_event*",
                    "*.youtube.com/pagead/*",
                    "*.youtube.com/api/stats/qoe*",
                    "*.play.google.com/log",
                    "*.adservice.google.com/*",
                    "*.adservice.google.ca/*",
                    "*.facebook.com/ads/*",
                    "*.bing.com/ad/*",
                    "*.amazon-adsystem.com/*",
                    "*.adnxs.com/*"
                };

                var blockedUrlsJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { urls = blockedUrls });
                await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Network.setBlockedURLs", blockedUrlsJson);

                // Inject custom ad-blocking JavaScript
                InjectAdBlockingScript(webView);
            };
        }


        private async void InjectAdBlockingScript(WebView2 webView)
        {
            const string script = @"
                // Function to remove ads
                const removeAds = () => {
                const adSelectors = [
                    'ytd-in-feed-ad-layout-renderer',
                    'ytd-promoted-sparkles-web-renderer',
                    'ytd-ad-slot-renderer',
                    'ytd-page-top-ad-layout-renderer',
                    'panel-ad-header-image-lockup-view-model',
                    'ytd-engagement-panel-section-list-renderer',
                    'ad-inline-playback-meta-block',
                    '.ad-container',
                    '.ad-showing',
                    'iframe[src*=\""ads\""]',
                    'iframe[src*=\""adservice\""]',
                    'div[id^=\""ad\""]',
                    '.ad-banner',
                    '.ad-wrapper',
                    '.ytp-ad-module'
                ];

                adSelectors.forEach(selector => {
                    document.querySelectorAll(selector).forEach(el => el.remove());
                });
            };

            // Observe DOM changes and continuously apply ad removal
            const observer = new MutationObserver(removeAds);
            observer.observe(document.body, { childList: true, subtree: true });
            
            // Initial ad removal
            removeAds();
        ";

            await webView.ExecuteScriptAsync(script);
        }


        private WebView2 GetCurrentWebView()
        {
            if (tabControl1.SelectedTab?.Controls[0] is WebView2 webView)
            {
                return webView;
            }
            return null;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            GetCurrentWebView()?.GoBack();
        }

        private void BtnForward_Click(object sender, EventArgs e)
        {
            GetCurrentWebView()?.GoForward();
        }

        private void BtnReload_Click(object sender, EventArgs e)
        {
            GetCurrentWebView()?.Reload();
        }

        private void BtnNewTab_Click(object sender, EventArgs e)
        {
            AddNewTab("https://websites.30-seven.cc/internal.html");
        }

        private void BtnCloseTab_Click(object sender, EventArgs e)
        {
            var currentTab = tabControl1.SelectedTab;
            if (currentTab != null)
            {
                tabControl1.TabPages.Remove(currentTab);
            }
        }

        private void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var webView = GetCurrentWebView();
                if (webView != null)
                {
                    string url = urlTextBox.Text;
                    if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                    {
                        url = "http://" + url;
                    }
                    webView.Source = new Uri(url);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count == 0)
            {
                AddNewTab("https://websites.30-seven.cc/internal.html");
            }
        }

        private void UrlTextBox_Click(object sender, EventArgs e)
        {
            if (!isUrlTextBoxClickedOnce)
            {
                urlTextBox.SelectAll();
                isUrlTextBoxClickedOnce = true;
                isUrlTextBoxSelectedAll = true;
            }
            else if (isUrlTextBoxSelectedAll)
            {
                var currentPos = urlTextBox.SelectionStart;
                urlTextBox.SelectionStart = currentPos;
                urlTextBox.SelectionLength = 0;
                isUrlTextBoxSelectedAll = false;
            }
        }

        private void UrlTextBox_LostFocus(object sender, EventArgs e)
        {
            // Reset flags when the URL bar loses focus
            isUrlTextBoxClickedOnce = false;
            isUrlTextBoxSelectedAll = false;
        }

        private void WebView_GotFocus(object sender, EventArgs e)
        {
            // Reset URL bar selection state when WebView gains focus
            isUrlTextBoxClickedOnce = false;
            isUrlTextBoxSelectedAll = false;
        }
    }
}
