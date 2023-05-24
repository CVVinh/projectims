<template>
    <Dialog
        header="Thêm nhóm"
        :maximizable="true"
        :closable="false"
        position="center"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '30vw' }"
    >
        <div class="contener">
            <form class="row" @submit.prevent="submitGroup()">
                <div class="col-sm-12 col-md-12">
                    <div class="row">
                        <div class="col-sm-12 col-md-12">
                            <label
                                :class="{
                                    'p-error': v$.formGroup.nameGroup.required.$invalid && isSubmit,
                                }"
                                >Mã nhóm<b style="color: red">*</b></label>
                            <p><small><i><span class="p-error"><b>Lưu ý:</b></span> mã nhóm viết liền, không khoảng trắng, và không trùng các nhóm khác!</i></small></p>
                            
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12 col-md-12 mb-3">
                            <InputText v-model="v$.formGroup.nameGroup.$model" class="custom-input-h" placeholder="Nhập mã nhóm"/>
                            <small class="p-error" v-if="v$.formGroup.nameGroup.required.$invalid && isSubmit">{{
                                v$.formGroup.nameGroup.required.$message.replace('Value', 'Name group')
                            }}</small>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12 col-md-12">
                            <label >Diễn đạt ý nghĩa tên nhóm </label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12 col-md-12 mb-3">
                            <Textarea v-model="this.formGroup.discription" class="custom-input-h" rows="5"  placeholder="Diễn đạt ý nghĩa nhóm"/>
                        </div>
                    </div>

                    <div class="btn-right">
                        <ButtonCustom  @click="closeDialog()" /> 
                        <!-- <Button label="Hủy" icon="pi pi-times" class="p-button-secondary btn-margin custom-button-h" v-on:click="closeDialog()" /> -->
                        <Button label="Lưu" type="submit" icon="pi pi-check" class="btn-margin custom-button-h ms-2"/>
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>
<script>
    import { HTTP } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { checkAccessModule } from '@/helper/checkAccessModule';
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
    export default {
        components: { ButtonCustom },
        props: ['isOpen'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                formGroup: {
                    nameGroup: null,
                    discription: '',
                },
                isSubmit: false,
            }
        },
        methods: {
            showError(message) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
            },

            showSuccess(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
            },

            async submitGroup() {
                this.isSubmit = true
                if (!this.v$.$invalid) {
                    var addGroupDto = {
                        NameGroup: this.formGroup.nameGroup,
                        Discription: this.formGroup.discription,
                        Key: this.formGroup.nameGroup,
                        userCreated: checkAccessModule.getUserIdCurrent(),
                    }
                    await HTTP.post('Group/addGroup', addGroupDto)
                        .then((res) => {
                            if (res.status == 200) {
                                this.showSuccess('Thêm mới thành công!');
                                this.closeDialog()
                                this.$emit('reloadPage')
                            }
                        })
                        .catch((err) => {
                            console.log(err.response.message);
                            this.showError("Nhóm đã tồn tại hoặc 1 lỗi khác!");
                        })
                }
            },

            closeDialog() {
                this.$emit('closeDialog')
            },

            resetForm() {
                this.isSubmit = false
                this.formGroup = {
                    nameGroup: null,
                    discription: '',
                }
            },
        },

        validations() {
            return {
                formGroup: {
                    nameGroup: { required },
                },
            }
        },
    }
</script>
<style scoped lang="scss">
    .form-layout {
        min-width: 600px;
        width: 100%;
        height: 100%;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-items: center;
    }
   
    .input-layout {
        width: calc(100% - 36px);
        margin: 24px 18px 0px 18px;
        display: flex;
        justify-items: flex-start;
        align-items: start;
        flex-direction: column;
    }
    .input-title {
        width: 100%;
    }
    .p-error {
        width: 100%;
        max-width: 600px;
    }
    .form-addproject {
        width: 100%;
        min-width: 120px;
    }
   
    .header-outside {
        z-index: 1;
        width: 100%;
        height: 84px;
        display: flex;
        justify-content: center;
        align-items: center;
        color: white;
        background-color: #33adff;
        position: sticky;
        top: 0px;
    }

    .note {
        font-size: small;
        font-style: italic;
    }
    .addDevice {
        margin-top: 24px;
    }
    .outSideField {
        width: 100%;
        display: flex;
        justify-content: center;
    }
    .btn-right {
        float: right;
        margin-top: 10px;

        .btn-margin {
            margin-right: 6px;
        }
    }
    .p-dialog-header-close-icon {
        z-index: 2000;
    }
</style>

