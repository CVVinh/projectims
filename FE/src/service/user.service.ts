import { API_KEY_1, HTTP, HTTP_API_GITLAB } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class UserService {
    public static getAllUser(pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.USER.GET_ALL(pageIndex, pageSize))
    }
    public static getUserByCode(value) {
        return HTTP.get(ApiApplication.USER.GET_BY_CODE + value)
    }
    public static getUserInfo() {
        return HTTP.get(ApiApplication.USER.GET_INFO)
    }
    public static updateProfileUser(id: number, object: any) {
        return HTTP.put(ApiApplication.USER.UPDATE_PROFILE(id), object)
    }
    public static getUserById(id: number) {
        return HTTP.get(ApiApplication.USER.GET_USER_BY_ID(id))
    }
    public static getUserByIdUserGitLab(id: number) {
        return HTTP.get(ApiApplication.USER.GET_USER_BY_ID_USER_GITLAB(id))
    }
    public static getAllUserByIdProject(id: number) {
        return HTTP.get(ApiApplication.USER.GET_ALL_USER_BY_IDPROJECT(id))
    }
    public static filterUserByName(name: string, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.USER.SEARCH_USER_BY_NAME(name, pageIndex, pageSize))
    }
    public static filterMemberByName(idProject: Number, name: string, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.USER.SEARCH_MEMBER_BY_NAME(idProject, name, pageIndex, pageSize))
    }
    public static getAllInfoUserGitLab(pageIndex: Number, pageSize: Number) {
        return HTTP_API_GITLAB.get(ApiApplication.USER.GET_ALL_INFO_USER_GITLAB(pageIndex, pageSize, API_KEY_1))
    }
    public static getAllUserInProjectByIdUser(idUser: number, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.USER.GET_ALL_USER_IN_PROJECT_BY_IDUSER(idUser, pageIndex, pageSize))
    }
}
