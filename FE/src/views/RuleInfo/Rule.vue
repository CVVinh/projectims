<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">QUY ĐỊNH</li>
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
                            <li class="nav-item me-2 mb-2 ">
                                <Button
                                    label="Thêm quy định"
                                    icon="pi pi-plus"
                                    @click="openAdd(null)"
                                    class="p-button-sm custom-button-h me-2" 
                                    v-if="this.showButton.add"
                                />
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2 ">
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite"
                                    icon="pi pi-filter-slash"
                                    class="custom-reload-h"
                                    @click="handlerReload"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText        
                                    v-model="keySearch"
                                    placeholder="Tìm kiếm theo tiêu đề"
                                    class="custom-input-h"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <MultiSelect
                                    v-model="selectedColumns"
                                    :options="columns"
                                    optionLabel="header"
                                    placeholder="Select Columns"
                                    class="custom-input-h"
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-12">
                    <DataTable
                        ref="tableRule"
                        showGridlines
                        class="p-datatable-customers"
                        :value="listRule"
                        :sort="1"
                        :loading="isLoading"
                        responsiveLayout="scroll"
                        filterDisplay="menu"
                        :rowHover="true"
                        :rows="5"
                        dataKey="idRule"
                        v-model:filters="filters"
                        :exportFilename="'List_Rules_Effort Summary Report_' + new Date()"
                        :globalFilterFields="[
                            'title',
                            'applyDay',
                            'expiredDay',
                            'content',
                            'userCreated',
                            'dateCreated',
                            'userUpdated',
                            'dateUpdated',
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

                        <Column field="title" header="Tiêu đề" sortable style="min-width: 15rem">
                            <template #body="{ data }">
                                {{ data.title }}
                            </template>
                        </Column>

                        <Column field="dateCreated" header="Ngày tạo " sortable>
                            <template #body="{ data }">
                                {{ data.dateCreated }}
                            </template>
                        </Column>

                        <Column field="applyDay" header="Ngày áp dụng" sortable>
                            <template #body="{ data }">
                                {{ data.applyDay }}
                            </template>
                        </Column>

                        <Column field="expiredDay" header="Ngày hết hạn " sortable>
                            <template #body="{ data }">
                                {{ data.expiredDay }}
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
                                <div class="actions-buttons">
                                    <Button
                                        icon="pi pi-eye"
                                        class="p-button p-component p-button-info btn-margin custom-button-update"
                                        @click="openDetailt(data)"
                                        v-tooltip.top="'Xem chi tiết'"
                                    ></Button>
                                    <Button
                                        icon="pi pi-pencil"
                                        class="p-button p-component p-button-warning btn-margin custom-button-update"
                                        @click="openEdit(data)"
                                        v-if="showButton.update"
                                        :disabled="checkCanOperation('sua',data)"
                                    ></Button>
                                    <Button
                                        icon="pi pi-trash"
                                        class="p-button p-component p-button-danger btn-margin custom-button-update"
                                        @click="confirmDelete(data.id)"
                                        v-if="showButton.delete"
                                        :disabled="checkCanOperation('xoa',data)"
                                    ></Button>

                                    <Button 
                                        v-if="showButton.export && data.fileCode!=null"
                                        icon="pi pi-download"
                                        class="p-button p-component p-button-help custom-button-update"
                                        @click="saveFile(data)"
                                        v-tooltip.top="'Tải file'"
                                    ></Button>
                                    <!-- <a v-if="data.fileCode && showButton.export" class="btn btn-secondary"  :href="this.linkFile + 'Rules/downloadFile/'+data.id"><i style="font-size:25px;" class="fa fa-download"></i></a> -->
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
                <Button label="Hoàn tất" icon="pi pi-check" @click="navigationToHome" autofocus class="custom-button-h"/>
            </template>
        </Dialog>

        <AddRuleDiaLog :statusopen="openAddform" @closeAdd="closeAdd()" @reloadpage="GetAllRuleList" />
        <EditRuleDiaLog
            :statusopen="openEditform"
            @closeEdit="closeEdit()"
            :dataRuleById="dataRuleById"
            @reloadpage="GetAllRuleList"
        />

        <DetailtRuleDiaLog :statusopen="openDetailtform" @closeDetailt="closeDetailt()" :dataRuleById="dataRuleById" />
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
    import AddRuleDiaLog from './AddRuleDiaLog.vue'
    import EditRuleDiaLog from './EditRuleDiaLog.vue'
    import DetailtRuleDiaLog from './DetailtRuleDiaLog.vue'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { HttpStatus } from '@/config/app.config'
    import { ref } from 'vue'
    import { DateHelper } from '@/helper/date.helper'
    import { saveAs } from 'file-saver'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import EditorCustom from '@/components/buttons/EditorCustom.vue'

    export default {
        name: 'RuleInfo',

        data() {
            return {
                dataRuleById: null,
                listRule: [],
                selectedColumns: null,
                openAddform: false,
                openEditform: false,
                openDetailtform: false,
                isLoading: true,
                displayBasic: false,
                keySearch: '',
                num: 5,
                timeOut: null,
                listGroup: null,
                linkFile:LinkDownloadFile,
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                columns: [
                    { field: 'userCreated', header: 'Người tạo ' },
                    { field: 'userUpdated', header: 'Người sửa ' },
                    { field: 'dateUpdated', header: 'Ngày sửa' },
                ],
                showButton: {
                    add: false,
                    update: false,
                    delete: false,
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
            keySearch: {
                handler: async function change(newVal) {
                    this.listRule = null
                    if (newVal == '') {
                        await this.GetAllRuleList()
                    } else {
                        this.isLoading = true
                        await this.getRuleByName()
                    }
                },
                deep: true,
            },
            resultPgae: {
                handler: async function change() {
                    if (this.keySearch != '') {
                        await this.getRuleByName()
                    } else {
                        await this.GetAllRuleList()
                    }
                },
                deep: true,
            },
        },
        methods: {
            showError(message) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
            },

            showSuccess(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
            },

            showInfo(message) {
                this.$toast.add({ severity: 'info', summary: 'Thông báo', detail: message, life: 3000 })
            },

            btnUserShow(data) {
                if (checkAccessModule.getListGroup().includes('1')) {
                    return true
                }
                if (
                    checkAccessModule.getListGroup().includes('2') &&
                    parseInt(data.userCreated) == parseInt(checkAccessModule.getUserIdCurrent())
                ) {
                    return true
                } else {
                    return false
                }
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

            async handlerReload() {
                this.keySearch = ''
                this.listRule = null
                this.isLoading = true
                await this.GetAllRuleList()
            },

            formatData() {
                this.listRule.forEach((element) => {
                    element.applyDay = DateHelper.formatDate(element.applyDay)
                    element.expiredDay = DateHelper.formatDate(element.expiredDay)
                    element.dateCreated = DateHelper.formatDate(element.dateCreated)
                    element.dateUpdated = DateHelper.formatDate(element.dateUpdated)
                })
            },
            
            async getRuleByName() {
                await HTTP.get(
                    `Rules/searchRulesByTitle/${this.keySearch}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
                )
                    .then((res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex

                        this.listRule = res.data._Data
                        this.formatData()
                        this.isLoading = false
                    })
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            async GetAllRuleList() {
                await HTTP.get(
                    `Rules/getAllRules?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
                )
                    .then((res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex

                        this.listRule = res.data._Data
                        this.formatData()
                        this.isLoading = false
                    })
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            deleteRule(ruleID) {
                let API_URL = 'Rules/deleteRules/' + ruleID
                HTTP.put(API_URL, {
                    idUser: checkAccessModule.getUserIdCurrent(),
                })
                    .then(async (res) => {
                        if (res.status == HttpStatus.OK) {
                            this.showSuccess('Xóa thành công!')
                            this.listRule = []
                            await this.GetAllRuleList()
                        }
                    })
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            confirmDelete(id) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa quy định  này?',
                    header: 'Xóa quy định',
                    icon: 'pi pi-exclamation-triangle',
                    acceptLabel: 'Xóa',
                    rejectLabel: 'Hủy',
                    acceptIcon: 'pi pi-trash',
                    rejectIcon:'pi pi-times',
                    acceptClass:'p-button-danger CustomButtonPrimeVue',
                    rejectClass:'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                    accept: () => {
                        this.deleteRule(id)
                    },
                    reject: () => {
                        return
                    },
                })
            },

            countTime() {
                if (this.num == 0) {
                    this.navigationToHome()
                    return
                }
                this.num = this.num - 1
                this.timeOut = setTimeout(() => this.countTime(), 1000)
            },

            navigationToHome() {
                clearTimeout(this.timeOut)
                this.$router.push({ name: 'home' })
            },

            exportCSV() {
                this.$refs.tableRule.exportCSV()
            },

            openEdit(dataRuleById) {
                this.dataRuleById = dataRuleById
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

            openDetailt(dataRuleById) {
                this.dataRuleById = dataRuleById
                this.openDetailtform = true
            },

            closeDetailt() {
                this.openDetailtform = false
            },

            downloadFile(url) {
                var nameFile = url.substring(url.lastIndexOf('/') + 1)
                HTTP.get(url, { responseType: 'blob' })
                    .then((response) => {
                        saveAs(response.data, nameFile)
                    })
                    .catch((error) => {
                        if (error.code == 'ERR_BAD_REQUEST') {
                            this.showError('File không tồn tại!')
                        }
                    })
            },

            async saveFile(data){
                var accessToken = localStorage.getItem("token"); 
                var request = new XMLHttpRequest();
                request.open('GET', LinkDownloadFile+'Rules/downloadFile/'+data.id);
                request.setRequestHeader('Authorization', `Bearer ${accessToken}`);
                request.responseType = 'blob';
                request.onload = function() {
                    var blob = request.response;
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = data.fileName;
                    link.click();
                };
                request.send();
            },

            async downloadFileRule(data) {
                return await HTTP.get(`Rules/downloadFile/${data.id}`)
                    .then(async (res) => {
                        if (res.status == HttpStatus.OK) {
                            //saveAs(res.data, ""+data.fileName);
                            this.showSuccess('Download file thành công!');
                        }
                    })
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
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
                if(checkAccessModule.isAdmin()){
                    return false
                }else{
                    if(checkAccessModule.isOffice() && data.userCreated === Number(checkAccessModule.getUserIdCurrent())){
                        return false
                    }else{
                        return true
                    }
                }
            }
            if (name === 'xoa') {
                if(checkAccessModule.isAdmin()){
                    return false
                }else{
                    if(checkAccessModule.isOffice() && data.userCreated === Number(checkAccessModule.getUserIdCurrent())){
                        return false
                    }else{
                        return true
                    }
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
            checkRoleSample() {
                return this.listGroup.includes('sample')
            },
        },
        async created() {
            if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '')) === true) {
                checkAccessModule.checkPermissionAction(this.$route.path.replace('/', ''), this.showButton)
                await this.GetAllRuleList()
                this.loading = false
        } else {
            this.countTime();
            this.displayBasic = true;
        }
        },
        components: { Export, Add, Edit, View, Delete, AddRuleDiaLog, EditRuleDiaLog, DetailtRuleDiaLog, EditorCustom },
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
    .header-container {
        display: flex;
        justify-content: space-between;
        align-items: center;
        // margin-bottom: 1rem;
        // height: 40px;
    }
    .input-text {
        display: flex;
        height: 45px;
    }
    .actions-buttons {
        display: flex;

        .btn-margin {
            margin-right: 5px;
        }
    }
</style>
