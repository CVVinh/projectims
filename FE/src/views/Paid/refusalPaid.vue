<template>
    <Dialog
        header="Lý do không chấp nhận thanh toán?"
        :maximizable="true"
        :closable="false"
        position="center"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="status"
    >
        <div class="container">
            <form class="form-addproject" @submit.prevent="submitRefusalPaids()">
                <div class="input-layout w-100">
                    <label class="mb-2" for="name" :class="{ 'p-error': v$.paids.reasonRefusal.$invalid && submitted }">
                        Lý do
                        <span style="color: red">*</span>
                    </label>
                    <Textarea
                        v-model="v$.paids.reasonRefusal.$model"
                        placeholder="Nhập lý do tại đây..."
                        class="input form-control"
                        rows="5"
                    />
                    <small class="p-error" v-if="v$.paids.reasonRefusal.required.$invalid && isSubmit">
                        {{ v$.paids.reasonRefusal.required.$message.replace('Value', 'ReasonRefusal') }}
                    </small>
                </div>
                <div class="group-button mt-3">
                    <div class="d-flex justify-content-end">
                        <Button  label="Hủy" class="p-button-secondary p-button-outlined CustomButtonPrimeVue me-2"  icon="pi pi-times"  @click="closeDialog()"/>
                        <Button  label="Lưu" type="submit" class="p-button-danger CustomButtonPrimeVue" autofocus icon="pi pi-trash"/>
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>
<script>
    import { HTTP, NOT_ACCEPT_LEAVE_OFF } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import jwtDecode from 'jwt-decode'
    import { checkAccessModule } from '@/helper/checkAccessModule';
    export default {
        props: ['status', 'refusalData'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                paids: {
                    id: null,
                    reasonRefusal: null,
                },
                isSubmit: false,
                submitted: false,
            }
        },
        async beforeUpdate() {
            if(this.refusalData !=  null){
                this.paids = this.refusalData;
            }
        },
        methods: {
            showError(message) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
            },

            showSuccess(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
            },
            
            async submitRefusalPaids() {
                this.isSubmit = true
                if (!this.v$.$invalid) {
                    var reasonRefusalPaid = this.paids.reasonRefusal;
                    var refusalPaymentPaidDtos = {
                        PersonConfirm: checkAccessModule.getUserIdCurrent(),
                        ReasonRefusal: reasonRefusalPaid,
                    }
                    await HTTP.put(`Paid/NotAcceptPayment/${this.paids.id}`, refusalPaymentPaidDtos)
                        .then((res) => {
                            if (res.status == 200) {
                                this.showSuccess('Đã từ chối!');
                                this.submitted = true
                                this.closeDialog()
                                this.resetForm()
                                this.$emit('reloadpage')
                            }
                        })
                        .catch((err) => {
                            this.showError('Lỗi!');
                        })
                }
            },
            closeDialog() {
                this.$emit('closemodal')
            },
            resetForm() {
                this.isSubmit = false
                this.paids = {
                    reasonRefusal: null,
                }
            },
        },
        validations() {
            return {
                paids: {
                    reasonRefusal: { required },
                },
            }
        },
    }
</script>
<style scoped></style>
