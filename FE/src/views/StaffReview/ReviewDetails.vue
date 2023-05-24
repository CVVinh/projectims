<template>
    <Dialog
        :visible="openDetailsDialog"
        maximizable
        modal
        header="Chi Tiết Phiếu Đánh Giá"
        :closable="false"
    >
        <div class="mb-2 d-flex justify-content-end">
            <!-- Confirm -->
            
            <Button
                icon="pi pi-check"
                class="p-button p-component p-button-success me-2 custom-button-h"
                @click="ConfirmReview()"
                name="confirm"
                :hidden="checkCanOperation('xacnhan')"
                v-if="this.showbutton.confirm"
                v-tooltip.top="'Duyệt đánh giá'"
            />

            <!-- Refuse -->
            <Delete
                icon="pi pi-times"
                class="p-button p-component p-button-danger me-2 custom-button-h"
                @click="openRejectPopup()"
                :hidden="checkCanOperation('tuchoi')"
                v-if="this.showbutton.refuse"
                v-tooltip.top="'Không duyệt đánh giá'"
            />
            
            <!-- Xuất file hình ảnh -->
            <Button
                icon="pi pi-camera"
                class="p-button p-component p-button-secondary custom-button-h"
                :hidden="this.reviewDetails.isConfirm !== true"
                @click="screenShot()"
                v-if="this.showbutton.export"
                v-tooltip.top="'In đánh giá'"
            />
        </div>
        <div ref="elementToCapture">
            <table style="width: 100%; height: 100%" id="table">
                <thead>
                    <tr>
                        <th colspan="2">
                            <div class="container">
                                <div class="row">
                                    <div class="col-sm-12 col-md-3">
                                        <span class="me-2"> Người đánh giá:</span>
                                        <b>{{ this.pmName }}</b>
                                    </div>
                                    <div class="col-sm-12 col-md-3">
                                        <span class="me-2"> Đánh giá nhân viên:</span>
                                        <b>{{ this.staffName }}</b>
                                    </div>
                                    <div class="col-sm-12 col-md-3">
                                        <span class="me-2"> Ngày đánh giá:</span>
                                        <b>{{ this.formatDate(this.reviewDetails.dateCreated) }}</b>
                                    </div>
                                    <div class="col-sm-12 col-md-3">
                                        <span class="me-2">Tổng hiệu suất</span>
                                        <b>{{ parseFloat(this.reviewDetails.totalPerformance).toFixed(2) }}%</b>
                                    </div>
                                </div>
                            </div>
                        </th>
                    </tr>
                    <tr style="background-color: #00b0f0">
                        <th class="text-center" style="width: 250px">Mục đánh giá</th>
                        <th class="text-center" colspan="4">Nội dung đánh giá</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td style="background-color: #00b0f0">Kinh Nghiệm</td>
                        <td>
                            <table id="contentTbl">
                                <tr>
                                    <th class="text-center">Nội dung</th>
                                    <th class="text-center">Thời gian làm</th>
                                    <th class="text-center">Thời gian theo est</th>
                                    <th class="text-center">Hiệu suất</th>
                                </tr>
                                <tr v-for="(item, index) in this.reviewDetails.experiences" :key="index">
                                    <td class="text-center">{{ item.taskName }}</td>
                                    <td class="text-center">{{ item.spend }}</td>
                                    <td class="text-center">{{ item.estimate }}</td>
                                    <td class="text-center">{{ item.performance }}%</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Thành tích nổi bật</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.achievements.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Kiến thức</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.knowledge.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Kỹ năng</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.skill.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Tư Duy Cầu Tiến</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.forwardMindset.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Tư Duy Tích Cực</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.positiveMindset.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Tư Duy Kiên Định</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.steadfastMindset.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Tư Duy Cầu Toàn</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.perfectionistMindset.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Hành Động(biến lời nói thành kết quả)</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.fromTalkToResult.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Hành Vi(Khả năng kết nối)</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.connectToAction.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #ffd966">Sở thích</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.hobbies.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #ffd966">Tính cách</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.personality.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #ffd966">Ý Kiến, đề xuất nguyện vọng và cam kết với Công ty</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.commitmentsForCompany.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #ffd966">Ý Kiến của Lead/ người làm cùng</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.colleagueOpinion.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #ffd966">Ý Kiến phòng HCNS</td>
                        <td>
                            <div class="form-floating">
                                <p v-html="reviewDetails.hrOpinion.replace(/\n/g, '<br>')"></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #f4b084">Kiến nghị / kết luận</td>
                        <td>
                            <b>Ký hợp đồng:</b>
                            <div v-if="this.reviewDetails.reviewResult.assignContract === 1"><p>- 6 tháng</p></div>
                            <div v-if="this.reviewDetails.reviewResult.assignContract === 2"><p>- 12 tháng</p></div>
                            <div v-if="this.reviewDetails.reviewResult.assignContract === 3"><p>- 24 tháng</p></div>
                            <div v-if="this.reviewDetails.reviewResult.assignContract === 4"><p>- 36 tháng</p></div>
                            <div v-if="this.reviewDetails.reviewResult.assignContract === 5"><p>- Vô Thời Hạn</p></div>
                            <div v-if="this.reviewDetails.reviewResult.isTerminate === true"><p>- Hủy Hợp Đồng</p></div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <template #footer>
            <div class="footer_btn">
                <Button
                label="Hủy"
                class="p-button-secondary p-button-outlined CustomButtonPrimeVue"
                autofocus
                icon="pi pi-times"
                @click="closeDetailsDialog()"
                />
                <!-- <Button label="Huỷ" icon="pi pi-times" class="p-button-secondary p-button p-button-icon p-component" @click="closeDetailsDialog()"
                ></Button> -->
            </div>
            
            <!-- <Button label="Yes" icon="pi pi-check" @click="closeDetailsDialog()" /> -->
        </template>
    </Dialog>

    <Dialog v-model:visible="rejectPopup" modal header="Không duyệt phiếu đánh giá" :style="{ width: '50vw' }">
        <div class="container">
            <div class="input-layout w-100">
                <label class="mb-2">Lý do không duyệt <span style="color: red">*</span></label>
                <Textarea v-model="reasonReject" rows="5" class="input form-control" />
            </div>
        </div>
        <template #footer>
            <ButtonCustom  @click="closeRejectPopup()"/>
            <Button label="Có" class="custom-button-h" icon="pi pi-check" @click="RejectReview()" autofocus />
        </template>
    </Dialog>
