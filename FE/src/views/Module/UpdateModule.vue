<template>
    <div class="d-inline">
        <Edit @click="openModal" :disabled="checkEdit" class="custom-button-h"/>
        <Dialog
            header="Cập nhật chức năng"
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
                                <label for="moduleName">Tên chức năng <b style="color: red">*</b></label>
                            </div>
                        </div>
                        <div class="row mb-3">
                            <div class="col-sm-12 col-md-12">
                                <InputText
                                    v-model="editModule.nameModule"
                                    id="moduleName"
                                    type="text"
                                    :class="v$.editModule.nameModule.$invalid && isSubmited ? 'p-invalid' : ' '"
                                    class="custom-input-h"
                                />
                                <small v-if="v$.editModule.nameModule.$invalid && isSubmited" id="moduleName-help" class="p-error">
                                    Trường này không được để trống
                                </small>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-12 col-md-12">
                                <label for="note">Diễn đạt ý nghĩa chức năng <b style="color: red">*</b></label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-12 col-md-12">
                                <Textarea
                                    id="note"
                                    v-model="editModule.note"
                                    :autoResize="true"
                                    rows="5"
                                    cols="30"
                                    style="width: 100%"
                                    :class="v$.editModule.note.$invalid && isSubmited ? 'p-invalid' : ' '"
                                    class="custom-input-h"
                                />
                                <small v-if="v$.editModule.note.$invalid && isSubmited" id="note-help" class="p-error">
                                    Trường này không được để trống
                                </small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <template #footer>
                <!-- <ButtonCustom  @click="closeModal()" />   -->
                <Button label="Đóng" icon="pi pi-times" @click="closeModal" class="p-button-secondary custom-button-h" style="background-color: white; color: black"/>
                <Button label="Lưu" icon="pi pi-check" @click="handleSave(v$.$invalid)" autofocus class="custom-button-h ms-2"/>
            </template>
        </Dialog>
    </div>
</template>

<script>
    import Edit from '../../components/buttons/Edit.vue'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { HTTP } from '@/http-common'
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
    export default {
        components: { ButtonCustom },
        setup: () => ({
            v$: useVuelidate(),
        }),
        props: {
            checkEdit: Boolean,
            module: Object,
        },
        data() {
            return {
                isSubmited: false,
                editModule: {
                    nameModule: '',
                    note: '',
                },
                displayModal: false,
                waiting: false,
            }
        },
        validations() {
            return {
                editModule: {
                    nameModule: { required },
                    note: { required },
                },
            }
        },
        mounted() {
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
                this.editModule = {
                    nameModule: this.$props.module.nameModule,
                    note: this.$props.module.note,
                }
            },
            handleUpdate() {
                HTTP.put(`Modules/updateModule/${this.$props.module.id}`, this.editModule)
                    .then((res) => {
                        this.closeModal()
                        this.$emit('render')
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Cập nhật thành công!',
                            life: 3000,
                        })
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
