<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-sm-12 col-md-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">THỐNG KÊ HIỆU SUẤT CỦA NHÂN VIÊN</li>
                        </ol>
                    </nav>
                </div>
            </div>

            <nav class="navbar navbar-expand-lg bg-light navbar-light">
                <div class="container-fluid">
                    <button class="navbar-toggler mb-2 mt-2 custom-input-h" type="button" data-bs-toggle="collapse"
                        data-bs-target="#collapsibleNavbar">
                        <span class="navbar-toggler-icon"></span>
                    </button>
                    <div class="collapse navbar-collapse mt-2" id="collapsibleNavbar">
                        <ul class="navbar-nav me-auto">
                            <li class="nav-item me-2 mb-2">
                                <Export class="custom-button-h" label="Xuất Excel" @click="exportBill()" />
                            </li>
                        </ul>
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2">
                                <Button type="button" style="background-color: antiquewhite" icon="pi pi-filter-slash"
                                    class="custom-reload-h" @click="handlerReload()" v-tooltip.top="'Bỏ lọc'"/>
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <date-picker v-model:value="selectedDate" type="month" format="YYYY-MM" placeholder="Chọn tháng"></date-picker>
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Dropdown 
                                    v-model="selectedUser" 
                                    :options="usersList" 
                                    filter 
                                    optionLabel="fullName"
                                    optionValue="id" 
                                    placeholder="Chọn nhân viên" 
                                    style="width: 100%;"
                                    class="custom-input-h" />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable 
                        showGridlines 
                        removableSort 
                        :sort="1" 
                        :loading="isLoading" 
                        :rowHover="true" :rows="5"
                        resizableColumns 
                        :value="billable"
                    >
                        <template #loading> </template>

                        <template #empty>
                            <div v-if="this.isLoading" style="display: flex; justify-items: flex-end">
                                <ProgressSpinner style="width: 42px" />
                            </div>
                            <div>Không tìm thấy.</div>
                        </template>

                        <Column field="#" header="#" style="width: 2%">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="userCode" header="Mã nhân viên" sortable style="width: 5%">
                            <template #body="{ data }">
                                {{ data.userCode }}
                            </template>
                        </Column>
                        <Column field="name" header="Tên nhân viên" style="width: 10%">
                            <template #body="{ data }">
                                {{ data.name}}
                            </template>
                        </Column>

                        <Column field="totalTimeEst" header="Billable" style="width: 5%;">
                            <template #body="{ data }">
                                {{ data.totalTimeEst }}
                            </template>
                        </Column>

                        <Column field="totalTimeSpent" header="Thời gian làm thực tế" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.totalTimeSpent }}
                            </template>
                        </Column>

                        <Column field="ot" header="Thời gian OT" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.ot }}
                            </template>
                        </Column>

                        <Column field="efficiency" header="Hiệu suất" style="width: 5%">
                            <template #body="{ data }">
                                {{ data.efficiency }}
                                <span>(%)</span>
                            </template>
                        </Column>

                        <Column header="Thực thi" style="width: 7%; text-align: left">
                            <template #body="{data}">
                                <div>
                                    <Export class="custom-button-h" label="Xuất Excel" @click="exportBill(data)"/>
                                </div>
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
                        :totalItems="this.totalItem" 
                        :itemIndex="this.itemIndex" />
                </div>
            </div>
        </div>
        <Dialog
            header="Không có quyền truy cập!"
            :visible="displayBasic"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            :modal="true"
            :closable="false"
        >
            <p>Bạn không có quyền truy cập !</p>
            <medium
                >Bạn sẽ được điều hướng vào trang chủ <strong>{{ num }}</strong> giây!
            </medium>
            <template #footer>
                <Button label="Hoàn tất" icon="pi pi-check" @click="redirectToHome()" autofocus />
            </template>
        </Dialog>
    </LayoutDefaultDynamic>
