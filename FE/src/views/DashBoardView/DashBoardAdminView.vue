<template>
  <div class="row">
        <div class="col-sm-12 col-md-4 col-lg-4 mb-3">
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-6 ">
                            <div class="d-flex justify-content-center">
                                <i class="bx bx-carousel " style="font-size: 2rem"></i>: {{ this.project.length > 0 ? this.project.length : 0 }}
                            </div>
                            <p>Dự án</p>
                        </div>
                        <div class="col-sm-12 col-md-12 col-lg-6 d-flex justify-content-center align-items-center">
                            <!-- <button-custom-edit  label="Tạo dự án" @click="openDialogAdd" /> -->
                            <Button
                                label="Tạo dự án"
                                class="custom-button-h style-btn"
                                @click="openDialogAdd"
                            />
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 ">
                            <router-link
                                :to="{ name: 'project', params: {} }"
                                style="text-decoration: none; color: #295bac; display: flex; align-items: center"
                                >Truy cập vào danh sách dự án
                                <i
                                    class="pi pi-angle-right"
                                    style="margin-top: 2px; margin-left: 2px; color: black"
                                ></i
                            ></router-link>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-sm-12 col-md-4 col-lg-4 mb-3">
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-6 ">
                            <div class="d-flex justify-content-center">
                                <i class="bx bx-user" style="font-size: 2rem"></i>: {{ this.user.length > 0 ? this.user.length : 0 }} 
                            </div>
                            <p>Người dùng</p>
                        </div>
                        <div class="col-sm-12 col-md-12 col-lg-6 d-flex justify-content-center align-items-center">
                            <!-- <button-custom-edit  label="Thêm người dùng" @click="OpenAdd" /> -->
                            <Button
                                label="Thêm người dùng"
                                class="custom-button-h style-btn"
                                @click="OpenAdd"
                            />
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 ">
                            <router-link
                                :to="{ name: 'users', params: {} }"
                                style="text-decoration: none; color: #295bac; display: flex; align-items: center"
                                >Truy cập vào danh sách người dùng
                                <i
                                    class="pi pi-angle-right"
                                    style="margin-top: 2px; margin-left: 2px; color: black"
                                ></i
                            ></router-link>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-sm-12 col-md-4 col-lg-4 mb-3">
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-6 ">
                            <div class="d-flex justify-content-center">
                                <i class="bx bxl-blogger" style="font-size: 2rem;font-weight: -10000;"></i>: {{ this.blog.length > 0 ? this.blog.length : 0 }}
                            </div>
                            <p>Bài viết</p>
                        </div>
                        <div class="col-sm-12 col-md-12 col-lg-6 d-flex justify-content-center align-items-center">
                            <!-- <button-custom-edit  label="Thêm bài viết" @click="openAddBlog" /> -->
                            <Button
                                type="button"
                                label="Thêm bài viết"
                                class="custom-button-h style-btn"
                                @click="openAddBlog"
                            />
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 ">
                            <router-link
                                :to="{ name: 'bloglist', params: {} }"
                                style="text-decoration: none; color: #295bac; display: flex; align-items: center"
                                >Truy cập vào danh sách bài viết
                                <i
                                    class="pi pi-angle-right"
                                    style="margin-top: 2px; margin-left: 2px; color: black"
                                ></i
                            ></router-link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <DialogAddEdit
        :isOpenDialog="isOpenDialog"
        :projectSelected="{ ...projectSelected }"
        @closeDialog="closeDialog"
        @getAllProject="getProject"
        :projectGit="dataProjects"
    />
    <AddUserDiaLog
        :statusopen="openAddform"
        @closeAdd="closeAdd()"
        :roleoption="Optionrole"
        @reloadpage="getUser()"
    />
    <DialogAddBlog
        :status="openStatus"
        @closemodal="closeAddBlog()"
    />  
</template>

<script>
import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
import DialogAddEdit from '../Project/DialogAddEdit.vue';
import AddUserDiaLog from '../Users/AddUserDiaLog.vue';
import DialogAddBlog from '../Blog/DialogAddBlog.vue';
import { GET_ALL_PROJECT, HTTP, HTTP_API_GITLAB } from '@/http-common';
import { ProjectDto } from '../Project/Project.dto';
import ButtonCustomEdit from '@/components/buttons/ButtonCustomEdit.vue';
export default {
  
    data(){
        return {
            project :[],
            user : [],
            blog : [],
            // users
            openAddform: false,
            Optionrole: [],
            //project
            isOpenDialog: false,
            projectSelected: new ProjectDto(),
            // blog
            dataProjects: [],
            openStatus: false,
        }
    },
    
    async created(){
        await this.getProject()
        await this.getUser()
        await this.getBlog()
        await this.handlerGetInfoProjects()
    },

    methods:{
        async getProject(){
            await HTTP.get("Project/getAllProject").then(res=>this.project = res.data._Data).catch(err=>console.log(err))
        },

        async getUser(){
            await  HTTP.get("Users/getAll").then(res=>this.user = res.data._Data).catch(err=>console.log(err))
        },

        async getBlog(){
            await HTTP.get("Posts/getAllPostAsync").then(res=>this.blog = res.data._Data).catch(err=>console.log(err))
        },

        // Users 
        OpenAdd() {
                this.openAddform = true
        },

        closeAdd() {
                this.openAddform = false
        },
        // project
        openDialogAdd() {
                this.isOpenDialog = true
                this.projectSelected = []
        },

        closeDialog() {
                this.isOpenDialog = false
                this.projectSelected = []
        },
        async handlerGetInfoProjects() {
                let resultCountPr = 100
                let resultPr = []
                let page = 1
                while (resultCountPr === 100) {
                    let newResultsPr = await this.getAllProects(page)
                    page++
                    this.dataProjects.push(...newResultsPr)
                    resultCountPr = newResultsPr.length
                    resultPr = resultPr.concat(newResultsPr)
                    newResultsPr.map((el) => {
                        return (el.name = `${el.name} (${el.name_with_namespace})`)
                    })
                }
        },
        getAllProects(page) {
                return HTTP_API_GITLAB.get(GET_ALL_PROJECT(100, page)).then((res) => res.data)
        },

        // blog
        openAddBlog() {
                this.openStatus = true
        },
        async closeAddBlog() {
            this.openStatus = false       
            this.getBlog()  
        },

    },

    components: { ButtonCustom, DialogAddEdit, AddUserDiaLog, DialogAddBlog, ButtonCustomEdit},

}
</script>

<style lang="scss" scoped>
    .style-btn {
        background: white;
        color: black;
        border: 1px solid rgb(176, 173, 173);
    }
    .style-btn:hover {
        background: white;
        color: black;
        border: 1px solid rgb(176, 173, 173);
    }
</style>