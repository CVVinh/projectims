<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">ĐĂNG KÝ NGHỈ PHÉP</li>
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
                        <ul class="navbar-nav me-auto">
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    class="p-button-sm custom-button-h"
                                    @click="handlerAddLeaveOff()"
                                    v-if="this.showButton.add"
                                    icon="pi pi-plus"
                                    label="Thêm nghỉ phép"
                                ></Button>
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    class="p-button-sm custom-button-h"
                                    icon="pi pi-info-circle"
                                    @click="handlerUserInfoLeaveOff()"
                                    label="Thông tin nghỉ phép cá nhân"
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        :value="listLeaveOffbyUser"
                        :rows="5"
                        :rowHover="true"
                        ref="dt"
                        :loading="loading"
                        showGridlines
                        responsiveLayout="scroll"
                        :globalFilterFields="[
                            'id',
                            'createTime',
                            'startTime',
                            'endTime',
                            'reasonNotAccept',
                            'reasons',
                            'status',
                        ]"
                    >
                        <template #empty> Không tìm thấy. </template>
                        <Column field="#" header="#">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="startTime" header="Ngày bắt đầu">
                            <template #body="{ data }">
                                {{ formartDate(data.startTime) }}
                            </template>
                        </Column>
                        <Column field="endTime" header="Ngày kết thúc ">
                            <template #body="{ data }">
                                {{ formartDate(data.endTime) }}
                            </template>
                        </Column>
                        <Column field="status" header="Trạng thái">
                            <template #body="{ data }">
                                <span :class="checkStatus(data.status).class">
                                    {{ checkStatus(data.status).title }}
                                </span>
                            </template>
                        </Column>

                        <Column header="Thực thi" style="width: 13%">
                            <template #body="{ data }">
                                <div class="d-flex justify-content-start">
                                    <Button
                                        class="p-button-sm custom-input-h p-button-info mt-1 me-2"
                                        @click="handlerDetailsLeaveOff(data)"
                                        icon="pi pi-eye"
                                        v-tooltip.top="'Xem chi tiết'"
                                    />
                                    <Button
                                        v-if="this.showButton.update"
                                        class="p-button-sm custom-input-h mt-1 me-2 p-button-warning"
                                        @click="handlerEditLeaveOff(data)"
                                        icon="pi pi-pencil"
                                        :disabled="checkCanOperation('sua', data)"
                                    />
                                    <Button
                                        v-if="this.showButton.delete || checkIsGroup('admin')"
                                        class="p-button-sm custom-input-h p-button-danger mt-1 me-2"
                                        @click="confirmDeleteLeaveOff(data)"
                                        icon="pi pi-trash"
                                        :disabled="checkCanOperation('xoa', data)"
                                    />
                                </div>
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
        <DialogAddLeaveOff
            :isOpen="this.isOpenDialog"
            :selectedLeaveOff="{ ...leaveOffSelected }"
            @getAllLeaveOff="getAllLeaveOffRegister"
            @closeDialog="closeDialogLeaveoff()"
            @setChange="setChange()"
        />
        <DialogViewDetailLeaveOff
            :isOpen="this.isOpenDialogDetailLeaveoff"
            :selectedLeaveOff="{ ...leaveOffDetail }"
            @closeDialog="closeDialogDetailLeaveoff()"
        />
        <DialogUserInfoLeaveOff
            :isOpen="this.isOpenDialogUserInfoLeaveOff"
            @closeDialog="closeDialogUserInfoLeaveOff()"
            :selectedallUser="false"
        />
    </LayoutDefaultDynamic>
</template>

<script>
import { HTTP, DELETE_LEAVE_OFF, GET_LIST_LEAVEOFF_BY_USER } from '@/http-common'
import dayjs from 'dayjs'
import DialogAddLeaveOff from './DialogAddLeaveOff.vue'
import jwtDecode from 'jwt-decode'
import { LeaveOffDto } from '@/views/LeaveOff/LeaveOff.dto'
import { HttpStatus } from '@/config/app.config'
import DialogViewDetailLeaveOff from './DialogViewDetailLeaveOff.vue'
import { LocalStorage } from '@/helper/local-storage.helper'
import { UserRoleHelper } from '@/helper/user-role.helper'
import router from '../../router'
import { checkAccessModule } from '@/helper/checkAccessModule'
import { LeaveOffService } from '@/service/leaveoff.service'
import DialogUserInfoLeaveOff from './DialogUserInfoLeaveOff.vue'

