<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />

        <div class="mb-5">
            <div>
                <div class="d-flex justify-content-center align-items-center">
                    <div class="d-flex flex-column">
                        <div class="flex-column d-flex mb-3 me-4">
                            <label for="project" class="justify-content-center d-flex">Dự án:</label>
                            <Dropdown
                                v-model="selectedProject"
                                :options="projectList"
                                optionLabel="projectName"
                                placeholder="Chọn Dự Án"
                                id="project"
                                filter="true"
                                class="projectDropdown"
                                :disabled="selectedStaff !== null"
                            />
                        </div>
                    </div>

                    <div class="d-flex flex-column">
                        <div class="flex-column d-flex mb-3 me-4">
                            <label for="staffName" class="justify-content-center d-flex">Tên Nhân Viên:</label>
                            <Dropdown
                                v-model="selectedStaff"
                                :options="allStaffList"
                                filter
                                optionLabel="fullName"
                                placeholder="Chọn Nhân Viên"
                                class="staffNameDropdown"
                                id="staffName"
                                :disabled="selectedProject !== null"
                            />
                        </div>
                        <!-- <div class="d-flex flex-column">
                            <label for="pointOfTime" class="text-center">Khoảng Thời Gian</label>
                            <div class="d-flex">
                                <date-picker
                                    v-model:value="startDate"
                                    type="date"
                                    placeholder="Chọn Ngày"
                                    :disabled="selectedProject === null"
                                ></date-picker>
                                <h3>~</h3>
                                <date-picker
                                    v-model:value="endDate"
                                    type="date"
                                    placeholder="Chọn Ngày"
                                    :disabled="selectedProject === null"
                                ></date-picker>
                            </div>
                        </div> -->
                    </div>
                    <div class="d-flex flex-column">
                        <div class="d-flex flex-column mb-3 me-4">
                            <label for="month" class="justify-content-center d-flex">Khoảng thời gian:</label
                            >{{ selectedPointDate }}
                            <div class="d-flex gap-3">
                                <date-picker
                                    v-model:value="startDate"
                                    format="YYYY-MM-DD"
                                    type="month"
                                    placeholder="Chọn Tháng"
                                    :default-value="getDefaultMonth()"
                                    :disabled="selectedProject === null && selectedStaff === null"
                                ></date-picker>
                            </div>
                        </div>
                    </div>

                    <Button
                        type="button"
                        icon="pi pi-filter-slash "
                        class="custom-reload-h me-3"
                        @click="handlerReload()"
                        v-tooltip.top="'Bỏ lọc'"
                        style="height: 48px; margin-top: 8px"
                    />
                    <Export
                        style="margin-left: 10px; height: 48px; margin-top: 8px"
                        class="custom-button-h"
                        label="Xuất Excel"
                        @click="exportBill()"
                    />
                </div>
            </div>
        </div>
        <div class="scroll">
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
                            <div class="container" style="padding-right: 0px; padding-left: 0px">
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
                <div class="vl-parent" ref="formContainer">
                    <div v-if="this.selectedStaff === null">
                        <table class="table table-bordered">
                            <tbody>
                                <tr v-for="(value, index) in billable" :key="value.id">
                                    <th scope="row">{{ index + 1 }}</th>
                                    <!-- Name -->
                                    <td class="td-col-2">{{ value.name }}</td>
                                    <!-- Project Name -->
                                    <td class="td-col-3">
                                        {{ this.selectedProject.name }}
                                    </td>
                                    <!-- <td class="td-col-3">IMS</td> -->

                                    <!-- Leader Name -->
                                    <td class="td-col-2">
                                        {{ value.leader }}
                                    </td>
                                    <!-- <td class="td-col-2">NVM</td> -->
                                    <!-- </div> -->

                                    <td class="td-col-sm-6">0</td>
                                    <td class="td-col-sm">0</td>
                                    <!-- column tuần -->

                                    <td class="td-col-week-1">{{ value.weeks[0].totalEst || 0 }}</td>
                                    <td class="td-colsubheader-week-1">{{ value.weeks[0].totalTimes || 0 }}</td>
                                    <td class="td-colsubtitle-week-1">0</td>

                                    <td class="td-col-week-2">{{ value.weeks[1].totalEst || 0 }}</td>
                                    <td class="td-colsubheader-week-2">{{ value.weeks[1].totalTimes || 0 }}</td>
                                    <td class="td-colsubtitle-week-2">0</td>

                                    <td class="td-col-week-3">{{ value.weeks[2].totalEst || 0 }}</td>
                                    <td class="td-colsubheader-week-3">{{ value.weeks[2].totalTimes || 0 }}</td>
                                    <td class="td-colsubtitle-week-3">0</td>

                                    <td class="td-col-week-4">{{ value.weeks[3].totalEst || 0 }}</td>
                                    <td class="td-colsubheader-week-4">{{ value.weeks[3].totalTimes || 0 }}</td>
                                    <td class="td-colsubtitle-week-4">0</td>

                                    <td class="td-col-week-5">{{ value.weeks[4].totalEst || 0 }}</td>
                                    <td class="td-colsubheader-week-5">{{ value.weeks[4].totalTimes || 0 }}</td>
                                    <td class="td-colsubtitle-week-5">0</td>
                                    <!-- column tuần -->

                                    <td class="td-colsubheader-title">{{ value.month_time_estimate }}</td>
                                    <td class="td-colsubheader-title">{{ value.month_time_spent }}</td>
                                    <td class="td-colsubheader-title">0</td>

                                    <td class="td-colsubheader-sumary">{{ value.month_time_estimate }}</td>
                                    <td class="td-colsubheader-sumary">{{ value.month_time_spent }}</td>
                                    <td class="td-colsubheader-sumary">0</td>
                                    <td class="td-colsubheader-sumary">{{ value.efficiency }}</td>
                                    <td></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div v-else>
                        <table class="table table-bordered">
                            <tbody>
                                <tr v-for="(value, index) in resultSelectedPersonArray" :key="index">
                                    <th scope="row">{{ index + 1 }}</th>
                                    <!-- Name -->
                                    <td class="td-col-2">
                                        {{ this.selectedStaff.lastName + ' ' + this.selectedStaff.firstName }}
                                    </td>
                                    <!-- Project Name -->
                                    <td class="td-col-3">
                                        {{ value.projectName }}
                                    </td>
                                    <!-- <td class="td-col-3">IMS</td> -->

                                    <!-- Leader Name -->
                                    <td class="td-col-2">{{ value.leader }}</td>
                                    <!-- <td class="td-col-2">NVM</td> -->
                                    <!-- </div> -->

                                    <td class="td-col-sm-6">0</td>
                                    <td class="td-col-sm">0</td>
                                    <!-- column tuần -->

                                    <td class="td-col-week-1">
                                        {{ value.week1Estimate.TotalEstimate || 0 }}
                                    </td>
                                    <td class="td-colsubheader-week-1">
                                        {{ value.week1SpentTime.TotalTimeSpent || 0 }}
                                    </td>
                                    <td class="td-colsubtitle-week-1">0</td>

                                    <td class="td-col-week-2">
                                        {{ value.week2Estimate.TotalEstimate || 0 }}
                                    </td>
                                    <td class="td-colsubheader-week-2">
                                        {{ value.week2SpentTime.TotalTimeSpent || 0 }}
                                    </td>
                                    <td class="td-colsubtitle-week-2">0</td>

                                    <td class="td-col-week-3">
                                        {{ value.week3Estimate.TotalEstimate || 0 }}
                                    </td>
                                    <td class="td-colsubheader-week-3">
                                        {{ value.week3SpentTime.TotalTimeSpent || 0 }}
                                    </td>
                                    <td class="td-colsubtitle-week-3">0</td>

                                    <td class="td-col-week-4">
                                        {{ value.week4Estimate.TotalEstimate || 0 }}
                                    </td>
                                    <td class="td-colsubheader-week-4">
                                        {{ value.week4SpentTime.TotalTimeSpent || 0 }}
                                    </td>
                                    <td class="td-colsubtitle-week-4">0</td>

                                    <td class="td-col-week-5">
                                        {{ value.week5Estimate.TotalEstimate || 0 }}
                                    </td>
                                    <td class="td-colsubheader-week-5">
                                        {{ value.week5SpentTime.TotalTimeSpent || 0 }}
                                    </td>
                                    <td class="td-colsubtitle-week-5">0</td>
                                    <!-- column tuần -->

                                    <td class="td-colsubheader-title">
                                        {{ value.monthEstimatePeformance }}
                                    </td>
                                    <td class="td-colsubheader-title">
                                        {{ value.monthSpentTimePeformance }}
                                    </td>
                                    <td class="td-colsubheader-title">0</td>

                                    <td class="td-colsubheader-sumary">
                                        {{ value.monthEstimatePeformance }}
                                    </td>
                                    <td class="td-colsubheader-sumary">
                                        {{ value.monthSpentTimePeformance }}
                                    </td>
                                    <td class="td-colsubheader-sumary">0</td>
                                    <td class="td-colsubheader-sumary">
                                        {{ value.efficiency }}
                                    </td>
                                    <td></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </LayoutDefaultDynamic>
