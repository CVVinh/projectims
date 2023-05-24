<template>
    <Toast position="top-right" />
    <Dialog
        header="Cập nhật trạng thái công việc"
        :visible="isOpenUpdateStatusTask"
        :closable="false"
        :modal="true"
        :maximizable="true"
        :dismissableMask="true"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '40vw' }"
    >
        <div class="container">
            <div class="row mb-5 mt-4">
                <div class="col-md-12 col-sm-12">
                    <div class="p-float-label" :class="{ 'form-group--error': v$.dataUpdateStatusTask.taskName.$error }">
                        <InputText
                            id=" taskName"
                            disabled=""
                            v-model="v$.dataUpdateStatusTask.taskName.$model"
                            :class="{ 'p-invalid': v$.dataUpdateStatusTask.taskName.$invalid && submitted }"
                            autocomplete="false"
                            class="custom-input-h"
                        />
                        <label for="taskName" :class="{ 'p-error': v$.dataUpdateStatusTask.taskName.$invalid && submitted }"
                            >Tên công việc<b style="color: red">*</b></label
                        >
                    </div>
                    <span v-if="v$.dataUpdateStatusTask.taskName.$error && submitted">
                        <span id="taskName-error" v-for="(error, index) of v$.dataUpdateStatusTask.taskName.$errors" :key="index">
                            <small class="p-error">{{ error.$message }}</small>
                        </span>
                    </span>
                    <small
                        v-else-if="(v$.dataUpdateStatusTask.taskName.$invalid && submitted) || v$.dataUpdateStatusTask.taskName.$pending.$response"
                        class="p-error"
                        >{{ v$.dataUpdateStatusTask.taskName.required.$message.replace('Value', 'Tên task') }}
                    </small>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-12 mb-5">
                    <div class="p-float-label" :class="{ 'form-group--error': v$.dataUpdateStatusTask.labels.$error }">
                        <MultiSelect
                            id="labels"
                            v-model="v$.dataUpdateStatusTask.labels.$model"
                            :options="this.listLabel"
                            optionLabel="name"
                            optionValue="name"
                            placeholder="Trạng thái công việc"
                            class="p-inputtext-sm custom-input-h"
                            :class="{ 'p-invalid': v$.dataUpdateStatusTask.labels.$invalid && submitted }"
                        />
                        <label for="labels" :class="{ 'p-error': v$.dataUpdateStatusTask.labels.$invalid && submitted }"
                            >Trạng thái công việc<b style="color: red">*</b></label
                        >
                    </div>
                    <small
                        v-if="(v$.dataUpdateStatusTask.labels.$invalid && submitted) || v$.dataUpdateStatusTask.labels.$pending.$response"
                        class="p-error"
                        >{{ v$.dataUpdateStatusTask.labels.required.$message.replace('Value', 'Trạng thái công việc') }}</small
                    >
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <div class="p-float-label" :class="{ 'form-group--error': v$.durationWork.$error }">
                        <InputText
                            id=" time_estimate"
                            v-model="v$.durationWork.$model"
                            :class="{ 'p-invalid': v$.durationWork.$invalid && submitted }"
                            autocomplete="false"
                            class="custom-input-h"
                        />
                        <label for="time_estimate" :class="{ 'p-error': v$.durationWork.$invalid && submitted }"
                            >Thời gian làm</label
                        >
                    </div>
                    <span v-if="v$.durationWork.$error && submitted">
                        <span id="time_estimate-error" v-for="(error, index) of v$.durationWork.$errors" :key="index">
                            <small class="p-error">{{ error.$message }}</small>
                        </span>
                    </span>
                    <small
                        v-else-if="(v$.durationWork.$invalid && submitted) || v$.durationWork.$pending.$response"
                        class="p-error"
                        >Thời gian làm việc là được yêu cầu
                    </small>
                    <small class="p-error" v-if="this.mesErrTimeSpend!=''">{{ this.mesErrTimeSpend }}</small>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-12 mb-5">
                    <p
                        class="mt-1"
                        style="font-size: 13px; font-style: italic"
                    >
                        <b style="color: red">Lưu ý (*): </b>Nhập đúng theo đinh dạng, Tuần 'w', Ngày 'd', Giờ 'h', Phút 'm', ví dụ:'1d3h30m'
                    </p>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-12 mb-2">
                    <div class="p-float-label" :class="{ 'form-group--error': v$.contentWork.$error }">
                        <h6>Nội dung công việc</h6>
                        <Editor
                            editorStyle="height: 250px"
                            v-model="v$.contentWork.$model"
                            :class="{ 'p-invalid': v$.contentWork.$invalid && submitted }"
                        />
                    </div>
                    <small v-if="this.objectCommentToWork!=null"><b><a href="#" @click="deleteCommentToWork()">Xoá</a></b> nội dung công việc này!</small> <br v-if="this.objectCommentToWork!=null"/>
                    <small
                        v-if="(v$.contentWork.$invalid && submitted) || v$.contentWork.$pending.$response"
                        class="p-error"
                        >Nội dung công việc là được yêu cầu
                    </small>
                </div>
            </div>

            <div class="row mb-4">
                <div class="col col-md-12 col-24">
                    <div class="d-flex justify-content-end">
                        <div class="btn-right">
                            <button-custom class="me-2" @click="closeUpdateStatusTask()" />
                            <Button
                                label="Lưu"
                                icon="pi pi-check"
                                class="custom-button-h"
                                autofocus
                                @click="handlerSubmit()"
                            />
                        </div>
                    </div>                 
                </div>
            </div>
        </div>
    </Dialog>
