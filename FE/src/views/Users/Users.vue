<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />

        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item" @click="goToOverViews()">Tổng quan</li>
                            <li class="breadcrumb-item active" aria-current="page">Người dùng</li>
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
                                    @click="exportCSV($event)"
                                    v-if="this.showButton.export"
                                    :disabled="canExport"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Add
                                    label="Thêm người dùng"
                                    class="p-button-sm custom-button-h"
                                    @click="OpenAdd"
                                    v-if="this.showButton.add"
                                />
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    type="button"
                                    icon="pi pi-filter-slash "
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
                                    v-model="selectedRank"
                                    :options="rankList"
                                    filter
                                    optionLabel="nameRank"
                                    optionValue="nameRank"
                                    placeholder="Chọn rank"
                                    style="width: 100%"
                                    class="custom-input-h"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <MultiSelect
                                    v-model="selectedColumns"
                                    :options="columns"
                                    optionLabel="header"
                                    placeholder="Chọn"
                                    class="custom-input-h"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    class="custom-input-h"
                                    @input="handlerFilterUserByName()"
                                    v-model="filterUserByNameOrUserCode"
                                    placeholder="Nhập tên hoặc mã nhân viên để tìm..."
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        :loading="isLoading"
                        :value="data"
                        :sort="1"
                        showGridlines
                        ref="dt"
                        class="p-datatable-customers"
                        :rows="5"
                        dataKey="id"
                        :rowHover="true"
                        v-model:filters="filters"
                        v-model:selection="selectedUser"
                        filterDisplay="menu"
                        responsiveLayout="scroll"
                        :globalFilterFields="['userCode']"
                        :exportFilename="'ListUser_' + newDateFormat"
                    >
                        <template #loading> </template>
                        <template #empty>
                            <div v-if="this.isLoading" style="display: flex; justify-items: flex-end">
                                <ProgressSpinner style="width: 42px" />
                            </div>
                            <div v-else>Không tìm thấy.</div>
                        </template>
                        <Column header="#" dataType="date">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="userCode" header="Mã nhân viên" sortable>
                            <template #body="{ data }">
                                {{ data.userCode }}
                            </template>
                        </Column>
                        <Column field="fullName" header="Họ và tên" sortable style="min-width: 15rem">
                            <template #body="{ data }">
                                {{ data.fullName }}
                            </template>
                        </Column>
                        <Column field="listgroup" header="Chức vụ" sortable>
                            <template #body="{ data }">
                                <div v-for="item in data.listgroup" :key="item">
                                    {{
                                        item === 1
                                            ? 'Admin'
                                            : item === 2
                                            ? 'Office'
                                            : item === 3
                                            ? 'Lead'
                                            : item === 4
                                            ? 'Staff'
                                            : item === 5
                                            ? 'Pm'
                                            : ''
                                    }}
                                </div>
                            </template>
                        </Column>
                        <Column field="email" header="Email" sortable>
                            <template #body="{ data }">
                                {{ data.email }}
                            </template>
                        </Column>
                        <!-- <Column field="idGroup" header="Nhóm" sortable>
                            <template #body="{ data }">
                                {{ data.idGroup }}
                            </template>
                        </Column> -->
                        <Column field="workStatus" header="Trạng thái" sortable>
                            <template #body="{ data }">
                                <p :style="{ color: data.workStatus === 'Đang làm việc' ? 'green' : 'red' }">
                                    {{ data.workStatus }}
                                </p>
                            </template>
                        </Column>
                        <Column field="dateCreated" header="Ngày gia nhập" sortable>
                            <template #body="{ data }">
                                {{ data.dateCreated }}
                            </template>
                        </Column>
                        <Column
                            sortable
                            v-for="(col, index) of selectedColumns"
                            :field="col.field"
                            :header="col.header"
                            :key="index"
                            style="min-width: 14rem"
                        ></Column>

                        <Column field="" header="Thực thi" style="width: 10rem; text-align: left">
                            <template #body="{ data }">
                                <div
                                    class="actions-buttons"
                                    v-if="data.workStatus !== 'Nghỉ việc' && checkCanOperation('thaotac', data)"
                                >
                                    <Edit
                                        class="p-button-warning custom-button-h"
                                        @click="OpenEdit(data.id)"
                                        v-if="this.showButton.update"
                                        :disabled="checkCanOperation('sua', data)"
                                    />
                                    <Delete
                                        class="custom-button-update custom-button-h"
                                        @click="confirmDelete(data.id, data.workStatus)"
                                        v-if="this.showButton.delete"
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
                        :totalItems="totalItem"
                        :itemIndex="itemIndex"
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
                <Button label="Lưu" icon="pi pi-check" @click="submit" autofocus />
            </template>
        </Dialog>

        <AddUserDiaLog
            :statusopen="openAddform"
            @closeAdd="closeAdd()"
            :roleoption="Optionrole"
            :branchList="branchList"
            @reloadpage="getData()"
            @reloadEditToSave="reloadEditToSave()"
        />
        <EditUserDiaLog
            :statusopen="OpenEditform"
            @closeModal="closeModal()"
            :iduser="Iduser"
            :branchList="branchList"
            :roleoption="Optionrole"
            @reloadpage="getData()"
            @reloadEditToSave="reloadEditToSave()"
            @coloseReloadEditToSave="coloseReloadEditToSave()"
        />
    </LayoutDefaultDynamic>
