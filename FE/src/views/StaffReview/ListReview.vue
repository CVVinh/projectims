<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-3 mt-3">
            <div class="row">
                <div class="col-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-2">
                            <li class="breadcrumb-item">DANH SÁCH NHÂN VIÊN ĐỢT ĐÁNH GIÁ</li>
                        </ol>
                    </nav>
                </div>
            </div>

            <nav class="navbar navbar-expand-lg bg-light navbar-light">
                <div class="container-fluid">
                    <button
                        class="navbar-toggler mb-2 mt-2 custom-input-h"
                        type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#collapsibleNavbar"
                    >
                        <span class="navbar-toggler-icon"></span>
                    </button>
                    <div class="collapse navbar-collapse mt-2" id="collapsibleNavbar">
                        <ul class="navbar-nav ms-auto">
                            <li class="nav-item me-2 mb-2">
                                <Button
                                    type="button"
                                    icon="pi pi-filter-slash"
                                    class="custom-reload-h"
                                    @click="handlerReload()"
                                    v-tooltip.top="'Bỏ lọc'"
                                />
                            </li>
                            <li class="nav-item me-2 mb-2">
                                <Dropdown
                                    v-model="selectedStaffReview"
                                    :options="staffReviewList"
                                    optionLabel="fullName"
                                    optionValue="id"
                                    placeholder="Chọn nhân viên"
                                    filter
                                    class="custom-input-h"
                                />
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>

            <div class="row mt-3">
                <div class="col-md-12">
                    <DataTable
                        class="p-datatable-customers"
                        :value="filteredList"
                        v-model:filters="filters"
                        :rows="5"
                        :rowHover="true"
                        :loading="loading"
                        responsiveLayout="scroll"
                        showGridlines
                        :sortOrder="1"
                        :sortField="'isConfirm'"
                    >
                        <template #empty> Không tìm thấy dữ liệu. </template>
                        <template #loading>
                            <ProgressSpinner />
                        </template>
                        <Column field="#" header="#">
                            <template #body="{ index }">
                                {{ index + 1 + (this.resultPgae.pageNumber - 1) * this.resultPgae.pageSize }}
                            </template>
                        </Column>
                        <Column field="userCode" header="Mã nhân viên">
                            <template #body="{ data }">
                                {{ data.userCode }}
                            </template>
                        </Column>
                        <Column field="staffReviewName" header="Tên nhân viên">
                            <template #body="{ data }">
                                {{ data.staffReviewName }}
                            </template>
                        </Column>
                        <Column field="totalPerformance" header="Hiệu suất">
                            <template #body="{ data }"> {{ data.totalPerformance }}% </template>
                        </Column>
                        <Column field="dateCreated" header="Ngày cập nhật">
                            <template #body="{ data }"> {{ this.formatDate(data.dateCreated) }} </template>
                        </Column>
                        <Column field="status" header="Trạng thái" sortable sortField="isConfirm">
                            <template #body="{ data }">
                                <div
                                    class="badge"
                                    :class="
                                        data.isConfirm === true
                                            ? 'bg-success'
                                            : data.isConfirm === false
                                            ? 'bg-danger'
                                            : 'bg-warning'
                                    "
                                >
                                    {{
                                        data.isConfirm === true
                                            ? 'Đã xác nhận'
                                            : data.isConfirm === false
                                            ? 'Từ chối xác nhận'
                                            : 'Đang chờ'
                                    }}
                                </div>
                            </template>
                        </Column>
                        <Column header="Thực thi">
                            <template #body="{ data }">
                                <!--  VIEW  -->
                                <div class="d-flex justify-content-start">
                                    <Button
                                        icon="pi pi-eye"
                                        class="p-button-sm custom-button-h me-2"
                                        aria-label="Submit"
                                        @click="openStaffReviewDetailsDialog(data, index)"
                                        v-tooltip.top="'Xem chi tiết'"
                                    />
                                    <!--  Delete -->
                                    <Delete
                                        @click="handleDelete(data.staffReview)"
                                        class="custom-button-h me-2"
                                        v-if="this.showButton.delete"
                                        :hidden="checkCanOperation('xoa', data)"
                                    ></Delete>
                                    <!-- Edit -->
                                    <Edit
                                        @click="openEditReview(data)"
                                        class="p-button-warning custom-button-h me-2"
                                        v-if="this.showButton.update"
                                    />
                                    <!-- Refuse   -->
                                    <Button
                                        icon="pi pi-times"
                                        @click="cancelConfirm(data)"
                                        class="p-button p-button-danger me-2 custom-button-h"
                                        v-if="this.showButton.refuse && data.staffReviewTime.isConfirm == false"
                                        :hidden="checkCanOperation('tuchoi', data)"
                                    />
                                </div>
                            </template>
                        </Column>
                    </DataTable>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <Pagination
                        v-model:pageNumber="resultPgae.pageNumber"
                        v-model:pageSize="resultPgae.pageSize"
                        :totalPages="totalMapPage"
                        :totalItems="this.totalItem"
                        :itemIndex="this.itemIndex"
                    />
                </div>
            </div>

            <Dialog
                v-model:visible="addOrEditDialog"
                :maximizable="true"
                modal
                header="Phiếu đánh giá nhân viên"
                :style="{ width: '80vw' }"
                :closable="false"
            >
                <table style="width: 100%; height: 100%" id="table">
                    <tr>
                        <th colspan="2">
                            <div class="container">
                                <div class="d-flex justify-content-between">
                                    <div class="align-items-center">
                                        <span class="me-2">Người đánh giá<span style="color: red">*</span>:</span>
                                        <div class="input-group">
                                            <Dropdown
                                                ref="pmLeadDropdown"
                                                v-model="selectedPmLead"
                                                :options="pmLeadList"
                                                filter
                                                optionLabel="fullName"
                                                placeholder="Tên người đánh giá"
                                                :class="{ 'p-invalid': isPmLeadSelected }"
                                            />
                                        </div>
                                    </div>
                                    <div class="align-items-center">
                                        <span class="me-2">Đánh giá nhân viên<span style="color: red">*</span>:</span>
                                        <div class="input-group">
                                            <Dropdown
                                                ref="staffDropdown"
                                                v-model="selectedStaff"
                                                :options="staffList"
                                                filter
                                                optionLabel="fullName"
                                                placeholder="Tên Nhân viên được đánh giá"
                                                :class="{ 'p-invalid': isStaffReviewSelected }"
                                            />
                                        </div>
                                    </div>
                                    <div class="align-items-center">
                                        <span class="me-2">Ngày đánh giá<span style="color: red">*</span>:</span>
                                        <div class="input-group">
                                            <Calendar v-model="staffReview.dateCreated" showIcon />
                                        </div>
                                    </div>
                                    <div class="align-items-center">
                                        <span class="me-2">Tổng hiệu suất:</span>
                                        <div class="input-group">
                                            <input
                                                type="number"
                                                class="form-control"
                                                :value="averagePerformance"
                                                aria-label=""
                                                style="height: 50px"
                                                disabled
                                                readonly
                                            />
                                            <div class="input-group-append">
                                                <span
                                                    class="input-group-text bg-primary text-white"
                                                    style="height: 50px"
                                                    >%</span
                                                >
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
                                                    min="0"
                                                    oninput="event.target.value = event.target.value.replace(/[^0-9]*/g,'');"
                                                    :class="{ invalid: item.invalid }"
                                                />
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
                                                    min="0"
                                                    oninput="event.target.value = event.target.value.replace(/[^0-9]*/g,'');"
                                                    :class="{ invalid: item.invalid }"
                                                />
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
                                    v-model="staffReview.achievements"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.achievements !== ''"
                                    >Nhập nội dung</label
                                >
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Kiến thức</td>
                        <td>
                            <div class="form-floating">
                                <textarea
                                    class="form-control"
                                    id="floatingTextarea"
                                    v-model="staffReview.knowledge"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.knowledge !== ''"
                                    >Nhập nội dung</label
                                >
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Kỹ năng</td>
                        <td>
                            <div class="form-floating">
                                <textarea
                                    class="form-control"
                                    id="floatingTextarea"
                                    v-model="staffReview.skill"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.skill !== ''"
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
                                    v-model="staffReview.forwardMindset"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.forwardMindset !== ''"
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
                                    v-model="staffReview.positiveMindset"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.positiveMindset !== ''"
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
                                    v-model="staffReview.steadfastMindset"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.steadfastMindset !== ''"
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
                                    v-model="staffReview.perfectionistMindset"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.perfectionistMindset !== ''"
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
                                    v-model="staffReview.fromTalkToResult"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.fromTalkToResult !== ''"
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
                                    v-model="staffReview.connectToAction"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.connectToAction !== ''"
                                    >Nhập nội dung</label
                                >
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #00b0f0">Sở thích</td>
                        <td>
                            <div class="form-floating">
                                <textarea
                                    class="form-control"
                                    id="floatingTextarea"
                                    v-model="staffReview.hobbies"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.hobbies !== ''"
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
                                    v-model="staffReview.personality"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.personality !== ''"
                                    >Nhập nội dung</label
                                >
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #ffd966">
                            Ý Kiến, đề xuất nguyện vọng và cam kết với Công ty<span style="color: red">*</span>
                        </td>
                        <td>
                            <div class="form-floating">
                                <textarea
                                    class="form-control"
                                    id="floatingTextarea"
                                    v-model="staffReview.commitmentsForCompany"
                                    :class="{ invalid: isCommitmentsForCompany }"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.commitmentsForCompany !== ''"
                                    >Nhập nội dung</label
                                >
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #ffd966">
                            Ý Kiến của Lead/ người làm cùng<span style="color: red">*</span>
                        </td>
                        <td>
                            <div class="form-floating">
                                <textarea
                                    class="form-control"
                                    id="floatingTextarea"
                                    v-model="staffReview.colleagueOpinion"
                                    :class="{ invalid: isColleagueOpinion }"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.colleagueOpinion !== ''"
                                    >Nhập nội dung</label
                                >
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #ffd966">Ý Kiến phòng HCNS<span style="color: red">*</span></td>
                        <td>
                            <div class="form-floating">
                                <textarea
                                    class="form-control"
                                    id="floatingTextarea"
                                    v-model="staffReview.HROpinion"
                                    :class="{ invalid: isHROpinion }"
                                ></textarea>
                                <label
                                    for="floatingTextarea"
                                    class="floatingTextarea"
                                    :hidden="staffReview.HROpinion !== ''"
                                    >Nhập nội dung</label
                                >
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #f4b084">
                            Kiến nghị / kết luận <span style="color: red">*</span>
                        </td>
                        <td>
                            <b>Ký hợp đồng:</b>
                            <div class="d-flex mt-2 mb-2">
                                <div v-for="(item, index) in checkbox" :key="item.id">
                                    <div class="justify-content-center align-items-center me-3">
                                        <input
                                            type="checkbox"
                                            v-bind:id="'assignContract' + index"
                                            v-bind:checked="selectedTimeContract.includes(item)"
                                            v-on:change="updateSelectedTimeContract(item)"
                                            name="assignContract"
                                            :value="item"
                                            :key="index"
                                            :disabled="staffReview.ReviewResult.isTerminate === true"
                                            class="form-check-input"
                                            :class="{ invalid: isAssignContractTime }"
                                        />
                                        <label :for="'assignContract' + index">&nbsp;{{ item.label }}</label>
                                    </div>
                                </div>
                            </div>
                            <div class="align-items-center">
                                <input
                                    type="checkbox"
                                    v-model="staffReview.ReviewResult.isTerminate"
                                    value="true"
                                    id="isTerminate"
                                    class="form-check-input"
                                    :disabled="selectedTimeContract.length > 0"
                                    :class="{ invalid: isAssignContractTime }"
                                />
                                <b for="isTerminate" class="me-1">&nbsp;Hủy Hợp Đồng</b>
                            </div>
                        </td>
                    </tr>
                </table>

                <template #footer>
                    <div class="d-flex justify-content-between">
                        <div>
                            <div>
                                <span class="d-flex justify-content-start"
                                    ><b><span style="color: red">*</span> Lưu ý:</b></span
                                >
                                <p>
                                    Các trường đánh dấu <span style="color: red"><b>*</b></span> không được để trống
                                    hoặc bằng 0
                                </p>
                            </div>
                        </div>
                        <div>
                            <Button
                                label="Lưu"
                                type="submit"
                                icon="pi pi-check"
                                @click="createStaffReview()"
                                autofocus
                            />
                            <Button
                                label="Hủy"
                                raised
                                icon="pi pi-times"
                                @click="closeAddOrEditPopup()"
                                class="p-button-secondary p-button-icon"
                            />
                        </div>
                    </div>
                </template>
            </Dialog>

            <Dialog
                v-model:visible="timesReviewDialog"
                modal
                header="Chi tiết số lần đánh giá của nhân viên "
                :style="{ width: '50vw' }"
            >
                <div class="mb-5">
                    <DataTable :value="staffReviewById" :loading="loading" showGridlines>
                        <template #empty> Không tìm thấy dữ liệu. </template>
                        <template #loading>
                            <ProgressSpinner />
                        </template>
                        <Column header="Số lần"
                            ><template #body="{ index }"> {{ ++index }} </template></Column
                        >
                        <Column header="Hiệu suất"
                            ><template #body="{ data }"> {{ data.totalPerformance }}% </template></Column
                        >
                        <Column header="Ngày đánh giá"
                            ><template #body="{ data }"> {{ this.formatDate(data.dateCreated) }} </template></Column
                        >
                        <Column header="Trạng thái">
                            <template #body="{ data }">
                                <div
                                    class="badge"
                                    :class="{
                                        'bg-success': data.isConfirm === true,
                                        'bg-danger': data.isConfirm === false,
                                        'bg-warning': data.isConfirm === null,
                                    }"
                                >
                                    {{
                                        data.isConfirm === true
                                            ? 'Đã xác nhận'
                                            : data.isConfirm === false
                                            ? 'Từ chối xác nhận'
                                            : 'Chưa xác nhận'
                                    }}
                                </div>
                            </template>
                        </Column>
                        <Column header="Tác vụ">
                            <template #body="{ data, index }">
                                <Button
                                    icon="pi pi-eye p-button-icon"
                                    class="me-2"
                                    aria-label="Submit"
                                    @click="openStaffReviewDetailsDialog(data, index)"
                                />
                                <Button
                                    icon="pi pi-file-excel"
                                    class="p-button-secondary me-2"
                                    @click="openReasonRejectDetailsPopup(data)"
                                    :hidden="data.isConfirm === true || data.isConfirm === null"
                                />
                                <Button
                                    icon="pi pi-pencil"
                                    class="p-button-warning me-2"
                                    @click="openEditReview(data)"
                                    :hidden="data.isConfirm !== null"
                                    v-if="this.showButton.update"
                                />
                                <Button
                                    icon="pi pi-trash"
                                    class="p-button p-component me-2 p-button-danger"
                                    @click="confirmDelete(data)"
                                    v-if="
                                        checkAdmin()
                                            ? this.showButton.delete
                                            : checkPMAndLead() && data.isConfirm === null
                                            ? this.showButton.delete
                                            : !this.showButton.delete
                                    "
                                />
                            </template>
                        </Column>
                    </DataTable>
                </div>
            </Dialog>

            <Dialog
                v-model:visible="reasonRejectDetails"
                modal
                header="Chi tiết lý do không duyệt"
                :style="{ width: '50vw' }"
            >
                <div class="container">
                    <div class="input-layout w-100">
                        <label class="mb-2">Lý do không duyệt <span style="color: red">*</span></label>
                        <Textarea
                            v-model="reasonDetails.reasonNotConfirm"
                            rows="5"
                            class="input form-control"
                            readonly
                        />
                    </div>
                </div>
                <template #footer>
                    <Button label="Hủy" icon="pi pi-times" @click="closeReasonRejectDetailsPopup()" text />
                </template>
            </Dialog>

            <reviewdetails
                :openDetailsDialog="this.openDetailsDialog"
                :ReviewTicket="this.ReviewTicket"
                :dataReviewDetailTicket="this.ReviewTicket"
                @closeDialog="closeDetailsDialog()"
                @closeTimesReviewDialog="closeTimesReviewDialog()"
                @getAllReviews="reloadData()"
                :usercode="this.usercode"
                :reviewTimes="this.reviewTimes"
                :currentUserId="currentUserId"
                :showbutton="this.showButton"
            />
            <editReview
                :openEditReviewDialog="this.openEditReviewDialog"
                @closeEditReview="closeEditReview()"
                @closeTimesReviewDialog="closeTimesReviewDialog()"
                @getAllReviews="reloadData()"
                :editObjectId="this.editReviewObjectId"
                :staffList="this.staffList"
                :pmLeadList="pmLeadList"
            />
            <Toast />
        </div>
    </LayoutDefaultDynamic>
