<template>
    <Dialog
        header="Lý do không cho phép nghỉ?"
        :maximizable="true"
        :closable="false"
        position="center"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
    >
        <div class="container">
            <form class="form-addproject">
                <div class="input-layout w-100">
                    <label class="mb-2" for="name" :class="{ 'p-error': v$.leaveOff.reason.$invalid && isSubmit }">
                        Lý do không duyệt<b style="color: red">*</b>
                    </label>
                    <Textarea
                        v-model="v$.leaveOff.reason.$model"
                        placeholder="Nhập lý do tại đây..."
                        class="input form-control"
                        rows="5"
                    />
                    <small class="p-error" v-if="v$.leaveOff.reason.required.$invalid && isSubmit">
                        <!-- {{ v$.leaveOff.reason.required.$message.replace('Value', 'Reason') }} -->
                        Lý do không được bỏ trống!
                    </small>
                </div>
            </form>
        </div>
        <template #footer>
            <Button
                label="Huỷ"
                icon="pi pi-times"
                class="p-button-secondary p-button-outlined CustomButtonPrimeVue"
                @click="closeDialog()"
            ></Button>
            <Button
                label="Lưu"
                icon="pi pi-check"
                class="p-button-primary CustomButtonPrimeVue"
                @click="submitLeaveOff()"
            >
            </Button>
        </template>
    </Dialog>
</template>
<script>
    import { HTTP, NOT_ACCEPT_LEAVE_OFF } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import jwtDecode from 'jwt-decode'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    export default {
        props: ['isOpen', 'idLeaveOff'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                leaveOff: {
                    reason: null,
                },
                isSubmit: false,
            }
        },
        methods: {
            submitLeaveOff() {
                this.isSubmit = true
                if (!this.v$.$invalid) {
                    var notAcceptLeaveOffDto = {
                        idAcceptUser: checkAccessModule.getUserIdCurrent(),
                        ReasonNotAccept: this.leaveOff.reason,
                    }
                    HTTP.put(NOT_ACCEPT_LEAVE_OFF(this.idLeaveOff), notAcceptLeaveOffDto)
                        .then((res) => {
                            if (res.status == 200) {
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Thành công',
                                    detail: res.data._Message,
                                    life: 3000,
                                })
                                this.closeDialog()
                                this.$emit('reloadPage')
                            }
                        })
                        .catch((err) => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: err.message,
                                life: 2000,
                            })
                        })
                }
            },
            closeDialog() {
                this.resetForm()
                this.$emit('closeDialog')
            },
            resetForm() {
                this.isSubmit = false
                this.leaveOff = {
                    reason: null,
                }
            },
        },
        validations() {
            return {
                leaveOff: {
                    reason: { required },
                },
            }
        },
    }
</script>
<style scoped lang="scss">
</style>
