<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        
        <div class="container-fluid">
            <div class="row">
                <div class="col-sm-12 col-md-12 col-lg-9 col-xs-9 mt-3">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <nav class="navbar navbar-expand-lg"> <!-- bg-light navbar-light -->
                                <div class="container-fluid">
                                    <button class="navbar-toggler mb-2 mt-2 custom-input-h" type="button" data-bs-toggle="collapse" data-bs-target="#collapsibleNavbar">
                                        <span class="navbar-toggler-icon"></span>
                                    </button>
                                    <div class="collapse navbar-collapse mt-2" id="collapsibleNavbar">
                                        <ul class="navbar-nav m-auto">
                                            <li class="nav-item me-2 mb-2 ">
                                                <Button
                                                    icon="pi pi-filter-slash"
                                                    class="p-button-outlined p-button p-button-sm custom-button-h"
                                                    @click="clearFilter()"
                                                    v-tooltip.top="'Bỏ lọc'"
                                                />
                                            </li>
                                            <li class="nav-item me-2 mb-2">
                                                <InputText
                                                    type="text"
                                                    v-model="fillterBlog.title"
                                                    placeholder="Tìm kiếm theo tên bài viết..."
                                                    class="custom-input-h"
                                                />
                                            </li>
                                            <li class="nav-item me-2 mb-2">
                                                <InputText
                                                    v-model="fillterBlog.postid"
                                                    class="custom-input-h"
                                                    placeholder="Tìm theo Id"
                                                />
                                            </li>
                                            <li class="nav-item me-2 mb-2">
                                                <Dropdown
                                                    class="custom-input-h me-2"
                                                    filter
                                                    optionLabel="fullName"
                                                    placeholder="Tên Nhân viên "
                                                    :options="staffList"
                                                    v-model="fillterBlog.user"
                                                    style="width: 100%;"
                                                />
                                            </li>
                                        </ul>
                                    </div>
                                </div>
                            </nav>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <hr style="border-top: 1px solid black" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <div v-for="blog in bloglist" class="blog-list clearfix" :key="blog.id">
                                <div class="d-flex justify-content-end">                      
                                    <div class="icon-container" :hidden="!blog.isOwner && !checkAdmin()">
                                        <i :class="['pi pi-ellipsis-h']" style="color: slateblue" @click="visible = !visible; visibleItem = blog.id"></i>
                                        <div
                                            :class="['w-full md:w-20rem listbox']"
                                            v-show="click(visibleItem, blog.id)"
                                        >                                                        
                                            <template v-for="(item, index) in items" :key="index">
                                                <div 
                                                    class="flex" 
                                                    @click="selectedItem = item; visible = false; checkClick(item, blog)"
                                                >
                                                    <i :class="item.icon"></i>
                                                    <p class="p-ml-2">{{ item.name }}</p>
                                                </div>                                   
                                            </template>
                                        </div>      
                                    </div>                       
                                </div>                   
                                <!-- Content -->
                                <router-link :to="{ name: 'blogdetail', params: { id: blog.id } }">
                                    <div class="blog-box row">
                                        <div class="col-sm-12 col-md-2">
                                            <div class="post-media">
                                                <a href="" title="">
                                                    <img
                                                        v-if="blog.imagePostsNavigations && blog.imagePostsNavigations[0]"
                                                        :src="blog.imagePostsNavigations[0].pathImage"
                                                        alt=""
                                                        class="img-fluid"
                                                    />
                                                    <!-- <img v-else src="src/assets/vhec.jpg" alt="" class="img-fluid" /> -->
                                                    <div class="hovereffect"></div>
                                                </a>
                                            </div>
                                            <!-- end media -->
                                        </div>
                                        <!-- end col blog-meta  big-meta-->
                                        <div class="col-sm-12 col-md-10">
                                            <div>
                                                <h4>
                                                    <a style="text-decoration: none !important" href="" title="">
                                                        {{ blog.title }}</a
                                                    >
                                                </h4>                   
                                                <!-- nếu như nội dung dài hơn 50 kí tự thì thêm dấu ... vào phía cuối câu -->
                                                <div>{{ handleContent(blog.content) }}</div>

                                                <small
                                                    ><a class="blog-info me-1 text-danger" href="" title="">
                                                        {{ blog.fullName }}</a
                                                    ></small
                                                >
                                                <small
                                                    ><a v-if="blog.dateUpdated" class="blog-info me-1">| {{ blog.dateCreated }}</a>
                                                    <a v-else class="blog-info me-1"
                                                        >|{{ blog.dateCreated ? formartDate(blog.dateCreated) : '' }}</a
                                                    ></small
                                                ><small
                                                    ><a class="blog-info" href="" title="">| {{ blog.categoryName }}</a></small
                                                >
                                                <small
                                                    ><a
                                                        class="justify-content-end d-flex text-gray-dark fst-italic"
                                                        href=""
                                                        title=""
                                                    >
                                                        #{{ blog.id }}</a
                                                    ></small
                                                >
                                            </div>
                                        </div>
                                        <!-- end meta -->
                                    </div>
                                </router-link>
                                <!-- end blog-box -->
                            </div>
                            <!-- end blog-list -->
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <hr style="border-top: 1px solid black" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <Pagination
                                v-model:pageNumber="resultPgae.pageNumber"
                                v-model:pageSize="resultPgae.pageSize"
                                :totalPages="totalMapPage"
                                :totalItems="totalItem"
                                :itemIndex="itemIndex"
                            />
                        </div>
                    </div>
                </div>
                <!-- end col -->

                <!-- Sidebar-left -->
                <div class="col-sm-12 col-md-3 col-lg-3 col-xs-3 mt-3">
                    <div class="sidebar-left">
                        <Button
                            class="p-button-sm w-100 mt-1 me-2 p-button-info ms-5"
                            @click="openAdd"
                            icon="pi pi-plus"
                            label="Thêm bài viết"
                            v-if="this.showButton.add"
                        />

                        <!-- end widget -->
                        <div class="widget ms-5 mt-5">
                            <h2 class="widget-title" style="font-weight: 900">Danh mục nổi bật</h2>
                            <hr style="border-top: 1px solid black" />
                            <div v-for="category in categorylist" class="link-widget" :key="category.id">
                                <ul>
                                    <li>
                                        <a href="#">{{ category.name }} <span></span></a>
                                    </li>
                                </ul>
                            </div>
                            <!-- end link-widget -->
                        </div>
                    </div>
                </div>
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
    </LayoutDefaultDynamic>
