<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">DANH SÁCH ĐỢT ĐÁNH GIÁ</li>
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
                                <Add
                                    class="custom-button-h"
                                    label="Thêm đợt đánh giá"
                                    @click="openAdd()"
                                    v-if="this.showButton.add"
                                />
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
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
                                <Dropdown
                                    v-model="selectedBranch"
                                    :options="branchList"
                                    filter
                                    optionLabel="branchName"
                                    optionValue="idBranch"
                                    placeholder="Chọn chi nhánh"
                                    style="width: 100%"
                                    class="custom-input-h"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Dropdown
                                    v-model="selectedStaffReview"
                                    :options="staffReviewList"
                                    filter
                                    optionLabel="name"
                                    optionValue="id"
                                    placeholder="Chọn đợt đánh giá"
                                    style="width: 100%"
                                    class="custom-input-h"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Dropdown
                                    v-model="selectedPmLead"
                                    :options="pmLeadList"
                                    filter
                                    optionLabel="fullName"
                                    optionValue="id"
                                    placeholder="Chọn người đánh giá"
                                    style="width: 100%"
                                    class="custom-input-h"
                                    v-if="!checkPmLead()"
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        ref="tableReview"
                        showGridlines
                        class="p-datatable-customers"
                        :value="listReview"
                        :sort="1"
                        :loading="isLoading"
                        responsiveLayout="scroll"
                        filterDisplay="menu"
                        :rowHover="true"
                        :rows="5"
                        dataKey="idRule"
                        v-model:filters="filters"
                        :exportFilename="'List_Ruview_Effort Summary Report_' + new Date()"
                        :globalFilterFields="[
                            'staffReviewTime',
                            'companyBranhId',
                            'reviewerId',
                            'staffReviewId',
                            'userCreated',
                            'dateCreated',
                            'dataReview',
                        ]"
                    >
                        <template #loading> </template>

                        <template #empty>
                            <div v-if="this.isLoading" style="display: flex; justify-items: flex-end">
                                <ProgressSpinner style="width: 42px" />
                            </div>
                            <div v-else>Không tìm thấy.</div>
                        </template>

                        <Column field="#" header="#" dataType="date">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>

                        <Column field="staffReviewTime" header="Đợt đánh giá" sortable>
                            <template #body="{ data }"> Đợt {{ data.staffReviewTime }} </template>
                        </Column>

                        <Column field="fullNameUserCreated" header="Người tạo" sortable>
                            <template #body="{ data }">
                                {{ data.fullNameUserCreated }}
                            </template>
                        </Column>

                        <Column field="fullNameReviewer" header="Người đánh giá" sortable>
                            <template #body="{ data }">
                                {{ data.fullNameReviewer }}
                            </template>
                        </Column>

                        <Column field="fullNameStaffReviewList" header="Nhân viên" sortable style="width: 20%">
                            <template #body="{ data }">
                                {{ data.fullNameStaffReviewList }}
                            </template>
                        </Column>

                        <Column field="branchName" header="Chi nhánh" sortable>
                            <template #body="{ data }">
                                {{ data.branchName }}
                            </template>
                        </Column>

                        <Column field="dateReview" header="Ngày đánh giá" sortable>
                            <template #body="{ data }">
                                {{ data.dateReview }}
                            </template>
                        </Column>

                        <Column field="" header="Thực thi" style="width: 10rem; text-align: left">
                            <template #body="{ data }">
                                <div class="actions-buttons">
                                    <!-- <Button
                                        icon="pi pi-eye"
                                        class="p-button p-component p-button-info btn-margin"
                                        @click="openDetailt(data)"
                                    ></Button>  -->
                                    <Button
                                        icon="bx bx-glasses-alt"
                                        class="p-button p-button-help btn-margin custom-button-h"
                                        v-if="checkCanOperation('thaotac', data)"
                                        v-tooltip.top="'Đánh giá nhân viên'"
                                    ></Button>
                                    <Button
                                        icon="bx bx-glasses-alt"
                                        class="p-button p-button-help btn-margin custom-button-h"
                                        @click="btnStaffReview(data.staffReviewTime, data.companyBranhId, data.reviewerId)"
                                        v-tooltip.top="'Đánh giá nhân viên'"
                                    ></Button>
                                    
                                    <Button
                                        icon="pi pi-pencil"
                                        class="p-button p-button-warning btn-margin custom-button-h"
                                        @click="openEdit(data)"
                                        :hidden="checkCanOperation('sua', data)"
                                        v-if="this.showButton.update"
                                    ></Button>
                                    <Button
                                        icon="pi pi-trash"
                                        class="p-button p-button-danger btn-margin custom-button-h"
                                        @click="confirmDelete(data.staffReviewTime, data.companyBranhId)"
                                        :hidden="checkCanOperation('xoa', data)"
                                        v-if="this.showButton.delete"
                                    ></Button>
                                    <Button
                                        icon="pi pi-check"
                                        class="p-button p-button-success btn-margin custom-button-h"
                                        @click="ConfirmReviewTime(data)"
                                        :hidden="checkCanOperation('xacnhan', data)"
                                        v-if="this.showButton.confirm"
                                        v-tooltip.top="'Hoàn thành đợt đánh giá'"
                                    />
                                </div>
                            </template>
                        </Column>
                    </DataTable>
                </div>
            </div>
            <div class="row">
                <div class="col-12">
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

        <Dialog
            header="Không có quyền truy cập !"
            :visible="displayBasic"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            :modal="true"
            :closable="false"
        >
            <p>Bạn không có quyền truy cập !</p>
            <medium
                >Bạn sẽ được điều hướng vào trang chủ <strong>{{ num }}</strong> giây!</medium
            >
            <template #footer>
                <Button label="Hoàn tất" icon="pi pi-check" @click="navigationToHome()" autofocus />
            </template>
        </Dialog>

        <AddReviewDiaLog
            :statusopen="openAddform"
            :pmLeadList="pmLeadList"
            :branchList="branchList"
            :usersList="usersList"
            @closeAdd="closeAdd()"
            @reloadPageListTimeReview="handlerReload()"
            :listReview="listReview"
        />

        <EditTimeReview
            :statusopen="openEditform"
            :pmLeadList="pmLeadList"
            :branchList="branchList"
            :usersList="usersList"
            :dataRiviewById="dataRiviewById"
            @closeEdit="closeEdit()"
            @reloadPageListTimeReview="handlerReload()"
        />

        <DetailTimeReview
            :statusopen="openDetailtform"
            @closeDetailt="closeDetailt()"
            :dataRiviewById="dataRiviewById"
        />
    </LayoutDefaultDynamic>
