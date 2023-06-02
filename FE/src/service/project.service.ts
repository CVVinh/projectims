import { HTTP, API_KEY_1, HTTP_API_GITLAB, API_KEY } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class ProjectService {
    public static getProjectById(id: number) {
        return HTTP.get(ApiApplication.PROJECT.GET_PROJECT_BY_ID(id))
    }
    public static getAllLabelsOfProject(idProject: number, pageSize: number, page: number) {
        return HTTP_API_GITLAB.get(
            ApiApplication.PROJECT.GET_ALL_LABELS_OF_PROJECT(idProject, pageSize, page, API_KEY_1),
        )
    }
    public static filterProjectByName(name: string, pageIndex: number, pageSize: number) {
        return HTTP.get(ApiApplication.PROJECT.SEARCH_PROJECT_BY_NAME(name, pageIndex, pageSize))
    }
    public static filterProjectByNameLead(idLead: Number, name: string, pageIndex: number, pageSize: number) {
        return HTTP.get(ApiApplication.PROJECT.SEARCH_PROJECT_BY_NAME_LEAD(idLead, name, pageIndex, pageSize))
    }
    public static filterProjectByNameStaff(idStaff: Number, name: string, pageIndex: number, pageSize: number) {
        return HTTP.get(ApiApplication.PROJECT.SEARCH_PROJECT_BY_NAME_STAFF(idStaff, name, pageIndex, pageSize))
    }
    public static getAllProjectByUser(idUser: Number) {
        return HTTP.get(ApiApplication.PROJECT.GET_ALL_PROJECT_BY_USER(idUser, API_KEY))
    }
}
