import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class ActionModuleService {
    public static filterActionModuleByName(name: string, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.ACTIONMODULES.SEARCH_ACTIONMODULES_BY_NAME(name, pageIndex, pageSize))
    }
}
