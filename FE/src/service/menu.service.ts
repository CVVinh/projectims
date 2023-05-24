import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class MenuService {
    public static filterMenuByNameOrAction(name: string, idModule: Number, pageIndex: Number, pageSize: Number) {
        return HTTP.get(ApiApplication.MENU.SEARCH_MENU_BY_NAME_OR_ACTION(name, idModule, pageIndex, pageSize))
    }
}
