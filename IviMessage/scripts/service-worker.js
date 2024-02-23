// scripts/service/service-worker.js
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open('my-cache-v1').then(cache => {
            return cache.addAll([
                '/',
                '/index.html',
                '/scripts/app.js',
                '/scripts/components.js',
                '/scripts/datamodels.js',
                '/styles/style.css',
                '/styles/themes.css',
                // Add other static assets here
            ]);
        })
    );
});

self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request).then(response => {
            return response || fetch(event.request);
        })
    );
});
