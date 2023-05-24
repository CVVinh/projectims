<template>
    <Toast position="top-right" />
    <Dialog
        header="Cập nhật thông tin người dùng"
        :visible="statusopen"
        :closable="false"
        :maximizable="true"
        :modal="true"
        :show="onShow()"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
    >
        <div class="container mt-4">
            <form @submit.prevent="handleSubmit(!v$.$invalid)">
                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.userCode.$error }">
                                <InputText
                                    id=" userCode"
                                    v-model="v$.form.userCode.$model"
                                    :class="{ 'p-invalid': v$.form.userCode.$invalid && submitted }"
                                    :disabled="true"
                                    class="custom-input-h"
                                />
                                <label for="userCode" :class="{ 'p-error': v$.form.userCode.$invalid && submitted }"
                                    >Tài khoản<b style="color: red">*</b></label
                                >
                            </div>
                            <span v-if="v$.form.userCode.$error && submitted">
                                <span
                                    id="userCode-error"
                                    v-for="(error, index) of v$.form.userCode.$errors"
                                    :key="index"
                                >
                                    <small class="p-error">{{ error.$message }}</small>
                                </span>
                            </span>
                            <small
                                v-else-if="
                                    (v$.form.userCode.$invalid && submitted) || v$.form.userCode.$pending.$response
                                "
                                class="p-error"
                                >{{ v$.form.userCode.required.$message.replace('Value', 'Account') }}</small
                            >
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4" v-if="checkIsAdmin()">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.idGroup.$error }">
                                <MultiSelect
                                    v-model="v$.form.idGroup.$model"
                                    filter
                                    :options="optionRoles"
                                    optionLabel="nameGroup"
                                    optionValue="id"
                                    id="moduleName"
                                    :class="{ 'p-invalid': v$.form.idGroup.$invalid && submitted }"
                                    :maxSelectedLabels="3"
                                    class="custom-input-h"
                                />
                                <label for="idGroup" :class="{ 'p-error': v$.form.idGroup.$invalid && submitted }"
                                    >Chức vụ<b style="color: red">*</b></label
                                >
                            </div>
                            <small
                                v-if="(v$.form.idGroup.$invalid && submitted) || v$.form.idGroup.$pending.$response"
                                class="p-error"
                                >{{ v$.form.idGroup.required.$message.replace('Value', 'Role') }}</small
                            >
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.lastName.$error }">
                                <InputText
                                    id=" lastName"
                                    v-model="v$.form.lastName.$model"
                                    :class="{ 'p-invalid': v$.form.lastName.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label for="lastName" :class="{ 'p-error': v$.form.lastName.$invalid && submitted }"
                                    >Họ<b style="color: red">*</b></label
                                >
                            </div>
                            <small
                                v-if="(v$.form.lastName.$invalid && submitted) || v$.form.lastName.$pending.$response"
                                class="p-error"
                                >{{ v$.form.lastName.required.$message.replace('Value', 'Last name') }}</small
                            >
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.firstName.$error }">
                                <InputText
                                    id=" firstName"
                                    v-model="v$.form.firstName.$model"
                                    :class="{ 'p-invalid': v$.form.firstName.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label for="firstName" :class="{ 'p-error': v$.form.firstName.$invalid && submitted }"
                                    >Tên<b style="color: red">*</b></label
                                >
                            </div>
                            <small
                                v-if="(v$.form.firstName.$invalid && submitted) || v$.form.firstName.$pending.$response"
                                class="p-error"
                                >{{ v$.form.firstName.required.$message.replace('Value', 'First name') }}</small
                            >
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.gender.$error }">
                                <Dropdown
                                    v-model="v$.form.gender.$model"
                                    :options="optionGender"
                                    optionValue="code"
                                    optionLabel="name"
                                    :class="{ 'p-invalid': v$.form.gender.$invalid && submitted }"
                                    class="custom-input-h"
                                    style="width: 100%"
                                />
                                <label for="gender" :class="{ 'p-error': v$.form.gender.$invalid && submitted }"
                                    >Giới tính<b style="color: red">*</b></label
                                >
                            </div>
                            <small
                                v-if="(v$.form.gender.$invalid && submitted) || v$.form.gender.$pending.$response"
                                class="p-error"
                                >{{ v$.form.gender.required.$message.replace('Value', 'Role') }}</small
                            >
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.maritalStatus.$error }">
                                <Dropdown
                                    v-model="v$.form.maritalStatus.$model"
                                    :options="optionMaritalStatus"
                                    optionLabel="name"
                                    optionValue="code"
                                    :class="{ 'p-invalid': v$.form.maritalStatus.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label
                                    for="maritalStatus"
                                    :class="{ 'p-error': v$.form.maritalStatus.$invalid && submitted }"
                                    >Tình trạng hôn nhân
                                </label>
                            </div>
                            <small
                                v-if="
                                    (v$.form.maritalStatus.$invalid && submitted) ||
                                    v$.form.maritalStatus.$pending.$response
                                "
                                class="p-error"
                                >{{ v$.form.maritalStatus.required.$message.replace('Value', 'Marital Status') }}</small
                            >
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label">
                                <InputText
                                    id="email"
                                    v-model="v$.form.email.$model"
                                    :class="{ 'p-invalid': v$.form.email.$invalid && submitted }"
                                    aria-describedby="email-error"
                                    class="custom-input-h"
                                />
                                <label for="email" :class="{ 'p-error': v$.form.email.$invalid && submitted }"
                                    >Email<b style="color: red">*</b></label
                                >
                            </div>
                            <span v-if="v$.form.email.$error && submitted">
                                <span id="email-error" v-for="(error, index) of v$.form.email.$errors" :key="index">
                                    <small class="p-error">{{ error.$message }}</small>
                                </span>
                            </span>
                            <small
                                v-else-if="(v$.form.email.$invalid && submitted) || v$.form.email.$pending.$response"
                                class="p-error"
                                >{{ v$.form.email.required.$message.replace('Value', 'Email') }}</small
                            >
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.skype.$error }">
                                <InputText
                                    id=" skype"
                                    v-model="v$.form.skype.$model"
                                    :class="{ 'p-invalid': v$.form.skype.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label for="skype" :class="{ 'p-error': v$.form.skype.$invalid && submitted }"
                                    >Skype</label
                                >
                            </div>
                            <small
                                v-if="(v$.form.skype.$invalid && submitted) || v$.form.skype.$pending.$response"
                                class="p-error"
                                >{{ v$.form.skype.required.$message.replace('Value', 'Skype') }}</small
                            >
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.phoneNumber.$error }">
                                <InputText
                                    id=" phoneNumber"
                                    v-model="v$.form.phoneNumber.$model"
                                    :class="{ 'p-invalid': v$.form.phoneNumber.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label
                                    for="phoneNumber"
                                    :class="{ 'p-error': v$.form.phoneNumber.$invalid && submitted }"
                                    >Số điện thoại</label
                                >
                            </div>
                            <span v-if="v$.form.phoneNumber.$error && submitted">
                                <span
                                    id="phoneNumber-error"
                                    v-for="(error, index) of v$.form.phoneNumber.$errors"
                                    :key="index"
                                >
                                    <small class="p-error">{{ error.$message + ' ' }} </small>
                                </span>
                            </span>
                            <small
                                v-if="
                                    (v$.form.phoneNumber.$invalid && submitted) ||
                                    v$.form.phoneNumber.$pending.$response
                                "
                                class="p-error"
                                >Độ dài điện thoại phải là 10
                            </small>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.identitycard.$error }">
                                <InputText
                                    id=" identitycard"
                                    v-model="v$.form.identitycard.$model"
                                    :class="{ 'p-invalid': v$.form.identitycard.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label
                                    for="identitycard"
                                    :class="{ 'p-error': v$.form.identitycard.$invalid && submitted }"
                                    >Căn cước công dân<b style="color: red">*</b></label
                                >
                            </div>
                            <span
                                v-if="
                                    (v$.form.identitycard.$invalid && submitted) ||
                                    v$.form.identitycard.$pending.$response
                                "
                                class="p-error"
                            >
                                <span v-for="(error, index) of v$.form.identitycard.$errors" :key="index">
                                    <small class="p-error">{{ error.$message }}</small>
                                </span>
                                <small
                                    v-if="
                                        (v$.form.identitycard.$invalid && submitted) ||
                                        v$.form.identitycard.$pending.$response
                                    "
                                    class="p-error"
                                    >. Độ dài phải từ 9 đến 12
                                </small>
                            </span>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.dob.$error }">
                                <Calendar
                                    id="dob"
                                    v-model="v$.form.dob.$model"
                                    :showIcon="true"
                                    :class="{
                                        'p-invalid': (v$.form.dob.$invalid || !checkDateOfBirth) && submitted,
                                    }"
                                />
                                <label
                                    for="dob"
                                    :class="{
                                        'p-error': (v$.form.dob.$invalid || !checkDateOfBirth) && submitted,
                                    }"
                                    >Ngày sinh</label
                                >
                            </div>
                            <small
                                v-if="(v$.form.dob.$invalid && submitted) || v$.form.dob.$pending.$response"
                                class="p-error"
                                >{{ v$.form.dob.required.$message.replace('Value', 'Birthday') }}</small
                            >
                            <small v-if="!checkDateOfBirth && submitted" class="p-error">
                                Người dùng phải trên 18 tuổi và dưới 60 tuổi
                            </small>
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.idBranch.$error }">
                                <Dropdown
                                    v-model="v$.form.idBranch.$model"
                                    :options="branchList"
                                    filter
                                    optionLabel="branchName"
                                    optionValue="idBranch"
                                    placeholder="Chọn chi nhánh"
                                    style="width: 100%"
                                    :class="{ 'p-invalid': v$.form.idBranch.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label for="idBranch" :class="{ 'p-error': v$.form.idBranch.$invalid && submitted }"
                                    >Chọn chi nhánh<b style="color: red">*</b></label
                                >
                            </div>
                            <small
                                v-if="(v$.form.idBranch.$invalid && submitted) || v$.form.idBranch.$pending.$response"
                                class="p-error"
                                >{{ v$.form.idBranch.required.$message.replace('Value', 'Branch') }}</small
                            >
                        </div>                       
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.university.$error }">
                                <InputText
                                    id=" university"
                                    v-model="v$.form.university.$model"
                                    :class="{ 'p-invalid': v$.form.university.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label for="university" :class="{ 'p-error': v$.form.university.$invalid && submitted }"
                                    >Đại học</label
                                >
                            </div>
                            <small
                                v-if="
                                    (v$.form.university.$invalid && submitted) || v$.form.university.$pending.$response
                                "
                                class="p-error"
                                >{{ v$.form.university.required.$message.replace('Value', 'University') }}</small
                            >
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.yearGraduated.$error }">
                                <InputText
                                    id=" yearGraduated"
                                    v-model="v$.form.yearGraduated.$model"
                                    :class="{ 'p-invalid': v$.form.yearGraduated.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label
                                    for="yearGraduated"
                                    :class="{ 'p-error': v$.form.yearGraduated.$invalid && submitted }"
                                    >Năm tốt nghiệp</label
                                >
                            </div>
                            <span
                                v-if="
                                    (v$.form.yearGraduated.$invalid && submitted) ||
                                    v$.form.yearGraduated.$pending.$response
                                "
                                class="p-error"
                            >
                                <span v-for="(error, index) of v$.form.yearGraduated.$errors" :key="index">
                                    <small class="p-error">{{ error.$message }}. </small>
                                </span>
                            </span>
                        </div>
                    </div>
                </div>

                <div class="row" v-if="checkIsAdmin()">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.dateStartWork.$error }">
                                <Calendar
                                    id="dateStartWork"
                                    v-model="v$.form.dateStartWork.$model"
                                    :showIcon="true"
                                    :class="{ 'p-invalid': v$.form.dateStartWork.$invalid && submitted }"
                                />
                                <label
                                    for="dateStartWork"
                                    :class="{ 'p-error': v$.form.dateStartWork.$invalid && submitted }"
                                    >Ngày bắt đầu làm*</label
                                >
                            </div>
                            <small
                                v-if="
                                    (v$.form.dateStartWork.$invalid && submitted) ||
                                    v$.form.dateStartWork.$pending.$response
                                "
                                class="p-error"
                                >{{
                                    v$.form.dateStartWork.required.$message.replace('Value', 'Date start work')
                                }}</small
                            >
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.dateLeave.$error }">
                                <Calendar
                                    id="dateLeave"
                                    v-model="v$.form.dateLeave.$model"
                                    :showIcon="true"
                                    :class="{ 'p-invalid': v$.form.dateLeave.$invalid && submitted }"
                                />
                                <label for="dateLeave" :class="{ 'p-error': v$.form.dateLeave.$invalid && submitted }"
                                    >Ngày thôi việc
                                </label>
                            </div>
                            <small
                                v-if="(v$.form.dateLeave.$invalid && submitted) || v$.form.dateLeave.$pending.$response"
                                class="p-error"
                                >{{ v$.form.dateLeave.required.$message.replace('Value', 'Date start work') }}</small
                            >
                        </div>
                    </div>
                </div>

                
                <div class="row" v-if="checkIsAdmin()">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.workStatus.$error }">
                                <Dropdown
                                    v-model="v$.form.workStatus.$model"
                                    :options="optionworkStatus"
                                    optionValue="code"
                                    optionLabel="name"
                                    :class="{ 'p-invalid': v$.form.workStatus.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label for="workStatus" :class="{ 'p-error': v$.form.workStatus.$invalid && submitted }"
                                    >Tình trạng làm việc*</label
                                >
                            </div>
                            <small
                                v-if="
                                    (v$.form.workStatus.$invalid && submitted) || v$.form.workStatus.$pending.$response
                                "
                                class="p-error"
                                >{{ v$.form.workStatus.required.$message.replace('Value', 'Role') }}</small
                            >
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.rank.$error }">
                                <InputText
                                    id="rank"
                                    v-model="v$.form.rank.$model"
                                    :class="{ 'p-invalid': v$.form.rank.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label for="rank" :class="{ 'p-error': v$.form.rank.$invalid && submitted }"
                                    >Xếp hạng</label
                                >
                            </div>
                            <small
                                v-if="(v$.form.rank.$invalid && submitted) || v$.form.rank.$pending.$response"
                                class="p-error"
                                >{{ v$.form.rank.required.$message.replace('Value', 'Rank') }}</small
                            >
                        </div>
                    </div>
                </div>

                <div class="row" v-if="checkIsAdmin()">
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.idUserGitLab.$error }">
                                <InputNumber
                                    :min="0"
                                    id=" idUserGitLab"
                                    v-model="v$.form.idUserGitLab.$model"
                                    :class="{ 'p-invalid': v$.form.idUserGitLab.$invalid && submitted }"
                                    class="custom-input-number-h"
                                    style="height: 37px;"
                                />
                                <label for="idUserGitLab" :class="{ 'p-error': v$.form.idUserGitLab.$invalid && submitted }"
                                    >Id người dùng trên gitlab<b style="color: red">*</b></label
                                >
                            </div>
                            <small
                                v-if="(v$.form.idUserGitLab.$invalid && submitted) || v$.form.idUserGitLab.$pending.$response"
                                class="p-error"
                                >{{ v$.form.idUserGitLab.required.$message.replace('Value', 'IdUserGitLab') }}</small
                            >
                        </div>
                    </div>
                    <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                        <div class="field">
                            <div class="p-inputgroup flex-1">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.tokenGitLab.$error }">
                                    <InputText
                                        disabled
                                        id=" tokenGitLab"
                                        v-model="v$.form.tokenGitLab.$model"
                                        class="custom-input-h"
                                        :class="{ 'p-invalid': v$.form.tokenGitLab.$invalid && submitted }"
                                    />
                                    <label for="tokenGitLab" :class="{ 'p-error': v$.form.tokenGitLab.$invalid && submitted }"
                                        >Token người dùng trên gitlab<b style="color: red">*</b></label
                                    >
                                    <Button icon="pi pi-plus" class="custom-button-h" @click="createTokenPrivate()" :disabled="!isHaveIdUserGitLab" v-tooltip.top="'Tạo token private'"/>
                                </div>
                            </div>
                            <small
                                v-if="(v$.form.tokenGitLab.$invalid && submitted) || v$.form.tokenGitLab.$pending.$response"
                                class="p-error"
                                >{{ v$.form.tokenGitLab.required.$message.replace('Value', 'TokenGitLab') }}</small
                            >
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12 mb-4">
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.form.address.$error }">
                                <Textarea
                                    rows="3"
                                    cols="30"
                                    id=" address"
                                    v-model="v$.form.address.$model"
                                    :class="{ 'p-invalid': v$.form.address.$invalid && submitted }"
                                    class="custom-input-h"
                                />
                                <label for="address" :class="{ 'p-error': v$.form.address.$invalid && submitted }"
                                    >Địa chỉ</label
                                >
                            </div>
                            <small
                                v-if="(v$.form.address.$invalid && submitted) || v$.form.address.$pending.$response"
                                class="p-error"
                                >{{ v$.form.address.required.$message.replace('Value', 'Address') }}</small
                            >
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12 col-md-12 col-lg-12 mb-4 d-flex justify-content-end">
                        <button-custom class="me-2" @click="closeModal()" />
                        <Button label="Lưu" icon="pi pi-check" type="submit" class="custom-button-h" autofocus />
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>

