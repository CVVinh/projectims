<template>
    <Dialog
        header="Cập nhật đợt đánh giá"
        :visible="statusopen"
        :closable="false"
        :maximizable="true"
        modal
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
    >
        <div class="container">
            <div class="row">
                <div class="col-sm-12 col-md-4">
                    <div class="row" >
                        <div class="col-sm-12" >
                            <label
                                :class="{
                                    'p-error': v$.dataSend.staffReviewTime.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Tên đợt đánh giá<b style="color: red">*</b></label
                            >
                        </div>
                    </div> 
                    <div class="row mb-2" >
                        <div class="col-sm-12" >
                            <InputNumber 
                                disabled 
                                v-model="v$.dataSend.staffReviewTime.$model" prefix="Đợt " 
                                :min=0 
                                mode="decimal" 
                                placeholder="Đợt đánh giá" 
                                :class="{ 'p-invalid': v$.dataSend.staffReviewTime.required.$invalid && isSubmit }" 
                                class="custom-input-number-h"/>

                            <!-- <InputText type="text" v-model="v$.dataSend.staffReviewTime.$model" placeholder="Đợt đánh giá"/> -->
                            <small class="p-error" v-if="v$.dataSend.staffReviewTime.required.$invalid && isSubmit">{{
                                v$.dataSend.staffReviewTime.required.$message.replace('Value', 'Review Name')
                            }}</small>
                        </div> 
                    </div> 
                </div>
                <div class="col-sm-12 col-md-4">
                    <div class="row">
                        <div class="col-sm-12" >
                            <label
                                :class="{
                                    'p-error': v$.dataSend.companyBranhId.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Chọn chi nhánh<b style="color: red">*</b></label
                            >
                        </div>
                    </div>
                    <div class="row mb-3" >
                        <div class="col-sm-12" >
                            <Dropdown
                                disabled
                                v-model="v$.dataSend.companyBranhId.$model"
                                :options="branchList"
                                optionValue="idBranch"
                                filter
                                optionLabel="branchName"
                                placeholder="Chọn chi nhánh"
                                :class="{ 'p-invalid': v$.dataSend.companyBranhId.required.$invalid && isSubmit }"
                                class="custom-input-h"
                            />
                            <small class="p-error" v-if="v$.dataSend.companyBranhId.required.$invalid && isSubmit">{{
                                v$.dataSend.companyBranhId.required.$message.replace('Value', 'Branch')
                            }}</small>  
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-4">
                    <div class="row">
                        <div class="col-sm-12" >
                            <label for="dateReview"
                                :class="{
                                    'p-error': v$.dataSend.dateReview.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Ngày đánh giá<b style="color: red">*</b></label
                            >
                        </div>
                    </div>
                    <div class="row mb-3" >
                        <div class="col-sm-12" >
                            <Calendar id="dateReview" showIcon v-model="v$.dataSend.dateReview.$model" :class="{ 'p-invalid': v$.dataSend.dateReview.required.$invalid && isSubmit }"/>
                            <small class="p-error" v-if="v$.dataSend.dateReview.required.$invalid && isSubmit">{{
                                v$.dataSend.dateReview.required.$message.replace('Value', 'Date Review')
                            }}</small> 
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12 col-md-6">    
                    <div class="row">
                        <div class="col-sm-12" >
                            <label
                                :class="{
                                    'p-error': v$.dataSend.reviewerId.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Người đánh giá<b style="color: red">*</b></label
                            >
                        </div>
                    </div>
                    <div class="row mb-3" >
                        <div class="col-sm-12" >
                            <Dropdown
                                v-model="v$.dataSend.reviewerId.$model"
                                :options="pmLeadList"
                                filter
                                optionLabel="fullName"
                                optionValue="id"
                                placeholder="Chọn người đánh giá"
                                :class="{ 'p-invalid': v$.dataSend.reviewerId.required.$invalid && isSubmit }"
                                class="custom-input-h"
                            />
                            <small class="p-error" v-if="v$.dataSend.reviewerId.required.$invalid && isSubmit">{{
                                v$.dataSend.reviewerId.required.$message.replace('Value', 'Reviewer')
                            }}</small>   
                        </div> 
                    </div> 
                </div>               
                <div class="col-sm-12 col-md-6">
                    <div class="row">
                        <div class="col-sm-12" >
                            <label for="idUserList"
                                :class="{
                                    'p-error': v$.selectedListStaffReviews.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Nhân viên được đánh giá<b style="color: red">*</b></label
                            >
                        </div>
                    </div>
                    <div class="row mb-3" >
                        <div class="col-sm-12" >
                            <MultiSelect  
                                filter
                                v-model="v$.selectedListStaffReviews.$model" 
                                :options="usersList" 
                                optionLabel="fullName" 
                                optionValue="id"
                                placeholder="Chọn nhân viên" 
                                id="idUserList"
                                :class="v$.selectedListStaffReviews.required.$invalid && isSubmit ? 'p-invalid' : ' '" 
                                class="custom-input-h"/>
                        
                            <small class="p-error" v-if="v$.selectedListStaffReviews.required.$invalid && isSubmit">{{
                                v$.selectedListStaffReviews.required.$message.replace('Value', 'Staffs')
                            }}</small>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row" >
                <div class="col-sm-12" >
                    <label for="note">Ghi chú</label>
                </div>
            </div>
            <div class="row mb-3" >
                <div class="col-sm-12" >
                    <Textarea
                        id="note"
                        v-model="this.dataSend.detail"
                        :autoResize="true"
                        rows="5"
                        cols="30"
                        style="width: 100%"
                        class="custom-input-h"
                    />
                </div>
            </div>
        </div>
        <template #footer>
            <button-custom
                class="me-2"
                @click="closeModal()"
            />
            <!-- <Button
                label="Huỷ"
                icon="pi pi-times"
                class="p-button-secondary p-button-icon custom-button-h"
                style="background: white; color: black;"
                @click="closeModal()"
            ></Button> -->
            <Button label="Lưu" icon="pi pi-check" class="p-button-primary p-button-icon custom-button-h" @click="handleSubmit"></Button>
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
    import { checkAccessModule } from '@/helper/checkAccessModule';
    import { NO } from '@vue/shared'
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'

    export default {
        components: { ButtonCustom },
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                dataSend: {
                    staffReviewTime: null,
                    companyBranhId: null,
                    branchName: '',
                    reviewerId: null,
                    staffReviewId: null,
                    fullNameReviewer: '',
                    fullNameUserCreated: '',
                    userCreated: null,
                    dateReview: null,
                    dateCreated: null,
                    detail: '',
                },
                idReviewTime: null,
                idBranch: null,
                idOffice: null,

                selectedListStaffReviews : [],
                dataDetail: [],
                isSubmit: false,
            }
        },
        validations() {
            return {
                dataSend: {
                    staffReviewTime: { required },
                    companyBranhId: { required },
                    reviewerId: { required },
                    dateReview: { required },
                },
                selectedListStaffReviews: { required },
            }
        },
        props: ['statusopen', 'pmLeadList', 'branchList', 'usersList', 'dataRiviewById'],

        methods: {
            reloadPage() {
                this.$emit('reloadPageListTimeReview')
            },
            closeModal() {                
                this.$emit('closeEdit');
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

            clearform() {
                this.dataSend.staffReviewTime = null;
                this.dataSend.companyBranhId = null;
                this.dataSend.reviewerId = null;
                this.dataSend.staffReviewId = null;
                this.dataSend.userCreated = null;
                this.dataSend.dateReview = null;
                this.dataSend.dateCreated = null;
                this.dataSend.detail = '';
                this.listStaffReviews = [];
                this.selectedListStaffReviews = [];
                this.dataDetail = [],
                this.isSubmit = false;
            },

            async CallApi(fromData) {
                try {
                    await HTTP.put(`StaffReviewTime/UpdateMultiNewStaffReviewTime/${this.idReviewTime}/${this.idBranch}/${this.idOffice}`, fromData).then(response => {
                        switch (response.status) {
                            case HttpStatus.OK:
                                this.reloadPage();
                                this.showSuccess('Cập nhật thành công!')
                                //this.clearform()
                                break
                            case HttpStatus.UNAUTHORIZED:
                            case HttpStatus.FORBIDDEN:
                                this.showError('Không có quyền thực hiện thao tác này!')
                                break
                            default:
                                this.showError('Lưu lỗi!')
                        }
                    })
                } catch (error) {
                    switch (error.code) {
                        case 'ERR_NETWORK':
                            this.showError('Kiểm tra kết nối!')
                            break
                        case 'ERR_BAD_REQUEST':
                            console.log(error.response.data)
                            break
                        default:
                    }
                }
            },
           
            async AddReviewTime() {
                const objData = {
                    staffReviewTime : this.dataSend.staffReviewTime,
                    companyBranhId : this.dataSend.companyBranhId,
                    reviewerId : this.dataSend.reviewerId,
                    detail : this.dataSend.detail,
                    dateReview : this.dataSend.dateReview,
                    userCreated : checkAccessModule.getUserIdCurrent(),
                    listStaffReviews : this.selectedListStaffReviews,
                }
                await this.CallApi(objData);
            },

            async handleSubmit() {
                try {
                    this.isSubmit = true
                    if (this.v$.$invalid === false) {
                        await this.AddReviewTime();
                        this.closeModal();
                    }
                } catch (err) {
                    console.log(err);
                }
            },
            async getByIdDetail(idStaffReviewTime, idBranch, idOffice) {
                this.isLoading = true;
                await HTTP.get(`StaffReviewTime/GetByIdOfficePmOrLeadNewStaffReviewTime/${idStaffReviewTime}/${idBranch}/${idOffice}`)
                    .then((res) => {
                        this.dataDetail = res.data._Data;
                    })
                    .catch((err) => {
                        console.log(err)
                    })
                    .finally(() => {
                        this.isLoading = false;
                    })
            },
        },

        async beforeUpdate() {
            this.selectedListStaffReviews = [];
            this.dataDetail = [];
            if(this.dataRiviewById != null){
                this.dataSend = this.dataRiviewById;
                this.idReviewTime = this.dataRiviewById.staffReviewTime;
                this.idBranch = this.dataRiviewById.companyBranhId;
                this.idOffice = this.dataRiviewById.userCreated;

                await this.getByIdDetail(this.dataRiviewById.staffReviewTime,this.dataRiviewById.companyBranhId, this.dataRiviewById.userCreated);
                this.dataDetail.map(ele => {
                    this.selectedListStaffReviews.push(ele.staffReviewId);
                });
            }
        },
    }
</script>

<style lang="scss" scoped>
    .p-dropdown {
        width: 100%;
    } 
    ::v-deep(.p-dialog .p-dialog-footer) {
        padding: 1rem 0.5rem 0.7rem 1.5rem !important;
        display: flow-root !important;
    }
    .p-button-secondary{
        float: right;
    }
</style>
