<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-4 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">QUẢN LÝ BLOG</li>
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
                                    label="Thêm bài viết"
                                    icon="pi pi-plus"
                                    @click="openAdd"
                                    v-if="this.showButton.add"
                                    class="p-button-sm custom-button-h me-2"
                                />
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2 ">
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite"
                                    icon="pi pi-filter-slash"
                                    class="custom-reload-h"
                                    @click="clearFilter()"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Dropdown
                                    class="custom-input-h"
                                    ref="staffDropdown"
                                    v-if="staffList !== null"
                                    v-model="fillterBlog.user"
                                    :options="staffListWithFullName"
                                    filter
                                    optionLabel="fullName"
                                    placeholder="Tên Nhân viên "
                                    :class="{ 'p-invalid': isStaffReviewSelected }"
                                    style="width: 100%;"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Dropdown
                                    class="custom-input-h"
                                    v-if="category !== null"
                                    v-model="fillterBlog.category"
                                    :options="category"
                                    optionLabel="name"
                                    placeholder="Chọn danh mục"
                                    :filter="true"
                                    filterPlaceholder="Tìm danh mục"
                                    style="width: 100%;"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    v-model="fillterBlog.title"
                                    class="custom-input-h"
                                    placeholder="Tìm theo tiêu đề"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    v-model="fillterBlog.postid"
                                    class="custom-input-h"
                                    placeholder="Tìm theo Id"
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        :value="bloglist"
                        :rows="5"
                        ref="dt"
                        :loading="loading"
                        :sortOrder="-1"
                        sortField="datecreate"
                        showGridlines="true"
                        responsiveLayout="scroll"
                        :globalFilterFields="[
                            '#',
                            'id',
                            'title',
                            'content',
                            'datecreate',
                            'dateupdate',
                            'usercreate',
                            'categoryid',
                            'imageid',
                        ]"
                    >
                        
                        <template #empty> Không tìm thấy. </template>
                        <Column sortable field="#" header="No.">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column sortable field="categoryid" header="ID bài viết">
                            <template #body="{ data }"> {{ data.id }} </template>
                        </Column>
                        <Column sortable field="categoryid" header="Danh mục">
                            <template #body="{ data }"> {{ data.categoryName }} </template>
                        </Column>
                        <Column sortable field="title" header="Tiêu đề">
                            <template #body="{ data }">
                                {{ data.title }}
                            </template>
                        </Column>
                        <Column sortable field="datecreate" header="Ngày tạo">
                            <template #body="{ data }">
                                {{ data.dateCreated ? formartDate(data.dateCreated) : '' }}
                            </template>
                        </Column>
                        <Column sortable field="usercreate" header="Người tạo">
                            <template #body="{ data }">
                                <span>{{ data.user ? data.user.fullName : '-' }}</span>
                            </template>
                        </Column>

                        <Column header="Thực thi" style="width: 13%">
                            <template #body="{ data }">
                                <div class="d-flex justify-content-start">
                                    <Button
                                        class="p-button-sm p-button-warning me-2 custom-button-h"
                                        @click="openEdit(data)"
                                        icon="pi pi-pencil"
                                        v-if="this.showButton.update"
                                    />
                                    <Button
                                        class="p-button-sm p-button-danger me-2 custom-button-h"
                                        @click="confirmDeleteBlog(data)"
                                        icon="pi pi-trash "
                                        v-if="this.showButton.delete"
                                    />
                                </div>
                            </template>
                        </Column>
                    </DataTable>
                </div>
            </div>
            <div class="row">
                <div class="col-12">
                    <Pagination
                        v-model:pageNumber="resultPgae.pageNumber"
                        v-model:pageSize="resultPgae.pageSize"
                        :totalPages="totalMapPage"
                        :totalItems="totalItem"
                        :itemIndex="itemIndex"
                    /> 
                </div>
            </div>

            <AddBlog
                :status="openStatus"
                @closemodal="closeAdd()"
                :optionmodule="OptionModule"
                :selectedBlog="{ ...BlogAdd }"
            />
            <EditBlog
                :status="openStatusEdit"
                @closemodal="closeEdit()"
                :optionmodule="OptionModule"
                :BlogId="BlogId"
            />
        </div>
    </LayoutDefaultDynamic>
