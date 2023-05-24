<template>
    <Toast position="top-right" />

    <Dialog
        header="Thêm blog"
        :visible="status"
        :closable="false"
        :maximizable="true"
        modal
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
    >
        <div class="container">
            <form @submit.prevent="handleSubmit()" enctype="multipart/form-data">
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

                    <div class="col-sm-12 col-md-6 col-lg-6 mb-3">
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
                    <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                        <h6>Hình ảnh</h6>
                        <div class="input_file">
                            <input type="file" @change="uploadFile($event)" accept="image/*" />
                        </div>
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
                                type="submit"
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
import { HTTP, GET_CATEGORY, ADD_BLOG } from '@/http-common'
import { checkAccessModule } from '@/helper/checkAccessModule'
import ButtonCustom from '@/components/buttons/ButtonCustom.vue'

export default {
    components: { ButtonCustom },
    name: 'AddBlog',
    props: ['status', 'optionmodule'],
    setup: () => ({
        v$: useVuelidate(),
    }),

    data() {
        return {
            title: null,
            content: null,
            userCreated: null,
            dateCreated: new Date(),
            pathFile: null,
            submitted: false,
            categoryId: null,
            category: [],
            selectedCategory: null,
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
            this.resetForm()
            this.$emit('closemodal')
        },
        getBlog() {
            this.$emit('getBlog')
        },

        handleSubmit() {
            this.submitted = true
            if (this.v$.$invalid === false) {
                this.addBlog()
                this.closeAdd()
            }
        },
        resetForm() {
            this.title = null
            this.usercreated = null
            this.selectedCategory = null
            this.content = null
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

        addBlog() {
            var fromData = new FormData()
            fromData.append('title', this.title)
            fromData.append('content', this.content)
            fromData.append('categoryId', this.selectedCategory)
            fromData.append('imagePostsNavigationss', this.pathFile)
            fromData.append('userCreated', checkAccessModule.getUserIdCurrent())

            HTTP.post(ADD_BLOG, fromData)
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
        async getCategory() {
            await HTTP.get(GET_CATEGORY).then((res) => {
                this.data = res.data._Data
                this.category = this.data
            })
            this.loading = false
        },
    },
    mounted() {
        this.getCategory()
    },
}
</script>
<style lang="scss" scoped>
pre {
    background-color: rgb(41, 41, 41);
    color: White;
    padding: 1%;
    font-size: 1.1em;
}
</style>
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
