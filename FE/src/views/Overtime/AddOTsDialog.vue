<template>
    <Dialog
        v-model:visible="this.display"
        :closable="false"
        :show="getData()"
        :modal="true"
        :maximizable="true"
        :dismissableMask="true"
    >
        <template #header>
            <h2>{{ this.AddOrEdit === true ? 'Cập nhật thông tin tăng ca' : 'Thêm tăng ca' }}</h2>
        </template>
        <template #footer >     
            <ButtonCustom v-on:click="closeForm" />       
            <Button
                label="Lưu"
                icon="pi pi-check"
                @click="handleSubmit(!v$.$invalid)"
                class="custom-button-h"
                style="margin-bottom: 1px;"
                autofocus
            />                  
        </template>

        <div class="container-fluid mt-4">
            <form @submit.prevent="handleSubmit(!v$.$invalid)" class="p-fluid">
                <div class="row">
                    <div class="col-sm-12 col-md-4 mb-3">
                        <!-- Ngay OT -->
                        <div class="field">
                            <div :class="{ 'form-group--error': v$.data.date.$error }">
                                <label
                                    for="date"
                                    class="label"
                                    :class="{ 'p-error': (v$.data.date.$invalid || !checkday()) && submitted }"
                                    >Ngày OT<b style="color: red">*</b></label
                                >
                                <Calendar
                                    id="date"
                                    v-model="v$.data.date.$model"
                                    :minDate="new Date()"
                                    :showIcon="true"
                                    :date-select="changeDate"
                                    :class="{
                                        'p-invalid': (v$.data.date.$invalid || !checkday()) && submitted, 
                                    }"
                                />
                            </div>
                            <small
                                v-if="(v$.data.date.$invalid && submitted) || v$.data.date.$pending.$response"
                                class="p-error"
                            >
                                Bạn chưa nhập ngày OT
                            </small>
                            <small v-if="!v$.data.date.$invalid && !checkday() && submitted" class="p-error">
                                Ngày OT phải là ngày mai trở đi
                            </small>
                        </div>
                    </div>

                    <div class="col-sm-12 col-md-4 mb-3">
                        <!-- Du an -->
                        <div class="field">
                            <div :class="{ 'form-group--error': v$.data.idProject.$error }">
                                <label
                                    for="idProject"
                                    class="label"
                                    :class="{ 'p-error': v$.data.idProject.$invalid && submitted }"
                                    >Dự án<b style="color: red">*</b></label
                                >
                                <Dropdown
                                    v-model="v$.data.idProject.$model"
                                    :options="project"
                                    optionLabel="projectName"
                                    optionValue="id"
                                    class =" custom-input-h"
                                    :class="{ 'p-invalid': v$.data.idProject.$invalid && submitted } "
                                    :disabled="checkEdit"
                                />
                            </div>
                            <small
                                v-if="
                                    (v$.data.idProject.$invalid && submitted) ||
                                    v$.data.idProject.$pending.$response
                                "
                                class="p-error"
                                >Bạn chưa chon dự án OT</small
                            >
                        </div>
                    </div>

                    <div class="col-sm-12 col-md-4 mb-3">
                        <!-- Edit -->
                        <div class="field" style="margin-bottom: 0" v-if="this.idproject !== null">
                            <div :class="{ 'form-group--error': v$.data.user.$error }">
                                <label
                                    for="user"
                                    class="label"
                                    :class="{ 'p-error': v$.data.user.$invalid && submitted }"
                                    >Người tăng ca</label
                                >
                                <Dropdown
                                    v-model="v$.data.user.$model"
                                    :options="userDropdown"
                                    optionLabel="name"
                                    optionValue="x.id"
                                    placeholder="Chọn nhân viên"
                                    class =" custom-input-h"
                                    :class="{ 'p-invalid': v$.data.user.$invalid && submitted } "
                                    disabled
                                />
                            </div>
                            <small
                                v-if="(v$.data.user.$invalid && submitted) || v$.data.user.$pending.$response"
                                class="p-error"
                                >Bạn chưa chọn người OT!</small
                            >
                        </div>

                        <!-- Add -->
                        <div class="field" style="margin-bottom: 0" v-if="this.idproject === null">
                            <div :class="{ 'form-group--error': v$.data.user.$error }">
                                <label
                                    for="user"
                                    class="label"
                                    :class="{ 'p-error': v$.data.user.$invalid && submitted }"
                                    >Người tăng ca</label
                                >
                                <MultiSelect
                                    v-model="v$.data.user.$model"
                                    :options="userDropdown"
                                    optionLabel="name"
                                    optionValue="x.id"
                                    placeholder="Chọn nhân viên"
                                    class =" custom-input-h"
                                    :class="{ 'p-invalid': v$.data.user.$invalid && submitted } "
                                />
                            </div>
                            <small
                                v-if="(v$.data.user.$invalid && submitted) || v$.data.user.$pending.$response"
                                class="p-error"
                                >Bạn chưa chọn người OT!</small
                            >
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-4 mb-3">
                        <!-- Gio bat dau -->
                        <div class="field" style="margin-bottom: 0">
                            <div :class="{ 'form-group--error': v$.data.start.$error }">
                                <label
                                    for="start"
                                    class="label"
                                    
                                    :class="{
                                        'p-error':
                                            (v$.data.start.$invalid ||
                                                timeError ||
                                                invalidTime1 ||
                                                invalidTime3) &&
                                            submitted,
                                    }"
                                    >Giờ bắt đầu OT<b style="color: red">*</b></label
                                >
                                <InputText
                                    id="start"
                                    v-model="v$.data.start.$model"
                                    type="time"
                                    timeformat="24h"
                                    class =" custom-input-h"
                                    :class="{
                                        'p-invalid':
                                            (submitted && timeError) ||
                                            (submitted && invalidTime1) ||
                                            (submitted && invalidTime3),
                                    }"
                                />
                            </div>
                            <span v-if="v$.data.start.$error && submitted">
                                <span
                                    id="start-error"
                                    v-for="(error, index) of v$.data.start.$errors"
                                    :key="index"
                                >
                                    <small class="p-error">{{
                                        error.$message.replace('Value', 'Start Time') + '. '
                                    }}</small>
                                </span>
                            </span>
                            <small
                                v-else-if="
                                    (v$.data.start.$invalid && submitted) || v$.data.start.$pending.$response
                                "
                                class="p-error"
                                >Bạn chưa nhập giờ bắt đầu OT</small
                            >
                            <small v-if="!v$.data.start.$invalid && invalidTime1 && submitted" class="p-error">
                                Giờ OT phải khác giờ làm việc
                            </small>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-4 mb-3">
                        <!-- Gio ket thuc -->
                        <div class="field">
                            <div :class="{ 'form-group--error': v$.data.end.$error }">
                                <label
                                    for="end"
                                    class="label"
                                    
                                    :class="{
                                        'p-error':
                                            (v$.data.end.$invalid ||
                                                timeError ||
                                                invalidTime2 ||
                                                invalidTime3) &&
                                            submitted,
                                    }"
                                    >Giờ kết thúc OT<b style="color: red">*</b></label
                                >
                                <InputText
                                    id="end"
                                    v-model="v$.data.end.$model"
                                    type="time"
                                    class =" custom-input-h"
                                    :class="{
                                        'p-invalid':
                                            (submitted && timeError) ||
                                            (submitted && invalidTime2) ||
                                            (submitted && invalidTime3),
                                    }"
                                />
                            </div>
                            <small
                                v-if="(v$.data.end.$invalid && submitted) || v$.data.end.$pending.$response"
                                class="p-error"
                                >Bạn chưa nhập giờ kết thúc OT</small
                            >
                            <small v-if="!v$.data.end.$invalid && timeError && submitted" class="p-error">
                                Giờ kết thúc phải lớn hơn giờ bắt đầu OT
                            </small>
                            <small v-if="!v$.data.start.$invalid && invalidTime2 && submitted" class="p-error">
                                Giờ OT phải khác giờ làm việc
                            </small>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-4 mb-3">
                        <!-- Thoi gian thuc te -->
                        <div class="field">
                            <div :class="{ 'form-group--error': v$.data.realTime.$error }">
                                <label
                                    for="realTime"
                                    class="label"
                                    :class="{ 'p-error': v$.data.realTime.$invalid && submitted }"
                                    >Thời gian OT thực tế</label
                                >
                                <InputText
                                    id="realTime"
                                    v-model="v$.data.realTime.$model"
                                    :disabled="true"
                                    class =" custom-input-h"
                                    :class="{ 'p-invalid': v$.data.realTime.$invalid && submitted }"
                                    :value="changeTime"
                                />
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Mota  -->
                <div class="row">
                    <div class="col-sm-12 mt-3">
                        <div class="field">
                            <div
                                class="p-float-label"
                                :class="{ 'form-group--error': v$.data.description.$error }"
                            >
                                <Textarea
                                    id="description"
                                    v-model="v$.data.description.$model"
                                    :showIcon="true"
                                    class =" custom-input-h" 
                                    :class="{ 'p-invalid': v$.data.description.$invalid && submitted }"
                                />
                                <label
                                    for=" description"
                                    :class="{ 'p-error': v$.data.description.$invalid && submitted }"
                                    >Mô tả<b style="color: red">*</b></label
                                >
                            </div>
                            <small
                                v-if="
                                    (v$.data.description.$invalid && submitted) ||
                                    v$.data.description.$pending.$response
                                "
                                class="p-error"
                                >Bạn chưa nhập mô tả</small
                            >
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

