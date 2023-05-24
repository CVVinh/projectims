<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <div class="container-fluid">
                <div class="permission-user-menu row">
                    <div class="col-sm-12 col-md-2 col-lg-2 card_content p-2">
                        <div class="row mb-2">
                            <div class="col-sm-12 col-md-12 col-lg-12">
                                <Dropdown
                                    showClear
                                    filter
                                    v-model="selectedUserGroup"
                                    :options="userGroup"
                                    optionLabel="nameGroup"
                                    optionValue="id"
                                    @change="onChangeUserGroup()"
                                    placeholder="Chọn nhóm người dùng"
                                    class="custom-input-h"
                                />
                            </div>
                        </div>
                        <div class="row mb-2">
                            <div class="col-sm-12 col-md-12 col-lg-12">
                                <InputText
                                    class="p-inputtext text-body custom-input-h"
                                    v-model="searchUserName"
                                    placeholder="Tìm kiếm theo tên ..."
                                />
                            </div>
                        </div>

                        <div class="row mb-2">
                            <div class="col-sm-12 col-md-12 col-lg-12">
                                <div class="card_content card_content_overflow" style="height: calc(100vh - 270px)">
                                    <template v-if="listUser.length <= 0">
                                        <p class="p-2 text-black-50">Không tìm thấy.</p>
                                    </template>
                                    <OrderList
                                        v-model="listUser"
                                        style="height: calc(100vh - 150px)"
                                        dataKey="id"
                                        v-if="listUser.length > 0"
                                    >
                                        <template #item="data">
                                            <Button @click="selectedAUser(data.item.idUser)" class="p-button-text">
                                                <h6 class="text-body" style="text-align: left">
                                                    {{ data.item.fullNameUser }}
                                                </h6>
                                            </Button>
                                        </template>
                                    </OrderList>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-sm-12 col-md-12 col-lg-12 ">
                                <div class="row">
                                    <div class="col-sm-5 mt-2">
                                        <Button label="Lưu" icon="pi pi-check" class="p-button-primary p-button-icon custom-button-h text-size" @click="confirmSave()"></Button>
                                    </div>
                                    <div class="col-sm-7 mt-2">
                                        <Button
                                            label="Quay về"
                                            icon="pi pi-arrow-left"
                                            class="p-button-secondary p-button-icon custom-button-h text-size"
                                            @click="cancel"
                                        ></Button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-10 col-lg-10 card_content">
                        <div class="">
                            <div class="row">
                                <div class="col-sm-12 col-md-12 col-lg-12">
                                    <div class="spinner-loading" v-if="loading">
                                        <ProgressSpinner
                                            style="width: 50px; height: 50px"
                                            strokeWidth="3"
                                            animationDuration=".2s"
                                        />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-md-12 col-lg-12">
                                    <div id="v-tabs" class="tab-panel-module" v-if="listUser.length > 0 && selectedUser != null">
                                        <div
                                            v-for="module in listModule"
                                            :key="module"
                                            @click="selectModule(module)"
                                            :class="{ active: selectedTab === module.id }"
                                            class="tab-item row"
                                        >
                                            <div class="col-sm-3 col-md-3 tab-item-sub">
                                                <h5 class="tab-item__heading">{{ module.nameModule }}</h5>
                                            </div>

                                            <div class="col-sm-9 col-md-9 tab-item-sub tab-item-sub-content">
                                                <itemPermissionActionModule :module="module" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
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
                    <Button label="Lưu" icon="pi pi-check" @click="submit" autofocus />
                </template>
            </Dialog>
        </div>
    </LayoutDefaultDynamic>
</template>