</template>

<script>
import Export from '../../components/buttons/Export.vue'
import Edit from '../../components/buttons/Edit.vue'
import Add from '../../components/buttons/Add.vue'
import View from '../../components/buttons/View.vue'
import Delete from '../../components/buttons/Delete.vue'
import { HTTP, HTTP_LOCAL, LinkDownloadFile } from '@/http-common'
import { FilterMatchMode } from 'primevue/api'
import AddReviewDiaLog from '@/views/StaffReview/CreateTimeReview.vue'
import EditTimeReview from '@/views/StaffReview/EditTimeReview.vue'
import DetailTimeReview from '@/views/StaffReview/DetailTimeReview.vue'
import { UserRoleHelper } from '@/helper/user-role.helper'
import { LocalStorage } from '@/helper/local-storage.helper'
import { HttpStatus } from '@/config/app.config'
import { ref } from 'vue'
import { DateHelper } from '@/helper/date.helper'
import { saveAs } from 'file-saver'
import { checkAccessModule } from '@/helper/checkAccessModule'
import router from '@/router'

export default {
    name: 'RuleInfo',

    data() {
        return {
            dataRiviewById: null,
            listReview: [],
            branchList: [],
            pmLeadList: [],
            usersList: [],
            staffReviewList: [],
            selectedBranch: null,
            selectedPmLead: null,
            selectedStaffReview: null,
            openAddform: false,
            openEditform: false,
            openDetailtform: false,
            isLoading: false,
            displayBasic: false,
            num: 5,
            timeOut: null,
            listGroup: null,
            linkFile: LinkDownloadFile,
            filters: {
                global: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
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
        }
    },
    watch: {
        selectedBranch: {
            handler: async function change(newVal) {
                if (newVal != null) {
                    await this.SearchReviewerBranchReviewTime()
                }
            },
            deep: true,
        },
        selectedPmLead: {
            handler: async function change(newVal) {
                if (newVal != null) {
                    await this.SearchReviewerBranchReviewTime()
                }
            },
            deep: true,
        },
        selectedStaffReview: {
            handler: async function change(newVal) {
                if (newVal != null) {
                    await this.SearchReviewerBranchReviewTime()
                }
            },
            deep: true,
        },
        resultPgae: {
            handler: async function change() {
                if (this.selectedBranch != null || this.selectedPmLead != null || this.selectedStaffReview != null) {
                    await this.SearchReviewerBranchReviewTime()
                } else {
                    await this.checkPermission()
                }
            },
            deep: true,
        },
    },
    methods: {
        countTime() {
            if (this.num == 0) {
                this.navigationToHome()
                return
            }
            this.num = this.num - 1
            this.timeOut = setTimeout(() => this.countTime(), 1000)
        },

        checkCanOperation(nameButton, data) {
            const name = nameButton.toLowerCase()
            if (name === 'them') {
            }
            if (name === 'sua') {
                if (data.userCreated === Number(checkAccessModule.getUserIdCurrent()) && data.isConfirm === false) {
                    return false
                } else {
                    return true
                }
            }
            if (name === 'xoa') {
                if (
                    (data.userCreated === Number(checkAccessModule.getUserIdCurrent()) && data.isConfirm === false) ||
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
                if (data.userCreated === Number(checkAccessModule.getUserIdCurrent()) && data.isConfirm === false) {
                    return false
                } else {
                    return true
                }
            }
            if (name === 'xacnhannhieu') {
            }
            if (name === 'themthanhvien') {
            }
            if (name === 'tuchoi') {
            }
        },

        navigationToHome() {
            clearTimeout(this.timeOut)
            this.$router.push({ name: 'home' })
        },

        exportCSV() {
            this.$refs.tableReview.exportCSV()
        },

        showError(message) {
            this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
        },

        showSuccess(message) {
            this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
        },

        showInfo(message) {
            this.$toast.add({ severity: 'info', summary: 'Thông báo', detail: message, life: 3000 })
        },

        showResponseApi(status, message = '') {
            switch (status) {
                case 401:
                case 403:
                    this.showError('Bạn không có quyền thực hiện chức năng này!')
                    break

                case 404:
                    this.showError('Lỗi! Load dữ liệu!')
                    break

                default:
                    if (message != '') {
                        this.showError(message)
                    } else {
                        this.showError('Có lỗi trong quá trình thực hiện!')
                    }
                    break
            }
        },
        btnStaffReview(idReviewTime, idBranch, idReviewer) {
            this.$router.push({
                name: 'reviewStaffs',
                params: { idReviewTime: idReviewTime, idBranch: idBranch, idReviewer: idReviewer },
            })
        },

        async getAllPM() {
            await HTTP.get('Users/GetUserInRolePM')
                .then((res) => {
                    this.pmlist = res.data
                    this.getAllLead()
                })
                .catch((err) => {
                    console.log(err)
                })
        },
        async getAllLead() {
            await HTTP.get('Users/GetUserInRoleLead')
                .then((res) => {
                    this.leadList = res.data
                    this.listPMLead()
                })
                .catch((err) => {
                    console.log(err)
                })
        },
        async getAllReviewTimeName() {
            await HTTP.get(`StaffReviewTime/getAllReviewTimeName`)
                .then((res) => {
                    this.staffReviewList = res.data._Data
                })
                .catch((err) => {
                    console.log(err)
                })
        },
        async listPMLead() {
            var data = await [...this.pmlist, ...this.leadList]
            this.pmLeadList = data
        },

        async getAllBranch() {
            await HTTP.get('Branchs/getListBranch')
                .then((res) => {
                    this.branchList = res.data
                })
                .catch((err) => {
                    console.log(err)
                })
        },

        async getAllUser() {
            await HTTP.get('Users/getAllUserByStaff')
                .then((res) => {
                    this.usersList = res.data._Data
                })
                .catch((err) => {
                    console.log(err)
                })
        },

        async ConfirmReviewTime(data) {
            this.$confirm.require({
                message: 'Bạn có muốn xác nhận hoàn thành đợt đánh giá này?',
                icon: 'pi pi-info-circle',
                header: 'Xác nhận',
                acceptLabel: 'Xác nhận',
                rejectLabel: 'Huỷ',
                acceptIcon: 'pi pi-check',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-primary CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: async () => {
                    var objectDto = {
                        staffReviewTime: data.staffReviewTime,
                        companyBranhId: data.companyBranhId,
                        reviewerId: data.reviewerId,
                        userUpdated: Number(checkAccessModule.getUserIdCurrent()),
                    }
                    await HTTP.put(`StaffReviewTime/ConfirmStaffReviewTime`, objectDto)
                        .then(async (res) => {
                            this.showSuccess('Xác nhận hoàn thành đợt đánh giá thành công!')
                            await this.handlerReload()
                        })
                        .catch((err) => {
                            this.showError('Lỗi! Xác nhận!')
                            console.log(err)
                        })
                },
                reject: () => {
                    return
                },
            })
        },

        async SearchReviewerBranchReviewTime() {
            var linkSearch = `StaffReviewTime/SearchReviewerBranchReviewTime`
            if (this.selectedStaffReview != null && this.selectedBranch == null && this.selectedPmLead == null) {
                linkSearch += `?staffReviewTime=${this.selectedStaffReview}`
            } else if (this.selectedStaffReview != null && this.selectedBranch != null && this.selectedPmLead == null) {
                linkSearch += `?staffReviewTime=${this.selectedStaffReview}&companyBranhId=${this.selectedBranch}`
            } else if (this.selectedStaffReview != null && this.selectedBranch != null && this.selectedPmLead != null) {
                linkSearch += `?staffReviewTime=${this.selectedStaffReview}&companyBranhId=${this.selectedBranch}&reviewerId=${this.selectedPmLead}`
            } else if (this.selectedStaffReview == null && this.selectedBranch != null && this.selectedPmLead == null) {
                linkSearch += `?companyBranhId=${this.selectedBranch}`
            } else if (this.selectedStaffReview == null && this.selectedBranch != null && this.selectedPmLead != null) {
                linkSearch += `?companyBranhId=${this.selectedBranch}&reviewerId=${this.selectedPmLead}`
            } else if (this.selectedStaffReview == null && this.selectedBranch == null && this.selectedPmLead != null) {
                linkSearch += `?reviewerId=${this.selectedPmLead}`
            }

            if (this.selectedStaffReview == null && this.selectedBranch == null && this.selectedPmLead == null) {
                if (checkAccessModule.getListGroup().includes('1')) {
                    linkSearch += `?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`
                } else if (checkAccessModule.getListGroup().includes('2')) {
                    linkSearch += `?idUserCreated=${checkAccessModule.getUserIdCurrent()}`
                } else if (
                    checkAccessModule.getListGroup().includes('3') ||
                    checkAccessModule.getListGroup().includes('5')
                ) {
                    linkSearch += `?idReviewer=${checkAccessModule.getUserIdCurrent()}`
                }
            } else {
                if (checkAccessModule.getListGroup().includes('1')) {
                    linkSearch += `&pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`
                } else if (checkAccessModule.getListGroup().includes('2')) {
                    linkSearch += `&idUserCreated=${checkAccessModule.getUserIdCurrent()}`
                } else if (
                    checkAccessModule.getListGroup().includes('3') ||
                    checkAccessModule.getListGroup().includes('5')
                ) {
                    linkSearch += `&idReviewer=${checkAccessModule.getUserIdCurrent()}`
                }
            }
            this.listReview = []
            this.isLoading = true
            this.totalMapPage = 0
            this.totalItem = 0
            await HTTP.get(linkSearch)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.listReview = res.data._Data
                })
                .catch((err) => {
                    console.log(err)
                })
                .finally(() => {
                    this.isLoading = false
                })
        },

        async handlerReload() {
            this.listReview = []
            this.selectedBranch = null
            this.selectedPmLead = null
            this.selectedStaffReview = null
            await this.checkPermission()
        },

        async checkPermission() {
            if (checkAccessModule.checkCallAPI(this.$route.path.replace('/', '').slice(0, 7))) {
                await this.getAllReviewTimeList()
            } else {
                if (checkAccessModule.isOffice()) {
                    await this.getAllReviewTimeList(null, checkAccessModule.getUserIdCurrent())
                }
                if (checkAccessModule.isLead()) {
                    await this.getAllReviewTimeList(checkAccessModule.getUserIdCurrent(), null)
                }
                if (checkAccessModule.isPm()) {
                    await this.getAllReviewTimeList(checkAccessModule.getUserIdCurrent(), null)
                }
            }
        },

        checkPmLead() {
            if(checkAccessModule.isLead() || checkAccessModule.isPm()){
                return true
            }else{
                return false
            }
        },

        async getAllReviewTimeList(idReviewer = null, idUserCreated = null) {
            this.isLoading = true
            this.totalMapPage = 0
            this.totalItem = 0
            var link = ''
            if (idReviewer == null && idUserCreated == null) {
                link = `StaffReviewTime/GetAllStaffRewviewTimeAsync?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`
            } else if (idUserCreated != null) {
                link = `StaffReviewTime/GetAllStaffRewviewTimeAsync?idUserCreated=${idUserCreated}&pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`
            } else if (idReviewer != null) {
                link = `StaffReviewTime/GetAllStaffRewviewTimeAsync?idReviewer=${idReviewer}&pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`
            }
            await HTTP.get(link)
                .then((res) => {
                    console.log(res.data)
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((ele) => {
                        this.listReview.push(ele)
                    })
                    this.listReview = this.listReview.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                })
                .catch((error) => {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                })
                .finally(() => {
                    this.isLoading = false
                })
        },

        async deleteRow(idStaffReviewTime, idBranch) {
            let API_URL = `StaffReviewTime/DeleteMultiStaffReviewTime/${idStaffReviewTime}/${idBranch}/${checkAccessModule.getUserIdCurrent()}`
            await HTTP.put(API_URL)
                .then(async (res) => {
                    if (res.status == HttpStatus.OK) {
                        this.showSuccess('Xóa thành công!')
                        await this.handlerReload()
                    }
                })
                .catch((error) => {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                })
        },

        async confirmDelete(idStaffReviewTime, idBranch) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn muốn xóa đợt đánh giá này?',
                header: 'Xóa',
                icon: 'pi pi-question-circle',
                rejectLabel: 'Hủy',
                acceptLabel: 'Xóa',
                acceptIcon: 'pi pi-trash',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-danger CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: async () => {
                    await this.deleteRow(idStaffReviewTime, idBranch)
                },
                reject: () => {
                    return
                },
            })
        },

        async openEdit(dataRiviewById) {
            this.dataRiviewById = dataRiviewById
            this.openEditform = true
        },

        closeEdit() {
            this.openEditform = false
        },

        openAdd() {
            this.openAddform = true
        },

        closeAdd() {
            this.openAddform = false
        },

        openDetailt(dataRiviewById) {
            this.dataRiviewById = dataRiviewById
            this.openDetailtform = true
        },

        closeDetailt() {
            this.openDetailtform = false
        },
    },
    async created() {
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '').slice(0, 7)) === true) {
            checkAccessModule.checkPermissionAction(this.$route.path.replace('/', '').slice(8), this.showButton)
            await this.checkPermission()
            await this.getAllPM()
            await this.getAllBranch()
            await this.getAllUser()
            await this.getAllReviewTimeName()
        } else {
            alert('bạn không có quyền')
            this.$router.push('/')
        }
    },
    components: { Export, Add, Edit, View, Delete, AddReviewDiaLog, EditTimeReview, DetailTimeReview },
}
</script>

