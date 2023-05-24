<template>
    <Dialog
        header="Chi tiết công việc"
        :maximizable="true"
        :closable="false"
        position="center"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '60vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
    >
        <div class="">
            <div class="row">
                <div class="col-md-3 border-end">
                    <div class="border-bottom mb-2">
                        <h6>Người thực hiện</h6>
                        <div class="ms-3 mb-3">
                            <div class="d-flex align-items-center">
                                {{
                                    DataDetailTask.assignee != null
                                        ? DataDetailTask.assignee.name
                                        : 'Chưa giao cho ai...'
                                }}
                            </div>
                        </div>
                    </div>
                    <div class="border-bottom mb-2">
                        <h6>Số giờ thực hiện</h6>
                        <div class="ms-3 mb-3">
                            {{ DataDetailTask.time_estimate != null ? DataDetailTask.time_estimate : 'Chưa thêm...' }}
                        </div>
                    </div>
                    <div class="border-bottom mb-2">
                        <h6>Thời gian đã thực hiện</h6>
                        <div class="ms-3 mb-3">
                            {{
                                DataDetailTask.total_time_spent != null
                                    ? DataDetailTask.total_time_spent
                                    : 'Chưa cập nhật...'
                            }}
                        </div>
                    </div>
                    <div class="border-bottom mb-2">
                        <h6>Ngày tạo công việc</h6>
                        <div class="ms-3 mb-3">
                            {{ formatDateTime(DataDetailTask.dateCreated) }}
                        </div>
                    </div>
                    <div class="border-bottom mb-2">
                        <h6>Ngày hết hạn</h6>
                        <div class="ms-3 mb-3">
                            {{
                                DataDetailTask.duedate != null
                                    ? formatDateTime(DataDetailTask.duedate)
                                    : 'Chưa cập nhật...'
                            }}
                        </div>
                    </div>
                    <div class="mb-2">
                        <h6>Trạng thái</h6>
                        <div class="ms-3 mb-3">
                            <div v-if="DataDetailTask.isOnGitLab" class="d-flex">
                                <div v-for="(lable, index) in DataDetailTask.arrayLabel" :key="index">
                                    <div class="tags-status">
                                        <span
                                            :style="{
                                                background: lable.color,
                                                color: lable.text_color,
                                            }"
                                            >{{ lable.name }}</span
                                        >
                                    </div>
                                </div>
                            </div>
                            <div v-else>
                                <div class="tags-status">
                                    <span
                                        :style="{
                                            background: renderStatus(Number(DataDetailTask.status)).color,
                                        }"
                                        >{{ renderStatus(Number(DataDetailTask.status)).name }}</span
                                    >
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-9">
                    <div class="border-top border-bottom">
                        <div style="padding: 15px">
                            <time class="ms-1 me-1"> {{ DateAgo(DataDetailTask.dateCreated) }} </time>
                            <strong> {{ DataDetailTask.createUser.name }}</strong>
                        </div>
                    </div>
                    <div style="padding: 15px" class="border-bottom">
                        <h2>{{ DataDetailTask.taskName }}</h2>
                    </div>
                    <div style="padding: 15px">
                        <p v-html="DataDetailTask.description"></p>
                    </div>
                </div>
            </div>
        </div>
        <template #footer>
            <Button
                label="Huỷ"
                icon="pi pi-times"
                class="p-button-secondary p-button-icon custom-button-h"
                @click="closeDialog()"
            ></Button>
        </template>
    </Dialog>
</template>
<script>
    import dayjs from 'dayjs'
    import { UserService } from '@/service/user.service'
    import { statusTask } from './DataStatusTask'

    export default {
        props: ['isOpen', 'DetailTask'],
        data() {
            return {
                DataDetailTask: null,
                status: statusTask,
            }
        },
        async beforeUpdate() {
            this.DataDetailTask = this.DetailTask
            if (this.DetailTask != null) {
                if (!this.DetailTask.isOnGitLab) {
                    this.getUserTask()
                    await this.getUserCreated()
                }
            }
        },
        methods: {
            formatDateTime(date) {
                var dateTime = new Date(date)
                return dayjs(dateTime).format('YYYY-MM-DD')
            },
            renderStatus(code) {
                const result = this.status.find((item) => item.code === code)
                return result
            },
            getUserTask() {
                if (this.DetailTask.assignee != null) {
                    UserService.getUserById(this.DetailTask.assignee).then((res) => {
                        this.DataDetailTask.assignee = {
                            id: res.data.id,
                            name: res.data.fullName,
                            username: res.data.userCode,
                        }
                    })
                }
            },
            async getUserCreated() {
                if (this.DetailTask.createUser != null) {
                    UserService.getUserById(this.DetailTask.createUser.id).then((res) => {
                        this.DataDetailTask.createUser = {
                            id: res.data.id,
                            name: res.data.fullName,
                            username: res.data.userCode,
                        }
                    })
                }
            },
            DateAgo(createDate) {
                const now = new Date()
                const created = new Date(createDate)

                const timeDiff = Math.abs(now.getTime() - created.getTime())

                const minute = 60 * 1000
                const hour = minute * 60
                const day = hour * 24
                const month = day * 30
                const year = day * 365

                let diff
                let unit
                if (timeDiff < hour) {
                    diff = Math.round(timeDiff / minute)
                    unit = 'phút'
                } else if (timeDiff < day) {
                    diff = Math.round(timeDiff / hour)
                    unit = 'giờ'
                } else if (timeDiff < month) {
                    diff = Math.round(timeDiff / day)
                    unit = 'ngày'
                } else if (timeDiff < year) {
                    diff = Math.round(timeDiff / month)
                    unit = 'tháng'
                } else {
                    diff = Math.round(timeDiff / year)
                    unit = 'năm'
                }
                const agoText = diff + ' ' + unit + ' trước'
                const createdText = 'Được tạo ' + agoText + ' bởi'
                return createdText
            },
            closeDialog() {
                this.$emit('closeDialogDetailTask')
            },
        },
    }
</script>

<style lang="scss" scoped>
    .tags-status span {
        display: inline-block;
        height: 24px;
        line-height: 24px;
        position: relative;
        margin: 0 5px 0 0;
        padding: 0 10px 0 12px;
        background: #777;
        -webkit-box-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
        box-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
        color: #fff;
        font-size: 12px;
        font-family: 'Lucida Grande', 'Lucida Sans Unicode', Verdana, sans-serif;
        text-decoration: none;
        text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
        font-weight: bold;
        border-radius: 4px;
    }

    .tags-status span:after {
        content: '';
        position: absolute;
        top: 10px;
        left: 3px;
        float: left;
        width: 5px;
        height: 5px;
        -webkit-border-radius: 50%;
        border-radius: 50%;
        background: #fff;
        -webkit-box-shadow: -1px -1px 2px rgba(0, 0, 0, 0.4);
        box-shadow: -1px -1px 2px rgba(0, 0, 0, 0.4);
    }
</style>
