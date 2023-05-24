<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <section class="section wb">
            <div class="header">
                <i class="pi pi-arrow-left back-icon" @click="backBlog()"></i>
            </div>
            <div class="row">
                <div class="col-lg-9 col-md-12 col-sm-12 col-xs-12">
                    <div v-for="blog in bloglist" class="page-wrapper" :key="blog.id">
                        <div class="d-flex justify-content-end">
                            <div class="icon-container" :hidden="!blog.isOwner && !checkAdmin()">
                                <i class="pi pi-ellipsis-h" style="color: slateblue" @click="visible = !visible"></i>
                                <div
                                    :class="['w-full md:w-20rem listbox']"
                                    v-show="blog.id ? visible : checkAdmin() ? visible : !visible"
                                >
                                    <template v-for="(item, index) in items" :key="index">
                                        <div
                                            class="flex"
                                            @click="
                                                () => {
                                                    selectedItem = item
                                                    visible = false
                                                    checkClick(item, blog)
                                                }
                                            "
                                        >
                                            <i :class="item.icon"></i>
                                            <p class="p-ml-2">{{ item.name }}</p>
                                        </div>
                                    </template>
                                </div>
                            </div>
                        </div>
                        <div style="border: 1px solid rgb(238, 238, 238)">
                            <div>
                                <div class="single-post-media d-flex justify-content-center">
                                    <img
                                        v-if="blog.imagePostsNavigations && blog.imagePostsNavigations[0]"
                                        :src="blog.imagePostsNavigations[0].pathImage"
                                        alt=""
                                        class="img-fluid"
                                        style="object-fit: cover; width: 100%; height: 30rem"
                                    />
                                </div>
                                <div class="blog-title-area" style="padding: 70px">
                                    <!-- <ol class="breadcrumb hidden-xs-down">
                                        <li class="breadcrumb-item"><a href="#">Home</a></li>
                                        <li class="breadcrumb-item"><a href="#">Blog</a></li>
                                        <li class="breadcrumb-item active">
                                            The golden rules you need to know for a positive life
                                        </li>
                                    </ol> -->

                                    <h4 style="font-weight: 700">{{ blog.title }}</h4>

                                    <div class="blog-meta big-meta">
                                        <small
                                            ><a href="#" title=""> {{ blog.user ? blog.user.fullName : '-' }}</a></small
                                        >
                                        <small
                                            ><a href="blog-author.html" title="">
                                                {{ blog.dateCreated ? formartDate(blog.dateCreated) : '' }}</a
                                            ></small
                                        >
                                    </div>
                                    <!-- end meta -->

                                    <!-- end title -->

                                    <!-- end media -->

                                    <div
                                        class="blog-content"
                                        style="border-top: 1px solid #eee; border-bottom: 1px solid #eee"
                                    >
                                        <div>
                                            <!-- <div v-html="blog.content.substring(0, 100) + '...'"></div> -->
                                            <p v-html="blog.content" style="color: #000000c9" class="mt-3 mb-3"></p>
                                        </div>
                                        <!-- end pp -->
                                    </div>
                                    <!-- <div>
                                        <span>
                                            <i class="fa fa-tags me-1"></i>
                                            <small>{{ blog.categoryName }}</small>
                                        </span>
                                    </div> -->
                                </div>
                            </div>
                            <!-- end content -->
                        </div>

                        <hr class="invis1" />

                        <!-- end custom-box -->

                        <hr class="invis1" />
                        <!-- Comment box -->
                        <div class="custombox clearfix">
                            <h4 class="small-title">{{ this.commentList.length }} Bình luận</h4>
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="comments-list" v-for="(comment, index) in commentList" :key="index">
                                        <div class="media">
                                            <a class="media-left" href="#">
                                                <img src=" " alt="" class="rounded-circle" />
                                            </a>
                                            <div class="media-body">
                                                <div class="border-bottom" style="padding: 0px 0px 10px">
                                                    <h4 class="media-heading user_name">
                                                        {{ comment.userCreatedNavigation.fullName }}
                                                    </h4>

                                                    <p v-show="!comment.editCommentStatus">
                                                        {{ comment.content }}
                                                    </p>
                                                    <div>
                                                        <textarea
                                                            class="form-control"
                                                            cols="100"
                                                            rows="3"
                                                            v-model="comment.content"
                                                            v-show="comment.editCommentStatus"
                                                        ></textarea>
                                                        <div class="mt-2 d-flex justify-content-end">
                                                            <button
                                                                class="btn btn-primary"
                                                                @click="submitEditComment(comment)"
                                                                v-show="comment.editCommentStatus"
                                                            >
                                                                Lưu
                                                            </button>
                                                        </div>
                                                    </div>
                                                    <small
                                                        >| {{ this.calculateCommentTime(comment.dateCreated) }} |</small
                                                    >
                                                    <span class="ms-1">
                                                        <small>
                                                            Lượt phản hồi
                                                            <i class="pi pi-send">
                                                                <span class="ms-1">{{
                                                                    comment.postCommentsNavigations.length
                                                                }}</span>
                                                            </i>
                                                        </small>
                                                        <button
                                                            v-if="comment.postCommentsNavigations.length > 0"
                                                            class="btn btn-outline-secondary ms-1"
                                                            style="
                                                                padding: 5px 15px !important;
                                                                font-size: 11px !important;
                                                                color: #937e7e;
                                                            "
                                                            type="button"
                                                            data-bs-toggle="collapse"
                                                            :data-bs-target="'#collapseShowReply' + comment.id"
                                                            aria-expanded="false"
                                                            :aria-controls="'collapseShowReply' + comment.id"
                                                        >
                                                            Xem thêm
                                                        </button>
                                                    </span>
                                                </div>
                                                <!-- Reply comment -->
                                                <div :id="'collapseShowReply' + comment.id" class="ms-3 collapse">
                                                    <div
                                                        class="mb-2 pt-2 border-bottom"
                                                        style="padding: 0px 0px 10px"
                                                        v-for="reply in comment.postCommentsNavigations"
                                                        :key="reply.id"
                                                    >
                                                        <div class="row">
                                                            <div class="col-md-10">
                                                                <h4 class="media-heading user_name">
                                                                    {{ reply.userComment?.fullName ?? 'Ẩn danh' }}
                                                                </h4>
                                                            </div>
                                                            <!-- Update Replay -->
                                                            <div class="col-md-2">
                                                                <div class="d-flex justify-content-end">
                                                                    <div
                                                                        :hidden="reply.userCreated !== this.userId"
                                                                        class="custom-button-edit"
                                                                    >
                                                                        <i
                                                                            class="me-1 d-flex justify-content-center align-items-center fas fa-pen"
                                                                            style="cursor: pointer"
                                                                            @click="handlerUpdateComment(reply.id)"
                                                                            :hidden="reply.userCreated !== this.userId"
                                                                        >
                                                                        </i>
                                                                    </div>
                                                                    <div
                                                                        class="custom-button-delete"
                                                                        :hidden="
                                                                            reply.userCreated !== this.userId ||
                                                                            editCommentStatusReply === true
                                                                        "
                                                                    >
                                                                        <i
                                                                            class="d-flex justify-content-center align-items-center fas fa-trash"
                                                                            style="cursor: pointer"
                                                                            @click="deleteComment(reply.id)"
                                                                            :hidden="
                                                                                reply.userCreated !== this.userId ||
                                                                                editCommentStatusReply === true
                                                                            "
                                                                        >
                                                                        </i>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <!-- End Update Replay -->
                                                        </div>
                                                        <div class="col-md-12">
                                                            <p v-show="!reply.editCommentStatusReply">
                                                                {{ reply.content }}
                                                            </p>
                                                            <div class="mt-2">
                                                                <textarea
                                                                    class="form-control"
                                                                    cols="100"
                                                                    rows="3"
                                                                    v-model="reply.content"
                                                                    v-show="reply.editCommentStatusReply"
                                                                ></textarea>
                                                                <div class="mt-2 d-flex justify-content-end">
                                                                    <button
                                                                        class="btn btn-primary"
                                                                        @click="submitEditComment(reply)"
                                                                        v-show="reply.editCommentStatusReply"
                                                                    >
                                                                        Lưu
                                                                    </button>
                                                                </div>
                                                            </div>
                                                            <small>
                                                                | {{ this.calculateCommentTime(reply.dateCreated) }} |
                                                            </small>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="mt-3">
                                                    <button
                                                        class="btn btn-primary"
                                                        type="button"
                                                        data-bs-toggle="collapse"
                                                        :data-bs-target="'#collapseExample' + comment.id"
                                                        aria-expanded="false"
                                                        :aria-controls="'collapseExample' + comment.id"
                                                    >
                                                        Phản hồi
                                                    </button>
                                                </div>
                                                <div class="collapse" :id="'collapseExample' + comment.id">
                                                    <div class="col-lg-12 mt-3" style="padding: 0px">
                                                        <form
                                                            class="form-wrapper"
                                                            @submit.prevent="handlerReplyComment(comment)"
                                                        >
                                                            <textarea
                                                                class="form-control"
                                                                v-model="comment.commentContent"
                                                            >
                                                            </textarea>
                                                            <button
                                                                type="submit"
                                                                class="btn btn-primary"
                                                                :disabled="
                                                                    comment.commentContent == undefined ||
                                                                    comment.commentContent == ''
                                                                "
                                                            >
                                                                Gửi
                                                            </button>
                                                        </form>
                                                    </div>
                                                </div>
                                                <!-- End Reply comment -->
                                            </div>
                                            <div class="d-flex justify-content-end">
                                                <div
                                                    :hidden="comment.userCreated !== this.userId"
                                                    class="custom-button-edit"
                                                >
                                                    <i
                                                        class="me-1 d-flex justify-content-center align-items-center fas fa-pen"
                                                        style="cursor: pointer"
                                                        @click="editComment(comment.id)"
                                                        :hidden="comment.userCreated !== this.userId"
                                                    >
                                                    </i>
                                                </div>
                                                <div
                                                    class="custom-button-delete"
                                                    :hidden="
                                                        comment.userCreated !== this.userId ||
                                                        editCommentStatus === true
                                                    "
                                                >
                                                    <i
                                                        class="d-flex justify-content-center align-items-center fas fa-trash"
                                                        style="cursor: pointer"
                                                        @click="deleteComment(comment.id)"
                                                        :hidden="
                                                            comment.userCreated !== this.userId ||
                                                            editCommentStatus === true
                                                        "
                                                    >
                                                    </i>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!-- end col -->
                            </div>
                            <!-- end row -->
                        </div>

                        <hr class="invis1" />

                        <div class="custombox clearfix">
                            <h4 class="small-title">Bình luận</h4>
                            <div class="row">
                                <div class="col-lg-12">
                                    <form class="form-wrapper" @submit.prevent="postCommentCreate()">
                                        <!-- <input type="text" class="form-control" placeholder="Email address" /> -->
                                        <!-- <input type="text" class="form-control" placeholder="Website" /> -->
                                        <label for="comment">Nội dung bình luận:</label>
                                        <textarea
                                            class="form-control"
                                            id="comment"
                                            v-model="commentContent"
                                            @input="onTextareaInput"
                                        >
                                        </textarea>
                                        <button type="submit" class="btn btn-primary" :disabled="isButtonDisabled">
                                            Bình luận
                                        </button>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- end page-wrapper -->
                </div>
                <!-- end col -->

                <!-- Sidebar-left -->
                <div class="col-lg-3 col-md-12 col-sm-12 col-xs-12">
                    <div class="sidebar-left">
                        <div class="widget ms-5" style="margin-top: 50px">
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
                        <div class="widget ms-5" style="margin-top: 50px">
                            <h2 class="widget-title" style="font-weight: 900">Bài viết mới</h2>
                            <hr style="border-top: 1px solid black" />
                            <!-- <div class="link-widget">
                                <ul>
                                    <a href="#"><li>bla bla</li></a>
                                    <small class="ms-1 fst-italic">1 ngày trước</small>
                                    <a href="#"><li>bla bla</li></a>
                                    <small class="ms-1 fst-italic">2 ngày trước</small>
                                    <a href="#"><li>bla bla</li></a>
                                    <small class="ms-1 fst-italic">3 ngày trước</small>
                                </ul>
                            </div> -->
                            <!-- end link-widget -->
                        </div>
                        <!-- end widget -->
                    </div>
                    <!-- end sidebar -->
                </div>
                <!-- end col -->
            </div>
            <!-- end row -->
            <!-- end container -->
            <EditBlog
                :status="openStatusEdit"
                @closemodal="closeEdit()"
                :optionmodule="OptionModule"
                :BlogId="BlogId"
            />
        </section>
        <!-- end row -->
    </LayoutDefaultDynamic>