<style lang="scss" scoped>
::v-deep(.p-dialog .p-dialog-footer) {
    padding: 1rem 0.5rem 0.7rem 1.5rem !important;
    display: flow-root !important;
}

::v-deep(.p-paginator) {
    .p-paginator-current {
        margin-left: auto;
    }
}

::v-deep(.p-progressbar) {
    height: 0.5rem;
    background-color: #d8dadc;

    .p-progressbar-value {
        background-color: #607d8b;
    }
}

::v-deep(.p-datepicker) {
    min-width: 25rem;

    td {
        font-weight: 400;
    }
}

::v-deep(.p-datatable.p-datatable-customers) {
    .p-datatable-header {
        padding: 1rem;
        text-align: left;
        font-size: 1.5rem;
    }

    .p-paginator {
        padding: 1rem;
    }

    .p-datatable-thead > tr > th {
        text-align: left;
    }

    .p-datatable-tbody > tr > td {
        cursor: auto;
    }

    .p-dropdown-label:not(.p-placeholder) {
        text-transform: uppercase;
    }

    .p-datatable-header {
        background-color: #607d8b;
    }

    // them
    .p-input-icon-left {
        float: right;
        margin-left: 1rem;
        display: inline;
    }

    /* .p-inputtext-sm {
            font-size: 0.96rem;
            padding: 25px;
        } */

    .p-inputtext {
        height: 42px;
    }

    .layout-left {
        float: right;
        display: flex;
        justify-content: flex-start;
        align-items: flex-start;
    }
    .p-button.p-button-outlined {
        width: 42px;
        height: 42px;
    }
    .p-multiselect .p-multiselect-label {
        padding: 0.48rem 0.48rem;
    }

    .p-datatable-header {
        background-color: #607d8b;
    }

    .mazin {
        // margin-left: 5px;
        margin-right: 5px;
    }

    .maz {
        margin-right: 5px;
    }
}
/*  .header-container {
        display: flex;
        justify-content: space-between;
        align-items: center;
    }
    .input-text {
        display: flex;
        height: 45px;
    } */
.actions-buttons {
    display: flex;

    .btn-margin {
        margin-right: 5px;
    }
}
</style>
