<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">THIẾT BỊ</li>
                        </ol>
                    </nav>
                </div>
            </div>

            <nav class="navbar navbar-expand-lg bg-light navbar-light">
                <div class="container-fluid">
                    <button class="navbar-toggler mb-2 mt-2 custom-input-h" type="button" data-bs-toggle="collapse" data-bs-target="#collapsibleNavbar">
                        <span class="navbar-toggler-icon"></span>
                    </button>
                    <div class="collapse navbar-collapse mt-2" id="collapsibleNavbar">
                        <ul class="navbar-nav me-auto">
                            <li class="nav-item me-2 mb-2 ">
                                <Button
                                    class="p-button-sm custom-button-h"
                                    @click="handlerAddBlockingWeb()"
                                    icon="pi pi-plus"
                                    label="Chỉnh sửa danh sách website bị chặn"
                                ></Button>
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2 ">
                                <Button
                                    type="button"
                                    icon="pi pi-filter-slash "
                                    class="custom-reload-h"
                                    @click="handlerReload()"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <InputText
                                    class="custom-input-h"
                                    type="text"
                                    v-model="name"
                                    placeholder="Nhập tên nhân viên..."
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        :value="dataEquipment"
                        :rows="5"
                        ref="dt"
                        :loading="loading"
                        showGridlines="true"
                        responsiveLayout="scroll"
                        :globalFilterFields="['#', 'id', 'userName', 'name', 'updateAt']"
                    >
                        <template #emty>Không tìm thấy</template>
                        <Column field="#" header="No.">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="userName" header="Mã nhân viên" :sortable="true">
                            <template #body="{ data }">
                                {{ data.userName }}
                            </template>
                        </Column>
                        <Column field="name" header="Tên nhân viên" :sortable="true">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column field="mechineName" header="Tên thiết bị" :sortable="true">
                            <template #body="{ data }">
                                {{ data.infoDevice.mechineName }}
                            </template>
                        </Column>
                        <Column field="updateAt" header="Thời gian cập nhật">
                            <template #body="{ data }">
                                {{ formartDate(data.updateAt) }}
                            </template>
                        </Column>

                        <Column header="Thực thi">
                            <template #body="{ data }">
                                <Button
                                    @click="handlerDetailsDevice(data)"
                                    class="custom-button-h"
                                    icon="pi pi-eye"
                                    v-tooltip.top="'Xem chi tiết'"
                                />
                                <!-- <Button
                                            @click="handlerRequesDevice(data)"
                                            class="p-button-sm p-button-success mt-1 me-2"
                                            icon="pi pi-send"
                                        /> -->
                            </template>
                        </Column>
                    </DataTable>
                </div>
            </div>
            <div class="row">
                <div class="col-12">
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
        <DialogDetailDevice
            :isOpen="this.isOpenDialogDevice"
            :selectedDevice="{ ...deviceDetail }"
            @closeDialogDevice="closeDialogDetailDevice()"
        />
        <DialogAddBlockingWeb
            :isOpen="this.isOpenDialogBlockingWeb"
            @closeDialogBlocking="closeDialogAddBlockingWeb()"
        />
    </LayoutDefaultDynamic>