</template>
<script>
import { HTTP, HTTP_LOCAL, GET_USER_NAME_BY_ID } from '@/http-common'
import { DateHelper } from '@/helper/date.helper'
import jwt_decode from 'jwt-decode'
import reviewdetails from './ReviewDetails.vue'
import editReview from './EditReview.vue'
import { FilterMatchMode } from 'primevue/api'
import { checkAccessModule } from '@/helper/checkAccessModule'
import router from '@/router'
import Delete from '@/components/buttons/Delete.vue'
import Edit from '@/components/buttons/Edit.vue'

export default {
    data() {
        return {
            reviewList: [],
            loading: false,
            addOrEditDialog: false,
            staffReview: {
                reviewerId: null,
                staffReview: null,
                totalPerformance: null,
                achievements: '',
                knowledge: '',
                skill: '',
                forwardMindset: '',
                positiveMindset: '',
                steadfastMindset: '',
                perfectionistMindset: '',
                fromTalkToResult: '',
                connectToAction: '',
                hobbies: '',
                personality: '',
                commitmentsForCompany: '',
                colleagueOpinion: '',
                HROpinion: '',
                dateCreated: new Date(),
                ReviewResult: {
                    isTerminate: false,
                    assignContract: null,
                },
            },
            experiences: [
                {
                    taskName: '',
                    spend: 0,
                    estimate: 0,
                    performance: 0,
                },
            ],
            now: this.formatDate(new Date()),
            pmlist: [],
            leadList: [],
            pmLeadList: [],
            staffList: [],
            staffReviewList: [],
            selectedStaffReview: null,
            selectedPmLead: null,
            selectedStaff: null,
            selectedTimeContract: [],
            checkbox: [
                { id: 1, label: '6 tháng', value: 1 },
                { id: 2, label: '12 tháng', value: 2 },
                { id: 3, label: '24 tháng', value: 3 },
                { id: 4, label: '36 tháng', value: 4 },
                { id: 5, label: 'Vô Thời Hạn', value: 5 },
            ],
            ReviewTicket: null,
            openDetailsDialog: false,
            timesReview: null,
            timesReviewDialog: false,
            staffReviewById: [],
            filters: {
                staffReviewName: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            currentUserId: null,
            reasonReject: '',
            reasonRejectDetails: false,
            reasonDetails: '',
            usercode: '',
            reviewTimes: null,
            isPmLeadSelected: false,
            isStaffReviewSelected: false,
            openEditReviewDialog: false,
            editReviewObjectId: null,
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
            filteredList: [],
            invalid: false,
            emptyList: [],
            isCommitmentsForCompany: false,
            isColleagueOpinion: false,
            isHROpinion: false,
            timesReviewByIdStaffIdReviewer: [],
            isAssignContractTime: false,
            idReviewTime: null,
            idBranch: null,
            idReviewer: null,
            resultPgae: {
                pageSize: 10,
                pageNumber: 1,
            },
            totalItem: 0,
            totalMapPage: 0,
            itemIndex: 0,
        }
    },
    components: { reviewdetails, editReview, Delete, Edit },

    async created() {
        if (checkAccessModule.checkAccessModule(this.$route.path.replace('/', '').slice(0, 7)) === true) {
            checkAccessModule.checkPermissionAction(this.$route.path.replace('/', '').slice(0, 7), this.showButton)
            await this.reloadData()
            //await this.getPM()
            //await this.getStaff()
            this.loading = false
        } else {
            alert('Bạn không có quyền')
            router.push('/')
            this.displayDialog1 = true
        }
        this.currentUserId = checkAccessModule.getUserIdCurrent()
    },

    computed: {
        CalculatePerformance() {
            return (item) => {
                if (item.spend === '' || item.spend === 0 || item.estimate === '' || item.estimate === 0) {
                    return 0
                } else {
                    this.updatePerformance()
                    return ((item.estimate / item.spend) * 100).toFixed(2)
                }
            }
        },
        averagePerformance() {
            return this.calculateTotalPerformance()
        },
        filteredList() {
            if (this.filteredList.length === 0 && this.selectedStaffReview !== null) {
                return this.emptyList
            } else if (this.filteredList.length === 0) {
                return this.reviewList
            } else {
                return this.filteredList
            }
        },
    },
    watch: {
        experiences: {
            deep: true,
            handler: function Changes(newValue) {
                this.updatePerformance()
            },
        },
        selectedStaffReview: {
            deep: true,
            handler(newVal) {
                this.searchByNameAndIdStaffReview()
            },
        },
        selectedPmLead: {
            deep: true,
            handler(newVal) {
                this.isPmLeadSelected = false
            },
        },
        selectedStaff: {
            deep: true,
            handler(newVal) {
                this.isStaffReviewSelected = false
            },
        },
        experiences: {
            deep: true,
            handler: function (newVal, oldVal) {
                for (let i = 0; i < newVal.length; i++) {
                    if (
                        newVal[i].taskName !== '' &&
                        newVal[i].spend !== 0 &&
                        newVal[i].spend !== '' &&
                        newVal[i].estimate !== '' &&
                        newVal[i].estimate !== 0
                    ) {
                        newVal[i].invalid = false
                    }
                }
            },
        },
        'staffReview.commitmentsForCompany': {
            deep: true,
            handler(newVal) {
                if (this.staffReview.commitmentsForCompany !== '' && this.staffReview.commitmentsForCompany !== null) {
                    this.isCommitmentsForCompany = false
                }
            },
        },
        'staffReview.colleagueOpinion': {
            deep: true,
            handler(newVal) {
                if (this.staffReview.colleagueOpinion !== '' && this.staffReview.colleagueOpinion !== null) {
                    this.isColleagueOpinion = false
                }
            },
        },
        'staffReview.HROpinion': {
            deep: true,
            handler(newVal) {
                if (this.staffReview.HROpinion !== '' && this.staffReview.HROpinion !== null) {
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
        resultPgae: {
            handler: async function change() {
                this.reloadData()
            },
            deep: true,
        },
    },
    methods: {
        formatDate(date) {
            return DateHelper.formatDate(date)
        },
        handleDelete(id) {
            this.$confirm.require({
                message: 'Bạn có muốn xóa đánh giá này ?',
                header: 'Xóa đánh giá',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Xóa',
                rejectLabel: 'Hủy',
                acceptIcon: 'pi pi-trash',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-danger CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: async () => {
                    await HTTP.put(`StaffReview/deleteReviewByUser/${id}`)
                        .then(async (res) => {
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: 'Xóa đánh giá thành công',
                                life: 3000,
                            })
                            await this.reloadData()
                        })
                        .catch((err) => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Hủy',
                                detail: 'Xóa đánh giá thất bại',
                                life: 3000,
                            })
                        })
                },
                reject: () => {
                    return
                },
            })
        },
        checkCanOperation(nameButton, data) {
            const name = nameButton.toLowerCase()
            if (name === 'them') {
            }
            if (name === 'sua') {
                if (data.reviewerId === Number(checkAccessModule.getUserIdCurrent()) && data.isConfirm === null) {
                    return false
                } else {
                    return true
                }
            }
            if (name === 'xoa') {
                if (checkAccessModule.isAdmin()) {
                    return false
                } else {
                    return true
                }
            }
            if (name === 'xoanhieu') {
            }
            if (name === 'xuatfile') {
            }
            if (name === 'xacnhan') {
            }
            if (name === 'xacnhannhieu') {
            }
            if (name === 'themthanhvien') {
            }
            if (name === 'tuchoi') {
                if (data.reviewerId === Number(checkAccessModule.getUserIdCurrent()) && data.isConfirm !== null) {
                    return false
                } else {
                    return true
                }
            }
        },
        async cancelConfirm(data) {
            this.$confirm.require({
                message: 'Bạn có muốn cập nhật đánh giá này?',
                header: 'Xác nhận',
                acceptLabel: 'Xác nhận',
                rejectLabel: 'Huỷ',
                icon: 'pi pi-exclamation-triangle',
                acceptIcon: 'pi pi-trash',
                rejectIcon: 'pi pi-times',
                acceptClass: 'p-button-danger CustomButtonPrimeVue',
                rejectClass: 'p-button-secondary p-button-outlined aloha CustomButtonPrimeVue',
                accept: async () => {
                    await HTTP.put(`StaffReview/ConfirmUpdateReview/${data.id}/${checkAccessModule.getUserIdCurrent()}`)
                        .then(async (res) => {
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Xác nhận',
                                detail: 'Xác nhận thành công!',
                                life: 3000,
                            })
                            await this.reloadData()
                        })
                        .catch((err) => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: 'Xác nhận cập nhật lỗi!',
                                life: 3000,
                            })
                        })
                },
                reject: () => {
                    return
                },
            })
        },

        async getAllReviews() {
            this.loading = true
            this.reviewList = []
            await HTTP.get(
                `StaffReview/GetAllStaffReviewByIdReviewTime/${this.idReviewTime}/${this.idBranch}/${this.idReviewer}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    this.reviewList = res.data._Data
                })
                .catch((err) => {
                    console.log(err)
                })
            await this.handleIdOfUser()
            this.loading = false
        },
        async getAllReviewsOffice() {
            this.loading = true
            await HTTP.get(
                `StaffReview/getAllByOffice/${this.idReviewTime}/${this.idBranch}/${this.idReviewer}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((ele) => {
                        this.reviewList.push(ele)
                    })
                    this.reviewList = this.reviewList.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                    if (this.reviewList.length <= 0) {
                        this.$toast.add({
                            severity: 'info',
                            summary: 'Thông Báo',
                            detail: 'Chưa có nhân viên nào được đánh giá!',
                            life: 3000,
                        })
                    }
                })
                .catch((err) => {
                    console.log(err)
                })
            await this.handleIdOfUser()
            this.loading = false
        },
        async getAllReviewsLead(id) {
            this.loading = true
            await HTTP.get(
                `StaffReview/getAllByLead/${this.idReviewTime}/${this.idBranch}/${this.idReviewer}?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
            )
                .then((res) => {
                    this.totalMapPage = res.data._totalPages
                    this.totalItem = res.data._totalItems
                    this.itemIndex = res.data._itemIndex
                    res.data._Data.map((ele) => {
                        this.reviewList.push(ele)
                    })
                    this.reviewList = this.reviewList.filter((obj, index, self) => {
                        return index === self.findIndex((t) => t.id === obj.id)
                    })
                })
                .catch((err) => {
                    console.log(err)
                })
            await this.handleIdOfUser()
            this.loading = false
        },
        async handleSearchUser() {
            this.staffReviewList = []
            if (this.filteredList.length !== 0) {
                for (let i = 0; i < this.filteredList.length; i++) {
                    if (this.filteredList[i].staffReview !== 0) {
                        var name = await this.getUserName(this.filteredList[i].staffReview)
                        var objectDrop = {
                            id: name.id,
                            fullName: name.fullName,
                        }
                        this.staffReviewList.push(objectDrop)
                    }
                }
            } else {
                for (let i = 0; i < this.reviewList.length; i++) {
                    if (this.reviewList[i].staffReview !== 0) {
                        var name = await this.getUserName(this.reviewList[i].staffReview)
                        var objectDrop = {
                            id: name.id,
                            fullName: name.fullName,
                        }
                        this.staffReviewList.push(objectDrop)
                    }
                }
            }
        },
        async handleIdOfUser() {
            if (this.filteredList.length !== 0) {
                for (let i = 0; i < this.filteredList.length; i++) {
                    if (this.filteredList[i].staffReview !== 0) {
                        var name = await this.getUserName(this.filteredList[i].staffReview)
                        this.filteredList[i].staffReviewName = name.fullName
                        this.filteredList[i].userCode = name.userCode
                    } else {
                        this.filteredList[i].staffReview = 'Không có dữ liệu...'
                    }
                }
            } else {
                for (let i = 0; i < this.reviewList.length; i++) {
                    if (this.reviewList[i].staffReview !== 0) {
                        var name = await this.getUserName(this.reviewList[i].staffReview)
                        this.reviewList[i].staffReviewName = name.fullName
                        this.reviewList[i].userCode = name.userCode
                    } else {
                        this.reviewList[i].staffReview = 'Không có dữ liệu...'
                    }
                }
            }
        },
        async getUserName(id) {
            return HTTP.get(GET_USER_NAME_BY_ID(id)).then((res) => res.data)
        },
        addRowToTable() {
            this.experiences.push({
                taskName: '',
                spend: 0,
                estimate: 0,
                performance: null,
            })
        },
        checkAdmin() {
            if (checkAccessModule.getListGroup().includes('1')) {
                return true
            } else {
                return false
            }
        },
        async reloadData() {
            this.loading = true
            this.idReviewTime = this.$route.params.idReviewTime
            this.idBranch = this.$route.params.idBranch
            this.idReviewer = this.$route.params.idReviewer
            this.reviewList = []
            if (checkAccessModule.checkCallAPI(this.$route.path.replace('/', '').slice(0, 20).toLowerCase())) {
                await this.getAllReviews()
            } else {
                if (checkAccessModule.isOffice()) {
                    await this.getAllReviewsOffice()
                }
                if (checkAccessModule.isLead()) {
                    await this.getAllReviewsLead(checkAccessModule.getUserIdCurrent())
                }
            }
            await this.handleSearchUser()
            this.loading = false
        },
        removeRowFromTable(index) {
            this.experiences.splice(index, 1)
        },
        updatePerformance() {
            this.experiences.forEach((item) => {
                if (item.spend !== '' && item.spend !== 0 && item.estimate !== '' && item.estimate !== 0) {
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
                this.staffReview.totalPerformance = total / count
                if (this.staffReview.totalPerformance === 0) return 0
                return (total / count).toFixed(2)
            }
        },

        async createStaffReview() {
            if (this.selectedPmLead === null) {
                this.isPmLeadSelected = true
                this.$nextTick(() => this.$refs.pmLeadDropdown.$el.querySelector('.p-dropdown-label').focus())
                return
            }
            if (this.selectedStaff === null) {
                this.isStaffReviewSelected = true
                this.$nextTick(() => this.$refs.staffDropdown.$el.querySelector('.p-dropdown-label').focus())
                return
            }

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
            if (hasInvalidExperience) {
                return
            }
            if (this.staffReview.commitmentsForCompany === '' || this.staffReview.commitmentsForCompany === null) {
                this.isCommitmentsForCompany = true
                return
            }
            if (this.staffReview.colleagueOpinion === '' || this.staffReview.colleagueOpinion === null) {
                this.isColleagueOpinion = true
                return
            }
            if (this.staffReview.HROpinion === '' || this.staffReview.HROpinion === null) {
                this.isHROpinion = true
                return
            }
            if (this.selectedTimeContract.length === 0 || this.staffReview.ReviewResult.isTerminate === null) {
                this.isAssignContractTime = true
            }
            var data = {
                reviewerId: this.selectedPmLead.id,
                staffReview: this.selectedStaff.id,
                totalPerformance: this.staffReview.totalPerformance,
                achievements: this.staffReview.achievements,
                knowledge: this.staffReview.knowledge,
                skill: this.staffReview.skill,
                forwardMindset: this.staffReview.forwardMindset,
                positiveMindset: this.staffReview.positiveMindset,
                steadfastMindset: this.staffReview.steadfastMindset,
                perfectionistMindset: this.staffReview.perfectionistMindset,
                fromTalkToResult: this.staffReview.fromTalkToResult,
                connectToAction: this.staffReview.connectToAction,
                hobbies: this.staffReview.hobbies,
                personality: this.staffReview.personality,
                commitmentsForCompany: this.staffReview.commitmentsForCompany,
                colleagueOpinion: this.staffReview.colleagueOpinion,
                HROpinion: this.staffReview.HROpinion,
                experiences: this.experiences,
                dateCreated: this.formatDate(this.staffReview.dateCreated),
                userCreated: Number(checkAccessModule.getUserIdCurrent()),
                ReviewResult: {
                    isTerminate: this.staffReview.ReviewResult.isTerminate,
                    assignContract: await this.selectedTimeContract.map((el) => {
                        return el.value
                    })[0],
                },
            }

            this.staffReview = { ...data }
            HTTP.post('StaffReview', this.staffReview)
                .then((res) => {
                    this.closeAddOrEditPopup()
                    this.toastSuccess()
                    this.reloadData()
                })
                .catch((err) => {
                    console.log(err)
                    this.toastError()
                })
        },
        getStaff() {
            HTTP.get('Users/GetUserInRoleStaff')
                .then((res) => {
                    this.staffList = res.data
                })
                .catch((err) => {
                    console.log(err)
                })
        },
        getPM() {
            HTTP.get('Users/GetUserInRolePM')
                .then((res) => {
                    this.pmlist = res.data
                    this.getLead()
                })
                .catch((err) => {
                    console.log(err)
                })
        },
        getLead() {
            HTTP.get('Users/GetUserInRoleLead')
                .then((res) => {
                    this.leadList = res.data
                    this.listPMLead()
                })
                .catch((err) => {
                    console.log(err)
                })
        },
        async listPMLead() {
            var data = await [...this.pmlist, ...this.leadList]
            this.pmLeadList = data
        },
        updateSelectedTimeContract(item) {
            if (this.selectedTimeContract.includes(item)) {
                this.selectedTimeContract = []
            } else {
                this.selectedTimeContract = [item]
            }
        },
        closeAddOrEditPopup() {
            this.addOrEditDialog = false
            this.clearForm()
        },
        openStaffReviewDetailsDialog(data, index) {
            console.log(data)
            this.ReviewTicket = data
            this.reviewTimes = ++index
            this.openDetailsDialog = true
        },
        closeDetailsDialog() {
            this.ReviewTicket = []
            this.openDetailsDialog = false
        },

        openTimesReviewDialog(data) {
            this.timesReviewDialog = true
            this.usercode = data.userCode
            if (
                checkAccessModule.getListGroup().includes('1') ||
                checkAccessModule.getListGroup().includes('5') ||
                checkAccessModule.getListGroup().includes('3')
            ) {
                this.staffReviewById = data
            } else {
                this.getReviewByStaffId(data.staffReview)
            }
        },
        closeTimesReviewDialog() {
            this.timesReviewDialog = false
        },
        getReviewById(id) {
            this.loading = true
            HTTP.get('StaffReview/GetReviewById/' + id)
                .then((res) => {
                    this.staffReviewById = res.data._Data
                })
                .catch((err) => {
                    console.log(err)
                })
            this.loading = false
        },
        getReviewByStaffId(id) {
            this.loading = true
            HTTP.get(`StaffReview/GetReviewByIdStaff/${id}/${this.idReviewer}`)
                .then((res) => {
                    this.staffReviewById = res.data._Data
                })
                .catch((err) => {
                    console.log(err)
                })
            this.loading = false
        },
        toastSuccess() {
            this.$toast.add({
                severity: 'success',
                summary: 'Thành công',
                detail: 'Tạo mới thẻ đánh giá thành công',
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
        clearForm() {
            this.selectedStaff = null
            this.selectedPmLead = null
            this.isPmLeadSelected = false
            this.isStaffReviewSelected = false
            this.invalid = false
            this.staffReview = {
                reviewerId: null,
                staffReview: null,
                totalPerformance: null,
                achievements: '',
                knowledge: '',
                skill: '',
                forwardMindset: '',
                positiveMindset: '',
                steadfastMindset: '',
                perfectionistMindset: '',
                fromTalkToResult: '',
                connectToAction: '',
                hobbies: '',
                personality: '',
                commitmentsForCompany: '',
                colleagueOpinion: '',
                HROpinion: '',
                dateCreated: new Date(),
            }
            this.staffReview.ReviewResult = {
                assignContract: null,
                isTerminate: false,
            }
            this.selectedTimeContract = []
            this.experiences = [
                {
                    taskName: '',
                    spend: 0,
                    estimate: 0,
                    performance: 0,
                },
            ]
        },
        openReasonRejectDetailsPopup(data) {
            this.reasonDetails = { ...data }
            this.reasonRejectDetails = true
        },
        closeReasonRejectDetailsPopup() {
            this.reasonRejectDetails = false
        },
        openEditReview(data) {
            this.editReviewObjectId = data.id
            this.openEditReviewDialog = true
        },
        closeEditReview() {
            this.openEditReviewDialog = false
        },
        async searchByNameAndIdStaffReview() {
            this.loading = true
            if (this.selectedStaffReview !== null) {
                const data = {
                    id: this.selectedStaffReview,
                    staffReviewTime: this.idReviewTime,
                    companyBranhId: this.idBranch,
                    // name: this.keyword,
                    idUserCurrent: checkAccessModule.getUserIdCurrent(),
                }
                await HTTP.post(
                    `StaffReview/SearchReviewByStaffNameOrReviewerId?pageIndex=${this.resultPgae.pageNumber}&pageSizeEnum=${this.resultPgae.pageSize}`,
                    data,
                )
                    .then((res) => {
                        this.filteredList = res.data._Data
                        this.totalMapPage = res.data._totalPages
                        this.totalItem = res.data._totalItems
                        this.itemIndex = res.data._itemIndex
                        if (this.filteredList.length === 0) {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Không tìm thấy',
                                detail: 'Không tìm thấy phiếu đánh giá phù hợp',
                                life: 3000,
                            })
                        }
                    })
                    .catch((err) => {
                        console.log(err)
                    })
                    .finally(() => {
                        this.loading = false
                    })
                await this.handleIdOfUser()
            }
        },

        handlerReload() {
            this.selectedStaffReview = null
            this.filteredList = []
            this.reloadData()
        },

        confirmDelete(data) {
            this.$confirm.require({
                message: 'Bạn có muốn xóa lần đánh giá này ?',
                header: 'Xóa đánh giá',
                icon: 'pi pi-exclamation-triangle',
                acceptLabel: 'Xóa',
                rejectLabel: 'Hủy',
                acceptClass: 'p-button-danger',
                accept: async () => {
                    await HTTP.put(`StaffReview/deleteReview/${data.id}`).then(async (res) => {
                        if (res.status == 200) {
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: 'Xóa thành công!',
                                life: 3000,
                            })
                            await this.reloadData()
                            this.timesReviewDialog = false
                        } else {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Thất bại',
                                detail: 'Xóa thất bại!',
                                life: 3000,
                            })
                        }
                    })
                },
                reject: () => {
                    return
                },
            })
        },
        checkPMAndLead() {
            if (checkAccessModule.getListGroup().includes('5') || checkAccessModule.getListGroup().includes('3')) {
                return true
            } else {
                return false
            }
        },
    },
}
</script>
<style lang="scss" scoped>
.p-card-header {
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
}
</style>
<style scoped>
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
</style>
<style  lang="scss" scoped>
.p-dialog .p-dialog-footer {
    padding: 1.2rem 0.5rem 0.7rem 1.5rem !important;
}
</style>
