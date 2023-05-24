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
                            <li class="breadcrumb-item active" aria-current="page">{{this.getNameProjectSelected!=null ? 'Time sheet '+this.getNameProjectSelected.name.toLowerCase():'Time sheet'}}</li>
                        </ol>
                    </nav>
                </div>
            </div>
            
            <nav class="navbar navbar-expand-lg bg-light navbar-light">
                <div class="container-fluid">
                    <button class="navbar-toggler mb-2 mt-2 custom-input-h" type="button" data-bs-toggle="collapse" data-bs-target="#collapsibleNavbar">
                        <span class="navbar-toggler-icon"></span>
                    </button>
                    <div class="collapse navbar-collapse mt-2" id="collapsibleNavbar">
                        <ul class="navbar-nav me-auto">
                            <li class="nav-item me-2 mb-2">
                                <Export
                                    class="custom-button-h"
                                    label="Xuất Excel"
                                    @click="exportTimeSheet($event)"
                                    :disabled="this.listTimeSheet.length<=0"
                                    v-if="this.showButton.export"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2" v-if="!this.isProjectFinished">
                                <Add
                                    class="custom-button-h"
                                    label="Thêm mới"
                                    @click="openAddEdit(null)"
                                    v-if="this.showButton.add && this.isUserAddTimeSheet"
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
                                    @click="cancelFilter()"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <date-picker
                                    v-model:value="selectedDate"
                                    type="month"
                                    placeholder="MM/YYYY"
                                ></date-picker>
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Dropdown
                                    v-model="selectedProject"
                                    :options="listProjectUsers"
                                    filter
                                    optionLabel="name"
                                    optionValue="projectCode"
                                    placeholder="Chọn dự án"
                                    style="width: 100%;"
                                    class="custom-input-h"
                                    :disabled="selectedDate==null"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2" v-if="isStaff()">
                                <Dropdown
                                    v-model="selectedUser"
                                    :options="usersList"
                                    filter
                                    optionLabel="fullName"
                                    optionValue="id"
                                    placeholder="Chọn nhân viên"
                                    style="width: 100%;"
                                    class="custom-input-h"
                                    :disabled="selectedProject==null"
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        ref="tableTimeSheet"
                        showGridlines
                        removableSort 
                        :value="listTimeSheet"
                        :sort="1"
                        :loading="isLoading"
                        responsiveLayout="scroll"
                        filterDisplay="menu"
                        :rows="10"
                        v-model:filters="filters"
                        rowGroupMode="rowspan" 
                        :groupRowsBy="['date']" 
                        sortMode="single" 
                        sortField="date" 
                        :sortOrder="1"
                        resizableColumns 
                        columnResizeMode="expand"
                        :exportFilename="'List_TimeSheet_Effort Summary Report_' + new Date()"
                        :globalFilterFields="['layout','spec','api','web','utc','ute','integration','deployment','fixbug','support','others','sum']"
                        >
                        <template #loading> </template>

                        <template #empty>
                            <div v-if="this.isLoading" style="display: flex; justify-items: flex-end">
                                <ProgressSpinner style="width: 42px" />
                            </div>
                            <div v-else>Không tìm thấy.</div>
                        </template>

                        <Column field="#" header="#" dataType="date" style="width: 2%">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>

                        <Column field="date" header="Ngày" sortable style="width: 5%">
                            <template #body="{ data }">
                                {{ data.date}}
                            </template>
                        </Column>

                        <Column field="taskContent" header="Nội dung">
                            <template #body="{ data }">
                                {{ data.taskContent }}
                            </template>
                        </Column>

                        <Column field="layout" header="Layout" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.layout }}
                            </template>
                        </Column>

                        <Column field="spec" header="Spec" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.spec }}
                            </template>
                        </Column>

                        <Column field="api" header="API" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.api }}
                            </template>
                        </Column>

                        <Column field="web" header="WEB" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.web }}
                            </template>
                        </Column>

                        <Column field="utc" header="UTC" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.utc }}
                            </template>
                        </Column>

                        <Column field="ute" header="UTE" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.ute }}
                            </template>
                        </Column>

                        <Column field="integration" header="Integration" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.integration }}
                            </template>
                        </Column>

                        <Column field="deployment" header="Deployment" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.deployment }}
                            </template>
                        </Column>

                        <Column field="fixbug" header="Fixbug" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.fixbug }}
                            </template>
                        </Column>

                        <Column field="support" header="Support" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.support }}
                            </template>
                        </Column>

                        <Column field="others" header="Others" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.others }}
                            </template>
                        </Column>
                        
                        <Column field="date" header="Sum" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.sum }}
                            </template>
                        </Column>

                        <ColumnGroup type="footer">
                            <Row>
                                <Column footer="Tổng" :colspan="3" footerStyle="text-align:center"/>
                                <Column :footer="listTimeSheetTotal.layoutTotal" />
                                <Column :footer="listTimeSheetTotal.specTotal" />
                                <Column :footer="listTimeSheetTotal.apiTotal" />
                                <Column :footer="listTimeSheetTotal.webTotal" />
                                <Column :footer="listTimeSheetTotal.utcTotal" />
                                <Column :footer="listTimeSheetTotal.uteTotal" />
                                <Column :footer="listTimeSheetTotal.integrationTotal" />
                                <Column :footer="listTimeSheetTotal.deploymentTotal" />
                                <Column :footer="listTimeSheetTotal.fixbugTotal" />
                                <Column :footer="listTimeSheetTotal.supportTotal" />
                                <Column :footer="listTimeSheetTotal.othersTotal" />
                                <Column :footer="listTimeSheetTotal.sumTotal" />
                                <Column />
                            </Row>
                        </ColumnGroup>

                        <Column field="date" header="Thực thi" style="width: 5%; text-align: left">
                            <template #body="{ data }">
                                <div class="actions-buttons">
                                    <Button
                                        icon="pi pi-pencil"
                                        class="p-button p-button-warning btn-margin custom-button-h"
                                        @click="openAddEdit(data)"
                                        v-if="this.showButton.update && data.isAction && !this.isProjectFinished"
                                    ></Button>
                                    <Button
                                        icon="pi pi-trash"
                                        class="p-button p-button-danger btn-margin custom-button-h"
                                        @click="confirmDelete(data)"
                                        v-if="(this.showButton.deleteMulti && data.isAction && !this.isProjectFinished) || this.isAdmin()"
                                    ></Button>
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
                <Button label="Hoàn tất" icon="pi pi-check" @click="navigationToHome()" autofocus class="custom-button-h"/>
            </template>
        </Dialog>

        <AddEditTimeSheet 
            :statusopen="openAddEditform" 
            :dataTimeSheetById="dataTimeSheetById"
            :listProjectUsers="listProjectUsers"
            :projectSelectedToAdd="selectedProject"
            @closeAddEdit="closeAddEdit()" 
            @loadingToHandlerAddEdit="loadingToHandlerAddEdit()"
            @reloadPageListTimeSheet="handlerReload"/>

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
    import AddEditTimeSheet from '@/views/TimeSheet/AddEditTimeSheet.vue'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { HttpStatus } from '@/config/app.config'
    import { ref } from 'vue'
    import { DateHelper } from '@/helper/date.helper'
    import { saveAs } from 'file-saver'
    import * as XLSX  from 'xlsx-js-style'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import { DataTimeSheet, DataTimeSheetTotal } from './DataTimeSheet'
    import { FormatExcel} from '@/helper/formatExcel.helper'
    export default {
        name: 'ListTimeSheet',
        components: { Export, Add, Edit, View, Delete, AddEditTimeSheet },

        data() {
            return {
                dataTimeSheetById: null,
                listTimeSheet: [],
                listTimeSheetTotal: [],
                usersList: [],
                selectedDate: null,
                selectedProject: null,
                selectedUser: null,
                openAddEditform: false,
                isLoading: false,
                displayBasic: false,
                listProjectUsers: [],
                getNameProjectSelected: null,
                num: 5,
                timeOut: null,
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
                isProjectFinished: false,
                isUserAddTimeSheet: true,
            }
        },
        async mounted() {
            if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '').slice(0,15)) === true) {
                checkAccessModule.checkPermissionAction(this.$route.path.replace('/', '').slice(0,15), this.showButton);
                this.selectedDate = new Date();
                await this.getProjectByUser(checkAccessModule.getUserIdCurrent());
                this.selectedProject = this.$route.params.idProject;
                this.selectedUser = checkAccessModule.getUserIdCurrent();
            } else {
                this.countTime();
                this.displayBasic = true;
            }
        },
        watch: {
            selectedDate: {
                handler: async function change(newVal) {
                    if (newVal != null && this.selectedProject != null) {
                        await this.searchByUserOrDate();
                    }
                    else if(newVal==null){
                        this.listTimeSheet = [];
                        this.listTimeSheetTotal = [];
                        this.selectedUser = null;
                        this.selectedProject = null;
                        this.getNameProjectSelected = null;
                    }
                },
                deep: true,
            }, 
            selectedProject: {
                handler: async function change(newVal) {
                    if (newVal != null) {
                        await this.searchByUserOrDate();
                        await this.getAllUserInProject();
                        this.getNameProjectSelectedFunc();
                    }
                },
                deep: true,
            }, 
            selectedUser: {
                handler: async function change(newVal) {
                    if (newVal != null) {
                        await this.searchByUserOrDate();
                        this.checkUserCurrent();
                    }
                },
                deep: true,
            },             
            resultPgae: {
                handler: async function change() {
                    if (this.selectedDate != null || this.selectedUser != null || this.selectedProject != null) {
                        await this.searchByUserOrDate();
                    } else {
                        this.listTimeSheet = [];
                        this.listTimeSheetTotal = [];
                    }
                },
                deep: true,
            },
        },
        methods: {
            goToOverViews() {
                this.$router.push({ name: 'home'})
            },
            goToProjects() {
                this.$router.push({ name: 'project'})
            },
            countTime() {
                if (this.num == 0) {
                    this.navigationToHome();
                    return
                }
                this.num = this.num - 1
                this.timeOut = setTimeout(() => this.countTime(), 1000)
            },
            navigationToHome() {
                clearTimeout(this.timeOut)
                this.$router.push({ name: 'home' })
            },
            loadingToHandlerAddEdit() {
                this.isLoading = true;
            },
            async cancelFilter(){
                this.selectedUser = null;
                this.checkUserCurrent();
                await this.searchByUserOrDate();
            },
            checkUserCurrent() {
                if(this.selectedUser == checkAccessModule.getUserIdCurrent()){
                    this.isUserAddTimeSheet = true;
                }
                else {
                    this.isUserAddTimeSheet = false;
                }
            },
            async exportTimeSheet(){
                var dataExport = [
                    ['Ngày', 'Nội dung', 'Layout', 'Spec', 'Api', 'Web', 'Utc', 'Ute', 'Integration', 'Deployment', 'Fixbug', 'Support','Others', 'Sum',],
                ]
                this.listTimeSheet.map(ele => {
                    var obj = [
                        ele.date,
                        ele.taskContent,
                        ele.layout==null?'':ele.layout,
                        ele.spec==null?'':ele.spec,
                        ele.api==null?'':ele.api,
                        ele.web==null?'':ele.web,
                        ele.utc==null?'':ele.utc,
                        ele.ute==null?'':ele.ute,
                        ele.integration==null?'':ele.integration,
                        ele.deployment==null?'':ele.deployment,
                        ele.fixbug==null?'':ele.fixbug,
                        ele.support==null?'':ele.support,
                        ele.others==null?'':ele.others,
                        ele.sum==null?'':ele.sum,]
                    dataExport.push(obj)
                })
                const ws = XLSX.utils.aoa_to_sheet(dataExport);
                                
                var prevData = '';
                var startDate = 0;
                var nextDate = 0;
                ws['!merges'] = ws['!merges'] || [];

                for (var i = 1; i < dataExport.length; i++) {
                    const currentData = ws[XLSX.utils.encode_cell({ r: i, c: 0 })].v;
                    if (currentData === prevData) {
                        nextDate = i;
                    } else {
                        if(nextDate > startDate) {
                            ws['!merges'].push(
                            { s: { r: startDate, c: 0 },e: { r: nextDate, c: 0 } }, 
                            { s: { r: startDate, c: 13 },e: { r: nextDate, c: 13 } }); 
                        }
                        startDate = i;
                        prevData = currentData;
                    }
                }
                if(nextDate > startDate) {
                    ws['!merges'].push(
                    { s: { r: startDate, c: 0 },e: { r: nextDate, c: 0 } }, 
                    { s: { r: startDate, c: 13 },e: { r: nextDate, c: 13 } }); 
                }
                const wb = XLSX.utils.book_new();
                XLSX.utils.sheet_add_aoa(ws, [
                    ["","Tổng",this.listTimeSheetTotal.layoutTotal, this.listTimeSheetTotal.specTotal, this.listTimeSheetTotal.apiTotal, this.listTimeSheetTotal.webTotal, this.listTimeSheetTotal.utcTotal, this.listTimeSheetTotal.uteTotal, this.listTimeSheetTotal.integrationTotal, this.listTimeSheetTotal.deploymentTotal, this.listTimeSheetTotal.fixbugTotal, this.listTimeSheetTotal.supportTotal, this.listTimeSheetTotal.othersTotal, this.listTimeSheetTotal.sumTotal,],
                ],  { origin: -1 }, );
                XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');

                var range = XLSX.utils.decode_range(ws['!ref']);
                var sheet = wb.Sheets[wb.SheetNames[0]];
                range.s.r = 0; 
                range.e.r = 0; 
                range.s.c = 0; 
                range.e.c = 13; 
                for (var C = range.s.c; C <= range.e.c; ++C) {
                    const headerCell = XLSX.utils.encode_cell({ r: range.s.r, c: C });
                    sheet[headerCell].s = FormatExcel.Header; 
                }
                range.s.r = 1; 
                range.e.r = dataExport.length-1; 
                range.s.c = 0; 
                range.e.c = 13; 
                for (var C = range.s.c; C <= range.e.c; ++C) {
                    for (var R = range.s.r; R <= range.e.r; ++R){
                        const headerCell = XLSX.utils.encode_cell({ r: R, c: C });
                        if(C == 1) sheet[headerCell].s = FormatExcel.ElementContent1;
                        else sheet[headerCell].s = FormatExcel.Body;
                    }
                }
                range.s.r = dataExport.length; 
                range.e.r = dataExport.length; 
                range.s.c = 0; 
                range.e.c = 13; 
                for (var C = range.s.c; C <= range.e.c; ++C) {
                    const headerCell = XLSX.utils.encode_cell({ r: range.s.r, c: C });
                    sheet[headerCell].s = FormatExcel.Header;
                }
                var colWidths = [];
                for (let i = 0; i < 13; i++) {
                    if(i==0) colWidths.push({ wch: 13 }); 
                    else if(i==1) colWidths.push({ wch: 30 }); 
                    else colWidths.push({ wch: 12 }); 
                }
                ws['!cols'] = colWidths;
                var nameFile = 'TimeSheetDailys'+DateHelper.formatDateDayjs(new Date())+'.xlsx'
                XLSX.writeFile(wb, nameFile);
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

            openAddEdit(dataTimeSheetById) {
                this.dataTimeSheetById = dataTimeSheetById;
                this.openAddEditform = true;
            },

            closeAddEdit() {
                this.openAddEditform = false;
            },
            async handlerReload(data){
                this.selectedDate = data.date;
                this.selectedProject = data.project;                
                await this.getProjectByUser(checkAccessModule.getUserIdCurrent());
                await this.searchByUserOrDate();
                this.checkUserCurrent();
            },
            async getAllUserInProject() {
                var getByIdProject = this.listProjectUsers.find((ele) => ele.projectCode == this.selectedProject)
                if(getByIdProject==undefined) return;
                await HTTP.get(`Users/getAllUsersByIdProjectByIdUser/${getByIdProject.id}/${checkAccessModule.getUserIdCurrent()}`)
                    .then((res) => {
                        this.usersList = res.data._Data;
                    })
                    .catch((err) => {
                        console.log(err);
                    })
            },
            async getAllProject() {
                await HTTP.get(`/Project/getAllProject`)
                    .then((res) => {
                        this.listProjectUsers = res.data._Data;
                    })
                    .catch((err) => console.log(err));
            },
            async getProjectByLead(idLead) {
                await HTTP.get(`/Project/getAllProjectByLead/${idLead}`)
                .then(res => {
                    res.data._Data.map(ele => {
                        this.listProjectUsers.push(ele);
                    })
                })
                .catch((err) => {
                    console.log(err);
                });
            },

            async getProjectByStaff(idStaff) {
                await HTTP.get(`/Project/getAllProjectByStaff/${idStaff}`)
                    .then((res) => {
                        this.listProjectUsers =  res.data._Data;
                    })
                    .catch((err) => console.log(err));
            },
        async getProjectByUser(idUser) {
            this.listProjectUsers = []
            if (checkAccessModule.checkCallAPI(this.$route.path.replace('/', '').slice(0,15))) {
                await this.getAllProject();
            } else {
                if (checkAccessModule.isLead()) {
                    await this.getProjectByLead(idUser);
                }
                if (checkAccessModule.isStaff()) {
                    await this.getProjectByStaff(idUser);
                }
            }
            var result = []
            this.listProjectUsers.map(ele => {
                if(ele.isOnGitlab==false) result.push(ele);
            });
            this.listProjectUsers = [];
            this.listProjectUsers = result;
        },
        async calculationTotalTimeSheet() {
            var linkGetApi = `TimeSheetDaily/ListTimeSheetTotal/`
            if (this.selectedUser != null) {
                linkGetApi += `${this.resultPgae.pageSize}/${this.selectedUser}/${this.selectedProject}`;
            } else {
                linkGetApi += `${this.resultPgae.pageSize}/${checkAccessModule.getUserIdCurrent()}/${
                    this.selectedProject
                }`;
            }
            await HTTP.get(linkGetApi)
                .then((res) => {
                    this.listTimeSheetTotal = res.data._Data;
                })
                .catch((err) => {
                    console.log(err);
                })
        },
        isAdmin() {
            if (checkAccessModule.isAdmin()) {
                return true;
            }
            return false;
        },
        isStaff() {
            if (checkAccessModule.isStaff()) {
                return false;
            }
            return true;
        },
        isUserCurrent() {
            if(this.selectedUser==null){
                return true;
            }
            return false;
        },
        async deleteTimeSheet(data) {
            var objDelete = {
                idUser: data.idUser,
                date: data.date,
                idProject: data.idProject,
            }
            await HTTP.put(`TimeSheetDaily/DeleteByDateTimeSheetDaily`, objDelete)
                .then(async (res) => {
                    if (res.status == HttpStatus.OK) {
                        this.showSuccess('Xóa thành công!');
                        await this.searchByUserOrDate();
                    }
                })
                .catch((error) => {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText;
                    this.showResponseApi(error.response.status, message);
                })
        },

        async confirmDelete(data) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn muốn xóa những công việc trong ngày ' + data.date + ' ?',
                header: 'Xóa',
                icon: 'pi pi-question-circle',
                rejectLabel: 'Hủy',
                acceptLabel: 'Xóa',
                acceptIcon: 'pi pi-trash',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-danger CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined CustomButtonPrimeVue',
                accept: async () => {
                    await this.deleteTimeSheet(data);
                },
                reject: () => {
                    return;
                },
            })
        },

        getNameProjectSelectedFunc() {
            this.getNameProjectSelected = this.listProjectUsers.find(ele => ele.projectCode == this.selectedProject);
            this.isProjectFinished = this.getNameProjectSelected.isFinished;
        },       
        async searchByUserOrDate(){
            var formatDate = DateHelper.formatMonthYearjs(this.selectedDate);
            var linkSearch = `TimeSheetDaily/SearchByDateOrIdUserTimeSheetDaily`;
            if(this.selectedProject!=null && this.selectedUser==null){
                linkSearch += `?idProject=${this.selectedProject}&date=${formatDate}&idUserCurrent=${checkAccessModule.getUserIdCurrent()}`;
            }
            else if(this.selectedProject!=null && this.selectedUser!=null){
                linkSearch += `?idUser=${this.selectedUser}&idProject=${this.selectedProject}&date=${formatDate}`;
            }
            else if(this.selectedProject==null && this.selectedUser==null){
                this.isLoading = false;
                return;
            }
            linkSearch += `&pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`;

            this.listTimeSheet = [];
            this.listTimeSheetTotal = [];
            this.isLoading = true;
            this.totalMapPage = 0;
            this.totalItem = 0;
            await HTTP.get(linkSearch)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages;
                    this.totalItem = res.data._totalItems;
                    this.itemIndex = res.data._itemIndex;
                    this.listTimeSheet = res.data._Data;
                })
                .catch((err) => {
                    console.log(err)
                })
                .finally(() => {
                    this.isLoading = false;
                });

            if(this.listTimeSheet.length > 0){
                await this.calculationTotalTimeSheet();      
            }else {
                if(this.selectedProject!=null && this.selectedUser==null){
                    this.showInfo("Không tìm thấy TimeSheet cho dự án này!");
                }
                else if(this.selectedProject!=null && this.selectedUser!=null){
                    this.showInfo("Không tìm thấy TimeSheet của nhân viên trong dự án này!");
                }
            }    
        }
    },
}
</script>

<style lang="scss" scoped>
    ::v-deep(.p-dialog .p-dialog-footer) {
        padding: 1rem 0.5rem 0.7rem 1.5rem !important;
        display: flow-root !important;
    }
    .actions-buttons {
        display: flex;

        .btn-margin {
            margin-right: 5px;
        }
    }
</style>
