<template>
    <div class="container-fluid" style="margin-top: 2rem">
        <div class="row">
            <div class="col-sm-12 col-md-3 col-lg-3 mb-3">
                <date-picker
                    v-model:value="sortMonth"
                    type="year"
                    class="date_time_pick"
                    placeholder="Chọn năm"
                ></date-picker>
            </div>
            <div class="col-sm-12 col-md-9 col-lg-9 d-flex justify-content-end  mb-3">
                <div class="p-1 mb-2 d-flex justify-content-center align-items-center" style="border-radius: 2px">
                    <div class="bg-success text-white" style="width: 20px; height: 20px"></div>
                    <span class="ms-2 me-3"> Đã duyệt</span>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-sm-12 col-md-3 col-lg-3 border-end">
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                        <h6 class="font-weight-bold">Số lần tăng ca</h6>
                        <span class="ms-3">{{ this.detailOTs.totalOTs }} lần tăng ca</span>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12  mb-3">
                        <h6 class="font-weight-bold">Tổng số giờ OTs</h6>
                        <span class="ms-3">{{ this.detailOTs.sumOTs }}</span>
                    </div>
                </div>
            </div>
            <div class="col-sm-12 col-md-9 col-lg-9">
                <div class="card">
                    <Chart type="bar" :data="chartDataOTs" :options="chartOptionsOTs" class="h-30rem" />
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import dayjs from 'dayjs'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import { LeaveOffService } from '@/service/leaveoff.service'
    import { DateHelper } from '@/helper/date.helper'
    import { HTTP } from '@/http-common'
    import { helpers } from '@vuelidate/validators'
    import { looseIndexOf } from '@vue/shared'
    export default {
        data() {
            return {
                sortMonth: new Date(),
                detailLeaveoff: {
                    dayLeaveOff: 0,
                    totalRestDays: '',
                    numberOfLeaveAccept: 0,
                    numberOfLeaveNoAccept: 0,
                },
                detailOTs:{
                    totalOTs : 0,
                    refuse : 0,
                    sumOTs : '',
                },
                dataLeaveOff: [],
                chartData: null,
                chartOptions: null,
                chartDataOTs : {
                    labels: [],
                    datasets: [
                        {
                            label: 'Thống kê tăng ca theo giờ',
                            data: [540, 325, 702, 620],
                            backgroundColor: ['#188a42'],                        
                            borderWidth: 1
                        }
                    ]
                },
                labelsOTs: [],
                datasOTs: [],
                chartOptionsOTs : {
                    maintainAspectRatio: false,
                    aspectRatio: 0,
                    plugins: {
                        legend: {
                            labels: {
                                fontColor: 'black',
                            },
                        },
                    },
                    scales: {
                        x: {
                            ticks: {
                                display: false,
                                color: '#136c34',
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
                                color: '#136c34',
                            },
                            grid: {
                                color: 'black',
                                drawBorder: false,
                            },
                        }
                    }
                }
            }
        },
        watch: {
            sortMonth: {
                handler: function Change(newVal) {
                    this.StatisticalOTs(new Date(newVal).getFullYear())
                },
            },
        },
        created() {
            this.StatisticalOTs(new Date(this.sortMonth).getFullYear())
        },
        methods: {
            StatisticalOTs(year = null){
                    // No filter
                this.dataLeaveOff = []
                this.labelsOTs= []
                this.datasOTs = []
                if(year === null){
                    HTTP.get("OTs/GetAllOTsByStaffAccept/" + checkAccessModule.getUserIdCurrent())
                    .then(res=>{
                        this.detailOTs.totalOTs = res.data._Data.length
   
                        var sumHour = 0
                        var sumMinutes = 0 
                        res.data._Data.sort((a,b)=>{
                            const maxDate = new Date(a.x.dateUpdate)
                            const minDate = new Date(b.x.dateUpdate)

                            if(maxDate > minDate){
                                return -1
                            }else{
                                return 1
                            }
                        })

                        res.data._Data.map(ele=>{
                            const hour = Number(ele.x.realTime.slice(0,2))
                            const minutes = Number(ele.x.realTime.slice(3,5))
                            sumHour+= hour;
                            sumMinutes += minutes
                            const sumMinutesDays = (minutes/60);
                            const sumDays = hour +sumMinutesDays
                            this.labelsOTs.push(this.getFormattedDate(new Date(ele.x.date)))
                            this.datasOTs.push(sumDays.toFixed(1))
                        })
                        
                        this.labelsOTs = this.labelsOTs.filter((item,index)=>{
                            return this.labelsOTs.indexOf(item) === index
                        })

                        this.chartDataOTs.labels = this.labelsOTs
                        this.chartDataOTs.datasets[0].data = this.datasOTs


                        const totalHours = sumHour + sumMinutes / 60; // Tổng số giờ
                        const days = Math.floor(totalHours / 8); // Số ngày
                        const remainingHours = Math.floor(totalHours % 8); // Số giờ còn lại
                        const remainingMinutes = Math.round((totalHours % 1) * 60); // Số phút còn lại
                        this.detailOTs.sumOTs = days + " ngày " + remainingHours +" giờ " + remainingMinutes +" phút"    
                        this.datasOTs.push(this.detailOTs.sumOTs)
                        
                    })
                    .catch(err=>console.log(err))
                }
                // filter
                else{
                    HTTP.get("OTs/GetAllOTsByStaffAccept/" + checkAccessModule.getUserIdCurrent())
                    .then(res=>{
                        this.detailOTs.totalOTs = res.data._Data.length
                        var sumHour = 0
                        var sumMinutes = 0 

                        res.data._Data = res.data._Data.filter(item => {
                            var yearItem = new Date(item.x.dateUpdate)
                            if(yearItem.getFullYear() === year){
                                return item
                            }
                        })



                        res.data._Data.sort((a,b)=>{
                            const maxDate = new Date(a.x.dateUpdate)
                            const minDate = new Date(b.x.dateUpdate)

                            if(maxDate > minDate){
                                return -1
                            }else{
                                return 1
                            }
                        })

                        res.data._Data.map(ele=>{
                            const hour = Number(ele.x.realTime.slice(0,2))
                            const minutes = Number(ele.x.realTime.slice(3,5))
                            sumHour+= hour;
                            sumMinutes += minutes
                            const sumMinutesDays = (minutes/60);
                            const sumDays = hour +sumMinutesDays
                            this.labelsOTs.push(this.getFormattedDate(new Date(ele.x.date)))
                            this.datasOTs.push(sumDays.toFixed(1))
                        })
                        
                        this.labelsOTs = this.labelsOTs.filter((item,index)=>{
                            return this.labelsOTs.indexOf(item) === index
                        })

                        this.chartDataOTs.labels = this.labelsOTs
                        this.chartDataOTs.datasets[0].data = this.datasOTs


                        const totalHours = sumHour + sumMinutes / 60; // Tổng số giờ
                        const days = Math.floor(totalHours / 8); // Số ngày
                        const remainingHours = Math.floor(totalHours % 8); // Số giờ còn lại
                        const remainingMinutes = Math.round((totalHours % 1) * 60); // Số phút còn lại
                        this.detailOTs.sumOTs = days + " ngày " + remainingHours +" giờ " + remainingMinutes +" phút"    
                        this.datasOTs.push(this.detailOTs.sumOTs)
                        
                    })
                    .catch(err=>console.log(err))
                }
            },
            getFormattedDate(date) {
                var year = date.getFullYear()

                var month = (1 + date.getMonth()).toString()
                month = month.length > 1 ? month : '0' + month

                var day = date.getDate().toString()
                day = day.length > 1 ? day : '0' + day

                return day + '-' + month + '-' + year
            },
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('YYYY/MM/DD-HH:mm:ss')
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
</style>
<style>
    .p-chart canvas {
        height: 250px !important;
    }
    .p-chart {
        padding: 13px;
    }
</style>
