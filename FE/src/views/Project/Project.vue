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
                            <li class="breadcrumb-item active" aria-current="page">Dự án</li>
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
                                    @click="exportToExcel()"
                                    v-if="this.showButton.export"
                                    :disabled="canExport"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    class="p-button-sm custom-button-h"
                                    @click="openDialogAdd()"
                                    v-if="this.showButton.add"
                                    icon="pi pi-plus"
                                    label="Thêm dự án"
                                ></Button>
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
                                <MultiSelect
                                    :modelValue="selectedColumns"
                                    :options="columns"
                                    optionLabel="header"
                                    @update:modelValue="onToggle"
                                    placeholder="Chọn"
                                    class="custom-input-h"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    class="custom-input-h"
                                    v-model="filterNameProject"
                                    placeholder="Nhập tên dự án..."
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
                        ref="dt"
                        class="p-datatable-customers"
                        dataKey="id"
                        showGridlines
                        :rowHover="true"
                        removableSort 
                        v-model:filters="filters"
                        v-model:selection="selectedProject"
                        sortField="projectCode"
                        :sortOrder="-1"
                        :loading="loading"
                        responsiveLayout="scroll"
                        :globalFilterFields="['name']"
                        selectionMode="single"
                        :metaKeySelection="false"                                          
                        @row-dblclick="goToTheAppropriateScreenPage"
                    >
                        <Column field="" header="#">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="subProjectCode" header="Mã dự án " sortable>
                            <template #body="{ data }">
                                {{ data.subProjectCode }}
                            </template>
                        </Column>
                        <Column field="projectName" header="Tên dự án " sortable style="min-width: 10rem">
                            <template #body="{ data }">
                                {{ data.projectName }}
                            </template>
                        </Column>
                        <Column field="startDate" header="Ngày bắt đầu " sortable style="min-width: 8rem">
                            <template #body="{ data }">
                                {{ getFormattedDate(new Date(data.startDate)) }}
                            </template>
                        </Column>
                        <Column field="endDate" header="Ngày kết thúc " sortable style="min-width: 8rem">
                            <template #body="{ data }">
                                {{
                                    getFormattedDate(new Date(data.endDate)) == '01-01-1970'
                                        ? 'Đang cập nhật...'
                                        : getFormattedDate(new Date(data.endDate))
                                }}
                            </template>
                        </Column>
                        <Column
                            style="min-width: 8rem"
                            sortable
                            v-for="(col, index) of selectedColumns"
                            :field="col.field"
                            :header="col.header"
                            :key="col.field + '_' + index"
                        ></Column>
                        <Column header="Trạng thái" field="statusProject" sortable>
                            <template #body="{ data }">
                                <div
                                    :class="
                                        (data.isFinished == false && data.isDeleted == false)
                                            ? 'badge bg-warning'
                                            : data.isFinished == true
                                            ? 'badge bg-success'
                                            : data.isDeleted == true
                                            ? 'badge bg-danger'
                                            : null
                                    "
                                >
                                    {{ data.statusProject }}
                                </div>
                            </template>
                        </Column>
                        <Column header="&emsp;&emsp;&emsp;Thực thi" style="min-width: 10rem">
                            <template #body="{ data }">
                                <div v-if="checkCanOperation('thaotac', data)" class="actions-buttons">
                                    <Member
                                        @click="toDetailProject(data.id)"
                                        class="p-button-info me-2 custom-button-h"
                                        :disabled="checkCanOperation('themthanhvien', data)"
                                        v-if="this.showButton.addMember"
                                        v-tooltip.top="'Thêm thành viên vào dự án'"
                                    />
                                    <Edit
                                    v-if="(data.isOnGitlab == false &&  this.showButton.update) || 
                                        (data.isOnGitlab == true && data.projectCode !== data.subProjectCode && this.showButton.update)"
                                        class="p-button-warning me-2 custom-button-h"
                                        @click="openDialogEdit(data)"
                                        :disabled="checkCanOperation('sua', data)"
                                    />
                                    <Delete
                                        class="p-button-danger me-2 custom-button-h"
                                        @click="confirmDelete(data.id)"
                                        :disabled="checkCanOperation('xoa', data)"
                                        v-if="this.showButton.delete"
                                    />
                                    <Button
                                        @click="finishProject(data.id)"
                                        class="p-button-sm p-button-success custom-button-h"
                                        icon="pi pi-check"
                                        :disabled="checkCanOperation('xacnhan', data)"
                                        v-if="this.showButton.confirm"
                                        v-tooltip.top="'Duyệt hoàn thành dự án'"
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
            <Dialog
                header="Không có quyền truy cập!"
                :visible="displayBasic"
                :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
                :style="{ width: '30vw' }"
                :modal="true"
                :closable="false"
            >
                <p>Bạn không có quyền truy cập !</p>
                <medium
                    >Bạn sẽ được điều hướng vào trang chủ <strong>{{ num }}</strong> giây!
                </medium>
                <template #footer>
                    <Button label="Hoàn tất" icon="pi pi-check" @click="submit" autofocus />
                </template>
            </Dialog>
            <DialogAddEdit
                :isOpenDialog="isOpenDialog"
                :projectSelected="{ ...projectSelected }"
                @closeDialog="closeDialog"
                @getAllProject="Permission"
                :user="user"
                :projectGit="dataProjects"
                @handlerOpenAddEditProject="handlerOpenAddEditProject()"
                @handlerCloseAddEditProject="handlerCloseAddEditProject()"
            />
        </div>
    </LayoutDefaultDynamic>
