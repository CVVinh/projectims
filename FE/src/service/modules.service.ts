import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class ModuleService {
    public static filterModulesByNameOrAction(name: string, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.MODULE.SEARCH_MODULES_BY_NAME(name, pageIndex, pageSize))
    }
}
