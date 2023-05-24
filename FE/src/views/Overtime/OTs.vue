<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">TĂNG CA</li>
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
                                    @click="exportToExcelFollowRole()"
                                    :disabled="canExport"
                                    v-if="this.showButtonNew.export"
                                    class="custom-button-h"
                                    label="Xuất Excel"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    label="Thêm tăng ca"
                                    icon="pi pi-plus"
                                    @click="openFormAddEdit(null)"
                                    class="p-button-sm custom-button-h"
                                    v-if="this.showButtonNew.add"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    label="Phê duyệt"
                                    icon="pi pi-check-square"
                                    @click="acceptMulti()"
                                    class="p-button-sm custom-button-h"
                                    v-if="this.showButtonNew.confirmMulti"
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
                                    @click="clearFilter()"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Dropdown
                                    class="custom-input-h"
                                    v-model="selectedMonth"
                                    :options="selectMonth"
                                    optionLabel="label"
                                    placeholder="Chọn tháng"
                                    :filter="true"
                                    filterPlaceholder="Tìm tháng"
                                    @change="Fillter()"
                                    style="width: 100%"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Dropdown
                                    class="custom-input-h"
                                    v-model="selectedProject"
                                    :options="selectProject"
                                    optionLabel="name"
                                    placeholder="Chọn dự án"
                                    :filter="true"
                                    filterPlaceholder="Tìm dự án"
                                    @change="Fillter()"
                                    style="width: 100%"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <MultiSelect
                                    class="custom-input-h"
                                    :modelValue="selectedColumns"
                                    :options="columns"
                                    optionLabel="header"
                                    @update:modelValue="onToggle"
                                    placeholder="Chọn "
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        :value="data"
                        showGridlines
                        ref="dt"
                        class="p-datatable-customers"
                        sortField="x.dateCreate"
                        :sortOrder="-1"
                        :rows="5"
                        :loading="loading"
                        dataKey="x.id"
                        :rowHover="true"
                        v-model:filters="filters"
                        v-model:selection="selectedOT"
                        filterDisplay="menu"
                        :globalFilterFields="[
                            'x.id',
                            'x.date',
                            'x.start',
                            'x.end',
                            'x.realTime',
                            'x.status',
                            'x.description',
                            'x.leadCreate',
                            'name',
                            'nameLead',
                            'nameUser',
                            'nameUserUpdate',
                            'x.dateCreate',
                            'x.updateUser',
                            'x.dateUpdate',
                            'x.note',
                            'x.user',
                            'x.idProject',
                        ]"
                        responsiveLayout="scroll"
                    >
                        <!-- Header -->

                        <template #empty> Không tìm thấy. </template>

                        <!-- Body -->
                        <Column selectionMode="multiple" headerStyle="width: 2rem"></Column>
                        <Column header="#">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="x.user" header="Nhân viên tăng ca" sortable style="min-width: 4rem">
                            <template #body="{ data }">
                                {{ data.nameUser }}
                            </template>
                        </Column>
                        <Column field="x.date" header="Ngày tăng ca" sortable dataType="date" style="min-width: 7.5rem">
                            <template #body="{ data }">
                                {{ getFormattedDate(new Date(data.x.date)) }}
                            </template>
                        </Column>
                        <Column field="x.realTime" header="Thời gian tăng ca" sortable style="min-width: 13rem">
                            <template #body="{ data }">
                                {{
                                    data.x.realTime.charAt(0) !== '0' || data.x.realTime.charAt(1) !== '0'
                                        ? data.x.realTime +
                                          ' giờ ' +
                                          '(' +
                                          data.x.start +
                                          ' giờ ' +
                                          ' - ' +
                                          data.x.end +
                                          ' giờ' +
                                          ')'
                                        : data.x.realTime +
                                          ' phút ' +
                                          '(' +
                                          data.x.start +
                                          ' giờ ' +
                                          ' - ' +
                                          data.x.end +
                                          ' giờ' +
                                          ')'
                                }}
                            </template>
                        </Column>
                        <Column field="x.leadCreate" header="Leader" sortable style="min-width: 6rem">
                            <template #body="{ data }">
                                {{ data.nameLead }}
                            </template>
                        </Column>
                        <!-- <Column field="updateUser" header="Approved by" sortable style="min-width: 10rem">
                                <template #body="{ data }">
                                    {{ data.x.updateUser }}
                                </template>
                            </Column> -->
                        <Column field="x.idProject" header="Dự án" sortable style="min-width: 5rem; max-width: 15rem">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column
                            sortable
                            v-for="(col, index) of selectedColumns"
                            :field="col.field"
                            :header="col.header"
                            :key="col.field + '_' + index"
                        >
                            <template #body="{ data }" v-if="col.field === 'dateUpdate'">
                                {{ data.dateUpdate !== null ? getFormattedDate(new Date(data.dateUpdate)) : null }}
                            </template>
                        </Column>
                        <Column
                            field="x.status"
                            header="Trạng thái"
                            sortable
                            :filterMenuStyle="{ width: '14rem' }"
                            style="min-width: 6rem"
                        >
                            <template #body="{ data }">
                                <span :class="'badge ' + color(data.x.status)">{{ statuses[data.x.status].text }}</span>
                            </template>
                        </Column>
                        <Column field="" header="Thực thi" v-if="!checkIsGroup('office')">
                            <template #body="{ data }">
                                <div v-if="checkShowButtonChangeData(data)" class="actions-buttons">
                                    <!--  VIEW  -->
                                    <Button
                                        icon="pi pi-eye"
                                        @click="OpenDetailOT(data)"
                                        class="p-button-sm custom-button-update me-2"
                                        v-tooltip.top="'Xem chi tiết'"
                                    />
                                    <!-- Edit -->
                                    <Edit
                                        @click="openFormAddEdit(data.x.id)"
                                        class="p-button-warning custom-button-update me-2"
                                        v-if="this.showButtonNew.update && checkCanOperation('sua', data)"
                                    >
                                    </Edit>
                                    <!--  Delete -->
                                    <Delete
                                        @click="confirmDelete(data.x.id)"
                                        class="custom-button-update me-2"
                                        v-if="this.showButtonNew.delete && checkCanOperation('xoa', data)"
                                    ></Delete>
                                    <!-- confirm -->
                                    <Edit
                                        icon="pi pi-check"
                                        @click="openConfirm(true, data.x.id, data.x.leadCreate)"
                                        class="p-button-success custom-button-update me-2"
                                        v-if="this.showButtonNew.confirm && checkCanOperation('xacnhan', data)"
                                        v-tooltip.top="'Duyệt OT'"
                                    />
                                    <!-- Refuse   -->
                                    <Delete
                                        class="me-2 custom-button-update"
                                        icon="pi pi-times"
                                        @click="accept(false, data.x.id, data.x.leadCreate)"
                                        v-if="this.showButtonNew.refuse && checkCanOperation('tuchoi', data)"
                                        v-tooltip="'Không duyệt OT'"
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
            :visible="displayDialog1"
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
                <Button label="Lưu" icon="pi pi-check" @click="submit" autofocus />
            </template>
        </Dialog>
        <Dialog
            header="Vui lòng nhập lý do từ chối tăng ca !"
            :visible="displayDialog2"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            :modal="true"
        >
            <Textarea v-model="reason" style="margin: auto; width: 100%; height: 100%"></Textarea>
            <p v-if="entered" class="p-error">Lý do bắt buộc nhập!</p>
            <template #footer>
                <Button
                    label="Hủy"
                    class="p-button-secondary p-button-outlined CustomButtonPrimeVue"
                    autofocus
                    icon="pi pi-times"
                    @click="closeBasic"
                />
                <Button
                    label="Lưu"
                    class="p-button-danger CustomButtonPrimeVue"
                    autofocus
                    icon="pi pi-trash"
                    @click="onSubmit(data.leadCreate)"
                />
            </template>
        </Dialog>
        <AddOTsDialog
            :display="this.displayFormAddEdit"
            @close="closeFormAddEdit"
            @reloadData="getAllOT"
            :idproject="this.idproject"
            :AddOrEdit="this.edit"
        />
        <DetailOT
            :show="DetailOT"
            @open="OpenDetailOT"
            @close="CloseDetailOT"
            :OTS="ots"
            :isPm="isPM"
            @reloadAPI="getAllOT"
            @alert="showSuccess"
            :displayDialog2="displayDialog2"
            :id="id"
            :lead="lead"
            @OpenFormRefuse="OpenFormRefuse"
        />
    </LayoutDefaultDynamic>
