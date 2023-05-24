<template>
    <Dialog
        header="Chỉnh sửa danh sách website bị chặn"
        :maximizable="true"
        :closable="false"
        position="center"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '60vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
        style="overflow-y: auto"
    >
        <div class="container">
            <form
                style="overflow-y;: auto"
                ref="formAddProject"
                @submit.prevent="() => handleSubmit(!v$.$invalid)"
            >
                <div class="row mb-4">
                    <div class="col-sm-12 col-md-12 ">
                        <div class="card">
                            <div id="headingOne" class="card-header">
                                <button
                                    type="button"
                                    data-bs-toggle="collapse"
                                    data-bs-target="#collapseOne1"
                                    aria-expanded="true"
                                    aria-controls="collapseOne"
                                    class="text-left m-0 p-0 btn btn-link btn-block"
                                    style="width: 100%; text-align: left"
                                >
                                    <h5 class="m-0 p-0">Thêm đường dẫn trang web</h5>
                                </button>
                            </div>
                            <div
                                data-parent="#accordion"
                                id="collapseOne1"
                                aria-labelledby="headingOne"
                                class="collapse show"
                            >
                                <div class="card-body">
                                    <vue3-tags-input
                                        id="BlockingWeb"
                                        :tags="tags"
                                        placeholder="Nhập địa chỉ website bị chặn..."
                                        @on-tags-changed="handleChangeTag"
                                    />
                                    <small class="p-error" v-if="v$.blockingWeb.linkWeb.required.$invalid && isSubmit">
                                        {{ v$.blockingWeb.linkWeb.required.$message.replace('Value', 'Đường dẫn web') }}
                                    </small>
                                    <hr />
                                    <div class="d-flex justify-content-between mt-3">
                                        <small style="font-style: italic">
                                            <span style="color: red">*</span>
                                            Lưu ý: Mỗi từ khóa cách nhau bằng dấu cách 'space'
                                        </small>
                                        <div class="btn-add">
                                            <Button
                                                label="Thêm"
                                                icon="pi pi-check"
                                                type="submit"
                                                class="btn btn-primary custom-button-h"
                                                data-bs-toggle="collapse"
                                                data-bs-target="#collapseExample"
                                                aria-expanded="false"
                                                aria-controls="collapseExample"
                                                autofocus
                                            ></Button>
                                            <!-- <button
                                                class="btn btn-primary "
                                                type="submit"
                                                icon="pi pi-check"
                                                data-bs-toggle="collapse"
                                                data-bs-target="#collapseExample"
                                                aria-expanded="false"
                                                aria-controls="collapseExample"
                                            >
                                                Thêm
                                            </button> -->
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="row mb-4">
                    <div class="col-sm-12 col-md-12 ">
                        <div class="card card-body">
                            <div class="row">
                                <div class="col-sm-12 col-md-12 ">
                                    <DataTable
                                        :value="data"
                                        :rows="5"
                                        ref="dt"
                                        :loading="loading"
                                        showGridlines="true"
                                        responsiveLayout="scroll"
                                        :globalFilterFields="['#', 'id', 'userName', 'name', 'updateAt']"
                                    >
                                        <Column style="width: 80%" field="userName" header="Tên Website" :sortable="true">
                                            <template #body="{ data }">
                                                {{ data.linkBlockingWeb }}
                                            </template>
                                        </Column>

                                        <Column header="Thực thi" class="d-flex justify-content-center">
                                            <template #body="{ data }">
                                                <div class="d-flex justify-content-center">
                                                    <Button
                                                        style="width: 50px !important; background: #ff0000a6"
                                                        icon="pi pi-times"
                                                        class="p-button-sm p-button-secondary custom-button-h"
                                                        @click="deleteData(data.id)"
                                                    />
                                                </div>
                                            </template>
                                        </Column>
                                    </DataTable>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12 col-md-12 ">
                                    <Pagination
                                        v-model:pageNumber="resultPgae.pageNumber"
                                        v-model:pageSize="resultPgae.pageSize"
                                        :totalPages="totalMapPage"
                                        :totalItems="totalItem"
                                        :itemIndex="itemIndex"
                                    />
                                </div>
                            </div>
                        </div>
                        <!-- <div class="d-flex justify-content-end mt-3">
                            <div class="btn-closing"></div>
                            <Button
                                label="Hủy"
                                icon="pi pi-times"
                                class="p-button-sm p-button-secondary"
                                @click="closeDialog()"
                            />
                        </div> -->
                    </div>
                </div>
            </form>
        </div>
        <template #footer>
            <Button
                label="Huỷ"
                icon="pi pi-times"
                class="p-button-secondary p-button-outlined CustomButtonPrimeVue"
                @click="closeDialog()"
                autofocus
            ></Button>
        </template>
    </Dialog>
