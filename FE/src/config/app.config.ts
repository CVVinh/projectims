import { duration } from 'moment'

export const HttpStatus = {
    OK: 200,
    CREATED: 201,
    BAD_REQUEST: 400,
    UNAUTHORIZED: 401,
    FORBIDDEN: 403,
    NOT_FOUND: 404,
    INTERNAL_SERVER_ERROR: 500,
    NOT_IMPLEMENTED: 501,
    BAD_GATEWAY: 502,
    SERVICE_UNAVAILABLE: 503,
    GATEWAY_TIMEOUT: 504,
    HTTP_VERSION_NOT_SUPPORTED: 505,
}

export const ApiApplication = {
    PROJECT: {
        GET_ALL: 'Project/getAllProject',
        UPDATED: 'Project/updateProject',
        CREATE: 'Project/addProject',
        DELETE: 'Project/DeleteProject',
        GET_PROJECT_BY_ID: (id: number) => `Project/getById/${id}`,
        GET_ALL_LABELS_OF_PROJECT: (idproject: number, pageSize: number, page: number, APIKey: any) =>
            `projects/${idproject}/labels?per_page=${pageSize}&page=${page}${APIKey}`,
        SEARCH_PROJECT_BY_NAME: (name: string, pageIndex: Number, pageSize: Number) =>
            `Project/FillterProjectByName?name=${name}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        SEARCH_PROJECT_BY_NAME_LEAD: (idLead: Number, name: string, pageIndex: Number, pageSize: Number) =>
            `Project/FillterProjectByNameOfLead/${idLead}?name=${name}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        SEARCH_PROJECT_BY_NAME_STAFF: (idStaff: Number, name: string, pageIndex: Number, pageSize: Number) =>
            `Project/FillterProjectByNameOfStaff/${idStaff}?name=${name}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
    },

    USER: {
        GET_ALL: 'Users/getAll',
        GET_BY_CODE: 'Users/getUserByUserCode/',
        GET_INFO: 'Users/getInfo',
        UPDATE_PROFILE: (id: number) => `Users/updateProfileUser/${id}`,
        GET_USER_BY_ID: (id: number) => `Users/getUserById/${id}`,
        GET_ALL_USER_BY_IDPROJECT: (id: number) => `Users/getAllUsersByIdProject/${id}`,
        SEARCH_USER_BY_NAME: (name: string, pageIndex: Number, pageSize: Number) =>
            `Users/SearchUserByNameOrUserCode?name=${name}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        SEARCH_MEMBER_BY_NAME: (idProject: Number, name: string, pageIndex: Number, pageSize: Number) =>
            `Users/SearchMemberByName/${idProject}?name=${name}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        GET_ALL_INFO_USER_GITLAB: (pageIndex: Number, pageSize: Number, APIKey: any) =>
        `users?per_page=${pageSize}&page=${pageIndex}${APIKey}`, 
    },
    PERMISSION_GROUP_MENU: {
        GET_PERMISSION_GROUP: 'Permission_Groups/decentralization_Group', // cu
        GET_BY_USER_GROUP: 'Permission_Groups/getPermissionGroup_By_IdModule/', // cu
    },
    PERMISSION_USER_MENU: {
        GET_ALL: 'Group/getListGroup',
        GET_USER_BY_GROUP: 'Group/getUserByGroup',
        GET_USER_MENU: 'Permission_Use_Menus/getPermissionUserMenuWithUserId/',
        ADD_BY_USER: 'Permission_Use_Menus/addPermissionByUser',
        ADD_BY_GROUP: 'Permission_Use_Menus/addPermissionByGroup',
    },
    MODULE: {
        GET_ALL: 'Modules/getListModule',
        SEARCH_MODULES_BY_NAME: (name: string, pageIndex: Number, pageSize: Number) =>
            `Modules/SearchModuleByName?name=${name}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
    },
    RULEFOR: {
        GET_ALL: 'ReluFor/getListRelue',
        CREATE: 'ReluFor',
        UPDATE: (id: number) => `ReluFor/${id}`,
        DELETE: (id: number) => `ReluFor/${id}`,
    },
    EQUIPMENT_DEVICE: {
        GET_ALL_INFO_DEVICE: 'InfoDevices/GetAllDeviceListWithApplication',
        SEARCH_DEVICE_BY_NAME: (name: string) => `InfoDevices/FindWithUserName/${name}`,
        SEARCH_DEVICE_BY_OPERATINGSYSTEM: (operatingSystem: any) =>
            `InfoDevices/FindWithOperatingSystem/${operatingSystem}`,
        GET_ALL_INFO_DEVICE_WITH_PAGE: (pageIndex: Number, pageSize: Number) =>
            `InfoDevices/GetAllDeviceListWithApplication?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        SEARCH_DEVICE_BY_NAME_WITH_PAGE: (pageIndex: Number, pageSize: Number, name: string) =>
            `InfoDevices/FindWithUserName/${name}?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
    },
    NOTIFICATION: {
        GET_ALL_NOTIFICATION: 'Notification/GetAllListNotification',
        GET_ALL_NOTIFICATION_BYLEAD: 'Notification/GetAllListNotification',
        IS_WATCH_NOTIFICATION: (id: number) => `Notification/WatchNotification/${id}`,
        REQUIRE_DELETE_APPLICATION: 'Notification/RequireDeleteApplication',
        GET_ALL_NOTIFICATION_Of_PM: (idPM: Number) => `Notification/GetNotificationOfPm?idPm=${idPM}`,
        GET_ALL_NOTIFICATION_Of_LEAD: (idLead: Number) => `Notification/GetNotificationOfLead?idLead=${idLead}`,
        GET_ALL_NOTIFICATION_TIMESHETDAILYS: () => `Notification/GetNotificationTimeSheetDailys`,
        CREATE_NOTIFICATION_TIMESHETDAILYS: () => `Notification/CreateNotificationTimeSheet`,
    },
    BLOCKINGWEB: {
        ADD_BLOCKING_WEB: 'BlockingWeb/AddBlockingWeb',
    },
    TASK: {
        ADD_TASK_ON_DB: 'tasks/addNewTask',
        ADD_TASK_ON_GITLAB: (idProject: Number, APIKey: any) => `projects/${idProject}/issues?private_token=${APIKey}`,
        GET_TOTAL_PAGE_GITLAB: (idProject: Number, perPage: Number, page: Number, APIKey: any) =>
            `projects/${idProject}/issues?per_page=${perPage}&page=${page}&sort=desc&private_token=${APIKey}`,
        GET_ALL_TASK_BY_IDPROJECT: (idProejct: Number) => `tasks/getAllTaskByIdProject/${idProejct}`,
        DELETE_TASK_ON_GITLAB: (idProject: Number, issue_iid: Number, APIKey: any) =>
            `/projects/${idProject}/issues/${issue_iid}${APIKey}`,
        DELETE_TASK_ON_API: (idTask: Number) => `tasks/deletedTask/${idTask}`,
        UPDATE_TASK_ON_GITLAB: (idProject: Number, issue_iid: Number, APIKey: any) =>
            `/projects/${idProject}/issues/${issue_iid}?private_token=${APIKey}`,
        UPDATE_TASK_ON_API: (idTask: Number) => `tasks/editTaskById/${idTask}`,
        MUTILATE_ADD_TASK_ON_API: (idProject: Number) => `tasks/mutilateAddTask/${idProject}`,
        GET_LABLE_BY_NAME: (idProject: Number) => `projects/${idProject}/labels`,
        UPDATE_TASK_API_GITLAB_TIME_ESTIMATE: (idProject: Number, iid_issue: Number, duration: string, APIKey: any) =>
            `projects/${idProject}/issues/${iid_issue}/time_estimate?duration=${duration}&private_token=${APIKey}`,
        GET_ALL_TASK_IN_GITLAB: (idProject: Number, pageSize: Number, page: Number, APIKey: any) =>
            `projects/${idProject}/issues?per_page=${pageSize}&page=${page}&private_token=${APIKey}`,
        GET_ALL_MEMBER_PROJECT: (idProject: Number, APIKey: any) => `projects/${idProject}/members/all${APIKey}`,
        GET_ALL_TASK_WITH_PAGE: (idProject: Number, pageIndex: Number, pageSize: Number) =>
            `tasks/getAllTaskByIdProject/${idProject}?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        GET_ALL_TASK_WITH_PAGE_BYIDSTAFF: (
            idProject: Number,
            idstaff: Number,
            pageIndex: Number,
            pageSizeEnum: Number,
        ) => `Tasks/getAllTasksByIdStaff/${idstaff}/${idProject}/${pageIndex}/${pageSizeEnum}`,
        SEARCH_TASKS_BY_NAME_API: (ipProject: Number, name: string, pageIndex: Number, pageSize: Number) =>
            `tasks/SearchTasksByName/${ipProject}?name=${name}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        SEARCH_TASKS_OF_STAFF_BY_NAME_API: (
            ipProject: Number,
            idStaff: Number,
            name: string,
            pageIndex: Number,
            pageSize: Number,
        ) =>
            `tasks/SearchTasksOfStaff/${ipProject}/${idStaff}?name=${name}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        SEARCH_TASKS_BY_NAME_ON_GITLAB: (
            idProject: Number,
            name: string,
            pageIndex: Number,
            pageSize: Number,
            APIKey: any,
        ) => `projects/${idProject}/issues?search=${name}&scope=all&per_page=${pageSize}&page=${pageIndex}&private_token=${APIKey}`,
        SEARCH_TASK_BY_NAME_OF_STAFF_ON_GITLAB: (
            idProject: Number,
            name: string,
            assignee_username: string,
            pageIndex: Number,
            pageSize: Number,
            APIKey: any,
        ) =>
            `projects/${idProject}/issues?search=${name}&assignee_username=${assignee_username}&per_page=${pageSize}&page=${pageIndex}&scope=all&state=opened${APIKey}`,
        ADD_TIME_SPENT_TO_WORK: (idProject: Number, iid_issue: Number, duration: string, APIKey: any,) =>
        `projects/${idProject}/issues/${iid_issue}/add_spent_time?duration=${duration}&private_token=${APIKey}`,  
        GET_ALL_COMMENT_TO_WORK: (idProject: Number, iid_issue: Number, APIKey: any,) =>
        `projects/${idProject}/issues/${iid_issue}/notes?private_token=${APIKey}`, 
        ADD_COMMENT_TO_WORK: (idProject: Number, iid_issue: Number, APIKey: any,) =>
        `projects/${idProject}/issues/${iid_issue}/notes?private_token=${APIKey}`, 
        UPDATE_COMMENT_TO_WORK: (idProject: Number, iid_issue: Number, idComment: Number, APIKey: any,) =>
        `projects/${idProject}/issues/${iid_issue}/notes/${idComment}?private_token=${APIKey}`, 
        DELETE_COMMENT_TO_WORK: (idProject: Number, iid_issue: Number, idComment: Number, APIKey: any,) =>
        `projects/${idProject}/issues/${iid_issue}/notes/${idComment}?private_token=${APIKey}`, 
        CREATE_USER_TOKEN_PRIVATE: (idUser: Number, APIKey: any,) =>
        `users/${idUser}/personal_access_tokens${APIKey}`, 
        GET_ALL_TASK_BY_USER: (idProject: Number, idUser: Number, pageSize: Number, page: Number, APIKey: any,) =>
        `projects/${idProject}/issues?assignee_id=${idUser}&per_page=${pageSize}&page=${page}${APIKey}`,
        SEARCH_TASKS_BY_NAME_ON_GITLAB_BY_USER: (
            idProject: Number,
            idUser: Number,
            name: string,
            pageIndex: Number,
            pageSize: Number,
            APIKey: any,
        ) => `projects/${idProject}/issues?assignee_id=${idUser}&search=${name}&scope=all&per_page=${pageSize}&page=${pageIndex}&private_token=${APIKey}`,
        GET_ALL_TASK_IN_GITLAB_BY_USER_AUTHOR: (idProject: Number, idAuthor: Number, pageSize: Number, page: Number, APIKey: any) =>
            `projects/${idProject}/issues?author_id=${idAuthor}&per_page=${pageSize}&page=${page}&private_token=${APIKey}`,
        ADD_MEMBER_PROJECT: (idProject: Number, APIKey: any) => `projects/${idProject}/members${APIKey}`,
        DELETE_MEMBER_PROJECT: (idProject: Number,idUser: Number, APIKey: any) => `projects/${idProject}/members/${idUser}${APIKey}`,
        SEARCH_USER_IN_PROJECT_ON_GITLAB: (idProject: Number, APIKey: any) => `projects/${idProject}/users${APIKey}`,
    },
    LEVAEOFF: {
        GET_ALL_LEAVEOFF_BY_ID_USER: (idUser: Number, pageIndex: Number, pageSize: Number) =>
            `leaveOff/getAllLeaveOffOfidUserByStatus/${idUser}?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        GET_ALL_LEAVEOFF: (pageIndex: Number, pageSize: Number) =>
            `leaveOff/getAllLeaveOff?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        GET_ALL_LEAVEOFF_BYLEAD: (pageIndex: Number, pageSize: Number, IdUser: Number) =>
            `leaveOff/getAllLeaveOffByLead/${IdUser}/?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        GET_ALL_LEAVEOFF_BYPM: (pageIndex: Number, pageSize: Number, IdUser: Number) =>
            `leaveOff/getAllLeaveOffByPM/${IdUser}/?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        FILTER_LEAVEOFF_BY_DATE_NAME_STATUS: (pageIndex: Number, pageSize: Number) =>
            `leaveOff/GetByNameStatusDate?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        GET_ALL_LEAVEOFF_INFO: (pageIndex: Number, pageSize: Number) =>
            `leaveOff/getAllLeaveOffInfo?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        SEARCH_LEAVEOFF_BY_YEAR: (pageIndex: Number, pageSize: Number) =>
            `leaveOff/GetAllLeaveOffByYear?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        FILTER_LEAVEOFF_BY_DATE_NAME_STATUS_LEAD: (idLead: Number, pageIndex: Number, pageSize: Number) =>
            `leaveOff/GetByNameStatusDateLead/${idLead}?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        FILTER_LEAVEOFF_BY_DATE_NAME_STATUS_PM: (idPm: Number, pageIndex: Number, pageSize: Number) =>
            `leaveOff/GetByNameStatusDatePM/${idPm}?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        GET_USER_INFO_LEAVE_OFF: (idUser: Number, sortMonth: Date) =>
            `leaveOff/GetInfoUserLeaveOff/${idUser}?date=${sortMonth}`,
    },
    GROUP: {
        SEARCH_GROUP_BY_NAME: (name: string, pageIndex: Number, pageSize: Number) =>
            `Group/SearchGroupByName?name=${name}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
    },
    MENU: {
        SEARCH_MENU_BY_NAME_OR_ACTION: (name: string, idModule: Number, pageIndex: Number, pageSize: Number) =>
            `Menu/SearchMenuByNameOrAction?name=${name}&idModule=${idModule}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
    },
    ACTIONMODULES: {
        SEARCH_ACTIONMODULES_BY_NAME: (name: string, pageIndex: Number, pageSize: Number) =>
            `ActionModule/SearchActionModulesByName?name=${name}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
    },
    BLOG: {
        GET_ALL_BLOG: (pageIndex: Number, pageSize: Number) =>
            `Posts/getAllPostAsync?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        GET_BLOG_BY_ID: (id: Number) => `Posts/search?postId=${id}`,
        SEARCH_BLOG: (
            postId: Number,
            title: string,
            user: string,
            category: string,
            pageIndex: Number,
            pageSize: Number,
        ) =>
            `Posts/search?postId=${postId}&title=${title}&user=${user}&category=${category}&pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`,
        REPLY_COMMENT_OF_POST: `PostComment/ReplyCommentOfPost`,
    },
}
