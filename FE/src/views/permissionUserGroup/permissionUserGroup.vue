<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12">
                        <div class="spinner-loading" v-if="loading">
                            <ProgressSpinner style="width: 50px; height: 50px" strokeWidth="3" animationDuration=".2s" />
                        </div>
                    </div>
                </div>
                <div class="permission-user-menu row">
                    <div class="col-sm-12 col-md-2 col-lg-2 card_content p-2">
                        <div class="row card_content_overflow">
                            <div class="col-sm-12 col-md-12 col-lg-12">
                                <template v-if="userGroup.length <= 0">
                                    <p class="p-2 text-black-50">Không tìm thấy.</p>
                                </template>
                                <OrderList
                                    v-model="userGroup"
                                    style="height: calc(100vh - 150px)"
                                    dataKey="id"
                                    v-if="userGroup.length > 0"
                                >
                                    <template #item="data">
                                        <Button @click="async () => await selectedGroup(data.item.id)" class="p-button-text">
                                            <div class="user-group">
                                                <div class="group-list-detail">
                                                    <h5 class="mb-2 text-body">{{ data.item.nameGroup }}</h5>
                                                </div>
                                            </div>
                                        </Button>
                                    </template>
                                </OrderList>
                            </div>
                        </div>
                        <div v-if="userGroup.length > 0" class="row">
                            <div class="col-sm-12 col-md-12 col-lg-12 ">
                                <div class="row">
                                    <div class="col-sm-5 mt-2">
                                        <Button label="Lưu" icon="pi pi-check" class="p-button-primary p-button-icon custom-button-h text-size"  @click="confirmSave()"></Button>
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
                    <div class="col-sm-12 col-md-10 col-lg-10 card_content p-2">
                        <div id="v-tabs" class="tab-panel-module">
                            <div
                                v-for="module in listModule"
                                :key="module"
                                @click="selectModule(module)"
                                :class="{ active: selectedTab === module.id }"
                                class="tab-item row"
                            >
                                <div class="col-sm-3 tab-item-sub d-flex">
                                    <h5 class="tab-item__heading field-checkbox d-flex align-items-center">
                                        <input
                                            class="input-checkbox"
                                            type="checkbox"
                                            :id="module.id"
                                            :value="module.id"
                                            v-bind:checked="module.access"
                                            @change="isSelectedCheck(module, $event.target.checked)"
                                        />
                                        <label :for="module.id" class="ms-2">{{ module.nameModule }}</label>
                                        
                                    </h5>
                                </div>
                                <div class="col-sm-9 tab-item-sub tab-item-sub-content">
                                    <div class="row">
                                        <div class="col-sm-12 action__module field-checkbox d-flex align-items-center">
                                            <div
                                                v-if="module.showAction"
                                                v-for="item in arrayActionModule" :key="item"
                                            >
                                                <div v-if="item.module.id === module.id">
                                                    <input
                                                        class="input-checkbox"
                                                        type="checkbox"
                                                        :id="module.id +'-'+item.actionModule.id"
                                                        :value="item.actionModule.id"
                                                        v-bind:checked="item.access === true"
                                                        :disabled="!this.selectedGroupId"
                                                        @change="isSelectedCheckPermission(item, $event.target.checked)"
                                                    />
                                                    <label :for="module.id +'-'+item.actionModule.id" class="me-3">{{ item.actionModule.name }}</label>
                                                </div>
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
                header="Truy cập bị từ chối!"
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
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { HTTP } from '@/http-common'
    import { ApiApplication, HttpStatus } from '@/config/app.config'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import router from '@/router'
    import { checkAccessModule } from '@/helper/checkAccessModule'

    export default {
        name: 'permissionUserGroup',
        data() {
            return {
                displayBasic: false,
                selectedGroupId: null,
                loading: false,
                selectedUserGroup: null,
                selectedTab: null,
                userGroup: [],
                listModule: [],
                selectedModule: [],
                selectedAccess: [],
                arrModuleAccess: [],
                arrModulePermissionAll: [],
                num: 5,
                arraySend: [],
                arraycompare: [],
                isChecked: false,
                showAction: false,
                arrayActionModule : [],
                arrayrenderActionModule : [],
                arrayPermissionGroupModuleAction : [],
                selectedDefaultPermission : [],
                selectChangeDefaultPermisison : [],
                selectChangeAccessModule : [],
                arraysendFalse : [],
                arraysendTrue : [],
                arraysendAdd : [],
                arrayfilterIdModuleAction : [],
                arrayTrueAction : true,
                arrayFalseAction : true,
                arrayAddAction : true,
                // Mảng chứa các module và action đã phân quyền mặc định
                arrayActionModuleDefault : [],
                arrayDeleteModule : [],
                arrayAddAccessModule : [],
                addModule:[],
                deleteModule:[],
                addAction:[],
                deleteAction:[],
            }
        },
        async mounted() {
            try {
                await this.getUserGroup()
                //await this.getListModule()
                //await this.getAllActionModule()
                //await this.getPermissionActionModule()
            } catch (err) {
                //this.loading = false
                this.countTime()
                this.displayBasic = true
            }
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
            async getAllActionModule() {
                await HTTP.get('PermissionActionModule/getAllPermissionActionModule')
                    .then((res) => {
                        console.log(res.data);
                        this.arrayActionModule = res.data._Data
                    })
                    .catch((err) => {
                        console.log(err.data)
                    })
            },
            ReloadAPI(){
                location.reload();
            },
            
            async getUserGroup() {
                this.userGroup = []
                await HTTP.get(`Group/getListGroup`).then((res) => {
                    if (res.data) {
                        this.userGroup = res.data._Data
                    }
                })
            },
            async getPermissionActionModule() {
                await HTTP.get('GroupModuleAction/getAllGroupModuleAction')
                    .then((res) => {
                        this.arrayPermissionGroupModuleAction = res.data._Data
                    })
                    .catch((err) => {
                        console.log(err)
                    })
            },
            async selectedGroup(data) {
                this.arraycompare = []
                await this.getListModule()
                await this.getAllActionModule()
                await this.getPermissionActionModule()
                this.loading = true
                this.selectedGroupId = data
                await HTTP.get('Permission_Groups/getPermissionGroup_By_IdGroup/' + this.selectedGroupId)
                    .then((res) => {
                        this.arraycompare = res.data._Data
                        this.loading = false
                    })
                    .catch((err) => console.log(err))
                await this.listModule.map((ele) => {
                    this.arraycompare.map((element) => {
                        if (element.idModule === ele.id && element.access === true) {
                            ele.access = true
                            ele.showAction = true
                        }
                            if (element.idModule === ele.id && element.access === false) {
                                ele.access = false
                                ele.showAction = false
                            }
                        
                    })
                })
                await this.listModule.map(ele=>{
                    if(ele.access === undefined){
                        ele.access = false
                        ele.showAction = false
                    }
                })
                // Lấy 1 mảng action thuộc module nào
                // Lấy các module với các action mặc định đã phân
                await this.arrayActionModule.map(ele=>{
                        this.arrayPermissionGroupModuleAction.map( element => {
                        if(ele.actionModule.id === element.idAction && ele.module.id === element.idModule && element.idGroup === data){
                                ele.access = true
                        }
                    })
                })
            },

            // keep
            async getListModule() {
                this.listModule = []
                this.selectedAccess = []
                this.arraySend = []
                await HTTP.get(`Modules/getListModule`).then((res) => {
                    if (res.data) {
                        this.listModule = res.data._Data
                    }
                })
            },

            cancel() {
                history.go(-1)
            },
            async confirmSave() {
                if (this.arrModulePermissionAll.length > 0) {
                    this.$confirm.require({
                        message: `Quá trình xử lý có thể tốn vài phút. Bạn có muốn tiếp tục?`,
                        header: 'Xác nhận',
                        icon: 'pi pi-exclamation-triangle',
                        accept:async () => {
                            await this.save()
                        },
                    })
                } else {
                    this.$confirm.require({
                        message: 'Bạn có muốn tiếp tục?',
                        header: 'Xác nhận',
                        icon: 'pi pi-check',
                        accept:async () => {
                            await this.save()
                        },
                        reject: () => {
                            return;
                        },
                        onHide: () => {
                            return;
                        },
                    })
                }
            },
            async save() {
                this.loading = true
                const arrayModule = this.addModule.concat(this.deleteModule)
                const arrayActionModule = this.addAction.concat(this.deleteAction)
                await HTTP.post('Permission_Groups/Update_Permission_Group',arrayModule).then(res=>{
                    this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Cập nhật quyền truy cập module thành công!',
                            life: 3000,
                        })   
                }).catch(err=>{
                    this.$toast.add({
                            severity: 'error',
                            summary: 'Cập nhật quyền nhóm thất bại',
                            detail: 'Cập nhật quyền nhóm thất bại',
                            life: 3000,
                        })
                })  
                if(arrayActionModule.length > 0) {
                       await HTTP.post('GroupModuleAction/updateAction',arrayActionModule).then(res=>{
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: 'Cập nhật quyền thao tác module thành công!',
                                life: 3000,
                            })
                        }).catch(err=>{
                            success = false
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Cập nhật quyền thao tác thất bại',
                                detail: 'Cập nhật quyền thao tác thất bại',
                                life: 3000,
                            })
                        }) 
                    } 

                if(this.deleteModule.length > 0){
                    this.deleteModule.map( async ele=>{
                     await HTTP.post(`GroupModuleAction/removeAllAction/idgroup=${ele.idGroup}/idModule=${ele.idModule}`)
                    })
                }

                await this.selectedGroup(this.selectedGroupId)
            },
            
            selectModule(module) {
                this.selectedModule = module
                this.selectedTab = module.id
                this.arrModuleAccess = []
                this.arrModuleAccess = this.selectedAccess
            },

            isSelectedCheck(module, isChecked) {  
                if(isChecked){  
                    module.showAction = true;
                    const object = {
                        idGroup:this.selectedGroupId, 
                        idModule: module.id,
                        access : true,
                    }
                    if(module.access === false || module.access === undefined){
                        this.addModule.push(object)
                    } 

                    this.deleteModule = this.deleteModule.filter(item => item.idModule !== module.id)
                }else{
                    module.showAction = false;
                    const object = {
                        idGroup:this.selectedGroupId, 
                        idModule: module.id,
                        access : false,
                    }
                    if(module.access === true){
                        this.deleteModule.push(object)
                    }

                    this.addModule = this.addModule.filter(item => item.idModule !== module.id)
                }
            },

            isSelectedCheckPermission(item, isChecked) {
                if (isChecked) {
                    const object = {
                        "idGroup": this.selectedGroupId,
                        "idModule": item.module.id,
                        "idAction": item.actionModule.id,
                        "isDeleted": false,
                        "userCreated": Number(checkAccessModule.getUserIdCurrent())
                    }

                    if(item.access === false || item.access === undefined){
                       this.addAction.push(object)
                    }  
                    this.deleteAction = this.deleteAction.filter(items => items.idAction !== item.actionModule.id )
                } else {
                    const object = {
                        "idGroup": this.selectedGroupId,
                        "idModule": item.module.id,
                        "idAction": item.actionModule.id,
                        "isDeleted": true,
                        "userCreated": Number(checkAccessModule.getUserIdCurrent())
                    }

                    if(item.access === true){
                       this.deleteAction.push(object)
                    }
                    this.addAction = this.addAction.filter(items => items.idAction !== item.actionModule.id )
                }
            }

        },
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

    .card_content {
        border: 2px #0e0f10 solid;
        box-shadow: 0 2px 3px rgba(213, 218, 223, 0.35);
        border-radius: 8px;
    }
    
    .card_content_overflow {
        overflow-wrap: anywhere; // them moi
    }


    .permission-user-menu {
        ::v-deep(.p-orderlist .p-orderlist-list) {
            padding: 0;
            border: 2px black solid;
            border-radius: 8px;
        }

        ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item) {
            padding: 0;
        }

        ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item.p-highlight) {
            color: white;
            background: #33adff;
            border-radius: 8px;
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
            height: calc(100vh - 150px);
        }

        ::v-deep(.p-datatable-thead) {
            display: none;
        }

        ::v-deep(td.p-selection-column) {
            max-width: min-content;
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

        .p-button.p-button-text {
            width: 100%;
        }

        .input-checkbox {
            border-radius: 6px;
            transform: scale(1.5);
            margin-right: 9px;
            cursor: pointer;
        }

        .tab-panel-module {
            height: calc(100vh - 100px);
            overflow: scroll;
            user-select: none;
            padding: 15px 15px 0px 15px;
        }

        .pen-heading {
            font-weight: bold;
            font-size: 4em;
            text-align: center;
            margin-bottom: 40px;
            color: #333;
        }

        .tab-item {
            background: white;
            border: 1px #d5dadf solid;
            border-left: 5px solid #d5dadf;
            box-shadow: 0 2px 3px rgba(213, 218, 223, 0.35);
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.2s ease;
            margin-bottom: 15px;
        }

        .tab-item-sub {
            border: 1px #d5dadf solid;
            padding: 15px;
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

        .action__module {
            font-size: 15px;
            border-radius: 5px;
        }

        .action__module--items {
            margin-left: 8px;
        }
        .text-body {
            font-size: 17px;
        }
       
    }
</style>
