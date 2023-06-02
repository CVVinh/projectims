<template>
    <LayoutDefaultDynamic>
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item" @click="goToOverViews()">Tổng quan</li>
                            <li class="breadcrumb-item active" aria-current="page">{{this.showProjectName}}</li>
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
                                    class="p-button-sm custom-button-h"
                                    @click="openBasic()"
                                    icon="pi pi-plus"
                                    label="Thêm thành viên vào dự án"
                                ></Button>
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2 ">
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
                                    v-model="filterMemberName"
                                    placeholder="Tìm kiếm"
                                    class="custom-input-h"
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
                        class="p-datatable-customers"
                        showGridlines
                        :rows="5"
                        dataKey="id"
                        :loading="loading"
                        :rowHover="true"
                        v-model:filters="filters"
                        filterDisplay="menu"
                        :globalFilterFields="['firstName', 'idGroup']"
                        responsiveLayout="scroll"
                    >
                        <template #empty> Không tìm thấy. </template>
                        <Column field="id" header="#" style="min-width: 5rem">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="fullName" header="Họ và tên" sortable style="min-width: 15rem"> </Column>
                        <Column field="" header="Thực thi"  >
                            <template #body="{ data }">
                                <Delete @click="openDeleteconfirm(data.id, this.$route.params.id)" class="custom-button-update" />
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
            <confirmDelete
                :display="this.display"
                @close="closeDeleteconfirm"
                @delete="deleteMember(this.iduser, this.idproject)"
            />
        </div>

        <Dialog
            header="Thêm thành viên vào dự án"
            :visible="displayBasic"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            modal
            :closable="false"
        >
            <MultiSelect 
                filter
                v-model="selectedMember"
                :options="data1"
                optionLabel="fullName"
                optionValue="idUserGitLab"
                placeholder="Chọn thành viên"
                :maxSelectedLabels="3"
                class="w-full md:w-20rem"
                v-if="this.projectData.isOnGitlab && this.projectData.projectCode === this.projectData.subProjectCode"
            />
            <MultiSelect 
                filter
                v-model="selectedMember"
                :options="data1"
                optionLabel="fullName"
                optionValue="id"
                placeholder="Chọn thành viên"
                :maxSelectedLabels="3"
                class="w-full md:w-20rem"
                v-else
            />
            <template #footer>
                <div class="d-flex justify-content-end">
                    <button-custom @click="closeBasic"  />       
                    <Button
                        label="Lưu"
                        icon="pi pi-check"
                        type="submit"
                        @click="addMemberToProject()" 
                        class="custom-button-h"
                        style="margin-bottom: 1px;"
                        autofocus
                    />   
                    
                </div>
            </template>
        </Dialog>
    </LayoutDefaultDynamic>
</template>

