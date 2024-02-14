// scripts/service/service-worker.js
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open('my-cache-v1').then(cache => {
            return cache.addAll([
                '/',
                '/index.html',
                '/scripts/site-scripts/script.js',
                '/styles/style.css'
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
