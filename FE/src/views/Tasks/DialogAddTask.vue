<template>
    <Dialog
        :header="dataUpdateTask == null ? 'Thêm công việc' : 'Cập nhật nội dung công việc'"
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
            <div class="row">
                <div class="col-sm-12 col-md-12" v-if="dataUpdateTask == null">
                    <div class="row mb-3 mt-3">
                        <div class="col-sm-2 col-md-2"></div>
                        <div class="col-sm-12 col-md-10 d-flex align-items-center">
                            <InputSwitch v-model="mutilateAdd" />
                            <label class="ms-2" for="OnGitlab">Thêm nhiều công việc</label>
                        </div>
                    </div>
                </div>
            </div>
            <form class="row">
                <div class="col-sm-12 col-md-12 mt-2" v-if="mutilateAdd == false">
                    <div class="row mb-3">
                        <div
                            class="col-sm-12 col-md-2 d-flex align-items-center"
                            :class="{ 'p-error': v$.dataTask.title.$invalid && isSubmit }"
                        >
                            <h6>Tên công việc<span style="color: red">*</span></h6>
                        </div>
                        <div class="col-sm-12 col-md-10">
                            <InputText
                                class="p-inputtext-sm custom-button-h"
                                v-model="v$.dataTask.title.$model"
                                type="text"
                                placeholder="Nhập tên công việc..."
                                :class="{ 'p-invalid': v$.dataTask.title.$invalid && isSubmit }"
                            />
                            <small class="p-error" v-if="v$.dataTask.title.required.$invalid && isSubmit">
                                <!-- {{ v$.dataTask.title.required.$message.replace('Value', 'Tên công việc') }} -->
                                Tên công việc không được để trống.
                            </small> <br v-if="this.mesErrTitleProject!=''"/>
                            <small class="p-error" v-if="this.mesErrTitleProject!=''">{{ this.mesErrTitleProject }}</small>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-sm-12 col-md-2 d-flex align-items-center">
                            <h6>Loại công việc</h6>
                        </div>
                        <div class="col-md-10">
                            <Dropdown
                                v-model="v$.dataTask.issue_type.$model"
                                :options="dataTypeIssue"
                                optionLabel="title"
                                optionValue="value"
                                placeholder="Chọn loại công việc"
                                class="p-inputtext-sm custom-input-h"
                                disabled
                            />
                        </div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-sm-12 col-md-2 d-flex align-items-start ">
                            <h6>Mô tả công việc</h6>
                        </div>
                        <div class="col-sm-12 col-md-10">
                            <!-- <vue-simplemde v-model="v$.dataTask.description.$model" /> -->
                            <Editor editorStyle="height: 250px" v-model="v$.dataTask.description.$model" id="content" />
                        </div>
                    </div>
                    <div style="border-bottom: 1px solid #dee2e6" class="mt-3 mb-3"></div>

                    <div class="row mb-3 mt-2">
                        <div class="col-sm-12 col-md-2">
                            <!-- :class="{ 'p-error': v$.dataTask.assignee_id.$invalid && isSubmit }" -->
                            <!-- <span style="color: red" v-if="!project.isOnGitlab">*</span> -->
                            <h6>Người được giao</h6>
                        </div>
                        <div class="col-sm-12 col-md-4">
                            <Dropdown
                                v-model="v$.dataTask.assignee_id.$model"
                                :options="dataAssigneeIssue"
                                filter
                                optionLabel="fullName"
                                optionValue="id"
                                placeholder="Người được giao"
                                class="p-inputtext-sm custom-input-h"
                            />
                        </div>
                        <div class="col-sm-12 col-md-2 mt-2 " >
                            <h6>Ngày kết thúc</h6>
                        </div>
                        <div class="col-sm-12 col-md-4">
                            <Calendar
                                showIcon
                                v-model="v$.dataTask.due_date.$model"
                                dateFormat="yy/mm/dd"
                                class="p-inputtext-sm"
                                placeholder="Ngày kết thúc"
                            />
                        </div>
                    </div>
                    
                    <div class="row">
                        <div
                            class="col-sm-12 col-md-2 "
                            :class="{ 'p-error': v$.dataTask.status.$invalid && isSubmit }"
                        >
                            <h6>Trạng thái<span style="color: red" >*</span></h6>
                        </div>
                        <div class="col-sm-12 col-md-4" v-if="!project.isOnGitlab">
                            <Dropdown
                                v-model="v$.dataTask.status.$model"
                                filter
                                :options="dataLabelIssue"
                                optionLabel="name"
                                class="p-inputtext-sm custom-input-h"
                                optionValue="code"
                                placeholder="Trạng thái công việc"
                                :class="{ 'p-invalid': v$.dataTask.status.$invalid && isSubmit }"
                            />
                            <small class="p-error" v-if="v$.dataTask.status.$invalid && isSubmit">
                                <!-- {{ v$.dataTask.status.required.$message.replace('Value', 'Trạng thái') }} -->
                                Trạng thái công việc không được bỏ trống.
                            </small>
                        </div>
                        <div class="col-sm-12 col-md-4" v-else>
                            <MultiSelect
                                v-model="v$.dataTask.labels.$model"
                                display="chip"
                                :options="dataLabelIssue"
                                optionLabel="name"
                                optionValue="name"
                                placeholder="Trạng thái công việc"
                                class="p-inputtext-sm custom-input-h"
                                :class="{ 'p-invalid': v$.dataTask.labels.$invalid && isSubmit }"
                            />
                            <small class="p-error" v-if="v$.dataTask.labels.$invalid && isSubmit">
                                <!-- {{ v$.dataTask.status.required.$message.replace('Value', 'Trạng thái') }} -->
                                Trạng thái công việc không được bỏ trống.
                            </small>
                        </div>
                        <div class="col-sm-12 col-md-2 mt-2" v-if="dataUpdateTask != null">
                            <h6>Thời gian dự kiến</h6>
                        </div>
                        <div class="col-sm-12 col-md-4" v-if="dataUpdateTask != null">
                            <InputText
                                class="p-inputtext-sm custom-button-h"
                                v-model="v$.dataTask.time_estimate.$model"
                                type="text"
                                placeholder="Nhập thời gian dự kiến..."
                            />
                            <small class="p-error" v-if="this.mesErrTimeEst!=''">{{ mesErrTimeEst }}</small>
                        </div>
                    </div>
                    <div class="row" v-if="dataUpdateTask != null">
                        <div class="col-sm-12 col-md-6">
                        </div>
                        <div class="col-sm-12 col-md-6">
                            <p
                                class="mt-1"
                                style="font-size: 12px; font-style: italic"
                            >
                                <b style="color: red">Lưu ý (*): </b>Nhập đúng theo đinh dạng, Tuần 'w', Ngày 'd', Giờ 'h', Phút 'm', ví dụ:
                                '1d3h30m'
                            </p>
                        </div>
                    </div>
                </div>

                <div class="col-md-12 mt-2" v-if="mutilateAdd">
                    <div class="row mb-3">
                        <div class="col-sm-12 col-md-2 d-flex align-items-center">
                            <h6>Danh sách công việc</h6>
                        </div>
                        <div class="col-sm-12 col-md-10">
                            <input class="form-control p-inputtext-sm" type="file" @change="handleFileUpload" />
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-md-2 d-flex justify-content-end align-items-center"></div>
                        <div class="col-md-10">
                            <small style="font-style: italic" class="mb-3">
                                <span style="color: red">*Lưu ý:</span>
                                kiểm tra file coi đúng định dạng chưa trước khi thêm, file mẫu nằm bên dưới ↓
                            </small>
                            <div class="mt-3">
                                <Panel toggleable class="mt-3">
                                    <div class="m-0">
                                        <small style="font-style: italic" class="mb-3">
                                            <span style="color: red">*Một số lưu ý:</span>
                                        </small>
                                        <div class="ms-3">
                                            <span class="d-block f-italic">1: Tải file template dưới đây.</span>
                                            <span class="d-block f-italic"
                                                >2: Thêm dữ liệu đúng vào các cột của file (không được thay đổi cấu trúc
                                                file template)</span
                                            >
                                            <span class="d-block f-italic"
                                                >3: Cột Due_date có định dạng: yyyy/MM/dd</span
                                            >
                                            <span class="d-block f-italic"
                                                >4: Cột time_estimate nhập theo định dạng, Tuần 'w', Ngày 'd', Giờ 'h', Phút 'm', ví dụ:'1d3h30m'</span
                                            >
                                            <span class="d-block f-italic"
                                                >5: Cột Label tương ứng là label của dự án hiện tại (nếu là dự án trên
                                                gitlab), tham khảo danh sách label tại màn hình tạo (nếu là dự án ngoài
                                                gitlab). Với công việc có nhiều trạng thái cách nhau bởi dấu phẩy ',' .
                                            </span>
                                        </div>
                                    </div>
                                    <div class="mt-3 ms-3">
                                        <a
                                            href="../../../public/download/ExampleTaskFile.xlsx"
                                            download="ExampleTaskFile.xlsx"
                                            class="btn btn-info"
                                        >
                                            <i class="pi pi-download me-1"></i>
                                            Tải xuống tệp tại đây</a
                                        >
                                    </div>
                                </Panel>
                            </div>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        <template #footer>
            <Button
                label="Huỷ"
                icon="pi pi-times"
                class="p-button-secondary p-button-icon custom-button-h"
                @click="closeDialog()"
                style="background: white; color: black"
            ></Button>
            <Button label="Lưu" icon="pi pi-check" class="p-button-primary p-button-icon custom-button-h" @click="submitAddTask()">
            </Button>
        </template>
    </Dialog>