<script>
import { HTTP } from '@/http-common'
import Add from '../../components/buttons/Add.vue'
import Delete from '../../components/buttons/Delete.vue'
import jwt_decode from 'jwt-decode'
import { FilterMatchMode } from 'primevue/api'
import { UserRoleHelper } from '@/helper/user-role.helper'
import confirmDelete from './confirmDelete.vue'
import { checkAccessModule } from '@/helper/checkAccessModule'
import { UserService } from '@/service/user.service'
import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
import { TaskService } from '@/service/task.service'
export default {
    data() {
        return {
            data: [],
            data1: [],
            displayBasic: false,
            selectedMember: [],
            filters: {
                global: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            filterMemberName: '',
            display: false,
            iduser: null,
            idproject: null,
            resultPgae: {
                pageSize: 10,
                pageNumber: 1,
            },
            totalItem: 0,
            totalMapPage: 0,
            loading: true,
            itemIndex: 0,
            projectData: null,
            showProjectName: "",
        }
    },
    watch: {
        resultPgae: {
            handler: async function change() {
                if (this.filterMemberName != '') {
                    await this.filterMemberByName(this.filterMemberName)
                    await this.getAllUsersByRole(this.$route.params.id)
                } else {
                    await this.handlerGetData()
                }
            },
            deep: true,
        },
        filterMemberName: {
            handler: async function Change(newVal) {
                if (newVal != ''){
                    await this.filterMemberByName(newVal)
                    await this.getAllUsersByRole(this.$route.params.id)
                } else {
                    await this.handlerGetData()
                }
            },
            deep: true,
        },
    },
    async mounted() {
        await this.handlerGetData()
        if(this.projectData!=null){
            if(this.projectData.projectCode === this.projectData.subProjectCode){
                this.showProjectName = "Thành viên dự án ["+this.projectData.name+"]"
            }
            else {
                this.showProjectName = "Thành viên dự án ["+this.projectData.projectName+"]"
            }
        }
    },
    methods: {
        goToOverViews() {
            this.$router.push({ name: 'home'})
        },
        async handlerGetData() {
            await this.getData()
            await this.getAllUsersByRole(this.$route.params.id)
        },
        async filterMemberByName(name) {
            this.data = []
            this.loading = true
            if(this.projectData.isOnGitlab && this.projectData.projectCode === this.projectData.subProjectCode) {
                await TaskService.searchUserInProjectOnGitLab(
                    this.projectData.projectCode,
                    name,
                    this.resultPgae.pageNumber,
                    this.resultPgae.pageSize,
                )
                .then((res) => {
                    let totalPages = 0;
                    let totalItems = 0;
                    let itemIndexs = 0;
                    totalPages = parseInt(res.headers['x-total-pages']);
                    totalItems = parseInt(res.headers['x-total']);
                    itemIndexs = (this.resultPgae.pageNumber - 1) *  this.resultPgae.pageSize + 1;
                    this.totalMapPage = totalPages;
                    this.totalItem = totalItems;
                    this.itemIndex = itemIndexs;

                    res.data.forEach((el) => {
                        this.data.push({
                            id: el.id,
                            fullName: el.name,
                        })
                    })
                    
                })
                .catch(async (err) => {
                    this.totalMapPage = 0
                    this.totalItem = 0
                    console.log(err);
                })
                .finally(() => {
                    this.loading = false
                })
            }
            else {
                await UserService.filterMemberByName(
                    this.$route.params.id,
                    name,
                    this.resultPgae.pageNumber,
                    this.resultPgae.pageSize,
                )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.data = res.data._Data
                })
                .catch(async (err) => {
                    this.totalMapPage = 0
                    this.totalItem = 0
                    console.log(err);
                })
                .finally(() => {
                    this.loading = false
                })
            }
        },
        async handlerReload() {
            this.selectedMember = []
            this.filterMemberName = ''
            await this.handlerGetData()
        },
        openDeleteconfirm(iduser, idproject) {
            ;(this.display = true), (this.iduser = iduser), (this.idproject = idproject)
        },
        closeDeleteconfirm() {
            this.display = false
            ;(this.iduser = null), (this.idproject = null)
        },
        openBasic() {
            this.displayBasic = true
        },
        async getData() {
            this.loading = true
            this.data = []
            await HTTP.get(
                `Project/getProjectById/${this.$route.params.id}`,
            ).then((res) => {
                this.projectData = res.data;
            })
            .catch((err) => {
                console.log(err);
            }).finally(() => {
                this.loading = false
            });
            if(this.projectData.isOnGitlab && this.projectData.projectCode === this.projectData.subProjectCode){
                await TaskService.getAllMenberProject(this.projectData.projectCode, this.resultPgae.pageNumber,this.resultPgae.pageSize)
                .then((res) => {
                    let totalPages = 0;
                    let totalItems = 0;
                    let itemIndexs = 0;
                    totalPages = parseInt(res.headers['x-total-pages']);
                    totalItems = parseInt(res.headers['x-total']);
                    itemIndexs = (this.resultPgae.pageNumber - 1) *  this.resultPgae.pageSize + 1;
                    this.totalMapPage = totalPages;
                    this.totalItem = totalItems;
                    this.itemIndex = itemIndexs;
                    
                    res.data.forEach((el) => {
                        this.data.push({
                            id: el.id,
                            fullName: el.name,
                        })
                    })
                })
                .catch((err) => {
                    this.totalMapPage = 0
                    this.totalItem = 0
                    console.log(err)
                }).finally(() => {
                    this.loading = false
                });
            }
            else {
                await HTTP.get(`Users/getAllUsersByIdProject/${this.$route.params.id}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
                ).then((res) => {
                    if (res.data._success) {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        this.data = res.data._Data
                    } else {
                        this.$message({
                            title: res.data._message,
                            type: 'error',
                            message: res.data._message,
                        })
                    }
                })
                .catch((err) => {
                    this.totalMapPage = 0
                    this.totalItem = 0
                    console.log(err)
                })
                .finally(() => {
                    this.loading = false
                });
            }
            if(this.data.length <= 0) {
                this.resultPgae.pageNumber = 1
            }
        },
        async addMemberToProject() {
            this.closeBasic()
            var checkShowInfo = false;
            try {
                if(this.projectData.isOnGitlab && this.projectData.projectCode === this.projectData.subProjectCode){
                    this.selectedMember.forEach(async ele => {
                        await TaskService.addMenberProject(this.projectData.projectCode, {
                            "user_id": ele,
                            "access_level": 30
                        })
                        .then((res) => {
                            checkShowInfo = true
                        })
                        .catch(err => {
                            checkShowInfo = false
                            if(err.response.status==409){
                                this.showInfo("Người này đã được thêm!")
                            }
                        })
                        var findIdUser = this.data1.find(item => item.idUserGitLab === ele)
                        await HTTP.post('memberProject/addMemberAtProject', {
                            idProject: this.$route.params.id,
                            member: findIdUser.id,
                            createUser: checkAccessModule.getUserIdCurrent(),
                        })
                        .then(async res =>{ 
                            await this.handlerReload()
                            checkShowInfo = true
                        })
                        .catch(err => {
                            console.log(err);
                        })
                    })
                }
                else {
                    this.selectedMember.forEach(async (id) => {
                        await HTTP.post('memberProject/addMemberAtProject', {
                            idProject: this.$route.params.id,
                            member: id,
                            createUser: checkAccessModule.getUserIdCurrent(),
                        })
                        .then(async res =>{ 
                            await this.handlerReload()
                            checkShowInfo = true
                        })
                        .catch(err => {
                            console.log(err);
                        })
                    })
                }
                if(checkShowInfo) this.showSuccess('Thêm thành công')
            }
            catch(err) {
                this.totalMapPage = 0
                this.totalItem = 0
                console.log(err)
            }
        },
        closeBasic() {
            this.displayBasic = false
        },
        async getAllUsersByRole(idProject) {
            await HTTP.get(`Users/getAllUsersByRoleInProject/${idProject}/${checkAccessModule.getUserIdCurrent()}`).then((res) => {
                if (res.data) {
                    this.data1 = res.data._Data
                }
            }).catch((err)=> {console.log(err)})

            if(this.projectData.isOnGitlab && this.projectData.projectCode === this.projectData.subProjectCode){
                var result = this.data1.filter(obj1 => !this.data.some(obj2 => obj2.id === obj1.idUserGitLab));
                this.data1 = result
            }
        },
        async deleteMember(idMember, idProject) {
            if(this.projectData.isOnGitlab && this.projectData.projectCode === this.projectData.subProjectCode){
                await TaskService.deleteMenberProject(this.projectData.projectCode,idMember).then(res => {}).catch((err)=> {console.log(err)})
                var findIdUser = this.data1.find(ele => ele.idUserGitLab === idMember)
                if(findIdUser === undefined) {
                    var getUserApi = await UserService.getUserByIdUserGitLab(idMember)
                    idMember = getUserApi.data.id
                }
            }
            await HTTP.delete('memberProject/deleteMemberInProject/' + idMember + '/' + idProject).then(async (res) => {
                if (res.data._success == true) {
                } else {
                    this.showError('Xóa lỗi!')
                }
            })
            .catch((err)=> {console.log(err)})

            this.closeDeleteconfirm()
            this.showSuccess('Xóa thành công!')
            await this.handlerReload()
        },
        showSuccess(mess) {
            this.$toast.add({ severity: 'success', summary: 'Thành công!', detail: mess, life: 3000 })
        },
        showError(mess) {
            this.$toast.add({ severity: 'error', summary: 'Lỗi!', detail: mess, life: 3000 })
        },
        showInfo(mess) {
            this.$toast.add({ severity: 'info', summary: 'Thông báo!', detail: mess, life: 3000 })
        },
    },
    components: { Add, Delete, confirmDelete, ButtonCustom },
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
        background-color: #607d8b;
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
}
</style>