</template>
<script>
import dayjs from 'dayjs'
import DialogDetailDevice from './DialogDetailDevice.vue'
import DialogAddBlockingWeb from './DialogAddBlockingWeb.vue'
import { DeviceService } from '@/service/device.service'
import { HTTP, GET_USER_BY_ID } from '@/http-common'
import { OperatingSystem } from './OperatingSystem'
import { checkAccessModule } from '@/helper/checkAccessModule'
import router from '@/router'
export default {
    async created() {
        if(checkAccessModule.isAdmin()){
            await this.getAllDevice()
        }else{
            alert("Bạn không có quyền truy cập ")
            this.$router.push("/")
        }
        
    },
    data() {
        return {
            loading: true,
            dataEquipment: [],
            isOpenDialogDevice: false,
            deviceDetail: [],
            name: '',
            operatingSystem: 19000,
            arrOperatingSystem: OperatingSystem,
            tagsDevice: [],
            isOpenDialogBlockingWeb: false,
            resultPgae: {
                pageSize: 10,
                pageNumber: 1,
            },
            totalItem: 0,
            totalMapPage: 0,
            itemIndex: 0,
        }
    },
    watch: {
        name: {
            handler: async function Change(newText) {
                await this.handlerSearchByName()
            },
        },
        deep: true,
        resultPgae: {
            handler: async function change() {
                if (this.name != '') {
                    await this.handlerSearchByName()
                } else {
                    await this.getAllDevice()
                }
            },
            deep: true,
        },
    },
    methods: {
        formartDate(date) {
            const fmDate = new Date(date)
            return dayjs(fmDate).format('YYYY/MM/DD-HH:mm:ss')
        },
        getUserById(id) {
            return HTTP.get(GET_USER_BY_ID(id)).then((res) => res.data)
        },
        async handlerLoadData() {
            for (let i = 0; i < this.dataEquipment.length; i++) {
                const user = await this.getUserById(Number(this.dataEquipment[i].idUser))
                this.dataEquipment[i].name = user.fullName
                this.dataEquipment[i].user = user
                this.dataEquipment[i].userName = user.userCode
                this.dataEquipment[i].infoDevice.userName = user.userCode
            }
        },
        async getAllDevice() {
            await DeviceService.getAllEquipmentDeviceWithPage(
                this.resultPgae.pageNumber,
                this.resultPgae.pageSize,
            ).then((res) => {
                this.totalMapPage = res.data._totalPages
                this.totalItem = res.data._totalItems
                this.itemIndex = res.data._itemIndex

                var data = res.data._Data.map((el) => ({
                    name: el.name,
                    userName: el.userName,
                    updateAt: el.deviceInfo.updateAt,
                    idUser: el.deviceInfo.userId,
                    user: el.user,
                    infoDeviceEmty: el.deviceInfo,
                    infoDevice: {
                        mechineName: el.deviceInfo.deviceName,
                        userName: el.userName,
                        operatingSystem: el.deviceInfo.operatingSystem,
                        systemType: el.deviceInfo.systemType,
                        cpuName: el.deviceInfo.cpuName,
                        deviceTotalRamSize: el.deviceInfo.deviceTotalRamSize,
                        mainName: el.deviceInfo.mainName,
                        diskDriveTotalSize: el.deviceInfo.diskDriveTotalSize,
                    },
                    infoInstallSoftware: el.applications,
                }))
                this.dataEquipment = data
            })
            this.loading = false
        },
        async handlerSearchByName() {
            this.loading = true
            await DeviceService.searchDeviceByNameWithPage(
                this.resultPgae.pageNumber,
                this.resultPgae.pageSize,
                this.name,
            )
                .then((res) => {
                    this.dataEquipment = []
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex

                    var data = res.data._Data.map((el) => ({
                        name: el.name,
                        userName: el.userName,
                        updateAt: el.deviceInfo.updateAt,
                        idUser: el.deviceInfo.userId,
                        user: el.user,
                        infoDeviceEmty: el.deviceInfo,
                        infoDevice: {
                            mechineName: el.deviceInfo.deviceName,
                            userName: el.userName,
                            operatingSystem: el.deviceInfo.operatingSystem,
                            systemType: el.deviceInfo.systemType,
                            cpuName: el.deviceInfo.cpuName,
                            deviceTotalRamSize: el.deviceInfo.deviceTotalRamSize,
                            mainName: el.deviceInfo.mainName,
                            diskDriveTotalSize: el.deviceInfo.diskDriveTotalSize,
                        },
                        infoInstallSoftware: el.applications,
                    }))
                    this.dataEquipment = data
                })
                .catch(() => {
                    this.dataEquipment = []
                    this.getAllDevice()
                    this.loading = false
                })
            this.loading = false
        },
        handlerDetailsDevice(data) {
            this.isOpenDialogDevice = true
            this.deviceDetail = { ...data }
        },
        closeDialogDetailDevice() {
            this.isOpenDialogDevice = false
            this.deviceDetail = []
        },
        handlerAddBlockingWeb() {
            this.isOpenDialogBlockingWeb = true
        },
        closeDialogAddBlockingWeb() {
            this.isOpenDialogBlockingWeb = false
        },
        async handlerReload() {
            this.loading = true
            this.dataEquipment = []
            this.name = ''
            this.operatingSystem = 19000
            await this.getAllDevice()
        },
    },
    components: {
        DialogDetailDevice,
        DialogAddBlockingWeb,
    },
}
</script>
<style lang="scss" scoped>
/* .p-card-header {
    padding: 10px;
}
.p-card-body {
    padding: 10px !important;
}
.p-card .p-card-content {
    padding: 0px 0px 1.25rem 0px;
}
.v3ti {
    height: 100%;
}
.v3ti > .v3ti-content > input {
    width: 100%;
}
.v3ti:focus-visible {
    border: none;
    box-shadow: none;
}
.v3ti:focus {
    border: none;
    box-shadow: none;
} */
</style>