<script>
import { HTTP } from '@/http-common'
import jwt_decode from 'jwt-decode'
import moment from 'moment'
import { email, required, alphaNum, numeric, between, maxLength, minLength } from '@vuelidate/validators'
import { useVuelidate } from '@vuelidate/core'
import { ToastSeverity } from 'primevue/api'
import LayoutDefault from '../../layouts/LayoutDefault/LayoutDefault.vue'
import { UserRoleHelper } from '@/helper/user-role.helper'
import { checkAccessModule } from '@/helper/checkAccessModule'
import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
import { TaskService } from '@/service/task.service'
export default {
    props: ['statusopen', 'iduser', 'roleoption', 'branchList'],
    setup: () => ({
        v$: useVuelidate(),
    }),
    name: 'edituser',
    data() {
        return {
            form: {
                userCode: null,
                password: null,
                email: null,
                userCreate: checkAccessModule.getUserIdCurrent(),
                dateCreate: new Date(),
                // dateModified: new Date(),
                passwordEmail: null,
                firstName: null,
                lastName: null,
                phoneNumber: '',
                dob: '',
                gender: null,
                address: null,
                university: null,
                yearGraduated: null,
                skype: null,
                passwordSkype: null,
                dateStartWork: '',
                dateLeave: '',
                maritalStatus: null,
                reasonResignation: null,
                workStatus: null,
                idGroup: [],
                dOBValidate: true,
                idBranch: null,
                rank: '',
                idUserGitLab: null,
                tokenGitLab: '',
            },
            isHaveIdUserGitLab: false,
            submitted: false,
            optionRoles: [],
            optionGender: [
                { name: 'Nam', code: 1 },
                { name: 'Nữ', code: 2 },
                { name: 'Khác', code: 3 },
            ],
            optionMaritalStatus: [
                { name: 'Kết hôn', code: 1 },
                { name: 'Chưa kết hôn', code: 2 },
                { name: 'Khác', code: 3 },
            ],
            // optionRoles: [
            //     { name: 'President', code: 1 },
            //     { name: 'HR', code: 2 },
            //     { name: 'PM', code: 3 },
            //     { name: 'Leader', code: 4 },
            //     { name: 'Accountant', code: 5 },
            //     { name: 'Staff', code: 6 },
            //     { name: 'Admin', code: 7 },
            // ],
            optionworkStatus: [
                { name: 'Đang làm', code: 1 },
                { name: 'Đã từ chức', code: 2 },
                { name: 'Nghỉ thai sản', code: 3 },
            ],
        }
    },
    validations() {
        return {
            form: {
                userCode: {
                    required,
                },
                email: {
                    required,
                    email,
                    maxLength: maxLength(50),
                },
                firstName: {
                    required,
                },
                lastName: {
                    required,
                },
                phoneNumber: {
                    required,
                    // checkPhoneNumber(value) {
                    //     0.3
                    //     if (!value) return true
                    //     if (value.length === 0) return true
                    //     if (value.includes('.')) return false
                    //     if (value.length === 10) return true
                    //     else return false
                    // },
                },
                dob: {},
                identitycard: {
                    numeric,
                    required,
                    valueNineOrTwelve(value) {
                        if (value.includes('.')) return false
                        if (value.length === 9 || value.length === 12) return true
                        else return false
                    },
                },
                workStatus: {
                    required,
                },
                gender: {},
                address: {},
                university: {},
                rank: {},
                yearGraduated: {
                    numeric,
                    between: between(1980, 2100),
                },
                skype: {
                    maxLength: maxLength(100),
                },
                reasonResignation: {},
                dateStartWork: {
                    required,
                },
                dateLeave: {},
                maritalStatus: {
                    between: between(1, 3),
                },
                idGroup: {
                    required,
                },
                idBranch: {
                    required,
                },
                idUserGitLab: {
                    required,
                },
                tokenGitLab: {
                    required,
                },
            },
        }
    },
    beforeUpdate() {
        this.onShow()
    },
    computed: {
        async ispermission() {
            if ((await UserRoleHelper.isHr()) || (await UserRoleHelper.isDirector())) return false
            return true
        },
        checkDateLeave() {
            if (this.v$.form.workStatus.$model === 2) return false
            else {
                this.v$.form.dateLeave.$model = null
                return true
            }
        },
        checkDateOfBirth() {
            const dOB = this.v$.form.dob.$model
            if (dOB == '') {
                this.dOBValidate = false
                return true
            }
            var today = new Date()
            var thisYear = new Date(today).getFullYear()
            var birthYear = new Date(dOB).getFullYear()
            if (thisYear - birthYear >= 18 && thisYear - birthYear <= 60) {
                this.dOBValidate = false
                return true
            } else {
                this.dOBValidate = true
                return false
            }
        },
    },
    mounted() {
        if (checkAccessModule.getListGroup().includes('1')) this.getAllGroup()
    },
    watch: {
        form: {
            handler: async function changeSelected(newValue) {
                if(newValue.idUserGitLab==null) {
                    this.form.tokenGitLab = "";
                    this.isHaveIdUserGitLab = false;
                }
                else {
                    this.isHaveIdUserGitLab = true;
                }
            },
            deep: true,
        },
    },
    methods: {
        checkIsAdmin() {
            if (checkAccessModule.isAdmin()) {
                return true
            } else {
                return false
            }
        },
        showError(message) {
            this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
        },
        showSuccess(message) {
            this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
        },
        showInfo(message) {
            this.$toast.add({ severity: 'info', summary: 'Thông báo', detail: message, life: 3000 })
        },
        async getAllGroup() {
            await HTTP.get('Group/getListGroup')
                .then((res) => {
                    this.optionRoles = res.data._Data
                })
                .catch((er) => {
                    this.showSuccess('Lỗi! Lấy thông tin nhóm!')
                })
        },
        closeModal() {
            this.$emit('closeModal')
            this.submitted = false
        },
        handleSubmit(isFormValid) {
            this.submitted = true
            if (!isFormValid) {
                return
            }
            this.Submit()
        },
        async Submit() {
            if (this.iduser) {
                if (!this.checkDateOfBirth) return
                const dataPost = {
                    userModified: parseInt(checkAccessModule.getUserIdCurrent()),
                    dateModified: new Date(),
                    firstName: this.form.firstName,
                    lastName: this.form.lastName,
                    phoneNumber: this.form.phoneNumber,
                    dOB: this.form.dob == null ? '' : this.form.dob,
                    identitycard: this.form.identitycard,
                    gender: this.form.gender,
                    address: this.form.address,
                    university: this.form.university,
                    yearGraduated: this.form.yearGraduated,
                    email: this.form.email,
                    skype: this.form.skype,
                    workStatus: this.form.workStatus,
                    dateStartWork: new Date(this.form.dateStartWork).toLocaleString('en-US', {
                        timeZone: 'Asia/Ho_Chi_Minh',
                    }),
                    dateLeave: this.form.dateLeave,
                    maritalStatus: this.form.maritalStatus,
                    reasonResignation: this.form.reasonResignation,
                    idBranch: this.form.idBranch,
                    rank: this.form.rank,
                    idUserGitLab: this.form.idUserGitLab,
                    tokenGitLab: this.form.tokenGitLab,
                }
                try {
                    var listGroup = this.form.idGroup
                    this.closeModal()
                    this.$emit('reloadEditToSave')
                    const res = await HTTP.put('Users/updateUser/' + this.iduser, dataPost)
                        .then(async (responseUpdate) => {
                            if (responseUpdate.status == 200) {
                                if (checkAccessModule.isAdmin()) {
                                    await HTTP.put(
                                        `User_Group/updateUserGroupMultiRoleUpgrade/${
                                            this.iduser
                                        }/${checkAccessModule.getUserIdCurrent()}`,
                                        listGroup,
                                    ).then((responseAddGroup) => {
                                        this.$emit('reloadpage')
                                        this.showSuccess('Cập nhật thông tin người dùng thành công!')
                                    })
                                } else {
                                    this.$emit('reloadpage')
                                    this.showSuccess('Cập nhật thông tin người dùng thành công!')
                                }
                            } else if (responseUpdate.status == 403 || responseUpdate.status == 401) {
                                this.showInfo('Không có quyền cập nhật người dùng!')
                            }
                        })
                        .catch((error) => {
                            switch (error.code) {
                                case 'ERR_NETWORK':
                                    this.showInfo('Kiểm tra kết nối !')
                                    break
                                case 'ERR_BAD_REQUEST':
                                    this.showError('Lỗi! cập nhật người dùng!') //error.response.data
                                    break
                            }
                        })
                        .finally(() => {
                            this.$emit('coloseReloadEditToSave');
                        })
                } catch (error) {
                    switch (error.code) {
                        case 'ERR_NETWORK':
                            this.showInfo('Kiểm tra kết nối !')
                            break
                        case 'ERR_BAD_REQUEST':
                            this.showError(error.response.data)
                            break
                    }
                }
            }
        },
        closeDialog() {
            this.submitted = false
        },
        async onShow() {
            if (this.iduser) {
                await HTTP.get('Users/getUserById/' + this.iduser)
                    .then(async (res) => {
                        if (res.status == 200) {
                            this.form = res.data
                            // this.userCode = res.data.userCode
                            this.form.dateStartWork =
                                res.data.dateStartWork == null
                                    ? null
                                    : new Date(
                                          res.data.dateStartWork,
                                      ) /*moment(res.data.dateStartWork).format('DD/MM/YYYY')*/
                            this.form.dateLeave =
                                res.data.dateLeave == null
                                    ? null
                                    : new Date(res.data.dateLeave) /*moment(res.data.dateLeave).format('DD/MM/YYYY')*/
                            this.form.dob =
                                res.data.dOB == null
                                    ? null
                                    : new Date(res.data.dOB) /*moment(res.data.dOB).format('DD/MM/YYYY')*/
                            this.optionworkStatus = [
                                { name: 'Đang làm', code: 1 },
                                { name: 'Đã từ chức', code: 2 },
                                { name: 'Nghỉ thai sản', code: 3 },
                            ]
                            if (this.form.gender != 2) {
                                this.optionworkStatus = [
                                    { name: 'Đang làm', code: 1 },
                                    { name: 'Đã từ chức', code: 2 },
                                ]
                            }
                            var arrGroup = []
                            await HTTP.get(`User_Group/getUserGroupWithUserId/${this.iduser}`)
                                .then((response) => {
                                    if (response.status == 200) {
                                        var groups = Object.values(response.data._Data)
                                        if (groups.length > 0) {
                                            groups.forEach((item) => {
                                                arrGroup.push(item.idGroup)
                                            })
                                        }
                                        this.form.idGroup = arrGroup
                                    }
                                })
                                .catch((error) => {
                                    console.log(error)
                                })
                        }
                    })
                    .catch((er) => {
                        this.$toast.add({
                            severity: ToastSeverity.ERROR,
                            summary: 'Lỗi ',
                            detail: 'Không thể lấy thông tin của người dùng!',
                            life: 3000,
                        })
                    })
            }
        },
        async createTokenPrivate(){
            try{
                if(this.form.idUserGitLab==null){ 
                    this.isHaveIdUserGitLab = false;
                    return; 
                }
                await TaskService.createUserTokenPrivate(this.form.idUserGitLab,{
                    "name": "New Token",
                    "scopes": ["api"]
                })
                .then((res) => {
                    this.form.tokenGitLab = res.data.token;
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
    },
    components: { ButtonCustom },
}
</script>

<style lang="scss" scoped>
.p-dropdown {
    width: 100%;
}
</style>
