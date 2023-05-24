<template>
    <Toast position="top-right" />
    <Dialog
        header="Thêm người dùng"
        :visible="statusopen"
        :maximizable="true"
        :closable="false"
        :modal="true"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
    >
        <div class="container">
            <div class="row mt-4">
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.userCode.$error }">
                            <InputText
                                id=" userCode"
                                v-model="v$.userCode.$model"
                                :class="{ 'p-invalid': v$.userCode.$invalid && submitted }"
                                autocomplete="nope"
                                class="custom-input-h"
                            />
                            <label for="userCode" :class="{ 'p-error': v$.userCode.$invalid && submitted }"
                                >Tài khoản<b style="color: red">*</b></label
                            >
                        </div>
                        <span v-if="v$.userCode.$error && submitted">
                            <span id="userCode-error" v-for="(error, index) of v$.userCode.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                        <small
                            v-else-if="(v$.userCode.$invalid && submitted) || v$.userCode.$pending.$response"
                            class="p-error"
                            >{{ v$.userCode.required.$message.replace('Value', 'Account') }}</small
                        >
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label">
                            <Password
                                id="password"
                                v-model="v$.password.$model"
                                autocomplete="new-password"
                                :class="{ 'p-invalid': v$.password.$invalid && submitted }"
                                toggleMask
                            >
                                <template #header>
                                    <h6>Pick a password</h6>
                                </template>
                                <template #footer="sp">
                                    {{ sp.level }}
                                </template>
                            </Password>
                            <label for="password" :class="{ 'p-error': v$.password.$invalid && submitted }"
                                >Mật khẩu<b style="color: red">*</b></label
                            >
                        </div>
                        <span v-if="v$.password.$error && submitted">
                            <span id="password-error" v-for="(error, index) of v$.password.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                        <small
                            v-else-if="(v$.password.$invalid && submitted) || v$.password.$pending.$response"
                            class="p-error"
                            >{{ v$.password.required.$message.replace('Value', 'Password') }}</small
                        >
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.firstName.$error }">
                            <InputText
                                id=" firstName"
                                v-model="v$.firstName.$model"
                                class="custom-input-h"
                                :class="{ 'p-invalid': v$.firstName.$invalid && submitted }"
                            />
                            <label for="firstName" :class="{ 'p-error': v$.firstName.$invalid && submitted }"
                                >Họ<b style="color: red">*</b></label
                            >
                        </div>
                        <small
                            v-if="(v$.firstName.$invalid && submitted) || v$.firstName.$pending.$response"
                            class="p-error"
                            >{{ v$.firstName.required.$message.replace('Value', 'First name') }}</small
                        >
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.lastName.$error }">
                            <InputText
                                id=" lastName"
                                v-model="v$.lastName.$model"
                                class="custom-input-h"
                                :class="{ 'p-invalid': v$.lastName.$invalid && submitted }"
                            />
                            <label for="lastName" :class="{ 'p-error': v$.lastName.$invalid && submitted }"
                                >Tên<b style="color: red">*</b></label
                            >
                        </div>
                        <small
                            v-if="(v$.lastName.$invalid && submitted) || v$.lastName.$pending.$response"
                            class="p-error"
                            >{{ v$.lastName.required.$message.replace('Value', 'Last name') }}</small
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
                                v-model="v$.email.$model"
                                class="custom-input-h"
                                :class="{ 'p-invalid': v$.email.$invalid && submitted }"
                                aria-describedby="email-error"
                            />
                            <label for="email" :class="{ 'p-error': v$.email.$invalid && submitted }"
                                >Email<b style="color: red">*</b></label
                            >
                        </div>
                        <span v-if="v$.email.$error && submitted">
                            <span id="email-error" v-for="(error, index) of v$.email.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                        <small
                            v-else-if="(v$.email.$invalid && submitted) || v$.email.$pending.$response"
                            class="p-error"
                            >{{ v$.email.required.$message.replace('Value', 'Email') }}</small
                        >
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.skype.$error }">
                            <InputText
                                id=" skype"
                                v-model="v$.skype.$model"
                                :class="{ 'p-invalid': v$.skype.$invalid && submitted } + ' custom-input-h'"
                            />
                            <label for="skype" :class="{ 'p-error': v$.skype.$invalid && submitted }">Skype</label>
                        </div>
                        <small v-if="(v$.skype.$invalid && submitted) || v$.skype.$pending.$response" class="p-error">{{
                            v$.skype.required.$message.replace('Value', 'Skype')
                        }}</small>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.phoneNumber.$error }">
                            <InputText
                                id=" phoneNumber"
                                v-model="v$.phoneNumber.$model"
                                :class="{ 'p-invalid': v$.phoneNumber.$invalid && submitted } + ' custom-input-h'"
                            />
                            <label for="phoneNumber" :class="{ 'p-error': v$.phoneNumber.$invalid && submitted }"
                                >Số điện thoại</label
                            >
                        </div>
                        <span v-if="v$.phoneNumber.$error && submitted">
                            <span id="phoneNumber-error" v-for="(error, index) of v$.phoneNumber.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                        <small
                            v-else-if="(v$.phoneNumber.$invalid && submitted) || v$.phoneNumber.$pending.$response"
                            class="p-error"
                            >{{ v$.phoneNumber.required.$message.replace('Value', 'PhoneNumber') }}</small
                        >
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.identitycard.$error }">
                            <InputText
                                id=" identitycard"
                                v-model="v$.identitycard.$model"
                                class="custom-input-h"
                                :class="{ 'p-invalid': v$.identitycard.$invalid && submitted }"
                            />
                            <label for="identitycard" :class="{ 'p-error': v$.identitycard.$invalid && submitted }"
                                >Căn cước công dân<b style="color: red">*</b></label
                            >
                        </div>
                        <small
                            v-if="(v$.identitycard.$invalid && submitted) || v$.identitycard.$pending.$response"
                            class="p-error"
                            >{{ v$.identitycard.required.$message.replace('Value', 'Identity card') }}</small
                        >
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.gender.$error }">
                            <Dropdown
                                v-model="v$.gender.$model"
                                :options="optionGender"
                                optionLabel="name"
                                class="custom-input-h"
                                :class="{ 'p-invalid': v$.gender.$invalid && submitted }"
                            />
                            <label for="gender" :class="{ 'p-error': v$.gender.$invalid && submitted }"
                                >Giới tính<b style="color: red">*</b></label
                            >
                        </div>
                        <small
                            v-if="(v$.gender.$invalid && submitted) || v$.gender.$pending.$response"
                            class="p-error"
                            >{{ v$.gender.required.$message.replace('Value', 'Gender') }}</small
                        >
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.idGroup.$error }">
                            <MultiSelect
                                v-model="v$.idGroup.$model"
                                :options="optionRoles"
                                filter
                                optionLabel="nameGroup"
                                optionValue="id"
                                id="moduleName"
                                :class="{ 'p-invalid': v$.idGroup.$invalid && submitted }"
                                :maxSelectedLabels="3"
                                class="custom-input-h"
                            />

                            <label for="idGroup" :class="{ 'p-error': v$.idGroup.$invalid && submitted }"
                                >Chức vụ<b style="color: red">*</b></label
                            >
                        </div>
                        <small
                            v-if="(v$.idGroup.$invalid && submitted) || v$.idGroup.$pending.$response"
                            class="p-error"
                            >{{ v$.idGroup.required.$message.replace('Value', 'idGroup') }}</small
                        >
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.dob.$error }">
                            <Calendar
                                id="dob"
                                v-model="v$.dob.$model"
                                :showIcon="true"
                                :class="{ 'p-invalid': (v$.dob.$invalid || dOBValidate) && submitted }"
                            />
                            <label for="dob" :class="{ 'p-error': (v$.dob.$invalid || dOBValidate) && submitted }"
                                >Ngày sinh</label
                            >
                        </div>
                        <small v-if="(v$.dob.$invalid && submitted) || v$.dob.$pending.$response" class="p-error">{{
                            v$.dob.required.$message.replace('Value', 'Birthday')
                        }}</small>
                        <small v-if="!this.checkDateOfBirth(v$.dob.$model) && submitted" class="p-error">
                            Người dùng phải trên 18 tuổi và dưới 60 tuổi
                        </small>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.dateStartWork.$error }">
                            <Calendar
                                id="dateStartWork"
                                v-model="v$.dateStartWork.$model"
                                :showIcon="true"
                                :class="{ 'p-invalid': v$.dateStartWork.$invalid && submitted }"
                            />
                            <label for="dateStartWork" :class="{ 'p-error': v$.dateStartWork.$invalid && submitted }"
                                >Ngày bắt đầu làm<b style="color: red">*</b></label
                            >
                        </div>
                        <small
                            v-if="(v$.dateStartWork.$invalid && submitted) || v$.dateStartWork.$pending.$response"
                            class="p-error"
                            >{{ v$.dateStartWork.required.$message.replace('Value', 'Date start work') }}</small
                        >
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.university.$error }">
                            <InputText
                                id=" university"
                                v-model="v$.university.$model"
                                :class="{ 'p-invalid': v$.university.$invalid && submitted } + ' custom-input-h'"
                            />
                            <label for="university" :class="{ 'p-error': v$.university.$invalid && submitted }"
                                >Đại học</label
                            >
                        </div>
                        <small
                            v-if="(v$.university.$invalid && submitted) || v$.university.$pending.$response"
                            class="p-error"
                            >{{ v$.university.required.$message.replace('Value', 'University') }}</small
                        >
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.yearGraduated.$error }">
                            <InputText
                                id=" yearGraduated"
                                v-model="v$.yearGraduated.$model"
                                :class="{ 'p-invalid': v$.yearGraduated.$invalid && submitted } + ' custom-input-h'"
                            />
                            <label for="yearGraduated" :class="{ 'p-error': v$.yearGraduated.$invalid && submitted }"
                                >Năm tốt nghiệp</label
                            >
                        </div>
                        <span v-if="v$.yearGraduated.$error && submitted">
                            <span
                                id="yearGraduated-error"
                                v-for="(error, index) of v$.yearGraduated.$errors"
                                :key="index"
                            >
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                        <small
                            v-else-if="(v$.yearGraduated.$invalid && submitted) || v$.yearGraduated.$pending.$response"
                            class="p-error"
                            >{{ v$.yearGraduated.required.$message.replace('Value', 'yearGraduated') }}</small
                        >
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.maritalStatus.$error }">
                            <Dropdown
                                v-model="v$.maritalStatus.$model"
                                :options="optionMaritalStatus"
                                optionLabel="name"
                                :class="{ 'p-invalid': v$.maritalStatus.$invalid && submitted } + ' custom-input-h'"
                            />
                            <label for="maritalStatus" :class="{ 'p-error': v$.maritalStatus.$invalid && submitted }"
                                >Tình trạng hôn nhân</label
                            >
                        </div>
                        <small
                            v-if="(v$.maritalStatus.$invalid && submitted) || v$.maritalStatus.$pending.$response"
                            class="p-error"
                            >{{ v$.maritalStatus.required.$message.replace('Value', 'Marital Status') }}</small
                        >
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.idBranch.$error }">
                            <Dropdown
                                v-model="v$.idBranch.$model"
                                :options="branchList"
                                filter
                                optionLabel="branchName"
                                optionValue="idBranch"
                                placeholder="Chọn chi nhánh"
                                style="width: 100%"
                                :class="{ 'p-invalid': v$.idBranch.$invalid && submitted }"
                                class="custom-input-h"
                            />
                            <label for="idBranch" :class="{ 'p-error': v$.idBranch.$invalid && submitted }"
                                >Chọn chi nhánh<b style="color: red">*</b></label
                            >
                        </div>
                        <small
                            v-if="(v$.idBranch.$invalid && submitted) || v$.idBranch.$pending.$response"
                            class="p-error"
                            >{{ v$.idBranch.required.$message.replace('Value', 'Branch') }}</small
                        >
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.idUserGitLab.$error }">
                            <InputNumber
                                id=" idUserGitLab"
                                :min="0"
                                v-model="v$.idUserGitLab.$model"
                                class="custom-input-number-h"
                                style="height: 37px;"
                                :class="{ 'p-invalid': v$.idUserGitLab.$invalid && submitted }"
                            />
                            <label for="idUserGitLab" :class="{ 'p-error': v$.idUserGitLab.$invalid && submitted }"
                                >Id người dùng trên gitlab<b style="color: red">*</b></label
                            >
                        </div>
                        <small
                            v-if="(v$.idUserGitLab.$invalid && submitted) || v$.idUserGitLab.$pending.$response"
                            class="p-error"
                            >{{ v$.idUserGitLab.required.$message.replace('Value', 'IdUserGitLab') }}</small
                        >
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-inputgroup flex-1">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.tokenGitLab.$error }">
                                <InputText
                                    disabled
                                    id=" tokenGitLab"
                                    v-model="v$.tokenGitLab.$model"
                                    class="custom-input-h"
                                    :class="{ 'p-invalid': v$.tokenGitLab.$invalid && submitted }"
                                />
                                <label for="tokenGitLab" :class="{ 'p-error': v$.tokenGitLab.$invalid && submitted }"
                                >Token người dùng trên gitlab<b style="color: red">*</b></label>
                                <Button icon="pi pi-plus" class="custom-button-h" @click="createTokenPrivate()" :disabled="!isHaveIdUserGitLab" v-tooltip.top="'Tạo token private'"/>
                            </div>
                        </div>
                        <small
                            v-if="(v$.tokenGitLab.$invalid && submitted) || v$.tokenGitLab.$pending.$response"
                            class="p-error"
                            >{{ v$.tokenGitLab.required.$message.replace('Value', 'TokenGitLab') }}
                        </small>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.rank.$error }">
                            <InputText
                                id="rank"
                                v-model="v$.rank.$model"
                                :class="{ 'p-invalid': v$.rank.$invalid && submitted }"
                                class="custom-input-h"
                            />
                            <label for="rank" :class="{ 'p-error': v$.rank.$invalid && submitted }">Xếp hạng</label>
                        </div>
                        <small v-if="(v$.rank.$invalid && submitted) || v$.rank.$pending.$response" class="p-error">{{
                            v$.rank.required.$message.replace('Value', 'Rank')
                        }}</small>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6 mb-4"></div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-12 col-lg-12 mb-4">
                    <div class="field">
                        <div class="p-float-label" :class="{ 'form-group--error': v$.address.$error }">
                            <Textarea
                                rows="3"
                                cols="30"
                                id=" address"
                                v-model="v$.address.$model"
                                :class="{ 'p-invalid': v$.address.$invalid && submitted }"
                            />

                            <label
                                for="address"
                                :class="{ 'p-error': v$.address.$invalid && submitted } + ' custom-input-h'"
                                >Địa chỉ</label
                            >
                        </div>
                        <small
                            v-if="(v$.address.$invalid && submitted) || v$.address.$pending.$response"
                            class="p-error"
                            >{{ v$.address.required.$message.replace('Value', 'Address') }}</small
                        >
                    </div>
                </div>
            </div>
        </div>
        <template #footer>
            <button-custom class="me-2" @click="closeAdd()" />
            <Button label="Lưu" icon="pi pi-check" @click="handleSubmit" class="custom-button-h" autofocus />
        </template>
    </Dialog>
