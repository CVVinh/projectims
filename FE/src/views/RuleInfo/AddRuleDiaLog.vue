<template>
    <Toast position="top-right" />
    <Dialog
        :visible="statusopen"
        :closable="false"
        :modal="true"
        :maximizable="true"
        :dismissableMask="true"
        header="Thêm"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
    >
        <div class="container">
            <form @submit.prevent="handleSubmit()" enctype="multipart/form-data">
                <div class="row mb-5 mt-4">
                    <div class="col-md-12 col-sm-12">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.title.$error }">
                            <InputText
                                id=" title"
                                v-model="v$.title.$model"
                                :class="{ 'p-invalid': v$.title.$invalid && submitted }"
                                autocomplete="false"
                                class="custom-input-h"
                            />
                            <label for="title" :class="{ 'p-error': v$.title.$invalid && submitted }"
                                >Tiêu đề<b style="color: red">*</b></label
                            >
                        </div>
                        <span v-if="v$.title.$error && submitted">
                            <span id="title-error" v-for="(error, index) of v$.title.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                        <small
                            v-else-if="(v$.title.$invalid && submitted) || v$.title.$pending.$response"
                            class="p-error"
                            >{{ v$.title.required.$message.replace('Value', 'Title') }}
                        </small>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-6  mb-5">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.applyDay.$error }">
                            <Calendar
                                id="applyDay"
                                v-model="v$.applyDay.$model"
                                :showIcon="true"
                                :class="{ 'p-invalid': v$.applyDay.$invalid && submitted }"
                            />
                            <label for="applyDay" :class="{ 'p-error': v$.applyDay.$invalid && submitted }"
                                >Ngày áp dụng<b style="color: red">*</b></label
                            >
                        </div>
                        <small
                            v-if="(v$.applyDay.$invalid && submitted) || v$.applyDay.$pending.$response"
                            class="p-error"
                            >{{ v$.applyDay.required.$message.replace('Value', 'Apply Day') }}</small
                        >
                    </div>

                    <div class="col-sm-12 col-md-6 mb-5">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.expiredDay.$error }">
                            <Calendar
                                id="expiredDay"
                                v-model="v$.expiredDay.$model"
                                :showIcon="true"
                                :class="{ 'p-invalid': v$.expiredDay.$invalid && submitted }"
                            />
                            <label for="expiredDay" :class="{ 'p-error': v$.expiredDay.$invalid && submitted }"
                                >Ngày hết hạn<b style="color: red">*</b></label
                            >
                        </div>
                        <small
                            v-if="(v$.expiredDay.$invalid && submitted) || v$.expiredDay.$pending.$response"
                            class="p-error"
                            >{{ v$.expiredDay.required.$message.replace('Value', 'Expire Day') }}</small
                        >
                        <small v-if="!v$.expiredDay.$invalid && !checkEndDate() && submitted" class="p-error">
                            Ngày hết hạn phải lớn hơn ngày áp dụng!
                        </small>
                    </div>
                </div>

                <div class="row mb-5">
                    <div class="col-sm-12 col-md-12">
                        <div class="input_file">
                            <input
                                type="file"
                                @change="uploadFile($event)"
                                accept=".doc, xls, .xlsx, .pdf, .ppt, .pptx, .txt"
                            />
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-12">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.content.$error }">
                            <h6>Nội dung</h6>
                            <Editor
                                editorStyle="height: 400px"
                                id=" content"
                                v-model="v$.content.$model"
                                :class="{ 'p-invalid': v$.content.$invalid && submitted }"
                            >
                                <template v-slot:toolbar>
                                    <EditorCustom />
                                </template>
                            </Editor>
                        </div>

                        <small
                            v-if="(v$.content.$invalid && submitted) || v$.content.$pending.$response"
                            class="p-error"
                            >{{ v$.content.required.$message.replace('Value', 'Content') }}
                        </small>
                    </div>
                </div>

                <div class="row mb-4">
                    <div class="col col-md-12 col-24">
                        <div class="d-flex justify-content-end mt-3">
                            <div class="btn-right">
                                <button-custom class="me-2" @click="closeAdd()" />
                                <Button
                                    label="Lưu"
                                    icon="pi pi-check"
                                    type="submit"
                                    class="custom-button-h"
                                    autofocus
                                />
                            </div>
                        </div>                 
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

<script>
    import { required, alphaNum, numeric, between, minLength, maxLength } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { HTTP, HTTP_LOCAL } from '@/http-common'
    import jwt_decode from 'jwt-decode'
    import { HttpStatus } from '@/config/app.config'
    import { DateHelper } from '@/helper/date.helper'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import EditorCustom from '@/components/buttons/EditorCustom.vue'
import ButtonCustom from '@/components/buttons/ButtonCustom.vue'

    export default {
        name: 'RuleInfoAdd',
        props: ['statusopen'],

        setup: () => ({
            v$: useVuelidate(),
        }),

        data() {
            return {
                title: null,
                applyDay: new Date(),
                expiredDay: new Date(),
                idUser: null,
                formFile: null,
                content: null,
                submitted: false,
            }
        },

        validations() {
            return {
                title: {
                    required,
                },
                applyDay: {
                    required,
                },
                expiredDay: {
                    required,
                },
                content: {
                    required,
                },
            }
        },

        methods: {
            closeAdd() {
                this.$emit('closeAdd')
            },

            handleSubmit() {
                this.submitted = true
                if (this.v$.$invalid === false) {
                    this.submit()
                    this.closeAdd()
                }
            },

            resetForm() {
                this.title = null
                this.applyDay = new Date()
                this.expiredDay = new Date()
                this.idUser = null
                this.content = null
                this.content = null
            },

            checkEndDate() {
                return (this.valid = DateHelper.compareDate(this.applyDay, '<', this.expiredDay))
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

            async CallApi(fromData) {
                try {
                    const res = await HTTP.post('/Rules', fromData)
                        .then((res) => {
                            if (res.status == 200) {
                                this.resetForm()
                                this.$emit('reloadpage')
                                this.showSuccess('Thêm mới thành công!')
                            } else this.showResponseApi(res.status)
                        })
                        .catch((error) => {
                            var message = error.response.data != '' ? error.response.data : error.response.statusText
                            this.showResponseApi(error.response.status, message)
                        })
                } catch (error) {
                    switch (error.code) {
                        case 'ERR_NETWORK':
                            this.showInfo('Kiểm tra kêt nối!')
                            break
                        case 'ERR_BAD_REQUEST':
                            console.log(error.response.data)
                            break
                        default:
                    }
                }
            },

            async submit() {
                const fromData = new FormData()
                fromData.append('title', this.title)
                fromData.append('applyDay', DateHelper.formatDateTime(this.applyDay))
                fromData.append('expiredDay', DateHelper.formatDateTime(this.expiredDay))
                fromData.append('content', this.content)
                fromData.append('idUser', checkAccessModule.getUserIdCurrent())
                fromData.append('formFile', this.formFile)

                await this.CallApi(fromData)
            },
            uploadFile(event) {
                const file = event.target.files[0]
                this.formFile = file
            },
        },
        components: { EditorCustom, ButtonCustom},
    }
</script>
<style lang="scss" scope>
    .form-demo {
        min-width: 900px;
        width: 100%;
        height: 100%;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-items: center;
        margin-top: 2rem;
    }

    .input {
        width: 100%;
    }

    .form-addrule {
        width: 100%;
    }

    .p-fileupload {
        margin-left: 30px;
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
