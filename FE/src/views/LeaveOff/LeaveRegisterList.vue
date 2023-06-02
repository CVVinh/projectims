<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">DANH SÁCH ĐĂNG KÝ NGHỈ PHÉP</li>
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
                    <div class="collapse navbar-collapse mt-2" id="collapsibleNavbar">
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    type="button"
                                    icon="pi pi-filter-slash "
                                    class="custom-reload-h"
                                    @click="reloadPage()"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <MultiSelect
                                    style="width: 100%"
                                    v-model="fillterLeaveOff.selectedLeaveOff"
                                    :options="statusLeave"
                                    optionLabel="title"
                                    optionValue="id"
                                    placeholder="Chọn trạng thái"
                                    class="custom-input-h"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <date-picker
                                    v-model:value="fillterLeaveOff.selectedDate"
                                    type="month"
                                    placeholder="Chọn tháng"
                                ></date-picker>
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    style="width: 100%"
                                    class="custom-input-h"
                                    v-model="fillterLeaveOff.searchLeaveOff"
                                    placeholder="Nhập tên nhân viên..."
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>
            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        :value="dataLeaveOff"
                        :rows="5"
                        :rowHover="true"
                        ref="dt"
                        :loading="loading"
                        showGridlines
                        responsiveLayout="scroll"
                        :globalFilterFields="[
                            'id',
                            'startTime',
                            'reasons',
                            'notAcceptUser',
                            'reasonAccept',
                            'idLeaveUser',
                            'endTime',
                            'user',
                            'name',
                        ]"
                    >
                        <template #empty> Không tìm thấy. </template>
                        <Column field="#" header="#">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="name" header="Tên nhân viên" :sortable="true">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column field="startTime" header="Ngày nghỉ" :sortable="true">
                            <template #body="{ data }">
                                {{ formartDate(data.startTime) + ' - ' + formartDate(data.endTime) }}
                            </template>
                        </Column>
                        <Column field="realTime" header="Thời gian thực tế">
                            <template #body="{ data }">
                                {{ data.realTime }}
                            </template>
                        </Column>
                        <Column field="reasons" header="Lý do nghỉ">
                            <template #body="{ data }">
                                {{ data.reasons }}
                            </template>
                        </Column>
                        <Column field="reasonAccept" header="Lý do không chấp nhận">
                            <template #body="{ data }">
                                <span v-if="data.status == 2 || data.status == 3">
                                    {{ data.reasonAccept }}
                                </span>
                                <span v-else style="color: burlywood"> Chưa nhập lý do... </span>
                            </template>
                        </Column>
                        <Column field="status" header="Trạng thái">
                            <template #body="{ data }">
                                <span :class="checkStatus(data.status).class">
                                    {{ checkStatus(data.status).title }}
                                </span>
                            </template>
                        </Column>
                        <Column header="Thực thi">
                            <template #body="{ data }">
                                <div class="d-flex justify-content-center">
                                    <Button
                                        @click="showConfirmLeaveOffReasonAccept(data)"
                                        class="p-button-sm custom-button-delete mt-1 me-2 p-button-success"
                                        icon="pi pi-check"
                                        :disabled="checkCanOperation('xacnhan', data)"
                                        :class="{
                                            'p-button-success': data.status != 2,
                                            'p-button-secondary': data.status == 2,
                                        }"
                                        v-if="this.showButton.confirm"
                                        v-tooltip.top="'Duyệt nghỉ phép'"
                                    />
                                    <Button
                                        @click="showConfirmLeaveOff(data)"
                                        class="p-button-sm custom-button-delete p-button-danger mt-1 me-2"
                                        icon="pi pi-times"
                                        :disabled="checkCanOperation('tuchoi', data)"
                                        v-if="this.showButton.refuse"
                                        v-tooltip.top="'Không duyệt nghỉ phép'"
                                    />
                                    <Button
                                        v-if="this.showButton.delete"
                                        :hidden="checkCanOperation('xoa', data)"
                                        class="p-button-sm custom-button-delete p-button-danger mt-1 me-2"
                                        @click="confirmDeleteLeaveOff(data)"
                                        icon="pi pi-trash"
                                    />
                                </div>
                            </template>
                        </Column>
                    </DataTable>
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
        <DialogConfirmLeave
            :idLeaveOff="idLeaveoff"
            :isOpen="this.isOpenDialog"
            @closeDialog="closeDialogLeaveoff()"
            @reloadPage="checkPermissionGroup()"
        />
        <DialogReasonAcceptLeveOff
            :idLeaveOff="idLeaveOffReasonAccept"
            :isOpen="this.isOpenDialogReasonAccept"
            @closeDialog="closeDialogLeaveoffReasonAccept()"
            @reloadPage="checkPermissionGroup()"
        />
    </LayoutDefaultDynamic>
