import { HTTP_API_GITLAB, HTTP, API_KEY, API_KEY_1, KEY, KEY_PROJECT } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class TaskService {
    public static addTaskOnGitLab(idProject: Number, object: any, api_Key: string) {
        return HTTP_API_GITLAB.post(ApiApplication.TASK.ADD_TASK_ON_GITLAB(idProject, api_Key), object)
    }
    public static addTaskOnAPI(object: any) {
        return HTTP.post(ApiApplication.TASK.ADD_TASK_ON_DB, object)
    }
    public static getTotalPageGitLab(idProject: Number, perPage: Number, page: Number) {
        return HTTP_API_GITLAB.get(ApiApplication.TASK.GET_TOTAL_PAGE_GITLAB(idProject, perPage, page, API_KEY_1))
    }
    public static getALlTaskByIdProject(idProject: Number) {
        return HTTP.get(ApiApplication.TASK.GET_ALL_TASK_BY_IDPROJECT(idProject))
    }
    public static deleteTaskOnAPI(idTask: Number, object: any) {
        return HTTP.put(ApiApplication.TASK.DELETE_TASK_ON_API(idTask), object)
    }
    public static deleteTaskOnGitLab(idProject: Number, iid_Issue: Number) {
        return HTTP_API_GITLAB.delete(ApiApplication.TASK.DELETE_TASK_ON_GITLAB(idProject, iid_Issue, API_KEY))
    }
    public static updateTaskOnApi(idTask: Number, object: any) {
        return HTTP.put(ApiApplication.TASK.UPDATE_TASK_ON_API(idTask), object)
    }
    public static updateTaskOnApiGitLab(idProject: Number, iid_Issue: Number, object: any, api_Key: string) {
        return HTTP_API_GITLAB.put(ApiApplication.TASK.UPDATE_TASK_ON_GITLAB(idProject, iid_Issue, api_Key), object)
    }
    public static mutilateAddTaskOnAPI(idProject: Number, object: any) {
        return HTTP.post(ApiApplication.TASK.MUTILATE_ADD_TASK_ON_API(idProject), object)
    }
    public static updateTaskAddTimeAstimateOnApiGitLab(idProject: Number, iid_Issue: Number, duration: string, api_Key: string) {
        return HTTP_API_GITLAB.post(
            ApiApplication.TASK.UPDATE_TASK_API_GITLAB_TIME_ESTIMATE(idProject, iid_Issue, duration, api_Key),
        )
    }
    public static getAllTaskOnAPIGitLab(idProject: Number, pageSize: Number, page: Number) {
        return HTTP_API_GITLAB.get(ApiApplication.TASK.GET_ALL_TASK_IN_GITLAB(idProject, pageSize, page, API_KEY_1))
    }
    public static getLabbelByName(idProject: Number, name: any) {
        return HTTP_API_GITLAB.get(ApiApplication.TASK.GET_LABLE_BY_NAME(idProject), {
            params: {
                search: name,
                exact_match: true,
                private_token: KEY,
            },
        })
    }
    public static getAllMenberProject(idProject: Number, pageIndex: Number, pageSize: Number) {
        return HTTP_API_GITLAB.get(ApiApplication.TASK.GET_ALL_MEMBER_PROJECT(idProject, API_KEY), {
            params: {
                page: pageIndex,
                per_page: pageSize
              }
        })
    }
    public static getAllTaskWithPage(idProject: Number, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.TASK.GET_ALL_TASK_WITH_PAGE(idProject, pageIndex, pageSize))
    }
    public static getAllTaskWithPageByIdStaff(idProject: Number, idStaff: Number, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.TASK.GET_ALL_TASK_WITH_PAGE_BYIDSTAFF(idProject, idStaff, pageIndex, pageSize))
    }
    public static getProjectGitLab(idProject: Number) {
        return HTTP_API_GITLAB.get(KEY_PROJECT(idProject))
    }
    public static filterTasksByName(idProject: Number, name: string, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.TASK.SEARCH_TASKS_BY_NAME_API(idProject, name, pageIndex, pageSize))
    }
    public static filterTasksOfStaffByName(
        idProject: Number,
        idStaff: Number,
        name: string,
        pageIndex: Number,
        pageSize: Number,
    ) {
        return HTTP.get(
            ApiApplication.TASK.SEARCH_TASKS_OF_STAFF_BY_NAME_API(idProject, idStaff, name, pageIndex, pageSize),
        )
    }
    public static filterTasksByNameOnGitlab(idProject: Number, name: string, pageIndex: Number, pageSize: Number, api_Key: string) {
        return HTTP_API_GITLAB.get(
            ApiApplication.TASK.SEARCH_TASKS_BY_NAME_ON_GITLAB(idProject, name, pageIndex, pageSize, api_Key),
        )
    }
    public static filterTasksByNameOfAssigneeOnGitlab(
        idProject: Number,
        name: string,
        assignee_username: string,
        pageIndex: Number,
        pageSize: Number,
    ) {
        return HTTP_API_GITLAB.get(
            ApiApplication.TASK.SEARCH_TASK_BY_NAME_OF_STAFF_ON_GITLAB(
                idProject,
                name,
                assignee_username,
                pageIndex,
                pageSize,
                API_KEY_1,
            ),
        )
    }
    public static addTimeSpentToWork(idProject: Number, iid_Issue: Number, duration: string, api_Key: string){
        return HTTP_API_GITLAB.post(ApiApplication.TASK.ADD_TIME_SPENT_TO_WORK(idProject, iid_Issue, duration, api_Key))
    }
    public static getAllCommentToWork(idProject: Number, iid_Issue: Number, api_Key: string){
        return HTTP_API_GITLAB.get(ApiApplication.TASK.GET_ALL_COMMENT_TO_WORK(idProject, iid_Issue, api_Key))
    }
    public static addCommentToWork(idProject: Number, iid_Issue: Number, object: any, api_Key: string){
        return HTTP_API_GITLAB.post(ApiApplication.TASK.ADD_COMMENT_TO_WORK(idProject, iid_Issue, api_Key), object)
    }
    public static updateCommentToWork(idProject: Number, iid_Issue: Number, idComment: Number, object: any, api_Key: string){
        return HTTP_API_GITLAB.put(ApiApplication.TASK.UPDATE_COMMENT_TO_WORK(idProject, iid_Issue, idComment, api_Key), object)
    }
    public static deleteCommentToWork(idProject: Number, iid_Issue: Number, idComment: Number, api_Key: string){
        return HTTP_API_GITLAB.delete(ApiApplication.TASK.DELETE_COMMENT_TO_WORK(idProject, iid_Issue, idComment, api_Key))
    }
    public static createUserTokenPrivate(idUser: Number, object: any){
        return HTTP_API_GITLAB.post(ApiApplication.TASK.CREATE_USER_TOKEN_PRIVATE(idUser, API_KEY), object)
    }
    public static getAllTaskByUser(idProject: Number, idUser: Number, pageSize: Number, page: Number){
        return HTTP_API_GITLAB.get(ApiApplication.TASK.GET_ALL_TASK_BY_USER(idProject, idUser, pageSize, page, API_KEY_1))
    }
    public static filterTasksByNameOnGitlabByUser(idProject: Number, idUser: Number, name: string, pageIndex: Number, pageSize: Number, api_Key: string) {
        return HTTP_API_GITLAB.get(ApiApplication.TASK.SEARCH_TASKS_BY_NAME_ON_GITLAB_BY_USER(idProject, idUser, name, pageIndex, pageSize, api_Key),)
    }
    public static getAllTaskOnAPIGitLabByUserAuthor(idProject: Number, idAuthor: Number, pageSize: Number, page: Number) {
        return HTTP_API_GITLAB.get(ApiApplication.TASK.GET_ALL_TASK_IN_GITLAB_BY_USER_AUTHOR(idProject, idAuthor, pageSize, page, API_KEY_1))
    }
    public static addMenberProject(idProject: Number, object: any) {
        return HTTP_API_GITLAB.post(ApiApplication.TASK.ADD_MEMBER_PROJECT(idProject, API_KEY), object)
    }
    public static deleteMenberProject(idProject: Number,idUser: Number) {
        return HTTP_API_GITLAB.delete(ApiApplication.TASK.DELETE_MEMBER_PROJECT(idProject, idUser, API_KEY))
    }
    public static searchUserInProjectOnGitLab(idProject: Number, name: any, pageIndex: Number, pageSize: Number) {
        return HTTP_API_GITLAB.get(ApiApplication.TASK.SEARCH_USER_IN_PROJECT_ON_GITLAB(idProject, API_KEY), {
            params: {
                search: name,
                page: pageIndex,
                per_page: pageSize
              }
        })
    }
}
