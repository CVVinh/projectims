<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">DANH SÁCH NGHỈ PHÉP</li>
                        </ol>
                    </nav>
                </div>
            </div>

            <nav class="navbar navbar-expand-lg bg-light navbar-light">
                <div class="container-fluid">
                    <button
                        class="navbar-toggler mb-2 mt-2 custom-input-h"
                        type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#collapsibleNavbar"
                    >
                        <span class="navbar-toggler-icon"></span>
                    </button>
                    <div class="collapse navbar-collapse mt-2 row" id="collapsibleNavbar">
                        <ul class="navbar-nav me-auto col-sm-12 col-md-12 col-lg-5">
                            <li class="nav-item me-2 mb-2 ms-2">
                                <Export
                                    class="custom-button-h"
                                    label="Xuất Excel"
                                    @click="exportCSV($event)"
                                    :disabled="canExport"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    label="Thông tin nghỉ phép tổng hợp"
                                    icon="pi pi-info-circle"
                                    @click="handlerUserInfoLeaveOff()"
                                    class="p-button-sm custom-button-h"
                                />
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto col-sm-12 col-md-12 col-lg-7 d-flex justify-content-end">
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite"
                                    icon="pi pi-filter-slash"
                                    class="custom-reload-h"
                                    @click="handlerReload()"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    class="p-inputtext custom-input-h"
                                    v-model="fillterLeaveOff.searchLeaveOff"
                                    placeholder="Nhập tên nhân viên..."
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <MultiSelect
                                    v-model="fillterLeaveOff.idstatus"
                                    :options="statusLeave"
                                    optionLabel="title"
                                    optionValue="id"
                                    placeholder="Chọn trạng thái"
                                    class="custom-input-h"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <date-picker
                                    v-model:value="fillterLeaveOff.sortMonthOfYear"
                                    type="month"
                                    placeholder="Chọn tháng"
                                ></date-picker>
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <date-picker
                                    v-model:value="fillterLeaveOff.sortMonth"
                                    type="year"
                                    placeholder="Chọn năm"
                                ></date-picker>
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>
            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        :value="arr"
                        ref="dt"
                        class="p-datatable-customers"
                        :rows="5"
                        dataKey="id"
                        :rowHover="true"
                        sortField="dateCreated"
                        :sortOrder="-1"
                        :loading="loading"
                        responsiveLayout="scroll"
                        showGridlines
                        :globalFilterFields="['name', 'startDate', 'endDate', 'isDeleted', 'isFinished']"
                    >
                        <template #empty> Không tìm thấy </template>
                        <template #loading>
                            <ProgressSpinner />
                        </template>
                        <Column field="#" header="#" dataType="date">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column sortable field="name" header="Tên nhân viên">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column sortable field="acceptBy" header="Người phê duyệt">
                            <template #body="{ data }">
                                {{ data.acceptBy }}
                            </template>
                        </Column>
                        <Column field="startTime" header="Ngày bắt đầu">
                            <template #body="{ data }">
                                {{ data.startTime }}
                            </template>
                        </Column>
                        <Column field="" header="Ngày kết thúc">
                            <template #body="{ data }">
                                {{ data.endTime }}
                            </template>
                        </Column>
                        <Column field="" header="Thời gian thực tế">
                            <template #body="{ data }">
                                {{ data.realTime }}
                            </template>
                        </Column>
                        <Column field="" header="Lý do">
                            <template #body="{ data }">
                                {{ data.reasons }}
                            </template>
                        </Column>
                        <Column field="status" header="Trạng thái">
                            <template #body="{ data }">
                                <span :class="checkStatus(data.status).class">
                                    {{ checkStatus(data.status).title }}
                                </span>
                            </template>
                        </Column>
                    </DataTable>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <Pagination
                        v-model:pageNumber="resultPgae.pageNumber"
                        v-model:pageSize="resultPgae.pageSize"
                        :totalPages="totalMapPage"
                        :totalItems="this.totalItem"
                        :itemIndex="this.itemIndex"
                    />
                </div>
            </div>
        </div>
        <DialogUserInfoLeaveOff
            :isOpen="this.isOpenDialogUserInfoLeaveOff"
            @closeDialog="closeDialogUserInfoLeaveOff()"
            :selectedallUser="true"
        />
    </LayoutDefaultDynamic>
