import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class LeaveOffService {
    public static getAllLeaveOffPage(idUser: Number, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.LEVAEOFF.GET_ALL_LEAVEOFF_BY_ID_USER(idUser, pageIndex, pageSize))
    }
    public static getAllLeaveOffPageList(pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.LEVAEOFF.GET_ALL_LEAVEOFF(pageIndex, pageSize))
    }
    public static getAllLeaveOffPageListByLead(pageIndex: Number, pageSize: Number, IdUser: Number) {
        return HTTP.get(ApiApplication.LEVAEOFF.GET_ALL_LEAVEOFF_BYLEAD(pageIndex, pageSize, IdUser))
    }

    public static getAllLeaveOffPageListByPM(pageIndex: Number, pageSize: Number, IdUser: Number) {
        return HTTP.get(ApiApplication.LEVAEOFF.GET_ALL_LEAVEOFF_BYPM(pageIndex, pageSize, IdUser))
    }
    public static handlerFilterLeaveOff(pageIndex: Number, pageSize: Number, object: any) {
        return HTTP.post(ApiApplication.LEVAEOFF.FILTER_LEAVEOFF_BY_DATE_NAME_STATUS(pageIndex, pageSize), object)
    }
    public static getAllLeaveOffPageListInfo(pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.LEVAEOFF.GET_ALL_LEAVEOFF_INFO(pageIndex, pageSize))
    }
    public static filterLeaveOffByYear(pageIndex: Number, pageSize: Number, object: any) {
        return HTTP.post(ApiApplication.LEVAEOFF.SEARCH_LEAVEOFF_BY_YEAR(pageIndex, pageSize), object)
    }
    public static handlerFilterLeaveOffLead(idLead: Number, pageIndex: Number, pageSize: Number, object: any) {
        return HTTP.post(
            ApiApplication.LEVAEOFF.FILTER_LEAVEOFF_BY_DATE_NAME_STATUS_LEAD(idLead, pageIndex, pageSize),
            object,
        )
    }
    public static handlerFilterLeaveOffPM(idPM: Number, pageIndex: Number, pageSize: Number, object: any) {
        return HTTP.post(
            ApiApplication.LEVAEOFF.FILTER_LEAVEOFF_BY_DATE_NAME_STATUS_PM(idPM, pageIndex, pageSize),
            object,
        )
    }
    public static getInfoUserLeaveOff(idUser: Number, date: Date) {
        return HTTP.get(ApiApplication.LEVAEOFF.GET_USER_INFO_LEAVE_OFF(idUser, date))
    }
}