</template>

<script>
    import dayjs from 'dayjs'
    import { ref } from "vue"
    import AddBlog from './DialogAddBlog.vue'
    import EditBlog from './DialogEditBlog.vue'
    import LayoutDefaultDynamic from '@/layouts/LayoutDefault/LayoutDefaultDynamic.vue'
    import { BlogService } from '@/service/blog.service'
    import { HTTP, GET_CATEGORY, GET_USER_BY_ID, GET_ALL_USERS_DATABASE } from '@/http-common'
    import jwt_decode from 'jwt-decode'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    
    export default {
        data() {
            return {
                isChange: false,
                openStatus: false,
                // loading: true,
                resultPgae: {
                    pageSize: 10,
                    pageNumber: 1,
                },
                totalItem: 0,
                totalMapPage: 0,
                itemIndex: 0,
                bloglist: [],
                categorylist: [],
                fillterBlog: {
                    user: null,
                    category: null,
                    title: null,
                    postid: null,
                },
                staffList: [],
                userId: null,
                visible: false,   
                selectedItem: null,
                items: ref([
                    { value: "edit", name: "Sửa", icon: "me-1 d-flex justify-content-center align-items-center fas fa-pen"},
                    { value: "delete", name: "Xóa", icon: "d-flex justify-content-center align-items-center fas fa-trash"},
                ]), 
                openStatusEdit: false,
                BlogId: null,
                visibleItem: null,
                showButton: {}
            }
        },
        async created() { 
            if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '')) === true) {
                checkAccessModule.checkPermissionAction(this.$route.path.replace('/', ''), this.showButton)
                var token = localStorage.getItem('token')
                var decode = jwt_decode(token)
                this.userId = parseInt(decode.id) 
                await this.getBlog()
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
            handleContent() {
                return (content) => {
                    try {
                        const parser = new DOMParser()
                        const doc = parser.parseFromString(content, 'text/html')
                        const paragraphs = doc.querySelectorAll('p')
                        if (paragraphs.length > 1) {
                            return this.truncateContent(paragraphs[0].textContent.trim())
                        }
                        return this.truncateContent(doc.querySelector('p').textContent.trim())
                    } catch (error) {
                        console.log('handle error')
                    }
                }
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
                    this.getBlog()
                },
                deep: true,
            },
        },
        props: ['blog'],      
        methods: {
            openAdd() {
                this.openStatus = true
            },
            closeAdd() {
                this.openStatus = false
                this.getBlog()
            },
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('DD-MM-YYYY')
            },
            async getBlog() {               
                this.loading = true
                BlogService.getAllBlogPageList(this.resultPgae.pageNumber, this.resultPgae.pageSize).then(async (res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    var data = res.data._Data.map((el) => ({
                        id: el.id,
                        title: el.title,
                        content: el.content,
                        categoryid: el.categoryId,
                        categoryName: el.postCate.name,
                        commentPostNavigations: el.commentPostNavigations,
                        imagePostsNavigations: el.imagePostsNavigations,
                        isDeleted: el.isDeleted,
                        dateCreated: this.formartDate(el.dateCreated),
                        usercreate: el.userCreated,
                        userUpdated: el.userUpdated,
                        dateUpdated: this.formartDate(el.dateUpdated),
                        isOwner: el.userCreated === this.userId,
                    }))
                    this.bloglist = data
                    this.handlerLoadUser()        
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
                } else {
                    this.bloglist = []
                    await this.getBlog()
                }
            },
            async handlerLoadUser() {
                for (let i = 0; i < this.bloglist.length; i++) {
                    const data = await this.getUserById(this.bloglist[i].usercreate)
                    this.bloglist[i].fullName = data.fullName
                    this.loading = true                  
                }
                this.loading = false
            },
            async getUserById(id) {
                return HTTP.get(GET_USER_BY_ID(id)).then((res) => res.data)
            },
            async getCategory() {
                await HTTP.get(GET_CATEGORY).then((res) => {
                    this.categorylist = res.data._Data
                    this.loading = true
                })
                this.loading = false
            },
            truncateContent(content) {
                if (content.length > 100) {
                    return content.substring(0, 250) + '...'
                }
                return content
            },
            async getAllUser() {
                await HTTP.get('Users/getInfo').then((res) => {
                    this.staffList = res.data
                    this.loading = true
                })
                this.loading = false
            },
            clearFilter(){
                this.fillterBlog.title = ''
                this.fillterBlog.postid = ''
                this.fillterBlog.user = ''
                this.getBlog()
            },
            deletePost(id){
                this.$confirm.require({
                    message: 'Bạn có muốn xóa bài đăng này?',
                    header: 'Xóa',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    acceptLabel: 'Xóa',
                    rejectLabel: 'Hủy',
                    accept: async () => {
                        await HTTP.delete('Posts/deletePost/' + id)
                            .then(async (res) => {    
                                if (res.status == 200) {
                                    this.$toast.add({
                                        severity: 'success',
                                        summary: 'Thành công',
                                        detail: 'Xóa bài đăng thành công!',
                                        life: 3000,
                                    })
                                    await this.getBlog()
                                } else {
                                    this.$toast.add({
                                        severity: 'error',
                                        summary: 'Thất bại',
                                        detail: 'Xóa bài đăng thất bại!',
                                        life: 3000,
                                    })
                                }                            
                            })
                            .catch((err) => {
                                console.log(err)
                            })
                    },
                    reject: () => {
                        return
                    },
                })
            },
            closeEdit() {
                this.openStatusEdit = false
                this.getBlog();
                this.BlogDetail = []
            },
            openEdit(BlogId) {
                this.BlogId = BlogId
                this.openStatusEdit = true
            },
            checkClick(item, blog){
                if(item.value === 'delete'){
                    this.deletePost(blog.id)
                }
                if(item.value === 'edit'){
                    this.openEdit(blog)
                }
            },
            click(id, blogId){
                if(id == blogId){                
                    return this.visible                   
                } else{
                    return false
                }                
            },
            checkAdmin() {
                if (checkAccessModule.getListGroup().includes('1')) {
                    return true
                } else {
                    return false
                }
            },
        },
        components: { LayoutDefaultDynamic, AddBlog, EditBlog },
    }
