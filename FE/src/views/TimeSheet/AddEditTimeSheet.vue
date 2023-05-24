<template>
    <Dialog
        :header="dataTimeSheetById == null ? 'Thêm việc hôm nay' : 'Cập nhật việc làm hằng ngày'"
        :visible="statusopen"
        :closable="false"
        :maximizable="true"
        modal
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        style="width: 100%; height: 100%"
    >
        <div class="container-fluid">
            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label
                                for="timeSheetInputDate"
                                :class="{
                                    'p-error':
                                        v$.timeSheetInputDate.required.$invalid && isSubmit && checkDayInputTimeSheet,
                                    'input-title': true,
                                }"
                                >Ngày làm việc<b style="color: red">*</b></label
                            >
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <Calendar
                                :disabled="dataTimeSheetById != null"
                                id="timeSheetInputDate"                              
                                :showIcon="true"
                                v-model="v$.timeSheetInputDate.$model"
                                :class="{
                                    'p-invalid':
                                        v$.timeSheetInputDate.required.$invalid && isSubmit && checkDayInputTimeSheet,
                                }"
                            />
                            <small class="p-error" v-if="v$.timeSheetInputDate.required.$invalid && isSubmit"
                                >Ngày nhập time sheet không rỗng!
                            </small>
                            <small class="p-error" v-if="checkDayInputTimeSheet">{{ errorCheckDay }} </small>
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="row">
                            <div class="col-sm-12 col-md-12 col-lg-12">
                                <label
                                    for="idProject"
                                    :class="{
                                        'p-error': v$.selectedProject.required.$invalid && isSubmit,
                                        'input-title': true,
                                    }"
                                    >Tên dự án<b style="color: red">*</b></label
                                >
                            </div>
                        </div>
                        <div class="row mb-3">
                            <div class="col-sm-12 col-md-12 col-lg-12">
                                <Dropdown
                                    disabled
                                    v-model="selectedProject"
                                    :options="listProjectUsers"
                                    filter
                                    optionLabel="name"
                                    optionValue="projectCode"
                                    placeholder="Chọn dự án"
                                    style="width: 100%"
                                    :class="{ 'p-invalid': v$.selectedProject.required.$invalid && isSubmit }"
                                    class="custom-input-h"
                                />
                                <small v-if="v$.selectedProject.required.$invalid && isSubmit" class="p-error"
                                    >Chọn dự án!</small
                                >
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12 col-md-12 col-lg-12 p-fluid">
                    <DataTable
                        showGridlines
                        removableSort
                        :loading="isLoading"
                        :value="dataSend"
                        :sort="1"
                        responsiveLayout="scroll"
                        filterDisplay="menu"
                        :rowHover="true"
                        rowGroupMode="rowspan"
                        sortMode="single"
                        :sortOrder="1"
                        resizableColumns
                        columnResizeMode="expand"
                        editMode="cell"
                        @cell-edit-complete="onCellEditComplete"
                        tableClass="editable-cells-table"
                    >
                        <template #header>
                            <div style="text-align: left">
                                <Button icon="pi pi-plus" class="p-button custom-button-h" @click="addWork()" v-tooltip.top="'Thêm công việc'"></Button>
                            </div>
                        </template>
                        <template #loading> </template>

                        <template #empty>
                            <div v-if="this.isLoading" style="display: flex; justify-items: flex-end">
                                <ProgressSpinner style="width: 42px" />
                            </div>
                        </template>

                        <Column field="#" header="#" dataType="date" style="width: 2%">
                            <template #body="{ index }">
                                {{ index+1 }}
                            </template>
                        </Column>

                        <Column field="taskContent" header="Nội dung" style="width: 33%">
                            <template #editor="{ data }">
                                <InputText
                                    v-model="data.taskContent"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="layout" header="Layout" style="width: 5%">
                            <template #editor="{ data }">
                                <input
                                    type="number"
                                    v-model="data.layout"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="spec" header="Spec" style="width: 5%">
                            <template #editor="{ data }">
                                <input
                                    type="number"
                                    v-model="data.spec"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="api" header="API" style="width: 5%">
                            <template #editor="{ data }">
                                <input
                                    type="number"
                                    v-model="data.api"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="web" header="WEB" style="width: 5%">
                            <template #editor="{ data }">
                                <input
                                    type="number"
                                    v-model="data.web"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="utc" header="UTC" style="width: 5%">
                            <template #editor="{ data }">
                                <input
                                    type="number"
                                    v-model="data.utc"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="ute" header="UTE" style="width: 5%">
                            <template #editor="{ data }">
                                <input
                                    type="number"
                                    v-model="data.ute"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="integration" header="Integration" style="width: 5%">
                            <template #editor="{ data }">
                                <input
                                    type="number"
                                    v-model="data.integration"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="deployment" header="Deployment" style="width: 5%">
                            <template #editor="{ data }">
                                <input
                                    type="number"
                                    v-model="data.deployment"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="fixbug" header="Fixbug" style="width: 5%">
                            <template #editor="{ data }">
                                <input
                                    type="number"
                                    v-model="data.fixbug"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="support" header="Support" style="width: 5%">
                            <template #editor="{ data }">
                                <input
                                    type="number"
                                    v-model="data.support"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="others" header="Others" style="width: 5%">
                            <template #editor="{ data }">
                                <input 
                                    type="number"
                                    v-model="data.others"
                                    :min="0"
                                    :max="24"
                                    autofocus
                                    :class="{ invalid: isOverValued }"
                                    @focus="onfocusInput"
                                    @blur="onBlurInput"
                                />
                            </template>
                        </Column>

                        <Column field="sum" header="Sum" sortable style="width: 5%">
                            <template #body="{ data }">
                                {{ data.sum }}
                            </template>
                        </Column>

                        <ColumnGroup type="footer">
                            <Row>
                                <Column footer="Tổng:" :colspan="13" footerStyle="text-align:right" />
                                <Column :footer="this.calculateDataSend" />
                                <Column />
                            </Row>
                        </ColumnGroup>

                        <Column field="date" header="" style="width: 5%">
                            <template #body="{ index }">
                                <Button
                                    :disabled="this.dataSend.length <= 1"
                                    icon="pi pi-minus"
                                    class="p-button p-button-danger custom-button-h"
                                    @click="removeRowFromTable(index)"
                                    v-tooltip.top="'Xoá công việc này'"
                                ></Button>
                            </template>
                        </Column>
                    </DataTable>
                    <small class="p-error" v-if="isOverValued">{{
                        this.errorEmptyTaskContent + this.errorEqualContent + this.errorEmptyTimeWork + this.errorEmptyOrOverTime
                    }}</small>
                </div>
            </div>
        </div>
        <template #footer>
            <button-custom class="me-2" @click="closeModal()" />
            <Button
                label="Lưu"
                icon="pi pi-check"
                class="p-button-primary p-button-icon custom-button-h"
                @click="handleSubmit()"
                :disabled="this.onCellEditCompleteBtn"
            ></Button>
        </template>
    </Dialog>
