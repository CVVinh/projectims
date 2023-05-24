<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="container text-center">
            <div v-if="checkGroup('admin')">
                <div class="row">
                    <div class="col-sm-12 col-md-12">
                        <p class="fs-2 text-start">TỔNG QUAN</p>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12 col-md-12">
                        <nav style="--bs-breadcrumb-divider: '>'" aria-label="breadcrumb">
                            <ol class="breadcrumb">
                                <li class="breadcrumb-item">Trang chủ</li>
                                <li class="breadcrumb-item active" aria-current="page">Tổng quan</li>
                            </ol>
                        </nav>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12">
                        <DashBoardAdminView />
                    </div>
                </div>
            </div>
        </div>

        <Toast position="top-right" />

        <div class="container-fluid">
            <ScrollPanel style="width: 100%">
                <div class="mx-3 mt-3 mb-3" v-if="checkGroup('lead') || checkGroup('staff')">
                    <div class="row">
                        <div
                            class="col-sm-12 col-md-8 border-end border-bottom mb-3"
                            style="padding: 0px 0px 30px 25px"
                        >
                            <UserInfoLeaveOffOverView :selectedallUser="false" />
                        </div>
                        <div class="col-sm-12 col-md-4"></div>
                    </div>
                </div>
                <div class="mx-3 mt-3 mb-3" v-if="checkGroup('office')">
                    <div class="row">
                        <div
                            class="col-sm-12 col-md-8 border-end border-bottom mb-3"
                            style="padding: 0px 0px 30px 25px"
                        >
                            <UserInfoLeaveOffOverView :selectedallUser="true" />
                        </div>
                        <div class="col-sm-12 col-md-4"></div>
                    </div>
                </div>
                <div class="mx-3 mt-3 border-bottom" v-if="checkGroup('lead') || checkGroup('staff')">
                    <div class="row">
                        <div class="col-sm-12 col-md-8 border-end mb-3">
                            <h5>Thống kê tăng ca</h5>
                            <UserInfoOTOverView />
                        </div>
                    </div>
                </div>
            </ScrollPanel>
        </div>
    </LayoutDefaultDynamic>
</template>
<script>
import sideBar from '@/layouts/LayoutDefault/Sidebar.vue'
import { HTTP } from '@/http-common'
import ViewMenusVue from './Menus/viewsMenu/ViewMenus.vue'
import { UserRoleHelper } from '@/helper/user-role.helper'
import { LocalStorage } from '@/helper/local-storage.helper'
import { checkAccessModule } from '@/helper/checkAccessModule'
import UserInfoLeaveOffOverView from '@/views/Overviews/UserInfoLeaveOffOverView.vue'
import UserInfoOTOverView from '@/views/Overviews/UserInfoOTOverView.vue'
import DashBoardAdminView from './DashBoardView/DashBoardAdminView.vue'
export default {
    name: 'home',
    data() {
        return {
            getToken: checkAccessModule.setToken(),
            loading: true,
            Menus: [],
            color: [
                'bg-primary',
                'bg-warning',
                'bg-info',
                'background-color-cyan-600',
                'background-color-yellow-800',
                'background-color-green-600',
                'background-color-blue-gray-500',
            ],
        }
    },
    created() {
        if(checkAccessModule.checkAccessRights()){
            window.localStorage.setItem('activeTag', '')
        }else{     
            alert("Bạn không có quyền")
            localStorage.clear();
            window.location.reload()
        }
       
    },
    async mounted() {
        checkAccessModule.getListAction();
        this.reloadClass()
        // this.getListMenuParent()
        // await this.getRoles()
        //await UserRoleHelper.getUserRole(this.user, this.leader)
    },
    components: {
        ViewMenusVue,
        sideBar,
        UserInfoLeaveOffOverView,
        UserInfoOTOverView,
        DashBoardAdminView,
    },
    methods: {
        reloadClass() {
            var el = document.getElementById('NavItems').querySelectorAll('a')
            if (el != null) {
                for (let i = 0; i < el.length; i++) {
                    if (el[i].classList.contains('active-nav-item')) {
                        el[i].classList.remove('active-nav-item')
                    }
                }
            }
        },
        checkGroup(nameGroup){
            checkAccessModule.checkAccessRights();
            const name = nameGroup.toLowerCase()
            if(name === 'admin'){
                if(checkAccessModule.isAdmin()){
                    return true
                }else{
                    return false
                }
            }

            if(name === 'lead'){
                if(checkAccessModule.isLead()){
                    return true
                }else{
                    return false
                }
            }

            if(name === 'pm'){
                if(checkAccessModule.isPm()){
                    return true
                }else{
                    return false
                }
            }

            if(name === 'lead'){
                if(checkAccessModule.isLead()){
                    return true
                }else{
                    return false
                }
            }

            if(name === 'staff'){
                if(checkAccessModule.isStaff()){
                    return true
                }else{
                    return false
                }
            }

            if(name === 'office'){
                if(checkAccessModule.isOffice()){
                    return true
                }else{
                    return false
                }
            }

        },
        isNotPMAdmin() {
            if (checkAccessModule.getListGroup().includes('1') || checkAccessModule.getListGroup().includes('5')) {
                return false
            } else {
                return true
            }
        },
        isNotOffice(){
            if (checkAccessModule.getListGroup().includes('2')) {
                return false
            } else {
                return true
            }
        },
        getRoles() {
            HTTP.get('Group/getListGroup').then((res) => {
                this.listRole = res.data
            })
        },
        addTask() {
            this.$router.push({ name: 'addtask' })
        },
        
    }
}
</script>
<style lang="scss" scoped>
    @media only screen and (max-width: 500px) {
        .padd {
            padding: 10px 60px;
        }
    }
</style>
