<template>
    <Dialog
        :header="selectedallUser ? 'Thông tin nghỉ phép tổng hợp' : 'Thông tin nghỉ phép cá nhân'"
        :maximizable="true"
        :closable="false"
        position="center"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '60vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
    >
        <div class="container mt-3">
            <div class="detail__content">
            <div class="row">
                <div class="col-sm-12 col-md-7 d-flex justify-content-start">
                    <div class="row">
                        <div class="col-sm-12 col-md-6 col-lg-6 mb-3">
                            <date-picker
                                v-model:value="filterLeaveOff.sortMonth"
                                type="year"
                                class="date_time_pick"
                                placeholder="Chọn năm"
                            ></date-picker>
                        </div>
                        <div class="col-sm-12 col-md-6 col-lg-6  mb-3" v-if="selectedallUser">
                            <Dropdown
                                class="custom-input-h"
                                id="selectedIdUser"
                                :filter="true"
                                v-model="filterLeaveOff.selectedIdUser"
                                :options="optionsAllUser"
                                optionLabel="fullName"
                                optionValue="id"
                                placeholder="Chọn nhân viên.."
                                style="width: 100%;"
                            />
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-5 d-flex justify-content-end">
                    <div
                        class="p-1 mb-2 d-flex justify-content-center align-items-center"
                        style="border-radius: 2px"
                    >
                        <div class="bg-success text-white" style="width: 30px; height: 30px"></div>
                        <span class="ms-2 me-3"> Có phép</span>
                    </div>
                    <div
                        class="p-1 mb-2 d-flex justify-content-center align-items-center"
                        style="border-radius: 2px"
                    >
                        <div class="bg-danger text-white" style="width: 30px; height: 30px"></div>
                        <span class="ms-2 me-3"> Không phép </span>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12 col-md-3 col-lg-3 border-end">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <h6 class="font-weight-bold">Số lần nghỉ phép có phép</h6>
                            <span class="ms-3">{{ detailLeaveoff.numberOfLeaveAccept }} lần nghỉ</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <h6 class="font-weight-bold">Số lần nghỉ phép không phép</h6>
                            <span class="ms-3">{{ detailLeaveoff.numberOfLeaveNoAccept }} lần nghỉ</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <h6 class="font-weight-bold">Số ngày nghỉ phép mặc định</h6>
                            <span class="ms-3">{{ detailLeaveoff.dayLeaveOff }} ngày</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <h6 class="font-weight-bold">Số ngày nghỉ phép còn lại</h6>
                            <span class="ms-3">{{ detailLeaveoff.totalRestDays }}</span>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-9 col-lg-9">
                    <div class="card">
                        <Chart type="bar" :data="chartData" :options="chartOptions" class="h-50rem" />
                    </div>
                </div>
            </div>
        </div>
        </div>
        <template #footer>
            <Button
                id="button-close"
                label="Huỷ"
                icon="pi pi-times"
                class="p-button-secondary p-button-outlined CustomButtonPrimeVue"
                @click="closeDialog()"
                autofocus
            ></Button>
        </template>
    </Dialog>
</template>

