<template>
    <Dialog
        :header="selectedLeaveOff.id ? 'Cập nhật nghỉ phép' : 'Thêm nghỉ phép'"
        :maximizable="true"
        :closable="false"
        position="center"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '55vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
    >
        <div class="container">
            <form class="form-addproject">
                <div class="row mb-3 mt-3">
                    <div class="col-sm-12 col-md-12">
                        <div class="d-flex align-items-center">
                            <InputSwitch id="onLeaveOff" v-model="onLeaveOff" />
                            <label class="ms-2" for="onLeaveOff">Nghỉ trong ngày</label>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12 col-md-4 col-lg-4 mb-3">
                        <div class="field">
                            <label for="dateStart" :class="{ 'p-error': v$.leaveOff.startTime.$invalid && isSubmit }">
                                Ngày bắt đầu
                                <span style="color: red">*</span>
                            </label>
                            <div class="p-float-label" :class="{ 'form-group--error': v$.leaveOff.startTime.$error }">
                                <Calendar
                                    timeOnly
                                    style="width: 100%"
                                    v-model="v$.leaveOff.startTime.$model"
                                    v-if="onLeaveOff"
                                    :minDate="futureTimeMin"
                                    :maxDate="futureTimeMax"
                                    :class="{ 'p-invalid': v$.leaveOff.startTime.$invalid && isSubmit }"
                                />
                                <vue-flatpickr
                                    v-else
                                    class="p-dropdown-label p-inputtext"
                                    v-model="v$.leaveOff.startTime.$model"
                                    :config="flatpickrOptions"
                                    style="width: 100%; height: 35px;"
                                    :class="{ 'p-invalid': v$.leaveOff.startTime.$invalid && isSubmit }"
                                ></vue-flatpickr>
                            </div>
                            <small class="p-error" v-if="v$.leaveOff.startTime.required.$invalid && isSubmit">
                                <!-- {{ v$.leaveOff.startTime.required.$message.replace('Value', 'Start Time') }} -->
                                Ngày nghỉ không được để trống
                            </small>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-4 col-lg-4 mb-3">
                        <div class="field">
                            <label for="dateEnd" :class="{ 'p-error': v$.leaveOff.endTime.$invalid && isSubmit }">
                                Ngày kết thúc
                                <span style="color: red">*</span>
                            </label>
                            <div class="p-float-label" :class="{ 'form-group--error': v$.leaveOff.endTime.$error }">
                                <Calendar
                                    timeOnly
                                    style="width: 100%"
                                    v-model="v$.leaveOff.endTime.$model"
                                    v-if="onLeaveOff"
                                    :minDate="futureTimeMin"
                                    :maxDate="futureTimeMax"
                                    :class="{ 'p-invalid': v$.leaveOff.endTime.$invalid && isSubmit }"
                                />
                                <vue-flatpickr
                                    v-else
                                    class="p-dropdown-label p-inputtext"
                                    v-model="v$.leaveOff.endTime.$model"
                                    :config="flatpickrOptions"
                                    style="width: 100%; height: 35px;"
                                    :class="{ 'p-invalid': v$.leaveOff.endTime.$invalid && isSubmit }"
                                ></vue-flatpickr>
                            </div>
                            <small class="p-error" v-if="v$.leaveOff.endTime.required.$invalid && isSubmit">
                                <!-- {{ v$.leaveOff.endTime.required.$message.replace('Value', 'Ngày kết thúc') }} -->
                                Ngày kết thúc không được để trống
                            </small>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-4 col-lg-4 mb-3">
                        <label for="dateEnd" :class="{ 'p-error': v$.leaveOff.idCompanyBranh.$invalid && isSubmit }">
                            Chọn chi nhánh
                            <span style="color: red">*</span>
                        </label>
                        <Dropdown
                            style="width: 100%"
                            id="idCompanyBranh"
                            v-model="v$.leaveOff.idCompanyBranh.$model"
                            :options="arrCompany"
                            optionLabel="name"
                            optionValue="id"
                            placeholder="Chọn chi nhánh"
                            class="custom-input-h"
                            :class="{ 'p-invalid': v$.leaveOff.idCompanyBranh.$invalid && isSubmit }"
                        />
                        <small class="p-error" v-if="v$.leaveOff.idCompanyBranh.required.$invalid && isSubmit">
                            <!-- {{ v$.leaveOff.idCompanyBranh.required.$message.replace('Value', 'Chi nhánh công ty') }} -->
                            Chi nhánh làm việc không được để trống
                        </small>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-12">
                        <label for="Reason" :class="{ 'p-error': v$.leaveOff.reason.$invalid && isSubmit }">
                            Lý do
                            <span style="color: red">*</span>
                        </label>
                        <Textarea
                            id="Reason"
                            v-model="v$.leaveOff.reason.$model"
                            placeholder="Nhập lý do nghỉ tại đây..."
                            class="input form-control"
                            rows="5"
                        />
                        <small class="p-error" v-if="v$.leaveOff.reason.required.$invalid && isSubmit">
                           <!--  {{ v$.leaveOff.reason.required.$message.replace('Value', 'Reason') }} -->
                           Lý do nghỉ không được để trống
                        </small>
                    </div>
                </div>
            </form>
        </div>
        <template #footer>
            <Button
                id="button-close"
                label="Huỷ"
                icon="pi pi-times"
                class="p-button-secondary p-button-outlined CustomButtonPrimeVue"
                @click="closeDialog()"
            ></Button>
            <Button
                id="button-save"
                label="Lưu"
                icon="pi pi-check"
                class="p-button-primary p-button-icon mt-2 p-button-primary CustomButtonPrimeVue"
                @click="submitRegisterLeaveOff()"
            >
            </Button>
        </template>
    </Dialog>
