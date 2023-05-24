<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">DANH SÁCH THAO TÁC MODULE</li>
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
                                <addPermisisonActionModule @render="getAllActions" v-if="showButton.add" />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        :loading="isLoading"
                        :value="this.listActionModule"
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

                        <Column field="name" header="Tên chức năng" sortable>
                            <template #body="{ data }">
                                {{ data.nameModule }}
                            </template>
                        </Column>
                        <Column field="description" header="Tên thao tác" sortable>
                            <template #body="{ data }">
                                <span v-for="item in data.actionModule" :key="item">
                                    <Chip :label="item.name" class="me-2 mt-1" />
                                </span>
                            </template>
                        </Column>

                        <Column header="Thực thi" style="min-width: 10rem">
                            <template #body="{ data }">
                                <editPermisisonActionModule
                                    v-if="showButton.update"
                                    :action="data"
                                    @render="getAllActions"
                                    class="me-2 update-module"
                                />
                                <!-- <Delete @click="confirmDelete(data)" style="margin-left: 10px" v-if="showButton.delete" /> -->
                                <deletePermisisonActionModule
                                    v-if="showButton.delete"
                                    :action="data"
                                    @render="getAllActions"
                                />
                            </template>
                        </Column>
                    </DataTable>
                </div>
            </div>
        </div>
        <Pagination
            v-model:pageNumber="resultPgae.pageNumber"
            v-model:pageSize="resultPgae.pageSize"
            :totalPages="totalMapPage"
            :totalItems="this.totalItem"
            :itemIndex="this.itemIndex"
        />
    </LayoutDefaultDynamic>
</template>

<script>
import Delete from '@/components/buttons/Delete.vue'
import Edit from '@/components/buttons/Edit.vue'
import Export from '@/components/buttons/Export.vue'
import { LocalStorage } from '@/helper/local-storage.helper'
import editPermisisonActionModule from './editPermissionActionModule.vue'
import deletePermisisonActionModule from './deletePermissionActionModule.vue'
import addPermisisonActionModule from './addPermissionActionModule.vue'
import { HTTP } from '@/http-common'
import { checkAccessModule } from '@/helper/checkAccessModule'

export default {
    data() {
        return {
            listActionModule: [],
            filters: {
                //global: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            isLoading: false,
            data: '',
            user: '',
            isOpenAdd: false,
            isOpenEdit: false,
            group: '',
            isChange: false,
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
            handler: function change() {
                this.getAllActions()
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
        async getAllActions() {
            this.isLoading = true
            this.groups = []
            await HTTP.get(
                `PermissionActionModule/getNameAllPermissionActionModule?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            ).then((res) => {
                if (res.status == 200) {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex

                    this.listActionModule = res.data._Data
                }
            })
            this.isLoading = false
        },
        showAddGroup() {
            this.isOpenAdd = true
        },
        closeDialogAdd() {
            this.isOpenAdd = false
            if (this.isChange === true) {
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
                this.isChange = false
            }
        },
    },
    components: {
        Delete,
        Edit,
        Export,
        editPermisisonActionModule,
        addPermisisonActionModule,
        deletePermisisonActionModule,
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
