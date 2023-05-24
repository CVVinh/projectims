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
                            <li class="breadcrumb-item" @click="goToProjects()">Dự án</li>
                            <li class="breadcrumb-item active" aria-current="page">Công việc</li>
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
                                <Dropdown
                                    class="custom-input-h"
                                    v-model="selectedProject"
                                    :options="projects"
                                    optionLabel="projectName"
                                    optionValue="id"
                                    placeholder="Chọn dự án"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Export
                                    class="custom-button-h"
                                    label="Xuất Excel"
                                    @click="exportToExcelFollowRole($event)"
                                    v-if="this.showButton.export"
                                    :disabled="selectedProject == null"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2" v-if="this.showButton.add && !checkCanOperation('them')">
                                <Button
                                    class="p-button-sm custom-button-h"
                                    @click="showDialogAddTask()"
                                    icon="pi pi-plus"
                                    label="Thêm công việc"
                                ></Button>
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    class="p-button-sm custom-button-h"
                                    @click="showExportBill()"
                                    icon="pi pi-file-excel"
                                    label="Xuất Bill"
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
                                <InputText
                                    v-model="filtersTaskName"
                                    placeholder="Tìm kiếm theo tên công việc..."
                                    class="custom-input-h"
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>
            <div class="row mt-3">
                <div class="col-sm-12 col-md-12">
                    <DataTable
                        :value="data"
                        ref="dt"
                        class="p-datatable-customers"
                        :rows="5"
                        dataKey="idTask"
                        :loading="loading"
                        :rowHover="true"
                        v-model:filters="filters"
                        filterDisplay="menu"
                        removableSort 
                        showGridlines
                        :globalFilterFields="[
                            '#',
                            'idTask',
                            'idParent',
                            'taskName',
                            'description',
                            'isDeleted',
                            'status',
                            'tag',
                            'startTaskDate',
                            'endTaskDate',
                            'createTaskDate',
                            'createUser',
                            'idProject',
                        ]"
                        responsiveLayout="scroll"
                        :sortOrder="1"
                        :sortField="'taskName'"
                    >
                        <template #empty> Không tìm thấy. </template>
                        <!-- Body -->
                        <Column field="#" header="#" sortable style="min-width: 3rem">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="taskName" header="Tên công việc" sortable style="min-width: 15rem">
                            <template #body="{ data }">
                                {{ data.taskName }}
                            </template>
                        </Column>
                        <!-- <Column header="Mô tả công việc" sortable style="min-width: 15rem">
                            <template #body="{ data }">
                                <p v-html="data.description"></p>
                            </template>
                        </Column> -->
                        <Column header="Trạng thái công việc" style="min-width: 8rem">
                            <template #body="{ data }">
                                <div v-if="data.isOnGitLab" class="d-flex">
                                    <div v-for="(lable, index) in data.arrayLabel" :key="index">
                                        <div class="tags-status">
                                            <span
                                                :style="{
                                                    background: lable.color,
                                                    color: lable.text_color,
                                                }"
                                                >{{ lable.name }}</span
                                            >
                                        </div>
                                    </div>
                                </div>
                                <div v-else>
                                    <div class="tags-status">
                                        <span
                                            :style="{
                                                background: renderStatus(Number(data.status)).color,
                                            }"
                                            >{{ renderStatus(Number(data.status)).name }}</span
                                        >
                                    </div>
                                </div>
                            </template>
                        </Column>
                        <Column field="dateCreated" header="Ngày tạo" sortable dataType="date" style="min-width: 8rem">
                            <template #body="{ data }">
                                {{ formatDateTime(data.dateCreated) }}
                            </template>
                        </Column>
                        <Column
                            field="time_estimate"
                            header="Thời gian ước tính (Dự kiến)"
                            sortable
                            style="min-width: 8rem"
                        >
                            <template #body="{ data }">
                                {{ data.time_estimate ?? 'Đang cập nhật...' }}
                            </template>
                        </Column>
                        <Column
                            field="duedate"
                            header="Ngày kết thúc (Dự kiến)"
                            sortable
                            dataType="date"
                            style="min-width: 8rem"
                        >
                            <template #body="{ data }">
                                {{ data.duedate != null ? formatDateTime(data.duedate) : 'Đang cập nhật...' }}
                            </template>
                        </Column>
                        <Column field="" header="&emsp;&emsp;&emsp;Thực thi" style="min-width: 10rem">
                            <template #body="{ data }">
                                <div class="actions-buttons">
                                    <Button
                                        class="p-button-info mt-1 me-2 custom-input-h"
                                        @click="handlerDetailsTask(data)"
                                        icon="pi pi-eye"
                                        v-tooltip.top="'Xem chi tiết'"
                                    />
                                    <Button
                                        class="mt-1 me-2 p-button-warning custom-input-h"
                                        @click="showDialogUpdateTask(data)"
                                        icon="pi pi-pencil"
                                        v-if="this.showButton.update && !checkCanOperation('sua', data) && !checkIsStaff()"
                                    />
                                    <Button
                                        @click="confirmDelete(data)"
                                        class="p-button-danger mt-1 me-2 custom-input-h"
                                        icon="pi pi-trash"
                                        v-if="this.showButton.delete && !checkCanOperation('xoa', data)"
                                    />
                                    <Button
                                        class="p-button-success mt-1 me-2 custom-input-h"
                                        @click="openUpdateStatusTask(data)"
                                        icon="pi pi-check-square"
                                        v-tooltip.top="'Nhập công số'"
                                    />
                                </div>
                            </template>
                        </Column>
                    </DataTable>
                    <div class="row">
                        <div class="col-sm-12 col-md-12">
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
            </div>
        </div>
        <DialogAddTask
            :isOpen="this.isOpenDialogTask"
            :selectedProject="selectedProject"
            :projectData="this.project"
            @ReloadDataGitLab="getTaskByProject()"
            @closeDialogTask="closeDialogAddTask()"
            @ReloadData="getTaskByProject()"
            :dataUpdateTask="this.selectedTaskEdit"
            :listDataTask="this.data"
            @loadingOpenHandlerAddEdit="loadingOpenHandlerAddEdit()"
            @loadingCloseHandlerAddEdit="loadingCloseHandlerAddEdit()"
        />
        <DialogDetailsTask
            :isOpen="this.isOpenDialogDetailsTask"
            :DetailTask="{ ...taskDetail }"
            @closeDialogDetailTask="closeDialogDetailTask()"
        />
        <UpdateStatusTask
            :isOpenUpdateStatusTask="this.isOpenUpdateStatusTask"
            :listLabel="this.listLabel"
            :dataTask="this.dataUpdateStatusTask"
            @closeUpdateStatusTask="closeUpdateStatusTask()"
            @ReloadDataGitLab="getTaskByProject()"
        />
        <ExportBill
            :isOpen="this.isOpenBill"
            :selectedProject="selectedProject"
            :projectData="this.project"
            @closeExportBill="closeExportBill()"
        />
    </LayoutDefaultDynamic>