</template>

<script>
import router from '@/router/index'
import { GET_USER_NAME_BY_ID, HTTP } from '@/http-common'
import { FilterMatchMode } from 'primevue/api'
import jwtDecode from 'jwt-decode'
import Add from '../../components/buttons/Add.vue'
import Edit from '../../components/buttons/Edit.vue'
import Delete from '../../components/buttons/Delete.vue'
import Member from '../../components/buttons/Member.vue'
import Export from '../../components/buttons/Export.vue'
import DialogAddEdit from '@/views/Project/DialogAddEdit.vue'
import { ProjectDto } from '@/views/Project/Project.dto'
import { UserRoleHelper } from '@/helper/user-role.helper'
import { HTTP_API_GITLAB, GET_ALL_PROJECT } from '@/http-common'
import { LocalStorage } from '@/helper/local-storage.helper'
import { DateHelper } from '@/helper/date.helper'
import { checkAccessModule } from '@/helper/checkAccessModule'
import { ProjectService } from '@/service/project.service'
import axios from 'axios'

export default {
    data() {
        return {
            filters: {
                name: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            filterNameProject: '',
            userInfo: [],
            selectedProject: [],
            selectedColumns: null,
            columns: [
                {field: 'projectCode', header: 'Mã dự án gốc'},
                { field: 'name', header: 'Tên dự án gốc'},
                { field: 'description', header: 'Mô tả' },
                { field: 'fullNameUserId', header: 'PM' },
                { field: 'leader', header: 'Leader' },
                { field: 'fullNameUserCreated', header: 'Người tạo' },
                { field: 'dateCreated', header: 'Ngày tạo' },
                { field: 'fullNameUserUpdate', header: 'Người chỉnh sửa' },
                { field: 'dateUpdate', header: 'Ngày chỉnh sửa' },
            ],
            arr: [],
            data: [],
            loading: true,
            displayBasic: false,
            pageIndex: [5, 10, 15, 20],
            num: 5,
            timeout: null,
            isOpenDialog: false,
            projectSelected: new ProjectDto(),
            user: [],
            token: null,
            showButton: {
                isNotSample: false,
            },
            dataProjects: [],
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
            handler: function change() {
                this.data = []
                if (this.filterNameProject != '') {
                    this.PermissionFilterProject(
                        checkAccessModule.getListGroup(),
                        checkAccessModule.getUserIdCurrent(),
                        this.filterNameProject,
                    )
                } else {
                    this.Permission()
                }
            },
            deep: true,
        },
        filterNameProject: {
            handler: async function Change(newValName) {
                this.data = []
                await this.PermissionFilterProject(
                    checkAccessModule.getListGroup(),
                    checkAccessModule.getUserIdCurrent(),
                    newValName,
                )
            },
        },
    },
    async created() {
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '')) === true) {
            checkAccessModule.checkPermissionAction(this.$route.path.replace('/', ''), this.showButton)
            await this.Permission()
            await this.handlerGetInfoProjects()
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
        goToProjectOnGit(id, parentProjectCode, projectCode) {
            this.$router.push({ name: 'tasks', params: { id: id, parentProjectCode:  parentProjectCode, projectCode:  projectCode} })
        },
        goToProjectOnNotGit(id) {
            this.$router.push({ name: 'timeSheetDailys', params: { idProject: id } })
        },
        async goToTheAppropriateScreenPage(event) {
            await HTTP.get(`Project/checkOnProjectGit/${event.data.projectCode}`)
                .then((res) => {
                    if(res.data.isOnGitlab){
                        this.goToProjectOnGit(event.data.id, event.data.projectCode, event.data.subProjectCode);
                    }else {
                        this.goToProjectOnNotGit(event.data.projectCode);
                    }
                })
                .catch((err) => { console.log(err) })
        },
        checkCanOperation(nameButton, data) {
            const name = nameButton.toLowerCase()
            if (name === 'them') {
            }
            else if (name === 'sua') {
                if (
                    (data.isDeleted === false &&
                        data.isFinished === false &&
                        data.userId === Number(checkAccessModule.getUserIdCurrent())) ||
                    (data.isDeleted === false && data.isFinished === false && checkAccessModule.isAdmin())
                ) {
                    return false
                } else {
                    return true
                }
            }
            else if (name === 'xoa') {
                if (
                    (data.isDeleted === false &&
                        data.isFinished === false &&
                        data.userId === Number(checkAccessModule.getUserIdCurrent())) ||
                    checkAccessModule.isAdmin()
                ) {
                    return false
                } else {
                    return true
                }
            }
            else if (name === 'xoanhieu') {
            }
            else if (name === 'xuatfile') {
            }
            else if (name === 'xacnhan') {
                if (
                    (data.isDeleted === false &&
                        data.isFinished === false &&
                        data.userId === Number(checkAccessModule.getUserIdCurrent())) ||
                    (data.isDeleted === false && data.isFinished === false && checkAccessModule.isAdmin())
                ) {
                    return false
                } else {
                    return true
                }
            }
            else if (name === 'xacnhannhieu') {
            }
            else if (name === 'themthanhvien') {
                if (data.isDeleted === false && data.isFinished === false) {
                    return false
                } else {
                    return true
                }
            }
            else if (name === 'tuchoi') {
            }
            else if (name === 'thaotac') {
                if (
                    data.idLeader === Number(checkAccessModule.getUserIdCurrent()) ||
                    data.userId === Number(checkAccessModule.getUserIdCurrent()) ||
                    checkAccessModule.isAdmin()
                ) {
                    return true
                } else {
                    return false
                }
            }
        },
        openDialogAdd() {
            this.isOpenDialog = true
            this.projectSelected = []
        },
        closeDialog() {
            this.isOpenDialog = false
            this.projectSelected = []
        },
        handlerOpenAddEditProject(){
            this.loading = true
        },
        handlerCloseAddEditProject(){
            this.loading = false
        },
        openDialogEdit(data) {
            this.projectSelected = { ...data }
            this.isOpenDialog = true
        },
        async finishProject(id) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn xác nhận duyệt dự án này?',
                header: 'Duyệt',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Duyệt',
                acceptIcon: 'pi pi-check',
                rejectLabel: 'Hủy',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-primary CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: async () => {
                    await HTTP.put('Project/FinishProject/' + id, { UserId: checkAccessModule.getUserIdCurrent() })
                        .then((res) => {
                            if (res.status == 200) {
                                this.Permission()
                                this.showSuccess('Dự án hoàn tất!')
                            }
                        })
                        .catch((err) => {
                            this.showError(err.response.data)
                            console.log(err);
                        })
                },
                reject: () => { return; },
            })
        },
        onToggle(value) {
            this.selectedColumns = this.columns.filter((col) => value.includes(col))
        },
        formatDate(value) {
            return DateHelper.convertFullDate(value)
        },
        async handlerReload() {
            this.data = []
            this.filterNameProject = ''
            await this.Permission()
        },
        async PermissionFilterProject(value, id, newValName) {
            if (value.length > 0) {
                if(checkAccessModule.checkCallAPI(this.$route.path.replace('/', ''))){
                    await this.filterProjectByName(newValName)
                }else{
                    if (checkAccessModule.isLead()) {
                            await this.filterProjectByNameOfLead(id, newValName)
                        }
                        if (checkAccessModule.isStaff()) {
                            await this.filterProjectByNameOfStaff(id, newValName)
                        }
                }
            }
        },
        async filterProjectByName(name) {
            this.data = []
            this.loading = true
            ProjectService.filterProjectByName(name, this.resultPgae.pageNumber, this.resultPgae.pageSize)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((el) => {
                        const object = {
                            ...el,
                            fullNameUserId: el.fullNameUserId ?? 'Chưa cập nhật...',
                            dateUpdate: this.formatDate(el.dateUpdate),
                            dateCreated: this.formatDate(el.dateCreated),
                            fullNameUserCreated: el.fullNameUserCreated ?? 'Chưa cập nhật...',
                            fullNameUserUpdate: el.fullNameUserUpdate ?? 'Chưa cập nhật...',
                        }
                        this.data.push(object)
                    })
                    this.data = this.data.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                })
                .catch(() => { this.Permission() })
                .finally(() => { this.loading = false })
        },
        async filterProjectByNameOfLead(idLead, name) {
            this.loading = true
            ProjectService.filterProjectByNameLead(idLead, name, this.resultPgae.pageNumber, this.resultPgae.pageSize)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex

                    res.data._Data.map((el) => {
                        const object = {
                            ...el,
                            fullNameUserId: el.fullNameUserId ?? 'Chưa cập nhật...',
                            dateUpdate: this.formatDate(el.dateUpdate),
                            dateCreated: this.formatDate(el.dateCreated),
                            fullNameUserCreated: el.fullNameUserCreated ?? 'Chưa cập nhật...',
                            fullNameUserUpdate: el.fullNameUserUpdate ?? 'Chưa cập nhật...',
                        }
                        this.data.push(object)
                    })
                    this.data = this.data.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                })
                .catch(() => {
                    this.getProjectByLead(idLead)
                })
                .finally(() => { this.loading = false })
        },
        async filterProjectByNameOfStaff(idStaff, name) {
            this.loading = true
            ProjectService.filterProjectByNameStaff(idStaff, name, this.resultPgae.pageNumber, this.resultPgae.pageSize)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((el) => {
                        const object = {
                            ...el,
                            fullNameUserId: el.fullNameUserId ?? 'Chưa cập nhật...',
                            dateUpdate: this.formatDate(el.dateUpdate),
                            dateCreated: this.formatDate(el.dateCreated),
                            fullNameUserCreated: el.fullNameUserCreated ?? 'Chưa cập nhật...',
                            fullNameUserUpdate: el.fullNameUserUpdate ?? 'Chưa cập nhật...',
                        }
                        this.data.push(object)
                    })
                    this.data = this.data.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                })
                .catch(() => {
                    this.getProjectByStaff(idStaff)
                })
                .finally(() => { this.loading = false })
        },
        async Permission() {
            this.data = []
            if(checkAccessModule.checkCallAPI(this.$route.path.replace('/', ''))){
                await this.getAllProject()
            } else {
                if (checkAccessModule.isLead()) {
                    await this.getProjectByLead(checkAccessModule.getUserIdCurrent())
                }
                if (checkAccessModule.isStaff()) {
                    await this.getProjectByStaff(checkAccessModule.getUserIdCurrent())
                }
            }
            if (this.data.length > 0) {
                this.canExport = false
            } else {
                this.canExport = true
            }
        },
        async getAllProject() {
            await HTTP.get(
                `Project/getAllProject?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((el) => {
                        const object = {
                            ...el,
                            fullNameUserId: el.fullNameUserId ?? 'Chưa cập nhật...',
                            dateUpdate: this.formatDate(el.dateUpdate),
                            dateCreated: this.formatDate(el.dateCreated),
                            fullNameUserCreated: el.fullNameUserCreated ?? 'Chưa cập nhật...',
                            fullNameUserUpdate: el.fullNameUserUpdate ?? 'Chưa cập nhật...',
                        }
                        this.data.push(object)
                    })
                    this.data = this.data.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                    if (this.data.length > 0) {
                        this.canExport = false
                    } else {
                        this.canExport = true
                    }
                })
                .catch((err) => {
                    console.log(err)
                })
                .finally(() => { this.loading = false })
        },
        async getProjectByLead(idlead) {
            await HTTP.get(
                `/Project/getAllProjectByLead/${idlead}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((el) => {
                        const object = {
                            ...el,
                            fullNameUserId: el.fullNameUserId ?? 'Chưa cập nhật...',
                            dateUpdate: this.formatDate(el.dateUpdate),
                            dateCreated: this.formatDate(el.dateCreated),
                            fullNameUserCreated: el.fullNameUserCreated ?? 'Chưa cập nhật...',
                            fullNameUserUpdate: el.fullNameUserUpdate ?? 'Chưa cập nhật...',
                        }
                        this.data.push(object)
                    })
                    this.data = this.data.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                    if (this.data.length > 0) {
                        this.canExport = false
                    } else {
                        this.canExport = true
                    }
                })
                .catch((err) => console.log(err))
                .finally(() => { this.loading = false })
        },
        async getProjectByStaff(idstaff) {
            await HTTP.get(
                `/Project/getAllProjectByStaff/${idstaff}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex

                    res.data._Data.map((el) => {
                        const object = {
                            ...el,
                            fullNameUserId: el.fullNameUserId ?? 'Chưa cập nhật...',
                            dateUpdate: this.formatDate(el.dateUpdate),
                            dateCreated: this.formatDate(el.dateCreated),
                            fullNameUserCreated: el.fullNameUserCreated ?? 'Chưa cập nhật...',
                            fullNameUserUpdate: el.fullNameUserUpdate ?? 'Chưa cập nhật...',
                        }
                        this.data.push(object)
                    })
                    this.data = this.data.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                    if (this.data.length > 0) {
                        this.canExport = false
                    } else {
                        this.canExport = true
                    }
                })
                .catch((err) => console.log(err))
                .finally(() => { this.loading = false })
        },
        async deleteProject(id) {
            let userlogin = jwtDecode(localStorage.getItem('token'))
            let idUser = userlogin.Id
            await HTTP.put('Project/DeleteProject/' + id, { userId: idUser })
                .then(async (res) => {
                    if (res.status == 200) {
                        await this.Permission()
                        this.showSuccess('Xóa thành công!')
                    }
                })
                .catch((err) => {
                    console.log(err);
                })
        },
        canOperation(isDeleted, isFinished, actionDelete = null) {
            if (isFinished === true && checkAccessModule.getListGroup().includes('1') && actionDelete !== null) {
                return false
            } else {
                if (isDeleted === true || isFinished === true) {
                    return true
                } else {
                    return false
                }
            }
        },
        confirmDelete(id) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn muốn xóa dự án này?',
                header: 'Xóa dự án',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Xóa',
                rejectLabel: 'Hủy',
                acceptIcon: 'pi pi-trash',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-danger CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: () => {
                    this.deleteProject(id)
                },
                reject: () => {
                    return
                },
            })
        },
        showSuccess(message) {
            this.$toast.add({severity: 'success', summary: 'Thành công!', detail: message, life: 2000, })
        },
        showWarn(message) {
            this.$toast.add({ severity: 'warn', summary: 'Cảnh báo!', detail: message, life: 2000 })
        },
        showError(message) {
            this.$toast.add({ severity: 'error', summary: 'Lỗi!', detail: message, life: 2000 })
        },
        toDetailProject(id) {
            this.$router.push({ name: 'detailproject', params: { id: id } })
        },
        FormatJSon(FilterData, JsonData) {
            return JsonData.map((v) =>
                FilterData.map((j) => {
                    return v[j]
                }),
            )
        },
        exportToExcel() {
           var dataExport = []
            this.data.map((ele) => {
                const object = {
                    projectCode: ele.projectCode,
                    name: ele.name,
                    subProjectCode: ele.subProjectCode,
                    projectName: ele.projectName,
                    startDate: this.getFormattedDate(new Date(ele.startDate)),
                    endDate: ele.endDate === null ? 'Đang cập nhật' : this.getFormattedDate(new Date(ele.endDate)),
                    PMName: ele.fullNameUserId,
                    description: ele.description!=null ? ele.description: "",
                    leader: ele.leader,
                    userCreated: ele.fullNameUserCreated,
                    dateCreated: this.getFormattedDate(new Date(ele.dateCreated)),
                    userEdited: ele.fullNameUserUpdate,
                    dateUpdate: this.getFormattedDate(new Date(ele.dateUpdate)),
                    status:
                        ele.isDeleted === false && ele.isFinished === false
                            ? 'Đang chạy'
                            : ele.isDeleted === true
                            ? 'Đã xóa'
                            : ele.isFinished === true
                            ? 'Đã hoàn thành'
                            : 'Đang cập nhật',
                }
                dataExport.push(object)
            })
            import('../../plugins/Export2Excel.js').then((excel) => {
                const OBJ = dataExport
                const Header = [
                    'Mã dự án gốc',
                    'Tên dự án gốc',
                    'Mã dự án con',
                    'Tên dự án con',
                    'Ngày bắt đầu',
                    'Ngày kết thúc',
                    'PM',
                    'Mô tả',
                    'Leader',
                    'Người tạo',
                    'Ngày tạo',
                    'Người chỉnh sửa',
                    'Ngày chỉnh sửa',
                    'Trạng thái',
                ]
                const Field = [
                    'projectCode',
                    'name',
                    'subProjectCode',
                    'projectName',
                    'startDate',
                    'endDate',
                    'PMName',
                    'description',
                    'leader',
                    'userCreated',
                    'dateCreated',
                    'userEdited',
                    'dateUpdate',
                    'status',
                ]
                const Data = this.FormatJSon(Field, OBJ)
                excel.export_json_to_excel({
                    header: Header,
                    data: Data,
                    sheetName: 'Sheet1',
                    filename: 'Danh sách dự án',
                    autoWidth: true,
                    bookType: 'xlsx',
                })
            })
        },
        
        getFormattedDate(date) {
            var year = date.getFullYear()
            var month = (1 + date.getMonth()).toString()
            month = month.length > 1 ? month : '0' + month
            var day = date.getDate().toString()
            day = day.length > 1 ? day : '0' + day
            return day + '-' + month + '-' + year
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
        async getAllProects(page) {
            return await HTTP_API_GITLAB.get(GET_ALL_PROJECT(100, page)).then((res) => res.data)
        },
        async handlerGetInfoProjects() {
            let resultCountPr = 100
            let resultPr = []
            let page = 1
            while (resultCountPr === 100) {
                let newResultsPr = await this.getAllProects(page)
                page++
                this.dataProjects.push(...newResultsPr)
                resultCountPr = newResultsPr.length
                resultPr = resultPr.concat(newResultsPr)
                newResultsPr.map((el) => {
                    return (el.name = `${el.name} (${el.name_with_namespace})`)
                })
            }
        },
    },
    components: { Add, Edit, Delete, Member, Export, DialogAddEdit },
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
    .p-datatable-thead > tr > th {
        text-align: left;
    }

    .p-datatable-tbody > tr > td {
        cursor: auto;
    }

    .p-dropdown-label:not(.p-placeholder) {
        text-transform: uppercase;
    }
}
.actions-buttons {
        display: flex;

        .btn-margin {
            margin-right: 5px;
        }
    }
</style>