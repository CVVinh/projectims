<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast />
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">DANH SÁCH MENU</li>
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
                                <Export label="Xuất Excel" v-if="showButton.export" class="custom-input-h"/>
                            </li>
                            <li class="nav-item me-2 mb-2 ">
                                <Add label="Thêm mới" class="custom-button-h" @click="Openmodal" v-if="showButton.add" /> 
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2 ">
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite"
                                    icon="pi pi-filter-slash"
                                    class="p-button-outlined custom-button-h"
                                    @click="handlerReload()"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Dropdown
                                    class="custom-input-h"
                                    v-model="filterMenu.filterByAction"
                                    :options="OptionModule"
                                    optionLabel="nameModule"
                                    optionValue="id"
                                    placeholder="Chọn chức năng"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    class="custom-input-h"
                                    v-model="filterMenu.filterByName"
                                    placeholder="Tìm kiếm theo tên và trang..."
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        :value="ListMenu"
                        :loading="loading"
                        :globalFilterFields="[
                            'nameModule',
                            'menu1.title',
                            'menu1.view',
                            'menu1.controller',
                            'menu1.icon',
                            'menu1.parent',
                        ]"
                        responsiveLayout="scroll"
                        :rowHover="true"
                        :rows="5"
                        showGridlines
                        v-model:filters="filters"
                    >
                        <template #empty> Không tìm thấy. </template>
                        <Column field="nameModule" header="Tên chức năng" sortable></Column>
                        <Column field="menu1.title" header="Tên" sortable></Column>
                        <Column field="menu1.view" header="Trang hiển thị" sortable></Column>
                        <Column field="menu1.controller" header="Trang điều hướng" sortable></Column>
                        <Column field="menu1.icon" header="Icon" sortable>
                            <!--                 
                        <template #body="{data}">
                            <div class="table__icon">        
                                <i :class="data.menu1.icon"></i>
                            </div>

                        </template> -->
                        </Column>
                        <Column field="menu1.parent" header="Nhánh/lớp cha" sortable></Column>
                        <Column field="menu1.isDeleted" header="Trạng thái" sortable>
                            <template #body="{ data }">
                                {{ data.menu1.isDeleted === 0 ? 'Đang dùng' : 'Đã xóa' }}
                            </template>
                        </Column>
                        <Column header="Thực thi" style="min-width: 9rem">
                            <template #body="{ data }">
                                <Edit
                                    v-if="showButton.update"
                                    class="p-button-warning custom-button-h me-2"
                                    @click="Openeditmodal(data.menu1.id)"
                                    :disabled="checkAction(data.menu1.isDeleted)"
                                />

                                <Delete
                                    v-if="showButton.delete"
                                    class="custom-button-h"
                                    :disabled="checkAction(data.menu1.isDeleted)"
                                    @click="Delete(data.menu1.id)"
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
            <AddmenuList
                :status="openStatus"
                @closemodal="Closemodal"
                @failed="showError1"
                @success="showSuccess1"
                :optionmodule="OptionModule"
                @reloadpage="GetAllMenuList"
            />
            <EditmenuList
                :status="openStatusEdit"
                @closemodal="closeeditmodal"
                @failed="showError2"
                @success="showSuccess2"
                :dataedit="editdata"
                :optionmodule="OptionModule"
                @reloadpage="GetAllMenuList"
            />
        </div>
    </LayoutDefaultDynamic>
</template>

