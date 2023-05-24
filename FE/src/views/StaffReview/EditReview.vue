<template>
    <Dialog
        :visible="openEditReviewDialog"
        maximizable
        modal
        header="Cập nhật Phiếu Đánh Giá"
        :style="{ width: '80vw' }"
        :closable="false"
        >
        <table style="width: 100%; height: 100%" id="table">
            <tr>
                <th colspan="2">
                    <div class="container">
                        <div class="row">
                            <div class="col-sm-12 col-md-6 col-lg-3">
                                <div class="row">
                                    <div class="col-12">
                                        <label for="pmName">Người đánh giá<span style="color: red">*</span>:</label>
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-12">
                                        <InputText id="pmName" disabled class =" custom-input-h" type="text" v-model="this.pmName" placeholder="Chọn người đánh giá"/>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-12 col-md-6 col-lg-3">
                                <div class="row">
                                    <div class="col-12">
                                        <label for="pmName">Nhân viên<span style="color: red">*</span>:</label>
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-12">
                                        <InputText disabled class =" custom-input-h" type="text" v-model="this.staffName" placeholder="Chọn nhân viên"/>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-12 col-md-6 col-lg-3">
                                <div class="row">
                                    <div class="col-12">
                                        <label for="pmName">Ngày đánh giá<span style="color: red">*</span>:</label>
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-12">
                                        <Calendar disabled showIcon v-model="editObject.dateCreated" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-12 col-md-6 col-lg-3">
                                <div class="row">
                                    <div class="col-12">
                                        <label for="pmName">Tổng hiệu suất</label>
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-sm-12">
                                        <div class="input-group" >
                                            <input
                                                type="number"
                                                :value="averagePerformance"
                                                disabled
                                                style="width: 87%"
                                            />
                                            <span class="input-group-text bg-primary text-white" >%</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </th>
            </tr>
            <tr style="background-color: #00b0f0">
                <th class="text-center" style="width: 250px">Mục đánh giá</th>
                <th class="text-center" colspan="4">Nội dung đánh giá</th>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Kinh Nghiệm</td>
                <td>
                    <table id="contentTbl">
                        <thead>
                            <tr>
                                <th class="text-center">Nội dung<span style="color: red">*</span></th>
                                <th class="text-center">Thời gian làm<span style="color: red">*</span></th>
                                <th class="text-center">Thời gian theo est<span style="color: red">*</span></th>
                                <th class="text-center">Hiệu suất</th>
                                <th class="text-center" style="width: 50px">
                                    <button class="btn btn-primary me-1" @click="addRowToTable()">
                                        <i class="fa-sharp fa-regular fa-plus"></i>
                                    </button>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in experiences" :key="index">
                                <td>
                                    <input
                                        type="text"
                                        class="form-control"
                                        v-model="item.taskName"
                                        :class="{ invalid: item.invalid }"
                                    />
                                </td>
                                <td>
                                    <div class="input-group">
                                        <input 
                                            type="number" 
                                            class="form-control" 
                                            v-model="item.spend" 
                                            oninput="event.target.value = event.target.value.replace(/[^0-9]*/g,'');" 
                                            :class="{ invalid: item.invalid }"/>
                                        <div class="input-group-append">
                                            <span class="input-group-text bg-primary text-white">Giờ</span>
                                        </div>
                                    </div>
                                </td>
                                <td>
                                    <div class="input-group">
                                        <input 
                                            type="number" 
                                            class="form-control" 
                                            v-model="item.estimate" 
                                            oninput="event.target.value = event.target.value.replace(/[^0-9]*/g,'');" 
                                            :class="{ invalid: item.invalid }"/>
                                        <div class="input-group-append">
                                            <span class="input-group-text bg-primary text-white">Giờ</span>
                                        </div>
                                    </div>
                                </td>

                                <td>
                                    <div class="input-group">
                                        <input
                                            type="number"
                                            class="form-control"
                                            :value="CalculatePerformance(item)"
                                            readonly
                                            disabled
                                        />
                                        <div class="input-group-append">
                                            <span class="input-group-text bg-primary text-white">%</span>
                                        </div>
                                    </div>
                                </td>
                                <td>
                                    <button
                                        class="btn btn-danger"
                                        @click="removeRowFromTable(index)"
                                        :disabled="this.experiences.length === 1"
                                    >
                                        <i class="fas fa-minus"></i>
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Thành tích nổi bật</td>
                <td>
                    <div class="form-floating">
                        <textarea
                            class="form-control"
                            id="floatingTextarea"
                            v-model="editObject.achievements"
                        ></textarea>
                        <label for="floatingTextarea" class="floatingTextarea" :hidden="editObject.achievements !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Kiến thức</td>
                <td>
                    <div class="form-floating">
                        <textarea class="form-control" id="floatingTextarea" v-model="editObject.knowledge"></textarea>
                        <label for="floatingTextarea" class="floatingTextarea" :hidden="editObject.knowledge !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Kỹ năng</td>
                <td>
                    <div class="form-floating">
                        <textarea class="form-control" id="floatingTextarea" v-model="editObject.skill"></textarea>
                        <label for="floatingTextarea" class="floatingTextarea" :hidden="editObject.skill !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Tư Duy Cầu Tiến</td>
                <td>
                    <div class="form-floating">
                        <textarea
                            class="form-control"
                            id="floatingTextarea"
                            v-model="editObject.forwardMindset"
                        ></textarea>
                        <label
                            for="floatingTextarea"
                            class="floatingTextarea"
                            :hidden="editObject.forwardMindset !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Tư Duy Tích Cực</td>
                <td>
                    <div class="form-floating">
                        <textarea
                            class="form-control"
                            id="floatingTextarea"
                            v-model="editObject.positiveMindset"
                        ></textarea>
                        <label
                            for="floatingTextarea"
                            class="floatingTextarea"
                            :hidden="editObject.positiveMindset !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Tư Duy Kiên Định</td>
                <td>
                    <div class="form-floating">
                        <textarea
                            class="form-control"
                            id="floatingTextarea"
                            v-model="editObject.steadfastMindset"
                        ></textarea>
                        <label
                            for="floatingTextarea"
                            class="floatingTextarea"
                            :hidden="editObject.steadfastMindset !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Tư Duy Cầu Toàn</td>
                <td>
                    <div class="form-floating">
                        <textarea
                            class="form-control"
                            id="floatingTextarea"
                            v-model="editObject.perfectionistMindset"
                        ></textarea>
                        <label
                            for="floatingTextarea"
                            class="floatingTextarea"
                            :hidden="editObject.perfectionistMindset !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Hành Động(biến lời nói thành kết quả)</td>
                <td>
                    <div class="form-floating">
                        <textarea
                            class="form-control"
                            id="floatingTextarea"
                            v-model="editObject.fromTalkToResult"
                        ></textarea>
                        <label
                            for="floatingTextarea"
                            class="floatingTextarea"
                            :hidden="editObject.fromTalkToResult !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Hành Vi(Khả năng kết nối)</td>
                <td>
                    <div class="form-floating">
                        <textarea
                            class="form-control"
                            id="floatingTextarea"
                            v-model="editObject.connectToAction"
                        ></textarea>
                        <label
                            for="floatingTextarea"
                            class="floatingTextarea"
                            :hidden="editObject.connectToAction !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Sở thích</td>
                <td>
                    <div class="form-floating">
                        <textarea class="form-control" id="floatingTextarea" v-model="editObject.hobbies"></textarea>
                        <label for="floatingTextarea" class="floatingTextarea" :hidden="editObject.hobbies !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #00b0f0">Tính cách</td>
                <td>
                    <div class="form-floating">
                        <textarea
                            class="form-control"
                            id="floatingTextarea"
                            v-model="editObject.personality"
                        ></textarea>
                        <label for="floatingTextarea" class="floatingTextarea" :hidden="editObject.personality !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #ffd966">Ý Kiến, đề xuất nguyện vọng và cam kết với Công ty<b style="color: red">*</b></td>
                <td>
                    <div class="form-floating">
                        <textarea
                            class="form-control"
                            id="floatingTextarea"
                            v-model="editObject.commitmentsForCompany"
                            :class="{ invalid: isCommitmentsForCompany }"
                        ></textarea>
                        <label
                            for="floatingTextarea"
                            class="floatingTextarea"
                            :hidden="editObject.commitmentsForCompany !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #ffd966">Ý Kiến của Lead/ người làm cùng<b style="color: red">*</b></td>
                <td>
                    <div class="form-floating">
                        <textarea
                            class="form-control"
                            id="floatingTextarea"
                            v-model="editObject.colleagueOpinion"
                            :class="{ invalid: isColleagueOpinion }"
                        ></textarea>
                        <label
                            for="floatingTextarea"
                            class="floatingTextarea"
                            :hidden="editObject.colleagueOpinion !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #ffd966">Ý Kiến phòng HCNS<b style="color: red">*</b></td>
                <td>
                    <div class="form-floating">
                        <textarea class="form-control" id="floatingTextarea" v-model="editObject.hrOpinion" :class="{ invalid: isHROpinion }"></textarea>
                        <label for="floatingTextarea" class="floatingTextarea" :hidden="editObject.hrOpinion !== ''"
                            >Nhập nội dung</label
                        >
                    </div>
                </td>
            </tr>
            <tr>
                <td style="background-color: #f4b084">Kiến nghị / kết luận<b style="color: red">*</b></td>
                <td>
                    <b>Ký hợp đồng:</b>
                    <div class="d-flex mt-2 mb-2">
                        <div v-for="(item, index) in checkbox" :key="item.id">
                            <div class="justify-content-center align-items-center me-3">
                                <input
                                    type="checkbox"
                                    v-bind:id="'assignContract' + index"
                                    :checked="selectedTimeContract.includes(item.value)"
                                    @change="updateSelectedTimeContract(item.value)"
                                    name="assignContract"
                                    :value="item.value"
                                    :key="index"
                                    :disabled="isTerminate === true"
                                    :class="['me-2',{ invalid: isAssignContractTime }]"
                                />
                                <label :for="'assignContract' + index" :class="{ text_color: isAssignContractTime }">{{ item.label }}</label>
                            </div>
                        </div>
                    </div>
                    <div class="align-items-center">
                        <input
                            type="checkbox"
                            value="true"
                            id="isTerminate"
                            v-model="isTerminate"
                            :disabled="selectedTimeContract.length > 0"
                            :class="['me-2',{ invalid: isAssignContractTime }]"
                        />
                        <b for="isTerminate" :class="['me-2', { text_color: isAssignContractTime }]">Hủy Hợp Đồng</b>
                    </div>
                </td>
            </tr>
        </table>

        <template #footer>
            <div class="d-flex justify-content-end">
                <ButtonCustom @click="closeEditReview()" />
                <Button label="Lưu" class="custom-button-h" icon="pi pi-check" @click="editReview()" autofocus />                
            </div> 
        </template
    ></Dialog>