</template>

<script>
import Edit from '@/components/buttons/Edit.vue'
import Export from '@/components/buttons/Export.vue'
import router from '@/router/index'
import { LocalStorage } from '@/helper/local-storage.helper'
import { HTTP } from '@/http-common.js'
import { FilterMatchMode } from 'primevue/api'
import Delete from '@/components/buttons/Delete.vue'
import jwtDecode from 'jwt-decode'
import { UserRoleHelper } from '@/helper/user-role.helper'
import { HttpStatus } from '@/config/app.config'
import DetailOT from './DetailOT.vue'
import { cloneVNode } from '@vue/runtime-core'
import storeRole from '@/stores/role'
import AddOTsDialog from './AddOTsDialog.vue'
import { checkAccessModule } from '@/helper/checkAccessModule'
export default {
    name: 'ots',
    data() {
        return {
            selectedOT: null,
            data: null,
            filters: {
                global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                status: { value: null, matchMode: FilterMatchMode.EQUALS },
            },
            selectedColumns: null,
            columns: null,
            project: [],
            proName: [],
            proPM: [],
            user: [],
            username: [],
            cantAdd: false,
            token: null,
            rolePM: true,
            displayDialog1: false,
            displayDialog2: false,
            num: 5,
            timeout: null,
            canAccept: true,
            entered: false,
            reason: '',
            id: '',
            lead: '',
            statuses: [
                { num: 0, text: 'Đang chờ' },
                { num: 1, text: 'Đã duyệt' },
                { num: 2, text: 'Đã từ chối' },
                { num: 3, text: 'Đã xóa' },
            ],
            userInfo: [],
            selectMonth: [],
            selectedMonth: null,
            selectProject: [],
            selectedProject: null,
            //axios: 'http://api.imsdemo.tk/api/',
            axios: import.meta.env.VITE_VUE_API_URL,
            DetailOT: false,
            ots: 0,
            isPM: false,
            showButton: {},
            showButtonNew: {},
            loading: true,
            displayFormAddEdit: false,
            idproject: null,
            dataExport: [],
            resultPgae: {
                pageSize: 10,
                pageNumber: 1,
            },
            totalItem: 0,
            totalMapPage: 0,
            itemIndex: 0,
            canExport: true,
        }
    },
    watch: {
        resultPgae: {
            handler: async function change() {
                await this.getAllOT()
            },
            deep: true,
        },
    },

    async created() {
        this.token = LocalStorage.jwtDecodeToken()
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '')) === true) {
            checkAccessModule.checkPermissionAction(this.$route.path.replace('/', ''), this.showButtonNew)
            await this.getAllOT()
            this.columns = [
                { field: 'x.dateUpdate', header: 'Ngày phê duyệt' },
                { field: 'x.dateCreate', header: 'Ngày tạo' },
                { field: 'x.description', header: 'Mô tả' },
                { field: 'pmName', header: 'PM' },
                { field: 'x.note', header: 'Lý do từ chối' },
            ]
            this.getMonthFrom()
            this.loading = false
        } else {
            this.countTime()
            this.displayDialog1 = true
        }
    },
    // async mounted() {

    // },
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
                if (data.x.status === 0) return true
                else return false
            }
            if (name === 'xoa') {
                if (
                    (data.x.status === 0 && data.x.leadCreate === Number(checkAccessModule.getUserIdCurrent())) ||
                    this.checkIsGroup('admin')
                )
                    return true
                else return false
            }
            if (name === 'xoanhieu') {
            }
            if (name === 'xuatfile') {
            }
            if (name === 'xacnhan') {
                if (data.x.status === 0) {
                    if (checkAccessModule.isPm()) {
                        if (data.pm === Number(checkAccessModule.getUserIdCurrent()) || checkAccessModule.isAdmin()) {
                            return true
                        } else {
                            return false
                        }
                    } else {
                        return true
                    }
                } else {
                    return false
                }
            }
            if (name === 'xacnhannhieu') {
            }
            if (name === 'themthanhvien') {
            }
            if (name === 'tuchoi') {
                if (data.x.status === 0) {
                    if (checkAccessModule.isPm()) {
                        if (data.pm === Number(checkAccessModule.getUserIdCurrent()) || checkAccessModule.isAdmin()) {
                            return true
                        } else {
                            return false
                        }
                    } else {
                        return true
                    }
                } else {
                    return false
                }
            }
        },
        checkShowButton() {
            if (checkAccessModule.isLead()) {
                return false
            } else {
                return true
            }
        },
        checkShowButtonChangeData(data) {
            if (checkAccessModule.isAdmin()) {
                return true
            } else {
                if (checkAccessModule.isPm()) {
                    return true
                } else {
                    if (
                        checkAccessModule.isLead() &&
                        data.x.leadCreate === Number(checkAccessModule.getUserIdCurrent())
                    ) {
                        return true
                    } else {
                        return false
                    }
                }
            }
        },
        getIdUser() {
            return checkAccessModule.getUserIdCurrent()
        },
        isAdmin() {
            if (checkAccessModule.getListGroup().includes('1')) {
                return true
            } else {
                return false
            }
        },
        // GET OTS BY ROLE PM
        getOTsByPM(idPM) {
            this.data = []
            HTTP.get(
                `OTs/getOTsByidPM/${idPM}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    // this.data = res.data
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.data = res.data._Data
                })
                .catch((err) => console.log(err))
        },
        // GET OTS BY ROLE LEAD
        getOTsByLead(idLEAD) {
            HTTP.get(
                `OTs/GetAllOTsByLead/${idLEAD}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((ele) => {
                        this.data.push(ele)
                    })
                    // res.data.map((ele) => {
                    //     this.data.push(ele)
                    // })
                    this.data = this.data.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.x.id === obj.x.id)
                    })
                    if (res.data._Data.length > 0) {
                        this.canExport = false
                    }
                })
                .catch((err) => console.log(err))
        },
        // GET OTS BY ROLE STAFF
        getOTsByStaff(idSTAFF) {
            HTTP.get(
                `OTs/GetAllOTsByStaff/${idSTAFF}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((ele) => {
                        this.data.push(ele)
                    })
                    this.data = this.data.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.x.id === obj.x.id)
                    })
                    // res.data.map((ele) => {
                    //     this.data.push(ele)
                    // })
                    if (res.data._Data.length > 0) {
                        this.canExport = false
                    }
                })
                .catch((err) => console.log(err))
        },
        // GET OTS BY ROLE SAMPLE
        getOTsByAdminPm() {
            this.data = []
            HTTP.get(`OTs/GetAllOTs?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.data = res.data._Data
                    if (res.data._Data.length > 0) {
                        this.canExport = false
                    }
                })
                .catch((err) => console.log(err))
        },
        getOTsBySample() {
            this.data = []
            HTTP.get(
                `OTs/getOTBySample?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((ele) => {
                        this.data.push(ele)
                    })
                    this.data = this.data.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.x.id === obj.x.id)
                    })
                    if (res.data._Data.length > 0) {
                        this.canExport = false
                    }
                })
                .catch((err) => console.log(err))
        },
        reloadApi() {
            if (checkAccessModule.isAdmin() || checkAccessModule.isPm()) {
                this.getAllOT()
            } else {
                this.getOTsByLead(checkAccessModule.getUserIdCurrent())
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
        acceptMulti() {
            let bool = true
            if (this.selectedOT == null) {
                this.showWarn('Chọn một mục để phê duyệt!')
                return
            }
            this.selectedOT.forEach((element) => {
                if (element.x.status != 0) {
                    bool = false
                }
            })
            if (bool) {
                this.selectedOT.forEach((element) => {
                    this.accept(true, element.x.id, element.x.leadCreate)
                })
            } else this.showWarn('Không thể phê duyệt mục không có trạng thái đang chờ!')
            this.selectedOT = []
        },
        countTime() {
            if (this.num == 0) {
                this.submit()
                return
            }
            this.num = this.num - 1
            this.timeout = setTimeout(() => this.countTime(), 1000)
        },
        submit() {
            clearTimeout(this.timeout)
            router.push('/')
        },
        onToggle(value) {
            this.selectedColumns = this.columns.filter((col) => value.includes(col))
        },
        formatDate(value) {
            return new Date(value).toLocaleDateString('en-CA', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
            })
        },
        async getAllOT() {
            this.data = []
            this.selectProject = []
            if (checkAccessModule.checkGetData(this.$route.path.replace('/', ''))) {
                await this.getOTsByAdminPm()
                await this.getProject()
            } else {
                if (checkAccessModule.isOffice()) {
                    await this.getOTsBySample()
                    await this.getProject()
                }
                if (checkAccessModule.isLead()) {
                    await this.getOTsByLead(checkAccessModule.getUserIdCurrent())
                    await this.getProjectLead()
                }
                if (checkAccessModule.isStaff()) {
                    await this.getOTsByStaff(checkAccessModule.getUserIdCurrent())
                    await this.getProjectStaff()
                }
            }

            if (this.data.length > 0) {
                this.canExport = false
            } else {
                this.canExport = true
            }
            this.loading = false
        },
        async getUserInfo() {
            HTTP.get('Users/getInfo')
                .then((res) => {
                    this.userInfo = res.data
                })
                .catch((err) => console.log(err))
        },
        deleteData(id, token) {
            HTTP.put('OTs/deleteOT?' + 'idOT=' + id + '&PM=' + token)
                .then((res) => {
                    if (res.status == 200) {
                        this.showSuccess('Xóa thẻ OTs thành công.')
                    }
                })
                .catch((err) => {
                    this.showWarn('Bạn không có quyền thực hiện thao tác xóa OT.')
                })
        },
        color(status) {
            if (status == 0) return 'bg-warning'
            if (status == 1) return 'bg-success'
            if (status == 2) return 'bg-danger'
            return 'bg-danger'
        },
        add() {
            router.push('/ots/addOT')
        },
        confirmDelete(id) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn muốn xóa tăng ca này?',
                header: 'Xóa',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Xóa',
                rejectLabel: 'Hủy',
                acceptIcon: 'pi pi-trash',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-danger CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: () => {
                    this.deleteData(id, checkAccessModule.getUserIdCurrent())
                    setTimeout(() => {
                        this.getAllOT()
                    }, 500)
                },
                reject: () => {
                    return
                },
            })
        },
        showSuccess(err) {
            this.$toast.add({
                severity: 'success',
                summary: 'Thành công',
                detail: err,
                life: 3000,
            })
        },

        exportToExcelFollowRole() {
            this.dataExport = []
            if (this.selectedOT !== null) {
                this.data.map((ele) => {
                    this.selectedOT.map((element) => {
                        if (ele.x.id === element.x.id) {
                            const object = {
                                user: ele.usercode,
                                nameUser: ele.nameUser,
                                date: this.getFormattedDate(new Date(ele.x.date)),
                                start: ele.x.start,
                                end: ele.x.end,
                                name: ele.name,
                                nameLead: ele.nameLead,
                                realTime: ele.x.realTime,
                                dateUpdate: this.getFormattedDate(new Date(ele.x.dateUpdate)),
                                dateCreate: this.getFormattedDate(new Date(ele.x.dateCreate)),
                                status:
                                    ele.x.status === 1 ? 'Đã duyệt' : ele.x.status === 0 ? 'Chưa duyệt' : 'Đã từ chối',
                                description: ele.x.description,
                            }
                            this.dataExport.push(object)
                        }
                    })
                })

                import('../../plugins/Export2Excel.js').then((excel) => {
                    const OBJ = this.dataExport
                    const Header = [
                        'Mã nhân viên',
                        'Tên nhân viên',
                        'Ngày tăng ca',
                        'Giờ bắt đầu',
                        'Giờ kết thúc',
                        'Thời gian thực tế',
                        'Mô tả',
                        'Dự án',
                        'Người quản lý',
                        'Ngày phê duyệt',
                        'Ngày tạo',
                        'Trạng thái',
                    ]

                    const Field = [
                        'user',
                        'nameUser',
                        'date',
                        'start',
                        'end',
                        'realTime',
                        'description',
                        'name',
                        'nameLead',
                        'dateUpdate',
                        'dateCreate',
                        'status',
                    ]

                    const Data = this.FormatJSon(Field, OBJ)
                    excel.export_json_to_excel({
                        header: Header,
                        data: Data,
                        sheetName: 'Danh sách tăng ca',
                        filename: 'Danh sách tăng ca',
                        autoWidth: true,
                        bookType: 'xlsx',
                    })
                })
            } else {
                this.dataExport = []
                this.data.map((ele) => {
                    const object = {
                        user: ele.usercode,
                        nameUser: ele.nameUser,
                        date: this.getFormattedDate(new Date(ele.x.date)),
                        start: ele.x.start,
                        end: ele.x.end,
                        name: ele.name,
                        nameLead: ele.nameLead,
                        realTime: ele.x.realTime,
                        dateUpdate: this.getFormattedDate(new Date(ele.x.dateUpdate)),
                        dateCreate: this.getFormattedDate(new Date(ele.x.dateCreate)),
                        status: ele.x.status === 1 ? 'Đã duyệt' : ele.x.status === 0 ? 'Chưa duyệt' : 'Đã từ chối',
                        description: ele.x.description,
                    }
                    this.dataExport.push(object)
                })

                import('../../plugins/Export2Excel.js').then((excel) => {
                    const OBJ = this.dataExport
                    const Header = [
                        'Mã nhân viên',
                        'Tên nhân viên',
                        'Ngày tăng ca',
                        'Giờ bắt đầu',
                        'Giờ kết thúc',
                        'Thời gian thực tế',
                        'Mô tả',
                        'Dự án',
                        'Người quản lý',
                        'Ngày phê duyệt',
                        'Ngày tạo',
                        'Trạng thái',
                    ]

                    const Field = [
                        'user',
                        'nameUser',
                        'date',
                        'start',
                        'end',
                        'realTime',
                        'description',
                        'name',
                        'nameLead',
                        'dateUpdate',
                        'dateCreate',
                        'status',
                    ]

                    const Data = this.FormatJSon(Field, OBJ)
                    excel.export_json_to_excel({
                        header: Header,
                        data: Data,
                        sheetName: 'Danh sách tăng ca',
                        filename: 'Danh sách tăng ca',
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
        exportToExcel() {
            var month = 0
            var year = 0
            var idProject = 0
            if (this.selectedMonth != null) {
                month = this.selectedMonth.month
                year = this.selectedMonth.year
            }
            if (this.selectedProject != null) idProject = this.selectedProject.code
            HTTP.get('OTs/exportExcel/month=' + month + '&year=' + year + '&idProject=' + idProject)
                .then((res) => {
                    if (res.status == 200) {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Xuất file excel thành công!',
                            life: 3000,
                        })
                        window.location = res.data
                    }
                })
                .catch((err) => {
                    this.showWarn('Bạn không có quyền thực hiện thao tác xuất file excel!')
                })
        },
        accept(accepted, id, lead) {
            this.PM = checkAccessModule.getUserIdCurrent()
            if (accepted) {
                this.status = 1
                HTTP.put('OTs/acceptOT', { id: id, status: this.status, pm: checkAccessModule.getUserIdCurrent() })
                    .then((res) => {
                        this.showSuccess('Xét duyệt thành công')
                    })
                    .catch((err) => {
                        if (err.response.status === 403) {
                            this.showWarn('Bạn không có quyền thực thi thao tác này')
                        } else {
                            this.showWarn('Có lỗi khi xét duyệt')
                        }
                        console.log(err)
                    })
                setTimeout(() => {
                    this.getAllOT()
                }, 500)
            } else {
                this.displayDialog2 = true
                this.id = id
                this.lead = lead
            }
        },
        closeBasic() {
            this.displayDialog2 = false
            this.entered = false
        },
        onSubmit() {
            if (this.reason != null && this.reason != '') {
                HTTP.put('OTs/acceptOT', { id: this.id, status: 2, PM: this.PM, note: this.reason }).then(
                    async (res) => {
                        if (res.status == 200) {
                            this.closeBasic()
                            this.showSuccess()
                            await this.getAllOT()
                        }
                    },
                )
            } else this.entered = true
        },
        clearFilter() {
            this.filters = {
                global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                status: { value: null, matchMode: FilterMatchMode.EQUALS },
            }
            this.getAllOT()
            this.selectedMonth = null
            this.selectedProject = null
        },
        showWarn(message) {
            this.$toast.add({ severity: 'warn', summary: 'Cảnh báo ', detail: message, life: 5000 })
        },
        getMonthFrom() {
            const now = new Date()
            var code = 1
            for (var y = 2020; y <= now.getFullYear(); y++) {
                for (var m = 1; m <= 12; m++) {
                    if (y == now.getFullYear() && m > now.getMonth() + 1) {
                        break
                    }
                    this.selectMonth.push({ id: code, month: m, year: y, label: m + '/' + y })
                    code++
                }
            }
        },

        filterByStaff(month, year, idProject) {
            HTTP.get(
                `OTs/filterByStaff/${month}/${year}/${idProject}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${
                    this.resultPgae.pageSize
                }&iduser=${checkAccessModule.getUserIdCurrent()}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((ele) => {
                        this.data.push(ele)
                    })
                    this.data = this.data.filter((element, index, self) => {
                        return (
                            index ===
                            self.findIndex((obj) => {
                                return JSON.stringify(obj) === JSON.stringify(element)
                            })
                        )
                    })
                })
                .catch((err) => {
                    console.log(err)
                })
        },

        filterByLead(month, year, idProject) {
            console.log(month + ' ' + year + ' ' + idProject)
            HTTP.get(
                `OTs/filterByLead/${month}/${year}/${idProject}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${
                    this.resultPgae.pageSize
                }&iduser=${checkAccessModule.getUserIdCurrent()}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((ele) => {
                        this.data.push(ele)
                    })
                    this.data = this.data.filter((element, index, self) => {
                        return (
                            index ===
                            self.findIndex((obj) => {
                                return JSON.stringify(obj) === JSON.stringify(element)
                            })
                        )
                    })
                })
                .catch((err) => {
                    console.log(err)
                })
        },

        filterByOffice(month, year, idProject) {
            HTTP.get(
                `OTs/filterByOffice/${month}/${year}/${idProject}?pageIndex=${
                    this.resultPgae.pageNumber
                }&pageSizeEnum=${this.resultPgae.pageSize}&iduser=${checkAccessModule.getUserIdCurrent()}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((ele) => {
                        this.data.push(ele)
                    })
                    this.data = this.data.filter((element, index, self) => {
                        return (
                            index ===
                            self.findIndex((obj) => {
                                return JSON.stringify(obj) === JSON.stringify(element)
                            })
                        )
                    })
                })
                .catch((err) => {
                    console.log(err)
                })
        },
        FillterAll(month, year, idProject) {
            HTTP.get(
                `OTs/filterAll/${month}/${year}/${idProject}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.data = res.data._Data
                })
                .catch((err) => {
                    console.log(err)
                })
        },
        async Fillter() {
            this.data = []
            var month = 0
            var year = 0
            var idProject = 0
            if (this.selectedMonth !== null) {
                month = this.selectedMonth.month
                year = this.selectedMonth.year
            }
            if (this.selectedProject !== null) {
                idProject = this.selectedProject.code
            }
            if (checkAccessModule.checkCallAPI(this.$route.path.replace('/', ''))) {
                await this.FillterAll(month, year, idProject)
            } else {
                if (checkAccessModule.isOffice()) {
                    await this.filterByOffice(month, year, idProject)
                    console.log('office');
                }
                if (checkAccessModule.isLead()) {
                    await this.filterByLead(month, year, idProject)
                    console.log('lead');
                }
                if (checkAccessModule.isStaff()) {
                    await this.filterByStaff(month, year, idProject)
                    console.log('staff');
                }
            }
        },
        async getProject() {
            await HTTP.get('Project/getAllProject')
                .then((res) => {
                    if (res.data) {
                        res.data._Data.forEach((element) => {
                            this.selectProject.push({ code: element.id, name: element.name })
                        })
                    }
                })
                .catch((err) => {
                    console.log(err)
                })
        },

        async getProjectLead() {
            await HTTP.get(`Project/getAllProjectByLead/${checkAccessModule.getUserIdCurrent()}`)
                .then((res) => {
                    if (res.data) {
                        res.data._Data.forEach((element) => {
                            this.selectProject.push({ code: element.id, name: element.name })
                        })
                    }
                })
                .catch((err) => {
                    console.log(err)
                })
        },

        async getProjectStaff() {
            await HTTP.get(`Project/getAllProjectByStaff/${checkAccessModule.getUserIdCurrent()}`)
                .then((res) => {
                    if (res.data) {
                        res.data._Data.forEach((element) => {
                            this.selectProject.push({ code: element.id, name: element.name })
                        })
                    }
                })
                .catch((err) => {
                    console.log(err)
                })
        },

        OpenDetailOT(id) {
            this.DetailOT = true
            this.ots = id
        },
        CloseDetailOT() {
            this.DetailOT = false
        },
        OpenFormRefuse() {
            this.displayDialog2 = true
        },
        OpenFormRefuse() {
            this.displayDialog2 = true
        },

        openFormAddEdit(value) {
            this.displayFormAddEdit = true
            this.idproject = value
            if (this.idproject === null) {
                this.edit = false
            } else {
                this.edit = true
            }
        },

        closeFormAddEdit() {
            this.displayFormAddEdit = false
            this.idproject = null
        },

        openConfirm(accepted, id, lead) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn xác nhận duyệt tăng ca này?',
                header: 'Duyệt',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Duyệt',
                acceptIcon: 'pi pi-check',
                rejectLabel: 'Hủy',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-primary CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined CustomButtonPrimeVue',
                accept: () => {
                    this.accept(accepted, id, lead)
                },
                reject: () => {},
                onShow: () => {
                    //callback to execute when dialog is shown
                },
                onHide: () => {
                    //callback to execute when dialog is hidden
                },
            })
        },
    },
    components: { Edit, Delete, Export, DetailOT, AddOTsDialog },
}
</script>

<style lang="scss" scoped>
.actions-buttons {
    display: flex;
    .btn-margin {
        margin-right: 5px;
    }
}
</style>