<script>
import { HTTP } from '@/http-common'
import { LocalStorage } from '@/helper/local-storage.helper'
import { ApiApplication, HttpStatus } from '@/config/app.config'
import { UserRoleHelper } from '@/helper/user-role.helper'
import router from '@/router'
import itemPermissionActionModule from './itemPermissionActionModule.vue'
import { checkAccessModule } from '@/helper/checkAccessModule'
export default {
    components: { itemPermissionActionModule },
    name: 'PermissionUserMenu',
    data() {
        return {
            displayBasic: false,
            num: 5,
            loading: false,
            selectedUserGroup: null,
            selectedTab: null,
            userGroup: [],
            listUser: [],
            listModule: [],
            selectedUser: null,
            selectedModule: [],
            searchUserName: null,
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
    async mounted() {
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/setups/', '')) === true) {
            checkAccessModule.checkShowButton(this.$route.path.replace('/setups/', ''), this.showButton)
            await this.getUserGroup()
        } else {
            this.countTime()
            this.displayBasic = true
        }
    },
    watch: {
        searchUserName: {
            handler: async function change(newVal) {
                if (newVal == null || newVal == '') {
                    await this.onChangeUserGroup()
                } else {
                    await this.getAllSearchUserName()
                }
            },
            deep: true,
        },
    },
    methods: {
        submit() {
            clearTimeout(this.timeout)
            router.push('/')
        },
        countTime() {
            if (this.num === 0) {
                return this.submit()
            }
            this.num = this.num - 1
            this.timeout = setTimeout(() => this.countTime(), 1000)
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

        async getUserGroup() {
            this.userGroup = []
            await HTTP.get(`Group/getListGroup`)
                .then((res) => {
                    if (res.data._success) {
                        this.userGroup = res.data._Data
                    }
                })
                .catch((error) => {
                    console.log(error)
                })
        },

        async getAllSearchUserName() {
            this.listUser = []
            this.selectedUser = null
            if (this.selectedUserGroup) {
                HTTP.get(
                    `User_Group/getAllSearchUserNameGroupWithGroupId/${this.selectedUserGroup}/${this.searchUserName}`,
                )
                    .then((res) => {
                        if (res.data._success) {
                            this.listUser = res.data._Data
                        }
                    })
                    .catch((error) => {
                        console.log(error)
                    })
            }
        },

        async onChangeUserGroup() {
            this.listUser = []
            this.selectedUser = null
            if (this.selectedUserGroup) {
                HTTP.get(`User_Group/getAllUserNameGroupWithGroupId/${this.selectedUserGroup}`)
                    .then((res) => {
                        if (res.data._success) {
                            this.listUser = res.data._Data
                        }
                    })
                    .catch((error) => {
                        console.log(error)
                    })
            }
        },

        async getPermissionUserMenu(data) {
            this.loading = true
            await HTTP.get('/Permission_Use_Menus/getPermissionUserMenuWithUserId/' + data)
                .then((res) => {
                    if (res.data._Data.length > 0) {
                        this.listModule.map((ele) => {
                            res.data._Data.map((element) => {
                                if (ele.id === element.idModule) {
                                    ele.add = element.add
                                    ele.addMember = element.addMember
                                    ele.confirm = element.confirm
                                    ele.confirmMulti = element.confirmMulti
                                    ele.delete = element.delete
                                    ele.deleteMulti = element.deleteMulti
                                    ele.export = element.export
                                    ele.refuse = element.refuse
                                    ele.update = element.update
                                }
                            })
                        })
                    }
                })
                .catch((err) => {
                    console.log(err)
                })
                .finally(() => {
                    this.loading = false
                })
        },

        async selectedAUser(data) {
            this.selectedUser = data
            await this.getListModule()
            await this.getPermissionUserMenu(data)
        },

        async getListModule() {
            this.listModule = []
            try {
                const result = await HTTP.get(`Modules/getModuleAccessByIdGroup/${this.selectedUserGroup}`)
                    .then((response) => {
                        if (response.data._success) {
                            this.listModule = response.data._Data
                        }
                    })
                    .catch((error) => {
                        console.log(error)
                    })
            } catch (error) {
                console.log(error)
            }
        },

        async confirmSave() {
            if (this.selectedUser && this.listModule.length > 0) {
                this.$confirm.require({
                    message: 'Bạn có muốn tiếp tục?',
                    header: 'Xác nhận',
                    icon: 'pi pi-check',
                    accept: async () => {
                        await this.callApiSave()
                        this.showSuccess('Cập nhật thành công!')
                    },
                    onHide: async () => {
                        // this.showError('Đã xảy ra lỗi');
                        // await this.selectedAUser(this.selectedUser);
                    },
                })
            } else {
                this.showInfo('Vui lòng kiểm tra lại dữ liệu!!!')
            }
        },

        async callApiSave() {
            this.listModule.map(async (ele) => {
                const newPermission = {
                    add: ele.add,
                    update: ele.update,
                    delete: ele.delete,
                    deleteMulti: ele.deleteMulti,
                    confirm: ele.confirm,
                    confirmMulti: ele.confirmMulti,
                    refuse: ele.refuse,
                    addMember: ele.addMember,
                    export: ele.export,
                    userModified: ele.userModified,
                }
                await HTTP.put(`Permission_Use_Menus/updatePermissionUserMenu/${this.selectedUser}/${ele.id}/${this.selectedUserGroup}`, newPermission)
                    .then(async res => {
                        if (res.data._success) {
                            await this.getPermissionUserMenu(this.selectedUser);
                        }
                    })
                    .catch(err => console.log(err))
            })
        },

        selectModule(module) {
            this.selectedModule = module
            this.selectedTab = module.id
        },

        cancel() {
            history.go(-1)
        },
    },
    components: { itemPermissionActionModule },
}
</script>

<style lang="scss" scoped>
    @media (min-width: 576px){
        .col-sm-5 {
            flex: 0 0 auto;
            width: 100%;
        }
        .col-sm-7 {
            display: flex;
            justify-content: flex-start;
            width: 100%;
        }
        .text-size{
            font-size: 100%;
        }
    }
    @media (min-width: 768px) {
        .col-sm-5 {
            flex: 0 0 auto;
            width: 100%;
        }
        .text-size{
            font-size: 60%;
        }
    }
    @media (min-width: 992px) {
        .col-sm-5 {
            flex: 0 0 auto;
            width: 100%;
        }
        .text-size{
            font-size: 60%;
        }
    }
    @media (min-width: 1200px) {
        .col-sm-5 {
            flex: 0 0 auto;
            width: 41.66666667%;
        }
        .col-sm-7 {
            display: flex;
            justify-content: flex-end;
            width: 58.33333333%;
        }
        .text-size{
            font-size: 60%;
        }
    }  
    @media (min-width: 1600px) {
        .col-sm-5 {
            flex: 0 0 auto;
            width: 41.66666667%;
        }
        .col-sm-7 {
            display: flex;
            justify-content: flex-end;
            width: 58.33333333%;
        }
        .text-size{
            font-size: 100%;
        }
    }  
    .p-progress-spinner {
        display: flex;
        position: fixed;
        top: 50%;
        bottom: 0;
        left: 0;
        right: 0;
    }

    .spinner-loading {
        display: flex;
        position: fixed;
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
        background: white;
        z-index: 9999999;
        opacity: 0.5;
    }

    .width_dropdown {
        width: calc(100vh - 370px);
    }

    .actions-buttons {
        display: flex;

        .btn-margin {
            margin-right: 5px;
        }
    }

.permission-user-menu {
    .p-dropdown {
        width: 100%;
    }
    .p-button.p-button-text {
        width: -webkit-fill-available;
    }

    ::v-deep(.p-orderlist .p-orderlist-list) {
        padding: 0;
    }

    ::v-deep(.p-orderlist-list) {
        border-radius: 10px;
    }

    ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item.p-highlight) {
        color: white;
        background: #33adff;
        border-radius: 8px;
    }

    ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item) {
        padding: 0;
    }

    ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item:focus) {
        box-shadow: inset;
    }

    ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item:hover) {
        border-radius: 8px;
    }

    ::v-deep(.p-orderlist-controls) {
        display: none;
        border-radius: 10px;
    }

    ::v-deep(.p-orderlist-header) {
        display: none;
    }

    ::v-deep(.p-orderlist-list) {
        max-height: 100%;
        height: calc(100vh - 274px);
    }

    ::v-deep(.p-datatable-thead) {
        display: none;
    }

    ::v-deep(.p-datatable-thead) {
        display: none;
    }

    ::v-deep(td.p-selection-column) {
        max-width: min-content;
    }

    .possition_search {
        display: block;
        margin-bottom: 10px;
    }

    .tabview-custom {

        i,
        span {
            vertical-align: middle;
        }
        span {
            margin: 0 0.5rem;
        }
    }

    .p-tabview p {
        line-height: 1.5;
        margin: 0;
    }

    body {
        font-family: 'Roboto', sans-serif;
        padding: 50px;
        background: #fcfcfc;
        height: 100vh;
    }

    label {
        cursor: pointer;
    }

    .tab-panel-module {
        height: calc(100vh - 120px);
        overflow: scroll;
        user-select: none;
    }

    .pen-heading {
        font-weight: bold;
        font-size: 4em;
        text-align: center;
        margin-bottom: 40px;
        color: #333;
    }

    .card_content {
        border: 2px #0e0f10 solid;
        box-shadow: 0 2px 3px rgba(213, 218, 223, 0.35);
        border-radius: 8px;
    }
    .card_content_overflow {
        overflow-wrap: anywhere; // them moi
    }

    .card_content_right {
        margin-left: 0px;
    }

    .tab-item {
        background: white;
        border: 1px #d5dadf solid;
        border-left: 5px solid #d5dadf;
        box-shadow: 0 2px 3px rgba(213, 218, 223, 0.35);
        border-radius: 8px;
        cursor: pointer;
        transition: all 0.2s ease;
        margin-top: 15px;
        margin-left: 1px;
    }

    .tab-item-sub {
        border-right: 1px #d5dadf solid;
        padding: 15px;
    }

    .tab-item-sub-content {
        display: flex;
        justify-content: left;
    }

    .tab-item.active {
        box-shadow: 0 3px 3px 2px rgba(213, 218, 223, 0.35);
        border-left: 5px solid #33adff;
    }

    .tab-item:hover {
        box-shadow: 0 3px 3px 2px rgba(213, 218, 223, 0.35);
        background-color: var(--blue-100);
    }

    .tab-item__heading {
        font-weight: bold;
        line-height: 1.3em;
        color: #314f8d;
        overflow-wrap: anywhere;
    }

    .tab-item__subheading {
        font-size: 18px;
        color: #333;
        margin: 0;
    }

    .tab-content__header {
        color: #314f8d;
        font-weight: bold;
        margin: 0 0 30px;
        font-size: 36px;
        line-height: 1.3em;
        letter-spacing: 0.02em;
    }

    .tab-content__text {
        margin: 0 0 30px;
        font-size: 1.25em;
    }

    .tab-content__btn {
        display: inline-block;
        margin-bottom: 30px;
        padding: 16px 50px;
        cursor: pointer;
        text-decoration: none;
        font-size: 14px;
        text-transform: uppercase;
        font-weight: 900;
        position: relative;
        transition: all 0.3s ease;
        text-align: center;
        line-height: 1;
        border-radius: 3px;
        background-color: transparent;
        box-shadow: 0 2px 3px rgba(213, 218, 223, 0.35);
        color: #33adff;
        fill: #33adff;
        border: 2px solid #33adff;
    }

    .tab-content__btn:hover {
        color: #33adff;
        text-decoration: none;
        box-shadow: 0 3px 3px 2px rgba(213, 218, 223, 0.35);
    }

    .tab-content__testimonial {
        margin-bottom: 15px;
        font-size: 1em;
        color: rgba(0, 0, 0, 0.75);
        font-style: italic;
    }

    .tab-content__testimonial-author {
        margin-bottom: 5px;
        font-size: 1em;
        color: rgba(0, 0, 0, 0.75);
        font-weight: bold;
    }

    ::-webkit-scrollbar {
        width: 20px;
    }

    ::-webkit-scrollbar-track {
        background-color: transparent;
    }

    ::-webkit-scrollbar-thumb {
        background-color: #d6dee1;
        border-radius: 20px;
        border: 6px solid transparent;
        background-clip: content-box;
    }

    ::-webkit-scrollbar-thumb:hover {
        background-color: #a8bbbf;
    }
}
</style>