</template>

<script>
    import dayjs from 'dayjs'
    import AddBlog from './DialogAddBlog.vue'
    import EditBlog from './DialogEditBlog.vue'
    import LayoutDefaultDynamic from '@/layouts/LayoutDefault/LayoutDefaultDynamic.vue'
    import jwt_decode from 'jwt-decode'
    import { HTTP, GET_CATEGORY, GET_USER_BY_ID } from '@/http-common'
    import { BlogService } from '@/service/blog.service'
    import { ref } from 'vue'
    import { useRouter } from 'vue-router'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import router from '@/router'

    export default {
        data() {
            return {
                isChange: false,
                openStatus: false,
                openStatusEdit: false,
                userName: null,
                userId: null,
                commentContent: '',
                blogId: this.$route.params.id,
                commentList: [],
                editCommentStatus: false,
                editCommentStatusReply: false,
                bloglist: [],
                resultPgae: {
                    pageSize: 10,
                    pageNumber: 1,
                },
                totalItem: 0,
                totalMapPage: 0,
                categorylist: [],
                isButtonDisabled: true,
                visible: false,
                selectedItem: null,
                items: ref([
                    {
                        value: 'edit',
                        name: 'Sửa',
                        icon: 'me-1 d-flex justify-content-center align-items-center fas fa-pen',
                    },
                    {
                        value: 'delete',
                        name: 'Xóa',
                        icon: 'd-flex justify-content-center align-items-center fas fa-trash',
                    },
                ]),
                BlogId: null,
                replayComment: {
                    commentContent: '',
                    isButtonDisabled: true,
                },
            }
        },
        created() {
            var token = localStorage.getItem('token')
            var decode = jwt_decode(token)
            this.userName = decode.fullName
            this.userId = parseInt(decode.id)

            const blogId = this.$route.params.id
            const router = useRouter()
            this.getBlogById(blogId)
            this.getAllComment()
            this.getCategory()
        },
        // computed: {
        //     HiddenEditComment() {
        //         console.log(this.comment.userCreated === this.userId)
        //         return this.comment.userCreated === this.userId
        //     },
        // },
        methods: {
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('DD-MM-YYYY HH:mm:ss')
            },
            openAdd() {
                this.openStatus = true
            },
            closeAdd() {
                this.openStatus = false
            },
            openEdit(BlogId) {
                this.BlogId = BlogId
                this.openStatusEdit = true
            },
            closeEdit() {
                this.getBlogById(this.$route.params.id)
                this.openStatusEdit = false
                this.BlogDetail = []
            },
            async getBlogById(id) {
                BlogService.getBlogByID(id).then((res) => {
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
                        isOwner: el.userCreated === this.userId,
                    }))
                    this.bloglist = data

                    this.handlerLoadUser()
                })
                this.loading = false
            },
            async handlerLoadUser() {
                const users = []
                for (let i = 0; i < this.bloglist.length; i++) {
                    const data = await this.getUserById(this.bloglist[i].usercreate)
                    users.push(data)
                }
                this.bloglist.forEach((blog, index) => {
                    blog.user = users[index]
                })
            },
            getUserById(id) {
                return HTTP.get(GET_USER_BY_ID(id)).then((res) => res.data)
            },
            async getCategory() {
                await HTTP.get(GET_CATEGORY).then((res) => {
                    this.categorylist = res.data._Data
                })
                this.loading = false
            },
            postCommentCreate() {
                var data = {
                    content: this.commentContent,
                    postId: this.blogId,
                    userCreated: this.userId,
                    idUserComment: checkAccessModule.getUserIdCurrent(),
                }
                HTTP.post('PostComment/createNewPostComment', data)
                    .then((res) => {
                        this.clearComment()
                        this.getAllComment()
                        this.isButtonDisabled = true
                    })
                    .catch((err) => {
                        console.log(err)
                    })
            },
            handlerReplyComment(idComment) {
                console.log(idComment)
                var data = {
                    content: idComment.commentContent,
                    postId: this.blogId,
                    userCreated: checkAccessModule.getUserIdCurrent(),
                    parentId: idComment.id,
                    idUserComment: checkAccessModule.getUserIdCurrent(),
                }
                BlogService.handlerReplyComment(data)
                    .then((res) => {
                        this.clearComment()
                        this.getAllComment()
                        this.replayComment.isButtonDisabled = true
                    })
                    .catch((err) => console.log(err))
            },
            clearComment() {
                this.commentContent = ''
                this.replayComment.commentContent = ''
            },
            getAllComment() {
                HTTP.get('PostComment/GetAllCommentByIdPost/' + this.$route.params.id)
                    .then((res) => {
                        console.log(res.data)
                        this.commentList = res.data._Data
                    })
                    .catch((err) => {
                        console.log(err)
                    })
            },
            calculateCommentTime(date) {
                const diff = Date.now() - new Date(date)
                const seconds = Math.floor(diff / 1000)
                const minutes = Math.floor(seconds / 60)
                const hours = Math.floor(minutes / 60)
                const days = Math.floor(hours / 24)
                if (days > 0) {
                    return `${days} Ngày${days > 1 ? '' : ''} trước`
                } else if (hours > 0) {
                    return `${hours} Giờ${hours > 1 ? '' : ''} trước`
                } else if (minutes > 0) {
                    return `${minutes} Phút${minutes > 1 ? '' : ''} trước`
                } else {
                    return `${seconds} Giây${seconds > 1 ? '' : ''} trước`
                }
            },
            deleteComment(id) {
                this.$confirm.require({
                    message: 'Bạn có muốn xóa bình luận này?',
                    header: 'Xóa',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    acceptLabel: 'Xóa',
                    rejectLabel: 'Hủy',
                    accept: () => {
                        HTTP.delete('PostComment/deletePostComment/' + id)
                            .then((res) => {
                                this.getAllComment()
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
            editComment(id) {
                const commentToEdit = this.commentList.find((comment) => comment.id === id)
                if (commentToEdit) {
                    commentToEdit.editCommentStatus = !commentToEdit.editCommentStatus
                }
            },
            handlerUpdateComment(id) {
                this.commentList
                    .filter((el) => {
                        return el.postCommentsNavigations.length > 0
                    })
                    .forEach((el) => {
                        const commentToEditReplay = el.postCommentsNavigations.find((comment) => comment.id === id)
                        console.log(commentToEditReplay)
                        if (commentToEditReplay) {
                            commentToEditReplay.editCommentStatusReply = !commentToEditReplay.editCommentStatusReply
                        }
                    })
            },
            submitEditComment(comments) {
                var data = {
                    content: comments.content,
                    postId: comments.postId,
                    userUpdated: this.userId,
                }
                HTTP.put('PostComment/updatePostComment/' + comments.id, data)
                    .then((res) => {
                        this.editComment(comments.id)
                        this.handlerUpdateComment(comments.id)
                    })
                    .catch((err) => {
                        console.log(err)
                    })
            },
            onTextareaInput() {
                if (this.commentContent.length === 0) {
                    this.isButtonDisabled = true
                } else {
                    this.isButtonDisabled = false
                }
            },
            backBlog() {
                router.go(-1)
            },
            checkAdmin() {
                if (checkAccessModule.getListGroup().includes('1')) {
                    return true
                } else {
                    return false
                }
            },
            checkClick(item, blog) {
                if (item.value === 'delete') {
                    this.deletePost(blog.id)
                }
                if (item.value === 'edit') {
                    this.openEdit(blog)
                }
            },
            deletePost(id) {
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
                                    await this.backBlog()
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
        },
        components: { LayoutDefaultDynamic, AddBlog, EditBlog },
    }
</script>
<style lang="scss">
    p[data-v-33b128f6] {
        margin: 0.5rem !important;
    }
    .me-1 {
        margin-right: 0 !important;
    }
</style>
<style lang="css" scoped>
    @import '@/styles/Css/style.css';
    @import '@/styles/Css/bootstrap.css';
    @import '@/styles/Css/bootstrap.min.css';
    @import '@/styles/Css/responsive.css';

    .color-green a {
        background-color: #34bf49 !important;
    }

    .row {
        margin-right: 0px;
        margin-left: 0px;
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

    .sidebar-left {
        width: 300px;
    }

    .form-control[data-v-71993c76]:focus {
        outline: 0 !important;
    }

    *:focus {
        outline: none;
    }

    section.section.wb {
        padding: 0;
        border: none;
    }

    .blog-title-area[data-v-33b128f6] {
        display: block;
        position: relative;
        padding: 0;
        padding-left: 20px;
        margin-bottom: 3rem;
    }

    .button-custom-i {
        padding: 5px;
        border: 1px solid;
        border-radius: 6px;
    }
    .hidden {
        display: none;
    }
    .header {
        padding-left: 12px;
        padding-bottom: 0px;
    }
    .back-icon {
        padding: 0.2rem;
    }
    .icon-container {
        position: relative;
        display: inline-block;
    }
    .listbox {
        position: absolute;
        top: 20px;
        left: 0px;
        z-index: 999;
        border: 1px solid rgb(218, 215, 215);
        border-radius: 5%;
        background-color: #ffffff;
    }
    .flex {
        display: flex;
        margin: 0.15rem 0.5rem;
        cursor: pointer;
    }
    .flex[data-v-33b128f6]:hover {
        margin: 0 !important;
        background-color: #f2f2f2;
        justify-content: center;
        font-size: 20px;
    }
    .custom-button-edit {
        width: 25px;
        position: relative;
        height: 25px;
        background: #f98440;
        border-radius: 5px;
        margin-right: 5px;
    }
    .custom-button-edit i {
        position: absolute;
        cursor: pointer;
        width: inherit;
        height: inherit;
        color: white;
    }
    .custom-button-delete {
        width: 25px;
        position: relative;
        height: 25px;
        background: #e32828;
        border-radius: 5px;
        margin-right: 5px;
    }
    .custom-button-delete i {
        position: absolute;
        cursor: pointer;
        width: inherit;
        height: inherit;
        color: white;
    }
</style>
