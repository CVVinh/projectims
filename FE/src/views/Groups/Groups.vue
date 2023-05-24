<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />

        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">DANH SÁCH NHÓM NGƯỜI DÙNG</li>
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
                                    @click="exportToExcel()"
                                    v-if="showButton.export"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    class="p-button-info p-button-sm custom-button-h"
                                    @click="showAddGroup()"
                                    icon="pi pi-plus"
                                    label="Thêm mới"
                                    v-if="showButton.add"
                                />
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite"
                                    icon="pi pi-filter-slash"
                                    class="p-button-outlined right custom-button-h"
                                    @click="handlerReload()"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    placeholder="Nhâp tên nhóm để tìm kiếm..."
                                    v-model="filterName"
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
                        :loading="isLoading"
                        :value="this.groups"
                        class="p-datatable-customers"
                        :rows="5"
                        dataKey="id"
                        :rowHover="true"
                        filterDisplay="menu"
                        v-model:filters="filters"
                        responsiveLayout="scroll"
                        ref="dt"
                        showGridlines
                        :globalFilterFields="['idGroup', 'nameGroup', 'desc']"
                    >
                        <template #empty> Không tìm thấy. </template>
                        <template #loading>
                            <ProgressSpinner />
                        </template>

                        <Column field="#" header="#" dataType="date">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>

                        <Column field="id" header="Mã Code" sortable>
                            <template #body="{ data }">
                                {{ data.id }}
                            </template>
                        </Column>
                        <Column field="nameGroup" header="Tên nhóm" sortable>
                            <template #body="{ data }">
                                {{ data.nameGroup }}
                            </template>
                        </Column>
                        <Column field="discription" header="Mô tả" sortable>
                            <template #body="{ data }">
                                {{ data.discription }}
                            </template>
                        </Column>

                        <Column header="Thực thi" style="min-width: 10rem">
                            <template #body="{ data }">
                                <div class="actions-buttons">
                                    <Edit
                                        class="p-button-warning btn-margin custom-button-h"
                                        @click="showEditGroup(data)"
                                        v-if="showButton.update"
                                    />
                                    <Delete
                                        class="btn-margin custom-button-h"
                                        @click="confirmDelete(data)"
                                        v-if="showButton.delete"
                                    />
                                </div>
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
        <EditGroupVue
            :isOpen="this.isOpenEdit"
            @closeDialog="closeDialogEdit()"
            :group="this.group"
            @reloadPage="reloadPage()"
        />
        <AddGroupVue :isOpen="this.isOpenAdd" @closeDialog="closeDialogAdd()" @reloadPage="reloadPage()" />
    </LayoutDefaultDynamic>
</template>

<script>
import Add from '../../components/buttons/Add.vue'
import Edit from '../../components/buttons/Edit.vue'
import Delete from '../../components/buttons/Delete.vue'
import Member from '../../components/buttons/Member.vue'
import Export from '../../components/buttons/Export.vue'
import { HTTP } from '@/http-common'
import jwtDecode from 'jwt-decode'
import { FilterMatchMode } from 'primevue/api'
import AddGroupVue from './AddGroup.vue'
import EditGroupVue from './EditGroup.vue'
import { checkAccessModule } from '@/helper/checkAccessModule'
import { GroupsService } from '@/service/group.service'

export default {
    data() {
        return {
            groups: [],
            filters: {
                global: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            filterName: '',
            isLoading: false,
            isOpenAdd: false,
            isOpenEdit: false,
            group: '',
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
        }
    },
    watch: {
        resultPgae: {
            handler: async function change() {
                if (this.filterName != '') {
                    await this.handlerFilerGroupByName(this.filterName)
                } else {
                    await this.getAllGroup()
                }
            },
            deep: true,
        },
        filterName: {
            handler: async function Change(newVal) {
                await this.handlerFilerGroupByName(newVal)
            },
            deep: true,
        },
    },
    async created() {
        if (checkAccessModule.isAdmin()) {
            if (checkAccessModule.checkAccessModule(this.$route.path.replace('/setups/', '')) === true) {
                checkAccessModule.checkPermissionAction(this.$route.path.replace('/setups/', ''), this.showButton)
                await this.getAllGroup()
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
        showError(message) {
            this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
        },

        showSuccess(message) {
            this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
        },

        async reloadPage() {
            await this.getAllGroup()
        },
        async handlerReload() {
            this.filterName = ''
            this.getAllGroup()
        },
        async handlerFilerGroupByName(name) {
            this.isLoading = true
            this.groups = []
            GroupsService.filterGroupByName(name, this.resultPgae.pageNumber, this.resultPgae.pageSize)
                .then((res) => {
                    if (res.status == 200) {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        this.groups = res.data._Data
                    }
                })
                .catch(() => {
                    this.getAllGroup()
                })
            this.isLoading = false
        },
        async getAllGroup() {
            this.isLoading = true
            this.groups = []
            await HTTP.get(
                `Group/getListGroup?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    if (res.status == 200) {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        this.groups = res.data._Data
                    }
                })
                .catch((error) => {
                    console.log(error)
                })
                .finally(() => {
                    this.isLoading = false
                })
        },
        async deleteGroup(data) {
            await HTTP.put(`Group/deleteGroup/${data.id}`)
                .then(async (res) => {
                    if (res.status == 200) {
                        this.showSuccess('Xóa thành công!')
                        await this.getAllGroup()
                    }
                })
                .catch((err) => {
                    if (err.data) {
                        this.showError('Lỗi khi xóa!')
                    }
                })
        },
        async exportToExcel() {
            await HTTP.get(`Group/exportExcel/`)
                .then(async (res) => {
                    if (res.status == 200) {
                        this.showSuccess('Xuất file excel thành công!')
                        await this.getAllGroup()
                    }
                })
                .catch((err) => {
                    if (err.data) {
                        this.showError('Lỗi khi xuất excel!')
                    }
                })
        },

        showAddGroup() {
            this.isOpenAdd = true
        },

        closeDialogAdd() {
            this.isOpenAdd = false
        },

        showEditGroup(group) {
            this.group = group
            this.isOpenEdit = true
        },

        closeDialogEdit() {
            this.isOpenEdit = false
        },
        async confirmDelete(data) {
            this.$confirm.require({
                message: 'Bạn có chắc chắn muốn xóa?',
                header: 'Xóa',
                icon: 'pi pi-exclamation-triangle',
                acceptClass: 'p-button-danger',
                accept: async () => {
                    await this.deleteGroup(data)
                },
                reject: () => {
                    return
                },
            })
        },
    },
    components: {
        Add,
        Edit,
        Delete,
        Member,
        Export,
        AddGroupVue,
        EditGroupVue,
    },
}
</script>

<style lang="scss" scoped>
.actions-buttons {
    display: flex;
    .btn-margin {
        margin-right: 5px;
    }
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
