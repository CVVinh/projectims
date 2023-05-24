<template>
    <Dialog
        header="Thêm thu chi"
        :visible="status"
        :closable="false"
        :maximizable="true"
        modal
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
    >
        <form enctype="multipart/form-data" class="container">
            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row" v-if="this.isCollapseCustomer">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label
                                :class="{
                                    'p-error': v$.Datasend.customerName.required.$invalid && isSubmit,
                                    'input-title': true,
                                }"
                                >Tên khách hàng<span style="color: red">*</span></label>
                        </div>
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <Dropdown
                                class="inputdrop custom-input-h"
                                v-model="Datasend.customerName"
                                :options="customerArray"
                                optionLabel="fullName"
                                optionValue="id"
                                placeholder="Chọn khách hàng"
                            />
                            <small class="p-error" v-if="v$.Datasend.customerName.required.$invalid && isSubmit">{{
                                v$.Datasend.customerName.required.$message.replace('Value', 'Customer Name')
                            }}</small>
                        </div>
                    </div>
                    <div class="row mb-3" v-if="checkOffice()">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <div id="add" v-if="this.showButtoncustomer.add">
                                <small style="font-style: bold">
                                    <span style="color: red">*</span>
                                    {{ this.valueTextCustomer }}
                                    <button
                                        type="button"
                                        data-bs-toggle="collapse"
                                        data-bs-target="#collapseOne1"
                                        aria-expanded="true"
                                        aria-controls="collapseOne"
                                        class="text-right m-0 p-0 btn btn-block"
                                        style="text-decoration: none"
                                        @click="clickAddCustomer"                                 
                                    >
                                        <h5 class="m-0 p-0" style="font-size: small; color: dodgerblue">{{ this.valueButtonCustomer }}</h5>
                                    </button>
                                </small>  
                            </div> 

                            <div
                                data-parent="#accordion"
                                id="collapseOne1"
                                aria-labelledby="headingOne"
                                class="collapse hidden"
                            >
                                <div class="card-body add-customer">
                                    <label
                                        :class="{
                                            'p-error': isSubmitCustomer,
                                            'input-title': true,
                                        }"
                                    >Tên khách hàng<span style="color: red">*</span></label>
                                    <InputText type="text" class="custom-input-h" placeholder="Nhập tên khách hàng" v-model="nameCustomer" />
                                    <small class="p-error" v-if="isSubmitCustomer">Tên là được yêu cầu</small>                                                       
                                    <div class="mt-2 mb-3 d-flex justify-content-end">                               
                                        <Button 
                                            label="Thêm" 
                                            size="small"                                       
                                            class="btn btn-primary small-button CustomButtonPrimeVue me-2"                                        
                                            icon="pi pi-plus"
                                            @click="addNewCustomer">
                                        </Button>    
                                        <Button 
                                            label="Hủy"                                          
                                            class="p-button-secondary CustomButtonPrimeVue"  
                                            data-bs-toggle="collapse"  
                                            data-bs-target="#collapseOne1"
                                            aria-controls="collapseOne"
                                            icon="pi pi-times"
                                            style="background-color: white;color: black;"
                                            @click="closeAddCustomer">
                                        </Button>                                  
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row" v-if="this.isCollapsePaidReason">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label
                            :class="{
                                'p-error': v$.Datasend.paidReason.required.$invalid && isSubmit,
                                'input-title': true,
                            }"
                            >Lý do chi trả<span style="color: red">*</span></label
                        >
                        </div>
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <Dropdown
                                class="inputdrop custom-input-h"
                                v-model="Datasend.paidReason"
                                :options="paidReasonArray"
                                optionLabel="name"
                                optionValue="id"
                                placeholder="Lý do chi"
                            />
                            <small class="p-error" v-if="v$.Datasend.paidReason.required.$invalid && isSubmit">{{
                                v$.Datasend.paidReason.required.$message.replace('Value', 'Paid Reason')
                            }}</small> 
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <div id="add" v-if="this.showButtonpaidsReson.add">
                                <small style="font-style: bold">
                                    <span style="color: red">*</span>
                                    {{ this.valueTextPaidReason }}
                                    <button
                                        type="button"
                                        data-bs-toggle="collapse"
                                        data-bs-target="#collapseOne2"
                                        aria-expanded="true"
                                        aria-controls="collapseOne"
                                        class="text-right m-0 p-0 btn btn-block"
                                        style="text-decoration: none"
                                        @click="clickAddPaidReason"
                                    >
                                        <h5 class="m-0 p-0" style="font-size: small; color: dodgerblue">{{ this.valueButtonPaidReason }}</h5>
                                    </button>    
                                </small> 
                            </div>
                            <div
                                data-parent="#accordion"
                                id="collapseOne2"
                                aria-labelledby="headingOne"
                                class="collapse hidden"
                            >
                                <div class="card-body paid-reason">
                                    <label
                                        :class="{
                                            'p-error': isSubmitPaidReason,
                                            'input-title': true,
                                        }"
                                    >Lý do chi trả <span style="color: red">*</span></label>
                                    <InputText type="text" class="custom-input-h" placeholder="Nhập lý do chi trả" v-model="namePaidReason"  />
                                    <small class="p-error" v-if="isSubmitPaidReason">Paid Reason is required</small>
                                    <div class="mt-2 mb-3 d-flex justify-content-end">                              
                                        <Button 
                                            label="Thêm" 
                                            size="small"                                       
                                            class="btn btn-primary small-button CustomButtonPrimeVue me-2"                                        
                                            icon="pi pi-plus"
                                            @click="addPaidReason">
                                        </Button>    
                                        <Button 
                                            label="Hủy"                                          
                                            class="p-button-secondary CustomButtonPrimeVue" 
                                            data-bs-toggle="collapse"  
                                            data-bs-target="#collapseOne2"
                                            aria-controls="collapseOne"
                                            icon="pi pi-times"
                                            style="background-color: white;color: black; "
                                            @click="closeAddPaidReason" >
                                        </Button>   
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label>Dự án</label>
                        </div>
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <Dropdown
                                class="inputdrop custom-input-h"
                                v-model="Datasend.projectId"
                                :options="projectArr"
                                optionLabel="name"
                                optionValue="id"
                                placeholder="Chọn dự án"
                            />
                        </div>
                    </div>
                </div>
                <div class="col-sm-12 col-md-6 col-lg-6">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label
                            :class="{
                                'p-error': v$.Datasend.amountPaid.required.$invalid && isSubmit,
                                'input-title': true,
                            }"
                            >Mức chi (VND)<span style="color: red">*</span></label
                        >
                        </div>
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <InputNumber v-model="v$.Datasend.amountPaid.$model" :min="0" mode="decimal" style="width: 100%"/>
                            <small class="p-error" v-if="v$.Datasend.amountPaid.required.$invalid && isSubmit">{{
                                v$.Datasend.amountPaid.required.$message.replace('Value', 'Amount Paid')
                            }}</small>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-12 col-lg-12">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <label for="note">Nội dung lý do chi</label>
                        </div>
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <Textarea
                                id="note"
                                class="custom-input-h"
                                v-model="Datasend.contentReason"
                                :autoResize="true"
                                rows="5"
                                cols="30"
                                style="width: 100%"
                            />
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12 col-md-12 col-lg-12">
                    <div class="row">
                        <div class="col-sm-12 col-md-12 col-lg-12">
                            <h6>Thêm ảnh</h6>
                        </div>
                        <div class="col-sm-12 col-md-12 col-lg-12 mb-3">
                            <div class="input_file">
                                <input type="file" multiple @change="onFileChange($event)" ref="fileupload" accept="image/*" />
                            </div>

                            <div class="jumbotron p-fluid mt-3 content_box" v-if="isHaveImg">
                                <div class="row">
                                    <div class="col-sm-12 col-md-2 container_img" v-for="(item, index) in images" :key="index" :id="index">
                                        <div class="image_item">
                                            <img class="preview img-thumbnail img_show" v-bind:ref="'image' + parseInt(index)"/>{{
                                                item.name
                                            }}
                                            <div class="middle_img">
                                                <button type="button" @click="removeImage(index)" class="button_del_img">
                                                    &times;
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </form>

        <template #footer>
            <ButtonCustom  @click="closeModal" />       
            <Button
                label="Lưu"
                icon="pi pi-check"
                @click="handleSubmit"
                class="custom-button-h"
                style="margin-bottom: 1px;"
                autofocus
            />   

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
                Datasend: {
                    projectId: 0,
                    customerName: '',
                    amountPaid: null,
                    paidReason: '',
                    paidPerson: '',
                    contentReason: '',
                    isPaid: false,
                    paidDate: null,
                    paidImage: [],
                },
                projectArr: [],
                customerArray: [],
                paidReasonArray: [],
                isSubmit: false,
                images: [],
                isHaveImg: false,
                namePaidReason : '',
                isSubmitPaidReason: false,
                isCollapsePaidReason: true,
                valueTextPaidReason: 'Không có lý do chi trả phù hợp',
                valueButtonPaidReason: 'Thêm mới',
                nameCustomer: '',
                lastNameCustomer: '',
                firstNameCustomer: '',
                genderCustomer: 0,
                phoneNumberCustomer: null,
                emailCustomer: null,
                companyCustomer: null,
                addressCustomer: null,
                workStatusCustomer: 0,
                identityCardCustomer: null,
                accountNumberCustomer: null,
                isSubmitCustomer: false,
                isCollapseCustomer: true,
                valueTextCustomer: 'Không có khách hàng phù hợp',
                valueButtonCustomer: 'Thêm mới',
                showButtoncustomer : {},
                showButtonpaidsReson : {},
            }
        },
        validations() {
            return {
                Datasend: {
                    customerName: { required },
                    amountPaid: { required },
                    paidReason: { required },
                },
            }
        },
        props: ['status', 'optionmodule', 'customerArr', 'paidReasonArr'],

        methods: {
            closeModal() {
                this.isHaveImg = false
                this.isCollapseCustomer = true
                this.isCollapsePaidReason = true
                this.valueTextPaidReason = "Không có lý do chi trả phù hợp"
                this.valueButtonPaidReason = "Thêm mới"
                this.valueTextCustomer = "Không có khách hàng phù hợp"
                this.valueButtonCustomer = "Thêm mới"
                this.$emit('closemodal')
            },

            checkOffice(){
                if(checkAccessModule.getListGroup().includes("2") ||checkAccessModule.getListGroup().includes("1")  ){
                    return true
                }else{
                    return false
                }
            },
            clearform() {
                this.Datasend.projectId = 0
                this.Datasend.customerName = ''
                this.Datasend.amountPaid = null
                this.Datasend.paidReason = ''
                this.Datasend.contentReason = ''
                this.isSubmit = false
                this.paidImage = []
            },
            onFileChange(event) {
                this.images = []
                this.isHaveImg = true
                const selectedFiles = event.target.files

                for (var i = 0; i < selectedFiles.length; i++) {
                    this.images.push(selectedFiles[i])
                }

                for (let i = 0; i < this.images.length; i++) {
                    let reader = new FileReader()
                    reader.addEventListener(
                        'load',
                        function () {
                            this.$refs['image' + parseInt(i)][0].src = reader.result
                        }.bind(this),
                        false,
                    ) //add event listener
                    reader.readAsDataURL(this.images[i])
                }
            },

            // removeImage(index) {
            //     this.images.splice(index, 1)
            //     let imagesRefs = this.$refs
            //     Object.keys(imagesRefs).forEach((key) => {
            //         let refIndex = key.slice(-1) // 1; index: 0
            //         if (refIndex > index) {
            //             imagesRefs[key][0].src = imagesRefs['image' + (refIndex - 1)][0].src
            //         }
            //     })
            // },

            removeImage(index) {
                this.images.splice(index, 1)
                if (this.images.length == 0) {
                    this.$refs.fileupload.value = null
                    this.isHaveImg = false
                }
                let imagesRefs = this.$refs
                Object.keys(imagesRefs).forEach((key) => {
                    let refIndex = key.slice(-1)
                    if (key.includes('image')) {
                        if (parseInt(refIndex) > parseInt(index) && imagesRefs[key][0]) {
                            imagesRefs['image' + (refIndex - 1)][0].src = imagesRefs[key][0].src
                        }
                    }
                })
            },

            async CallApi(fromData) {
                try {
                    const res = await HTTP.post(`Paid`, fromData)

                    switch (res.status) {
                        case HttpStatus.OK:
                            this.clearform()
                            this.showSuccess('Thêm thành công!')
                            this.$emit('reloadpage')
                            break
                        case HttpStatus.UNAUTHORIZED:
                        case HttpStatus.FORBIDDEN:
                            this.showError2('Không có quyền thực hiện thao tác này!')
                            break
                        default:
                            this.showError('Lưu lỗi!')
                    }
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

            async AddPaid() {
                const formData = new FormData()
                formData.append('PaidPerson', checkAccessModule.getUserIdCurrent())
                formData.append('ProjectId', this.Datasend.projectId)
                formData.append('CustomerName', this.Datasend.customerName)
                formData.append('AmountPaid', this.Datasend.amountPaid)
                formData.append('PaidReason', this.Datasend.paidReason)
                formData.append('ContentReason', this.Datasend.contentReason)

                this.images.forEach((item) => {
                    formData.append('paidImage', item)
                })
                await this.CallApi(formData)
            },

            async handleSubmit() {
                try {
                    this.isSubmit = true
                    if (!this.v$.$invalid) {
                        await this.AddPaid()
                        this.closeModal()
                    }
                } catch (err) {
                    console.log(err)
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
            async addPaidReason(){
                if (this.namePaidReason.length > 0) {
                    const dataSend = {
                    "name": this.namePaidReason,
                    "userCreated": checkAccessModule.getUserIdCurrent()
                    }
                    await HTTP.post('PaidReason/CreatePaidReasons', dataSend)
                    .then(res=>{
                        this.$emit('reloadPaidReason')
                        this.isSubmitPaidReason = false
                        this.showSuccess(res.data._Message)
                        this.namePaidReason = ''
                        this.isCollapsePaidReason = false
                    }).catch(err=>{
                        this.showError(err.response.data._Message)
                        this.isCollapsePaidReason = false
                        this.isSubmitPaidReason = false
                        this.namePaidReason = ''
                    })
                }
                else{
                    this.isSubmitPaidReason = true
                }                                 
            },
            clickAddPaidReason(){
                this.isSubmitPaidReason = false               
                if(this.isCollapsePaidReason == true){
                    this.isCollapsePaidReason = false
                    this.namePaidReason = ''
                    this.valueTextPaidReason = "Quay lại"
                    this.valueButtonPaidReason = "chọn lý do chi trả"                                      
                }
                else{
                    this.isCollapsePaidReason = true
                    this.namePaidReason = ''
                    this.valueTextPaidReason = "Không có lý do chi trả phù hợp"
                    this.valueButtonPaidReason = "Thêm mới"
                }
            },
            closeAddPaidReason(){
                this.isCollapsePaidReason = true
                this.namePaidReason = ''
                this.valueTextPaidReason = "Không có lý do chi trả phù hợp"
                this.valueButtonPaidReason = "Thêm mới"
            },
           
            async addNewCustomer(){
                if(this.nameCustomer.length > 0){
                    this.nameCustomer.trim()                   
                    const number = this.nameCustomer.lastIndexOf(" ")
                    this.firstNameCustomer = this.nameCustomer.substring(number+1, this.nameCustomer.length)
                    this.lastNameCustomer = this.nameCustomer.substring(0, number)
                    const dataSend = {
                        "firstName": this.firstNameCustomer,
                        "lastName": this.lastNameCustomer,                     
                        "gender": this.genderCustomer,
                        "phoneNumber": this.phoneNumberCustomer,
                        "email": this.emailCustomer,
                        "company": this.companyCustomer,
                        "address": this.addressCustomer,
                        "workStatus": this.workStatusCustomer,
                        "identityCard": this.identityCardCustomer,
                        "accountNumber": this.accountNumberCustomer,
                        "userCreated": checkAccessModule.getUserIdCurrent()
                    }
                    await HTTP.post('Customer/createCustomer', dataSend)
                    .then(res=>{
                        this.$emit('reloadCustomer')
                        this.isSubmitCustomer = false
                        this.showSuccess(res.data._Message)
                        this.nameCustomer = ''
                        this.isCollapseCustomer = false
                    }).catch(err=>{
                        console.log(err);
                        this.showError(err.response.data._Message)
                        this.isCollapseCustomer = true
                        this.isSubmitCustomer = false
                        this.nameCustomer = ''
                    })
                }
                else{
                    this.isSubmitCustomer = true
                }
                
            },
            clickAddCustomer(){
                this.isSubmitCustomer = false
                if(this.isCollapseCustomer == true){
                    this.isCollapseCustomer = false
                    this.nameCustomer = ''
                    this.valueTextCustomer = "Quay lại"
                    this.valueButtonCustomer = "chọn khách hàng"                 
                }
                else{
                    this.isCollapseCustomer = true
                    this.nameCustomer = ''
                    this.valueTextCustomer = "Không có khách hàng phù hợp"
                    this.valueButtonCustomer = "Thêm mới"
                }                
            },
            closeAddCustomer(){
                this.isCollapseCustomer = true  
                this.nameCustomer = ''
                this.isSubmitCustomer = false
                this.valueTextCustomer = "Không có khách hàng phù hợp"
                this.valueButtonCustomer = "Thêm mới"
            },
        },

        beforeUpdate() {
            checkAccessModule.checkPermissionAction('paidResons',this.showButtonpaidsReson)
            checkAccessModule.checkPermissionAction('customers',this.showButtoncustomer)
            this.projectArr = this.optionmodule
            this.customerArray = this.customerArr
            this.paidReasonArray = this.paidReasonArr
        },
    }
</script>

<style lang="scss" scoped>
   .p-dropdown {
        width: 100%;
    } 
    .p-dialog-footer {
        display: flex;
    }
    /* .p-fileupload.p-fileupload-advanced.p-component {
        padding-right: 10px;
        padding-left: 10px;
    } */
    /* .preview {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100px;
        width: 100px;
    } */

    .input_file {
        border: 1px solid #e5e5e5;
        border-radius: 3px;
    }

    input[type='file']::file-selector-button {
        background-color: #7128fa;
        color: #fff;
        border: 0px;
        border-right: 1px solid #e5e5e5;
        padding: 6px 15px;
        margin-right: 20px;
        border-top-left-radius: 3px;
        border-bottom-left-radius: 3px;
        cursor: pointer;
    }

    input[type='file']::file-selector-button:hover {
        background-color: #591bcc;
        border: 0px;
        border-right: 1px solid #591bcc;
    }

    .content_box {
        box-shadow: -3px 3px 5px -3px #888888, 4px 5px 3px -4px #888888, 4px 5px 2px -5px #888888 inset;
        padding: 10px;
        border-radius: 10px;
    }

    .img_show:hover {
        cursor: pointer;
        box-shadow: 0 0 5px 2px rgba(0, 140, 186, 0.5);
    }

    .container_img {
        position: relative;
    }

    .image_item {
        opacity: 1;
        display: block;
        height: auto;
        transition: 0.5s ease;
        backface-visibility: hidden;
    }

    .middle_img {
        transition: 0.5s ease;
        opacity: 0;
        position: absolute;
        top: 40%;
        left: 45%;
        transform: translate(-50%, -50%);
        -ms-transform: translate(-50%, -50%);
        text-align: center;
    }

    .container_img:hover .image_item {
        opacity: 0.5;
    }

    .container_img:hover .middle_img {
        opacity: 1;
    }

    .button_del_img {
        background-color: #ddd;
        border: none;
        color: black;
        padding: 5px 10px;
        text-align: center;
        text-decoration: none;
        display: inline-block;
        margin: 4px 2px;
        cursor: pointer;
        border-radius: 16px;
        font-size: 16px;
    }

    .button_del_img:hover {
        color: white;
        background-color: red;
    }
    .add-customer{
        display: flex;
        height: 100%;
        margin: 0;
        padding: 0;
        flex-direction: column;
    }
    .paid-reason{
        display: flex;
        height: 100%;
        margin: 0;
        padding: 0;
        flex-direction: column;
    }
   
    ::v-deep(.p-inputnumber-input){
        height: 36px;
        border-radius: 4px;
    }

    @media (min-width: 576px ) and (max-width: 760px ){
        .middle_img {
            transition: 0.5s ease;
            opacity: 0;
            position: absolute;
            left: 16%;
            transform: translate(-50%, -50%);
            -ms-transform: translate(-50%, -50%);
            text-align: center;
        }
    }
   
    @media (min-width: 768px) {
        .middle_img {
            transition: 0.5s ease;
            opacity: 0;
            position: absolute;
            top: 40%;
            left: 45%;
            transform: translate(-50%, -50%);
            -ms-transform: translate(-50%, -50%);
            text-align: center;
        }
    }
   
</style>
