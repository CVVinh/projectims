<template>
    <Dialog
        header="Thêm đợt đánh giá"
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
                    <div class="row">
                        <div class="col-sm-12" >
                            <label
                                :class="{
                                    'p-error': v$.dataSend.staffReviewTime.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Đợt đánh giá<b style="color: red">*</b></label
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
                                class="custom-input-number-h" />

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
                                v-model="v$.dataSend.companyBranhId.$model"
                                :options="branchList"
                                optionValue="idBranch"
                                filter
                                optionLabel="branchName"
                                placeholder="Chọn chi nhánh"
                                class="custom-input-h"
                                :class="{ 'p-invalid': v$.dataSend.companyBranhId.required.$invalid && isSubmit }"
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
                            <Calendar id="dateReview" :showIcon="true" v-model="v$.dataSend.dateReview.$model" :class="{ 'p-invalid': v$.dataSend.dateReview.required.$invalid && isSubmit }"/>
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
                        <div class="col-sm-12">
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
                                class="custom-input-h"
                                :class="{ 'p-invalid': v$.dataSend.reviewerId.required.$invalid && isSubmit }"
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
                                class="custom-input-h"
                                :class="v$.selectedListStaffReviews.required.$invalid && isSubmit ? 'p-invalid' : ' '" />
                        
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
            <div class="row mb-2" >
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
                    reviewerId: null,
                    staffReviewId: null,
                    userCreated: null,
                    dateReview: null,
                    dateCreated: null,
                    detail: '',
                },
                selectedListStaffReviews : [],
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
        props: ['statusopen', 'pmLeadList', 'branchList', 'usersList','listReview'],

        watch: {
            
            dataSend: {
                handler: async function change(newVal) {
                    if(newVal.companyBranhId != null){
                        await this.handleIdReviewTime(newVal.companyBranhId);
                    }
                },
                deep: true,
            },
        },

        methods: {
            reloadPage() {
                this.$emit('reloadPageListTimeReview')
            },
            closeModal() {
                this.$emit('closeAdd')
            },

            checkOffice(){
                if(checkAccessModule.getListGroup().includes("2") ||checkAccessModule.getListGroup().includes("1")  ){
                    return true
                }else{
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

            async handleIdReviewTime(idBranch) {
                await HTTP.get(`StaffReviewTime/IdHandleStaffReviewTime/${idBranch}`)
                    .then((res) => {
                        this.dataSend.staffReviewTime = res.data._Data
                    })
                    .catch((err) => {
                        this.dataSend.staffReviewTime = this.listReview.length+1
                        console.log(err)
                    })
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
                this.isSubmit = false;
            },
            
            async CallApi(fromData) {
                try {
                    const res = await HTTP.post(`StaffReviewTime/CreateMultiNewStaffReviewTime`, fromData).then(response => {
                        switch (response.status) {
                            case HttpStatus.OK:
                                this.clearform()
                                this.reloadPage();
                                this.showSuccess('Thêm thành công!')
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
   /*  .Menu__form {
        display: flex;
    }
    .Menu__form--items_two_ele {
        width: 50%;
        height: 100%;
        padding: 0px 10px;
        display: flex;
        flex-direction: column;
    }
    .Menu__form--items_three_ele  {
        width: 33.33%;
        height: 100%;
        padding: 0px 10px;
        display: flex;
        justify-content: space-between;
    }
    .Menu__form--items-content {
        width: 100%;
        height: 85px;
        display: flex;
        border-radius: 0px;
        flex-direction: column;
        justify-content: center;
    }
    .Menu-form-items-content {
        display: flex;
        justify-content: space-around;
    }

    .country-item-value {
        display: flex;
        height: 30px;
    } */
    
    button.btn.btn-primary{
        float: right;
        margin-right: 5px;
    }
    .p-button-secondary{
        float: right;
    }
</style>
