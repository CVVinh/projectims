<template>
    <div class="d-inline">
        <Edit @click="openModal" :disabled="checkEdit" class="custom-button-h"/>
        <Dialog
            header="Cập nhật thao tác"
            :visible="displayModal"
            :breakpoints="{ '1500px': '45vw', '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw', maxWidth: '500px' }"
            :modal="true"
            :maximizable="true"
            :closable="false"
            @hide="handleHide"
            @show="handleLoad"
        >
            <div class="container">
                <div class="row">
                    <div class="col-sm-12 col-md-12">
                        <div class="row">
                            <div class="col-sm-12 col-md-12">
                                <label for="moduleName">Tên thao tác <b style="color: red">*</b></label>
                            </div>
                        </div> 
                        <div class="row mb-3">
                            <div class="col-sm-12 col-md-12">
                                <InputText
                                    v-model="editAction.name"
                                    id="moduleName"
                                    type="text"
                                    style="width: 100%"
                                    :class="v$.editAction.name.$invalid && isSubmited ? 'p-invalid' : ' '"
                                    class="custom-input-h"
                                />
                                <small v-if="v$.editAction.name.$invalid && isSubmited" id="moduleName-help" class="p-error">
                                    Nhập tên thao tác.
                                </small>
                            </div>
                        </div> 
                        <div class="row">
                            <div class="col-sm-12 col-md-12">
                                <label for="note">Diễn đạt ý nghĩa thao tác <b style="color: red">*</b></label>
                            </div>
                        </div> 
                        <div class="row">
                            <div class="col-sm-12 col-md-12">
                                <Textarea
                                    id="note"
                                    v-model="editAction.description"
                                    :autoResize="true"
                                    rows="5"
                                    cols="30"
                                    style="width: 100%"
                                    :class="v$.editAction.description.$invalid && isSubmited ? 'p-invalid' : ' '"
                                    class="custom-input-h"
                                />
                                <small v-if="v$.editAction.description.$invalid && isSubmited" id="note-help" class="p-error">
                                    Trường này không được để trống
                                </small>
                            </div>
                        </div> 
                    </div>
                </div> 
            </div>
            <template #footer>
                <Button label="Đóng" icon="pi pi-times" @click="closeModal" class="p-button-secondary custom-button-h" style="background-color: white; color: black"/>
                <Button label="Lưu" icon="pi pi-check" @click="handleSave(v$.$invalid)" autofocus class="custom-button-h"/>
            </template>
        </Dialog>
    </div>
</template>

<script>
    import Edit from '../../components/buttons/Edit.vue'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { HTTP } from '@/http-common'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
    export default {
        components: { ButtonCustom },
        setup: () => ({
            v$: useVuelidate(),
        }),
        props: {
            checkEdit: Boolean,
            action: Object,
        },
        data() {
            return {
                isSubmited: false,
                editAction: {
                    nameAction: '',
                    description: ''
                },
                displayModal: false,
                waiting: false,
            }
        },
        validations() {
            return {
                editAction: {
                    name: { required },
                    description: { required },
                },
            }
        },
        mounted() {
            this.token= LocalStorage.jwtDecodeToken();
            this.handleLoad()
        },
        methods: {
            openModal() {
                this.displayModal = true
            },
            closeModal() {
                this.isSubmited = false
                this.displayModal = false
            },
            handleLoad() {
                this.editAction = {
                    name: this.$props.action.name,
                    userUpdated: this.token.Id,
                    description : this.$props.action.description
                }
            },
            handleUpdate() {
                HTTP.put(`ActionModule/updateActionModule/${this.action.id}`, this.editAction)
                    .then((res) => {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Sửa thành công!',
                            life: 3000,
                        })
                        this.closeModal()
                        this.$emit('render')
                    })
                    .catch((err) => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Lỗi ',
                            detail: 'Lỗi máy chủ',
                            life: 3000,
                        })
                    })
            },
            handleSave(isFormValid) {
                if (this.waiting) {
                    return
                } else {
                    this.waiting = true
                    setTimeout(() => {
                        this.waiting = false
                    }, 3000)
                }
                this.isSubmited = true
                if (isFormValid) {
                    return
                }
                this.handleUpdate()
            },
            handleHide() {
                this.closeModal()
            },
        },
        components: {
            Edit,
        },
    }
</script>

<style scoped>
    .field label {
        margin-bottom: 5px;
    }
</style>