</template>

<script>
    import dayjs from 'dayjs'
    import AddBlog from './DialogAddBlog.vue'
    import EditBlog from './DialogEditBlog.vue'
    import LayoutDefaultDynamic from '@/layouts/LayoutDefault/LayoutDefaultDynamic.vue'
    import { HTTP, DELETE_BLOG, GET_CATEGORY, GET_USER_BY_ID, GET_ALL_USERS_DATABASE } from '@/http-common'
    import { BlogService } from '@/service/blog.service'
import { checkAccessModule } from '@/helper/checkAccessModule'
import router from '@/router'
    export default {
        data() {
            return {
                isChange: false,
                openStatus: false,
                openStatusEdit: false,
                bloglist: [],
                categorylist: [],
                user: [],
                staffList: [],
                resultPgae: {
                    pageSize: 10,
                    pageNumber: 1,
                },
                totalItem: 0,
                totalMapPage: 0,
                itemIndex: 0,
                BlogId: null,
                idUser: null,
                idCategory: null,
                selectedStaff: null,
                fillterBlog: {
                    user: null,
                    category: null,
                    title: null,
                    postid: null,
                },
                loading: true,
                showButton:{},
            }
        },
        async created() {
            if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '')) === true) {
                checkAccessModule.checkPermissionAction(this.$route.path.replace('/', ''), this.showButton)
                    this.getBlog()
                    this.getCategory()
                    this.getAllUser()
            this.loading = false
            } else {
                alert("không có quyền")
                this.$router.push('/')
            }            
        },
        computed: {
            staffListWithFullName() {
                return this.staffList.map((staff) => {
                    const fullName = `${staff.lastName} ${staff.firstName}`
                    return {
                        ...staff,
                        fullName,
                    }
                })
            },
        },
        watch: {
            fillterBlog: {
                handler: function change(newVal) {
                    this.handlerFillterBlog()
                },
                deep: true,
            },
            resultPgae: {
                handler: function change() {
                    this.loading = true
                    if (
                        this.fillterBlog.postid != '' ||
                        this.fillterBlog.title != '' ||
                        this.fillterBlog.user.lastName != null ||
                        this.fillterBlog.category != null
                    ) {
                        this.handlerFillterBlog()
                    } else {
                        this.getBlog()
                    }
                },
                deep: true,
            },
        },
        methods: {
            clearFilter() {
                this.fillterBlog = {
                    user: null,
                    category: null,
                    title: null,
                    postid: null,
                }
            },
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('DD/MM/YYYY HH:mm:ss')
            },
            openAdd() {
                this.openStatus = true
            },
            closeAdd() {
                this.openStatus = false
                this.getBlog()
            },
            openEdit(BlogId) {
                this.BlogId = BlogId
                this.openStatusEdit = true
            },
            closeEdit() {
                this.openStatusEdit = false
                this.BlogDetail = []
                // this.getBlog()
            },
            async getBlog() {
                BlogService.getAllBlogPageList(this.resultPgae.pageNumber, this.resultPgae.pageSize).then((res) => {
                    this.loading = true
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    var data = res.data._Data.map((el) => ({
                        id: el.id,
                        title: el.title,
                        content: el.content,
                        categoryid: el.categoryId,
                        imagePostsNavigations: el.imagePostsNavigations,
                        categoryName: el.postCate.name,
                        isDeleted: el.isDeleted,
                        dateCreated: el.dateCreated,
                        usercreate: el.userCreated,
                        dateDeleted: el.dateDeleted,
                    }))
                    this.bloglist = data
                    this.idCategory = this.bloglist.categoryid
                    this.handlerLoadUser()
                })
                this.loading = false
            },
            async handlerLoadUser() {
                const users = []
                for (let i = 0; i < this.bloglist.length; i++) {
                    const data = await this.getUserById(this.bloglist[i].usercreate)
                    users.push(data)
                    this.loading = true
                }
                this.bloglist.forEach((blog, index) => {
                    blog.user = users[index]
                })
                this.loading = false
            },
            getUserById(id) {
                return HTTP.get(GET_USER_BY_ID(id)).then((res) => res.data)
            },

            async getAllUser() {
                await HTTP.get(GET_ALL_USERS_DATABASE).then((res) => {
                    this.staffList = res.data._Data
                })
            },
            async getCategory() {
                await HTTP.get(GET_CATEGORY).then((res) => {
                    this.category = res.data._Data
                })
                this.loading = false
            },
            async handlerFillterBlog() {
                if (
                    this.fillterBlog.postid != null ||
                    this.fillterBlog.title != null ||
                    (this.fillterBlog.user != null && this.fillterBlog.user.lastName != null) ||
                    (this.fillterBlog.category != null && this.fillterBlog.category.name != null)
                ) {
                    this.loading = true
                    this.bloglist = []
                    BlogService.handlerFilterBlog(
                        this.fillterBlog.postid || '',
                        this.fillterBlog.title || '',
                        this.fillterBlog.user ? this.fillterBlog.user.lastName : '',
                        this.fillterBlog.category && this.fillterBlog.category.name
                            ? this.fillterBlog.category.name
                            : '',
                        this.resultPgae.pageNumber,
                        this.resultPgae.pageSize,
                    ).then((res) => {
                        this.loading = true
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex

                        var data = res.data._Data.map((el) => ({
                            id: el.id,
                            title: el.title,
                            content: el.content,
                            categoryid: el.categoryId,
                            imagePostsNavigations: el.imagePostsNavigations,
                            categoryName: el.postCate.name,
                            isDeleted: el.isDeleted,
                            dateCreated: el.dateCreated,
                            usercreate: el.userCreated,
                            dateDeleted: el.dateDeleted,
                        }))
                        this.bloglist = data
                        this.handlerLoadUser()
                    })
                    this.loading = false
                } else {
                    this.bloglist = []
                    await this.getBlog()
                }
            },
            handlerDeleteBlog(data) {
                if (data) {
                    HTTP.delete(DELETE_BLOG(data.id))
                        .then((res) => {
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: res.data._Message,
                                life: 3000,
                            })
                            this.getBlog()
                        })
                        .catch((err) => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: err.message,
                                life: 3000,
                            })
                        })
                }
            },
            confirmDeleteBlog(data) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa bài viết này này?',
                    header: 'Xóa bài viết',
                    icon: 'pi pi-exclamation-triangle',
                    acceptLabel: 'Xóa',
                    rejectLabel: 'Hủy',
                    acceptIcon: 'pi pi-trash',
                    rejectIcon:'pi pi-times',
                    acceptClass:'p-button-danger CustomButtonPrimeVue',
                    rejectClass:'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                    accept: () => {
                        this.handlerDeleteBlog(data)
                    },
                    reject: () => {
                        return
                    },
                })
            },
        },
        components: { LayoutDefaultDynamic, AddBlog, EditBlog },
    }
</script>
<style scoped>
    .p-card-header {
        padding: 20px 20px 0px 20px !important;
    }

    .p-card .p-card-body {
        padding: 0px 1.25rem 1.25rem 1.25rem !important;
    }

    .d-grid {
        display: grid;
    }

    .p-breadcrumb {
        border: none !important;
    }

    .p-button.p-button-info.p-button-outlined {
        background-color: #3b82f6;
        color: white;
        border: 1px solid;
    }

    .p-button.p-button-secondary.p-button-outlined,
    .p-buttonset.p-button-secondary > .p-button.p-button-outlined,
    .p-splitbutton.p-button-secondary > .p-button.p-button-outlined {
        background-color: transparent;
        color: white;
        border: 1px solid;
    }
    .search-bar-right .p-dropdown {
        margin-right: 10px;
    }
    .input-text {
        margin-right: 10px;
    }
</style>
<style>
    .p-datatable-header {
        background-color: #607d8b !important;
    }
</style>
