<template>
    <Dialog
        :visible="statusopen"
        header="Chi tiết nhân viên đợt đánh giá"
        modal
        :maximizable="true"
        position="center"
        :style="{ width: '60vw' }"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
    >
        <div class="mb-5">
            <DataTable 
                :value="dataDetail" 
                :loading="isLoading"
                :rowHover="true"  
                showGridlines 
                responsiveLayout="scroll"
            >
                <template #empty> Không tìm thấy dữ liệu. </template>
                <template #loading>
                    <ProgressSpinner />
                </template>

                <Column field="#" header="#" dataType="date">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                
                <Column field="staffReviewTime" header="Đợt đánh giá" sortable >
                    <template #body="{ data }">
                        Đợt {{ data.staffReviewTime}}
                    </template>
                </Column>

                <Column field="fullNameUserCreated" header="Người tạo" sortable>
                    <template #body="{ data }">
                        {{ data.fullNameUserCreated }}
                    </template>
                </Column>

                <Column field="fullNameReviewer" header="Người đánh giá" sortable>
                    <template #body="{ data }">
                        {{ data.fullNameReviewer }}
                    </template>
                </Column>

                <Column field="fullNameStaffReview" header="Nhân viên" sortable>
                    <template #body="{ data }">
                        {{ data.fullNameStaffReview }}
                    </template>
                </Column>

                <Column field="branchName" header="Chi nhánh" sortable>
                    <template #body="{ data }">
                        {{ data.branchName }}
                    </template>
                </Column>

                <Column field="dateReview" header="Ngày đánh giá" sortable>
                    <template #body="{ data }">
                        {{ data.dateReview }}
                    </template>
                </Column>
            </DataTable>
        </div>

        <template #footer>
            
            <Button
                label="Huỷ"
                icon="pi pi-times"
                class="p-button-secondary p-button p-button-icon p-component"
                @click="closeModal"
                enter="closeModal"
            ></Button>
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
                dataDetail: null,
                isLoading: false,
            }
        },
        props: ['statusopen', 'dataRiviewById'],
        methods: {
            closeModal() {
                this.$emit('closeDetailt')
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
            await this.getByIdDetail(this.dataRiviewById.staffReviewTime,this.dataRiviewById.companyBranhId, this.dataRiviewById.userCreated);
        }
    }
</script>

<style lang="scss" scoped>
    ::v-deep(.p-dialog .p-dialog-footer) {
        padding: 1rem 0.5rem 0.7rem 1.5rem !important;
        display: flow-root !important;
    }

</style>