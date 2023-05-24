<template>
    <Dialog
        header="Chi tiết thu chi"
        :visible="status"
        :closable="false"
        :maximizable="true"
        position="center"
        modal
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '60vw' }"
    >
        <div class="detail__content">
            <div class="detail__content-box box-left">
                <div
                    class="detail__content-box"
                    :style="[
                        {
                            background:
                            this.Datasend.isAccept == 1
                                    ? 'green'
                                    : this.Datasend.isAccept == 2
                                    ? 'red'
                                    : 'orange'
                        },
                    ]"
                >
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text detail__content-box-items-format-text">
                            <b>{{
                                this.Datasend.isAccept == 1
                                    ? 'Đã Thanh Toán'
                                    : this.Datasend.isAccept == 2
                                    ? 'Từ chối thanh toán'
                                    : 'Chưa thanh toán'
                            }}</b>
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top">
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text">
                            <b><i class="pi pi-id-card p-button-icon"></i> Người chi tiêu:</b>
                            {{ this.Datasend.paidPersonName }}
                        </div>
                    </div>

                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="pi pi-user-edit"></i> Người xác nhận:</b> {{ this.Datasend.personConfirmName }}
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top">
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text">
                            <b><i class="pi pi-users p-button-icon"></i> Khách hàng:</b>
                            {{ this.Datasend.customerFullName }}
                        </div>
                    </div>

                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-notepad"></i> Dự án:</b> {{ this.Datasend.nameProject }}
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top">
                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="p-confirm-dialog-icon pi pi-info-circle"></i> Lý do chi:</b>
                            {{ Datasend.paidReasonName }}
                        </div>
                    </div>
                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-wallet"></i> Mức chi:</b> {{ this.Datasend.amountPaidName }}
                        </div>
                    </div>

                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-time-five"></i> Ngày Tạo:</b> {{ Datasend.createDate }}
                        </div>
                    </div>
                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-time-five"></i> Ngày Chi:</b> {{ Datasend.paidDate }}
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top detail__content-box-size_content_reason">
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text">
                            <b><i class="p-confirm-dialog-icon pi pi-book"></i> Nội dung chi:</b>
                            {{ Datasend.contentReason }}
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top detail__content-box-size_content_reason" v-if="Datasend.reasonRefusal != null">
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text">
                            <b><i class="p-confirm-dialog-icon pi pi-book"></i> Lý do từ chối:</b>
                            {{ Datasend.reasonRefusal }}
                        </div>
                    </div>
                </div>
            </div>

            <div class="detail__content-box box-right">
                <div v-if="Datasend.paidImages.length > 0">
                    <Galleria
                        :value="dataImgDetail"
                        :responsiveOptions="responsiveOptions"
                        :numVisible="3"
                        :circular="true"
                        :showItemNavigators="true"
                        :showItemNavigatorsOnHover="true"
                        :showIndicators="true"
                        :showIndicatorsOnItem="true"
                    >
                        <template #item="slotProps">
                            <img
                                :src="slotProps.item.itemImageSrc"
                                :alt="slotProps.item.alt"
                                style="width: 100%; display: block"
                            />
                        </template>
                        <template #thumbnail="slotProps">
                            <img
                                :src="slotProps.item.thumbnailImageSrc"
                                :alt="slotProps.item.alt"
                                style="display: block"
                            />
                        </template>
                    </Galleria>
                </div>
                <div v-else>
                    <img src="@/assets/noImage2.png" alt="anh" class="no_image" />
                </div>
            </div>
        </div>

        <template #footer>

            <Button label="Hủy" class="p-button-secondary p-button-outlined CustomButtonPrimeVue" autofocus icon="pi pi-times" @click="closeModal" enter="closeModal"/>
            <Button  label="Thanh toán" class="p-button-primary CustomButtonPrimeVue" autofocus icon="pi pi-check" @click="confirmPayment" v-if="Datasend.isAccept == 0"/>

        </template>
    </Dialog>
</template>

<script>
    import { GET_LIST_PAID, HTTP, HTTP_LOCAL, GET_PROJECT_BY_ID, GET_USER_BY_ID, HTTP_API_GITLAB } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { DateHelper } from '@/helper/date.helper'

    export default {
        data() {
            return {
                Datasend: {
                    projectId: '',
                    customerName: '',
                    amountPaid: 0,
                    paidReason: '',
                    contentReason: '',
                    paidPerson: 0,
                    isAccept: 0,
                    paidDate: null,
                    paidImages: [],
                    paidPersonName: null,
                    personConfirmName: null,
                    customerFullName: null,
                    paidReasonName: null,
                    nameProject: null,
                    amountPaidName: null,
                    createDate: null,
                },
                dataImgDetail: [],
                responsiveOptions: [
                    {
                        breakpoint: '1024px',
                        numVisible: 4,
                    },
                    {
                        breakpoint: '960px',
                        numVisible: 3,
                    },
                    {
                        breakpoint: '768px',
                        numVisible: 2,
                    },
                    {
                        breakpoint: '560px',
                        numVisible: 1,
                    },
                ],
            }
        },
        props: ['status', 'dataDetail'],
        methods: {
            
            closeModal() {
                this.imagesOld = []
                this.images = []
                this.$emit('closemodal')
            },

            confirmPayment() {
                this.$emit('confirmPayment', this.Datasend)
            },
        },

        async beforeUpdate() {
            //beforeUpdate
            this.dataImgDetail = []
            this.Datasend = []
            if (this.dataDetail != null) {
                this.Datasend = this.dataDetail

                if (this.Datasend.paidImages.length > 0) {
                    this.Datasend.paidImages.forEach((item) => {
                        var imgObj = {
                            itemImageSrc: item.imagePath,
                            thumbnailImageSrc: item.imagePath,
                            alt: 'Image ' + item.imageId,
                        }
                        this.dataImgDetail.push(imgObj)
                    })
                }
            }
        },
    }
</script>

<style scoped lang="scss">
     .p-dialog {
        .p-dialog-footer {
            padding: 1rem 0.5rem 0.7rem 0.5rem !important;
            display: flow-root !important; 
        }
    }
    .detail__content {
        display: flex;
        justify-content: space-between;
        padding: 20px;
        margin-bottom: 20px;
        border-radius: 15px;
        box-shadow: rgba(0, 0, 0, 0.02) 0px 1px 3px 0px, rgba(27, 31, 35, 0.15) 0px 0px 0px 1px;

        .detail__content-box {
            display: flex;
            flex-direction: column;
            box-shadow: rgba(6, 24, 44, 0.4) 0px 0px 0px 2px, rgba(6, 24, 44, 0.65) 0px 4px 6px -1px,
                rgba(255, 255, 255, 0.08) 0px 1px 0px inset;
            padding: 10px;
            border-radius: 10px;

            .detail__content-box-items {
                width: 100%;

                .detail__content-box-items-text {
                    font-size: 18px;
                }

                .detail__content-box-items-format-text {
                    color: white;
                    text-align: center;
                }

                .top {
                    margin-top: 10px;
                }
            }
        }

        .detail__content-box-top {
            margin-top: 15px;
        }

        .detail__content-box-size_content_reason {
            min-height: 100px;
        }

        .box-left {
            flex: 35%;
        }

        .box-right {
            flex: 65%;
            margin-left: 15px;
        }
    }

    .no_image {
        border: 1px solid #ddd;
        border-radius: 15px;
    }

    @media (max-width: 500px) {
        .detail__content {
            flex-direction: column;

            .box-left,
            .box-right {
                flex: 100%;
            }
            .box-right {
                margin-top: 10px;
            }
        }
    }
</style>
