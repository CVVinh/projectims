<template noInheritAttrs>
    <component :is="layout">
        <router-view v-model:layout="layout" />
    </component>
</template>

<script setup>
import { shallowRef, onMounted } from 'vue'
import { initializeApp } from 'firebase/app'
import { getMessaging, getToken, onMessage } from 'firebase/messaging'
import { firebaseConfig } from '@/helper/firebase.helper'

let layout = shallowRef('div')

onMounted(async () => {
    const app = initializeApp(firebaseConfig)
    const messaging = getMessaging(app)

    // Đăng ký hàm onMessage trước khi gửi thông báo
    onMessage(messaging, (payload) => {
        console.log('Message received. ', payload)
    })

    const permission = await Notification.requestPermission()
    if (permission === 'granted') {
        console.log('Permission Access')
    } else {
        console.error('Permission denied')
    }
})
</script>

<style lang="scss">
@import '@/styles/index.scss';
</style>
