<template>
    <Dialog :visible="this.show" :closable="false" modal :maximizable="true" position="center">
        <template #header>
            <h3>Chi tiết thông tin thẻ OT</h3>
        </template>
        <div class="container-fluid ">
            <div class="row p-3 detail__content">
                <div class="col-sm-12 col-md-4 detail__content-box">
                    <div class="detail__content-box  mb-3 text-white"
                        :style="{
                            backgroundColor:
                                this.OTS.x.status === 0
                                    ? 'Orange'
                                    : this.OTS.x.status === 1
                                    ? 'green'
                                    : this.OTS.x.status === 2
                                    ? 'gray'
                                    : this.OTS.x.status === 3
                                    ? 'red'
                                    : '',
                            }"
                    >
                        <div class="row ">
                            <div class="col-sm-12 text-center">
                                <b>
                                    {{
                                        this.OTS
                                            ? this.OTS.x.status === 0
                                                ? 'Đang chờ'
                                                : this.OTS.x.status === 1
                                                ? 'Xác nhận'
                                                : this.OTS.x.status === 2
                                                ? 'Đã từ chối'
                                                : this.OTS.x.status === 3
                                                ? 'Đã xóa'
                                                : ''
                                            : ''
                                    }}
                                </b>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <b>Ngày tạo: </b><span>{{ this.OTS ? getFormattedDate(new Date(this.OTS.x.dateCreate)) : null }}</span>
                        </div>
                    </div>
                    <div class="row ">
                        <div class="col-sm-12">
                            <b>Người quản lý: </b><span>{{ this.OTS ? this.OTS.nameUserUpdate : null }}</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <b>Người xác nhận: </b><span>{{ this.OTS ? this.OTS.nameUserUpdate : null }}</span>
                        </div>
                    </div>
                </div>

                <div class="col-sm-12 col-md-4 detail__content-box">
                    <div class="row">
                            <div class="col-sm-12">
                                <b>Dự án: </b><span>{{ this.OTS ? this.OTS.name : null }}</span>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-12">
                                <b>Mô tả: </b><span>{{ this.OTS ? this.OTS.x.description : null }}</span>
                            </div>
                        </div>
                </div>

                <div class="col-sm-12 col-md-4 detail__content-box ">
                    <div class="row">
                        <div class="col-sm-12">
                            <b>Nhân viên tăng ca: </b><span>{{ this.OTS ? this.OTS.nameUser + ' ' : null }}</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <b>Ngày tăng ca: </b><span>{{ this.OTS ? getFormattedDate(new Date(this.OTS.x.date)) : null }}</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <b>Thời gian tăng ca: </b>
                            <span>
                                {{
                                this.OTS
                                    ? this.OTS.x.realTime.charAt(0) === '0' && this.OTS.x.realTime.charAt(1) === '0'
                                        ? this.OTS.x.realTime + ' phút'
                                        : this.OTS.x.realTime + ' giờ'
                                    : ''
                                }}
                                {{ this.OTS ? '(' + this.OTS.x.start + 'h ->' + this.OTS.x.end + 'h)' : null }}
                            </span>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <template #footer>
            <Button
                label="Hủy"
                class="p-button-secondary p-button-outlined CustomButtonPrimeVue"
                autofocus
                icon="pi pi-times"
                @click="$emit('close')"
            />
            <Button
                label="Hủy duyệt"
                class="p-button-danger CustomButtonPrimeVue"
                autofocus
                icon="pi pi-trash"
                @click="accept(false, this.OTS.x.id, this.OTS.x.leadCreate)"
                v-if="(this.isPm && this.OTS.x.status === 0) || (isAdmin() && this.OTS.x.status === 0)"
            />
            <Button
                label="Lưu"
                class="p-button-primary CustomButtonPrimeVue"
                autofocus
                icon="pi pi-check"
                @click="accept(true, this.OTS.x.id, this.OTS.x.leadCreate)"
                v-if="(this.isPm && this.OTS.x.status === 0) || (isAdmin() && this.OTS.x.status === 0)"
            />
        </template>
    </Dialog>
    <Dialog
        header="Vui lòng nhập lý do!"
        :visible="displayDialog2"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '30vw' }"
        :modal="true"
    >
        <Textarea v-model="reason" style="margin: auto; width: 100%; height: 100%"></Textarea>
        <p v-if="entered" class="p-error">Lý do bắt buộc nhập!</p>
        <template #footer>
            <Button
                label="Hủy"
                class="p-button-secondary p-button-outlined CustomButtonPrimeVue"
                autofocus
                icon="pi pi-times"
                @click="this.displayDialog2 = false"
            />
            <Button
                label="Lưu"
                class="p-button-primary CustomButtonPrimeVue"
                autofocus
                icon="pi pi-check"
                @click="onSubmit(this.OTS.x.leadCreate)"
                v-if="(this.isPm && this.OTS.x.status === 0) || (isAdmin() && this.OTS.x.status === 0)"
            />
        </template>
    </Dialog>