</template>

<script>
import Edit from '../../components/buttons/Edit.vue'
import Delete from '../../components/buttons/Delete.vue'
import { HTTP } from '@/http-common'
import Add from '../../components/buttons/Add.vue'
import Export from '../../components/buttons/Export.vue'
import { FilterMatchMode } from 'primevue/api'
import AddUserDiaLog from './AddUserDiaLog.vue'
import EditUserDiaLog from './EditUserDiaLog.vue'
import { UserRoleHelper } from '@/helper/user-role.helper'
import { LocalStorage } from '@/helper/local-storage.helper'
import { HttpStatus } from '@/config/app.config'
import { DateHelper } from '@/helper/date.helper'
import { checkAccessModule } from '@/helper/checkAccessModule'
import { UserService } from '@/service/user.service'

export default {
    name: 'users',
    data() {
        return {
            isLoading: true,
            openAddform: false,
            OpenEditform: false,
            Iduser: null,
            selectedUser: null,
            data: [],
            idArr: [],
            usercodeArr: [],
            isAsc: false,
            isDesc: false,
            isSort: false,
            disableAddButton: true,
            disableEditButton: true,
            disableDeleteButton: true,
            displayBasic: false,
            selectedBranch: null,
            selectedRank: '',
            branchList: [],
            rankList: [],
            num: 5,
            timeOut: null,
            filters: {
                userCode: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            filterUserByNameOrUserCode: '',
            selectedColumns: null,
            columns: [
                { field: 'dOB', header: 'Ngày sinh' },
                { field: 'gender', header: 'Giới tính' },
                { field: 'identitycard', header: 'Căn cước công dân' },
                { field: 'phoneNumber', header: 'Số điện thoại' },
                { field: 'address', header: 'Địa chỉ' },
                { field: 'dateStartWork', header: 'Ngày bắt đầu làm việc' },
                { field: 'university', header: 'Đại học' },
                { field: 'yearGraduated', header: 'Năm tốt nghiệp' },
                { field: 'skype', header: 'Skype' },
                { field: 'maritalStatus', header: 'Tình trạng hôn nhân' },
                { field: 'dateLeave', header: 'Ngày thôi việc' },
                { field: 'nameBranch', header: 'Chi nhánh' },
                { field: 'rank', header: 'Rank' },
                { field: 'userCreated', header: 'Người tạo ' },
                { field: 'dateCreated', header: 'Ngày tạo ' },
                { field: 'userModified', header: 'Người chỉnh sửa ' },
                { field: 'dateModified', header: 'Ngày chỉnh sửa ' },
            ],
            Optionrole: [],
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
                isNotSample: false,
            },
            roleList: [],
            newDateFormat: DateHelper.formatDate(new Date()),
            resultPgae: {
                pageSize: 10,
                pageNumber: 1,
            },
            totalItem: 0,
            totalMapPage: 0,
            loading: true,
            itemIndex: 0,
            canExport: true,
            listGroupUser: [],
        }
    },
    watch: {
        resultPgae: {
            handler: function change() {
                if (this.filterUserByNameOrUserCode != '' || this.selectedBranch != null || this.selectedRank != '') {
                    this.handlerFilterUserByName()
                } else {
                    this.getData()
                }
            },
            deep: true,
        },
        selectedBranch: {
            handler: function change(value) {
                if (value != null) {
                    this.handlerFilterUserByName()
                } else {
                    this.getData()
                }
            },
            deep: true,
        },
        selectedRank: {
            handler: function change(value) {
                if (value != '') {
                    this.handlerFilterUserByName()
                } else {
                    this.getData()
                }
            },
            deep: true,
        },
        filterUserByNameOrUserCode: {
            handler: function change(value) {
                if (value != '') {
                    this.handlerFilterUserByName()
                } else {
                    this.getData()
                }
            },
            deep: true,
        },
    },
    async created() {
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '')) === true) {
            checkAccessModule.checkPermissionAction(this.$route.path.replace('/', ''), this.showButton)
            await this.Permission(checkAccessModule.getListGroup(), Number(checkAccessModule.getUserIdCurrent()))
            await this.getAllGroupUser()
            this.loading = false
        } else {
            this.countTime()
            this.displayBasic = true
        }
    },
    methods: {
        goToOverViews() {
            this.$router.push({ name: 'home'})
        },
        checkCanOperation(nameButton, data) {
            const name = nameButton.toLowerCase()
            if (name === 'them') {
            }
            if (name === 'sua') {
                if (data.workStatus === 'Đang làm việc') {
                    return false
                } else {
                    return true
                }
            }
            if (name === 'xoa') {
                if (checkAccessModule.isAdmin()) {
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

            if (name === 'thaotac') {
                if (checkAccessModule.isAdmin()) {
                    return true
                } else {
                    return false
                }
            }
        },

        OpenAdd() {
            this.openAddform = true
        },

        closeAdd() {
            this.openAddform = false
        },

        OpenEdit(id) {
            this.Iduser = id
            this.OpenEditform = true
        },

        closeModal() {
            this.OpenEditform = false
        },
        checkShowActionAdmin() {
            if (checkAccessModule.getListGroup().includes('1')) {
                return true
            } else {
                return false
            }
        },

        getRoles() {
            HTTP.get('Group/getListGroup').then((res) => {
                this.Optionrole = res.data
                this.roleList = res.data
            })
        },
        async Permission() {
            await this.getData()
            await this.getAllBranch()
            this.loading = false
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
        async getAllRank() {
            await HTTP.get('Users/getAllRank')
                .then((res) => {
                    this.rankList = res.data
                })
                .catch((err) => {
                    console.log(err)
                })
        },
        async getDataByRole(id) {
            this.isLoading = false
            await HTTP.get('/Users/getUserById/' + id)
                .then((res) => {
                    if (res.status == 200) {
                        this.totalMapPage = 1
                        this.totalItem = 1
                        this.itemIndex = 1

                        const temp = [res.data]
                        temp.forEach((element) => {
                            if (element.isDeleted == 0) {
                                this.data.push({ ...element, fullName: '' })
                            }
                        })
                        this.data.forEach((element) => {
                            element.fullName = this.mergeString(element.lastName, element.firstName)
                            element.dateStartWork = this.formatDate(element.dateStartWork)
                            element.dateLeave = this.formatDate(element.dateLeave)
                            element.dateCreated = this.formatDate(element.dateCreated)
                            element.dateModified = this.formatDate(element.dateModified)
                            element.dOB = this.formatDate(element.dOB)
                            element.workStatus = this.getWorkStatus(element.workStatus)
                            element.gender = this.getGender(element.gender)
                            element.maritalStatus = this.getMaritalStatus(element.maritalStatus)
                            element.identitycard =
                                element.identitycard === '' ? 'Không có dữ liệu' : element.identitycard
                            element.skype = element.skype === '' ? 'Không có dữ liệu' : element.skype
                            element.university = element.university === '' ? 'Không có dữ liệu' : element.university
                            element.yearGraduated =
                                element.yearGraduated === null ? 'Không có dữ liệu' : element.yearGraduated
                            element.dateLeave = element.dateLeave === null ? 'Chưa thôi việc' : element.dateLeave
                            element.userModified =
                                element.userModified === null ? 'Không có dữ liệu' : element.userModified
                            element.dateModified =
                                element.dateModified === null ? 'Không có dữ liệu' : element.dateModified
                            temp.forEach((user) => {
                                if (user.id === element.userCreated)
                                    element.userCreated = user.lastName + ' ' + user.firstName
                                if (user.id === element.userModified)
                                    element.userModified = user.lastName + ' ' + user.firstName
                            })
                        })
                        this.isLoading = false
                    }
                })
                .catch((err) => console.log(err))
        },

        reloadEditToSave() {
            this.isLoading = true
        },
        coloseReloadEditToSave() {
            this.isLoading = false
        },
        getAllGroupUser() {
            HTTP.get('User_Group/getAllUserGroup')
                .then((res) => {
                    this.listGroupUser = res.data._Data
                })
                .catch((err) => {
                    console.log(err)
                })
        },

        async reloadData() {
            this.data = []
            this.rankList = []
            if (checkAccessModule.getListGroup().includes('1')) {
                HTTP.get('Users/getAll').then((res) => {
                    if (res.status == 200) {
                        const temp = res.data
                        temp.forEach((element) => {
                            if (element.isDeleted == 0) {
                                this.data.push({ ...element, fullName: '' })
                            }
                        })
                        this.data.forEach((element) => {
                            element.fullName = this.mergeString(element.lastName, element.firstName)
                            element.dateStartWork = this.formatDate(element.dateStartWork)
                            element.dateLeave = this.formatDate(element.dateLeave)
                            element.dateCreated = this.formatDate(element.dateCreated)
                            element.dateModified = this.formatDate(element.dateModified)
                            element.dOB = this.formatDate(element.dOB)
                            element.workStatus = this.getWorkStatus(element.workStatus)
                            element.gender = this.getGender(element.gender)
                            element.maritalStatus = this.getMaritalStatus(element.maritalStatus)
                            temp.forEach((user) => {
                                if (user.id === element.userCreated)
                                    element.userCreated = user.lastName + ' ' + user.firstName
                                if (user.id === element.userModified)
                                    element.userModified = user.lastName + ' ' + user.firstName
                            })
                        })
                        this.isLoading = false
                    }
                })
            } else {
                await this.getDataByRole(checkAccessModule.getUserIdCurrent())
            }
            await this.getAllRank()
        },
        getData() {
            HTTP.get(
                `Users/getAll?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            ).then((res) => {
                if (res.status == 200) {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    var data1 = res.data._Data.map((element) => ({
                        id: element.id,
                        userCode: element.userCode,
                        firstName: element.firstName,
                        lastName: element.lastName,
                        fullName: this.mergeString(element.lastName, element.firstName),
                        dateStartWork: this.formatDate(element.dateStartWork),
                        dateLeave: this.formatDate(element.dateLeave),
                        dateCreated: this.formatDate(element.dateCreated),
                        dateModified: this.formatDate(element.dateModified),
                        dOB: this.formatDate(element.dOB),
                        workStatus: this.getWorkStatus(element.workStatus),
                        gender: this.getGender(element.gender),
                        maritalStatus: this.getMaritalStatus(element.maritalStatus),
                        identitycard: element.identitycard === '' ? 'Không có dữ liệu' : element.identitycard,
                        skype: element.skype === '' ? 'Không có dữ liệu' : element.skype,
                        university: element.university === '' ? 'Không có dữ liệu' : element.university,
                        yearGraduated: element.yearGraduated === null ? 'Không có dữ liệu' : element.yearGraduated,
                        dateLeave: element.dateLeave === null ? 'Chưa thôi việc' : element.dateLeave,
                        userModified: element.fullNameModifier === null ? 'Không có dữ liệu' : element.fullNameModifier,
                        userCreated: element.fullNameCreate === null ? 'Không có dữ liệu' : element.fullNameCreate,
                        dateModified: element.dateModified === null ? 'Không có dữ liệu' : element.dateModified,
                        phoneNumber: element.phoneNumber === '' ? 'Không có dữ liệu' : element.phoneNumber,
                        email: element.email,
                        address: element.address == '' ? 'Chưa cập nhật...' : element.address,
                        idBranch: element.idBranch,
                        nameBranch: element.nameBranch,
                        rank: element.rank,
                        listgroup: [],
                    }))

                    if (data1.length > 0 && this.listGroupUser.length > 0) {
                        data1.map((ele) => {
                            this.listGroupUser.map((element) => {
                                if (ele.id === element.idUser) {
                                    ele.listgroup.push(element.idGroup)
                                }
                            })
                        })
                    }
                    this.data = data1
                    this.isLoading = false
                }
                if (res.data._Data.length > 0) {
                    this.canExport = false
                } else {
                    this.canExport = true
                }
            })
        },
        async handlerFilterUserByName() {
            this.isLoading = true
            this.data = []
            var link = `Users/SearchUserByNameOrUserCode`
            if (this.filterUserByNameOrUserCode != '' && this.selectedBranch == null && this.selectedRank == '') {
                link += `?name=${this.filterUserByNameOrUserCode}`
            } else if (
                this.filterUserByNameOrUserCode != '' &&
                this.selectedBranch != null &&
                this.selectedRank == ''
            ) {
                link += `?name=${this.filterUserByNameOrUserCode}&idBranch=${this.selectedBranch}`
            } else if (
                this.filterUserByNameOrUserCode != '' &&
                this.selectedBranch == null &&
                this.selectedRank != ''
            ) {
                link += `?name=${this.filterUserByNameOrUserCode}&rank=${this.selectedRank}`
            } else if (
                this.filterUserByNameOrUserCode != '' &&
                this.selectedBranch != null &&
                this.selectedRank != ''
            ) {
                link += `?name=${this.filterUserByNameOrUserCode}&idBranch=${this.selectedBranch}&rank=${this.selectedRank}`
            } else if (
                this.filterUserByNameOrUserCode == '' &&
                this.selectedBranch != null &&
                this.selectedRank == ''
            ) {
                link += `?idBranch=${this.selectedBranch}`
            } else if (
                this.filterUserByNameOrUserCode == '' &&
                this.selectedBranch != null &&
                this.selectedRank != ''
            ) {
                link += `?idBranch=${this.selectedBranch}&rank=${this.selectedRank}`
            } else if (
                this.filterUserByNameOrUserCode == '' &&
                this.selectedBranch == null &&
                this.selectedRank != ''
            ) {
                link += `?rank=${this.selectedRank}`
            } else if (
                this.filterUserByNameOrUserCode == '' &&
                this.selectedBranch == null &&
                this.selectedRank == ''
            ) {
                link += `?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`
            } else {
                link += `&pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`
            }
            await HTTP.get(link)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    var data = res.data._Data.map((element) => ({
                        id: element.id,
                        userCode: element.userCode,
                        firstName: element.firstName,
                        lastName: element.lastName,
                        fullName: this.mergeString(element.lastName, element.firstName),
                        dateStartWork: this.formatDate(element.dateStartWork),
                        dateLeave: this.formatDate(element.dateLeave),
                        dateCreated: this.formatDate(element.dateCreated),
                        dateModified: this.formatDate(element.dateModified),
                        dOB: this.formatDate(element.dOB),
                        workStatus: this.getWorkStatus(element.workStatus),
                        gender: this.getGender(element.gender),
                        maritalStatus: this.getMaritalStatus(element.maritalStatus),
                        identitycard: element.identitycard === '' ? 'Không có dữ liệu' : element.identitycard,
                        skype: element.skype === '' ? 'Không có dữ liệu' : element.skype,
                        university: element.university === '' ? 'Không có dữ liệu' : element.university,
                        yearGraduated: element.yearGraduated === null ? 'Không có dữ liệu' : element.yearGraduated,
                        dateLeave: element.dateLeave === null ? 'Chưa thôi việc' : element.dateLeave,
                        userModified: element.fullNameModifier === null ? 'Không có dữ liệu' : element.fullNameModifier,
                        userCreated: element.fullNameCreate === null ? 'Không có dữ liệu' : element.fullNameCreate,
                        dateModified: element.dateModified === null ? 'Không có dữ liệu' : element.dateModified,
                        phoneNumber: element.phoneNumber === '' ? 'Không có dữ liệu' : element.phoneNumber,
                        email: element.email,
                        address: element.address == '' ? 'Chưa cập nhật...' : element.address,
                        idBranch: element.idBranch,
                        nameBranch: element.nameBranch,
                        rank: element.rank,
                    }))
                    this.data = data
                })
                .catch(() => {
                    this.getData()
                })
                .finally(() => {
                    this.isLoading = false
                })
        },

        async searchByBranch(){
            await HTTP.get('')
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    var data = res.data._Data.map((element) => ({
                        id: element.id,
                        userCode: element.userCode,
                        firstName: element.firstName,
                        lastName: element.lastName,
                        fullName: this.mergeString(element.lastName, element.firstName),
                        dateStartWork: this.formatDate(element.dateStartWork),
                        dateLeave: this.formatDate(element.dateLeave),
                        dateCreated: this.formatDate(element.dateCreated),
                        dateModified: this.formatDate(element.dateModified),
                        dOB: this.formatDate(element.dOB),
                        workStatus: this.getWorkStatus(element.workStatus),
                        gender: this.getGender(element.gender),
                        maritalStatus: this.getMaritalStatus(element.maritalStatus),
                        identitycard: element.identitycard === '' ? 'Không có dữ liệu' : element.identitycard,
                        skype: element.skype === '' ? 'Không có dữ liệu' : element.skype,
                        university: element.university === '' ? 'Không có dữ liệu' : element.university,
                        yearGraduated: element.yearGraduated === null ? 'Không có dữ liệu' : element.yearGraduated,
                        dateLeave: element.dateLeave === null ? 'Chưa thôi việc' : element.dateLeave,
                        userModified: element.fullNameModifier === null ? 'Không có dữ liệu' : element.fullNameModifier,
                        userCreated: element.fullNameCreate === null ? 'Không có dữ liệu' : element.fullNameCreate,
                        dateModified: element.dateModified === null ? 'Không có dữ liệu' : element.dateModified,
                        phoneNumber: element.phoneNumber === '' ? 'Không có dữ liệu' : element.phoneNumber,
                        email: element.email,
                        address: element.address == '' ? 'Chưa cập nhật...' : element.address,
                        idBranch: element.idBranch,
                        nameBranch: element.nameBranch,
                        rank: element.rank,
                    }))
                    this.data = data
                })
                .catch(() => {
                    this.getData()
                })
                .finally(() => {
                    this.isLoading = false
                })
        },
        async searchByNameOrUserCode(){
            await HTTP.get(link)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    var data = res.data._Data.map((element) => ({
                        id: element.id,
                        userCode: element.userCode,
                        firstName: element.firstName,
                        lastName: element.lastName,
                        fullName: this.mergeString(element.lastName, element.firstName),
                        dateStartWork: this.formatDate(element.dateStartWork),
                        dateLeave: this.formatDate(element.dateLeave),
                        dateCreated: this.formatDate(element.dateCreated),
                        dateModified: this.formatDate(element.dateModified),
                        dOB: this.formatDate(element.dOB),
                        workStatus: this.getWorkStatus(element.workStatus),
                        gender: this.getGender(element.gender),
                        maritalStatus: this.getMaritalStatus(element.maritalStatus),
                        identitycard: element.identitycard === '' ? 'Không có dữ liệu' : element.identitycard,
                        skype: element.skype === '' ? 'Không có dữ liệu' : element.skype,
                        university: element.university === '' ? 'Không có dữ liệu' : element.university,
                        yearGraduated: element.yearGraduated === null ? 'Không có dữ liệu' : element.yearGraduated,
                        dateLeave: element.dateLeave === null ? 'Chưa thôi việc' : element.dateLeave,
                        userModified: element.fullNameModifier === null ? 'Không có dữ liệu' : element.fullNameModifier,
                        userCreated: element.fullNameCreate === null ? 'Không có dữ liệu' : element.fullNameCreate,
                        dateModified: element.dateModified === null ? 'Không có dữ liệu' : element.dateModified,
                        phoneNumber: element.phoneNumber === '' ? 'Không có dữ liệu' : element.phoneNumber,
                        email: element.email,
                        address: element.address == '' ? 'Chưa cập nhật...' : element.address,
                        idBranch: element.idBranch,
                        nameBranch: element.nameBranch,
                        rank: element.rank,
                    }))
                    this.data = data
                })
                .catch(() => {
                    this.getData()
                })
                .finally(() => {
                    this.isLoading = false
                })
        },
        async searchByRank(){
            await HTTP.get(link)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    var data = res.data._Data.map((element) => ({
                        id: element.id,
                        userCode: element.userCode,
                        firstName: element.firstName,
                        lastName: element.lastName,
                        fullName: this.mergeString(element.lastName, element.firstName),
                        dateStartWork: this.formatDate(element.dateStartWork),
                        dateLeave: this.formatDate(element.dateLeave),
                        dateCreated: this.formatDate(element.dateCreated),
                        dateModified: this.formatDate(element.dateModified),
                        dOB: this.formatDate(element.dOB),
                        workStatus: this.getWorkStatus(element.workStatus),
                        gender: this.getGender(element.gender),
                        maritalStatus: this.getMaritalStatus(element.maritalStatus),
                        identitycard: element.identitycard === '' ? 'Không có dữ liệu' : element.identitycard,
                        skype: element.skype === '' ? 'Không có dữ liệu' : element.skype,
                        university: element.university === '' ? 'Không có dữ liệu' : element.university,
                        yearGraduated: element.yearGraduated === null ? 'Không có dữ liệu' : element.yearGraduated,
                        dateLeave: element.dateLeave === null ? 'Chưa thôi việc' : element.dateLeave,
                        userModified: element.fullNameModifier === null ? 'Không có dữ liệu' : element.fullNameModifier,
                        userCreated: element.fullNameCreate === null ? 'Không có dữ liệu' : element.fullNameCreate,
                        dateModified: element.dateModified === null ? 'Không có dữ liệu' : element.dateModified,
                        phoneNumber: element.phoneNumber === '' ? 'Không có dữ liệu' : element.phoneNumber,
                        email: element.email,
                        address: element.address == '' ? 'Chưa cập nhật...' : element.address,
                        idBranch: element.idBranch,
                        nameBranch: element.nameBranch,
                        rank: element.rank,
                    }))
                    this.data = data
                })
                .catch(() => {
                    this.getData()
                })
                .finally(() => {
                    this.isLoading = false
                })
        },

        handlerReload() {
            this.data = []
            this.filterUserByNameOrUserCode = ''
            this.selectedRank = ''
            this.selectedBranch = null
            this.isLoading = true
            this.getData()
        },
        countTime() {
            if (this.num == 0) {
                this.submit()
                return
            }
            this.num = this.num - 1
            this.timeOut = setTimeout(() => this.countTime(), 1000)
        },
        submit() {
            clearTimeout(this.timeOut)
            this.$router.push({ name: 'home' })
        },
        getGender(index) {
            if (index == 1) return 'Nam'
            else if (index == 2) return 'Nữ'
            else if (index == 3) return 'Khác'
        },
        getWorkStatus(index) {
            if (index == 1) return 'Đang làm việc'
            else if (index == 2) return 'Nghỉ việc'
            else if (index == 3) return 'Nghỉ thai sản'
        },
        getMaritalStatus(index) {
            if (index == 1) return 'Đã kết hôn'
            else if (index == 2) return 'Chưa kết hôn'
            else if (index == 3) return 'Chưa xác định'
        },
        toAddUserPage() {
            this.$router.push({ name: 'adduser' })
        },
        toEditUserPage(id) {
            this.$router.push({ name: 'edituser', params: { id: id } })
        },
        confirmDelete(id) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn muốn xóa người dùng này?',
                header: 'Xóa',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Xóa',
                rejectLabel: 'Hủy',
                acceptIcon: 'pi pi-trash',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-danger CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: async () => {
                    await this.deleteUser(id)
                },
                reject: () => {
                    return
                },
            })
        },
        formatDate(value) {
            if (value == null) return null
            else
                return new Date(value).toLocaleDateString('en-CA', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                })
        },
        CheckEdit(id) {
            if (checkAccessModule.getListGroup().includes('1') === true) {
                return false
            } else {
                if (Number(checkAccessModule.getUserIdCurrent()) === id) {
                    return false
                } else {
                    return true
                }
            }
        },
        async deleteUser(userId) {
            this.isLoading = true
            let API_URL = 'Users/deleteUser/' + userId
            await HTTP.put(API_URL, { userModified: checkAccessModule.getUserIdCurrent() })
                .then((res) => {
                    if (res.status == HttpStatus.OK) {
                        this.data = []
                        this.getData()
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Xóa thành công!',
                            life: 2000,
                        })
                    }
                })
                .catch((error) => {
                    this.$toast.add({ severity: 'warn', summary: 'Cảnh báo ', detail: error, life: 2000 })
                })
                .finally(() => {
                    this.isLoading = false
                })
        },

        exportCSV() {
            this.$refs.dt.exportCSV()
        },
        mergeString(str1, str2) {
            return str1 + ' ' + str2
        },
        async getInfoUserGitLab() {
            try {
                var allInfoUser = [];
                var page = 1;
                var  perPage = 100;
                var users = [];
                while(true){
                    await UserService.getAllInfoUserGitLab(page, perPage).then(res => {
                        users = res.data;
                        allInfoUser = allInfoUser.concat(users);
                    })
                    .catch((error) => {
                        console.error(error);
                    });
                    if(users.length < perPage){
                        break;
                    }
                    page ++;
                }
            } catch (error) {
                console.error(error);
            }
        },
    },
    components: { Export, Add, Edit, Delete, AddUserDiaLog, EditUserDiaLog },
}
</script>
<style lang="scss" scoped>
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
}

.input-text {
    display: flex;
    height: 45px;
}

.header-container {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1rem;
    height: 40px;
}

.actions-buttons {
    display: flex;
    width: 110px;
    justify-content: space-evenly;
}
</style>