</template>

<script>
import { HTTP, HTTP_LOCAL } from '@/http-common'
import { useVuelidate } from '@vuelidate/core'
import { required } from '@vuelidate/validators'
import jwt_decode from 'jwt-decode'
import { DateHelper } from '@/helper/date.helper'
import { LocalStorage } from '@/helper/local-storage.helper'
import { HttpStatus } from '@/config/app.config'
import { checkAccessModule } from '@/helper/checkAccessModule'
import { NO } from '@vue/shared'
import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
export default {
    name: 'AddEditTimeSheet',
    components: { ButtonCustom },
    props: ['statusopen', 'dataTimeSheetById', 'listProjectUsers','projectSelectedToAdd',  ],
    setup: () => ({ v$: useVuelidate() }),
    data() {
        return {
            dataSend: [],
            isLoading: false,
            timeSheetInputDate: new Date(),
            selectedProject: null,
            isSubmit: false,
            onCellEditCompleteBtn: false,
            isOverValued: false,
            sumWorkToday: 0,
            errorEmptyOrOverTime: '',
            errorEmptyTaskContent: '',
            errorEmptyTimeWork: '',
            errorEmptyOrNoTaskName: false,
            errorCheckDay: '',
            errorEqualContent: '',
            errorIsEqualContent: false,
        }
    },
    validations() {
        return {
            selectedProject: { required },
            timeSheetInputDate: { required },
        }
    },
    computed: {        
        calculateDataSend() {
            var result = 0;
            this.sumWorkToday = 0;
            this.dataSend.forEach((element) => {
                element.sum = this.calculateFieldSum(element);
                result += element.sum;
            })
            var checkSum = this.dataSend.some((s) => s.sum == 0);
            var checkContent = this.dataSend.some((s) => s.taskContent.trim().length <= 0);
            if (checkSum) {
                this.sumWorkToday = 0;
            } else {
                this.sumWorkToday = result;
            }
            if (checkContent) {
                this.errorEmptyOrNoTaskName = true;
            } else {
                this.errorEmptyOrNoTaskName = false;
            }
            this.compareString(this.dataSend, this.dataSend.length)
            return result;
        },
        async checkDayInputTimeSheet() {
            var getDay = this.timeSheetInputDate.getDay();
            var today = DateHelper.formatDayMonthYearjs(new Date());
            var formatDate = DateHelper.formatDayMonthYearjs(this.timeSheetInputDate).toString();
            var checkDate = false;
            this.errorCheckDay = '';
           
            if (getDay == 0 || getDay == 6) {
                this.errorCheckDay = 'Không nhập thứ 7, chủ nhật!';
                return true;
            } else if (formatDate > today) {
                this.errorCheckDay = 'Không nhập công việc cho ngày mai!';
                checkDate = true;
            } else if (formatDate < today) {
                this.errorCheckDay = 'Không nhập công việc ngày hôm qua!';
                checkDate = true;
            } else if (formatDate === today){
                if (this.selectedProject != null) {
                    await HTTP.post(
                        `TimeSheetDaily/checkToDay?date=${formatDate}&idUser=${checkAccessModule.getUserIdCurrent()}&idProject=${
                            this.selectedProject
                        }`,
                    )
                        .then((res) => {
                            if (res.status == 200) {
                                if (this.dataTimeSheetById == null) {
                                    this.errorCheckDay = 'Ngày này đã nhập timesheet rồi!';
                                    checkDate = true;
                                }
                            } else {
                                checkDate = false;
                            }
                        })
                        .catch((err) => {
                            checkDate = false;
                        })
                }
            }
            return checkDate;
        },
    },
    watch: {
        errorEmptyOrNoTaskName: {
            handler: async function change(newVal) {
                if (newVal == false) {
                    this.errorEmptyTaskContent = '';
                }
            },
            deep: true,
        },
        errorIsEqualContent: {
            handler: async function change(newVal) {
                if (newVal == false) {
                    this.errorEqualContent = '';
                }
            },
            deep: true,
        },
        sumWorkToday: {
            handler: async function change(newVal) {
                if (newVal <= 8) {
                    this.errorEmptyOrOverTime = '';
                }
                if (newVal != 0) {
                    this.errorEmptyTimeWork = '';
                }
            },
            deep: true,
        },
    },

    async beforeUpdate() {
        await this.clearform();
        if (this.dataTimeSheetById != null) {
            this.isLoading = true;
            this.selectedProject = this.dataTimeSheetById.idProject;
            this.timeSheetInputDate = new Date(this.dataTimeSheetById.dateInputTimeSheet);
            var getByDateDto = {
                idUser: this.dataTimeSheetById.idUser,
                date: this.dataTimeSheetById.date,
                idProject: this.dataTimeSheetById.idProject,
            }
            await HTTP.post(`TimeSheetDaily/GetByDateTimeSheetDaily`, getByDateDto)
                .then((res) => {
                    this.dataSend = res.data._Data;
                })
                .catch((err) => {
                    console.log(err);
                })
                .finally(() => {
                    this.isLoading = false;
                })
        }
        else {
            this.selectedProject = this.projectSelectedToAdd;
        }
    },

    methods: {
        compareString(array, size){
            this.errorIsEqualContent = false;
            for (let i = 0; i < size - 1; ++i) {
                for (let j = i + 1; j < size; ++j) {
                    if ((array[i].taskContent.trim() === array[j].taskContent.trim()) && (array[i].taskContent.trim().length === array[j].taskContent.trim().length)) {
                        this.errorIsEqualContent = true;
                        break;
                    }
                }
            }
        },
        onfocusInput(event) {
            this.onCellEditCompleteBtn = true;
        },
        onBlurInput(event) {
            this.onCellEditCompleteBtn = false;
        },
        async reloadPage() {
            this.$emit('reloadPageListTimeSheet', { date: this.timeSheetInputDate, project: this.selectedProject });
        },
        loadingToHandlerAddEdit() {
            this.$emit('loadingToHandlerAddEdit');
        },
        closeModal() {
            this.timeSheetInputDate = new Date();
            this.$emit('closeAddEdit');
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
        addWork() {
            var addObj = {
                taskContent: '',
                layout: 0,
                spec: 0,
                api: 0,
                web: 0,
                utc: 0,
                ute: 0,
                integration: 0,
                deployment: 0,
                fixbug: 0,
                support: 0,
                others: 0,
                sum: 0,
            }
            this.dataSend.push(addObj);
        },
        removeRowFromTable(index) {
            if (this.dataSend.length > 1) {
                this.dataSend.splice(index, 1);
            }
        },
        onCellEditComplete(event) {
            let { data, newValue, field } = event
            switch (field) {
                case 'taskContent':
                    if (newValue.trim().length > 0) {
                        data[field] = newValue
                    }
                    break
                case 'layout':
                case 'spec':
                case 'api':
                case 'web':
                case 'utc':
                case 'ute':
                case 'integration':
                case 'deployment':
                case 'fixbug':
                case 'support':
                case 'others':
                    if (newValue != null) {
                        if (newValue != 0 && newValue > 0 && newValue < 25) {
                            data[field] = newValue
                        }
                        else {
                            data[field] = 0
                        }
                    } else {
                        data[field] = 0
                    }
                    break;

                default:
                    if (newValue.trim().length > 0) {
                        data[field] = newValue;
                    } else {
                        event.preventDefault();
                    }
                    break;
            }
        },
        calculateFieldSum(item) {
            var result = 0
            if (item.layout != null) {
                result += item.layout
            }
            if (item.spec != null) {
                result += item.spec
            }
            if (item.api != null) {
                result += item.api
            }
            if (item.web != null) {
                result += item.web
            }
            if (item.utc != null) {
                result += item.utc
            }
            if (item.ute != null) {
                result += item.ute
            }
            if (item.integration != null) {
                result += item.integration
            }
            if (item.deployment != null) {
                result += item.deployment
            }
            if (item.fixbug != null) {
                result += item.fixbug
            }
            if (item.support != null) {
                result += item.support
            }
            if (item.others != null) {
                result += item.others
            }
            return result
        },
        async clearform() {
            this.dataSend = [];
            this.addWork();
            this.isSubmit = false;
            this.selectedProject = null;
            this.timeSheetInputDate = new Date();
            this.isOverValued = false;
            this.sumWorkToday = 0;
            this.errorEmptyOrOverTime = '';
            this.errorEmptyTaskContent = '';
            this.errorEmptyTimeWork = '';
            this.errorEmptyOrNoTaskName = false;
            this.errorEqualContent = '';
            this.errorIsEqualContent = false;

        },
        async callApi(addOrEdit, fromData, checkShowNoti) {
            try {
                this.loadingToHandlerAddEdit();
                switch (addOrEdit) {
                    case 'Edit':
                        await HTTP.put(`TimeSheetDaily/UpdateMultiTimeSheetDaily`, fromData).then(async (response) => {
                            switch (response.status) {
                                case HttpStatus.OK:
                                    if(checkShowNoti) this.showSuccess('Cập nhật thành công!');
                                    break;
                                case HttpStatus.UNAUTHORIZED:
                                case HttpStatus.FORBIDDEN:
                                    this.showError('Không có quyền thực hiện thao tác này!');
                                    break;
                                default:
                                    this.showError('Lưu lỗi!');
                            }
                        })
                        break;
                    
                    case 'Add':
                        await HTTP.post(`TimeSheetDaily/CreateMultiTimeSheetDaily`, fromData).then(async (response) => {
                            switch (response.status) {
                                case HttpStatus.OK:
                                if(checkShowNoti) this.showSuccess('Thêm thành công!');
                                    break;
                                case HttpStatus.UNAUTHORIZED:
                                case HttpStatus.FORBIDDEN:
                                    this.showError('Không có quyền thực hiện thao tác này!');
                                    break;
                                default:
                                    this.showError('Lưu lỗi!');
                            }
                        })
                        break;

                    default:
                        break;
                }
                await this.reloadPage();
            } catch (error) {
                switch (error.code) {
                    case 'ERR_NETWORK':
                        this.showError('Kiểm tra kết nối!');
                        break;
                    case 'ERR_BAD_REQUEST':
                        console.log(error.response.data);
                        break;
                    default:
                        break;
                }
            }
        },
        addTimeSheetDto(data, idUserCureent) {
            const objData = {
                idUser: idUserCureent,
                idProject: this.selectedProject,
                date: DateHelper.formatDayMonthYearjs(this.timeSheetInputDate),
                taskContent: data.taskContent.trim(),
                layout: data.layout,
                spec: data.spec,
                api: data.api,
                web: data.web,
                utc: data.utc,
                ute: data.ute,
                integration: data.integration,
                deployment: data.deployment,
                fixbug: data.fixbug,
                support: data.support,
                others: data.others,
                sum: data.sum,
                userCreated: idUserCureent,
            }
            return objData;
        },
        editTimeSheetDto(data, idUserCureent) {
            const objData = {
                id: data.id,
                idUser: idUserCureent,
                idProject: this.selectedProject,
                date: DateHelper.formatDayMonthYearjs(this.timeSheetInputDate),
                taskContent: data.taskContent.trim(),
                layout: data.layout != null ? data.layout : 0,
                spec: data.spec != null ? data.spec : 0,
                api: data.api != null ? data.api : 0,
                web: data.web != null ? data.web : 0,
                utc: data.utc != null ? data.utc : 0,
                ute: data.ute != null ? data.ute : 0,
                integration: data.integration != null ? data.integration : 0,
                deployment: data.deployment != null ? data.deployment : 0,
                fixbug: data.fixbug != null ? data.fixbug : 0,
                support: data.support != null ? data.support : 0,
                others: data.others != null ? data.others : 0,
                sum: data.sum,
                userUpdated: idUserCureent,
            }
            return objData;
        },
        async addEditTimeReviewDaily() {
            this.closeModal();
            var listObj = [];
            var idUserCureent = checkAccessModule.getUserIdCurrent();
            if (this.dataTimeSheetById != null) {
                var listObjAdd = [];
                this.dataSend.map((ele) => {
                    if(ele.id!=undefined){
                        var result = this.editTimeSheetDto(ele, idUserCureent);
                        listObj.push(result);
                    }
                    else {
                        var result = this.addTimeSheetDto(ele, idUserCureent);
                        listObjAdd.push(result);
                    }
                });
                await this.callApi('Edit', listObj, true);
                if(listObjAdd.length > 0){
                    await this.callApi('Add', listObjAdd, false);
                }
            } else {
                this.dataSend.map((ele) => {
                    var result = this.addTimeSheetDto(ele, idUserCureent);
                    listObj.push(result);
                })
                await this.callApi('Add', listObj, true);
            }
        },

        async handleSubmit() {
            try {
                this.isSubmit = true;
                this.isOverValued = false;
                if (this.errorEmptyOrNoTaskName) {
                    this.errorEmptyTaskContent = 'Nội dung công việc không được bỏ trống. ';
                    this.isOverValued = true;
                }
                if (this.errorIsEqualContent) {
                    this.errorEqualContent = 'Hai công việc có nội dung giống nhau! ';
                    this.isOverValued = true;
                }
                if (this.sumWorkToday === 0) {
                    this.errorEmptyTimeWork = 'Thời gian làm việc không được bỏ trống. ';
                    this.isOverValued = true;
                }
                if (this.sumWorkToday > 24) {
                    this.errorEmptyOrOverTime = 'Tổng thời gian làm việc vượt quá 24 giờ! ';
                    this.isOverValued = true;
                }
                if (this.v$.$invalid === false && this.isOverValued === false && this.errorCheckDay == '') {
                    await this.addEditTimeReviewDaily();
                }
            } catch (err) {
                console.log(err);
            }
        },
    },
}
</script>

<style lang="scss" scoped>
.p-dropdown {
    width: 100%;
}
::v-deep(.p-dialog.p-dialog-footer) {
    padding: 1rem 0.5rem 0.7rem 1.5rem !important;
    display: flow-root !important;
}
.invalid {
    border-color: #e24c4c;
}
</style>