</template>
<script>
    import {
        HTTP,
        HTTP_API_GITLAB,
        GET_ALL_ISSUE_IN_PROJECT,
        GET_ALL_NOTES_BY_ISSUE,
        KEY_MAP,
        GET_ALL_PROJECT,
        PROJECT_MEMBER,
        GET_ALL_USERS,
        GET_ALL_USER_IN_PROJECT_DATABASE,
        GET_ALL_PROJECT_DATABASE,
        STAFF_IS_IN_PROJECT_CHECK,
        GET_ALL_USERS_DATABASE,
        GET_ALL_PARTICIPANTS_BY_ISSUE,
    } from '@/http-common'
    import * as XLSX from 'xlsx-js-style'
    import { FormatExcel } from '@/helper/formatExcel.helper'
    import { DateHelper } from '@/helper/date.helper'
    import Export from '../../components/buttons/Export.vue'
    import { handleError } from 'vue'
    export default {
        data() {
            return {
                listProjectStaffIn: [],
                startDate: null,
                endDate: null,
                selectedPointDate: null,
                projectList: [],
                selectedProject: null,
                staffList: [],
                allStaffList: [],
                selectedStaff: null,
                selectedStaffId: null,
                reportList: [],
                filterByPointOfDateList: [],
                filterWithAddedFieldList: [],
                filterWithEstimateList: [],
                date: null,
                currentMonth: null,
                currentYear: null,
                validWeeks: [],
                taskList: [],
                weekStart: null,
                weekEnd: null,
                staff_Infos: [],
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
                projectCode_Gitlab: [],
                totalWorkingHours: null,
                workingHoursPerDay: 8,
                issueList: [],
                reportList: [],
                filteredByWeek: [],
                merge_spent_estimate: [],
                noteid_List: [],
                invidualSelected: [],
                subProjectCode: null,
                leader: null,
                resultSelectedPersonArray: [],
                fullPage: false,
            }
        },
        watch: {
            selectedProject(newVal, oldVal) {
                if (this.selectedProject !== null) {
                    this.getDate()
                    this.getAllStaffInProject(this.selectedProject.id)
                    this.projectCode = this.selectedProject.id
                    this.subProjectCode = this.selectedProject.subProjectCode
                    this.projectCode_Gitlab = this.selectedProject.projectCode
                    this.getAllIssueInProject()
                }
            },
            selectedStaff(newVal, oldVal) {
                if (this.selectedStaff !== null) {
                    this.selectedStaffId = this.selectedStaff.idUserGitLab
                    this.FindProjectWithIdStaff()
                }
            },
            startDate(newVal, oldVal) {
                if (this.startDate !== null && this.selectedProject !== null) {
                    this.getDate()
                    this.getAllIssueInProject()
                } else {
                    this.getDate()
                    this.invidualSelected = []
                    this.listProjectStaffIn = []
                    if (this.selectedStaff !== null) {
                        this.FindProjectWithIdStaff()
                    }
                }
            },
        },
        created() {
            this.getAllProject()
            this.getAllStaff()
            this.calculateTotalWorkingHours()
            this.startDate = this.getDefaultMonth()
            this.getDate()
        },
        methods: {
            handlerReload() {
                this.selectedStaff = null
                this.selectedProject = null
                this.billable = []
                this.billable = []
                this.issueList = []
                this.noteid_List = []
                this.reportList = []
                this.staff_Info = []
                this.filterWithAddedFieldList = []
                this.filterWithEstimateList = []
                this.merge_spent_estimate = []
                this.memberProject = []
                this.invidualSelected = []
                this.listProjectStaffIn = []
                this.resultSelectedPersonArray = []
            },
            loading() {
                try {
                    if (this.$refs.formContainer) {
                        let load = this.$loading.show({
                            // Optional parameters
                            container: this.fullPage ? null : this.$refs.formContainer,
                            canCancel: true,
                            onCancel: this.load,
                        })

                        setTimeout(() => {
                            load.hide()
                        }, 5000)
                    }
                } catch (error) {
                    console.log('handle loading error')
                }
            },

            async getAllProject() {
                try {
                    const res = await HTTP.get(GET_ALL_PROJECT_DATABASE)
                    this.projectList = res.data._Data.filter((project) => project.isOnGitlab === true)
                } catch (err) {
                    console.log(err)
                }
                this.projectList = JSON.parse(JSON.stringify(this.projectList))
            },
            async FindProjectWithIdStaff() {
                this.listProjectStaffIn = []
                this.loading()
                try {
                    const requests = this.projectList.map(async (project) => {
                        try {
                            const res = await HTTP_API_GITLAB.get(
                                STAFF_IS_IN_PROJECT_CHECK(project.projectCode, this.selectedStaffId),
                            )

                            if (res.status === 200) {
                                this.listProjectStaffIn.push(project)
                            }
                        } catch (err) {
                            if (err.response && err.response.status === 404) {
                                // Handle the 404 error (user not found in the project) silently
                                return
                            } else {
                                console.log('Handle other error:', err)
                            }
                        }
                    })

                    await Promise.all(requests)

                    await this.getAllIssueInListProjectStaffIn()
                } catch (error) {
                    console.log('handle error')
                }
            },
            async getAllStaff(page = 1) {
                try {
                    const res = await HTTP.get(GET_ALL_USERS_DATABASE)
                    const newData = res.data

                    this.allStaffList = newData._Data
                    // if (newData.length > 0) {
                    //     await this.getAllStaff(page + 1)
                    // }
                } catch (err) {
                    console.log(err)
                }
            },
            getAllStaffInProject(id) {
                this.loading()
                HTTP.get(GET_ALL_USER_IN_PROJECT_DATABASE(id))
                    .then((res) => {
                        this.staffList = res.data
                    })
                    .catch((err) => {
                        console.log(err)
                    })
            },

            async getProjectLeader() {
                if (this.selectedStaffId !== null) {
                    this.resultSelectedPersonArray.forEach(async (element) => {
                        await HTTP.get(`/Project/GetLeadByProject/${element.subIdDB}`)
                            .then(async (res) => {
                                element.leader = res.data.fullName
                            })
                            .catch((er) => {
                                console.log(er)
                            })
                    })
                } else {
                    const id = this.subProjectCode
                    await HTTP.get(`/Project/GetLeadByProject/${id}`)
                        .then(async (res) => {
                            this.memberProject = res.data.fullName
                            this.billable.forEach((assignee) => {
                                assignee.leader = this.memberProject
                            })
                        })
                        .catch((er) => {
                            console.log(er)
                        })
                }
            },
            getDefaultMonth() {
                const currentDate = new Date()
                const year = currentDate.getFullYear()
                const month = currentDate.getMonth() + 1
                return `${year}-${month.toString().padStart(2, '0')}-01`
            },
            getDate() {
                this.billable = []
                this.issueList = []
                this.noteid_List = []
                this.reportList = []
                this.staff_Info = []
                this.filterWithAddedFieldList = []
                this.filterWithEstimateList = []
                this.merge_spent_estimate = []
                this.memberProject = []
                this.invidualSelected = []
                this.listProjectStaffIn = []
                this.resultSelectedPersonArray = []

                this.validWeeks = []
                if (this.startDate === null) {
                    this.startDate = this.getDefaultMonth()
                }
                const currentDate = new Date(this.startDate)
                // const currentDate = new Date()
                this.currentMonth = currentDate.getMonth() + 1
                // this.currentMonth = 4

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
                    // console.log('tuần của tháng', this.validWeeks)
                }
            },
            async getAllIssueInListProjectStaffIn() {
                try {
                    const pageSize = 100
                    const page = 1
                    this.billable = []
                    this.invidualSelected = []

                    const startDay = '01'
                    const endDay = new Date(this.currentYear, this.currentMonth, 0)
                        .getDate()
                        .toString()
                        .padStart(2, '0')
                    const created_after = new Date(
                        `${this.currentYear}-${this.currentMonth.toString().padStart(2, '0')}-${startDay}`,
                    )
                    created_after.setHours(0, 0, 0) // Set the start time of the date to 00:00:00

                    const created_before = new Date(
                        `${this.currentYear}-${this.currentMonth.toString().padStart(2, '0')}-${endDay}`,
                    )
                    created_before.setHours(23, 59, 59) // Set the end time of the date to 23:59:59

                    for (let i = 0; i < this.listProjectStaffIn.length; i++) {
                        const x = this.listProjectStaffIn[i]

                        const issues = await this.getIssuesByDate(x.projectCode, pageSize, page)

                        const projectId = x.projectCode
                        const projectIdInDB = x.subProjectCode

                        if (!this.invidualSelected.some((e) => e.projectId === projectId)) {
                            const ProjectAndListIssue = {
                                projectId: projectId,
                                subIdDB: projectIdInDB,
                                projectName: x.name,
                                issue: issues,
                            }

                            const staff = {
                                id: this.selectedStaff.id,
                                name: this.selectedStaff.name,
                            }

                            this.staff_Info = staff
                            this.invidualSelected.push(ProjectAndListIssue)

                            await Promise.all(
                                this.invidualSelected.map(async (e) => {
                                    const projectId = e.projectId
                                    const notes = []

                                    await Promise.all(
                                        e.issue.map(async (element) => {
                                            const note = await this.fetchNotesForIssueWithListProject(
                                                projectId,
                                                element.iid,
                                            )
                                            notes.push(...note) // Concatenate the note array into the notes array
                                        }),
                                    )

                                    e.notes = notes
                                }),
                            )
                        }
                    }
                    await this.getTheAddedItemAndEstimateWithExactlyPerson()
                } catch (error) {
                    console.log(error)
                }
            },
            async getAllIssueInProject() {
                this.billable = []
                this.issueList = []
                this.noteid_List = []
                this.reportList = []
                this.staff_Info = []
                this.filterWithAddedFieldList = []
                this.filterWithEstimateList = []
                this.merge_spent_estimate = []
                this.memberProject = []

                const idProject = this.projectCode_Gitlab
                const pageSize = 100
                const page = 1

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
                // gộp các phần tử có chung iid( issue id)
                const issues = await this.getIssuesByDate(idProject, pageSize, page)
                issues.forEach((e) => {
                    if (!this.issueList.some((issue) => issue.iid === e.iid)) {
                        const { iid, assignees } = e
                        let assigneeId = null
                        let assigneeName = null
                        if (assignees.length > 0) {
                            assigneeId = assignees[0].id
                            assigneeName = assignees[0].name
                        }
                        this.issueList.push({ iid, assigneeId, assigneeName })
                    }
                })

                await this.issueList.forEach((element) => {
                    const ProjectId = idProject
                    this.fetchNotesForIssue(ProjectId, element.iid)
                })

                // console.log('danh sach issue', this.issueList)
                this.staff_Info = await this.getAssigneeList(issues)
            },

            async getIssuesByDate(idProject, pageSize, page) {
                const response = await HTTP_API_GITLAB.get(GET_ALL_ISSUE_IN_PROJECT(idProject, pageSize, page))
                return response.data.filter((item) => item.assignee !== null)
            },
            // lấy nhân viên
            async getAssigneeList(data) {
                const assigneeList = []

                for (const bill of data) {
                    const assigneeId = bill.assignee.id
                    const assigneeName = bill.assignee.name
                    const noteable_id = bill.iid
                    // Kiểm tra xem assigneeId đã tồn tại trong assigneeList chưa
                    const existingAssignee = assigneeList.find((assignee) => assignee.id === assigneeId)

                    if (!existingAssignee) {
                        assigneeList.push({
                            id: assigneeId,
                            name: assigneeName,
                            noteable_id: noteable_id,
                        })
                    }
                }
                return assigneeList
            },
            // lấy tất cả note trong issue
            async fetchNotesForIssue(projectId, issueId) {
                this.reportList = []

                const pageSize = 100
                let page = 1
                try {
                    while (true) {
                        const response = await HTTP_API_GITLAB.get(
                            GET_ALL_NOTES_BY_ISSUE(projectId, issueId, pageSize, page),
                        )
                        const newData = response.data

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
                    // Handle the error here
                }
            },
            async fetchNotesForIssueWithListProject(projectId, issueId) {
                const pageSize = 100
                let page = 1
                let reportList = []

                try {
                    while (true) {
                        const response = await HTTP_API_GITLAB.get(
                            GET_ALL_NOTES_BY_ISSUE(projectId, issueId, pageSize, page),
                        )
                        const newData = response.data

                        if (newData.length > 0) {
                            reportList = reportList.concat(newData)
                            page++
                        } else {
                            break
                        }
                    }

                    return reportList
                } catch (error) {
                    console.error(error)
                    // Handle the error here
                }
            },

            async getTheAddedItem() {
                this.filterWithAddedFieldList = []
                this.filterWithEstimateList = []
                this.merge_spent_estimate = []

                this.filterWithAddedFieldList = this.reportList
                    .filter((x) => x.body.startsWith('added')) // Filter for items starting with "added"
                    .filter((addedItem) => {
                        const matchingRemovedItem = this.reportList.find(
                            (x) =>
                                x.body.startsWith('removed') &&
                                x.noteable_iid === addedItem.noteable_iid &&
                                x.created_at > addedItem.created_at,
                        )

                        return !matchingRemovedItem // Keep the "added" item if no matching "removed" item is found
                    })
                    .map((item) => JSON.parse(JSON.stringify(item)))
                console.log(this.filterWithAddedFieldList)
                this.filterWithEstimateList = this.reportList
                    .filter((x) => x.body.startsWith('changed time estimate to'))
                    .map((item) => JSON.parse(JSON.stringify(item)))
                // Khởi tạo một Map để lưu trữ kết quả đã gộp chung
                const mergedResults = new Map()
                // Duyệt qua mảng filterWithEstimateList lấy ngày tạo gần nhất
                for (const item of this.filterWithEstimateList) {
                    const noteableId = item.noteable_id
                    const createdAt = new Date(item.created_at)

                    // Kiểm tra xem noteableId đã tồn tại trong Map chưa
                    if (!mergedResults.has(noteableId)) {
                        // Nếu chưa tồn tại, thêm phần tử vào Map
                        mergedResults.set(noteableId, item)
                    } else {
                        // Nếu đã tồn tại, so sánh createdAt với phần tử đã lưu trong Map
                        const existingItem = mergedResults.get(noteableId)
                        const existingCreatedAt = new Date(existingItem.created_at)

                        // So sánh createdAt của phần tử hiện tại với createdAt đã lưu trong Map
                        if (createdAt > existingCreatedAt) {
                            // Nếu createdAt hiện tại lớn hơn, thay thế giá trị trong Map
                            mergedResults.set(noteableId, item)
                        }
                    }
                }

                const mergedResultsArray = Array.from(mergedResults.values())
                this.filterWithEstimateList = JSON.parse(JSON.stringify(mergedResultsArray))

                // Gán giá trị từ this.staff_Info vào this.billable
                this.billable = this.staff_Info.map((assignee) => {
                    return {
                        id: assignee.id,
                        name: assignee.name,
                        leader: null,
                        noteable_id: [],
                        weeks: [[], [], [], [], []],
                        total_time_spent: 0,
                        total_time_estimate: 0,
                        month_time_spent: 0,
                        month_time_estimate: 0,
                        efficiency: 0,
                    }
                })
                const noteableIdsByAssignee = {}

                // Lấy  issue Id theo nhân viên
                for (const assignee of this.billable) {
                    const assigneeId = assignee.id
                    const noteableIdsSet = new Set()

                    for (const item of this.issueList) {
                        if (item.assigneeId === assigneeId) {
                            noteableIdsSet.add(item.iid)
                        }
                    }
                    const noteableIds = Array.from(noteableIdsSet)
                    noteableIdsByAssignee[assigneeId] = noteableIds
                    assignee.noteable_id = noteableIds
                }
                this.getProjectLeader()
                this.filterReportsByDate()
            },
            async getTheAddedItemAndEstimateWithExactlyPerson() {
                await Promise.all(
                    this.invidualSelected.map(async (e) => {
                        if (e.notes) {
                            const addedNotes = JSON.parse(
                                JSON.stringify(
                                    e.notes
                                        .filter(
                                            (x) => x.body.startsWith('added') && x.author.id === this.selectedStaffId,
                                        )
                                        .filter((addedItem) => {
                                            const matchingRemovedItem = e.notes.find(
                                                (x) =>
                                                    x.body.startsWith('removed') &&
                                                    x.noteable_iid === addedItem.noteable_iid &&
                                                    x.created_at > addedItem.created_at,
                                            )
                                            return !matchingRemovedItem
                                        })
                                        .map((item) => JSON.parse(JSON.stringify(item))),
                                ),
                            )
                            console.log(addedNotes)
                            const changedNotes = e.notes
                                .filter((note) => note.body.startsWith('changed time estimate to'))
                                .map((note) => {
                                    const author = { ...note.author } // Copy the properties of the proxied author object
                                    return { ...note, author } // Create a new object with the copied author object
                                })

                            e.notes = addedNotes.map((note) => ({ ...note }))

                            e.estimate = changedNotes
                        } else {
                            e.notes = [] // Set an empty array if notes is undefined or null
                        }
                    }),
                )

                await this.filterEstimate()
            },
            async filterEstimate() {
                await Promise.all(
                    this.invidualSelected.map(async (obj) => {
                        if (obj.estimate && Array.isArray(obj.estimate)) {
                            const { estimate } = obj

                            const groupedEstimates = new Map()

                            await Promise.all(
                                estimate.map(async (est) => {
                                    const { noteable_iid, created_at } = est

                                    if (!groupedEstimates.has(noteable_iid)) {
                                        groupedEstimates.set(noteable_iid, [])
                                    }

                                    groupedEstimates.get(noteable_iid).push(est)
                                }),
                            )

                            const newestEstimates = []
                            groupedEstimates.forEach((estimates) => {
                                estimates.sort((a, b) => new Date(b.created_at) - new Date(a.created_at))
                                newestEstimates.push(estimates[0])
                            })

                            obj.estimate = JSON.parse(JSON.stringify(newestEstimates))
                        }
                    }),
                )
                this.resultSelectedPersonArray = this.invidualSelected

                this.resultSelectedPersonArray.forEach((item) => {
                    if (item.notes && Array.isArray(item.notes)) {
                        const noteableIids = item.notes.reduce((uniqueIids, note) => {
                            const noteableIid = note.noteable_iid

                            if (!uniqueIids.includes(noteableIid)) {
                                uniqueIids.push(noteableIid)
                            }

                            return uniqueIids
                        }, [])

                        item.noteableIids = noteableIids

                        if (item.estimate && Array.isArray(item.estimate)) {
                            item.estimate = JSON.parse(
                                JSON.stringify(item.estimate.filter((est) => noteableIids.includes(est.noteable_iid))),
                            )
                        }
                    }
                })
                // console.log('aloo', this.resultSelectedPersonArray)
                await this.filterEstimateByDateWithExactlyPerson()
            },

            async filterEstimateByDateWithExactlyPerson() {
                try {
                    for (let i = 0; i < this.validWeeks.length; i++) {
                        const week = this.validWeeks[i]
                        const weekRange = week.split('-')
                        const weekStart = parseInt(weekRange[0])
                        const weekEnd = parseInt(weekRange[1])
                        const startWeekDate = new Date(this.currentYear, this.currentMonth - 1, weekStart)
                        const endWeekDate = new Date(this.currentYear, this.currentMonth - 1, weekEnd)
                        for (let index = 0; index < this.resultSelectedPersonArray.length; index++) {
                            const x = this.resultSelectedPersonArray[index]
                            const weekEstimate = []
                            x.estimate.forEach((element) => {
                                const date = DateHelper.formatDate(element.created_at)
                                if (
                                    date >= DateHelper.formatDate(startWeekDate) &&
                                    date <= DateHelper.formatDate(endWeekDate)
                                ) {
                                    weekEstimate.push(JSON.parse(JSON.stringify(element)))
                                }
                            })
                            // Calculate total time spent for the week
                            let totalEstimate = 0
                            weekEstimate.forEach((est) => {
                                const timeString = est.body // Assuming the time value is stored in the 'body' property
                                totalEstimate += this.calculateTimeEstimate(timeString)
                            })
                            // Add the filtered week array and the total time spent as an object to the current object in this.invidualSelected
                            this.resultSelectedPersonArray[index][`week${i + 1}Estimate`] = {
                                ...weekEstimate,
                                TotalEstimate: totalEstimate,
                            }
                        }
                    }
                } catch (error) {
                    console.error(error)
                }
                await this.filterReportsByDateWithExactlyPerson()
            },
            async filterReportsByDateWithExactlyPerson() {
                try {
                    for (let i = 0; i < this.validWeeks.length; i++) {
                        const week = this.validWeeks[i]
                        const weekRange = week.split('-')
                        const weekStart = parseInt(weekRange[0])
                        const weekEnd = parseInt(weekRange[1])

                        const startWeekDate = new Date(this.currentYear, this.currentMonth - 1, weekStart)
                        const endWeekDate = new Date(this.currentYear, this.currentMonth - 1, weekEnd)

                        for (let index = 0; index < this.resultSelectedPersonArray.length; index++) {
                            const x = this.resultSelectedPersonArray[index]
                            const weekNotes = []

                            x.notes.forEach((element) => {
                                const date = DateHelper.formatDate(element.created_at)
                                if (
                                    date >= DateHelper.formatDate(startWeekDate) &&
                                    date <= DateHelper.formatDate(endWeekDate)
                                ) {
                                    weekNotes.push(JSON.parse(JSON.stringify(element)))
                                }
                            })

                            // Find "remove" items and remove preceding "added" items
                            let removeTimestamp = null
                            let filteredNotes = weekNotes.filter((note) => {
                                if (note.body.startsWith('remove')) {
                                    removeTimestamp = note.created_at
                                    return true // Include the "remove" item itself
                                }
                                return !removeTimestamp || note.created_at >= removeTimestamp
                            })

                            // Remove "added" items created before the "remove" item
                            if (removeTimestamp) {
                                filteredNotes = filteredNotes.filter((note) => {
                                    return note.created_at >= removeTimestamp || !note.body.startsWith('added')
                                })
                            }

                            // Remove the "remove" item itself from the filtered notes
                            if (removeTimestamp) {
                                filteredNotes = filteredNotes.filter((note) => !note.body.startsWith('remove'))
                            }

                            // Recalculate total time spent for the week after removing "added" items and "remove" item
                            const totalTimespent = filteredNotes.reduce((total, note) => {
                                const timeString = note.body // Assuming the time value is stored in the 'body' property
                                return total + this.calculateTimespent(timeString)
                            }, 0)

                            // Add the filtered week array and the total time spent as an object to the current object in this.resultSelectedPersonArray
                            this.resultSelectedPersonArray[index][`week${i + 1}SpentTime`] = {
                                ...filteredNotes,
                                TotalTimeSpent: isNaN(totalTimespent) ? 0 : totalTimespent,
                            }
                        }
                    }
                } catch (error) {
                    console.error(error)
                }
                await this.calculateMonthPeformanceOfPerson()
            },
            calculateMonthPeformanceOfPerson() {
                // Calculate the total of all TotalTimeSpent values

                this.resultSelectedPersonArray.forEach((individual) => {
                    let totalTimeEstimate = 0
                    for (let i = 0; i < this.validWeeks.length; i++) {
                        const week = individual[`week${i + 1}Estimate`]
                        if (week && week.TotalEstimate) {
                            totalTimeEstimate += week.TotalEstimate
                        }
                    }
                    individual.TotalMonthEstimate = totalTimeEstimate
                })
                // Calculate the total of all TotalTimeSpent values

                this.resultSelectedPersonArray.forEach((individual) => {
                    let totalTimePersonSpent = 0
                    for (let i = 0; i < this.validWeeks.length; i++) {
                        const week = individual[`week${i + 1}SpentTime`]
                        if (week && week.TotalTimeSpent) {
                            totalTimePersonSpent += week.TotalTimeSpent
                        }
                    }

                    individual.TotalTimeMonthPersonSpent = totalTimePersonSpent
                })
                this.resultSelectedPersonArray.forEach((individual) => {
                    individual.monthEstimatePeformance = this.formatNumber(
                        individual.TotalMonthEstimate / this.totalWorkingHours,
                    )
                    individual.monthSpentTimePeformance = this.formatNumber(
                        individual.TotalTimeMonthPersonSpent / this.totalWorkingHours,
                    )
                    if (individual.monthEstimatePeformance === 0) {
                        individual.efficiency = 0
                    } else {
                        individual.efficiency = (
                            (individual.monthSpentTimePeformance / individual.monthEstimatePeformance) *
                            100
                        ).toFixed(2)
                        if (individual.efficiency === 'Infinity' || individual.efficiency === 'NaN') {
                            individual.efficiency = 0
                        }
                    }
                })
                this.getProjectLeader()
            },
            async filterReportsByDate() {
                const filteredByWeek_Spent = []
                const filteredByWeek_Est = []
                // Lấy danh sách time spent và est dựa theo 5 tuần của tháng
                for (const week of this.validWeeks) {
                    const weekRange = week.split('-')
                    const weekStart = parseInt(weekRange[0])
                    const weekEnd = parseInt(weekRange[1])

                    const startWeekDate = new Date(this.currentYear, this.currentMonth - 1, weekStart)
                    startWeekDate.setHours(0, 0, 0)
                    const endWeekDate = new Date(this.currentYear, this.currentMonth - 1, weekEnd)
                    endWeekDate.setHours(23, 59, 59)

                    const getTimeSpent = this.filterWithAddedFieldList
                        .flatMap((subArray) => subArray)
                        .filter((report) => {
                            const date = new Date(report.created_at)
                            return date >= startWeekDate && date <= endWeekDate
                        })

                    const getTimeEst = this.filterWithEstimateList
                        .flatMap((subArray) => subArray)
                        .filter((report) => {
                            const date = new Date(report.created_at)
                            return date >= startWeekDate && date <= endWeekDate
                        })

                    // chuyển đổi sang JSON
                    const convertedReports = JSON.parse(JSON.stringify(getTimeSpent))
                    filteredByWeek_Spent.push(convertedReports)

                    const convertedReports_Est = JSON.parse(JSON.stringify(getTimeEst))
                    filteredByWeek_Est.push(convertedReports_Est)
                }

                // console.log('filteredByWeek ', filteredByWeek_Spent)
                // console.log('est ', filteredByWeek_Est)

                // Lấy dữ liệu để tính time spent
                for (let i = 0; i < filteredByWeek_Spent.length; i++) {
                    const filteredWeekSpent = filteredByWeek_Spent[i]

                    for (const assignee of this.billable) {
                        const weekReportsByAuthorSpent = filteredWeekSpent.filter((weekReport, index, self) => {
                            return (
                                weekReport.author.id === assignee.id &&
                                self.findIndex((r) => r.id === weekReport.id) === index
                            )
                        })

                        if (weekReportsByAuthorSpent.length > 0) {
                            const sumOfTimeSpent = this.calculateTimeSum(weekReportsByAuthorSpent)

                            assignee.weeks[i] = {
                                reports: weekReportsByAuthorSpent,
                                totalTimes: sumOfTimeSpent.totalTimes,
                            }

                            assignee.total_time_spent += sumOfTimeSpent.totalTimes
                        }
                    }
                }
                // Lấy dữ liệu để tính time Est
                for (let i = 0; i < filteredByWeek_Est.length; i++) {
                    const filteredWeekEst = filteredByWeek_Est[i]

                    for (const assignee of this.billable) {
                        const weekReportsByAuthorEst = filteredWeekEst.filter((weekReport, index, self) => {
                            return (
                                assignee.noteable_id.includes(weekReport.noteable_iid) &&
                                self.findIndex((r) => r.id === weekReport.id) === index
                            )
                        })

                        if (weekReportsByAuthorEst.length > 0) {
                            const sumOfTimeEst = this.calculateTimeSum(weekReportsByAuthorEst)

                            if (!assignee.weeks[i]) {
                                assignee.weeks[i] = {}
                            }

                            assignee.weeks[i].reports = assignee.weeks[i].reports
                                ? [...assignee.weeks[i].reports, ...weekReportsByAuthorEst]
                                : weekReportsByAuthorEst

                            assignee.weeks[i].totalEst = this.formatNumber(sumOfTimeEst.totalEst)

                            assignee.total_time_estimate += parseFloat(this.formatNumber(sumOfTimeEst.totalEst))
                        }
                    }
                }
                // Lấy dữ liệu để tính tổng spent và est của tháng
                for (const assignee of this.billable) {
                    assignee.month_time_spent = this.formatNumber(assignee.total_time_spent / this.totalWorkingHours)
                    assignee.month_time_estimate = this.formatNumber(
                        assignee.total_time_estimate / this.totalWorkingHours,
                    )

                    assignee.efficiency =
                        assignee.month_time_estimate > 0
                            ? ((assignee.month_time_spent / assignee.month_time_estimate) * 100).toFixed(2)
                            : '0'
                }
            },

            formatNumber(num) {
                if (Number.isInteger(num)) {
                    return num.toString() // Trả về số tự nhiên dưới dạng chuỗi
                } else {
                    return num.toFixed(2) // Làm tròn số thập phân đến 2 chữ số và trả về chuỗi
                }
            },
            calculateTimespent(timeString) {
                let totalTimes = 0

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
                                totalTimes += numericValue // Hours already specified
                            }
                        }
                    }
                    return totalTimes
                }
            },
            calculateTimeEstimate(timeString) {
                let totalTimes = 0

                if (timeString.includes('changed')) {
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
                                totalTimes += numericValue // Hours already specified
                            }
                        }
                    }
                    return totalTimes
                }
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

                return {
                    totalTimes,
                    totalEst,
                }
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
            // xử lý tuần theo tháng
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
                        this.selectedProject.name,
                        ele.leader,
                        0,
                        0,
                        ele.weeks[0].totalEst ? ele.weeks[0].totalEst : 0,
                        ele.weeks[0].totalTimes ? ele.weeks[0].totalTimes : 0,
                        '0',
                        ele.weeks[1].totalEst ? ele.weeks[1].totalEst : 0,
                        ele.weeks[1].totalTimes ? ele.weeks[1].totalTimes : 0,
                        '0',
                        ele.weeks[2].totalEst ? ele.weeks[2].totalEst : 0,
                        ele.weeks[2].totalTimes ? ele.weeks[2].totalTimes : 0,
                        '0',
                        ele.weeks[3].totalEst ? ele.weeks[3].totalEst : 0,
                        ele.weeks[3].totalTimes ? ele.weeks[3].totalTimes : 0,
                        '0',
                        ele.weeks[4].totalEst ? ele.weeks[4].totalEst : 0,
                        ele.weeks[4].totalTimes ? ele.weeks[4].totalTimes : 0,
                        '0',
                        ele.month_time_estimate,
                        ele.month_time_spent,
                        '0',
                        ele.month_time_estimate,
                        ele.month_time_spent,
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
                var nameFile = this.selectedProject.name + DateHelper.formatDateDayjs(new Date()) + '.xlsx'
                XLSX.writeFile(wb, nameFile)
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
<style>
    .projectDropdown {
        width: 200px;
    }
    .staffNameDropdown {
        width: 200px;
    }
    .mx-input {
        height: 48px !important;
    }