</template>

<script>
import { HTTP } from '@/http-common'
import jwtDecode from 'jwt-decode'
import ButtonCustom from '@/components/buttons/ButtonCustom.vue'
import { checkAccessModule } from '@/helper/checkAccessModule'

export default {
    components: { ButtonCustom },
    props: ['show', 'OTS', 'isPm'],
    data() {
        return {
            display: true,
            Accept: true,
            Token: null,
            reason: '',
            displayDialog2: false,
            lead: null,
            id: null,
            entered: false,
        }
    },
    methods: {
        isAdmin() {
            if (checkAccessModule.getListGroup().includes('1')) return true
            else return false
        },
        getFormattedDate(date) {
            var year = date.getFullYear()

            var month = (1 + date.getMonth()).toString()
            month = month.length > 1 ? month : '0' + month

            var day = date.getDate().toString()
            day = day.length > 1 ? day : '0' + day

            return day + '-' + month + '-' + year
        },
        accept(accepted, id, lead) {
            let user = jwtDecode(localStorage.getItem('token'))
            this.PM = user.Id
            if (accepted) {
                this.status = 1
                HTTP.put('OTs/acceptOT', { id: id, status: this.status, pm: this.PM })
                    .then((res) => {
                        this.$emit('alert')
                        this.$emit('close')
                    })
                    .catch((err) => {
                        console.log(err)
                    })

                setTimeout(() => {
                    this.$emit('reloadAPI')
                }, 500)
            } else {
                this.displayDialog2 = true
                this.lead = lead
                this.id = id
            }
        },
        onSubmit() {
            if (this.reason != null && this.reason != '') {
                HTTP.put('OTs/acceptOT', { id: this.id, status: 2, PM: this.PM, note: this.reason }).then(
                    async (res) => {
                        if (res.status == 200) {
                            this.displayDialog2 = false
                            this.$emit('alert')
                            await this.$emit('reloadAPI')
                            this.$emit('close')
                        }
                    },
                )
            } else this.entered = true
        },
    },
}
</script>

<style lang="scss" scoped>
.detail__content {
   
    border-radius: 15px;
    box-shadow: rgba(0, 0, 0, 0.02) 0px 1px 3px 0px, rgba(27, 31, 35, 0.15) 0px 0px 0px 1px;
}

.detail__content-box {
    box-shadow: rgba(6, 24, 44, 0.4) 0px 0px 0px 2px, rgba(6, 24, 44, 0.65) 0px 4px 6px -1px,
        rgba(255, 255, 255, 0.08) 0px 1px 0px inset;
    padding: 10px;
    border-radius: 10px;
}
.box-left {
    max-width: 400px;
}
.detail__content {
    display: flex;
}

.detail__content-box-items {
    padding: 5px;
    width: 100%;
}
.detail__content-box-items-text {
    font-size: 18px;
}

.title-text {
    color: #495057;
    min-width: 60px;
}
.content-text {
    margin-left: 5px;
}
</style>
