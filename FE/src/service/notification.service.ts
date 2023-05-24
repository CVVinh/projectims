import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class NotificationService {
    public static getAllNotification() {
        return HTTP.get(ApiApplication.NOTIFICATION.GET_ALL_NOTIFICATION)
    }
    public static getAllNotificationByIdLead() {
        return HTTP.get(ApiApplication.NOTIFICATION.GET_ALL_NOTIFICATION_BYLEAD)
    }
    public static isWatchNotification(id: number) {
        return HTTP.put(ApiApplication.NOTIFICATION.IS_WATCH_NOTIFICATION(id))
    }
    public static handlerRequireDelete(object: any) {
        return HTTP.post(ApiApplication.NOTIFICATION.REQUIRE_DELETE_APPLICATION, object)
    }
    public static GetAllNotificationOfPm(idPm: Number) {
        return HTTP.get(ApiApplication.NOTIFICATION.GET_ALL_NOTIFICATION_Of_PM(idPm))
    }
    public static GetAllNotificationOfLead(idLead: Number) {
        return HTTP.get(ApiApplication.NOTIFICATION.GET_ALL_NOTIFICATION_Of_LEAD(idLead))
    }
    public static GetAllNotificationTimeSheetDailys() {
        return HTTP.get(ApiApplication.NOTIFICATION.GET_ALL_NOTIFICATION_TIMESHETDAILYS())
    }
    public static CreateNotificationTimeSheetDailys() {
        return HTTP.post(ApiApplication.NOTIFICATION.CREATE_NOTIFICATION_TIMESHETDAILYS())
    }
}