export default {
    async created() {
        ///leaveoff/Registerlists
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '').slice(0, 9)) === true) {
                checkAccessModule.checkPermissionAction(this.$route.path.replace('/', '').slice(0, 9), this.showButton)
                await this.getAllLeaveOffRegister()
        } else {
            alert('bạn không có quyền giờ đến trang HOME nhé')
            router.push('/')
        }
    },
    data() {
        return {
            isOpenDialog: false,
            isOpenDialogDetailLeaveoff: false,
            isChange: false,
            loading: true,
            listLeaveOffbyUser: [],
            leaveOffSelected: new LeaveOffDto(),
            leaveOffDetail: new LeaveOffDto(),
            statusLeave: [
                {
                    id: 1,
                    title: 'Chờ duyệt',
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
            resultPgae: {
                pageSize: 10,
                pageNumber: 1,
            },
            totalItem: 0,
            totalMapPage: 0,
            itemIndex: 0,
            isOpenDialogUserInfoLeaveOff: false,
        }
    },
    watch: {
        resultPgae: {
            handler: function change() {
                this.getAllLeaveOffRegister()
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
        showWarn(message) {
            this.$toast.add({ severity: 'warn', summary: 'Cảnh báo!', detail: message, life: 2000 })
        },
        async setChange() {
            this.isChange = true
        },

        checkIsGroup(nameGroup) {
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
        },

        checkCanOperation(nameButton, data) {
            const name = nameButton.toLowerCase()
            if (name === 'them') {
            }
            if (name === 'sua') {
                if (data.status === 1) {
                    return false
                } else {
                    return true
                }
            }
            if (name === 'xoa') {
                if (data.status === 1 || this.checkIsGroup('admin')) {
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
            }
            if (name === 'xacnhannhieu') {
            }
            if (name === 'themthanhvien') {
            }
            if (name === 'tuchoi') {
            }
        },

        handlerAddLeaveOff() {
            this.isOpenDialog = true
            this.leaveOffSelected = []
        },
        handlerEditLeaveOff(data) {
            this.leaveOffSelected = { ...data }
            this.isOpenDialog = true
        },
        async closeDialogLeaveoff() {
            this.isOpenDialog = false
            this.leaveOffSelected = []
            if (this.isChange === true) {
                this.listLeaveOffbyUser = []
                await this.getAllLeaveOffRegister()
                this.isChange = false
            }
        },
        checkStatus(id) {
            var fillter = this.statusLeave.filter(function (el) {
                return el.id == id
            })
            return Object.assign({}, fillter[0])
        },
        formartDate(date) {
            const fmDate = new Date(date)
            return dayjs(fmDate).format('YYYY/MM/DD-HH:mm:ss')
        },
        async getAllLeaveOffRegister() {
            var userId = checkAccessModule.getUserIdCurrent()
            await LeaveOffService.getAllLeaveOffPage(userId, this.resultPgae.pageNumber, this.resultPgae.pageSize).then(
                (res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.listLeaveOffbyUser = res.data._Data
                },
            )
            this.loading = false
        },

        confirmDeleteLeaveOff(data) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn muốn xóa nghỉ phép này?',
                header: 'Xóa nghỉ phép',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Xóa',
                rejectLabel: 'Hủy',
                acceptIcon: 'pi pi-trash',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-danger CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: () => {
                    this.handlerDeleteLeaveOff(data)
                },
                reject: () => {
                    return
                },
            })
        },
        handlerDeleteLeaveOff(data) {
            if (data) {
                HTTP.delete(DELETE_LEAVE_OFF(data.id))
                    .then((res) => {
                        if (res.status === HttpStatus.OK) {
                            this.showSuccess(res.data._Message)
                            this.getAllLeaveOffRegister()
                        } else {
                            this.showWarn(res.data._Message)
                        }
                    })
                    .catch((err) => {
                        this.showError("Có lỗi trong quá trình xử lý!")
                    })
            }
        },
        handlerDetailsLeaveOff(data) {
            this.isOpenDialogDetailLeaveoff = true
            this.leaveOffDetail = { ...data }
        },
        closeDialogDetailLeaveoff() {
            this.isOpenDialogDetailLeaveoff = false
            this.leaveOffDetail = []
        },
        handlerUserInfoLeaveOff() {
            this.isOpenDialogUserInfoLeaveOff = true
        },
        closeDialogUserInfoLeaveOff() {
            this.isOpenDialogUserInfoLeaveOff = false
        },
    },
    components: {
        DialogAddLeaveOff,
        DialogViewDetailLeaveOff,
        DialogUserInfoLeaveOff,
    },
}
</script>
<style lang="scss" scoped>

</style>
