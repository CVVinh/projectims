import axios from 'axios'

export const firebaseConfig = {
    apiKey: 'AIzaSyCRcogGq4UBgNaEwbDIgV9p0eABSWvvxHc',
    authDomain: 'ims-notification.firebaseapp.com',
    projectId: 'ims-notification',
    storageBucket: 'ims-notification.appspot.com',
    messagingSenderId: '436171651515',
    appId: '1:436171651515:web:2e1fc267efa652291f6c9e',
    measurementId: 'G-ZTNBKJLF73',
}
export const FIREBASE_SERVER_KEY =
    'key=AAAAZY3bObs:APA91bFBntjq6kWTpzl5_QaP3PwNBs6ePzusTHA-wwHe-zIAkpMRTLwC5d1lH8DMwLkH-SNl2aORIRmkSlbpe322YvwCAV3dNeGN4vWxEaePt8KG4V-WxyzyWJRzikwKSSMky1QVNRI2'

export const VALID_KEY = 'BMNiApoEJBGjbZ4Ve-Iwa_tgXkM0SWmwJApj61BFom08syy3CrqwcAsi8SN8pPnto1EsCUxjAIacmtGiGfUqmUk'

export const HTTP_FIREBASE = axios.create({
    baseURL: 'https://fcm.googleapis.com/',
    headers: {
        'Content-Type': 'application/json',
        Authorization: FIREBASE_SERVER_KEY,
    },
})
