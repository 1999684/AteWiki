new Vue({
    el: '#app',
    mixins: [
        loadMask()
    ],
    data: {
        openNav: false,
        iframeKey: 0,
        isLoading: true,
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
        }
    }
});