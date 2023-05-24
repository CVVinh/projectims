<template>
    <div class="mx-3 mt-3 mb-3">
        <div class="row" style="margin: 0px 15px">
            <div class="col-md-4 d-flex align-items-center justify-content-start" id="page-nav">
                <div>
                    <div class="pagination pagination1 pagination3 pagination4 pagination6">
                        <a @click="ChangePage('Prev')">&laquo;</a>
                        <a
                            :class="{ active: item == pageNumber }"
                            v-for="(item, index) in page"
                            :key="index"
                            @click="ChangePage(item)"
                            >{{ item }}</a
                        >
                        <a class="page-link" @click="ChangePage('Next')">&raquo;</a>
                    </div>
                </div>
            </div>
            <div class="col-md-7 d-flex align-items-center justify-content-end">
                <div class="row">
                    <div class="col-md-10 d-flex align-items-center" id="page-nav-info">
                        <span style="color: #6c757d">
                            Tổng số trang: {{ totalPages }} | Trang hiện tại: {{ totalItems > 0 ? pageNumber : 0 }} |
                            Hiển thị từ {{ itemIndex }} đến {{ totalItems > 0 ? itemIndex + pageSize - 1 : 0 }} trong
                            tổng {{ totalItems }} dữ liệu
                        </span>
                    </div>
                    <div class="col-md-2" id="page-nav-control">
                        <div class="ms-3">
                            <Dropdown
                                v-model="ChangePageSize"
                                :options="selectPageList"
                                optionLabel="key"
                                optionValue="default"
                                placeholder="Số dòng"
                                :disabled="totalItems == 0"
                            />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        props: {
            pageSize: {
                type: Number,
                default: 5,
            },
            pageNumber: {
                type: Number,
                default: 1,
            },
            totalPages: {
                type: Number,
                default: 0,
            },
            totalRow: {
                type: Number,
                default: 0,
            },
            totalItems: {
                type: Number,
                default: 0,
            },
            itemIndex: {
                type: Number,
                default: 0,
            },
        },
        data() {
            return {
                selectPageList: [
                    {
                        key: '5',
                        default: 5,
                    },
                    {
                        key: '10',
                        default: 10,
                    },
                    {
                        key: '20 ',
                        default: 20,
                    },
                    {
                        key: '50',
                        default: 50,
                    },
                    {
                        key: '100',
                        default: 100,
                    },
                    {
                        key: '200',
                        default: 200,
                    },
                    {
                        key: '250',
                        default: 250,
                    },
                ],
            }
        },
        computed: {
            ChangePageSize: {
                get() {
                    return this.pageSize
                },
                set(value) {
                    this.$emit('update:pageSize', value)
                },
            },
            totalPgae() {
                return this.totalPages
            },
            page() {
                let page = []
                for (let i = 1; i <= this.totalPgae; i++) {
                    if (i == 1 || i == this.totalPage || (i <= this.pageNumber + 2 && i >= this.pageNumber - 2)) {
                        page.push(i)
                    } else if (i == this.pageNumber + 3 || i == this.pageNumber - 3) {
                        page.push('...')
                    }
                }
                return page
            },
        },
        methods: {
            ChangePage(page) {
                if (page == 'Prev' && this.pageNumber <= 1) {
                    return
                }
                if (page == 'Next' && this.pageNumber >= this.totalPgae) {
                    return
                }
                if (page == 'Next') {
                    page = this.pageNumber + 1
                }
                if (page == 'Prev') {
                    page = this.pageNumber - 1
                }
                if (page === '...') {
                    return
                }
                this.$emit('update:pageNumber', page)
            },
        },
    }
</script>
<style>
    .pagination {
        display: inline-block;
        cursor: pointer;
    }
    .pagination a {
        text-decoration: none;
        color: #000;
        float: left;
        cursor: pointer;
        padding: 8px 16px;
    }
    .pagination1 a.active {
        background-color: #3b82f6;
        color: #fff;
    }
    .pagination1 a:hover:not(.active) {
        background-color: #ddd;
    }
    .pagination2 a.active {
        background-color: #3b82f6;
        color: #fff;
        border-radius: 5px;
    }
    .pagination2 a:hover:not(.active) {
        background-color: #ddd;
        border-radius: 5px;
    }
    .pagination3 a {
        transition: background-color 0.3s;
    }
    .pagination4 a:not(.active) {
        border: 1px solid #ddd;
    }
    .pagination4 a.active {
        border: 1px solid #3b82f6;
    }
    .pagination5 a:first-child {
        border-top-left-radius: 5px;
        border-bottom-left-radius: 5px;
    }
    .pagination5 a:last-child {
        border-top-right-radius: 5px;
        border-bottom-right-radius: 5px;
    }
    .pagination6 a {
        margin: 0 4px;
    }
    .pagination7 a {
        font-size: 22px;
    }
    .center {
        text-align: center;
    }

    .margin-bottom-page {
        margin-bottom: 2px;
    }

    @media (max-width: 573px) {
        .col-md-7 .row .col-md-10 span {
            font-size: 12px;
        }
        .col-md-7 .row .col-md-10 {
            margin-bottom: 10px;
        }
        #page-nav {
            display: flex !important;
            align-content: center !important;
            justify-content: center !important;
            font-size: 12px;
        }
        .page-link {
            font-size: 12px;
        }
        #page-nav-control {
            display: flex;
            justify-content: center;
        }
        #page-nav-info {
            text-align: center;
        }
        .ms-3 {
            margin-left: 0px !important;
        }
    }
</style>
