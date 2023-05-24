<template>
    <Toast position="top-right" />

    <Dialog
        header="Cập nhật thông tin blog"
        :visible="status"
        :closable="false"
        :maximizable="true"
        modal
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :show="resetForm()"
    >
        <div class="container">
            <form enctype="multipart/form-data">
                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-3">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.title.$error }">
                            <h6>Tiêu đề <b style="color: red">*</b></h6>
                            <InputText id="title" v-model="v$.title.$model" :value="v$.title.$model" class="custom-input-h"/>
                        </div>
                        <label
                            v-if="(v$.title.$invalid && submitted) || v$.title.$pending.$response"
                            for="title"
                            :class="{ 'p-error': v$.title.$invalid && submitted }"
                        >
                            Vui lòng nhập tiêu đề
                        </label>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <h6>Danh mục <b style="color: red">*</b></h6>
                        <div
                            class="p-float-label"
                            :class="{ 'form-group--error': v$.selectedCategory.$error }"
                        ></div>
                        <Dropdown
                            id="selectedCategory"
                            v-if="category !== null"
                            v-model="selectedCategory"
                            :options="category"
                            optionLabel="name"
                            optionValue="id"
                            class="custom-input-h"
                        />
                        <label
                            v-if="
                                (v$.selectedCategory.$invalid && submitted) ||
                                v$.selectedCategory.$pending.$response
                            "
                            for="selectedCategory"
                            :class="{ 'p-error': v$.selectedCategory.$invalid && submitted }"
                        >
                            Vui lòng chọn danh mục</label
                        >
                    </div>
                </div>
                <div class="row">
                    <h6>Hình ảnh</h6>
                    <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                        <div class="input_file">
                            <input type="file" @change="uploadFile($event)" accept="image/*" />
                        </div>
                        <!-- <Message
                            v-if="pathFile"
                            @close="closeMessage"
                            :sticky="false"
                            :life="3000"
                            severity="info"
                            >{{ pathFile }}</Message
                        > -->
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.content.$error }">
                            <h6>Nội dung <b style="color: red">*</b></h6>
                            <Editor
                                editorStyle="height: 400px"
                                id=" content"
                                v-model="v$.content.$model"
                                :class="{ 'p-invalid': v$.content.$invalid && submitted }"
                            />
                        </div>
                        <label
                            v-if="(v$.content.$invalid && submitted) || v$.content.$pending.$response"
                            for="contentcontent"
                            :class="{ 'p-error': v$.content.$invalid && submitted }"
                        >
                            Vui lòng nhập nội dung</label
                        >
                    </div>
                </div>
                
                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12">
                        <div class="btn-right">
                            <button-custom @click="closeAdd()" class="me-2" />
                            <Button
                                label="Lưu"
                                icon="pi pi-check"
                                @click="editBlog(this.BlogId.id)"
                                class="custom-button-h"
                                style="margin-bottom: 1px"
                                autofocus
                            />
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

<script>
    import { required } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { HTTP, GET_CATEGORY, EDIT_BLOG } from '@/http-common'
    import { checkAccessModule } from '@/helper/checkAccessModule'
import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
    export default {
  components: { ButtonCustom },
        name: 'EditBlog',
        props: ['status', 'optionmodule', 'BlogId'],

        setup: () => ({
            v$: useVuelidate(),
        }),

        data() {
            return {
                idblog: null,
                submitted: false,
                title: null,
                userCreated: null,
                dateCreated: new Date(),
                pathFile: null,
                content: null,
                dateUpdated: new Date(),
                userUpdated: null,
                categoryid: null,
                selectedCategory: null,
                category: [],
            }
        },

        validations() {
            return {
                title: {
                    required,
                },
                content: {
                    required,
                },
                selectedCategory: {
                    required,
                },
            }
        },
        watch: {
            selectedCategory(newValue) {},
        },
        methods: {
            closeAdd() {
                this.$emit('closemodal')
            },
            resetForm() {
                this.idblog = this.BlogId ? this.BlogId.id : null
                this.title = this.BlogId ? this.BlogId.title : null
                this.selectedCategory = this.BlogId ? this.BlogId.categoryid : null
                this.dateCreated = this.BlogId ? this.BlogId.dateCreated : new Date()
                this.userCreated = this.BlogId ? this.BlogId.userCreated : checkAccessModule.getUserIdCurrent()
                this.content = this.BlogId ? this.BlogId.content : null
                this.userUpdated = this.BlogId ? this.BlogId.userUpdated : checkAccessModule.getUserIdCurrent()
                this.pathFile = this.BlogId ? this.BlogId.imagePostsNavigations[0]?.pathImage : null
            },

            async getCategory() {
                await HTTP.get(GET_CATEGORY).then((res) => {
                    this.data = res.data._Data

                    this.category = this.data
                })
                this.loading = false
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

            showResponseApi(status, message = '') {
                switch (status) {
                    case 401:
                    case 403:
                        this.showError('Bạn không có quyền thực hiện chức năng này!')
                        break

                    case 404:
                        this.showError('Lỗi! Load dữ liệu!')
                        break

                    default:
                        if (message != '') {
                            this.showError(message)
                        } else {
                            this.showError('Có lỗi trong quá trình thực hiện!')
                        }
                        break
                }
            },

            editBlog(id) {
                var fromData = new FormData()
                fromData.append('title', this.title)
                fromData.append('content', this.content)
                fromData.append('categoryId', this.selectedCategory)
                fromData.append('imagePostsNavigationss', this.pathFile)
                fromData.append('userUpdated', checkAccessModule.getUserIdCurrent())

                HTTP.put(EDIT_BLOG(id), fromData)
                    .then((response) => {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Thêm mới thành công!',
                            life: 3000,
                        })
                        this.closeAdd()
                    })
                    .catch((error) => {
                        this.$toast.add({
                            severity: 'warn',
                            summary: 'Cảnh báo ',
                            detail: error.response.data._Message,
                            life: 3000,
                        })
                    })
            },
            uploadFile(event) {
                const file = event.target.files[0]
                this.pathFile = file
            },
            // closeMessage() {
            //     this.pathFile = null
            // },
        },
        mounted() {
            this.getCategory()
        },
    }
</script>

<style lang="scss" scoped>
    .p-dropdown {
        width: 100%;
    } 
    .input_file {
        border: 1px solid #e5e5e5;
        border-radius: 3px;
    }

    input[type='file']::file-selector-button {
        background-color: #7128fa;
        color: #fff;
        border: 0px;
        border-right: 1px solid #e5e5e5;
        padding: 6px 15px;
        margin-right: 20px;
        border-top-left-radius: 3px;
        border-bottom-left-radius: 3px;
        cursor: pointer;
    }


    input[type='file']::file-selector-button:hover {
        background-color: #591bcc;
        border: 0px;
        border-right: 1px solid #591bcc;
    }
</style>