</style>
<style lang="scss" scoped>
    .mx-input {
        height: 48px !important;
    }
    .scroll {
        position: relative;
        overflow: auto;
        width: 100%;
    }
    .keep {
        position: sticky;
        top: 0;
    }
    .table.table-bordered {
        height: 500px;
    }
    th {
        width: 2%;
        text-align: center;
        padding-top: 40px;
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
        padding-top: 40px;
    }

    td.td-col-3 {
        width: 14.98%;
        padding-top: 40px;
    }

    td.td-col-sm-6 {
        width: 3.22%;
        text-align: right;
        padding-top: 40px;
    }

    td.td-col-sm {
        width: 3.26%;
        text-align: right;
        padding-top: 40px;
    }

    // styling cho subheader-column
    td.td-col-week-1 {
        width: 2.15%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubheader-week-1 {
        width: 2.37%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubtitle-week-1 {
        width: 2.14%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-col-week-2 {
        width: 2.13%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubheader-week-2 {
        width: 2.38%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubtitle-week-2 {
        width: 2.14%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-col-week-3 {
        width: 2.15%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubheader-week-3 {
        width: 2.38%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubtitle-week-3 {
        width: 2.12%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-col-week-4 {
        width: 2.17%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubheader-week-4 {
        width: 2.38%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubtitle-week-4 {
        width: 2.12%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-col-week-5 {
        width: 2.17%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubheader-week-5 {
        width: 2.35%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubtitle-week-5 {
        width: 2.18%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubheader-title {
        width: 5%;
        text-align: center;
        padding-top: 40px;
    }

    td.td-colsubheader-sumary {
        width: 3.02%;
        text-align: center;
        padding-top: 40px;
    }
</style>
