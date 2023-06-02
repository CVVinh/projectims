<template>
    <Dialog
        :header="projectSelected.id ? 'Cập nhật dự án' : 'Thêm dự án'"
        :visible="isOpenDialog"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '45vw' }"
        :maximizable="true"
        :modal="true"
        :closable="false"
    >
        <div class="container">
            <form ref="formAddProject" @submit.prevent="() => handleSubmit(!v$.$invalid)" class="p-fluid">
                <div class="col-md-12 mb-3">
                    <div class="d-flex align-items-center mt-1">
                        <InputSwitch id="OnGitlab" v-model="isOnGitlab" :disabled="projectSelected.id"/>
                        <label class="ms-2" for="OnGitlab">Dự án trên GitLab</label>                                            
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12 col-md-9 mb-3">
                        <div v-if="isOnGitlab == true" class="field">                          
                            <div class="row">
                                <div class="col-sm-12 col-md-6 col-lg-6">
                                    <label
                                        :class="{ 'p-error': v$.dataProject.projectCode.$invalid && submitted }"
                                    >
                                        Dự án trên GitLab
                                        <b style="color: red">*</b>
                                    </label>
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.dataProject.projectCode.$error }"
                                    >
                                        <Dropdown
                                            v-model="v$.dataProject.projectCode.$model"
                                            :options="projectGit !== null && projectGit !== undefined ? projectGit : []"
                                            optionLabel="name"
                                            optionValue="id"
                                            class="custom-input-h"
                                            placeholder="Chọn dự án"
                                            :filter="true"
                                            :showClear="true"
                                            :class="{ 'p-invalid': v$.dataProject.projectCode.$invalid && submitted }"
                                            :disabled="projectSelected.id"
                                        />
                                    </div>
                                    <small
                                        v-if="
                                            (v$.dataProject.projectCode.$invalid && submitted) ||
                                            v$.dataProject.projectCode.$pending.$response
                                        "
                                        class="p-error"
                                        >
                                        <!-- {{ v$.dataProject.projectCode.required.$message.replace('Value', 'projectGit')}} -->
                                        Tên dự án không được để trống!</small
                                    >
                                </div>                                        
                                <div class="col-sm-12 col-md-6 col-lg-6">
                                    <label
                                        for="projectName"
                                        :class="{'p-error': v$.dataProject.projectName.$invalid && submitted}"
                                    >
                                        Tên dự án
                                        <!-- <b style="color: red">*</b> -->
                                    </label>
                                    <div class="p-float-label" >
                                        <InputText 
                                            class="custom-input-h" 
                                            id="projectName"
                                            v-model="v$.dataProject.projectName.$model"
                                            
                                            :class="{
                                                'p-invalid': v$.dataProject.projectName.$invalid && submitted,
                                            }"
                                        />
                                    </div>
                                </div>                  
                            </div>                           
                        </div>

                        <div v-else class="field">
                            <label
                                for="name"
                                :class="{ 'p-error': v$.dataProject.name.$invalid && submitted }"
                            >
                                Tên dự án<b style="color: red">*</b>
                            </label>
                            <div class="p-float-label" :class="{ 'form-group--error': v$.dataProject.name.$error }">
                                <InputText
                                    id="name"
                                    :disabled="isOnGitlab === true"
                                    v-model="v$.dataProject.name.$model"
                                    class="custom-input-h"
                                    :class="{
                                        input_disabled: isOnGitlab === true,
                                        'p-invalid': v$.dataProject.name.$invalid && submitted,
                                    }"
                                />
                            </div>
                            <span v-if="v$.dataProject.name.$error && submitted">
                                <span
                                    id="name-error"
                                    v-for="(error, index) of v$.dataProject.name.$errors"
                                    :key="index"
                                >
                                    <small class="p-error">
                                        <!-- {{ error.$message.replace('Value', 'Project name') }} -->
                                        Tên dự án không được để trống!
                                    </small>
                                </span>
                            </span>
                            <small
                                v-else-if="
                                    (v$.dataProject.name.$invalid && submitted) ||
                                    v$.dataProject.name.$pending.$response
                                "
                                class="p-error"
                                >
                                <!-- {{ v$.dataProject.name.required.$message.replace('Value', 'Project name') }} -->
                                Tên dự án không được để trống!</small
                            >
                        </div>
                    </div>                 
                    <div class="col-sm-12 col-md-3 mb-3">
                        <div class="field">
                            <label
                                for="projectCode"
                                :class="{ 'p-error': v$.dataProject.projectCode.$invalid && submitted }"
                            >
                                Mã dự án<b style="color: red">*</b>
                            </label>
                            <div
                                class="p-float-label"
                                :class="{ 'form-group--error': v$.dataProject.projectCode.$error }"
                            >
                                <InputText
                                    id="projectCode"
                                    class="custom-input-h"
                                    v-model="v$.dataProject.projectCode.$model"
                                    :disabled="true"
                                    :class="{
                                        input_disabled: isOnGitlab === true,
                                        'p-invalid': v$.dataProject.projectCode.$invalid && submitted,
                                    }"
                                />
                            </div>
                            <span v-if="v$.dataProject.projectCode.$error && submitted">
                                <span
                                    id="projectCode-error"
                                    v-for="(error, index) of v$.dataProject.projectCode.$errors"
                                    :key="index"
                                >
                                    <small class="p-error">
                                        <!-- {{error.$message.replace('Value', 'Project code')}} -->
                                        Mã dự án không được để trống!
                                    </small>
                                </span>
                            </span>
                            <small
                                v-else-if="
                                    (v$.dataProject.projectCode.$invalid && submitted) ||
                                    v$.dataProject.projectCode.$pending.$response
                                "
                                class="p-error"
                                >
                                <!-- {{v$.dataProject.projectCode.required.$message.replace('Value', 'Project code')}} -->
                                Mã dự án không được để trống!
                                </small
                            >
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-6 mb-3">
                        <div class="field">
                            <label
                                for=" startDate"
                                :class="{
                                    'p-error':
                                        (v$.dataProject.startDate.$invalid || !checkStartDate()) &&
                                        submitted &&
                                        isOnGitlab == false,
                                }"
                            >
                                Ngày bắt đầu<b style="color: red">*</b>
                            </label>
                            <div
                                class="p-float-label"
                                :class="{
                                    'form-group--error': v$.dataProject.startDate.$error && isOnGitlab == false,
                                }"
                            >
                                <Calendar
                                    id="startDate"
                                    v-model="v$.dataProject.startDate.$model"
                                    :showIcon="true"
                                    :disabled="isOnGitlab === true"
                                    :class="{
                                        input_disabled: isOnGitlab === true,
                                        'p-invalid':
                                            v$.dataProject.startDate.$invalid /*|| !checkStartDate()*/ &&
                                            submitted &&
                                            isOnGitlab == false,
                                    }"
                                />
                            </div>
                            <small
                                v-if="
                                    (v$.dataProject.startDate.$invalid && submitted && isOnGitlab == false) ||
                                    v$.dataProject.startDate.$pending.$response
                                "
                                class="p-error"
                                >
                                <!-- {{v$.dataProject.startDate.required.$message.replace('Value', 'Start date')}} -->
                                Ngày bắt đầu không được để trống!
                                </small
                            >
                            <!-- <small
                                v-if="
                                    !v$.dataProject.startDate.$invalid &&
                                    !checkStartDate() &&
                                    submitted &&
                                    isOnGitlab == false
                                "
                                class="p-error"
                            >
                                Ngày bắt đầu phải lớn hơn hôm nay!
                            </small> -->
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 mb-3">
                        <div class="field">
                            <label for="endDate">
                                Ngày kết thúc
                            </label>
                            <div class="p-float-label">
                                <Calendar
                                    id="endDate"
                                    v-model="dataProject.endDate"
                                    :showIcon="true"
                                    :disabled="checkDisable()"
                                    :minDate="this.minDate"
                                />
                            </div>
                            <!-- <small
                                v-if="
                                    (v$.dataProject.endDate.$invalid && submitted && isOnGitlab == false) ||
                                    v$.dataProject.endDate.$pending.$response
                                "
                                class="p-error"
                                >{{ v$.dataProject.endDate.required.$message.replace('Value', 'End date') }}</small
                            > -->
                            <!-- <small
                                v-if="
                                    !v$.dataProject.endDate.$invalid &&
                                    !checkEndDate() &&
                                    submitted &&
                                    isOnGitlab == false
                                "
                                class="p-error"
                            >
                                Ngày kết thúc phải lớn hơn ngày bắt đầu!
                            </small> -->
                        </div>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-sm-12 col-md-12">
                        <div class="field">
                            <label
                                for="idLeader"
                                :class="{ 'p-error': v$.dataProject.idLeader.$invalid && submitted }"
                            >
                                Leader<b style="color: red">*</b>
                            </label>
                            <div class="p-float-label" :class="{ 'form-group--error': v$.dataProject.idLeader.$error }">
                                <Dropdown
                                    v-model="v$.dataProject.idLeader.$model"
                                    :options="Optionleader"
                                    optionLabel="fullName"
                                    optionValue="id"
                                    class="custom-input-h"
                                    :filter="true"
                                    :showClear="true"
                                    :class="{ 'p-invalid': v$.dataProject.idLeader.$invalid && submitted }"
                                />
                            </div>
                            <small
                                v-if="
                                    (v$.dataProject.idLeader.$invalid && submitted) ||
                                    v$.dataProject.idLeader.$pending.$response
                                "
                                class="p-error"
                                ><!-- {{ v$.dataProject.idLeader.required.$message.replace('Value', 'Leader') }} -->
                                Leader không được bỏ trống!
                                </small
                            >
                        </div>
                    </div>
                </div>
                
                <div class="row mb-3">
                    <div class="col-sm-12 col-md-12">
                        <div class="field">
                            <label
                                for="description"
                                :class="{ 'p-error': v$.dataProject.description.$invalid && submitted }"
                                >Mô tả</label
                            >
                            <div class="p-float-label" :class="{ 'form-group--error': v$.dataProject.description.$error }">
                                <Textarea
                                    class="custom-input-h"
                                    id="description"
                                    style="height: 150px"
                                    v-model="v$.dataProject.description.$model"
                                    :class="{
                                        'p-invalid': v$.dataProject.description.$invalid && submitted,
                                    }"
                                />
                            </div>
                            <span v-if="v$.dataProject.description.$error && submitted">
                                <span
                                    id="description-error"
                                    v-for="(error, index) of v$.dataProject.description.$errors"
                                    :key="index"
                                >
                                    <small class="p-error">{{ error.$message.replace('Value', 'Description') }}</small>
                                </span>
                            </span>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-12 ">
                        <div class="float-end d-inline-flex">
                            <Button
                                icon="pi pi-times"
                                label="Hủy"
                                v-on:click="onClickCancel()"
                                class="me-2 custom-button-h"
                                style="background-color: white; color: black"
                            />
                            <Button
                                label="Lưu"
                                icon="pi pi-check"
                                type="submit"
                                class="custom-button-h"
                                autofocus
                            />
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

