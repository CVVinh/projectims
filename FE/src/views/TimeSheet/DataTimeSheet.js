export const DataTimeSheet = {
    getData() {
        return [
            {
                date: '20/04/2022',
                taskContent: 'task 1',
                layout: null,
                spec: null,
                api: 4,
                web: 4,
                utc: null,
                ute: null,
                integration: null,
                deployment: null,
                fixbug: null,
                support: null,
                others: null,
                sum: 8,
            },
            {
                date: '20/04/2022',
                taskContent: 'task 2',
                layout: null,
                spec: null,
                api: 4,
                web: 4,
                utc: null,
                ute: null,
                integration: null,
                deployment: null,
                fixbug: null,
                support: null,
                others: null,
                sum: 8,
            },
            {
                date: '21/04/2022',
                taskContent: 'task 1',
                layout: null,
                spec: null,
                api: 2,
                web: 6,
                utc: null,
                ute: null,
                integration: null,
                deployment: null,
                fixbug: null,
                support: null,
                others: null,
                sum: 8,
            },
            {
                date: '21/04/2022',
                taskContent: 'task 2',
                layout: null,
                spec: null,
                api: 6,
                web: 2,
                utc: null,
                ute: null,
                integration: null,
                deployment: null,
                fixbug: null,
                support: null,
                others: null,
                sum: 8,
            },
            {
                date: '21/04/2022',
                taskContent: 'task 3',
                layout: null,
                spec: null,
                api: 3,
                web: 2,
                utc: null,
                ute: null,
                integration: null,
                deployment: null,
                fixbug: null,
                support: null,
                others: 3,
                sum: 8,
            },
        ]
    },

    getDataSmall() {
        return Promise.resolve(this.getData().slice(0, 10))
    },

    getDataMedium() {
        return Promise.resolve(this.getData().slice(0, 50))
    },

    getDataLarge() {
        return Promise.resolve(this.getData().slice(0, 200))
    },

    getDataXLarge() {
        return Promise.resolve(this.getData())
    },
}

export const DataTimeSheetTotal = {
    getData() {
        return {
            layoutTotal: 16,
            specTotal: 16,
            apiTotal: 16,
            webTotal: 16,
            utcTotal: 16,
            uteTotal: 16,
            integrationTotal: 16,
            deploymentTotal: 16,
            fixbugTotal: 16,
            supportTotal: 16,
            othersTotal: 16,
            sumTotal: 16,
        }
    },

    getDataSmall() {
        return Promise.resolve(this.getData())
    },
}