<script>
    import dayjs from 'dayjs'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import { LeaveOffService } from '@/service/leaveoff.service'
    import { DateHelper } from '@/helper/date.helper'
    import { UserService } from '@/service/user.service'

    export default {
        props: ['isOpen', 'selectedallUser'],
        data() {
            return {
                filterLeaveOff: {
                    sortMonth: new Date(),
                    selectedIdUser: null,
                },
                detailLeaveoff: {
                    dayLeaveOff: 0,
                    totalRestDays: '',
                    numberOfLeaveAccept: 0,
                    numberOfLeaveNoAccept: 0,
                },
                dataLeaveOff: [],
                chartData: null,
                chartOptions: null,
                optionsAllUser: [],
            }
        },
        watch: {
            filterLeaveOff: {
                handler: async function Change(newVal) {
                    await this.getUserInfoLeaveOffById(newVal.sortMonth, newVal.selectedIdUser)
                },
                deep: true,
            },
        },
        beforeUpdate() {
            this.getAllUser()
            if(!this.selectedallUser) this.getUserInfoLeaveOffById(this.filterLeaveOff.sortMonth)
        },
        methods: {
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('YYYY/MM/DD-HH:mm:ss')
            },
            getAllUser() {
                UserService.getAllUser()
                    .then((res) => {
                        this.optionsAllUser = res.data._Data
                    })
                    .catch((err) => {
                        console.log(err)
                    })
            },
            getUserInfoLeaveOffById(sortMonth, selectedIdUser) {
                this.dataLeaveOff = []
                let idUser = 0
                if (!this.selectedallUser) {
                    idUser = checkAccessModule.getUserIdCurrent()
                } else {
                    idUser = selectedIdUser
                }
                LeaveOffService.getInfoUserLeaveOff(idUser, DateHelper.convertUTC(sortMonth)).then((res) => {
                    let totalRestMinute = res.data._Data.dayLeaveOff
                    this.detailLeaveoff.dayLeaveOff = res.data._Data.dayLeaveOff / 480
                    this.detailLeaveoff.numberOfLeaveAccept = res.data._Data.numberOfLeaveAccept
                    this.detailLeaveoff.numberOfLeaveNoAccept = res.data._Data.numberOfLeaveNoAccept

                    const leaveOffNavigations = res.data._Data.leaveOffNavigations.map((el) => ({
                        ...el,
                        hasLeave: el.status == 2,
                        realTime: this.mathLeaveOffDate(el.startTime, el.endTime, el.idCompanyBranh, el.flagOnDay),
                    }))

                    const result = leaveOffNavigations.reduce((acc, el) => {
                        if (el.realTime.days > 0) {
                            acc -= el.realTime.days * 480
                        }
                        if (el.realTime.hours > 0) {
                            acc -= el.realTime.hours * 60
                        }
                        if (el.realTime.minutes > 0) {
                            acc -= el.realTime.minutes
                        }
                        return acc
                    }, totalRestMinute)

                    const numberHours = Math.floor(result / 60)
                    const numberRestMinute = Math.floor(result % 60)
                    const numberDays = Math.floor(numberHours / 8)
                    const numberRestHours = Math.floor(numberHours % 8)
                    this.detailLeaveoff.totalRestDays = `${numberDays} ngày ${numberRestHours} giờ ${numberRestMinute} phút`

                    const allLeaveOffNavigations = res.data._Data.allLeaveOffNavigations.map((el) => ({
                        ...el,
                        hasLeave: el.status == 2 ? true : false,
                        realTime: this.mathLeaveOffDate(el.startTime, el.endTime, el.idCompanyBranh, el.flagOnDay),
                    }))

                    var data = allLeaveOffNavigations.map((el) => `${DateHelper.formatDateDayjs(el.startTime)}`)

                    var dataDateValue = allLeaveOffNavigations.map((el, acc) => {
                        var hasLeave = el.status == 2 ? true : false
                        if (el.realTime.days > 0) {
                            acc += el.realTime.days * 8 * 60
                        }
                        if (el.realTime.hours > 0) {
                            acc += el.realTime.hours * 60
                        }
                        if (el.realTime.minutes > 0) {
                            acc += el.realTime.minutes
                        }
                        var map = acc
                        var time = el.realTime.days * 480 + el.realTime.hours * 60 + el.realTime.minutes
                        return { map, hasLeave, time }
                    })
                    this.chartOptions = this.setChartOptions()
                    this.chartData = this.setChartData(data, dataDateValue)
                })
            },
            mathLeaveOffDate(startTime, endTime, idCompanyBranh) {
                const start = new Date(startTime)
                const end = new Date(endTime)

                // Kiểm tra xem thời gian bắt đầu và kết thúc có cùng ngày không
                const isSameDay =
                    start.getFullYear() === end.getFullYear() &&
                    start.getMonth() === end.getMonth() &&
                    start.getDate() === end.getDate()

                let diff = end - start

                if (start.getHours() <= 12 && end.getHours() >= 13) {
                    let lunchTime
                    if (idCompanyBranh == 1) {
                        lunchTime = 90
                    }
                    if (idCompanyBranh == 2 || idCompanyBranh == 3 || idCompanyBranh == 4) {
                        lunchTime = 60
                    }
                    diff -= lunchTime * 60 * 1000
                }

                if (
                    start.getHours() == 8 &&
                    start.getMinutes() == 0 &&
                    end.getHours() == 17 &&
                    end.getMinutes() <= 30
                ) {
                    let lunchTime
                    if (idCompanyBranh == 1) {
                        lunchTime = 0
                    }
                    if (idCompanyBranh == 2 || idCompanyBranh == 3 || idCompanyBranh == 4) {
                        lunchTime = 0
                    }
                    diff -= lunchTime * 60 * 1000
                } else if (
                    start.getHours() <= 11 &&
                    start.getMinutes() <= 45 &&
                    end.getHours() >= 12 &&
                    end.getMinutes() <= 45
                ) {
                    let lunchTime
                    if (idCompanyBranh == 1) {
                        lunchTime = 0
                    }
                    if (idCompanyBranh == 2 || idCompanyBranh == 3 || idCompanyBranh == 4) {
                        lunchTime = 15
                    }
                    diff -= lunchTime * 60 * 1000
                } else if (
                    start.getHours() <= 12 &&
                    start.getMinutes() <= 45 &&
                    end.getHours() >= 13 &&
                    end.getMinutes() <= 45
                ) {
                    let lunchTime
                    if (idCompanyBranh == 1) {
                        lunchTime = 0
                    }
                    if (idCompanyBranh == 2 || idCompanyBranh == 3 || idCompanyBranh == 4) {
                        lunchTime = 15
                    }
                    diff += lunchTime * 60 * 1000
                }
                // }
                if (
                    start.getHours() == 11 &&
                    start.getMinutes() <= 45 &&
                    end.getHours() == 12 &&
                    end.getMinutes() >= 45 &&
                    isSameDay
                ) {
                    let lunchTime
                    if (idCompanyBranh == 2 || idCompanyBranh == 3 || idCompanyBranh == 4) {
                        lunchTime = 60
                    }
                    diff -= lunchTime * 60 * 1000
                }

                const millisecondsPerDay = 24 * 60 * 60 * 1000
                let count = 0
                let currentDate = new Date(start)
                let countT7CN = 0
                while (currentDate <= end) {
                    if (currentDate.getDay() !== 0 && currentDate.getDay() !== 6) {
                        // Bỏ qua ngày thứ 7 và chủ nhật
                        let workHours = 8.5 // Số giờ làm việc trong ngày
                        if (currentDate.getHours() >= 12 && currentDate.getHours() < 13) {
                            workHours -= 1 // Nếu là giờ nghỉ trưa, trừ đi 1 giờ
                        } else if (currentDate.getHours() >= 13 && currentDate.getHours() < 17) {
                            workHours -= 0.5 // Nếu là giờ chiều, trừ đi 0.5 giờ
                        }
                        count += workHours // Cộng số giờ làm việc của ngày đó vào tổng số giờ làm việc
                    } else {
                        countT7CN += 1
                    }
                    currentDate.setDate(currentDate.getDate() + 1)
                }

                const totalWorkingHours = count

                let days = Math.floor(diff / millisecondsPerDay)
                diff -= days * millisecondsPerDay
                days -= countT7CN

                const hours = Math.floor(diff / (1000 * 60 * 60))
                diff -= hours * (1000 * 60 * 60)

                const minutes = Math.floor(diff / (1000 * 60))

                return {
                    days: days,
                    hours: hours,
                    minutes: minutes,
                }
            },
            setChartData(data, dataDateValue) {
                const documentStyle = getComputedStyle(document.documentElement)
                const colors = dataDateValue.map((el) => {
                    return el.hasLeave ? '#198754' : '#dc3545'
                })
                return {
                    labels: data,
                    datasets: [
                        {
                            label: 'Thống kê nghỉ phép theo giờ',
                            backgroundColor: colors,
                            borderColor: documentStyle.getPropertyValue('--blue-500'),
                            data: dataDateValue.map((el) => {
                                var percentage = parseFloat(el.time / 60)
                                var formattedMinutes = percentage.toFixed(2)
                                return formattedMinutes
                            }),
                        },
                    ],
                }
            },
            setChartOptions() {
                const documentStyle = getComputedStyle(document.documentElement)
                const textColor = documentStyle.getPropertyValue('--text-color')
                const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary')
                const surfaceBorder = documentStyle.getPropertyValue('--surface-border')

                return {
                    maintainAspectRatio: false,
                    aspectRatio: 0,
                    plugins: {
                        legend: {
                            labels: {
                                fontColor: textColor,
                            },
                        },
                    },
                    scales: {
                        x: {
                            ticks: {
                                display: false,
                                color: textColorSecondary,
                                font: {
                                    weight: 500,
                                },
                                beginAtZero: true,
                                callback: function (value) {
                                    return value.toFixed(2)
                                },
                            },
                            grid: {
                                display: false,
                                drawBorder: false,
                            },
                        },
                        y: {
                            ticks: {
                                color: textColorSecondary,
                            },
                            grid: {
                                color: surfaceBorder,
                                drawBorder: false,
                            },
                        },
                    },
                }
            },
            closeDialog() {
                this.$emit('closeDialog')
            },
        },
    }
</script>
<style lang="scss" scoped>
    ::v-deep(.p-scrollpanel) {
        p {
            padding: 0.5rem;
            line-height: 1.5;
            margin: 0;
        }
        &.custombar1 {
            .p-scrollpanel-wrapper {
                border-right: 9px solid var(--surface-ground);
            }

            .p-scrollpanel-bar {
                background-color: #9fc8ee;
                opacity: 1;
                transition: background-color 0.2s;

                &:hover {
                    background-color: #007ad9;
                }
            }
        }
    }
    @media (max-width: 573px) {
        #button-close {
            font-size: 12px;
        }
    }
    .p-chart canvas {
        height: 250px !important;
    }
    .p-chart {
        padding: 13px;
    }
    .detail__content {
        border-radius: 15px;
        box-shadow: rgba(0, 0, 0, 0.02) 0px 1px 3px 0px, rgba(27, 31, 35, 0.15) 0px 0px 0px 1px;
        padding: 30px;
    }
</style>