</template>
<script>
    import {
        HTTP,
        HTTP_API_GITLAB,
        GET_ALL_ISSUE_IN_PROJECT_BY_DATE,
        GET_ALL_NOTES_BY_ISSUE,
        GET_ALL_USER_IN_PROJECT_DATABASE,
        GET_ALL_PROJECT_DATABASE,
        GET_ALL_USERS_DATABASE,
        GET_ALL_PROJECT_BY_STAFF,
        GET_ALL_TIME_STATS_IN_ISSUE,
        STAFF_IS_IN_PROJECT_CHECK,
    } from '@/http-common'
    import * as XLSX from 'xlsx-js-style'
    import { FormatExcel } from '@/helper/formatExcel.helper'
    import { DateHelper } from '@/helper/date.helper'
    import Export from '../../components/buttons/Export.vue'
    import { UserService } from '@/service/user.service'
    import { checkAccessModule } from '@/helper/checkAccessModule'
    import router from '@/router/index'
    export default {
    data() {
        return {
            projectList: [],
            staffList: [],
            reportList: [],
            filterWithAddedFieldList: [],
            filterWithEstimateList: [],
            currentMonth: null,
            currentYear: null,
            billable: [],
            totalWorkingHours: null,
            workingHoursPerDay: 8,
            pageSize: 100,
            page: 1,
            selectedUser: null,
            resultPgae: {
                pageSize: 10,
                pageNumber: 1,
            },
            totalItem: 0,
            totalMapPage: 0,
            itemIndex: 0,
            usersList: [],
            isLoading: false,
            selectedDate: null,
            displayBasic: false,
            num: 5,
            timeout: null,
            showButton: {
                add: false,
                update: false,
                delete: false,
                deleteMulti: false,
                confirm: false,
                confirmMulti: false,
                refuse: false,
                addMember: false,
                export: false,
            },
        }
    },
    watch: {
        selectedUser: {
            handler: async function change(newVal) {
                if (newVal != null) {
                    await this.getAllStaff();
                }
            },
            deep: true,
        },
        selectedDate: {
            handler: async function change(newVal) {
                this.getDefaultMonth();
                this.calculateTotalWorkingHours()
                await this.getAllStaff()
            },
            deep: true,
        },
        resultPgae: {
            handler: async function change() {
                await this.getAllStaff()
            },
            deep: true,
        },
    },
    async created() {
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '')) === true) {
            checkAccessModule.checkPermissionAction(this.$route.path.replace('/', ''), this.showButton)
            await this.handlerGetAllUser()
            this.calculateTotalWorkingHours()
            this.getDefaultMonth()
            await this.getAllStaff()
        } else {
            this.countTime()
            this.displayBasic = true
        }
    },
    methods: {
        countTime() {
            if (this.num == 0) {
                this.redirectToHome()
                return
            }
            this.num = this.num - 1
            this.timeout = setTimeout(() => this.countTime(), 1000)
        },
        redirectToHome() {
            clearTimeout(this.timeout)
            router.push('/')
        },
        handlerReload() {
            //this.selectedDate = null
            this.selectedUser = null
            this.getDefaultMonth()
            this.getAllStaff()
        },
        async getAllUser() {
            try {
                var page = 1;
                var  perPage = 100;
                var users = [];
                while(true){
                    await UserService.getAllUser(page, perPage).then(res => {
                        users = res.data._Data;
                        this.usersList.push(users);
                    })
                    .catch((error) => {
                        console.error(error);
                    });
                    if(users.length < perPage){
                        break;
                    }
                    page ++;
                }
                this.usersList = this.usersList[0]
            } catch (error) {
                console.error(error);
            }
        },
        async getAllUserInProjectByIdUser() {
            try {
                var page = 1;
                var  perPage = 100;
                var users = [];
                while(true){
                    await UserService.getAllUserInProjectByIdUser(checkAccessModule.getUserIdCurrent(), page, perPage).then(res => {
                        users = res.data._Data;
                        this.usersList.push(users);
                    })
                    .catch((error) => {
                        console.error(error);
                    });
                    if(users.length < perPage){
                        break;
                    }
                    page ++;
                }
                this.usersList = this.usersList[0]
            } catch (error) {
                console.error(error);
            }
        },
        async handlerGetAllUser(){
            this.usersList = []
            if(checkAccessModule.isAdmin() || checkAccessModule.isOffice()){
                this.getAllUser();
            }
            else {
                this.getAllUserInProjectByIdUser();
            }
        },
        async getAllStaff() {
            this.isLoading = true
            this.billable = []
            var pageSize = this.pageSize
            var page = this.page
            try {
                if(this.selectedUser!=null){
                    this.totalMapPage = 1
                    this.totalItem = 1
                    this.itemIndex = 1
                    this.staffList = this.usersList.filter((user) => user.id === this.selectedUser)
                }
                else {
                    if(checkAccessModule.isAdmin() || checkAccessModule.isOffice()){
                        await UserService.getAllUser(this.resultPgae.pageNumber, this.resultPgae.pageSize).then(res => {
                            this.totalMapPage = res.data._totalPages
                            this.totalItem = res.data._totalItems
                            this.itemIndex = res.data._itemIndex
                            this.staffList = res.data._Data
                        })
                        .catch((error) => {
                            console.error(error);
                        });
                    }
                    else {
                        await UserService.getAllUserInProjectByIdUser(checkAccessModule.getUserIdCurrent(), this.resultPgae.pageNumber, this.resultPgae.pageSize).then(res => {
                            this.totalMapPage = res.data._totalPages
                            this.totalItem = res.data._totalItems
                            this.itemIndex = res.data._itemIndex
                            this.staffList = res.data._Data
                        })
                        .catch((error) => {
                            console.error(error);
                        });
                    }
                }
                // Lấy giá trị id, fullName và idUserGitLab từ staffList và gán vào mảng billable
                this.billable = this.staffList.map((staff) => ({
                        userCode: staff.userCode,
                        id: staff.id,
                        idUserGitlab: staff.idUserGitLab,
                        name: staff.fullName,
                        project: [],
                        noteid: [],
                        timeSpent: 0,
                        timeEst: 0,
                        totalTimeSpent: 0,
                        totalTimeEst: 0,
                        ot: 0,
                        efficiency: 0,
                    }))
                    this.staffList.forEach(async (element) => {
                        await this.getAllProjectByStaff(element.id, page, pageSize)
                    })
                this.isLoading = false
            } catch (err) {
                console.error(err)
            }
        },
        async getAllProjectByStaff(idStaff, page, pageSize) {
            try {
                this.projectList = []
                await HTTP.get(GET_ALL_PROJECT_BY_STAFF(idStaff, page, pageSize))
                .then(async res => {
                    this.projectList = res.data._Data.filter((project) => project.isOnGitlab === true)
                    if(this.projectList.length <= 0) {
                        this.loading = false
                        return
                    }
                    else {
                        this.billable.forEach((billableItem) => {
                            if (billableItem.id === idStaff) {
                                billableItem.project = this.projectList
                            }
                        })
                        await this.getAllIssueInProject()
                    }
                })
                .catch ((err) => {
                    console.error(err)
                })
            } catch (err) {
                console.error(err)
            }
        },
        async getAllIssueInProject() {
            var pageSize = this.pageSize
            var page = this.page
            // lấy project code của project
            this.projectList.forEach(async (element) => {
                await this.getIssuesByProjectByDate(element.projectCode, pageSize, page)
            })
        },
        async getIssuesByProjectByDate(idProject, pageSize, page) {
            var firstDayOfMonth = new Date(this.currentYear, this.currentMonth - 1, 1) // Ngày đầu tiên của tháng
            firstDayOfMonth.setHours(0, 0, 0) // Đặt thời gian bắt đầu ngày về 00:00:00
            var lastDayOfMonth = new Date(this.currentYear, this.currentMonth, 0) // Ngày cuối cùng của tháng
            lastDayOfMonth.setHours(23, 59, 59) // Đặt thời gian kết thúc ngày về 23:59:59
            var filteredData = []
            await HTTP_API_GITLAB.get(GET_ALL_ISSUE_IN_PROJECT_BY_DATE(idProject, firstDayOfMonth, lastDayOfMonth, pageSize, page),)
                .then(async (response) => {
                filteredData = response.data.filter((item) => item.assignee !== null)
                filteredData.forEach((item) => {
                    var assigneeId = item.assignee.id // Lấy id của mảng con assignees
                    var noteid = item.iid // Lấy iid
                    var billableItem = this.billable.find((item) => item.idUserGitlab === assigneeId) // Tìm phần tử tương ứng trong billable
                    if (billableItem) {
                        if (!billableItem.noteid.includes(noteid)) {
                            // Kiểm tra xem iid đã tồn tại trong mảng noteid chưa
                            billableItem.noteid.push({
                                noteid: noteid,
                                idProject: idProject
                            })
                        }
                    }
                })
                this.billable.forEach((item) => {
                    item.noteid.forEach(ele => {
                        if(ele != undefined){
                            this.fetchNotesForIssue(ele.idProject, ele.noteid)
                        }
                    })
                })
                if (filteredData.length === pageSize) {
                    var nextPage = page + 1
                    var nextData = await this.getIssuesByProjectByDate(idProject, pageSize, nextPage)
                    return filteredData.concat(nextData)
                } else {
                    return filteredData
                }
            })
            .catch((error) => {
                console.error(error)
            })
        },
        async fetchNotesForIssue(projectId, issueId) {
            this.reportList = []
            var pageSize = this.pageSize
            var page = this.page
            try {
                while (true) {
                    var newData = []
                    await HTTP_API_GITLAB.get(GET_ALL_NOTES_BY_ISSUE(projectId, issueId, pageSize, page),)
                    .then(async response => {
                        response.data.forEach(ele => {if(ele.system) newData.push(ele)})
                    })
                    .catch((error) => {
                        console.error(error)
                    })
                    if (newData.length > 0) {
                        this.reportList = this.reportList.concat(newData)
                        this.reportList = JSON.parse(JSON.stringify(this.reportList))
                        page++
                    } else {
                        break // No more notes available, exit the loop
                    }
                }
                await this.getTheAddedItem()
            } catch (error) {
                console.error(error)
            }
        },
        async getTheAddedItem() {
            this.filterWithAddedFieldList = []
            this.filterWithEstimateList = []
            this.merge_spent_estimate = []

            this.filterWithAddedFieldList = this.reportList
                .filter((x) => x.body.startsWith('added'))
                .filter((addedItem) => {
                    const matchingRemovedItem = this.reportList.find(
                        (x) =>
                            x.body.startsWith('removed') &&
                            x.noteable_iid === addedItem.noteable_iid &&
                            x.created_at > addedItem.created_at,
                    )
                    return !matchingRemovedItem
                })
                .map((item) => JSON.parse(JSON.stringify(item)))

            this.filterWithEstimateList = this.reportList
                .filter((x) => x.body.startsWith('changed time estimate to'))
                .map((item) => JSON.parse(JSON.stringify(item)))

            // Khởi tạo một Map để lưu trữ kết quả đã gộp chung
            // var mergedResults = new Map()
            // // Duyệt qua mảng filterWithEstimateList lấy ngày tạo gần nhất
            // for (var item of this.filterWithEstimateList) {
            //     var noteableId = item.noteable_id
            //     var createdAt = new Date(item.created_at)

            //     // Kiểm tra xem noteableId đã tồn tại trong Map chưa
            //     if (!mergedResults.has(noteableId)) {
            //         // Nếu chưa tồn tại, thêm phần tử vào Map
            //         mergedResults.set(noteableId, item)
            //     } else {
            //         // Nếu đã tồn tại, so sánh createdAt với phần tử đã lưu trong Map
            //         var existingItem = mergedResults.get(noteableId)
            //         var existingCreatedAt = new Date(existingItem.created_at)

            //         // So sánh createdAt của phần tử hiện tại với createdAt đã lưu trong Map
            //         if (createdAt > existingCreatedAt) {
            //             // Nếu createdAt hiện tại lớn hơn, thay thế giá trị trong Map
            //             mergedResults.set(noteableId, item)                       
            //         }
            //     }
            // }
            // var mergedResultsArray = Array.from(mergedResults.values())
            // this.filterWithEstimateList = JSON.parse(JSON.stringify(mergedResultsArray))
            await this.fillBill()
        },
        async fillBill() {
            var filteredByWeek_Spent = []
            var filteredByWeek_Est = []
            // Lấy danh sách time spent và est dựa theo tháng
            var firstDayOfMonth = new Date(this.currentYear, this.currentMonth - 1, 1) // Ngày đầu tiên của tháng
            firstDayOfMonth.setHours(0, 0, 0)
            var lastDayOfMonth = new Date(this.currentYear, this.currentMonth, 0) // Ngày cuối cùng của tháng
            lastDayOfMonth.setHours(23, 59, 59)
            // var startWeekDate = new Date(this.currentYear, this.currentMonth - 1, weekStart)
            // startWeekDate.setHours(0, 0, 0)
            // var endWeekDate = new Date(this.currentYear, this.currentMonth - 1, weekEnd)
            // endWeekDate.setHours(23, 59, 59, 999)

            var getTimeSpent = this.filterWithAddedFieldList
                .flatMap((subArray) => subArray)
                .filter((report) => {
                    var date = new Date(report.created_at)
                    return date >= firstDayOfMonth && date <= lastDayOfMonth
                })

            var getTimeEst = this.filterWithEstimateList
                .flatMap((subArray) => subArray)
                .filter((report) => {
                    var date = new Date(report.created_at)
                    return date >= firstDayOfMonth && date <= lastDayOfMonth
                })

            // chuyển đổi sang JSON
            var convertedReports = JSON.parse(JSON.stringify(getTimeSpent))
            filteredByWeek_Spent.push(convertedReports)

            var convertedReports_Est = JSON.parse(JSON.stringify(getTimeEst))
            filteredByWeek_Est.push(convertedReports_Est)

            // Lấy dữ liệu để tính time spent
            for (let item = 0; item < filteredByWeek_Spent[0].length; item++) {
                var filteredWeekSpent = filteredByWeek_Spent[item]
                if(filteredWeekSpent!=undefined) {
                    for (var assignee of this.billable) {
                        var weekReportsByAuthorSpent = filteredWeekSpent.filter((weekReport, index, self) => {
                            return (
                                weekReport.author.id === assignee.idUserGitlab &&
                                self.findIndex((r) => r.id === weekReport.id) === index
                            )
                        })
                        if (weekReportsByAuthorSpent.length > 0) {
                            var sumOfTimeSpent = this.calculateTimeSum(weekReportsByAuthorSpent)
                            // assignee.weeks[item] = {
                            //     reports: weekReportsByAuthorSpent,
                            //     totalTimes: sumOfTimeSpent.totalTimes,
                            // }
                            assignee.timeSpent = sumOfTimeSpent.totalTimes
                        }
                    }
                }
            }
            for (let i = 0; i < filteredByWeek_Est[0].length; i++) {
                var filteredWeekEst = filteredByWeek_Est[i]
                if(filteredWeekEst!=undefined) {
                    for (var assignee of this.billable) {
                        var weekReportsByAuthorEst = filteredWeekEst.filter((weekReport, index, self) => {
                            return (
                                assignee.noteid.find(ele => ele.noteid == weekReport.noteable_iid) &&
                                self.findIndex((r) => r.id === weekReport.id) === index
                            )
                        })
                        if (weekReportsByAuthorEst.length > 0) {
                            var sumOfTimeEst = this.calculateTimeSum(weekReportsByAuthorEst)
                            // if (!assignee.weeks[i]) {
                            //     assignee.weeks[i] = {}
                            // }
                            // assignee.weeks[i].reports = assignee.weeks[i].reports
                            //     ? [...assignee.weeks[i].reports, ...weekReportsByAuthorEst]
                            //     : weekReportsByAuthorEst
                            // assignee.weeks[i].totalEst = this.formatNumber(sumOfTimeEst.totalEst)
                            assignee.timeEst = parseFloat(this.formatNumber(sumOfTimeEst.totalEst))
                        }
                    }
                }
            }
            // Lấy dữ liệu để tính tổng spent và est của tháng
            for (const assignee of this.billable) {
                assignee.totalTimeSpent = this.formatNumber(assignee.timeSpent / this.totalWorkingHours)
                assignee.totalTimeEst = this.formatNumber(assignee.timeEst / this.totalWorkingHours)
                assignee.efficiency = assignee.timeEst > 0 ? ((assignee.timeEst / assignee.timeSpent) * 100).toFixed(2) : '0'
            }
        },
        formatNumber(num) {
            if (Number.isInteger(num)) {
                return num.toString() // Trả về số tự nhiên dưới dạng chuỗi
            } else {
                return num.toFixed(2) // Làm tròn số thập phân đến 2 chữ số và trả về chuỗi
            }
        },
        getDefaultMonth() {
            if(this.selectedDate==null){
                this.selectedDate = new Date()
            }
            this.currentMonth = this.selectedDate.getMonth() + 1
            this.currentYear = this.selectedDate.getFullYear()
        },
        calculateTimeSum(arr) {
            let totalTimes = 0
            let totalEst = 0
            for (let i = 0; i < arr.length; i++) {
                const report = arr[i]
                const timeString = report.body

                if (timeString.includes('added')) {
                    const components = timeString.split(' ')
                    for (let j = 0; j < components.length; j++) {
                        const component = components[j]
                        const numericValue = parseFloat(component)

                        if (!isNaN(numericValue)) {
                            if (component.includes('d')) {
                                totalTimes += numericValue * 8 // 1d = 8h
                            } else if (component.includes('w')) {
                                totalTimes += numericValue * 40 // 1w = 40h
                            } else if (component.includes('m')) {
                                totalTimes += numericValue / 60 // 1m = 1/60h
                            } else if (component.includes('h')) {
                                totalTimes += numericValue // Giờ đã được chỉ định
                            }
                        }
                    }
                } else if (timeString.includes('changed')) {
                    // Handle "changed" case
                    // Tính tổng thời gian cho "changed" và lưu vào totalEst
                    const components = timeString.split(' ')
                    for (let j = 0; j < components.length; j++) {
                        const component = components[j]
                        const numericValue = parseFloat(component)

                        if (!isNaN(numericValue)) {
                            if (component.includes('d')) {
                                totalEst += numericValue * 8 // 1d = 8h
                            } else if (component.includes('w')) {
                                totalEst += numericValue * 40 // 1w = 40h
                            } else if (component.includes('m')) {
                                totalEst += numericValue / 60 // 1m = 1/60h
                            } else if (component.includes('h')) {
                                totalEst += numericValue // Giờ đã được chỉ định
                            }
                        }
                    }
                }
            }
            return {totalTimes, totalEst}
        },
        // xử lý 1 ngày 8 giờ làm việc
        calculateTotalWorkingHours() {
            this.weekends = [0, 6] // Thay đổi giá trị này theo yêu cầu của bạn
            const firstDayOfMonth = new Date(this.currentYear, this.currentMonth - 1, 1) // Ngày đầu tiên của tháng
            const lastDayOfMonth = new Date(this.currentYear, this.currentMonth, 0) // Ngày cuối cùng của tháng
            let totalHours = 0

            for (let day = firstDayOfMonth; day <= lastDayOfMonth; day.setDate(day.getDate() + 1)) {
                const dayOfWeek = day.getDay()
                if (!this.weekends.includes(dayOfWeek)) {
                    totalHours += this.workingHoursPerDay
                }
            }
            this.totalWorkingHours = totalHours
        },
        async exportBill(data=null) {
            var dataExport = []
            var nameFile = `Danh sách hiệu suất`
            if(data != null){
                const bill = {
                    userCode: data.userCode,
                    name: data.name,
                    totalTimeEst: data.totalTimeEst,
                    totalTimeSpent: data.totalTimeSpent,
                    ot: data.ot,
                    efficiency: data.efficiency
                }
                dataExport.push(bill)
                nameFile = `Hiệu suất nhân viên ${data.name}`
            }
            else {
                this.billable.map((ele) => {
                    const bill = {
                        userCode: ele.userCode,
                        name: ele.name,
                        totalTimeEst: ele.totalTimeEst,
                        totalTimeSpent: ele.totalTimeSpent,
                        ot: ele.ot,
                        efficiency: ele.efficiency
                    }
                    dataExport.push(bill)
                })
            }
            import('../../plugins/Export2Excel.js').then((excel) => {
                const Header = [
                    'Mã nhân viên',
                    'Tên nhân viên',
                    'Billable',
                    'Thời gian làm thực tế',
                    'Thời gian OT',
                    'Hiệu suất',
                ]
                const Field = [
                    'userCode',
                    'name',
                    'totalTimeEst',
                    'totalTimeSpent',
                    'ot',
                    'efficiency',
                ]
                excel.export_json_to_excel({
                    header: Header,
                    data: this.FormatJSon(Field, dataExport),
                    sheetName: `Sheet1`,
                    filename: nameFile,
                    autoWidth: true,
                    bookType: 'xlsx',
                })
            })
        },
        FormatJSon(FilterData, JsonData) {
            return JsonData.map((v) =>
                FilterData.map((j) => {
                    return v[j]
                }),
            )
        },
    },
    computed: {
        currentMonthText() {
            return 'Tháng ' + this.currentMonth
        },
    },
    components: { Export },
}
</script>
<style lang="scss" scoped>

</style>
