new Vue({
    el: '#app',
    methods: {
        goHome: function () {
            if (window.chrome && window.chrome.webview) {
                window.chrome.webview.postMessage("NavigateToHome");
            } else {
                alert("请在 WPF 程序中运行此页面 (暗号已尝试发送)");
            }
        }
    }
});