</template>

<script>
    import { email, required, alphaNum, numeric, between, minLength, maxLength } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { HTTP } from '@/http-common'
    import jwt_decode from 'jwt-decode'
    import { HttpStatus } from '@/config/app.config'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
    import { TaskService } from '@/service/task.service'

    export default {
        components: { ButtonCustom },
        props: ['statusopen', 'roleoption', 'branchList'],

    setup: () => ({
        v$: useVuelidate(),
    }),
    data() {
        return {
            userCode: '',
            password: '',
            email: '',
            userCreate: localStorage.getItem('username'),
            dateCreate: new Date(),
            firstName: '',
            lastName: '',
            phoneNumber: '',
            identitycard: '',
            dob: '',
            gender: '',
            address: '',
            university: '',
            yearGraduated: '',
            skype: '',
            dateStartWork: '',
            maritalStatus: '',
            workStatus: 1,
            dOBValidate: true,
            idBranch: null,
            idGroup: [],
            submitted: false,
            rank: '',
            idUserGitLab: null,
            tokenGitLab: '',
            optionGender: [
                { name: 'Male', code: 1 },
                { name: 'Female', code: 2 },
                { name: 'Other', code: 3 },
            ],
            optionMaritalStatus: [
                { name: 'Married', code: 1 },
                { name: 'Unmarried', code: 2 },
                { name: 'Undefined', code: 3 },
            ],
            optionRoles: [],
            messages: [{ severity: 'info', content: 'Dynamic Info Message' }],
            isHaveIdUserGitLab: false,
        }
    },

    validations() {
        return {
            userCode: {
                required,
            },
            email: {
                required,
                email,
                maxLength: maxLength(50),
            },
            password: {
                required,
                minLength: minLength(8),
            },
            firstName: {
                required,
            },
            lastName: {
                required,
            },
            phoneNumber: {
                numeric,
                minLength: minLength(10),
                maxLength: maxLength(11),
            },
            identitycard: {
                required,
                numeric,
                maxLength: maxLength(12),
                minLength: minLength(9),
            },
            dob: {},
            gender: {
                required,
            },
            address: {},
            university: {},
            rank: {},
            yearGraduated: {
                between: between(2000, 3000),
            },
            skype: {
                maxLength: maxLength(100),
            },
            dateStartWork: {
                required,
            },
            maritalStatus: {},
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
        }
    },
    mounted() {
        this.GetRole()
    },
    watch: {
        idUserGitLab: {
            handler: async function changeSelected(newValue) {
                if(newValue==null) {
                    this.tokenGitLab = "";
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
        showError(message) {
            this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
        },

        showSuccess(message) {
            this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
        },
        async handleSubmit() {
            this.submitted = true
            if (this.v$.$invalid === false) {
                await this.Submit()
            }
        },
        async GetRole() {
            await HTTP.get('Group/getListGroup')
                .then((res) => {
                    this.optionRoles = res.data._Data
                })
                .catch((err) => console.log(err))
        },
        closeAdd() {
            this.$emit('closeAdd')
        },
        async CallApiSubmit(data) {
                try {
                    await HTTP.post('Users/addUser', data).then(async (res) => {
                        switch (res.status) {
                            case HttpStatus.OK:
                                var idUserCreated = res.data.id

                                this.closeAdd()
                                this.$emit('reloadEditToSave')
                                await HTTP.put(
                                    `User_Group/updateUserGroupMultiRoleUpgrade/${idUserCreated}/${parseInt(
                                        checkAccessModule.getUserIdCurrent(),
                                    )}`,
                                    Object.values(this.idGroup),
                                )
                                    .then((response) => {
                                        if (response.data._success) {
                                            this.$emit('reloadpage')
                                            this.resetForm()
                                            this.showSuccess('Thêm người dùng thành công!')
                                        }
                                    })
                                    .catch((error) => {
                                        console.log(error)
                                    })
                                break
                            case HttpStatus.UNAUTHORIZED:
                            case HttpStatus.FORBIDDEN:
                                this.showError('Không có quyền thực hiện thao tác thêm dự án!')
                                break
                            default:
                                this.showError('Lưu không thành công')
                        }
                    })
                } catch (error) {
                    switch (error.code) {
                        case 'ERR_NETWORK':
                            this.showError('Kiểm tra kết nối !')
                            break
                        case 'ERR_BAD_REQUEST':
                            this.showError(error.response.data)
                            break
                        default:
                    }
                }
            },
            async Submit() {
            const data = {
                userCode: this.userCode,
                userPassword: this.password,
                userCreated: parseInt(checkAccessModule.getUserIdCurrent()),
                dateCreated: this.dateCreate,
                email: this.email,
                firstName: this.firstName,
                lastName: this.lastName,
                phoneNumber: this.phoneNumber,
                dOB: this.dob,
                gender: this.gender.code,
                address: this.address,
                university: this.university,
                yearGraduated: this.yearGraduated,
                identitycard: this.identitycard,
                skype: this.skype,
                workStatus: this.workStatus,
                dateStartWork: this.dateStartWork,
                dateLeave: null,
                maritalStatus: this.maritalStatus.code,
                idBranch: this.idBranch,
                rank: this.rank,
                idUserGitLab: this.idUserGitLab,
                tokenGitLab: this.tokenGitLab,
            }
            await this.CallApiSubmit(data)
        },
        resetForm() {
            this.userCode = ''
            this.password = ''
            this.email = ''
            this.userCreate = localStorage.getItem('username')
            this.dateCreate = new Date()
            this.firstName = ''
            this.lastName = ''
            this.phoneNumber = ''
            this.identitycard = ''
            this.dob = ''
            this.gender = ''
            this.address = ''
            this.university = ''
            this.yearGraduated = ''
            this.skype = ''
            this.dateStartWork = ''
            this.maritalStatus = ''
            this.workStatus = 1
            this.submitted = false
            this.idBranch = null
            this.rank = ''
            this.idUserGitLab = null
            this.tokenGitLab = ''
            this.isHaveIdUserGitLab = false;
        },
        backToUser() {
            this.$router.push('/users')
        },
        checkDateOfBirth(dOB) {
            if (dOB == '') {
                this.dOBValidate = false
                return true
            }
            var today = new Date()
            var thisYear = new Date(today).getFullYear()
            var birthYear = new Date(dOB).getFullYear()
            var age = thisYear - birthYear
            if (age >= 18 && age <= 60) {
                this.dOBValidate = false
                return true
            } else {
                this.dOBValidate = true
                return false
            }
        },
        async createTokenPrivate(){
            try{
                if(this.idUserGitLab==null){ 
                    this.isHaveIdUserGitLab = false;
                    return; 
                }
                await TaskService.createUserTokenPrivate(this.idUserGitLab,{
                    "name": "New Token",
                    "scopes": ["api"]
                })
                .then((res) => {
                    this.tokenGitLab = res.data.token;
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
    }
}
</script>

<style lang="scss" scoped>
    .col-2 {
        width: 15% !important;
        margin-left: 0.5rem !important;
    }
</style>
<style>
    .select_project_displayNone {
        display: block;
    }
    .input_disabled {
        background: #ececec !important;
    }
    .btn-right {
        float: right;
        display: inline-flex;
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
<style lang="scss" scoped>
    .p-dropdown {
        width: 100%;
    }
    ::v-deep(.p-input-icon-right > .p-inputtext) {
        height: 36px;
        border-radius: 2px;
    }
</style>