</template>
<script>
    import { HTTP, HTTP_LOCAL } from '@/http-common'
    import { DateHelper } from '@/helper/date.helper'
    import html2canvas from 'html2canvas'
    import Delete from '@/components/buttons/Delete.vue'
    import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
import { checkAccessModule } from '@/helper/checkAccessModule'
    export default {
        props: [
            'openDetailsDialog',
            'ReviewTicket',
            'dataReviewDetailTicket',
            'usercode',
            'reviewTimes',
            'currentUserId',
            'showbutton',
        ],
        data() {
            return {
                reviewDetails: [],
                pmName: null,
                staffName: null,
                contractTime: null,
                image: null,
                rejectPopup: false,
                showButton : {
                    confirmDetail : true,
                    refuseDetail : true,
                }
            }
        },
        async beforeUpdate() {
            this.reviewDetails = this.dataReviewDetailTicket
            if(this.reviewDetails.reviewerId === Number(checkAccessModule.getUserIdCurrent())){
                this.showButton.confirmDetail = false
                this.showButton.refuseDetail = false
            }
            // await this.getReviewById(this.ReviewTicket.id)
            if (this.ReviewTicket !== null) {
                this.getPMName(this.ReviewTicket.reviewerId)
                this.getStaffName(this.ReviewTicket.staffReview)
            }
        },
        methods: {

            checkCanOperation(nameButton) {
                const name = nameButton.toLowerCase()
                if (name === 'them') {
                }
                if (name === 'sua') {
                   
                }
                if (name === 'xoa') {
                   
                }
                if (name === 'xoanhieu') {
                }
                if (name === 'xuatfile') {
                }
                if (name === 'xacnhan') {
                    if(this.reviewDetails.reviewerId === Number(checkAccessModule.getUserIdCurrent()) && this.reviewDetails.isConfirm === null ){
                        return false
                    }else{
                        return true
                    }
                }
                if (name === 'xacnhannhieu') {
                }
                if (name === 'themthanhvien') {
                }
                if (name === 'tuchoi') {
                    if(this.reviewDetails.reviewerId === Number(checkAccessModule.getUserIdCurrent()) && this.reviewDetails.isConfirm === null){
                        return false
                    }else{
                        return true
                    }
                }
            },
            formatDate(date) {
                return DateHelper.formatDate(date)
            },
            closeDetailsDialog() {
                this.$emit('closeDialog')
            },
            // async getReviewById(id) {
            //     await HTTP_LOCAL.get('StaffReview/GetReviewById/' + id)
            //         .then((res) => {
            //             this.reviewDetails = res.data._Data
            //             console.log('review', this.reviewDetails)
            //             this.reviewDetails.reviewResult.assignContract = this.contractTime
            //             console.log('Contract Time', this.contractTime)
            //         })
            //         .catch((err) => {
            //             console.log(err)
            //         })
            // },
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
            screenShot() {
                html2canvas(this.$refs.elementToCapture).then((canvas) => {
                    this.image = canvas.toDataURL()
                    this.downloadImage()
                })
            },
            downloadImage() {
                const link = document.createElement('a')
                link.download = `${this.usercode}_${this.pmName}_lan${this.reviewTimes}_${this.formatDate(
                    this.reviewDetails.dateCreated,
                )}.png`
                link.href = this.image
                document.body.appendChild(link)
                link.click()
                document.body.removeChild(link)

                console.log(this.reviewDetails)
            },
            ConfirmReview() {
                this.$confirm.require({
                    message: 'Bạn có muốn xác nhận hoàn thành đánh giá này?',
                    header: 'Xác nhận',
                    acceptLabel: "Xác nhận",
                    rejectLabel: "Huỷ",
                    icon: 'pi pi-exclamation-triangle',
                    acceptIcon: 'pi pi-check',
                    rejectIcon:'pi pi-times',
                    acceptClass:'p-button-primary CustomButtonPrimeVue',
                    rejectClass:'p-button-secondary p-button-outlined CustomButtonPrimeVue',
                    accept: () => {
                        HTTP.put(
                            'StaffReview/ConfirmReview/' + this.ReviewTicket.id + '?idAccepted=' + this.currentUserId,
                        )
                            .then((res) => {
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Xác nhận',
                                    detail: 'Xác nhận đánh giá nhân viên thành công',
                                    life: 3000,
                                })
                                this.$emit('closeDialog')
                                this.$emit('getAllReviews')
                                this.$emit('closeTimesReviewDialog')
                            })
                            .catch((err) => {
                                this.$toast.add({
                                    severity: 'error',
                                    summary: 'Lỗi',
                                    detail: 'Xác nhận đánh giá nhân viên không thành công',
                                    life: 3000,
                                })
                            })
                    },
                    reject: () => {
                        // this.$toast.add({
                        //     severity: 'error',
                        //     summary: 'Hủy',
                        //     detail: 'Hủy đánh giá nhân viên',
                        //     life: 3000,
                        // })
                        return;
                    },
                })
            },
            openRejectPopup() {
                this.idTicketReject = this.ReviewTicket.id
                this.rejectPopup = true
            },
            closeRejectPopup() {
                this.rejectPopup = false
                this.reasonReject = ''
            },
            RejectReview() {
                const rejectContent = {
                    idRejecters: this.currentUserId,
                    reason: this.reasonReject,
                }
                this.$confirm.require({
                    message: 'Bạn có muốn không chấp nhận phiếu đánh giá này?',
                    header: 'Xác nhận',
                    acceptLabel: "Xác nhận",
                    rejectLabel: "Huỷ",
                    icon: 'pi pi-exclamation-triangle',
                    acceptIcon: 'pi pi-trash',
                    rejectIcon:'pi pi-times',
                    acceptClass:'p-button-danger CustomButtonPrimeVue',
                    rejectClass:'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                    accept: () => {
                        HTTP.put('StaffReview/RejectReview/' + this.ReviewTicket.id, rejectContent)
                            .then((res) => {
                                console.log(res.data)
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Xác nhận',
                                    detail: 'Xác nhận không chấp nhận phiếu đánh giá nhân viên thành công',
                                    life: 3000,
                                })
                                this.closeRejectPopup()
                                this.$emit('closeDialog')
                                this.$emit('closeTimesReviewDialog')
                                this.$emit('getAllReviews')
                            })
                            .catch((err) => {
                                console.log(err)
                                this.$toast.add({
                                    severity: 'error',
                                    summary: 'Lỗi',
                                    detail: 'Xác nhận không chấp nhận phiếu đánh giá nhân viên không thành công',
                                    life: 3000,
                                })
                            })
                    },
                    reject: () => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Hủy',
                            detail: 'Hủy không chấp nhận phiếu đánh giá nhân viên',
                            life: 3000,
                        })
                    },
                })
                //console.log('data sending_ ticket id: ' + this.idTicketReject + ' ' + 'Content ', rejectContent)
            },
        },
        components: {Delete, ButtonCustom},
    }
</script>
<style lang="scss" scoped>
    .footer_btn {
        padding: 1rem 0.5rem .7rem 1.5rem !important;
    }
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
</style>
