<template>
    <div class="d-inline">
        <Edit @click="openModal" class="custom-button-h" />
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
                        <label for="moduleName">Tên chức năng thay đổi thao tác<b class="p-error">*</b></label>      
                    </div>
                </div>
                <div class="row mb-3">
                    <div class="col-sm-12 col-md-12">
                        <Dropdown 
                            disabled
                            v-model="idModule" :options="listModule" optionLabel="nameModule" optionValue="id"
                            placeholder="Select a module" 
                            id="moduleName"
                            :class="v$.idModule.$invalid && isSubmited ? 'p-invalid' : ' '"
                            class="custom-input-h"
                        />  
                        <small v-if="v$.idModule.$invalid && isSubmited" id="moduleName-help" class="p-error">
                            Chọn chức năng.
                        </small>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12 col-md-12">
                        <label for="note">Chọn thao tác cho chức năng<b class="p-error">*</b></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12 col-md-12">
                        <MultiSelect  
                            filter
                            v-model="idAction" :options="listAction" optionLabel="name" optionValue="id"
                            placeholder="Chọn thao tác" 
                            id="moduleName"
                            :maxSelectedLabels="5" 
                            :class="v$.idAction.$invalid && isSubmited ? 'p-invalid' : ' '" 
                            class="custom-input-h"
                        />
                        
                        <small v-if="v$.idAction.$invalid && isSubmited" id="note-help" class="p-error">
                            Chọn thao tác
                        </small>
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
    import { checkAccessModule } from '@/helper/checkAccessModule'
    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        props: {
            action: Object,
        },
        data() {
            return {
                isSubmited: false,
                idModule: null,
                idAction:[],
                listModule:[],
                listAction:[],
                displayModal: false,
            }
        },
        validations() { 
            return {
                idModule: { required },
                idAction: { required },
            }
        },
        async mounted() {
            await this.getListSelect()
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
            async getListSelect(){
                this.listModule = []
                this.listAction = []
                await HTTP.get("ActionModule/getAllActionModule")
                .then(res=>{
                    this.listAction = res.data._Data
                })
                .catch(err=>console.log(err))
                await HTTP.get("Modules/getListModule")
                .then(res=>{
                    this.listModule = res.data._Data
                })
                .catch(err=>console.log(err))
            },
            handleLoad() {
                this.idAction = []
                this.idModule = this.$props.action.moduleId
                this.$props.action.actionModule.map(ele => {
                    this.idAction.push(ele.actionModuleId);
                })
            },
            async handleUpdate() {
                var addMduleActions = [];
                if(this.idAction.length > 0){
                    this.idAction.map(ele => {
                        var newObject = {
                            "moduleId": this.idModule,
                            "actionModuleId": ele,
                            "userCreated": checkAccessModule.getUserIdCurrent()
                        }
                        addMduleActions.push(newObject);
                    });
                }
                
                await HTTP.post(`PermissionActionModule/createPermissionActionModule`, addMduleActions)
                    .then((res) => {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Cập nhật thành công!',
                            life: 3000,
                        })
                        this.closeModal()
                        this.$emit('render')
                    })
                    .catch((err) => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Lỗi',
                            detail: 'Cập nhật lỗi!',
                            life: 3000,
                        })
                    })
            },
            async handleSave(isFormValid) {
                this.isSubmited = true
                if (isFormValid) {
                    return
                }
                await this.handleUpdate()
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

<style lang="scss" scoped>
    .p-dropdown {
        width: 100%;
    }
    .field label {
        margin-bottom: 5px;
    }
</style>
