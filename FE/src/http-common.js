import axios from 'axios'
export const LinkDownloadFile = 'http://localhost:5001/api/'
export const HTTP = axios.create({
    baseURL: 'http://localhost:5001/api/',
    headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
    },
})

export const reLoadHTTP = () => {
    HTTP.defaults.headers = {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
    }
}
export const KEY = 'scLbs6vA88DVDuzeZeDm'
export const API_KEY = '?private_token=scLbs6vA88DVDuzeZeDm'
export const API_KEY_1 = '&private_token=scLbs6vA88DVDuzeZeDm'
export const PRIVATE_TOKEN = 'private_token=scLbs6vA88DVDuzeZeDm'

export const SEARCH = (search) => `search?scope=projects&search=${search}&` + PRIVATE_TOKEN
export const GET_ALL_ISSUE = (id, pageSize, page) =>
    `projects/${id}/issues?per_page=${pageSize}&page=${page}` + API_KEY_1

export const PROJECT_MEMBER = (id) => `projects/${id}/members/all` + API_KEY
export const STAFF_IS_IN_PROJECT_CHECK = (idProject, idStaff) => `projects/${idProject}/members/${idStaff}` + API_KEY
export const KEY_MAP = (id) => `projects/${id}/issues` + API_KEY
export const KEY_PROJECT = (id) => `projects/${id}` + API_KEY

export const GET_ALL_PROJECT = (pageSize, page) => `projects?per_page=${pageSize}&page=${page}` + API_KEY_1

export const SORT_BY_MONTH = (value, pageSize, page) =>
    `projects?orderby=id&last_activity_after=${value}&per_page=${pageSize}&page=${page}${API_KEY_1}`

export const GET_ALL_USERS = (pageSize, page) => `users?per_page=${pageSize}&page=${page}${API_KEY_1}`

export const GET_ALL_ISSUE_IN_PROJECT = (idproject, pageSize, page) =>
    `projects/${idproject}/issues?per_page=${pageSize}&page=${page}${API_KEY_1}`

export const GET_ALL_ISSUE_IN_PROJECT_BY_DATE = (idproject, created_after, created_before, pageSize, page) =>
    `projects/${idproject}/issues?created_after=${created_after}&created_before=${created_before}&per_page=${pageSize}&page=${page}${API_KEY_1}`

export const GET_ALL_PARTICIPANTS_BY_ISSUE = (idproject, issueId, pageSize, page) =>
    `projects/${idproject}/issues/${issueId}/participants?per_page=${pageSize}&page=${page}${API_KEY_1}`

export const GET_ALL_NOTES_BY_ISSUE = (idproject, issueId, pageSize, page) =>
    `projects/${idproject}/issues/${issueId}/notes?per_page=${pageSize}&page=${page}${API_KEY_1}`

export const GET_ALL_TIME_STATS_IN_ISSUE = (idproject, issueId) =>
    `projects/${idproject}/issues${issueId}/time_stats${API_KEY_1}`

export const HTTP_API_GITLAB = axios.create({
    baseURL: 'http://10.32.4.160:3080/api/v4/',
    headers: {
        'Content-Type': 'application/json',
    },
})

export const ENDPIONTS = {
    LEAVEOFF_REGISTER_LIST: 'leaveOff/getAllLeaveOff',
    SEARCH_REGISTER_LIST: 'leaveOff/GetByNameStatusDate',
    ADD_NEW_LEAVE_OFF: 'leaveOff/addNewLeaveOff',
}

export const GET_USER_BY_ID = (id) => `Users/getUserById/${id}`
export const ACCEPT_LEAVE_OFF = (id) => `leaveOff/accceptLeaveOff/${id}`
export const NOT_ACCEPT_LEAVE_OFF = (id) => `leaveOff/notAcceptLeaveOff/${id}`
export const GET_LEAVEOFF_BY_STATUS = (value) => `leaveOff/getAllLeaveOffByStatus?statusLO=${value}`
export const GET_LIST_LEAVEOFF_BY_USER = (id) => `leaveOff/getAllLeaveOffOfidUserByStatus/${id}`
export const GET_LEAVE_OFF_BY_ID = (id) => `leaveOff/getLeaveOffById/${id}`
export const UPDATE_LEAVE_OFF = (id) => `leaveOff/editRegisterLeaveOff/${id}`
export const DELETE_LEAVE_OFF = (id) => `leaveOff/deleteLeaveOffById/${id}`

export const HTTP_LOCAL = axios.create({
    baseURL: 'http://localhost:5001/api/',
    headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
    },
})
export const GET_GITLAB_PROJECT = `Project/getProjectOnGitlab`
export const GET_GITLAB_PROJECT_BY_DATE = (value) => `Project/getProjectGitLabByDayAfter?d=${value}`
export const GET_BY_YEAR = `leaveOff/GetAllLeaveOffByYear`
export const GET_LIST_PAID = `paid/`
export const GET_ALL_PROJECT_DATABASE = `Project/getAllProject`
export const GET_ALL_PROJECT_BY_STAFF = (id, page, pageSize) =>
    `Project/getAllProjectByStaff/${id}?pageIndex=${page}&pageSizeEnum=${pageSize}`
export const GET_ALL_USERS_DATABASE = `Users/getAll`
export const GET_ALL_USER_IN_PROJECT_DATABASE = (id) => `Project/UserInProject/${id}`
export const GET_PROJECT_BY_ID = (id) => `Project/getById/${id}`
export const GET_GROUP_BY_ID = (id) => `/Group/getUserByGroup/${id}`

export const HTTP_SINGNALRHUB = 'http://localhost:5001/NotificationHub'

export const GET_USER_NAME_BY_ID = (id) => `Users/GetNameOfUserById/${id}`

export const GET_BLOCKING_WEB = (pageIndex, pageSize) =>
    `BlockingWeb/GetAllBlockWeb?pageIndex=${pageIndex}&pageSizeEnum=${pageSize}`
export const DELETE_BLOCKING_WEB = (id) => `BlockingWeb/DeleteBlockWeb/${id}`

export const ADD_BLOG = 'Posts/createNewPost'
export const EDIT_BLOG = (id) => `Posts/updatePost/${id}`
export const DELETE_BLOG = (id) => `Posts/deletePost/${id}`
export const GET_CATEGORY = 'PostCategory/getAllPostCategoryAsync'
export const GET_CATEGORY_BY_ID = (id) => `PostCategory/getPostCategoryByIdAsync/${id}`

export const GET_FIREBASE_TOKEN = `FirebaseToken`
export const GET_FIREBASE_TOKEN_BY_USERNAME = (name) => `FirebaseToken/${name}`
export const EDIT_FIREBASE_TOKEN_BY_ID = (id) => `FirebaseToken/${id}`