<script>
    import { useVuelidate } from '@vuelidate/core'
    import { maxLength, required } from '@vuelidate/validators'
    import { ProjectDto } from '@/views/Project/Project.dto'
    import { LocalStorage, LocalStorageKey } from '@/helper/local-storage.helper'
    import { DateHelper } from '@/helper/date.helper'
    import { HttpStatus } from '@/config/app.config'
    import { UserService } from '@/service/user.service'
    import { HTTP } from '@/http-common'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
    import ButtonCustomEdit from '@/components/buttons/ButtonCustomEdit.vue'
import { TaskService } from '@/service/task.service'

        export default {
        components: { ButtonCustom, ButtonCustomEdit },
        name: 'DialogAddEdit',
        setup: () => ({
            v$: useVuelidate(),
        }),
        data() {
            return {
                valid: true,
                submitted: false,
                dataProject: new ProjectDto(),
                isOnGitlab: false,
                Optionleader: [],
                checkIdgitLab: null,
                idProjectInternal: null,
                minDate: null,
                dataAssigneeIssue: [],
            }
        },
        async beforeUpdate() {
            this.isOnGitlab = this.projectSelected.isOnGitlab
            await this.getListLeader()
        },
        async updated(){
            if (this.projectSelected.id) {
                if(this.projectSelected.isOnGitlab === true){    
                    this.dataProject = this.projectSelected 
                    this.dataProject.projectCode = Number(this.projectSelected.projectCode)
                    this.dataProject.subProjectCode = this.projectSelected.subProjectCode
                    this.minDate = new Date(this.projectSelected.startDate)
                } else {
                   HTTP.get('Project/getProjectById/' + this.projectSelected.id).then((res) => {
                        if (res.status === HttpStatus.OK) {
                            this.dataProject = res.data
                            this.dataProject.startDate = new Date(res.data.startDate)
                            this.minDate = new Date(res.data.startDate)
                            this.dataProject.endDate = new Date(res.data.endDate)
                            this.isOnGitlab = res.data.isOnGitlab
                            this.dataProject.projectCode = res.data.projectCode
                            this.dataProject.name = res.data.name
                            this.dataProject.subProjectCode = res.data.subProjectCode
                            this.dataProject.idLeader = res.data.leader
                        }
                    })
                }
            }
            else{
                this.getIdProjectInternal()
            }
        },        
        validations() {
            return {
                dataProject: {
                    name: { required },
                    projectCode: { required, maxLength: maxLength(10) },
                    description: { maxLength: maxLength(1000) },
                    startDate: { required },
                    idLeader: { required },
                    projectName: {maxLength: maxLength(100)},
                },
            }
        },
        props: ['isOpenDialog', 'projectSelected', 'user', 'leader', 'projectGit'],
        watch: {
            dataProject: {
                handler: async function Change(newVal) {
                    if (this.isOnGitlab && this.projectGit !== null && this.projectGit !== undefined) {
                        var project = this.projectGit.filter(function (el) {
                            return el.id == newVal.projectCode
                            
                        })
                        if (this.dataProject.projectCode === null) {
                            this.dataProject.description = null
                            this.dataProject.startDate = null
                            this.dataProject.endDate = null
                            this.dataProject.name = null
                        }                  
                        if(project.length > 0) {
                          //  this.dataProject.description = project[0].description
                            this.dataProject.startDate = new Date(project[0].created_at)
                            this.dataProject.endDate = null
                            this.dataProject.name = project[0].name
                        }
                          
                    } else {
                        this.dataProject.name = newVal.name
                    }
                },
                deep: true,
            },

            isOnGitlab: function (newValue, OldValue) {
                this.clearData(newValue)
                if (newValue === false && !this.projectSelected.id) {
                    this.getIdProjectInternal();
                } 
            },

            'dataProject.startDate': function (newValue, OldValue) {
                this.minDate = new Date(newValue)
            },
        },
        methods: {
            async getAllMemberByIdProject(idProject) {
                this.dataAssigneeIssue = []
                await TaskService.getAllMenberProject(idProject)
                .then((res) => {
                    res.data.forEach((el) => {
                        this.dataAssigneeIssue.push({
                            id: el.id,
                            fullName: el.name,
                            created_at: el.created_at,
                            username: el.username,
                        })
                    })
                })
                .catch((err) => {
                    console.log(err)
                })
            },
            clearData(newValue) {
                if (this.isOnGitlab === true) {
                    this.dataProject = new ProjectDto({
                        id: 0,
                        name: '',
                        projectCode: '',
                        subProjectCode: '',
                        projectName: '',
                        description: '',
                        startDate: '',
                        endDate: '',
                        isDeleted: false,
                        isFinished: false,
                        userCreated: '',
                        userUpdate: '',
                        dateCreated: '',
                        dateUpdate: '',
                        userId: 0,
                        idLeader: '',
                        IsOnGitlab: newValue,
                    })
                } else {
                    this.dataProject = new ProjectDto({
                        id: 0,
                        name: '',
                        projectCode: this.idProjectInternal !== null ? this.idProjectInternal : '',
                        subProjectCode: '',
                        projectName: '',
                        description: '',
                        startDate: '',
                        endDate: '',
                        isDeleted: false,
                        isFinished: false,
                        userCreated: '',
                        userUpdate: '',
                        dateCreated: '',
                        dateUpdate: '',
                        userId: 0,
                        idLeader: '',
                        IsOnGitlab: newValue,
                    })
                }
            },
            getIdProjectInternal() {
                HTTP.get('Project/getMaxIdProject')
                    .then((res) => {
                        this.idProjectInternal = res.data
                        this.dataProject.projectCode = res.data
                    })
                    .catch((err) => {
                        console.log(err)
                    })
            },
            async getListLeader() {
                await HTTP.get('/Project/getListLead')
                    .then((res) => {
                        this.Optionleader = res.data
                    })
                    .catch((err) => console.log(err))
            },
            onClickCancel() {
                this.resetForm()
                this.$emit('closeDialog')
            },
            openHandlerAddEdit(){
                this.$emit('closeDialog')
                this.$emit('handlerOpenAddEditProject')
            },
            closeHandlerAddEdit(){
                this.resetForm()
                this.$emit('handlerCloseAddEditProject')
            },
            getAllProject() {
                this.$emit('getAllProject')
            },
            handleSubmit(isFormValid) {
                this.submitted = true
                if (this.v$.$invalid === false) {
                    if (this.projectSelected.id) {
                        this.editData()
                    } else {
                        this.AddData()
                    }
                }
            },
            async AddData() {
                this.openHandlerAddEdit()
                if(this.dataProject.projectName === ''|| this.dataProject.projectName === null || this.dataProject.projectName === undefined){
                    this.dataProject.projectName = this.dataProject.name
                    this.subProjectCode = this.dataProject.projectCode
                } else {
                    this.subProjectCode = this.idProjectInternal               
                }
                const dataSave = {
                    name: this.dataProject.name,
                    projectCode: this.dataProject.projectCode,
                    subProjectCode: this.subProjectCode,
                    projectName: this.dataProject.projectName,
                    description: this.dataProject.description,
                    startDate: DateHelper.convertUTC(this.dataProject.startDate),
                    endDate: this.dataProject.endDate === '' ? new Date() : DateHelper.convertUTC(this.dataProject.endDate),
                    isFinished: this.dataProject.isFinished,
                    isDeleted: this.dataProject.isDeleted,
                    userId: checkAccessModule.getUserIdCurrent(),
                    leader: this.dataProject.idLeader,
                    dateCreated: new Date(),
                    dateUpdate: new Date(),
                    userUpdate: 0,
                    userCreated: checkAccessModule.getUserIdCurrent(),
                    isOnGitlab: this.isOnGitlab,
                }
                if (this.isOnGitlab) {
                    dataSave.endDate = new Date()
                }
                if (dataSave) {
                    var checkProject = this.isOnGitlab;
                    await HTTP.post('Project/addProject', dataSave)
                        .then(async (res) => {
                            if (res.status == 200) {
                                var idSubProject = res.data.id
                                if(checkProject){
                                    //if(res.data.projectCode != res.data.subProjectCode){
                                        await this.getAllMemberByIdProject(res.data.projectCode);
                                        if(this.dataAssigneeIssue.length > 0) {
                                            var addMemberObject = []
                                            this.dataAssigneeIssue.forEach(ele => {
                                                var object = {
                                                    idProject: idSubProject,
                                                    member: ele.id,
                                                    createUser: checkAccessModule.getUserIdCurrent(),
                                                }
                                                addMemberObject.push(object)
                                            })
                                            await HTTP.post('memberProject/addMemberFromGitAsync', addMemberObject)
                                            .then(async res => {})
                                            .finally(() => { this.closeHandlerAddEdit() })
                                        }
                                    //}
                                }
                                this.showSuccess('Thêm mới thành công!')
                                this.getAllProject()
                            } 
                        })
                        .catch((err) => {
                            if(err.response.status === 400 ){
                                this.showError('Dự án đã tồn tại!')
                                this.closeHandlerAddEdit()
                            }
                            console.log(err);
                        })
                }
            },
            editData() { 
                this.openHandlerAddEdit()
                let data = {
                    id: this.dataProject.id,
                    name: this.dataProject.name,
                    projectCode: this.dataProject.projectCode,
                    subProjectCode: this.dataProject.projectCode,
                    projectName: this.dataProject.name,
                    description: this.dataProject.description,
                    startDate: DateHelper.convertUTC(this.dataProject.startDate),
                    endDate: DateHelper.convertUTC(this.dataProject.endDate),
                    isFinished: this.dataProject.isFinished,
                    isDeleted: this.dataProject.isDeleted,
                    userId: this.dataProject.userId,
                    userCreated : this.dataProject.userId,
                    leader: this.dataProject.idLeader,
                    dateUpdate: new Date(),
                    userUpdate: checkAccessModule.getUserIdCurrent(),
                    isOnGitlab: this.isOnGitlab,
                }
                if (this.isOnGitlab) {
                    data.subProjectCode = this.dataProject.subProjectCode
                    data.projectName = this.dataProject.projectName
                   //data.endDate = new Date()
                }
                if (data) {
                    HTTP.put(`Project/updateProject/${data.id}`, data)
                        .then((res) => {
                            switch (res.status) {
                                case HttpStatus.OK:
                                    this.showSuccess('Cập nhật thành công!')
                                    this.getAllProject()
                                    break
                                case HttpStatus.FORBIDDEN:
                                case HttpStatus.UNAUTHORIZED:
                                    this.showError('Bạn không có quyền thực hiện thao tác sửa dự án!')
                                    break
                                default:
                            }
                        })
                        .catch((err) => {
                            this.closeHandlerAddEdit()
                            console.log(err);
                        })
                }
            },
            resetForm() {
                this.dataProject = new ProjectDto()
                this.dataAssigneeIssue = []
                this.Optionleader = []
            },
            showSuccess(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: 'Thêm mới thành công!', life: 2000,})
            },
            showWarn(err) {
                this.$toast.add({severity: 'warn',summary: 'Cảnh báo',detail: err, life: 2000, })
            },
            showError(message) {
                this.$toast.add({severity: 'error', summary: 'Lỗi', detail: message, life: 2000, })
            },
            checkStartDate() {
                if (this.dataProject.startDate < new Date(new Date().toLocaleDateString('en-EU'))) {
                    return (this.valid = true)
                } else {
                    return (this.valid = true)
                }
            },
            checkEndDate() {
                return (this.valid = DateHelper.compareDate(this.dataProject.startDate, '<=', this.dataProject.endDate))
            },
            checkDisable() {
                if (this.isOnGitlab === true || this.minDate === null) {
                    return true
                } else {
                    return false
                }
            },
        },
    }
</script>

<style>
    .select_project_displayNone {
        display: block;
    }
    .input_disabled {
        background: #ececec !important;
    }
    .btn-right {
        float: right;
        /* width: 240px; */
        display: inline-flex;
        /* height: 40px; */
    }
    .p-multiselect.p-component.p-inputwrapper.p-inputwrapper-filled.p-inputwrapper-focus.p-overlay-open {
        width: 100%;
    }
    .p-multiselect.p-component.p-inputwrapper {
        width: 100%;
    }
</style>
<style lang="scss" scoped>
    .p-dropdown-panel {
        width: 33.3%;
        min-width: 33.3%;
    }

    #projectCode {
        border: 1px solid #9ca8b4;
    }
</style>
