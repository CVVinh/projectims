<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />

        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">THU CHI</li>
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
                                <Export
                                    class="custom-button-h"
                                    label="Xuất Excel"
                                    @click="exportToExcelFollowRole"
                                    v-if="this.showButton.export"
                                    :disabled="canExport"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    class="p-button-sm custom-button-h"
                                    @click="Openmodal"
                                    v-if="this.showButton.add"
                                    icon="pi pi-plus"
                                    label="Thêm thu chi"
                                ></Button>
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    type="button"
                                    icon="pi pi-filter-slash"
                                    class="custom-reload-h"
                                    @click="reload()"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    type="date"
                                    v-model="filterStartDate"
                                    class="custom-input-h me-2"
                                    style="width: 100%"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    type="date"
                                    v-model="filterEndDate"
                                    class="custom-input-h"
                                    style="width: 100%"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    type="button"
                                    icon="pi pi-filter"
                                    class="custom-reload-h"
                                    @click="btnFilterByDate()"
                                    v-tooltip.top="'Lọc'"
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        :value="paids"
                        :rows="5"
                        :rowHover="true"
                        :loading="loading"
                        responsiveLayout="scroll"
                        filterDisplay="menu"
                        v-model:filters="filters"
                        :globalFilterFields="['customerFullName', 'nameProject']"
                        showGridlines
                        v-model:selection="selectedPaid"
                    >
                        <template #empty> Không tìm thấy dữ liệu. </template>
                        <template #loading>
                            <ProgressSpinner />
                        </template>
                        <Column selectionMode="multiple" headerStyle="width: 3rem"></Column>
                        <Column field="#" header="#" dataType="date">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>

                        <Column field="nameProject" header="Dự án">
                            <template #body="{ data }">
                                {{ data.nameProject }}
                            </template>
                            <template #filter="{ filterModel }">
                                <InputText
                                    type="text"
                                    v-model="filterModel.value"
                                    class="p-column-filter"
                                    placeholder="Nhập tên"
                                />
                            </template>
                        </Column>

                        <Column field="paidPersonName" header="Người chi tiêu">
                            <template #body="{ data }">
                                {{ data.paidPersonName }}
                            </template>
                        </Column>

                        <Column field="personConfirmName" header="Người xác nhận">
                            <template #body="{ data }">
                                {{ data.personConfirmName }}
                            </template>
                        </Column>

                        <Column field="customerFullName" header="Khách hàng">
                            <template #body="{ data }">
                                {{ data.customerFullName }}
                            </template>
                            <template #filter="{ filterModel }">
                                <InputText
                                    type="text"
                                    v-model="filterModel.value"
                                    class="p-column-filter"
                                    placeholder="Nhập tên"
                                />
                            </template>
                        </Column>

                        <Column field="amountPaidName" header="Giá tiền">
                            <template #body="{ data }">
                                {{ data.amountPaidName }}
                            </template>
                        </Column>

                        <Column field="paidReasonName" header="Lý do">
                            <template #body="{ data }">
                                {{ data.paidReasonName }}
                            </template>
                        </Column>

                        <Column sortable field="paidDate" header="Ngày Chấp Nhận">
                            <template #body="{ data }">
                                {{ data.paidDate }}
                            </template>
                        </Column>

                        <Column field="isAccept" header="Trạng thái" style="width: 5rem">
                            <template #body="{ data }">
                                <div
                                    :class="
                                        data.isAccept == 1
                                            ? 'badge bg-success'
                                            : data.isAccept == 2
                                            ? 'badge bg-danger'
                                            : 'badge bg-warning'
                                    "
                                >
                                    {{
                                        data.isAccept == 1
                                            ? 'Đã Thanh Toán'
                                            : data.isAccept == 2
                                            ? 'Từ chối thanh toán'
                                            : 'Chưa thanh toán'
                                    }}
                                </div>
                            </template>
                        </Column>

                        <Column header="Thực thi" style="width: 15rem; text-align: left">
                            <template #body="{ data }">
                                <div class="actions-buttons">
                                    <Button
                                        icon="pi pi-eye"
                                        @click="openModalDetails(data)"
                                        class="p-button p-component btn-margin custom-button-h"
                                        v-tooltip.top="'Xem chi tiết'"
                                    />
                                    <div class="actions-buttons" v-if="data.isAccept === 0 || isAdmin()">
                                        <Button
                                            icon="pi pi-check"
                                            class="p-button p-component p-button-success btn-margin custom-button-h"
                                            @click="paymentConfirmation(data)"
                                            v-if="this.showButton.confirm"
                                            :disabled="checkCanOperation('xacnhan', data)"
                                            v-tooltip.top="'Xác nhận thanh toán'"
                                        />
                                        <Button
                                            icon="pi pi-times"
                                            class="p-button p-component p-button-danger btn-margin custom-button-h"
                                            @click="openModalRefusal(data)"
                                            v-if="this.showButton.refuse"
                                            :disabled="checkCanOperation('tuchoi', data)"
                                            v-tooltip.top="'Từ chối thanh toán'"
                                        />
                                        <div class="actions-buttons" v-if="btnUserActionShow(data)">
                                            <Button
                                                icon="pi pi-pencil"
                                                class="p-button p-component p-button-warning btn-margin custom-button-h"
                                                @click="Openeditmodal(data)"
                                                v-if="this.showButton.update"
                                                :disabled="checkCanOperation('sua', data)"
                                            />
                                            <Button
                                                icon="pi pi-trash"
                                                class="p-button p-component p-button-danger btn-margin custom-button-h"
                                                @click="Delete(data)"
                                                v-if="this.showButton.delete"
                                                :disabled="checkCanOperation('xoa', data)"
                                            />
                                        </div>
                                    </div>
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

            <AddPaid
                :status="openStatus"
                @closemodal="Closemodal()"
                :optionmodule="OptionModule"
                @reloadpage="getData()"
                :customerArr="customerArr"
                :paidReasonArr="paidReasonArr"
                @reloadPaidReason="getAllPaidReason"
                @reloadCustomer="getAllCustomer"
            />

            <EditPaid
                :status="openStatusEdit"
                @closemodal="closeeditmodal()"
                :dataedit="editdata"
                :optionmodule="OptionModule"
                @reloadpage="getData()"
                :customerArr="customerArr"
                :paidReasonArr="paidReasonArr"
            />

            <DetailPaid
                :status="displayImage"
                @closemodal="closeDetails()"
                :dataDetail="detailData"
                @confirmPayment="paymentConfirmation($event)"
            />

            <RefusalPaid
                :status="openStatusRefusal"
                @closemodal="closeRefusalModal()"
                :refusalData="refusalData"
                @reloadpage="getData()"
            />
        </div>
    </LayoutDefaultDynamic>
