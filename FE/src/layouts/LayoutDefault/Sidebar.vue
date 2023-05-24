<template>
    <Toast />
    <nav id="sidebarMenu" class="d-lg-block sidebar bg-white" :class="{ 'collapse-width-map': isCollapse == true }">
        <!-- style="margin-right: 9px !important; margin-left: 1rem !important" -->
        <div class="position-sticky">
            <div class="list-group list-group-flush m-t-5" id="NavItems">
                <!-- Tổng quan -->
                <div
                    class="py-2 ripple sidebar_after list-group-item-action d-flex padding-left-custom"
                    data-bs-toggle="collapse"
                    data-bs-target="#overview-collapse"
                    aria-expanded="true"
                    :class="{
                        'sidebar-top-level-items': isActive1,
                        afterCollapse: isCollapse == true,
                        'active-boder-left': isActive1,
                    }"
                    @click="onActive(1)"
                    style="cursor: pointer"
                >
                    <a style="width: 80%">
                        <i class="bx bx-notepad"></i>
                        <span :class="{ 'label-disble': isCollapse == true }">Tổng quan</span>
                    </a>
                </div>
                <div
                    class="collapse"
                    id="overview-collapse"
                    style="margin-top: 5px"
                    :class="{ 'label-disble': isCollapse == true }"
                >
                    <ul class="dropdownList btn-toggle-nav list-unstyled" :class="{ 'active-boder-left': isActive1 }">
                        <li class="list-group-item-action mt-1 drop-botton-css" v-if="checkshow('project')">
                            <router-link
                                :to="{ name: 'project', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                aria-current="true"
                                @click="activeTag = 'tag1'"
                                :class="{ 'active-nav-item': activeTag === 'tag1' }"
                            >
                                <!-- <i class="bx bx-notepad"></i> -->
                                <span :class="{ 'label-disble': isCollapse == true }" class="padding-left-chill">Dự án</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action"
                           v-if="(checkcanOperation('add','users') && checkcanOperation('delete','users') && checkcanOperation('update','users')) || checkIsGroup('office')"
                        >
                            <router-link
                                :to="{ name: 'users', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                @click="activeTag = 'tag2'"
                                :class="{ 'active-nav-item': activeTag === 'tag2' }"
                            >
                                <!-- <i class="bx bx-user"></i> -->
                                <span class="padding-left-chill">Người dùng</span>
                            </router-link>
                        </li>
                    </ul>
                </div>

                <!-- Thiết bị -->
                <router-link
                    :to="{ name: 'Equipment-Device', params: {} }"
                    class="py-2 ripple list-group-item-action padding-left-custom"
                    @click="activeTag = 'tag4'"
                    :class="{ 'active-nav-item': activeTag === 'tag4', 'active-boder-left': activeTag === 'tag4' }"
                    v-if="checkIsGroup('admin')"
                >
                    <i class="bx bx-devices"></i>
                    <span :class="{ 'label-disble': isCollapse == true }">Thiết bị</span>
                </router-link>
                <!-- Nghỉ phép -->
                <div
                    class="py-2 ripple sidebar_after list-group-item-action d-flex padding-left-custom"
                    data-bs-toggle="collapse"
                    data-bs-target="#settings-collapse-leaveOff"
                    aria-expanded="true"
                    style="cursor: pointer"
                    :class="{
                        'sidebar-top-level-items': isActiveLeaveOff,
                        afterCollapse: isCollapse == true,
                        'active-boder-left': isActiveLeaveOff,
                    }"
                    @click="onActive(4)"
                >
                    <a style="width: 80%">
                        <i class="bx bx-log-out-circle"></i>
                        <span :class="{ 'label-disble': isCollapse == true }">Nghỉ phép</span>
                    </a>
                </div>
                <div
                    class="collapse"
                    id="settings-collapse-leaveOff"
                    style="margin-top: 9px"
                    :class="{ 'label-disble': isCollapse == true }"
                >
                    <ul
                        class="dropdownList btn-toggle-nav list-unstyled"
                        :class="{ 'active-boder-left': isActiveLeaveOff }"
                        v-if="checkshow('leaveoffs')"
                    >
                        <li
                            class="list-group-item-action drop-botton-css"
                            v-if="checkcanOperation('add','leaveOffs')"
                        >
                            <router-link
                                :to="{ name: 'LeaveOffRegisterlists', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                @click="activeTag = 'tag20'"
                                :class="{ 'active-nav-item': activeTag === 'tag20' }"
                            >
                                <!-- <i class="pi pi-calendar-plus"></i> -->
                                <span class="padding-left-chill">Đăng ký</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action drop-botton-css"
                           v-if="checkcanOperation('confirm','leaveOffs')"
                        >
                            <router-link
                                :to="{ name: 'Acceptregisterlists', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                @click="activeTag = 'tag21'"
                                :class="{ 'active-nav-item': activeTag === 'tag21' }"
                            >
                                <!-- <i class="pi pi-check-circle"></i> -->
                                <span class="padding-left-chill">Duyệt</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action"
                           v-if="checkcanOperation('export','leaveOffs')"
                        >
                            <router-link
                                :to="{ name: 'Leaveoff', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                @click="activeTag = 'tag22'"
                                :class="{ 'active-nav-item': activeTag === 'tag22' }"
                            >
                                <!-- <i class="pi pi-calendar"></i> -->
                                <span class="padding-left-chill">Tổng hợp</span>
                            </router-link>
                        </li>
                    </ul>
                </div>
                <!-- /Nghỉ phép -->
                <!-- Tăng ca -->
                <router-link
                    :to="{ name: 'ots', params: {} }"
                    class="py-2 ripple list-group-item-action padding-left-custom"
                    @click="activeTag = 'tag6'"
                    :class="{ 'active-nav-item': activeTag === 'tag6', 'active-boder-left': activeTag === 'tag6' }"
                    v-if="checkshow('ots')"
                >
                    <i class="bx bx-time-five"></i>
                    <span :class="{ 'label-disble': isCollapse == true }">Tăng ca</span>
                </router-link>
                <!-- /Tăng ca -->

                <!-- Đánh giá -->
                <!-- <router-link
                    :to="{ name: 'reviewStaffs', params: {} }"
                    class="py-2 ripple list-group-item-action padding-left-custom"
                    @click="activeTag = 'tag26'"
                    :class="{ 'active-nav-item': activeTag === 'tag26', 'active-boder-left': activeTag === 'tag26' }"
                    v-if="this.showLink.leaveoffPmLead"
                >
                    <i class="bx bx-glasses-alt"></i>
                    <span :class="{ 'label-disble': isCollapse == true }">Đánh giá</span>
                </router-link> -->
                <router-link
                    :to="{ name: 'reviewsTime', params: {} }"
                    class="py-2 ripple list-group-item-action padding-left-custom"
                    @click="activeTag = 'tag26'"
                    :class="{ 'active-nav-item': activeTag === 'tag26', 'active-boder-left': activeTag === 'tag26' }"
                    v-if="checkshow('reviews')"
                >
                    <i class="bx bx-glasses-alt"></i>
                    <span :class="{ 'label-disble': isCollapse == true }">Đánh giá</span>
                </router-link>
                <!-- /Đánh giá -->
                <!-- Đánh giá -->

                <!-- <router-link
                    :to="{ name: 'tasks', params: {} }"
                    class="py-2 ripple list-group-item-action padding-left-custom"
                    @click="activeTag = 'tag25'"
                    :class="{ 'active-nav-item': activeTag === 'tag25', 'active-boder-left': activeTag === 'tag25' }"
                    v-if="checkshow('tasks')"
                >
                    <i class="bx bx-calendar-exclamation"></i>
                    <span :class="{ 'label-disble': isCollapse == true }">Công việc</span>
                </router-link> -->
                <!-- /Đánh giá -->

                <!-- Phân Công -->
                <!-- <router-link
                    :to="{ name: 'tasks', params: {} }"
                    class="py-2 ripple list-group-item-action"
                    @click="activeTag = 'tag17'"
                    :class="{ 'active-nav-item': activeTag === 'tag17' }"
                >
                    <i class="bx bx-task"></i>
                    <span>Phân công</span>
                </router-link> -->

                <!-- /Phân Công -->
                <!-- Công tác -->
                <!-- <router-link
                    :to="{ name: 'remotes', params: {} }"
                    class="py-2 ripple list-group-item-action"
                    @click="activeTag = 'tag7'"
                    :class="{ 'active-nav-item': activeTag === 'tag7' }"
                >
                    <i class="bx bx-notepad"></i>
                    <span>Công tác</span>
                </router-link> -->
                <!-- /Công tác -->
                <!-- Thu chi -->
                <router-link
                    :to="{ name: 'Paid', params: {} }"
                    class="py-2 ripple list-group-item-action padding-left-custom"
                    @click="activeTag = 'tag8'"
                    :class="{ 'active-nav-item': activeTag === 'tag8', 'active-boder-left': activeTag === 'tag8' }"
                    v-if="checkshow('paids')"
                >
                    <i class="bx bx-wallet"></i>
                    <span :class="{ 'label-disble': isCollapse == true }">Thu chi</span>
                </router-link>
                <!-- /Thu chi -->
                <!-- Quy định -->
                <router-link
                    :to="{ name: 'ruleinfo', params: {} }"
                    class="py-2 ripple list-group-item-action padding-left-custom"
                    @click="activeTag = 'tag9'"
                    :class="{ 'active-nav-item': activeTag === 'tag9', 'active-boder-left': activeTag === 'tag9' }"
                    v-if="checkshow('rules')"
                >
                    <i class="bx bxs-notepad"></i>
                    <span :class="{ 'label-disble': isCollapse == true }">Quy định</span>
                </router-link>
                <router-link
                    :to="{ name: 'blog', params: {} }"
                    class="py-2 ripple list-group-item-action padding-left-custom"
                    @click="activeTag = 'tag27'"
                    :class="{ 'active-nav-item': activeTag === 'tag27' }"
                    v-if="checkshow('blog') && !checkIsGroup('admin')"
                >
                    <i class="bx bxl-blogger"></i>
                    <span :class="{ 'label-disble': isCollapse == true }">Blog</span>
                </router-link>

                <router-link
                    :to="{ name: 'bloglist', params: {} }"
                    class="py-2 ripple list-group-item-action padding-left-custom"
                    @click="activeTag = 'tag28'"
                    :class="{ 'active-nav-item': activeTag === 'tag28' }"
                    v-if="checkshow('bloglist') && !checkIsGroup('admin')"
                >
                    <i class="bx bxl-blogger"></i>
                    <span :class="{ 'label-disble': isCollapse == true }">Quản lý blog</span>
                </router-link>
    
                <!-- TimeSheet -->
               <!--  <router-link
                    :to="{ name: 'timeSheetDailys', params: {} }"
                    class="py-2 ripple list-group-item-action padding-left-custom"
                    @click="activeTag = 'tag31'"
                    :class="{ 'active-nav-item': activeTag === 'tag31', 'active-boder-left': activeTag === 'tag31' }"
                    v-if="checkshow('timeSheetDailys')"
                >
                    <i class="bx bx-table"></i>
                    <span :class="{ 'label-disble': isCollapse == true }">TimeSheet</span>
                </router-link> -->
                <!-- TimeSheet -->

                <div
                    class="py-2 ripple sidebar_after list-group-item-action d-flex padding-left-custom"
                    data-bs-toggle="collapse"
                    data-bs-target="#settings-collapse-blog"
                    aria-expanded="true"
                    style="cursor: pointer; margin-bottom: 9px"
                    :class="{
                        'sidebar-top-level-items': isActive4,
                        afterCollapse: isCollapse == true,
                        'active-boder-left': isActive4,
                    }"
                    v-if="checkIsGroup('admin')"
                    @click="onActive(5)"
                >
                    <a style="width: 80%">
                        <i class="bx bxl-blogger"></i>
                        <span :class="{ 'label-disble': isCollapse == true }">Blog</span>
                    </a>
                </div>

                <div class="collapse" id="settings-collapse-blog" :class="{ 'label-disble': isCollapse == true }">
                    <ul class="dropdownList btn-toggle-nav list-unstyled" :class="{ 'active-boder-left': isActive4 }">
                        <li class="list-group-item-action drop-botton-css" v-if="checkIsGroup('admin')">
                            <router-link
                                :to="{ name: 'blog', params: {} }"
                                class="py-2 ripple list-group-item-action padding-left-custom"
                                @click="activeTag = 'tag27'"
                                :class="{ 'active-nav-item': activeTag === 'tag30' }"
                            >
                                <span :class="{ 'label-disble': isCollapse == true }">Blog</span>
                            </router-link>
                        </li>

                        <li
                            class="list-group-item-action drop-botton-css"
                        >
                            <router-link
                                :to="{ name: 'bloglist', params: {} }"
                                class="py-2 ripple list-group-item-action padding-left-custom"
                                @click="activeTag = 'tag28'"
                                :class="{ 'active-nav-item': activeTag === 'tag28' }"
                            >
                                <span :class="{ 'label-disble': isCollapse == true }">Quản lý blog</span>
                            </router-link>
                        </li>
                    </ul>
                </div>

                <!-- /Quy định -->
                <!--  Báo cáo-->
                <!-- <div
                    class="py-2 ripple sidebar_after list-group-item-action d-flex"
                    data-bs-toggle="collapse"
                    data-bs-target="#report-collapse"
                    aria-expanded="true"
                    style="cursor: pointer; margin-bottom: 5px"
                    :class="{ 'sidebar-top-level-items': isActive2 }"
                    @click="onActive(2)"
                >
                    <a style="width: 80%">
                        <i class="bx bx-notepad"></i>
                        <span>Báo cáo</span>
                    </a>
                </div>
                <div class="collapse" id="report-collapse">
                    <ul class="dropdownList btn-toggle-nav list-unstyled">
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'listprojects', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag10'"
                                :class="{ 'active-nav-item': activeTag === 'tag10' }"
                            >
                                <i class="bx bx-spreadsheet"></i>
                                <span>Bảng thời gian</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'projectDetails', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag11'"
                                :class="{ 'active-nav-item': activeTag === 'tag11' }"
                            >
                                <i class="bx bx-notepad"></i>
                                <span>Chi tiết dự án</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'TaskReport', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag12'"
                                :class="{ 'active-nav-item': activeTag === 'tag12' }"
                            >
                                <i class="bx bxs-report"></i><span>Báo cáo công việc</span>
                            </router-link>
                        </li>
                    </ul>
                </div> -->
                <!--  /Báo cáo-->

                

                <!-- Thiết lập -->
                <div
                    class="py-2 ripple sidebar_after list-group-item-action d-flex padding-left-custom"
                    data-bs-toggle="collapse"
                    data-bs-target="#settings-collapse"
                    aria-expanded="true"
                    style="cursor: pointer; margin-bottom: 9px"
                    :class="{
                        'sidebar-top-level-items': isActive3,
                        afterCollapse: isCollapse == true,
                        'active-boder-left': isActive3,
                    }"
                    @click="onActive(3)"
                >
                    <a style="width: 80%">
                        <i class="bx bx-cog"></i>
                        <span :class="{ 'label-disble': isCollapse == true }">Cài đặt</span>
                    </a>
                </div>
                <div
                    class="collapse"
                    id="settings-collapse"
                    v-if="checkIsGroup('admin')"
                    :class="{ 'label-disble': isCollapse == true }"
                >
                    <ul class="dropdownList btn-toggle-nav list-unstyled" :class="{ 'active-boder-left': isActive3 }">
                        <li class="list-group-item-action drop-botton-css" v-if="checkshow('groups')">
                            <router-link
                                :to="{ name: 'setupsGroups', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                @click="activeTag = 'tag3'"
                                :class="{ 'active-nav-item': activeTag === 'tag3' }"
                            >
                                <!-- <i class="bx bxs-group"></i> -->
                                <span class="padding-left-chill">Nhóm</span>
                            </router-link>
                        </li>

                        <!-- <li class="list-group-item-action drop-botton-css" v-if="checkshow('menus')">
                            <router-link
                                :to="{ name: 'setupsMenus', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                aria-current="true"
                                @click="activeTag = 'tag13'"
                                :class="{ 'active-nav-item': activeTag === 'tag13' }"
                            >
                                <i class="bx bx-list-ul"></i>
                                <span class="padding-left-chill">Menu</span>
                            </router-link>
                        </li> -->

                        <li class="list-group-item-action drop-botton-css" v-if="checkshow('modules')">
                            <router-link
                                :to="{ name: 'setupsModules', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                aria-current="true"
                                @click="activeTag = 'tag14'"
                                :class="{ 'active-nav-item': activeTag === 'tag14' }"
                            >
                                <!-- <i class="pi pi-database"></i> -->
                                <span class="padding-left-chill">Chức năng</span>
                            </router-link>
                        </li>

                        <li class="list-group-item-action drop-botton-css" v-if="checkshow('actionmodules')">
                            <router-link
                                :to="{ name: 'setupsActionModules', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                aria-current="true"
                                @click="activeTag = 'tag17'"
                                :class="{ 'active-nav-item': activeTag === 'tag17' }"
                            >
                                <!-- <i class="pi pi-server"></i> -->
                                <span class="padding-left-chill">Thao tác</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action drop-botton-css" v-if="checkshow('permissionactionmodules')">
                            <router-link
                                :to="{ name: 'permissionActionModules', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                aria-current="true"
                                @click="activeTag = 'tag18'"
                                :class="{ 'active-nav-item': activeTag === 'tag18' }"
                            >
                                <!-- <i class="bx bx-spreadsheet"></i> -->
                                <span class="padding-left-chill">Thao tác của chức năng</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action drop-botton-css" v-if="checkshow('permissiongroups')">
                            <router-link
                                :to="{ name: 'permissionGroups', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                aria-current="true"
                                @click="activeTag = 'tag16'"
                                :class="{ 'active-nav-item': activeTag === 'tag16' }"
                            >
                                <!-- <i class="bx bxs-detail"></i> -->
                                <span class="padding-left-chill">Phân quyền</span>
                            </router-link>
                        </li>
                        <!-- <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'permissionUserMenus', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                aria-current="true"
                                @click="activeTag = 'tag15'"
                                :class="{ 'active-nav-item': activeTag === 'tag15' }"
                            >
                                <i class="bx bxs-user-account"></i> 
                                <span class="padding-left-chill">Quyền người dùng</span>
                            </router-link>
                        </li> -->
                    </ul>
                </div>
                <!-- /Thiết lập -->
                <!-- <a class="py-2 ripple list-group-item-action"><i class="bx bx-note"></i><span>Thông tin</span></a> -->
                <!-- <a class="py-2 ripple list-group-item-action"
                    ><i class="bx bxs-credit-card"></i>
                    <span>Chi tiêu</span>
                </a> -->
            </div>
        </div>
    </nav>
    <div
        class="sidebar-1 d-flex justify-content-start"
        @click="handlerCollapse()"
        :class="{ 'collapse-width-map': isCollapse == true }"
    >
        <h5 class="text-center ms-3 d-flex justify-content-start align-items-center">
            <span v-if="isCollapse == false" class="d-flex justify-content-center align-items-center">
                <i class="bx bxs-chevrons-left me-2"></i> Collapse sidebar
            </span>
            <span v-else><i class="bx bx-chevrons-right"></i></span>
        </h5>
    </div>
    <section class="home" :class="{ 'copllapse-home': isCollapse == true }">
        <div class="text">
            <slot> </slot>
        </div>
    </section>