</template>
<script>
import { HTTP, ENDPIONTS, GET_USER_NAME_BY_ID, ACCEPT_LEAVE_OFF, DELETE_LEAVE_OFF } from '@/http-common'
import dayjs from 'dayjs'
import DialogConfirmLeave from './DialogConfirmLeave.vue'
import DialogReasonAcceptLeveOff from './DialogReasonAcceptLeveOff.vue'
import { DateHelper } from '@/helper/date.helper'
import { checkAccessModule } from '@/helper/checkAccessModule'
import { LeaveOffService } from '@/service/leaveoff.service'
export default {
    async created() {
        ///leaveoff/acceptregisterlists
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '').slice(0, 9)) === true) {
            checkAccessModule.checkPermissionAction(this.$route.path.replace('/', '').slice(0, 9), this.showButton)
            this.checkPermissionGroup()
        } else {
            this.displayDialog1 = true
            alert('Group không có quyền truy cập')
            this.$router.push('/')
        }
    },
    data() {
        return {
            isOpenDialog: false,
            isChange: false,
            idLeaveoff: null,
            isSearchAdvanced: false,
            fillterLeaveOff: {
                selectedLeaveOff: [],
                selectedDate: null,
                searchLeaveOff: '',
            },
            dataLeaveOff: [],
            Action: false,
            statusLeave: [
                {
                    id: 1,
                    title: 'Đang chờ',
                    class: 'badge bg-warning',
                },
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
            showButton: {
                add: false,
                update: false,
                delete: false,
                deleteMulti: false,
                confirm: false,
                confirmMulti: false,
                refuse: false,
                addMember: false,
                export: false,
            },
            loading: false,
            idLeaveOffReasonAccept: null,
            isOpenDialogReasonAccept: false,
            isChangeReasonAccept: false,
            resultPgae: {
                pageSize: 10,
                pageNumber: 1,
            },
            totalItem: 0,
            totalMapPage: 0,
            itemIndex: 0,
        }
    },
    watch: {
        fillterLeaveOff: {
            handler: async function change(newVal) {
                await this.handlerFillerPermission()
            },
            deep: true,
        },
        resultPgae: {
            handler: async function change() {
                if (
                    this.fillterLeaveOff.searchLeaveOff != '' ||
                    this.fillterLeaveOff.selectedDate != null ||
                    this.fillterLeaveOff.selectedLeaveOff.length > 0
                ) {
                    await this.handlerFillerPermission()
                } else {
                    await this.checkPermissionGroup()
                }
            },
            deep: true,
        },
    },
    methods: {
        showError(message) {
            this.$toast.add({ severity: 'error', summary: 'Lỗi!', detail: message, life: 2000 })
        },
        showSuccess(message) {
            this.$toast.add({ severity: 'success', summary: 'Thành công!', detail: message, life: 2000 })
        },
        async checkPermissionGroup() {
            this.dataLeaveOff = []
            this.loading = true
            if(checkAccessModule.checkCallAPI(this.$route.path.replace('/', ''))){
                await this.getAllLeaveOffRegister()
            }else{
                if (checkAccessModule.isLead()) {
                        await this.getAllLeaveOffRegisterByLead()
                }
                if (checkAccessModule.isPm()) {
                    await this.getAllLeaveOffRegisterByPM()
                }
            }
        },
        /* checkIsGroup(nameGroup) {
            var name = nameGroup.toLowerCase()
            if (name === 'admin') {
                if (checkAccessModule.isAdmin()) {
                    return true
                } else {
                    return false
                }
            }

            if (name === 'lead') {
                if (checkAccessModule.isLead()) {
                    return true
                } else {
                    return false
                }
            }

            if (name === 'pm') {
                if (checkAccessModule.isPm()) {
                    return true
                } else {
                    return false
                }
            }

            if (name === 'office') {
                if (checkAccessModule.isOffice() && checkAccessModule.getListNameGroup().length === 1) {
                    return true
                } else {
                    return false
                }
            }

            if (name === 'staff') {
                if (checkAccessModule.isStaff()) {
                    return true
                } else {
                    return false
                }
            }
        }, */

        checkCanOperation(nameButton, data) {
            const name = nameButton.toLowerCase()
            if (name === 'them') {
            }
            if (name === 'sua') {
            }
            if (name === 'xoa') {
                if (
                    (data.status === 1 && data.idLeaveUser === Number(checkAccessModule.getUserIdCurrent())) ||
                    checkAccessModule.isAdmin()
                ) {
                    return false
                } else {
                    return true
                }
            }
            if (name === 'xoanhieu') {
            }
            if (name === 'xuatfile') {
            }
            if (name === 'xacnhan') {
                if (data.status !== 1) {
                    return true
                } else {
                    return false
                }
            }
            if (name === 'xacnhannhieu') {
            }
            if (name === 'themthanhvien') {
            }
            if (name === 'tuchoi') {
                if (data.status !== 1) {
                    return true
                } else {
                    return false
                }
            }
        },
        async reloadPage() {
            this.fillterLeaveOff.searchLeaveOff = ''
            this.fillterLeaveOff.selectedDate = null
            this.fillterLeaveOff.selectedLeaveOff = []
            await this.checkPermissionGroup()
        },
        checkStatus(id) {
            var fillter = this.statusLeave.filter(function (el) {
                return el.id == id
            })
            return Object.assign({}, fillter[0])
        },
        formartDate(date) {
            const fmDate = new Date(date)
            return dayjs(fmDate).format('YYYY/MM/DD (HH:mm)')
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
        showConfirmLeaveOff(data) {
            this.idLeaveoff = data.id
            this.isOpenDialog = true
        },
        async confirmDeleteLeaveOff(data) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn muốn xóa đăng ký nghỉ phép này?',
                header: 'Xóa nghỉ phép!',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Xóa',
                rejectLabel: 'Hủy',
                acceptIcon: 'pi pi-trash',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-danger CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: async () => {
                    await this.handlerDeleteLeaveOff(data)
                },
                reject: () => {
                    return
                },
            })
        },
        async handlerDeleteLeaveOff(data) {
            if (data) {
                await HTTP.delete(DELETE_LEAVE_OFF(data.id))
                    .then(async (res) => {
                        this.showSuccess(res.data._Message)
                        await this.handlerFillterLeaveOff()
                    })
                    .catch((err) => {
                        this.showError("Có lỗi trong quá trình xử lý!")
                        console.log(err)
                    })
            }
        },
        async closeDialogLeaveoff() {
            this.isOpenDialog = false
        },
        showConfirmLeaveOffReasonAccept(data) {
            this.idLeaveOffReasonAccept = data.id
            this.isOpenDialogReasonAccept = true
        },
        async closeDialogLeaveoffReasonAccept() {
            this.isOpenDialogReasonAccept = false
        },
        async handlerData(arrData, filterData = false) {
            if(filterData){
                await arrData.map((el) => {
                    const object = {
                        id: el.id,
                        startTime: el.startTime,
                        endTime: el.endTime,
                        acceptTime: el.acceptTime,
                        createTime: el.createTime,
                        idAcceptUser: el.idAcceptUser,
                        idLeaveUser: el.idLeaveUser,
                        reasons: el.reasons,
                        status: el.status,
                        name: el.name,
                        acceptBy: el.acceptBy,
                        realTime: this.mathLeaveOffDate(el.startTime,el.endTime,el.idCompanyBranh,el.flagOnDay,),
                        idCompanyBranh: el.idCompanyBranh,
                        userCode: el.userCode,
                        notAcceptUser: el.reasonNotAccept,
                        reasonAccept: el.reasonAccept,
                    }
                    this.dataLeaveOff.push(object)
                })
            }
            else {
                await arrData.map((el) => {
                    const object = {
                        id: el.id,
                        startTime: el.startTime,
                        endTime: el.endTime,
                        acceptTime: el.acceptTime,
                        createTime: el.createTime,
                        idAcceptUser: el.idAcceptUser,
                        idLeaveUser: el.idLeaveUser,
                        reasons: el.reasons,
                        status: el.status,
                        name: el.name,
                        acceptBy: el.acceptBy,
                        realTime: this.mathLeaveOffDate(el.startTime, el.endTime, el.idCompanyBranh),
                        idCompanyBranh: el.idCompanyBranh,
                        userCode: el.userCode,
                        notAcceptUser: el.reasonNotAccept,
                        reasonAccept: el.reasonAccept,
                    }
                    this.dataLeaveOff.push(object)
                })
            }
            this.dataLeaveOff = this.dataLeaveOff.filter((obj, index, self) => {
                return index === self.findIndex((t) => t.id === obj.id)
            })
        },
        async getAllLeaveOffRegisterByPM() {
            await LeaveOffService.getAllLeaveOffPageListByPM(
                this.resultPgae.pageNumber,
                this.resultPgae.pageSize,
                Number(checkAccessModule.getUserIdCurrent()),
            ).then(async (res) => {
                this.totalMapPage = res.data._totalPages
                this.totalItem = res.data._totalItems
                this.itemIndex = res.data._itemIndex
                await this.handlerData(res.data._Data, false)
            })
            .finally(() => {
                this.loading = false
            })
        },
        async getAllLeaveOffRegisterByLead() {
            await LeaveOffService.getAllLeaveOffPageListByLead(
                this.resultPgae.pageNumber,
                this.resultPgae.pageSize,
                Number(checkAccessModule.getUserIdCurrent()),
            ).then(async (res) => {
                this.totalMapPage = res.data._totalPages
                this.totalItem = res.data._totalItems
                this.itemIndex = res.data._itemIndex
                await this.handlerData(res.data._Data, false)
            })
            .finally(() => {
                this.loading = false
            })
        },
        async getAllLeaveOffRegister() {
            await LeaveOffService.getAllLeaveOffPageList(this.resultPgae.pageNumber, this.resultPgae.pageSize).then(
                async (res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    await this.handlerData(res.data._Data, false)
                },
            )
            .finally(() => {
                this.loading = false
            })
        },
        async handlerFillerPermission() {
            this.dataLeaveOff = []
            this.loading = true
            await checkAccessModule.getListNameGroup().some(async (ele) => {
                if (checkAccessModule.checkGetData(this.$route.path.replace('/', '')).includes(ele)) {
                    await this.handlerFillterLeaveOff()
                } else {
                    if (checkAccessModule.isLead()) {
                        await this.handlerFillterLeaveOffLead()
                    }
                    if (checkAccessModule.isPm()) {
                        await this.handlerFillterLeaveOffPM()
                    }
                }
            })
        },
        async handlerFillterLeaveOff() {
            if (
                this.fillterLeaveOff.searchLeaveOff != '' ||
                this.fillterLeaveOff.selectedDate != null ||
                this.fillterLeaveOff.selectedLeaveOff.length > 0
            ) {
                var findByNameStatusDateDtos = {
                    fullName: this.fillterLeaveOff.searchLeaveOff,
                    date: DateHelper.convertUTC(this.fillterLeaveOff.selectedDate),
                    status: this.fillterLeaveOff.selectedLeaveOff,
                }
                await LeaveOffService.handlerFilterLeaveOff(
                    this.resultPgae.pageNumber,
                    this.resultPgae.pageSize,
                    findByNameStatusDateDtos,
                )
                    .then(async(res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        await this.handlerData(res.data._Data, true)
                    })
                    .catch((err) => {
                        this.showError("Lỗi! trong quá trình xử lý!")
                        console.log(err);
                    })
                    .finally(() => {
                        this.loading = false
                    })
            } else {
                this.getAllLeaveOffRegister()
            }
        },
        async handlerFillterLeaveOffLead() {
            if (
                this.fillterLeaveOff.searchLeaveOff != '' ||
                this.fillterLeaveOff.selectedDate != null ||
                this.fillterLeaveOff.selectedLeaveOff.length > 0
            ) {
                var findByNameStatusDateDtos = {
                    fullName: this.fillterLeaveOff.searchLeaveOff,
                    date: DateHelper.convertUTC(this.fillterLeaveOff.selectedDate),
                    status: this.fillterLeaveOff.selectedLeaveOff,
                }
                await LeaveOffService.handlerFilterLeaveOffLead(
                    checkAccessModule.getUserIdCurrent(),
                    this.resultPgae.pageNumber,
                    this.resultPgae.pageSize,
                    findByNameStatusDateDtos,
                )
                    .then(async(res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        await this.handlerData(res.data._Data, true)
                    })
                    .catch((err) => {
                        this.showError("Lỗi! trong quá trình xử lý!")
                        console.log(err);
                    })
                    .finally(() => {
                        this.loading = false
                    })
            } else {
                await this.getAllLeaveOffRegisterByLead()
            }
        },
        async handlerFillterLeaveOffPM() {
            if (
                this.fillterLeaveOff.searchLeaveOff != '' ||
                this.fillterLeaveOff.selectedDate != null ||
                this.fillterLeaveOff.selectedLeaveOff.length > 0
            ) {
                var findByNameStatusDateDtos = {
                    fullName: this.fillterLeaveOff.searchLeaveOff,
                    date: DateHelper.convertUTC(this.fillterLeaveOff.selectedDate),
                    status: this.fillterLeaveOff.selectedLeaveOff,
                }
                await LeaveOffService.handlerFilterLeaveOffPM(
                    checkAccessModule.getUserIdCurrent(),
                    this.resultPgae.pageNumber,
                    this.resultPgae.pageSize,
                    findByNameStatusDateDtos,
                )
                    .then(async (res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        await this.handlerData(res.data._Data, true)
                    })
                    .catch((err) => {
                        this.showError("Lỗi! trong quá trình xử lý!")
                        console.log(err);
                    })
                    .finally(() => {
                        this.loading = false
                    })
            } else {
                await this.getAllLeaveOffRegisterByPM()
            }
        },
    },
    components: {
        DialogConfirmLeave,
        DialogReasonAcceptLeveOff,
    },
}
</script>
<style scoped lang="scss">

</style>
