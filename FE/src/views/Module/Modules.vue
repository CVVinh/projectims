<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">DANH SÁCH MODULE CHỨC NĂNG</li>
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
                                    label="Xuất Excel"
                                    class="custom-button-h"
                                    @click="exportCSV($event)"
                                    v-if="showButton.export"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <AddModulesVue @render="getAllModules()" v-if="showButton.add" />
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
                                    placeholder="Tìm kiếm tên chức năng..."
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
                        :value="modules"
                        class="p-datatable-customers"
                        :rows="5"
                        dataKey="id"
                        :loading="loading"
                        :rowHover="true"
                        filterDisplay="menu"
                        v-model:filters="filters"
                        :globalFilterFields="['#', 'nameModule', 'note']"
                        responsiveLayout="scroll"
                        ref="dt"
                        showGridlines
                    >
                        <template #empty> Không tìm thấy. </template>
                        <template #loading>
                            <ProgressSpinner />
                        </template>
                        <Column field="" header="#"
                            ><template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="nameModule" header="Tên chức năng " sortable>
                            <template #body="{ data }">
                                {{ data.nameModule }}
                            </template>
                        </Column>
                        <Column field="note" header="Ghi chú">
                            <template #body="{ data }"> {{ data.note }}</template>
                        </Column>
                        <Column header="Thực thi" style="min-width: 10rem">
                            <template #body="{ data }">
                                <UpdateModuleVue
                                    v-if="showButton.update"
                                    class="update-module"
                                    :checkEdit="checkEdit(data.isDeleted)"
                                    :module="data"
                                    @render="getAllModules()"
                                />
                                <Delete
                                    class="custom-button-h ms-2"
                                    @click="handleDelete(data.id)"
                                    :disabled="checkEdit(data.isDeleted)"
                                    v-if="showButton.delete"
                                />
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
        </div>
    </LayoutDefaultDynamic>
</template>

<script>
import Edit from '../../components/buttons/Edit.vue'
import Delete from '../../components/buttons/Delete.vue'
import Export from '../../components/buttons/Export.vue'
import Add from '../../components/buttons/Add.vue'
import { FilterMatchMode } from 'primevue/api'
import { HTTP } from '@/http-common'
import jwtDecode from 'jwt-decode'
import AddModulesVue from './AddModule.vue'
import UpdateModuleVue from './UpdateModule.vue'
import { warn } from 'vue'
import { checkAccessModule } from '@/helper/checkAccessModule'
import { ModuleService } from '@/service/modules.service'
export default {
    data() {
        return {
            loading: true,
            modules: [
                {
                    id: 1,
                    nameModule: 'Mot',
                    note: 'Ghi chu',
                },
            ],
            filters: {
                global: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            filtersName: '',
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
                    await this.handlerFilterByNameModule(this.filtersName)
                } else {
                    await this.getAllModules()
                }
            },
            deep: true,
        },
        filtersName: {
            handler: async function Change(newVal) {
                if (newVal != '') {
                    await this.handlerFilterByNameModule(newVal)
                } else {
                    await this.getAllModules()
                }
            },
            deep: true,
        },
    },
    async mounted() {
        if (checkAccessModule.isAdmin()) {
            if (checkAccessModule.checkAccessModule(this.$route.path.replace('/setups/', '')) === true) {
                checkAccessModule.checkPermissionAction(this.$route.path.replace('/setups/', ''), this.showButton)
                await this.getAllModules()
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
        async handlerFilterByNameModule(name) {
            this.loading = true
            this.modules = []
            ModuleService.filterModulesByNameOrAction(name, this.resultPgae.pageNumber, this.resultPgae.pageSize)
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.modules = res.data._Data
                })
                .catch(() => {
                    this.totalMapPage = 0
                    this.totalItem = 0
                })
                .finally(() => {
                    this.loading = false
                })
        },
        async getAllModules() {
            await HTTP.get(
                `Modules/getListModule?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.modules = res.data._Data
                })
                .catch((err) => {
                    console.log(err)
                })
                .finally(() => {
                    this.loading = false
                })
        },
        async handlerReload() {
            this.filtersName = ''
            this.modules = []
            await this.getAllModules()
        },
        async handleDelete(id) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn muốn xóa?',
                header: 'Xóa',
                icon: 'pi pi-info-circle',
                acceptClass: 'p-button-danger',
                accept: async () => {
                    this.loading = true
                    await HTTP.put(`Modules/deleteModule/${id}`)
                        .then((res) => {
                            this.getAllModules()
                            this.$toast.add({
                                severity: 'info',
                                summary: 'Xác nhận',
                                detail: 'Xoá thành công!',
                                life: 3000,
                            })
                        })
                        .catch((err) => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi ',
                                detail: 'Lỗi máy chủ',
                                life: 3000,
                            })
                        })
                        .finally(() => {
                            this.loading = false
                        })
                },
                reject: () => {
                    return
                },
            })
        },
        exportCSV() {
            this.$refs.dt.exportCSV()
        },
        checkEdit(isDeleted) {
            return isDeleted === 1
        },
    },
    components: {
        Edit,
        Add,
        Delete,
        Export,
        AddModulesVue,
        UpdateModuleVue,
    },
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
</style>