</template>

<script>
import { HTTP, GET_GROUP_BY_ID } from '@/http-common'
import { Module } from '@/views/Menus'
import jwt_decode from 'jwt-decode'
import { LocalStorage } from '@/helper/local-storage.helper'
import { UserRoleHelper } from '@/helper/user-role.helper'
import { checkAccessModule } from '@/helper/checkAccessModule'
import sideBar from '@/layouts/LayoutDefault/Sidebar.vue'
export default {
    name: 'home',
    data() {
        return {
            moduleCurrent: '',
            menus: ['this is item default'],
            modules: [],
            idModule: '',
            submenu: [],
            isLoading: true,
            module: [],
            isActive1: false,
            isActive2: false,
            isActive3: false,
            isActive4: false,
            isActiveLeaveOff: false,
            dataGrounp: [],
            path: null,
            token: null,
            activeTag: null,
            dopdowntag: null,
            showLink: {
                leaveoffAdminPm: false,
                leaveoffAdminSample: false,
                leaveoffStaff: false,
                leaveoffSample: false,
                leaveoffAdminPmLead: false,
                leaveoffAdminPmLeadSample: false,
                leaveoffPmLead: false,
                leaveoffPM: false,
            },
            isCollapse: false,
            showSidebar: [],
        }
    },
    async created() {
        this.token = LocalStorage.jwtDecodeToken()
        this.activeTag = localStorage.getItem('activeTag')
        await checkAccessModule.checksidebar(this.showSidebar)
    },

    watch: {
        activeTag(newValue) {
            localStorage.setItem('activeTag', newValue)
        },
    },
    methods: {
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
                if (checkAccessModule.isOffice()) {
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
        checkcanOperation(operation,nameModule){
            if(checkAccessModule.checkOperationSidebar(operation,nameModule)){
                return true
            }else{
                return false
            }
        },
        checkshow(nameModule) {
            const name = nameModule.toLowerCase()
            if (this.showSidebar[name] === true) {
                return true
            } else {
                return false
            }       
        },
        onActive(dropdownNumber) {
            if (dropdownNumber === 1) {
                this.isActive1 = !this.isActive1
            } else if (dropdownNumber === 2) {
                this.isActive2 = !this.isActive2
            } else if (dropdownNumber === 3) {
                this.isActive3 = !this.isActive3
            } else if (dropdownNumber === 4) {
                this.isActiveLeaveOff = !this.isActiveLeaveOff
            } else if (dropdownNumber === 5) {
                this.checkStaff()
                this.isActive4 = !this.isActive4
            }
        },
        async getRole() {
            HTTP.get(GET_GROUP_BY_ID(Number(this.token.listGroup[0]))).then((res) => {
                this.dataGrounp = res.data
            })
        },
        getListMenuByIdModule(id) {
            HTTP.get(`Menu/getMenuByModule?moduleId=${id}`).then((res) => {
                if (res.status == 200) {
                    this.menus = res.data
                    const temp = []
                    this.menus.forEach((menu) => {
                        if (menu.parent != 0 && menu.isDeleted != 1) {
                            temp.push(menu)
                        }
                    })
                    this.menus = temp
                }
            })
        },
        getListModule() {
            HTTP.get('Modules/getListModule').then((res) => {
                if (res.status == 200) {
                    this.modules = res.data

                    this.modules.forEach((module) => {
                        if (module.nameModule == this.moduleCurrent && module.isDeleted == 0) {
                            this.module.push(module)
                        }
                    })
                    if (this.module.length > 0) {
                        this.getListMenuByIdModule(this.module[0].id)
                    } else {
                        this.menus = []
                    }
                    this.isLoading = false
                }
            })
        },
 
        
        handlerCollapse() {
            this.isCollapse = !this.isCollapse
        },
    },
}
</script>
<style scoped>
* {
    scrollbar-width: thin;
    scrollbar-color: #c0bfbf #ffffff;
}

*::-webkit-scrollbar {
    width: 9px;
}

*::-webkit-scrollbar-track {
    background: #ffffff;
}

*::-webkit-scrollbar-thumb {
    background-color: #c0bfbf;
    border-radius: 10px;
    border: 3px solid #ffffff;
}
.collapse-width-map {
    width: 60px;
    transform: translateX(200px);
    left: -200px;
    transition: 0.5s ease;
}
.copllapse-home {
    margin-left: 64px;
    transition: 0.5s ease;
}
.padding-left-custom {
    padding-left: 10px;
}
.active-boder-left {
    border-left: 3px solid #3b82f6;
}
.padding-left-chill {
    padding-left: 24px;
}
</style>
<style>
.active-nav-item {
    color: #0d6efd !important;
    text-decoration: none;
    background: #e7e7e7;
    font-weight: 500;
}
.active-nav-item i {
    font-weight: 600;
}
a i {
    font-size: 21px;
}
.sidebar_after::after {
    content: '\f0d7';
    font-family: FontAwesome;
    font-style: normal;
    font-weight: normal;
    text-decoration: inherit;
    justify-content: end;
    width: 20%;
    display: flex;
    color: rgb(149 147 147);
    margin-right: 13px;
}
.afterCollapse::after {
    content: '';
}
.m-t--4 {
    margin-top: 0.5rem !important;
}
.home {
    margin-top: 57px;
    margin-left: 220px;
    transition: 0.5s ease;
}

/* Sidebar */
.sidebar {
    position: fixed;
    top: -15px;
    bottom: 0;
    left: 0;
    padding: 58px 0 0;
    border-radius: 2px;
    box-shadow: 0 0 3px 0 #a7a4a4, 0 2px 3px 0 #d1cfcf;
    z-index: 99;
    width: 220px;
    background-color: #fafafa;
    font-size: 14px;
}

/* @media (max-width: 991.98px) {
        .sidebar {
            width: 100%;
        }
    } */

.sidebar .active {
    border-radius: 5px;
    box-shadow: 0 2px 5px 0 rgb(0 0 0 / 16%), 0 2px 10px 0 rgb(0 0 0 / 12%);
}

.sidebar-sticky {
    position: relative;
    top: 0;
    height: calc(100vh - 48px);
    padding-top: 0.5rem;
    overflow-x: hidden;
    overflow-y: auto;
    /* Scrollable contents if viewport is shorter than content. */
}

.py-2 {
    text-decoration: none;
    color: black;
}

.py-2 i {
    margin-right: 5px;
    margin-left: 5px;
}

.dropdown-item {
    padding: 0.25rem 0.5rem !important;
}

.dropdownList li a span {
    margin-left: 15px;
}

.dropdownList li a i {
    margin-left: 15px;
}

.dropdownList li {
    margin-bottom: 7px;
    height: 25px;
    display: flex;
    align-items: center;
}
.sidebar-top-level-items {
    background: rgba(0, 0, 0, 0.04);
    font-weight: 500;
    color: #3b82f6;
}

.dropdown-item.active,
.dropdown-item:active {
    color: #0d6efd;
    text-decoration: none;
    background: rgba(0, 0, 0, 0.04);
}
.drop-botton-css {
    margin-bottom: 17px !important;
}
.sidebar {
    height: 96%;
    overflow-y: scroll;
    transition: 0.5s ease;
}
/* .sidebar:hover {
        width: 258px;
    } */
.label-disble {
    display: none;
    transition: 0.5s ease;
}
.sidebar-1 {
    position: fixed;
    bottom: 0;
    left: 0;
    border-radius: 2px;
    z-index: 99;
    width: 220px;
    height: 5%;
    margin-bottom: 2px;
    cursor: pointer;
    transition: 0.5s ease;
    box-shadow: 0 0 3px 0 #a7a4a4, 0 2px 3px 0 #d1cfcf;
    background-color: #fafafa;
}
.sidebar-1 h5 {
    margin-bottom: 0;
    font-size: 14px;
    font-weight: 400;
}
.sidebar-1 h5 span i {
    font-size: 21px;
}
.sidebar-1:hover {
    background-color: rgb(216, 216, 216);
}

@media (max-width: 573px) {
    .sidebar {
        width: 20%;
        display: none !important;
    }
    .home {
        margin-left: 0px;
    }
}
@media (max-width: 573px) {
    .sidebar-1 {
        display: none !important;
    }
}
</style>
