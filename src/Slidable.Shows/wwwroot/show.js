$(() => {

    const slideImage = document.getElementById('slide-image');

    function loadSlide(url) {
        return fetch(url)
            .then(response => response.ok ? response.text() : null);
    }

    function transition() {
        const url = window.location.href + '/partial';
        loadSlide(url)
            .then(json => {
                if (!json) {
                    goBack();
                    return;
                }
                const partial = JSON.parse(json);
                if (partial && partial.slideImageUrl) {
                    slideImage.src = partial.slideImageUrl;
                }
            });
    }

    function go(href) {
        history.pushState(null, null, href);
        transition();
    }

    function goBack () {
        const parts = location.pathname.split('/');
        const current = parseInt(parts.pop());
        if (current <= 0) return;
        const href = location.href.replace(/\/[0-9]+$/, `/${current - 1}`);
        go(href);
    }

    window.addEventListener('popstate', transition);

    Slidable.Hub.subject('slideAvailable')
        .subscribe(slide => {
            if (slide && slide.number !== undefined && slide.number !== null) {
                go(window.location.pathname.replace(/\/[0-9]+$/, `/${slide.number}`));
            }
        });
});