</template>

<script>
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { UserService } from '@/service/user.service'
    import { ProjectService } from '@/service/project.service'
    import { statusTask } from './DataStatusTask'
    import { TaskDto } from './TaskModel.dto'
    import jwt_decode from 'jwt-decode'
    import { DateHelper } from '@/helper/date.helper'
    import { TaskService } from '@/service/task.service'
    import dayjs from 'dayjs'
    import * as XLSX from 'xlsx'
    import { checkAccessModule } from '@/helper/checkAccessModule'

    export default {
        props: ['isOpen', 'selectedProject', 'projectData', 'dataUpdateTask', 'ReloadDataGitLab', 'listDataTask'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                mutilateAdd: false,
                dataTask: new TaskDto(),
                status: statusTask,
                dataTypeIssue: [
                    {
                        id: 1,
                        title: 'issue',
                        value: 'issue',
                    },
                ],
                dataLabelIssue: [],
                dataAssigneeIssue: [],
                project: null,
                defaultPageSize: 100,
                isSubmit: false,
                currenUser: jwt_decode(localStorage.getItem('token')),
                iid_AutoIncrement: 0,
                isFlagOnGit: false,
                mutilateDataTask: [],
                mapDataMuitlate: [],
                mesErrTimeEst: "",
                mesErrTitleProject: "",
            }
        },       
        watch: {
            dataTask: {
                handler: async function change(newVal) {
                    if (newVal.title === "") {
                        this.mesErrTitleProject = "";
                    }
                },
                deep: true,
            },
        },
        async beforeUpdate() {
            this.project = this.projectData
            await this.getAllMemberByIdProject()
            await this.getAllLabelOfProject()
            this.getTotalPageOfIssue()
            this.dataTask.issue_type = 'issue'
            if (this.dataUpdateTask != null) {
                if (!this.dataUpdateTask.isOnGitLab) {
                    this.dataTask.assignee_id = this.dataUpdateTask.assignee
                } else {
                    this.dataTask.assignee_id =
                        this.dataUpdateTask.assignee == null ? null : this.dataUpdateTask.assignee.id
                }
                this.dataTask.idTask = this.dataUpdateTask.idTask
                this.dataTask.issue_type = this.dataUpdateTask.typeTask
                this.dataTask.title = this.dataUpdateTask.taskName
                this.dataTask.description = this.dataUpdateTask.description
                this.isFlagOnGit = this.dataUpdateTask.isOnGitLab
                this.dataTask.time_estimate = this.dataUpdateTask.time_estimate == null ? "" : this.dataUpdateTask.time_estimate
                this.dataTask.total_time_spent = this.dataUpdateTask.total_time_spent
                this.dataTask.status = this.dataUpdateTask.status
                this.dataTask.labels = this.dataUpdateTask.labels
                this.dataTask.iid = this.dataUpdateTask.iid_issue
                this.dataTask.due_date =
                    this.dataUpdateTask.duedate != null ? this.formatDateTime(this.dataUpdateTask.duedate) : null
                this.dataTask.issue_type = 'issue'
            }
        },
        validations() {
            let validate = {
                dataTask: {
                    title: { required },
                    labels: {required},
                    assignee_id: {},
                    status: {},
                    createUser: {},
                    idTask: {},
                    iid: {},
                    description: {},
                    issue_type: {},
                    due_date: {},
                    time_estimate: {},
                    total_time_spent: {},
                    createUser: {},
                },
            }
            if (this.project != null) {
                if (!this.project.isOnGitlab) {
                    validate.dataTask.status = { required }
                }
                if (this.project.isOnGitlab) {
                    validate.dataTask.assignee_id = {}
                }
            }
            if (this.mutilateAdd) {
                validate.dataTask.status = {}
                validate.dataTask.title = {}
                validate.dataTask.assignee_id = {}
            }
            return validate
        },
        methods: {
            loadingToHandlerAddEdit(){
                this.$emit('loadingOpenHandlerAddEdit')
                this.$emit('closeDialogTask')
            },
            compareString(value){
                if(this.listDataTask.length < 0) return false;
                var checkTitle = false;
                for (let i = 0; i < this.listDataTask.length; ++i) {
                    if ((this.listDataTask[i].taskName.trim() === value.trim()) && (value.trim().length === this.listDataTask[i].taskName.trim().length)) {
                        checkTitle = true;
                        break;
                    }
                }
                return checkTitle;
            },
            checkTimeEstimate(){
                if(this.dataTask.time_estimate==""){ return true; }
                else {
                    const time_astimate = this.dataTask.time_estimate.replace(/\s+/g, '');
                    var pattern = /^((?=.*[1-9])\d{1,}['w','m','h','d'])([0-9]{1,}['w','m','h','d'])?([0-9]{1,}['w','m','h','d'])?$/;
                    return pattern.test(time_astimate.trim());
                }
            },
            formatDateTime(date) {
                var dateTime = new Date(date)
                return dayjs(dateTime).format('YYYY-MM-DD')
            },
            async submitAddTask() {
                if (this.mutilateAdd) {
                    await this.handlerAddMutilateTask()
                }
                else {
                    this.isSubmit = true
                    if (!this.v$.$invalid) {
                        
                        let iid_AutoIncrement = ++this.iid_AutoIncrement
                        if (this.dataUpdateTask != null) {
                            if(!this.checkTimeEstimate()){
                                this.mesErrTimeEst = "Thời gian không đúng định dạng!";
                                return;
                            }
                            if (this.project.isOnGitlab) {
                                await this.handlerUpdateTaskAPIGitLab()
                            } else {
                                await this.handlerUpdateTaskAPI()
                            }
                        } else {
                            if(this.compareString(this.dataTask.title)){
                                this.mesErrTitleProject = "Công việc này đã tồn tại!";
                                return
                            }
                            if (this.project.isOnGitlab) {
                                this.isFlagOnGit = true
                                await this.handlerAddTaskOnGitLab(iid_AutoIncrement)
                                this.$forceUpdate()
                            } else {
                                this.isFlagOnGit = false
                                await this.handlerAddTaskOnAPI(iid_AutoIncrement)
                            }
                        }
                        this.resetForm();
                    }
                }
            },
            async handlerUpdateTaskAPI() {
                this.loadingToHandlerAddEdit()
                const objectData = {
                    userUpdated: Number(this.currenUser.id),
                    taskName: this.dataTask.title,
                    description: this.dataTask.description,
                    status: this.dataTask.status,
                    labels: this.dataTask.labels,
                    Duedate: this.dataTask.due_date == null ? null : DateHelper.formatDateTime(this.dataTask.due_date),
                    assignee: this.dataTask.assignee_id,
                    time_estimate: this.dataTask.time_estimate == "" ? null : this.dataTask.time_estimate,
                }
                if (objectData) {
                    await TaskService.updateTaskOnApi(this.dataTask.idTask, objectData)
                        .then((res) => {
                            this.resetForm()
                            this.$emit('ReloadData')
                            this.successMessage('Cập nhât công việc thành công !')
                        })
                        .catch((err) => {
                            this.$emit('loadingCloseHandlerAddEdit')
                            this.ErrorMessage('Cập nhật công việc thất bại có lỗi gì đó sải ra !')
                        })
                }
            },
            async handlerUpdateTaskAPIGitLab() {
                this.loadingToHandlerAddEdit()
                const objectData = {
                    title: this.dataTask.title,
                    description: this.dataTask.description,
                    labels: this.dataTask.labels.length > 0 ? this.dataTask.labels : [],
                    due_date: this.dataTask.due_date == null ? null : DateHelper.formatDateTime(this.dataTask.due_date),
                    assignee_id: this.dataTask.assignee_id,
                }
                if (objectData) {
                    await TaskService.updateTaskOnApiGitLab(this.project.projectCode, this.dataTask.iid, objectData, checkAccessModule.getTokenUserOnGitLab())
                        .then(async (res) => {
                            const time_estimate = this.dataTask.time_estimate.trim()
                            if (time_estimate != "") {
                                await TaskService.updateTaskAddTimeAstimateOnApiGitLab(
                                    this.project.projectCode,
                                    this.dataTask.iid,
                                    time_estimate,
                                    checkAccessModule.getTokenUserOnGitLab(),
                                )
                                .then((res) => {
                                    // this.successMessage('Cập nhât công việc thành công !')
                                })
                                .catch((err) => {
                                    this.$emit('loadingCloseHandlerAddEdit')
                                    this.ErrorMessage('Thêm thời gian dự kiến thất bại trên gitlab có lỗi gì đó xảy ra!')
                                })
                            }
                            this.resetForm();
                            this.$emit('ReloadDataGitLab')
                            this.successMessage('Cập nhât công việc thành công!')
                        })
                        .catch((err) => {
                            this.$emit('loadingCloseHandlerAddEdit')
                            this.ErrorMessage('Cập nhật công việc thất bại trên gitlab có lỗi gì đó xảy ra!')
                            console.log(err);
                        })
                }
                
            },
            handleFileUpload(event) {
                const file = event.target.files[0]
                if (!file.name.endsWith('.xlsx') && !file.name.endsWith('.xls')) {
                    this.WarningMessage('Loại tệp không đúng, vui lòng chọn tệp Excel!')
                    return
                }
                const reader = new FileReader()
                reader.onload = () => {
                    const datas = reader.result
                    const workbook = XLSX.read(datas, { type: 'binary' })
                    const worksheet = workbook.Sheets[workbook.SheetNames[0]]
                    const rows = XLSX.utils.sheet_to_json(worksheet, { header: 1 })

                    const titles = rows[0]
                    const values = rows.slice(1)

                    const data = []
                    values.forEach((row) => {
                        var iid = this.project.isOnGitlab ? ++this.iid_AutoIncrement : null
                        const title = row[titles.indexOf('title')]
                        const description = row[titles.indexOf('description')]
                        const sst = row[titles.indexOf('stt')]
                        const issue_type = 'issue'
                        const due_date = row[titles.indexOf('due_date(yyyy/mm/dd)')]
                        const labels = row[titles.indexOf('labels')]
                        const time_estimate = row[titles.indexOf('time_estimate')]
                        // const labelArray = labels.split(',').map((label) => label.trim())
                        let labelArray = []
                        if (labels && labels.includes(',')) {
                            labelArray = labels.split(',').map((label) => label.trim())
                        } else if (labels) {
                            labelArray.push(labels.trim())
                        }
                        if(sst!=undefined)
                        data.push( {
                                sst,
                                iid: iid,
                                title,
                                description,
                                issue_type,
                                type_issue: issue_type != null ? issue_type.toUpperCase() : null,
                                due_date,
                                labels: labelArray,
                                time_estimate,
                            })
                    })
                    this.mutilateDataTask = data
                }
                reader.readAsBinaryString(file)
            },
            async handlerAddMutilateTask() {
                if (this.mutilateAdd) {
                    this.loadingToHandlerAddEdit()
                    if (this.mutilateDataTask.length > 0) {
                        if (this.project.isOnGitlab) {
                            this.isFlagOnGit = true
                            await this.addMutilateTaskOnAPIGitLab()
                        } else {
                            this.isFlagOnGit = false
                            await this.addMutilateTaskOnAPI()
                        }
                    } else {
                        this.WarningMessage('Chưa có dữ liệu hoặc chưa chọn tệp!')
                    }
                    this.resetForm()
                }
            },
            async addMutilateTaskOnAPIGitLab() {
                if (this.mutilateDataTask.length > 0) {
                    const tasks = this.mutilateDataTask
                    const addTasks = async () => {
                        for (const task of tasks) {
                            const newTask = {
                                iid: task.iid,
                                title: task.title,
                                description: task.description,
                                issue_type: task.issue_type,
                                type_issue: task.type_issue,
                                labels: task.labels.length > 0 ? task.labels : [],
                                due_date: task.due_date == null ? null : DateHelper.formatDateTime(task.due_date),
                            }
                            try {
                                await TaskService.addTaskOnGitLab(this.project.projectCode, newTask, checkAccessModule.getTokenUserOnGitLab())
                                .then(async res => {
                                    if(task.time_estimate!=undefined || task.time_estimate!=null){
                                        await TaskService.updateTaskAddTimeAstimateOnApiGitLab(
                                            this.project.projectCode,
                                            res.data.iid,
                                            task.time_estimate,
                                            checkAccessModule.getTokenUserOnGitLab(),
                                        )
                                        .catch((err) => {
                                            console.log(err);
                                        })
                                    }
                                })
                                .catch((err) => { console.log(err); })
                                //console.log(`Thêm thành công công việc "${newTask.title}" trên GitLab.`)
                            } catch (error) {
                                this.$emit('loadingCloseHandlerAddEdit')
                                this.ErrorMessage(`Thêm công việc "${newTask.title}" thất bại trên GitLab `)
                            }
                        }
                        this.$emit('ReloadDataGitLab')
                        this.successMessage(`Thêm thành công ${tasks.length} công việc trên GitLab!`)
                    }
                    await addTasks();
                }
            },
            async addMutilateTaskOnAPI() {
                let taskArray = this.mutilateDataTask.map((issue) => {
                    return {
                        iid_issue: issue.iid,
                        createUser: Number(this.currenUser.id),
                        idProject: this.selectedProject,
                        taskName: issue.title,
                        description: issue.description,
                        typeTask: issue.issue_type,
                        labels: issue.labels,
                        Duedate: issue.due_date == null ? null : DateHelper.formatDateTime(issue.due_date),
                        isOnGitLab: this.isFlagOnGit,
                    }
                })
                if (taskArray) {
                    await TaskService.mutilateAddTaskOnAPI(this.selectedProject, taskArray)
                        .then((res) => {
                            this.$emit('ReloadData')
                            this.successMessage('Thêm dữ liệu từ tệp thành công')
                        })
                        .catch((err) => {
                            this.$emit('loadingCloseHandlerAddEdit')
                            console.log(err)
                        })
                }
            },
            async handlerAddTaskOnGitLab(iid_AutoIncrement) {
                this.loadingToHandlerAddEdit()
                const objectData = {
                    iid: iid_AutoIncrement,
                    title: this.dataTask.title,
                    description: this.dataTask.description,
                    issue_type: this.dataTask.issue_type,
                    type_issue: this.dataTask.issue_type.toUpperCase(),
                    labels: this.dataTask.labels.length > 0 ? this.dataTask.labels : [],
                    due_date: this.dataTask.due_date == null ? null : DateHelper.formatDateTime(this.dataTask.due_date),
                    assignee_id: this.dataTask.assignee_id,
                }
                if (objectData) {
                    await TaskService.addTaskOnGitLab(this.project.projectCode, objectData, checkAccessModule.getTokenUserOnGitLab())
                        .then((res) => {
                            this.resetForm()
                            this.$emit('ReloadDataGitLab')
                            this.successMessage('Thêm công việc thành công!')
                        })
                        .catch((err) => {
                            this.$emit('loadingCloseHandlerAddEdit')
                            this.WarningMessage("Kiểm tra quyền người dùng trên gitlab!")
                        })
                    this.$forceUpdate()
                }
            },
            async handlerAddTaskOnAPI(iid_AutoIncrement) {
                this.loadingToHandlerAddEdit()
                const objectData = {
                    iid_issue: this.isFlagOnGit == false ? null : iid_AutoIncrement,
                    createUser: Number(this.currenUser.id),
                    idProject: this.selectedProject,
                    taskName: this.dataTask.title,
                    description: this.dataTask.description,
                    typeTask: this.dataTask.issue_type,
                    status: this.dataTask.status,
                    labels: this.dataTask.labels,
                    Duedate: this.dataTask.due_date == null ? null : DateHelper.formatDateTime(this.dataTask.due_date),
                    assignee: this.dataTask.assignee_id,
                    isOnGitLab: this.isFlagOnGit,
                }
                if (objectData) {
                    await TaskService.addTaskOnAPI(objectData)
                        .then((res) => {
                            if (res.data._success) {
                                this.resetForm()
                                this.$emit('ReloadData')
                                this.successMessage('Thêm công việc thành công!')
                            } else {
                                this.WarningMessage(res.data._Message)
                            }
                        })
                        .catch((err) => {
                            this.$emit('loadingCloseHandlerAddEdit')
                            this.WarningMessage('Có lỗi trong quá trình thực hiện!')
                        })
                }
            },
            getTotalPageOfIssue() {
                if (this.project != null) {
                    if (this.project.isOnGitlab == true) {
                        TaskService.getTotalPageGitLab(this.project.projectCode, 100, 1).then((res) => {
                            if (res.data.length > 0) {
                                const totalPages = parseInt(res.headers['x-total-pages'])
                                TaskService.getTotalPageGitLab(this.project.projectCode, 1, totalPages).then((res) => {
                                    this.iid_AutoIncrement = res.data[0].iid
                                })
                            }
                        })
                    }
                }
            },
            async getAllMemberByIdProject() {
                this.dataAssigneeIssue = []
                if (this.project != null) {
                    if (this.projectData.isOnGitlab) {
                        await TaskService.getAllMenberProject(this.projectData.projectCode)
                            .then((res) => {
                                res.data.forEach((el) => {
                                    this.dataAssigneeIssue.push({
                                        id: el.id,
                                        fullName: el.name,
                                        created_at: el.created_at,
                                        username: el.username,
                                        avatar_url: el.avatar_url,
                                    })
                                })
                            })
                            .catch((err) => {
                                console.log(err)
                            })
                    } else {
                        await UserService.getAllUserByIdProject(this.selectedProject).then((res) => {
                            if (res.data._success) {
                                this.dataAssigneeIssue = res.data._Data
                            } else {
                                this.WarningMessage(res.data._message)
                            }
                        })
                    }
                }
            },
            async getAllLabelOnApiGitLab(page) {
                return await ProjectService.getAllLabelsOfProject(
                    Number(this.project.projectCode),
                    this.defaultPageSize,
                    page,
                ).then((res) => res.data)
            },
            async getAllLabelOfProject() {
                if (this.project != null) {
                    if (this.project.isOnGitlab) {
                        let resultCount = 100
                        let results = []
                        let page = 1
                        while (resultCount === 100) {
                            let newResults = await this.getAllLabelOnApiGitLab(page)
                            page++
                            resultCount = newResults.length
                            results = results.concat(newResults)
                        }
                        this.dataLabelIssue = results
                    } else {
                        this.dataLabelIssue = this.status
                    }
                }
            },
            resetForm() {
                this.mutilateAdd = false
                this.isSubmit = false
                this.mesErrTimeEst = ""
                this.dataTask = {
                    title: "",
                    labels: null,
                    assignee_id: null,
                    status: null,
                    createUser: null,
                    idTask: null,
                    iid: null,
                    description: null,
                    issue_type: null,
                    due_date: null,
                    time_estimate: "",
                    total_time_spent: null,
                    createUser: null,
                }
            },
            successMessage(mess) {
                this.$toast.add({severity: 'success',summary: 'Thành công!',detail: mess,life: 2000,})
            },
            WarningMessage(mess) {
                this.$toast.add({severity: 'warn',summary: 'Cảnh báo!',detail: mess,life: 2000,})
            },
            ErrorMessage(mess) {
                this.$toast.add({severity: 'error',summary: 'Lỗi!',detail: mess,life: 2000,})
            },
            closeDialog() {
                this.resetForm()
                this.$emit('closeDialogTask')
            },
        },
    }
</script>
<style lang="scss" scoped>
    .p-dropdown {
        width: 100%;
    } 
    .f-italic {
        font-style: italic;
    }
</style>