</template>
<script>
    import { HTTP, HTTP_LOCAL } from '@/http-common'
    import { DateHelper } from '@/helper/date.helper'
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
    export default {
        data() {
            return {
                selectedStaff: null,
                staffLists: [],
                listPmLead: null,
                editObject: [],
                //averagePerformance: 0,
                checkbox: [
                    { id: 1, label: '6 tháng', value: 1 },
                    { id: 2, label: '12 tháng', value: 2 },
                    { id: 3, label: '24 tháng', value: 3 },
                    { id: 4, label: '36 tháng', value: 4 },
                    { id: 5, label: 'Vô Thời Hạn', value: 5 },
                ],
                selectedTimeContract: [],
                experiences: [
                    {
                        taskName: '',
                        spend: 0,
                        estimate: 0,
                        performance: 0,
                    },
                ],
                dataEdit: null,
                isTerminate: false,
                pmName: null,
                staffName: null,
                isCommitmentsForCompany: false,
                isColleagueOpinion: false,
                isHROpinion: false,
                isAssignContractTime: false,
            }
        },
        props: ['openEditReviewDialog', 'editObjectId', 'staffList', 'pmLeadList'],
        async beforeUpdate() {
            try {
                await this.getReviewById()
                var data = this.pmLeadList.map((el) => {
                    return (el = Object.assign({}, el))
                })
                this.listPmLead = data
            } catch (error) {
                console.log(error)
            }
        },
        watch: {
            experiences: {
                deep: true,
                handler: function Changes(newValue) {
                    this.updatePerformance()
                },
            },
            experiences: {
                deep: true,
                handler: function (newVal, oldVal) {
                    for (let i = 0; i < newVal.length; i++) {
                        if (
                            newVal[i].taskName !== '' &&
                            newVal[i].spend !== 0 &&
                            newVal[i].spend !== null &&
                            newVal[i].estimate !== null &&
                            newVal[i].estimate !== 0
                        ) {
                            newVal[i].invalid = false
                        }
                    }
                },
            },
            'editObject.commitmentsForCompany': {
                deep: true,
                handler(newVal) {
                    if (
                        this.editObject.commitmentsForCompany !== '' &&
                        this.editObject.commitmentsForCompany !== null
                    ) {
                        this.isCommitmentsForCompany = false
                    }
                },
            },
            'editObject.colleagueOpinion': {
                deep: true,
                handler(newVal) {
                    if (this.editObject.colleagueOpinion !== "" && this.editObject.colleagueOpinion !== null) {
                        this.isColleagueOpinion = false
                    }
                },
            },
            'editObject.hrOpinion': {
                deep: true,
                handler(newVal) {
                    if (this.editObject.HROpinion !== "" && this.editObject.HROpinion !== null) {
                        this.isHROpinion = false
                    }
                },
            },
            selectedTimeContract: {
                deep: true,
                handler(newVal) {
                    if (this.selectedTimeContract.length !== 0) {
                        this.isAssignContractTime = false
                    }
                },
            },
        },
        computed: {
            CalculatePerformance() {
                return (item) => {
                    if (item.spend === null  || item.spend === '' || item.estimate === '' || item.estimate === null) {
                        return 0
                    } else {
                        this.updatePerformance()
                        return ((item.estimate / item.spend) * 100).toFixed(2)
                    }
                }
            },
            averagePerformance() {
                if (this.editObject !== null) return this.calculateTotalPerformance()
            },
        },
        methods: {
            async getReviewById() {
                if (this.editObjectId === null) {
                    return
                } else {
                    try {
                        await HTTP.get('StaffReview/GetReviewById/' + this.editObjectId)
                            .then(async (res) => {
                                this.editObject = res.data._Data

                                this.editObject.dateCreated = this.formatDate(this.editObject.dateCreated)
                                
                                this.selectedTimeContract = this.editObject.reviewResult.assignContract
                                    ? [this.editObject.reviewResult.assignContract]
                                    : []
                                this.experiences = this.editObject.experiences

                                this.getPMName(this.editObject.reviewerId)
                                this.getStaffName(this.editObject.staffReview)
                            })
                            .catch((err) => {
                                console.log(err)
                            })
                    } catch (error) {
                        console.log(err)
                    }
                }
            },
            getPMName(id) {
                if (id !== undefined) {
                    HTTP.get('Users/GetNameOfUserById/' + id)
                        .then((res) => {
                            this.pmName = res.data.fullName
                        })
                        .catch((err) => {
                            console.log(err)
                        })
                }
            },
            getStaffName(id) {
                if (id !== undefined) {
                    HTTP.get('Users/GetNameOfUserById/' + id)
                        .then((res) => {
                            this.staffName = res.data.fullName
                        })
                        .catch((err) => {
                            console.log(err)
                        })
                }
            },
            closeEditReview() {
                this.$emit('closeEditReview')
            },
            formatDate(date) {
                return DateHelper.formatDate(date)
            },
            updatePerformance() {
                this.experiences.forEach((item) => {
                    if (item.spend !== null && item.estimate !== null && item.estimate !== 0) {
                        item.performance = ((item.estimate / item.spend) * 100).toFixed(2)
                    } else {
                        item.performance = 0
                    }
                })
            },
            calculateTotalPerformance() {
                let total = 0.0
                let count = 0

                for (let i = 0; i < this.experiences.length; i++) {
                    const experience = this.experiences[i]

                    if (experience.performance !== null && experience.performance !== 'N/A') {
                        total += parseFloat(experience.performance)
                        count++
                    }
                }

                if (count === 0) {
                    return 0
                } else {
                    this.editObject.totalPerformance = (total / count).toFixed(2)
                    if (this.editObject.totalPerformance === 0) return 0
                    return (total / count).toFixed(2)
                }
            },
            addRowToTable() {
                let newExperience = {
                    taskName: '',
                    spend: '',
                    estimate: '',
                    performance: '',
                }
                this.experiences.push(newExperience)
            },
            removeRowFromTable(index) {
                if (this.experiences.length > 1) {
                    this.experiences.splice(index, 1)
                }
            },
            updateSelectedTimeContract(item) {
                if (this.selectedTimeContract.includes(item)) {
                    this.selectedTimeContract = []
                } else {
                    this.selectedTimeContract = [item]
                }
            },
            async editReview() {
                let hasInvalidExperience = false
                this.experiences.forEach((item) => {
                    if (
                        item.spend === '' ||
                        item.spend === 0 ||
                        item.estimate === '' ||
                        item.estimate === 0 ||
                        item.taskName === ''
                    ) {
                        item.invalid = true
                        hasInvalidExperience = true
                    }
                })
                
                if (this.editObject.commitmentsForCompany.trim() === '' || this.editObject.commitmentsForCompany === null) {
                    this.isCommitmentsForCompany = true
                    hasInvalidExperience = true
                }
                else {
                    hasInvalidExperience = false
                }
                if (this.editObject.colleagueOpinion.trim() === '' || this.editObject.colleagueOpinion === null) {
                    this.isColleagueOpinion = true
                    hasInvalidExperience = true
                }
                else {
                    hasInvalidExperience = false
                }
                if (this.editObject.hrOpinion.trim() === '' || this.editObject.hrOpinion === null) {
                    this.isHROpinion = true
                    hasInvalidExperience = true
                }
                else {
                    hasInvalidExperience = false
                }
                if (this.selectedTimeContract.length === 0 || this.isTerminate === null) {
                    this.isAssignContractTime = true
                    hasInvalidExperience = true
                    console.log("5");
                    console.log(this.isAssignContractTime);

                }
                else {
                    hasInvalidExperience = false
                }

                if (hasInvalidExperience) {
                    return
                }
                var data = {
                    reviewerId: this.editObject.reviewerId,
                    staffReview: this.editObject.staffReview,
                    totalPerformance: this.editObject.totalPerformance,
                    achievements: this.editObject.achievements,
                    knowledge: this.editObject.knowledge,
                    skill: this.editObject.skill,
                    forwardMindset: this.editObject.forwardMindset,
                    positiveMindset: this.editObject.positiveMindset,
                    steadfastMindset: this.editObject.steadfastMindset,
                    perfectionistMindset: this.editObject.perfectionistMindset,
                    fromTalkToResult: this.editObject.fromTalkToResult,
                    connectToAction: this.editObject.connectToAction,
                    hobbies: this.editObject.hobbies,
                    personality: this.editObject.personality,
                    commitmentsForCompany: this.editObject.commitmentsForCompany,
                    colleagueOpinion: this.editObject.colleagueOpinion,
                    HROpinion: this.editObject.hrOpinion,
                    experiences: this.experiences,
                    dateCreated: this.formatDate(this.editObject.dateCreated),
                    ReviewResult: {
                        isTerminate: this.isTerminate,
                        assignContract: this.selectedTimeContract[0],
                    },
                    idstaffReviewDate: this.editObject.idstaffReviewDate,
                }
                this.dataEdit = { ...data }
                HTTP.put('StaffReview/EditReview/' + this.editObjectId, this.dataEdit)
                    .then((res) => {
                        this.closeEditReview()
                        this.toastSuccess()
                        this.$emit('getAllReviews')
                        this.$emit('closeTimesReviewDialog')
                    })
                    .catch((err) => {
                        console.log(err)
                    })
            },
            toastSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Cập nhật thẻ đánh giá thành công',
                    life: 3000,
                })
            },
            toastError() {
                this.$toast.add({
                    severity: 'error',
                    summary: 'Lỗi',
                    detail: 'Tạo mới thẻ đánh giá thất bại ',
                    life: 3000,
                })
            },
        },
        components:{ButtonCustom},
    }
</script>
<style scoped>
    /* ::v-deep(.p-dialog ) {
        padding: 1.2rem 0.5rem 0.7rem 1.5rem !important;
    }
    ::v-deep(.p-dialog .p-dialog-footer) {
        padding: 1.2rem 0.5rem 0.7rem 1.5rem !important;
    }
    ::v-deep(.p-inputnumber-input){
        width: 100%;
    } */
    /* ::v-deep(.p-calendar input) {
        height: 48px !important;
        border-top-left-radius: 6px  !important;
        border-bottom-left-radius: 6px  !important;
    }
    ::v-deep(.p-calendar button) {
        border-top-right-radius: 6px !important;
        border-bottom-right-radius: 6px !important;
    } */

    table,
    th,
    td {
        border: 1px solid black;
        border-collapse: collapse;
    }
    th,
    td {
        padding: 5px;
        text-align: left;
    }
    table#contentTbl {
        margin-top: 2x;
        width: 100%;
    }
    table#contentTbl,
    th,
    td {
        border: 1px solid black;
        border-collapse: collapse;
    }
    .invalid {
        border-color: #e24c4c;
    }
    .text_color {
        color: red;
    }
</style>
