importScripts('https://www.gstatic.com/firebasejs/8.10.0/firebase-app.js')
importScripts('https://www.gstatic.com/firebasejs/8.10.0/firebase-messaging.js')
// Initialize the Firebase app in the service worker by passing in
// your app's Firebase config object.
firebase.initializeApp({
    apiKey: 'AIzaSyCRcogGq4UBgNaEwbDIgV9p0eABSWvvxHc',
    authDomain: 'ims-notification.firebaseapp.com',
    projectId: 'ims-notification',
    storageBucket: 'ims-notification.appspot.com',
    messagingSenderId: '436171651515',
    appId: '1:436171651515:web:2e1fc267efa652291f6c9e',
    measurementId: 'G-ZTNBKJLF73',
})
// Add listeners for push notifications
self.addEventListener('push', function (event) {
    console.log('Received a push message', event)

    var notificationTitle = 'Thông báo'
    var notificationOptions = {
        body: 'Bạn đã nhận được một tin nhắn mới',
        icon: '/firebase-logo.png',
        badge: '/firebase-logo.png',
    }
    if (event.data) {
        try {
            const data = JSON.parse(event.data.text())
            console.log('data', data)
            notificationTitle = data.notification.title
            notificationOptions.body = data.notification.body
        } catch (error) {
            console.log('Error parsing notification data', error)
        }
    }
    event.waitUntil(Promise.all([self.registration.showNotification(notificationTitle, notificationOptions)]))
})

self.addEventListener('notificationclick', function (event) {
    event.notification.close()
    event.waitUntil(clients.openWindow('http://10.32.4.170/leaveOffs/Registerlists'))
})