</template>

<script>
import { HTTP, GET_USER_NAME_BY_ID, GET_BY_YEAR, GET_LEAVEOFF_BY_STATUS, HTTP_LOCAL } from '@/http-common'
import View from '../../components/buttons/View.vue'
import Export from '../../components/buttons/Export.vue'
import { DateHelper } from '@/helper/date.helper'
import { LocalStorage } from '@/helper/local-storage.helper'
import router from '../../router/index'
import { UserRoleHelper } from '@/helper/user-role.helper'
import dayjs from 'dayjs'
import DatePicker from 'vue-datepicker-next'
import 'vue-datepicker-next/index.css'
import { checkAccessModule } from '@/helper/checkAccessModule'
import { LeaveOffService } from '@/service/leaveoff.service'
import DialogUserInfoLeaveOff from './DialogUserInfoLeaveOff.vue'

export default {
    data() {
        return {
            searchtext: null,
            sortMonth: null,
            idUser: null,
            arr: [],
            resultCount: null,
            loading: true,
            displayBasic: false,
            pageIndex: [5, 10, 15, 20],
            num: 5,
            isOpenDialog: false,
            defaultPageSize: 100,
            statusLeave: [
                {
                    id: 2,
                    title: 'Đã duyệt',
                    class: 'badge bg-success',
                },
                {
                    id: 3,
                    title: 'Đã hủy',
                    class: 'badge bg-danger ',
                },
            ],
            fillterLeaveOff: {
                sortMonth: null,
                searchLeaveOff: '',
                idstatus: [],
                sortMonthOfYear: null,
            },
            showButton: {
                add: false,
            },
            resultPgae: {
                pageSize: 10,
                pageNumber: 1,
            },
            totalItem: 0,
            totalMapPage: 0,
            itemIndex: 0,
            canExport: true,
            isOpenDialogUserInfoLeaveOff: false,
        }
    },
    async created() {
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '').slice(0, 9)) === true) {
            checkAccessModule.checkPermissionAction(this.$route.path.replace('/', '').slice(0, 9), this.showButton)
            await this.checkPermissionGroup()
        } else {
            alert('bạn không có quyền giờ đến trang HOME nhé')
            router.push('/')
        }
    },
    watch: {
        fillterLeaveOff: {
            handler: function change(newVal) {
                this.handlerFillterLeaveOff()
            },
            deep: true,
        },
        resultPgae: {
            handler: function change() {
                if (
                    this.fillterLeaveOff.searchLeaveOff != '' ||
                    this.fillterLeaveOff.sortMonth != null ||
                    this.fillterLeaveOff.sortMonthOfYear != null ||
                    this.fillterLeaveOff.idstatus != []
                ) {
                    this.handlerFillterLeaveOff()
                } else {
                    this.getLeaveOff()
                }
            },
            deep: true,
        },
    },
    methods: {
        showError(message) {
            this.$toast.add({ severity: 'error', summary: 'Lỗi!', detail: message, life: 2000 })
        },
        async checkPermissionGroup() {
           if(checkAccessModule.checkCallAPI(this.$route.path.replace('/', ''))){
                   this.getLeaveOff()
           }else{
                    setTimeout(() => {
                        this.showError('Người dùng không có quyền!')
                        this.$router.push('/')
                    }, 800)
           }
        },
        checkStatus(id) {
            var fillter = this.statusLeave.filter(function (el) {
                return el.id == id
            })
            return Object.assign({}, fillter[0])
        },
        async getLeaveOff() {
            LeaveOffService.getAllLeaveOffPageListInfo(this.resultPgae.pageNumber, this.resultPgae.pageSize).then(
                (res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    var data = res.data._Data.map((el) => ({
                        id: el.id,
                        startTime: this.formatDate(el.startTime),
                        endTime: this.formatDate(el.endTime),
                        acceptTime: el.acceptTime,
                        createTime: el.createTime,
                        idAcceptUser: el.idAcceptUser,
                        idLeaveUser: el.idLeaveUser,
                        reasons: el.reasons,
                        status: el.status,
                        name: el.name,
                        acceptBy: el.acceptBy,
                        realTime: this.mathLeaveOffDate(el.startTime, el.endTime, el.idCompanyBranh, el.flagOnDay),
                        idCompanyBranh: el.idCompanyBranh,
                        userCode: el.userCode,
                    }))
                    this.arr = data
                    if (res.data._Data.length > 0) {
                        this.canExport = false
                    } else {
                        this.canExport = true
                    }
                },
            )
            this.loading = false
        },
        async handlerReload() {
            this.loading = true
            this.arr = []
            this.fillterLeaveOff.searchLeaveOff = ''
            this.fillterLeaveOff.sortMonth = null
            this.fillterLeaveOff.sortMonthOfYear = null
            this.fillterLeaveOff.idstatus = []
            await this.getLeaveOff()
        },
        formatDate(date) {
            const fmDate = new Date(date)
            return dayjs(fmDate).format('YYYY/MM/DD (HH:mm)')
        },
        async handlerIdUser() {
            const userIds = this.arr.map((item) => item.idLeaveUser)
            const acceptUserIds = this.arr.map((item) => item.idAcceptUser)

            const [users, acceptUsers] = await Promise.all([
                this.getUsersByIds(userIds),
                this.getUsersByIds(acceptUserIds),
            ])

            const acceptUserCache = {}
            const promises = []
            for (const item of this.arr) {
                item.user = users[item.idLeaveUser]
                item.name = item.user?.fullName ?? 'Chưa có tên...'
                item.userCode = item.user?.userCode

                if (!acceptUserCache[item.idAcceptUser]) {
                    const promise = await this.getUserById(item.idAcceptUser).then((accept) => {
                        acceptUserCache[item.idAcceptUser] = accept
                    })
                    promises.push(promise)
                }
            }
            await Promise.all(promises)
            for (const item of this.arr) {
                item.acceptBy = acceptUserCache[item.idAcceptUser]?.fullName ?? 'Chưa có tên...'
            }
        },
        async getUsersByIds(ids) {
            const cache = {}
            const promises = []
            for (const id of ids) {
                if (!cache[id]) {
                    const promise = await this.getUserById(id)
                    cache[id] = promise
                    promises.push(promise)
                }
            }
            const results = await Promise.all(promises)
            return results.reduce((acc, user, index) => {
                acc[ids[index]] = user
                return acc
            }, {})
        },
        async getUserById(id) {
            return await HTTP.get(GET_USER_NAME_BY_ID(id)).then((res) => res.data)
        },
        async handlerFillterLeaveOff() {
            this.loading = true
            if (
                this.fillterLeaveOff.searchLeaveOff != '' ||
                this.fillterLeaveOff.sortMonth != null ||
                this.fillterLeaveOff.sortMonthOfYear != null ||
                this.fillterLeaveOff.idstatus != []
            ) {
                var currentDate = new Date()
                if (this.fillterLeaveOff.sortMonth == null) {
                    currentDate = new Date()
                } else {
                    currentDate = new Date(this.fillterLeaveOff.sortMonth)
                }
                var theFirst = new Date(currentDate.getFullYear(), 0, 1)

                var findByNameStatusDateDtos = {
                    fullName: this.fillterLeaveOff.searchLeaveOff,
                    date: DateHelper.convertUTC(theFirst),
                    status: this.fillterLeaveOff.idstatus,
                    dateMonth: DateHelper.convertUTC(this.fillterLeaveOff.sortMonthOfYear),
                }
                await LeaveOffService.filterLeaveOffByYear(
                    this.resultPgae.pageNumber,
                    this.resultPgae.pageSize,
                    findByNameStatusDateDtos,
                ).then((res) => {
                    this.arr = []
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    var data = res.data._Data.map((el) => ({
                        id: el.id,
                        startTime: this.formatDate(el.startTime),
                        endTime: this.formatDate(el.endTime),
                        acceptTime: el.acceptTime,
                        createTime: el.createTime,
                        idAcceptUser: el.idAcceptUser,
                        idLeaveUser: el.idLeaveUser,
                        reasons: el.reasons,
                        status: el.status,
                        name: el.name,
                        acceptBy: el.acceptBy,
                        realTime: this.mathLeaveOffDate(el.startTime, el.endTime, el.idCompanyBranh, el.flagOnDay),
                        idCompanyBranh: el.idCompanyBranh,
                        userCode: el.userCode,
                    }))
                    this.arr = data
                })
                this.loading = false
            } else {
                await this.getLeaveOff()
                this.loading = false
            }
        },
        exportCSV() {
            const data = this.arr.map((el) => ({
                userCode: el.userCode,
                name: el.name,
                startTime: el.startTime,
                endTime: el.endTime,
                realTime: el.realTime,
                reasons: el.reasons,
                status: this.checkStatus(el.status).title,
                acceptBy: el.acceptBy,
            }))
            import('../../plugins/Export2Excel.js').then((excel) => {
                const OBJ = data

                const Header = [
                    'Mã nhân viên',
                    'Tên nhân viên',
                    'Ngày bắt đầu',
                    'Ngày kết thúc',
                    'Thời gian thực tế',
                    'Lý do',
                    'Trạng thái',
                    'Người duyệt',
                ]
                const Field = ['userCode', 'name', 'startTime', 'endTime', 'realTime', 'reasons', 'status', 'acceptBy']

                const Data = this.FormatJSon(Field, OBJ)
                excel.export_json_to_excel({
                    header: Header,
                    data: Data,
                    sheetName: 'sheet1',
                    filename: 'Danh sách nghỉ phép',
                    autoWidth: true,
                    bookType: 'xlsx',
                })
            })
        },

        FormatJSon(FilterData, JsonData) {
            return JsonData.map((v) =>
                FilterData.map((j) => {
                    return v[j]
                }),
            )
        },
        mathLeaveOffDate(startTime, endTime, idCompanyBranh, flagOnDay) {
            const start = new Date(startTime)
            const end = new Date(endTime)

            // Kiểm tra xem thời gian bắt đầu và kết thúc có cùng ngày không
            const isSameDay =
                start.getFullYear() === end.getFullYear() &&
                start.getMonth() === end.getMonth() &&
                start.getDate() === end.getDate()

            let diff = end - start

            // Trừ thời gian nghỉ trưa nếu trong giờ làm việc
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
            // if (flagOnDay != true) {
            if (start.getHours() == 8 && start.getMinutes() == 0 && end.getHours() == 17 && end.getMinutes() <= 30) {
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

            // Định dạng chuỗi kết quả
            let result = ''
            if (days > 0) {
                result += days + 'd '
            }
            if (hours > 0) {
                if (hours == 8) {
                    var map = Number(hours / 8)
                    result = ''
                    result += days + map + 'd '
                } else {
                    result += hours + 'h '
                }
            }
            if (minutes > 0) {
                result += minutes + 'm'
            }
            if (hours == 23 && minutes <= 30 && isSameDay) {
                result = 'Không tính.'
            }
            return result.trim()
        },
        handlerUserInfoLeaveOff() {
            this.isOpenDialogUserInfoLeaveOff = true
        },
        closeDialogUserInfoLeaveOff() {
            this.isOpenDialogUserInfoLeaveOff = false
        },
    },
    components: {
        View,
        Export,
        DialogUserInfoLeaveOff,
    },
}
</script>

<style lang="scss" scoped>
.mx-input {
    display: inline-block;
    box-sizing: border-box;
    width: 100%;
    height: 36px !important;
    padding: 6px 30px;
    padding-left: 10px;
    font-size: 14px;
    line-height: 1.4;
    color: #555;
    background-color: #fff;
    border: 1px solid #ccc;
    border-radius: 4px;
    box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.075);
}
</style>
