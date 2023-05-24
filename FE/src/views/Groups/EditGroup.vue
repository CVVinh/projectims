<template>
    <Dialog
        header="Cập nhật thông tin nhóm"
        :maximizable="true"
        :closable="false"
        position="center"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '30vw' }"
    >
        <div class="containter">
            <form class="row" @submit.prevent="submitGroup()">
                <div class="col-sm-12 col-md-12">
                    <div class="row">
                        <div class="col-sm-12 col-md-12">
                            <label
                                :class="{
                                    'p-error': v$.group.nameGroup.required.$invalid && isSubmit,
                                }"
                                >Mã nhóm<b style="color: red">*</b></label
                            >
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12 col-md-12 mb-3">
                            <InputText v-model="v$.group.nameGroup.$model" class="custom-input-h" disabled="false" placeholder="Nhập mã nhóm"/>
                            <small class="p-error" v-if="v$.group.nameGroup.required.$invalid && isSubmit">{{
                                v$.group.nameGroup.required.$message.replace('Value', 'Name group')
                            }}</small>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12 col-md-12">
                            <label>Diễn đạt ý nghĩa tên nhóm </label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12 col-md-12 mb-3">
                            <Textarea v-model="this.group.discription" class="custom-input-h" rows="5" placeholder="Diễn đạt ý nghĩa nhóm"/>
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
        props: ['group', 'isOpen'],
        components: { ButtonCustom },
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
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
                if (!this.v$.$invalid ) {
                    var updateGroupDto = {
                        NameGroup: this.group.nameGroup,
                        Discription: this.group.discription,
                        Key: this.group.nameGroup,
                        userModified: checkAccessModule.getUserIdCurrent(),
                    }
                    await HTTP.put(`Group/updateGroup/${this.group.id}`, updateGroupDto)
                        .then((res) => {
                            if (res.status == 200) {
                                this.closeDialog()
                                this.showSuccess('Cập nhật thành công!');
                                this.$emit('reloadPage')
                            }
                        })
                        .catch((err) => {
                            if (err.data) {
                                this.showError("Lỗi! cập nhật nhóm!");
                                console.log(err.response.message);
                            } 
                        })
                }
            },

            closeDialog() {
                this.resetForm()
                this.$emit('closeDialog')
            },

            resetForm() {
                this.isSubmit = false
            },
        },

        validations() {
            return {
                group: {
                    nameGroup: { required },
                },
            }
        },
    }
</script>
<style scoped lang="scss">
    
    .p-error {
        width: 100%;
        max-width: 600px;
    }
    .form-addproject {
        width: 100%;
        min-width: 120px;
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

