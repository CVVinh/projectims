<template>
    <Dialog
        header="Billable"
        :visible="isOpen"
        :closable="false"
        :maximizable="true"
        modal
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        style="width: 100%; height: 100%"
    >
        <div class="scroll-container">
            <div class="header-table">
                <div class="row">
                    <div class="col-1">
                        <p class="header-title" style="padding-left: 35px">#</p>
                    </div>
                    <div class="col-2">
                        <p class="header-title" style="padding-left: 115px">Tên</p>
                    </div>
                    <div class="col-3">
                        <p class="header-title" style="padding-left: 250px">Tên dự án</p>
                    </div>
                    <div class="col-2">
                        <p class="header-title" style="padding-left: 100px">Leader</p>
                    </div>

                    <div class="col-2">
                        <p class="subheader-title">Plan công số(MM)</p>
                        <hr />
                        <p class="subheader-title" id="current-month">{{ currentMonthText }}</p>
                        <hr />
                        <div class="row-subheader">
                            <div class="col-sm-6">
                                <p class="subheader-title">BILLABLE</p>
                            </div>
                            <div class="col-sm-6" style="border-right: none">
                                <p class="subheader-title">OT</p>
                            </div>
                        </div>
                    </div>

                    <div class="col-4">
                        <p
                            class="subheader-title"
                            style="border-right: 1px solid; border-color: var(--bs-table-border-color)"
                        >
                            Công số(MH)
                        </p>
                        <div class="container">
                            <div class="row">
                                <div v-for="(item, index) in validWeeks" :key="index" class="col">
                                    <p class="subheader-title">Tuần {{ index + 1 }} ({{ item }})</p>
                                    <div class="row">
                                        <div class="col">
                                            <p class="subheader-title">BILLABLE</p>
                                        </div>
                                        <div class="col-3" style="width: 36%">
                                            <p class="subheader-title">Thực tế</p>
                                        </div>
                                        <div class="col">
                                            <p class="subheader-title">OT</p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-3">
                        <p class="subheader-title-fill">Tổng hợp theo dự án, theo tháng</p>
                        <div class="row">
                            <div class="col">
                                <p class="subheader-title">BILLABLE</p>
                            </div>
                            <div class="col">
                                <p class="subheader-title">Thực tế</p>
                            </div>
                            <div class="col">
                                <p class="subheader-title">OT</p>
                            </div>
                        </div>
                    </div>

                    <div class="col">
                        <p class="subheader-title-fill">Tổng hợp theo tháng (MM)</p>
                        <div class="row">
                            <div class="col">
                                <p class="subheader-title">BILLABLE</p>
                            </div>
                            <div class="col" style="width: 36%">
                                <p class="subheader-title">Thực tế</p>
                            </div>
                            <div class="col">
                                <p class="subheader-title">OT</p>
                            </div>
                            <div class="col">
                                <p class="subheader-title">Hiệu suất</p>
                            </div>
                            <div class="col">
                                <p class="subheader-title">Rank</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <table class="table table-bordered">
                <tbody>
                    <tr v-for="(value, index) in billable" :key="value.id">
                        <th scope="row">{{ index + 1 }}</th>
                        <!-- Name -->
                        <td class="td-col-2">{{ value.name }}</td>
                        <!-- Project Name -->
                        <td class="td-col-3">{{ projectData.name }}</td>
                        <!-- <td class="td-col-3">IMS</td> -->

                        <!-- Leader Name -->
                        <td class="td-col-2">
                            {{ memberProject.fullName }}
                        </td>
                        <!-- <td class="td-col-2">NVM</td> -->
                        <!-- </div> -->

                        <td class="td-col-sm-6">0</td>
                        <td class="td-col-sm">0</td>
                        <!-- column tuần -->

                        <td class="td-col-week-1">{{ value.weeks[0].total_time_estimate || 0 }}</td>
                        <td class="td-colsubheader-week-1">{{ value.weeks[0].total_time_spent || 0 }}</td>
                        <td class="td-colsubtitle-week-1">0</td>

                        <td class="td-col-week-2">{{ value.weeks[1].total_time_estimate || 0 }}</td>
                        <td class="td-colsubheader-week-2">{{ value.weeks[1].total_time_spent || 0 }}</td>
                        <td class="td-colsubtitle-week-2">0</td>

                        <td class="td-col-week-3">{{ value.weeks[2].total_time_estimate || 0 }}</td>
                        <td class="td-colsubheader-week-3">{{ value.weeks[2].total_time_spent || 0 }}</td>
                        <td class="td-colsubtitle-week-3">0</td>

                        <td class="td-col-week-4">{{ value.weeks[3].total_time_estimate || 0 }}</td>
                        <td class="td-colsubheader-week-4">{{ value.weeks[3].total_time_spent || 0 }}</td>
                        <td class="td-colsubtitle-week-4">0</td>

                        <td class="td-col-week-5">{{ value.weeks[4].total_time_estimate || 0 }}</td>
                        <td class="td-colsubheader-week-5">{{ value.weeks[4].total_time_spent || 0 }}</td>
                        <td class="td-colsubtitle-week-5">0</td>
                        <!-- column tuần -->

                        <td class="td-colsubheader-title">{{ value.total_time_estimate }}</td>
                        <td class="td-colsubheader-title">{{ value.total_time_spent }}</td>
                        <td class="td-colsubheader-title">0</td>

                        <td class="td-colsubheader-sumary">{{ value.total_time_estimate }}</td>
                        <td class="td-colsubheader-sumary">{{ value.total_time_spent }}</td>
                        <td class="td-colsubheader-sumary">0</td>
                        <td class="td-colsubheader-sumary">{{ value.efficiency }}</td>
                        <td></td>
                    </tr>
                </tbody>
            </table>
        </div>
        <template #footer>
            <Button
                style="height: 44px"
                label="Hủy"
                icon="pi pi-check"
                class="p-button-secondary"
                @click="closeModal()"
            ></Button>
            <Export label="Xuất Excel" @click="exportBill($event)" />
        </template>
    </Dialog>
