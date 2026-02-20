(function (window) {
    function loadMask() {
        return {
            data: function () {
                return {
                };
            },
            mounted: function () {
            },
            methods: {
                onIframeLoad: function () {
                    setTimeout(() => {
                        this.isLoading = false;
                    }, 500);
                },
                goHome: function () {
                    if (window.chrome && window.chrome.webview) {
                        window.chrome.webview.postMessage("NavigateToHome");
                    } else {
                        alert("请在 WPF 程序中运行此页面 (暗号已尝试发送)");
                    }
                }
            }
        };
    }
    window.loadMask = loadMask;
})(window);