</template>
<script>
import { GET_LIST_PAID, HTTP, HTTP_LOCAL, GET_PROJECT_BY_ID, GET_USER_NAME_BY_ID, HTTP_API_GITLAB } from '@/http-common'
import Add from '../../components/buttons/Add.vue'
import Delete from '../../components/buttons/Delete.vue'
import Edit from '../../components/buttons/Edit.vue'
import dayjs from 'dayjs'
import { FilterMatchMode, FilterOperator } from 'primevue/api'
import LayoutDefaultDynamic from '../../layouts/LayoutDefault/LayoutDefaultDynamic.vue'
import AddPaid from './addPaid.vue'
import EditPaid from './editPaid.vue'
import DetailPaid from './detailPaid.vue'
import RefusalPaid from './refusalPaid.vue'
import { LocalStorage } from '@/helper/local-storage.helper'
import { UserRoleHelper } from '@/helper/user-role.helper'
import router from '@/router'
import { DateHelper } from '@/helper/date.helper'
import { checkAccessModule } from '@/helper/checkAccessModule'
import Export from '../../components/buttons/Export.vue'
export default {
    data() {
        return {
            openStatus: false,
            openStatusEdit: false,
            openStatusRefusal: false,
            editdata: null,
            detailData: null,
            refusalData: null,
            OptionModule: [],
            customerArr: [],
            paidReasonArr: [],
            paids: [],
            loading: false,
            filters: {
                global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                customerFullName: {
                    operator: FilterOperator.AND,
                    constraints: [{ value: null, matchMode: FilterMatchMode.STARTS_WITH }],
                },
                nameProject: {
                    operator: FilterOperator.AND,
                    constraints: [{ value: null, matchMode: FilterMatchMode.STARTS_WITH }],
                },
            },
            filterStartDate: '',
            filterEndDate: '',
            displayImage: false,
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
            dataExport: [],
            selectedPaid: [],
            canExport: true,
        }
    },
    watch: {
        resultPgae: {
            handler: async function change() {
                if (this.filterStartDate != '' || this.filterEndDate != '') {
                    await this.filterEventEndDate()
                } else {
                    if (checkAccessModule.getListGroup().includes('1')) {
                        // getAPI (Admin)
                        await this.getPaid()
                        await this.getAllProject()
                    } else if (checkAccessModule.getListGroup().includes('2')) {
                        // (Sample)
                        await this.getPaid(checkAccessModule.getUserIdCurrent())
                        await this.getAllProject()
                    } else if (
                        !checkAccessModule.getListGroup().includes('1') &&
                        !checkAccessModule.getListGroup().includes('1')
                    ) {
                        if (checkAccessModule.getListGroup().includes('5')) {
                            // pm
                            await this.getPaidByIdUser(checkAccessModule.getUserIdCurrent())
                            await this.getAllProject()
                        } else {
                            await this.getPaidByIdUser(checkAccessModule.getUserIdCurrent())
                            await this.getAllProject(checkAccessModule.getUserIdCurrent())
                        }
                    }
                }
            },
            deep: true,
        },
    },

    async created() {
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '')) === true) {
            checkAccessModule.checkPermissionAction(this.$route.path.replace('/', ''), this.showButton)
            await this.getData()
            this.loading = false
        } else {
            alert('Bạn không có quyền truy cập module này')
            router.push('/')
        }
    },

    methods: {
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
                if (data.isAccept === 0) {
                    return false
                } else {
                    return true
                }
            }
            if (name === 'xoa') {
                if (data.isAccept === 0 || checkAccessModule.isAdmin()) {
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
                if (data.isAccept === 0) {
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
                if (data.isAccept === 0) {
                    return false
                } else {
                    return true
                }
            }
        },
        Openmodal() {
            this.openStatus = true
        },

        Closemodal() {
            this.openStatus = false
        },

        Openeditmodal(id) {
            this.openStatusEdit = true
            this.editdata = id
        },

        closeeditmodal() {
            this.openStatusEdit = false
        },

        closeRefusalModal() {
            this.openStatusRefusal = false
        },

        openModalDetails(data) {
            this.displayImage = true
            this.detailData = data
        },

        openModalRefusal(data) {
            this.openStatusRefusal = true
            this.refusalData = data
        },

        closeDetails() {
            this.displayImage = false
        },

        resetFieldDate() {
            this.filterStartDate = ''
            this.filterEndDate = ''
        },

        async reload() {
            this.resetFieldDate()
            await this.getData()
        },

        btnUserActionShow(data) {
            if (checkAccessModule.getListGroup().includes('1')) {
                return true
            }
            if (parseInt(data.paidPerson) == parseInt(checkAccessModule.getUserIdCurrent())) {
                return true
            } else {
                return false
            }
        },

        formatDate(date) {
            var dateFormat = new Date(date)
            return dayjs(dateFormat).format('DD/MM/YYYY')
        },

        async getProjects(id) {
            return await HTTP.get(`Project/getProByIdDel/${id}`)
                .then((respone) => respone.data)
                .catch((error) => {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                })
        },
        isAdmin() {
            if (checkAccessModule.getListGroup().includes('1')) {
                return true
            } else {
                return false
            }
        },
        async getUsers(id) {
            return await HTTP.get(GET_USER_NAME_BY_ID(id))
                .then((respone) => respone.data)
                .catch((error) => {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                })
        },

        async getCustomerId(id) {
            return await HTTP.get(`Customer/GetById/${id}`)
                .then((respone) => respone.data._Data)
                .catch((error) => {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                })
        },

        async getPaidReasonId(id) {
            return await HTTP.get(`PaidReason/GetById/${id}`)
                .then((respone) => respone.data._Data)
                .catch((error) => {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                })
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

        async getData() {
            try {
                this.paids = []
                if (checkAccessModule.checkCallAPI(this.$route.path.replace('/', ''))) {
                    if (checkAccessModule.isAdmin()) {
                            await this.getPaid()
                            await this.getAllProject()
                    } else {
                        if (checkAccessModule.isOffice()) {
                            await this.getPaid(checkAccessModule.getUserIdCurrent())
                            await this.getAllProject()
                        }
                    }
                } else {
                    if (checkAccessModule.isPm()) {
                        await this.getPaidByIdUser(checkAccessModule.getUserIdCurrent())
                        await this.getAllProject()
                    } else {
                        await this.getPaidByIdUser(checkAccessModule.getUserIdCurrent())
                        await this.getAllProject(checkAccessModule.getUserIdCurrent())
                    }
                }
                await this.getAllCustomer()
                await this.getAllPaidReason()
                if (this.paids.length > 0) {
                    this.canExport = false
                } else {
                    this.canExport = true
                }
            } catch (error) {
                var message = error.response.data != '' ? error.response.data : error.response.statusText
                this.showResponseApi(error.response.status, message)
                router.push('/')
            }
        },

        async DeletePaid(id) {
            try {
                await HTTP.delete(`Paid/${id}`).then(async (res) => {
                    if (res.status == 200) {
                        this.showSuccess('Xóa thành công!')
                        await this.getData()
                    } else {
                        this.showResponseApi(res.status)
                    }
                })
            } catch (error) {
                var message = error.response.data != '' ? error.response.data : error.response.statusText
                this.showResponseApi(error.response.status, message)
            }
        },

        Delete(data) {
            this.$confirm.require({
                message: `Bạn có chắc muốn xóa thu chi của của khách hàng này?`,
                header: 'Xóa thu chi',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Xóa',
                rejectLabel: 'Hủy',
                acceptIcon: 'pi pi-trash',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-danger CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: async () => {
                    await this.DeletePaid(data.id)
                },
                reject: () => {
                    return
                },
            })
        },

        async getAllCustomer() {
            this.customerArr = []
            await HTTP.get('Customer/getAllCustomer')
                .then((res) => {
                    this.customerArr = res.data._Data
                })
                .catch((error) => {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                })
        },

        async getAllPaidReason() {
            this.paidReasonArr = []
            await HTTP.get(`PaidReason/getAllPaidReason`)
                .then((res) => {
                    this.paidReasonArr = res.data._Data
                })
                .catch((error) => {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                })
        },

        async getAllProject(idUser = null) {
            this.OptionModule = []
            if (idUser == null) {
                await HTTP.get('Project/getAllProjectRunning')
                    .then((res) => {
                        this.OptionModule = res.data
                    })
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            } else {
                if (checkAccessModule.getListGroup().includes('3')) {
                    // lead
                    await HTTP.get(`Project/getAllProjectByLead/${idUser}`)
                        .then((res) => {
                            this.OptionModule = res.data._Data
                        })
                        .catch((error) => {
                            var message = error.response.data != '' ? error.response.data : error.response.statusText
                            this.showResponseApi(error.response.status, message)
                        })
                }
                if (checkAccessModule.getListGroup().includes('4')) {
                    // staff
                    await HTTP.get(`Project/getAllProjectByStaff/${idUser}`)
                        .then((res) => {
                            this.OptionModule = res.data._Data
                        })
                        .catch((error) => {
                            var message = error.response.data != '' ? error.response.data : error.response.statusText
                            this.showResponseApi(error.response.status, message)
                        })
                }
            }
        },
        async getPaidByIdUser(iduser) {
            try {
                this.loading = true
                this.totalMapPage = 0
                this.totalItem = 0
                await HTTP.get(
                    `Paid/GetByUserId/${iduser}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
                )
                    .then((res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        this.paids = res.data._Data
                        if (this.paids.length > 0) {
                            this.canExport = false
                        } else {
                            this.canExport = true
                        }
                    })
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
                //await this.getWithName()
            } catch (err) {
                console.log(err)
            } finally {
                this.loading = false
            }
        },

        async getPaid(idSample = null) {
            try {
                this.loading = true
                this.paids = []

                if (idSample == null) {
                    await HTTP.get(
                        `Paid/GetAllPaid?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
                    )
                        .then((respone) => {
                            this.totalMapPage = respone.data._totalPages
                            this.totalItem = respone.data._totalItems
                            this.itemIndex = respone.data._itemIndex
                            this.paids = respone.data._Data.filter(
                                (obj, index, self) => index === self.findIndex((t) => t.id === obj.id),
                            )
                        })
                        .catch((error) => {
                            var message = error.response.data != '' ? error.response.data : error.response.statusText
                            this.showResponseApi(error.response.status, message)
                        })
                } else {
                    await HTTP.get(
                        `Paid/GetByIdSample/${idSample}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
                    )
                        .then((respone) => {
                            this.totalMapPage = respone.data._totalPages
                            this.totalItem = respone.data._totalItems
                            this.itemIndex = respone.data._itemIndex
                            this.paids = respone.data._Data.filter(
                                (obj, index, self) => index === self.findIndex((t) => t.id === obj.id),
                            )
                        })
                        .catch((error) => {
                            var message = error.response.data != '' ? error.response.data : error.response.statusText
                            this.showResponseApi(error.response.status, message)
                        })
                }
                if (this.paids.length > 0) {
                    this.canExport = false
                } else {
                    this.canExport = true
                }
                //await this.getWithName()
            } catch (error) {
                console.log(error)
            } finally {
                this.loading = false
            }
        },

        async callApiSearch(objSearch) {
            this.loading = true
            this.paids = []
            await HTTP.post(
                `Paid/SearchPaidByDay?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
                objSearch,
            )
                .then(async (respone) => {
                    this.totalMapPage = respone.data._totalPages
                    this.totalItem = respone.data._totalItems
                    this.itemIndex = respone.data._itemIndex
                    this.paids = respone.data._Data
                    if (this.paids.length == 0) {
                        this.showInfo('Không tìm thấy dữ liệu!')
                    } else {
                        //await this.getWithName()
                    }
                })
                .catch((error) => {
                    //this.showError(error.response.data._Message);
                    console.log(error)
                })
                .finally(() => {
                    this.loading = false
                })
        },

        checkValidateDay() {
            if (this.filterStartDate > this.filterEndDate) {
                this.showInfo('Ngày kết thúc phải lớn hơn ngày bắt đầu!')
                return false
            }
            return true
        },

        checkStartDateEmpty() {
            if (this.filterStartDate != '') {
                return true
            }
            return false
        },

        checkEndDateEmpty() {
            if (this.filterEndDate != '') {
                return true
            }
            return false
        },

        async callApiFilterDay(startDate = null, endDate = null) {
            // getAPI (Sample) || (Admin)
            if (checkAccessModule.getListGroup().includes('2') || checkAccessModule.getListGroup().includes('1')) {
                await this.callApiSearch({
                    startDate: startDate,
                    endDate: endDate,
                })
            }
            // getAPI tất cả role còn lại
            if (!checkAccessModule.getListGroup().includes('2') && !checkAccessModule.getListGroup().includes('1')) {
                await this.callApiSearch({
                    id: checkAccessModule.getUserIdCurrent(),
                    startDate: startDate,
                    endDate: endDate,
                })
            }
        },

        async filterEventStartDate() {
            if (this.checkStartDateEmpty() && this.checkEndDateEmpty() == false) {
                await this.callApiFilterDay(this.filterStartDate, null)
                return
            }
            if (this.checkStartDateEmpty() && this.checkEndDateEmpty()) {
                if (this.checkValidateDay()) {
                    await this.callApiFilterDay(this.filterStartDate, this.filterEndDate)
                } else {
                    this.resetFieldDate()
                }
            }
        },

        async filterEventEndDate() {
            if (this.checkStartDateEmpty() == false) {
                this.showInfo('Nhập ngày bắt đầu trước!')
                this.resetFieldDate()
                return
            }
            await this.filterEventStartDate()
        },

        async btnFilterByDate() {
            if (this.checkStartDateEmpty() == false && this.checkEndDateEmpty() == false) {
                this.showInfo('Ngày lọc rỗng!')
            } else {
                await this.filterEventEndDate()
            }
        },

        async CallApiPaymentConfirm(idPaid) {
            await HTTP.put(`Paid/acceptPayment/${idPaid}`, { PersonConfirm: checkAccessModule.getUserIdCurrent() })
                .then(async (res) => {
                    if (res.status == 200) {
                        this.showSuccess('Thanh toán thành công!')
                        await this.getData()
                    } else {
                        this.showError('Lỗi! cập nhật thanh toán!')
                    }
                })
                .catch((error) => {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                })
        },

        exportToExcelFollowRole() {
            this.dataExport = []
            if (this.selectedPaid.length > 0) {
                this.paids.map((ele) => {
                    this.selectedPaid.map((element) => {
                        if (ele.id === element.id) {
                            const object = {
                                nameProject: ele.nameProject,
                                paidPersonName: ele.paidPersonName,
                                personConfirmName: ele.personConfirmName,
                                customerFullName: ele.customerFullName,
                                amountPaid: ele.amountPaid,
                                paidReasonName: ele.paidReasonName,
                                paidDate: ele.paidDate,
                                isAccept:
                                    ele.isAccept === 0
                                        ? 'Chưa xác nhận'
                                        : ele.isAccept === 1
                                        ? 'Đã thanh toán'
                                        : 'Đã từ chối',
                            }
                            this.dataExport.push(object)
                        }
                    })
                })

                import('../../plugins/Export2Excel.js').then((excel) => {
                    const OBJ = this.dataExport
                    const Header = [
                        'Dự án',
                        'Người chi tiêu',
                        'Người xác nhận',
                        'Khách hàng',
                        'Giá tiền',
                        'Lý do',
                        'Ngày chấp nhận',
                        'Trạng thái',
                    ]

                    const Field = [
                        'nameProject',
                        'paidPersonName',
                        'personConfirmName',
                        'customerFullName',
                        'amountPaid',
                        'paidReasonName',
                        'paidDate',
                        'isAccept',
                    ]

                    const Data = this.FormatJSon(Field, OBJ)
                    excel.export_json_to_excel({
                        header: Header,
                        data: Data,
                        sheetName: 'Danh sách thu chi',
                        filename: 'Danh sách thu chi',
                        autoWidth: true,
                        bookType: 'xlsx',
                    })
                })
            } else {
                this.dataExport = []
                this.paids.map((ele) => {
                    const object = {
                        nameProject: ele.nameProject,
                        paidPersonName: ele.paidPersonName,
                        personConfirmName: ele.personConfirmName,
                        customerFullName: ele.customerFullName,
                        amountPaid: ele.amountPaid,
                        paidReasonName: ele.paidReasonName,
                        paidDate: ele.paidDate,
                        isAccept:
                            ele.isAccept === 0 ? 'Chưa xác nhận' : ele.isAccept === 1 ? 'Đã thanh toán' : 'Đã từ chối',
                    }
                    this.dataExport.push(object)
                })

                import('../../plugins/Export2Excel.js').then((excel) => {
                    const OBJ = this.dataExport
                    const Header = [
                        'Dự án',
                        'Người chi tiêu',
                        'Người xác nhận',
                        'Khách hàng',
                        'Giá tiền',
                        'Lý do',
                        'Ngày chấp nhận',
                        'Trạng thái',
                    ]

                    const Field = [
                        'nameProject',
                        'paidPersonName',
                        'personConfirmName',
                        'customerFullName',
                        'amountPaid',
                        'paidReasonName',
                        'paidDate',
                        'isAccept',
                    ]

                    const Data = this.FormatJSon(Field, OBJ)
                    excel.export_json_to_excel({
                        header: Header,
                        data: Data,
                        sheetName: 'Danh sách thu chi',
                        filename: 'Danh sách thu chi',
                        autoWidth: true,
                        bookType: 'xlsx',
                    })
                })
            }

            //Thừa api `OTs/exportExcelFollowRole/${month}/${year}/${idProject}/${this.token.listGroup[0]}/${checkAccessModule.getUserIdCurrent()
        },
        FormatJSon(FilterData, JsonData) {
            return JsonData.map((v) =>
                FilterData.map((j) => {
                    return v[j]
                }),
            )
        },

        paymentConfirmation(item) {
            this.$confirm.require({
                message: `Bạn có chắc muốn thanh toán cho khách hàng này?`,
                header: 'Xác nhận thanh toán',
                icon: 'pi pi-info-circle',
                acceptLabel: 'Xác nhận',
                acceptIcon: 'pi pi-check',
                rejectLabel: 'Hủy',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-primary CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined CustomButtonPrimeVue',
                accept: () => {
                    this.displayImage = false
                    this.CallApiPaymentConfirm(item.id)
                },
                reject: () => {
                    return
                },
            })
        },
    },

    components: { LayoutDefaultDynamic, Add, Edit, Delete, AddPaid, EditPaid, DetailPaid, RefusalPaid, Export },
}
</script>
<style lang="scss">
.header-container {
    display: flex;
    justify-content: space-between;
    align-items: center;
    // margin-bottom: 1rem;
    // height: 40px;
}

/*  .p-datatable.p-datatable-gridlines .p-datatable-header {
        background-color: #607d8b;
        color: white;
    } */

.p-dialog .p-dialog-footer {
    padding: 1rem 0.5rem 0.7rem 1.5rem !important;
    display: flow-root !important;
}

.p-dialog-content img {
    width: 100%;
}

.content_box {
    box-shadow: -3px 3px 5px -3px #888888, 4px 5px 3px -4px #888888, 4px 5px 2px -5px #888888 inset;
    padding: 10px;
    border-radius: 10px;
}

.actions-buttons {
    display: flex;
    .btn-margin {
        margin-right: 5px;
    }
}
</style>