</template>
<script>
    import { HTTP, HTTP_API_GITLAB, GET_ALL_ISSUE_IN_PROJECT_BY_DATE, KEY_MAP } from '@/http-common'
    import Export from '../../components/buttons/Export.vue'
    import * as XLSX from 'xlsx-js-style'
    import { FormatExcel } from '@/helper/formatExcel.helper'
    import { DateHelper } from '@/helper/date.helper'
    export default {
        props: ['isOpen', 'projectData'],
        data() {
            return {
                reportList: [],
                filterByPointOfDateList: [],
                filterWithAddedFieldList: [],
                date: null,
                currentMonth: null,
                currentYear: null,
                validWeeks: [],
                taskList: [],
                weekStart: null,
                weekEnd: null,
                billByWeeks: {},
                staffInTask: [],
                taskInWeek1: [],
                taskInWeek2: [],
                taskInWeek3: [],
                taskInWeek4: [],
                taskInWeek5: [],
                stafflist: [],
                billable: [],
                memberProject: [],
                projectCode: [],
                totalWorkingHours: null,
                workingHoursPerDay: 8,
            }
        },
        async beforeUpdate() {
            this.projectCode = this.projectData
            await this.getProjectLeader()
        },
        created() {
            this.getDate()
            this.calculateTotalWorkingHours()
        },
        methods: {
            closeModal() {
                this.$emit('closeExportBill')
            },
            async exportBill() {
                var strMonth = []
                for (var i = 0; i < this.validWeeks.length; i++) {
                    strMonth.push('Tuần ' + (i + 1) + ' (' + this.validWeeks[i] + ')')
                }
                var dataExport = [
                    [
                        'Tên',
                        'Tên dự án',
                        'Leader',
                        'Plan công số (MM)',
                        '',
                        'Công số (MH)',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        'Tổng hợp theo dự án, theo tháng',
                        '',
                        '',
                        'Tổng hợp theo tháng (MM)',
                        '',
                        '',
                        '',
                        '',
                        '',
                    ],
                    [
                        '',
                        '',
                        '',
                        'Tháng ' + this.currentMonth,
                        '',
                        strMonth[0],
                        '',
                        '',
                        strMonth[1],
                        '',
                        '',
                        strMonth[2],
                        '',
                        '',
                        strMonth[3],
                        '',
                        '',
                        strMonth[4],
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                        '',
                    ],
                    [
                        '',
                        '',
                        '',
                        'BILLABLE',
                        'OT',
                        'BILLABLE',
                        'Thực tế',
                        'OT',
                        'BILLABLE',
                        'Thực tế',
                        'OT',
                        'BILLABLE',
                        'Thực tế',
                        'OT',
                        'BILLABLE',
                        'Thực tế',
                        'OT',
                        'BILLABLE',
                        'Thực tế',
                        'OT',
                        'BILLABLE',
                        'Thực tế',
                        'OT',
                        'BILLABLE',
                        'Thực tế',
                        'OT',
                        'Hiệu suất',
                        'Rank',
                    ],
                ]
                this.billable.map((ele) => {
                    var objectToAdd = [
                        ele.name,
                        this.projectData.name,
                        this.memberProject[0].author.name,
                        0,
                        0,
                        ele.weeks[0].total_time_estimate,
                        ele.weeks[0].total_time_spent,
                        '0',
                        ele.weeks[1].total_time_estimate,
                        ele.weeks[1].total_time_spent,
                        '0',
                        ele.weeks[2].total_time_estimate,
                        ele.weeks[2].total_time_spent,
                        '0',
                        ele.weeks[3].total_time_estimate,
                        ele.weeks[3].total_time_spent,
                        '0',
                        ele.weeks[4].total_time_estimate,
                        ele.weeks[4].total_time_spent,
                        '0',
                        ele.total_time_estimate,
                        ele.total_time_spent,
                        '0',
                        ele.total_time_estimate,
                        ele.total_time_spent,
                        '0',
                        ele.efficiency,
                        '-',
                    ]
                    dataExport.push(objectToAdd)
                })
                const ws = XLSX.utils.aoa_to_sheet(dataExport)

                ws['!merges'] = [
                    { s: { r: 0, c: 0 }, e: { r: 2, c: 0 } },
                    { s: { r: 0, c: 1 }, e: { r: 2, c: 1 } },
                    { s: { r: 0, c: 2 }, e: { r: 2, c: 2 } },
                    { s: { r: 0, c: 3 }, e: { r: 0, c: 4 } },
                    { s: { r: 1, c: 3 }, e: { r: 1, c: 4 } },
                    { s: { r: 0, c: 5 }, e: { r: 0, c: 19 } },
                    { s: { r: 1, c: 5 }, e: { r: 1, c: 7 } },
                    { s: { r: 1, c: 8 }, e: { r: 1, c: 10 } },
                    { s: { r: 1, c: 11 }, e: { r: 1, c: 13 } },
                    { s: { r: 1, c: 14 }, e: { r: 1, c: 16 } },
                    { s: { r: 1, c: 17 }, e: { r: 1, c: 19 } },
                    { s: { r: 0, c: 20 }, e: { r: 1, c: 22 } },
                    { s: { r: 0, c: 23 }, e: { r: 1, c: 27 } },
                ]
                var colWidths = []
                for (let i = 0; i < 3; i++) {
                    if (i == 0) colWidths.push({ wch: 18 })
                    if (i == 1) colWidths.push({ wch: 40 })
                    if (i == 2) colWidths.push({ wch: 18 })
                }
                ws['!cols'] = colWidths

                const wb = XLSX.utils.book_new()
                XLSX.utils.book_append_sheet(wb, ws, 'Sheet1')

                var range = XLSX.utils.decode_range(ws['!ref'])
                var sheet = wb.Sheets[wb.SheetNames[0]]
                range.s.r = 0
                range.e.r = 2
                range.s.c = 0
                range.e.c = 27
                for (var C = range.s.c; C <= range.e.c; ++C) {
                    for (var R = range.s.r; R <= range.e.r; ++R) {
                        const headerCell = XLSX.utils.encode_cell({ r: R, c: C })
                        sheet[headerCell].s = FormatExcel.Header
                    }
                }
                range.s.r = 3
                range.e.r = dataExport.length - 1
                range.s.c = 0
                range.e.c = 27
                for (var C = range.s.c; C <= range.e.c; ++C) {
                    for (var R = range.s.r; R <= range.e.r; ++R) {
                        const headerCell = XLSX.utils.encode_cell({ r: R, c: C })
                        sheet[headerCell].s = FormatExcel.Body
                    }
                }
                var nameFile = this.projectData.name + DateHelper.formatDateDayjs(new Date()) + '.xlsx'
                XLSX.writeFile(wb, nameFile)
            },
            async getProjectLeader() {
                if (this.projectData) {
                    const id = this.projectData.projectCode
                    await HTTP.get(`/Project/GetLeadByProject/${id}`)
                        .then(async (res) => {
                            this.memberProject = res.data
                            console.log('1', this.memberProject)
                            await this.getAllIssueInProject()
                        })
                        .catch((er) => {
                            console.log(er)
                        })
                }
            },

            getDate() {
                const currentDate = new Date()
                this.currentMonth = currentDate.getMonth() + 1
                // this.currentMonth = 3 // Tháng hiện tại
                this.currentYear = currentDate.getFullYear() // Năm hiện tại
                const firstDayOfMonth = new Date(this.currentYear, this.currentMonth - 1, 1) // Ngày đầu tiên của tháng
                const lastDayOfMonth = new Date(this.currentYear, this.currentMonth, 0) // Ngày cuối cùng của tháng

                let currentWeek = ''
                let startDate = new Date(firstDayOfMonth) // Ngày bắt đầu của tuần hiện tại, khởi tạo bằng ngày đầu tiên của tháng

                while (startDate <= lastDayOfMonth) {
                    const day = startDate.getDay() // Lấy ngày trong tuần
                    const dayFormatted = startDate.getDate() // Lấy ngày trong tháng (từ 1 đến 31)

                    // Loại bỏ ngày thứ Bảy (6) và Chủ Nhật (0)
                    if (day !== 6 && day !== 0) {
                        const formattedDate = ('0' + dayFormatted).slice(-2) // Định dạng ngày thành chuỗi có 2 chữ số (ví dụ: 01, 02, ..., 31)

                        if (currentWeek === '') {
                            currentWeek += formattedDate // Thêm ngày đầu tiên của tuần vào chuỗi
                        } else {
                            currentWeek += `-${formattedDate}` // Thêm các ngày tiếp theo của tuần vào chuỗi, phân tách bằng dấu "-"
                        }
                    }
                    startDate.setDate(startDate.getDate() + 1) // Di chuyển đến ngày tiếp theo trong tháng
                    // Kết thúc một tuần và lưu vào mảng validWeeks
                    if (day === 5 || startDate > lastDayOfMonth) {
                        const weekRange = currentWeek.split('-') // Tách chuỗi các ngày thành một mảng các ngày trong tuần
                        this.weekStart = weekRange[0] // Ngày bắt đầu của tuần
                        this.weekEnd = weekRange[weekRange.length - 1] // Ngày kết thúc của tuần
                        this.validWeeks.push(this.weekStart + '-' + this.weekEnd) // Thêm cặp ngày bắt đầu và kết thúc vào mảng validWeeks
                        currentWeek = '' // Đặt lại chuỗi ngày trong tuần để chuẩn bị cho tuần tiếp theo
                    }
                }
            },

            async getAllIssueInProject() {
                // const idProject = '112'
                const idProject = this.projectData.projectCode
                const pageSize = '100'
                const page = '1'
                this.billable = []

                const startDay = '01'
                const endDay = new Date(this.currentYear, this.currentMonth, 0).getDate().toString().padStart(2, '0')

                const created_after = new Date(
                    `${this.currentYear}-${this.currentMonth.toString().padStart(2, '0')}-${startDay}`,
                )
                created_after.setHours(0, 0, 0) // Đặt thời gian bắt đầu ngày về 00:00:00

                const created_before = new Date(
                    `${this.currentYear}-${this.currentMonth.toString().padStart(2, '0')}-${endDay}`,
                )
                created_before.setHours(23, 59, 59) // Đặt thời gian kết thúc ngày về 23:59:59

                const issues = await this.getIssuesByDate(idProject, created_after, created_before, pageSize, page)

                this.billByWeek = await this.getAssigneeList(issues)

                for (const assignee of this.billByWeek) {
                    const billableItem = {
                        id: assignee.id,
                        name: assignee.name,
                        weeks: [[], [], [], [], []],
                        total_time_spent: 0,
                        total_time_estimate: 0,
                        efficiency: 0,
                    }
                    this.billable.push(billableItem)
                }

                for (const issue of issues) {
                    const createdDate = new Date(issue.created_at)
                    const weekNumber = this.getWeekOfMonth(createdDate)

                    const billableItem = this.billable.find((item) => item.id === issue.assignee.id)
                    if (billableItem) {
                        const weekIndex = weekNumber - 1
                        billableItem.weeks[weekIndex].push(issue.time_stats)

                        const total_time_spent =
                            billableItem.total_time_spent + (issue.time_stats.total_time_spent || 0)
                        const total_time_estimate =
                            billableItem.total_time_estimate + (issue.time_stats.time_estimate || 0)
                        billableItem.total_time_spent = total_time_spent
                        billableItem.total_time_estimate = total_time_estimate
                    }
                }

                const secondsInHour = 3600 // Number of seconds in an hour

                for (const billableItem of this.billable) {
                    let total_time_spent_weeks = 0
                    let total_time_estimate_weeks = 0
                    for (let i = 0; i < billableItem.weeks.length; i++) {
                        const week = billableItem.weeks[i]
                        const total_time_spent = week.reduce((sum, issue) => sum + (issue.total_time_spent || 0), 0)
                        const total_time_estimate = week.reduce((sum, issue) => sum + (issue.time_estimate || 0), 0)
                        billableItem.weeks[i] = {
                            total_time_spent: total_time_spent / secondsInHour, // Convert to hours
                            total_time_estimate: total_time_estimate / secondsInHour, // Convert to hours
                        }
                    }
                    // Tính tổng total_time_spent của các tuần
                    for (const week of billableItem.weeks) {
                        total_time_spent_weeks += week.total_time_spent
                        total_time_estimate_weeks += week.total_time_estimate
                    }

                    billableItem.total_time_spent = (total_time_spent_weeks / this.totalWorkingHours).toFixed(2) //  tròn 2 chữ số
                    billableItem.total_time_estimate = (total_time_estimate_weeks / this.totalWorkingHours).toFixed(2) // tròn 2 chữ số

                    // Tính tỷ lệ hiệu suất (efficiency)
                    billableItem.efficiency =
                        billableItem.total_time_estimate !== 0
                            ? (billableItem.total_time_estimate / billableItem.total_time_spent).toFixed(2)
                            : 0

                    if (isNaN(billableItem.efficiency) || !isFinite(billableItem.efficiency)) {
                        // Xử lý trường hợp tỷ lệ hiệu suất là Infinity
                        billableItem.efficiency = 0 // Hoặc giá trị mặc định khác tùy ý
                    }
                }
            },

            async getIssuesByDate(idProject, createdAfter, createdBefore, pageSize, page) {
                const response = await HTTP_API_GITLAB.get(
                    GET_ALL_ISSUE_IN_PROJECT_BY_DATE(idProject, createdAfter, createdBefore, pageSize, page),
                )
                return response.data.filter((item) => item.assignee !== null)
            },
            async getAssigneeList(data) {
                const assigneeList = []

                for (const bill of data) {
                    const assigneeId = bill.assignee.id
                    const assigneeName = bill.assignee.name
                    const timeSpent = bill.time_stats.total_time_spent || 0
                    const timeEstimate = bill.time_stats.time_estimate || 0

                    // Kiểm tra xem assigneeId đã tồn tại trong assigneeList chưa
                    const existingAssignee = assigneeList.find((assignee) => assignee.id === assigneeId)

                    if (existingAssignee) {
                        existingAssignee.total_time_spent += timeSpent
                        existingAssignee.total_time_estimate += timeEstimate
                    } else {
                        assigneeList.push({
                            id: assigneeId,
                            name: assigneeName,
                            total_time_spent: timeSpent,
                            total_time_estimate: timeEstimate,
                        })
                    }
                }

                return assigneeList
            },
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
            getWeekOfMonth(dateString) {
                const date = new Date(dateString)
                const month = date.getMonth()
                const year = date.getFullYear()
                const firstDayOfMonth = new Date(year, month, 1)
                const totalDays = new Date(year, month + 1, 0).getDate()
                let weekNumber = 0
                let dayCounter = 0

                for (let i = 1; i <= totalDays; i++) {
                    const currentDate = new Date(year, month, i)
                    const currentDay = currentDate.getDay()

                    if (currentDay !== 0 && currentDay !== 6) {
                        dayCounter++

                        if (currentDate.getDate() === date.getDate()) {
                            return Math.ceil(dayCounter / 5) // Trả về tuần tương ứng (chia cho 5 và làm tròn lên)
                        }

                        if (dayCounter % 5 === 1) {
                            weekNumber++
                        }
                    }
                }
                return -1
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
    .container {
        padding: 0px;
    }

    th {
        width: 2%;
        text-align: center;
    }

    .header-title {
        padding-top: 50px;
    }

    p.subheader-title {
        text-align: center;
        margin-bottom: 0;
        // border-right: 1px solid;
        padding-top: 13px;
        border-color: var(--bs-table-border-color);
    }

    p.subheader-title-fill {
        text-align: center;
        margin-bottom: 0;
        padding-top: 25px;
        padding-bottom: 25px;
        border-color: var(--bs-table-border-color);
    }

    .col,
    .col-1,
    .col-2,
    .col-3,
    .col-4,
    .col-5,
    .col-6,
    .col-7,
    .col-8,
    .col-sm-3,
    .col-sm-4,
    .col-sm-6,
    .col-sm-7,
    .col-sm-8 {
        border-right: 1px solid;
        border-color: var(--bs-table-border-color);
        padding: 0px;
    }

    .col-1 {
        flex: 0 0 auto;
        width: 2%;
    }

    .col-2 {
        flex: 0 0 auto;
        width: 6.5%;
    }

    .col-3 {
        flex: 0 0 auto;
        width: 15%;
    }

    .col {
        flex: 1 0 0%;
    }

    .col-6 {
        padding: 0px;
    }

    .col-subheader {
        border-right: 1px solid;
        border-color: var(--bs-table-border-color);
        flex: 1 0 0%;
        padding: 0px;
    }

    .row {
        border-top: 1px solid;
        border-color: var(--bs-table-border-color);
        width: 100%;
        margin: 0px;
        --bs-gutter-x: 1.5rem;
        --bs-gutter-y: 0;
        display: flex;
        flex-wrap: wrap;
    }

    .row-subheader {
        border-color: var(--bs-table-border-color);
        width: 100%;
        margin: 0px;
        --bs-gutter-x: 1.5rem;
        --bs-gutter-y: 0;
        display: flex;
        flex-wrap: wrap;
    }

    .header-table {
        display: flex;
        border-collapse: collapse;
        --bs-table-color: var(--bs-body-color);
        --bs-table-bg: transparent;
        --bs-table-border-color: var(--bs-border-color);
        --bs-table-accent-bg: transparent;
        --bs-table-striped-color: var(--bs-body-color);
        --bs-table-striped-bg: rgba(0, 0, 0, 0.05);
        --bs-table-active-color: var(--bs-body-color);
        --bs-table-active-bg: rgba(0, 0, 0, 0.1);
        --bs-table-hover-color: var(--bs-body-color);
        --bs-table-hover-bg: rgba(0, 0, 0, 0.075);
        width: 100%;
        color: var(--bs-table-color);
        vertical-align: top;
        border: 1px solid;
        border-color: var(--bs-table-border-color);
        background: #f8f9fa;
    }

    hr {
        color: inherit;
        border: 0;
        border-top: 1px solid;
        opacity: 0.25;
        width: 100%;
        margin: 0px;
    }

    .scroll-container {
        width: 210%;
        overflow-x: auto;
        white-space: nowrap;
    }

    td.td-col-2 {
        width: 6.51%;
    }

    td.td-col-3 {
        width: 14.98%;
    }

    td.td-col-sm-6 {
        width: 3.22%;
    }

    td.td-col-sm {
        width: 3.26%;
    }

    // styling cho subheader-column
    td.td-col-week-1 {
        width: 2.15%;
    }

    td.td-colsubheader-week-1 {
        width: 2.37%;
    }

    td.td-colsubtitle-week-1 {
        width: 2.14%;
    }

    td.td-col-week-2 {
        width: 2.13%;
    }

    td.td-colsubheader-week-2 {
        width: 2.38%;
    }

    td.td-colsubtitle-week-2 {
        width: 2.14%;
    }

    td.td-col-week-3 {
        width: 2.15%;
    }

    td.td-colsubheader-week-3 {
        width: 2.38%;
    }

    td.td-colsubtitle-week-3 {
        width: 2.12%;
    }

    td.td-col-week-4 {
        width: 2.17%;
    }

    td.td-colsubheader-week-4 {
        width: 2.38%;
    }

    td.td-colsubtitle-week-4 {
        width: 2.12%;
    }

    td.td-col-week-5 {
        width: 2.17%;
    }

    td.td-colsubheader-week-5 {
        width: 2.35%;
    }

    td.td-colsubtitle-week-5 {
        width: 2.18%;
    }

    td.td-colsubheader-title {
        width: 5%;
    }

    td.td-colsubheader-sumary {
        width: 3.02%;
    }
</style>