</script>
<style lang="scss" scoped>
    p[data-v-71993c76]{
        margin: 0.5rem!important;
    }
    .me-1{
        margin-right: 0!important;
    }
</style>
<style lang="scss" scoped>
    /* @import '@/styles/Css/style.css';
    @import '@/styles/Css/bootstrap.css';
    @import '@/styles/Css/bootstrap.min.css';
    @import '@/styles/Css/responsive.css';
 */
    .navbar {
        border-top: none;
        border-bottom: none;
        padding: 0px;
    }
    .blog-list .blog-meta.big-meta h4 {
        margin-left: -3%;
        margin-top: 1rem;
        background-color: #ffffff;
        padding: 1rem 1.5rem 1rem;
    }

    a {
        color: #111111;
        text-decoration: none !important;
        touch-action: manipulation;
    }

    a.blog-info {
        font-size: 11px;
        display: inline-block;
        margin-bottom: 0;
        padding-bottom: 0;
        color: #111111;
        font-weight: bold;
        text-transform: uppercase;
        margin-right: 0.5rem;
    }

    .form-control[data-v-71993c76]:focus {
        outline: 0 !important;
    }

    *:focus {
        outline: none;
    }

    .blog-list.clearfix:hover {
        background-color: #f2f2f2;
    }
    .icon-container{
        position: relative;
        display: inline-block;
    }
    .listbox {
        position: absolute;
        top: 20px;
        left: 0;
        z-index: 999;
        border: 1px solid rgb(218, 215, 215);
        border-radius: 5%;
        background-color: #ffffff;
    }
    .flex{
        display: flex;
        margin: 0.15rem 0.5rem;
        cursor: pointer;
    }
    .flex[data-v-71993c76]:hover{
        margin: 0!important;
        background-color: #f2f2f2;
        justify-content: center;
        font-size:20px;
    }
   
    @media (min-width: 576px) {
        .text-size{
            font-weight: bold;
            font-size: 20px;
        }
    }
    @media (min-width: 768px) {
        .text-size{
            font-weight: bold;
            font-size: 18px;
        }
    }
    @media (min-width: 1040px) {
        .text-size{
            font-weight: bold;
            font-size: 24px;
        }
    }
</style>