</template>
<script>
    import { HTTP, GET_LEAVE_OFF_BY_ID, ENDPIONTS, UPDATE_LEAVE_OFF, GET_FIREBASE_TOKEN } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { LeaveOffDto } from '@/views/LeaveOff/LeaveOff.dto'
    import { required } from '@vuelidate/validators'
    import { HttpStatus } from '@/config/app.config'
    import { DateHelper } from '@/helper/date.helper'
    import jwtDecode from 'jwt-decode'
    import { Company } from './Company'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import { HTTP_FIREBASE } from '@/helper/firebase.helper'

    export default {
        props: ['isOpen', 'selectedLeaveOff','idCompanybranch'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            const todayDate = new Date()
            todayDate.setDate(todayDate.getDate() - 7)

            const todayTimeMin = new Date()
            todayTimeMin.setDate(todayTimeMin.getDate())
            todayTimeMin.setHours(8 - 1)
            const todayTimeMax = new Date()
            todayTimeMax.setDate(todayTimeMax.getDate())
            todayTimeMax.setHours(17, 30)
            return {
                tokenPM: [],
                leaveOff: new LeaveOffDto(),
                isSubmit: false,
                timeFormat: new Date() ,
                onLeaveOff: false,
                idCompanyBranh: 1,
                arrCompany: Company,
                futureTimeMin: todayTimeMin,
                futureTimeMax: todayTimeMax,
                flatpickrOptions: {
                    enableTime: true,
                    allowInput: true,
                    time_24hr: true,
                    dateFormat: 'Y-m-d H:i',
                    minDate: null,
                    disable: [
                        function (date) {
                            return date.getDay() === 0 || date.getDay() === 6
                        },
                    ],
                    locale: {
                        weekdays: {
                            shorthand: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                            longhand: ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'],
                        },
                        months: {
                            shorthand: [
                                'Th1',
                                'Th2',
                                'Th3',
                                'Th4',
                                'Th5',
                                'Th6',
                                'Th7',
                                'Th8',
                                'Th9',
                                'Th10',
                                'Th11',
                                'Th12',
                            ],
                            longhand: [
                                'Tháng 1',
                                'Tháng 2',
                                'Tháng 3',
                                'Tháng 4',
                                'Tháng 5',
                                'Tháng 6',
                                'Tháng 7',
                                'Tháng 8',
                                'Tháng 9',
                                'Tháng 10',
                                'Tháng 11',
                                'Tháng 12',
                            ],
                        },
                        firstDayOfWeek: 1,
                        rangeSeparator: ' đến ',
                    },
                },
            }
        },
        created() {
            this.timeFormat.setHours(0)
            this.timeFormat.setMinutes(0)
            this.timeFormat.setSeconds(0)
        },
        watch: {
            onLeaveOff: {
                handler: function Change(status) {
                    if (this.selectedLeaveOff.id) {
                    } else {
                        if (status == false) {
                            this.timeFormat.setHours(0)
                            this.timeFormat.setMinutes(0)
                            this.timeFormat.setSeconds(0)
                            this.leaveOff.endTime = this.timeFormat
                            this.leaveOff.startTime = this.timeFormat
                        } else {
                            this.timeFormat.setHours(8)
                            this.timeFormat.setMinutes(0)
                            this.timeFormat.setSeconds(0)
                            this.leaveOff.endTime = this.timeFormat
                            this.leaveOff.startTime = this.timeFormat
                        }
                    }
                },
                deep: true,
            },
        },
        beforeUpdate() {
            this.leaveOff.startTime = this.timeFormat
            this.leaveOff.idCompanyBranh = Number(checkAccessModule.getBranchUser());
            this.leaveOff.endTime = this.timeFormat
            if (this.selectedLeaveOff.id) {
                HTTP.get(GET_LEAVE_OFF_BY_ID(this.selectedLeaveOff.id))
                    .then((res) => {
                        if (res.status === HttpStatus.OK) {
                            var date1 = new Date(res.data._Data.startTime)
                            var date2 = new Date(res.data._Data.endTime)
                            if (
                                date1.getHours() != 8 ||
                                date1.getMinutes() != 0 ||
                                date2.getHours() != 0 ||
                                date2.getMinutes() != 0
                            ) {
                                this.onLeaveOff = true
                            }
                            this.leaveOff.reason = res.data._Data.reasons
                            this.leaveOff.startTime = new Date(res.data._Data.startTime)
                            this.leaveOff.endTime = new Date(res.data._Data.endTime)
                            this.leaveOff.idCompanyBranh = res.data._Data.idCompanyBranh
                            this.onLeaveOff = res.data._Data.flagOnDay
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
        methods: {
            submitRegisterLeaveOff() {
                this.isSubmit = true
                var date1 = new Date(this.leaveOff.startTime)
                var date2 = new Date(this.leaveOff.endTime)
                if (!this.v$.$invalid && this.checkDate(this.leaveOff.startTime, this.leaveOff.endTime)) {
                    return
                }
                if (this.onLeaveOff) {
                    if (
                        date1.getDate() != date2.getDate() ||
                        date1.getMonth() != date2.getMonth() ||
                        date1.getFullYear() != date2.getFullYear()
                    ) {
                        this.toastWarn('Chỉ cho phép nghỉ trong ngày, ngày tháng năm phải trùng nhau !')
                        return
                    }
                    if (date1.getDay() == 0 || date1.getDay() == 6 || date2.getDay() == 0 || date2.getDay() == 6) {
                        this.toastWarn('Không được phép nghỉ thứ 7, CN !')
                        return
                    }
                }
                if (date1.getDay() == 0 || date1.getDay() == 6) {
                    this.toastWarn('Không được phép nghỉ thứ 7, CN !')
                    return
                } else if (
                    (date1.getDay() == 5 && date2.getDay() == 6) ||
                    (date1.getDay() == 5 && date2.getDay() == 0)
                ) {
                    this.toastWarn('Không được phép nghỉ thứ 7, CN !')
                    return
                }
                if (
                    date1.getHours() != 0 ||
                    date1.getMinutes() != 0 ||
                    date2.getHours() != 0 ||
                    date2.getMinutes() != 0
                ) {
                    if (
                        date1.getHours() < 8 ||
                        (date1.getHours() > 17 && date1.getMinutes() > 30) ||
                        date2.getHours() < 8 ||
                        (date2.getHours() > 17 && date2.getMinutes() > 30)
                    ) {
                        this.toastWarn('Giờ phải nằm trong khoản 8 giờ đến 17 giờ 30 phút')
                        return
                    }
                }

                if (!this.v$.$invalid) {
                    if (this.selectedLeaveOff.id) {
                        this.handlerEditLeaveOff()
                    } else {
                        this.handlerAddLeaveOff()
                    }
                }
            },
            handlerAddLeaveOff() {
                const addNewLeaveOffDto = {
                    idLeaveUser: checkAccessModule.getUserIdCurrent(),
                    startTime: DateHelper.formatDateTime(this.leaveOff.startTime),
                    endTime: DateHelper.formatDateTime(this.leaveOff.endTime),
                    reasons: this.leaveOff.reason,
                    idCompanyBranh: this.leaveOff.idCompanyBranh,
                    flagOnDay: this.onLeaveOff,
                }
                if (addNewLeaveOffDto) {
                    HTTP.post(ENDPIONTS.ADD_NEW_LEAVE_OFF, addNewLeaveOffDto)
                        .then((res) => {
                            if (res.status == 200) {
                                this.toastSuccess(res.data._Message)
                                this.sendMessage()
                                this.closeDialog()
                                this.getAllLeaveOff()
                            } else {
                                this.closeDialog()
                            }
                        })
                        .catch((err) => {
                            this.toastWarn('Ngày bắt đầu và kết thúc trùng nhau.')
                        })
                }
            },
            handlerEditLeaveOff() {
                var dateMap = new Date(this.leaveOff.startTime)
                const editRegisterLeaveOffDtos = {
                    idLeaveUser: checkAccessModule.getUserIdCurrent(),
                    startTime: DateHelper.formatDateTime(dateMap),
                    endTime: DateHelper.formatDateTime(this.leaveOff.endTime),
                    reasons: this.leaveOff.reason,
                    idCompanyBranh: this.leaveOff.idCompanyBranh,
                    flagOnDay: this.onLeaveOff,
                }
                if (editRegisterLeaveOffDtos) {
                    HTTP.put(UPDATE_LEAVE_OFF(this.selectedLeaveOff.id), editRegisterLeaveOffDtos)
                        .then((res) => {
                            switch (res.status) {
                                case HttpStatus.OK:
                                    this.toastSuccess(res.data._Message)
                                    this.closeDialog()
                                    this.sendMessage()
                                    this.getAllLeaveOff()
                                    break
                                case HttpStatus.FORBIDDEN:
                                case HttpStatus.UNAUTHORIZED:
                                    this.toastError(res.data._Message)
                                    this.onClickCancel()
                                    break
                                default:
                            }
                        })
                        .catch((err) => {
                            this.toastWarn(err.message)
                        })
                }
            },
            async sendMessage() {
                try {
                    const res = await HTTP.get(GET_FIREBASE_TOKEN)
                    const registrationIds = res.data.map((item) => item.token)
                    const message = {
                        registration_ids: registrationIds,
                        notification: {
                            title: 'Thông báo đến từ IMS',
                            body: 'Có 1 đơn xin nghỉ phép',
                        },
                        webpush: {
                            headers: {
                                Urgency: 'high',
                            },
                        },
                    }
                    const response = await HTTP_FIREBASE.post('/fcm/send', message)
                } catch (error) {
                    console.log('Error sending message', error)
                }
            },
            getAllLeaveOff() {
                this.$emit('getAllLeaveOff')
            },
            closeDialog() {
                this.resetForm()
                this.$emit('closeDialog')
            },
            resetForm() {
                this.onLeaveOff = false
                this.isSubmit = false
                this.leaveOff = {
                    startTime: null,
                    endTime: null,
                    reason: null,
                    idCompanyBranh: 1,
                }
            },
            toastSuccess(message) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: message,
                    life: 3000,
                })
            },
            toastWarn(err) {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Cảnh báo',
                    detail: err,
                    life: 3000,
                })
            },
            toastError(err) {
                this.$toast.add({
                    severity: 'error',
                    summary: 'Lỗi',
                    detail: err,
                    life: 3000,
                })
            },
            checkDate(startDate, endDate) {
                if (startDate > endDate) {
                    this.$toast.add({
                        severity: 'error',
                        summary: 'Lỗi',
                        detail: 'Ngày kết thúc không được nhỏ hơn ngày bắt đầu!',
                        life: 3000,
                    })
                    return true
                }
                return false
            },
        },
        validations() {
            return {
                leaveOff: {
                    endTime: { required },
                    startTime: { required },
                    reason: { required },
                    idCompanyBranh: { required },
                    flagOnDay: {},
                },
            }
        },
    }
</script>
<style lang="scss" scoped>
    @media (max-width: 573px) {
        #button-save {
            font-size: 12px;
        }
        #button-close {
            font-size: 12px;
        }
    }
</style>
