<template>
    <Dialog 
        header="Cập nhật thông tin menu"
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
                                    'p-error': v$.Datasend.idModule.required.$invalid,
                                    'input-title': true,
                                }"
                                >Chức năng<b style="color: red">*</b>
                            </label>
                        </div>
                        
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12 mb-3">
                            <Dropdown
                                class="custom-input-h"
                                @load="GetParenByModule"
                                @change="GetParenByModule"
                                v-model="v$.Datasend.idModule.$model"
                                :options="optionmodule"
                                optionLabel="nameModule"
                                optionValue="id"
                                placeholder="Select a module"
                            />
                            <small class="p-error" v-if="v$.Datasend.idModule.required.$invalid">{{
                                v$.Datasend.idModule.required.$message.replace('Value', 'Title')
                            }}</small>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.title.required.$invalid,
                                    'input-title': true,
                                }"
                                >Tên<b style="color: red">*</b>
                            </label>
                        </div>
                        
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12 mb-3">
                            <InputText type="text" v-model="v$.Datasend.title.$model" placeholder="Nhập tên" class="custom-input-h"/>
                            <small class="p-error" v-if="v$.Datasend.title.required.$invalid">{{
                                v$.Datasend.title.required.$message.replace('Value', 'Title')
                            }}</small>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.controller.required.$invalid,
                                    'input-title': true,
                                }"
                                >Điều hướng<b style="color: red">*</b>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12 mb-3">
                            <InputText
                                class="custom-input-h"
                                type="text"
                                v-model="v$.Datasend.controller.$model"
                                placeholder="Nhập trang điều hướng"
                            />
                            <small class="p-error" v-if="v$.Datasend.controller.required.$invalid">{{
                                v$.Datasend.controller.required.$message.replace('Value', 'Controller')
                            }}</small>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.action.required.$invalid,
                                    'input-title': true,
                                }"
                                >Thực thi<b style="color: red">*</b>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12 mb-3">
                            <InputText type="text" v-model="v$.Datasend.action.$model" placeholder="Nhập trang thực thi" class="custom-input-h"/>
                            <small class="p-error" v-if="v$.Datasend.action.required.$invalid">{{
                                v$.Datasend.action.required.$message.replace('Value', 'Action')
                            }}</small>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.icon.required.$invalid,
                                    'input-title': true,
                                }"
                                >Icon<b style="color: red">*</b>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12 mb-3">
                            <InputText type="text" v-model="v$.Datasend.icon.$model" placeholder="Nhập lớp Icon" class="custom-input-h"/>
                            <small class="p-error" v-if="v$.Datasend.icon.required.$invalid">{{
                                v$.Datasend.icon.required.$message.replace('Value', 'icon')
                            }}</small>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.action.required.$invalid,
                                    'input-title': true,
                                }"
                                >Thực thi<b style="color: red">*</b>
                            </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12 mb-3">
                            <InputText type="text" v-model="v$.Datasend.action.$model" placeholder="Nhập trang thực thi" class="custom-input-h"/>
                            <small class="p-error" v-if="v$.Datasend.action.required.$invalid">{{
                                v$.Datasend.action.required.$message.replace('Value', 'Action')
                            }}</small>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12">
                            <label>Nhánh/Lớp cha</label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12 mb-3">
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
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12">
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-12">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <template #footer>
            <ButtonCustom  @click="closeModal" />   
            <!-- <Button label="Đóng" icon="pi pi-times" class="p-button-secondary custom-input-h" @click="closeModal" style="background: white; color: black;" /> -->
            <Button label="Lưu" icon="pi pi-check" autofocus @click="updateMenu" class="custom-input-h"/>
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
                    idModule: 0,
                    title: '',
                    icon: '',
                    view: '',
                    controller: '',
                    action: '',
                    parent: 0,
                },
                Issubmit: false,
                parentArr: [],
                ArrpushOptions: [],
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
        props: ['status', 'module', 'dataedit', 'optionmodule'],
        methods: {
            closeModal() {
                this.$emit('closemodal')
            },
            GetMenubyid(id) {
                if (this.dataedit) {
                    HTTP.get(`Menu/getlistmenuID/${id}`).then((res) => {
                        this.Datasend = res.data
                        this.GetMenuByIdModule()
                    })
                }
            },
            GetParenByModule() {
                if (this.Datasend.parent !== 0) {
                    this.Datasend.parent = this.Datasend.parent
                } else {
                    this.Datasend.parent = 0
                }
                HTTP.get(`Menu/getlistMenubyIdModoule/${this.Datasend.idModule}`).then((res) => {
                    this.parentArr = res.data
                    this.parentArr.push({ title: 'root', id: 0 })
                })
            },

            updateMenu() {
                this.Issubmit = true
                if (!this.v$.$invalid) {
                    HTTP.put('Menu/updateMenu', this.Datasend).then((res) => {
                        if (res.data === 'success') {
                            this.$emit('success')
                        }
                        this.$emit('reloadpage')
                    })
                } else {
                    this.$emit('failed')
                }
            },

            GetMenuByIdModule() {
                if (this.Datasend.parent !== 0) {
                    this.Datasend.parent = this.Datasend.parent
                } else {
                    this.Datasend.parent = 0
                }
                HTTP.get(`Menu/GetlistMenuByIdModuleedit/${this.Datasend.idModule}/${this.dataedit}`).then((res) => {
                    this.parentArr = res.data
                    this.parentArr.push({ title: 'root', id: 0 })
                })
            },
        },

        //  mounted(){
        //     this.GetParenByModule()
        //  },

        beforeUpdate() {
            this.GetMenubyid(this.dataedit)
            //this.GetParenByModule()
        },
        computed: {
            arrSelect() {
                const arr = this.parentArr
                return arr
            },
        },
    }
</script>

<style lang="scss" scoped>
    .p-dropdown {
        width: 100%;
    }
    .Menu__form {
        width: 800px;
        height: 400px;
        display: flex;
    }
    .Menu__form--items {
        width: 50%;
        height: 100%;
        padding: 10px;
        display: flex;
        flex-direction: column;
    }
    .Menu__form--items-content {
        width: 100%;
        height: 95px;
        display: flex;
        flex-direction: column;
        justify-content: center;
    }
    .Menu-form-items-content {
        display: flex;
        justify-content: space-around;
    }

    .country-item-value {
        display: flex;
        height: 30px;
    }
</style>