<script>
    import { HTTP } from '@/http-common'
    import LayoutDefaultDynamic from '../../layouts/LayoutDefault/LayoutDefaultDynamic.vue'
    import Export from '../../components/buttons/Export.vue'
    import Add from '../../components/buttons/Add.vue'
    import Delete from '../../components/buttons/Delete.vue'
    import Edit from '../../components/buttons/Edit.vue'
    import AddmenuList from './addmenuList.vue'
    import EditmenuList from './editmenuList.vue'
    import { FilterMatchMode } from 'primevue/api'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import { MenuService } from '@/service/menu.service'
    export default {
        data() {
            return {
                ListMenu: [],
                Listrendertable: [],
                search: '',
                OptionsModule: [],
                selectModule: '',
                openStatus: false,
                openStatusEdit: false,
                loading: true,
                OptionChooseParent: [],
                editdata: null,
                OptionModule: [],
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
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
                filterMenu: {
                    filterByName: '',
                    filterByAction: 0,
                },
            }
        },
        watch: {
            resultPgae: {
                handler: function change() {
                    if (this.filterMenu.filterByName != '' || this.filterMenu.filterByAction != 0) {
                        this.handlerFilterMenuByNameOrAction(
                            this.filterMenu.filterByName,
                            this.filterMenu.filterByAction,
                        )
                    } else {
                        this.GetAllMenuList()
                    }
                },
                deep: true,
            },
            filterMenu: {
                handler: async function Change(newVal) {
                    this.handlerFilterMenuByNameOrAction(newVal.filterByName, newVal.filterByAction)
                },
                deep: true,
            },
        },
        methods: {
            Openmodal() {
                this.openStatus = true
            },

            Closemodal() {
                this.openStatus = false
            },

            Openeditmodal(id) {
                this.openStatusEdit = true
                this.editdata = id
            },

            closeeditmodal() {
                this.openStatusEdit = false
            },
            async handlerFilterMenuByNameOrAction(name, idModule) {
                this.ListMenu = []
                this.loading = true
                MenuService.filterMenuByNameOrAction(
                    name,
                    idModule,
                    this.resultPgae.pageNumber,
                    this.resultPgae.pageSize,
                )
                    .then((res) => {
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        this.ListMenu = res.data._Data
                        this.loading = false
                    })
                    .catch(() => {
                        this.totalMapPage = 0
                        this.totalItem = 0
                        this.loading = false
                    })
            },
            GetAllMenuList() {
                HTTP.get(
                    `Menu/getListMenu?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
                ).then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.ListMenu = res.data._Data
                    this.loading = false
                })
            },
            async Getparentselect() {
                await HTTP.get('Menu/getListMenuParent').then((res) => {
                    this.OptionsModule = res.data
                })
            },

            async GetMenuModule() {
                await HTTP.get('Menu/getListMenuModule').then((res) => {
                    console.log(res.data)
                    this.OptionModule = res.data
                })
            },
            checkAction(id) {
                if (id === 0) {
                    return false
                } else {
                    return true
                }
            },
            async DeleteMenu(id) {
                await HTTP.put(`Menu/deleteMenu/${id}`)
                    .then(async (res) => {
                        if (res.data === 'success') {
                            this.showSuccess()
                        }
                        await this.GetAllMenuList()
                    })
                    .catch((err) => {
                        this.showError()
                    })
            },

            async Delete(id) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa?',
                    header: 'Xóa',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    accept: async () => {
                        await this.DeleteMenu(id)
                    },
                    reject: () => {
                        return
                    },
                })
            },
            showSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Thêm mới thành công!',
                    life: 3000,
                })
            },
            showError() {
                this.$toast.add({ severity: 'error', summary: 'Lỗi ', detail: 'Lỗi', life: 3000 })
            },
            showSuccess1() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công ',
                    detail: 'Thêm mới thành công',
                    life: 3000,
                })
            },
            showError1() {
                this.$toast.add({ severity: 'error', summary: 'Lỗi ', detail: 'Thêm Lỗi', life: 3000 })
            },
            showSuccess2() {
                this.$toast.add({ severity: 'success', summary: 'Thành công ', detail: 'Sửa thành công!', life: 3000 })
            },
            showError2() {
                this.$toast.add({ severity: 'error', summary: 'Lỗi ', detail: 'Sửa Lỗi', life: 3000 })
            },
            async handlerReload() {
                this.ListMenu = []
                this.filterMenu.filterByAction = 0
                this.filterMenu.filterByName = ''
                await this.GetAllMenuList()
            },
        },

        async mounted() {
            if (checkAccessModule.checkAccessModule(this.$route.path.replace('/setups/', '')) === true) {
               if(checkAccessModule.checkPermissionAction(this.$route.path.replace('/setups/', ''), this.showButton)){
                    await this.GetMenuModule()
                    await this.GetAllMenuList()
                    await this.Getparentselect()
                }
            }
            // if (checkAccessModule.checkAccessModule(this.$route.path.replace('/setups/', '')) === true) {
            //     checkAccessModule.checkShowButton(this.$route.path.replace('/setups/', ''), this.showButton)
                
            // } else {
            //     alert('Bạn không có quyền truy cập module này')
            //     router.push('/')
            // }
        },
        // computed: {
        //     dataTable() {
        //         let array = this.ListMenu
        //         if (this.selectModule) {
        //             array = array.filter((ele) => {
        //                 return ele.menu1.idModule === this.selectModule
        //             })
        //         }
        //         return array
        //     },
        // },
        components: { LayoutDefaultDynamic, Export, Add, Delete, Edit, AddmenuList, EditmenuList },
    }
</script>

<style scoped>
    .app-table {
        width: 100%;

        display: flex;
        align-content: center;
        justify-content: center;
    }
    .Menu-table {
        width: 98%;

        margin-top: 15px;
    }

    ::v-deep(.p-datatable .p-datatable-header) {
        background-color: #607d8b;
        padding: 15px;
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

    .itemsbutton {
        margin-left: 10px;
    }
    .itemsinput {
        margin-right: 10px;
        height: 50px;
    }
    .dropdowninput {
        min-width: 184px;
    }
    ::v-deep(.p-input-icon-left > .p-inputtext) {
        height: 50px;
    }

    .table__icon {
        width: 40px;
        font-size: 30px;
        color: rgb(83, 83, 83);
        display: flex;
        align-content: center;
        justify-content: center;
    }
</style>