</template>

<script>
    import { required, alphaNum, numeric, between, minLength, maxLength } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { HTTP, HTTP_API_GITLAB, HTTP_LOCAL } from '@/http-common'
    import jwt_decode from 'jwt-decode'
    import { HttpStatus } from '@/config/app.config'
    import { DateHelper } from '@/helper/date.helper'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import EditorCustom from '@/components/buttons/EditorCustom.vue'
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
    import { TaskService } from '@/service/task.service'
    import { UserService } from '@/service/user.service'

    export default {
        name: 'UpdateStatusTask',
        props: ['isOpenUpdateStatusTask', 'listLabel', 'dataTask'],
        setup: () => ({
            v$: useVuelidate(),
        }),
        data() {
            return {
                dataUpdateStatusTask: {
                    taskName: null,
                    labels: [],
                },
                durationWork: "",
                contentWork: "",
                objectCommentToWork: null,
                infoUserCurrent: checkAccessModule.getTokenUserOnGitLab(),
                submitted: false,
                mesErrTimeSpend: "",
            }
        },
        validations() {
            return {
                dataUpdateStatusTask: {
                    taskName: { required },
                    labels: { required },
                },
                durationWork: {},
                contentWork: {},
            }
        },
        async beforeUpdate() {
            this.objectCommentToWork = null;
            this.contentWork = "";
            this.durationWork = "";    
            this.dataUpdateStatusTask = this.dataTask;
            if(this.dataUpdateStatusTask!=null){
                if(this.dataUpdateStatusTask.labels==null || this.dataUpdateStatusTask.labels.length <= 0 ){
                    this.dataUpdateStatusTask.arrayLabel.map(ele => {
                        this.dataUpdateStatusTask.labels.push(ele.name);
                    })
                }
                await this.getAllCommentToWork();
            }
        },
        watch: {
            durationWork: {
                handler: async function change(newValue) {
                    if(newValue=="") {
                        this.mesErrTimeSpend = ""
                    }
                },
                deep: true,
            },
        },
        methods: {
            closeUpdateStatusTask() {
                this.resetForm();
                this.$emit('closeUpdateStatusTask');
            },
            handlerSubmit() {
                try {
                    var checkSubmit = true;
                    if(this.durationWork!=""){
                        if(!this.checkTimeSpend()){
                            this.mesErrTimeSpend = "Thời gian không đúng định dạng!"
                            checkSubmit = false;
                        }
                    }
                    this.submitted = true;
                    if (this.v$.$invalid === false && checkSubmit) {
                        this.submitForm();
                        this.closeUpdateStatusTask();
                    }
                } catch (err) {
                    console.log(err);
                }
            },
            resetForm() {
                this.dataUpdateStatusTask.taskName = "";
                this.dataUpdateStatusTask.label = [];
                this.durationWork = "";
                this.objectCommentToWork = null;
                this.contentWork = "";
            },
            showError(message) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 });
            },
            showSuccess(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 });
            },
            showInfo(message) {
                this.$toast.add({ severity: 'info', summary: 'Thông báo', detail: message, life: 3000 });
            },
            showResponseApi(status, message = '') {
                switch (status) {
                    case 401:
                    case 403:
                        this.showError('Bạn không có quyền thực hiện chức năng này!');
                        break

                    case 404:
                        this.showError('Lỗi! Load dữ liệu!');
                        break

                    default:
                        if (message != '') {
                            this.showError(message);
                        } else {
                            this.showError('Có lỗi trong quá trình thực hiện!');
                        }
                        break
                }
            },
            async getAllCommentToWork() {
                try{
                    await TaskService.getAllCommentToWork(this.dataTask.idProject, this.dataTask.iid_issue, this.infoUserCurrent)
                    .then((res) => {
                        res.data.map(ele => {
                            if(ele.system == false){
                                if(DateHelper.formatDayMonthYearjs(ele.created_at) == DateHelper.formatDayMonthYearjs(new Date())){
                                    this.objectCommentToWork = ele;
                                    this.contentWork = ele.body;
                                }
                            }
                        });
                    })
                    .catch((err) => {
                        console.log(err);
                    });
                }
                catch (error) {
                    switch (error.code) {
                        case 'ERR_NETWORK':
                            this.showInfo('Kiểm tra kêt nối!');
                            break;
                        case 'ERR_BAD_REQUEST':
                            console.log(error.response.data);
                            break;
                        default:
                    }
                }
            },
            checkTimeSpend(){
                const time_spend = this.durationWork.replace(/\s+/g, '');
                var pattern = /^((?=.*[1-9])\d{1,}['w','m','h','d'])([0-9]{1,}['w','m','h','d'])?([0-9]{1,}['w','m','h','d'])?$/;
                return pattern.test(time_spend.trim());
            },
            async submitForm() {
                try {
                    var timeSpent = this.durationWork;
                    var objectComment = {
                        body: this.contentWork,
                    }
                    if(this.contentWork=="") {
                        objectComment = null;
                    }
                    if(this.objectCommentToWork!=null && objectComment!=null){
                        await TaskService.updateCommentToWork(this.dataTask.idProject, this.dataTask.iid_issue,this.objectCommentToWork.id, objectComment, this.infoUserCurrent)
                    }
                    else {
                        var objectData = {
                            labels: this.dataTask.labels.length > 0 ? this.dataTask.labels : [],
                        }
                        if (objectData) {
                            await TaskService.updateTaskOnApiGitLab(this.dataTask.idProject, this.dataTask.iid_issue, objectData, this.infoUserCurrent)
                            .then(async (res) => {
                                if(objectComment!=null){
                                    await TaskService.addCommentToWork(this.dataTask.idProject, this.dataTask.iid_issue, objectComment, this.infoUserCurrent)
                                }
                            })
                            .catch((err) => {
                                switch (err.code) {
                                    case 'ERR_NETWORK':
                                        this.showInfo('Kiểm tra kêt nối!');
                                        break;
                                    case 'ERR_BAD_REQUEST':
                                        this.showInfo("Lỗi! Người dùng có thể chưa có trong dự án trên git! Hoặc một lỗi khác!");
                                        break;
                                    default:
                                }
                            });
                        }
                    }
                    if(timeSpent!="") {
                        await TaskService.addTimeSpentToWork(this.dataTask.idProject, this.dataTask.iid_issue, timeSpent, this.infoUserCurrent)
                        .then((res) =>{
                        })
                        .catch((err) =>{console.log(err);});
                    }
                    this.showSuccess('Cập nhât công việc thành công!');
                    this.$emit('ReloadDataGitLab');
                    this.closeUpdateStatusTask();
                } catch (error) {
                    switch (error.code) {
                        case 'ERR_NETWORK':
                            this.showInfo('Kiểm tra kêt nối!');
                            break;
                        case 'ERR_BAD_REQUEST':
                            console.log(error.response.data);
                            break;
                        default:
                    }
                }
            },
            async deleteCommentToWork() {
                await TaskService.deleteCommentToWork(this.dataTask.idProject, this.dataTask.iid_issue,this.objectCommentToWork.id, this.infoUserCurrent)
                .then(async (res) => {
                    this.$emit('ReloadDataGitLab');
                    this.showSuccess('Xoá comment thành công!');
                    this.closeUpdateStatusTask();
                })
                .catch((err) => {
                    console.log(err);
                });
            },

        },
        components: { EditorCustom, ButtonCustom},
    }
</script>
<style lang="scss" scope>
    
    
</style>