</template>
<script>
    import jwtDecode from 'jwt-decode'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { BlockingWebDto } from '@/views/EquipmentAndDevices/BlockingWeb.dto'
    import { BlockingWebService } from '@/service/blockingweb.service'
    import { GET_BLOCKING_WEB, DELETE_BLOCKING_WEB, HTTP } from '@/http-common'
    export default {
        async created() {
            await this.loadData()
        },
        props: ['isOpen'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                userRequest: jwtDecode(localStorage.getItem('token')),
                blockingWeb: new BlockingWebDto(),
                isSubmit: false,
                submited: false,
                tags: [],
                data: [],
                resultPgae: {
                    pageSize: 10,
                    pageNumber: 1,
                },
                totalItem: 0,
                totalMapPage: 0,
                itemIndex: 0,
            }
        },
        beforeUpdate() {},
        watch: {
            resultPgae: {
                handler: async function change() {
                    await this.loadData()
                },
                deep: true,
            },
        },
        methods: {
            handleSubmit() {
                this.isSubmit = true
                if (!this.v$.$invalid) {
                    this.handlerAddBlockingWeb()
                }
            },
            async loadData() {
                await HTTP.get(GET_BLOCKING_WEB(this.resultPgae.pageNumber, this.resultPgae.pageSize)).then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.data = res.data._Data
                    console.log(this.data)
                })
                this.loading = false
            },
            async deleteData(id) {
                await HTTP.delete(DELETE_BLOCKING_WEB(id)).then((res) => {
                    this.data = res.data._Data
                    this.loadData()
                    console.log(this.data)
                })
                this.loading = false
            },

            handlerAddBlockingWeb() {
                const addBlocking = {
                    arrayBlockWeb: this.blockingWeb.linkWeb,
                    userCreate: this.userRequest.Id,
                }
                if (addBlocking) {
                    BlockingWebService.handlerRequireBlockingWeb(addBlocking)
                        .then((res) => {
                            if (res.status == 200) {
                                // this.closeDialog()
                                this.resetForm()
                                this.loadData()
                                this.successMessage('Thêm thành công các trang web bị chặn')
                            } else {
                                // this.closeDialog()
                                this.WarningMessage('Có sự cố gì xảy ra đối với hệ thống !')
                            }
                        })
                        .catch(() => {
                            this.WarningMessage('Có sự cố gì xảy ra đối với hệ thống !')
                        })
                }
            },
            resetForm() {
                this.isSubmit = false
                this.blockingWeb.linkWeb = null
            },
            handleChangeTag(tags) {
                this.blockingWeb.linkWeb = tags
            },
            successMessage(mess) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: mess,
                    life: 3000,
                })
            },
            WarningMessage(mess) {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Cảnh báo',
                    detail: mess,
                    life: 2000,
                })
            },
            closeDialog() {
                this.$emit('closeDialogBlocking')
                this.resetForm()
            },
        },
        validations() {
            return {
                blockingWeb: {
                    linkWeb: { required },
                },
            }
        },
    }
</script>
<style lang="scss" scoped>
    .v3ti {
        height: 150px !important;
        display: block;
        border: none !important;
    }
    .v3ti:focus-visible {
        border: none;
        box-shadow: none;
        outline: none !important;
    }
    /* button.p-button.p-component.p-button-sm.p-button-secondary {
        width: 100px !important ;
    } */
    .card.card-body {
        border: none;
        padding: 0;
    }
</style>