<script>
import jwtDecode from 'jwt-decode'
import { UserRoleHelper } from '@/helper/user-role.helper'
import moment from 'moment'
import { required } from '@vuelidate/validators'
import { useVuelidate } from '@vuelidate/core'
import LayoutDefault from '../../layouts/LayoutDefault/LayoutDefault.vue'
import router from '@/router'
import { HTTP } from '@/http-common'
import { DateHelper } from '@/helper/date.helper'
import { LocalStorage } from '@/helper/local-storage.helper'
import { checkAccessModule } from '@/helper/checkAccessModule'
import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
export default {
    setup: () => ({
        v$: useVuelidate(),
    }),
    props: ['display', 'idproject', 'AddOrEdit'],
    data() {
        return {
            data: {
                type: 0,
                date: '',
                start: null,
                end: null,
                realTime: null,
                status: 0,
                description: '',
                leadCreate: null,
                dateCreate: '',
                updateUser: '',
                dateUpdate: '',
                note: '',
                user: null,
                idProject: null,
            },
            userDropdown: [],
            project: [],
            submitted: false,
            time: null,
            valid: true,
            timeError: true,
            invalidTime1: false,
            invalidTime2: false,
            invalidTime3: false,
            leader:[],
            statuses: [
                { num: 0, text: 'Waiting' },
                { num: 1, text: 'Accepted' },
                { num: 2, text: 'Denied' },
                { num: 3, text: 'Deleted' },
            ],
        }
    },
    validations() {
        return {
            data: {
                date: {
                    required,
                },
                start: {
                    required,
                },
                end: {
                    required,
                },
                realTime: {
                    required,
                },
                description: {
                    required,
                },
                user: {
                    required,
                },
                idProject: {
                    required,
                },
            },
        }
    },
    computed: {
        checkEdit() {
            if (this.AddOrEdit) return this.AddOrEdit
        },
        changeTime() {
            if (this.data.start != null && this.data.end != null) {
                let start_hour = parseInt(this.data.start.substring(0, 2))
                let start_minute = parseInt(this.data.start.substring(3, 5))
                let end_hour = parseInt(this.data.end.substring(0, 2))
                let end_minute = parseInt(this.data.end.substring(3, 5))

                if ((8 <= start_hour && start_hour < 12) || (14 <= start_hour && start_hour < 17))
                    this.invalidTime1 = true
                else if ((start_hour == 13 && start_minute >= 30) || (start_hour == 17 && start_minute <= 30))
                    this.invalidTime1 = true
                else this.invalidTime1 = false
                if ((8 < end_hour && end_hour < 12) || (14 <= end_hour && end_hour < 17)) this.invalidTime2 = true
                else if ((end_hour == 13 && end_minute >= 30) || (end_hour == 17 && end_minute <= 30))
                    this.invalidTime2 = true
                else this.invalidTime2 = false
                if ((start_hour < 8 && end_hour >= 12) || (12 <= start_hour && start_hour <= 13 && end_hour > 17))
                    this.invalidTime2 = true
                if (this.data.date != '')
                    if (
                        new Date(moment(this.data.date).format('MM-DD-YYYY')).getDay() == 0 ||
                        new Date(moment(this.data.date).format('MM-DD-YYYY')).getDay() == 6
                    ) {
                        this.invalidTime1 = this.invalidTime2 = false
                    }
                if (start_hour < end_hour) {
                    if (start_minute <= end_minute)
                        if (end_minute - start_minute >= 10)
                            this.time = '0' +end_hour - start_hour + ':' + (end_minute - start_minute)
                        else{
                            this.time = end_hour - start_hour + ':0' + (end_minute - start_minute) 
                            this.time = "0" + this.time
                        } 
                    else if (end_minute + 60 - start_minute >= 10){
                        this.time = '0' + (end_hour - start_hour - 1).toString() + ':' + (end_minute + 60 - start_minute)
                    }      
                    else this.time = end_hour - start_hour - 1 + ':0' + (end_minute + 60 - start_minute)
                    this.data.realTime = this.time
                    this.timeError = false
                    return this.time
                } else if (start_hour == end_hour)
                    if (start_minute < end_minute) {
                        if (end_minute - start_minute >= 10) this.time = '00:' + (end_minute - start_minute)
                        else this.time = '00:0' + (end_minute - start_minute)
                        this.timeError = false
                        this.data.realTime = this.time
                        return this.time
                    }
                this.timeError = true
            }
        },
        changeDate() {
            var today = new Date(new Date().toLocaleDateString('en-EU'))
            var selectedDate = new Date(this.data.date)
            if (selectedDate.getTime() == today.getTime()) {
                let start_hour = parseInt(this.data.start.substring(0, 2))
                let end_hour = parseInt(this.data.end.substring(0, 2))
                if (start_hour <= 8 || end_hour <= 8) {
                    this.invalidTime3 = true
                } else this.invalidTime3 = false
            } else {
                this.invalidTime3 = false
            }
        },
    },
    async mounted() {
        await this.getListProjectByGroup();
        if (this.idproject !== null || this.idproject !== 0) {
            await HTTP.get('OTs/getOTByID/' + this.idproject).then((res) => {
                if (res.status == 200) {
                    this.data = res.data
                    this.data.date = moment(res.data.date).format('MM/DD/YYYY')
                    this.data.date = new Date(new Date(this.data.date).toLocaleDateString('en-EU'))
                }
            })
        } else {
            this.data.start = '00:00'
            this.data.end = '00:00'
        }
    },
     watch: {
        'data.idProject': async function (newId, oldId) {
            if (newId !== null || oldId !== null) {
                await this.getUserByProject(newId)
                await this.getLeadProject(newId);
            }
        },
    },
    methods: {
        showSuccess(mess) {
            this.$toast.add({severity: 'success',summary: 'Thành công',detail: mess,life: 2000,})
        },
        showWarn(err) {
            this.$toast.add({ severity: 'warn', summary: 'Cảnh báo!', detail: err, life: 2000 })
        },
        checkday() {
            this.valid = true
            return true
        },
        async getListProjectByGroup(){
            if(checkAccessModule.isAdmin()){
               await this.getProject()
            }else{
                    if(checkAccessModule.isLead()){
                        await this.getListProjectByLead(checkAccessModule.getUserIdCurrent())
                    }
                    if(checkAccessModule.isPm()){
                        await this.getListProjectByPm(checkAccessModule.getUserIdCurrent())
                    }
            }
            console.log(this.project);
        },
        async getListProjectByLead(idLead){
            await HTTP.get('Project/GetProjectByIdLead/' + idLead).then((res) => {
                if (res.status == 200) var formatDate = DateHelper.formatDate(new Date())
                res.data.forEach((element) => {
                    if (
                        element.isDeleted != true &&
                        element.isFinished != true &&
                        (element.endDate > formatDate || element.endDate == null)
                    ) {
                        this.project.push(element)
                    }
                })
                this.project = this.project.filter((obj, index, self) => {
                     return index === self.findIndex((t) => t.id === obj.id);
                });
            })
        },

        async getListProjectByPm(idPm){
            await HTTP.get('Project/GetProjectByIdPM/' + idPm).then((res) => {
                if (res.status == 200) var formatDate = DateHelper.formatDate(new Date())
                res.data.forEach((element) => {
                    if (
                        element.isDeleted != true &&
                        element.isFinished != true &&
                        (element.endDate > formatDate || element.endDate == null)
                    ) {
                        this.project.push(element)
                    }
                })
                this.project = this.project.filter((obj, index, self) => {
                     return index === self.findIndex((t) => t.id === obj.id);
                });
            })
        },
        async handleSubmit(isFormValid) {
            this.submitted = true
            if (!isFormValid) {
                return
            }
            if (!this.valid || this.timeError || this.invalidTime1 || this.invalidTime2 || this.invalidTime3) return
            await this.addData()
        },
        reloadform() {
            this.data = {
                type: 0,
                date: '',
                start: null,
                end: null,
                realTime: null,
                status: 0,
                description: '',
                leadCreate: null,
                dateCreate: '',
                updateUser: '',
                dateUpdate: '',
                note: '',
                user: null,
                idProject: null,
            }
        },
        async getLeadProject(projectid){
            await HTTP.get("Project/GetLeadByProject/" + projectid).then(rass=>{
                this.leader = rass.data
            }).catch(err => console.log(err))
        },
        async addData() {
            console.log(this.leader);
            this.data.date = moment(new Date(this.data.date)).format('YYYY-MM-DD')
            this.data.dateUpdate = new Date()
            if (this.idproject) {
                await HTTP.put('OTs/updateOT/' + this.idproject, this.data)
                    .then((res) => {
                        if (res.status == 200) {
                            this.showSuccess('Cập nhật OT thành công!')
                            this.reloadform()
                            this.$emit('reloadData')
                            this.$emit('close')
                            this.submitted = false
                        }
                    })
                    .catch((err) => {
                        this.showWarn('Bạn không có quyền thực hiện thao tác này!')
                        console.log(err)
                    })
            }
            if (this.idproject === null) {
                this.data.dateCreate = new Date()
                this.data.leadCreate = this.leader.x.id
                HTTP.post('OTs/AddOTs', this.data)
                    .then((res) => {
                        if (res.status == 200) {
                            this.showSuccess('Thêm mới OT thành công!')
                            this.$emit('reloadData')
                            this.closeForm()
                        }
                    })
                    .catch(() => {
                        this.showWarn('Bạn không có quyền thực hiện thao tác này!')
                    })
            }
        },              
        async getUserByProject(projectid) {
            this.userDropdown = []
            if (projectid !== null) {
                await HTTP.get('Project/UserInProject/' + projectid)
                    .then(async (res) => {
                        await HTTP.get("Project/GetLeadByProject/" + projectid).then(rass=>{
                            const object = {
                                x : rass.data.x,
                                name : rass.data.fullName
                            }      
                            res.data.push(object)    
                        }).catch(err => console.log(err))
                        this.userDropdown = res.data      
                    })
                .catch((err) => console.log(err))
            }
        },
        closeForm() {
            this.$emit('close')
            this.reloadform()
        },
        async getData() {
            if (this.idproject !== null) {
                await HTTP.get('OTs/getOTByID/' + this.idproject).then((res) => {
                    if (res.status == 200) {
                        this.data = res.data
                        this.data.date = moment(res.data.date).format('MM/DD/YYYY')
                        this.data.date = new Date(new Date(this.data.date).toLocaleDateString('en-EU'))
                    }
                })
            }
        },
        async getProject() {
            await HTTP.get('Project/getAllProject').then((res) => {
                if (res.status == 200) var formatDate = DateHelper.formatDate(new Date())
                res.data._Data.forEach((element) => {
                    if (
                        element.isDeleted != true &&
                        element.isFinished != true &&
                        (element.endDate > formatDate || element.endDate == null)
                    ) {
                        this.project.push(element)
                    }
                })
            })
        },
    },
    components: { ButtonCustom },
}
</script>
<style lang="scss" scoped>

</style>