</template>

<script>
    import { API_KEY, HTTP, HTTP_API_GITLAB, KEY } from '@/http-common.js'
    import { FilterMatchMode, FilterOperator } from 'primevue/api'
    import Export from '../../components/buttons/Export.vue'
    import ExportBill from './ExportBill.vue'
    import DialogAddTask from './DialogAddTask.vue'
    import UpdateStatusTask from './UpdateStatusTask.vue'
    import { ProjectService } from '@/service/project.service'
    import dayjs from 'dayjs'
    import { TaskService } from '@/service/task.service'
    import jwt_decode from 'jwt-decode'
    import { statusTask } from './DataStatusTask'
    import DialogDetailsTask from './DialogDetailsTask.vue'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import { DateHelper } from '@/helper/date.helper'

    export default {
        data() {
            return {
                data: [],
                dataExport: [],
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                status: statusTask,
                projects: [],
                selectedProject: null,
                booleanProject: false,
                isOpenDialogTask: false,
                isOpenBill: false,
                project: null,
                currenUser: jwt_decode(localStorage.getItem('token')),
                loading: false,
                selectedTaskEdit: null,
                mapBel: [],
                pageSize: 100,
                dataTaskOnGitLab: [],
                gitlab: false,
                taskDetail: null,
                isOpenDialogDetailsTask: false,
                resultPgae: {
                    pageSize: 10,
                    pageNumber: 1,
                },
                totalItem: 0,
                totalMapPage: 0,
                itemIndex: 0,
                url_web: null,
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
                filtersTaskName: '',
                listLabel: [],
                projectCode: null,
                parentProjectCode: null,
                isOpenUpdateStatusTask: false,
                dataUpdateStatusTask: null,
            }
        },
        async created() {
            await this.Permisison() 
            this.projectCode = this.$route.params.projectCode
            this.parentProjectCode = this.$route.params.parentProjectCode
            await this.renderStatus()
            await this.getAllLabelOfProject()
            this.selectedProject = Number(this.$route.params.id)
        },
        watch: {
            selectedProject: {
                handler: async function changeSelected(newValue) {
                    if(newValue!=null || newValue!="") {
                        this.selectedProject = newValue
                        this.project = null
                        await this.getProjectById(newValue)
                        this.loading = true
                        await this.getTaskByProject()
                    }
                },
                deep: true,
            },
            resultPgae: {
                handler: async function change() {
                    if (this.filtersTaskName != '') {
                        await this.handlerFilterTaskByName(this.filtersTaskName)
                    } else {
                        await this.getTaskByProject()
                    }
                },
                deep: true,
            },
            filtersTaskName: {
                handler: async function change(val) {
                    if (this.booleanProject) {
                        if (val != '') {
                            await this.handlerFilterTaskByName(val)
                        } else {
                            await this.getTaskByProject()
                        }
                    }
                },
            },
        },
        methods: {
            goToOverViews() {
                this.$router.push({ name: 'home'})
            },
            goToProjects() {
                this.$router.push({ name: 'project'})
            },
            checkIsStaff(){
                return checkAccessModule.isStaff();
            },
            loadingOpenHandlerAddEdit(){
                this.loading = true;
            },
            loadingCloseHandlerAddEdit(){
                this.loading = false;
            },
            checkCanOperation(nameButton, data) {
                if (this.project == null || data == undefined) return
                const name = nameButton.toLowerCase()
                if (name === 'them') {
                    if (
                        this.project.leader === Number(checkAccessModule.getUserIdCurrent()) ||
                        this.project.userId === Number(checkAccessModule.getUserIdCurrent()) ||
                        (checkAccessModule.isAdmin() && this.selectedProject !== null)
                    ) {
                        return false
                    } else {
                        return true
                    }
                }
                else if (name === 'sua') {
                    if (
                        (data.createUser != null && data.createUser.id === Number(checkAccessModule.getIdUserOnGitLab())) ||
                        (data.assignee != null && data.assignee.id === Number(checkAccessModule.getIdUserOnGitLab()))
                    ) {
                        return false
                    } else {
                        return true
                    }
                }
                else if (name === 'xoa') {
                    if (
                        data.createUser.id === Number(checkAccessModule.getIdUserOnGitLab()) ||
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
                }
                else if (name === 'xacnhannhieu') {
                }
                else if (name === 'themthanhvien') {
                }
                else if (name === 'tuchoi') {
                }
            },
            closeUpdateStatusTask() {
                this.isOpenUpdateStatusTask = false
            },
            async openUpdateStatusTask(data) {
                this.dataUpdateStatusTask = { ...data }
                this.isOpenUpdateStatusTask = true
            },
            async getAllLabelOnApiGitLab(page) {
                return await ProjectService.getAllLabelsOfProject(this.parentProjectCode, 100, page).then((res) => res.data)
            },
            async getAllLabelOfProject() {
                let resultCount = 100
                let results = []
                let page = 1
                while (resultCount === 100) {
                    let newResults = await this.getAllLabelOnApiGitLab(page)
                    page++
                    resultCount = newResults.length
                    results = results.concat(newResults)
                }
                this.listLabel = results
            },
            async getProjectById(selectedProject) {
                this.isFinishedDelete = true
                this.project = null
                await ProjectService.getProjectById(selectedProject)
                    .then((res) => {
                        this.project = res.data
                    })
                    .catch(() => {
                        this.WarningMessage('Lấy dữ liệu dự án không thành công !!!')
                    })
            },
            async Permisison() {
                this.project = []
                if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '').slice(0, 5)) === true) {
                    checkAccessModule.checkPermissionAction(
                        this.$route.path.replace('/', '').slice(0, 5),
                        this.showButton,
                    )
                    if (checkAccessModule.checkCallAPI(this.$route.path.replace('/', '').slice(0, 5))) {
                        await this.getProjects()
                    } else {
                        if (checkAccessModule.isPm()) {
                            await this.getProjectsByPM()
                        }
                        if (checkAccessModule.isLead()) {
                            await this.getProjectsByLead()
                        }
                        if (checkAccessModule.isStaff()) {
                            await this.getProjectsByStaff()
                        }
                    }  
                    var result = []
                    this.projects.map(ele => {
                        if(ele.isOnGitlab===true) result.push(ele);
                    });
                    this.projects = [];
                    this.projects = result;                 
                } else {
                    alert('Khong co quyen')
                    this.$router.push('/')
                    this.displayDialog1 = true
                }
            },
            async handlerGetAllTaskOnAPIGitLab(idProject) {
                const { pageSize, pageNumber } = this.resultPgae
                const { headers, data: taskData } = await TaskService.getAllTaskOnAPIGitLab(
                    idProject,
                    pageSize,
                    pageNumber,
                )
                let totalPages = 0
                let totalItems = 0
                let itemIndexs = 0
                if (taskData.length > 0) {
                    totalPages = parseInt(headers['x-total-pages'])
                    totalItems = parseInt(headers['x-total'])
                    itemIndexs = (pageNumber - 1) * pageSize + 1
                }
                this.totalMapPage = totalPages
                this.totalItem = totalItems
                this.itemIndex = itemIndexs

                await Promise.all(
                    taskData.map(async (el) => {
                        //const lable = await this.renderColorLabel(...el.labels)
                        var lable = this.filterLabeProject(el.labels);
                        this.data.push({
                            assignee: el.assignee,
                            dateCreated: el.created_at,
                            description: el.description,
                            iid_issue: el.iid,
                            id: el.id,
                            labels: el.labels,
                            idProject: el.project_id,
                            idTask: el.iid,
                            taskName: el.title,
                            time_estimate: el.time_stats.human_time_estimate,
                            total_time_spent: el.time_stats.human_total_time_spent,
                            createUser: el.author,
                            isOnGitLab: true,
                            arrayLabel:
                                lable.length > 0
                                    ? lable
                                    : [
                                        {
                                            color: 'rgb(135 224 252)',
                                            text_color: 'rgb(95 81 81)',
                                            name: 'Chưa có trạng thái',
                                        },
                                    ],
                            duedate: el.due_date,
                        })
                    }),
                )
                this.loading = false
                return Promise.resolve(this.data)
            },
            async handlerGetAllTaskOnAPIGitLabByUserAuthor(idProject) {
                const { pageSize, pageNumber } = this.resultPgae
                const { headers, data: taskData } = await TaskService.getAllTaskOnAPIGitLabByUserAuthor(
                    idProject,
                    checkAccessModule.getIdUserOnGitLab(),
                    pageSize,
                    pageNumber,
                )
                let totalPages = 0
                let totalItems = 0
                let itemIndexs = 0
                if (taskData.length > 0) {
                    totalPages = parseInt(headers['x-total-pages'])
                    totalItems = parseInt(headers['x-total'])
                    itemIndexs = (pageNumber - 1) * pageSize + 1
                }
                this.totalMapPage = totalPages
                this.totalItem = totalItems
                this.itemIndex = itemIndexs

                await Promise.all(
                    taskData.map(async (el) => {
                        var lable = this.filterLabeProject(el.labels);
                        this.data.push({
                            assignee: el.assignee,
                            dateCreated: el.created_at,
                            description: el.description,
                            iid_issue: el.iid,
                            id: el.id,
                            labels: el.labels,
                            idProject: el.project_id,
                            idTask: el.iid,
                            taskName: el.title,
                            time_estimate: el.time_stats.human_time_estimate,
                            total_time_spent: el.time_stats.human_total_time_spent,
                            createUser: el.author,
                            isOnGitLab: true,
                            arrayLabel:
                                lable.length > 0
                                    ? lable
                                    : [
                                        {
                                            color: 'rgb(135 224 252)',
                                            text_color: 'rgb(95 81 81)',
                                            name: 'Chưa có trạng thái',
                                        },
                                    ],
                            duedate: el.due_date,
                        })
                    }),
                )
                this.loading = false
                return Promise.resolve(this.data)
            },
            async handlerGetAllTaskOnAPIGitLabByUser(idProject, IdUser) {
                const { pageSize, pageNumber } = this.resultPgae
                const { headers, data: taskData } = await TaskService.getAllTaskByUser(
                    idProject,
                    IdUser,
                    pageSize,
                    pageNumber,
                )
                let totalPages = 0
                let totalItems = 0
                let itemIndexs = 0
                if (taskData.length > 0) {
                    totalPages = parseInt(headers['x-total-pages'])
                    totalItems = parseInt(headers['x-total'])
                    itemIndexs = (pageNumber - 1) * pageSize + 1
                }
                this.totalMapPage = totalPages
                this.totalItem = totalItems
                this.itemIndex = itemIndexs

                await Promise.all(
                    taskData.map(async (el) => {
                        //const lable = await this.renderColorLabel(...el.labels)
                        var lable = this.filterLabeProject(el.labels);
                        this.data.push({
                            assignee: el.assignee,
                            dateCreated: el.created_at,
                            description: el.description,
                            iid_issue: el.iid,
                            id: el.id,
                            labels: el.labels,
                            idProject: el.project_id,
                            idTask: el.iid,
                            taskName: el.title,
                            time_estimate: el.time_stats.human_time_estimate,
                            total_time_spent: el.time_stats.human_total_time_spent,
                            createUser: el.author,
                            isOnGitLab: true,
                            arrayLabel:
                                lable.length > 0
                                    ? lable
                                    : [
                                          {
                                              color: 'rgb(135 224 252)',
                                              text_color: 'rgb(95 81 81)',
                                              name: 'Chưa có trạng thái',
                                          },
                                      ],
                            duedate: el.due_date,
                        })
                    }),
                )
                this.loading = false
                return Promise.resolve(this.data)
            },
            async renderColorLabel(name) {
                return await TaskService.getLabbelByName(this.parentProjectCode, name).then((res) => res.data);
            },
            async renderStatus(code) {
                const result = this.status.find((item) => item.code === code)
                return result
            },
            filterLabeProject(arrLabel){
                var result = []
                this.listLabel.map(ele => {
                    arrLabel.map(item => {
                        if(ele.name == item){
                            result.push(ele);
                        }
                    })
                })
                return result;
            },
            formatDateTime(date) {
                var dateTime = new Date(date)
                return dayjs(dateTime).format('YYYY-MM-DD')
            },
            async handlerFilterTaskByName(name) {
                this.data = []
                this.booleanProject = true
                this.loading = true
                const selectedProject = Object.assign({},this.projects.find((el) => el.id === this.selectedProject),)
                if (checkAccessModule.getListGroup().includes('4')) {
                    if (selectedProject.isOnGitlab) {
                        this.gitlab = true
                        await this.handlerFilterTaskByNameOnGitlabByUser(selectedProject.projectCode, checkAccessModule.getIdUserOnGitLab(), name)
                    } else {
                        this.gitlab = false
                        this.handlerFilterTaskOfStaff(selectedProject.id, checkAccessModule.getUserIdCurrent(), name)
                    }
                } else {
                    if (selectedProject.isOnGitlab) {
                        this.gitlab = true
                        await this.handlerFilterTaskByNameOnGitlab(selectedProject.projectCode, name)
                    } else {
                        await this.handlerFilterTask_IMS(name, selectedProject.id)
                        this.gitlab = false
                    }
                }
                this.loading = false
            },
            async handlerFilterTaskByNameOnGitlabByUser(idProject, idUser, name) {
                const { pageSize, pageNumber } = this.resultPgae
                const { headers, data: taskData } = await TaskService.filterTasksByNameOnGitlabByUser(
                    idProject,
                    idUser,
                    name,
                    pageNumber,
                    pageSize,
                    checkAccessModule.getTokenUserOnGitLab(),
                ).finally(() => { this.loading = false })
                let totalPages = 0
                let totalItems = 0
                let itemIndexs = 0
                if (taskData.length > 0) {
                    totalPages = parseInt(headers['x-total-pages'])
                    totalItems = parseInt(headers['x-total'])
                    itemIndexs = (pageNumber - 1) * pageSize + 1
                }

                this.totalMapPage = totalPages
                this.totalItem = totalItems
                this.itemIndex = itemIndexs
                const newData = await Promise.all(
                    taskData.map(async (el) => {
                        var lable = this.filterLabeProject(el.labels);
                        return {
                            assignee: el.assignee,
                            dateCreated: el.created_at,
                            description: el.description,
                            iid_issue: el.iid,
                            id: el.id,
                            labels: el.labels,
                            idProject: el.project_id,
                            idTask: el.iid,
                            taskName: el.title,
                            time_estimate: el.time_stats.human_time_estimate,
                            total_time_spent: el.time_stats.human_total_time_spent,
                            createUser: el.author,
                            isOnGitLab: true,
                            arrayLabel:
                                lable.length > 0
                                    ? lable
                                    : [
                                          {
                                              color: 'rgb(135 224 252)',
                                              text_color: 'rgb(95 81 81)',
                                              name: 'Chưa có trạng thái',
                                          },
                                      ],
                            duedate: el.due_date,
                        }
                    }),
                )

                this.data = [...newData]
                return Promise.resolve(this.data)
            },
            async handlerFilterTaskByNameOnGitlab(idProject, name) {
                const { pageSize, pageNumber } = this.resultPgae
                var tokenPrivate = "";
                if(checkAccessModule.isAdmin()){
                    tokenPrivate = KEY;
                }
                else {
                    tokenPrivate = checkAccessModule.getTokenUserOnGitLab();
                }
                const { headers, data: taskData } = await TaskService.filterTasksByNameOnGitlab(
                    idProject,
                    name,
                    pageNumber,
                    pageSize,
                    tokenPrivate,
                ).finally(() => { this.loading = false })
                let totalPages = 0
                let totalItems = 0
                let itemIndexs = 0
                if (taskData.length > 0) {
                    totalPages = parseInt(headers['x-total-pages'])
                    totalItems = parseInt(headers['x-total'])
                    itemIndexs = (pageNumber - 1) * pageSize + 1
                }

                this.totalMapPage = totalPages
                this.totalItem = totalItems
                this.itemIndex = itemIndexs
                const newData = await Promise.all(
                    taskData.map(async (el) => {
                        var lable = this.filterLabeProject(el.labels);
                        return {
                            assignee: el.assignee,
                            dateCreated: el.created_at,
                            description: el.description,
                            iid_issue: el.iid,
                            id: el.id,
                            labels: el.labels,
                            idProject: el.project_id,
                            idTask: el.iid,
                            taskName: el.title,
                            time_estimate: el.time_stats.human_time_estimate,
                            total_time_spent: el.time_stats.human_total_time_spent,
                            createUser: el.author,
                            isOnGitLab: true,
                            arrayLabel:
                                lable.length > 0
                                    ? lable
                                    : [
                                          {
                                              color: 'rgb(135 224 252)',
                                              text_color: 'rgb(95 81 81)',
                                              name: 'Chưa có trạng thái',
                                          },
                                      ],
                            duedate: el.due_date,
                        }
                    }),
                )

                this.data = [...newData]
                return Promise.resolve(this.data)
            },
            async handlerFilterTaskOfStaff(idProject, idStaff, name) {
                await TaskService.filterTasksOfStaffByName(
                    idProject,
                    idStaff,
                    name,
                    this.resultPgae.pageNumber,
                    this.resultPgae.pageSize,
                )
                    .then((res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        var map = res.data._Data.map((el) => ({
                            assignee: el.assignee,
                            createUser: el.createUser,
                            dateCreated: el.dateCreated,
                            description: el.description,
                            duedate: el.duedate,
                            idProject: el.idProject,
                            idTask: el.idTask,
                            iid_issue: el.iid_issue,
                            isOnGitLab: el.isOnGitLab,
                            labels: el.labels,
                            status: el.status,
                            taskCode: el.taskCode,
                            taskName: el.taskName,
                            time_estimate: el.time_estimate,
                            total_time_spent: el.total_time_spent,
                            typeTask: el.typeTask,
                            arrayLabel: null,
                        }))
                        this.data = map
                    })
                    .catch((error) => {
                        this.totalMapPage = 0
                        this.totalItem = 0
                        console.log(error);
                    }).finally(() => { this.loading = false })
            },
            async handlerFilterTask_IMS(name, id) {
                await TaskService.filterTasksByName(id, name, this.resultPgae.pageNumber, this.resultPgae.pageSize)
                    .then((res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        var map = res.data._Data.map((el) => ({
                            assignee: el.assignee,
                            createUser: el.createUser,
                            dateCreated: el.dateCreated,
                            description: el.description,
                            duedate: el.duedate,
                            idProject: el.idProject,
                            idTask: el.idTask,
                            iid_issue: el.iid_issue,
                            isOnGitLab: el.isOnGitLab,
                            labels: el.labels,
                            status: el.status,
                            taskCode: el.taskCode,
                            taskName: el.taskName,
                            time_estimate: el.time_estimate,
                            total_time_spent: el.total_time_spent,
                            typeTask: el.typeTask,
                            arrayLabel: null,
                        }))
                        this.data = map
                    })
                    .catch((error) => {
                        this.totalMapPage = 0
                        this.totalItem = 0
                        console.log(error);
                    }).finally(() => { this.loading = false })
            },
            async filterDataByStaff(){
                var result = this.data.map(ele => {
                    return ele.assignee.id == checkAccessModule.getIdUserOnGitLab();
                })
                this.data = result;
            },
            async getTaskByProject() {
                this.data = [];
                this.booleanProject = true;
                this.loading = true;
                const selectedProject = Object.assign({},this.projects.find((el) => el.id === this.selectedProject),)
                if(Object.keys(selectedProject).length==0) {
                    this.loading = false;
                    return;
                }
                if (checkAccessModule.isAdmin() || checkAccessModule.isOffice()) {
                    if (selectedProject.isOnGitlab) {
                        await this.handlerGetAllTaskOnAPIGitLab(selectedProject.projectCode);
                    } else {
                        await this.handlerGetAllTaskOnAPI_IMS(selectedProject.id);
                    }
                } else {
                    if (selectedProject.isOnGitlab) {
                        if (
                            selectedProject.idLeader === Number(checkAccessModule.getUserIdCurrent()) ||
                            selectedProject.userId === Number(checkAccessModule.getUserIdCurrent())
                        ) {
                            this.gitlab = true
                            await this.handlerGetAllTaskOnAPIGitLabByUserAuthor(selectedProject.projectCode)
                        } else {
                            await this.handlerGetAllTaskOnAPIGitLabByUser(selectedProject.projectCode, checkAccessModule.getIdUserOnGitLab());
                        }
                    } else {
                        if (
                            selectedProject.idLeader === Number(checkAccessModule.getUserIdCurrent()) ||
                            selectedProject.userId === Number(checkAccessModule.getUserIdCurrent())
                        ) {
                            await this.handlerGetAllTaskOnAPI_IMS(selectedProject.id)
                            this.gitlab = false
                        } else {
                                await this.handlergetAllTaskOnAPI_IMSByIdStaff(
                                selectedProject.id,
                                checkAccessModule.getUserIdCurrent(),
                            )
                            this.gitlab = false
                        }
                    }
                }
                this.loading = false
            },
            async handlergetAllTaskOnAPI_IMSByIdStaff(id, idstaff) {
                await TaskService.getAllTaskWithPageByIdStaff(
                    id,
                    idstaff,
                    this.resultPgae.pageNumber,
                    this.resultPgae.pageSize,
                )
                    .then((res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        res.data._Data.map((el) => {
                            const object = {
                                assignee: el.assignee,
                                createUser: el.createUser,
                                dateCreated: el.dateCreated,
                                description: el.description,
                                duedate: el.duedate,
                                idProject: el.idProject,
                                idTask: el.idTask,
                                iid_issue: el.iid_issue,
                                isOnGitLab: el.isOnGitLab,
                                labels: el.labels,
                                status: el.status,
                                taskCode: el.taskCode,
                                taskName: el.taskName,
                                time_estimate: el.time_estimate,
                                total_time_spent: el.total_time_spent,
                                typeTask: el.typeTask,
                                arrayLabel: null,
                            }
                            this.data.push(object)
                        })
                        this.loading = false
                    })
                    .catch((error) => {
                        this.totalMapPage = 0
                        this.totalItem = 0
                        console.log(error);
                        this.loading = false
                    }).finally(() => { this.loading = false })
            },
            async handlerGetAllTaskOnAPI_IMS(id) {
                await TaskService.getAllTaskWithPage(id, this.resultPgae.pageNumber, this.resultPgae.pageSize)
                    .then((res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        res.data._Data.map((el) => {
                            const object = {
                                assignee: el.assignee,
                                createUser: el.createUser,
                                dateCreated: el.dateCreated,
                                description: el.description,
                                duedate: el.duedate,
                                idProject: el.idProject,
                                idTask: el.idTask,
                                iid_issue: el.iid_issue,
                                isOnGitLab: el.isOnGitLab,
                                labels: el.labels,
                                status: el.status,
                                taskCode: el.taskCode,
                                taskName: el.taskName,
                                time_estimate: el.time_estimate,
                                total_time_spent: el.total_time_spent,
                                typeTask: el.typeTask,
                                arrayLabel: null,
                            }
                            this.data.push(object)
                        })
                        this.loading = false
                    })
                    .catch((error) => {
                        this.totalMapPage = 0
                        this.totalItem = 0
                        console.log(error);
                        this.loading = false
                    }).finally(() => { this.loading = false })
            },
            showDialogAddTask() {
                this.isOpenDialogTask = true
            },
            closeDialogAddTask() {
                this.isOpenDialogTask = false
                this.selectedTaskEdit = null
            },
            showDialogUpdateTask(data) {
                this.isOpenDialogTask = true
                this.selectedTaskEdit = { ...data }
            },
            confirmDelete(task) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa công việc này?',
                    header: 'Xóa',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    rejectLabel: 'Hủy',
                    acceptLabel: 'Xóa',
                    acceptClass: 'p-button-danger CustomButtonPrimeVue',
                    rejectClass: 'p-button-secondary p-button-outlined CustomButtonPrimeVue',
                    accept: () => {
                        if (task.isOnGitLab) {
                            this.deleteTaskOnApiGitLab(task)
                        } else {
                            this.deleteTaskOnAPI(task)
                        }
                    },
                    reject: () => {
                        return
                    },
                })
            },
            async deleteTaskOnAPI(task) {
                await deleteTaskOnAPI(task.idTask, { userDelete: this.currenUser.Id })
                    .then(async (res) => {
                        await this.getTaskByProject()
                        this.successMessage('Xóa thành công!')
                    })
                    .catch((err) => {
                        this.WarningMessage('Có lỗi gì đó đã sải ra trong quá trình thực hiện!!!')
                        console.log(err)
                    })
            },
            async deleteTaskOnApiGitLab(task) {
                await TaskService.deleteTaskOnGitLab(this.project.projectCode, task.iid_issue, checkAccessModule.getTokenUserOnGitLab())
                    .then(async (res) => {
                        await this.getTaskByProject()
                        this.successMessage('Xóa thành công!')
                    })
                    .catch((err) => {
                        this.WarningMessage('Có lỗi gì đó đã sải ra trong quá trình thực hiện!!!')
                    })
            },
            handlerDetailsTask(data) {
                this.isOpenDialogDetailsTask = true
                this.taskDetail = { ...data }
            },
            closeDialogDetailTask() {
                this.isOpenDialogDetailsTask = false
                this.taskDetail = []
            },
            exportCSV() {
                this.$refs.dt.exportCSV()
            },
            successMessage(mess) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: mess,
                    life: 3000,
                })
            },
            WarningMessage(mess) {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Cảnh báo',
                    detail: mess,
                    life: 2000,
                })
            },
            async getProjects() {
                await HTTP.get('Project/getAllProject').then((res) => {
                    if (res.data) {
                        this.projects = res.data._Data
                        if (checkAccessModule.getListGroup().includes('5')) {
                            this.projects = this.projects.filter(
                                (item) => item.userCreated === Number(checkAccessModule.getUserIdCurrent()),
                            )
                        }
                    }
                })
            },
            async getProjectsByLead() {
                await HTTP.get(`Project/getAllProjectByLead/${checkAccessModule.getUserIdCurrent()}`).then((res) => {
                    if (res.data) {
                        res.data._Data.map((ele) => {
                            this.projects.push(ele)
                        })
                    }
                    this.projects = this.projects.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                })
            },
            async getProjectsByStaff() {
                await HTTP.get(`Project/getAllProjectByStaff/${checkAccessModule.getUserIdCurrent()}`).then((res) => {
                    res.data._Data.map((ele) => {
                        this.projects.push(ele)
                    })
                    this.projects = this.projects.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                })
            },

            async getProjectsByPM() {
                await HTTP.get(`Project/getAllProjectByPM/${checkAccessModule.getUserIdCurrent()}`).then((res) => {
                    res.data._Data.map((ele) => {
                        this.projects.push(ele)
                    })
                    this.projects = this.projects.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                })
            },

            getProjectName(idProject) {
                for (var p of this.projects) {
                    if (p.id === idProject) {
                        return p.name
                    }
                }
            },
            async handlerReload() {
                if (this.booleanProject) {
                    this.loading = true
                    this.filtersTaskName = ""

                    this.data = []
                    if (this.selectedProject != null) {
                        await this.getTaskByProject()
                    }
                }
            },
            FormatJSon(FilterData, JsonData) {
                return JsonData.map((v) =>
                    FilterData.map((j) => {
                        return v[j]
                    }),
                )
            },
            exportToExcelFollowRole() {
                this.dataExport = []
                this.data.map((ele) => {
                    const task = {
                        taskCode: ele.isOnGitLab ? ele.id : ele.taskCode,
                        taskName: ele.taskName,
                        dateCreated: DateHelper.formatDateDayjs(ele.dateCreated),
                        time_estimate: ele.time_estimate!=null ? ele.time_estimate: '',
                        total_time_spent: ele.total_time_spent!=null ? ele.total_time_spent: '',
                        duedate: ele.duedate!=null ? ele.duedate: '',
                    }
                    this.dataExport.push(task)
                })
                import('../../plugins/Export2Excel.js').then((excel) => {
                    const Header = [
                        'Mã công việc',
                        'Tên Công việc',
                        'Ngày tạo',
                        'Thời gian ước tính(Dự kiến)',
                        'Thời gian thực hiện(Thời gian làm)',
                        'Ngày kết thúc',
                    ]
                    const Field = [
                        'taskCode',
                        'taskName',
                        'dateCreated',
                        'time_estimate',
                        'total_time_spent',
                        'duedate',
                    ]
                    excel.export_json_to_excel({
                        header: Header,
                        data: this.FormatJSon(Field, this.dataExport),
                        sheetName: `Sheet1`,
                        filename: `Danh sách công việc ${DateHelper.formatDateDayjs(new Date())}`,
                        autoWidth: true,
                        bookType: 'xlsx',
                    })
                })
            },
            showExportBill() {
                this.isOpenBill = true
            },
            closeExportBill() {
                this.isOpenBill = false
                this.selectedTaskEdit = null
            },
        },
        components: { DialogDetailsTask, Export, DialogAddTask, UpdateStatusTask, ExportBill },
    }
</script>

<style lang="scss" scoped>
    .tags-status span {
        display: inline-block;
        height: 24px;
        line-height: 24px;
        position: relative;
        margin: 0 5px 0 0;
        padding: 0 10px 0 12px;
        background: #777;
        -webkit-box-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
        box-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
        color: #fff;
        font-size: 12px;
        font-family: 'Lucida Grande', 'Lucida Sans Unicode', Verdana, sans-serif;
        text-decoration: none;
        text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
        font-weight: bold;
        border-radius: 4px;
    }

    .tags-status span:after {
        content: '';
        position: absolute;
        top: 10px;
        left: 3px;
        float: left;
        width: 5px;
        height: 5px;
        -webkit-border-radius: 50%;
        border-radius: 50%;
        background: #fff;
        -webkit-box-shadow: -1px -1px 2px rgba(0, 0, 0, 0.4);
        box-shadow: -1px -1px 2px rgba(0, 0, 0, 0.4);
    }
    .actions-buttons {
        display: flex;

        .btn-margin {
            margin-right: 5px;
        }
    }
</style>
