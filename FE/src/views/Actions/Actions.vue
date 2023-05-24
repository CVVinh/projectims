<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">DANH SÁCH THAO TÁC</li>
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
                                <Export label="Xuất Excel" @click="exportToExcel()" class="custom-button-h" />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <addActions @render="getAllActions" />
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite"
                                    icon="pi pi-filter-slash"
                                    class="p-button-outlined custom-button-h"
                                    @click="handlerReload()"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    v-model="filtersName"
                                    placeholder="Tìm kiếm theo tên..."
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
                        :value="this.actions"
                        class="p-datatable-customers"
                        :rows="5"
                        dataKey="id"
                        :rowHover="true"
                        filterDisplay="menu"
                        v-model:filters="filters"
                        responsiveLayout="scroll"
                        ref="dt"
                        showGridlines
                        :globalFilterFields="['id', 'name', 'description']"
                    >
                        <template #empty> Không tìm thấy. </template>
                        <template #loading>
                            <ProgressSpinner />
                        </template>

                        <Column header="STT">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>

                        <Column field="name" header="Tên nhóm" sortable>
                            <template #bodFEy="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column field="description" header="Mô tả" sortable>
                            <template #bodFEy="{ data }">
                                {{ data.description }}
                            </template>
                        </Column>

                        <Column header="Thực thi" style="min-width: 10rem">
                            <template #body="{ data }">
                                <!-- <Edit :disabled="data.isDeleted" /> -->
                                <editActions
                                    :checkEdit="checkEdit(data.isDeleted)"
                                    :action="data"
                                    @render="getAllActions"
                                    class="update-module"
                                />
                                <Delete
                                    @click="confirmDelete(data.id)"
                                    :disabled="data.isDeleted"
                                    class="custom-button-h ms-2"
                                />
                            </template>
                        </Column>
                    </DataTable>
                </div>
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
    </LayoutDefaultDynamic>
</template>

<script>
import Delete from '@/components/buttons/Delete.vue'
import Edit from '@/components/buttons/Edit.vue'
import Export from '@/components/buttons/Export.vue'
import { LocalStorage } from '@/helper/local-storage.helper'
import editActions from './editActions.vue'
import addActions from './addActions.vue'
import { HTTP } from '@/http-common'
import { checkAccessModule } from '@/helper/checkAccessModule'
import { ActionModuleService } from '@/service/actionModule.service'

export default {
    data() {
        return {
            actions: [],
            filters: {
                //  global: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            filtersName: '',
            data: '',
            user: '',
            isOpenAdd: false,
            isOpenEdit: false,
            group: '',
            isChange: false,
            token: null,
            resultPgae: {
                pageSize: 10,
                pageNumber: 1,
            },
            totalItem: 0,
            totalMapPage: 0,
            itemIndex: 0,
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
        }
    },
    watch: {
        resultPgae: {
            handler: async function change() {
                if (this.filtersName != '') {
                    await this.handlerFilterActionModuleByName(this.filtersName)
                } else {
                    await this.getAllActions()
                }
            },
            deep: true,
        },
        filtersName: {
            handler: async function change(newVal) {
                if (newVal != '') {
                    await this.handlerFilterActionModuleByName(newVal)
                } else {
                    await this.getAllActions()
                }
            },
            deep: true,
        },
    },
    async mounted() {
        if (checkAccessModule.isAdmin()) {
            if (checkAccessModule.checkAccessModule(this.$route.path.replace('/setups/', '')) === true) {
                checkAccessModule.checkPermissionAction(this.$route.path.replace('/setups/', ''), this.showButton)
                await this.getAllActions()
            } else {
                alert('Bạn không có quyền truy cập module này')
                this.$router.push('/')
            }
        } else {
            alert('Bạn không có quyền truy cập module này')
            this.$router.push('/')
        }
    },
    methods: {
        setChange() {
            this.isChange = true
        },
        async handlerReload() {
            this.filtersName = ''
            await this.getAllActions()
        },
        async handlerFilterActionModuleByName(name) {
            this.actions = []
            ActionModuleService.filterActionModuleByName(name, this.resultPgae.pageNumber, this.resultPgae.pageSize)
                .then((res) => {
                    if (res.status == 200) {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        this.actions = res.data._Data
                    }
                })
                .catch(() => {
                    this.totalMapPage = 0
                    this.totalItem = 0
                })
        },
        async getAllActions() {
            this.actions = []
            HTTP.get(
                `ActionModule/getAllActionModule?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            ).then((res) => {
                if (res.status == 200) {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.actions = res.data._Data
                }
            })
        },
        async deleteGroup(id) {
            if (id) {
                const UserDelete = {
                    userDeleted: checkAccessModule.getUserIdCurrent(),
                }
                await HTTP.put(`ActionModule/deleteActionModule/${id}`, UserDelete).then(async (res) => {
                    if (res.status == 200) {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Xóa thành công!',
                            life: 3000,
                        })
                        await this.getAllActions()
                    }
                })
            }
        },
        exportToExcel() {
            HTTP.get(`Group/exportExcel/`)
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
                    if (err.data) {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Lỗi',
                            details: 'Lỗi khi xuất excel!',
                            life: 3000,
                        })
                    }
                })
        },
        showAddGroup() {
            this.isOpenAdd = true
        },
        closeDialogAdd() {
            this.isOpenAdd = false
            if (this.isChange === true) {
                this.getAllGroup()
                this.isChange = false
            }
        },
        showEditGroup(group) {
            this.group = group
            this.isOpenEdit = true
        },
        closeDialogEdit() {
            this.isOpenEdit = false
            if (this.isChange === true) {
                this.getAllGroup()
                this.isChange = false
            }
        },
        confirmDelete(id) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn muốn xóa?',
                header: 'Xóa',
                icon: 'pi pi-exclamation-triangle',
                acceptClass: 'p-button-danger',
                accept: async () => {
                    this.deleteGroup(id)
                },
                reject: () => {
                    this.$toast.add({
                        severity: 'error',
                        summary: 'Từ chối',
                        detail: 'Bạn đã bị từ chối',
                        life: 3000,
                    })
                },
            })
        },
        checkEdit(isDeleted) {
            return isDeleted === 1
        },
    },
    components: { Delete, Edit, Export, editActions, addActions },
}
</script>

<style lang="scss" scoped>
.header-left {
    height: 100%;
    display: flex;
    flex-direction: row;
    justify-content: end;
    align-items: flex-end;
    margin-right: 10px;
}

.button-gray {
    color: #6c757d;
    background-color: #fff;
    border-color: #ced4da;
}

.margin-right {
    margin-right: 14px;
}

.header-tilte {
    color: #fff;
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

::v-deep(.p-datatable.p-datatable-customers) {
    .p-datatable-header {
        background-color: #607d8b;
    }
}

::v-deep(.p-dropdown .p-dropdown-label.p-placeholder) {
    padding: 0.582rem 0.75rem;
}

::v-deep(.p-dropdown .p-dropdown-label) {
    padding: 0.582rem 0.75rem;
}
.Menu-table-header {
    width: 100%;
    height: 100%;
    color: white;
    display: flex;
    flex-direction: column;
}

.Menu-table-header-Groupinput {
    height: 50px;
    display: flex;
    align-items: center;
    justify-content: space-between;
}
</style>
