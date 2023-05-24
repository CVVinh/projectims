<template>
    <Dialog 
        header="Thêm menu" 
        :visible="status" 
        :maximizable="true" 
        :closable="false" 
        modal
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
    >
        <div class="container">
            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.idModule.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Chức năng<b style="color: red">*</b>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <Dropdown
                                class="custom-input-h"
                                @load="GetParenByModule"
                                @change="GetParenByModule"
                                v-model="v$.Datasend.idModule.$model"
                                :options="optionnew"
                                optionLabel="nameModule"
                                optionValue="id"
                                placeholder="Chọn chức năng"
                            />
                            <small class="p-error" v-if="v$.Datasend.idModule.required.$invalid && isSubmit">{{
                                v$.Datasend.idModule.required.$message.replace('Value', 'Title')
                            }}</small>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.title.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Tên<b style="color: red">*</b>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <InputText type="text" v-model="v$.Datasend.title.$model" placeholder="Nhập tên" class="custom-input-h"/>
                            <small class="p-error" v-if="v$.Datasend.title.required.$invalid && isSubmit">{{
                                v$.Datasend.title.required.$message.replace('Value', 'Title')
                            }}</small>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.controller.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Điều hướng<b style="color: red">*</b>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <InputText
                                type="text"
                                v-model="v$.Datasend.controller.$model"
                                placeholder="Nhập trang điều hướng"
                                class="custom-input-h"
                            />
                            <small class="p-error" v-if="v$.Datasend.controller.required.$invalid && isSubmit">{{
                                v$.Datasend.controller.required.$message.replace('Value', 'Controller')
                            }}</small>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.action.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Thực thi<b style="color: red">*</b>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <InputText type="text" v-model="v$.Datasend.action.$model" placeholder="Nhập trang thực thi" class="custom-input-h"/>
                            <small class="p-error" v-if="v$.Datasend.action.required.$invalid && isSubmit">{{
                                v$.Datasend.action.required.$message.replace('Value', 'Action')
                            }}</small>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.icon.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Icon<b style="color: red">*</b>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <InputText type="text" v-model="v$.Datasend.icon.$model" placeholder="Nhập lớp Icon" class="custom-input-h"/>
                            <small class="p-error" v-if="v$.Datasend.icon.required.$invalid && isSubmit">{{
                                v$.Datasend.icon.required.$message.replace('Value', 'icon')
                            }}</small>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.view.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Hiển thị<b style="color: red">*</b>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <InputText type="text" v-model="v$.Datasend.view.$model" placeholder="Nhập trang hiển thị" class="custom-input-h"/>
                            <small class="p-error" v-if="v$.Datasend.view.required.$invalid && isSubmit">{{
                                v$.Datasend.view.required.$message.replace('Value', 'View')
                            }}</small>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label>Nhánh/Lớp cha</label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <Dropdown
                                class="custom-input-h"
                                v-model="Datasend.parent"
                                :options="parentArr"
                                optionLabel="title"
                                optionValue="id"
                                placeholder="Chọn nhánh/lớp "
                            />
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                        </div>
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <template #footer>
            <ButtonCustom  @click="closeModal" />  
            <!-- <Button label="Đóng" icon="pi pi-times" class="p-button-secondary custom-input-h" @click="closeModal" style="background: white; color: black;"/> -->
            <Button label="Lưu" icon="pi pi-check" autofocus @click="AddMenu" class="custom-input-h"/>
        </template>
    </Dialog>
</template>

<script>
    import { HTTP } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
    export default {
        components: { ButtonCustom },
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                Datasend: {
                    idModule: '',
                    title: '',
                    icon: '',
                    view: '',
                    controller: '',
                    action: '',
                    parent: 0,
                },
                selectedCountry: null,
                parentArr: [],
                isSubmit: false,
                optionnew: [],
            }
        },
        validations() {
            return {
                Datasend: {
                    idModule: { required },
                    title: { required },
                    icon: { required },
                    view: { required },
                    controller: { required },
                    action: { required },
                    parent: 0,
                },
            }
        },
        props: ['status', 'optionmodule'],
        methods: {
            closeModal() {
                this.$emit('closemodal')
            },
            GetParenByModule() {
                this.Datasend.parent = 0
                HTTP.get(`Menu/getlistMenubyIdModoule/${this.Datasend.idModule}`).then((res) => {
                    this.parentArr = res.data
                })
            },
            clearform() {
                this.Datasend.idModule = ''
                this.Datasend.title = ''
                this.Datasend.icon = ''
                this.Datasend.view = ''
                this.Datasend.controller = ''
                this.Datasend.action = ''
                this.Datasend.parent = 0
                this.isSubmit = false
            },
            AddMenu() {
                this.isSubmit = true
                if (!this.v$.$invalid) {
                    HTTP.post('Menu/addMenu', this.Datasend).then((res) => {
                        if (res.data === 'Sucess') {
                            this.clearform()
                            this.$emit('success')
                            this.closeModal()
                        }
                        this.$emit('reloadpage')
                    })
                } else {
                    this.$emit('failed')
                }
            },
        },

        beforeUpdate() {
            this.optionmodule.map((ele) => {
                if (ele.isDeleted == 0) {
                    this.optionnew.push(ele)
                }
            })
        },
    }
</script>

<style lang="scss" scoped>
   .p-dropdown {
        width: 100%;
    }
</style>
