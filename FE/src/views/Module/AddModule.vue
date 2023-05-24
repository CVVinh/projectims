<template>
    <Add label="Thêm mới" @click="openModal" class="custom-button-h"/>
    <Dialog
        header="Thêm chức năng"
        :visible="displayModal"
        :breakpoints="{ '1500px': '45vw', '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '30vw', maxWidth: '500px' }"
        :modal="true"
        :maximizable="true"
        :closable="false"
        @hide="handleHide"
    >
        <div class="container">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <div class="row">
                        <div class="col-sm-12 col-md-12">
                            <label for="moduleName">Tên chức năng <b style="color: red">*</b></label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12  mb-3">
                            <InputText
                                v-model="newModule.nameModule"
                                id="moduleName"
                                type="text"
                                style="width: 100%"
                                :class="v$.newModule.nameModule.$invalid && isSubmited ? 'p-invalid' : ' '"
                                class="custom-input-h"
                            />
                            <small v-if="v$.newModule.nameModule.$invalid && isSubmited" id="moduleName-help" class="p-error">
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
                                v-model="newModule.note"
                                :autoResize="true"
                                rows="5"
                                cols="30"
                                style="width: 100%"
                                :class="v$.newModule.note.$invalid && isSubmited ? 'p-invalid' : ' '"
                                class="custom-input-h"
                            />
                            <small v-if="v$.newModule.note.$invalid && isSubmited" id="note-help" class="p-error">
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
            <Button label="Lưu" icon="pi pi-check" autofocus @click="handleSave(v$.$invalid)" class="custom-button-h ms-2"/>
        </template>
    </Dialog>
</template>

<script>
    import Add from '../../components/buttons/Add.vue'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { HTTP } from '@/http-common'
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
    export default {
        components: { ButtonCustom },
        setup: () => ({
            v$: useVuelidate(),
        }),
        data() {
            return {
                isSubmited: false,
                newModule: {
                    nameModule: '',
                    note: '',
                },
                displayModal: false,
                waitingAdd: false,
            }
        },
        validations() {
            return {
                newModule: {
                    nameModule: { required },
                    note: { required },
                },
            }
        },
        methods: {
            openModal() {
                this.displayModal = true
            },
            closeModal() {
                this.isSubmited = false
                this.newModule = {
                    nameModule: '',
                    note: '',
                }
                this.displayModal = false
            },
            async handleAdd() {
                await HTTP.post(`Modules/createModule`, this.newModule)
                    .then((res) => {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Thêm mới thành công!',
                            life: 3000,
                        })
                        this.closeModal()
                        this.$emit('render')
                    })
                    .catch((err) => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Lỗi',
                            detail: 'Lỗi! Thêm mới!',
                            life: 3000,
                        })
                    })
            },
            async handleSave(isFormValid) {
                if (this.waitingAdd) {
                    return
                } else {
                    this.waitingAdd = true
                    setTimeout(() => {
                        this.waitingAdd = false
                    }, 3000)
                }
                this.isSubmited = true
                if (isFormValid) {
                    return
                }
                await this.handleAdd()
            },
            handleHide() {
                this.closeModal()
            },
        },
        components: {
            Add,
        },
    }
</script>

<style scoped>
    .field label {
        margin-bottom: 5px;
    }
</style>
