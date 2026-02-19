new Vue({
    el: '#app',
    data: {
        isNavOpen: false,
        isLoading: true,
        iframeKey: 0,
        navigator: [
            { name: "主页", action: "Home" },
            { name: "素材", action: "Margin" },
        ],
        iframeSrc: './pages/home.html'
    },
    methods: {
        handleNavClick: function (item) {
            let targetSrc = '';

            switch (item.action) {
                case "Home":
                    targetSrc = './pages/home.html';
                    break;
                case "Margin":
                    targetSrc = './pages/margin.html'; 
                    break;
                default:
                    targetSrc = './pages/home.html';
                    break;
            }

            this.isLoading = true;
            this.iframeKey++;
            this.iframeSrc = targetSrc;
        },
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
});